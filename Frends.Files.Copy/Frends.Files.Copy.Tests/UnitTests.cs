using Frends.Files.Copy.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;
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
            IfTargetFileExists = FileExistsAction.Throw,
            ThrowErrorOnFail = true,
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

        ClassicAssert.AreEqual(7, result.Files.Count);
        ClassicAssert.IsTrue(File.Exists(result.Files[0].TargetPath));
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

        ClassicAssert.AreEqual(7, result.Files.Count);
        ClassicAssert.IsTrue(File.Exists(result.Files[0].TargetPath));
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

        ClassicAssert.AreEqual(7, result.Files.Count);
        ClassicAssert.IsTrue(File.Exists(result.Files[0].TargetPath));
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

        ClassicAssert.AreEqual(2, result.Files.Count);
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

        ClassicAssert.IsEmpty(result.Files);
    }

    [Test]
    public void FileCopyShouldThrowIfDirectoryIsNotFound()
    {
        var input = new Input()
        {
            Directory = @"F:\directory\that\dont\exists",
            Pattern = "**/*.unknown",
            TargetDirectory = _TargetDir
        };

        var ex = Assert.ThrowsAsync<DirectoryNotFoundException>(() => Files.Copy(input, _options, default));
        ClassicAssert.AreEqual($"Directory does not exist or you do not have read access. Tried to access directory '{input.Directory}'", ex!.Message);
    }

    [Test]
    public void FileCopyShouldThrowIfFileExists()
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
        ClassicAssert.AreEqual($"File '{Path.Combine(_TargetDir, testFile)}' already exists. No files copied.", ex!.Message);
    }

    [Test]
    public void FileCopyShouldThrowIfFileExists2()
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
        ClassicAssert.AreEqual($"File '{Path.Combine(_TargetDir, testFile)}' already exists. No files copied.", ex!.Message);
    }

    [Test]
    public async Task FileCopyWithRegexPattern()
    {
        var result = await Files.Copy(
            new Input
            {
                Directory = _SourceDir,
                Pattern = "<regex>^(?!prof).*_test.txt$",
                TargetDirectory = _TargetDir
            },
            _options,
            default
        );

        Assert.AreEqual(3, result.Files.Count);
    }

    [Test]
    public async Task FileCopyShouldNotThrowIfThrowErrorOnFailIsFalse()
    {
        var testFile = "prof_test.txt";

        var options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            PreserveDirectoryStructure = true,
            CreateTargetDirectories = false,
            IfTargetFileExists = FileExistsAction.Throw,
            ThrowErrorOnFail = false,
        };

        File.Copy(Path.Combine(_SourceDir, testFile), Path.Combine(_TargetDir, testFile));

        var result = await Files.Copy(_input, options, default);

        Assert.IsTrue(File.Exists(result.Files[0].TargetPath));
        Assert.AreEqual(1, result.FailedFiles.Count);
        Assert.AreEqual(Path.Combine(_SourceDir, testFile), result.FailedFiles[0].SourcePath);
    }
}
