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
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Data;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	class MetadataTreeNode : ILSpyTreeNode
	{
		private readonly MetadataFile metadataFile;
		private readonly string title;

		public MetadataTreeNode(MetadataFile module, string title)
		{
			this.metadataFile = module;
			this.title = title;
			this.LazyLoading = true;
		}

		public override object Text => title;

		public override object Icon => Images.Metadata;

		public override bool View(TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			return false;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, title);

			DumpMetadataInfo(language, output, this.metadataFile.Metadata);
		}

		internal static void DumpMetadataInfo(Language language, ITextOutput output, MetadataReader metadata)
		{
			language.WriteCommentLine(output, "MetadataKind: " + metadata.MetadataKind);
			language.WriteCommentLine(output, "MetadataVersion: " + metadata.MetadataVersion);

			if (metadata.DebugMetadataHeader is { } header)
			{
				output.WriteLine();
				language.WriteCommentLine(output, "Header:");
				language.WriteCommentLine(output, "Id: " + header.Id.ToHexString(header.Id.Length));
				language.WriteCommentLine(output, "EntryPoint: " + MetadataTokens.GetToken(header.EntryPoint).ToString("X8"));
			}

			output.WriteLine();
			language.WriteCommentLine(output, "Tables:");

			foreach (var table in Enum.GetValues<TableIndex>())
			{
				int count = metadata.GetTableRowCount(table);
				if (count > 0)
				{
					language.WriteCommentLine(output, $"{(byte)table:X2} {table}: {count} rows");
				}
			}
		}

		protected override void LoadChildren()
		{
			if (metadataFile is PEFile module)
			{
				this.Children.Add(new DosHeaderTreeNode(module));
				this.Children.Add(new CoffHeaderTreeNode(module));
				this.Children.Add(new OptionalHeaderTreeNode(module));
				this.Children.Add(new DataDirectoriesTreeNode(module));
				this.Children.Add(new DebugDirectoryTreeNode(module));
			}
			this.Children.Add(new MetadataTablesTreeNode(metadataFile));
			this.Children.Add(new StringHeapTreeNode(metadataFile));
			this.Children.Add(new UserStringHeapTreeNode(metadataFile));
			this.Children.Add(new GuidHeapTreeNode(metadataFile));
			this.Children.Add(new BlobHeapTreeNode(metadataFile));
		}

		public MetadataTableTreeNode FindNodeByHandleKind(HandleKind kind)
		{
			return this.Children.OfType<MetadataTablesTreeNode>().Single()
				.Children.OfType<MetadataTableTreeNode>().SingleOrDefault(x => x.Kind == (TableIndex)kind);
		}
	}

	class Entry
	{
		public string Member { get; }
		public int Offset { get; }
		public int Size { get; }
		public object Value { get; }
		public string Meaning { get; }

		public IList<BitEntry> RowDetails { get; }

		public Entry(int offset, object value, int size, string member, string meaning, IList<BitEntry> rowDetails = null)
		{
			this.Member = member;
			this.Offset = offset;
			this.Size = size;
			this.Value = value;
			this.Meaning = meaning;
			this.RowDetails = rowDetails;
		}
	}

	class BitEntry
	{
		public bool Value { get; }
		public string Meaning { get; }

		public BitEntry(bool value, string meaning)
		{
			this.Value = value;
			this.Meaning = meaning;
		}
	}

	class ByteWidthConverter : IValueConverter
	{
		public static readonly ByteWidthConverter Instance = new ByteWidthConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.Format("{0:X" + 2 * ((Entry)value).Size + "}", ((Entry)value).Value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
