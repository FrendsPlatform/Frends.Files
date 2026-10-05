using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Files.LocalBackup.Definitions;

/// <summary>
/// Additional task options.
/// </summary>
public class Options
{
    /// <summary>
    /// Whether to throw an error on failure.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure. If ThrowErrorOnFailure is true, the original exception is wrapped in a new exception with this message.
    /// </summary>
    /// <example>Backup operation failed</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
