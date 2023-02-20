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

    internal Result(List<FileItem> files)
    {
        Files = files;
    }
}