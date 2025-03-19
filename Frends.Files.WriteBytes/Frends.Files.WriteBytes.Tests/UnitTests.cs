using Frends.Files.WriteBytes.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Frends.Files.WriteBytes.Tests;

[TestFixture]
public class UnitTests
{
    private static readonly string _FullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/test.txt"));
    private Input _input = new Input();
    private Options _options = new Options();

    [SetUp]
    public void Setup()
    {
        var content = Encoding.ASCII.GetBytes("This is a test file.");
        _input = new Input
        {
            ContentBytes = content,
            Path = _FullPath
        };

        _options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            WriteBehaviour = WriteBehaviour.Throw,
        };

        Directory.CreateDirectory(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/")));
    }

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/")));
    }

    [Test]
    public async Task FilesWriteSimpleWrite()
    {
        var result = await Files.WriteBytes(_input, _options);
        ClassicAssert.IsTrue(File.Exists(result.Path));
        ClassicAssert.AreEqual(Math.Round(File.ReadAllText(_FullPath).Length / 1024d / 1024d, 3), result.SizeInMegaBytes);
    }

    [Test]
    public async Task FilesWriteOverwrite()
    {
        await Files.WriteBytes(_input, _options);

        var options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            WriteBehaviour = WriteBehaviour.Overwrite,
        };
        var result = await Files.WriteBytes(_input, options);
        ClassicAssert.IsTrue(File.Exists(result.Path));
        ClassicAssert.AreEqual(Math.Round(File.ReadAllText(_FullPath).Length / 1024d / 1024d, 3), result.SizeInMegaBytes);
    }

    [Test]
    public async Task FilesWriteAppend()
    {
        await Files.WriteBytes(_input, _options);

        var options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            WriteBehaviour = WriteBehaviour.Append,
        };

        var result = await Files.WriteBytes(_input, options);
        ClassicAssert.IsTrue(File.Exists(result.Path));
        ClassicAssert.AreEqual(Math.Round(File.ReadAllText(_FullPath).Length / 1024d / 1024d, 3), result.SizeInMegaBytes);
    }

    [Test]
    public async Task FilesWriteShouldThrowIfFileExists()
    {
        await Files.WriteBytes(_input, _options);

        var ex = Assert.ThrowsAsync<IOException>(() => Files.WriteBytes(_input, _options));
        ClassicAssert.AreEqual($"File already exists: {_FullPath}.", ex.Message);
    }

    [Test]
    public void FilesWriteShouldThrowIfDirectoryNotExists()
    {
        _input.Path = @"f:\path\not\exist";

        var ex = Assert.ThrowsAsync<DirectoryNotFoundException>(() => Files.WriteBytes(_input, _options));
        ClassicAssert.AreEqual($"Could not find a part of the path '{_input.Path}'.", ex.Message);
    }
}
