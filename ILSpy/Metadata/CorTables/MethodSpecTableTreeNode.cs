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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class MethodSpecTableTreeNode : MetadataTableTreeNode
	{
		public MethodSpecTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.MethodSpec, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<MethodSpecEntry>();
			MethodSpecEntry scrollTargetEntry = default;

			foreach (var row in metadata.GetMethodSpecifications())
			{
				MethodSpecEntry entry = new MethodSpecEntry(metadataFile, row);
				if (entry.RID == this.scrollTarget)
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

		struct MethodSpecEntry
		{
			readonly MetadataFile metadataFile;
			readonly MethodSpecificationHandle handle;
			readonly MethodSpecification methodSpec;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.MethodSpec)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.MethodSpec) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Method => MetadataTokens.GetToken(methodSpec.Method);

			public void OnMethodClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, methodSpec.Method, protocol: "metadata")));
			}

			string methodTooltip;
			public string MethodTooltip => GenerateTooltip(ref methodTooltip, metadataFile, methodSpec.Method);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Signature => MetadataTokens.GetHeapOffset(methodSpec.Signature);

			public string SignatureTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var signature = methodSpec.DecodeSignature(new DisassemblerSignatureTypeProvider(metadataFile, output), default);
					bool first = true;
					foreach (var type in signature)
					{
						if (first)
							first = false;
						else
							output.Write(", ");
						type(ILNameSyntax.TypeName);
					}
					return output.ToString();
				}
			}

			public MethodSpecEntry(MetadataFile metadataFile, MethodSpecificationHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.methodSpec = metadataFile.Metadata.GetMethodSpecification(handle);
				this.methodTooltip = null;
			}
		}
	}
}