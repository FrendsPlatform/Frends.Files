using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Frends.Files.WriteBytes.Definitions;
using Frends.Files.WriteBytes.Helpers;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;

namespace Frends.Files.WriteBytes;

/// <summary>
/// Files task.
/// </summary>
public static class Files
{
    /// <summary>
    /// Write file in bytes.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Files.WriteBytes)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Options parameters</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>Object { bool Success, string Path, double SizeInMegaBytes, Object Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> WriteBytes([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await ExecuteActionAsync(
                () => ExecuteWriteBytes(input, options, cancellationToken), options.UseGivenUserCredentialsForRemoteConnections, options.UserName, options.Password).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex.Handle(options);
        }
    }

    internal static Tuple<string, string> GetDomainAndUsername(string username)
    {
        var domainAndUserName = username.Split('\\');
        if (domainAndUserName.Length != 2)
            throw new ArgumentException($@"UserName field must be of format domain\username was: {username}");
        return new Tuple<string, string>(domainAndUserName[0], domainAndUserName[1]);
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

    private static async Task<Result> ExecuteWriteBytes(Input input, Options options, CancellationToken cancellationToken)
    {
        byte[] bytes = input?.ContentBytes as byte[];
        if (bytes == null)
            throw new ArgumentException("Input.ContentBytes must be a byte array", nameof(input.ContentBytes));

        var fileMode = GetAndCheckWriteMode(options.WriteBehaviour, input.Path);

        using var fileStream = new FileStream(input.Path, fileMode, FileAccess.Write, FileShare.Write, 4096, useAsync: true);
        using var memoryStream = new MemoryStream(bytes);
        await memoryStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);

        return new Result(true, new FileInfo(input.Path));
    }

    private static FileMode GetAndCheckWriteMode(WriteBehaviour givenWriteBehaviour, string filePath)
    {
        switch (givenWriteBehaviour)
        {
            case WriteBehaviour.Append:
                return FileMode.Append;

            case WriteBehaviour.Overwrite:
                return FileMode.Create;

            case WriteBehaviour.Throw:
                if (File.Exists(filePath))
                    throw new IOException($"File already exists: {filePath}.");
                return FileMode.Create;
            default:
                throw new ArgumentException("Unsupported write option: " + givenWriteBehaviour);
        }
    }
}
