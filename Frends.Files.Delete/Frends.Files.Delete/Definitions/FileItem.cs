using System.IO;

namespace Frends.Files.Delete.Definitions;

/// <summary>
/// Class for return items.
/// </summary>
public class FileItem
{
    /// <summary>
    /// Name of the deleted file.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Full path of the deleted file.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Size of the deleted file in mega bytes.
    /// </summary>
    public double SizeInMegaBytes { get; set; }

    internal FileItem(FileInfo file)
    {
        Name = file.Name;
        Path = file.FullName;
        SizeInMegaBytes = file.Length / 1024d / 1024d;
    }
}

