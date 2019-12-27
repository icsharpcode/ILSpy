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
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class TypeRefTableTreeNode : MetadataTableTreeNode
	{
		public TypeRefTableTreeNode(PEFile module)
			: base(HandleKind.TypeReference, module)
		{
		}

		public override object Text => $"01 TypeRef ({module.Metadata.GetTableRowCount(TableIndex.TypeRef)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage);
			var metadata = module.Metadata;
			
			var list = new List<TypeRefEntry>();
			TypeRefEntry scrollTargetEntry = default;

			foreach (var row in metadata.TypeReferences) {
				TypeRefEntry entry = new TypeRefEntry(module, row);
				if (entry.RID == this.scrollTarget) {
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;
			
			tabPage.Content = view;

			if (scrollTargetEntry.RID > 0) {
				view.ScrollIntoView(scrollTargetEntry);
				this.scrollTarget = default;
			}

			return true;
		}

		struct TypeRefEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly TypeReferenceHandle handle;
			readonly TypeReference typeRef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.TypeRef)
				+ metadata.GetTableRowSize(TableIndex.TypeRef) * (RID-1);

			[StringFormat("X8")]
			public int ResolutionScope => MetadataTokens.GetToken(typeRef.ResolutionScope);

			public string ResolutionScopeTooltip {
				get {
					if (typeRef.ResolutionScope.IsNil)
						return null;
					var output = new PlainTextOutput();
					switch (typeRef.ResolutionScope.Kind) {
						case HandleKind.ModuleDefinition:
							output.Write(metadata.GetString(metadata.GetModuleDefinition().Name));
							break;
						case HandleKind.ModuleReference:
							ModuleReference moduleReference = metadata.GetModuleReference((ModuleReferenceHandle)typeRef.ResolutionScope);
							output.Write(metadata.GetString(moduleReference.Name));
							break;
						case HandleKind.AssemblyReference:
							var asmRef = new Decompiler.Metadata.AssemblyReference(module, (AssemblyReferenceHandle)typeRef.ResolutionScope);
							output.Write(asmRef.ToString());
							break;
						default:
							typeRef.ResolutionScope.WriteTo(module, output, GenericContext.Empty);
							break;
					}
					return output.ToString();
				}
			}

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(typeRef.Name):X} \"{Name}\"";

			public string Name => metadata.GetString(typeRef.Name);

			public string NamespaceTooltip => $"{MetadataTokens.GetHeapOffset(typeRef.Namespace):X} \"{Namespace}\"";

			public string Namespace => metadata.GetString(typeRef.Namespace);

			public TypeRefEntry(PEFile module, TypeReferenceHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.typeRef = metadata.GetTypeReference(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "TypeRefs");
		}
	}
}