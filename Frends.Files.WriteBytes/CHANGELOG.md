# Changelog

## [1.2.0] - 2026-10-14
### Changed
- Upgraded the Task to target .NET 8.
- The Task now requires a CancellationToken, which Frends provides automatically, and can be cancelled while writing.
- The Task now throws an error on failure by default. Set `ThrowErrorOnFailure` to false in Options to receive a result with `Success` set to false and error details in `Error` instead.
### Added
- Added `Success` and `Error` properties to the result.
- Added `ThrowErrorOnFailure` and `ErrorMessageOnFailure` options.

### Added
- New options `ThrowErrorOnFailure` and `ErrorMessageOnFailure` to control whether failures throw an error or are returned in the result with a custom message.

## [1.1.0] - 2025-03-19
### Changed
- Update packages:
  Microsoft.Extensions.FileSystemGlobbing  7.0.0 -> 9.0.3
  System.ComponentModel.Annotations        4.7.0 -> 5.0.0
  System.DirectoryServices                 7.0.0 -> 8.0.0

## [1.0.1] - 2023-08-10
### Fixed
- Fixed memory leak from ExecuteWriteBytes by adding using statement when opening a new MewmoryStream.

## [1.0.0] - 2023-04-17
### Added
- Initial implementation