namespace Frends.Files.MoveDirectory.Definitions;

/// <summary>
/// Action to be executed when the directory exits in destination.
/// </summary>
public enum DirectoryExistsAction
{
    /// <summary>
    /// Throw an error
    /// </summary>
    Throw,

    /// <summary>
    /// Overwrite the target directory.
    /// </summary>
    Overwrite,

    /// <summary>
    /// Rename the transferred directory by appending a number to the end.
    /// </summary>
    Rename
}
