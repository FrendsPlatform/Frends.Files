using System.IO;

namespace Frends.Files.Delete.Definitions;

internal class FileItem
{
    public string Name { get; set; }

    public string Path { get; set; }

    public double SizeInMegaBytes { get; set; }

    public FileItem(FileInfo file)
    {
        Name = file.Name;
        Path = file.FullName;
        SizeInMegaBytes = file.Length / 1024d / 1024d;
    }
}

