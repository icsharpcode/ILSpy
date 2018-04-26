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
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy.TreeNodes
{
	sealed class BaseTypesEntryNode : ILSpyTreeNode, IMemberTreeNode
	{
		readonly Entity tr;
		TypeDefinition td;
		readonly bool isInterface;

		public BaseTypesEntryNode(Entity entity, bool isInterface)
		{
			if (entity.IsNil) throw new ArgumentNullException(nameof(entity));
			this.tr = entity;
			this.td = tr.ResolveAsType();
			this.isInterface = isInterface;
			this.LazyLoading = true;
		}

		public override bool ShowExpander
		{
			get {
				if (td.IsNil) return false;
				var typeDef = td.This();
				return !typeDef.BaseType.IsNil || typeDef.GetInterfaceImplementations().Any();
			}
		}

		public override object Text
		{
			get { return tr.Handle.GetFullTypeName(tr.Module.GetMetadataReader()) + tr.Handle.ToSuffixString(); }
		}

		public override object Icon
		{
			get
			{
				if (td != null)
					return TypeTreeNode.GetIcon(td);
				else
					return isInterface ? Images.Interface : Images.Class;
			}
		}

		protected override void LoadChildren()
		{
			if (td != null)
				BaseTypesTreeNode.AddBaseTypes(this.Children, td);
		}

		public override void ActivateItem(System.Windows.RoutedEventArgs e)
		{
			// on item activation, try to resolve once again (maybe the user loaded the assembly in the meantime)
			if (td.IsNil) {
				td = tr.ResolveAsType();
				if (!td.IsNil)
					this.LazyLoading = true;
				// re-load children
			}
			e.Handled = ActivateItem(this, td);
		}

		internal static bool ActivateItem(SharpTreeNode node, TypeDefinition def)
		{
			if (def != null) {
				var assemblyListNode = node.Ancestors().OfType<AssemblyListTreeNode>().FirstOrDefault();
				if (assemblyListNode != null) {
					assemblyListNode.Select(assemblyListNode.FindTypeNode(def));
					return true;
				}
			}
			return false;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, language.TypeDefinitionToString(tr, true));
		}

		IMetadataEntity IMemberTreeNode.Member => tr;
	}
}
