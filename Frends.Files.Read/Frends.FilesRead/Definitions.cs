using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
#pragma warning disable 1591

namespace Frends.FilesRead
{
	public enum FileEncoding { UTF8, ANSI, ASCII, Unicode, Other }

	public class ReadInput
	{
		/// <summary>
		/// Full path of the file
		/// </summary>
		[DefaultValue("\"c:\\temp\\foo.txt\"")]
		public string Path { get; set; }
	}


	public class ReadOption
	{
		/// <summary>
		/// If set, allows you to give the user credentials to use to read files on remote hosts.
		/// If not set, the agent service user credentials will be used.
		/// Note: For reading files on the local machine, the agent service user credentials will always be used, even if this option is set.
		/// </summary>
		public bool UseGivenUserCredentialsForRemoteConnections { get; set; }

		/// <summary>
		/// This needs to be of format domain\username
		/// </summary>
		[DefaultValue("\"domain\\username\"")]
		[UIHint(nameof(UseGivenUserCredentialsForRemoteConnections), "", true)]
		public string UserName { get; set; }

		[PasswordPropertyText]
		[UIHint(nameof(UseGivenUserCredentialsForRemoteConnections), "", true)]
		public string Password { get; set; }

		/// <summary>
		/// Encoding for the read content. By selecting 'Other' you can use any encoding.
		/// </summary>
		public FileEncoding FileEncoding { get; set; }

		[UIHint(nameof(FileEncoding), "", FileEncoding.UTF8)]
		public bool EnableBom { get; set; }

		/// <summary>
		/// File encoding to be used. A partial list of possible encodings: https://en.wikipedia.org/wiki/Windows_code_page#List
		/// </summary>
		[UIHint(nameof(FileEncoding), "", FileEncoding.Other)]
		public string EncodingInString { get; set; }
	}
	public class ReadResult
	{
		public ReadResult(FileInfo info, string content)
		{
			Path = info.FullName;
			SizeInMegaBytes = Math.Round((info.Length / 1024d / 1024d), 3);
			Content = content;
			CreationTime = info.CreationTime;
			LastWriteTime = info.LastWriteTime;
		}
		public string Content { get; set; }
		public string Path { get; set; }
		public double SizeInMegaBytes { get; set; }
		public DateTime CreationTime { get; set; }
		public DateTime LastWriteTime { get; set; }
	}

    }
