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

using System.Linq;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class PreconfiguredAssemblyListTests
{
	[AvaloniaTest]
	public void Preconfigured_Menu_Offers_Installed_Runtimes_And_Creates_A_Populated_List()
	{
		// The "Add preconfigured list..." menu resolves one entry per installed .NET runtime
		// (discovered under the dotnet install's shared folder) on every platform, and
		// selecting one builds a populated assembly list via the shared AssemblyListManager.

		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		var manager = settingsService.AssemblyListManager;
		var dialog = new ManageAssemblyListsDialog(settingsService);

		var configs = dialog.GetPreconfiguredAssemblyLists().ToList();

		// At least one runtime-path entry must be discovered (ILSpy itself runs on one).
		var runtimeEntry = configs.FirstOrDefault(c => c.Path != null);
		((object?)runtimeEntry).Should().NotBeNull(
			"the running machine has at least one installed .NET runtime to offer");

		var newName = "Preconfigured Test " + System.Guid.NewGuid().ToString("N");
		manager.AssemblyLists.Should().NotContain(newName);

		var created = dialog.CreatePreconfiguredList(runtimeEntry!, newName);

		((object?)created).Should().NotBeNull("a runtime directory resolves to a non-empty list");
		created!.Count.Should().BeGreaterThan(0, "the runtime's framework assemblies populate the list");
		manager.AssemblyLists.Should().Contain(newName, "the new preconfigured list is registered");
	}

	[AvaloniaTest]
	public void Preconfigured_Menu_Hides_Gac_Lists_When_No_Gac_Is_Present()
	{
		// The three GAC-based framework lists (.NET 4.x / 3.5 / ASP.NET MVC) resolve nothing on
		// platforms without a GAC, so they are only offered when a GAC directory actually
		// exists (i.e. on Windows). On a GAC-less host they must not appear.
		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		var dialog = new ManageAssemblyListsDialog(settingsService);

		var names = dialog.GetPreconfiguredAssemblyLists().Select(c => c.Name).ToList();

		bool gacPresent = ICSharpCode.Decompiler.Metadata.UniversalAssemblyResolver
			.GetGacPaths().Any(System.IO.Directory.Exists);

		if (gacPresent)
		{
			names.Should().Contain(AssemblyListManager.DotNet4List);
		}
		else
		{
			names.Should().NotContain(AssemblyListManager.DotNet4List);
			names.Should().NotContain(AssemblyListManager.DotNet35List);
			names.Should().NotContain(AssemblyListManager.ASPDotNetMVC3List);
		}
	}
}
