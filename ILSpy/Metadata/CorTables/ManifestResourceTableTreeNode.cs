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
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	class ManifestResourceTableTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public ManifestResourceTableTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => $"28 ManifestResource ({module.Metadata.GetTableRowCount(TableIndex.ManifestResource)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.SupportsLanguageSwitching = false;
			ListView view = Helpers.CreateListView("ManifestResourcesView");
			var metadata = module.Metadata;

			var list = new List<ManifestResourceEntry>();

			foreach (var row in metadata.ManifestResources) {
				list.Add(new ManifestResourceEntry(module, row));
			}

			view.ItemsSource = list;

			tabPage.Content = view;
			return true;
		}

		struct ManifestResourceEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly ManifestResourceHandle handle;
			readonly ManifestResource manifestResource;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.ManifestResource)
				+ metadata.GetTableRowSize(TableIndex.ManifestResource) * (RID - 1);

			public int Attributes => (int)manifestResource.Attributes;

			public string AttributesTooltip => null; // TODO : Helpers.AttributesToString(manifestResource.Attributes);

			public int NameStringHandle => MetadataTokens.GetHeapOffset(manifestResource.Name);

			public string Name => metadata.GetString(manifestResource.Name);

			public int ImplementationHandle => MetadataTokens.GetToken(manifestResource.Implementation);

			public string ImplementationTooltip {
				get {
					if (manifestResource.Implementation.IsNil)
						return null;
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					manifestResource.Implementation.WriteTo(module, output, context);
					return output.ToString();
				}
			}

			public ManifestResourceEntry(PEFile module, ManifestResourceHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.manifestResource = metadata.GetManifestResource(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "ManifestResources");
		}
	}
}
