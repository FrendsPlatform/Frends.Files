using Frends.Files.Delete.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;


namespace Frends.Files.Delete.Tests;

[TestFixture]
public class UnitTests
{
    private readonly string _dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/");
    Input? _input;
    Options? _options;

    [SetUp]
    public void Setup()
    {
        Helper.CreateTestFiles(_dir);

        _input = new Input
        {
            Directory = _dir,
            Pattern = "*"
        };

        _options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false
        };
    }

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(_dir);
    }

    [Test]
    public void FileDeleteAll()
    {
        var result = Files.Delete(_input, _options, default);

        Assert.AreEqual(7, result.Files.Count);
        Assert.IsFalse(File.Exists(result.Files[0].Path));
    }

    [Test]
    public void FileDeleteWithPattern()
    {
        var result = Files.Delete(
            new Input
            {
                Directory = _dir,
                Pattern = "Test1*"
            }, _options, default);

        Assert.AreEqual(2, result.Files.Count);
        Assert.IsFalse(File.Exists(result.Files[0].Path));
    }

    [Test]
    public void FileDeleteShouldNotThrowIfNoFilesFound()
    {
        var result = Files.Delete(
            new Input()
            {
                Directory = _dir,
                Pattern = "**/*.unknown"
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
            Pattern = "**/*.unknown"
        };

        var ex = Assert.Throws<DirectoryNotFoundException>(() => Files.Delete(input, _options, default));
        Assert.AreEqual($"Directory does not exist or you do not have read access. Tried to access directory '{input.Directory}'", ex.Message);
    }
}
