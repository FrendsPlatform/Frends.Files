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
    /// Pattern to match for files.
    /// </summary>
    /// <example>**\Folder\*.xml</example>
    [DefaultValue("\"**\\Folder\\*.xml\"")]
    public string Pattern { get; set; }
}
