using System.Collections.Generic;

namespace Frends.Files.Delete.Definitions;

/// <summary>
/// Backup and cleanup results.
/// </summary>
public class Result
{
    /// <summary>
    /// Path 
    /// </summary>
    public IEnumerable<object> Files { get; private set; }

    internal Result(List<FileItem> files)
    {
        Files = files;
    }
}