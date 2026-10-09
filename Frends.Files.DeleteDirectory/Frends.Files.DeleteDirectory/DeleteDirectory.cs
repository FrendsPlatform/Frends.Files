using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using Frends.Files.DeleteDirectory.Definitions;
using Frends.Files.DeleteDirectory.Helpers;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;

namespace Frends.Files.DeleteDirectory;

/// <summary>
/// Task class.
/// </summary>
public static class Files
{
    /// <summary>
    /// Deletes all directories and subdirectories in the specified path. Will not do anything if the directory do not exist.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-Files-DeleteDirectory)
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="options">Additional task options.</param>
    /// <param name="cancellationToken">Token to stop task execution.</param>
    /// <returns>Object { string Path, bool Success } </returns>
    public static Result DeleteDirectory([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidationHandler.Run(input, options);

            if (!options.UseGivenUserCredentialsForRemoteConnections)
                return ExecuteDelete(input, options.DeleteRecursively);

            var domainAndUserName = GetDomainAndUserName(options.UserName);
            return RunAsUser(domainAndUserName[0], domainAndUserName[1], options.Password, () => ExecuteDelete(input, options.DeleteRecursively));
        }
        catch (Exception exception)
        {
            return exception.Handle(options);
        }
    }

    private static T RunAsUser<T>(string domain, string username, string password, Func<T> action)
        where T : Result
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new Exception("Impersonation only supported on Windows systems");
        }

        var credentials = new UserCredentials(domain, username, password);
        using SafeAccessTokenHandle userHandle = credentials.LogonUser(LogonType.NewCredentials);

        return WindowsIdentity.RunImpersonated(userHandle, action);
    }

    private static Result ExecuteDelete(Input input, bool optionsDeleteRecursivly)
    {
        if (!Directory.Exists(input.Directory))
            return new Result(input.Directory, false);

        Directory.Delete(input.Directory, optionsDeleteRecursivly);
        return new Result(input.Directory, true);
    }

    private static string[] GetDomainAndUserName(string username)
    {
        var domainAndUserName = username.Split('\\');
        if (domainAndUserName.Length != 2)
            throw new ArgumentException($@"UserName field must be of format domain\username was: {username}");

        return domainAndUserName;
    }
}
