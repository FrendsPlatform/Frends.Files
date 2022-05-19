using System.ComponentModel.DataAnnotations;

namespace Frends.Files.LocalBackup.Definitions
{
    /// <summary>
    /// Input parameters.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Source file. The file mask uses regular expressions, but for convenience, it has special handling for * and ? wildcards.
        /// </summary>
        /// <example>test.txt, test*.txt, test?.txt, test.(txt|xml), test.[^t][^x][^t], <regex>^(?!prof).*_test.txt</example>
        [DisplayFormat(DataFormatString = "Text")]
        public string SourceFile { get; set; }

        /// <summary>
        /// Source directory.
        /// </summary>
        /// <example>c:\temp</example>
        [DisplayFormat(DataFormatString = "Text")]
        public string SourceDirectory { get; set; }

        /// <summary>
        /// Destination directory where backup folder will be created. Backup directory's format: {BackupDirectory}{timestamp}-{Guid}.
        /// </summary>
        /// <example>c:\temp\backups</example>
        [DisplayFormat(DataFormatString = "Text")]
        public string BackupDirectory { get; set; }

        /// <summary>
        /// Cleanup older than {DaysOlder} folders from backup directory.
        /// </summary>
        /// <example>false</example>
        public bool Cleanup { get; set; }

        /// <summary>
        /// Clean up backups older than given value in days. Enabled when {Cleanup} is true.
        /// </summary>
        /// <example>30</example>
        [UIHint(nameof(Cleanup), "", true)]
        public int DaysOlder { get; set; }
    }
}