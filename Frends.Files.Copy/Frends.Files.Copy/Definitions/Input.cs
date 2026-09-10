using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Frends.Files.Copy.Definitions;
/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Source directory.
    /// </summary>
    /// <example>c:\temp</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Directory { get; set; }

    /// <summary>
    /// Pattern used to select files under Directory.
    /// Use glob syntax by default (for example <c>*.txt</c>, <c>**\*.xml</c>, <c>Folder\*.csv</c>).
    /// If the value starts with <c>&lt;regex&gt;</c>, the remaining text is treated as a regular expression
    /// and matched against the file name only (not the full path).
    /// </summary>
    /// <example>*.txt, **\*.xml, Folder\*.csv, &lt;regex&gt;^(?!prof).*_test\.txt$</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("\"**\\Folder\\*.xml\"")]
    public string Pattern { get; set; }

    /// <summary>
    /// Target directory where the found files should be copied to
    /// </summary>
    /// <example>d:\backup\</example>
    [DefaultValue("\"d:\\backup\"")]
    public string TargetDirectory { get; set; }
}
