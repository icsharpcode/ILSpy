// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Manages the available assembly lists.
	/// 
	/// Contains the list of list names; and provides methods for loading/saving and creating/deleting lists.
	/// </summary>
	sealed class AssemblyListManager
	{
		ILSpySettings spySettings;

		public AssemblyListManager(ILSpySettings spySettings)
		{
			this.spySettings = spySettings;
			XElement doc = spySettings["AssemblyLists"];
			foreach (var list in doc.Elements("List")) {
				AssemblyLists.Add((string)list.Attribute("name"));
			}
		}
		
		public ObservableCollection<string> AssemblyLists { get; } = new ObservableCollection<string>();
		
		/// <summary>
		/// Loads an assembly list from the ILSpySettings.
		/// If no list with the specified name is found, the default list is loaded instead.
		/// </summary>
		public AssemblyList LoadList(ILSpySettings settings, string listName)
		{
			this.spySettings = settings;
			AssemblyList list = DoLoadList(spySettings, listName);
			if (!AssemblyLists.Contains(list.ListName))
				AssemblyLists.Add(list.ListName);
			return list;
		}
		
		AssemblyList DoLoadList(ILSpySettings spySettings, string listName)
		{
			XElement doc = spySettings["AssemblyLists"];
			if (listName != null) {
				foreach (var list in doc.Elements("List")) {
					if ((string)list.Attribute("name") == listName) {
						return new AssemblyList(list);
					}
				}
			}
			return new AssemblyList(listName ?? DefaultListName);
		}

		public bool CloneList(string selectedAssemblyList, string newListName)
		{
			var list = DoLoadList(spySettings, selectedAssemblyList);
			var newList = new AssemblyList(list, newListName);
			return CreateList(newList);
		}

		public bool RenameList(string selectedAssemblyList, string newListName)
		{
			var list = DoLoadList(spySettings, selectedAssemblyList);
			var newList = new AssemblyList(list, newListName);
			return DeleteList(selectedAssemblyList) && CreateList(newList);
		}

		public const string DefaultListName = "(Default)";
		
		/// <summary>
		/// Saves the specifies assembly list into the config file.
		/// </summary>
		public static void SaveList(AssemblyList list)
		{
			ILSpySettings.Update(
				delegate (XElement root) {
					XElement doc = root.Element("AssemblyLists");
					if (doc == null) {
						doc = new XElement("AssemblyLists");
						root.Add(doc);
					}
					XElement listElement = doc.Elements("List").FirstOrDefault(e => (string)e.Attribute("name") == list.ListName);
					if (listElement != null)
						listElement.ReplaceWith(list.SaveAsXml());
					else
						doc.Add(list.SaveAsXml());
				});
		}

		public bool CreateList(AssemblyList list)
		{
			if (!AssemblyLists.Contains(list.ListName))
			{
				AssemblyLists.Add(list.ListName);
				SaveList(list);
				return true;
			}
			return false;
		}

		public bool DeleteList(string Name)
		{
			if (AssemblyLists.Contains(Name))
			{
				AssemblyLists.Remove(Name);

				ILSpySettings.Update(
					delegate(XElement root)
					{
						XElement doc = root.Element("AssemblyLists");
						if (doc == null)
						{
							return;
						}
						XElement listElement = doc.Elements("List").FirstOrDefault(e => (string)e.Attribute("name") == Name);
						if (listElement != null)
							listElement.Remove();
					});
				return true;
			}
			return false;
		}

		public void ClearAll()
		{
			AssemblyLists.Clear();
			ILSpySettings.Update(
				delegate (XElement root) {
					XElement doc = root.Element("AssemblyLists");
					if (doc == null) {
						return;
					}
					doc.Remove();
				});
		}

		public void CreateDefaultAssemblyLists()
		{
			if (AssemblyLists.Count > 0)
				return;

			if (!AssemblyLists.Contains(ManageAssemblyListsViewModel.DotNet4List)) {
				AssemblyList dotnet4 = ManageAssemblyListsViewModel.CreateDefaultList(ManageAssemblyListsViewModel.DotNet4List);
				if (dotnet4.assemblies.Count > 0) {
					CreateList(dotnet4);
				}
			}

			if (!AssemblyLists.Contains(ManageAssemblyListsViewModel.DotNet35List)) {
				AssemblyList dotnet35 = ManageAssemblyListsViewModel.CreateDefaultList(ManageAssemblyListsViewModel.DotNet35List);
				if (dotnet35.assemblies.Count > 0) {
					CreateList(dotnet35);
				}
			}

			if (!AssemblyLists.Contains(ManageAssemblyListsViewModel.ASPDotNetMVC3List)) {
				AssemblyList mvc = ManageAssemblyListsViewModel.CreateDefaultList(ManageAssemblyListsViewModel.ASPDotNetMVC3List);
				if (mvc.assemblies.Count > 0) {
					CreateList(mvc);
				}
			}
		}
	}
}
