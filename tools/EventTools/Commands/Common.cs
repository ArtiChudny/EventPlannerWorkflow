using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EventTools.Commands;

internal static class Common
{
    public static readonly UTF8Encoding Utf8NoBom = new(false);

    public static readonly JsonSerializerOptions Pretty = new() { WriteIndented = true };

    /// <summary>Full path with forward slashes (comparison form).</summary>
    public static string Norm(string path, string? basePath = null)
    {
        string full = string.IsNullOrEmpty(basePath)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(path, basePath);
        return full.Replace('\\', '/');
    }

    public static bool Eq(string? a, string? b) =>
        string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    public static string Sha256Bytes(byte[] data) =>
        Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant();

    public static string? Sha256File(string path)
    {
        try
        {
            return File.Exists(path) ? Sha256Bytes(File.ReadAllBytes(path)) : null;
        }
        catch (IOException)
        {
            return null;
        }
    }

    public static void AtomicWrite(string path, string content)
    {
        string? dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        string temp = $"{path}.{Guid.NewGuid():N}.tmp";
        File.WriteAllText(temp, content, Utf8NoBom);
        File.Move(temp, path, overwrite: true);
    }

    public static string Now() => DateTimeOffset.Now.ToString("yyyy-MM-dd'T'HH:mm:sszzz");

    /// <summary>Walks up from the start folder to the folder containing ".claude" (repo root).</summary>
    public static string FindRoot(string? start = null)
    {
        string? env = Environment.GetEnvironmentVariable("CLAUDE_PROJECT_DIR");
        string cur = Path.GetFullPath(!string.IsNullOrWhiteSpace(start) ? start
            : !string.IsNullOrWhiteSpace(env) ? env : Directory.GetCurrentDirectory());
        for (string? d = cur; !string.IsNullOrEmpty(d); d = Path.GetDirectoryName(d))
        {
            if (Directory.Exists(Path.Combine(d, ".claude"))) return d;
        }
        return cur;
    }

    /// <summary>Splits a normalized path ".../runs/&lt;id&gt;/&lt;rest&gt;" into run dir, run id and remainder.</summary>
    public static (string RunDir, string RunId, string Remainder)? ParseRunPath(string normPath)
    {
        int idx = normPath.LastIndexOf("/runs/", StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return null;
        string after = normPath[(idx + 6)..];
        int slash = after.IndexOf('/');
        if (slash <= 0 || slash == after.Length - 1) return null;
        string id = after[..slash];
        if (id.StartsWith('_')) return null; // runs/_sample-inputs etc.
        return (normPath[..(idx + 6 + slash)], id, after[(slash + 1)..]);
    }

    public sealed class HookInput
    {
        public string Tool = "";
        public JsonNode? ToolInput;
        public string Cwd = "";

        public string? Str(string key) => ToolInput?[key]?.GetValue<string>();
    }

    /// <summary>Reads the hook JSON from stdin. Returns null (with a stderr warning) when missing or malformed.</summary>
    public static HookInput? ReadHook()
    {
        string raw;
        try
        {
            using Stream stdin = Console.OpenStandardInput();
            using var ms = new MemoryStream();
            stdin.CopyTo(ms);
            raw = Encoding.UTF8.GetString(ms.ToArray()).TrimStart('﻿');
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"EventTools: cannot read stdin: {ex.Message}");
            return null;
        }
        if (string.IsNullOrWhiteSpace(raw))
        {
            Console.Error.WriteLine("EventTools: empty hook input; allowing.");
            return null;
        }
        try
        {
            if (JsonNode.Parse(raw) is not JsonObject obj) throw new JsonException("root is not an object");
            return new HookInput
            {
                Tool = obj["tool_name"]?.GetValue<string>() ?? "",
                ToolInput = obj["tool_input"],
                Cwd = obj["cwd"]?.GetValue<string>() ?? ""
            };
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException or FormatException)
        {
            Console.Error.WriteLine($"EventTools: malformed hook JSON ({ex.Message}); allowing.");
            return null;
        }
    }

    public static bool IsWriteTool(string tool) => tool is "Write" or "Edit" or "MultiEdit";

    public static string BaseDir(HookInput h) =>
        !string.IsNullOrWhiteSpace(h.Cwd) && Directory.Exists(h.Cwd) ? h.Cwd : FindRoot();

    public static int Block(string reason)
    {
        Console.Error.WriteLine(reason);
        return 2;
    }
}
