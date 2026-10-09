# Changelog

## [2.1.0] - 2026-10-05
### Changed
- Expanded the result with `Success` and `Error` fields to report whether file discovery succeeded and provide failure details.
- Added `ThrowErrorOnFailure` and `ErrorMessageOnFailure` options to choose whether failures are returned in the result or thrown, and to set a custom failure message.
- Added cancellation token support to stop file discovery when cancellation is requested before it starts.

## [2.0.0] - 2026-09-10
### Changed
- Upgraded the task to .NET 8.
- [Breaking Change] Standardize pattern matching logic between tasks.
- Clarified relative input matching rules: patterns are evaluated relative to Directory, separators are normalized to /, matching is case-insensitive, glob mode supports * and **, and &lt;regex&gt; enables regex evaluation against the relative path and file name.

## [1.1.0] - 2025-03-19
### Changed
- Update packages:
  Microsoft.Extensions.FileSystemGlobbing  7.0.0  -> 9.0.3
  System.DirectoryServices                 7.0.0  -> 8.0.0
  coverlet.collector                       3.1.0  -> 6.0.4
  Microsoft.NET.Test.Sdk                   16.6.1 -> 17.13.0
  MSTest.TestAdapter                       2.2.7  -> 3.8.3
  MSTest.TestFramework                     2.2.8  -> 3.8.3
  nunit                                    3.12.0 -> 4.3.2
  NUnit3TestAdapter                        3.17.0 -> 5.0.0

## [1.0.1] - 2023-12-13
### Changed
- Result documentation updated

## [1.0.0] - 2023-03-07
### Added
- Initial implementation