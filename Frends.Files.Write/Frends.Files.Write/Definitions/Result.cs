using System;
using System.IO;
namespace Frends.Files.Write.Definitions;

/// <summary>
/// Write result class.
/// </summary>
public class Result
{
    /// <summary>
    /// Full path to the file.
    /// </summary>
    /// <example>c:\temp\foo.txt</example>
    public string Path { get; private set; }

    /// <summary>
    /// Size of the written file in mega bytes.
    /// </summary>
    /// <example>32</example>
    public double SizeInMegaBytes { get; private set; }

    internal Result(FileInfo info)
    {
        Path = info.FullName;
        SizeInMegaBytes = Math.Round(info.Length / 1024d / 1024d, 3);
    }
}