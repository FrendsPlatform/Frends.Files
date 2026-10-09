namespace Frends.Files.MoveDirectory.Definitions;

/// <summary>
/// Result class
/// </summary>
public class Result
{
    /// <summary>
    /// Source path of directory
    /// </summary>
    /// <example>C:/User/SourceDirectory</example>
    public string SourcePath { get; private set; }

    /// <summary>
    /// Target path of directory
    /// </summary>
    /// <example>C:/User/TargetDirectory</example>
    public string TargetPath { get; private set; }

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

    internal Result(bool success, string sourcePath, string targetPath, Error error = null)
    {
        Success = success;
        SourcePath = sourcePath;
        TargetPath = targetPath;
        Error = error;
    }
}
