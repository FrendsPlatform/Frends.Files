# Frends.File
FRENDS Tasks to preform various file system based operations.

- [Frends.File](#frendsfile)
- [Installing](#installing)
- [Documentation](#documentation)
  - [Pattern matching](#pattern-matching)
- [Tasks](#tasks)
  - [Read](#read)
- [License](#license)
- [Building](#building)
- [Contributing](#contributing)

Installing
==========
You can install the task via FRENDS UI Task view, by searching for packages. You can also download the latest NuGet package from https://www.myget.org/feed/frends/package/nuget/Frends.File and import it manually via the Task view.

Documentation
===============

Tasks
=====

## Read
File.Read task reads the string contents of one file.

Input:

| Property        | Type     | Description                      | Example                             |
|-----------------|----------|----------------------------------|-------------------------------------|
| Path            | string   | Full path to the file to be read.| `c:\temp\foo.txt` `c:/temp/foo.txt` |

Options:

| Property                                    | Type           | Description                                    | Example                   |
|---------------------------------------------|----------------|------------------------------------------------|---------------------------|
| UseGivenUserCredentialsForRemoteConnections | bool           | If set, allows you to give the user credentials to use to read files on remote hosts. If not set, the agent service user credentials will be used. Note: For deleting directories on the local machine, the agent service user credentials will always be used, even if this option is set.| |
| UserName                                    | string         | Needs to be of format `domain\username`        | `example\Admin`           |
| Password                                    | string         | | |
| FileEncoding                                | Enum           | Encoding for the read content. By selecting 'Other' you can use any encoding. | |
| EncodingInString                            | string         | The name of encoding to use. Required if the FileEncoding choice is 'Other'. A partial list of supported encoding names: https://msdn.microsoft.com/en-us/library/system.text.encoding.getencodings(v=vs.110).aspx | `iso-8859-1` |

Result:

| Property        | Type     | Description                 |
|-----------------|----------|-----------------------------|
| Content         | string   |                             |
| Path            | string   | Full path for the read file |
| SizeInMegaBytes | double   |                             |
| CreationTime    | DateTime |                             |
| LastWriteTime   | DateTime |                             |

License
=======

This project is licensed under the MIT License - see the LICENSE file for details

Building
========

Clone a copy of the repo

`git clone https://github.com/FrendsPlatform/Frends.File.git`

Restore dependencies

`dotnet restore`

Rebuild the project

`dotnet build`

Run Tests

`dotnet test Frends.File.Tests`

Create a nuget package

`dotnet pack Frends.File`

Contributing
============
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!
