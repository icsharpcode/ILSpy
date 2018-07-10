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
using System.Threading;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// This is the common interface for all analyzers. Note: implementing this interface alone will not provide a full analyzer.
	/// You must implement either the <see cref="ITypeDefinitionAnalyzer{T}"/>, <see cref="IEntityAnalyzer{T}"/> or <see cref="IMethodBodyAnalyzer{T}"/> interfaces.
	/// </summary>
	public interface IAnalyzer<T> where T : IEntity
	{
		/// <summary>
		/// The caption used in the analyzer tree view.
		/// </summary>
		string Text { get; }

		/// <summary>
		/// Returns true, if the analyzer should be shown for an entity, otherwise false.
		/// </summary>
		bool Show(T entity);
	}

	/// <summary>
	/// This interface can be used to implement an analyzer that runs on a single entity.
	/// For an example see <see cref="Builtin.MethodUsesAnalyzer"/>.
	/// </summary>
	/// <typeparam name="T">The type of enitities to be analyzed.</typeparam>
	public interface IEntityAnalyzer<T> : IAnalyzer<T> where T : IEntity
	{
		/// <summary>
		/// Returns all entities that the analyzer found in the given entity.
		/// </summary>
		IEnumerable<IEntity> Analyze(T analyzedEntity, AnalyzerContext context);
	}

	/// <summary>
	/// This interface can be used to implement an analyzer that runs on multiple types.
	/// </summary>
	/// <typeparam name="T">The type of enitities to be analyzed.</typeparam>
	public interface ITypeDefinitionAnalyzer<T> : IAnalyzer<T> where T : IEntity
	{
		/// <summary>
		/// Returns all entities that the analyzer found in the given entity.
		/// </summary>
		/// <param name="analyzedEntity">The entity, which we are looking for.</param>
		/// <param name="type">The type definition, we currently analyze.</param>
		/// <param name="context">Context providing additional information.</param>
		IEnumerable<IEntity> Analyze(T analyzedEntity, ITypeDefinition type, AnalyzerContext context);
	}

	/// <summary>
	/// This interface can be used to implement an analyzer that runs on method bodies.
	/// </summary>
	/// <typeparam name="T">The type of enitities to be analyzed.</typeparam>
	public interface IMethodBodyAnalyzer<T> : IAnalyzer<T> where T : IEntity
	{
		/// <summary>
		/// Returns all entities that the analyzer found in the given method body.
		/// </summary>
		/// <param name="analyzedEntity">The entity, which we are looking for.</param>
		/// <param name="method">The method we analyze.</param>
		/// <param name="methodBody">The method body.</param>
		/// <param name="context">Context providing additional information.</param>
		IEnumerable<IEntity> Analyze(T analyzedEntity, IMethod method, MethodBodyBlock methodBody, AnalyzerContext context);
	}

	/// <summary>
	/// Provides additional context for analyzers.
	/// </summary>
	public class AnalyzerContext
	{
		public AnalyzerContext(IDecompilerTypeSystem typeSystem)
		{
			this.TypeSystem = typeSystem;
		}

		/// <summary>
		/// A type system where all entities of the current assembly and its references can be found.
		/// </summary>
		public IDecompilerTypeSystem TypeSystem { get; }

		/// <summary>
		/// CancellationToken. Currently Analyzers do not support cancellation from the UI, but it should be checked nonetheless.
		/// </summary>
		public CancellationToken CancellationToken { get; internal set; }

		/// <summary>
		/// Mapping info to find parts of a method.
		/// </summary>
		public CodeMappingInfo CodeMappingInfo { get; internal set; }

		/// <summary>
		/// Currently used language.
		/// </summary>
		public Language Language { get; internal set; }
	}
}
