using System;
using System.IO;

namespace Frends.Files.Write.Definitions;

/// <summary>
/// Write result class.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets full path to the file.
    /// </summary>
    /// <example>c:\temp\foo.txt</example>
    public string Path { get; private set; }

    /// <summary>
    /// Gets size of the written file in mega bytes.
    /// </summary>
    /// <example>32</example>
    public double SizeInMegaBytes { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="info">FileInfo</param>
    internal Result(FileInfo info)
    {
        Path = info.FullName;
        SizeInMegaBytes = Math.Round(info.Length / 1024d / 1024d, 3);
    }
}