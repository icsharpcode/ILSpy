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
using System.IO;
using System.Linq;
using System.Xml.Linq;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpyX
{
	public interface ISettingsProvider
	{
		XElement this[XName section] { get; }

		void Update(Action<XElement> action);
		ISettingsProvider Load();
	}

	/// <summary>
	/// Manages the available assembly lists.
	/// 
	/// Contains the list of list names; and provides methods for loading/saving and creating/deleting lists.
	/// </summary>
	public sealed class AssemblyListManager
	{
		public const string DotNet4List = ".NET 4 (WPF)";
		public const string DotNet35List = ".NET 3.5";
		public const string ASPDotNetMVC3List = "ASP.NET (MVC3)";

		private ISettingsProvider settingsProvider;

		public AssemblyListManager(ISettingsProvider settingsProvider)
		{
			this.settingsProvider = settingsProvider;
			XElement doc = this.settingsProvider["AssemblyLists"];
			foreach (var list in doc.Elements("List"))
			{
				var name = (string?)list.Attribute("name");
				if (name != null)
				{
					AssemblyLists.Add(name);
				}
			}
		}

		public bool ApplyWinRTProjections { get; set; }
		public bool UseDebugSymbols { get; set; }

		public ObservableCollection<string> AssemblyLists { get; } = new ObservableCollection<string>();

		/// <summary>
		/// Loads an assembly list from the ILSpySettings.
		/// If no list with the specified name is found, the default list is loaded instead.
		/// </summary>
		public AssemblyList LoadList(string listName)
		{
			this.settingsProvider = this.settingsProvider.Load();
			AssemblyList list = DoLoadList(listName);
			if (!AssemblyLists.Contains(list.ListName))
				AssemblyLists.Add(list.ListName);
			return list;
		}

		AssemblyList DoLoadList(string listName)
		{
			XElement doc = this.settingsProvider["AssemblyLists"];
			if (listName != null)
			{
				foreach (var list in doc.Elements("List"))
				{
					if ((string?)list.Attribute("name") == listName)
					{
						return new AssemblyList(this, list);
					}
				}
			}
			return new AssemblyList(this, listName ?? DefaultListName);
		}

		public bool CloneList(string selectedAssemblyList, string newListName)
		{
			var list = DoLoadList(selectedAssemblyList);
			var newList = new AssemblyList(list, newListName);
			return AddListIfNotExists(newList);
		}

		public bool RenameList(string selectedAssemblyList, string newListName)
		{
			var list = DoLoadList(selectedAssemblyList);
			var newList = new AssemblyList(list, newListName);
			return DeleteList(selectedAssemblyList) && AddListIfNotExists(newList);
		}

		public const string DefaultListName = "(Default)";

		/// <summary>
		/// Saves the specified assembly list into the config file.
		/// </summary>
		public void SaveList(AssemblyList list)
		{
			this.settingsProvider.Update(
				delegate (XElement root) {
					XElement? doc = root.Element("AssemblyLists");
					if (doc == null)
					{
						doc = new XElement("AssemblyLists");
						root.Add(doc);
					}
					XElement? listElement = doc.Elements("List").FirstOrDefault(e => (string?)e.Attribute("name") == list.ListName);
					if (listElement != null)
						listElement.ReplaceWith(list.SaveAsXml());
					else
						doc.Add(list.SaveAsXml());
				});
		}

		public bool AddListIfNotExists(AssemblyList list)
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
			if (AssemblyLists.Remove(Name))
			{
				this.settingsProvider.Update(
					delegate (XElement root) {
						XElement? doc = root.Element("AssemblyLists");
						if (doc == null)
						{
							return;
						}
						XElement? listElement = doc.Elements("List").FirstOrDefault(e => (string?)e.Attribute("name") == Name);
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
			this.settingsProvider.Update(
				delegate (XElement root) {
					XElement? doc = root.Element("AssemblyLists");
					if (doc == null)
					{
						return;
					}
					doc.Remove();
				});
		}

		public void CreateDefaultAssemblyLists()
		{
			if (AssemblyLists.Count > 0)
				return;

			if (!AssemblyLists.Contains(DotNet4List))
			{
				AssemblyList dotnet4 = CreateDefaultList(DotNet4List);
				if (dotnet4.Count > 0)
				{
					AddListIfNotExists(dotnet4);
				}
			}

			if (!AssemblyLists.Contains(DotNet35List))
			{
				AssemblyList dotnet35 = CreateDefaultList(DotNet35List);
				if (dotnet35.Count > 0)
				{
					AddListIfNotExists(dotnet35);
				}
			}

			if (!AssemblyLists.Contains(ASPDotNetMVC3List))
			{
				AssemblyList mvc = CreateDefaultList(ASPDotNetMVC3List);
				if (mvc.Count > 0)
				{
					AddListIfNotExists(mvc);
				}
			}
		}

		public AssemblyList CreateList(string name)
		{
			return new AssemblyList(this, name);
		}

		public AssemblyList CreateDefaultList(string name, string? path = null, string? newName = null)
		{
			var list = new AssemblyList(this, newName ?? name);
			switch (name)
			{
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
					foreach (var file in Directory.GetFiles(path, "*.dll"))
					{
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
				string? file = UniversalAssemblyResolver.GetAssemblyInGac(reference);
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
	}
}
