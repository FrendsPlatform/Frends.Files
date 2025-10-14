using System;
using System.IO;
using System.Security.Principal;
using Frends.Files.Move.Definitions;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using SimpleImpersonation;

namespace Frends.Files.Move.Tests;

[TestFixture]
internal class RemoteTests
{
    private static readonly string LocalWorkdir =
        Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData"));

    private static readonly string SrcUser = Environment.GetEnvironmentVariable("SRC_USER")!;

    private static readonly string
        SrcUserPassword = Environment.GetEnvironmentVariable("SRC_PASSWORD")!;

    private static readonly string
        DstUser = Environment.GetEnvironmentVariable("DST_USER")!;

    private static readonly string
        DstUserPassword = Environment.GetEnvironmentVariable("DST_PASSWORD")!;

    private static readonly string
        AdminUser = Environment.GetEnvironmentVariable("ADMIN_USER")!;

    private static readonly string
        AdminUserPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")!;

    [Test]
    public async Task MoveFileFromLocalToRemoteWithImpersonation()
    {
        var input = new Input
        {
            SourceDirectory = LocalWorkdir,
            Pattern = "*",
            TargetDirectory = @"\\20.67.234.98\Shared\dst",
        };
        var options = new Options
        {
            IfTargetFileExists = FileExistsAction.Rename
        };
        var connection = new Connection
        {
            TargetIsRemote = true,
            TargetUserName = DstUser,
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
            SourceDirectory = @"\\20.67.234.98\Shared\src",
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
            SourceUserName = SrcUser,
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
            SourceDirectory = @"\\20.67.234.98\Shared\src",
            Pattern = "*",
            TargetDirectory = @"\\20.67.234.98\Shared\dst",
        };
        var options = new Options
        {
            IfTargetFileExists = FileExistsAction.Rename
        };
        var connection = new Connection
        {
            SourceIsRemote = true,
            SourceUserName = SrcUser,
            SourcePassword = SrcUserPassword,
            TargetIsRemote = true,
            TargetUserName = DstUser,
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

    private static void PrepareSourceAndTarget(Input input, Connection connection)
    {
        var (domain, user) = GetDomainAndUsername(AdminUser);
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