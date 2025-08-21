namespace Frends.Files.Move.Definitions;

/// <summary>
/// Defines the types of remote path configurations for file operations.
/// Used to identify which path requires remote credentials and impersonation.
/// </summary>
public enum RemotePathType
{
    /// <summary>
    /// Both source and target paths are local. No remote credentials needed.
    /// </summary>
    None,

    /// <summary>
    /// The source path is remote. Remote credentials will be used for reading from source.
    /// </summary>
    Source,

    /// <summary>
    /// The target path is remote. Remote credentials will be used for writing to target.
    /// </summary>
    Target
}
