using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.Controls;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class TypeRefTableTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public TypeRefTableTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => $"01 TypeRef ({module.Metadata.GetTableRowCount(TableIndex.TypeRef)})";

		public override object Icon => Images.Literal;

		public override bool View(DecompilerTextView textView)
		{
			ListView view = Helpers.CreateListView("TypeRefsView");

			var metadata = module.Metadata;
			
			var list = new List<TypeRefEntry>();
			
			foreach (var row in metadata.TypeReferences)
				list.Add(new TypeRefEntry(module.Reader.PEHeaders.MetadataStartOffset, metadata, row, metadata.GetTypeReference(row)));

			view.ItemsSource = list;
			
			textView.ShowContent(new[] { this }, view);
			return true;
		}

		struct TypeRefEntry
		{
			readonly int metadataOffset;
			readonly MetadataReader metadata;
			readonly TypeReferenceHandle handle;
			readonly TypeReference typeRef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.TypeRef)
				+ metadata.GetTableRowSize(TableIndex.TypeRef) * (RID-1);

			public int ResolutionScope => MetadataTokens.GetToken(typeRef.ResolutionScope);

			public int Name => MetadataTokens.GetHeapOffset(typeRef.Name);

			public string NameTooltip => metadata.GetString(typeRef.Name);

			public int Namespace => MetadataTokens.GetHeapOffset(typeRef.Namespace);

			public string NamespaceTooltip => metadata.GetString(typeRef.Namespace);

			public TypeRefEntry(int metadataOffset, MetadataReader metadata, TypeReferenceHandle handle, TypeReference typeRef)
			{
				this.metadataOffset = metadataOffset;
				this.metadata = metadata;
				this.handle = handle;
				this.typeRef = typeRef;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "TypeRefs");
		}
	}
}