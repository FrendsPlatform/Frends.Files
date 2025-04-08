using System.Collections.Generic;

namespace Frends.Files.Copy.Definitions;

/// <summary>
/// Copy result class.
/// </summary>
public class Result
{
    /// <summary>
    /// List of FileItems including source directory and target directory.
    /// </summary>
    /// <example>[object {SourcePath: C:\test\testfolder\test1.txt, TargetPath: C:\test\moved\test1.txt}, object {SourcePath: C:\test\testfolder\test2.txt, TargetPath: C:\test\moved\test2.txt}]</example>
    public List<FileItem> Files { get; private set; }

    /// <summary>
    /// List of FailedItems including path of the source file and failure exception.
    /// This list will always be empty unless <see cref="Options.ThrowErrorOnFail"/> is set to false.
    /// </summary>
    /// <example>[object {SourcePath: C:\test\testfolder\test1.txt, Exception: object {Message: Unable to create 'C:\test\moved' directory}}, object {SourcePath: C:\test\testfolder\test2.txt, Exception: object {Message: File 'C:\test\moved\test2.txt' already exists}}]</example>
    public List<FailedFileItem> FailedFiles { get; private set; }

    internal Result(List<FileItem> files, List<FailedFileItem> failedFiles)
    {
        Files = files;
        FailedFiles = failedFiles;
    }
}