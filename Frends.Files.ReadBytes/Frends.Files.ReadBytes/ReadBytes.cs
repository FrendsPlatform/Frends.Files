using Frends.Files.ReadBytes.Definitions;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Threading;

namespace Frends.Files.ReadBytes;

///<summary>
/// Files task.
/// </summary>
public class Files
{
    /// <summary>
    /// Read contents of a file as a byte array.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Files.ReadBytes)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Options parameters</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Object { string ContentBytes, string Path, double SizeInMegaBytes, DateTime CreationTime, DateTime LastWriteTime }</returns>
    public static async Task<Result> ReadBytes([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        return await ExecuteActionAsync(
                    () => ExecuteReadBytes(input, cancellationToken),
                    options.UseGivenUserCredentialsForRemoteConnections,
                    options.UserName,
                    options.Password)
                .ConfigureAwait(false);
    }

    private static async Task<TResult> ExecuteActionAsync<TResult>(Func<Task<TResult>> action, bool useGivenCredentials, string username, string password)
    {
        if (!useGivenCredentials)
            return await action().ConfigureAwait(false);

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("UseGivenCredentials feature is only supported on Windows.");

        var (domain, user) = GetDomainAndUsername(username);

        UserCredentials credentials = new(domain, user, password);
        using SafeAccessTokenHandle userHandle = credentials.LogonUser(LogonType.NewCredentials);

        return await WindowsIdentity.RunImpersonated(userHandle, async () => await action().ConfigureAwait(false));
    }

    private static async Task<Result> ExecuteReadBytes(Input input, CancellationToken cancellationToken)
    {
        using var file = new FileStream(input.Path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        var buffer = new byte[file.Length];
        await file.ReadAsync(buffer, 0, (int)file.Length, cancellationToken).ConfigureAwait(false);

        return new Result(new FileInfo(input.Path), buffer);
    }

    internal static Tuple<string, string> GetDomainAndUsername(string username)
    {
        var domainAndUserName = username.Split('\\');
        if (domainAndUserName.Length != 2)
            throw new ArgumentException($@"UserName field must be of format domain\username was: {username}");
        return new Tuple<string, string>(domainAndUserName[0], domainAndUserName[1]);
    }
}
