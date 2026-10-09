# Changelog

## [2.0.0] - 2026-10-09
### Changed
- The Task now targets .NET 8.
- Breaking: By default the Task now throws an error when reading fails. Set the new `ThrowErrorOnFailure` option to `false` to receive a failed result instead.

### Added
- New options `ThrowErrorOnFailure` and `ErrorMessageOnFailure` for controlling error handling.
- Result now contains `Success` and `Error` properties.

## [1.2.0] - 2026-05-08
### Fixed
- Fix documentation of result 
  
## [1.1.0] - 2025-03-19
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

## [1.0.0] - 2023-04-20
### Added
- Initial implementation
