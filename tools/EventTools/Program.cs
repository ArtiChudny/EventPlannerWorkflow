using EventTools.Commands;

namespace EventTools;

internal static class Program
{
    private const string Usage =
        "Usage: EventTools <approval-gate-guard|no-leak-guard|post-write-state|check-artifact|approve|reject|new-run> [args]";

    private static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine(Usage);
            return 64;
        }

        string[] rest = args[1..];
        try
        {
            return args[0].ToLowerInvariant() switch
            {
                "approval-gate-guard" => ApprovalGateGuard.Run(),
                "no-leak-guard" => NoLeakGuard.Run(),
                "post-write-state" => PostWriteState.Run(),
                "check-artifact" => CheckArtifact.Run(rest),
                "approve" => ApprovalCommands.Approve(rest),
                "reject" => ApprovalCommands.Reject(rest),
                "new-run" => NewRun.Run(rest),
                _ => Fail($"Unknown subcommand '{args[0]}'. {Usage}")
            };
        }
        catch (Exception ex)
        {
            // Never write diagnostics to stdout; hooks treat stdout as output.
            Console.Error.WriteLine($"EventTools {args[0]} failed unexpectedly: {ex.Message}");
            // Guards fail open on internal errors (exit 1 = non-blocking); only exit 2 blocks.
            return 1;
        }
    }

    private static int Fail(string message)
    {
        Console.Error.WriteLine(message);
        return 64;
    }
}
