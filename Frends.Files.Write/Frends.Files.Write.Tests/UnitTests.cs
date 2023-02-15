using Frends.Files.Write.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Frends.Files.Write.Tests;

[TestFixture]
public class UnitTests
{
    private static readonly string _FullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/test.txt"));
    private Input _input = new Input();
    private Options _options = new Options();

    [SetUp]
    public void Setup()
    {
        _input = new Input
        {
            Content = "This is a test file.",
            Path = _FullPath
        };

        _options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            WriteBehaviour = WriteBehaviour.Throw,
            FileEncoding = FileEncoding.UTF8,
            EnableBom = true
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
        var result = await Files.Write(_input, _options);
        Assert.IsTrue(File.Exists(result.Path));
        Assert.AreEqual(Math.Round(File.ReadAllText(_FullPath).Length / 1024d / 1024d, 3), result.SizeInMegaBytes);
        
    }

    [Test]
    public async Task FilesWriteOverwrite()
    {
        await Files.Write(_input, _options);

        var options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            WriteBehaviour = WriteBehaviour.Overwrite,
            FileEncoding = FileEncoding.UTF8,
            EnableBom = true
        };
        var result = await Files.Write(_input, options);
        Assert.IsTrue(File.Exists(result.Path));
        Assert.AreEqual(Math.Round(File.ReadAllText(_FullPath).Length / 1024d / 1024d, 3), result.SizeInMegaBytes);
    }

    [Test]
    public async Task FilesWriteAppend()
    {
        await Files.Write(_input, _options);

        var options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            WriteBehaviour = WriteBehaviour.Append,
            FileEncoding = FileEncoding.UTF8,
            EnableBom = true
        };

        var result = await Files.Write(_input, options);
        Assert.IsTrue(File.Exists(result.Path));
        Assert.AreEqual(Math.Round(File.ReadAllText(_FullPath).Length / 1024d / 1024d, 3), result.SizeInMegaBytes);
    }

    [Test]
    public async Task FilesWriteShouldThrowIfFileExists()
    {
        await Files.Write(_input, _options);

        var ex = Assert.ThrowsAsync<IOException>(() => Files.Write(_input, _options));
        Assert.AreEqual($"File already exists: {_FullPath}.", ex.Message);
    }

    [Test]
    public async Task FilesWriteShouldThrowIfDirectoryNotExists()
    {
        var input = new Input
        {
            Content = "test",
            Path = @"f:\path\not\exist",
        };
        var ex = Assert.ThrowsAsync<DirectoryNotFoundException>(() => Files.Write(input, _options));
        Assert.AreEqual($"Could not find a part of the path '{input.Path}'.", ex.Message);
    }
}
