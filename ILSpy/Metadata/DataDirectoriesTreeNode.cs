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
using System.Linq;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// 15-row table of (RVA, Size, Containing-Section) triples that lives at the tail of the
	/// PE Optional Header. Each entry points at one well-known PE region (Export Table,
	/// Import Table, CLI Header, …).
	/// </summary>
	public sealed class DataDirectoriesTreeNode : ILSpyTreeNode
	{
		readonly PEFile module;

		public DataDirectoriesTreeNode(PEFile module)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
		}

		public override object Text => "Data Directories";
		public override object Icon => Images.ListFolder;
		public override object ExpandedIcon => Images.ListFolderOpen;
		public override string ToString() => "Data Directories";

		public override ContentPageModel CreateTab()
		{
			var page = new MetadataTablePageModel {
				Title = "Data Directories",
				Items = BuildEntries(),
			};
			MetadataColumnBuilder.Populate<DataDirectoryEntry>(page);
			return page;
		}

		IReadOnlyList<DataDirectoryEntry> BuildEntries()
		{
			var headers = module.Reader.PEHeaders;
			var header = headers.PEHeader!;
			return new List<DataDirectoryEntry> {
				new(headers, "Export Table", header.ExportTableDirectory),
				new(headers, "Import Table", header.ImportTableDirectory),
				new(headers, "Resource Table", header.ResourceTableDirectory),
				new(headers, "Exception Table", header.ExceptionTableDirectory),
				new(headers, "Certificate Table", header.CertificateTableDirectory),
				new(headers, "Base Relocation Table", header.BaseRelocationTableDirectory),
				new(headers, "Debug Table", header.DebugTableDirectory),
				new(headers, "Copyright Table", header.CopyrightTableDirectory),
				new(headers, "Global Pointer Table", header.GlobalPointerTableDirectory),
				new(headers, "Thread Local Storage Table", header.ThreadLocalStorageTableDirectory),
				new(headers, "Load Config", header.LoadConfigTableDirectory),
				new(headers, "Bound Import", header.BoundImportTableDirectory),
				new(headers, "Import Address Table", header.ImportAddressTableDirectory),
				new(headers, "Delay Import Descriptor", header.DelayImportTableDirectory),
				new(headers, "CLI Header", header.CorHeaderTableDirectory),
			};
		}

		public sealed class DataDirectoryEntry
		{
			public string Name { get; }
			[ColumnInfo("X8")]
			public int RVA { get; }
			[ColumnInfo("X8")]
			public int Size { get; }
			public string Section { get; }

			public DataDirectoryEntry(PEHeaders headers, string name, DirectoryEntry entry)
			{
				Name = name;
				RVA = entry.RelativeVirtualAddress;
				Size = entry.Size;
				int sectionIndex = headers.GetContainingSectionIndex(entry.RelativeVirtualAddress);
				Section = sectionIndex >= 0 ? headers.SectionHeaders[sectionIndex].Name : "";
			}
		}
	}
}
