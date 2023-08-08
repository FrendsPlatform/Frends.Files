﻿using Frends.Files.LocalBackup.Definitions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Frends.Files.LocalBackup
{
    ///<summary>
    /// Files task.
    /// </summary>
    public class Files
    {
        /// <summary>
        /// Create a local backup. Optionally task can create new subdirectories for backups and delete old backup directories.
        /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Files.LocalBackup)
        /// </summary>
        /// <param name="input">Input parameters</param>
        /// <param name="cancellationToken">Token to stop task. This is generated by Frends.</param>
        /// <returns>Result object { string Directory, int FileCountInBackup, List&lt;string&gt; Backups, List&lt;string&gt; Cleanups }</returns>
        public static Result LocalBackup([PropertyTab] Input input, CancellationToken cancellationToken)
        {
            if (input.FilePaths == null && (string.IsNullOrWhiteSpace(input.SourceDirectory) || string.IsNullOrWhiteSpace(input.SourceFile))) throw new Exception("Source parameters required.");
            if (string.IsNullOrWhiteSpace(input.BackupDirectory)) throw new Exception("Backup directory required.");
            if (input.CreateSubdirectories && string.IsNullOrWhiteSpace(input.TaskExecutionId)) throw new Exception("Task execution id required.");

            var timestampString = DateTime.UtcNow.ToString("yyyy-MM-dd_HH_mm_ss");
            var backupDirectory = input.CreateSubdirectories
                ? Path.Combine(input.BackupDirectory, $"{timestampString}-{input.TaskExecutionId}")
                : input.BackupDirectory;

            var (directory, backup) = CreateBackup(input, backupDirectory, cancellationToken);

            var cleanup = input.Cleanup ? CleanUp(input, input.BackupDirectory, cancellationToken) : null;

            return new Result(directory, backup, cleanup);
        }

        private static Tuple<string, List<string>> CreateBackup(Input input, string backupDirectory, CancellationToken cancellationToken)
        {
            var result = new List<string>();

            Directory.CreateDirectory(backupDirectory);
            string[] files;

            if (input.FilePaths != null)
            {
                files = (string[])input.FilePaths;
                foreach (string file in files)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (File.Exists(file))
                    {
                        var backupFile = Path.Combine(backupDirectory, Path.GetFileName(file));
                        File.Copy(file, backupFile, true);
                        result.Add($"Backup complete: {file} to {backupFile}");
                    }
                }
            }
            else
            {
                files = Directory.GetFiles(input.SourceDirectory);
                foreach (string file in files)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (FileMatchesMask(Path.GetFileName(file), input.SourceFile))
                    {
                        var backupFile = Path.Combine(backupDirectory, Path.GetFileName(file));
                        File.Copy(file, backupFile, true);
                        result.Add($"Backup complete: {file} to {backupFile}");
                    }
                }
            }

            if (!Directory.GetFiles(backupDirectory).Any())
            {
                Directory.Delete(backupDirectory, false);
                return new Tuple<string, List<string>>("No source files present to backup.", new List<string>());
            }

            return new Tuple<string, List<string>>(backupDirectory, result);
        }

        private static List<string> CleanUp(Input input, string backupDirectory, CancellationToken cancellationToken)
        {
            var result = new List<string>();

            if (!input.CreateSubdirectories)
            {
                if (Directory.Exists(backupDirectory))
                {
                    var directories = Directory.GetDirectories(backupDirectory);
                    foreach (var dir in directories)
                    {
                        if (Directory.GetLastWriteTime(dir) < DateTime.Now.AddDays(-input.DaysOlder))
                        {
                            Directory.Delete(dir, true);
                            result.Add($"{dir} deleted.");
                        }
                    }
                    var files = Directory.GetFiles(backupDirectory);
                    foreach (var file in files)
                    {
                        if (File.GetLastWriteTime(file) < DateTime.Now.AddDays(-input.DaysOlder))
                        {
                            File.Delete(file);
                            result.Add($"{file} deleted.");
                        }
                    }
                }
            }
            else
            {
                ConcurrentDictionary<string, DateTime> _lastCleanup = new();
                _lastCleanup[backupDirectory] = DateTime.Now;

                if (input.DaysOlder < 0)
                    throw new ArgumentException("Days older cannot be a negative number", "daysOlder");

                var directories = Directory.GetDirectories(backupDirectory).Where(dirName => ShouldBeCleanedUp(dirName, input));

                foreach (var dir in directories)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Directory.Delete(dir, true);
                    result.Add($"{dir} deleted.");
                }
            }
            return result;
        }

        private static bool ShouldBeCleanedUp(string dirPath, Input input)
        {
            var TimestampPattern = "yyyy-MM-dd_HH_mm_ss";
            var DirNamePatternLength = TimestampPattern.Length + input.TaskExecutionId.Length + 1; // Add one to the Length because of '-' character which separates the timestamp and guid.
            var dirName = Path.GetFileName(dirPath) ?? string.Empty;

            if (dirName.Length == DirNamePatternLength)
            {
                var timestampPart = dirName.Substring(0, TimestampPattern.Length);

                if (DateTime.TryParseExact(timestampPart, TimestampPattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timeStamp))
                {
                    return timeStamp < DateTime.UtcNow.AddDays(-input.DaysOlder);
                }
            }
            return File.GetCreationTimeUtc(dirPath) < DateTime.UtcNow.AddDays(-input.DaysOlder);
        }

        private static bool FileMatchesMask(string filename, string mask)
        {
            const string regexEscape = "<regex>";
            string pattern;

            //check is pure regex wished to be used for matching
            if (mask.StartsWith(regexEscape))
                //use substring instead of string.replace just in case some has regex like '<regex>//File<regex>' or something else like that
                pattern = mask.Substring(regexEscape.Length);
            else
            {
                pattern = mask.Replace(".", "\\.");
                pattern = pattern.Replace("*", ".*");
                pattern = pattern.Replace("?", ".+");
                pattern = string.Concat("^", pattern, "$");
            }

            try
            {
                return Regex.IsMatch(filename, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                if (filename.Equals(mask, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (mask.StartsWith("*") && filename.EndsWith(mask.Replace("*", "")))
                    return true;
                if (mask.EndsWith("*") && filename.StartsWith(mask.Replace("*", "")))
                    return true;
                return false;
            }
        }
    }
}