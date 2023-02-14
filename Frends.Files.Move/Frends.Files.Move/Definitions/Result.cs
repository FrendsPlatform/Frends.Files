using System.Collections.Generic;

namespace Frends.Files.Move.Definitions;

/// <summary>
/// Backup and cleanup results.
/// </summary>
public class Result
{
    /// <summary>
    /// Path 
    /// </summary>
    public List<FileItem> Files { get; private set; }

    internal Result(List<FileItem> files)
    {
        Files = files;
    }
}