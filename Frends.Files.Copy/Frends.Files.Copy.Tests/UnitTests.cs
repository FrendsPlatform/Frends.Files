using Frends.Files.Copy.Definitions;
using Frends.Files.Copy.Helpers;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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
            IfTargetFileExists = FileExistsAction.Throw,
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

        ClassicAssert.AreEqual(3, result.Files.Count);
    }

    [Test]
    public async Task FileCopyContinuesOnFailureAndReportsFailedFiles()
    {
        var fileNames = FilesHandler.FindMatchingFiles(_SourceDir, _input.Pattern).ToList();
        var failedNames = fileNames.Take(2).ToList();
        _options.ContinueOnFailure = true;

        foreach (var name in failedNames)
            File.Copy(Path.Combine(_SourceDir, name), Path.Combine(_TargetDir, name));

        var result = await Files.Copy(_input, _options, default);

        ClassicAssert.IsTrue(result.Success);
        ClassicAssert.IsNull(result.Error);
        ClassicAssert.AreEqual(fileNames.Count - failedNames.Count, result.Files.Count);
        CollectionAssert.AreEquivalent(failedNames.Select(name => Path.Combine(_SourceDir, name)),
            result.FailedFiles.Select(file => file.SourcePath));
        Assert.That(result.FailedFiles.All(file => file.Exception is IOException), Is.True);
        Assert.That(result.Files.All(file => File.Exists(file.TargetPath)), Is.True);
    }

    [Test]
    public async Task FileCopyStopsOnFailureAndListsUnprocessedFiles()
    {
        var fileNames = FilesHandler.FindMatchingFiles(_SourceDir, _input.Pattern).ToList();
        var firstName = fileNames[0];
        File.Copy(Path.Combine(_SourceDir, firstName), Path.Combine(_TargetDir, firstName));
        _options.ThrowErrorOnFailure = false;

        var result = await Files.Copy(_input, _options, default);

        ClassicAssert.IsFalse(result.Success);
        ClassicAssert.IsEmpty(result.Files);
        Assert.That(result.Error.AdditionalInfo, Is.SameAs(result.FailedFiles[0].Exception));
        CollectionAssert.AreEqual(fileNames.Select(name => Path.Combine(_SourceDir, name)),
            result.FailedFiles.Select(file => file.SourcePath));
        Assert.That(result.FailedFiles[0].Exception, Is.TypeOf<IOException>());
        Assert.That(result.FailedFiles.Skip(1).All(file => file.Exception == null), Is.True);
        Assert.That(fileNames.Skip(1).All(name => !File.Exists(Path.Combine(_TargetDir, name))), Is.True);
    }

    [Test]
    public async Task FileCopyStopsAfterCopyingEarlierFiles()
    {
        var fileNames = FilesHandler.FindMatchingFiles(_SourceDir, _input.Pattern).ToList();
        var failedName = fileNames[1];
        File.Copy(Path.Combine(_SourceDir, failedName), Path.Combine(_TargetDir, failedName));
        _options.ThrowErrorOnFailure = false;
        _options.ErrorMessageOnFailure = "Copy failed";

        var result = await Files.Copy(_input, _options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Files.Select(file => file.SourcePath),
            Is.EqualTo(new[] { Path.Combine(_SourceDir, fileNames[0]) }));
        Assert.That(result.FailedFiles.Select(file => file.SourcePath),
            Is.EqualTo(fileNames.Skip(1).Select(name => Path.Combine(_SourceDir, name))));
        Assert.That(result.Error.Message, Does.StartWith("Copy failed: "));
        Assert.That(result.Error.AdditionalInfo, Is.SameAs(result.FailedFiles[0].Exception));
        Assert.That(fileNames.Skip(2).All(name => !File.Exists(Path.Combine(_TargetDir, name))), Is.True);
    }

    [Test]
    public void FileCopyDefaultOptionsThrowOriginalFileException()
    {
        var firstName = FilesHandler.FindMatchingFiles(_SourceDir, _input.Pattern).First();
        File.Copy(Path.Combine(_SourceDir, firstName), Path.Combine(_TargetDir, firstName));

        var ex = Assert.ThrowsAsync<IOException>(() => Files.Copy(_input, _options, default));

        Assert.That(ex!.Message, Does.Contain(firstName));
    }
}
