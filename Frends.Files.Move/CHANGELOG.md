# Changelog

## [2.0.0] - 2025-10-14
### Fixed
- Fixed issue with impersonation when moving files between remote places.
- Fixed issue with PreserveDirectoryStructure when regex patterns were used.
### Changed
- Added options to handle errors
- Add Success and Error parameters to Result object
- Upgrade target framework to net8.0
    #### Breaking Changes
  - Moved parameters related to impersonation to new Connection tab
  - Renamed input Directory to SourceDirectory

## [1.4.0] - 2025-08-12
### Changed
- Refactor ExecuteAction to run synchronously for impersonated calls.

## [1.3.0] - 2025-03-19
### Changed
- Update packages:
  Microsoft.Extensions.FileSystemGlobbing  7.0.0  -> 9.0.3
  System.ComponentModel.Annotations        4.7.0  -> 5.0.0
  System.DirectoryServices                 7.0.0  -> 8.0.0
  coverlet.collector                       3.1.0  -> 6.0.4
  Microsoft.NET.Test.Sdk                   16.6.1 -> 17.13.0
  MSTest.TestAdapter                       2.2.7  -> 3.8.3
  MSTest.TestFramework                     2.2.8  -> 3.8.3
  nunit                                    3.12.0 -> 4.3.2
  NUnit3TestAdapter                        3.17.0 -> 5.0.0

## [1.2.0] - 2025-03-13
### Fixed
- Fixed bug where regex patterns were not handled correctly in file matching.

## [1.1.0] - 2024-11-1
### Fixed
- Fixed issue with Task not working with impersonation by removing the source file in the main method.

## [1.0.0] - 2023-02-14
### Added
- Initial implementation