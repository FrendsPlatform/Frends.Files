using Frends.Files.Move.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Frends.Files.Move.Tests;

[TestFixture]
class ImpersonationTests
{
    /// <summary>
    /// Impersonation tests needs to be run as administrator so that the OneTimeSetup can create a local test user. Impersonation tests can only be run in Windows OS.
    /// </summary>
    private static readonly string _SourceDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/");
    private static readonly string _TargetDir = Path.Combine(_SourceDir, "destination");
    Input? _input;
    Options? _options;

    private readonly string _domain = Environment.MachineName;
    private readonly string _name = "test";
    private readonly string _pwd = "pas5woRd!";


    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Helper.CreateTestUser(_domain, _name, _pwd);

        _input = new Input
        {
            Directory = _SourceDir,
            Pattern = "*",
            TargetDirectory = _TargetDir,
        };

        _options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = true,
            UserName = $"{_domain}\\{_name}",
            Password = _pwd
        };
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Helper.DeleteTestUser(_name);
    }

    [SetUp]
    public void Setup()
    {
        Helper.CreateTestFiles(_SourceDir);
        Directory.CreateDirectory(_TargetDir);
    }

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(_SourceDir);
    }

    [Test]
    public async Task FileMoveTestWithCredentials()
    {
        var result = await Files.Move(
            _input,
            _options, default);

        Assert.AreEqual(7, result.Files.Count);
        Assert.IsTrue(File.Exists(result.Files[0].TargetPath));
        Assert.IsFalse(File.Exists(result.Files[0].SourcePath));
    }

    [Test]
    public void FileMoveTestWithUsernameWithoutDomain()
    {
        var options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = true,
            UserName = "test",
            Password = _pwd
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(() => Files.Move(_input, options, default));
        Assert.AreEqual($@"UserName field must be of format domain\username was: {options.UserName}", ex.Message);
    }
}
