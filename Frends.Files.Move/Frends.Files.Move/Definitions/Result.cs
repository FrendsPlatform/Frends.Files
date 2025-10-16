using System.Collections.Generic;

namespace Frends.Files.Move.Definitions;

/// <summary>
/// Move result class.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates if the task completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }

    /// <summary>
    /// List of FileItem objects with source path and target path attributes.
    /// </summary>
    /// <example>[object {SourcePath: C:\test\testfolder\test1.txt, TargetPath: C:\test\moved\test1.txt}, object {SourcePath: C:\test\testfolder\test2.txt, TargetPath: C:\test\moved\test2.txt}]</example>
    public List<FileItem> Files { get; set; } = [];
}