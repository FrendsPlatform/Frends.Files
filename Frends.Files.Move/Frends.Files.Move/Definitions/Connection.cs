using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Files.Move.Definitions;

/// <summary>
/// Connection parameters to set file access permissions.
/// </summary>
public class Connection
{
    /// <summary>
    /// Defines if the source path is on a remote host.
    /// Note: This feature is only possible with Windows agents.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool SourceIsRemote { get; set; }

    /// <summary>
    /// This needs to be of format domain\username
    /// </summary>
    /// <example>WORKGROUP\John</example>
    [DefaultValue("\"domain\\username\"")]
    [UIHint(nameof(SourceIsRemote), "", true)]
    public string SourceUserName { get; set; }

    /// <summary>
    /// Password for the used credentials.
    /// </summary>
    /// <example>test</example>
    [PasswordPropertyText]
    [UIHint(nameof(SourceIsRemote), "", true)]
    public string SourcePassword { get; set; }

    /// <summary>
    /// Defines if the target path is on a remote host.
    /// Note: This feature is only possible with Windows agents.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool TargetIsRemote { get; set; }

    /// <summary>
    /// This needs to be of format domain\username
    /// </summary>
    /// <example>domain\username</example>
    [DefaultValue("\"domain\\username\"")]
    [UIHint(nameof(TargetIsRemote), "", true)]
    public string TargetUserName { get; set; }

    /// <summary>
    /// Password for the used credentials.
    /// </summary>
    /// <example>test</example>
    [PasswordPropertyText]
    [UIHint(nameof(TargetIsRemote), "", true)]
    public string TargetPassword { get; set; }
}