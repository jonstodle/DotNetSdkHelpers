# DotNetSdkHelpers

[![NuGet](https://img.shields.io/nuget/v/DotNetSdkHelpers)](https://www.nuget.org/packages/DotNetSdkHelpers/)

A shameless .NET Core CLI tool port of [.NET Core SDK CLI Helpers](https://github.com/faniereynders/dotnet-sdk-helpers)

This global tool helps you manage the set of installed .NET Core SDK versions and get new ones as needed.

## Installation

```bash
# Install global tool
dotnet tool install -g dotnetsdkhelpers

# Get started!
dotnet-sdk -h
```

## Help and Usage

```text
Manage .NET Core SDKs

Usage: dotnet-sdk [command] [options]

Options:
  -?|-h|--help  Show help information.

Commands:
  download      Downloads the provided release version & platform.
  list          Lists .NET Core SDKs
  set           Creates a global.json to switch to the specified .NET Core SDK version

Run 'dotnet-sdk [command] -?|-h|--help' for more information about a command.
```

### dotnet-sdk download

```text
Downloads the provided release version & platform.

Usage: dotnet-sdk download [options] <Version>

Arguments:
  Version                   'active', 'go-live', 'preview' or a specific version. (Default: 'active')
                            Default value is: active.

Options:
  -r|--runtime              Indicate the version specified is the runtime version, NOT the SDK version.
  -p|--platform <PLATFORM>  The platform to download for. Defaults to the current platform on Windows and MacOS.
  -n|--no-hash-validation   Indicate that validation of hash should NOT be done.
  -?|-h|--help              Show help information.
```

### dotnet-sdk list

```text
Lists .NET Core SDKs

Usage: dotnet-sdk list [options] <Filter>

Arguments:
  Filter              Filters list to only SDKs starting with the provided string.

Options:
  -a|--available      List SDKs available to download; default is to list installed SDKs.
  -r|--release-type   Show available versions matching the specified release types.
                      Allowed values are: sts, lts.
  -s|--support-phase  Show available versions matching the specified support phases.
                      Allowed values are: preview, go-live, active, maintenance, eol.
  --all               Show all available versions including all support phases and release types.
  -?|-h|--help        Show help information.
```

### dotnet-sdk set

```text
Switches to the specified .NET Core SDK version

Usage: dotnet-sdk set [options] <Version>

Arguments:
  Version       'stable', 'preview' or a specific version

Options:
  -?|-h|--help  Show help information
```
