using Frends.Files.Rename.Definitions;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Frends.Files.Rename;

///<summary>
/// Files task.
/// </summary>
public class Files
{
    /// <summary>
    /// Rename file.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Files.Rename)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Options parameters</param>
    /// <returns>Object {string Path, double SizeInMegaBytes}</returns>
    public static Result Rename([PropertyTab] Input input, [PropertyTab] Options options)
    {
        return ExecuteAction(() => ExecuteRename(input, options.RenameBehaviour),
            options.UseGivenUserCredentialsForRemoteConnections, options.UserName, options.Password);
    }

    private static Result ExecuteAction<Result>(Func<Result> action, bool useGivenCredentials, string username, string password)
    {
        if (!useGivenCredentials)
            return action();

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("UseGivenCredentials feature is only supported on Windows.");

        var (domain, user) = GetDomainAndUsername(username);

        UserCredentials credentials = new UserCredentials(domain, user, password);
        using SafeAccessTokenHandle userHandle = credentials.LogonUser(LogonType.NewCredentials);

        return WindowsIdentity.RunImpersonated(userHandle, () => action());
    }

    private static Result ExecuteRename(Input input, RenameBehaviour fileExistsAction)
    {
        if (!Directory.Exists(Path.GetDirectoryName(input.Path)))
            throw new DirectoryNotFoundException($"Directory does not exist or you do not have read access. Tried to access directory '{Path.GetDirectoryName(input.Path)}'.");
        if (!File.Exists(input.Path))
            throw new FileNotFoundException($"Could't find part of the path '{input.Path}'.");
        var directoryPath = Path.GetDirectoryName(input.Path);
        var newFileFullPath = Path.Combine(directoryPath, input.NewFileName);

        switch (fileExistsAction)
        {
            case RenameBehaviour.Rename:
                newFileFullPath = GetNonConflictingDestinationFilePath(input.Path, newFileFullPath);
                break;
            case RenameBehaviour.Overwrite:
                if (File.Exists(newFileFullPath)) File.Delete(newFileFullPath);
                break;
            case RenameBehaviour.Throw:
                if (File.Exists(newFileFullPath)) throw new IOException($"File already exists {newFileFullPath}. No file renamed.");
                break;
        }
        File.Move(input.Path, newFileFullPath);
        return new Result(newFileFullPath);
    }

    internal static Tuple<string, string> GetDomainAndUsername(string username)
    {
        var domainAndUserName = username.Split('\\');
        if (domainAndUserName.Length != 2)
            throw new ArgumentException($@"UserName field must be of format domain\username was: {username}");
        return new Tuple<string, string>(domainAndUserName[0], domainAndUserName[1]);
    }

    internal static string GetNonConflictingDestinationFilePath(string sourceFilePath, string destFilePath)
    {
        var count = 1;
        while (File.Exists(destFilePath))
        {
            string tempFileName = $"{Path.GetFileNameWithoutExtension(destFilePath)}({count++})";
            destFilePath = Path.Combine(Path.GetDirectoryName(destFilePath), path2: tempFileName + Path.GetExtension(sourceFilePath));
        }

        return destFilePath;
    }
}
