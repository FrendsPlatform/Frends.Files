using System;
using System.IO;
using System.Linq;
using Frends.Files.Copy.Helpers;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Frends.Files.Copy.Tests;

[TestFixture]
public class FileHandlerTests
{
    private string rootDir = string.Empty;

    [SetUp]
    public void Setup()
    {
        rootDir = Path.Combine(Path.GetTempPath(), $"FrendsFiles_FileHandlerTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(rootDir);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(rootDir))
            Directory.Delete(rootDir, true);
    }

    [Test]
    public void FindMatchingFilesShouldThrowWhenDirectoryDoesNotExist()
    {
        var missingDir = Path.Combine(rootDir, "missing");

        var ex = Assert.Throws<DirectoryNotFoundException>(() =>
            FilesHandler.FindMatchingFiles(missingDir, "*"));

        ClassicAssert.AreEqual(
            $"Directory does not exist or you do not have read access. Tried to access directory '{missingDir}'",
            ex!.Message);
    }

    [TestCase(null)]
    [TestCase("")]
    public void FindMatchingFilesShouldReturnEmptyWhenMaskIsNullOrEmpty(string? mask)
    {
        CreateFile("a.txt");

        var result = FilesHandler.FindMatchingFiles(rootDir, mask!).ToList();

        ClassicAssert.IsEmpty(result);
    }

    [Test]
    public void FindMatchingFilesShouldMatchGlobPatternCaseInsensitivelyAndReturnRelativePaths()
    {
        CreateFile("Top.TXT");
        CreateFile(Path.Combine("Sub", "nested.xml"));

        var result = FilesHandler.FindMatchingFiles(rootDir, "**/*.txt").ToList();

        ClassicAssert.AreEqual(1, result.Count);
        ClassicAssert.AreEqual("Top.TXT", result[0]);
    }

    [Test]
    public void FindMatchingFilesShouldNormalizeBackslashesInMask()
    {
        CreateFile(Path.Combine("Folder", "item.txt"));

        var result = FilesHandler.FindMatchingFiles(rootDir, @"Folder\*.txt").ToList();

        ClassicAssert.AreEqual(1, result.Count);
        ClassicAssert.AreEqual("Folder/item.txt", result[0]);
    }

    [Test]
    public void FindMatchingFilesShouldUseRegexModeAgainstRelativePath()
    {
        CreateFile(Path.Combine("FolderA", "match_test.txt"));
        CreateFile(Path.Combine("FolderB", "other.txt"));

        var result = FilesHandler.FindMatchingFiles(rootDir, "<regex>^FolderA/.+_test\\.txt$").ToList();

        ClassicAssert.AreEqual(1, result.Count);
        ClassicAssert.AreEqual("FolderA/match_test.txt", result[0]);
    }

    [Test]
    public void FindMatchingFilesShouldUseRegexModeAgainstFileNameToo()
    {
        CreateFile(Path.Combine("X", "named.txt"));
        CreateFile(Path.Combine("Y", "other.dat"));

        var result = FilesHandler.FindMatchingFiles(rootDir, "<regex>^named\\.txt$").ToList();

        ClassicAssert.AreEqual(1, result.Count);
        ClassicAssert.AreEqual("X/named.txt", result[0]);
    }

    [Test]
    public void FindMatchingFilesShouldThrowOnInvalidRegexPattern()
    {
        CreateFile("a.txt");

        Assert.Throws<System.Text.RegularExpressions.RegexParseException>(() =>
            FilesHandler.FindMatchingFiles(rootDir, "<regex>[invalid"));
    }

    private void CreateFile(string relativePath)
    {
        var fullPath = Path.Combine(rootDir, relativePath);
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
        File.WriteAllText(fullPath, "x");
    }
}
