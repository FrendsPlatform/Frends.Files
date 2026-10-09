using System;

namespace Frends.Files.ReadBytes.Definitions;

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
    /// The original exception.
    /// </summary>
    /// <example>System.IO.FileNotFoundException: Could not find file 'c:\temp\foo.txt'.</example>
    public Exception AdditionalInfo { get; set; }
}
