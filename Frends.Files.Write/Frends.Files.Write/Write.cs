using Frends.Files.Write.Definitions;
using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using SimpleImpersonation;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using System.Threading.Tasks;

namespace Frends.Files.Write;
///<summary>
/// Files task.
/// </summary>
public class Files
{
    /// <summary>
    /// Write file.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Files.Write)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Options parameters</param>
    /// <returns>Object {string Path, double SizeInMegaBytes}</returns>
    public static async Task<Result> Write([PropertyTab] Input input, [PropertyTab] Options options)
    {
        return await ExecuteActionAsync(() => ExecuteWrite(input, options),
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

    private static async Task<Result> ExecuteWrite(Input input, Options options)
    {
        var encoding = GetEncoding(options.FileEncoding, options.EnableBom, options.EncodingInString);
        var fileMode = GetAndCheckWriteMode(options.WriteBehaviour, input.Path);

        using var fileStream = new FileStream(input.Path, fileMode, FileAccess.Write, FileShare.Write, 4096, useAsync: true);
        using var writer = new StreamWriter(fileStream, encoding);
        await writer.WriteAsync(input.Content).ConfigureAwait(false);

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
                {
                    throw new IOException($"File already exists: {filePath}.");
                }
                return FileMode.Create;
            default:
                throw new ArgumentException("Unsupported write option: " + givenWriteBehaviour);
        }
    }

    private static Encoding GetEncoding(FileEncoding optionsFileEncoding, bool optionsEnableBom, string optionsEncodingInString)
    {
        switch (optionsFileEncoding)
        {
            case FileEncoding.Other:
                return Encoding.GetEncoding(optionsEncodingInString);
            case FileEncoding.ASCII:
                return Encoding.ASCII;
            case FileEncoding.ANSI:
                return Encoding.Default;
            case FileEncoding.UTF8:
                return optionsEnableBom ? new UTF8Encoding(true) : new UTF8Encoding(false);
            case FileEncoding.Unicode:
                return Encoding.Unicode;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
