﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Frends.Files.Read.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// If set, allows you to give the user credentials to use to delete files on remote hosts.
    /// If not set, the agent service user credentials will be used.
    /// Note: This feature is only possible with Windows agents.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(false)]
    public bool UseGivenUserCredentialsForRemoteConnections { get; set; }

    /// <summary>
    /// This needs to be of format domain\username
    /// </summary>
    /// <example>domain\username</example>
    [DefaultValue("\"domain\\username\"")]
    [UIHint(nameof(UseGivenUserCredentialsForRemoteConnections), "", true)]
    public string UserName { get; set; }

    /// <summary>
    /// Password for the used credentials.
    /// </summary>
    /// <example>testpwd</example>
    [PasswordPropertyText]
    [UIHint(nameof(UseGivenUserCredentialsForRemoteConnections), "", true)]
    public string Password { get; set; }

    /// <summary>
    /// Encoding for the written content. By selecting 'Other' you can use any encoding.
    /// </summary>
    /// <example>FileEncoding.UTF8</example>
    [DefaultValue(FileEncoding.UTF8)]
    public FileEncoding FileEncoding { get; set; }

    /// <summary>
    /// Enable BOM for UTF-8
    /// </summary>
    /// <example>true</example>
    [UIHint(nameof(FileEncoding), "", FileEncoding.UTF8)]
    [DefaultValue(false)]
    public bool EnableBom { get; set; }

    /// <summary>
    /// File encoding to be used. A partial list of possible encodings: https://en.wikipedia.org/wiki/Windows_code_page#List
    /// </summary>
    /// <example>ISO-8859-2</example>
    [UIHint(nameof(FileEncoding), "", FileEncoding.Other)]
    public string EncodingInString { get; set; }
}

