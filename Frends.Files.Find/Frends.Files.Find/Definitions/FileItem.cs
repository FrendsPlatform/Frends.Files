using System;
using System.IO;

namespace Frends.Files.Find.Definitions;

/// <summary>
/// Class for files for the Result class.
/// </summary>
public class FileItem
{
    /// <summary>
    /// File extension.
    /// </summary>
    /// <example>.txt</example>
    public string Extension { get; set; }

    /// <summary>
    /// File directory.
    /// </summary>
    /// <example>C:\Reports</example>
    public string DirectoryName { get; set; }

    /// <summary>
    /// Full path of the file.
    /// </summary>
    /// <example>C:\Reports\report.txt</example>
    public string FullPath { get; set; }

    /// <summary>
    /// Name of the file.
    /// </summary>
    /// <example>report.txt</example>
    public string FileName { get; set; }

    /// <summary>
    /// Boolean value for is the file only readable.
    /// </summary>
    /// <example>false</example>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Size of the file in mega bytes.
    /// </summary>
    /// <example>1.25</example>
    public double SizeInMegaBytes { get; set; }

    /// <summary>
    ///  Local DateTime when file was created.
    /// </summary>
    /// <example>2025-01-15T10:30:00</example>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// Utc DateTime when the file was created.
    /// </summary>
    /// <example>2025-01-15T10:30:00Z</example>
    public DateTime CreationTimeUtc { get; set; }

    /// <summary>
    /// Local DateTime when file was last accessed.
    /// </summary>
    /// <example>2025-01-15T10:35:00</example>
    public DateTime LastAccessTime { get; set; }

    /// <summary>
    /// Utc DateTime when file was last accessed.
    /// </summary>
    /// <example>2025-01-15T10:35:00Z</example>
    public DateTime LastAccessTimeUtc { get; set; }

    /// <summary>
    /// Local DateTime when file was last modified.
    /// </summary>
    /// <example>2025-01-15T10:30:00</example>
    public DateTime LastWriteTime { get; set; }

    /// <summary>
    /// Utc DateTime when file was last modified.
    /// </summary>
    /// <example>2025-01-15T10:30:00Z</example>
    public DateTime LastWriteTimeUtc { get; set; }

    internal FileItem(FileInfo fileInfo)
    {
        Extension = fileInfo.Extension;
        DirectoryName = fileInfo.DirectoryName;
        FullPath = fileInfo.FullName;
        FileName = fileInfo.Name;
        IsReadOnly = fileInfo.IsReadOnly;
        SizeInMegaBytes = fileInfo.Length / 1024d / 1024d;
        CreationTime = fileInfo.CreationTime;
        CreationTimeUtc = fileInfo.CreationTimeUtc;
        LastAccessTime = fileInfo.LastAccessTime;
        LastAccessTimeUtc = fileInfo.LastAccessTimeUtc;
        LastWriteTime = fileInfo.LastWriteTime;
        LastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
    }
}
