using System.Collections.Generic;

namespace Frends.Files.Find.Definitions;

/// <summary>
/// Find result class.
/// </summary>
public class Result
{
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="success">Whether the operation completed successfully.</param>
    /// <param name="error">Error details when the operation failed.</param>
    /// <param name="files">Files found by the task.</param>
    public Result(bool success, Error error = null, List<FileItem> files = null)
    {
        Success = success;
        Error = error;
        Files = files ?? new List<FileItem>();
    }
}