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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using ICSharpCode.Decompiler.Metadata;

// Alias the WPF-shared Resources class — Window inherits an IResourceDictionary Resources
// property that would otherwise shadow ICSharpCode.ILSpy.Properties.Resources, turning every
// `Resources.X` into an IResourceDictionary indexer lookup that doesn't compile.
using Loc = ICSharpCode.ILSpy.Properties.Resources;

namespace ICSharpCode.ILSpy.Views
{
	/// <summary>
	/// "Open from GAC" dialog: enumerates the Global Assembly Cache via the shared
	/// <see cref="UniversalAssemblyResolver.EnumerateGac"/> + <see cref="UniversalAssemblyResolver.GetAssemblyInGac"/>
	/// pair and surfaces each entry in a five-column DataGrid (Reference Name / Version /
	/// Culture / Public Key Token / Location) — each header click-to-sort, initial sort
	/// by name ascending. Multi-select; returns the selected absolute file paths via
	/// <see cref="SelectedFileNames"/>. Windows-only — the GAC doesn't exist on Linux/macOS,
	/// and the command's <c>CanExecute</c> gates on <see cref="OperatingSystem.IsWindows"/>.
	/// </summary>
	public partial class OpenFromGacDialog : Window
	{
		readonly ObservableCollection<GacEntry> allEntries = new();
		readonly ObservableCollection<GacEntry> filteredEntries = new();
		readonly CancellationTokenSource fetchCts = new();

		DataGrid entriesGrid = null!;
		TextBox filterBox = null!;
		Label filterLabel = null!;
		ProgressBar loadingBar = null!;
		Button okButton = null!;
		Button cancelButton = null!;
		DataGridColumn nameColumn = null!;

		public OpenFromGacDialog()
		{
			InitializeComponent();
			entriesGrid = this.FindControl<DataGrid>("EntriesGrid")!;
			filterBox = this.FindControl<TextBox>("FilterBox")!;
			filterLabel = this.FindControl<Label>("FilterLabel")!;
			loadingBar = this.FindControl<ProgressBar>("LoadingBar")!;
			okButton = this.FindControl<Button>("OkButton")!;
			cancelButton = this.FindControl<Button>("CancelButton")!;
			nameColumn = this.FindControl<DataGrid>("EntriesGrid")!.Columns[0];

			Title = Loc.OpenFrom;
			// OpenListDialog__Open and _Search carry leading "_" Win32-mnemonic markers
			// (e.g. "_Open"). Avalonia's Label/Button use "_" for the same access-key purpose,
			// so the strings drop in unchanged; the Label.Target binding wires Alt+S to the
			// filter box exactly as WPF's <Label Target=...> did.
			filterLabel.Content = Loc._Search;
			okButton.Content = Loc.OpenListDialog__Open;
			cancelButton.Content = Loc.Cancel;

			// Column headers — set in code-behind rather than AXAML because the localised
			// strings live in the linked WPF Resources.resx; binding `{x:Static ...}` from
			// AXAML would need a namespace mapping per Window and is more boilerplate than
			// just five assignments here.
			entriesGrid.Columns[0].Header = Loc.ReferenceName;
			entriesGrid.Columns[1].Header = Loc.Version;
			entriesGrid.Columns[2].Header = Loc.CultureLabel;
			entriesGrid.Columns[3].Header = Loc.PublicToken;
			entriesGrid.Columns[4].Header = Loc.Location;

			entriesGrid.ItemsSource = filteredEntries;
			entriesGrid.SelectionChanged += (_, _) => okButton.IsEnabled = entriesGrid.SelectedItems!.Count > 0;
			filterBox.TextChanged += (_, _) => Refilter();
			okButton.Click += (_, _) => Close(SelectedFileNames);
			cancelButton.Click += (_, _) => Close(System.Array.Empty<string>());
			Closed += (_, _) => fetchCts.Cancel();

			// Initial sort: name ascending — matches WPF's
			// SortableGridViewColumn.SetCurrentSortColumn(listView, nameColumn) +
			// SetSortDirection(Ascending). Avalonia's DataGridColumn.Sort(direction) takes
			// System.ComponentModel.ListSortDirection; the call mutates the underlying view
			// and updates the header glyph.
			Opened += (_, _) => nameColumn.Sort(System.ComponentModel.ListSortDirection.Ascending);

			// Auto-focus the filter so typing starts narrowing immediately — WPF achieves the
			// same via FocusManager.FocusedElement on the Window.
			Opened += (_, _) => filterBox.Focus();

			_ = FetchAsync();
		}

		void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		public string[] SelectedFileNames
			=> entriesGrid.SelectedItems!.OfType<GacEntry>().Select(e => e.FileName).ToArray();

		async Task FetchAsync()
		{
			var token = fetchCts.Token;
			var seen = new HashSet<string>();
			loadingBar.IsIndeterminate = true;
			try
			{
				// Two-phase progress, like WPF: indeterminate during EnumerateGac (we don't know
				// the count yet), then determinate while resolving each reference's on-disk path.
				// EnumerateGac is materialised to a list off-thread so the count is known before
				// the resolve pass starts.
				var references = await Task.Run(() => {
					var list = new List<AssemblyNameReference>();
					foreach (var reference in UniversalAssemblyResolver.EnumerateGac())
					{
						if (token.IsCancellationRequested)
							break;
						list.Add(reference);
					}
					return list;
				}, token);

				if (token.IsCancellationRequested)
					return;

				loadingBar.IsIndeterminate = false;
				loadingBar.Minimum = 0;
				loadingBar.Maximum = references.Count;
				loadingBar.Value = 0;

				await Task.Run(() => {
					foreach (var reference in references)
					{
						if (token.IsCancellationRequested)
							return;
						if (!seen.Add(reference.FullName))
							continue;
						var path = UniversalAssemblyResolver.GetAssemblyInGac(reference);
						if (path == null)
							continue;
						var entry = new GacEntry(reference, path);
						Dispatcher.UIThread.Post(() => {
							AddEntry(entry);
							loadingBar.Value++;
						});
					}
				}, token);
			}
			catch (OperationCanceledException)
			{
				// Window closed before the enumeration finished — nothing to do.
			}
			finally
			{
				loadingBar.IsVisible = false;
			}
		}

		void AddEntry(GacEntry entry)
		{
			allEntries.Add(entry);
			if (PassesFilter(entry))
				filteredEntries.Add(entry);
		}

		void Refilter()
		{
			filteredEntries.Clear();
			foreach (var entry in allEntries.Where(PassesFilter))
				filteredEntries.Add(entry);
		}

		bool PassesFilter(GacEntry entry)
		{
			var text = filterBox.Text?.Trim();
			if (string.IsNullOrEmpty(text))
				return true;
			// Match every space-separated token against the full assembly name OR the version
			// string — mirrors WPF's filter (FullName || FormattedVersion). All tokens must
			// match: "system 4.0" finds entries whose FullName contains "system" AND
			// FormattedVersion contains "4.0".
			foreach (var token in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
			{
				bool inFullName = entry.FullName.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
				bool inVersion = entry.FormattedVersion.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
				if (!inFullName && !inVersion)
					return false;
			}
			return true;
		}
	}
}
