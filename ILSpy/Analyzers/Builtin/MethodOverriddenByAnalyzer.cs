using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows methods that override a method.
	/// </summary>
	[Export(typeof(IAnalyzer<IMethod>))]
	class MethodOverriddenByAnalyzer : ITypeDefinitionAnalyzer<IMethod>
	{
		public string Text => "Overridden By";

		public IEnumerable<IEntity> Analyze(IMethod analyzedEntity, ITypeDefinition type, AnalyzerContext context)
		{
			if (!analyzedEntity.DeclaringType.GetAllBaseTypeDefinitions()
				.Any(t => t.MetadataToken == analyzedEntity.DeclaringTypeDefinition.MetadataToken && t.ParentAssembly.PEFile == type.ParentAssembly.PEFile))
				yield break;

			foreach (var property in type.Properties) {
				if (!property.IsOverride) continue;
				if (InheritanceHelper.GetBaseMembers(property, false)
					.Any(p => p.MetadataToken == analyzedEntity.MetadataToken &&
							  p.ParentAssembly.PEFile == analyzedEntity.ParentAssembly.PEFile)) {
					yield return property;
				}
			}
		}

		public bool Show(IMethod entity)
		{
			return entity.IsOverridable && entity.DeclaringType.Kind != TypeKind.Interface;
		}
	}
}
