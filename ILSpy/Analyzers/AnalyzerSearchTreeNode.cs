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

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;

namespace ILSpy.Analyzers
{
	/// <summary>
	/// Row that runs a single <see cref="IAnalyzer"/> against an analysed symbol and
	/// shows its results as its lazy-loaded children. The shell defined here holds the
	/// analyser + symbol + header triple; the background-fetch wiring (Task.Run,
	/// "Loading…" placeholder, count + elapsed-time text update, cancellation) lands in
	/// a follow-up commit.
	/// </summary>
	public class AnalyzerSearchTreeNode : AnalyzerTreeNode
	{
		readonly ISymbol analyzedSymbol;
		readonly IAnalyzer analyzer;

		public AnalyzerSearchTreeNode(ISymbol analyzedSymbol, IAnalyzer analyzer, string? analyzerHeader)
		{
			this.analyzedSymbol = analyzedSymbol ?? throw new ArgumentNullException(nameof(analyzedSymbol));
			this.analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
			AnalyzerHeader = analyzerHeader ?? string.Empty;
			LazyLoading = true;
		}

		/// <summary>The analyser whose <see cref="IAnalyzer.Analyze"/> drives this row.</summary>
		public IAnalyzer Analyzer => analyzer;

		/// <summary>The symbol being analysed (a type, method, field, …).</summary>
		public ISymbol AnalyzedSymbol => analyzedSymbol;

		/// <summary>
		/// The header string this row started with (e.g. "Used By"). Updated in-place by
		/// the background-fetch pipeline to include the result count and elapsed time
		/// once the analyser completes.
		/// </summary>
		public string AnalyzerHeader { get; protected set; }

		public override object Text => AnalyzerHeader;

		public override bool HandleAssemblyListChanged(
			ICollection<LoadedAssembly> removedAssemblies,
			ICollection<LoadedAssembly> addedAssemblies)
		{
			this.Children.RemoveAll(node =>
				node is not AnalyzerTreeNode an
				|| !an.HandleAssemblyListChanged(removedAssemblies, addedAssemblies));
			return true;
		}
	}
}
