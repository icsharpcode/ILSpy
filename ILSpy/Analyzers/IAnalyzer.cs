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
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// This is the common interface for all analyzers. Note: implementing this interface alone will not provide a full analyzer.
	/// You must implement either the <see cref="ITypeDefinitionAnalyzer{T}"/>, <see cref="IEntityAnalyzer{T}"/> or <see cref="IMethodBodyAnalyzer{T}"/> interfaces.
	/// </summary>
	public interface IAnalyzer<T> where T : IEntity
	{
		string Text { get; }
		bool Show(T entity);
	}

	public interface IEntityAnalyzer<T> : IAnalyzer<T> where T : IEntity
	{
		IEnumerable<IEntity> Analyze(T analyzedEntity, AnalyzerContext context);
	}

	public interface ITypeDefinitionAnalyzer<T> : IAnalyzer<T> where T : IEntity
	{
		IEnumerable<IEntity> Analyze(T analyzedEntity, ITypeDefinition type, AnalyzerContext context);
	}

	public interface IMethodBodyAnalyzer<T> : IAnalyzer<T> where T : IEntity
	{
		IEnumerable<IEntity> Analyze(T analyzedEntity, IMethod method, MethodBodyBlock methodBody, AnalyzerContext context);
	}

	public class AnalyzerContext
	{
		public AnalyzerContext(IDecompilerTypeSystem typeSystem)
		{
			this.TypeSystem = typeSystem;
		}

		public IDecompilerTypeSystem TypeSystem { get; }
		public CancellationToken CancellationToken { get; internal set; }
		public CodeMappingInfo CodeMappingInfo { get; internal set; }
		public Language Language { get; internal set; }
	}
}
