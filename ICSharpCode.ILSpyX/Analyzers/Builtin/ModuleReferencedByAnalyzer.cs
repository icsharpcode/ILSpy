// Copyright (c) 2026 Piero Viano
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
using System.Composition;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Dependencies;

namespace ICSharpCode.ILSpyX.Analyzers.Builtin
{
	/// <summary>
	/// For an assembly, lists the assemblies of the list that reference it (by name and public key
	/// token; the referenced version may differ). Each result is itself a module, so it can be
	/// expanded to follow the chain of referencing assemblies further.
	/// </summary>
	[ExportAnalyzer(Header = ModuleAnalyzerHeaders.ReferencedBy, Order = 10)]
	[Shared]
	class ModuleReferencedByAnalyzer : IAnalyzer
	{
		public bool Show(ISymbol? symbol) => symbol is IModule { MetadataFile.IsAssembly: true };

		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			ArgumentNullException.ThrowIfNull(analyzedSymbol);
			ArgumentNullException.ThrowIfNull(context);
			if (analyzedSymbol is not IModule { MetadataFile: { } target })
				throw new ArgumentException("A module with metadata is required.", nameof(analyzedSymbol));
			return AnalyzeCore(target, context);
		}

		static IEnumerable<ISymbol> AnalyzeCore(MetadataFile target, AnalyzerContext context)
		{
			var assemblies = context.AssemblyList.GetAllAssemblies().GetAwaiter().GetResult();
			foreach (var referencing in ReferencedByFinder.FindReferencingAssemblies(assemblies, target, context.CancellationToken))
			{
				context.CancellationToken.ThrowIfCancellationRequested();
				yield return context.GetOrCreateTypeSystem(referencing.Module).MainModule;
			}
		}
	}
}
