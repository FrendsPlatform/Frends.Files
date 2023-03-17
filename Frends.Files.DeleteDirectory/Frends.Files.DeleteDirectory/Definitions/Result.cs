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

    internal Result(string path)
    {
        Path = path;
    }
}
