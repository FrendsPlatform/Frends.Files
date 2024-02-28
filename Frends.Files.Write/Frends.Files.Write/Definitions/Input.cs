using System.ComponentModel.DataAnnotations;

namespace Frends.Files.Write.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Gets or sets source directory.
    /// </summary>
    /// <example>This is test content</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Content { get; set; }

    /// <summary>
    /// Gets or sets full path of the target file to be written
    /// </summary>
    /// <example>c:\temp\foo.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Path { get; set; }
}
