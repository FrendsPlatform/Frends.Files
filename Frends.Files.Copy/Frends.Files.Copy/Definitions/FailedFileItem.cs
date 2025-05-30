using System;

namespace Frends.Files.Copy.Definitions;

/// <summary>
/// Represents a file that failed to copy.
/// </summary>
public class FailedFileItem
{
    /// <summary>
    /// Path of the source file.
    /// </summary>
    public string SourcePath { get; set; }

    /// <summary>
    /// Exception that caused the copy to fail.
    /// </summary>
    public Exception Exception { get; set; }

    internal FailedFileItem(string sourcePath, Exception exception)
    {
        SourcePath = sourcePath;
        Exception = exception;
    }
}
