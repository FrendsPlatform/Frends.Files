﻿using System;
using System.IO;
namespace Frends.Files.Read.Definitions;

/// <summary>
/// Write result class.
/// </summary>
public class Result
{
    /// <summary>
    /// File content.
    /// </summary>
    /// <example>This is a test file.</example>
    public string Content { get; private set; }

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
    /// DateTime when file was created.
    /// </summary>
    /// <example>2023-01-31T12:54:17.6431957+02:00</example>
    public DateTime CreationTime { get; private set; }

    /// <summary>
    /// DateTime for last write time of the file.
    /// </summary>
    /// <example>2023-02-06T11:59:13.8696745+02:00</example>
    public DateTime LastWriteTime { get; private set; }

    internal Result(FileInfo info, string content)
    {
        Content = content;
        Path = info.FullName;
        SizeInMegaBytes = Math.Round(info.Length / 1024d / 1024d, 3);
        SizeInKiloBytes = Math.Round(info.Length / 1024d, 3);
        SizeInBytes = info.Length;
        CreationTime = info.CreationTime;
        LastWriteTime = info.LastWriteTime;
    }
}