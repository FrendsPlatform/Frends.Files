using System.IO;

namespace Frends.Files.Copy.Definitions;

internal class FileItem
{
    public string SourcePath { get; set; }

    public string TargetPath { get; set; }
    
    public FileItem(string source, string target)
    {
        SourcePath = source;
        TargetPath = target;
    }
}

