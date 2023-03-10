using Frends.Files.Rename.Definitions;
using NUnit.Framework;
using System;
using System.IO;

namespace Frends.Files.Rename.Tests;

[TestFixture]
public class UnitTests
{
    private static readonly string _FullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/"));
    private Input _input = new Input();
    private Options _options = new Options();

    [SetUp]
    public void Setup()
    {
        _input = new Input
        {
            Path = Path.Combine(_FullPath, "Test1.txt"),
            NewFileName = "NewTestFile.txt"
        };

        _options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            RenameBehaviour = RenameBehaviour.Throw,
        };

        Helper.CreateTestFiles(_FullPath);
    }

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/")));
    }

    [Test]
    public void FilesRenameSimpleRename()
    {
        var result = Files.Rename(_input, _options);
        Assert.IsTrue(File.Exists(result.Path));
    }

    [Test]
    public void FilesRenameBehaviourThrow()
    {
        Files.Rename(_input, _options);

        var ex = Assert.Throws<IOException>(() => Files.Rename(_input, _options));
        Assert.AreEqual($"File already exists {Path.Combine(_FullPath, _input.NewFileName)}. No file renamed.", ex.Message);
    }

    [Test]
    public void FilesRenameBehaviourOverwrite()
    {
        var result = Files.Rename(_input, _options);
        var lastAccessed = File.GetLastAccessTime(result.Path);

        _options.RenameBehaviour = RenameBehaviour.Overwrite;

        result = Files.Rename(_input, _options);
        Assert.IsTrue(File.Exists(result.Path));
        Assert.IsTrue(lastAccessed > File.GetLastAccessTime(result.Path));
    }

    [Test]
    public void FilesRenameBehaviourRename()
    {
        Files.Rename(_input, _options);

        _options.RenameBehaviour = RenameBehaviour.Rename;

        var result = Files.Rename(_input, _options);
        var newFile = Path.GetFileNameWithoutExtension(_input.NewFileName) + "(1)" + Path.GetExtension(_input.NewFileName);
        Assert.AreEqual(newFile, Path.GetFileName(result.Path));
        Assert.IsTrue(File.Exists(Path.Combine(_FullPath, newFile)));
    }

    [Test]
    public void FilesRenameShouldThrowIfDirectoryNotExists()
    {
        _input.Path = @"f:\path\not\exist\Test1.txt";
        _input.NewFileName = "test.txt";

        var ex = Assert.Throws<DirectoryNotFoundException>(() => Files.Rename(_input, _options));
        Assert.AreEqual($"Directory does not exist or you do not have read access. Tried to access directory '{Path.GetDirectoryName(_input.Path)}'.", ex.Message);
    }
}
