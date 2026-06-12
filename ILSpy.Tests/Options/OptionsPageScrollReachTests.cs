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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class OptionsPageScrollReachTests
{
	[AvaloniaTest]
	public void Last_Item_In_Display_Panel_Is_Reachable_At_Max_Scroll()
	{
		// Regression for the "Reset-to-defaults border obscures last setting" bug:
		// before the fix, the outer StackPanel's measured DesiredHeight didn't include
		// the trailing HeaderedContentControl's inner content (the "Sort results by
		// fitness" checkbox), so ScrollViewer.Extent was short by ~30 pixels and the
		// checkbox couldn't be scrolled into view. Padding on the inner Border (rather
		// than the ScrollViewer, where vertical padding is collapsed) forces the
		// measure pass to include all children plus breathing room.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Width = 900;
		window.Height = 600;
		window.Show();
		Dispatcher.UIThread.RunJobs();
		TestCapture.Step("booted");

		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._Options));
		command.Execute(null);
		Dispatcher.UIThread.RunJobs();
		TestCapture.Step("options-opened");

		var view = window.GetVisualDescendants().OfType<OptionsPageView>().Single();
		var model = (OptionsPageModel)((ContentTabPage)((MainWindowViewModel)window.DataContext!)
			.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>().Single(t => t.Content is OptionsPageModel)).Content!;
		model.SelectedPage = model.Pages.OfType<DisplaySettingsViewModel>().Single();
		Dispatcher.UIThread.RunJobs();
		TestCapture.Step("display-page-selected");

		// Each panel now declares its own ScrollViewer (so per-tab scroll offset is
		// independent), so the visual tree may contain multiple ScrollViewer instances
		// — one per panel that's been materialized at least once. Pick the visible one.
		var scrollViewer = view.GetVisualDescendants().OfType<ScrollViewer>()
			.First(sv => sv.IsEffectivelyVisible && sv.Bounds.Height > 0);
		scrollViewer.Offset = new Vector(0, scrollViewer.Extent.Height);
		Dispatcher.UIThread.RunJobs();
		TestCapture.Step("scrolled-to-max");

		// The "Sort results by fitness" checkbox sits inside the last HeaderedContentControl
		// of the Display panel. After scrolling to max, its rendered bottom edge in
		// window-coordinate space must sit at or above the Reset-to-defaults border's top.
		// Scope the search inside the visible ScrollViewer in case other panels materialized
		// a stale copy of the same checkbox label.
		var sortCheckbox = scrollViewer.GetVisualDescendants().OfType<CheckBox>()
			.FirstOrDefault(cb => cb.Content?.ToString() == Resources.SortResultsFitness);
		sortCheckbox.Should().NotBeNull(
			"the Display panel must expose a 'Sort results by fitness' checkbox; if this fails the "
			+ "panel structure changed and the test needs updating, not just the assertion below");

		var resetBorder = view.GetVisualDescendants().OfType<Border>()
			.Single(b => b.GetVisualDescendants().OfType<Button>()
				.Any(btn => btn.Content?.ToString() == Resources.ResetToDefaults));

		var checkboxBottom = sortCheckbox!.TranslatePoint(
			new Point(0, sortCheckbox.Bounds.Height), window)!.Value.Y;
		var resetTop = resetBorder.TranslatePoint(new Point(0, 0), window)!.Value.Y;

		checkboxBottom.Should().BeLessThanOrEqualTo(resetTop,
			$"the last checkbox's bottom ({checkboxBottom}) must not extend below the Reset "
			+ $"border's top ({resetTop}) when scrolled to max; otherwise the Reset row visually "
			+ "obscures it. Indicates either ScrollViewer.Extent under-measures the StackPanel "
			+ "content or the ContentPresenter under-allocates the TabControl's vertical room.");
	}

	[AvaloniaTest]
	public void Each_Decompiler_Settings_Group_Has_A_Header_Checkbox()
	{
		// Every Decompiler-settings category (the C# language-version groups) carries a tri-state
		// header checkbox bound to AreAllItemsChecked that bulk-toggles the group. The group
		// viewmodel has the whole plumbing, but the panel's Expander header had been reduced to
		// plain category text, so each group rendered WITHOUT its checkbox. Realize the panel and
		// assert each group's header checkbox is present (its Content is the Category string,
		// distinct from the per-item checkboxes whose Content is a setting Description).
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Width = 900;
		window.Height = 600;
		window.Show();
		Dispatcher.UIThread.RunJobs();

		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._Options));
		command.Execute(null);
		Dispatcher.UIThread.RunJobs();

		var view = window.GetVisualDescendants().OfType<OptionsPageView>().Single();
		var model = (OptionsPageModel)((ContentTabPage)((MainWindowViewModel)window.DataContext!)
			.DockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>().Single(t => t.Content is OptionsPageModel)).Content!;
		var decompilerPage = model.Pages.OfType<DecompilerSettingsViewModel>().Single();
		model.SelectedPage = decompilerPage;
		Dispatcher.UIThread.RunJobs();

		var renderedCheckboxContents = view.GetVisualDescendants().OfType<CheckBox>()
			.Select(cb => cb.Content?.ToString())
			.Where(s => !string.IsNullOrEmpty(s))
			.ToHashSet();

		decompilerPage.Settings.Should().NotBeEmpty("baseline: the Decompiler panel must have groups");
		foreach (var group in decompilerPage.Settings)
		{
			renderedCheckboxContents.Should().Contain(group.Category,
				$"the '{group.Category}' group must render a header checkbox (the bulk-toggle), "
				+ "not just the category text");
		}
	}
}
