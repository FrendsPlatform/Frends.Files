using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Frends.Files.Copy.Helpers;

internal static class FilesHandler
{
    private const string RegexPrefix = "<regex>";

    internal static IEnumerable<string> FindMatchingFiles(string directoryPath, string mask)
    {
        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException(
                $"Directory does not exist or you do not have read access. Tried to access directory '{directoryPath}'");

        if (string.IsNullOrEmpty(mask))
        {
            return [];
        }

        var results = new List<string>();
        string normalizedDir = Path.GetFullPath(directoryPath);

        if (mask.StartsWith(RegexPrefix, StringComparison.OrdinalIgnoreCase))
        {
            string pattern = mask.Substring(RegexPrefix.Length);
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            foreach (string file in Directory.EnumerateFiles(normalizedDir, "*", SearchOption.AllDirectories))
            {
                string relativePath = NormalizePath(Path.GetRelativePath(normalizedDir, file));
                string fileName = Path.GetFileName(file);
                if (regex.IsMatch(relativePath) || regex.IsMatch(fileName)) results.Add(relativePath);
            }

            return results;
        }

        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        matcher.AddInclude(NormalizePath(mask));

        var executeResult = matcher.Execute(
            new DirectoryInfoWrapper(new DirectoryInfo(normalizedDir))
        );

        results.AddRange(executeResult.Files.Select(file => file.Path));

        return results;
    }

    private static string NormalizePath(string path)
    {
        return path.Replace('\\', '/');
    }
}
