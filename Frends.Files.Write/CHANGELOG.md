# Changelog

## [1.3.0] - 2025-06-05
### Fixed
- Resolve issue with an unsupported library in .net 6.
### Changed
- Downgrade package System.Text.Encoding.CodePages from 9.0.3 to 8.0.0
- Remove unused package Microsoft.Extensions.FileSystemGlobbing

## [1.2.0] - 2025-03-19
### Changed
- Update packages:
  Microsoft.Extensions.FileSystemGlobbing  7.0.0  -> 9.0.3
  System.ComponentModel.Annotations        4.7.0  -> 5.0.0
  System.DirectoryServices                 7.0.0  -> 8.0.0
  System.Text.Encoding.CodePages           8.0.0  -> 9.0.3
  coverlet.collector                       3.1.0  -> 6.0.4
  Microsoft.NET.Test.Sdk                   16.6.1 -> 17.13.0
  MSTest.TestAdapter                       2.2.7  -> 3.8.3
  MSTest.TestFramework                     2.2.8  -> 3.8.3
  nunit                                    3.12.0 -> 4.3.2
  NUnit3TestAdapter                        3.17.0 -> 5.0.0

## [1.1.0] - 2024-10-14
### Fixed
- Renamed ANSI encoding to Default.
### Added
- Added result properties SizeInKiloBytes and SizeInBytes.

## [1.0.1] - 2024-02-27
### Added
- Windows-1252 added to "Encoding" options.

## [1.0.0] - 2023-02-15
### Added
- Initial implementation