using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EventTools.Commands;

/// <summary>Reads/updates workflow-state.json under a cross-process lock, writing atomically.</summary>
internal static class StateStore
{
    public static readonly string[] Gates = ["G1", "G2", "G3", "G4", "G5", "G6", "G7", "G8", "G9"];

    /// <summary>Agent, its artifact (relative to artifacts/ unless it starts with "../"), and upstream agents.</summary>
    public static readonly (string Agent, string Artifact, string[] Upstream)[] Agents =
    [
        ("requirements-formalizer", "requirements.md", []),
        ("venue-planner", "venue.md", ["requirements-formalizer"]),
        ("weather-outlook", "weather.md", ["requirements-formalizer"]),
        ("catering-planner", "catering.md", ["requirements-formalizer", "venue-planner", "weather-outlook"]),
        ("program-planner", "program.md", ["requirements-formalizer", "venue-planner", "weather-outlook"]),
        ("decor-av-planner", "decor-av.md", ["requirements-formalizer", "venue-planner", "weather-outlook"]),
        ("logistics-planner", "logistics.md", ["requirements-formalizer", "venue-planner", "weather-outlook", "catering-planner", "program-planner", "decor-av-planner"]),
        ("budget-aggregator", "budget.md", ["requirements-formalizer", "venue-planner", "catering-planner", "program-planner", "decor-av-planner", "logistics-planner"]),
        ("validator", "validation-report.md", ["requirements-formalizer", "venue-planner", "weather-outlook", "catering-planner", "program-planner", "decor-av-planner", "logistics-planner", "budget-aggregator"]),
        ("event-plan-builder", "../event-plan.md", ["requirements-formalizer", "venue-planner", "weather-outlook", "catering-planner", "program-planner", "decor-av-planner", "logistics-planner", "budget-aggregator", "validator"]),
        ("html-builder", "../event-guide.html", ["event-plan-builder"])
    ];

    public static string StatePath(string runDir) => Path.Combine(runDir, "workflow-state.json");

    public static JsonObject NewState(string runId)
    {
        var agents = new JsonObject();
        var deps = new JsonObject();
        foreach (var (agent, artifact, upstream) in Agents)
        {
            agents[agent] = new JsonObject
            {
                ["status"] = "pending",
                ["artifact"] = artifact.StartsWith("../") ? artifact[3..] : "artifacts/" + artifact,
                ["sha256"] = null,
                ["input_hashes"] = new JsonObject(),
                ["updated"] = null
            };
            var arr = new JsonArray();
            foreach (string u in upstream) arr.Add(u);
            deps[agent] = arr;
        }
        var retries = new JsonObject();
        foreach (string g in Gates) retries[g] = 0;
        return new JsonObject
        {
            ["run_id"] = runId,
            ["created"] = Common.Now(),
            ["updated"] = Common.Now(),
            ["status"] = "initialized",
            ["current_stage"] = "intake",
            ["revision_count"] = 0,
            ["requirements_confirmed"] = false,
            ["agents"] = agents,
            ["artifacts"] = new JsonObject(),
            ["dependencies"] = deps,
            ["gate_retries"] = retries,
            ["approval"] = new JsonObject { ["status"] = "none", ["plan_sha256"] = null },
            ["decisions"] = new JsonArray()
        };
    }

    public static JsonObject Load(string runDir)
    {
        string path = StatePath(runDir);
        if (File.Exists(path))
        {
            try
            {
                if (JsonNode.Parse(File.ReadAllText(path)) is JsonObject o) return o;
            }
            catch (JsonException ex)
            {
                // Keep the broken file for inspection rather than silently destroying it.
                string bak = path + ".corrupt-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                File.Copy(path, bak, true);
                Console.Error.WriteLine($"EventTools: workflow-state.json was malformed ({ex.Message}); backed up to {bak}.");
            }
        }
        return NewState(Path.GetFileName(runDir.TrimEnd('/', '\\')));
    }

    public static void Update(string runDir, Action<JsonObject> mutate)
    {
        string key = "EventTools_" + Common.Sha256Bytes(Encoding.UTF8.GetBytes(
            Common.Norm(runDir).ToLowerInvariant()))[..24];
        using var mutex = new Mutex(false, key);
        bool got = false;
        try
        {
            try { got = mutex.WaitOne(TimeSpan.FromSeconds(15)); }
            catch (AbandonedMutexException) { got = true; }
            if (!got) Console.Error.WriteLine("EventTools: state lock timeout; proceeding without lock.");

            JsonObject state = Load(runDir);
            mutate(state);
            state["updated"] = Common.Now();
            Common.AtomicWrite(StatePath(runDir), state.ToJsonString(Common.Pretty));
        }
        finally
        {
            if (got) mutex.ReleaseMutex();
        }
    }

    public static string? OwnerOf(string artifactFile) =>
        Agents.FirstOrDefault(a => a.Artifact.Equals(artifactFile, StringComparison.OrdinalIgnoreCase)).Agent;
}
