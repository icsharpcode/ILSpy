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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	class CoffHeaderTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public CoffHeaderTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => "COFF Header";

		public override object Icon => Images.Literal;

		public override bool View(TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var dataGrid = Helpers.PrepareDataGrid(tabPage, this);

			dataGrid.RowDetailsTemplateSelector = new CharacteristicsDataTemplateSelector("Characteristics");
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
			var header = headers.CoffHeader;

			var entries = new List<Entry>();
			entries.Add(new Entry(headers.CoffHeaderStartOffset, (int)header.Machine, 2, "Machine", header.Machine.ToString()));
			entries.Add(new Entry(headers.CoffHeaderStartOffset + 2, (int)header.NumberOfSections, 2, "Number of Sections", "Number of sections; indicates size of the Section Table, which immediately follows the headers."));
			entries.Add(new Entry(headers.CoffHeaderStartOffset + 4, header.TimeDateStamp, 4, "Time/Date Stamp", DateTimeOffset.FromUnixTimeSeconds(unchecked((uint)header.TimeDateStamp)).DateTime + " - Time and date the file was created in seconds since January 1st 1970 00:00:00 or 0."));
			entries.Add(new Entry(headers.CoffHeaderStartOffset + 8, header.PointerToSymbolTable, 4, "Pointer to Symbol Table", "Always 0 in .NET executables."));
			entries.Add(new Entry(headers.CoffHeaderStartOffset + 12, header.NumberOfSymbols, 4, "Number of Symbols", "Always 0 in .NET executables."));
			entries.Add(new Entry(headers.CoffHeaderStartOffset + 16, (int)header.SizeOfOptionalHeader, 2, "Optional Header Size", "Size of the optional header."));
			Entry characteristics;
			entries.Add(characteristics = new Entry(headers.CoffHeaderStartOffset + 18, (int)header.Characteristics, 2, "Characteristics", "Flags indicating attributes of the file.", new[] {
				new BitEntry(((int)header.Characteristics & 0x0001) != 0, "<0001> Relocation info stripped from file"),
				new BitEntry(((int)header.Characteristics & 0x0002) != 0, "<0002> File is executable"),
				new BitEntry(((int)header.Characteristics & 0x0004) != 0, "<0004> Line numbers stripped from file"),
				new BitEntry(((int)header.Characteristics & 0x0008) != 0, "<0008> Local symbols stripped from file"),
				new BitEntry(((int)header.Characteristics & 0x0010) != 0, "<0010> Aggressively trim working set"),
				new BitEntry(((int)header.Characteristics & 0x0020) != 0, "<0020> Large address aware"),
				new BitEntry(((int)header.Characteristics & 0x0040) != 0, "<0040> Reserved"),
				new BitEntry(((int)header.Characteristics & 0x0080) != 0, "<0080> Bytes of machine words are reversed (Low)"),
				new BitEntry(((int)header.Characteristics & 0x0100) != 0, "<0100> 32-bit word machine"),
				new BitEntry(((int)header.Characteristics & 0x0200) != 0, "<0200> Debugging info stripped from file in .DBG file"),
				new BitEntry(((int)header.Characteristics & 0x0400) != 0, "<0400> If image is on removable media, copy and run from the swap file"),
				new BitEntry(((int)header.Characteristics & 0x0800) != 0, "<0800> If image is on Net, copy and run from the swap file"),
				new BitEntry(((int)header.Characteristics & 0x1000) != 0, "<1000> System"),
				new BitEntry(((int)header.Characteristics & 0x2000) != 0, "<2000> DLL"),
				new BitEntry(((int)header.Characteristics & 0x4000) != 0, "<4000> File should only be run on a UP machine"),
				new BitEntry(((int)header.Characteristics & 0x8000) != 0, "<8000> Bytes of machine words are reversed (High)"),
			}));

			dataGrid.ItemsSource = entries;
			dataGrid.SetDetailsVisibilityForItem(characteristics, Visibility.Visible);
			tabPage.Content = dataGrid;
			return true;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "COFF Header");
		}
	}

	public class CharacteristicsDataTemplateSelector : DataTemplateSelector
	{
		string detailsFieldName;

		public CharacteristicsDataTemplateSelector(string detailsFieldName)
		{
			this.detailsFieldName = detailsFieldName;
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (((Entry)item).Member == detailsFieldName)
				return (DataTemplate)MetadataTableViews.Instance["HeaderFlagsDetailsDataGrid"];
			return null;
		}
	}
}
