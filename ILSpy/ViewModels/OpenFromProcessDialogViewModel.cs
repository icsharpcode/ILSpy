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

		partial void OnSelectedProcessChanged(ProcessRowViewModel? value) => _ = LoadModulesAsync(value);

		[RelayCommand]
		void Refresh() => _ = RefreshAsync();

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
			refreshCts?.Cancel();
			modulesCts?.Cancel();
		}

		async Task RefreshAsync()
		{
			refreshCts?.Cancel();
			var cts = refreshCts = new CancellationTokenSource();
			IsLoadingProcesses = true;
			try
			{
				var processes = await explorer.GetProcessesAsync(cts.Token);
				if (cts.IsCancellationRequested)
					return;

				ErrorMessage = null;
				SelectedProcess = null;
				allProcesses.Clear();
				allProcesses.AddRange(processes.Select(p => new ProcessRowViewModel(p)));
				ApplyFilter();
			}
			catch (OperationCanceledException)
			{
				// Superseded by another refresh, or the dialog closed.
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
				allProcesses.Clear();
				ApplyFilter();
			}
			finally
			{
				if (!cts.IsCancellationRequested)
					IsLoadingProcesses = false;
			}
		}

		void ApplyFilter()
		{
			Processes.Clear();
			foreach (var process in allProcesses.Where(p => p.Matches(FilterText)))
				Processes.Add(process);
		}

		async Task LoadModulesAsync(ProcessRowViewModel? process)
		{
			modulesCts?.Cancel();
			var cts = modulesCts = new CancellationTokenSource();
			int generation = ++modulesGeneration;

			Modules.Clear();
			SelectedModules.Clear();
			SetEntryAssemblyPath(null);
			if (process == null)
				return;

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
