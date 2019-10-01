# ILSpy [![Join the chat at https://gitter.im/icsharpcode/ILSpy](https://badges.gitter.im/icsharpcode/ILSpy.svg)](https://gitter.im/icsharpcode/ILSpy?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![NuGet](https://img.shields.io/nuget/v/ICSharpCode.Decompiler.svg)](https://nuget.org/packages/ICSharpCode.Decompiler) [![Build status](https://ci.appveyor.com/api/projects/status/imgec05g0wwv25ij/branch/master?svg=true)](https://ci.appveyor.com/project/icsharpcode/ilspy/branch/master) [![Twitter Follow](https://img.shields.io/twitter/follow/ILSpy.svg?label=Follow%20@ILSpy)](https://twitter.com/ilspy) [![ILSpy VS extension](https://img.shields.io/badge/VS%20Extension-ILSpy-blue.svg)](https://visualstudiogallery.msdn.microsoft.com/8ef1d688-f80c-4380-8004-2ec7f814e7de) [![Build Status](https://icsharpcode.visualstudio.com/icsharpcode-pipelines/_apis/build/status/icsharpcode.ILSpy?branchName=master)](https://icsharpcode.visualstudio.com/icsharpcode-pipelines/_build/latest?definitionId=1&branchName=master)

ILSpy is the open-source .NET assembly browser and decompiler.

Download: [latest release](https://github.com/icsharpcode/ILSpy/releases) | [latest CI build (master)](https://ci.appveyor.com/api/projects/icsharpcode/ilspy/artifacts/ILSpy_binaries.zip?branch=master&job=Configuration%3A+Release) | [Microsoft Store (RC & RTM versions only)](https://www.microsoft.com/store/apps/9MXFBKFVSQ13)

CI Build Nuget Feed (master): https://ci.appveyor.com/nuget/ilspy-masterfeed

Decompiler Frontends
-------

Aside from the WPF UI ILSpy (downloadable via Releases, see also [plugins](https://github.com/icsharpcode/ILSpy/wiki/Plugins)), the following other frontends are available:

* Visual Studio 2017/2019 extension [marketplace](https://marketplace.visualstudio.com/items?itemName=SharpDevelopTeam.ILSpy)
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
- Install Visual Studio (documented version: 16.3) with the following components:
  - Workload ".NET Desktop Development". This includes by default .NET Framework 4.8 SDK and the .NET Framework 4.7.2 targeting pack, as well as the .NET Core 3 SDK (ILSpy.csproj targets .NET 4.7.2, and ILSpy.sln uses SDK-style projects).
  - Workload "Visual Studio extension development" (ILSpy.sln contains a VS extension project)
  - Individual Component "MSVC v142 - VS 2019 C++ x64/x86 build tools (v14.23)" (or similar)
    - The VC++ toolset is optional; if present it is used for `editbin.exe` to modify the stack size used by ILSpy.exe from 1MB to 16MB, because the decompiler makes heavy use of recursion, where small stack sizes lead to problems in very complex methods.
- Check out the ILSpy repository using git.
- Execute `git submodule update --init --recursive` to download the ILSpy-Tests submodule (used by some test cases).
- Open ILSpy.sln in Visual Studio.
  - NuGet package restore will automatically download further dependencies
  - Run project "ILSpy" for the ILSpy UI
  - Use the Visual Studio "Test Explorer" to see/run the tests

Unix / Mac:
- Make sure .NET Core 2.1 LTS Runtime is installed (you can get it here: https://get.dot.net).
- Make sure [.NET Core 3 SDK](https://dotnet.microsoft.com/download/dotnet-core) is installed.
- Check out the repository using git.
- Execute `git submodule update --init --recursive` to download the ILSpy-Tests submodule (used by some test cases).
- Use `dotnet build Frontends.sln` to build the non-Windows flavors of ILSpy (.NET Core Global Tool and PowerShell Core).

How to contribute
-----------------

- Report bugs
- If you want to contribute a pull request, please add https://gist.github.com/siegfriedpammer/75700ea61609eb22714d21885e4eb084 to your `.git/hooks` to prevent checking in code with wrong indentation. We use tabs and not spaces. The build server runs the same script, so any pull requests using wrong indentation will fail.

Current and past [contributors](https://github.com/icsharpcode/ILSpy/graphs/contributors).

Privacy Policy for ILSpy
------------------------

ILSpy does not collect any personally identifiable information, nor does it send user files to 3rd party services. 
ILSpy does not use any APM (Application Performance Management) service to collect telemetry or metrics.
