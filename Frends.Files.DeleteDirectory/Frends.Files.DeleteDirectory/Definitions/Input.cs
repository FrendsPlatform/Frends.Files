using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Files.DeleteDirectory.Definitions;

/// <summary>
/// Input class
/// </summary>
public class Input
{
    /// <summary>
    /// Directory path.
    /// </summary>
    /// <example>C:\Temp</example>
    [DefaultValue("\"c:\\temp\"")]
    [Required]
    public string Directory { get; set; }
}
