using Frends.Files.Delete.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;
using System.Threading.Tasks;

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

        ClassicAssert.AreEqual(7, result.Files.Count);
        ClassicAssert.IsFalse(File.Exists(result.Files[0].Path));
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

        ClassicAssert.AreEqual(2, result.Files.Count);
        ClassicAssert.IsFalse(File.Exists(result.Files[0].Path));
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

        ClassicAssert.IsEmpty(result.Files);
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
        ClassicAssert.AreEqual($"Directory does not exist or you do not have read access. Tried to access directory '{input.Directory}'", ex.Message);
    }

    [Test]
    public void FileDeleteWithRegexPattern()
    {
        var result = Files.Delete(
            new Input
            {
                Directory = _dir,
                Pattern = "<regex>^(?!prof).*_test.txt$",
            }, _options, default);

        ClassicAssert.AreEqual(3, result.Files.Count);
        ClassicAssert.IsFalse(File.Exists(result.Files[0].Path));
        ClassicAssert.IsFalse(File.Exists(result.Files[1].Path));
        ClassicAssert.IsFalse(File.Exists(result.Files[2].Path));
    }
}
