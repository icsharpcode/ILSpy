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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class DocumentTableTreeNode : DebugMetadataTableTreeNode
	{
		private readonly bool isEmbedded;

		public DocumentTableTreeNode(PEFile module, MetadataReader metadata, bool isEmbedded)
			: base(HandleKind.Document, module, metadata)
		{
			this.isEmbedded = isEmbedded;
		}

		public override object Text => $"30 Document ({metadata.GetTableRowCount(TableIndex.Document)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var list = new List<DocumentEntry>();
			DocumentEntry scrollTargetEntry = default;

			foreach (var row in metadata.Documents)
			{
				DocumentEntry entry = new DocumentEntry(metadata, isEmbedded, row);
				if (entry.RID == scrollTarget)
				{
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry.RID > 0)
			{
				ScrollItemIntoView(view, scrollTargetEntry);
			}

			return true;
		}

		struct DocumentEntry
		{
			readonly int? offset;
			readonly MetadataReader metadata;
			readonly DocumentHandle handle;
			readonly Document document;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public object Offset => offset == null ? "n/a" : (object)offset;

			public string Name => metadata.GetString(document.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(document.Name):X} \"{Name}\"";

			[StringFormat("X")]
			public int HashAlgorithm => MetadataTokens.GetHeapOffset(document.HashAlgorithm);

			public string HashAlgorithmTooltip {
				get {
					if (document.HashAlgorithm.IsNil)
						return null;
					Guid guid = metadata.GetGuid(document.HashAlgorithm);
					if (guid == KnownGuids.HashAlgorithmSHA1)
						return "SHA1 [ff1816ec-aa5e-4d10-87f7-6f4963833460]";
					if (guid == KnownGuids.HashAlgorithmSHA256)
						return "SHA256 [8829d00f-11b8-4213-878b-770e8597ac16]";
					return $"Unknown [" + guid + "]";
				}
			}

			[StringFormat("X")]
			public int Hash => MetadataTokens.GetHeapOffset(document.Hash);

			public string HashTooltip {
				get {
					if (document.Hash.IsNil)
						return null;
					System.Collections.Immutable.ImmutableArray<byte> token = metadata.GetBlobContent(document.Hash);
					return token.ToHexString(token.Length);
				}
			}

			[StringFormat("X")]
			public int Language => MetadataTokens.GetHeapOffset(document.Language);

			public string LanguageTooltip {
				get {
					if (document.Language.IsNil)
						return null;
					Guid guid = metadata.GetGuid(document.Language);
					if (guid == KnownGuids.CSharpLanguageGuid)
						return "Visual C# [3f5162f8-07c6-11d3-9053-00c04fa302a1]";
					if (guid == KnownGuids.VBLanguageGuid)
						return "Visual Basic [3a12d0b8-c26c-11d0-b442-00a0244a1dd2]";
					if (guid == KnownGuids.FSharpLanguageGuid)
						return "Visual F# [ab4f38c9-b6e6-43ba-be3b-58080b2ccce3]";
					return $"Unknown [" + guid + "]";
				}
			}

			public DocumentEntry(MetadataReader metadata, bool isEmbedded, DocumentHandle handle)
			{
				this.offset = isEmbedded ? null : (int?)metadata.GetTableMetadataOffset(TableIndex.Document)
					+ metadata.GetTableRowSize(TableIndex.Document) * (MetadataTokens.GetRowNumber(handle) - 1);
				this.metadata = metadata;
				this.handle = handle;
				this.document = metadata.GetDocument(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Document");
		}
	}
}