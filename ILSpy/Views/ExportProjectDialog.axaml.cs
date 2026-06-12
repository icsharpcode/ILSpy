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

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;

namespace ICSharpCode.ILSpy.Views
{
	/// <summary>
	/// One row of the export preview: the project that will be written for an assembly plus the
	/// badges (invalid / duplicate-name / PDB-eligible) computed by
	/// <see cref="ExportProjectDialog.BuildPreviewRows"/>.
	/// </summary>
	public sealed record ProjectPreviewRow(
		string ProjectName, string TargetSubdirectory,
		bool IsValidAssembly, bool HasDuplicateShortName, bool IsPdbEligible)
	{
		public string Display => TargetSubdirectory is { Length: > 0 } sub && sub != "."
			? sub + " / " + ProjectName
			: ProjectName;

		public string BadgeText {
			get {
				var badges = new List<string>();
				if (!IsValidAssembly)
					badges.Add("not a valid assembly");
				if (HasDuplicateShortName)
					badges.Add("duplicate name");
				if (IsPdbEligible)
					badges.Add("PDB");
				return string.Join("   ", badges);
			}
		}
	}

	/// <summary>
	/// Configuration dialog for the "Export Project/Solution" command. Adapts to the selection
	/// (one valid assembly = project mode, several = solution mode), pre-fills format/decompiler
	/// toggles from the live settings, previews the projects to be written, and returns a
	/// <see cref="ProjectExportOptions"/> (or <c>null</c> on cancel) via
	/// <see cref="Window.ShowDialog{TResult}"/>. All export work happens in
	/// <see cref="ProjectExporter"/>; this window only collects choices.
	/// </summary>
	public partial class ExportProjectDialog : Window
	{
		readonly bool solutionMode;
		bool hasDuplicateConflict;

		// Design-time / parameterless ctor for the XAML previewer.
		public ExportProjectDialog() : this(TryResolveSettings(), System.Array.Empty<LoadedAssembly>(), false)
		{
		}

		public ExportProjectDialog(SettingsService? settingsService,
			IReadOnlyList<LoadedAssembly> assemblies, bool solutionMode)
		{
			InitializeComponent();
			this.solutionMode = solutionMode;
			Title = solutionMode ? "Export Solution" : "Export Project";

			var rows = BuildPreviewRows(assemblies, solutionMode);
			hasDuplicateConflict = solutionMode && rows.Any(r => r.HasDuplicateShortName);
			PreviewList.ItemsSource = rows;
			ConflictWarning.IsVisible = hasDuplicateConflict;

			var settings = settingsService?.DecompilerSettings;
			if (settings != null)
			{
				SdkStyleCheck.IsChecked = settings.UseSdkStyleProjectFormat;
				NestedDirsCheck.IsChecked = settings.UseNestedDirectoriesForNamespaces;
				RemoveDeadCodeCheck.IsChecked = settings.RemoveDeadCode;
				RemoveDeadStoresCheck.IsChecked = settings.RemoveDeadStores;
				UseDebugSymbolsCheck.IsChecked = settings.UseDebugSymbols;
			}

			BrowseOutputButton.Click += async (_, _) => {
				var folder = await FilePickers.PickFolderAsync("Select the export output folder");
				if (!string.IsNullOrEmpty(folder))
				{
					OutputBox.Text = folder;
					UpdateExportEnabled();
				}
			};
			BrowseKeyButton.Click += async (_, _) => await BrowseKeyFileAsync();

			// Embedding source into the PDB only makes sense once a PDB is being generated; default it
			// off in project mode (the .cs are already written to disk next to it).
			GeneratePdbCheck.IsChecked = false;
			EmbedSourceCheck.IsChecked = false;
			GeneratePdbCheck.IsCheckedChanged += (_, _) =>
				EmbedSourceCheck.IsEnabled = GeneratePdbCheck.IsChecked == true;

			ExportButton.Click += (_, _) => Close(BuildOptions());
			CancelButton.Click += (_, _) => Close(null);

			UpdateExportEnabled();
		}

		/// <summary>
		/// Computes one preview row per assembly: the project name + target subdirectory (solution
		/// mode places each project in a <c>ShortName</c> subfolder; project mode writes into the
		/// chosen folder directly) and the invalid / duplicate-name / PDB-eligible badges. Pure and
		/// side-effect free so it can be unit-tested without the window.
		/// </summary>
		internal static IReadOnlyList<ProjectPreviewRow> BuildPreviewRows(
			IReadOnlyList<LoadedAssembly> assemblies, bool solutionMode)
		{
			var duplicates = assemblies
				.GroupBy(a => a.ShortName)
				.Where(g => g.Count() > 1)
				.Select(g => g.Key)
				.ToHashSet();

			var rows = new List<ProjectPreviewRow>(assemblies.Count);
			foreach (var assembly in assemblies)
			{
				bool pdbEligible = false;
				try
				{
					pdbEligible = assembly.GetMetadataFileOrNull() is PEFile file
						&& PortablePdbWriter.HasCodeViewDebugDirectoryEntry(file);
				}
				catch
				{
					pdbEligible = false;
				}

				rows.Add(new ProjectPreviewRow(
					ProjectName: assembly.ShortName + ".csproj",
					TargetSubdirectory: solutionMode ? assembly.ShortName : ".",
					IsValidAssembly: assembly.IsLoadedAsValidAssembly,
					HasDuplicateShortName: duplicates.Contains(assembly.ShortName),
					IsPdbEligible: pdbEligible));
			}
			return rows;
		}

		ProjectExportOptions BuildOptions()
			=> new(
				OutputBox.Text ?? string.Empty,
				SdkStyleCheck.IsChecked == true,
				NestedDirsCheck.IsChecked == true,
				RemoveDeadCodeCheck.IsChecked == true,
				RemoveDeadStoresCheck.IsChecked == true,
				UseDebugSymbolsCheck.IsChecked == true,
				string.IsNullOrEmpty(KeyFileBox.Text) ? null : KeyFileBox.Text,
				GeneratePdbCheck.IsChecked == true,
				EmbedSourceCheck.IsChecked == true);

		void UpdateExportEnabled()
			=> ExportButton.IsEnabled = !string.IsNullOrEmpty(OutputBox.Text) && !hasDuplicateConflict;

		async System.Threading.Tasks.Task BrowseKeyFileAsync()
		{
			var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
				Title = "Select strong-name key file",
				AllowMultiple = false,
				FileTypeFilter = new[] {
					new FilePickerFileType("Strong-name key (*.snk)") { Patterns = new[] { "*.snk" } },
					FilePickerFileTypes.All,
				},
			});
			var path = files.Count > 0 ? files[0].TryGetLocalPath() : null;
			if (!string.IsNullOrEmpty(path))
				KeyFileBox.Text = path;
		}

		static SettingsService? TryResolveSettings()
		{
			try
			{ return AppComposition.Current.GetExport<SettingsService>(); }
			catch
			{ return null; }
		}
	}
}
