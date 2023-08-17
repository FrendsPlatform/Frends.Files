using System.ComponentModel;

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
    public string Directory { get; set; }
}
