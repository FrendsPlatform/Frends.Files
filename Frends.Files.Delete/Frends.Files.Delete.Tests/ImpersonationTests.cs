using System;
using System.IO;
using Frends.Files.Delete.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Frends.Files.Delete.Tests;

[TestFixture]
internal class ImpersonationTests
{
    /// <summary>
    /// Impersonation tests needs to be run as administrator so that the OneTimeSetup can create a local test user. Impersonation tests can only be run in Windows OS.
    /// </summary>
    private readonly string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/");
    private readonly string domain = Environment.MachineName;
    private readonly string name = "test";
    private readonly string pwd = "pas5woRd!";
    private Input input = new();
    private Options options = new();

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Helper.CreateTestUser(domain, name, pwd);

        input = new Input
        {
            Directory = dir,
            Pattern = "*",
        };

        options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = true,
            UserName = $"{domain}\\{name}",
            Password = pwd,
        };
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Helper.DeleteTestUser(name);
    }

    [SetUp]
    public void Setup()
    {
        Helper.CreateTestFiles(dir);
    }

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(dir);
    }

    [Test]
    public void FileDeleteTestWithCredentials()
    {
        var result = Files.Delete(
            input,
            options,
            default);

        ClassicAssert.AreEqual(7, result.Files.Count);
        ClassicAssert.IsFalse(File.Exists(result.Files[0].Path));
    }

    [Test]
    public void FileDeleteTestWithUsernameWithoutDomain()
    {
        var options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = true,
            UserName = "test",
            Password = pwd,
        };

        var ex = Assert.Throws<ArgumentException>(() => Files.Delete(input, options, default));
        ClassicAssert.AreEqual($@"UserName field must be of format domain\username was: {options.UserName}", ex!.Message);
    }
}
