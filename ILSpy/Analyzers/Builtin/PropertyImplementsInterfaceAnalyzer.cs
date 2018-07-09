using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows properties that implement an interface property.
	/// </summary>
	[Export(typeof(IAnalyzer<IProperty>))]
	class PropertyImplementsInterfaceAnalyzer : ITypeDefinitionAnalyzer<IProperty>
	{
		public string Text => "Implemented By";

		public IEnumerable<IEntity> Analyze(IProperty analyzedEntity, ITypeDefinition type, AnalyzerContext context)
		{
			var token = analyzedEntity.DeclaringTypeDefinition.MetadataToken;
			var module = analyzedEntity.DeclaringTypeDefinition.ParentAssembly.PEFile;
			if (!type.GetAllBaseTypeDefinitions()
				.Any(t => t.MetadataToken == token && t.ParentAssembly.PEFile == module))
				yield break;

			foreach (var property in type.GetProperties(options: GetMemberOptions.ReturnMemberDefinitions)) {
				if (InheritanceHelper.GetBaseMembers(property, true)
					.Any(m => m.DeclaringTypeDefinition.MetadataToken == token && m.ParentAssembly.PEFile == module))
					yield return property;
			}
		}

		public bool Show(IProperty entity)
		{
			return entity.DeclaringType.Kind == TypeKind.Interface;
		}
	}
}
