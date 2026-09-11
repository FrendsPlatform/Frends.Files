using System;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Principal;
using Frends.Files.Move.Definitions;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using dotenv.net;
using SimpleImpersonation;

namespace Frends.Files.Move.Tests;

[TestFixture]
internal class RemoteTests
{
    private static readonly string LocalWorkdir =
        Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData"));

    internal required string  SrcUser { get; set; }
    internal required string SrcUserPassword { get; set; }
    internal required string DstUser { get; set; }
    internal required string DstUserPassword { get; set; }
    internal required string AdminUser { get; set; }
    internal required string AdminUserPassword { get; set; }
    internal required string RemoteIp { get; set; }
    internal required string Domain { get; set; }

    [OneTimeSetUp]
    public void Setup()
    {
        DotEnv.Load();
        SrcUser = GetEnvVar("SRC_USER");
        SrcUserPassword = GetEnvVar("SRC_PASSWORD");
        DstUser = GetEnvVar("DST_USER");
        DstUserPassword = GetEnvVar("DST_PASSWORD");
        AdminUser = GetEnvVar("ADMIN_USER");
        AdminUserPassword = GetEnvVar("ADMIN_PASSWORD");
        RemoteIp = GetEnvVar("REMOTE_IP");
        Domain = GetEnvVar("DOMAIN");
    }

    private static string GetEnvVar(string name) =>
        Environment.GetEnvironmentVariable(name) ??
        throw new InvalidOperationException($"Missing required env var: {name}");

    [Test]
    [SupportedOSPlatform("windows")]
    public async Task MoveFileFromLocalToRemoteWithImpersonation()
    {
        var input = new Input
        {
            SourceDirectory = LocalWorkdir,
            Pattern = "*",
            TargetDirectory = $@"{RemoteIp}\Shared\dst",
        };
        var options = new Options
        {
            IfTargetFileExists = FileExistsAction.Rename
        };
        var connection = new Connection
        {
            TargetIsRemote = true,
            TargetUserName = $@"{Domain}\{DstUser}",
            TargetPassword = DstUserPassword,
        };
        PrepareSourceAndTarget(input, connection);
        var result = await Files.Move(input, connection, options, CancellationToken.None);

        Assert.That(result.Files.Count, Is.EqualTo(1));
    }

    [Test]
    [SupportedOSPlatform("windows")]
    public async Task MoveFileFromRemoteToLocalWithImpersonation()
    {
        var input = new Input
        {
            SourceDirectory = $@"{RemoteIp}\Shared\src",
            Pattern = "*",
            TargetDirectory = LocalWorkdir,
        };
        var options = new Options
        {
            IfTargetFileExists = FileExistsAction.Rename
        };
        var connection = new Connection
        {
            SourceIsRemote = true,
            SourceUserName =  $@"{Domain}\{SrcUser}",
            SourcePassword = SrcUserPassword,
        };
        PrepareSourceAndTarget(input, connection);
        var result = await Files.Move(input, connection, options, CancellationToken.None);

        Assert.That(result.Files.Count, Is.EqualTo(1));
    }

    [Test]
    [SupportedOSPlatform("windows")]
    public async Task MoveFileFromRemoteToRemoteWithImpersonation()
    {
        var input = new Input
        {
            SourceDirectory = $@"{RemoteIp}\Shared\src",
            Pattern = "*",
            TargetDirectory = $@"{RemoteIp}\Shared\dst",
        };
        var options = new Options
        {
            IfTargetFileExists = FileExistsAction.Rename
        };
        var connection = new Connection
        {
            SourceIsRemote = true,
            SourceUserName = $@"{Domain}\{SrcUser}",
            SourcePassword = SrcUserPassword,
            TargetIsRemote = true,
            TargetUserName = $@"{Domain}\{DstUser}",
            TargetPassword = DstUserPassword,
        };
        PrepareSourceAndTarget(input, connection);
        var result = await Files.Move(input, connection, options, CancellationToken.None);

        Assert.That(result.Files.Count, Is.EqualTo(1));
    }

    private static void PrepareSource(string sourcePath)
    {
        if (!Directory.Exists(sourcePath))
            Directory.CreateDirectory(sourcePath);
        var sourceFilePath = Path.Combine(sourcePath, "test.txt");
        if (!File.Exists(sourceFilePath))
            File.WriteAllText(sourceFilePath, "This is a test file.");
    }

    private static void PrepareTarget(string targetPath)
    {
        if (!Directory.Exists(targetPath))
            Directory.CreateDirectory(targetPath);

        var targetFilePath = Path.Combine(targetPath, "test.txt");
        if (File.Exists(targetFilePath))
            File.Delete(targetFilePath);
    }

    [SupportedOSPlatform("Windows")]
    private void PrepareSourceAndTarget(Input input, Connection connection)
    {
        var (domain, user) = GetDomainAndUsername($@"{Domain}\{AdminUser}");
        var credentials = new UserCredentials(domain, user, AdminUserPassword);
        using var userHandle = credentials.LogonUser(LogonType.NewCredentials);
        if (connection.SourceIsRemote)
            WindowsIdentity.RunImpersonated(userHandle, () => PrepareSource(input.SourceDirectory));
        else
            PrepareSource(input.SourceDirectory);

        if (connection.TargetIsRemote)
            WindowsIdentity.RunImpersonated(userHandle, () => PrepareTarget(input.TargetDirectory));
        else
            PrepareTarget(input.TargetDirectory);
    }

    private static Tuple<string, string> GetDomainAndUsername(string username)
    {
        var domainAndUserName = username.Split('\\');

        return domainAndUserName.Length != 2
            ? throw new ArgumentException($@"UserName field must be of format domain\username was: {username}")
            : new Tuple<string, string>(domainAndUserName[0], domainAndUserName[1]);
    }
}
