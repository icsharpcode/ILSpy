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

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using ICSharpCode.ILSpy.Processes;
using ICSharpCode.ILSpy.ViewModels;

// Alias the shared Resources class - Window inherits an IResourceDictionary Resources
// property that would otherwise shadow ICSharpCode.ILSpy.Properties.Resources.
using Loc = ICSharpCode.ILSpy.Properties.Resources;

namespace ICSharpCode.ILSpy.Views
{
	/// <summary>
	/// "Open from Running Process" dialog: lists the running .NET processes, shows the
	/// assemblies loaded in the selected one, and closes with the paths to open - either the
	/// assemblies picked in the grid or the process's entry assembly, which for a modern app
	/// is the dll behind its native host. Closes with null when cancelled; all behavior lives
	/// in <see cref="OpenFromProcessDialogViewModel"/>.
	/// </summary>
	public partial class OpenFromProcessDialog : Window
	{
		readonly OpenFromProcessDialogViewModel viewModel;

		// Runtime-loader/designer constructor; production callers and tests use the overload
		// below to inject the explorer (a fake one, in tests).
		public OpenFromProcessDialog()
			: this(new ProcessExplorer())
		{
		}

		public OpenFromProcessDialog(IProcessExplorer explorer)
		{
			InitializeComponent();

			viewModel = new OpenFromProcessDialogViewModel(explorer);
			DataContext = viewModel;

			Title = Loc.OpenFromProcess_Title;
			this.FindControl<Label>("FilterLabel")!.Content = Loc.OpenFromProcess_Filter;
			this.FindControl<Button>("RefreshButton")!.Content = Loc.OpenFromProcess_Refresh;
			this.FindControl<TextBlock>("ModulesHeader")!.Text = Loc.OpenFromProcess_Assemblies;
			this.FindControl<TextBlock>("VisibilityHint")!.Text = Loc.OpenFromProcess_VisibilityHint;
			this.FindControl<Button>("AddEntryAssemblyButton")!.Content = Loc.OpenFromProcess_AddEntryAssembly;
			this.FindControl<Button>("AddSelectedButton")!.Content = Loc.OpenFromProcess_AddSelected;
			var cancelButton = this.FindControl<Button>("CancelButton")!;
			cancelButton.Content = Loc.Cancel;

			SetColumnHeaders();

			// A DataGrid's multi-selection is not bindable, so the grid pushes it into the
			// view model, which is what the Add button's command acts on.
			var modulesGrid = this.FindControl<DataGrid>("ModulesGrid")!;
			modulesGrid.SelectionChanged += (_, _) => {
				viewModel.SelectedModules.Clear();
				foreach (var module in modulesGrid.SelectedItems.OfType<ProcessModuleRowViewModel>())
					viewModel.SelectedModules.Add(module);
			};

			cancelButton.Click += (_, _) => Close(null);
			viewModel.CloseRequested += paths => Close(paths);
			Closed += (_, _) => viewModel.CancelAllOperations();

			// Scanning takes a moment, so it starts as the dialog appears rather than
			// waiting for the user to ask for a list that is always wanted.
			Opened += (_, _) => viewModel.RefreshCommand.Execute(null);
			Opened += (_, _) => this.FindControl<TextBox>("FilterBox")!.Focus();
		}

		void SetColumnHeaders()
		{
			var processes = this.FindControl<DataGrid>("ProcessesGrid")!;
			processes.Columns[0].Header = Loc.OpenFromProcess_Process;
			processes.Columns[1].Header = Loc.OpenFromProcess_Pid;
			processes.Columns[2].Header = Loc.OpenFromProcess_Runtime;
			processes.Columns[3].Header = Loc.OpenFromProcess_Architecture;
			processes.Columns[4].Header = Loc.OpenFromProcess_EntryAssembly;

			var modules = this.FindControl<DataGrid>("ModulesGrid")!;
			modules.Columns[0].Header = Loc.Assembly;
			modules.Columns[1].Header = Loc.OpenFromProcess_Location;
		}

		void InitializeComponent() => AvaloniaXamlLoader.Load(this);
	}
}
