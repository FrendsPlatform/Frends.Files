using System;

namespace Frends.Files.Read.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Could not find file 'c:\temp\foo.txt'.</example>
    public string Message { get; set; }

    /// <summary>
    /// The exception that caused the error.
    /// </summary>
    /// <example>System.IO.FileNotFoundException: Could not find file 'c:\temp\foo.txt'.</example>
    public Exception AdditionalInfo { get; set; }
}
