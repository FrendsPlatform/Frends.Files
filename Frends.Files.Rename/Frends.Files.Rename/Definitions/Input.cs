using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Files.Rename.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Full path to the file to be renamed.
    /// </summary>
    /// <example>c:\root\folder\example.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("\"c:\\temp\\foo.txt\"")]
    public string Path { get; set; }

    /// <summary>
    /// The new filename including extension
    /// </summary>
    /// <example>newName.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("\"bar.txt\"")]
    public string NewFileName { get; set; }
}
