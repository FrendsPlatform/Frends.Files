namespace Frends.Files.Rename.Definitions;

/// <summary>
/// Rename result class.
/// </summary>
public class Result
{
    /// <summary>
    /// Full path to the file.
    /// </summary>
    /// <example>c:\temp\foo.txt</example>
    public string Path { get; private set; }

    internal Result(string path)
    {
        Path = path;
    }
}