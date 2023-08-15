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

    internal Result(string sourcePath, string targetPath)
    {
        SourcePath = sourcePath;
        TargetPath = targetPath;
    }
}
