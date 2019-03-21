using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	class DosHeaderTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public DosHeaderTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => "DOS Header";

		public override object Icon => Images.Literal;

		public override bool View(DecompilerTextView textView)
		{
			var view = Helpers.CreateListView("EntryView");
			var reader = module.Reader.GetEntireImage().GetReader(0, 64);

			var entries = new List<Entry>();

			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_magic", "Magic Number (MZ)"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_cblp", "Bytes on last page of file"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_cp", "Pages in file"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_crlc", "Relocations"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_cparhdr", "Size of header in paragraphs"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_minalloc", "Minimum extra paragraphs needed"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_maxalloc", "Maximum extra paragraphs needed"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_ss", "Initial (relative) SS value"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_sp", "Initial SP value"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_csum", "Checksum"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_ip", "Initial IP value"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_cs", "Initial (relative) CS value"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_lfarlc", "File address of relocation table"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_ovno", "Overlay number"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res[0]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res[1]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res[2]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res[3]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_oemid", "OEM identifier (for e_oeminfo)"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_oeminfo", "OEM information; e_oemid specific"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res2[0]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res2[1]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res2[2]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res2[3]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res2[4]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res2[5]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res2[6]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res2[7]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res2[8]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadUInt16(), 2, "e_res2[9]", "Reserved words"));
			entries.Add(new Entry(reader.Offset, reader.ReadInt32(), 4, "e_lfanew", "File address of new exe header"));

			view.ItemsSource = entries;

			textView.ShowContent(new[] { this }, view);
			return true;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "DOS Header");
		}
	}
}
