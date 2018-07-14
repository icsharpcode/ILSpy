using System;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy
{
	class TypeSearchStrategy : AbstractSearchStrategy
	{
		public TypeSearchStrategy(params string[] terms)
			: base(terms)
		{
		}

		public override void Search(ITypeDefinition type, Language language, Action<SearchResult> addResult)
		{
			if (MatchName(type, language)) {
				string name = language.TypeToString(type, includeNamespace: false);
				var declaringType = type.DeclaringTypeDefinition;
				addResult(new SearchResult {
					Member = type,
					Fitness = CalculateFitness(type),
					Image = TypeTreeNode.GetIcon(type),
					Name = name,
					LocationImage = declaringType != null ? TypeTreeNode.GetIcon(declaringType) : Images.Namespace,
					Location = declaringType != null ? language.TypeToString(declaringType, includeNamespace: true) : type.Namespace
				});
			}

			foreach (var nestedType in type.NestedTypes) {
				Search(nestedType, language, addResult);
			}
		}
	}
}
