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
    public string Extension { get; set; }

    /// <summary>
    /// File directory.
    /// </summary>
    public string DirectoryName { get; set; }

    /// <summary>
    /// Full path of the file.
    /// </summary>
    public string FullPath { get; set; }

    /// <summary>
    /// Name of the file.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Boolean value for is the file only readable.
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Size of the file in mega bytes.
    /// </summary>
    public double SizeInMegaBytes { get; set; }

    /// <summary>
    ///  Local DateTime when file was created.
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// Utc DateTime when the file was created.
    /// </summary>
    public DateTime CreationTimeUtc { get; set; }

    /// <summary>
    /// Local DateTime when file was last accessed.
    /// </summary>
    public DateTime LastAccessTime { get; set; }

    /// <summary>
    /// Utc DateTime when file was last accessed.
    /// </summary>
    public DateTime LastAccessTimeUtc { get; set; }

    /// <summary>
    /// Local DateTime when file was last modified.
    /// </summary>
    public DateTime LastWriteTime { get; set; }

    /// <summary>
    /// Utc DateTime when file was last modified.
    /// </summary>
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

