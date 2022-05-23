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
        /// Create and delete local backups.
        /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Files.LocalBackup)
        /// </summary>
        /// <param name="input">Input parameters</param>
        /// <param name="cancellationToken">Token to stop task. This is generated by Frends.</param>
        /// <returns>List { string Backup, string Cleanup }</returns>
        public static Result LocalBackup([PropertyTab] Input input, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(input.SourceDirectory) || string.IsNullOrWhiteSpace(input.SourceFile)) throw new Exception("Source parameters required.");
            if (string.IsNullOrWhiteSpace(input.BackupDirectory)) throw new Exception("Backup directory required.");

            var backup = CreateBackup(input, cancellationToken);
            var cleanup = input.Cleanup ? CleanUp(input, cancellationToken) : null;

            return new Result(backup, cleanup);
        }

        private static List<BackupObject> CreateBackup(Input input, CancellationToken cancellationToken)
        {
            var timestampString = DateTime.UtcNow.ToString("yyyy-MM-dd_HH_mm_ss");
            var result = new List<BackupObject>();

            //Create backup folder
            var backupDirectory = Path.Combine(input.BackupDirectory, $"{timestampString}-{Guid.NewGuid()}");
            Directory.CreateDirectory(backupDirectory);

            var files = Directory.GetFiles(input.SourceDirectory);
            foreach (string file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (FileMatchesMask(Path.GetFileName(file), input.SourceFile))
                {
                    var backupFile = Path.Combine(backupDirectory, Path.GetFileName(file));
                    File.Copy(file, backupFile, true);
                    result.Add(new BackupObject { Backup = $"Backup complete: {file} to {backupFile}" });
                }
            }

            return result;
        }

        private static List<BackupObject> CleanUp(Input input, CancellationToken cancellationToken)
        {
            var result = new List<BackupObject>();
            ConcurrentDictionary<string, DateTime> _lastCleanup = new ConcurrentDictionary<string, DateTime>();
            _lastCleanup[input.BackupDirectory] = DateTime.Now;

            if (input.DaysOlder < 0)
                throw new ArgumentException("Days older cannot be a negative number", "daysOlder");

            var directories = Directory.GetDirectories(input.BackupDirectory).Where(dirName => IsOlderThan(dirName, input.DaysOlder));
            foreach (var dir in directories)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Directory.Delete(dir, true);
                result.Add(new BackupObject { Cleanup = $"{dir} deleted." });
            }
            return result;
        }

        private static bool IsOlderThan(string dirPath, int daysOlder)
        {
            var TimestampPattern = "yyyy-MM-dd_HH_mm_ss";
            var DirNamePatternLength = TimestampPattern.Length + 37; //TimestampPattern.Length+GUID
            var dirName = Path.GetFileName(dirPath) ?? string.Empty;

            if (dirName.Length == DirNamePatternLength)
            {
                var timestampPart = dirName.Substring(0, TimestampPattern.Length);

                if (DateTime.TryParseExact(timestampPart, TimestampPattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timeStamp))
                    return timeStamp < DateTime.UtcNow.AddDays(-daysOlder);
            }

            return new FileInfo(dirPath).LastWriteTime < DateTime.Now.AddDays(-daysOlder);
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
                pattern = String.Concat("^", pattern, "$");
            }

            return Regex.IsMatch(filename, pattern, RegexOptions.IgnoreCase);
        }
    }
}