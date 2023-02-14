using Frends.Files.Copy.Definitions;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using SimpleImpersonation;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Files.Copy;
///<summary>
/// Files task.
/// </summary>
public class Files
{
    /// <summary>
    /// Copy files. See: https://github.com/FrendsPlatform/Frends.File#Copy
    /// </summary>
    /// <returns>List [ Object { string SourcePath, string Path } ]</returns>
    public static async Task<IList<FileItem>> Copy(
        [PropertyTab] Input input,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
    {
        return await ExecuteActionAsync(() => ExecuteCopyAsync(input, options, cancellationToken),
            options.UseGivenUserCredentialsForRemoteConnections, options.UserName, options.Password).ConfigureAwait(false);
    }

    private static async Task<TResult> ExecuteActionAsync<TResult>(Func<Task<TResult>> action, bool useGivenCredentials, string username, string password)
    {
        if (!useGivenCredentials)
            return await action().ConfigureAwait(false);

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("UseGivenCredentials feature is only supported on Windows.");

        var (domain, user) = GetDomainAndUsername(username);

        UserCredentials credentials = new UserCredentials(domain, user, password);
        using SafeAccessTokenHandle userHandle = credentials.LogonUser(LogonType.NewCredentials);

        return await WindowsIdentity.RunImpersonated(userHandle, async () => await action().ConfigureAwait(false));

    }

    private static async Task ExecuteCopyAsync(Input input, Options options, CancellationToken cancellationToken)
    {
        var results = FindMatchingFiles(input.Directory, input.Pattern);
        var fileTransferEntries = GetFileTransferEntries(results.Files, input.Directory, input.TargetDirectory, options.PreserveDirectoryStructure);

        if (options.IfTargetFileExists == FileExistsAction.Throw)
        {
            AssertNoTargetFileConflicts(fileTransferEntries.Values);
        }

        if (options.CreateTargetDirectories)
        {
            Directory.CreateDirectory(input.TargetDirectory);
        }

        var fileResults = new List<FileItem>();
        try
        {
            foreach (var entry in fileTransferEntries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var sourceFilePath = entry.Key;
                var targetFilePath = entry.Value;

                if (options.CreateTargetDirectories)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));
                }

                switch (options.IfTargetFileExists)
                {
                    case FileExistsAction.Rename:
                        targetFilePath = GetNonConflictingDestinationFilePath(sourceFilePath, targetFilePath);
                        await CopyFileAsync(sourceFilePath, targetFilePath, cancellationToken);
                        break;
                    case FileExistsAction.Overwrite:
                        if (System.IO.File.Exists(targetFilePath))
                        {
                            System.IO.File.Delete(targetFilePath);
                        }
                        await CopyFileAsync.Invoke(sourceFilePath, targetFilePath, cancellationToken).ConfigureAwait(false);
                        break;
                    case FileExistsAction.Throw:
                        if (System.IO.File.Exists(targetFilePath))
                        {
                            throw new IOException($"File '{targetFilePath}' already exists. No files copied.");
                        }
                        await CopyFileAsync.Invoke(sourceFilePath, targetFilePath, cancellationToken).ConfigureAwait(false);
                        break;
                }
                fileResults.Add(new FileItem(sourceFilePath, targetFilePath));
            }
        }
        catch (Exception)
        {
            //Delete the target files that were already moved before a file that exists breaks the move command
            DeleteExistingFiles(fileResults.Select(x => x.TargetPath));
            throw;
        }

        return fileResults;
    }

    private static async Task CopyFileAsync(Input input, CancellationToken cancellationToken)
    {
        using (FileStream sourceStream = File.Open(Path.Combine(input.Directory, input.Pattern), FileMode.Open))
        {
            using (FileStream destinationStream = File.Open(input.TargetDirectory, FileMode.CreateNew))
            {
                await sourceStream.CopyToAsync(destinationStream, 81920, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    internal static Tuple<string, string> GetDomainAndUsername(string username)
    {
        var domainAndUserName = username.Split('\\');
        if (domainAndUserName.Length != 2)
        {
            throw new ArgumentException($@"UserName field must be of format domain\username was: {username}");
        }
        var test = domainAndUserName[0];
        var test2 = domainAndUserName[1];
        return new Tuple<string, string>(domainAndUserName[0], domainAndUserName[1]);
    }

    internal static PatternMatchingResult FindMatchingFiles(string directoryPath, string pattern)
    {
        // Check the user can access the folder
        // This will return false if the path does not exist or you do not have read permissions.
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory does not exist or you do not have read access. Tried to access directory '{directoryPath}'");
        }

        var matcher = new Matcher();
        matcher.AddInclude(pattern);
        var results = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directoryPath)));
        return results;
    }

    private static Dictionary<string, string> GetFileTransferEntries(IEnumerable<FilePatternMatch> fileMatches, string sourceDirectory, string targetDirectory, bool preserveDirectoryStructure)
    {
        return fileMatches
            .ToDictionary(
                f => Path.Combine(sourceDirectory, f.Path),
                f => preserveDirectoryStructure
                 ? Path.GetFullPath(Path.Combine(targetDirectory, f.Path))
                 : Path.GetFullPath(Path.Combine(targetDirectory, Path.GetFileName(f.Path))));
    }

    private static void AssertNoTargetFileConflicts(IEnumerable<string> filePaths)
    {
        // check the target file list to see there should not be conflicts before doing anything
        var duplicateTargetPaths = GetDuplicateValues(filePaths);
        if (duplicateTargetPaths.Any())
        {
            throw new IOException($"Multiple files written to {string.Join(", ", duplicateTargetPaths)}. The files would get overwritten. No files copied.");
        }

        foreach (var targetFilePath in filePaths)
        {
            if (File.Exists(targetFilePath))
            {
                throw new IOException($"File '{targetFilePath}' already exists. No files copied.");
            }
        }
    }

    private static IList<string> GetDuplicateValues(IEnumerable<string> values)
    {
        return values.GroupBy(v => v).Where(x => x.Count() > 1).Select(k => k.Key).ToList();
    }

    internal static void DeleteExistingFiles(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }

    internal static string GetNonConflictingDestinationFilePath(string sourceFilePath, string destFilePath)
    {
        var count = 1;
        while (System.IO.File.Exists(destFilePath))
        {
            string tempFileName = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}({count++})";
            destFilePath = Path.Combine(Path.GetDirectoryName(destFilePath), path2: tempFileName + Path.GetExtension(sourceFilePath));
        }

        return destFilePath;
    }
}
