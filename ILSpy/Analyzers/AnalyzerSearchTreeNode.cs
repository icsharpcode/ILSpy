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

namespace ICSharpCode.ILSpy.Analyzers
{
	class AnalyzerSearchTreeNode<T> : AnalyzerTreeNode where T : IEntity
	{
		private readonly ThreadingSupport threading = new ThreadingSupport();
		readonly T analyzedEntity;
		readonly IAnalyzer<T> analyzer;

		public AnalyzerSearchTreeNode(T entity, IAnalyzer<T> analyzer)
		{
			this.analyzedEntity = entity;
			this.analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
			this.LazyLoading = true;
		}

		public override object Text => analyzer.Text;

		public override object Icon => Images.Search;

		protected override void LoadChildren()
		{
			threading.LoadChildren(this, FetchChildren);
		}

		protected IEnumerable<AnalyzerTreeNode> FetchChildren(CancellationToken ct)
		{
			if (analyzer is IEntityAnalyzer<T> entityAnalyzer) {
				var module = analyzedEntity.ParentModule.PEFile;
				var ts = new DecompilerTypeSystem(module, module.GetAssemblyResolver());
				var context = new AnalyzerContext(ts) {
					CancellationToken = ct,
					Language = Language,
					CodeMappingInfo = Language.GetCodeMappingInfo(module, analyzedEntity.MetadataToken)
				};
				foreach (var result in entityAnalyzer.Analyze(analyzedEntity, context)) {
					yield return EntityTreeNodeFactory(result);
				}
			} else {
				foreach (var result in new ScopedWhereUsedAnalyzer<T>(Language, analyzedEntity, analyzer).PerformAnalysis(ct).Distinct()) {
					yield return EntityTreeNodeFactory(result);
				}
			}
		}

		AnalyzerTreeNode EntityTreeNodeFactory(IEntity entity)
		{
			if (entity == null) {
				throw new ArgumentNullException(nameof(entity));
			}

			switch (entity) {
				case ITypeDefinition td:
					return new AnalyzedTypeTreeNode(td) {
						Language = this.Language
					};
				case IField fd:
					return new AnalyzedFieldTreeNode(fd) {
						Language = this.Language
					};
				case IMethod md:
					return new AnalyzedMethodTreeNode(md) {
						Language = this.Language
					};
				case IProperty pd:
					return new AnalyzedPropertyTreeNode(pd) {
						Language = this.Language
					};
				case IEvent ed:
					return new AnalyzedEventTreeNode(ed) {
						Language = this.Language
					};
				default:
					throw new ArgumentOutOfRangeException(nameof(entity), $"Entity {entity.GetType().FullName} is not supported.");
			}
		}

		protected override void OnIsVisibleChanged()
		{
			base.OnIsVisibleChanged();
			if (!this.IsVisible && threading.IsRunning) {
				this.LazyLoading = true;
				threading.Cancel();
				this.Children.Clear();
			}
		}

		public override bool HandleAssemblyListChanged(ICollection<LoadedAssembly> removedAssemblies, ICollection<LoadedAssembly> addedAssemblies)
		{
			// only cancel a running analysis if user has manually added/removed assemblies
			bool manualAdd = false;
			foreach (var asm in addedAssemblies) {
				if (!asm.IsAutoLoaded)
					manualAdd = true;
			}
			if (removedAssemblies.Count > 0 || manualAdd) {
				this.LazyLoading = true;
				threading.Cancel();
				this.Children.Clear();
			}
			return true;
		}
	}
}
