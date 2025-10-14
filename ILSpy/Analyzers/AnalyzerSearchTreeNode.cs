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
using System.Threading;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Analyzers.TreeNodes;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;

namespace ICSharpCode.ILSpy.Analyzers
{
	class AnalyzerSearchTreeNode : AnalyzerTreeNode
	{
		private readonly ThreadingSupport threading = new ThreadingSupport();
		readonly ISymbol symbol;
		readonly IAnalyzer analyzer;
		readonly string analyzerHeader;

		public AnalyzerSearchTreeNode(ISymbol symbol, IAnalyzer analyzer, string analyzerHeader)
		{
			this.symbol = symbol;
			this.analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
			this.LazyLoading = true;
			this.analyzerHeader = analyzerHeader;
		}

		public override object Text => analyzerHeader
			+ (Children.Count > 0 && !threading.IsRunning ? " (" + Children.Count + " in " + threading.EllapsedMilliseconds + "ms)" : "");

		public override object Icon => Images.Search;

		protected override void LoadChildren()
		{
			threading.LoadChildren(this, FetchChildren);
		}

		protected IEnumerable<AnalyzerTreeNode> FetchChildren(CancellationToken ct)
		{
			if (symbol is IEntity)
			{
				var context = new AnalyzerContext {
					CancellationToken = ct,
					Language = Language,
					AssemblyList = AssemblyList
				};
				var results = analyzer.Analyze(symbol, context).Select(SymbolTreeNodeFactory);
				if (context.SortResults)
				{
					results = results.OrderBy(tn => tn.Text?.ToString(), NaturalStringComparer.Instance);
				}
				return results;
			}
			else
			{
				throw new NotSupportedException("Currently symbols that are not entities are not supported!");
			}
		}

		AnalyzerTreeNode SymbolTreeNodeFactory(ISymbol resultSymbol)
		{
			if (resultSymbol == null)
			{
				throw new ArgumentNullException(nameof(resultSymbol));
			}

			switch (resultSymbol)
			{
				case IModule module:
					return new AnalyzedModuleTreeNode(module, (IEntity)this.symbol);
				case ITypeDefinition td:
					return new AnalyzedTypeTreeNode(td, (IEntity)this.symbol);
				case IField fd:
					return new AnalyzedFieldTreeNode(fd, (IEntity)this.symbol);
				case IMethod md:
					return new AnalyzedMethodTreeNode(md, (IEntity)this.symbol);
				case IProperty pd:
					return new AnalyzedPropertyTreeNode(pd, (IEntity)this.symbol);
				case IEvent ed:
					return new AnalyzedEventTreeNode(ed, (IEntity)this.symbol);
				default:
					throw new ArgumentOutOfRangeException(nameof(resultSymbol), $"Symbol {resultSymbol.GetType().FullName} is not supported.");
			}
		}

		protected override void OnIsVisibleChanged()
		{
			base.OnIsVisibleChanged();
			if (!this.IsVisible && threading.IsRunning)
			{
				this.LazyLoading = true;
				threading.Cancel();
				this.Children.Clear();
				RaisePropertyChanged(nameof(Text));
			}
		}

		public override bool HandleAssemblyListChanged(ICollection<LoadedAssembly> removedAssemblies, ICollection<LoadedAssembly> addedAssemblies)
		{
			// only cancel a running analysis if user has manually added/removed assemblies
			bool manualAdd = false;
			foreach (var asm in addedAssemblies)
			{
				if (!asm.IsAutoLoaded)
					manualAdd = true;
			}
			if (removedAssemblies.Count > 0 || manualAdd)
			{
				this.LazyLoading = true;
				threading.Cancel();
				this.Children.Clear();
				RaisePropertyChanged(nameof(Text));
			}
			return true;
		}
	}
}
