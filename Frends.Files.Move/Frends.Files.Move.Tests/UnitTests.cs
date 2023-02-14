using Frends.Files.Move.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Frends.Files.Move.Tests;

[TestFixture]
public class UnitTests
{
    private static readonly string _SourceDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/"));
    private static readonly string _TargetDir = Path.Combine(_SourceDir, "destination");
    private Input _input = new Input();
    private Options _options = new Options();

    [SetUp]
    public void Setup()
    {
        Helper.CreateTestFiles(_SourceDir);

        _input = new Input
        {
            Directory = _SourceDir,
            Pattern = "*",
            TargetDirectory = _TargetDir
        };

        _options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            CreateTargetDirectories = false,
            IfTargetFileExists = FileExistsAction.Throw,
            PreserveDirectoryStructure = true,
        };
    }

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(_SourceDir);
    }

    [Test]
    public async Task FileMoveAll()
    {
        var result = await Files.Move(_input, _options, default);

        Assert.AreEqual(7, result.Files.Count);
    }

    [Test]
    public async Task FileMoveWithPattern()
    {
        var result = await Files.Move(
            new Input
            {
                Directory = _SourceDir,
                Pattern = "Test1*",
                TargetDirectory = _TargetDir
            }, _options, default);

        Assert.AreEqual(2, result.Files.Count);
    }

    [Test]
    public async Task FileMoveShouldNotThrowIfNoFilesFound()
    {
        var result = await Files.Move(
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
    public void FileMoveShouldThrowIfDirectoryIsNotFound()
    {
        var input = new Input()
        {
            Directory = @"F:\directory\that\dont\exists",
            Pattern = "**/*.unknown"
        };

        var ex = Assert.ThrowsAsync<DirectoryNotFoundException>(() => Files.Move(input, _options, default));
        Assert.AreEqual($"Directory does not exist or you do not have read access. Tried to access directory '{input.Directory}'", ex.Message);
    }
}
