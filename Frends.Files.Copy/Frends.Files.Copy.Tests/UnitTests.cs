using Frends.Files.Copy.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Frends.Files.Copy.Tests;

[TestFixture]
public class UnitTests
{
    private static readonly string _SourceDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/");
    private static readonly string _TargetDir = Path.Combine(_SourceDir, "destination");
    Input? _input;
    Options? _options;

    [SetUp]
    public void Setup()
    {
        Helper.CreateTestFiles(_SourceDir);
        Directory.CreateDirectory(_TargetDir);

        _input = new Input
        {
            Directory = _SourceDir,
            Pattern = "*",
            TargetDirectory = _TargetDir
        };

        _options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false
        };
    }

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(_SourceDir);
    }

    [Test]
    public async Task FileCopyAll()
    {
        var result = await Files.Copy(_input, _options, default);

        Assert.AreEqual(7, result.Files.Count);
        Assert.IsTrue(File.Exists(result.Files[0].TargetPath));
    }

    [Test]
    public async Task FileCopyWithPattern()
    {
        var result = await Files.Copy(
            new Input
            {
                Directory = _SourceDir,
                Pattern = "Test1*",
                TargetDirectory= _TargetDir
            }, _options, default);

        Assert.AreEqual(2, result.Files.Count);
    }

    [Test]
    public async Task FileCopyShouldNotThrowIfNoFilesFound()
    {
        var result = await Files.Copy(
            new Input()
            {
                Directory = _SourceDir,
                Pattern = "**/*.unknown",
                TargetDirectory = _TargetDir
            },
            _options,
            default);

        Assert.IsEmpty(result.Files);
    }

    [Test]
    public void FileDeleteShouldThrowIfDirectoryIsNotFound()
    {
        var input = new Input()
        {
            Directory = @"F:\directory\that\dont\exists",
            Pattern = "**/*.unknown",
            TargetDirectory = _TargetDir
        };

        var ex = Assert.ThrowsAsync<DirectoryNotFoundException>(() => Files.Copy(input, _options, default));
        Assert.AreEqual($"Directory does not exist or you do not have read access. Tried to access directory '{input.Directory}'", ex.Message);
    }
}
