using Frends.Files.MoveDirectory.Definitions;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;
using System.IO;

namespace Frends.Files.MoveDirectory;

/// <summary>
/// Task class.
/// </summary>
public class Files
{
    /// <summary>
    /// Moves a directory. By default will throw an error if the directory already exists.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Files.MoveDirectory)
    /// </summary>
    /// <returns>Object { string SourcePath, string TargetPath } </returns>
    public static Result MoveDirectory([PropertyTab] Input input, [PropertyTab] Options options)
    {
        if (string.IsNullOrEmpty(input.SourceDirectory) || string.IsNullOrEmpty(input.TargetDirectory))
            throw new ArgumentNullException("Source or Target Directory cannot be empty.");
        if (!options.UseGivenUserCredentialsForRemoteConnections)
            return ExecuteMove(input, options.IfTargetDirectoryExists);

        var domainAndUserName = GetDomainAndUserName(options.UserName);
        return RunAsUser(domainAndUserName[0], domainAndUserName[1], options.Password, () =>
                ExecuteMove(input, options.IfTargetDirectoryExists));
    }

    private static Result ExecuteMove(Input input, DirectoryExistsAction directoryExistsAction)
    {
        var destinationFolderPath = input.TargetDirectory;
        switch (directoryExistsAction)
        {
            case DirectoryExistsAction.Rename:
                var count = 1;
                while (Directory.Exists(destinationFolderPath))
                {
                    destinationFolderPath = $"{destinationFolderPath}({count++})";
                }
                break;
            case DirectoryExistsAction.Overwrite:
                if (Directory.Exists(destinationFolderPath))
                    Directory.Delete(destinationFolderPath, true);
                break;
            case DirectoryExistsAction.Throw: //Will throw if target folder exist
                break;
        }

        Directory.Move(input.SourceDirectory, destinationFolderPath);
        return new Result(input.SourceDirectory, destinationFolderPath);
    }

    private static T RunAsUser<T>(string domain, string username, string password, Func<T> action) where T : Result
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("Impersonation only supported on Windows systems");

        var credentials = new UserCredentials(domain, username, password);
        using SafeAccessTokenHandle userHandle = credentials.LogonUser(LogonType.NewCredentials);

        return WindowsIdentity.RunImpersonated(userHandle, action);
    }

    private static string[] GetDomainAndUserName(string username)
    {
        var domainAndUserName = username.Split('\\');
        if (domainAndUserName.Length != 2)
            throw new ArgumentException($@"UserName field must be of format domain\username was: {username}");
        return domainAndUserName;
    }
}

