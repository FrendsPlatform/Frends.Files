#pragma warning disable CS1591 // self explanatory
#pragma warning disable SA1602 // self explanatory
namespace Frends.Files.Read.Definitions;

/// <summary>
/// File encoding for write operations.
/// </summary>
public enum FileEncoding
{
    UTF8,
    Default,
    ASCII,
    Unicode,
    Windows1252,
    Other,
}