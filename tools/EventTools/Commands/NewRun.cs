using System.Text;
using System.Text.RegularExpressions;

namespace EventTools.Commands;

/// <summary>
/// new-run &lt;slug&gt; [--input-file &lt;path&gt;]
/// Creates runs/YYYYMMDD-HHMM-&lt;slug&gt;/ with artifacts/, input.md and the initial workflow-state.json.
/// Prints only the run id on stdout.
/// </summary>
internal static class NewRun
{
    public static int Run(string[] args)
    {
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Usage: new-run <slug> [--input-file <path>]");
            return 64;
        }

        string slug = Slugify(args[0]);
        string? inputFile = null;
        for (int i = 1; i < args.Length - 1; i++)
            if (args[i] == "--input-file") inputFile = args[i + 1];

        string root = Common.FindRoot();
        string runsDir = Path.Combine(root, "runs");
        Directory.CreateDirectory(runsDir);

        string baseId = $"{DateTime.Now:yyyyMMdd-HHmm}-{slug}";
        string id = baseId;
        for (int n = 2; Directory.Exists(Path.Combine(runsDir, id)); n++) id = $"{baseId}-{n}";

        string runDir = Path.Combine(runsDir, id);
        Directory.CreateDirectory(Path.Combine(runDir, "artifacts"));

        string input = "_Pending: the coordinator writes the original user request here._\n";
        if (inputFile is not null)
        {
            if (!File.Exists(inputFile))
            {
                Console.Error.WriteLine($"Input file not found: {inputFile}");
                return 64;
            }
            input = File.ReadAllText(inputFile, Encoding.UTF8);
        }
        Common.AtomicWrite(Path.Combine(runDir, "input.md"), input);
        StateStore.Update(runDir, _ => { });
        Console.WriteLine(id);
        return 0;
    }

    private static string Slugify(string s)
    {
        string t = Regex.Replace(s.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
        if (t.Length > 30) t = t[..30].Trim('-');
        return t.Length == 0 ? "event" : t;
    }
}
