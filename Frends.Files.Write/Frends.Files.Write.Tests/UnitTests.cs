using Frends.Files.Write.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Frends.Files.Write.Tests;

[TestFixture]
public class UnitTests
{
    private static readonly string _FullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/test.txt"));
    private Input _input = new();
    private Options _options = new();

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
        ClassicAssert.IsTrue(File.Exists(result.Path));
        ClassicAssert.AreEqual(Math.Round(File.ReadAllText(_FullPath).Length / 1024d / 1024d, 3), result.SizeInMegaBytes);
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
        ClassicAssert.IsTrue(File.Exists(result.Path));
        ClassicAssert.AreEqual(Math.Round(File.ReadAllText(_FullPath).Length / 1024d / 1024d, 3), result.SizeInMegaBytes);
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
        ClassicAssert.IsTrue(File.Exists(result.Path));
        ClassicAssert.AreEqual(Math.Round(File.ReadAllText(_FullPath).Length / 1024d / 1024d, 3), result.SizeInMegaBytes);
    }

    [Test]
    public async Task FilesWriteEncodeWindows1252()
    {
        await Files.Write(_input, _options);

        var options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            WriteBehaviour = WriteBehaviour.Overwrite,
            FileEncoding = FileEncoding.Windows1252,
            EnableBom = true
        };

        var result = await Files.Write(_input, options);
        ClassicAssert.IsTrue(File.Exists(result.Path));
        ClassicAssert.AreEqual(Math.Round(File.ReadAllText(_FullPath).Length / 1024d / 1024d, 3), result.SizeInMegaBytes);
    }

    [Test]
    public async Task FilesWriteShouldThrowIfFileExists()
    {
        await Files.Write(_input, _options);

        var ex = Assert.ThrowsAsync<IOException>(() => Files.Write(_input, _options));
        ClassicAssert.AreEqual($"File already exists: {_FullPath}.", ex.Message);
    }

    [Test]
    public void FilesWriteShouldThrowIfDirectoryNotExists()
    {
        var input = new Input
        {
            Content = "test",
            Path = @"f:\path\not\exist",
        };
        var ex = Assert.ThrowsAsync<DirectoryNotFoundException>(() => Files.Write(input, _options));
        ClassicAssert.AreEqual($"Could not find a part of the path '{input.Path}'.", ex.Message);
    }

    [Test]
    public async Task FilesWrite_ReturnsCorrectFileSizes()
    {
        var result = await Files.Write(_input, _options);

        ClassicAssert.IsTrue(File.Exists(result.Path), "File should exist after write.");

        var info = new FileInfo(_FullPath);

        var expectedBytes = info.Length;
        var expectedKb = Math.Round(expectedBytes / 1024d, 3);
        var expectedMb = Math.Round(expectedBytes / 1024d / 1024d, 3);

        ClassicAssert.AreEqual(expectedBytes, result.SizeInBytes);
        ClassicAssert.AreEqual(expectedKb, result.SizeInKiloBytes);
        ClassicAssert.AreEqual(expectedMb, result.SizeInMegaBytes);
    }
}
