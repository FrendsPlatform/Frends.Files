﻿namespace Frends.Files.WriteBytes.Definitions;

/// <summary>
/// Action to be executed when the file exits in destination.
/// </summary>
public enum WriteBehaviour
{
    /// <summary>
    /// Throw an error and roll back all transfers.
    /// </summary>
    Append,

    /// <summary>
    /// Overwrite the target file.
    /// </summary>
    Overwrite,

    /// <summary>
    /// Rename the transferred file by appending a number to the end.
    /// </summary>
    Throw
}
