using Frends.Files.CreateDirectory.Definitions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("Frends.HTTP.RequestBytes.Tests")]
namespace Frends.Files.CreateDirectory;

/// <summary>
/// Task class.
/// </summary>
public class Files
{
    /// <summary>
    /// Creates all directories and subdirectories in the specified path unless they already exist. Will not do anything if the directory exists. See https://github.com/FrendsPlatform/Frends.Directory
    /// </summary>
    /// <returns>Object { string Path } </returns>
    public static Result CreateDirectory([PropertyTab] Input input, [PropertyTab] Options options)
    {
        if (!options.UseGivenUserCredentialsForRemoteConnections)
        {
            return ExecuteCreate(input);
        }

        var domainAndUserName = GetDomainAndUserName(options.UserName);
        return RunAsUser(domainAndUserName[0], domainAndUserName[1], options.Password, () => ExecuteCreate(input));
    }

    private static T RunAsUser<T>(string domain, string username, string password, Func<T> action) where T : Result
    {
#if !NET461
        // For some reason impersonation is not working on the Core agent even when running on Windows, will have to be investigated
        // Works fine on the Framework agent for both framework and standard Processes
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new Exception("Impersonation only supported on Windows systems");
        }
#endif

        return Impersonation.RunAsUser(new UserCredentials(domain, username, password),
            LogonType.NewCredentials, action);
    }

    private static CreateResult ExecuteCreate(SharedInput input)
    {
        var newFolder = System.IO.Directory.CreateDirectory(input.Directory);
        return new CreateResult(newFolder.FullName);
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
}

