using System;

namespace Frends.Files.WriteBytes.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Could not find a part of the path 'c:\temp\foo.txt'.</example>
    public string Message { get; set; }

    /// <summary>
    /// The exception that caused the error.
    /// </summary>
    /// <example>System.IO.DirectoryNotFoundException: Could not find a part of the path</example>
    public Exception AdditionalInfo { get; set; }
}
