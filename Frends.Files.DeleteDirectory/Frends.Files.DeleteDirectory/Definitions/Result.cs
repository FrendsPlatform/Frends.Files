namespace Frends.Files.DeleteDirectory.Definitions;

/// <summary>
/// Result class
/// </summary>
public class Result
{
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

    internal Result(string path, bool success)
    {
        Path = path;
        Success = success;
    }
}
