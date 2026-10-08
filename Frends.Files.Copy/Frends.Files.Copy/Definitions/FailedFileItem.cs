using System;

namespace Frends.Files.Copy.Definitions;

/// <summary>
/// Represents a file that failed to copy.
/// </summary>
public class FailedFileItem
{
    internal FailedFileItem(string sourcePath, Exception exception)
    {
        SourcePath = sourcePath;
        Exception = exception;
    }

    /// <summary>
    /// Path of the source file.
    /// </summary>
    /// <example>C:\data\report.csv</example>
    public string SourcePath { get; set; }

    /// <summary>
    /// Exception that caused the copy to fail. Null if the file was not processed
    /// because copying stopped after an earlier failure.
    /// </summary>
    /// <example>object { Message: Access to the path is denied. }</example>
    public Exception Exception { get; set; }
}
