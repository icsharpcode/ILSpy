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
using System.Linq;
using System.Reflection.Metadata;
using System.Windows.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Lists the base types of a class.
	/// </summary>
	sealed class BaseTypesTreeNode : ILSpyTreeNode
	{
		readonly PEFile module;
		readonly ITypeDefinition type;

		public BaseTypesTreeNode(PEFile module, ITypeDefinition type)
		{
			this.module = module;
			this.type = type;
			this.LazyLoading = true;
		}

		public override object Text => "Base Types";

		public override object Icon => Images.SuperTypes;

		protected override void LoadChildren()
		{
			AddBaseTypes(this.Children, module, type);
		}

		internal static void AddBaseTypes(SharpTreeNodeCollection children, PEFile module, ITypeDefinition typeDefinition)
		{
			var typeDef = module.Metadata.GetTypeDefinition((TypeDefinitionHandle)typeDefinition.MetadataToken);
			var baseTypes = typeDefinition.DirectBaseTypes.ToArray();
			int i = 0;
			if (typeDefinition.Kind == TypeKind.Interface) {
				i++;
			} else if (!typeDef.BaseType.IsNil) {
				children.Add(new BaseTypesEntryNode(module, typeDef.BaseType, baseTypes[i], false));
				i++;
			}
			foreach (var h in typeDef.GetInterfaceImplementations()) {
				var impl = module.Metadata.GetInterfaceImplementation(h);
				children.Add(new BaseTypesEntryNode(module, impl.Interface, baseTypes[i], true));
				i++;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(EnsureLazyChildren));
			foreach (ILSpyTreeNode child in this.Children) {
				child.Decompile(language, output, options);
			}
		}
	}
}