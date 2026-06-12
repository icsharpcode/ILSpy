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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy.Views
{
	/// <summary>
	/// Manage Assembly Lists dialog: list of saved assembly-list names with New / Clone /
	/// Rename / Delete / Reset operations + a Select button that swaps the active list.
	/// Mirrors WPF's <c>ManageAssemblyListsDialog</c>; all CRUD ops route through the
	/// shared <see cref="AssemblyListManager"/>.
	/// </summary>
	public partial class ManageAssemblyListsDialog : Window
	{
		readonly SettingsService settingsService;
		readonly AssemblyListManager manager;
		readonly ListBox listsBox;

		public ManageAssemblyListsDialog() : this(NullSettingsService()) { }

		public ManageAssemblyListsDialog(SettingsService settingsService)
		{
			InitializeComponent();
			this.settingsService = settingsService;
			this.manager = settingsService.AssemblyListManager;
			listsBox = this.FindControl<ListBox>("ListsBox")!;
			listsBox.ItemsSource = manager.AssemblyLists;
			WireButtons();
		}

		void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		// Design-time-only no-op fallback so XAML preview doesn't blow up on the null
		// SettingsService when a designer instantiates the dialog without composition.
		static SettingsService NullSettingsService()
			=> AppEnv.AppComposition.Current.GetExport<SettingsService>();

		void WireButtons()
		{
			this.FindControl<Button>("NewButton")!.Click += async (_, _) => await NewListAsync();
			this.FindControl<Button>("CloneButton")!.Click += async (_, _) => await CloneListAsync();
			this.FindControl<Button>("RenameButton")!.Click += async (_, _) => await RenameListAsync();
			this.FindControl<Button>("DeleteButton")!.Click += (_, _) => DeleteList();
			this.FindControl<Button>("ResetButton")!.Click += (_, _) => ResetLists();
			var addPreconfigured = this.FindControl<Button>("AddPreconfiguredButton")!;
			addPreconfigured.Click += (_, _) => ShowPreconfiguredMenu(addPreconfigured);
			this.FindControl<Button>("SelectButton")!.Click += (_, _) => SelectAndClose();
			this.FindControl<Button>("CloseButton")!.Click += (_, _) => Close();
			// Double-clicking an entry is a shortcut for the Select button: activate that list and
			// close. Guarded to a real entry so a double-click on empty list space does nothing.
			listsBox.DoubleTapped += OnListDoubleTapped;
		}

		void OnListDoubleTapped(object? sender, TappedEventArgs e)
		{
			var visual = e.Source as global::Avalonia.Visual;
			while (visual is not null and not ListBoxItem)
				visual = visual.GetVisualParent();
			if (visual is ListBoxItem)
				SelectAndClose();
		}

		void ShowPreconfiguredMenu(Button anchor)
		{
			var flyout = new MenuFlyout();
			foreach (var config in GetPreconfiguredAssemblyLists())
			{
				var captured = config;
				var item = new MenuItem { Header = captured.Name };
				item.Click += async (_, _) => await AddPreconfiguredListAsync(captured);
				flyout.Items.Add(item);
			}
			flyout.ShowAt(anchor);
		}

		string? SelectedListName => listsBox.SelectedItem as string;

		/// <summary>The lists control, exposed for tests to drive the selection.</summary>
		internal ListBox ListsControl => listsBox;

		async Task<string?> PromptAsync(string title, string? initialText = null)
		{
			var dlg = new CreateListDialog(title, initialText);
			return await dlg.ShowDialog<string?>(this);
		}

		async Task NewListAsync()
		{
			var name = await PromptAsync("New Assembly List");
			if (string.IsNullOrWhiteSpace(name) || manager.AssemblyLists.Contains(name))
				return;
			var list = manager.CreateList(name);
			manager.AddListIfNotExists(list);
		}

		async Task CloneListAsync()
		{
			if (SelectedListName is not { } selected)
				return;
			var name = await PromptAsync("Clone Assembly List");
			if (string.IsNullOrWhiteSpace(name) || manager.AssemblyLists.Contains(name))
				return;
			manager.CloneList(selected, name);
		}

		async Task RenameListAsync()
		{
			if (SelectedListName is not { } selected)
				return;
			var name = await PromptAsync("Rename Assembly List", selected);
			if (string.IsNullOrWhiteSpace(name) || name == selected || manager.AssemblyLists.Contains(name))
				return;
			manager.RenameList(selected, name);
			if (settingsService.SessionSettings.ActiveAssemblyList == selected)
				settingsService.SessionSettings.ActiveAssemblyList = name;
		}

		void DeleteList()
		{
			if (SelectedListName is not { } selected)
				return;
			int index = manager.AssemblyLists.IndexOf(selected);
			manager.DeleteList(selected);
			if (manager.AssemblyLists.Count > 0)
			{
				listsBox.SelectedIndex = Math.Max(0, index - 1);
				if (settingsService.SessionSettings.ActiveAssemblyList == selected)
					settingsService.SessionSettings.ActiveAssemblyList = manager.AssemblyLists[Math.Max(0, index - 1)];
			}
		}

		void ResetLists()
		{
			manager.ClearAll();
			manager.CreateDefaultAssemblyLists();
			if (manager.AssemblyLists.Count > 0)
				settingsService.SessionSettings.ActiveAssemblyList = manager.AssemblyLists[0];
		}

		void SelectAndClose()
		{
			ActivateSelectedList();
			Close();
		}

		/// <summary>
		/// Switches the live active assembly list to the selected one. Goes through
		/// <see cref="AssemblyTree.AssemblyTreeModel.ActiveListName"/> rather than poking the
		/// session setting directly, because that setter is what reloads the tree
		/// (<c>OnActiveListNameChanged</c> -> <c>ShowAssemblyList</c>) and persists the choice.
		/// Setting the session setting alone only takes effect on the next launch.
		/// </summary>
		internal void ActivateSelectedList()
		{
			if (SelectedListName is not { } selected)
				return;
			AppEnv.AppComposition.Current.GetExport<AssemblyTree.AssemblyTreeModel>().ActiveListName = selected;
		}

		/// <summary>
		/// The list of preconfigured assembly lists offered by the "Add preconfigured list..."
		/// menu: the three GAC-based framework lists (only when a GAC is actually present, i.e.
		/// on Windows) plus one entry per installed .NET runtime discovered under the dotnet
		/// install's <c>shared</c> folder (works on every platform). Mirrors WPF's
		/// ManageAssemblyListsViewModel.ResolvePreconfiguredAssemblyLists.
		/// </summary>
		internal IEnumerable<PreconfiguredAssemblyList> GetPreconfiguredAssemblyLists()
		{
			if (IsGacAvailable())
			{
				yield return new PreconfiguredAssemblyList(AssemblyListManager.DotNet4List);
				yield return new PreconfiguredAssemblyList(AssemblyListManager.DotNet35List);
				yield return new PreconfiguredAssemblyList(AssemblyListManager.ASPDotNetMVC3List);
			}

			var basePath = DotNetCorePathFinder.FindDotNetExeDirectory();
			if (basePath == null)
				yield break;

			var sharedRoot = Path.Combine(basePath, "shared");
			if (!Directory.Exists(sharedRoot))
				yield break;

			var foundVersions = new Dictionary<string, string>();
			var latestRevision = new Dictionary<string, int>();
			foreach (var sdkDir in Directory.GetDirectories(sharedRoot))
			{
				if (sdkDir.EndsWith(".Ref", StringComparison.OrdinalIgnoreCase))
					continue;
				foreach (var versionDir in Directory.GetDirectories(sdkDir))
				{
					var match = Regex.Match(versionDir, @"[/\\](?<name>[A-z0-9.]+)[/\\](?<version>\d+\.\d+)(.(?<revision>\d+))?(?<suffix>-.*)?$");
					if (!match.Success)
						continue;
					string name = match.Groups["name"].Value;
					int index = name.LastIndexOfAny(new[] { '/', '\\' });
					if (index >= 0)
						name = name.Substring(index + 1);
					string text = name + " " + match.Groups["version"].Value;
					if (!latestRevision.TryGetValue(text, out int revision))
						revision = -1;
					int newRevision = int.Parse(match.Groups["revision"].Value);
					if (newRevision > revision)
					{
						latestRevision[text] = newRevision;
						foundVersions[text] = versionDir;
					}
				}
			}

			foreach (var pair in foundVersions)
				yield return new PreconfiguredAssemblyList($"{pair.Key}(.{latestRevision[pair.Key]})", pair.Value);
		}

		static bool IsGacAvailable()
			=> UniversalAssemblyResolver.GetGacPaths().Any(Directory.Exists);

		async Task AddPreconfiguredListAsync(PreconfiguredAssemblyList config)
		{
			var name = await PromptAsync("Add preconfigured list...", config.Name);
			if (string.IsNullOrWhiteSpace(name) || manager.AssemblyLists.Contains(name))
				return;
			CreatePreconfiguredList(config, name);
		}

		/// <summary>
		/// Builds the preconfigured list via the shared <see cref="AssemblyListManager"/> and
		/// registers it under <paramref name="newName"/>, but only if it actually resolved any
		/// assemblies (the GAC-based lists yield nothing off Windows). Returns the created list,
		/// or <see langword="null"/> when nothing was added. Separated from the prompt so it can
		/// be driven directly by tests.
		/// </summary>
		internal AssemblyList? CreatePreconfiguredList(PreconfiguredAssemblyList config, string newName)
		{
			var list = manager.CreateDefaultList(config.Name, config.Path, newName);
			if (list.Count == 0)
				return null;
			manager.AddListIfNotExists(list);
			return list;
		}

		internal sealed record PreconfiguredAssemblyList(string Name, string? Path = null);
	}
}
