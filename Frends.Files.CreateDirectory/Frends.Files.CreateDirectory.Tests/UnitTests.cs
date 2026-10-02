using Frends.Files.CreateDirectory.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework.Legacy;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using Assert = NUnit.Framework.Assert;

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
        var result = Files.CreateDirectory(new Input() { Directory = newPath }, new Options() { UseGivenUserCredentialsForRemoteConnections = false }, CancellationToken.None);
        ClassicAssert.AreEqual(result.Path, newPath);
        ClassicAssert.IsTrue(result.Success);
    }

    [TestMethod]
    public void CreateFolderShouldDoNothingIfPathExists()
    {
        _context.CreateFiles("temp/foo/bar/foo.txt");
        var newPath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var result = Files.CreateDirectory(new Input() { Directory = newPath }, new Options() { UseGivenUserCredentialsForRemoteConnections = false }, CancellationToken.None);
        ClassicAssert.AreEqual(result.Path, newPath);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ThrowUsernameInvalidError()
    {
        var newPath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var result = Files.CreateDirectory(new Input() { Directory = newPath }, new Options() { UseGivenUserCredentialsForRemoteConnections = true, UserName = "domain/example", Password = "Password123" }, CancellationToken.None);
        ClassicAssert.AreEqual("UserName field must be of format domain\\username was: domain/example", result);
    }

    [TestMethod]
    public void ThrowRemoteConnectionError()
    {
        var newPath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var result = Files.CreateDirectory(new Input() { Directory = newPath }, new Options() { UseGivenUserCredentialsForRemoteConnections = true, UserName = "domain\\example", Password = "Password123" }, CancellationToken.None);
        ClassicAssert.AreEqual(result.Path, newPath);
    }

    [TestMethod]
    [ExpectedException(typeof(ValidationException))]
    public void ThrowInputEmpty()
    {
        var result = Files.CreateDirectory(new Input() { }, new Options() { }, CancellationToken.None);
        ClassicAssert.AreEqual("The Directory field is required", result);
    }
}
