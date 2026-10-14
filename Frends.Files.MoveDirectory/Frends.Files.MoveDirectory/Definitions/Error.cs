using System;

namespace Frends.Files.MoveDirectory.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Cannot create 'C:/User/TargetDirectory' because a file or directory with the same name already exists.</example>
    public string Message { get; set; }

    /// <summary>
    /// The exception that caused the error.
    /// </summary>
    /// <example>System.IO.IOException</example>
    public Exception AdditionalInfo { get; set; }
}
