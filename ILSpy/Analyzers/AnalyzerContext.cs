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

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Provides additional context for analyzers.
	/// </summary>
	public class AnalyzerContext
	{
		public AssemblyList AssemblyList { get; internal set; }

		/// <summary>
		/// CancellationToken. Currently Analyzers do not support cancellation from the UI, but it should be checked nonetheless.
		/// </summary>
		public CancellationToken CancellationToken { get; internal set; }

		/// <summary>
		/// Currently used language.
		/// </summary>
		public Language Language { get; internal set; }

		public MethodBodyBlock GetMethodBody(IMethod method)
		{
			if (!method.HasBody || method.MetadataToken.IsNil)
				return null;
			var module = method.ParentModule.PEFile;
			var md = module.Metadata.GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
			try
			{
				return module.Reader.GetMethodBody(md.RelativeVirtualAddress);
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

		readonly ConcurrentDictionary<PEFile, DecompilerTypeSystem> typeSystemCache = new ConcurrentDictionary<PEFile, DecompilerTypeSystem>();

		public DecompilerTypeSystem GetOrCreateTypeSystem(PEFile module)
		{
			return typeSystemCache.GetOrAdd(module, m => new DecompilerTypeSystem(m, m.GetAssemblyResolver()));
		}
	}
}
