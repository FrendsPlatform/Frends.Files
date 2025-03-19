using Frends.Files.Rename.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;

namespace Frends.Files.Rename.Tests;

[TestFixture]
class ImpersonationTests
{
    /// <summary>
    /// Impersonation tests needs to be run as administrator so that the OneTimeSetup can create a local test user. Impersonation tests can only be run in Windows OS.
    /// </summary>
    private static readonly string _FullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/"));
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
            Path = Path.Combine(_FullPath, "Test1.txt"),
            NewFileName = "NewTestFile.txt"
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
        Helper.CreateTestFiles(_FullPath);
    }

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(Path.GetDirectoryName(_FullPath));
    }

    [Test]
    public void FileMoveTestWithCredentials()
    {
        var result = Files.Rename(_input, _options);
        ClassicAssert.IsTrue(File.Exists(result.Path));
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

        var ex = Assert.Throws<ArgumentException>(() => Files.Rename(_input, options));
        ClassicAssert.AreEqual($@"UserName field must be of format domain\username was: {options.UserName}", ex.Message);
    }
}
