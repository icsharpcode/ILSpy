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

using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX.Extensions;

using TomsToolbox.Essentials;

namespace ICSharpCode.ILSpy.Metadata
{
	class OptionalHeaderTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public OptionalHeaderTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => "Optional Header";

		public override object Icon => Images.Header;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var dataGrid = Helpers.PrepareDataGrid(tabPage, this);

			dataGrid.RowDetailsTemplateSelector = new CharacteristicsDataTemplateSelector("DLL Characteristics");
			dataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;

			dataGrid.Columns.Clear();
			dataGrid.AutoGenerateColumns = false;
			dataGrid.Columns.AddRange(
				new[] {
					new DataGridTextColumn { IsReadOnly = true, Header = "Member", Binding = new Binding("Member") },
					new DataGridTextColumn { IsReadOnly = true, Header = "Offset", Binding = new Binding("Offset") { StringFormat = "X8" } },
					new DataGridTextColumn { IsReadOnly = true, Header = "Size", Binding = new Binding("Size") },
					new DataGridTextColumn { IsReadOnly = true, Header = "Value", Binding = new Binding(".") { Converter = ByteWidthConverter.Instance } },
					new DataGridTextColumn { IsReadOnly = true, Header = "Meaning", Binding = new Binding("Meaning") }
				}
			);

			var headers = module.Reader.PEHeaders;
			var reader = module.Reader.GetEntireImage().GetReader(headers.PEHeaderStartOffset, 128);
			var header = headers.PEHeader;
			var isPE32Plus = (header.Magic == PEMagic.PE32Plus);

			var entries = new List<Entry>();
			ushort dllCharacteristics;
			Entry characteristics;
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadUInt16(), 2, "Magic", header.Magic.ToString()));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadByte(), 1, "Major Linker Version", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadByte(), 1, "Minor Linker Version", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Code Size", "Size of the code (text) section, or the sum of all code sections if there are multiple sections."));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Initialized Data Size", "Size of the initialized data section, or the sum of all initialized data sections if there are multiple data sections."));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Uninitialized Data Size", "Size of the uninitialized data section, or the sum of all uninitialized data sections if there are multiple uninitialized data sections."));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Entry Point RVA", "RVA of entry point, needs to point to bytes 0xFF 0x25 followed by the RVA in a section marked execute / read for EXEs or 0 for DLLs"));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Base Of Code", "RVA of the code section."));
			entries.Add(new Entry(isPE32Plus ? 0 : headers.PEHeaderStartOffset + reader.Offset, isPE32Plus ? 0UL : reader.ReadUInt32(), isPE32Plus ? 0 : 4, "Base Of Data", "PE32 only (not present in PE32Plus): RVA of the data section, relative to the Image Base."));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, isPE32Plus ? reader.ReadUInt64() : reader.ReadUInt32(), isPE32Plus ? 8 : 4, "Image Base", "Shall be a multiple of 0x10000."));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Section Alignment", "Shall be greater than File Alignment."));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "File Alignment", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadUInt16(), 2, "Major OS Version", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadUInt16(), 2, "Minor OS Version", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadUInt16(), 2, "Major Image Version", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadUInt16(), 2, "Minor Image Version", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadUInt16(), 2, "Major Subsystem Version", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadUInt16(), 2, "Minor Subsystem Version", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadUInt32(), 4, "Win32VersionValue", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Image Size", "Size, in bytes, of image, including all headers and padding; shall be a multiple of Section Alignment."));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Header Size", "Combined size of MS-DOS Header, PE Header, PE Optional Header and padding; shall be a multiple of the file alignment."));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "File Checksum", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadUInt16(), 2, "Subsystem", header.Subsystem.ToString()));
			entries.Add(characteristics = new Entry(headers.PEHeaderStartOffset + reader.Offset, dllCharacteristics = reader.ReadUInt16(), 2, "DLL Characteristics", header.DllCharacteristics.ToString(), new[] {
					new BitEntry((dllCharacteristics & 0x0001) != 0, "<0001> Process Init (Reserved)"),
					new BitEntry((dllCharacteristics & 0x0002) != 0, "<0002> Process Term (Reserved)"),
					new BitEntry((dllCharacteristics & 0x0004) != 0, "<0004> Thread Init (Reserved)"),
					new BitEntry((dllCharacteristics & 0x0008) != 0, "<0008> Thread Term (Reserved)"),
					new BitEntry((dllCharacteristics & 0x0010) != 0, "<0010> Unused"),
					new BitEntry((dllCharacteristics & 0x0020) != 0, "<0020> Image can handle a high entropy 64-bit virtual address space (ASLR)"),
					new BitEntry((dllCharacteristics & 0x0040) != 0, "<0040> DLL can be relocated at load time"),
					new BitEntry((dllCharacteristics & 0x0080) != 0, "<0080> Code integrity checks are enforced"),
					new BitEntry((dllCharacteristics & 0x0100) != 0, "<0100> Image is NX compatible"),
					new BitEntry((dllCharacteristics & 0x0200) != 0, "<0200> Isolation aware, but do not isolate the image"),
					new BitEntry((dllCharacteristics & 0x0400) != 0, "<0400> Does not use structured exception handling (SEH)"),
					new BitEntry((dllCharacteristics & 0x0800) != 0, "<0800> Do not bind the image"),
					new BitEntry((dllCharacteristics & 0x1000) != 0, "<1000> Image must execute in an AppContainer"),
					new BitEntry((dllCharacteristics & 0x2000) != 0, "<2000> Driver is a WDM Driver"),
					new BitEntry((dllCharacteristics & 0x4000) != 0, "<4000> Image supports Control Flow Guard"),
					new BitEntry((dllCharacteristics & 0x8000) != 0, "<8000> Image is Terminal Server aware"),
				}));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, isPE32Plus ? reader.ReadUInt64() : reader.ReadUInt32(), isPE32Plus ? 8 : 4, "Stack Reserve Size", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, isPE32Plus ? reader.ReadUInt64() : reader.ReadUInt32(), isPE32Plus ? 8 : 4, "Stack Commit Size", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, isPE32Plus ? reader.ReadUInt64() : reader.ReadUInt32(), isPE32Plus ? 8 : 4, "Heap Reserve Size", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, isPE32Plus ? reader.ReadUInt64() : reader.ReadUInt32(), isPE32Plus ? 8 : 4, "Heap Commit Size", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadUInt32(), 4, "Loader Flags", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Number of Data Directories", ""));

			dataGrid.ItemsSource = entries;
			dataGrid.SetDetailsVisibilityForItem(characteristics, Visibility.Visible);

			tabPage.Content = dataGrid;
			return true;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Optional Header");
		}
	}
}
