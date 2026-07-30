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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ICSharpCode.ILSpy.Processes;

namespace ICSharpCode.ILSpy.ViewModels
{
	/// <summary>
	/// Drives the "Open from Running Process" dialog: lists the running .NET processes,
	/// fetches the assemblies loaded in the selected one, and hands the chosen files back to
	/// be opened. All process inspection goes through <see cref="IProcessExplorer"/>; errors
	/// land in <see cref="ErrorMessage"/> rather than escaping. The dialog closes itself when
	/// <see cref="CloseRequested"/> fires with the paths to open.
	/// </summary>
	public sealed partial class OpenFromProcessDialogViewModel : ViewModelBase
	{
		readonly IProcessExplorer explorer;
		readonly List<ProcessRowViewModel> allProcesses = new();

		CancellationTokenSource? refreshCts;
		CancellationTokenSource? modulesCts;
		// Distinguishes the current assembly query from ones the user has moved on from: a
		// superseded query must not publish its result over the current selection.
		int modulesGeneration;

		string? entryAssemblyPath;

		[ObservableProperty]
		string filterText = string.Empty;

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(AddEntryAssemblyCommand))]
		ProcessRowViewModel? selectedProcess;

		[ObservableProperty]
		bool isLoadingProcesses;

		[ObservableProperty]
		bool isLoadingModules;

		[ObservableProperty]
		string? errorMessage;

		public ObservableCollection<ProcessRowViewModel> Processes { get; } = new();

		public ObservableCollection<ProcessModuleRowViewModel> Modules { get; } = new();

		/// <summary>
		/// The assembly rows the user has picked in the grid; the view keeps this in sync
		/// with the grid's selection.
		/// </summary>
		public ObservableCollection<ProcessModuleRowViewModel> SelectedModules { get; } = new();

		/// <summary>
		/// Raised with the assembly paths to open, or null when the dialog is dismissed; the
		/// view responds by closing the dialog with that result.
		/// </summary>
		public event Action<string[]?>? CloseRequested;

		public OpenFromProcessDialogViewModel(IProcessExplorer explorer)
		{
			this.explorer = explorer;
			SelectedModules.CollectionChanged += (_, _) => AddSelectedModulesCommand.NotifyCanExecuteChanged();
		}

		partial void OnFilterTextChanged(string value) => ApplyFilter();

		partial void OnSelectedProcessChanged(ProcessRowViewModel? value)
			=> LoadModulesAsync(value).HandleExceptions();

		[RelayCommand]
		void Refresh() => RefreshAsync().HandleExceptions();

		bool CanAddSelectedModules => SelectedModules.Any(m => !m.IsInMemory);

		[RelayCommand(CanExecute = nameof(CanAddSelectedModules))]
		void AddSelectedModules()
		{
			var paths = SelectedModules
				.Where(m => !m.IsInMemory && m.Path != null)
				.Select(m => m.Path!)
				.ToArray();
			if (paths.Length > 0)
				CloseRequested?.Invoke(paths);
		}

		bool CanAddEntryAssembly => entryAssemblyPath != null;

		[RelayCommand(CanExecute = nameof(CanAddEntryAssembly))]
		void AddEntryAssembly()
		{
			if (entryAssemblyPath != null)
				CloseRequested?.Invoke(new[] { entryAssemblyPath });
		}

		/// <summary>Cancels every in-flight query; called when the dialog closes.</summary>
		public void CancelAllOperations()
		{
			CancelAndDispose(ref refreshCts);
			CancelAndDispose(ref modulesCts);
		}

		/// <summary>
		/// Cancels a query's token source and lets go of it. Disposing straight after
		/// cancelling is safe because the cancellation callbacks have already run by then, and
		/// the method that owns the source only ever reads
		/// <see cref="CancellationTokenSource.IsCancellationRequested"/> afterwards, which
		/// stays valid on a disposed source.
		/// </summary>
		static void CancelAndDispose(ref CancellationTokenSource? source)
		{
			var previous = source;
			source = null;
			previous?.Cancel();
			previous?.Dispose();
		}

		async Task RefreshAsync()
		{
			CancelAndDispose(ref refreshCts);
			var cts = refreshCts = new CancellationTokenSource();
			IsLoadingProcesses = true;
			try
			{
				var processes = await explorer.GetProcessesAsync(cts.Token);
				if (cts.IsCancellationRequested)
					return;

				ErrorMessage = null;
				ReplaceProcesses(processes);
			}
			catch (OperationCanceledException)
			{
				// Superseded by another refresh, or the dialog closed.
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
				ReplaceProcesses(Array.Empty<RunningDotNetProcess>());
			}
			finally
			{
				if (!cts.IsCancellationRequested)
					IsLoadingProcesses = false;
			}
		}

		/// <summary>
		/// Puts a freshly scanned list in place of the current one. The selection goes first:
		/// the rows it pointed into are about to be gone, and a selection left behind keeps the
		/// entry-assembly button armed against a process nobody can see any more. The failure
		/// path empties the list the same way, for the same reason.
		/// </summary>
		void ReplaceProcesses(IReadOnlyList<RunningDotNetProcess> processes)
		{
			SelectedProcess = null;
			allProcesses.Clear();
			allProcesses.AddRange(processes.Select(p => new ProcessRowViewModel(p)));
			ApplyFilter();
		}

		/// <summary>
		/// Brings the bound collection in line with what the filter admits, by removing and
		/// inserting rows rather than rebuilding it. Clearing it would make the grid drop its
		/// selection and write that null back through the two-way binding - discarding the
		/// assembly list of a process the newly typed filter still matches.
		/// </summary>
		void ApplyFilter()
		{
			var matching = allProcesses.Where(p => p.Matches(FilterText)).ToList();
			var admitted = new HashSet<ProcessRowViewModel>(matching);
			// A selection the filter no longer admits is dropped here rather than left to the
			// grid, which holds on to a removed row: the assembly pane must not go on
			// describing a process that is no longer in the list.
			if (SelectedProcess != null && !admitted.Contains(SelectedProcess))
				SelectedProcess = null;
			for (int i = Processes.Count - 1; i >= 0; i--)
			{
				if (!admitted.Contains(Processes[i]))
					Processes.RemoveAt(i);
			}
			// What is left is the matching rows in their original relative order, so any row
			// missing at position i belongs exactly there.
			for (int i = 0; i < matching.Count; i++)
			{
				if (i >= Processes.Count)
					Processes.Add(matching[i]);
				else if (!ReferenceEquals(Processes[i], matching[i]))
					Processes.Insert(i, matching[i]);
			}
		}

		async Task LoadModulesAsync(ProcessRowViewModel? process)
		{
			CancelAndDispose(ref modulesCts);
			var cts = modulesCts = new CancellationTokenSource();
			int generation = ++modulesGeneration;

			Modules.Clear();
			SelectedModules.Clear();
			SetEntryAssemblyPath(null);
			if (process == null)
			{
				// Nothing is selected, so nothing is loading. The query just cancelled cannot
				// say so on its way out: its generation has been superseded, which is what the
				// guard below tests.
				IsLoadingModules = false;
				return;
			}

			IsLoadingModules = true;
			try
			{
				var modules = await explorer.GetModulesAsync(process.Process, cts.Token);
				if (generation != modulesGeneration)
					return;

				ErrorMessage = null;
				foreach (var module in modules)
					Modules.Add(new ProcessModuleRowViewModel(module));
				SetEntryAssemblyPath(process.Process.ResolveEntryAssemblyPath(modules));
			}
			catch (OperationCanceledException)
			{
				// The selection moved on or the dialog closed.
			}
			catch (Exception ex)
			{
				if (generation != modulesGeneration)
					return;
				ErrorMessage = ex.Message;
				// The command line still names an assembly worth offering even when the
				// process refused to list what it has loaded.
				SetEntryAssemblyPath(process.Process.ResolveEntryAssemblyPath(Array.Empty<ProcessModuleInfo>()));
			}
			finally
			{
				if (generation == modulesGeneration)
					IsLoadingModules = false;
			}
		}

		void SetEntryAssemblyPath(string? path)
		{
			entryAssemblyPath = path;
			AddEntryAssemblyCommand.NotifyCanExecuteChanged();
		}
	}
}
