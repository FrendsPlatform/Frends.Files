using Frends.Files.Read.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Frends.Files.Read.Tests;

[TestFixture]
class ImpersonationTests
{
    /// <summary>
    /// Impersonation tests needs to be run as administrator so that the OneTimeSetup can create a local test user. Impersonation tests can only be run in Windows OS.
    /// </summary>
    private static readonly string _root = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/"));
    private static Input _input = new Input();
    private static Options _options = new Options();

    private readonly string _domain = Environment.MachineName;
    private readonly string _name = "test";
    private readonly string _pwd = "pas5woRd!";


    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Helper.CreateTestUser(_domain, _name, _pwd);

        _input = new Input
        {
            Path = Path.Combine(_root, "test.txt")
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
        Directory.CreateDirectory(_root);
        var fileContent = "Well this is content with some extra nice ümlauts: ÄÖåå 你好!";
        File.WriteAllText(Path.Combine(_root, "test.txt"), fileContent);
    }

    [TearDown]
    public void TearDown()
    {
        Directory.Delete(_root, true);
    }

    [Test]
    public async Task FileMoveTestWithCredentials()
    {
        var result = await Files.Read(_input, _options);

        Assert.IsTrue(File.Exists(_input.Path));
        Assert.AreEqual(Math.Round(File.ReadAllText(_input.Path).Length / 1024d / 1024d, 3), result.SizeInMegaBytes);
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

        var ex = Assert.ThrowsAsync<ArgumentException>(() => Files.Read(_input, options));
        Assert.AreEqual($@"UserName field must be of format domain\username was: {options.UserName}", ex.Message);
    }
}
