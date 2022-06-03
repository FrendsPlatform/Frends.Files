using System.Collections.Generic;

namespace Frends.Files.LocalBackup.Definitions
{
    /// <summary>
    /// Backup and cleanup results.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Backup results.
        /// </summary>
        public List<BackupObject> Backups { get; private set; }

        /// <summary>
        /// Cleanup results.
        /// </summary>
        public List<BackupObject> Cleanups { get; private set; }

        internal Result(List<BackupObject> backups, List<BackupObject> cleanups)
        {
            Backups = backups;
            Cleanups = cleanups;
        }
    }
}

