// Copyright (c) 2026 Franck
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using Redecomp.Core.Recovery;

namespace Redecomp.Tests;

public static class DepsFileTests
{
	const string Json = """
		{
		  "runtimeTarget": { "name": ".NETCoreApp,Version=v10.0", "signature": "" },
		  "targets": {
		    ".NETCoreApp,Version=v10.0": {
		      "Portail.App/1.0.0": {
		        "dependencies": { "Portail.Metier": "1.0.0" },
		        "runtime": { "Portail.App.dll": {} }
		      },
		      "Agl.Outils/1.2.0": {
		        "runtime": { "lib/net10.0/Agl.Outils.dll": { "assemblyVersion": "1.2.0.0" } }
		      },
		      "Portail.Metier/1.0.0": {
		        "dependencies": { "Agl.Outils": "1.2.0" },
		        "runtime": { "Portail.Metier.dll": {} }
		      }
		    }
		  },
		  "libraries": {
		    "Portail.App/1.0.0": { "type": "project", "serviceable": false, "sha512": "" },
		    "Agl.Outils/1.2.0": { "type": "package", "serviceable": true, "path": "agl.outils/1.2.0" },
		    "Portail.Metier/1.0.0": { "type": "project", "serviceable": false, "sha512": "" }
		  }
		}
		""";

	[Test]
	public static void ReadsProjectsPackagesAndDirectDependencies()
	{
		var deps = DepsFile.Parse(Json);
		Assert.AreEqual("Portail.App", deps.Root.Name, "root");
		Assert.AreEqual(DepsLibraryType.Project, deps.Root.Type, "root type");
		Assert.AreEqual("Portail.Metier", string.Join(",", deps.Root.Dependencies), "root dependencies");

		var package = deps.Find("Agl.Outils")!;
		Assert.AreEqual(DepsLibraryType.Package, package.Type, "package type");
		Assert.AreEqual("1.2.0", package.Version, "package version");
		Assert.AreEqual("Agl.Outils.dll", string.Join(",", package.RuntimeAssemblies), "package assemblies");

		var metier = deps.Find("Portail.Metier")!;
		Assert.AreEqual(DepsLibraryType.Project, metier.Type, "project type");
		Assert.AreEqual("Agl.Outils", string.Join(",", metier.Dependencies), "project dependencies");
		Assert.AreEqual("Portail.Metier.dll", string.Join(",", metier.RuntimeAssemblies), "project assemblies");
	}
}
