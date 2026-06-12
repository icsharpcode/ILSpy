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

using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Renders the 64-byte legacy DOS header sitting at offset 0 of every PE file. Phase 1
	/// emits a fixed-width text table; Phase 2 swaps to a DataGrid tab.
	/// </summary>
	public sealed class DosHeaderTreeNode : ILSpyTreeNode
	{
		readonly PEFile module;

		public DosHeaderTreeNode(PEFile module)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
		}

		public override object Text => "DOS Header";
		public override object Icon => Images.Header;
		public override string ToString() => "DOS Header";

		public override ContentPageModel CreateTab()
		{
			var page = new MetadataTablePageModel {
				Title = "DOS Header",
				Items = BuildEntries(),
			};
			MetadataColumnBuilder.Populate<Entry>(page);
			return page;
		}

		IReadOnlyList<Entry> BuildEntries()
		{
			var reader = module.Reader.GetEntireImage().GetReader(0, 64);
			var entries = new List<Entry>(31) {
				new(reader.Offset, reader.ReadUInt16(), 2, "e_magic", "Magic Number (MZ)"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_cblp", "Bytes on last page of file"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_cp", "Pages in file"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_crlc", "Relocations"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_cparhdr", "Size of header in paragraphs"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_minalloc", "Minimum extra paragraphs needed"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_maxalloc", "Maximum extra paragraphs needed"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_ss", "Initial (relative) SS value"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_sp", "Initial SP value"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_csum", "Checksum"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_ip", "Initial IP value"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_cs", "Initial (relative) CS value"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_lfarlc", "File address of relocation table"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_ovno", "Overlay number"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res[0]", "Reserved words"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res[1]", "Reserved words"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res[2]", "Reserved words"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res[3]", "Reserved words"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_oemid", "OEM identifier (for e_oeminfo)"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_oeminfo", "OEM information; e_oemid specific"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res2[0]", "Reserved words"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res2[1]", "Reserved words"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res2[2]", "Reserved words"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res2[3]", "Reserved words"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res2[4]", "Reserved words"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res2[5]", "Reserved words"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res2[6]", "Reserved words"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res2[7]", "Reserved words"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res2[8]", "Reserved words"),
				new(reader.Offset, reader.ReadUInt16(), 2, "e_res2[9]", "Reserved words"),
				new(reader.Offset, reader.ReadInt32(), 4, "e_lfanew", "File address of new exe header"),
			};
			return entries;
		}
	}
}
