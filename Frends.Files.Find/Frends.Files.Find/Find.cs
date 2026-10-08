using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

using Frends.Files.Find.Definitions;
using Frends.Files.Find.Helpers;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;

namespace Frends.Files.Find;

/// <summary>
/// Files task.
/// </summary>
public static class Files
{
    /// <summary>
    /// Find files from directory.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Files.Find)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Options parameters</param>
    /// <param name="cancellationToken">Token used to cancel the operation before file discovery.</param>
    /// <returns>Object { bool Success, object Error, List [object { string Extension, string DirectoryName, string FullPath, string FileName, bool IsReadOnly, double SizeInMegaBytes, DateTime CreationTime, DateTime CreationTimeUtc, DateTime LastAccessTime, DateTime LastAccessTimeUtc, DateTime LastWriteTime, DateTime LastWriteTimeUtc }] Files }</returns>
    public static Result Find([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        try
        {
            ValidationHandler.Run(input, options);
            cancellationToken.ThrowIfCancellationRequested();

            return ExecuteAction(
                () => ExecuteFind(input),
                options.UseGivenUserCredentialsForRemoteConnections,
                options.UserName,
                options.Password);
        }
        catch (Exception exception)
        {
            return exception.Handle(options);
        }
    }

    internal static Tuple<string, string> GetDomainAndUsername(string username)
    {
        var domainAndUserName = username.Split('\\');
        if (domainAndUserName.Length != 2)
            throw new ArgumentException($@"UserName field must be of format domain\username was: {username}");
        return new Tuple<string, string>(domainAndUserName[0], domainAndUserName[1]);
    }

    private static TResult ExecuteAction<TResult>(Func<TResult> action, bool useGivenCredentials, string username, string password)
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
        var results = FilesHandler.FindMatchingFiles(input.Directory, input.Pattern);
        var files = results.Select(path => new FileItem(new FileInfo(Path.Combine(input.Directory, path)))).ToList();
        return new Result(true, files: files);
    }
}
