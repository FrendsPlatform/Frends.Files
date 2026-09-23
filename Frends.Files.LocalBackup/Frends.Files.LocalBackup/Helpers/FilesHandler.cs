using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Frends.Files.LocalBackup.Helpers;

internal static class FilesHandler
{
    private const string RegexPrefix = "<regex>";
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

    internal static bool FileMatchesMask(string filePath, string mask)
    {
        if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(mask))
        {
            return false;
        }

        var fileName = Path.GetFileName(filePath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        if (mask.StartsWith(RegexPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var pattern = mask[RegexPrefix.Length..];
            var regex = new Regex(pattern, RegexOptions.IgnoreCase, RegexTimeout);

            return regex.IsMatch(fileName);
        }

        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        matcher.AddInclude(mask);

        return matcher.Match(fileName).HasMatches;
    }
}
