using Frends.Files.Find.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;


namespace Frends.Files.Find.Tests;

[TestFixture]
public class UnitTests
{
    private static readonly string _FullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/"));
    private Input _input = new();
    private Options _options = new();

    [SetUp]
    public void Setup()
    {
        _input = new Input
        {
            Directory = _FullPath,
            Pattern = @"*"
        };

        _options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false
        };

        Helper.CreateTestFiles(_FullPath);
    }

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/")));
    }

    [Test]
    public void FilesFindAll()
    {
        var result = Files.Find(_input, _options);
        ClassicAssert.AreEqual(7, result.Files.Count);
    }

    [Test]
    public void FilesFindPattern()
    {
        var input = new Input
        {
            Directory = _FullPath,
            Pattern = "*.xml",
        };
        var result = Files.Find(input, _options);
        ClassicAssert.AreEqual(1, result.Files.Count);
    }

    [Test]
    public void FilesFindPatternSubfolders()
    {
        foreach (var file in new DirectoryInfo(_FullPath).GetFiles())
            file.Delete();

        Helper.CreateTestFiles(Path.Combine(_FullPath, "sub"));

        var input = new Input
        {
            Directory = _FullPath,
            Pattern = @"\**\sub\*.txt",
        };

        var result = Files.Find(input, _options);
        ClassicAssert.AreEqual(6, result.Files.Count);
    }

    [Test]
    public void FilesWriteShouldThrowIfDirectoryNotExists()
    {
        var input = new Input
        {
            Directory = @"f:\path\not\exist",
            Pattern = "*",
        };
        var ex = Assert.Throws<DirectoryNotFoundException>(() => Files.Find(input, _options));
        ClassicAssert.AreEqual($"Directory does not exist or you do not have read access. Tried to access directory '{input.Directory}'.", ex.Message);
    }
}
