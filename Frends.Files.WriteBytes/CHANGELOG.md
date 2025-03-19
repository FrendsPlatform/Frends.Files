# Changelog

## [1.0.2] - 2025-03-19
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