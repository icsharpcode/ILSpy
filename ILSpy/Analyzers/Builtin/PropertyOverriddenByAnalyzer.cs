using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows properties that override a property.
	/// </summary>
	[Export(typeof(IAnalyzer<IProperty>))]
	class PropertyOverriddenByAnalyzer : ITypeDefinitionAnalyzer<IProperty>
	{
		public string Text => "Overridden By";

		public IEnumerable<IEntity> Analyze(IProperty analyzedEntity, ITypeDefinition type, AnalyzerContext context)
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

		public bool Show(IProperty entity)
		{
			return entity.IsOverridable && entity.DeclaringType.Kind != TypeKind.Interface;
		}
	}
}
