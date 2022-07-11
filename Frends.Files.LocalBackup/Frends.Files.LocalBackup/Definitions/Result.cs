using System.Collections.Generic;

namespace Frends.Files.LocalBackup.Definitions;
/// <summary>
/// Backup and cleanup results.
/// </summary>
public class Result
{

    /// <summary>
    /// Backup directory.
    /// </summary>
    /// <example>C:\\directory\\backup\\2022-07-07_08_51_02-e7e34166-f4fd-45e5-9307-ea5c2cf8e037</example>
    public string Directory { get; private set; }

    /// <summary>
    /// Count of files which were copied to backup directory.
    /// </summary>
    public int FileCountInBackup { get; private set; }

    /// <summary>
    /// Backup results.
    /// </summary>
    /// <example>
    /// "Backups": [
	///     {
	///	        "Backup": "Backup complete: C:\\test\\localbackup\\test - Copy (2).txt to C:\\test\\backup\\2022-07-07_08_51_02-e7e34166-f4fd-45e5-9307-ea5c2cf8e037\\test - Copy (2).txt",
	///	        "Cleanup": null
	///     },
	///     {
	///	        "Backup": "Backup complete: C:\\test\\localbackup\\test - Copy (3).txt to C:\\test\\backup\\2022-07-07_08_51_02-e7e34166-f4fd-45e5-9307-ea5c2cf8e037\\test - Copy (3).txt",
	///	        "Cleanup": null
	///     }
    /// ]
    /// </example>
    public List<BackupObject> Backups { get; private set; }

    /// <summary>
    /// Cleanup results.
    /// </summary>
    /// <example>
    /// "Cleanups: [
	///     {
	///	        "Backup": null,
	///	        "Cleanup": "C:\\test\\backup\\2022-07-11_05_44_41-ab214387-98f0-44de-8ec5-16ba01b8ab97 deleted."
	///     },
    ///     {
	///	        "Backup": null,
	///	        "Cleanup": "C:\\test\\backup\\2022-06-25_06_23_56-ab214387-98f0-44de-8ec5-16ba01b8ab97 deleted."
	///     }
    /// ]
    /// </example>
    public List<BackupObject> Cleanups { get; private set; }

    internal Result(string directory, int fileCount, List<BackupObject> backups, List<BackupObject> cleanups)
    {
        Directory = directory;
        FileCountInBackup = fileCount;
        Backups = backups;
        Cleanups = cleanups;
    }

    internal Result()
    {

    }
}


