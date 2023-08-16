using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using Frends.Files.MoveDirectory.Definitions;
using NUnit.Framework;

namespace Frends.Files.MoveDirectory.Tests;

[TestClass]
public class UnitTests
{
    private DisposableFileSystem _context;

    [TestInitialize]
    public void Setup()
    {
        _context = new DisposableFileSystem();
    }

    [TestCleanup]
    public void Dispose()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void MoveShouldThrowIfDestinationFolderExistsAndBehaviourIsThrow()
    {
        _context.CreateFiles("temp/foo/bar/foo.txt");
        _context.CreateFiles("temp/bar/foo.txt");
        var sourcePath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var targetPath = Path.Combine(_context.RootPath, "temp\\bar");
        var ex = Assert.Throws<IOException>(() => Files.MoveDirectory(new Input() { SourceDirectory = sourcePath, TargetDirectory = targetPath }, new Options() { IfTargetDirectoryExists = DirectoryExistsAction.Throw }));
        Assert.IsTrue(ex.Message.ToString().Contains("Cannot create a file when that file already exists"));
    }

    [TestMethod]
    public void MoveShouldCreateCopyIfDestinationFolderExistsAndBehaviourIsCopy()
    {
        _context.CreateFiles("temp/foo/bar/foo.txt");
        _context.CreateFiles("temp/bar/foo.txt");
        var sourcePath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var targetPath = Path.Combine(_context.RootPath, "temp\\bar");
        var result = Files.MoveDirectory(new Input() { SourceDirectory = sourcePath, TargetDirectory = targetPath }, new Options() { IfTargetDirectoryExists = DirectoryExistsAction.Rename });
        Assert.AreEqual(result.TargetPath, targetPath + "(1)");
    }

    [Test]
    public void MoveShouldOverwriteIfDestinationFolderExistsAndBehaviourIsOverwrite()
    {
        _context.CreateFiles("temp/foo/bar/foo.txt");
        _context.CreateFiles("temp/bar/foo.txt");
        var sourcePath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var targetPath = Path.Combine(_context.RootPath, "temp\\bar");

        var result = Files.MoveDirectory(new Input() { SourceDirectory = sourcePath, TargetDirectory = targetPath }, new Options() { IfTargetDirectoryExists = DirectoryExistsAction.Overwrite });
        Assert.AreEqual(result.TargetPath, targetPath);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ThrowUsernameInvalidError()
    {
        _context.CreateFiles("temp/foo/bar/foo.txt");
        _context.CreateFiles("temp/bar/foo.txt");
        var sourcePath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var targetPath = Path.Combine(_context.RootPath, "temp\\bar");

        var result = Files.MoveDirectory(new Input() { SourceDirectory = sourcePath, TargetDirectory = targetPath }, new Options() { UseGivenUserCredentialsForRemoteConnections = true, UserName = "domain/example", Password = "Password123" });
        Assert.AreEqual("UserName field must be of format domain\\username was: domain/example", result);
    }

    [TestMethod]
    public void ThrowRemoteConnectionError()
    {
        _context.CreateFiles("temp/foo/bar/foo.txt");
        _context.CreateFiles("temp/bar/foo.txt");
        var sourcePath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var targetPath = Path.Combine(_context.RootPath, "temp\\bar");

        var result = Files.MoveDirectory(new Input() { SourceDirectory = sourcePath, TargetDirectory = targetPath }, new Options() { UseGivenUserCredentialsForRemoteConnections = true, UserName = "domain\\example", Password = "Password123" });
        Assert.AreEqual(result.TargetPath, targetPath);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ThrowInputEmpty()
    {
        var result = Files.MoveDirectory(new Input() { }, new Options() { });
        Assert.AreEqual("Directory cannot be empty.", result);
    }
}
