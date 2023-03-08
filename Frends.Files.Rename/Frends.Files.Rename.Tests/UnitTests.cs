using Frends.Files.Rename.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Frends.Files.Rename.Tests;

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
            Path = _FullPath,
            NewFileName = "NewTestFile.txt"
        };

        _options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            RenameBehaviour = RenameBehaviour.Throw,
        };
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

    }

    [Test]
    public void FilesRenameBehaviourOverwrite()
    {
        Files.Rename(_input, _options);

        var options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            RenameBehaviour = RenameBehaviour.Overwrite,
        };
        var result = Files.Rename(_input, options);
    }

    [Test]
    public void FilesRenameBehaviourRename()
    {
        Files.Rename(_input, _options);

        var options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
            RenameBehaviour = RenameBehaviour.Rename,
        };

        var result = Files.Rename(_input, options);
    }

    [Test]
    public void FilesRenameShouldThrowIfDirectoryNotExists()
    {
        var input = new Input
        {
            Path = @"f:\path\not\exist",
            NewFileName = "test.txt"
        };
        var ex = Assert.Throws<DirectoryNotFoundException>(() => Files.Rename(input, _options));
        Assert.AreEqual($"Could not find a part of the path '{input.Path}'.", ex.Message);
    }
}
