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

using ILSpy;
using ILSpy.AppEnv;
using ILSpy.AssemblyTree;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class MainWindowTests
{
	[AvaloniaTest]
	public void MainWindow_Resolves_From_Composition_And_Shows()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		window.IsVisible.Should().BeTrue();
		window.Title.Should().Be("ILSpy");
		window.DataContext.Should().BeOfType<MainWindowViewModel>();
	}

	[AvaloniaTest]
	public async Task Assembly_Tree_Pane_Is_Visible_In_Layout()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<AssemblyListPane>().Any());
		var pane = window.GetVisualDescendants().OfType<AssemblyListPane>().Single();

		pane.IsVisible.Should().BeTrue();
		pane.Bounds.Width.Should().BeGreaterThan(0);
		pane.Bounds.Height.Should().BeGreaterThan(0);
	}

	[AvaloniaTest]
	public async Task Toolbar_Disabled_Button_Renders_Dimmed_Icon()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		// At startup the Back button is disabled (no nav history yet — the bound Command's
		// CanExecute returns false). Its icon should render visibly dimmer so the user can
		// tell at a glance that it's not clickable.
		await Waiters.WaitForAsync(() => window.GetVisualDescendants()
			.OfType<Button>()
			.Any(b => !b.IsEffectivelyEnabled
				&& b.GetVisualDescendants().OfType<Image>().Any()));

		var dimmedImages = window.GetVisualDescendants()
			.OfType<Button>()
			.Where(b => !b.IsEffectivelyEnabled)
			.SelectMany(b => b.GetVisualDescendants().OfType<Image>())
			.ToList();
		dimmedImages.Should().NotBeEmpty();
		dimmedImages.Should().OnlyContain(img => img.Opacity <= 0.35,
			"disabled toolbar buttons must render their icon at < 0.4 opacity to be visually distinct");
	}

	[AvaloniaTest]
	public async Task Toolbar_Has_Open_Button_Wired_To_Open_Command()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		await Waiters.WaitForAsync(() => window.GetVisualDescendants()
			.OfType<Button>()
			.Any(b => (string?)b.Tag == nameof(ICSharpCode.ILSpy.Properties.Resources.Open)));

		var openButton = window.GetVisualDescendants()
			.OfType<Button>()
			.Single(b => (string?)b.Tag == nameof(ICSharpCode.ILSpy.Properties.Resources.Open));

		openButton.Command.Should().NotBeNull();
		openButton.Command!.CanExecute(null).Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Taskbar_Progress_Goes_Indeterminate_While_Decompiling_Then_Clears()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var service = AppComposition.Current.GetExport<TaskbarProgressService>();
		var states = new List<TaskbarProgressState>();
		void Observe(TaskbarProgressState s) => states.Add(s);
		service.StateChanged += Observe;

		var node = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectedItem = node;
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		service.StateChanged -= Observe;

		states.Should().Contain(TaskbarProgressState.Indeterminate,
			"taskbar must show indeterminate progress while a decompile is in flight");
		states.Last().Should().Be(TaskbarProgressState.None,
			"taskbar progress must clear after decompile completes");
	}
}
