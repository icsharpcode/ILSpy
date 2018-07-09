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
	}
}
