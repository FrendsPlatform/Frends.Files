using System.ComponentModel.DataAnnotations;

namespace Frends.Files.WriteBytes.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Source directory.
    /// </summary>
    /// <example>VGhpcyBpcyBhIHRlc3QgZmlsZS4=</example>
    [DisplayFormat(DataFormatString = "Text")]
    public object ContentBytes { get; set; }

    /// <summary>
    /// Full path of the target file to be written
    /// </summary>
    /// <example>c:\temp\foo.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Path { get; set; }
}
