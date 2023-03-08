namespace Frends.Files.Move.Definitions;

/// <summary>
/// Class for return items.
/// </summary>
public class FileItem
{
    /// <summary>
    /// Source path to the file.
    /// </summary>
    public string SourcePath { get; set; }

    /// <summary>
    /// Target path to the file.
    /// </summary>
    public string TargetPath { get; set; }

    internal FileItem(string source, string target)
    {
        SourcePath = source;
        TargetPath = target;
    }
}

