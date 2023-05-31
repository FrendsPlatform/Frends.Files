using Frends.Files.LocalBackup.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Frends.Files.LocalBackup.Tests;

[TestClass]
public class UnitTests
{
    private readonly string _dir = Path.Combine(Environment.CurrentDirectory, "Tests"); // ...Test\bin\Debug\net6.0\
    Input? input;

    [TestInitialize]
    public void Setup()
    {
        CreateTestFiles();
    }

    [TestCleanup]
    public void TearDown()
    {
        DeleteTestFolder();
    }

    /// <summary>
    /// Copy all files to backup directory. Don't create subdir.
    /// </summary>
    [TestMethod]
    public void CopyAll_CreateSubdirectoriesFalse_Test()
    {
        var buDir = Path.Combine(_dir, "Backup");

        input = new Input()
        {
            SourceDirectory = _dir,
            SourceFile = "*",
            BackupDirectory = buDir,
            TaskExecutionId = null,
            DaysOlder = 5,
            Cleanup = false,
            CreateSubdirectories = false
        };

        var result = Files.LocalBackup(input, default);
        Assert.IsNotNull(result);

        foreach (var dir in Directory.GetDirectories(buDir))
        {
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                Assert.IsTrue(file.Contains(Path.Combine(dir, "Overwrite.txt")) || file.Contains(Path.Combine(dir, "Test1.txt")) || file.Contains(Path.Combine(dir, "Test2.txt")) || file.Contains(Path.Combine(dir, "Test1.xml")));
            }
        }
    }

    /// <summary>
    /// Copy all files to backup directory. Create subdir.
    /// </summary>
    [TestMethod]
    public void CopyAll_CreateSubdirectoriesTrue_Test()
    {
        var buDir = Path.Combine(_dir, "Backup");

        input = new Input()
        {
            SourceDirectory = _dir,
            SourceFile = "*",
            BackupDirectory = buDir,
            TaskExecutionId = Guid.NewGuid().ToString(),
            DaysOlder = 5,
            Cleanup = false,
            CreateSubdirectories = true
        };

        var result = Files.LocalBackup(input, default);
        Assert.IsNotNull(result);

        foreach (var dir in Directory.GetDirectories(buDir, "2022-05-*"))
        {
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                Assert.IsTrue(file.Contains(Path.Combine(dir, "Overwrite.txt")) || file.Contains(Path.Combine(dir, "Test1.txt")) || file.Contains(Path.Combine(dir, "Test2.txt")) || file.Contains(Path.Combine(dir, "Test1.xml")));
            }
        }
    }

    /// <summary>
    /// Copy all files to backup directory. Create subdir and use something else but GUID as TaskExecutionId.
    /// </summary>
    [TestMethod]
    public void CopyAll_CreateSubdirectoriesTrue_NonGUID_Test()
    {
        var buDir = Path.Combine(_dir, "Backup");

        input = new Input()
        {
            SourceDirectory = _dir,
            SourceFile = "*",
            BackupDirectory = buDir,
            TaskExecutionId = "qwerty123",
            DaysOlder = 5,
            Cleanup = false,
            CreateSubdirectories = true
        };

        var result = Files.LocalBackup(input, default);
        Assert.IsNotNull(result);

        foreach (var dir in Directory.GetDirectories(buDir, "*qwerty123*"))
        {
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                Assert.IsTrue(file.Contains(Path.Combine(dir, "Overwrite.txt")) || file.Contains(Path.Combine(dir, "Test1.txt")) || file.Contains(Path.Combine(dir, "Test2.txt")) || file.Contains(Path.Combine(dir, "Test1.xml")));
            }
        }
    }

    /// <summary>
    /// Copy only Test1.txt and Test2.txt.
    /// </summary>
    [TestMethod]
    public void CopyWithPrefixTest()
    {
        var buDir = Path.Combine(_dir, "Backup");

        input = new Input()
        {
            SourceDirectory = _dir,
            SourceFile = "Test*",
            BackupDirectory = buDir,
            TaskExecutionId = Guid.NewGuid().ToString(),
            DaysOlder = 5,
            Cleanup = false,
        };

        var result = Files.LocalBackup(input, default);
        Assert.IsNotNull(result);

        foreach (var dir in Directory.GetDirectories(buDir, "2022-05-*"))
        {
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                Assert.IsTrue(file.Contains(Path.Combine(dir, "Test1.txt")) || file.Contains(Path.Combine(dir, "Test2.txt")) || file.Contains(Path.Combine(dir, "Test1.xml")));
                Assert.IsFalse(file.Contains(Path.Combine(dir, "Overwrite.txt")));
            }
        }
    }

    /// <summary>
    /// Copy only Test1.txt and Test1.xml.
    /// </summary>
    [TestMethod]
    public void CopyWithPrefix2Test()
    {
        var buDir = Path.Combine(_dir, "Backup");

        input = new Input()
        {
            SourceDirectory = _dir,
            SourceFile = "Test1.(txt|xml)",
            BackupDirectory = buDir,
            TaskExecutionId = Guid.NewGuid().ToString(),
            DaysOlder = 5,
            Cleanup = false,
        };

        var result = Files.LocalBackup(input, default);
        Assert.IsNotNull(result);

        foreach (var dir in Directory.GetDirectories(buDir, "2022-05-*"))
        {
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                Assert.IsTrue(file.Contains(Path.Combine(dir, "Test1.txt")) || file.Contains(Path.Combine(dir, "Test1.xml")));
                Assert.IsFalse(file.Contains(Path.Combine(dir, "Overwrite.txt")) || file.Contains(Path.Combine(dir, "Test2.txt")));
            }
        }
    }

    /// <summary>
    /// Copy only Test1.xml.
    /// </summary>
    [TestMethod]
    public void CopyWithPrefix3Test()
    {
        var buDir = Path.Combine(_dir, "Backup");

        input = new Input()
        {
            SourceDirectory = _dir,
            SourceFile = "Test1.[^t][^x][^t]",
            BackupDirectory = buDir,
            TaskExecutionId = Guid.NewGuid().ToString(),
            DaysOlder = 5,
            Cleanup = false,
        };

        var result = Files.LocalBackup(input, default);
        Assert.IsNotNull(result);

        foreach (var dir in Directory.GetDirectories(buDir, "2022-05-*"))
        {
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                Assert.IsTrue(file.Contains(Path.Combine(dir, "Test1.xml")));
                Assert.IsFalse(file.Contains(Path.Combine(dir, "Test1.txt")) || file.Contains(Path.Combine(dir, "Overwrite.txt")) || file.Contains(Path.Combine(dir, "Test2.txt")));
            }
        }
    }

    /// <summary>
    /// Copy pro_test.txt, pref_test.txt, _test.txt and skip prof_test.txt, pro_tet.txt.
    /// </summary>
    [TestMethod]
    public void CopyWithPrefix4Test()
    {
        var buDir = Path.Combine(_dir, "Backup", "Pro");

        input = new Input()
        {
            SourceDirectory = Path.Combine(_dir, "Pro"),
            SourceFile = "<regex>^(?!prof).*_test.txt",
            BackupDirectory = buDir,
            TaskExecutionId = Guid.NewGuid().ToString(),
            DaysOlder = 5,
            Cleanup = false,
        };

        var result = Files.LocalBackup(input, default);
        Assert.IsNotNull(result);

        foreach (var dir in Directory.GetDirectories(buDir, "2022-05-*"))
        {
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                Assert.IsTrue(file.Contains(Path.Combine(dir, "pro_test.txt")) || file.Contains(Path.Combine(dir, "pref_test.txt")) || file.Contains(Path.Combine(dir, "_test.txt")));
                Assert.IsFalse(file.Contains(Path.Combine(dir, "prof_test.txt")) || file.Contains(Path.Combine(dir, "pro_tet.txt")));
            }
        }
    }

    /// <summary>
    /// Delete files older than {DaysOlder}. CreateSubdirectories = true.
    /// </summary>
    [TestMethod]
    public void CleanupFile_CreateSubdirectoriesTrue_Test()
    {
        var timestampString = DateTime.UtcNow.AddDays(-10).ToString("yyyy-MM-dd_HH_mm_ss");
        var backupDirectory = Path.Combine(_dir, "Cleanup", $"{timestampString}-{Guid.NewGuid()}");
        Directory.CreateDirectory(backupDirectory);

        input = new Input()
        {
            SourceDirectory = _dir,
            SourceFile = "*",
            BackupDirectory = Path.Combine(_dir, "Cleanup"),
            TaskExecutionId = Guid.NewGuid().ToString(),
            DaysOlder = 1,
            Cleanup = true,
            CreateSubdirectories = true,
        };

        var result = Files.LocalBackup(input, default);
        Assert.IsNotNull(result);
    }

    /// <summary>
    /// Delete files older than {DaysOlder}. CreateSubdirectories = false.
    /// </summary>
    [TestMethod]
    public void CleanupFile_CreateSubdirectoriesFalse_Test()
    {
        input = new Input()
        {
            SourceDirectory = _dir,
            SourceFile = "*",
            BackupDirectory = Path.Combine(_dir, "Cleanup"),
            TaskExecutionId = null,
            DaysOlder = 1,
            Cleanup = true,
            CreateSubdirectories = false,
        };

        var backupDirectory = Path.Combine(_dir, "Cleanup", "DeleteThis");
        Directory.CreateDirectory(backupDirectory);
        Directory.SetLastWriteTimeUtc(backupDirectory, DateTime.Now.AddDays(-2));

        var result = Files.LocalBackup(input, default);
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Cleanups);
        Assert.IsFalse(Directory.Exists(backupDirectory));
    }

    /// <summary>
    /// Clean individual files when CreateSubdirectories is false.
    /// </summary>
    [TestMethod]
    public void CleanupFile_CleanIndividualFiles_Test()
    {
        var backup = Path.Combine(_dir, "Cleanup");

        input = new Input()
        {
            SourceDirectory = _dir,
            SourceFile = "*",
            BackupDirectory = backup,
            TaskExecutionId = null,
            DaysOlder = 1,
            Cleanup = true,
            CreateSubdirectories = false,
        };

        Files.LocalBackup(input, default);
        foreach (var dir in Directory.GetDirectories(backup))
            Directory.SetLastWriteTime(dir, DateTime.Now.AddDays(-2));
        var files = Directory.GetFiles(backup).ToList();
        foreach (var file in files)
        {
            File.SetLastWriteTime(file, DateTime.Now.AddDays(-2));
            var newName = Path.GetFileNameWithoutExtension(file) + "(1)" + Path.GetExtension(file);
            File.Move(file, Path.Combine(Path.GetDirectoryName(file) ?? backup, newName));
        }
        var result = Files.LocalBackup(input, default);
        Assert.AreEqual(4, result.Cleanups.Count);
    }

    [TestMethod]
    public void CleanupFile_CleanWithoutTimestampInDirectoryName()
    {
        var backup = Path.Combine(_dir, "Cleanup");

        input = new Input()
        {
            SourceDirectory = _dir,
            SourceFile = "*",
            FilePaths = null,
            BackupDirectory = backup,
            TaskExecutionId = Guid.NewGuid().ToString(),
            DaysOlder = 2,
            Cleanup = true,
            CreateSubdirectories = true
        };

        var newDir = Path.Combine(backup, Guid.NewGuid().ToString());
        Directory.CreateDirectory(newDir);
        Directory.SetCreationTimeUtc(newDir, DateTime.UtcNow.AddDays(-2));
        var result = Files.LocalBackup(input, default);
        Assert.AreEqual(1, result.Cleanups.Count);
    }

    [TestMethod]
    public void TestCleanupWithoutBackup()
    {
        var input = new Input
        {
            SourceDirectory = Environment.CurrentDirectory,
            SourceFile = "FileThatDontExist",
            FilePaths = null,
            BackupDirectory = _dir,
            CreateSubdirectories = true,
            Cleanup = true,
            DaysOlder = 14,
            TaskExecutionId = Guid.NewGuid().ToString()
        };

        var result = Files.LocalBackup(input, default);
        Assert.AreEqual(0, result.Cleanups.Count);
    }

    [TestMethod]
    public void TestBackupWithFilePaths()
    {
        var input = new Input
        {
            SourceDirectory = "",
            SourceFile = "",
            FilePaths = new string[]
            {
                Path.Combine(_dir, "Test1.txt"),
                Path.Combine(_dir, "Test2.txt"),
                Path.Combine(_dir, "Test1.xml"),
            },
            BackupDirectory = _dir,
            CreateSubdirectories = true,
            Cleanup = true,
            DaysOlder = 14,
            TaskExecutionId = Guid.NewGuid().ToString()
        };
        var result = Files.LocalBackup(input, default);
        Assert.AreEqual(3, result.FileCountInBackup);
    }

    [TestMethod]
    public void TestBackup_OnlyFilePathsAreUsedEvenIfDirectoryAndFileMaskIsSet()
    {
        var input = new Input
        {
            SourceDirectory = _dir,
            SourceFile = "*",
            FilePaths = new string[]
            {
                Path.Combine(_dir, "Test1.txt"),
                Path.Combine(_dir, "Test2.txt"),
                Path.Combine(_dir, "Test1.xml"),
            },
            BackupDirectory = _dir,
            CreateSubdirectories = true,
            Cleanup = true,
            DaysOlder = 14,
            TaskExecutionId = Guid.NewGuid().ToString()
        };
        var result = Files.LocalBackup(input, default);
        Assert.AreEqual(3, result.FileCountInBackup);
    }

    [TestMethod]
    public void TestBackup_FilePathsFilesNotFound()
    {
        var input = new Input
        {
            SourceDirectory = "",
            SourceFile = "",
            FilePaths = new string[]
            {
                Path.Combine(_dir, "Test56.txt"),
                Path.Combine(_dir, "Test57.txt"),
                Path.Combine(_dir, "Test59.xml"),
            },
            BackupDirectory = _dir,
            CreateSubdirectories = true,
            Cleanup = true,
            DaysOlder = 14,
            TaskExecutionId = Guid.NewGuid().ToString()
        };
        var result = Files.LocalBackup(input, default);
        Assert.AreEqual(0, result.FileCountInBackup);
    }

    public void CreateTestFiles()
    {
        Directory.CreateDirectory(Path.Combine(_dir, "Sub"));
        Directory.CreateDirectory(Path.Combine(_dir, "Pro"));

        var list = new List<string>
        {
            Path.Combine(_dir, "Test1.txt"),
            Path.Combine(_dir, "Test2.txt"),
            Path.Combine(_dir, "Test1.xml"),
            Path.Combine(_dir, "Overwrite.txt"),
            Path.Combine(_dir, "Sub", "Overwrite.txt"),
            Path.Combine(_dir, "Pro", "pro_test.txt"),
            Path.Combine(_dir, "Pro", "pref_test.txt"),
            Path.Combine(_dir, "Pro", "_test.txt"),
            Path.Combine(_dir, "Pro", "prof_test.txt"),
            Path.Combine(_dir, "Pro", "pro_test.txt")
        };

        // Create test files and edit creationdate.
        foreach (var file in list)
        {
            if (file.StartsWith(Path.Combine(_dir, "Overwrite.txt")))
                File.AppendAllText(file, "Overwrite complete.");
            else
                File.AppendAllText(file, $"Test {file}");
        }
    }

    public void DeleteTestFolder()
    {
        DirectoryInfo directoryInfo = new(_dir);
        directoryInfo.Delete(true);
    }
}
