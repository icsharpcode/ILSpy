---
layout: default
title: ILSpy
subtitle: .NET Decompiler
binaryDownload: https://github.com/icsharpcode/ILSpy/releases/download/v2.4/ILSpy_Master_2.4.0.1963_Binaries.zip
sourceDownload: https://github.com/icsharpcode/ILSpy/archive/v2.4.zip
---

ILSpy is the open-source .NET assembly browser and decompiler.

Development started after Red Gate [announced](http://www.red-gate.com/products/dotnet-development/reflector/announcement) that the free version of .NET Reflector would cease to exist by end of February 2011.

ILSpy requires the [.NET Framework 4.0](http://www.microsoft.com/downloads/en/details.aspx?FamilyID=5765d7a8-7722-4888-a970-ac39b33fd8ab&amp;displaylang=en).

Important links:

* [Discussion forum](http://community.sharpdevelop.net/forums/69.aspx)
* [Issue Tracker](https://github.com/icsharpcode/ILSpy/issues)
* [ILSpy plugin list](https://github.com/icsharpcode/ILSpy/wiki/Plugins)
* [Build server](http://build.sharpdevelop.net/BuildArtefacts/#ILSpyMaster)


## Release History

Want to know when major new features are added? When a new stable version is released?
[Follow us on Twitter!](http://twitter.com/ilspy)

* 6/5/2016 [Version 2.4](https://github.com/icsharpcode/ILSpy/issues?q=milestone%3A2.4+is%3Aclosed)
* 4/9/2016 [Version 2.3.2](https://github.com/icsharpcode/ILSpy/releases/tag/v2.3.2)
* 7/13/2015 [Version 2.3.1](https://github.com/icsharpcode/ILSpy/releases/tag/2.3.1)
* 3/9/2015 [Version 2.3](https://github.com/icsharpcode/ILSpy/releases/tag/2.3)
* 6/29/2014 [Version 2.2](http://community.sharpdevelop.net/blogs/danielgrunwald/archive/2014/06/29/ilspy-2-2-release.aspx)
* 6/3/2012 [Version 2.1](http://community.sharpdevelop.net/blogs/christophwille/archive/2012/06/03/ilspy-2-1-async-await-decompilation-support.aspx)
* 4/15/2012 [Version 2.0](http://community.sharpdevelop.net/blogs/christophwille/archive/2012/04/15/ilspy-2-0-final.aspx)
* 2/16/2012 [2.0 Beta](http://community.sharpdevelop.net/blogs/danielgrunwald/archive/2012/02/16/ilspy-2-0-beta-1.aspx)
* 7/16/2011 [Version 1.0](http://community.sharpdevelop.net/blogs/christophwille/archive/2011/07/16/ilspy-1-0-has-landed.aspx)
* 5/29/2011 [Beta](http://community.sharpdevelop.net/blogs/christophwille/archive/2011/05/29/ilspy-1-0-beta.aspx)
* 5/4/2011 [M3](http://community.sharpdevelop.net/blogs/christophwille/archive/2011/05/04/ilspy-1-0-m3-object-initializer-search-ui-xml-documentation.aspx)
* 4/13/2011 [M2](http://community.sharpdevelop.net/blogs/christophwille/archive/2011/04/13/ilspy-1-0-milestone-2-quot-m2-quot.aspx)
* 2/24/2011 [M1 (Milestone 1) Release](http://community.sharpdevelop.net/blogs/christophwille/archive/2011/02/24/ilspy-1-0-m1-milestone-1.aspx)
* 2/16/2011 [First Preview](http://community.sharpdevelop.net/blogs/christophwille/archive/2011/02/16/new-from-sharpdevelop-ilspy.aspx)
* 2/4/2011 Development Starts (github repository created)


## ILSpy Features

* Assembly browsing
* IL Disassembly
* Support C# 5.0 "async"
* Decompilation to C#
    * Supports lambdas and 'yield return'
    * Shows XML documentation
* Decompilation to VB
* Saving of resources
* Save decompiled assembly as .csproj
* Search for types/methods/properties (substring)
* Hyperlink-based type/method/property navigation
* Base/Derived types navigation
* Navigation history
* BAML to XAML decompiler
* Save Assembly as C# Project
* Find usage of field/method
* Extensible via [plugins](https://github.com/icsharpcode/ILSpy/wiki/Plugins) (MEF)
* Assembly Lists  


## ILSpy - Further Down the Road

 * Bookmarks
 * Debugger [Eusebiu's blog](http://community.sharpdevelop.net/blogs/marcueusebiu/default.aspx)
 * Support C# 4.0 "dynamic"
 * Add casts where required to make C# overload resolution call the correct method
 * Support for fixed fields ("`struct A { public unsafe fixed int Field[10]; }`")
 * Decompiling ILSpy with itself and recompiling the result should result in a working ILSpy copy
 * Assembly editing capabilities (similar to [Reflexil](http://reflexil.net/))  


## Blog Posts on ILSpy Development

* [Daniel Grunwald's blog](http://community.sharpdevelop.net/blogs/danielgrunwald/archive/tags/ILSpy/default.aspx)
* [David Srbecky's blog](http://community.sharpdevelop.net/blogs/dsrbecky/archive/tags/ILSpy/default.aspx)
* [Siegfried Pammer's blog](http://community.sharpdevelop.net/blogs/siegfried_pammer/archive/tags/ILSpy/default.aspx)
* [Eusebiu Marcu's blog](http://community.sharpdevelop.net/blogs/marcueusebiu/archive/tags/ILSpy/default.aspx)  


## Screencasts, Demo & How To Videos

* [Overview of features in ILSpy Build 296](http://www.youtube.com/watch?v=CDi5yT1ekuU) Resolution: 720p  


## Screenshots

Viewing IL (Build 199)

[![Image](images/screenshots/build199_viewingil_small.jpg)](images/screenshots/build199_viewingil.png)

  
Navigating Types (Build 199)

[![Image](images/screenshots/build199_navigatingtypes_small.jpg)](images/screenshots/build199_navigatingtypes.png)

  
Saving Resources (Build 199)

[![Image](images/screenshots/build199_savingresources_small.jpg)](images/screenshots/build199_savingresources.png)

  
Decompiling a Type to C# (Build 199)

[![Image](images/screenshots/build199_decompilingtocsharp_small.jpg)](images/screenshots/build199_decompilingtocsharp.png)

  
Decompiling method with 'yield return' (Build 528)

[![Image](images/screenshots/build258_yieldreturn.png)](images/screenshots/build258_yieldreturn.png)

