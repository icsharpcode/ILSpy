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
	/// Shows methods that implement an interface method.
	/// </summary>
	[Export(typeof(IAnalyzer<IMethod>))]
	class MethodImplementsInterfaceAnalyzer : ITypeDefinitionAnalyzer<IMethod>
	{
		public string Text => "Implemented By";

		public IEnumerable<IEntity> Analyze(IMethod analyzedEntity, ITypeDefinition type, AnalyzerContext context)
		{
			var token = analyzedEntity.DeclaringTypeDefinition.MetadataToken;
			var module = analyzedEntity.DeclaringTypeDefinition.ParentAssembly.PEFile;
			if (!type.GetAllBaseTypeDefinitions()
				.Any(t => t.MetadataToken == token && t.ParentAssembly.PEFile == module))
				yield break;

			foreach (var method in type.GetMethods(options: GetMemberOptions.ReturnMemberDefinitions)) {
				if (InheritanceHelper.GetBaseMembers(method, true)
					.Any(m => m.DeclaringTypeDefinition.MetadataToken == token && m.ParentAssembly.PEFile == module))
					yield return method;
			}
		}

		public bool Show(IMethod entity)
		{
			return entity.DeclaringType.Kind == TypeKind.Interface;
		}
	}
}
