using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using NUnit.Framework;
using Frends.Files.CreateDirectory.Definitions;

namespace Frends.Files.CreateDirectory.Tests;

[TestClass]
public class UnitTests : IDisposable
{
    private DisposableFileSystem _context;

    [SetUp]
    public void Setup()
    {
        _context = new DisposableFileSystem();
    }
    public void Dispose()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void CreateFolderShouldCreateWholePath()
    {
        var newPath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var result = Files.CreateDirectory(new Input() { Directory = newPath }, new Options() { });
        Assert.That(result.Path, Is.EqualTo(newPath));
    }

    [TestMethod]
    public void CreateFolderShouldDoNothingIfPathExists()
    {
        _context.CreateFiles("temp/foo/bar/foo.txt");
        var newPath = Path.Combine(_context.RootPath, "temp\\foo\\bar");
        var result = Files.CreateDirectory(new Input() { Directory = newPath }, new Options() { });
        Assert.That(result.Path, Is.EqualTo(newPath));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ThrowArgumentEmpty()
    {
        var result = Files.CreateDirectory(new Input() { }, new Options() { });
        Assert.AreEqual("Directory cannot be empty.", result);
    }
}
