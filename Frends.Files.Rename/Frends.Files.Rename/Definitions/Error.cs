using System;

namespace Frends.Files.Rename.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Directory does not exist or you do not have read access.</example>
    public string Message { get; set; }

    /// <summary>
    /// The exception that caused the error.
    /// </summary>
    /// <example>System.IO.DirectoryNotFoundException: Directory does not exist</example>
    public Exception AdditionalInfo { get; set; }
}
