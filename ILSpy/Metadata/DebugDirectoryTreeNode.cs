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

#nullable enable

using System.Collections.Generic;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	class DebugDirectoryTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public DebugDirectoryTreeNode(PEFile module)
		{
			this.module = module;
			this.LazyLoading = true;
		}

		public override object Text => "Debug Directory";

		public override object Icon => Images.ListFolder;
		public override object ExpandedIcon => Images.ListFolderOpen;


		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var dataGrid = Helpers.PrepareDataGrid(tabPage, this);
			var entries = new List<DebugDirectoryEntryView>();
			foreach (var entry in module.Reader.ReadDebugDirectory())
			{
				int dataOffset = module.Reader.IsLoadedImage ? entry.DataRelativeVirtualAddress : entry.DataPointer;
				var data = module.Reader.GetEntireImage().GetContent(dataOffset, entry.DataSize);

				entries.Add(new DebugDirectoryEntryView(entry, data.ToHexString(data.Length)));
			}

			dataGrid.ItemsSource = entries.ToArray();

			tabPage.Content = dataGrid;
			return true;
		}

		protected override void LoadChildren()
		{
			foreach (var entry in module.Reader.ReadDebugDirectory())
			{
				switch (entry.Type)
				{
					case DebugDirectoryEntryType.CodeView:
						var codeViewData = module.Reader.ReadCodeViewDebugDirectoryData(entry);
						this.Children.Add(new CodeViewTreeNode(codeViewData));
						break;

					case DebugDirectoryEntryType.EmbeddedPortablePdb:
						var embeddedPortablePdbProvider = module.Reader.ReadEmbeddedPortablePdbDebugDirectoryData(entry);
						var embeddedPortablePdbMetadataFile = new MetadataFile(MetadataFile.MetadataFileKind.ProgramDebugDatabase, module.FileName, embeddedPortablePdbProvider, isEmbedded: true);
						this.Children.Add(new MetadataTreeNode(embeddedPortablePdbMetadataFile, "Debug Metadata (Embedded)"));
						break;

					case DebugDirectoryEntryType.PdbChecksum:
						var pdbChecksumData = module.Reader.ReadPdbChecksumDebugDirectoryData(entry);
						this.Children.Add(new PdbChecksumTreeNode(pdbChecksumData));
						break;

					case DebugDirectoryEntryType.Unknown:
					case DebugDirectoryEntryType.Coff:
					case DebugDirectoryEntryType.Reproducible:
					default:
						this.Children.Add(new DebugDirectoryEntryTreeNode(module, entry));
						break;
				}
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Data Directories");
		}

		class DebugDirectoryEntryView
		{
			public uint Timestamp { get; set; }
			public ushort MajorVersion { get; set; }
			public ushort MinorVersion { get; set; }
			public string Type { get; set; }
			public int SizeOfRawData { get; set; }
			public int AddressOfRawData { get; set; }
			public int PointerToRawData { get; set; }
			public string RawData { get; set; }

			public DebugDirectoryEntryView(DebugDirectoryEntry entry, string data)
			{
				this.RawData = data;
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
