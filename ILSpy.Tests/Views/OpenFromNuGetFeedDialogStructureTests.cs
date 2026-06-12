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

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.Tests.NuGetFeeds;

using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Views;

/// <summary>
/// Pins the package-chooser dialog's shape:
/// search box, pre-release toggle, refresh, an editable package-source ComboBox,
/// a results list, a version dropdown with an Open button, a Cancel button, and an
/// error bar that appears when the feed reports a problem. The editable-ComboBox
/// assertion doubles as a canary for the Simple theme actually templating
/// PART_EditableTextBox - if a theme change drops it, custom feed URLs become untypable.
/// </summary>
[TestFixture]
public class OpenFromNuGetFeedDialogStructureTests
{
	static OpenFromNuGetFeedDialog CreateDialog()
		=> new(settingsService: null, client: new FakeNuGetFeedClient());

	[AvaloniaTest]
	public void Dialog_Title_And_Captions_Come_From_Localised_Resources()
	{
		var dialog = CreateDialog();

		dialog.Title.Should().Be(Resources.NuGetFeedSelectPackage);
		dialog.FindControl<CheckBox>("PrereleaseCheckBox")!.Content
			.Should().Be(Resources.NuGetFeedShowPrerelease);
		dialog.FindControl<Button>("OpenButton")!.Content
			.Should().Be(Resources.OpenListDialog__Open);
		dialog.FindControl<Button>("CancelButton")!.Content
			.Should().Be(Resources.Cancel);
	}

	[AvaloniaTest]
	public void Dialog_Contains_The_Package_Chooser_Controls()
	{
		var dialog = CreateDialog();

		dialog.FindControl<TextBox>("SearchBox").Should().NotBeNull("packages are found by searching");
		dialog.FindControl<Button>("RefreshButton").Should().NotBeNull();
		dialog.FindControl<CheckBox>("PrereleaseCheckBox").Should().NotBeNull();
		dialog.FindControl<ListBox>("PackagesList").Should().NotBeNull("search hits go into the left-hand list");
		dialog.FindControl<ComboBox>("VersionsCombo").Should().NotBeNull("the version is picked from a dropdown");
		dialog.FindControl<Button>("LoadMoreButton").Should().NotBeNull("results are paged");
		dialog.FindControl<Button>("CancelDownloadButton").Should().NotBeNull("long downloads must be abortable");

		var errorBar = dialog.FindControl<Border>("ErrorBar");
		errorBar.Should().NotBeNull("feed failures surface in an in-dialog status bar");
		errorBar!.IsVisible.Should().BeFalse("no error is showing before anything went wrong");
	}

	[AvaloniaTest]
	public async Task Feed_ComboBox_Is_Editable_So_Custom_Feed_Urls_Can_Be_Typed()
	{
		var dialog = CreateDialog();
		dialog.Show();

		var feedCombo = dialog.FindControl<ComboBox>("FeedCombo");
		feedCombo.Should().NotBeNull();
		feedCombo!.IsEditable.Should().BeTrue("users must be able to type arbitrary V3 feed URLs");

		// The editable text box is an optional template part; this pins that the active
		// theme's ComboBox template actually provides it.
		await Waiters.WaitForAsync(() => feedCombo.GetVisualDescendants()
			.OfType<TextBox>().Any(t => t.Name == "PART_EditableTextBox"));
	}

	[AvaloniaTest]
	public async Task Error_Bar_Shows_The_ViewModels_Error_Message()
	{
		var dialog = CreateDialog();
		dialog.Show();
		var vm = (OpenFromNuGetFeedDialogViewModel)dialog.DataContext!;

		vm.ErrorMessage = "the feed is on fire";
		await Waiters.WaitForAsync(() => dialog.FindControl<Border>("ErrorBar")!.IsVisible);

		dialog.FindControl<Border>("ErrorBar")!.GetVisualDescendants().OfType<TextBlock>()
			.Should().Contain(t => t.Text == "the feed is on fire");
	}

	[AvaloniaTest]
	public async Task Clear_Button_Lives_Inside_The_Search_Box_And_Only_Shows_While_It_Has_Text()
	{
		var dialog = CreateDialog();
		dialog.Show();

		var searchBox = dialog.FindControl<TextBox>("SearchBox")!;
		var clearButton = searchBox.InnerRightContent.Should().BeOfType<Button>(
			"the clear affordance sits inside the search box, like the Search pane's").Subject;
		clearButton.IsVisible.Should().BeFalse("there is nothing to clear in an empty box");

		var vm = (OpenFromNuGetFeedDialogViewModel)dialog.DataContext!;
		vm.SearchText = "newtonsoft";
		await Waiters.WaitForAsync(() => clearButton.IsVisible);

		clearButton.Command.Should().BeSameAs(vm.ClearSearchCommand);
	}

	[AvaloniaTest]
	public async Task Selecting_A_Package_Preselects_The_Latest_Version_In_The_Dropdown()
	{
		var client = new FakeNuGetFeedClient();
		client.VersionsToReturn = new[] { "13.0.3", "13.0.2", "12.0.1" };
		var dialog = new OpenFromNuGetFeedDialog(settingsService: null, client: client);
		dialog.Show();

		var vm = (OpenFromNuGetFeedDialogViewModel)dialog.DataContext!;
		await Waiters.WaitForAsync(() => vm.Packages.Count > 0);
		vm.SelectedPackage = vm.Packages[0];

		var combo = dialog.FindControl<ComboBox>("VersionsCombo")!;
		await Waiters.WaitForAsync(() => Equals(combo.SelectedItem, "13.0.3"),
			TimeSpan.FromSeconds(5));
		combo.SelectedIndex.Should().Be(0, "the newest version leads the list and starts selected");
	}

	[AvaloniaTest]
	public async Task Opening_The_Dialog_Lists_The_Feeds_Default_Packages()
	{
		var client = new FakeNuGetFeedClient();
		var dialog = new OpenFromNuGetFeedDialog(settingsService: null, client: client);
		dialog.Show();

		await Waiters.WaitForAsync(() => client.SearchCalls.Count > 0);
		client.SearchCalls[0].SearchText.Should().BeEmpty(
			"the dialog must not open empty - the feed's default listing gives the user something to browse");

		var vm = (OpenFromNuGetFeedDialogViewModel)dialog.DataContext!;
		await Waiters.WaitForAsync(() => vm.Packages.Count > 0);
		var list = dialog.FindControl<ListBox>("PackagesList")!;
		list.ItemCount.Should().Be(vm.Packages.Count, "the list shows exactly the search hits");
	}
}
