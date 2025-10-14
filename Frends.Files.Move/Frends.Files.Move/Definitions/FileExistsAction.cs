namespace Frends.Files.Move.Definitions;

/// <summary>
/// Action to be executed when the file exits in destination.
/// </summary>
public enum FileExistsAction
{
    /// <summary>
    /// Throw an error and roll back all transfers.
    /// </summary>
    Throw = 0,

    /// <summary>
    /// Overwrite the target file.
    /// </summary>
    Overwrite = 1,

    /// <summary>
    /// Rename the transferred file by appending a number to the end.
    /// </summary>
    Rename = 2
}