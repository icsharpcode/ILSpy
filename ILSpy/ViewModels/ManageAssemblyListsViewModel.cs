// Copyright (c) 2019 AlphaSierraPapa for the SharpDevelop Team
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
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy.ViewModels
{
	public class ManageAssemblyListsViewModel : ViewModelBase
	{
		private readonly AssemblyListManager manager;
		private readonly Window parent;

		public ManageAssemblyListsViewModel(Window parent)
		{
			this.manager = MainWindow.Instance.AssemblyListManager;
			this.parent = parent;

			NewCommand = new DelegateCommand(ExecuteNew);
			CloneCommand = new DelegateCommand(ExecuteClone, CanExecuteClone);
			RenameCommand = new DelegateCommand(ExecuteRename, CanExecuteRename);
			ResetCommand = new DelegateCommand(ExecuteReset);
			DeleteCommand = new DelegateCommand(ExecuteDelete, CanExecuteDelete);
			CreatePreconfiguredAssemblyListCommand = new DelegateCommand<PreconfiguredAssemblyList>(ExecuteCreatePreconfiguredAssemblyList);
			SelectAssemblyListCommand = new DelegateCommand(ExecuteSelectAssemblyList, CanExecuteSelectAssemblyList);

			PreconfiguredAssemblyLists = new List<PreconfiguredAssemblyList>(ResolvePreconfiguredAssemblyLists());
		}

		IEnumerable<PreconfiguredAssemblyList> ResolvePreconfiguredAssemblyLists()
		{
			yield return new PreconfiguredAssemblyList(AssemblyListManager.DotNet4List);
			yield return new PreconfiguredAssemblyList(AssemblyListManager.DotNet35List);
			yield return new PreconfiguredAssemblyList(AssemblyListManager.ASPDotNetMVC3List);

			var basePath = DotNetCorePathFinder.FindDotNetExeDirectory();
			if (basePath == null)
				yield break;

			Dictionary<string, string> foundVersions = new Dictionary<string, string>();
			Dictionary<string, int> latestRevision = new Dictionary<string, int>();

			foreach (var sdkDir in Directory.GetDirectories(Path.Combine(basePath, "shared")))
			{
				if (sdkDir.EndsWith(".Ref", StringComparison.OrdinalIgnoreCase))
					continue;
				foreach (var versionDir in Directory.GetDirectories(sdkDir))
				{
					var match = Regex.Match(versionDir, @"[/\\](?<name>[A-z0-9.]+)[/\\](?<version>\d+\.\d)+(.(?<revision>\d+))?(?<suffix>-preview.*)?$");
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
			{
				yield return new PreconfiguredAssemblyList(pair.Key + "(." + latestRevision[pair.Key] + ")", pair.Value);
			}
		}

		public ObservableCollection<string> AssemblyLists => manager.AssemblyLists;

		public List<PreconfiguredAssemblyList> PreconfiguredAssemblyLists { get; }

		private string selectedAssemblyList;

		public string SelectedAssemblyList {
			get => selectedAssemblyList;
			set {
				if (selectedAssemblyList != value)
				{
					selectedAssemblyList = value;
					RaisePropertyChanged();
				}
			}
		}

		public ICommand NewCommand { get; }
		public ICommand CloneCommand { get; }
		public ICommand ResetCommand { get; }
		public ICommand RenameCommand { get; }
		public ICommand DeleteCommand { get; }
		public ICommand CreatePreconfiguredAssemblyListCommand { get; }
		public ICommand SelectAssemblyListCommand { get; }

		private void ExecuteNew()
		{
			CreateListDialog dlg = new CreateListDialog(Resources.NewList);
			dlg.Owner = parent;
			dlg.Closing += (s, args) => {
				if (dlg.DialogResult == true)
				{
					if (manager.AssemblyLists.Contains(dlg.ListName))
					{
						args.Cancel = true;
						MessageBox.Show(Resources.ListExistsAlready, null, MessageBoxButton.OK);
					}
				}
			};
			if (dlg.ShowDialog() == true)
			{
				manager.CreateList(dlg.ListName);
			}
		}

		private bool CanExecuteClone()
		{
			return selectedAssemblyList != null;
		}

		private void ExecuteClone()
		{
			CreateListDialog dlg = new CreateListDialog(Resources.NewList);
			dlg.Owner = parent;
			dlg.Closing += (s, args) => {
				if (dlg.DialogResult == true)
				{
					if (manager.AssemblyLists.Contains(dlg.ListName))
					{
						args.Cancel = true;
						MessageBox.Show(Resources.ListExistsAlready, null, MessageBoxButton.OK);
					}
				}
			};
			if (dlg.ShowDialog() == true)
			{
				manager.CloneList(SelectedAssemblyList, dlg.ListName);
			}
		}

		private void ExecuteReset()
		{
			if (MessageBox.Show(parent, Resources.ListsResetConfirmation,
				"ILSpy", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No, MessageBoxOptions.None) != MessageBoxResult.Yes)
				return;
			manager.ClearAll();
			manager.CreateDefaultAssemblyLists();
			MainWindow.Instance.SessionSettings.ActiveAssemblyList = manager.AssemblyLists[0];
		}

		private void ExecuteDelete()
		{
			if (MessageBox.Show(parent, Resources.ListDeleteConfirmation,
"ILSpy", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No, MessageBoxOptions.None) != MessageBoxResult.Yes)
				return;
			string assemblyList = SelectedAssemblyList;
			SelectedAssemblyList = null;
			int index = manager.AssemblyLists.IndexOf(assemblyList);
			manager.DeleteList(assemblyList);
			if (manager.AssemblyLists.Count > 0)
			{
				SelectedAssemblyList = manager.AssemblyLists[Math.Max(0, index - 1)];
				if (MainWindow.Instance.SessionSettings.ActiveAssemblyList == assemblyList)
				{
					MainWindow.Instance.SessionSettings.ActiveAssemblyList = SelectedAssemblyList;
				}
			}
		}

		private bool CanExecuteDelete()
		{
			return selectedAssemblyList != null;
		}

		private bool CanExecuteRename()
		{
			return selectedAssemblyList != null;
		}

		private void ExecuteRename()
		{
			CreateListDialog dlg = new CreateListDialog(Resources.RenameList);
			dlg.Owner = parent;
			dlg.ListName = selectedAssemblyList;
			dlg.ListNameBox.SelectAll();
			dlg.Closing += (s, args) => {
				if (dlg.DialogResult == true)
				{
					if (dlg.ListName == selectedAssemblyList)
					{
						args.Cancel = true;
						return;
					}
					if (manager.AssemblyLists.Contains(dlg.ListName))
					{
						args.Cancel = true;
						MessageBox.Show(Resources.ListExistsAlready, null, MessageBoxButton.OK);
					}
				}
			};
			if (dlg.ShowDialog() == true)
			{
				string assemblyList = SelectedAssemblyList;
				SelectedAssemblyList = dlg.ListName;
				manager.RenameList(assemblyList, dlg.ListName);
				if (MainWindow.Instance.SessionSettings.ActiveAssemblyList == assemblyList)
				{
					MainWindow.Instance.SessionSettings.ActiveAssemblyList = manager.AssemblyLists[manager.AssemblyLists.Count - 1];
				}
			}
		}

		private void ExecuteCreatePreconfiguredAssemblyList(PreconfiguredAssemblyList config)
		{
			CreateListDialog dlg = new CreateListDialog(Resources.AddPreconfiguredList);
			dlg.Owner = parent;
			dlg.ListName = config.Name;
			dlg.ListNameBox.SelectAll();
			dlg.Closing += (s, args) => {
				if (dlg.DialogResult == true)
				{
					if (manager.AssemblyLists.Contains(dlg.ListName))
					{
						args.Cancel = true;
						MessageBox.Show(Resources.ListExistsAlready, null, MessageBoxButton.OK);
					}
				}
			};
			if (dlg.ShowDialog() == true)
			{
				var list = manager.CreateDefaultList(config.Name, config.Path, dlg.ListName);
				if (list.Count > 0)
				{
					manager.AddListIfNotExists(list);
				}
			}
		}

		private bool CanExecuteSelectAssemblyList()
		{
			return SelectedAssemblyList != null;
		}

		private void ExecuteSelectAssemblyList()
		{
			MainWindow.Instance.SessionSettings.ActiveAssemblyList = SelectedAssemblyList;
			this.parent.Close();
		}
	}

	public class PreconfiguredAssemblyList
	{
		public string Name { get; }
		public string Path { get; }

		public PreconfiguredAssemblyList(string name, string path = null)
		{
			this.Name = name;
			this.Path = path;
		}
	}
}
