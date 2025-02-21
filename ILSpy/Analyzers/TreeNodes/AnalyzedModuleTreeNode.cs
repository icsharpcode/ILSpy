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
using System.Windows;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy.Analyzers.TreeNodes
{
	internal class AnalyzedModuleTreeNode : AnalyzerEntityTreeNode
	{
		readonly IModule analyzedModule;

		public AnalyzedModuleTreeNode(IModule analyzedModule)
		{
			this.analyzedModule = analyzedModule ?? throw new ArgumentNullException(nameof(analyzedModule));
			this.LazyLoading = true;
		}

		public override object Icon => Images.Assembly;

		public override object Text => analyzedModule.AssemblyName;

		public override object ToolTip => analyzedModule.MetadataFile?.FileName;

		protected override void LoadChildren()
		{
			foreach (var lazy in Analyzers)
			{
				var analyzer = lazy.Value;
				if (analyzer.Show(analyzedModule))
				{
					this.Children.Add(new AnalyzerSearchTreeNode(analyzedModule, analyzer, lazy.Metadata.Header));
				}
			}
		}

		public override void ActivateItem(IPlatformRoutedEventArgs e)
		{
			e.Handled = true;
			if (analyzedModule.MetadataFile == null)
			{
				MessageBox.Show(Properties.Resources.CannotAnalyzeMissingRef, "ILSpy");
				return;
			}
			MessageBus.Send(this, new NavigateToReferenceEventArgs(analyzedModule.MetadataFile));
		}

		public override IEntity Member => null;

		public override bool HandleAssemblyListChanged(ICollection<LoadedAssembly> removedAssemblies, ICollection<LoadedAssembly> addedAssemblies)
		{
			if (analyzedModule == null)
			{
				return true;
			}
			foreach (LoadedAssembly asm in removedAssemblies)
			{
				if (this.analyzedModule.MetadataFile == asm.GetMetadataFileOrNull())
					return false; // remove this node
			}
			this.Children.RemoveAll(
				delegate (SharpTreeNode n) {
					AnalyzerTreeNode an = n as AnalyzerTreeNode;
					return an == null || !an.HandleAssemblyListChanged(removedAssemblies, addedAssemblies);
				});
			return true;
		}
	}
}
