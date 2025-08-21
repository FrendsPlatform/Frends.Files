using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Frends.Files.Move.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Specifies which path (source or target) is remote to determine when to use 
    /// remote credentials and Windows impersonation for file operations.
    /// This allows the method to handle local-to-remote, remote-to-local, 
    /// or local-to-local file copy operations appropriately.
    /// </summary>
    /// <example>RemotePathType.Source</example>
    [DefaultValue(RemotePathType.None)]
    public RemotePathType RemotePath { get; set; }

    /// <summary>
    /// This needs to be of format domain\username
    /// </summary>
    /// <example>domain\username</example>
    [DefaultValue("\"domain\\username\"")]
    [UIHint(nameof(RemotePath), "", RemotePathType.Source, RemotePathType.Target)]
    public string UserName { get; set; }

    /// <summary>
    /// Password for the used credentials.
    /// </summary>
    /// <example>testpwd</example>
    [PasswordPropertyText]
    [UIHint(nameof(RemotePath), "", RemotePathType.Source, RemotePathType.Target)]
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
}

