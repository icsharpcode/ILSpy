# ILSpy [![NuGet](https://img.shields.io/nuget/v/ICSharpCode.Decompiler.svg)](https://nuget.org/packages/ICSharpCode.Decompiler) ![Build ILSpy](https://github.com/icsharpcode/ILSpy/workflows/Build%20ILSpy/badge.svg?branch=master) [![Twitter Follow](https://img.shields.io/twitter/follow/ILSpy.svg?label=Follow%20@ILSpy)](https://twitter.com/ilspy) [![ILSpy VS extension](https://img.shields.io/badge/VS%20Extension-ILSpy-blue.svg)](https://visualstudiogallery.msdn.microsoft.com/8ef1d688-f80c-4380-8004-2ec7f814e7de) 

ILSpy is the open-source .NET assembly browser and decompiler.

Download: [latest release](https://github.com/icsharpcode/ILSpy/releases) | [latest CI build (master)](https://github.com/icsharpcode/ILSpy/actions?query=workflow%3A%22Build+ILSpy%22+branch%3Amaster+is%3Asuccess+event%3Apush) | [Microsoft Store (RC & RTM versions only)](https://www.microsoft.com/store/apps/9MXFBKFVSQ13)

Decompiler Frontends
-------

Aside from the WPF UI ILSpy (downloadable via Releases, see also [plugins](https://github.com/icsharpcode/ILSpy/wiki/Plugins)), the following other frontends are available:

* Visual Studio 2022 ships with decompilation support for F12 enabled by default (using our engine v7.1).
* In Visual Studio 2019, you have to manually enable F12 support. Go to Tools / Options / Text Editor / C# / Advanced and check "Enable navigation to decompiled source"
* [C# for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) ships with decompilation support as well. To enable, activate the setting "Enable Decompilation Support".
* Our Visual Studio 2022 extension [marketplace](https://marketplace.visualstudio.com/items?itemName=SharpDevelopTeam.ILSpy2022)
* Our Visual Studio 2017/2019 extension [marketplace](https://marketplace.visualstudio.com/items?itemName=SharpDevelopTeam.ILSpy)
* Our Visual Studio Code Extension [repository](https://github.com/icsharpcode/ilspy-vscode) | [marketplace](https://marketplace.visualstudio.com/items?itemName=icsharpcode.ilspy-vscode)
* Our Linux/Mac/Windows ILSpy UI based on [Avalonia](http://www.avaloniaui.net/) - check out https://github.com/icsharpcode/AvaloniaILSpy
* Our [ICSharpCode.Decompiler](https://www.nuget.org/packages/ICSharpCode.Decompiler/) NuGet for your own projects
* Our dotnet tool for Linux/Mac/Windows - check out [ILSpyCmd](ICSharpCode.ILSpyCmd) in this repository
* Our Linux/Mac/Windows [PowerShell cmdlets](ICSharpCode.Decompiler.PowerShell) in this repository

Features
-------

 * Decompilation to C# (check out the [language support status](https://github.com/icsharpcode/ILSpy/issues/829))
 * Whole-project decompilation (csproj, not sln!)
 * Search for types/methods/properties (learn about the [options](https://github.com/icsharpcode/ILSpy/wiki/Search-Options))
 * Hyperlink-based type/method/property navigation
 * Base/Derived types navigation, history
 * Assembly metadata explorer ([feature walkthrough](https://github.com/icsharpcode/ILSpy/wiki/Metadata-Explorer))
 * BAML to XAML decompiler
 * ReadyToRun binary support for .NET Core (see the [tutorial](https://github.com/icsharpcode/ILSpy/wiki/ILSpy.ReadyToRun))
 * Extensible via [plugins](https://github.com/icsharpcode/ILSpy/wiki/Plugins)
 * Additional features in DEBUG builds ([for the devs](https://github.com/icsharpcode/ILSpy/wiki/Additional-Features-in-DEBUG-Builds))

License
-------

ILSpy is distributed under the MIT License. Please see the [About](doc/ILSpyAboutPage.txt) doc for details, 
as well as [third party notices](doc/third-party-notices.txt) for included open-source libraries.

How to build
------------

#### Windows:

- Make sure PowerShell (at least version) 5.0 is installed.
- Clone the ILSpy repository using git.
- Execute `git submodule update --init --recursive` to download the ILSpy-Tests submodule (used by some test cases).
- Install Visual Studio (documented version: 17.1). You can install the necessary components in one of 3 ways:
  - Follow Microsoft's instructions for [importing a configuration](https://docs.microsoft.com/en-us/visualstudio/install/import-export-installation-configurations?view=vs-2022#import-a-configuration), and import the .vsconfig file located at the root of the solution.
  - Alternatively, you can open the ILSpy solution (ILSpy.sln) and Visual Studio will [prompt you to install the missing components](https://docs.microsoft.com/en-us/visualstudio/install/import-export-installation-configurations?view=vs-2022#automatically-install-missing-components).
  - Finally, you can manually install the necessary components via the Visual Studio Installer. The workloads/components are as follows:
    - Workload ".NET Desktop Development". This workload includes the .NET Framework 4.8 SDK and the .NET Framework 4.7.2 targeting pack, as well as the [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) (ILSpy.csproj targets .NET 6.0, but we have net472 projects too). _Note: The optional components of this workload are not required for ILSpy_
    - Workload "Visual Studio extension development" (ILSpy.sln contains a VS extension project) _Note: The optional components of this workload are not required for ILSpy_
    - Individual Component "MSVC v143 - VS 2022 C++ x64/x86 build tools" (or similar)
      - _The VC++ toolset is optional_; if present it is used for `editbin.exe` to modify the stack size used by ILSpy.exe from 1MB to 16MB, because the decompiler makes heavy use of recursion, where small stack sizes lead to problems in very complex methods.
  - Open ILSpy.sln in Visual Studio.
    - NuGet package restore will automatically download further dependencies
    - Run project "ILSpy" for the ILSpy UI
    - Use the Visual Studio "Test Explorer" to see/run the tests
    - If you are only interested in a specific subset of ILSpy, you can also use
      - ILSpy.Wpf.slnf: for the ILSpy WPF frontend
      - ILSpy.XPlat.slnf: for the cross-platform CLI or PowerShell cmdlets
      - ILSpy.AddIn.slnf: for the Visual Studio plugin

**Note:** Visual Studio 16.3 and later include a version of the .NET (Core) SDK that is managed by the Visual Studio installer - once you update, it may get upgraded too.
Please note that ILSpy is only compatible with the .NET 6.0 SDK and Visual Studio will refuse to load some projects in the solution (and unit tests will fail). 
If this problem occurs, please manually install the .NET 6.0 SDK from [here](https://dotnet.microsoft.com/download/dotnet/6.0).

#### Unix / Mac:

- Make sure [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) is installed.
- Make sure [PowerShell](https://github.com/PowerShell/PowerShell) is installed (formerly known as PowerShell Core)
- Clone the repository using git.
- Execute `git submodule update --init --recursive` to download the ILSpy-Tests submodule (used by some test cases).
- Use `dotnet build ILSpy.XPlat.slnf` to build the non-Windows flavors of ILSpy (.NET Core Global Tool and PowerShell Core).

How to contribute
-----------------

- Report bugs
- If you want to contribute a pull request, please add https://gist.github.com/siegfriedpammer/75700ea61609eb22714d21885e4eb084 to your `.git/hooks` to prevent checking in code with wrong indentation. We use tabs and not spaces. The build server runs the same script, so any pull requests using wrong indentation will fail.

Current and past [contributors](https://github.com/icsharpcode/ILSpy/graphs/contributors).

Privacy Policy for ILSpy
------------------------

ILSpy does not collect any personally identifiable information, nor does it send user files to 3rd party services. 
ILSpy does not use any APM (Application Performance Management) service to collect telemetry or metrics.
