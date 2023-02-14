using Frends.Files.Move.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Frends.Files.Move.Tests;

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
    public async Task FileDeleteAll()
    {
        var result = await Files.Move(_input, _options, default);

        Assert.AreEqual(7, result.Files.Count);
    }

    [Test]
    public async Task FileDeleteWithPattern()
    {
        var result = await Files.Move(
            new Input
            {
                Directory = _dir,
                Pattern = "Test1*"
            }, _options, default);

        Assert.AreEqual(2, result.Files.Count);
    }

    [Test]
    public async Task FileDeleteShouldNotThrowIfNoFilesFound()
    {
        var result = await Files.Move(
            new Input()
            {
                Directory = _dir,
                Pattern = "**/*.unknown"
            },
            _options,
            default);

        Assert.IsEmpty(result.Files);
    }

    [Test]
    public void FileDeleteShouldThrowIfDirectoryIsNotFound()
    {
        var input = new Input()
        {
            Directory = @"F:\directory\that\dont\exists",
            Pattern = "**/*.unknown"
        };

        var ex = Assert.ThrowsAsync<DirectoryNotFoundException>(() => Files.Move(input, _options, default));
        Assert.AreEqual($"Directory does not exist or you do not have read access. Tried to access directory '{input.Directory}'", ex.Message);
    }
}
