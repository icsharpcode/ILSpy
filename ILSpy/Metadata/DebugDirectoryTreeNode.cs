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
	/// Lists each entry of the PE debug directory: timestamp, version, type, and the size /
	/// addresses of the raw debug payload. Click the node to see the entries as a grid;
	/// expand it to drill into per-entry sub-trees — currently embedded portable PDBs are
	/// surfaced as a nested <see cref="MetadataTreeNode"/> so the user can browse the
	/// debug-only metadata tables (Document, MethodDebugInformation, …) the same way they
	/// browse the host module's tables.
	/// </summary>
	public sealed class DebugDirectoryTreeNode : ILSpyTreeNode
	{
		readonly PEFile module;

		public DebugDirectoryTreeNode(PEFile module)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			LazyLoading = true;
		}

		public override object Text => "Debug Directory";
		public override object Icon => Images.ListFolder;
		public override object ExpandedIcon => Images.ListFolderOpen;
		public override string ToString() => "Debug Directory";

		public override ContentPageModel CreateTab()
		{
			var page = new MetadataTablePageModel {
				Title = "Debug Directory",
				Items = BuildEntries(),
			};
			MetadataColumnBuilder.Populate<DebugDirectoryEntryView>(page);
			return page;
		}

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

		protected override void LoadChildren()
		{
			foreach (var entry in module.Reader.ReadDebugDirectory())
			{
				switch (entry.Type)
				{
					case DebugDirectoryEntryType.CodeView:
						try
						{
							var cv = module.Reader.ReadCodeViewDebugDirectoryData(entry);
							Children.Add(new CodeViewTreeNode(module, entry, cv));
						}
						catch (BadImageFormatException)
						{
							Children.Add(new DebugDirectoryEntryTreeNode(module, entry));
						}
						break;
					case DebugDirectoryEntryType.PdbChecksum:
						try
						{
							var sum = module.Reader.ReadPdbChecksumDebugDirectoryData(entry);
							Children.Add(new PdbChecksumTreeNode(module, entry, sum));
						}
						catch (BadImageFormatException)
						{
							Children.Add(new DebugDirectoryEntryTreeNode(module, entry));
						}
						break;
					case DebugDirectoryEntryType.EmbeddedPortablePdb:
						try
						{
							var provider = module.Reader.ReadEmbeddedPortablePdbDebugDirectoryData(entry);
							var pdbMetadata = new MetadataFile(
								MetadataFile.MetadataFileKind.ProgramDebugDatabase,
								module.FileName,
								provider,
								isEmbedded: true);
							Children.Add(new MetadataTreeNode(pdbMetadata, "Debug Metadata (Embedded)"));
						}
						catch (BadImageFormatException)
						{
							// A corrupt embedded PDB shouldn't take the whole tree down — skip it
							// silently; the entry is still visible in the grid view above.
						}
						break;
					default:
						// Reproducible, Vendor-specific, Mpx, ExtendedDllCharacteristics, …
						// The generic fallback shows the type + raw hex dump in Decompile.
						Children.Add(new DebugDirectoryEntryTreeNode(module, entry));
						break;
				}
			}
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
