﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	class DataDirectoriesTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public DataDirectoriesTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => "Data Directories";

		public override object Icon => Images.ListFolder;
		public override object ExpandedIcon => Images.ListFolderOpen;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var dataGrid = Helpers.PrepareDataGrid(tabPage, this);
			//dataGrid.AutoGenerateColumns = false;
			//dataGrid.Columns.Add(new DataGridTextColumn { IsReadOnly = true, Header = "Name", Binding = new Binding("Name") });
			//dataGrid.Columns.Add(new DataGridTextColumn { IsReadOnly = true, Header = "RVA", Binding = new Binding("RVA") { StringFormat = "X8" } });
			//dataGrid.Columns.Add(new DataGridTextColumn { IsReadOnly = true, Header = "Size", Binding = new Binding("Size") { StringFormat = "X8" } });
			//dataGrid.Columns.Add(new DataGridTextColumn { IsReadOnly = true, Header = "Section", Binding = new Binding("Section") });
			var headers = module.Reader.PEHeaders;
			var header = headers.PEHeader;

			var entries = new DataDirectoryEntry[] {
				new DataDirectoryEntry(headers, "Export Table", header.ExportTableDirectory),
				new DataDirectoryEntry(headers, "Import Table", header.ImportTableDirectory),
				new DataDirectoryEntry(headers, "Resource Table", header.ResourceTableDirectory),
				new DataDirectoryEntry(headers, "Exception Table", header.ExceptionTableDirectory),
				new DataDirectoryEntry(headers, "Certificate Table", header.CertificateTableDirectory),
				new DataDirectoryEntry(headers, "Base Relocation Table", header.BaseRelocationTableDirectory),
				new DataDirectoryEntry(headers, "Debug Table", header.DebugTableDirectory),
				new DataDirectoryEntry(headers, "Copyright Table", header.CopyrightTableDirectory),
				new DataDirectoryEntry(headers, "Global Pointer Table", header.GlobalPointerTableDirectory),
				new DataDirectoryEntry(headers, "Thread Local Storage Table", header.ThreadLocalStorageTableDirectory),
				new DataDirectoryEntry(headers, "Load Config", header.LoadConfigTableDirectory),
				new DataDirectoryEntry(headers, "Bound Import", header.BoundImportTableDirectory),
				new DataDirectoryEntry(headers, "Import Address Table", header.ImportAddressTableDirectory),
				new DataDirectoryEntry(headers, "Delay Import Descriptor", header.DelayImportTableDirectory),
				new DataDirectoryEntry(headers, "CLI Header", header.CorHeaderTableDirectory),
			};

			dataGrid.ItemsSource = entries;

			tabPage.Content = dataGrid;
			return true;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
		}

		class DataDirectoryEntry
		{
			public string Name { get; set; }
			public int RVA { get; set; }
			public int Size { get; set; }
			public string Section { get; set; }

			public DataDirectoryEntry(string name, int rva, int size, string section)
			{
				this.Name = name;
				this.RVA = rva;
				this.Size = size;
				this.Section = section;
			}

			public DataDirectoryEntry(PEHeaders headers, string name, DirectoryEntry entry)
				: this(name, entry.RelativeVirtualAddress, entry.Size, (headers.GetContainingSectionIndex(entry.RelativeVirtualAddress) >= 0) ? headers.SectionHeaders[headers.GetContainingSectionIndex(entry.RelativeVirtualAddress)].Name : "")
			{
			}
		}
	}
}
