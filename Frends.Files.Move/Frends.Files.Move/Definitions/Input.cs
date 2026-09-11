using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Frends.Files.Move.Definitions;

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
    public string SourceDirectory { get; set; }

    /// <summary>
    /// Pattern to match relative to SourceDirectory. Matching is case-insensitive and separators are normalized to /. For regex mode, prefix the pattern with &lt;regex&gt; and the matcher checks the full relative path and filename. For wildcard mode, * and ** work as glob operators and regex special characters are treated literally.
    /// </summary>
    /// <example>**/Folder/*.xml, Folder/*.txt, report-*.csv, &lt;regex&gt;^(?!prof).*_test.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("\"**\\Folder\\*.xml\"")]
    public string Pattern { get; set; }

    /// <summary>
    /// Target directory where the found files should be copied to
    /// </summary>
    /// <example>\\183.169.59.122\Shared\dst</example>
    [DefaultValue("\"d:\\backup\"")]
    public string TargetDirectory { get; set; }
}