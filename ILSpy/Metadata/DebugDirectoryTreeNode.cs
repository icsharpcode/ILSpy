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
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Windows.Controls;
using System.Windows.Data;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	class DebugDirectoryTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public DebugDirectoryTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => "Debug Directory";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var dataGrid = Helpers.PrepareDataGrid(tabPage, this);
			var entries = new List<DebugDirectoryEntryView>();
			foreach (var entry in module.Reader.ReadDebugDirectory())
			{
				entries.Add(new DebugDirectoryEntryView(entry));
			}

			dataGrid.ItemsSource = entries.ToArray();

			tabPage.Content = dataGrid;
			return true;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Data Directories");
		}

		class DebugDirectoryEntryView
		{
			public int Characteristics { get; set; }
			public uint Timestamp { get; set; }
			public ushort MajorVersion { get; set; }
			public ushort MinorVersion { get; set; }
			public string Type { get; set; }
			public int SizeOfRawData { get; set; }
			public int AddressOfRawData { get; set; }
			public int PointerToRawData { get; set; }

			public DebugDirectoryEntryView(DebugDirectoryEntry entry)
			{
				this.Characteristics = 0;
				this.Timestamp = entry.Stamp;
				this.MajorVersion = entry.MajorVersion;
				this.MinorVersion = entry.MinorVersion;
				this.Type = entry.Type.ToString();
				this.SizeOfRawData = entry.DataSize;
				this.AddressOfRawData = entry.DataRelativeVirtualAddress;
				this.AddressOfRawData = entry.DataPointer;
			}
		}
	}
}
