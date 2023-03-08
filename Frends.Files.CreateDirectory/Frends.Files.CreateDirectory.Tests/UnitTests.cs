using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using NUnit.Framework;
using Frends.Files.CreateDirectory.Definitions;

namespace Frends.Files.CreateDirectory.Tests;

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
    public void CreateFolderShouldCreateWholePath()
    {
        var newPath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var result = Files.CreateDirectory(new Input() { Directory = newPath }, new Options() { UseGivenUserCredentialsForRemoteConnections = false });
        Assert.AreEqual(result.Path, newPath);
    }

    [TestMethod]
    public void CreateFolderShouldDoNothingIfPathExists()
    {
        _context.CreateFiles("temp/foo/bar/foo.txt");
        var newPath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var result = Files.CreateDirectory(new Input() { Directory = newPath }, new Options() { UseGivenUserCredentialsForRemoteConnections = false });
        Assert.AreEqual(result.Path, newPath);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ThrowUsernameInvalidError()
    {
        var newPath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var result = Files.CreateDirectory(new Input() { Directory = newPath }, new Options() { UseGivenUserCredentialsForRemoteConnections = true, UserName = "domain/example", Password = "Password123" });
        Assert.AreEqual("UserName field must be of format domain\\username was: domain/example", result);
    }

    [TestMethod]
    public void ThrowRemoteConnectionError()
    {
        var newPath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var result = Files.CreateDirectory(new Input() { Directory = newPath }, new Options() { UseGivenUserCredentialsForRemoteConnections = true, UserName = "domain\\example", Password = "Password123" });
        Assert.AreEqual(result.Path, newPath);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ThrowInputEmpty()
    {
        var result = Files.CreateDirectory(new Input() { }, new Options() { });
        Assert.AreEqual("Directory cannot be empty.", result);
    }
}
