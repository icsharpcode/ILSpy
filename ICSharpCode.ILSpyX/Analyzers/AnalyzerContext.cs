// Copyright (c) 2018 Siegfried Pammer
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
using System.Collections.Concurrent;
using System.Reflection.Metadata;
using System.Threading;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Abstractions;

namespace ICSharpCode.ILSpyX.Analyzers
{
	/// <summary>
	/// Provides additional context for analyzers.
	/// </summary>
	public class AnalyzerContext
	{
		public required AssemblyList AssemblyList { get; init; }

		/// <summary>
		/// CancellationToken. Currently Analyzers do not support cancellation from the UI, but it should be checked nonetheless.
		/// </summary>
		public CancellationToken CancellationToken { get; init; }

		/// <summary>
		/// Currently used language.
		/// </summary>
		public required ILanguage Language { get; init; }

		/// <summary>
		/// Allows the analyzer to control whether the tree nodes will be sorted.
		/// Must be set within <see cref="IAnalyzer.Analyze(ISymbol, AnalyzerContext)"/>
		/// before the results are enumerated.
		/// </summary>
		public bool SortResults { get; set; }

		public MethodBodyBlock? GetMethodBody(IMethod method)
		{
			if (!method.HasBody || method.MetadataToken.IsNil || method.ParentModule?.MetadataFile == null)
				return null;
			var module = method.ParentModule.MetadataFile;
			var md = module.Metadata.GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
			try
			{
				return module.GetMethodBody(md.RelativeVirtualAddress);
			}
			catch (BadImageFormatException)
			{
				return null;
			}
		}

		public AnalyzerScope GetScopeOf(IEntity entity)
		{
			return new AnalyzerScope(AssemblyList, entity);
		}

		readonly ConcurrentDictionary<MetadataFile, DecompilerTypeSystem> typeSystemCache = new();

		public DecompilerTypeSystem GetOrCreateTypeSystem(MetadataFile module)
		{
			return typeSystemCache.GetOrAdd(module, m => new DecompilerTypeSystem(m, m.GetAssemblyResolver()));
		}
	}
}
