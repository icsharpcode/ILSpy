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
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Controls.Omnibar;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Controls;

/// <summary>
/// The omnibar ships off by default behind the Options / Display "Tab options"
/// <c>EnableOmnibar</c> toggle, and the toggle takes effect live (no re-decompile).
/// </summary>
[TestFixture]
public class OmnibarSettingTests
{
	[AvaloniaTest]
	public async Task Omnibar_Is_Off_By_Default_And_Revealed_Live_When_Enabled()
	{
		var (window, vm) = await TestHarness.BootAsync(3);
		var display = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		Assert.That(display.EnableOmnibar, Is.False, "the omnibar is off by default");

		// Realize a decompiler document by selecting a node.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectedItem = typeNode;
		Omnibar? omnibar = null;
		for (int i = 0; i < 200; i++)
		{
			Dispatcher.UIThread.RunJobs();
			omnibar = window.GetVisualDescendants().OfType<DecompilerTextView>()
				.Where(v => v.IsEffectivelyVisible)
				.SelectMany(v => v.GetVisualDescendants().OfType<Omnibar>())
				.FirstOrDefault();
			if (omnibar != null)
				break;
			await Task.Delay(20);
		}
		Assert.That(omnibar, Is.Not.Null, "selecting a node realizes a decompiler text view hosting the omnibar");

		Assert.That(omnibar!.IsVisible, Is.False,
			"the omnibar is hidden while the EnableOmnibar setting is off");

		display.EnableOmnibar = true;
		Dispatcher.UIThread.RunJobs();

		Assert.That(omnibar.IsVisible, Is.True,
			"enabling the setting reveals the omnibar without a re-decompile");
	}
}
