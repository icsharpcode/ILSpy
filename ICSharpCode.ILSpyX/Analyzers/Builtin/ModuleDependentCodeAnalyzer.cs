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
	/// For an assembly, lists the code in the other assemblies of the list that depends on it: every
	/// type or member whose signature, base types, attributes or body uses a type or member of the
	/// analyzed assembly. Results are grouped by referencing assembly (assemblies in list order).
	/// </summary>
	[ExportAnalyzer(Header = "Dependent Code", Order = 20)]
	[Shared]
	class ModuleDependentCodeAnalyzer : IAnalyzer
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
			var ct = context.CancellationToken;
			var assemblies = context.AssemblyList.GetAllAssemblies().GetAwaiter().GetResult();
			foreach (var assembly in assemblies)
			{
				ct.ThrowIfCancellationRequested();
				var module = assembly.GetMetadataFileOrNull();
				if (module == null || module.IsMetadataOnly || ReferenceEquals(module, target))
					continue;
				var references = DependentCodeScanner.FindReferencesTo(module, target);
				if (references.Count == 0)
					continue;
				var typeSystem = context.GetOrCreateTypeSystem(module);
				foreach (var symbol in DependentCodeScanner.FindDependentSymbols(module, typeSystem, references, ct))
					yield return symbol;
			}
		}
	}
}
