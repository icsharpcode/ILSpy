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

namespace ICSharpCode.ILSpy.ViewModels
{
	public class ManageAssemblyListsViewModel : ViewModelBase
	{
		public const string DotNet4List = ".NET 4 (WPF)";
		public const string DotNet35List = ".NET 3.5";
		public const string ASPDotNetMVC3List = "ASP.NET (MVC3)";

		private readonly AssemblyListManager manager;
		private readonly Window parent;

		public ManageAssemblyListsViewModel(Window parent)
		{
			this.manager = MainWindow.Instance.AssemblyListManager;
			this.parent = parent;
			CreateDefaultAssemblyLists();

			NewCommand = new DelegateCommand(ExecuteNew);
			CloneCommand = new DelegateCommand(ExecuteClone, CanExecuteClone);
			RenameCommand = new DelegateCommand(ExecuteRename, CanExecuteRename);
			ResetCommand = new DelegateCommand(ExecuteReset);
			DeleteCommand = new DelegateCommand(ExecuteDelete, CanExecuteDelete);
			CreatePreconfiguredAssemblyListCommand = new DelegateCommand<PreconfiguredAssemblyList>(ExecuteCreatePreconfiguredAssemblyList);

			PreconfiguredAssemblyLists = new List<PreconfiguredAssemblyList>(ResolvePreconfiguredAssemblyLists());
		}

		IEnumerable<PreconfiguredAssemblyList> ResolvePreconfiguredAssemblyLists()
		{
			yield return new PreconfiguredAssemblyList(DotNet4List);
			yield return new PreconfiguredAssemblyList(DotNet35List);
			yield return new PreconfiguredAssemblyList(ASPDotNetMVC3List);

			var basePath = DotNetCorePathFinder.FindDotNetExeDirectory();
			if (basePath == null)
				yield break;

			Dictionary<string, string> foundVersions = new Dictionary<string, string>();
			Dictionary<string, int> latestRevision = new Dictionary<string, int>();

			foreach (var sdkDir in Directory.GetDirectories(Path.Combine(basePath, "shared"))) {
				if (sdkDir.EndsWith(".Ref", StringComparison.OrdinalIgnoreCase))
					continue;
				foreach (var versionDir in Directory.GetDirectories(sdkDir)) {
					var match = Regex.Match(versionDir, @"[/\\](?<name>[A-z0-9.]+)[/\\](?<version>\d+\.\d)+(.(?<revision>\d+))?$");
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
					if (newRevision > revision) {
						latestRevision[text] = newRevision;
						foundVersions[text] = versionDir;
					}
				}
			}

			foreach (var pair in foundVersions) {
				yield return new PreconfiguredAssemblyList(pair.Key + "(." + latestRevision[pair.Key] + ")", pair.Value);
			}
		}

		public ObservableCollection<string> AssemblyLists => manager.AssemblyLists;

		public List<PreconfiguredAssemblyList> PreconfiguredAssemblyLists { get; }

		private string selectedAssemblyList;

		public string SelectedAssemblyList {
			get => selectedAssemblyList;
			set {
				if (selectedAssemblyList != value) {
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

		private void ExecuteNew()
		{
			CreateListDialog dlg = new CreateListDialog(Resources.NewList);
			dlg.Owner = parent;
			dlg.Closing += (s, args) => {
				if (dlg.DialogResult == true) {
					if (manager.AssemblyLists.Contains(dlg.ListName)) {
						args.Cancel = true;
						MessageBox.Show(Properties.Resources.ListExistsAlready, null, MessageBoxButton.OK);
					}
				}
			};
			if (dlg.ShowDialog() == true) {
				manager.CreateList(new AssemblyList(dlg.ListName));
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
				if (dlg.DialogResult == true) {
					if (manager.AssemblyLists.Contains(dlg.ListName)) {
						args.Cancel = true;
						MessageBox.Show(Properties.Resources.ListExistsAlready, null, MessageBoxButton.OK);
					}
				}
			};
			if (dlg.ShowDialog() == true) {
				manager.CloneList(SelectedAssemblyList, dlg.ListName);
			}
		}

		private void ExecuteReset()
		{
			if (MessageBox.Show(parent, Properties.Resources.ListsResetConfirmation,
				"ILSpy", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No, MessageBoxOptions.None) != MessageBoxResult.Yes)
				return;
			manager.ClearAll();
			CreateDefaultAssemblyLists();
			MainWindow.Instance.SessionSettings.ActiveAssemblyList = manager.AssemblyLists[0];
		}

		private void ExecuteDelete()
		{
			if (MessageBox.Show(parent, Properties.Resources.ListDeleteConfirmation,
"ILSpy", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No, MessageBoxOptions.None) != MessageBoxResult.Yes)
				return;
			manager.DeleteList(SelectedAssemblyList);
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
			dlg.Closing += (s, args) => {
				if (dlg.DialogResult == true) {
					if (manager.AssemblyLists.Contains(dlg.ListName)) {
						args.Cancel = true;
						MessageBox.Show(Properties.Resources.ListExistsAlready, null, MessageBoxButton.OK);
					}
				}
			};
			if (dlg.ShowDialog() == true) {
				manager.RenameList(selectedAssemblyList, dlg.ListName);
			}
		}

		private AssemblyList CreateDefaultList(string name, string path = null, string newName = null)
		{
			var list = new AssemblyList(newName ?? name);
			switch (name) {
				case DotNet4List:
					AddToListFromGAC("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Data.DataSetExtensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
					AddToListFromGAC("PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					AddToListFromGAC("PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					AddToListFromGAC("WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					break;
				case DotNet35List:
					AddToListFromGAC("mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Data.DataSetExtensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Xml.Linq, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("PresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					AddToListFromGAC("PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					AddToListFromGAC("WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					break;
				case ASPDotNetMVC3List:
					AddToListFromGAC("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					AddToListFromGAC("System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
					AddToListFromGAC("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Data.DataSetExtensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Data.Entity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
					AddToListFromGAC("System.EnterpriseServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
					AddToListFromGAC("System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
					AddToListFromGAC("System.Web.Abstractions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					AddToListFromGAC("System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					AddToListFromGAC("System.Web.DynamicData, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					AddToListFromGAC("System.Web.Entity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					AddToListFromGAC("System.Web.Mvc, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					AddToListFromGAC("System.Web.Routing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					AddToListFromGAC("System.Web.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
					AddToListFromGAC("System.Web.WebPages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					AddToListFromGAC("System.Web.Helpers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
					AddToListFromGAC("System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
					AddToListFromGAC("Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
					break;
				case object _ when path != null:
					foreach (var file in Directory.GetFiles(path, "*.dll")) {
						var dllname = Path.GetFileName(file);
						if (DoIncludeFile(dllname))
							AddToListFromDirectory(file);
					}
					break;
			}
			return list;

			void AddToListFromGAC(string fullName)
			{
				AssemblyNameReference reference = AssemblyNameReference.Parse(fullName);
				string file = GacInterop.FindAssemblyInNetGac(reference);
				if (file != null)
					list.OpenAssembly(file);
			}

			void AddToListFromDirectory(string file)
			{
				if (File.Exists(file))
					list.OpenAssembly(file);
			}

			bool DoIncludeFile(string fileName)
			{
				if (fileName == "Microsoft.DiaSymReader.Native.amd64.dll")
					return false;
				if (fileName.EndsWith("_cor3.dll", StringComparison.OrdinalIgnoreCase))
					return false;
				if (char.IsUpper(fileName[0]))
					return true;
				if (fileName == "netstandard.dll")
					return true;
				if (fileName == "mscorlib.dll")
					return true;
				return false;
			}
		}

		private void CreateDefaultAssemblyLists()
		{
			if (!manager.AssemblyLists.Contains(DotNet4List)) {
				AssemblyList dotnet4 = CreateDefaultList(DotNet4List);
				if (dotnet4.assemblies.Count > 0) {
					manager.CreateList(dotnet4);
				}
			}

			if (!manager.AssemblyLists.Contains(DotNet35List)) {
				AssemblyList dotnet35 = CreateDefaultList(DotNet35List);
				if (dotnet35.assemblies.Count > 0) {
					manager.CreateList(dotnet35);
				}
			}

			if (!manager.AssemblyLists.Contains(ASPDotNetMVC3List)) {
				AssemblyList mvc = CreateDefaultList(ASPDotNetMVC3List);
				if (mvc.assemblies.Count > 0) {
					manager.CreateList(mvc);
				}
			}
		}

		private void ExecuteCreatePreconfiguredAssemblyList(PreconfiguredAssemblyList config)
		{
			CreateListDialog dlg = new CreateListDialog(Resources.AddPreconfiguredList);
			dlg.Owner = parent;
			dlg.Closing += (s, args) => {
				if (dlg.DialogResult == true) {
					if (manager.AssemblyLists.Contains(dlg.ListName)) {
						args.Cancel = true;
						MessageBox.Show(Properties.Resources.ListExistsAlready, null, MessageBoxButton.OK);
					}
				}
			};
			if (dlg.ShowDialog() == true) {
				var list = CreateDefaultList(config.Name, config.Path, dlg.ListName);
				if (list.assemblies.Count > 0) {
					manager.CreateList(list);
				}
			}
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
