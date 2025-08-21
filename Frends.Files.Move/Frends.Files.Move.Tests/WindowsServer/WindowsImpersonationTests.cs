using Frends.Files.Move.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace Frends.Files.Move.Tests.WindowsServer
{
    [TestFixture]
    public class WindowsImpersonationTests
    {
        private string _localSourceDir;
        private string _localTargetDir;
        private string _remoteShare;
        private Options _options;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.Ignore("Windows only tests");

            // Simple manual setup - just set these values for your environment
            //_remoteShare = @"\\172.24.187.181%3a1445\testshare";
            //_remoteShare = @"\\windows_fileserver\testshare";
            _remoteShare = @"\\127.0.0.1\testshare";
            _options = new Options
            {
                UserName = @"testfileuser",
                Password = "Password123!",
                CreateTargetDirectories = true,
                IfTargetFileExists = FileExistsAction.Overwrite
            };

            // Create temp directories
            var tempRoot = Path.Combine(Path.GetTempPath(), "ImpersonationTests");
            _localSourceDir = Path.Combine(tempRoot, "Source");
            _localTargetDir = Path.Combine(tempRoot, "Target");
            Directory.CreateDirectory(_localSourceDir);
            Directory.CreateDirectory(_localTargetDir);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            var tempRoot = Path.GetDirectoryName(_localSourceDir);
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, true);
        }

        [SetUp]
        public void Setup()
        {
            // Create local test files
            for (int i = 1; i <= 3; i++)
            {
                File.WriteAllText(Path.Combine(_localSourceDir, $"test{i}.txt"), $"Content {i}");
            }

            // Clean remote directory
            try
            {
                Files.ExecuteWithImpersonation(_options, async () =>
                {
                    if (Directory.Exists(_remoteShare))
                    {
                        foreach (var file in Directory.GetFiles(_remoteShare))
                            File.Delete(file);
                    }
                    await Task.CompletedTask;
                }).Wait();
            }
            catch { }
        }

        [Test]
        public async Task RemoteToLocal_MovesFiles()
        {
            // Arrange - Create files on remote
            await Files.ExecuteWithImpersonation(_options, async () =>
            {
                for (int i = 1; i <= 3; i++)
                {
                    await File.WriteAllTextAsync(Path.Combine(_remoteShare, $"remote{i}.txt"), $"Remote content {i}");
                }
            });

            var input = new Input
            {
                Directory = _remoteShare,
                Pattern = "*.txt",
                TargetDirectory = _localTargetDir
            };
            _options.RemotePath = RemotePathType.Source;

            var result = await Files.Move(input, _options, CancellationToken.None);

            Assert.That(result.Files.Count, Is.EqualTo(3));
            foreach (var file in result.Files)
            {
                Assert.That(File.Exists(file.TargetPath), Is.True);
                Assert.That(File.Exists(file.SourcePath), Is.False);
            }
        }

        [Test]
        public async Task LocalToRemote_MovesFiles()
        {
            var input = new Input
            {
                Directory = _localSourceDir,
                Pattern = "*.txt",
                TargetDirectory = _remoteShare
            };
            _options.RemotePath = RemotePathType.Target;

            var result = await Files.Move(input, _options, CancellationToken.None);

            Assert.That(result.Files.Count, Is.EqualTo(3));

            await Files.ExecuteWithImpersonation(_options, async () =>
            {
                foreach (var file in result.Files)
                {
                    Assert.That(File.Exists(file.TargetPath), Is.True);
                }
                await Task.CompletedTask;
            });

            foreach (var file in result.Files)
            {
                Assert.That(File.Exists(file.SourcePath), Is.False);
            }
        }
    }
}