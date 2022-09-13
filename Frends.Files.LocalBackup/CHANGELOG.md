# Changelog

## [2.0.1] - 2022-09-13
### Added
- Added CreateSubdirectories input.
- Cleanup change: Directory to be deleted is determined by timestamp in directory name when CreateSubdirectories is true, else by directory's LastWriteTime value.

## [2.0.0] - 2022-07-11
### Added
- Modified the task to use #process.executionid instead of a random guid
- [Breaking change] Added input for TaskExecutionId as guid
- Added backup directory value to Result object
- Added file count how many were made backup of
- [Breaking change] Removed BackupObject class and changed the result object to have list of string for backup and cleanup

## [1.0.0] - 2022-05-16
### Added
- Initial implementation