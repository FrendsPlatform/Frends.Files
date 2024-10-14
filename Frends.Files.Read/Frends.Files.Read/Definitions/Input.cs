using System.ComponentModel.DataAnnotations;

namespace Frends.Files.Read.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Full path of the target file to be read.
    /// </summary>
    /// <example>c:\temp\foo.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Path { get; set; }
}
