using System.Collections.Generic;

namespace Frends.Files.Find.Definitions;

/// <summary>
/// Find result class.
/// </summary>
public class Result
{
    /// <summary>
    /// List of files found from directory.
    /// </summary>
    /// <example>[object FileItem, object FileItem]</example>
    public List<FileItem> Files { get; private set; }

    internal Result(List<FileItem> files)
    {
        Files = files;
    }
}