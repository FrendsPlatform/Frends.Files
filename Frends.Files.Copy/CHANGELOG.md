# Changelog

# [1.2.0] - 2025-04-08
### Added
- New option ThrowErrorOnFail to allow task to continue despite some files failing to copy.
	- Files that failed to copy will be listed in the Result.FailedFiles property.
	- When set to false and IfTargetFileExists is set to Throw, the task will continue copying other files despite a error.

# [1.1.0] - 2025-03-13
### Fixed
- Fixed bug where regex patterns were not handled correctly in file matching.

# [1.0.2] - 2023-11-27
### Changed
- Documentational changes.

# [1.0.1] - 2023-09-21
### Fixed
- Fixed bug with file access. Changed File.Open to use FileAccess.Read, because there's no reason to give more access to the Task.

## [1.0.0] - 2023-02-14
### Added
- Initial implementation