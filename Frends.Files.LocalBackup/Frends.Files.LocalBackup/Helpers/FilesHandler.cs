using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Frends.Files.LocalBackup.Helpers;

internal static class FilesHandler
{
    private const string RegexPrefix = "<regex>";
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

    internal static bool FileMatchesMask(string filePath, string mask)
    {
        if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(mask))
        {
            return false;
        }

        string normalizedPath = NormalizePath(filePath);

        if (mask.StartsWith(RegexPrefix, StringComparison.OrdinalIgnoreCase))
        {
            string pattern = mask[RegexPrefix.Length..];
            var regex = new Regex(pattern, RegexOptions.IgnoreCase, RegexTimeout);

            return regex.IsMatch(normalizedPath);
        }

        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        matcher.AddInclude(NormalizePath(mask));

        return matcher.Match(normalizedPath).HasMatches;
    }

    private static string NormalizePath(string path)
    {
        return path.Replace('\\', '/');
    }
}
