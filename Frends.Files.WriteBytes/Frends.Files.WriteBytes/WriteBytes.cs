using Frends.Files.WriteBytes.Definitions;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Frends.Files.WriteBytes;

///<summary>
/// Files task.
/// </summary>
public class Files
{
    /// <summary>
    /// Write file in bytes.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Files.WriteBytes)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Options parameters</param>
    /// <returns>Object {string Path, double SizeInMegaBytes}</returns>
    public static async Task<Result> WriteBytes([PropertyTab] Input input, [PropertyTab] Options options)
    {
        return await ExecuteActionAsync(() => ExecuteWriteBytes(input, options),
            options.UseGivenUserCredentialsForRemoteConnections, options.UserName, options.Password).ConfigureAwait(false);
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

    private static async Task<Result> ExecuteWriteBytes(Input input, Options options)
    {
        byte[] bytes = input?.ContentBytes as byte[];
        if (bytes == null)
            throw new ArgumentException("Input.ContentBytes must be a byte array", nameof(input.ContentBytes));

        var fileMode = GetAndCheckWriteMode(options.WriteBehaviour, input.Path);

        using var fileStream = new FileStream(input.Path, fileMode, FileAccess.Write, FileShare.Write, 4096, useAsync: true);
        using var memoryStream = new MemoryStream(bytes);
        await memoryStream.CopyToAsync(fileStream).ConfigureAwait(false);

        return new Result(new FileInfo(input.Path));
    }

    internal static Tuple<string, string> GetDomainAndUsername(string username)
    {
        var domainAndUserName = username.Split('\\');
        if (domainAndUserName.Length != 2)
            throw new ArgumentException($@"UserName field must be of format domain\username was: {username}");
        return new Tuple<string, string>(domainAndUserName[0], domainAndUserName[1]);
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
