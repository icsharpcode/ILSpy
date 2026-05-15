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
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using ICSharpCode.Decompiler.Metadata;

namespace ILSpy.Views
{
	/// <summary>
	/// "Open from GAC" dialog: enumerates the Global Assembly Cache via the shared
	/// <see cref="UniversalAssemblyResolver.EnumerateGac"/> + <see cref="UniversalAssemblyResolver.GetAssemblyInGac"/>
	/// pair, surfaces the assemblies in a filterable list, returns the selected
	/// absolute file paths. Windows-only — the GAC doesn't exist on Linux/macOS, and
	/// the command's CanExecute gates on <c>OperatingSystem.IsWindows()</c>.
	/// </summary>
	public partial class OpenFromGacDialog : Window
	{
		readonly ObservableCollection<GacEntry> allEntries = new();
		readonly ObservableCollection<GacEntry> filteredEntries = new();
		readonly System.Threading.CancellationTokenSource fetchCts = new();

		ListBox entriesList = null!;
		TextBox filterBox = null!;
		ProgressBar loadingBar = null!;
		Button okButton = null!;

		public OpenFromGacDialog()
		{
			InitializeComponent();
			entriesList = this.FindControl<ListBox>("EntriesList")!;
			filterBox = this.FindControl<TextBox>("FilterBox")!;
			loadingBar = this.FindControl<ProgressBar>("LoadingBar")!;
			okButton = this.FindControl<Button>("OkButton")!;
			entriesList.ItemsSource = filteredEntries;
			entriesList.SelectionChanged += (_, _) => okButton.IsEnabled = entriesList.SelectedItems!.Count > 0;
			filterBox.TextChanged += (_, _) => Refilter();
			okButton.Click += (_, _) => Close(SelectedFileNames);
			this.FindControl<Button>("CancelButton")!.Click += (_, _) => Close(System.Array.Empty<string>());
			Closed += (_, _) => fetchCts.Cancel();
			_ = FetchAsync();
		}

		void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		public string[] SelectedFileNames
			=> entriesList.SelectedItems!.OfType<GacEntry>().Select(e => e.FileName).ToArray();

		async Task FetchAsync()
		{
			var token = fetchCts.Token;
			var seen = new HashSet<string>();
			try
			{
				await Task.Run(() => {
					foreach (var reference in UniversalAssemblyResolver.EnumerateGac())
					{
						if (token.IsCancellationRequested)
							return;
						if (!seen.Add(reference.FullName))
							continue;
						var path = UniversalAssemblyResolver.GetAssemblyInGac(reference);
						if (path == null)
							continue;
						var entry = new GacEntry(reference, path);
						Dispatcher.UIThread.Post(() => AddEntry(entry));
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
			var label = entry.ToString();
			foreach (var token in text.Split(' ', System.StringSplitOptions.RemoveEmptyEntries))
			{
				if (label.IndexOf(token, System.StringComparison.OrdinalIgnoreCase) < 0)
					return false;
			}
			return true;
		}

		sealed class GacEntry(AssemblyNameReference reference, string fileName)
		{
			public string FileName { get; } = fileName;
			public override string ToString()
				=> $"{reference.Name}, Version={reference.Version}, Culture={(reference.Culture is { Length: > 0 } c ? c : "neutral")}, PublicKeyToken={FormatPublicKey(reference)}";

			static string FormatPublicKey(AssemblyNameReference reference)
			{
				var token = reference.PublicKeyToken;
				if (token == null || token.Length == 0)
					return "null";
				var sb = new System.Text.StringBuilder(token.Length * 2);
				foreach (var b in token)
					sb.AppendFormat("{0:x2}", b);
				return sb.ToString();
			}
		}
	}
}
