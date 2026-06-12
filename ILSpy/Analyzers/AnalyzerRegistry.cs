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

using System.Collections.Generic;
using System.Composition;
using System.Linq;

using ICSharpCode.ILSpyX.Analyzers;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// MEF aggregator for <see cref="ExportAnalyzerAttribute"/>-tagged analyzers. System.Composition
	/// only resolves <c>[ImportMany]</c> with metadata through constructor injection, so this
	/// registry is the single place that pulls the factories out of the composition host. Each
	/// <see cref="AnalyzerEntityTreeNode"/> reads <see cref="Analyzers"/> through the static
	/// accessor on <see cref="AnalyzerTreeNode"/>, which in turn resolves this registry once.
	/// </summary>
	[Export]
	[Shared]
	public sealed class AnalyzerRegistry
	{
		[ImportingConstructor]
		public AnalyzerRegistry(
			[ImportMany("Analyzer")] IEnumerable<ExportFactory<IAnalyzer, AnalyzerMetadata>> analyzers)
		{
			Analyzers = analyzers
				.OrderBy(a => a.Metadata?.Order ?? 0)
				.ToArray();
		}

		public IReadOnlyList<ExportFactory<IAnalyzer, AnalyzerMetadata>> Analyzers { get; }
	}
}
