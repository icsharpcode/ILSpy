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

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Frozen tabs keep their own <see cref="DecompilerTabPageModel"/> with cached output.
/// Output-affecting global changes — the language/version combos and the
/// Redecompile-class display settings — must re-decompile EVERY decompiler tab, not just
/// the active preview tab, or frozen tabs silently keep showing stale output.
/// </summary>
[TestFixture]
public class FrozenTabRefreshTests
{
	/// <summary>
	/// Boots the app, decompiles Enumerable.Select into the preview tab, freezes it, then
	/// routes Enumerable.Where into a fresh preview tab. Returns both tab models settled.
	/// </summary>
	static async Task<(DecompilerTabPageModel Frozen, DecompilerTabPageModel Preview, MainWindowViewModel Vm)> BootWithFrozenAndPreviewTabsAsync()
	{
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var selectNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Select" && m.MethodDefinition.Parameters.Count == 2);
		var whereNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Where" && m.MethodDefinition.Parameters.Count == 2);

		vm.AssemblyTreeModel.SelectNode(selectNode);
		vm.DockWorkspace.SettleSelection();
		var frozen = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		vm.DockWorkspace.FreezeCurrentTab();

		vm.AssemblyTreeModel.SelectNode(whereNode);
		vm.DockWorkspace.SettleSelection();
		var preview = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		preview.Should().NotBeSameAs(frozen,
			"setup precondition — selecting after freeze must route to a fresh preview tab");
		return (frozen, preview, vm);
	}

	[AvaloniaTest]
	public async Task Switching_The_Language_Redecompiles_Frozen_Tabs_Too()
	{
		var (frozen, preview, _) = await BootWithFrozenAndPreviewTabsAsync();
		var frozenCSharp = frozen.Text;
		var previewCSharp = preview.Text;

		var languageService = AppComposition.Current.GetExport<LanguageService>();
		languageService.CurrentLanguage = languageService.GetLanguage("IL");

		await Waiters.WaitForAsync(
			() => !frozen.IsDecompiling && !preview.IsDecompiling
				&& frozen.Text != frozenCSharp && preview.Text != previewCSharp,
			TimeSpan.FromSeconds(15),
			"both the frozen tab and the preview tab to re-decompile after the language change");

		frozen.Language.Name.Should().Be("IL",
			"the frozen tab must adopt the newly selected language");
		frozen.Text.Should().Contain(".method",
			"the frozen tab must show IL output after the switch");
		preview.Text.Should().Contain(".method",
			"the preview tab must show IL output after the switch");
	}

	[AvaloniaTest]
	public async Task Toggling_A_Redecompile_Display_Setting_Refreshes_Frozen_Tabs_Too()
	{
		var (frozen, preview, _) = await BootWithFrozenAndPreviewTabsAsync();
		var frozenBefore = frozen.Text;
		var previewBefore = preview.Text;

		// IndentationUseTabs is in the Redecompile reaction class (DisplaySettingReactions)
		// and changes every indented line, so the refresh is observable on any output.
		var display = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		display.IndentationUseTabs = !display.IndentationUseTabs;

		await Waiters.WaitForAsync(
			() => !frozen.IsDecompiling && !preview.IsDecompiling
				&& frozen.Text != frozenBefore && preview.Text != previewBefore,
			TimeSpan.FromSeconds(15),
			"both the frozen tab and the preview tab to re-decompile after the display-setting change");

		frozen.Text.Should().NotBe(frozenBefore,
			"the frozen tab must re-render with the new indentation setting");
	}
}
