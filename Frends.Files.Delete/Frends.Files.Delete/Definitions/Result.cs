using System.Collections.Generic;

namespace Frends.Files.Delete.Definitions;

/// <summary>
/// Backup and cleanup results.
/// </summary>
public class Result
{
    internal Result(bool success, List<FileItem> files, Error error = null)
    {
        Success = success;
        Files = files;
        Error = error;
    }

    /// <summary>
    /// Indicates whether the task completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// List of file items deleted from directory.
    /// </summary>
    /// <example>[test.txt, test2.txt]</example>
    public List<FileItem> Files { get; private set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }
}
