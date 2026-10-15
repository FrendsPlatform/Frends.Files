namespace Frends.Files.Write.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Unable to write the file.</example>
    public string Message { get; set; }

    /// <summary>
    /// The exception that caused the error.
    /// </summary>
    /// <example>System.IO.IOException: File already exists</example>
    public object AdditionalInfo { get; set; }
}
