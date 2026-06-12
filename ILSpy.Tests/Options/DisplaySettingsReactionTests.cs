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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The Options page is non-modal and live-apply (no "OK" step), so unlike WPF -- which did a full
/// assembly-list Refresh() when the modal dialog closed -- every display setting has to drive its own
/// live reaction. These tests pin that a decompiler-output setting actually re-decompiles the active
/// tab, and that every setting is classified so a newly-added one can't silently fall through.
/// </summary>
[TestFixture]
public class DisplaySettingsReactionTests
{
	static async Task Pump(int ticks = 12)
	{
		for (int i = 0; i < ticks; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(20);
		}
	}

	[AvaloniaTest]
	public async Task Toggling_A_Decompiler_Output_Setting_Re_Decompiles_The_Active_Tab()
	{
		var (_, vm) = await TestHarness.BootAsync();
		// Decompile a single small method rather than the whole Enumerable type: the full type
		// takes >15 s in headless and times out WaitForDecompiledTextAsync under CI load. The
		// re-decompile reaction under test fires on whatever the active tab shows, so one method
		// exercises it just as well.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");
		vm.AssemblyTreeModel.SelectNode(method);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		int decompileStarts = 0;
		void OnTab(object? _, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(DecompilerTabPageModel.IsDecompiling) && tab.IsDecompiling)
				decompileStarts++;
		}
		tab.PropertyChanged += OnTab;
		try
		{
			var display = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
			display.DecodeCustomAttributeBlobs = !display.DecodeCustomAttributeBlobs;
			await Pump();
		}
		finally
		{
			tab.PropertyChanged -= OnTab;
		}

		decompileStarts.Should().BeGreaterThan(0,
			"toggling a decompiler-output display setting must re-decompile the active tab "
			+ "(the live Options page has no full-refresh-on-close to fall back on)");
	}

	[Test]
	public void Every_Display_Setting_Is_Classified()
	{
		// The safety net: each settable DisplaySettings property must appear in the reaction table,
		// so adding a new setting forces a deliberate classification (editor / tree / re-decompile /
		// none) instead of silently doing nothing. Set equality also catches stale entries left
		// behind when a setting is removed.
		var settable = typeof(DisplaySettings)
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(p => p is { CanRead: true, CanWrite: true })
			.Select(p => p.Name)
			.ToHashSet();
		settable.Should().NotBeEmpty();

		var classified = DisplaySettingReactions.ClassifiedProperties.ToHashSet();
		settable.Should().BeSubsetOf(classified,
			"every settable DisplaySettings property must be classified in DisplaySettingReactions");
		classified.Should().BeSubsetOf(settable,
			"DisplaySettingReactions must not reference settings that no longer exist");
	}
}
