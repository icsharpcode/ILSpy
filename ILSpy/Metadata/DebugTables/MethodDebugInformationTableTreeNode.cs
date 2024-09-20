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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class MethodDebugInformationTableTreeNode : DebugMetadataTableTreeNode
	{
		public MethodDebugInformationTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.MethodDebugInformation, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var list = new List<MethodDebugInformationEntry>();
			MethodDebugInformationEntry scrollTargetEntry = default;

			foreach (var row in metadataFile.Metadata.MethodDebugInformation)
			{
				MethodDebugInformationEntry entry = new MethodDebugInformationEntry(metadataFile, row);
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

		struct MethodDebugInformationEntry
		{
			readonly int? offset;
			readonly MetadataFile metadataFile;
			readonly MethodDebugInformationHandle handle;
			readonly MethodDebugInformation debugInfo;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public object Offset => offset == null ? "n/a" : (object)offset;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Document => MetadataTokens.GetToken(debugInfo.Document);

			public void OnDocumentClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, debugInfo.Document, protocol: "metadata")));
			}

			public string DocumentTooltip {
				get {
					if (debugInfo.Document.IsNil)
						return null;
					var document = metadataFile.Metadata.GetDocument(debugInfo.Document);
					return $"{MetadataTokens.GetHeapOffset(document.Name):X} \"{metadataFile.Metadata.GetString(document.Name)}\"";
				}
			}

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int SequencePoints => MetadataTokens.GetHeapOffset(debugInfo.SequencePointsBlob);

			public string SequencePointsTooltip {
				get {
					if (debugInfo.SequencePointsBlob.IsNil)
						return null;
					StringBuilder sb = new StringBuilder();
					foreach (var p in debugInfo.GetSequencePoints())
					{
						sb.AppendLine($"document='{MetadataTokens.GetToken(p.Document):X8}', offset={p.Offset}, start={p.StartLine};{p.StartColumn}, end={p.EndLine};{p.EndColumn}, hidden={p.IsHidden}");
					}
					return sb.ToString().TrimEnd();
				}
			}

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int LocalSignature => MetadataTokens.GetToken(debugInfo.LocalSignature);

			public void OnLocalSignatureClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, debugInfo.LocalSignature, protocol: "metadata")));
			}

			public string LocalSignatureTooltip {
				get {
					if (debugInfo.LocalSignature.IsNil)
						return null;
					ITextOutput output = new PlainTextOutput();
					var context = new MetadataGenericContext(default(TypeDefinitionHandle), metadataFile.Metadata);
					StandaloneSignature localSignature = metadataFile.Metadata.GetStandaloneSignature(debugInfo.LocalSignature);
					var signatureDecoder = new DisassemblerSignatureTypeProvider(metadataFile, output);
					int index = 0;
					foreach (var item in localSignature.DecodeLocalSignature(signatureDecoder, context))
					{
						if (index > 0)
							output.WriteLine();
						output.Write("[{0}] ", index);
						item(ILNameSyntax.Signature);
						index++;
					}
					return output.ToString();
				}
			}

			public MethodDebugInformationEntry(MetadataFile metadataFile, MethodDebugInformationHandle handle)
			{
				this.metadataFile = metadataFile;
				this.offset = metadataFile.IsEmbedded ? null : (int?)metadataFile.Metadata.GetTableMetadataOffset(TableIndex.MethodDebugInformation)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.MethodDebugInformation) * (MetadataTokens.GetRowNumber(handle) - 1);
				this.handle = handle;
				this.debugInfo = metadataFile.Metadata.GetMethodDebugInformation(handle);
			}
		}
	}
}