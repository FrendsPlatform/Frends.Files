namespace Frends.Files.CreateDirectory.Definitions;

/// <summary>
/// Result class
/// </summary>
public class Result
{
    /// <summary>
    /// Body of response
    /// </summary>
    /// <example>{"id": "abcdefghijkl123456789",  "success": true,  "errors": []}</example>
    public string Path { get; private set; }

    internal Result(string path)
    {
        Path = path;
    }
}
