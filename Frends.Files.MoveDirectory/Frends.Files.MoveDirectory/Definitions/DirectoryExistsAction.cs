namespace Frends.Files.MoveDirectory.Definitions;

/// <summary>
/// Enum class
/// </summary>
public enum DirectoryExistsAction {
    /// <summary>
    /// Throw error on default.
    /// </summary>
    Throw,
    /// <summary>
    /// Create a new directory with a name that appends a number to the end, e.g. "directory(2)".
    /// </summary>
    Rename,
    /// <summary>
    /// Overwrite the target directory, by removing it first before moving the source directory.
    /// </summary>
    Overwrite
}
