﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

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
    /// as well as any sub directories if <see cref="PreserveDirectoryStructure"/> is set.
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
    /// Whether to throw an error on a failure to copy a file.
    /// This option is independent of the <see cref="IfTargetFileExists"/> option with the value of <see cref="FileExistsAction.Throw"/> and will affect handling of all errors during the copy process.
    /// If set to false, task will continue executing despite files failing to copy, otherwise execution will stop at the first failure.
    /// 
    /// Note: When set to false and <see cref="IfTargetFileExists"/> is set to <see cref="FileExistsAction.Throw"/>, the task will continue copying other files despite a failure.
    /// Behaviour of <see cref="FileExistsAction.Overwrite"/> and <see cref="FileExistsAction.Rename"/> will not be affected by this setting.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFail { get; set; }
}
