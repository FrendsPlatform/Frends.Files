using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Files.Copy.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// If set, allows you to give the user credentials to use to delete files on remote hosts.
    /// If not set, the agent service user credentials will be used.
    /// Note: This feature is only possible with Windows agents.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(false)]
    public bool UseGivenUserCredentialsForRemoteConnections { get; set; }

    /// <summary>
    /// This needs to be of format domain\username
    /// </summary>
    /// <example>domain\username</example>
    [DefaultValue("\"domain\\username\"")]
    [UIHint(nameof(UseGivenUserCredentialsForRemoteConnections), "", true)]
    public string UserName { get; set; }

    /// <summary>
    /// Password for the used credentials.
    /// </summary>
    /// <example>testpwd</example>
    [PasswordPropertyText]
    [UIHint(nameof(UseGivenUserCredentialsForRemoteConnections), "", true)]
    public string Password { get; set; }

    /// <summary>
    /// If set, will recreate the directory structure from the SourceDirectory under the TargetDirectory for copied files
    /// </summary>
    /// <example>true</example>
    [DefaultValue(false)]
    public bool PreserveDirectoryStructure { get; set; }

    /// <summary>
    /// If set, will create the target directory if it does not exist,
    /// as well as any sub directories when PreserveDirectoryStructure is set.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool CreateTargetDirectories { get; set; }

    /// <summary>
    /// What should happen if a file with the same name already exists in the target directory.
    /// * Throw - Throw an error and roll back all transfers
    /// * Overwrite - Overwrites the target file
    /// * Rename - Renames the transferred file by appending a number to the end
    /// </summary>
    /// <example>FileExistsAction.Overwrite</example>
    public FileExistsAction IfTargetFileExists { get; set; }

    /// <summary>
    /// Whether to continue copying after an individual file fails.
    /// When true, failed files are listed in the result and Success remains true.
    /// When false, copying stops at the first failure. The failed and unprocessed files
    /// are listed in FailedFiles when ThrowErrorOnFailure is false.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool ContinueOnFailure { get; set; }

    /// <summary>
    /// Whether to throw the exception when the task fails, including when a file fails
    /// and ContinueOnFailure is false. If false, returns a result with Success=false and Error details.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message for task-level failures.
    /// </summary>
    /// <example>File copy failed due to insufficient permissions</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
