using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Files.DeleteDirectory.Definitions;

/// <summary>
/// Options class
/// </summary>
public class Options
{
    /// <summary>
    /// Delete all files and sub folders.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(false)]
    public bool DeleteRecursively { get; set; }

    /// <summary>
    /// If set, allows you to give the user credentials to use to create directories on remote hosts.
    /// If not set, the agent service user credentials will be used.
    /// Note: For creating directories on the local machine, the agent service user credentials will always be used, even if this option is set.
    /// </summary>
    /// <example>true</example>
    public bool UseGivenUserCredentialsForRemoteConnections { get; set; }

    /// <summary>
    /// This needs to be of format domain\username
    /// </summary>
    /// <example>Domain\Username</example>
    [DefaultValue("\"domain\\username\"")]
    [UIHint(nameof(UseGivenUserCredentialsForRemoteConnections), "", true)]
    public string UserName { get; set; }

    /// <summary>
    /// Password of user
    /// </summary>
    /// <example>Password123</example>
    [PasswordPropertyText]
    [UIHint(nameof(UseGivenUserCredentialsForRemoteConnections), "", true)]
    public string Password { get; set; }

    /// <summary>
    /// Whether to throw an error on failure.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure. If is true,
    /// the original exception is wrapped in a new exception with this message.
    /// </summary>
    /// <example>Directory deletion failed</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
