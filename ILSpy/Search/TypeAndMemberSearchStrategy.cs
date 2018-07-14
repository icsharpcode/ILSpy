using System;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy
{
	class TypeAndMemberSearchStrategy : AbstractSearchStrategy
	{
		public TypeAndMemberSearchStrategy(params string[] terms)
			: base(terms)
		{
		}

		public override void Search(ITypeDefinition type, Language language, Action<SearchResult> addResult)
		{
			if (MatchName(type, language))
			{
				string name = language.TypeToString(type, includeNamespace: false);
				var declaringType = type.DeclaringTypeDefinition;
				addResult(new SearchResult {
					Member = type,
					Image = TypeTreeNode.GetIcon(type),
					Fitness = CalculateFitness(type),
					Name = name,
					LocationImage = declaringType != null ? TypeTreeNode.GetIcon(declaringType) : Images.Namespace,
					Location = declaringType != null ? language.TypeToString(declaringType, includeNamespace: true) : type.Namespace
				});
			}

			base.Search(type, language, addResult);
		}

		protected override bool IsMatch(IField field, Language language)
		{
			return MatchName(field, language);
		}

		protected override bool IsMatch(IProperty property, Language language)
		{
			return MatchName(property, language);
		}

		protected override bool IsMatch(IEvent ev, Language language)
		{
			return MatchName(ev, language);
		}

		protected override bool IsMatch(IMethod m, Language language)
		{
			return MatchName(m, language);
		}
	}
}
