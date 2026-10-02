using System.IO;

namespace Frends.Files.CreateDirectory.Definitions;

/// <summary>
/// Result class
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="success">Whether the operation completed successfully.</param>
    /// <param name="error">Error details when the operation failed.</param>
    /// <param name="path">The path of the created directory.</param>
    public Result(bool success, Error error = null, string path = null)
    {
        Success = success;
        Error = error;
        Path = path;
    }

    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }

    /// <summary>
    /// Path of directory
    /// </summary>
    /// <example>C:/User/NewDirectory</example>
    public string Path { get; private set; }
}
