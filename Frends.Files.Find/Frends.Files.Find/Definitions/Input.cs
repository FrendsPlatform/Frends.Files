using System.ComponentModel;

namespace Frends.Files.Find.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Root directory where the pattern matching should start
    /// </summary>
    /// <example>C:\directory\</example>
    [DefaultValue("\"c:\\\"")]
    public string Directory { get; set; }

    /// <summary>
    /// Pattern to match relative to Directory. Matching is case-insensitive and separators are normalized to /. For regex mode, prefix the pattern with &lt;regex&gt; and the matcher checks the full relative path and filename. For wildcard mode, * and ** work as glob operators and regex special characters are treated literally.
    /// </summary>
    /// <example>**/Folder/*.xml, Folder/*.txt, report-*.csv, &lt;regex&gt;^(?!prof).*_test.txt</example>
    [DefaultValue("\"**\\Folder\\*.xml\"")]
    public string Pattern { get; set; }
}
