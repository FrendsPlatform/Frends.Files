using System;
using System.IO;

namespace Frends.Files.Write.Definitions;

/// <summary>
/// Write result class.
/// </summary>
public class Result
{
    internal Result(bool success, FileInfo info = null, Error error = null)
    {
        Success = success;
        Error = error;
        if (info == null) return;
        Path = info.FullName;
        SizeInMegaBytes = Math.Round(info.Length / 1024d / 1024d, 3);
        SizeInKiloBytes = Math.Round(info.Length / 1024d, 3);
        SizeInBytes = info.Length;
    }

    /// <summary>
    /// Indicates whether the write completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

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

    /// <summary>
    /// Size of the written file in kilo bytes.
    /// </summary>
    /// <example>32</example>
    public double SizeInKiloBytes { get; private set; }

    /// <summary>
    /// Size of the written file in bytes.
    /// </summary>
    /// <example>32</example>
    public double SizeInBytes { get; private set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }
}
