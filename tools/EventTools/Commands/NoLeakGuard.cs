using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace EventTools.Commands;

/// <summary>
/// PreToolUse guard for user-facing outputs (event-guide.html, event-plan.md): blocks content that leaks
/// internal artifact filenames, agent names or quality-gate IDs.
/// </summary>
internal static class NoLeakGuard
{
    private static readonly string[] Files =
    [
        "requirements.md", "venue.md", "weather.md", "catering.md", "program.md", "decor-av.md",
        "logistics.md", "budget.md", "validation-report.md", "workflow-state.json", "execution-plan.md",
        "approval.json", "revision-feedback.md", "input.md"
    ];

    private static readonly string[] Agents =
    [
        "requirements-formalizer", "venue-planner", "weather-outlook", "catering-planner", "program-planner",
        "decor-av-planner", "logistics-planner", "budget-aggregator", "event-plan-builder", "html-builder",
        "artifact-validator", "event-html-theme-builder"
    ];

    private static readonly Regex Leak = BuildRegex();

    private static Regex BuildRegex()
    {
        string names = string.Join("|", Files.Concat(Agents).Select(Regex.Escape));
        return new Regex($@"(?<![\w-])(?<name>{names})(?![\w-])|(?<![\w])(?<gate>G[1-9])(?![\w])",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    public static int Run()
    {
        Common.HookInput? h = Common.ReadHook();
        if (h is null || !Common.IsWriteTool(h.Tool)) return 0;

        string? file = h.Str("file_path");
        if (string.IsNullOrWhiteSpace(file)) return 0;
        string name = Path.GetFileName(file.Replace('\\', '/'));
        if (!Common.Eq(name, "event-guide.html") && !Common.Eq(name, "event-plan.md")) return 0;

        string text = CollectText(h);
        if (text.Length == 0) return 0;

        var hits = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match m in Leak.Matches(text))
            hits.Add(m.Value);

        if (hits.Count == 0) return 0;
        return Common.Block(
            $"Blocked: {name} is user-facing and must not mention internal workflow details. " +
            $"Remove: {string.Join(", ", hits)}. (Internal filenames, agent names and gate IDs G1..G9 are forbidden; " +
            "describe the content in plain language instead.)");
    }

    private static string CollectText(Common.HookInput h)
    {
        var sb = new StringBuilder();
        void Add(string? s)
        {
            if (!string.IsNullOrEmpty(s)) sb.AppendLine(s);
        }

        Add(h.Str("content"));
        Add(h.Str("new_string"));
        if (h.ToolInput?["edits"] is JsonArray edits)
        {
            foreach (JsonNode? e in edits)
                Add(e?["new_string"]?.GetValue<string>());
        }
        return sb.ToString();
    }
}
