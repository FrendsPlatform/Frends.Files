using System.Collections.Generic;

namespace Frends.Files.Find.Definitions;

/// <summary>
/// Find result class.
/// </summary>
public class Result
{
    internal Result(bool success, Error error = null, List<FileItem> files = null)
    {
        Success = success;
        Error = error;
        Files = files ?? [];
    }

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

    /// <summary>
    /// List of files found from directory.
    /// </summary>
    /// <example>List [object { string Extension, string DirectoryName, string FullPath, string FileName, bool IsReadOnly, double SizeInMegaBytes, DateTime CreationTime, DateTime CreationTimeUtc, DateTime LastAccessTime, DateTime LastAccessTimeUtc, DateTime LastWriteTime, DateTime LastWriteTimeUtc }]</example>
    public List<FileItem> Files { get; private set; }
}
