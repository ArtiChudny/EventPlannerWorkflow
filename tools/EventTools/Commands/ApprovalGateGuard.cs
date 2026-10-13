using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace EventTools.Commands;

/// <summary>
/// PreToolUse guard. Blocks creating/modifying event-guide.html unless approval.json matches the current
/// SHA-256 of event-plan.md and status == "approved". Blocks any agent write to approval.json and any
/// agent invocation of the approve/reject commands.
/// </summary>
internal static class ApprovalGateGuard
{
    private const string UserOnly =
        "Blocked: approval.json is written only by the human via the approve command " +
        "(scripts/approve.cmd <run-id>). Agents must never create or edit it.";

    private static readonly Regex WriteIndicator = new(
        @"(>|\btee\b|set-content|out-file|add-content|new-item|\becho\b|\bcp\b|\bcopy\b|\bmv\b|\bmove\b|\bsed\b|\bdel\b|\berase\b|\brm\b|remove-item|writeall|\bsc\b|\bni\b|\bwget\b|\bcurl\b|\bpython\d*\b|\bnode\b|\bdotnet\b|\bperl\b|\bsave\b)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex ApproveInvocation = new(
        @"(eventtools(\.dll)?[""']?\s+(approve|reject)\b|scripts/(approve|reject)(\.cmd)?\b)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex HtmlRunPath = new(
        @"runs/(?<id>[^/\s""'`;|&]+)/event-guide\.html", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static int Run()
    {
        Common.HookInput? h = Common.ReadHook();
        if (h is null) return 0;

        if (Common.IsWriteTool(h.Tool))
        {
            string? file = h.Str("file_path");
            if (string.IsNullOrWhiteSpace(file)) return 0;
            return CheckPath(Common.Norm(file, Common.BaseDir(h)));
        }

        if (h.Tool == "Bash" || h.Tool == "PowerShell")
        {
            string? cmd = h.Str("command");
            return string.IsNullOrWhiteSpace(cmd) ? 0 : CheckCommand(cmd);
        }

        return 0;
    }

    private static int CheckPath(string norm)
    {
        string name = Path.GetFileName(norm);
        if (Common.Eq(name, "approval.json")) return Common.Block(UserOnly);

        if (Common.Eq(name, "event-guide.html"))
        {
            var run = Common.ParseRunPath(norm);
            if (run is null || run.Value.Remainder.Contains('/'))
                return Common.Block("Blocked: event-guide.html may only be written to runs/<run-id>/event-guide.html.");
            string? problem = Verify(run.Value.RunDir);
            if (problem is not null) return Common.Block("Blocked: " + problem);
        }
        return 0;
    }

    private static int CheckCommand(string rawCmd)
    {
        string cmd = rawCmd.Replace('\\', '/');

        if (ApproveInvocation.IsMatch(cmd))
            return Common.Block("Blocked: approve/reject are human-only commands. Ask the user to run them in their own terminal (or with the ! prefix).");

        bool writes = WriteIndicator.IsMatch(cmd);

        if (cmd.Contains("approval.json", StringComparison.OrdinalIgnoreCase) && writes)
            return Common.Block(UserOnly);

        if (cmd.Contains("event-guide.html", StringComparison.OrdinalIgnoreCase) && writes)
        {
            MatchCollection ms = HtmlRunPath.Matches(cmd);
            if (ms.Count == 0)
                return Common.Block("Blocked: shell commands touching event-guide.html must use the explicit path runs/<run-id>/event-guide.html (and require a valid approval).");
            string root = Common.FindRoot();
            foreach (Match m in ms)
            {
                string runDir = Common.Norm(Path.Combine(root, "runs", m.Groups["id"].Value));
                string? problem = Verify(runDir);
                if (problem is not null) return Common.Block("Blocked: " + problem);
            }
        }
        return 0;
    }

    /// <summary>Returns null when the HTML may be written, otherwise the reason.</summary>
    private static string? Verify(string runDir)
    {
        string plan = Path.Combine(runDir, "event-plan.md");
        if (!File.Exists(plan))
            return "event-plan.md does not exist yet; the HTML guide cannot be created before the plan.";

        string approvalPath = Path.Combine(runDir, "approval.json");
        if (!File.Exists(approvalPath))
            return "no approval.json found. The plan has not been approved; ask the user to run scripts/approve.cmd <run-id>.";

        string? status, stored;
        try
        {
            JsonObject? a = JsonNode.Parse(File.ReadAllText(approvalPath)) as JsonObject;
            status = a?["status"]?.GetValue<string>();
            stored = a?["plan_sha256"]?.GetValue<string>();
        }
        catch (Exception ex) when (ex is System.Text.Json.JsonException or InvalidOperationException or IOException)
        {
            return $"approval.json is unreadable or malformed ({ex.Message}).";
        }

        if (!Common.Eq(status, "approved"))
            return $"approval status is '{status ?? "missing"}', not 'approved'.";

        string? current = Common.Sha256File(plan);
        if (current is null || !Common.Eq(current, stored))
            return "event-plan.md changed after approval (hash mismatch). Approval is invalidated; the plan must be re-approved.";

        return null;
    }
}
