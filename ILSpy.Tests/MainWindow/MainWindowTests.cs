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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;
using ICSharpCode.ILSpy.Views.Controls;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class MainWindowTests
{
	[AvaloniaTest]
	public void MainWindow_Resolves_From_Composition_And_Shows()
	{
		// Smoke-test that MEF resolves MainWindow, the window shows, and its DataContext is the
		// MainWindowViewModel — the foundation every other UI test relies on.

		// Arrange + Act — resolve and show.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		TestCapture.Step("booted");

		// Assert — visible, titled, with the correct DataContext.
		window.IsVisible.Should().BeTrue();
		// DEBUG builds append the full version (e.g. "ILSpy 11.0.0.x-<branch>-pre") to the
		// title bar, so match the prefix rather than the exact string.
		window.Title.Should().StartWith("ILSpy");
		window.DataContext.Should().BeOfType<MainWindowViewModel>();
	}

	[AvaloniaTest]
	public async Task Assembly_Tree_Pane_Is_Visible_In_Layout()
	{
		// The dock layout must materialise an AssemblyListPane on first show with non-zero
		// bounds, otherwise the user is staring at a blank window.

		// Arrange + Act — boot the window and wait for the pane to be realized in the visual tree.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<AssemblyListPane>().Any());
		var pane = await window.WaitForComponent<AssemblyListPane>();
		TestCapture.Step("before-assembly-pane-check");

		// Assert — visible with positive width and height.
		pane.IsVisible.Should().BeTrue();
		pane.Bounds.Width.Should().BeGreaterThan(0);
		pane.Bounds.Height.Should().BeGreaterThan(0);
	}

	[AvaloniaTest]
	public async Task Toolbar_Disabled_Button_Icon_Is_GrayscaleAware()
	{
		// At startup the Back button is disabled (no nav history yet — the bound Command's
		// CanExecute returns false). Its icon must be a GrayscaleAwareImage so it desaturates
		// instead of just dimming. We trust IsEffectivelyEnabled to drive the swap inside the
		// control; the visual result is verified manually via CaptureAndShow when needed.

		// Arrange + Act — show the window, wait until at least one disabled icon-bearing button
		// has been laid out.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		await Waiters.WaitForAsync(() => window.GetVisualDescendants()
			.OfType<Button>()
			.Any(b => !b.IsEffectivelyEnabled
				&& b.GetVisualDescendants().OfType<Image>().Any()));
		TestCapture.Step("before-disabled-icon-check");

		// Assert — every disabled toolbar icon is the grayscale-aware variant, not a plain Image.
		var disabledIcons = window.GetVisualDescendants()
			.OfType<Button>()
			.Where(b => !b.IsEffectivelyEnabled)
			.SelectMany(b => b.GetVisualDescendants().OfType<Image>())
			.ToList();
		disabledIcons.Should().NotBeEmpty();
		disabledIcons.Should().OnlyContain(img => img is GrayscaleAwareImage,
			"disabled toolbar buttons must use GrayscaleAwareImage so their icons desaturate");
	}

	[AvaloniaTest]
	public async Task Toolbar_Has_Open_Button_Wired_To_Open_Command()
	{
		// The MEF-driven Open button (Tag = "Open") must be present in the toolbar with its
		// Command resolved and enabled.

		// Arrange + Act — show window, wait for the MEF-built Open button to land.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		await Waiters.WaitForAsync(() => window.GetVisualDescendants()
			.OfType<Button>()
			.Any(b => (string?)b.Tag == nameof(ICSharpCode.ILSpy.Properties.Resources.Open)));

		var openButton = window.GetVisualDescendants()
			.OfType<Button>()
			.Single(b => (string?)b.Tag == nameof(ICSharpCode.ILSpy.Properties.Resources.Open));
		TestCapture.Step("before-open-button-check");

		// Assert — Command is wired and CanExecute is true.
		openButton.Command.Should().NotBeNull();
		openButton.Command!.CanExecute(null).Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Taskbar_Progress_Goes_Indeterminate_While_Decompiling_Then_Clears()
	{
		// Selecting a node kicks off a decompile; while it's running the taskbar progress must
		// transition to Indeterminate, and once done it must drop back to None.

		// Arrange — boot window, wait for assemblies, subscribe to TaskbarProgressService events.
		var (_, vm) = await TestHarness.BootAsync(3);

		var service = AppComposition.Current.GetExport<TaskbarProgressService>();
		var states = new List<TaskbarProgressState>();
		void Observe(TaskbarProgressState s) => states.Add(s);
		service.StateChanged += Observe;

		// Act — trigger a decompile.
		var node = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectNode(node);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("linq-decompiled");

		service.StateChanged -= Observe;

		// Assert — Indeterminate appeared mid-flight and None is the final state.
		states.Should().Contain(TaskbarProgressState.Indeterminate,
			"taskbar must show indeterminate progress while a decompile is in flight");
		states.Last().Should().Be(TaskbarProgressState.None,
			"taskbar progress must clear after decompile completes");
	}
}
