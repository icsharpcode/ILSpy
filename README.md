# ILSpy [![Join the chat at https://gitter.im/icsharpcode/ILSpy](https://badges.gitter.im/icsharpcode/ILSpy.svg)](https://gitter.im/icsharpcode/ILSpy?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![NuGet](https://img.shields.io/nuget/v/ICSharpCode.Decompiler.svg)](https://nuget.org/packages/ICSharpCode.Decompiler) [![Build status](https://ci.appveyor.com/api/projects/status/imgec05g0wwv25ij/branch/master?svg=true)](https://ci.appveyor.com/project/icsharpcode/ilspy/branch/master) [![Twitter Follow](https://img.shields.io/twitter/follow/ILSpy.svg?label=Follow%20@ILSpy)](https://twitter.com/ilspy) [![ilspy.net](https://img.shields.io/badge/@-ilspy.net-blue.svg)](http://www.ilspy.net)  [![ILSpy VS extension](https://img.shields.io/badge/VS%20Extension-ILSpy-blue.svg)](https://visualstudiogallery.msdn.microsoft.com/8ef1d688-f80c-4380-8004-2ec7f814e7de)

ILSpy is the open-source .NET assembly browser and decompiler.

Download: [latest release](https://github.com/icsharpcode/ILSpy/releases) | [latest CI build (master)](https://ci.appveyor.com/api/projects/icsharpcode/ilspy/artifacts/ILSpy_binaries.zip?branch=master&job=Configuration%3A+Release)

Decompiler Frontends
-------

Aside from the WPF UI ILSpy (downloadable via Releases, see also [plugins](https://github.com/icsharpcode/ILSpy/wiki/Plugins)), the following other frontends are available:

* Linux/Mac/Windows command line client - check out [ICSharpCode.Decompiler.Console](ICSharpCode.Decompiler.Console) in this repository
* Linux/Mac/Windows [PowerShell cmdlets](ICSharpCode.Decompiler.PowerShell) in this repository
* Visual Studio Code Extension [repository](https://github.com/icsharpcode/ilspy-vscode) | [marketplace](https://marketplace.visualstudio.com/items?itemName=icsharpcode.ilspy-vscode)
* [ICSharpCode.Decompiler](https://www.nuget.org/packages/ICSharpCode.Decompiler/) NuGet for your own projects

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
 * Mono.Cecil: MIT License (part of ICSharpCode.Decompiler)
 * LightJson: MIT License (part of ICSharpCode.Decompiler)
 * Humanizer: MIT License (part of ICSharpCode.Decompiler)
 * AvalonEdit: MIT License
 * SharpTreeView: LGPL
 * Ricciolo.StylesExplorer: MS-PL (part of ILSpy.BamlDecompiler.Plugin)
 * CommandLineUtils: Apache License 2.0 (part of ICSharpCode.Decompiler.Console)

How to build
------------

Windows:
- Check out the repository using git.
- Execute `git submodule update --init --recursive` to get all required submodules.
- Use ILSpy.sln to work.

Unix:
- Check out the repository using git.
- Execute `git submodule update --init --recursive` to get all required submodules.
- Edit `\ICSharpCode.Decompiler\ICSharpCode.Decompiler.csproj`
  Add `Sdk="Microsoft.NET.Sdk"` to the `Project` element.
  This is required due to a tooling issue on Unix.
  Please do not commit this when contributing a pull request!
- Use ICSharpCode.Decompiler.Console.sln to work.

How to contribute
-----------------

- Report bugs
- If you want to contribute a pull request, please add https://gist.github.com/siegfriedpammer/75700ea61609eb22714d21885e4eb084 to your `.git/hooks` to prevent checking in code with wrong indentation. We use tabs and not spaces. The build server runs the same script, so any pull requests using wrong indentation will fail.

Current and past [contributors](https://github.com/icsharpcode/ILSpy/graphs/contributors).