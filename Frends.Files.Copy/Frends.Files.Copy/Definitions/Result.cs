using System.Collections.Generic;

namespace Frends.Files.Copy.Definitions;

/// <summary>
/// Copy result class.
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
    /// List of FileItems including source directory and target directory.
    /// </summary>
    /// <example>[object {SourcePath: C:\test\testfolder\test1.txt, TargetPath: C:\test\moved\test1.txt}, object {SourcePath: C:\test\testfolder\test2.txt, TargetPath: C:\test\moved\test2.txt}]</example>
    public List<FileItem> Files { get; private set; }

    /// <summary>
    /// List of FailedItems including path of the source file and failure exception.
    /// This list will always be empty unless ThrowErrorOnFail is set to false.
    /// </summary>
    /// <example>[object {SourcePath: C:\test\testfolder\test1.txt, Exception: object {Message: Unable to create 'C:\test\moved' directory}}, object {SourcePath: C:\test\testfolder\test2.txt, Exception: object {Message: File 'C:\test\moved\test2.txt' already exists}}]</example>
    public List<FailedFileItem> FailedFiles { get; private set; }

    /// <summary>
    /// Initializes a new result.
    /// </summary>
    /// <param name="success">Whether the operation completed successfully.</param>
    /// <param name="error">Error details when the operation failed.</param>
    /// <param name="files">Files copied successfully.</param>
    /// <param name="failedFiles">Files that failed to copy.</param>
    public Result(bool success, Error error = null, List<FileItem> files = null, List<FailedFileItem> failedFiles = null)
    {
        Success = success;
        Error = error;
        Files = files ?? new List<FileItem>();
        FailedFiles = failedFiles ?? new List<FailedFileItem>();
    }

}