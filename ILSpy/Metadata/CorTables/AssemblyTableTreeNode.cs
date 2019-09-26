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
	internal class AssemblyTableTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public AssemblyTableTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => $"20 Assembly ({module.Metadata.GetTableRowCount(TableIndex.Assembly)})";

		public override object Icon => Images.Literal;

		public override bool View(DecompilerTextView textView)
		{
			ListView view = Helpers.CreateListView("AssemblyView");

			var metadata = module.Metadata;

			var list = new List<AssemblyEntry>();

			list.Add(new AssemblyEntry(module));

			view.ItemsSource = list;

			textView.ShowContent(new[] { this }, view);
			return true;
		}

		struct AssemblyEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly AssemblyDefinition assembly;

			public int RID => MetadataTokens.GetRowNumber(EntityHandle.AssemblyDefinition);

			public int Token => MetadataTokens.GetToken(EntityHandle.AssemblyDefinition);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.Assembly)
				+ metadata.GetTableRowSize(TableIndex.Assembly) * (RID - 1);

			public int HashAlgorithm => (int)assembly.HashAlgorithm;

			public string HashAlgorithmTooltip => assembly.HashAlgorithm.ToString();

			public int Flags => (int)assembly.Flags;

			public string FlagsTooltip => null;// Helpers.AttributesToString(assembly.Flags);

			public Version Version => assembly.Version;

			public int NameStringHandle => MetadataTokens.GetHeapOffset(assembly.Name);

			public string Name => metadata.GetString(assembly.Name);

			public int CultureStringHandle => MetadataTokens.GetHeapOffset(assembly.Culture);

			public string Culture => metadata.GetString(assembly.Culture);

			public AssemblyEntry(PEFile module)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.assembly = metadata.GetAssemblyDefinition();
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Assembly");
		}
	}
}