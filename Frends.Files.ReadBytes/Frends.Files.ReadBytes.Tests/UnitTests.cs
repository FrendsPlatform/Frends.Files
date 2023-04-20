using Frends.Files.ReadBytes.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Frends.Files.ReadBytes.Tests;

[TestFixture]
public class UnitTests
{
    private readonly string _root = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestFiles"));

    [Test]
    public async Task ReadFileContent()
    {
        var binaryTestFilePath = Path.Combine(_root, "frends_favicon.png");
        var result = await Files.ReadBytes(new Input() { Path = binaryTestFilePath }, new Options() { UseGivenUserCredentialsForRemoteConnections = false  }, default);

        var expectedData = File.ReadAllBytes(binaryTestFilePath);

        Assert.AreEqual(expectedData.Length, result.ContentBytes.Length);
        Assert.AreEqual(expectedData, result.ContentBytes);
    }
}
