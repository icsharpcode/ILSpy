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
using System.Collections.ObjectModel;
using System.Composition;
using System.Runtime.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;
using ICSharpCode.ILSpyX.TreeView;

using ILSpy.Languages;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;

namespace ILSpy.AssemblyTree
{
	[Export]
	[Shared]
	public partial class AssemblyTreeModel : ToolPaneModel
	{
		public const string PaneContentId = "AssemblyTree";

		readonly SettingsService settingsService;
		readonly LanguageService languageService;
		AssemblyListManager? listManager;

		[ObservableProperty]
		[property: IgnoreDataMember]
		private SharpTreeNode? root;

		[ObservableProperty]
		[property: IgnoreDataMember]
		private SharpTreeNode? selectedItem;

		[ObservableProperty]
		[property: IgnoreDataMember]
		private string? activeListName;

		[IgnoreDataMember]
		public AssemblyList? AssemblyList { get; private set; }

		[IgnoreDataMember]
		public ObservableCollection<string> AssemblyLists { get; } = [];

		[ImportingConstructor]
		public AssemblyTreeModel(SettingsService settingsService, LanguageService languageService)
		{
			this.settingsService = settingsService;
			this.languageService = languageService;
			languageService.PropertyChanged += (_, e) => {
				if (e.PropertyName == nameof(LanguageService.CurrentLanguage) && Root != null)
					NotifyTextChanged(Root);
			};
			Id = PaneContentId;
			Title = "Assemblies";
			CanClose = false;
		}

		// Walks already-materialized children and re-raises Text PropertyChanged so the cell
		// templates pick up the new language's formatting -- without collapsing the user's
		// expanded state. Lazy-loaded subtrees that haven't been opened yet are skipped (they'll
		// format with the active language the next time they get expanded).
		static void NotifyTextChanged(SharpTreeNode node)
		{
			node.RaisePropertyChanged(nameof(SharpTreeNode.Text));
			if (node.LazyLoading)
				return;
			foreach (var child in node.Children)
				NotifyTextChanged(child);
		}

		public void Initialize()
		{
			var settings = ILSpySettings.Load();
			listManager = new AssemblyListManager(settings);
			listManager.CreateDefaultAssemblyLists();

			SyncListNames();
			listManager.AssemblyLists.CollectionChanged += (_, _) => SyncListNames();

			var saved = settingsService.SessionSettings.ActiveAssemblyList;
			ActiveListName = !string.IsNullOrEmpty(saved) && AssemblyLists.Contains(saved)
				? saved
				: AssemblyListManager.DefaultListName;
		}

		void SyncListNames()
		{
			if (listManager == null)
				return;
			AssemblyLists.Clear();
			foreach (var name in listManager.AssemblyLists)
				AssemblyLists.Add(name);
		}

		partial void OnActiveListNameChanged(string? value)
		{
			if (listManager == null || string.IsNullOrEmpty(value))
				return;

			AssemblyList = listManager.LoadList(value);

			if (AssemblyList.GetAssemblies().Length == 0 && value == AssemblyListManager.DefaultListName)
				LoadInitialAssemblies(AssemblyList);

			Root = new AssemblyListTreeNode(AssemblyList);

			settingsService.SessionSettings.ActiveAssemblyList = value;
		}

		static void LoadInitialAssemblies(AssemblyList assemblyList)
		{
			System.Reflection.Assembly[] initialAssemblies = {
				typeof(object).Assembly,
				typeof(Uri).Assembly,
				typeof(System.Linq.Enumerable).Assembly,
			};
			foreach (var asm in initialAssemblies)
			{
				if (!string.IsNullOrEmpty(asm.Location))
					assemblyList.OpenAssembly(asm.Location);
			}
		}

		public void OpenAssembly(string path)
		{
			AssemblyList?.Open(path);
		}
	}
}
