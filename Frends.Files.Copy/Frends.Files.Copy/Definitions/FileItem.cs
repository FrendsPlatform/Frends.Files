namespace Frends.Files.Copy.Definitions;

/// <summary>
/// Class for return items.
/// </summary>
public class FileItem
{
    /// <summary>
    /// Source path to the file.
    /// </summary>
    /// <example>C:\data\report.csv</example>
    public string SourcePath { get; set; }

    /// <summary>
    /// Target path to the file.
    /// </summary>
    /// <example>D:\backup\report.csv</example>
    public string TargetPath { get; set; }

    internal FileItem(string source, string target)
    {
        SourcePath = source;
        TargetPath = target;
    }
}
