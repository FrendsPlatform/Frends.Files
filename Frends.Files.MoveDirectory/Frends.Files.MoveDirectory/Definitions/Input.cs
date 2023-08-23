using System.ComponentModel;

namespace Frends.Files.MoveDirectory.Definitions;

/// <summary>
/// Input class
/// </summary>
public class Input
{

    /// <summary>
    /// Source Directory path.
    /// </summary>
    /// <example>C:\Temp</example>
    [DefaultValue("\"c:\\temp\"")]
    public string SourceDirectory { get; set; }

    /// <summary>
    /// Target Directory path.
    /// </summary>
    /// <example>C:\Temp</example>
    [DefaultValue("\"c:\\temp\"")]
    public string TargetDirectory { get; set; }
}
