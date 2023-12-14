using Frends.Files.Find.Definitions;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Linq;

namespace Frends.Files.Find;

///<summary>
/// Files task.
/// </summary>
public class Files
{
    /// <summary>
    /// Find files from directory.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Files.Find)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Options parameters</param>
    /// <returns>Object { List [object { string Extension, string DirectoryName, string FullPath, string FileName, bool IsReadOnly, double SizeInMegaBytes, DateTime CreationTime, DateTime CreationTimeUtc, DateTime LastAccessTime, DateTime LastAccessTimeUtc, DateTime LastWriteTime, DateTime LastWriteTimeUtc }] Files }</returns>
    public static Result Find([PropertyTab] Input input, [PropertyTab] Options options)
    {
        return ExecuteAction(() => ExecuteFind(input),
            options.UseGivenUserCredentialsForRemoteConnections, options.UserName, options.Password);
    }

    private static Result ExecuteAction<Result>(Func<Result> action, bool useGivenCredentials, string username, string password)
    {
        if (!useGivenCredentials)
            return action();

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("UseGivenCredentials feature is only supported on Windows.");

        var (domain, user) = GetDomainAndUsername(username);

        var credentials = new UserCredentials(domain, user, password);
        using SafeAccessTokenHandle userHandle = credentials.LogonUser(LogonType.NewCredentials);

        return WindowsIdentity.RunImpersonated(userHandle, () => action());
    }

    private static Result ExecuteFind(Input input)
    {
        var results = FindMatchingFiles(input.Directory, input.Pattern);
        var foundFiles = results.Files.Select(match => Path.Combine(input.Directory, match.Path)).ToArray();
        var files = foundFiles.Select(fullPath => new FileItem(new FileInfo(fullPath))).ToList();
        return new Result(files);
    }

    internal static Tuple<string, string> GetDomainAndUsername(string username)
    {
        var domainAndUserName = username.Split('\\');
        if (domainAndUserName.Length != 2)
            throw new ArgumentException($@"UserName field must be of format domain\username was: {username}");
        return new Tuple<string, string>(domainAndUserName[0], domainAndUserName[1]);
    }

    internal static PatternMatchingResult FindMatchingFiles(string directoryPath, string pattern)
    {
        // Check the user can access the folder
        // This will return false if the path does not exist or you do not have read permissions.
        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Directory does not exist or you do not have read access. Tried to access directory '{directoryPath}'.");

        var matcher = new Matcher();
        matcher.AddInclude(pattern);
        var results = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directoryPath)));
        return results;
    }
}
