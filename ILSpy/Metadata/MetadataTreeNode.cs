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
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Synthetic container surfaced under each loaded assembly's tree node. Its children
	/// expose the raw CLI metadata: PE headers (DOS / COFF / Optional / DataDirectories /
	/// DebugDirectory), the metadata tables, and the four heaps (String / UserString / Guid
	/// / Blob). Lazy-loaded — children only materialise on first expansion.
	/// </summary>
	public sealed class MetadataTreeNode : ILSpyTreeNode
	{
		readonly MetadataFile metadataFile;
		readonly string title;

		public MetadataTreeNode(MetadataFile metadataFile, string title)
		{
			this.metadataFile = metadataFile ?? throw new ArgumentNullException(nameof(metadataFile));
			this.title = title ?? throw new ArgumentNullException(nameof(title));
			LazyLoading = true;
		}

		public override object Text => title;

		public override object Icon => Images.Metadata;

		// Stable identity for SessionSettings.ActiveTreeViewPath.
		public override string ToString() => "Metadata: " + title;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, title);
			DumpMetadataInfo(language, output, metadataFile.Metadata);
		}

		/// <summary>
		/// Resolves a <see cref="HandleKind"/> to the per-table tree node under this metadata
		/// folder, or <c>null</c> when the handle has no table (e.g. heap handles: String /
		/// UserString / Blob / Guid). Used by <c>MetadataProtocolHandler</c> to drill into
		/// the right table when navigating a <c>metadata://</c> reference. Cast from
		/// <see cref="HandleKind"/> to <see cref="TableIndex"/> relies on the two enums sharing
		/// numeric values for the 0..44 metadata-table range.
		/// </summary>
		public MetadataTableTreeNode? FindNodeByHandleKind(HandleKind kind)
		{
			EnsureLazyChildren();
			var tables = Children.OfType<MetadataTablesTreeNode>().FirstOrDefault();
			if (tables is null)
				return null;
			tables.EnsureLazyChildren();
			return tables.Children
				.OfType<MetadataTableTreeNode>()
				.FirstOrDefault(x => x.Kind == (TableIndex)kind);
		}

		internal static void DumpMetadataInfo(Language language, ITextOutput output, MetadataReader metadata)
		{
			language.WriteCommentLine(output, "MetadataKind: " + metadata.MetadataKind);
			language.WriteCommentLine(output, "MetadataVersion: " + metadata.MetadataVersion);

			if (metadata.DebugMetadataHeader is { } header)
			{
				output.WriteLine();
				language.WriteCommentLine(output, "Header:");
				language.WriteCommentLine(output, "Id: " + ToHexString(header.Id));
				language.WriteCommentLine(output, "EntryPoint: " + MetadataTokens.GetToken(header.EntryPoint).ToString("X8"));
			}

			output.WriteLine();
			language.WriteCommentLine(output, "Tables:");

			foreach (var table in Enum.GetValues<TableIndex>())
			{
				int count = metadata.GetTableRowCount(table);
				if (count > 0)
					language.WriteCommentLine(output, $"{(byte)table:X2} {table}: {count} rows");
			}
		}

		static string ToHexString(System.Collections.Immutable.ImmutableArray<byte> bytes)
		{
			var sb = new System.Text.StringBuilder(bytes.Length * 2);
			foreach (var b in bytes)
				sb.Append(b.ToString("x2"));
			return sb.ToString();
		}

		protected override void LoadChildren()
		{
			if (metadataFile is PEFile peFile)
			{
				Children.Add(new DosHeaderTreeNode(peFile));
				Children.Add(new CoffHeaderTreeNode(peFile));
				Children.Add(new OptionalHeaderTreeNode(peFile));
				Children.Add(new DataDirectoriesTreeNode(peFile));
				Children.Add(new DebugDirectoryTreeNode(peFile));
			}

			Children.Add(new MetadataTablesTreeNode(metadataFile));

			Children.Add(new StringHeapTreeNode(metadataFile));
			Children.Add(new UserStringHeapTreeNode(metadataFile));
			Children.Add(new GuidHeapTreeNode(metadataFile));
			Children.Add(new BlobHeapTreeNode(metadataFile));
		}
	}
}
