# ILSpy [![Join the chat at https://gitter.im/icsharpcode/ILSpy](https://badges.gitter.im/icsharpcode/ILSpy.svg)](https://gitter.im/icsharpcode/ILSpy?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![NuGet](https://img.shields.io/nuget/v/ICSharpCode.Decompiler.svg)](https://nuget.org/packages/ICSharpCode.Decompiler) [![Build status](https://ci.appveyor.com/api/projects/status/imgec05g0wwv25ij/branch/master?svg=true)](https://ci.appveyor.com/project/icsharpcode/ilspy/branch/master) [![Twitter Follow](https://img.shields.io/twitter/follow/ILSpy.svg?label=Follow%20@ILSpy)](https://twitter.com/ilspy) [![ILSpy VS extension](https://img.shields.io/badge/VS%20Extension-ILSpy-blue.svg)](https://visualstudiogallery.msdn.microsoft.com/8ef1d688-f80c-4380-8004-2ec7f814e7de) [![Build Status](https://icsharpcode.visualstudio.com/icsharpcode-pipelines/_apis/build/status/icsharpcode.ILSpy?branchName=master)](https://icsharpcode.visualstudio.com/icsharpcode-pipelines/_build/latest?definitionId=1&branchName=master)

ILSpy is the open-source .NET assembly browser and decompiler.

Download: [latest release](https://github.com/icsharpcode/ILSpy/releases) | [latest CI build (master)](https://ci.appveyor.com/api/projects/icsharpcode/ilspy/artifacts/ILSpy_binaries.zip?branch=master&job=Configuration%3A+Release)

CI Build Nuget Feed (master): https://ci.appveyor.com/nuget/ilspy-masterfeed

Decompiler Frontends
-------

Aside from the WPF UI ILSpy (downloadable via Releases, see also [plugins](https://github.com/icsharpcode/ILSpy/wiki/Plugins)), the following other frontends are available:

* Visual Studio 2017 extension [marketplace](https://marketplace.visualstudio.com/items?itemName=SharpDevelopTeam.ILSpy)
* Visual Studio Code Extension [repository](https://github.com/icsharpcode/ilspy-vscode) | [marketplace](https://marketplace.visualstudio.com/items?itemName=icsharpcode.ilspy-vscode)
* [ICSharpCode.Decompiler](https://www.nuget.org/packages/ICSharpCode.Decompiler/) NuGet for your own projects
* Linux/Mac/Windows ILSpy UI based on [Avalonia](http://www.avaloniaui.net/) - check out https://github.com/icsharpcode/AvaloniaILSpy
* Linux/Mac/Windows command line client - check out [ICSharpCode.Decompiler.Console](ICSharpCode.Decompiler.Console) in this repository
* Linux/Mac/Windows [PowerShell cmdlets](ICSharpCode.Decompiler.PowerShell) in this repository

Features
-------

 * Decompilation to C#
 * Whole-project decompilation (csproj, not sln!)
 * Search for types/methods/properties (substring)
 * Hyperlink-based type/method/property navigation
 * Base/Derived types navigation, history
 * BAML to XAML decompiler
 * Extensible via [plugins](https://github.com/icsharpcode/ILSpy/wiki/Plugins) (MEF)
 * Check out the [language support status](https://github.com/icsharpcode/ILSpy/issues/829)

License
-------

ILSpy is distributed under the MIT License.

Included open-source libraries:
 * Mono.Cecil: MIT License (part of ILSpy)
 * LightJson: MIT License (part of ICSharpCode.Decompiler)
 * Humanizer: MIT License (part of ICSharpCode.Decompiler)
 * AvalonEdit: MIT License
 * SharpTreeView: LGPL
 * ILSpy.BamlDecompiler: MIT license
 * CommandLineUtils: Apache License 2.0 (part of ICSharpCode.Decompiler.Console)

How to build
------------

Windows:
- Install Visual Studio (minimum version: 2019.2) with the following components:
  - Workload ".NET Desktop Development"
  - .NET Framework 4.6.2 Targeting Pack (if the VS installer does not offer this option, install the [.NET 4.6.2 developer pack](https://www.microsoft.com/en-us/download/details.aspx?id=53321) separately)
  - Individual Component "MSVC v142 - VS 2019 C++ x64/x86 build tools (v14.22)" (or similar)
    - The VC++ toolset is optional; if present it is used for `editbin.exe` to modify the stack size used by ILSpy.exe from 1MB to 16MB, because the decompiler makes heavy use of recursion, where small stack sizes lead to problems in very complex methods.
- Install the [.NET Core SDK 2.2](https://dotnet.microsoft.com/download)
- Install the [.NET Core SDK 3](https://dotnet.microsoft.com/download/dotnet-core)
- Check out the ILSpy repository using git.
- Execute `git submodule update --init --recursive` to download the ILSpy-Tests submodule (used by some test cases).
- Open ILSpy.sln in Visual Studio.
  - NuGet package restore will automatically download further dependencies
  - Run project "ILSpy" for the ILSpy UI
  - Use the Visual Studio "Test Explorer" to see/run the tests

Unix:
- Make sure .NET Core 2.2 is installed (you can get it here: https://get.dot.net).
- Make sure [.NET Core SDK 3](https://dotnet.microsoft.com/download/dotnet-core) is installed.
- Check out the repository using git.
- Execute `git submodule update --init --recursive` to download the ILSpy-Tests submodule (used by some test cases).
- Use `dotnet build Frontends.sln` to build the non-Windows flavors of ILSpy (cli and powershell core).

(Visual Studio for Mac users only:)
- Edit `\ICSharpCode.Decompiler\ICSharpCode.Decompiler.csproj`
  Add `Sdk="Microsoft.NET.Sdk"` to the `Project` element.
  This is required due to a tooling issue.
  Please do not commit this when contributing a pull request!
- Use Frontends.sln to work.

How to contribute
-----------------

- Report bugs
- If you want to contribute a pull request, please add https://gist.github.com/siegfriedpammer/75700ea61609eb22714d21885e4eb084 to your `.git/hooks` to prevent checking in code with wrong indentation. We use tabs and not spaces. The build server runs the same script, so any pull requests using wrong indentation will fail.

Current and past [contributors](https://github.com/icsharpcode/ILSpy/graphs/contributors).
