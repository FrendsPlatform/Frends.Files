using Frends.Files.LocalBackup.Helpers;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Frends.Files.LocalBackup.Tests;

[TestFixture]
public class FileHandlerTests
{
    [TestCase(null, "*")]
    [TestCase("", "*")]
    [TestCase("file.txt", null)]
    [TestCase("file.txt", "")]
    public void FileMatchesMaskShouldReturnFalseWhenInputIsNullOrEmpty(string? filePath, string? mask)
    {
        ClassicAssert.IsFalse(FilesHandler.FileMatchesMask(filePath!, mask!));
    }

    [Test]
    public void FileMatchesMaskShouldMatchGlobPatternsCaseInsensitively()
    {
        ClassicAssert.IsTrue(FilesHandler.FileMatchesMask("Folder\\Item.TXT", @"folder/*.txt"));
    }

    [Test]
    public void FileMatchesMaskShouldNormalizeBackslashesInBothPathAndMask()
    {
        ClassicAssert.IsTrue(FilesHandler.FileMatchesMask(@"Folder\Sub\Item.txt", @"Folder\Sub\*.txt"));
    }

    [Test]
    public void FileMatchesMaskShouldUseRegexModeForRegexMasks()
    {
        ClassicAssert.IsTrue(FilesHandler.FileMatchesMask(@"Folder\match_test.txt", "<regex>^Folder/.+_test\\.txt$"));
    }

    [Test]
    public void FileMatchesMaskShouldReturnFalseForNonMatchingRegexMasks()
    {
        ClassicAssert.IsFalse(FilesHandler.FileMatchesMask("Folder/item.txt", "<regex>^Other/.+\\.txt$"));
    }
}
