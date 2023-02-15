using Frends.Files.Write.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Frends.Files.Write.Tests;

[TestFixture]
class ImpersonationTests
{
    /// <summary>
    /// Impersonation tests needs to be run as administrator so that the OneTimeSetup can create a local test user. Impersonation tests can only be run in Windows OS.
    /// </summary>
    private static readonly string _FullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/test.txt"));
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
            Content = "This is a test file.",
            Path = _FullPath
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

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(Path.GetDirectoryName(_FullPath));
    }

    [Test]
    public async Task FileMoveTestWithCredentials()
    {
        var result = await Files.Write(_input, _options);

        Assert.IsTrue(File.Exists(_FullPath));
        Assert.AreEqual(Math.Round(File.ReadAllText(_FullPath).Length / 1024d / 1024d, 3), result.SizeInMegaBytes);
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

        var ex = Assert.ThrowsAsync<ArgumentException>(() => Files.Write(_input, options));
        Assert.AreEqual($@"UserName field must be of format domain\username was: {options.UserName}", ex.Message);
    }
}
