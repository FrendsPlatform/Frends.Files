using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using Frends.Files.CreateDirectory.Definitions;
using Frends.Files.CreateDirectory.Helpers;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;

namespace Frends.Files.CreateDirectory;

/// <summary>
/// Task class.
/// </summary>
public static class Files
{
    /// <summary>
    /// Creates all directories and subdirectories in the specified path unless they already exist. Will not do anything if the directory exists.
    /// [Documentation](https://tasks.frends.com/tasks#frends-tasks/Frends.Files.CreateDirectory)
    /// </summary>
    /// <param name="input">Directory path to create.</param>
    /// <param name="options">Additional task options.</param>
    /// <param name="cancellationToken">Token used to cancel the operation before directory creation.</param>
    /// <returns>Object { string Path } </returns>
    public static Result CreateDirectory([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        try
        {
            ValidationHandler.Run(input, options);
            cancellationToken.ThrowIfCancellationRequested();

            if (!options.UseGivenUserCredentialsForRemoteConnections)
                return ExecuteCreate(input);

            var domainAndUserName = GetDomainAndUserName(options.UserName);
            return RunAsUser(domainAndUserName[0], domainAndUserName[1], options.Password, () => ExecuteCreate(input));
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

    private static Result ExecuteCreate(Input input)
    {
        var newFolder = System.IO.Directory.CreateDirectory(input.Directory);
        return new Result(true, path: newFolder.FullName);
    }

    private static string[] GetDomainAndUserName(string username)
    {
        var domainAndUserName = username.Split('\\');
        if (domainAndUserName.Length != 2)
        {
            throw new ArgumentException($@"UserName field must be of format domain\username was: {username}");
        }

        return domainAndUserName;
    }
}
