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

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Processes;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Processes;

/// <summary>
/// Behavior of the "Open from Running Process" dialog's view model against a fake explorer:
/// which processes are listed and filtered, when a process's assemblies are fetched, and
/// what the dialog hands back to be opened. Assemblies that exist only in a process's
/// memory are listed but cannot be opened, which several of these tests pin down.
/// </summary>
[TestFixture]
public class OpenFromProcessDialogViewModelTests
{
	static string TestFile(string name) => Path.Combine(TestContext.CurrentContext.TestDirectory, name);

	static ProcessModuleInfo OnDisk(string name) => new(name, TestFile(name), IsInMemory: false);

	static ProcessModuleInfo InMemory(string name) => new(name, Path: null, IsInMemory: true);

	static (OpenFromProcessDialogViewModel vm, FakeProcessExplorer explorer) CreateViewModel()
	{
		var explorer = new FakeProcessExplorer();
		explorer.ProcessesToReturn.Add(FakeProcessExplorer.Process(100, "ILSpy", "ILSpy"));
		explorer.ProcessesToReturn.Add(FakeProcessExplorer.Process(200, "dotnet", "MyTool"));
		explorer.ModulesByPid[100] = new[] { OnDisk("ILSpy.dll"), OnDisk("ICSharpCode.Decompiler.dll") };
		explorer.ModulesByPid[200] = new[] { OnDisk("ILSpy.Tests.dll"), InMemory("Dynamic.Proxies") };
		return (new OpenFromProcessDialogViewModel(explorer), explorer);
	}

	[AvaloniaTest]
	public async Task Refresh_Lists_The_Running_Processes()
	{
		var (vm, explorer) = CreateViewModel();

		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);

		vm.Processes.Select(p => p.Pid).Should().Equal(new[] { 100, 200 });
		vm.Processes[0].ProcessName.Should().Be("ILSpy");
		vm.IsLoadingProcesses.Should().BeFalse();
		explorer.ProcessCalls.Should().Be(1);
	}

	[AvaloniaTest]
	public async Task The_Filter_Matches_Process_Name_Pid_And_Entry_Assembly()
	{
		var (vm, _) = CreateViewModel();
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);

		vm.FilterText = "ilspy";
		vm.Processes.Select(p => p.Pid).Should().Equal(new[] { 100 }, "the name matches case-insensitively");

		vm.FilterText = "200";
		vm.Processes.Select(p => p.Pid).Should().Equal(new[] { 200 }, "a pid is a natural thing to search for");

		vm.FilterText = "MyTool";
		vm.Processes.Select(p => p.Pid).Should().Equal(new[] { 200 }, "the entry assembly is the interesting name");

		vm.FilterText = "";
		vm.Processes.Should().HaveCount(2);
	}

	[AvaloniaTest]
	public async Task Selecting_A_Process_Loads_Its_Assemblies()
	{
		var (vm, explorer) = CreateViewModel();
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);

		vm.SelectedProcess = vm.Processes[0];
		await Waiters.WaitForAsync(() => vm.Modules.Count == 2);

		explorer.ModuleCalls.Should().Equal(new[] { 100 });
		vm.Modules.Select(m => m.Name).Should().Equal(new[] { "ILSpy.dll", "ICSharpCode.Decompiler.dll" });
		vm.IsLoadingModules.Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Clearing_The_Selection_Empties_The_Assembly_List()
	{
		var (vm, _) = CreateViewModel();
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);
		vm.SelectedProcess = vm.Processes[0];
		await Waiters.WaitForAsync(() => vm.Modules.Count == 2);

		vm.SelectedProcess = null;
		await Waiters.WaitForAsync(() => vm.Modules.Count == 0);
	}

	[AvaloniaTest]
	public async Task Add_Selected_Closes_The_Dialog_With_The_Assembly_Paths()
	{
		var (vm, _) = CreateViewModel();
		string[]? closedWith = null;
		vm.CloseRequested += paths => closedWith = paths;
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);
		vm.SelectedProcess = vm.Processes[0];
		await Waiters.WaitForAsync(() => vm.Modules.Count == 2);

		vm.SelectedModules.Add(vm.Modules[0]);
		vm.SelectedModules.Add(vm.Modules[1]);
		vm.AddSelectedModulesCommand.Execute(null);

		closedWith.Should().Equal(new[] { TestFile("ILSpy.dll"), TestFile("ICSharpCode.Decompiler.dll") });
	}

	[AvaloniaTest]
	public async Task Assemblies_That_Exist_Only_In_Memory_Cannot_Be_Added()
	{
		var (vm, _) = CreateViewModel();
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);
		vm.SelectedProcess = vm.Processes[1];
		await Waiters.WaitForAsync(() => vm.Modules.Count == 2);

		var dynamicModule = vm.Modules.Single(m => m.IsInMemory);
		vm.SelectedModules.Add(dynamicModule);

		vm.AddSelectedModulesCommand.CanExecute(null).Should().BeFalse(
			"an assembly with no file on disk cannot be opened");

		vm.SelectedModules.Add(vm.Modules.Single(m => !m.IsInMemory));
		vm.AddSelectedModulesCommand.CanExecute(null).Should().BeTrue();

		string[]? closedWith = null;
		vm.CloseRequested += paths => closedWith = paths;
		vm.AddSelectedModulesCommand.Execute(null);

		closedWith.Should().Equal(new[] { TestFile("ILSpy.Tests.dll") },
			"the in-memory row is skipped rather than blocking the rest");
	}

	[AvaloniaTest]
	public async Task Add_Entry_Assembly_Closes_With_The_Assembly_Behind_The_Host()
	{
		var (vm, explorer) = CreateViewModel();
		explorer.ProcessesToReturn.Clear();
		explorer.ProcessesToReturn.Add(FakeProcessExplorer.Process(300, "MyApp", "ILSpy.Tests"));
		explorer.ModulesByPid[300] = new[] { OnDisk("ILSpy.Tests.dll"), OnDisk("ILSpy.dll") };
		string[]? closedWith = null;
		vm.CloseRequested += paths => closedWith = paths;

		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 1);
		vm.SelectedProcess = vm.Processes[0];
		await Waiters.WaitForAsync(() => vm.AddEntryAssemblyCommand.CanExecute(null));

		vm.AddEntryAssemblyCommand.Execute(null);

		closedWith.Should().Equal(new[] { TestFile("ILSpy.Tests.dll") });
	}

	[AvaloniaTest]
	public async Task A_Process_Whose_Entry_Assembly_Is_Unknown_Offers_Nothing_To_Add()
	{
		var (vm, explorer) = CreateViewModel();
		explorer.ProcessesToReturn.Clear();
		explorer.ProcessesToReturn.Add(FakeProcessExplorer.Process(400, "ghost", entryAssembly: null));
		explorer.ModulesByPid[400] = Array.Empty<ProcessModuleInfo>();

		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 1);
		vm.SelectedProcess = vm.Processes[0];
		await Waiters.WaitForAsync(() => !vm.IsLoadingModules);

		vm.AddEntryAssemblyCommand.CanExecute(null).Should().BeFalse();
		vm.AddSelectedModulesCommand.CanExecute(null).Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task A_Superseded_Assembly_Load_Does_Not_Overwrite_The_Current_One()
	{
		var (vm, explorer) = CreateViewModel();
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);

		var gate = new TaskCompletionSource();
		explorer.ModulesGate = gate;
		vm.SelectedProcess = vm.Processes[0];
		await Waiters.WaitForAsync(() => explorer.ModuleCalls.Count == 1);

		// The user moves on before the first process answers.
		explorer.ModulesGate = null;
		vm.SelectedProcess = vm.Processes[1];
		await Waiters.WaitForAsync(() => vm.Modules.Count == 2);
		gate.SetResult();
		await Task.Delay(50);

		vm.Modules.Select(m => m.Name).Should().Equal(new[] { "ILSpy.Tests.dll", "Dynamic.Proxies" },
			"the abandoned query must not paint its result over the current selection");
	}

	[AvaloniaTest]
	public async Task A_Failing_Process_Scan_Surfaces_The_Error_And_Clears_On_Retry()
	{
		var (vm, explorer) = CreateViewModel();
		explorer.ProcessesException = new IOException("the diagnostics port is unreachable");

		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.ErrorMessage != null);

		vm.ErrorMessage.Should().Contain("the diagnostics port is unreachable");
		vm.IsLoadingProcesses.Should().BeFalse();

		explorer.ProcessesException = null;
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);
		vm.ErrorMessage.Should().BeNull("a successful retry must dismiss the stale error");
	}

	[AvaloniaTest]
	public async Task A_Failing_Assembly_Query_Surfaces_The_Error_Without_Closing()
	{
		var (vm, explorer) = CreateViewModel();
		bool closed = false;
		vm.CloseRequested += _ => closed = true;
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);

		explorer.ModulesException = new UnauthorizedAccessException("access denied");
		vm.SelectedProcess = vm.Processes[0];
		await Waiters.WaitForAsync(() => vm.ErrorMessage != null);

		vm.ErrorMessage.Should().Contain("access denied");
		vm.Modules.Should().BeEmpty();
		vm.IsLoadingModules.Should().BeFalse();
		closed.Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Refreshing_While_Assemblies_Are_Loading_Stops_The_Progress_Bar()
	{
		var (vm, explorer) = CreateViewModel();
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);
		explorer.ModulesGate = new TaskCompletionSource();
		vm.SelectedProcess = vm.Processes[0];
		await Waiters.WaitForAsync(() => vm.IsLoadingModules);

		// Refresh drops the selection, so no assembly list is being loaded any more - while
		// the query that was in flight has been superseded and can no longer report that it
		// stopped. Something has to turn the indicator off, or it animates over an empty pane
		// until the user selects another process.
		explorer.ModulesGate = null;
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.SelectedProcess == null && vm.Processes.Count == 2);

		vm.IsLoadingModules.Should().BeFalse("nothing is loading, so nothing may animate");
	}

	[AvaloniaTest]
	public async Task A_Failed_Scan_Leaves_Nothing_Selected_To_Act_On()
	{
		var (vm, explorer) = CreateViewModel();
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);
		vm.SelectedProcess = vm.Processes[0];
		await Waiters.WaitForAsync(() => vm.AddEntryAssemblyCommand.CanExecute(null));

		explorer.ProcessesException = new IOException("the diagnostics port is unreachable");
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.ErrorMessage != null);

		// The list the selection pointed into has just been emptied; leaving the selection
		// behind keeps the entry-assembly button armed against a row nobody can see.
		vm.Processes.Should().BeEmpty();
		vm.SelectedProcess.Should().BeNull();
		vm.AddEntryAssemblyCommand.CanExecute(null).Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Closing_The_Dialog_Cancels_Work_Still_In_Flight()
	{
		var (vm, explorer) = CreateViewModel();
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Processes.Count == 2);
		explorer.ModulesGate = new TaskCompletionSource();
		vm.SelectedProcess = vm.Processes[0];
		await Waiters.WaitForAsync(() => explorer.ModuleCalls.Count == 1);

		vm.CancelAllOperations();

		explorer.LastModulesToken.IsCancellationRequested.Should().BeTrue(
			"a dialog that is gone must not keep a rundown session open");
	}
}
