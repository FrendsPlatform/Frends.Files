namespace Frends.Files.DeleteDirectory.Definitions;

/// <summary>
/// Result class
/// </summary>
public class Result
{
    internal Result(string path, bool success, Error error = null)
    {
        Path = path;
        Success = success;
        Error = error;
    }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; private set; }

    /// <summary>
    /// Path of directory
    /// </summary>
    /// <example>C:/User/NewDirectory</example>
    public string Path { get; private set; }

    /// <summary>
    /// Directory found
    /// </summary>
    /// <example>C:/User/NewDirectory</example>
    public bool Success { get; private set; }
}
