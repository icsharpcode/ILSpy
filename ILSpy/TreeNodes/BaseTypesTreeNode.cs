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
using ICSharpCode.ILSpyX;
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

		public override object Text => Properties.Resources.BaseTypes;

		public override object Icon => Images.SuperTypes;

		protected override void LoadChildren()
		{
			AddBaseTypes(this.Children, module, type);
		}

		internal static void AddBaseTypes(SharpTreeNodeCollection children, PEFile module, ITypeDefinition typeDefinition)
		{
			TypeDefinitionHandle handle = (TypeDefinitionHandle)typeDefinition.MetadataToken;
			DecompilerTypeSystem typeSystem = new DecompilerTypeSystem(module, module.GetAssemblyResolver(),
				TypeSystemOptions.Default | TypeSystemOptions.Uncached);
			var t = typeSystem.MainModule.ResolveEntity(handle) as ITypeDefinition;
			foreach (var td in t.GetAllBaseTypeDefinitions().Reverse().Skip(1))
			{
				if (t.Kind != TypeKind.Interface || t.Kind == td.Kind)
					children.Add(new BaseTypesEntryNode(td));
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(EnsureLazyChildren));
			foreach (ILSpyTreeNode child in this.Children)
			{
				child.Decompile(language, output, options);
			}
		}
	}
}