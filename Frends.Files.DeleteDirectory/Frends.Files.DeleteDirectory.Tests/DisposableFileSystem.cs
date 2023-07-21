﻿using System;
using System.IO;

namespace Frends.Files.DeleteDirectory.Tests;

public class DisposableFileSystem : IDisposable
{
    public string RootPath { get; set; }

    public DirectoryInfo DirectoryInfo { get; set; }

    public DisposableFileSystem()
    {
        RootPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(RootPath);
        DirectoryInfo = new DirectoryInfo(RootPath);
    }

    public DisposableFileSystem CreateFolder(string path)
    {
        Directory.CreateDirectory(Path.Combine(RootPath, path));
        return this;
    }

    public DisposableFileSystem CreateFiles(params string[] fileRelativePaths)
    {
        foreach (var path in fileRelativePaths)
        {
            var fullPath = Path.Combine(RootPath, path);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            File.WriteAllText(
                fullPath,
                string.Format("Automatically generated for testing on {0:yyyy}/{0:MM}/{0:dd} {0:hh}:{0:mm}:{0:ss}", DateTime.UtcNow));
        }

        return this;
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(RootPath, true);
        }
        catch
        {
            // Don't throw if this fails.
        }
    }
}
