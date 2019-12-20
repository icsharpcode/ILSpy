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
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using System.Diagnostics;
using System.Reflection;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class MethodTableTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public MethodTableTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => $"06 Method ({module.Metadata.GetTableRowCount(TableIndex.MethodDef)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage);
			var metadata = module.Metadata;
			var list = new List<MethodDefEntry>();

			foreach (var row in metadata.MethodDefinitions)
				list.Add(new MethodDefEntry(module, row));

			view.ItemsSource = list;

			tabPage.Content = view;
			return true;
		}

		struct MethodDefEntry : IMemberTreeNode
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly MethodDefinitionHandle handle;
			readonly MethodDefinition methodDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.MethodDef)
				+ metadata.GetTableRowSize(TableIndex.MethodDef) * (RID - 1);

			public MethodAttributes Attributes => methodDef.Attributes;

			public object AttributesTooltip => new FlagsTooltip((int)methodDef.Attributes, typeof(MethodAttributes));

			public MethodImplAttributes ImplAttributes => methodDef.ImplAttributes;

			public object ImplAttributesTooltip => new FlagsTooltip((int)methodDef.ImplAttributes, typeof(MethodImplAttributes));

			public int RVA => methodDef.RelativeVirtualAddress;

			public string Name => metadata.GetString(methodDef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(methodDef.Name):X} \"{Name}\"";

			public int Signature => MetadataTokens.GetHeapOffset(methodDef.Signature);

			string signatureTooltip;

			public string SignatureTooltip {
				get {
					if (signatureTooltip == null) {
						ITextOutput output = new PlainTextOutput();
						var context = new Decompiler.Metadata.GenericContext(default(TypeDefinitionHandle), module);
						((EntityHandle)handle).WriteTo(module, output, context);
						signatureTooltip = output.ToString();
					}
					return signatureTooltip;
				}
			}

			IEntity IMemberTreeNode.Member => ((MetadataModule)module.GetTypeSystemOrNull()?.MainModule).GetDefinition(handle);

			public MethodDefEntry(PEFile module, MethodDefinitionHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.methodDef = metadata.GetMethodDefinition(handle);
				this.signatureTooltip = null;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "MethodDefs");
		}
	}
}
