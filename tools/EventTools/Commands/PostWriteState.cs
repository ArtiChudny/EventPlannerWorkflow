using System.Text.Json.Nodes;

namespace EventTools.Commands;

/// <summary>
/// PostToolUse hook. After any write inside runs/&lt;id&gt;/ (an artifact, event-plan.md, event-guide.html or
/// workflow-state.json itself) it reconciles workflow-state.json with the files on disk: every output that exists
/// gets its owner, status, SHA-256 and timestamp recorded, so writes the hook did not see and rewrites of the
/// state file by the coordinator are repaired. A changed artifact that was the file just written marks its
/// completed downstream agents "stale". Other fields, retry counters and the decision log are preserved.
/// </summary>
internal static class PostWriteState
{
    private const string StateFile = "workflow-state.json";

    public static int Run()
    {
        Common.HookInput? h = Common.ReadHook();
        if (h is null || !Common.IsWriteTool(h.Tool)) return 0;

        string? file = h.Str("file_path");
        if (string.IsNullOrWhiteSpace(file)) return 0;

        string norm = Common.Norm(file, Common.BaseDir(h));
        var run = Common.ParseRunPath(norm);
        if (run is null) return 0;

        (string runDir, _, string rest) = run.Value;

        // The state file itself was rewritten: reconcile with no file counted as changed.
        if (Common.Eq(rest, StateFile))
        {
            Reconcile(runDir, null);
            return 0;
        }

        bool isOutput = rest.StartsWith("artifacts/", StringComparison.OrdinalIgnoreCase) && rest.Count(c => c == '/') == 1
            || rest.IndexOf('/') < 0;
        if (!isOutput) return 0;

        Reconcile(runDir, Path.GetFileName(rest));
        return 0;
    }

    /// <summary>Every output the workflow knows about: (owner agent, path relative to the run directory).</summary>
    private static IEnumerable<(string Agent, string Rel)> KnownOutputs() =>
        StateStore.Agents.Select(a => (a.Agent, a.Artifact.StartsWith("../")
            ? a.Artifact[3..]
            : "artifacts/" + a.Artifact));

    private static void Reconcile(string runDir, string? changedKey)
    {
        StateStore.Update(runDir, state =>
        {
            var agents = EnsureObject(state, "agents");
            var artifacts = EnsureObject(state, "artifacts");
            string now = Common.Now();

            // Pass 1: record every output that exists on disk.
            var current = new List<(string Agent, string Key, string Sha, string? Previous)>();
            foreach (var (agent, rel) in KnownOutputs())
            {
                string? sha = Common.Sha256File(Path.Combine(runDir, rel));
                if (sha is null) continue;

                string key = Path.GetFileName(rel);
                string? previous = artifacts[key]?["sha256"]?.GetValue<string>();
                bool differs = previous is null || !Common.Eq(previous, sha);
                if (differs || artifacts[key]?["status"]?.GetValue<string>() != "completed")
                {
                    artifacts[key] = new JsonObject
                    {
                        ["owner"] = agent,
                        ["status"] = "completed",
                        ["sha256"] = sha,
                        ["timestamp"] = differs ? now : artifacts[key]?["timestamp"]?.GetValue<string>() ?? now
                    };
                }
                current.Add((agent, key, sha, previous));
            }

            // Unknown files written under artifacts/ are still recorded, with no owner.
            if (changedKey is not null && !StateStore.Agents.Any(a => Common.Eq(Path.GetFileName(a.Artifact), changedKey))
                && Common.Sha256File(Path.Combine(runDir, "artifacts", changedKey)) is string extraSha)
            {
                artifacts[changedKey] = new JsonObject
                {
                    ["owner"] = "unknown",
                    ["status"] = "completed",
                    ["sha256"] = extraSha,
                    ["timestamp"] = now
                };
            }

            // Pass 2: update the owning agents (input hashes read the artifacts recorded in pass 1).
            foreach (var (agent, key, sha, previous) in current)
            {
                if (agents[agent] is not JsonObject entry)
                {
                    entry = new JsonObject();
                    agents[agent] = entry;
                }

                string status = entry["status"]?.GetValue<string>() ?? "pending";
                bool written = Common.Eq(key, changedKey);

                // A "stale" agent stays stale until it is re-run (the file it wrote is the proof).
                if (written || status is "pending" or "running" or "failed")
                    entry["status"] = "completed";

                if (entry["sha256"]?.GetValue<string>() != sha)
                {
                    entry["sha256"] = sha;
                    entry["updated"] = now;
                    entry["input_hashes"] = SnapshotInputs(state, runDir, agent);
                }

                // Only a recorded hash that changed, in the file just written, invalidates downstream work.
                // Recording a file the state had never seen does not: its consumers already ran after it.
                if (written && previous is not null && !Common.Eq(previous, sha))
                    MarkDownstreamStale(state, agents, agent);
            }

            // Approval is invalid once the plan no longer matches the hash that was approved.
            string planSha = Common.Sha256File(Path.Combine(runDir, "event-plan.md")) ?? "";
            if (planSha.Length > 0 && state["approval"] is JsonObject approval
                && approval["status"]?.GetValue<string>() == "approved"
                && !Common.Eq(approval["plan_sha256"]?.GetValue<string>(), planSha))
            {
                approval["status"] = "invalidated";
            }
        });
    }

    private static JsonObject EnsureObject(JsonObject parent, string key) =>
        parent[key] as JsonObject ?? (JsonObject)(parent[key] = new JsonObject())!;

    private static JsonObject SnapshotInputs(JsonObject state, string runDir, string owner)
    {
        var inputs = new JsonObject();
        var artifacts = state["artifacts"] as JsonObject;
        if (owner == "requirements-formalizer")
        {
            string? input = Common.Sha256File(Path.Combine(runDir, "input.md"));
            if (input is not null) inputs["input.md"] = input;
        }
        if (state["dependencies"]?[owner] is JsonArray ups)
        {
            foreach (JsonNode? u in ups)
            {
                string? upstream = u?.GetValue<string>();
                if (upstream is null) continue;
                var spec = StateStore.Agents.FirstOrDefault(a => a.Agent == upstream);
                if (spec.Artifact is null) continue;
                string art = Path.GetFileName(spec.Artifact);
                string? hash = artifacts?[art]?["sha256"]?.GetValue<string>();
                if (hash is not null) inputs[art] = hash;
            }
        }
        return inputs;
    }

    /// <summary>Marks every completed transitive downstream agent of <paramref name="changed"/> as stale.</summary>
    private static bool MarkDownstreamStale(JsonObject state, JsonObject agents, string changed)
    {
        var deps = state["dependencies"] as JsonObject;
        if (deps is null) return false;

        var stale = new HashSet<string>();
        var queue = new Queue<string>();
        queue.Enqueue(changed);
        while (queue.Count > 0)
        {
            string cur = queue.Dequeue();
            foreach (var (name, ups) in deps)
            {
                if (ups is not JsonArray arr || stale.Contains(name)) continue;
                if (arr.Any(x => x?.GetValue<string>() == cur))
                {
                    stale.Add(name);
                    queue.Enqueue(name);
                }
            }
        }

        bool any = false;
        foreach (string name in stale)
        {
            if (agents[name] is JsonObject a && a["status"]?.GetValue<string>() == "completed")
            {
                a["status"] = "stale";
                any = true;
            }
        }
        return any;
    }
}
