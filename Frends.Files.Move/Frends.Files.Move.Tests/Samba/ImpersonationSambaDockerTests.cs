using Frends.Files.Move.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace Frends.Files.Move.Tests.Samba;

[TestFixture]
public class ImpersonationSambaDockerTests
{
    private string _localSourceDir;
    private string _localTargetDir;
    private string _remoteDir;
    private Options _options;
    private bool _isDockerEnvironment;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "ImpersonationDockerTests_" + Guid.NewGuid().ToString("N")[..8]);
        _localSourceDir = Path.Combine(tempRoot, "Source");
        _localTargetDir = Path.Combine(tempRoot, "Target");
        Directory.CreateDirectory(_localSourceDir);
        Directory.CreateDirectory(_localTargetDir);

        WaitForDockerContainer("impersonation_test_samba", TimeSpan.FromMinutes(2));

        var connectionMethods = new[]
        {
            @"\\localhost:1445\testshare",      // Docker port mapping
            @"\\127.0.0.1:1445\testshare",     // Localhost IP with port
            GetContainerDirectPath(),           // Container IP fallback
            @"\\host.docker.internal:1445\testshare"  // Docker Desktop host
        };

        _isDockerEnvironment = false;
        string lastError = "";

        foreach (var path in connectionMethods)
        {
            if (string.IsNullOrEmpty(path)) continue;

            try
            {
                if (TestSambaConnection(path, "testuser", "password123!", TimeSpan.FromSeconds(10)))
                {
                    _remoteDir = path;
                    _isDockerEnvironment = true;
                    break;
                }
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                continue;
            }
        }

        if (!_isDockerEnvironment)
        {
            Assert.Ignore("Docker Samba environment not available");
        }

        _options = new Options
        {
            UserName = @"WORKGROUP\testuser",
            Password = "password123!",
            CreateTargetDirectories = true,
            IfTargetFileExists = FileExistsAction.Overwrite
        };
    }

    private void WaitForDockerContainer(string containerName, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            try
            {
                var psi = new ProcessStartInfo("docker", $"exec {containerName} smbclient -L //localhost/ -U testuser%password123! --option=\"client min protocol=SMB2\"")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process.WaitForExit(5000) && process.ExitCode == 0)
                {
                    return;
                }
            }
            catch { }

            Console.WriteLine($"⏳ Waiting for container {containerName} to be ready... ({stopwatch.Elapsed.TotalSeconds:F0}s)");
            Thread.Sleep(2000);
        }
    }

    private string GetContainerDirectPath()
    {
        try
        {
            var psi = new ProcessStartInfo("docker", "inspect impersonation_test_samba -f \"{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process.WaitForExit(5000) && process.ExitCode == 0)
            {
                var ip = process.StandardOutput.ReadToEnd().Trim();
                if (!string.IsNullOrEmpty(ip) && ip != "localhost")
                {
                    return $@"\\{ip}\testshare";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get container IP: {ex.Message}");
        }

        return null;
    }

    private bool TestSambaConnection(string uncPath, string username, string password, TimeSpan timeout)
    {
        var host = ExtractHostFromUncPath(uncPath);

        var psi = new ProcessStartInfo("docker", $"exec impersonation_test_samba smbclient //{host}/testshare -U {username}%{password} -c dir")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        return process.WaitForExit((int)timeout.TotalMilliseconds) && process.ExitCode == 0;
    }

    private string ExtractHostFromUncPath(string uncPath)
    {
        // Extract host
        var parts = uncPath.TrimStart('\\').Split('\\');
        return parts.Length > 0 ? parts[0] : "localhost";
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        var tempRoot = Path.GetDirectoryName(_localSourceDir);
        if (Directory.Exists(tempRoot))
        {
            Directory.Delete(tempRoot, true);
        }
    }

    [SetUp]
    public void Setup()
    {
        Helper.CreateTestFiles(_localSourceDir);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_localTargetDir))
        {
            Helper.DeleteTestFolder(_localTargetDir);
            Directory.CreateDirectory(_localTargetDir);
        }

        if (Directory.Exists(_localSourceDir))
        {
            Helper.DeleteTestFolder(_localSourceDir);
            Directory.CreateDirectory(_localSourceDir);
        }
        try
        {
            CleanRemoteDirectory();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to clean remote directory: {ex.Message}");
        }
    }

    private void CleanRemoteDirectory()
    {
        var psi = new ProcessStartInfo("docker", $"exec impersonation_test_samba find /mount/testshare -type f -delete")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        process.WaitForExit(10000);
    }

    [Test]
    public async Task RemoteToLocal_WithImpersonation_Success()
    {
        CreateRemoteTestFiles();

        var input = new Input
        {
            Directory = _remoteDir,
            Pattern = "*.txt",
            TargetDirectory = _localTargetDir
        };

        _options.RemotePath = RemotePathType.Source;

        var result = await Files.Move(input, _options, default);

        ClassicAssert.AreEqual(5, result.Files.Count);

        foreach (var file in result.Files)
        {
            ClassicAssert.IsTrue(File.Exists(file.TargetPath));

            var content = await File.ReadAllTextAsync(file.TargetPath);
            ClassicAssert.IsNotEmpty(content);

            ClassicAssert.IsFalse(File.Exists(file.SourcePath));
        }
    }

    [Test]
    public async Task LocalToRemote_WithImpersonation_Success()
    {
        var input = new Input
        {
            Directory = _localSourceDir,
            Pattern = "*.txt",
            TargetDirectory = _remoteDir
        };

        _options.RemotePath = RemotePathType.Target;

        var result = await Files.Move(input, _options, default);

        ClassicAssert.AreEqual(5, result.Files.Count);

        foreach (var file in result.Files)
        {
            ClassicAssert.IsTrue(File.Exists(file.TargetPath));
            ClassicAssert.IsFalse(File.Exists(file.SourcePath));
        }
    }

    private void CreateRemoteTestFiles()
    {
        // Use docker exec to create test files in the container
        var commands = new[]
        {
            "echo 'Test1 content' > /mount/testshare/Test1.txt",
            "echo 'Test2 content' > /mount/testshare/Test2.txt",
            "echo 'Test3 content' > /mount/testshare/Test3.txt",
            "echo 'Test4 content' > /mount/testshare/Test4.txt",
            "echo 'Test5 content' > /mount/testshare/Test5.txt"
        };

        foreach (var command in commands)
        {
            var psi = new ProcessStartInfo("docker", $"exec impersonation_test_samba sh -c \"{command}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            process.WaitForExit(5000);
        }
    }
}