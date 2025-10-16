using Frends.Files.Move.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace Frends.Files.Move.Tests;

[TestFixture]
public class UnitTests
{
    private static readonly string SourceDir =
        Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/"));

    private static readonly string TargetDir = Path.Combine(SourceDir, "destination");
    private Input _input = new();
    private Options _options = new();

    [SetUp]
    public void Setup()
    {
        Helper.CreateTestFiles(SourceDir);
        Directory.CreateDirectory(TargetDir);

        _input = new Input
        {
            SourceDirectory = SourceDir,
            Pattern = "*",
            TargetDirectory = TargetDir
        };

        _options = new Options
        {
            CreateTargetDirectories = false,
            IfTargetFileExists = FileExistsAction.Throw,
            PreserveDirectoryStructure = true,
        };
    }

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(SourceDir);
    }

    [Test]
    public async Task FileMoveAll()
    {
        var result = await Files.Move(_input, new Connection(), _options, CancellationToken.None);

        ClassicAssert.AreEqual(7, result.Files.Count);
        ClassicAssert.IsTrue(File.Exists(result.Files[0].TargetPath));
        ClassicAssert.IsFalse(File.Exists(result.Files[0].SourcePath));
    }

    [Test]
    public async Task FileMovePreserveDirectoryStructure()
    {
        var options = new Options
        {
            CreateTargetDirectories = false,
            IfTargetFileExists = FileExistsAction.Throw,
            PreserveDirectoryStructure = true
        };
        var result = await Files.Move(_input, new Connection(), options, CancellationToken.None);

        ClassicAssert.AreEqual(7, result.Files.Count);
        ClassicAssert.IsTrue(File.Exists(result.Files[0].TargetPath));
        ClassicAssert.IsFalse(File.Exists(result.Files[0].SourcePath));
    }

    [Test]
    public async Task FileMoveCreateTargetDirectories()
    {
        var options = new Options
        {
            CreateTargetDirectories = true,
            IfTargetFileExists = FileExistsAction.Throw,
            PreserveDirectoryStructure = true
        };

        Directory.Delete(TargetDir, true);

        var result = await Files.Move(_input, new Connection(), options, CancellationToken.None);

        ClassicAssert.AreEqual(7, result.Files.Count);
        ClassicAssert.IsTrue(File.Exists(result.Files[0].TargetPath));
        ClassicAssert.IsFalse(File.Exists(result.Files[0].SourcePath));
    }

    [Test]
    public async Task FileMoveWithPattern()
    {
        var result = await Files.Move(
            new Input
            {
                SourceDirectory = SourceDir,
                Pattern = "Test1*",
                TargetDirectory = TargetDir
            }, new Connection(), _options, CancellationToken.None);

        ClassicAssert.AreEqual(2, result.Files.Count);
        ClassicAssert.IsTrue(File.Exists(result.Files[0].TargetPath));
        ClassicAssert.IsFalse(File.Exists(result.Files[0].SourcePath));
    }

    [Test]
    public async Task FileMoveShouldNotThrowIfNoFilesFound()
    {
        var result = await Files.Move(
            new Input
            {
                SourceDirectory = SourceDir,
                Pattern = "**/*.unknown",
                TargetDirectory = TargetDir
            }, new Connection(),
            _options,
            CancellationToken.None);

        ClassicAssert.IsEmpty(result.Files);
    }

    [Test]
    public void FileMoveShouldThrowIfDirectoryIsNotFound()
    {
        var input = new Input
        {
            SourceDirectory = @"F:\directory\that\dont\exists",
            Pattern = "**/*.unknown",
            TargetDirectory = TargetDir
        };

        var options = new Options
        {
            ThrowErrorOnFailure = true,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Files.Move(input, new Connection(), options, CancellationToken.None));
        ClassicAssert.AreEqual(
            $"Directory does not exist or you do not have read access. Tried to access directory '{input.SourceDirectory}'",
            ex?.Message);
    }

    [Test]
    public void FileMoveShouldThrowIfFileExists()
    {
        const string testFile = "Test1.txt";
        var input = new Input
        {
            SourceDirectory = SourceDir,
            Pattern = testFile,
            TargetDirectory = TargetDir
        };
        var options = new Options
        {
            ThrowErrorOnFailure = true,
        };

        File.Copy(Path.Combine(SourceDir, testFile), Path.Combine(TargetDir, testFile));
        var ex = Assert.ThrowsAsync<Exception>(() =>
            Files.Move(input, new Connection(), options, CancellationToken.None));
        ClassicAssert.AreEqual($"File '{Path.Combine(TargetDir, testFile)}' already exists. No files moved.",
            ex?.Message);
    }

    [Test]
    public async Task FileMoveWithRegexPattern()
    {
        var result = await Files.Move(
            new Input
            {
                SourceDirectory = SourceDir,
                Pattern = "<regex>^(?!prof).*_test.txt$",
                TargetDirectory = TargetDir
            }, new Connection(), _options, CancellationToken.None);

        ClassicAssert.AreEqual(3, result.Files.Count);
        ClassicAssert.IsTrue(File.Exists(result.Files[0].TargetPath));
        ClassicAssert.IsFalse(File.Exists(result.Files[0].SourcePath));
    }

    [Test]
    public async Task FileMoveTest_RegexPattern_WithNestedDirectories_PreserveStructure()
    {
        var nestedSourceDir = Path.Combine(SourceDir, "SubDir1", "SubDir2");
        Directory.CreateDirectory(nestedSourceDir);
        await File.WriteAllTextAsync(Path.Combine(SourceDir, "lvl0_test_leveled_regex.txt"), "root level");
        await File.WriteAllTextAsync(Path.Combine(SourceDir, "SubDir1", "lvl1_test_leveled_regex.txt"), "level 1");
        await File.WriteAllTextAsync(Path.Combine(nestedSourceDir, "lvl2_test_leveled_regex.txt"), "level 2");

        var input = new Input
        {
            SourceDirectory = SourceDir,
            Pattern = "<regex>.*_test_leveled_regex\\.txt$",
            TargetDirectory = TargetDir
        };
        var options = new Options
        {
            PreserveDirectoryStructure = true,
            CreateTargetDirectories = true,
            IfTargetFileExists = FileExistsAction.Throw
        };

        var result = await Files.Move(input, new Connection(), options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Files.Count, Is.EqualTo(3));
        Assert.That(File.Exists(Path.Combine(TargetDir, "lvl0_test_leveled_regex.txt")), Is.True);
        Assert.That(File.Exists(Path.Combine(TargetDir, "SubDir1", "lvl1_test_leveled_regex.txt")), Is.True);
        Assert.That(File.Exists(Path.Combine(TargetDir, "SubDir1", "SubDir2", "lvl2_test_leveled_regex.txt")), Is.True);
    }
}