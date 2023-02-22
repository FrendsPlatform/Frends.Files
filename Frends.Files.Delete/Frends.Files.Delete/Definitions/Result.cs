using System.Collections.Generic;

namespace Frends.Files.Delete.Definitions;

/// <summary>
/// Backup and cleanup results.
/// </summary>
public class Result
{
    /// <summary>
    /// List of file items deleted from directory.
    /// </summary>
    /// <example>[test.txt, test2.txt]</example>
    public List<FileItem> Files { get; private set; }

    internal Result(List<FileItem> files)
    {
        Files = files;
    }
}