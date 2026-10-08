using System;
using System.IO;
using Frends.Files.Delete.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Frends.Files.Delete.Tests;

[TestFixture]
public class UnitTests
{
    private readonly string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/");
    private Input input = new();
    private Options options = new();

    [SetUp]
    public void Setup()
    {
        Helper.CreateTestFiles(dir);

        input = new Input
        {
            Directory = dir,
            Pattern = "*",
        };

        options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false,
        };
    }

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(dir);
    }

    [Test]
    public void FileDeleteAll()
    {
        var result = Files.Delete(input, options, default);

        ClassicAssert.AreEqual(7, result.Files.Count);
        ClassicAssert.IsFalse(File.Exists(result.Files[0].Path));
    }

    [Test]
    public void FileDeleteWithPattern()
    {
        var result = Files.Delete(
            new Input
            {
                Directory = dir,
                Pattern = "Test1*",
            },
            options,
            default);

        ClassicAssert.AreEqual(2, result.Files.Count);
        ClassicAssert.IsFalse(File.Exists(result.Files[0].Path));
    }

    [Test]
    public void FileDeleteShouldNotThrowIfNoFilesFound()
    {
        var result = Files.Delete(
            new Input()
            {
                Directory = dir,
                Pattern = "**/*.unknown",
            },
            options,
            default);

        ClassicAssert.IsEmpty(result.Files);
    }

    [Test]
    public void FileDeleteShouldThrowIfDirectoryIsNotFound()
    {
        var input = new Input()
        {
            Directory = @"F:\directory\that\dont\exists",
            Pattern = "**/*.unknown",
        };

        var ex = Assert.Throws<DirectoryNotFoundException>(() => Files.Delete(input, options, default));
        ClassicAssert.AreEqual($"Directory does not exist or you do not have read access. Tried to access directory '{input.Directory}'", ex!.Message);
    }

    [Test]
    public void FileDeleteWithRegexPattern()
    {
        var result = Files.Delete(
            new Input
            {
                Directory = dir,
                Pattern = "<regex>^(?!prof).*_test.txt$",
            },
            options,
            default);

        ClassicAssert.AreEqual(3, result.Files.Count);
        ClassicAssert.IsFalse(File.Exists(result.Files[0].Path));
        ClassicAssert.IsFalse(File.Exists(result.Files[1].Path));
        ClassicAssert.IsFalse(File.Exists(result.Files[2].Path));
    }
}
