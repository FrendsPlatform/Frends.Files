using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
[SupportedOSPlatform("windows")]
internal class RemoteTests
{
    private static readonly string LocalWorkdir =
        Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData"));

    internal required string SrcUser { get; set; }
    internal required string SrcUserPassword { get; set; }
    internal required string DstUser { get; set; }
    internal required string DstUserPassword { get; set; }
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
        RemoteIp = GetEnvVar("REMOTE_IP");
        Domain = GetEnvVar("DOMAIN");
        EnsureRemoteShareIsReachable();
    }

    private static string GetEnvVar(string name) =>
        Environment.GetEnvironmentVariable(name) ??
        throw new InvalidOperationException($"Missing required env var: {name}");

    private void EnsureRemoteShareIsReachable()
    {
        var endpoint = GetRemoteSharePath();
        using var socket = new TcpClient();

        try
        {
            var connected = socket.ConnectAsync(RemoteIp, 445);
            if (!connected.Wait(TimeSpan.FromSeconds(10)))
                Assert.Fail($"Remote SMB connection unavailable: could not reach '{endpoint}' on port 445.");
        }
        catch (SocketException ex)
        {
            Assert.Fail($"Remote SMB connection unavailable: could not reach '{endpoint}' on port 445. {ex.Message}");
        }

        Assert.That(
            socket.Connected,
            Is.True,
            $"Remote SMB connection unavailable: could not establish a TCP connection to '{endpoint}' on port 445.");
    }

    private string GetRemoteSharePath(string folderName = "")
    {
        List<string> elements = [$@"\\{RemoteIp}\Shared", folderName];

        return string.Join(@"\", elements.Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    [Test]
    public async Task MoveFileFromLocalToRemoteWithImpersonation()
    {
        var input = new Input
        {
            SourceDirectory = LocalWorkdir,
            Pattern = "*",
            TargetDirectory = GetRemoteSharePath("dst"),
        };
        var options = new Options
        {
            IfTargetFileExists = FileExistsAction.Rename,
            ThrowErrorOnFailure = true
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
    public async Task MoveFileFromRemoteToLocalWithImpersonation()
    {
        var input = new Input
        {
            SourceDirectory = GetRemoteSharePath("src"),
            Pattern = "*",
            TargetDirectory = LocalWorkdir,
        };
        var options = new Options
        {
            IfTargetFileExists = FileExistsAction.Rename,
            ThrowErrorOnFailure = true
        };
        var connection = new Connection
        {
            SourceIsRemote = true,
            SourceUserName = $@"{Domain}\{SrcUser}",
            SourcePassword = SrcUserPassword,
        };
        PrepareSourceAndTarget(input, connection);
        var result = await Files.Move(input, connection, options, CancellationToken.None);

        Assert.That(result.Files.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task MoveFileFromRemoteToRemoteWithImpersonation()
    {
        var input = new Input
        {
            SourceDirectory = GetRemoteSharePath("src"),
            Pattern = "*",
            TargetDirectory = GetRemoteSharePath("dst"),
        };
        var options = new Options
        {
            IfTargetFileExists = FileExistsAction.Rename,
            ThrowErrorOnFailure = true
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

    private void PrepareSourceAndTarget(Input input, Connection connection)
    {
        if (connection.SourceIsRemote)
            RunAs(connection.SourceUserName, connection.SourcePassword, () => PrepareSource(input.SourceDirectory));
        else
            PrepareSource(input.SourceDirectory);

        if (connection.TargetIsRemote)
            RunAs(connection.TargetUserName, connection.TargetPassword, () => PrepareTarget(input.TargetDirectory));
        else
            PrepareTarget(input.TargetDirectory);
    }

    private static void RunAs(string username, string password, Action action)
    {
        var (domain, user) = GetDomainAndUsername(username);
        var credentials = new UserCredentials(domain, user, password);
        using var userHandle = credentials.LogonUser(LogonType.NewCredentials);
        WindowsIdentity.RunImpersonated(userHandle, action);
    }

    private static Tuple<string, string> GetDomainAndUsername(string username)
    {
        var domainAndUserName = username.Split('\\');

        return domainAndUserName.Length != 2
            ? throw new ArgumentException($@"UserName field must be of format domain\username was: {username}")
            : new Tuple<string, string>(domainAndUserName[0], domainAndUserName[1]);
    }
}
