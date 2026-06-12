// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.Analyzers.TreeNodes;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Row that runs a single <see cref="IAnalyzer"/> against an analysed symbol and shows
	/// its results as its lazy-loaded children. On expansion a <see cref="Task.Run"/> calls
	/// <see cref="IAnalyzer.Analyze"/> off the UI thread and posts each result back through
	/// <see cref="Dispatcher.UIThread"/> so the tree updates incrementally. Collapsing the
	/// row cancels the in-flight task and re-arms <see cref="SharpTreeNode.LazyLoading"/>
	/// so the next expand starts a fresh fetch.
	/// </summary>
	public class AnalyzerSearchTreeNode : AnalyzerTreeNode
	{
		readonly ISymbol analyzedSymbol;
		readonly IAnalyzer analyzer;
		readonly string headerText;
		readonly Stopwatch stopwatch = new Stopwatch();
		CancellationTokenSource? cancellation;
		Task? loadTask;

		public AnalyzerSearchTreeNode(ISymbol analyzedSymbol, IAnalyzer analyzer, string? analyzerHeader)
		{
			this.analyzedSymbol = analyzedSymbol ?? throw new ArgumentNullException(nameof(analyzedSymbol));
			this.analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
			this.headerText = analyzerHeader ?? string.Empty;
			LazyLoading = true;
		}

		public IAnalyzer Analyzer => analyzer;
		public ISymbol AnalyzedSymbol => analyzedSymbol;

		/// <summary>The header string this row started with (e.g. "Used By").</summary>
		public string AnalyzerHeader => headerText;

		/// <summary>True while the background fetch is in flight.</summary>
		public bool IsLoading => loadTask is { IsCompleted: false };

		public override object Text => IsLoading || Children.Count == 0
			? headerText
			: headerText + " (" + Children.Count + " in " + stopwatch.ElapsedMilliseconds + " ms)";

		/// <summary>
		/// Semantic icon for an analyzer-search header row ("Used By", "Uses", "Exposed By",
		/// etc.). Without this override the row inherits null from <see cref="SharpTreeNode"/>
		/// and renders an empty icon slot next to the header text — visually mismatched with
		/// the result rows underneath, which all carry entity-kind icons.
		/// </summary>
		public override object Icon => Images.Search;

		protected override void LoadChildren()
		{
			cancellation?.Cancel();
			cancellation?.Dispose();
			cancellation = new CancellationTokenSource();
			var token = cancellation.Token;

			// "Loading…" placeholder. Removed once the task settles. Posted synchronously
			// so users who expand a long-running analysis see immediate feedback.
			Children.Add(new LoadingPlaceholderNode());

			stopwatch.Restart();
			loadTask = Task.Run(() => RunAnalyzer(token), token);
		}

		void RunAnalyzer(CancellationToken ct)
		{
			var assemblyList = CurrentAssemblyList;
			if (assemblyList == null)
			{
				FinishOnUIThread(error: new InvalidOperationException("no active assembly list"));
				return;
			}
			var context = new AnalyzerContext {
				CancellationToken = ct,
				Language = Language,
				AssemblyList = assemblyList,
			};
			try
			{
				foreach (var resultSymbol in analyzer.Analyze(analyzedSymbol, context))
				{
					if (ct.IsCancellationRequested)
						return;
					var child = WrapResult(resultSymbol);
					Dispatcher.UIThread.Post(() => {
						if (ct.IsCancellationRequested)
							return;
						// Insert before the trailing placeholder.
						var index = Math.Max(0, Children.Count - 1);
						Children.Insert(index, child);
					});
				}
				FinishOnUIThread(error: null);
			}
			catch (OperationCanceledException)
			{
				// Expected on collapse; OnCollapsing has already cleared state.
			}
			catch (Exception ex)
			{
				FinishOnUIThread(error: ex);
			}
		}

		void FinishOnUIThread(Exception? error)
		{
			Dispatcher.UIThread.Post(() => {
				stopwatch.Stop();
				// Drop the placeholder if it's still the trailing child.
				if (Children.Count > 0 && Children[Children.Count - 1] is LoadingPlaceholderNode)
					Children.RemoveAt(Children.Count - 1);
				if (error != null)
					Children.Add(new AnalyzerErrorNode(error));
				RaisePropertyChanged(nameof(Text));
			});
		}

		AnalyzerTreeNode WrapResult(ISymbol resultSymbol)
		{
			var sourceEntity = analyzedSymbol as IEntity;
			return resultSymbol switch {
				IModule module => new AnalyzedModuleTreeNode(module, sourceEntity),
				ITypeDefinition type => new AnalyzedTypeTreeNode(type, sourceEntity),
				IField field => new AnalyzedFieldTreeNode(field, sourceEntity),
				IMethod method => new AnalyzedMethodTreeNode(method, sourceEntity),
				IProperty property => new AnalyzedPropertyTreeNode(property, sourceEntity),
				IEvent ev => new AnalyzedEventTreeNode(ev, sourceEntity),
				_ => throw new ArgumentOutOfRangeException(nameof(resultSymbol),
					$"Symbol {resultSymbol.GetType().FullName} is not supported.")
			};
		}

		protected override void OnCollapsing()
		{
			base.OnCollapsing();
			if (loadTask == null)
				return;
			cancellation?.Cancel();
			cancellation?.Dispose();
			cancellation = null;
			loadTask = null;
			stopwatch.Reset();
			Children.Clear();
			LazyLoading = true;
			RaisePropertyChanged(nameof(Text));
		}

		public override bool HandleAssemblyListChanged(
			ICollection<LoadedAssembly> removedAssemblies,
			ICollection<LoadedAssembly> addedAssemblies)
		{
			bool manualAdd = addedAssemblies.Any(a => !a.IsAutoLoaded);
			if (removedAssemblies.Count > 0 || manualAdd)
			{
				cancellation?.Cancel();
				cancellation?.Dispose();
				cancellation = null;
				loadTask = null;
				stopwatch.Reset();
				LazyLoading = true;
				Children.Clear();
				RaisePropertyChanged(nameof(Text));
			}
			return true;
		}

		sealed class LoadingPlaceholderNode : AnalyzerTreeNode
		{
			public override object Text => "Loading…";

			public override bool HandleAssemblyListChanged(
				ICollection<LoadedAssembly> removedAssemblies,
				ICollection<LoadedAssembly> addedAssemblies) => true;
		}
	}
}
