# DotNetSdkHelpers

[![NuGet](https://img.shields.io/nuget/v/DotNetSdkHelpers)](https://www.nuget.org/packages/DotNetSdkHelpers/)

A shameless .NET Core CLI tool port of [.NET Core SDK CLI Helpers](https://github.com/faniereynders/dotnet-sdk-helpers)

This global tool helps you manage the set of installed .NET Core SDK versions and get new ones as needed.

## Installation

```bash
# Install global tool
dotnet tool install -g dotnetsdkhelpers

# Get started!
dotnet sdk -h
```

## Help and Usage

```text
Manage .NET Core SDKs

Usage: dotnet-sdk [options] [command]

Options:
  -?|-h|--help  Show help information

Commands:
  download      Downloads the provided release version & platform.
  list          Lists .NET Core SDKs
  set           Switches to the specified .NET Core SDK version

Run 'dotnet-sdk [command] --help' for more information about a command.
```

### dotnet sdk download

```text
Downloads the provided release version & platform.

Usage: dotnet-sdk download [options] <Version>

Arguments:
  Version                   'lts', 'current', 'preview' or a specific version. (Default: 'current')

Options:
  -r|--runtime              Indicate the version specified is the runtime version, NOT the SDK version.
  -p|--platform <PLATFORM>  The platform to download for. Defaults to the current platform on Windows and MacOS
  -n|--no-hash-validation   Indicate that validation of hash should NOT be done.
  -?|-h|--help              Show help information
```

### dotnet sdk list

```text
Lists .NET Core SDKs

Usage: dotnet-sdk list [options] <Filter>

Arguments:
  Filter             Filters list to only SDKs starting with the provided string.

Options:
  -a|--available     List SDKs available to download; default is to list installed SDKs.
  -l|--lts-only      Show available LTS versions only.
  -p|--preview-only  Show available preview versions only.
  --all              Show all available versions including all previews and patch versions.
  -?|-h|--help       Show help information
```

### dotnet sdk set

```text
Switches to the specified .NET Core SDK version

Usage: dotnet-sdk set [options] <Version>

Arguments:
  Version       'stable', 'preview' or a specific version

Options:
  -?|-h|--help  Show help information
```
