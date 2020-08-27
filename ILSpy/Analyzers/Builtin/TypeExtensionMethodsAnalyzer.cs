using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Finds all extension methods defined for a type.
	/// </summary>
	[ExportAnalyzer(Header = "Extension Methods", Order = 50)]
	class TypeExtensionMethodsAnalyzer : IAnalyzer
	{
		public bool Show(ISymbol symbol) => symbol is ITypeDefinition entity && !entity.IsStatic;

		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			Debug.Assert(analyzedSymbol is ITypeDefinition);
			var scope = context.GetScopeOf((ITypeDefinition)analyzedSymbol);
			foreach (var type in scope.GetTypesInScope(context.CancellationToken))
			{
				foreach (var result in ScanType((ITypeDefinition)analyzedSymbol, type, context))
					yield return result;
			}
		}

		IEnumerable<IEntity> ScanType(ITypeDefinition analyzedType, ITypeDefinition type, AnalyzerContext context)
		{
			if (!type.HasExtensionMethods)
				yield break;

			foreach (IMethod method in type.Methods)
			{
				if (!method.IsExtensionMethod)
					continue;

				var firstParamType = method.Parameters[0].Type.GetDefinition();
				if (firstParamType != null &&
					firstParamType.MetadataToken == analyzedType.MetadataToken &&
					firstParamType.ParentModule.PEFile == analyzedType.ParentModule.PEFile)
					yield return method;
			}
		}
	}
}
