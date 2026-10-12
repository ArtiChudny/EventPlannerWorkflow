using System.Text.RegularExpressions;

namespace EventTools.Commands;

/// <summary>
/// check-artifact &lt;artifact-path&gt; &lt;template-path&gt;
/// Template format: markdown headings are the required headings (same level and text, in order; text containing
/// "&lt;" is matched by its prefix). HTML-comment directives in the template add citation rules:
///   &lt;!-- require-url: Heading --&gt;        the section contains at least one http(s) URL
///   &lt;!-- require-url-items: Heading --&gt;  every bullet / table data row in the section has an http(s) URL
///   &lt;!-- require-url-sections: Prefix --&gt; every heading starting with Prefix has a URL in its own body
/// Exit codes: 0 pass, 1 fail, 64 usage / unreadable input.
/// </summary>
internal static class CheckArtifact
{
    private sealed record Heading(int Level, string Text, int Line);

    private static readonly Regex HeadingRx = new(@"^(?<h>#{1,6})\s+(?<t>.+?)\s*#*\s*$", RegexOptions.Compiled);
    private static readonly Regex UrlRx = new(@"https?://[^\s<>\)\]""']+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex DirectiveRx = new(@"^\s*<!--\s*(?<k>require-url(?:-items|-sections)?)\s*:\s*(?<v>.+?)\s*-->\s*$", RegexOptions.Compiled);
    private static readonly Regex TableSep = new(@"^\s*\|?\s*:?-{2,}:?\s*(\|\s*:?-{2,}:?\s*)*\|?\s*$", RegexOptions.Compiled);

    public static int Run(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: EventTools check-artifact <artifact-path> <template-path>");
            return 64;
        }
        string artifactPath = args[0], templatePath = args[1];
        if (!File.Exists(artifactPath))
        {
            Console.WriteLine($"FAIL {artifactPath}");
            Console.WriteLine($"  - artifact file not found: {artifactPath}");
            return 1;
        }
        if (!File.Exists(templatePath))
        {
            Console.Error.WriteLine($"Template not found: {templatePath}");
            return 64;
        }

        string[] artLines = File.ReadAllLines(artifactPath);
        string[] tplLines = File.ReadAllLines(templatePath);
        List<Heading> art = ParseHeadings(artLines);
        List<Heading> tpl = ParseHeadings(tplLines);

        var problems = new List<string>();

        // 1. Required headings, in order.
        int cursor = -1;
        foreach (Heading t in tpl)
        {
            int found = art.FindIndex(cursor + 1, a => Matches(t, a));
            if (found >= 0) { cursor = found; continue; }
            bool earlier = art.Any(a => Matches(t, a));
            problems.Add(earlier
                ? $"heading out of order: '{new string('#', t.Level)} {t.Text}'"
                : $"missing heading: '{new string('#', t.Level)} {t.Text}'");
        }

        // 2. Unfilled placeholders.
        for (int i = 0; i < artLines.Length; i++)
        {
            if (artLines[i].Contains("{{") || Regex.IsMatch(artLines[i], @"\bTODO\b"))
                problems.Add($"line {i + 1}: unfilled placeholder or TODO");
        }

        // 3. Citation directives.
        foreach (string line in tplLines)
        {
            Match d = DirectiveRx.Match(line);
            if (!d.Success) continue;
            string kind = d.Groups["k"].Value, target = d.Groups["v"].Value;
            switch (kind)
            {
                case "require-url":
                    foreach (var sec in Sections(art, artLines, h => Same(h.Text, target)))
                        if (!sec.Body.Any(l => UrlRx.IsMatch(l)))
                            problems.Add($"section '{sec.Head.Text}' has no http(s) source URL");
                    if (!art.Any(h => Same(h.Text, target)))
                        problems.Add($"cannot check URLs: section '{target}' is missing");
                    break;

                case "require-url-items":
                    bool seen = false;
                    foreach (var sec in Sections(art, artLines, h => Same(h.Text, target)))
                    {
                        seen = true;
                        List<(int Line, string Text)> items = Items(sec.Body, sec.StartLine);
                        if (items.Count == 0)
                            problems.Add($"section '{sec.Head.Text}' lists no recommendations (bullets or table rows)");
                        foreach (var it in items.Where(it => !UrlRx.IsMatch(it.Text)))
                            problems.Add($"line {it.Line}: recommendation without http(s) URL: {Trim(it.Text)}");
                    }
                    if (!seen) problems.Add($"cannot check URLs: section '{target}' is missing");
                    break;

                case "require-url-sections":
                    var hits = Sections(art, artLines, h => h.Text.StartsWith(target, StringComparison.OrdinalIgnoreCase), ownBodyOnly: true).ToList();
                    if (hits.Count == 0) problems.Add($"no section starting with '{target}' found");
                    foreach (var sec in hits.Where(s => !s.Body.Any(l => UrlRx.IsMatch(l))))
                        problems.Add($"section '{sec.Head.Text}' has no http(s) source URL");
                    break;
            }
        }

        if (problems.Count == 0)
        {
            Console.WriteLine($"PASS {artifactPath} ({tpl.Count} required headings)");
            return 0;
        }
        Console.WriteLine($"FAIL {artifactPath}");
        foreach (string p in problems.Distinct()) Console.WriteLine($"  - {p}");
        return 1;
    }

    private static string Trim(string s) => s.Length <= 90 ? s.Trim() : s.Trim()[..90] + "...";

    private static bool Same(string a, string b) => a.Trim().Equals(b.Trim(), StringComparison.OrdinalIgnoreCase);

    private static bool Matches(Heading tpl, Heading actual)
    {
        if (tpl.Level != actual.Level) return false;
        int lt = tpl.Text.IndexOf('<');
        return lt >= 0
            ? actual.Text.StartsWith(tpl.Text[..lt].TrimEnd(), StringComparison.OrdinalIgnoreCase)
            : Same(tpl.Text, actual.Text);
    }

    private static List<Heading> ParseHeadings(string[] lines)
    {
        var list = new List<Heading>();
        bool fence = false;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].TrimStart().StartsWith("```")) { fence = !fence; continue; }
            if (fence) continue;
            Match m = HeadingRx.Match(lines[i]);
            if (m.Success) list.Add(new Heading(m.Groups["h"].Length, m.Groups["t"].Value.Trim(), i));
        }
        return list;
    }

    private sealed record Section(Heading Head, List<string> Body, int StartLine);

    /// <summary>Sections whose heading matches; body runs to the next heading of the same or higher level
    /// (or to the next heading of any level when <paramref name="ownBodyOnly"/>).</summary>
    private static IEnumerable<Section> Sections(List<Heading> headings, string[] lines, Func<Heading, bool> pick, bool ownBodyOnly = false)
    {
        for (int i = 0; i < headings.Count; i++)
        {
            Heading h = headings[i];
            if (!pick(h)) continue;
            int end = lines.Length;
            for (int j = i + 1; j < headings.Count; j++)
            {
                if (ownBodyOnly || headings[j].Level <= h.Level) { end = headings[j].Line; break; }
            }
            yield return new Section(h, lines.Skip(h.Line + 1).Take(end - h.Line - 1).ToList(), h.Line + 2);
        }
    }

    /// <summary>Bullets, numbered items and table data rows (table header and separator rows are skipped).</summary>
    private static List<(int Line, string Text)> Items(List<string> body, int startLine)
    {
        var items = new List<(int, string)>();
        bool inTable = false, headerSkipped = false;
        for (int i = 0; i < body.Count; i++)
        {
            string l = body[i];
            string t = l.TrimStart();
            if (t.StartsWith('|'))
            {
                if (!inTable) { inTable = true; headerSkipped = false; }
                if (TableSep.IsMatch(l)) { headerSkipped = true; continue; }
                if (!headerSkipped) continue; // header row precedes the separator
                items.Add((startLine + i, l));
                continue;
            }
            inTable = false;
            if (Regex.IsMatch(t, @"^([-*+]|\d+\.)\s+\S")) items.Add((startLine + i, l));
        }
        return items;
    }
}
