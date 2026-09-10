using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Frends.Files.LocalBackup.Helpers;

internal static class FilesHandler
{
    private const string RegexPrefix = "<regex>";
    private const RegexOptions DefaultRegexOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(10);

    internal static PatternMatchingResult FindMatchingFiles(string directoryPath, string mask)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException(
                $"Directory does not exist or you do not have read access: '{directoryPath}'");
        }

        var matches = Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories)
            .Where(filePath => FileMatchesMask(Path.GetFileName(filePath), mask))
            .Select(filePath =>
            {
                string relativePath = Path.GetRelativePath(directoryPath, filePath);
                return new FilePatternMatch(relativePath, relativePath);
            })
            .ToList();

        return new PatternMatchingResult(matches);
    }

    internal static bool FileMatchesMask(string filename, string mask)
    {
        if (string.IsNullOrWhiteSpace(filename) || string.IsNullOrWhiteSpace(mask))
            return false;

        if (mask.StartsWith(RegexPrefix, StringComparison.OrdinalIgnoreCase))
        {
            string rawRegex = mask[RegexPrefix.Length..];
            try
            {
                return Regex.IsMatch(
                    filename,
                    rawRegex,
                    DefaultRegexOptions,
                    RegexTimeout);
            }
            catch (Exception ex) when (ex is RegexMatchTimeoutException or ArgumentException)
            {
                return false;
            }
        }

        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        matcher.AddInclude(mask);

        var result = matcher.Match(filename);
        return result.HasMatches;
    }
}
