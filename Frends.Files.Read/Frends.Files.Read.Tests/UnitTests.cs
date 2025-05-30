using Frends.Files.Read.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Frends.Files.Read.Tests;

[TestFixture]
public class UnitTests
{
    private string _root = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestFiles"));
    private Options _options = new Options();

    [SetUp]
    public void Setup()
    {
        _options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            FileEncoding = FileEncoding.UTF8,
            EnableBom = true
        };
    }

    [Test]
    public async Task ReadFileContent()
    {
        var fileContent = "Well this is content with some extra nice ümlauts: ÄÖåå 你好!";
        Directory.CreateDirectory(Path.Combine(_root, "folder"));
        File.WriteAllText(Path.Combine(_root, "folder", "test.txt"), fileContent);
        var result = await Files.Read(new Input() { Path = Path.Combine(_root, "folder/test.txt") }, _options);
        ClassicAssert.AreEqual(fileContent, result.Content);

        Directory.Delete(Path.Combine(_root, "folder"), true);
    }

    [Test]
    public async Task ReadAnsi()
    {
        var options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            FileEncoding = FileEncoding.Other,
            EncodingInString = "Latin1",
        };
        var result = await Files.Read(new Input() { Path = Path.Combine(_root, "ansi.txt") }, options);

        ClassicAssert.AreEqual(File.ReadAllText(result.Path, Encoding.GetEncoding("Latin1")), result.Content);
    }
}
