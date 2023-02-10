using System.IO;
using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Frends.Files.Read
{
    /// <summary>
    /// Classes and methods in tasks must be static
    /// </summary>

    public static class FilesRead
    {
        /// <summary>
        /// Read contents of a file as a string. See: https://github.com/FrendsPlatform/Frends.File#Read
        /// </summary>
        /// <returns>Object {string Content, string Path, double SizeInMegaBytes, DateTime CreationTime, DateTime LastWriteTime }  </returns>
        public static async Task<ReadResult> Read([PropertyTab] ReadInput input, [PropertyTab] ReadOptions options)
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

			_ = GetDomainAndUserName(userName);


			throw new PlatformNotSupportedException("Impersonation not supported for this platform. Only works on full framework.");

        }


        private static async Task<ReadResult> ExecuteRead(ReadInput input, ReadOptions options)
        {
            var encoding = GetEncoding(options.FileEncoding, options.EnableBom, options.EncodingInString);

			using var fileStream = new FileStream(input.Path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
			using var reader = new StreamReader(fileStream, encoding, detectEncodingFromByteOrderMarks: true);
			var content = await reader.ReadToEndAsync().ConfigureAwait(false);
			return new ReadResult(new FileInfo(input.Path), content);
		}

		

		private static Encoding GetEncoding(FileEncodings optionsFileEncoding, bool optionsEnableBom, string optionsEncodingInString)
		{
            return optionsFileEncoding switch
            {
                FileEncodings.Other => Encoding.GetEncoding(optionsEncodingInString),
                FileEncodings.ASCII => Encoding.ASCII,
                FileEncodings.ANSI => Encoding.Default,
                FileEncodings.UTF8 => optionsEnableBom ? new UTF8Encoding(true) : new UTF8Encoding(false),
                FileEncodings.Unicode => Encoding.Unicode,
                _ => throw new ArgumentOutOfRangeException()

            };
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
