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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Node within assembly reference list.
	/// </summary>
	public sealed class AssemblyReferenceTreeNode : ILSpyTreeNode
	{
		readonly AssemblyReference r;
		readonly AssemblyTreeNode parentAssembly;

		public AssemblyReferenceTreeNode(AssemblyReference r, AssemblyTreeNode parentAssembly)
		{
			this.r = r ?? throw new ArgumentNullException(nameof(r));
			this.parentAssembly = parentAssembly ?? throw new ArgumentNullException(nameof(parentAssembly));
			this.LazyLoading = true;
		}

		public IAssemblyReference AssemblyNameReference => r;

		public override object Text {
			get { return r.Name + ((System.Reflection.Metadata.EntityHandle)r.Handle).ToSuffixString(); }
		}

		public override object Icon => Images.Assembly;

		public override bool ShowExpander {
			get {
				if (r.Name == "mscorlib")
					EnsureLazyChildren(); // likely doesn't have any children
				return base.ShowExpander;
			}
		}

		public override void ActivateItem(System.Windows.RoutedEventArgs e)
		{
			var assemblyListNode = parentAssembly.Parent as AssemblyListTreeNode;
			if (assemblyListNode != null)
			{
				assemblyListNode.Select(assemblyListNode.FindAssemblyNode(parentAssembly.LoadedAssembly.LookupReferencedAssembly(r)));
				e.Handled = true;
			}
		}

		protected override void LoadChildren()
		{
			var assemblyListNode = parentAssembly.Parent as AssemblyListTreeNode;
			if (assemblyListNode != null)
			{
				var refNode = assemblyListNode.FindAssemblyNode(parentAssembly.LoadedAssembly.LookupReferencedAssembly(r));
				if (refNode != null)
				{
					var module = refNode.LoadedAssembly.GetPEFileOrNull();
					if (module != null)
					{
						foreach (var childRef in module.AssemblyReferences)
							this.Children.Add(new AssemblyReferenceTreeNode(childRef, refNode));
					}
				}
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			var loaded = parentAssembly.LoadedAssembly.LoadedAssemblyReferencesInfo.TryGetInfo(r.FullName, out var info);
			if (r.IsWindowsRuntime)
			{
				language.WriteCommentLine(output, r.FullName + " [WinRT]" + (!loaded ? " (unresolved)" : ""));
			}
			else
			{
				language.WriteCommentLine(output, r.FullName + (!loaded ? " (unresolved)" : ""));
			}
			if (loaded)
			{
				output.Indent();
				language.WriteCommentLine(output, "Assembly reference loading information:");
				if (info.HasErrors)
					language.WriteCommentLine(output, "There were some problems during assembly reference load, see below for more information!");
				foreach (var item in info.Messages)
				{
					language.WriteCommentLine(output, $"{item.Item1}: {item.Item2}");
				}
				output.Unindent();
				output.WriteLine();
			}
		}
	}
}
