namespace Frends.Files.Move.Definitions;

/// <summary>
/// Class for return items.
/// </summary>
public class FileItem
{
    /// <summary>
    /// Source path to the file.
    /// </summary>
    /// <example>C:\test</example>
    public string SourcePath { get; set; }

    /// <summary>
    /// Target path to the file.
    /// </summary>
    /// <example>\\183.169.59.122\Shared\dst</example>
    public string TargetPath { get; set; }
}