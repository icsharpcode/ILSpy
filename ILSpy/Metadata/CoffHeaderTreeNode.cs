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
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

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

		public override bool View(DecompilerTextView textView)
		{
			var dataGrid = new DataGrid {
				Columns = {
					new DataGridTextColumn { IsReadOnly = true, Header = "Member", Binding = new Binding("Member") },
					new DataGridTextColumn { IsReadOnly = true, Header = "Offset", Binding = new Binding("Offset") { StringFormat = "X8" } },
					new DataGridTextColumn { IsReadOnly = true, Header = "Size", Binding = new Binding("Size") },
					new DataGridTextColumn { IsReadOnly = true, Header = "Value", Binding = new Binding(".") { Converter = ByteWidthConverter.Instance } },
					new DataGridTextColumn { IsReadOnly = true, Header = "Meaning", Binding = new Binding("Meaning") },
				},
				AutoGenerateColumns = false,
				CanUserAddRows = false,
				CanUserDeleteRows = false,
				RowDetailsTemplateSelector = new CharacteristicsDataTemplateSelector(),
				RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible
			};
			var headers = module.Reader.PEHeaders;
			var header = headers.CoffHeader;

			var entries = new List<Entry>();
			entries.Add(new Entry(headers.CoffHeaderStartOffset, (int)header.Machine, 2, "Machine", header.Machine.ToString()));
			entries.Add(new Entry(headers.CoffHeaderStartOffset + 2, (int)header.NumberOfSections, 2, "Number of Sections", "Number of sections; indicates size of the Section Table, which immediately follows the headers."));
			entries.Add(new Entry(headers.CoffHeaderStartOffset + 4, header.TimeDateStamp, 4, "Time/Date Stamp", DateTimeOffset.FromUnixTimeSeconds(unchecked((uint)header.TimeDateStamp)).DateTime + " - Time and date the file was created in seconds since January 1st 1970 00:00:00 or 0."));
			entries.Add(new Entry(headers.CoffHeaderStartOffset + 8, header.PointerToSymbolTable, 4, "Pointer to Symbol Table", "Always 0 in .NET executables."));
			entries.Add(new Entry(headers.CoffHeaderStartOffset + 12, header.NumberOfSymbols, 4, "Number of Symbols", "Always 0 in .NET executables."));
			entries.Add(new Entry(headers.CoffHeaderStartOffset + 16, (int)header.SizeOfOptionalHeader, 2, "Optional Header Size", "Size of the optional header."));
			entries.Add(new Entry(headers.CoffHeaderStartOffset + 18, (int)header.Characteristics, 2, "Characteristics", "Flags indicating attributes of the file."));

			dataGrid.ItemsSource = entries;

			textView.ShowContent(new[] { this }, dataGrid);
			return true;
		}

		private class CharacteristicsDataTemplateSelector : DataTemplateSelector
		{
			public override DataTemplate SelectTemplate(object item, DependencyObject container)
			{
				if (((Entry)item).Member == "Characteristics")
					return MakeDataTemplate((int)((Entry)item).Value);
				return new DataTemplate();
			}

			private DataTemplate MakeDataTemplate(int flags)
			{
				FrameworkElementFactory dataGridFactory = new FrameworkElementFactory(typeof(DataGrid));
				dataGridFactory.SetValue(DataGrid.ItemsSourceProperty, new[] {
					new { Value = (flags & 0x0001) != 0, Meaning = "Relocation info stripped from file" },
					new { Value = (flags & 0x0002) != 0, Meaning = "File is executable" },
					new { Value = (flags & 0x0004) != 0, Meaning = "Line numbers stripped from file" },
					new { Value = (flags & 0x0008) != 0, Meaning = "Local symbols stripped from file" },
					new { Value = (flags & 0x0010) != 0, Meaning = "Aggressively trim working set" },
					new { Value = (flags & 0x0020) != 0, Meaning = "Large address aware" },
					new { Value = (flags & 0x0040) != 0, Meaning = "Reserved" },
					new { Value = (flags & 0x0080) != 0, Meaning = "Bytes of machine words are reversed (Low)" },
					new { Value = (flags & 0x0100) != 0, Meaning = "32-bit word machine" },
					new { Value = (flags & 0x0200) != 0, Meaning = "Debugging info stripped from file in .DBG file" },
					new { Value = (flags & 0x0400) != 0, Meaning = "If image is on removable media, copy and run from the swap file" },
					new { Value = (flags & 0x0800) != 0, Meaning = "If image is on Net, copy and run from the swap file" },
					new { Value = (flags & 0x1000) != 0, Meaning = "System" },
					new { Value = (flags & 0x2000) != 0, Meaning = "DLL" },
					new { Value = (flags & 0x4000) != 0, Meaning = "File should only be run on a UP machine" },
					new { Value = (flags & 0x8000) != 0, Meaning = "Bytes of machine words are reversed (High)" },
				});
				DataTemplate template = new DataTemplate();
				template.VisualTree = dataGridFactory;
				return template;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "COFF Header");
		}
	}
}
