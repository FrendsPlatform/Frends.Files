using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Runtime.InteropServices;

namespace Frends.Files.Delete.Tests;

internal class Helper
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

        DirectoryEntry aD = new DirectoryEntry("WinNT://" + domain + ",computer");
        DirectoryEntry newUser = aD.Children.Add(name, "user");
        newUser.Invoke("SetPassword", new object[] { pwd });
        newUser.Invoke("Put", new object[] { "Description", "Test User from .NET" });
        newUser.CommitChanges();
        DirectoryEntry grp;

        grp = aD.Children.Find("Administrators", "group");
        if (grp != null)
            grp.Invoke("Add", newUser.Path);
    }

    public static void DeleteTestUser(string name)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("UseGivenCredentials feature is only supported on Windows.");

        DirectoryEntry localDirectory = new DirectoryEntry("WinNT://" + Environment.MachineName.ToString());
        DirectoryEntries users = localDirectory.Children;
        DirectoryEntry user = users.Find(name);
        users.Remove(user);
    }
}
