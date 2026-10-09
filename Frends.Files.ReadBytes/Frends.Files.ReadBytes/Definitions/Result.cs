using System;
using System.IO;
namespace Frends.Files.ReadBytes.Definitions;

/// <summary>
/// Write result class.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// File content.
    /// </summary>
    /// <example>This is a test file.</example>
    public byte[] ContentBytes { get; private set; }

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
    /// DateTime when file was created.
    /// </summary>
    /// <example>2023-01-31T12:54:17.6431957+02:00</example>
    public DateTime CreationTime { get; private set; }

    /// <summary>
    /// DateTime for last write time of the file.
    /// </summary>
    /// <example>2023-02-06T11:59:13.8696745+02:00</example>
    public DateTime LastWriteTime { get; private set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }

    internal Result(bool success, FileInfo info = null, byte[] content = null, Error error = null)
    {
        Success = success;
        Error = error;
        if (info == null) return;

        ContentBytes = content;
        Path = info.FullName;
        SizeInMegaBytes = Math.Round(info.Length / 1024d / 1024d, 3);
        CreationTime = info.CreationTime;
        LastWriteTime = info.LastWriteTime;
    }
}
