// Copyright (c) 2026 Christoph Wille
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

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.Tests.Processes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Views;

/// <summary>
/// Pins the process-explorer dialog's shape: a filter box, a process grid above an
/// assembly grid, and the buttons that drive it. Everything is reachable by button - the
/// dialog carries no context menu - and the assembly grid allows multi-select so several
/// assemblies can be added in one go.
/// </summary>
[TestFixture]
public class OpenFromProcessDialogStructureTests
{
	static OpenFromProcessDialog CreateDialog(FakeProcessExplorer? explorer = null)
	{
		explorer ??= new FakeProcessExplorer();
		return new OpenFromProcessDialog(explorer);
	}

	[AvaloniaTest]
	public void Dialog_Title_And_Captions_Come_From_Localised_Resources()
	{
		var dialog = CreateDialog();

		dialog.Title.Should().Be(Resources.OpenFromProcess_Title);
		dialog.FindControl<Button>("RefreshButton")!.Content.Should().Be(Resources.OpenFromProcess_Refresh);
		dialog.FindControl<Button>("AddSelectedButton")!.Content.Should().Be(Resources.OpenFromProcess_AddSelected);
		dialog.FindControl<Button>("AddEntryAssemblyButton")!.Content.Should().Be(Resources.OpenFromProcess_AddEntryAssembly);
		dialog.FindControl<Button>("CancelButton")!.Content.Should().Be(Resources.Cancel);
	}

	[AvaloniaTest]
	public void Dialog_Contains_The_Process_Explorer_Controls()
	{
		var dialog = CreateDialog();

		dialog.FindControl<TextBox>("FilterBox").Should().NotBeNull("long process lists need filtering");
		dialog.FindControl<DataGrid>("ProcessesGrid").Should().NotBeNull();
		dialog.FindControl<DataGrid>("ModulesGrid").Should().NotBeNull("the selected process's assemblies are listed");
		dialog.FindControl<TextBlock>("VisibilityHint").Should().NotBeNull(
			"the dialog states which processes it cannot show");

		var errorBar = dialog.FindControl<Border>("ErrorBar");
		errorBar.Should().NotBeNull();
		errorBar!.IsVisible.Should().BeFalse("no error is showing before anything went wrong");
	}

	[AvaloniaTest]
	public void Several_Assemblies_Can_Be_Selected_At_Once()
	{
		var grid = CreateDialog().FindControl<DataGrid>("ModulesGrid")!;

		grid.SelectionMode.Should().Be(DataGridSelectionMode.Extended,
			"adding several assemblies of one process in one go is the common case");
		grid.IsReadOnly.Should().BeTrue("the grid lists assemblies, it does not edit them");
	}

	[AvaloniaTest]
	public async Task Opening_The_Dialog_Lists_The_Running_Processes()
	{
		var explorer = new FakeProcessExplorer();
		explorer.ProcessesToReturn.Add(FakeProcessExplorer.Process(100, "ILSpy", "ILSpy"));
		var dialog = CreateDialog(explorer);

		dialog.Show();

		await Waiters.WaitForAsync(() => explorer.ProcessCalls > 0);
		var vm = (OpenFromProcessDialogViewModel)dialog.DataContext!;
		await Waiters.WaitForAsync(() => vm.Processes.Count == 1);
		dialog.FindControl<DataGrid>("ProcessesGrid")!.ItemsSource.Should().BeSameAs(vm.Processes);
	}

	[AvaloniaTest]
	public async Task Selecting_Assemblies_In_The_Grid_Feeds_The_Add_Button()
	{
		var explorer = new FakeProcessExplorer();
		explorer.ProcessesToReturn.Add(FakeProcessExplorer.Process(100, "ILSpy", "ILSpy"));
		explorer.ModulesByPid[100] = new[] {
			new ICSharpCode.ILSpy.Processes.ProcessModuleInfo("A.dll", @"C:\a\A.dll", IsInMemory: false),
			new ICSharpCode.ILSpy.Processes.ProcessModuleInfo("B.dll", @"C:\b\B.dll", IsInMemory: false),
		};
		var dialog = CreateDialog(explorer);
		dialog.Show();
		var vm = (OpenFromProcessDialogViewModel)dialog.DataContext!;
		await Waiters.WaitForAsync(() => vm.Processes.Count == 1);

		vm.SelectedProcess = vm.Processes[0];
		await Waiters.WaitForAsync(() => vm.Modules.Count == 2);

		var grid = dialog.FindControl<DataGrid>("ModulesGrid")!;
		grid.SelectedItems.Add(vm.Modules[0]);
		await Waiters.WaitForAsync(() => vm.SelectedModules.Count == 1);

		vm.SelectedModules.Single().Name.Should().Be("A.dll",
			"the grid's selection is what the Add button acts on");
	}

	[AvaloniaTest]
	public async Task Filtering_Keeps_A_Selected_Process_That_Still_Matches()
	{
		// Only reproducible with the grid attached: the view model alone never sees the
		// selection being written back, because it is the grid that pushes it.
		var explorer = new FakeProcessExplorer();
		explorer.ProcessesToReturn.Add(FakeProcessExplorer.Process(100, "ILSpy", "ILSpy"));
		explorer.ProcessesToReturn.Add(FakeProcessExplorer.Process(200, "dotnet", "MyTool"));
		explorer.ModulesByPid[100] = new[] {
			new ICSharpCode.ILSpy.Processes.ProcessModuleInfo("A.dll", @"C:\a\A.dll", IsInMemory: false),
		};
		var dialog = CreateDialog(explorer);
		dialog.Show();
		var vm = (OpenFromProcessDialogViewModel)dialog.DataContext!;
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);
		dialog.FindControl<DataGrid>("ProcessesGrid")!.SelectedItem = vm.Processes[0];
		await Waiters.WaitForAsync(() => vm.Modules.Count == 1);

		// One keystroke that narrows the list without excluding the selected row.
		vm.FilterText = "ILSpy";

		vm.Processes.Should().ContainSingle().Which.Pid.Should().Be(100);
		vm.SelectedProcess.Should().NotBeNull("the selected process still matches what was typed");
		vm.Modules.Should().ContainSingle("its assembly list must survive a keystroke");
	}

	[AvaloniaTest]
	public async Task Filtering_Out_The_Selected_Process_Clears_Its_Assemblies()
	{
		var explorer = new FakeProcessExplorer();
		explorer.ProcessesToReturn.Add(FakeProcessExplorer.Process(100, "ILSpy", "ILSpy"));
		explorer.ProcessesToReturn.Add(FakeProcessExplorer.Process(200, "dotnet", "MyTool"));
		explorer.ModulesByPid[100] = new[] {
			new ICSharpCode.ILSpy.Processes.ProcessModuleInfo("A.dll", @"C:\a\A.dll", IsInMemory: false),
		};
		var dialog = CreateDialog(explorer);
		dialog.Show();
		var vm = (OpenFromProcessDialogViewModel)dialog.DataContext!;
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);
		dialog.FindControl<DataGrid>("ProcessesGrid")!.SelectedItem = vm.Processes[0];
		await Waiters.WaitForAsync(() => vm.Modules.Count == 1);

		vm.FilterText = "dotnet";

		await Waiters.WaitForAsync(() => vm.SelectedProcess == null);
		vm.Modules.Should().BeEmpty("the assemblies of a process that is no longer listed are stale");
		vm.IsLoadingModules.Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Error_Bar_Shows_The_ViewModels_Error_Message()
	{
		var dialog = CreateDialog();
		dialog.Show();
		var vm = (OpenFromProcessDialogViewModel)dialog.DataContext!;

		vm.ErrorMessage = "the diagnostics port is unreachable";
		await Waiters.WaitForAsync(() => dialog.FindControl<Border>("ErrorBar")!.IsVisible);

		dialog.FindControl<Border>("ErrorBar")!.GetVisualDescendants().OfType<TextBlock>()
			.Should().Contain(t => t.Text == "the diagnostics port is unreachable");
	}
}
