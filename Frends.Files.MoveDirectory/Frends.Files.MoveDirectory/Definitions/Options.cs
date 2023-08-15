using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Files.MoveDirectory.Definitions;

/// <summary>
/// Options class
/// </summary>
public class Options
{
    /// <summary>
    /// What to do if target directory already exists.
    ///  * Throw - Throw an error (the default)
    ///  * Rename - Create a new directory with a name that appends a number to the end, e.g. "directory(2)"
    ///  * Overwrite - Overwrite the target directory, by removing it first before moving the source directory
    /// </summary>
    public DirectoryExistsAction IfTargetDirectoryExists { get; set; }

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
}
