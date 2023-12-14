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
    /// <example>List [object { string Extension, string DirectoryName, string FullPath, string FileName, bool IsReadOnly, double SizeInMegaBytes, DateTime CreationTime, DateTime CreationTimeUtc, DateTime LastAccessTime, DateTime LastAccessTimeUtc, DateTime LastWriteTime, DateTime LastWriteTimeUtc }]</example>
    public List<FileItem> Files { get; private set; }

    internal Result(List<FileItem> files)
    {
        Files = files;
    }
}