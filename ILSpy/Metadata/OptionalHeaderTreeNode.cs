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

using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

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

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var dataGrid = Helpers.PrepareDataGrid(tabPage, this);
			dataGrid.RowDetailsTemplateSelector = new DllCharacteristicsDataTemplateSelector();
			dataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
			dataGrid.AutoGenerateColumns = false;
			dataGrid.Columns.Add(new DataGridCustomTextColumn { Header = "Member", Binding = new Binding("Member") { Mode = BindingMode.OneWay } });
			dataGrid.Columns.Add(new DataGridCustomTextColumn { Header = "Offset", Binding = new Binding("Offset") { StringFormat = "X8", Mode = BindingMode.OneWay } });
			dataGrid.Columns.Add(new DataGridCustomTextColumn { Header = "Size", Binding = new Binding("Size") { Mode = BindingMode.OneWay } });
			dataGrid.Columns.Add(new DataGridCustomTextColumn { Header = "Value", Binding = new Binding(".") { Converter = ByteWidthConverter.Instance, Mode = BindingMode.OneWay } });
			dataGrid.Columns.Add(new DataGridCustomTextColumn { Header = "Meaning", Binding = new Binding("Meaning") { Mode = BindingMode.OneWay } });

			var headers = module.Reader.PEHeaders;
			var reader = module.Reader.GetEntireImage().GetReader(headers.PEHeaderStartOffset, 128);
			var header = headers.PEHeader;

			var entries = new List<Entry>();
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadUInt16(), 2, "Magic", header.Magic.ToString()));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadByte(), 1, "Major Linker Version", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadByte(), 1, "Minor Linker Version", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Code Size", "Size of the code (text) section, or the sum of all code sections if there are multiple sections."));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Initialized Data Size", "Size of the initialized data section, or the sum of all initialized data sections if there are multiple data sections."));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Uninitialized Data Size", "Size of the uninitialized data section, or the sum of all uninitialized data sections if there are multiple uninitialized data sections."));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Entry Point RVA", "RVA of entry point, needs to point to bytes 0xFF 0x25 followed by the RVA in a section marked execute / read for EXEs or 0 for DLLs"));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Base Of Code", "RVA of the code section."));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, header.Magic == PEMagic.PE32Plus ? reader.ReadUInt64() : reader.ReadUInt32(), header.Magic == PEMagic.PE32Plus ? 8 : 4, "Base Of Data", "RVA of the data section."));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, header.Magic == PEMagic.PE32Plus ? reader.ReadUInt64() : reader.ReadUInt32(), header.Magic == PEMagic.PE32Plus ? 8 : 4, "Image Base", "Shall be a multiple of 0x10000."));
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
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadUInt16(), 2, "DLL Characteristics", header.DllCharacteristics.ToString()));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, header.Magic == PEMagic.PE32Plus ? reader.ReadUInt64() : reader.ReadUInt32(), header.Magic == PEMagic.PE32Plus ? 8 : 4, "Stack Reserve Size", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, header.Magic == PEMagic.PE32Plus ? reader.ReadUInt64() : reader.ReadUInt32(), header.Magic == PEMagic.PE32Plus ? 8 : 4, "Stack Commit Size", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, header.Magic == PEMagic.PE32Plus ? reader.ReadUInt64() : reader.ReadUInt32(), header.Magic == PEMagic.PE32Plus ? 8 : 4, "Heap Reserve Size", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, header.Magic == PEMagic.PE32Plus ? reader.ReadUInt64() : reader.ReadUInt32(), header.Magic == PEMagic.PE32Plus ? 8 : 4, "Heap Commit Size", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadUInt32(), 4, "Loader Flags", ""));
			entries.Add(new Entry(headers.PEHeaderStartOffset + reader.Offset, reader.ReadInt32(), 4, "Number of Data Directories", ""));

			dataGrid.ItemsSource = entries;

			tabPage.Content = dataGrid;
			return true;
		}

		private class DllCharacteristicsDataTemplateSelector : DataTemplateSelector
		{
			public override DataTemplate SelectTemplate(object item, DependencyObject container)
			{
				if (((Entry)item).Member == "DLL Characteristics")
					return MakeDataTemplate((ushort)((Entry)item).Value);
				return new DataTemplate();
			}

			private DataTemplate MakeDataTemplate(ushort flags)
			{
				FrameworkElementFactory dataGridFactory = new FrameworkElementFactory(typeof(DataGrid));
				dataGridFactory.SetValue(DataGrid.ItemsSourceProperty, new[] {
					new { Value = (flags & 0x0001) != 0, Meaning = "<0001> Process Init (Reserved)" },
					new { Value = (flags & 0x0002) != 0, Meaning = "<0002> Process Term (Reserved)" },
					new { Value = (flags & 0x0004) != 0, Meaning = "<0004> Thread Init (Reserved)" },
					new { Value = (flags & 0x0008) != 0, Meaning = "<0008> Thread Term (Reserved)" },
					new { Value = (flags & 0x0010) != 0, Meaning = "<0010> Unused" },
					new { Value = (flags & 0x0020) != 0, Meaning = "<0020> Image can handle a high entropy 64-bit virtual address space (ASLR)" },
					new { Value = (flags & 0x0040) != 0, Meaning = "<0040> DLL can be relocated at load time" },
					new { Value = (flags & 0x0080) != 0, Meaning = "<0080> Code integrity checks are enforced" },
					new { Value = (flags & 0x0100) != 0, Meaning = "<0100> Image is NX compatible" },
					new { Value = (flags & 0x0200) != 0, Meaning = "<0200> Isolation aware, but do not isolate the image" },
					new { Value = (flags & 0x0400) != 0, Meaning = "<0400> Does not use structured exception handling (SEH)" },
					new { Value = (flags & 0x0800) != 0, Meaning = "<0800> Do not bind the image" },
					new { Value = (flags & 0x1000) != 0, Meaning = "<1000> Image must execute in an AppContainer" },
					new { Value = (flags & 0x2000) != 0, Meaning = "<2000> Driver is a WDM Driver" },
					new { Value = (flags & 0x4000) != 0, Meaning = "<4000> Image supports Control Flow Guard" },
					new { Value = (flags & 0x8000) != 0, Meaning = "<8000> Image is Terminal Server aware" },
				});
				dataGridFactory.SetValue(DataGrid.GridLinesVisibilityProperty, DataGridGridLinesVisibility.None);
				DataTemplate template = new DataTemplate();
				template.VisualTree = dataGridFactory;
				return template;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Optional Header");
		}
	}
}
