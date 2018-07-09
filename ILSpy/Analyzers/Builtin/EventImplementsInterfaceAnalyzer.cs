using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows events that implement an interface event.
	/// </summary>
	[Export(typeof(IAnalyzer<IEvent>))]
	class EventImplementsInterfaceAnalyzer : ITypeDefinitionAnalyzer<IEvent>
	{
		public string Text => "Implemented By";

		public IEnumerable<IEntity> Analyze(IEvent analyzedEntity, ITypeDefinition type, AnalyzerContext context)
		{
			var token = analyzedEntity.DeclaringTypeDefinition.MetadataToken;
			var module = analyzedEntity.DeclaringTypeDefinition.ParentAssembly.PEFile;
			if (!type.GetAllBaseTypeDefinitions()
				.Any(t => t.MetadataToken == token && t.ParentAssembly.PEFile == module))
				yield break;

			foreach (var @event in type.GetEvents(options: GetMemberOptions.ReturnMemberDefinitions)) {
				if (InheritanceHelper.GetBaseMembers(@event, true)
					.Any(m => m.DeclaringTypeDefinition.MetadataToken == token && m.ParentAssembly.PEFile == module))
					yield return @event;
			}
		}

		public bool Show(IEvent entity)
		{
			return entity.DeclaringType.Kind == TypeKind.Interface;
		}
	}
}
