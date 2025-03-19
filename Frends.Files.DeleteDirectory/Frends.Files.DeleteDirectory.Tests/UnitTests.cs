using System;
using System.IO;
using Frends.Files.DeleteDirectory.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework.Legacy;
using Assert = NUnit.Framework.Assert;

namespace Frends.Files.DeleteDirectory.Tests;

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
    public void DeleteFolderNotEmptyShouldThrowIfOptionNotSet()
    {
        _context.CreateFiles("temp/foo.txt");
        var ex = Assert.Throws<IOException>(() => Files.DeleteDirectory(new Input() { Directory = Path.Combine(_context.RootPath, "temp") }, new Options()));
        ClassicAssert.IsTrue(ex.Message.Contains("The directory is not empty"));
    }

    [TestMethod]
    public void DeleteFolderNotEmptyShouldNotThrowIfOptionSet()
    {
        _context.CreateFiles("temp/foo.txt", "temp/foo1.txt", "temp/foo2.txt", "temp/foo3.txt", "temp/foo4.txt", "temp/foo5.txt");
        var result = Files.DeleteDirectory(new Input() { Directory = Path.Combine(_context.RootPath, "temp") }, new Options() { DeleteRecursively = true });
        ClassicAssert.AreEqual(result.Path, Path.Combine(_context.RootPath, "temp"));
        ClassicAssert.IsTrue(result.Success);
    }

    [TestMethod]
    public void DeleteFolderShouldNotThrowIfNotExists()
    {
        _context.CreateFiles("temp/foo.txt");
        var result = Files.DeleteDirectory(new Input() { Directory = Path.Combine(_context.RootPath, "temp/whatever") }, new Options());
        ClassicAssert.IsFalse(result.Success, "The error flag should have been set");
    }

    [TestMethod]
    public void DeleteFolderShouldDeleteEmptyDirectory()
    {
        _context.CreateFolder("temp");
        var result = Files.DeleteDirectory(new Input() { Directory = Path.Combine(_context.RootPath, "temp") }, new Options());
        ClassicAssert.AreEqual(result.Path, Path.Combine(_context.RootPath, "temp"));
        ClassicAssert.IsTrue(result.Success);
    }

    [TestMethod]
    public void DeleteFolderShouldDoNothingIfPathDoesNotExists()
    {
        var newPath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var result = Files.DeleteDirectory(new Input() { Directory = newPath }, new Options() { UseGivenUserCredentialsForRemoteConnections = false });
        ClassicAssert.AreEqual(result.Path, newPath);
        ClassicAssert.IsFalse(result.Success);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ThrowUsernameInvalidError()
    {
        var newPath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var result = Files.DeleteDirectory(new Input() { Directory = newPath }, new Options() { UseGivenUserCredentialsForRemoteConnections = true, UserName = "domain/example", Password = "Password123" });
        ClassicAssert.AreEqual("UserName field must be of format domain\\username was: domain/example", result);
    }

    [TestMethod]
    public void ThrowRemoteConnectionError()
    {
        var newPath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var result = Files.DeleteDirectory(new Input() { Directory = newPath }, new Options() { UseGivenUserCredentialsForRemoteConnections = true, UserName = "domain\\example", Password = "Password123" });
        ClassicAssert.AreEqual(result.Path, newPath);
        ClassicAssert.IsFalse(result.Success);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ThrowInputEmpty()
    {
        var result = Files.DeleteDirectory(new Input() { }, new Options() { });
        ClassicAssert.AreEqual("Directory cannot be empty.", result);
    }
}
