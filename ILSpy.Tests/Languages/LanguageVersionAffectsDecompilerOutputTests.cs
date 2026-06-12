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
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class LanguageVersionAffectsDecompilerOutputTests
{
	[AvaloniaTest]
	public async Task Changing_Language_Version_Triggers_Redecompile_And_Output_Reflects_New_Version()
	{
		// Picking System.Linq.Enumerable.Select (the IEnumerable+Func overload) — its body uses
		// `is`-pattern-matching with declaration (C# 7+), so dropping to C# 6 forces the decompiler
		// to fall back to explicit cast + null-check + variable assignment. Output text therefore
		// differs sharply between C# 14 and C# 6.
		//
		// Without the fix the test fails two ways at once:
		//   1. Setting LanguageService.CurrentVersion never triggers a re-decompile (the
		//      DockWorkspace listener watches CurrentLanguage only) so the wait for "text differs"
		//      times out.
		//   2. Even if a re-decompile fired, TryGetLiveDecompilerSettings never reads
		//      CurrentVersion, so the produced DecompilerSettings still targets Latest and the
		//      output text would not change.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		// Two `Select` overloads (with and without index); the IEnumerable + Func<TSource,TResult>
		// one is the version-sensitive target.
		var selectNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Select"
				&& m.MethodDefinition.Parameters.Count == 2);

		var languageService = AppComposition.Current.GetExport<LanguageService>();
		// Baseline: latest C# version so `is`-pattern-matching is used.
		languageService.CurrentVersion = languageService.CurrentLanguage.LanguageVersions.Last();

		vm.AssemblyTreeModel.SelectNode(selectNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var latestText = tab.Text;

		// Act — drop to C# 6 (no `is`-pattern-matching declarations) and wait for the next
		// decompile to land.
		var cs6 = languageService.CurrentLanguage.LanguageVersions
			.First(v => v.DisplayName.StartsWith("C# 6.0"));
		languageService.CurrentVersion = cs6;

		await Waiters.WaitForAsync(
			() => !tab.IsDecompiling && tab.Text != latestText,
			TimeSpan.FromSeconds(15),
			"re-decompile to fire after CurrentVersion change AND emit different output");

		tab.Text.Should().NotBe(latestText,
			"switching the language version must visibly change the decompiled output");
	}
}
