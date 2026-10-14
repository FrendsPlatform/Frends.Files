using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
    [Required]
    public string SourceDirectory { get; set; }

    /// <summary>
    /// Target Directory path.
    /// </summary>
    /// <example>C:\Temp</example>
    [DefaultValue("\"c:\\temp\"")]
    [Required]
    public string TargetDirectory { get; set; }
}
