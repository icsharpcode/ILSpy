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
using Avalonia.Markup.Xaml;

using ICSharpCode.ILSpyX;

namespace ILSpy.Views
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
			this.FindControl<Button>("SelectButton")!.Click += (_, _) => SelectAndClose();
			this.FindControl<Button>("CloseButton")!.Click += (_, _) => Close();
		}

		string? SelectedListName => listsBox.SelectedItem as string;

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
			if (SelectedListName is { } selected)
				settingsService.SessionSettings.ActiveAssemblyList = selected;
			Close();
		}
	}
}
