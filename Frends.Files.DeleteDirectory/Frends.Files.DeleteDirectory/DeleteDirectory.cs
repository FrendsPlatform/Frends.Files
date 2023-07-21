using Frends.Files.DeleteDirectory.Definitions;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;

namespace Frends.Files.DeleteDirectory;

/// <summary>
/// Task class.
/// </summary>
public class Files
{
    /// <summary>
    /// Deletes all directories and subdirectories in the specified path. Will not do anything if the directory do not exist.
    /// [Documentation](https://tasks.frends.com/tasks#frends-tasks/Frends.Files.DeleteDirectory)
    /// </summary>
    /// <returns>Object { string Path } </returns>
    public static Result DeleteDirectory([PropertyTab] Input input, [PropertyTab] Options options)
    {
        if (string.IsNullOrEmpty(input.Directory))
            throw new ArgumentNullException("Directory cannot be empty.");

        if (!options.UseGivenUserCredentialsForRemoteConnections)
        {
            return ExecuteDelete(input, options.DeleteRecursively);
        }

        var domainAndUserName = GetDomainAndUserName(options.UserName);
        return RunAsUser(domainAndUserName[0], domainAndUserName[1], options.Password, () => ExecuteDelete(input, options.DeleteRecursively));
    }

    private static T RunAsUser<T>(string domain, string username, string password, Func<T> action) where T : Result
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
        if (!System.IO.Directory.Exists(input.Directory))
        {
            return new Result(input.Directory, false);
        }
        System.IO.Directory.Delete(input.Directory, optionsDeleteRecursivly);
        return new Result(input.Directory, true);
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

