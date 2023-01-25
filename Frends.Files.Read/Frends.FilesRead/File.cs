using SimpleImpersonation;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System.Collections.Generic;
using System.IO;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

#pragma warning disable 1591

namespace Frends.FilesRead
{
    public static class FilesRead
    {
        /// <summary>
        /// Read contents of a file as a string. See: https://github.com/FrendsPlatform/Frends.File#Read
        /// </summary>
        /// <returns>Object {string Content, string Path, double SizeInMegaBytes, DateTime CreationTime, DateTime LastWriteTime }  </returns>
        public static async Task<ReadResult> Read([PropertyTab] ReadInput input, [PropertyTab] ReadOption options)
        {
            return await ExecuteActionAsync(
                    () => ExecuteRead(input, options),
                    options.UseGivenUserCredentialsForRemoteConnections,
                    options.UserName,
                    options.Password)
                .ConfigureAwait(false);
        }


        private static async Task<TResult> ExecuteActionAsync<TResult>(Func<Task<TResult>> action, bool useGivenCredentials, string userName, string password)
        {
            if (!useGivenCredentials)
            {
                return await action().ConfigureAwait(false);
            }

            var domainAndUserName = GetDomainAndUserName(userName);
#if NET461
            return await Impersonation.RunAsUser(
                            new UserCredentials(domainAndUserName[0], domainAndUserName[1], password), LogonType.NewCredentials,
                            async () => await action().ConfigureAwait(false));
     
#else
            throw new PlatformNotSupportedException("Impersonation not supported for this platform. Only works on full framework.");
#endif

        }

        private static TResult ExecuteAction<TResult>(Func<TResult> action, bool useGivenCredentials, string userName, string password)
        {
            if (!useGivenCredentials)
            {
                return action();
            }

            var domainAndUserName = GetDomainAndUserName(userName);

            return Impersonation.RunAsUser(new UserCredentials(domainAndUserName[0], domainAndUserName[1], password),
                LogonType.NewCredentials, action);
        }

        internal static PatternMatchingResult FindMatchingFiles(string directoryPath, string pattern)
        {
            // Check the user can access the folder
            // This will return false if the path does not exist or you do not have read permissions.
            if (!Directory.Exists(directoryPath))
            {
                throw new Exception($"Directory does not exist or you do not have read access. Tried to access directory '{directoryPath}'");
            }

            var matcher = new Matcher();
            matcher.AddInclude(pattern);
            var results = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directoryPath)));
            return results;
        }

#region Executes for that public static tasks.
        private static async Task<ReadResult> ExecuteRead(ReadInput input, ReadOption options)
        {
            var encoding = GetEncoding(options.FileEncoding, options.EnableBom, options.EncodingInString);

            using (var fileStream = new FileStream(input.Path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
            using (var reader = new StreamReader(fileStream, encoding, detectEncodingFromByteOrderMarks: true))
            {
                var content = await reader.ReadToEndAsync().ConfigureAwait(false);
                return new ReadResult(new FileInfo(input.Path), content);
            }
        }

      
      

        internal static string GetNonConflictingDestinationFilePath(string sourceFilePath, string destFilePath)
        {
            var count = 1;
            while (System.IO.File.Exists(destFilePath))
            {
                string tempFileName = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}({count++})";
                destFilePath = Path.Combine(Path.GetDirectoryName(destFilePath), path2: tempFileName + Path.GetExtension(sourceFilePath));
            }

            return destFilePath;
        }

      

       
       


#endregion

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

        internal static void DeleteExistingFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file); // TODO: Add error handling?
                }
            }
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
