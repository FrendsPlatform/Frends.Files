using System;
using System.Collections.Generic;
using System.IO;
using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace Frends.Files.Move.Tests;

internal static class Helper
{
    public static void CreateTestFiles(string directory)
    {
        if (!File.Exists(directory))
            Directory.CreateDirectory(directory);

        var list = new List<string>
        {
            Path.Combine(directory, "Test1.txt"),
            Path.Combine(directory, "Test2.txt"),
            Path.Combine(directory, "Test1.xml"),
            Path.Combine(directory, "pro_test.txt"),
            Path.Combine(directory, "pref_test.txt"),
            Path.Combine(directory, "_test.txt"),
            Path.Combine(directory, "prof_test.txt"),
        };

        // Create test files and edit creation date
        foreach (var path in list)
        {
            File.WriteAllText(path, $"Test {path}");
        }
    }

    public static void DeleteTestFolder(string directory)
    {
        Directory.Delete(directory, true);
    }

    public static void CreateTestUser(string domain, string name, string pwd)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("UseGivenCredentials feature is only supported on Windows.");

        var ad = new DirectoryEntry("WinNT://" + domain + ",computer");
        using var newUser = ad.Children.Add(name, "user");
        newUser.Invoke("SetPassword", pwd);
        newUser.Invoke("Put", "Description", "Test User from .NET");
        newUser.CommitChanges();

        var grp = ad.Children.Find("Administrators", "group");
        grp.Invoke("Add", newUser.Path);
    }

    public static void DeleteTestUser(string name)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("UseGivenCredentials feature is only supported on Windows.");

        using var localDirectory = new DirectoryEntry("WinNT://" + Environment.MachineName);
        var users = localDirectory.Children;
        using var user = users.Find(name);
        users.Remove(user);
    }
}