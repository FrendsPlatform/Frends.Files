using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

using Frends.Files.Copy.Definitions;
using Frends.Files.Copy.Helpers;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;

namespace Frends.Files.Copy;

/// <summary>
/// Files task.
/// </summary>
public static class Files
{
    /// <summary>
    /// Copy files.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Files.Copy)
    /// </summary>
    /// <param name="input">Input parameters for the Task.</param>
    /// <param name="options">Additional options for the Task.</param>
    /// <param name="cancellationToken">CancellationToken given by Frends.</param>
    /// <returns>Object {bool Success, List&lt;Object { string SourcePath, string TargetPath }&gt; Files, List&lt;Object { string SourcePath, Exception Exception }&gt; FailedFiles, object Error {string Message, Exception AdditionalInfo} }</returns>
    public static async Task<Result> Copy([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        List<FileItem> files;
        List<FailedFileItem> failed;
        try
        {
            ValidationHandler.Run(input, options);
            (files, failed) = await ExecuteActionAsync(
                () => ExecuteCopyAsync(input, options, cancellationToken),
                options.UseGivenUserCredentialsForRemoteConnections,
                options.UserName,
                options.Password).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex.Handle(options ?? new Options());
        }

        if (!options.ContinueOnFailure && failed.Count > 0)
            return failed[0].Exception.Handle(options, files, failed);

        return new Result(true, files: files, failedFiles: failed);
    }

    private static Tuple<string, string> GetDomainAndUsername(string username)
    {
        var domainAndUserName = username.Split('\\');
        if (domainAndUserName.Length != 2)
            throw new ArgumentException($@"UserName field must be of format domain\username was: {username}");
        return new Tuple<string, string>(domainAndUserName[0], domainAndUserName[1]);
    }

    private static string GetNonConflictingDestinationFilePath(string sourceFilePath, string destFilePath)
    {
        var count = 1;
        while (File.Exists(destFilePath))
        {
            string tempFileName = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}({count++})";
            destFilePath = Path.Combine(Path.GetDirectoryName(destFilePath), path2: tempFileName + Path.GetExtension(sourceFilePath));
        }

        return destFilePath;
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

    private static async Task<(List<FileItem> Files, List<FailedFileItem> FailedFiles)> ExecuteCopyAsync(Input input, Options options, CancellationToken cancellationToken)
    {
        var results = FilesHandler.FindMatchingFiles(input.Directory, input.Pattern);
        var fileTransferEntries = GetFileTransferEntries(results, input.Directory, input.TargetDirectory, options.PreserveDirectoryStructure);

        if (options.IfTargetFileExists == FileExistsAction.Throw)
            AssertNoTargetFileConflicts(fileTransferEntries.Values);

        if (options.CreateTargetDirectories)
            Directory.CreateDirectory(input.TargetDirectory);

        var files = new List<FileItem>();
        var failedFiles = new List<FailedFileItem>();
        var stopped = false;

        foreach (var entry in fileTransferEntries)
        {
            var sourceFilePath = entry.Key;
            if (stopped)
            {
                failedFiles.Add(new FailedFileItem(sourceFilePath, null));
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();
            var targetFilePath = entry.Value;

            try
            {
                if (options.CreateTargetDirectories)
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));

                switch (options.IfTargetFileExists)
                {
                    case FileExistsAction.Rename:
                        targetFilePath = GetNonConflictingDestinationFilePath(sourceFilePath, targetFilePath);
                        await CopyFileAsync(sourceFilePath, targetFilePath, cancellationToken);
                        break;

                    case FileExistsAction.Overwrite:
                        if (File.Exists(targetFilePath))
                            File.Delete(targetFilePath);
                        await CopyFileAsync(sourceFilePath, targetFilePath, cancellationToken).ConfigureAwait(false);
                        break;

                    case FileExistsAction.Throw:
                        if (File.Exists(targetFilePath))
                            throw new IOException($"File '{targetFilePath}' already exists. No files copied.");
                        await CopyFileAsync(sourceFilePath, targetFilePath, cancellationToken).ConfigureAwait(false);
                        break;
                }

                files.Add(new FileItem(sourceFilePath, targetFilePath));
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException) throw;
                failedFiles.Add(new FailedFileItem(sourceFilePath, ex));
                stopped = !options.ContinueOnFailure;
            }
        }

        return (files, failedFiles);
    }

    private static async Task CopyFileAsync(string source, string destination, CancellationToken cancellationToken)
    {
        using FileStream sourceStream = File.Open(source, FileMode.Open, FileAccess.Read);
        using FileStream destinationStream = File.Open(destination, FileMode.CreateNew);

        await sourceStream.CopyToAsync(destinationStream, 81920, cancellationToken).ConfigureAwait(false);
    }

    private static Dictionary<string, string> GetFileTransferEntries(IEnumerable<string> fileMatches, string sourceDirectory, string targetDirectory, bool preserveDirectoryStructure)
    {
        return fileMatches
            .ToDictionary(
                f => Path.Combine(sourceDirectory, f),
                f => preserveDirectoryStructure
                 ? Path.GetFullPath(Path.Combine(targetDirectory, f))
                 : Path.GetFullPath(Path.Combine(targetDirectory, Path.GetFileName(f))));
    }

    private static void AssertNoTargetFileConflicts(IEnumerable<string> filePaths)
    {
        // check the target file list to see there should not be conflicts before doing anything
        var duplicateTargetPaths = GetDuplicateValues(filePaths);
        if (duplicateTargetPaths.Any())
            throw new IOException($"Multiple files written to {string.Join(", ", duplicateTargetPaths)}. The files would get overwritten. No files copied.");
    }

    private static IList<string> GetDuplicateValues(IEnumerable<string> values)
    {
        return values.GroupBy(v => v).Where(x => x.Count() > 1).Select(k => k.Key).ToList();
    }
}
