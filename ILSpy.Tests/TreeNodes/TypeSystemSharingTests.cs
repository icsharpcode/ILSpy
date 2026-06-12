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

using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Extensions;
using ICSharpCode.ILSpyX.Search;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TreeNodes;

// LoadedAssembly caches ONE type system per TypeSystemOptions value
// (GetTypeSystemOrNull(options)). The tree, the search, and anything else that resolves
// entities for display must all derive their options from the user's current decompiler
// settings: that keeps them consistent with what decompilation elides AND makes them share
// the single cached instance instead of materialising a second type system per module.
[TestFixture]
public class TypeSystemSharingTests
{
	[AvaloniaTest]
	public async Task Tree_nodes_resolve_through_the_settings_derived_type_system()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		var fixture = await vm.OpenFixtureAsync();
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			fixture.ShortName, fixture.ShortName, $"{fixture.ShortName}.{FixtureAssembly.TypeName}");
		Assert.That(typeNode, Is.Not.Null);

		var module = fixture.GetMetadataFileOrNull()!;
		var expected = module.GetTypeSystemWithDecompilerSettingsOrNull(
			settingsService.CreateEffectiveDecompilerSettings());

		typeNode!.Member!.Compilation.Should().BeSameAs(expected,
			"the tree must resolve entities through the cached type system derived from the " +
			"current decompiler settings, not a separate default-options one");
	}

	[AvaloniaTest]
	public async Task Search_reuses_the_same_cached_type_system_as_the_tree()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		// A non-default option that changes the derived TypeSystemOptions: a search that
		// ignores the live settings materialises a second, default-options type system.
		settingsService.DecompilerSettings.NullableReferenceTypes = false;
		var fixture = await vm.OpenFixtureAsync();
		var module = fixture.GetMetadataFileOrNull()!;
		var language = AppComposition.Current.GetExport<LanguageService>().CurrentLanguage;

		var search = new RunningSearch(
			new[] { fixture }, FixtureAssembly.TypeName, SearchMode.TypeAndMember, language,
			ApiVisibility.PublicAndInternal, new AvaloniaSearchResultFactory(language),
			new ObservableCollection<SearchResult>(), SearchResult.ComparerByName);
		var request = search.BuildRequest();

		var fromSearch = module.GetTypeSystemWithDecompilerSettingsOrNull(request.DecompilerSettings);
		var effective = module.GetTypeSystemWithDecompilerSettingsOrNull(
			settingsService.CreateEffectiveDecompilerSettings());

		fromSearch.Should().BeSameAs(effective,
			"the search must derive its type system from the current decompiler settings so it " +
			"shares the per-module cache with the tree and respects the user's options");
	}
}
