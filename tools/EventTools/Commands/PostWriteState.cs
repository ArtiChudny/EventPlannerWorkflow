using System.Text.Json.Nodes;

namespace EventTools.Commands;

/// <summary>
/// PostToolUse hook. When a file under runs/&lt;id&gt;/artifacts/ (or the run's event-plan.md) is written,
/// records artifact name, owner agent, status, SHA-256 and timestamp in workflow-state.json. Marks completed
/// downstream agents "stale" when an upstream artifact's content changed. Preserves all other fields.
/// </summary>
internal static class PostWriteState
{
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
        bool isArtifact = rest.StartsWith("artifacts/", StringComparison.OrdinalIgnoreCase) && rest.Count(c => c == '/') == 1;
        bool isPlan = Common.Eq(rest, "event-plan.md");
        if (!isArtifact && !isPlan) return 0;

        string? sha = Common.Sha256File(norm);
        if (sha is null)
        {
            Console.Error.WriteLine($"post-write-state: {norm} not found after write; state not updated.");
            return 0;
        }

        string artifactName = Path.GetFileName(norm);
        string? owner = isPlan ? "event-plan-builder" : StateStore.OwnerOf(artifactName);

        StateStore.Update(runDir, state =>
        {
            var agents = state["agents"] as JsonObject ?? (JsonObject)(state["agents"] = new JsonObject())!;
            var artifacts = state["artifacts"] as JsonObject ?? (JsonObject)(state["artifacts"] = new JsonObject())!;

            string? previous = artifacts[artifactName]?["sha256"]?.GetValue<string>();
            string now = Common.Now();

            artifacts[artifactName] = new JsonObject
            {
                ["owner"] = owner ?? "unknown",
                ["status"] = "completed",
                ["sha256"] = sha,
                ["timestamp"] = now
            };

            if (owner is not null)
            {
                var entry = agents[owner] as JsonObject ?? (JsonObject)(agents[owner] = new JsonObject())!;
                entry["status"] = "completed";
                entry["sha256"] = sha;
                entry["updated"] = now;
                entry["input_hashes"] = SnapshotInputs(state, runDir, owner);
            }

            if (isPlan)
            {
                // A changed plan invalidates any earlier approval (the guard re-checks the hash anyway).
                var approval = state["approval"] as JsonObject ?? (JsonObject)(state["approval"] = new JsonObject())!;
                string? approved = approval["plan_sha256"]?.GetValue<string>();
                if (approved is not null && !Common.Eq(approved, sha) && approval["status"]?.GetValue<string>() == "approved")
                    approval["status"] = "invalidated";
            }

            if (owner is not null && previous is not null && !Common.Eq(previous, sha))
                MarkDownstreamStale(state, agents, owner);
        });
        return 0;
    }

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

    private static void MarkDownstreamStale(JsonObject state, JsonObject agents, string changed)
    {
        var deps = state["dependencies"] as JsonObject;
        if (deps is null) return;

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

        foreach (string name in stale)
        {
            if (agents[name] is JsonObject a && a["status"]?.GetValue<string>() == "completed")
                a["status"] = "stale";
        }
    }
}
