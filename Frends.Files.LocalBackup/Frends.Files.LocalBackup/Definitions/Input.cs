using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Frends.Files.LocalBackup.Definitions
{
    /// <summary>
    /// Input parameters.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Source directory.
        /// </summary>
        /// <example>c:\temp</example>
        [DisplayFormat(DataFormatString = "Text")]
        public string SourceDirectory { get; set; }

        /// <summary>
        /// Source file mask matched against file name only.
        /// By default, matching is case-insensitive and uses glob-style matching for file names.
        /// Use '&lt;regex&gt;' prefix to evaluate the value as a regular expression instead.
        /// </summary>
        /// <example>
        /// Glob: test.txt, test*.txt, report[1].xml, invoice(2024).pdf
        /// Regex: &lt;regex&gt;^test\.(txt|xml)$, &lt;regex&gt;^test.[^t][^x][^t]$, &lt;regex&gt;^(?!prof).*_test.txt$
        /// </example>
        [DisplayFormat(DataFormatString = "Text")]
        public string SourceFile { get; set; }

        /// <summary>
        /// The paths to the files to be backuped, mainly meant to be used with the file trigger with the syntax: #trigger.data.filePaths
        /// FilePaths will overwrite SourceDirectory and SourceFile parameters.
        /// </summary>
        /// <example>#trigger.data.filePaths</example>
        [DefaultValue("")]
        public object FilePaths { get; set; }

        /// <summary>
        /// Destination directory where backup folder will be created. Backup directory's format if CreateSubdirectories=true: {BackupDirectory}{timestamp}-{Guid}.
        /// </summary>
        /// <example>c:\temp\backups, c:\temp\backups\ </example>
        [DisplayFormat(DataFormatString = "Text")]
        public string BackupDirectory { get; set; }

        /// <summary>
        /// Create a subdirectory into the given backup directory.
        /// </summary>
        /// <example>true</example>
        [DefaultValue(true)]
        public bool CreateSubdirectories { get; set; }

        /// <summary>
        /// Enabled when (Create Subdirectories) is true. Value which is used in naming the backup directory. Can be any string but using task's execution id as default to avoid duplicate directory names.
        /// </summary>
        /// <example>e7e34166-f4fd-45e5-9307-ea5c2cf8e037, foobar123</example>
        [UIHint(nameof(CreateSubdirectories), "", true)]
        [DefaultValue("#process.executionid")]
        public string TaskExecutionId { get; set; }

        /// <summary>
        /// Cleanup older than {DaysOlder} folders from backup directory.
        /// If Create Subdirectories is false, individual files will also get cleaned.
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
