using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Frends.Files.Move.Definitions;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Frends.Files.Move;

internal static class Helpers
{
    internal static Tuple<string, string> GetDomainAndUsername(string username)
    {
        var domainAndUserName = username.Split('\\');
        return domainAndUserName.Length != 2
            ? throw new ArgumentException($@"UserName field must be of format domain\username was: {username}")
            : new Tuple<string, string>(domainAndUserName[0], domainAndUserName[1]);
    }

    internal static PatternMatchingResult FindMatchingFiles(string directoryPath, string pattern)
    {
        // This will return false if the path does not exist, or you do not have read permissions.
        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException(
                $"Directory does not exist or you do not have read access. Tried to access directory '{directoryPath}'");

        if (pattern.StartsWith("<regex>"))
        {
            var regexPattern = pattern[7..];

            var matchingFiles = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories)
                .Where(file => Regex.IsMatch(Path.GetFileName(file), regexPattern))
                .Select(file => new FilePatternMatch(Path.GetFileName(file), Path.GetFileName(file)))
                .ToList();

            return new PatternMatchingResult(matchingFiles);
        }

        var matcher = new Matcher();
        matcher.AddInclude(pattern);
        var results = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directoryPath)));
        return results;
    }

    internal static async Task CopyFileImpersonated(string sourceFilePath, string targetFilePath, Connection connection,
        CancellationToken cancellationToken)
    {
        await using var sourceStream = ImpersonatedAction.Execute(() => File.Open(sourceFilePath, FileMode.Open),
            connection,
            ImpersonatedPart.Source);
        await using var targetStream = ImpersonatedAction.Execute(() => File.Open(targetFilePath, FileMode.CreateNew),
            connection, ImpersonatedPart.Target);
        await sourceStream.CopyToAsync(targetStream, 81920, cancellationToken).ConfigureAwait(false);
    }

    internal static Dictionary<string, string> GetFileTransferEntries(IEnumerable<FilePatternMatch> fileMatches,
        string sourceDirectory, string targetDirectory, bool preserveDirectoryStructure)
    {
        return fileMatches
            .ToDictionary(
                f => Path.Combine(sourceDirectory, f.Path),
                f => preserveDirectoryStructure
                    ? Path.GetFullPath(Path.Combine(targetDirectory, f.Path))
                    : Path.GetFullPath(Path.Combine(targetDirectory, Path.GetFileName(f.Path))));
    }

    internal static void AssertNoTargetFileConflicts(string[] filePaths)
    {
        // check the target file list to see there should not be conflicts before doing anything
        var duplicateTargetPaths = GetDuplicateValues(filePaths);
        if (duplicateTargetPaths.Count != 0)
            throw new IOException(
                $"Multiple files written to {string.Join(", ", duplicateTargetPaths)}. The files would get overwritten. No files moved.");

        foreach (var targetFilePath in filePaths)
            if (File.Exists(targetFilePath))
                throw new IOException($"File '{targetFilePath}' already exists. No files moved.");
    }

    internal static void DeleteExistingFiles(IEnumerable<string> files)
    {
        foreach (var file in files)
            if (File.Exists(file))
                File.Delete(file);
    }

    internal static string GetNonConflictingTargetFilePath(string sourceFilePath, string targetFilePath)
    {
        var count = 1;
        while (File.Exists(targetFilePath))
        {
            var tempFileName = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}({count++})";
            targetFilePath = Path.Combine(Path.GetDirectoryName(targetFilePath) ?? string.Empty,
                path2: tempFileName + Path.GetExtension(sourceFilePath));
        }

        return targetFilePath;
    }

    private static List<string> GetDuplicateValues(IEnumerable<string> values)
    {
        return values.GroupBy(v => v).Where(x => x.Count() > 1).Select(k => k.Key).ToList();
    }
}