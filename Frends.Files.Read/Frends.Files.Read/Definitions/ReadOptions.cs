using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;



namespace Frends.Files.Read


	public class ReadOptions
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

	/// <summary>
	/// Indicates that an object's text representation is obscured by characters such as asterisks. This class cannot be inherited.
	/// </summary>
	[PasswordPropertyText]
	[UIHint(nameof(UseGivenUserCredentialsForRemoteConnections), "", true)]
	public string Password { get; set; }

	/// <summary>
	/// Encoding for the read content. By selecting 'Other' you can use any encoding.
	/// </summary>
	public FileEncodings FileEncoding { get; set; }

	/// <summary>
	/// File Encoding UTF8 is set as default value
	/// </summary>
	[UIHint(nameof(FileEncoding), "", FileEncodings.UTF8)]
	public bool EnableBom { get; set; }

	/// <summary>
	/// File encoding to be used. A partial list of possible encodings: https://en.wikipedia.org/wiki/Windows_code_page#List
	/// </summary>
	[UIHint(nameof(FileEncoding), "", FileEncodings.Other)]
	public string EncodingInString { get; set; }
}