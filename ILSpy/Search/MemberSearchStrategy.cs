using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy
{
	class MemberSearchStrategy : AbstractSearchStrategy
	{
		MemberSearchKind searchKind;

		public MemberSearchStrategy(string term, MemberSearchKind searchKind = MemberSearchKind.All)
			: this(new[] { term }, searchKind)
		{
		}

		public MemberSearchStrategy(string[] terms, MemberSearchKind searchKind = MemberSearchKind.All)
			: base(terms)
		{
			this.searchKind = searchKind;
		}

		protected override bool IsMatch(IField field, Language language)
		{
			return (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Field) && MatchName(field, language);
		}

		protected override bool IsMatch(IProperty property, Language language)
		{
			return (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Property) && MatchName(property, language);
		}

		protected override bool IsMatch(IEvent ev, Language language)
		{
			return (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Event) && MatchName(ev, language);
		}

		protected override bool IsMatch(IMethod m, Language language)
		{
			return (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Method) && MatchName(m, language);
		}
	}

	enum MemberSearchKind
	{
		All,
		Field,
		Property,
		Event,
		Method
	}
}
