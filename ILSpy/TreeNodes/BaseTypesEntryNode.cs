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
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy.TreeNodes
{
	sealed class BaseTypesEntryNode : ILSpyTreeNode, IMemberTreeNode
	{
		readonly PEFile module;
		readonly EntityHandle handle;
		readonly IType type;
		readonly bool isInterface;
		bool showExpander;

		public BaseTypesEntryNode(PEFile module, EntityHandle handle, IType type, bool isInterface)
		{
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			this.handle = handle;
			this.type = type;
			this.isInterface = isInterface;
			this.LazyLoading = true;
			TryResolve(module, handle, type);
		}

		ITypeDefinition TryResolve(PEFile module, EntityHandle handle, IType type, bool mayRetry = true)
		{
			DecompilerTypeSystem typeSystem = new DecompilerTypeSystem(module, module.GetAssemblyResolver(),
				TypeSystemOptions.Default | TypeSystemOptions.Uncached);
			var t = typeSystem.MainModule.ResolveEntity(handle) as ITypeDefinition;
			if (t != null) {
				showExpander = t.DirectBaseTypes.Any();
				var other = t.ParentModule.PEFile.GetTypeSystemOrNull();
				Debug.Assert(other != null);
				t = other.FindType(t.FullTypeName).GetDefinition();
			} else {
				showExpander = mayRetry;
			}
			RaisePropertyChanged(nameof(Text));
			RaisePropertyChanged(nameof(ShowExpander));
			return t;
		}

		public override bool ShowExpander => showExpander && base.ShowExpander;

		public override object Text => this.Language.TypeToString(type, includeNamespace: true) + handle.ToSuffixString();

		public override object Icon => 
			isInterface ? Images.GetIcon(Images.Interface) : Images.GetIcon(Images.Class);

		protected override void LoadChildren()
		{
			var t = TryResolve(module, handle, type, false);
			if (t != null) {
				BaseTypesTreeNode.AddBaseTypes(this.Children, t.ParentModule.PEFile, t);
			}
		}

		public override void ActivateItem(System.Windows.RoutedEventArgs e)
		{
			var t = TryResolve(module, handle, type, false);
			e.Handled = ActivateItem(this, t);
		}

		internal static bool ActivateItem(SharpTreeNode node, ITypeDefinition def)
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
			language.WriteCommentLine(output, language.TypeToString(type, includeNamespace: true));
		}

		IEntity IMemberTreeNode.Member {
			get {
				return TryResolve(module, handle, type, false);
			}
		}
	}
}
