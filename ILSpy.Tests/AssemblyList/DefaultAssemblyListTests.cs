// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using System;
using System.IO;
using System.Linq;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class DefaultAssemblyListTests
{
	[AvaloniaTest]
	public void Framework_Directory_Seed_Loads_Managed_Assemblies_And_Skips_Native()
	{
		// The first-run default list seeds itself from the shared-framework directory the
		// running runtime lives in, via AssemblyListManager.AddFrameworkAssembliesFromDirectory.
		// This exercises that seeding directly (the app wiring just calls it for the running
		// runtime's directory): managed framework assemblies load; native runtime libraries are
		// filtered out.
		var manager = AppComposition.Current.GetExport<SettingsService>().AssemblyListManager;
		var list = manager.CreateList("framework-seed-probe-" + Guid.NewGuid().ToString("N"));
		var frameworkDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

		manager.AddFrameworkAssembliesFromDirectory(list, frameworkDir);

		var assemblies = list.GetAssemblies();
		var names = assemblies.Select(a => a.ShortName).ToHashSet();

		names.Should().Contain("System.Private.CoreLib");
		names.Should().Contain("System.Linq");
		names.Should().Contain("System.Collections");

		assemblies.Length.Should().BeGreaterThan(20,
			"the whole shared-framework directory is far larger than a hard-coded handful");
		assemblies.Should().OnlyContain(a => Path.GetDirectoryName(a.FileName) == frameworkDir,
			"every seeded assembly comes from the framework directory");

		// Native runtime libraries must not be loaded as assemblies.
		names.Should().NotContain(n => n.Contains("coreclr"));
		names.Should().NotContain(n => n.Contains("clrjit"));
	}
}
