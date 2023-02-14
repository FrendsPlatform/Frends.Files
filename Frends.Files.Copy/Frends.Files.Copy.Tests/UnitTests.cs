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
    private static readonly string _SourceDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/"));
    private static readonly string _TargetDir = Path.Combine(_SourceDir, "destination");
    private Input _input = new Input();
    private Options _options = new Options();

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
            UseGivenUserCredentialsForRemoteConnections = false,
            PreserveDirectoryStructure = true,
            CreateTargetDirectories = false,
            IfTargetFileExists = FileExistsAction.Throw
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
    public async Task FileCopyPreserveDirectoryStructure()
    {
        var options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            CreateTargetDirectories = false,
            IfTargetFileExists = FileExistsAction.Throw,
            PreserveDirectoryStructure = true
        };
        var result = await Files.Copy(_input, options, default);

        Assert.AreEqual(7, result.Files.Count);
        Assert.IsTrue(File.Exists(result.Files[0].TargetPath));
    }

    [Test]
    public async Task FileCopyCreateTargetDirectories()
    {
        var options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            CreateTargetDirectories = true,
            IfTargetFileExists = FileExistsAction.Throw,
            PreserveDirectoryStructure = true
        };

        Directory.Delete(_TargetDir, true);

        var result = await Files.Copy(_input, options, default);

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
                TargetDirectory = _TargetDir
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

    [Test]
    public void FileDeleteShouldThrowIfFileExists()
    {
        var testFile = "Test1.txt";
        var input = new Input()
        {
            Directory = _SourceDir,
            Pattern = testFile,
            TargetDirectory = _TargetDir
        };
        File.Copy(Path.Combine(_SourceDir, testFile), Path.Combine(_TargetDir, testFile));
        var ex = Assert.ThrowsAsync<IOException>(() => Files.Copy(input, _options, default));
        Assert.AreEqual($"File '{Path.Combine(_TargetDir, testFile)}' already exists. No files copied.", ex.Message);
    }

    [Test]
    public void FileDeleteShouldThrowIfFileExists2()
    {
        var testFile = "pref_test.txt";
        var input = new Input()
        {
            Directory = _SourceDir,
            Pattern = testFile,
            TargetDirectory = _TargetDir
        };
        File.Copy(Path.Combine(_SourceDir, testFile), Path.Combine(_TargetDir, testFile));
        var ex = Assert.ThrowsAsync<IOException>(() => Files.Copy(input, _options, default));
        Assert.AreEqual($"File '{Path.Combine(_TargetDir, testFile)}' already exists. No files copied.", ex.Message);
    }
}
