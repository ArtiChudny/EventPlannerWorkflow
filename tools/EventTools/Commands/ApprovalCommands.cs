using System.Text.Json.Nodes;

namespace EventTools.Commands;

/// <summary>Human-only commands: approve &lt;run-id&gt; and reject &lt;run-id&gt; "&lt;feedback&gt;".</summary>
internal static class ApprovalCommands
{
    private static string? ResolveRunDir(string runId)
    {
        if (runId.IndexOfAny(['/', '\\']) >= 0 || runId.Contains(".."))
        {
            Console.Error.WriteLine($"Invalid run id '{runId}'.");
            return null;
        }
        string dir = Path.Combine(Common.FindRoot(), "runs", runId);
        if (!Directory.Exists(dir))
        {
            Console.Error.WriteLine($"Run not found: {dir}");
            return null;
        }
        return dir;
    }

    public static int Approve(string[] args)
    {
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Usage: approve <run-id>");
            return 64;
        }
        string? runDir = ResolveRunDir(args[0]);
        if (runDir is null) return 1;

        string plan = Path.Combine(runDir, "event-plan.md");
        string? sha = Common.Sha256File(plan);
        if (sha is null)
        {
            Console.Error.WriteLine($"Cannot approve: {plan} does not exist.");
            return 1;
        }

        string ts = Common.Now();
        var approval = new JsonObject { ["status"] = "approved", ["plan_sha256"] = sha, ["timestamp"] = ts };
        Common.AtomicWrite(Path.Combine(runDir, "approval.json"), approval.ToJsonString(Common.Pretty));

        StateStore.Update(runDir, s =>
        {
            s["status"] = "approved";
            s["current_stage"] = "html";
            s["approval"] = new JsonObject { ["status"] = "approved", ["plan_sha256"] = sha, ["timestamp"] = ts };
        });

        Console.WriteLine($"Approved run {args[0]}.");
        Console.WriteLine($"  plan sha256: {sha}");
        Console.WriteLine("Return to Claude Code and say \"done\". Any later change to event-plan.md invalidates this approval.");
        return 0;
    }

    public static int Reject(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: reject <run-id> \"<feedback>\"");
            return 64;
        }
        string? runDir = ResolveRunDir(args[0]);
        if (runDir is null) return 1;

        string feedback = string.Join(' ', args.Skip(1)).Trim();
        if (feedback.Length == 0)
        {
            Console.Error.WriteLine("Feedback must not be empty.");
            return 64;
        }

        string ts = Common.Now();
        Common.AtomicWrite(Path.Combine(runDir, "revision-feedback.md"),
            $"# Revision Feedback\n\n_Submitted: {ts}_\n\n{feedback}\n");

        string approvalPath = Path.Combine(runDir, "approval.json");
        if (File.Exists(approvalPath)) File.Delete(approvalPath);

        StateStore.Update(runDir, s =>
        {
            s["status"] = "revision_requested";
            s["current_stage"] = "revision";
            s["revision_count"] = (s["revision_count"]?.GetValue<int>() ?? 0) + 1;
            s["approval"] = new JsonObject { ["status"] = "rejected", ["plan_sha256"] = null, ["timestamp"] = ts };
        });

        Console.WriteLine($"Rejected run {args[0]}; feedback saved to revision-feedback.md.");
        Console.WriteLine("Return to Claude Code and say \"done\" so the revision can start.");
        return 0;
    }
}
