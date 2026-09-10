using System;
using System.IO;
using System.Linq;
using Frends.Files.Copy.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Files.Copy.Tests;

[TestClass]
public class FilesHandlerTests
{
    private string _tempDirectory = string.Empty;

    [TestInitialize]
    public void Setup()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Frends.Files.LocalBackup.Tests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        _tempDirectory = directory;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (!string.IsNullOrWhiteSpace(_tempDirectory) && Directory.Exists(_tempDirectory))
            Directory.Delete(_tempDirectory, true);
    }

    [DataTestMethod]
    [DataRow(null, "*.txt")]
    [DataRow("", "*.txt")]
    [DataRow(" ", "*.txt")]
    [DataRow("file.txt", null)]
    [DataRow("file.txt", "")]
    [DataRow("file.txt", " ")]
    public void FileMatchesMask_ReturnsFalse_ForNullOrWhitespaceValues(string filename, string mask)
    {
        var result = FilesHandler.FileMatchesMask(filename, mask);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void FileMatchesMask_RegexPrefix_MatchesCaseInsensitively()
    {
        var result = FilesHandler.FileMatchesMask("REPORT_2026.XML", @"<regex>^report_\d{4}\.xml$");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void FileMatchesMask_RegexPrefix_InvalidRegex_ReturnsFalse()
    {
        var result = FilesHandler.FileMatchesMask("report.xml", "<regex>[invalid");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void FileMatchesMask_QuestionMarkPattern_ReturnsFalse_WithCurrentMatcherBehavior()
    {
        Assert.IsFalse(FilesHandler.FileMatchesMask("ab1.txt", "ab?.txt"));
        Assert.IsFalse(FilesHandler.FileMatchesMask("ab12.txt", "ab?.txt"));
    }

    [TestMethod]
    public void FileMatchesMask_GlobPattern_MatchesCaseInsensitively()
    {
        Assert.IsTrue(FilesHandler.FileMatchesMask("Test1.XML", "test*.xml"));
        Assert.IsFalse(FilesHandler.FileMatchesMask("Test1.txt", "test*.xml"));
    }

    [TestMethod]
    public void FileMatchesMask_PathLikeMask_DoesNotMatchFilenameOnlyInput()
    {
        const string mask = "/docs/**/foo?.txt";

        Assert.IsFalse(FilesHandler.FileMatchesMask("foo1.txt", mask));
        Assert.IsFalse(FilesHandler.FileMatchesMask("foobar.txt", mask));
    }

    [TestMethod]
    public void FindMatchingFiles_Throws_WhenDirectoryIsMissing()
    {
        var missingDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        Assert.ThrowsExactly<DirectoryNotFoundException>(() => FilesHandler.FindMatchingFiles(missingDirectory, "*"));
    }

    [TestMethod]
    public void FindMatchingFiles_ReturnsRecursiveRelativeMatches_ForWildcardMask()
    {
        Directory.CreateDirectory(Path.Combine(_tempDirectory, "sub"));

        File.WriteAllText(Path.Combine(_tempDirectory, "alpha.txt"), "root");
        File.WriteAllText(Path.Combine(_tempDirectory, "beta.txt"), "root");
        File.WriteAllText(Path.Combine(_tempDirectory, "sub", "alpha.csv"), "sub");
        File.WriteAllText(Path.Combine(_tempDirectory, "sub", "alpha.txt"), "sub");

        var result = FilesHandler.FindMatchingFiles(_tempDirectory, "*alpha*.txt");
        var relativePaths = result.Files.Select(f => f.Path).OrderBy(p => p).ToArray();

        CollectionAssert.AreEqual(
            new[] { "alpha.txt", Path.Combine("sub", "alpha.txt") },
            relativePaths);
    }

    [TestMethod]
    public void FindMatchingFiles_ReturnsSeparatorAgnosticRelativePaths()
    {
        Directory.CreateDirectory(Path.Combine(_tempDirectory, "docs", "bar"));

        File.WriteAllText(Path.Combine(_tempDirectory, "docs", "bar", "foo1.txt"), "ok");
        File.WriteAllText(Path.Combine(_tempDirectory, "docs", "bar", "ignore.xml"), "skip");

        var result = FilesHandler.FindMatchingFiles(_tempDirectory, "foo*.txt");
        var relativePaths = result.Files
            .Select(f => NormalizePathSeparators(f.Path))
            .OrderBy(path => path)
            .ToArray();

        CollectionAssert.AreEqual(new[] { "docs/bar/foo1.txt" }, relativePaths);
    }

    [TestMethod]
    public void FindMatchingFiles_PathLikeMask_ReturnsNoMatches_WhenMaskContainsDirectories()
    {
        Directory.CreateDirectory(Path.Combine(_tempDirectory, "docs", "bar"));
        Directory.CreateDirectory(Path.Combine(_tempDirectory, "docs", "baz", "bazinga"));

        File.WriteAllText(Path.Combine(_tempDirectory, "docs", "bar", "foo1.txt"), "ok");
        File.WriteAllText(Path.Combine(_tempDirectory, "docs", "bar", "foo2.txt"), "ok");
        File.WriteAllText(Path.Combine(_tempDirectory, "docs", "baz", "bazinga", "foo1.txt"), "ok");
        File.WriteAllText(Path.Combine(_tempDirectory, "docs", "bar", "foobar.txt"), "skip");

        var result = FilesHandler.FindMatchingFiles(_tempDirectory, "/docs/**/foo?.txt");

        Assert.AreEqual(0, result.Files.Count());
    }

    [TestMethod]
    public void FindMatchingFiles_ReturnsRegexMatches_AndSkipsNonMatchingFiles()
    {
        string[] expected = ["invoice_1001.xml"];

        File.WriteAllText(Path.Combine(_tempDirectory, "invoice_1001.xml"), "ok");
        File.WriteAllText(Path.Combine(_tempDirectory, "invoice_A.xml"), "skip");
        File.WriteAllText(Path.Combine(_tempDirectory, "report_1001.xml"), "skip");

        var result = FilesHandler.FindMatchingFiles(_tempDirectory, "<regex>^invoice_\\d+\\.xml$");
        var relativePaths = result.Files.Select(f => f.Path).ToArray();

        CollectionAssert.AreEqual(expected, relativePaths);
    }

    [TestMethod]
    public void FindMatchingFiles_ReturnsEmpty_WhenNoFilesMatchMask()
    {
        File.WriteAllText(Path.Combine(_tempDirectory, "alpha.txt"), "data");

        var result = FilesHandler.FindMatchingFiles(_tempDirectory, "*.xml");

        Assert.AreEqual(0, result.Files.Count());
    }

    private static string NormalizePathSeparators(string path) => path.Replace('\\', '/');
}
