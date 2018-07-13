using System.Collections.Generic;
using System.ComponentModel.Composition;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Finds all extension methods defined for a type.
	/// </summary>
	[Export(typeof(IAnalyzer<ITypeDefinition>))]
	class TypeExtensionMethodsAnalyzer : ITypeDefinitionAnalyzer<ITypeDefinition>
	{
		public string Text => "Extension Methods";

		public bool Show(ITypeDefinition entity) => !entity.IsStatic;

		public IEnumerable<IEntity> Analyze(ITypeDefinition analyzedType, ITypeDefinition type, AnalyzerContext context)
		{
			if (!type.HasExtensionMethods)
				yield break;

			foreach (IMethod method in type.Methods) {
				if (!method.IsExtensionMethod) continue;

				var firstParamType = method.Parameters[0].Type.GetDefinition();
				if (firstParamType != null &&
					firstParamType.MetadataToken == analyzedType.MetadataToken &&
					firstParamType.ParentModule.PEFile == analyzedType.ParentModule.PEFile)
					yield return method;
			}
		}
	}
}
