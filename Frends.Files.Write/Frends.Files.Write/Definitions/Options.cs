using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Files.Write.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Sets a value indicating whether remote connection is used.
    /// If set, allows you to give the user credentials to use to delete files on remote hosts.
    /// If not set, the agent service user credentials will be used.
    /// Note: This feature is only possible with Windows agents.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(false)]
    public bool UseGivenUserCredentialsForRemoteConnections { get; set; }

    /// <summary>
    /// Sets username. This needs to be of format domain\username.
    /// </summary>
    /// <example>domain\username</example>
    [DefaultValue("\"domain\\username\"")]
    [UIHint(nameof(UseGivenUserCredentialsForRemoteConnections), "", true)]
    public string UserName { get; set; }

    /// <summary>
    /// Sets password for the used credentials.
    /// </summary>
    /// <example>testpwd</example>
    [PasswordPropertyText]
    [UIHint(nameof(UseGivenUserCredentialsForRemoteConnections), "", true)]
    public string Password { get; set; }

    /// <summary>
    /// Sets encoding for the written content. By selecting 'Other' you can use any encoding.
    /// </summary>
    /// <example>FileEncoding.UTF8</example>
    [DefaultValue(FileEncoding.UTF8)]
    public FileEncoding FileEncoding { get; set; }

    /// <summary>
    /// Sets a value indicating whether enablation BOM for UTF-8.
    /// </summary>
    /// <example>true</example>
    [UIHint(nameof(FileEncoding), "", FileEncoding.UTF8)]
    [DefaultValue(false)]
    public bool EnableBom { get; set; }

    /// <summary>
    /// Sets file encoding to be used. A partial list of possible encodings: https://en.wikipedia.org/wiki/Windows_code_page#List.
    /// </summary>
    /// <example>ISO-8859-2</example>
    [UIHint(nameof(FileEncoding), "", FileEncoding.Other)]
    public string EncodingInString { get; set; }

    /// <summary>
    /// Sets write behaviour.
    /// How the file write should work if a file with the new name already exists.
    /// </summary>
    /// <example>WriteBehaviour.Throw</example>
    [DefaultValue(WriteBehaviour.Throw)]
    public WriteBehaviour WriteBehaviour { get; set; }
}