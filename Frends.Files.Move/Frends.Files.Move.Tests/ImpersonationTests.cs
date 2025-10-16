using Frends.Files.Move.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace Frends.Files.Move.Tests;

[TestFixture]
internal class ImpersonationTests
{
    /// <summary>
    /// Impersonation tests needs to be run as administrator so that the OneTimeSetup can create a local test user. Impersonation tests can only be run in Windows OS.
    /// </summary>
    private static readonly string
        SourceDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/");

    private static readonly string TargetDir = Path.Combine(SourceDir, "destination");
    private Input? _input;
    private Connection? _connection;

    private readonly string _domain = Environment.MachineName;
    private const string Name = "test";
    private const string Pwd = "pas5woRd!";


    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Helper.CreateTestUser(_domain, Name, Pwd);

        _input = new Input
        {
            SourceDirectory = SourceDir,
            Pattern = "*",
            TargetDirectory = TargetDir,
        };

        _connection = new Connection
        {
            SourceIsRemote = true,
            SourceUserName = $"{_domain}\\{Name}",
            SourcePassword = Pwd,
            TargetIsRemote = true,
            TargetUserName = $"{_domain}\\{Name}",
            TargetPassword = Pwd
        };
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Helper.DeleteTestUser(Name);
    }

    [SetUp]
    public void Setup()
    {
        Helper.CreateTestFiles(SourceDir);
        Directory.CreateDirectory(TargetDir);
    }

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(SourceDir);
    }

    [Test]
    public async Task FileMoveTestWithCredentials()
    {
        var result = await Files.Move(
            _input,
            _connection, new Options(), CancellationToken.None);

        ClassicAssert.AreEqual(7, result.Files.Count);
        ClassicAssert.IsTrue(File.Exists(result.Files[0].TargetPath));
        ClassicAssert.IsFalse(File.Exists(result.Files[0].SourcePath));
    }

    [Test]
    public void FileMoveTestWithUsernameWithoutDomain()
    {
        var connection = new Connection
        {
            SourceIsRemote = true,
            SourceUserName = "test",
            SourcePassword = Pwd,
            TargetIsRemote = true,
            TargetUserName = "test",
            TargetPassword = Pwd,
        };
        var options = new Options
        {
            ThrowErrorOnFailure = true
        };

        var ex = Assert.ThrowsAsync<Exception>(() => Files.Move(_input, connection, options, CancellationToken.None));
        ClassicAssert.AreEqual($@"UserName field must be of format domain\username was: {connection.SourceUserName}",
            ex?.Message);
    }
}