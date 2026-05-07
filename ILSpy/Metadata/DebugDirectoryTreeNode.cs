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

using ILSpy.TreeNodes;
using ILSpy.ViewModels;

namespace ILSpy.Metadata
{
	/// <summary>
	/// Lists each entry of the PE debug directory: timestamp, version, type, and the size /
	/// addresses of the raw debug payload. Phase 1 renders the entries as text; the
	/// per-entry sub-tree (CodeView, embedded portable PDB, PDB checksum) lands with the
	/// rest of the metadata-tables work later in Phase 1.
	/// </summary>
	public sealed class DebugDirectoryTreeNode : ILSpyTreeNode
	{
		readonly PEFile module;

		public DebugDirectoryTreeNode(PEFile module)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
		}

		public override object Text => "Debug Directory";
		public override object Icon => Images.Images.MetadataTable;
		public override string ToString() => "Debug Directory";

		public override TabPageModel CreateTab() => new MetadataTablePageModel {
			Title = "Debug Directory",
			Items = BuildEntries().Cast<object>().ToList(),
			Columns = MetadataColumnBuilder.For<DebugDirectoryEntryView>(),
		};

		IReadOnlyList<DebugDirectoryEntryView> BuildEntries()
		{
			var entries = new List<DebugDirectoryEntryView>();
			foreach (var entry in module.Reader.ReadDebugDirectory())
			{
				int dataOffset = module.Reader.IsLoadedImage ? entry.DataRelativeVirtualAddress : entry.DataPointer;
				var data = module.Reader.GetEntireImage().GetContent(dataOffset, entry.DataSize);
				entries.Add(new DebugDirectoryEntryView(entry, data.ToHexString(data.Length)));
			}
			return entries;
		}

		public sealed class DebugDirectoryEntryView
		{
			public uint Timestamp { get; }
			public ushort MajorVersion { get; }
			public ushort MinorVersion { get; }
			public string Type { get; }
			public int SizeOfRawData { get; }
			[ColumnInfo("X8")]
			public int AddressOfRawData { get; }
			[ColumnInfo("X8")]
			public int PointerToRawData { get; }
			public string RawData { get; }

			public DebugDirectoryEntryView(DebugDirectoryEntry entry, string data)
			{
				Timestamp = entry.Stamp;
				MajorVersion = entry.MajorVersion;
				MinorVersion = entry.MinorVersion;
				Type = entry.Type.ToString();
				SizeOfRawData = entry.DataSize;
				AddressOfRawData = entry.DataRelativeVirtualAddress;
				PointerToRawData = entry.DataPointer;
				RawData = data;
			}
		}
	}
}
