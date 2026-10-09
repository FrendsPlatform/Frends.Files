namespace Frends.Files.Rename.Definitions;

/// <summary>
/// Rename result class.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the rename completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Full path to the file. Null when Success is false.
    /// </summary>
    /// <example>c:\temp\foo.txt</example>
    public string Path { get; private set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }

    internal Result(bool success, string path, Error error = null)
    {
        Success = success;
        Path = path;
        Error = error;
    }
}
