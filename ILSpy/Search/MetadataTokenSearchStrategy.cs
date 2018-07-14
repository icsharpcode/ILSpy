using System.Globalization;
using SRM = System.Reflection.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy
{
	class MetadataTokenSearchStrategy : TypeAndMemberSearchStrategy
	{
		readonly int searchTermToken;

		public MetadataTokenSearchStrategy(params string[] terms)
			: base(terms)
		{
			if (searchTerm.Length == 1) {
				int.TryParse(searchTerm[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out searchTermToken);
			}
		}

		protected override bool MatchName(IEntity m, Language language)
		{
			return SRM.Ecma335.MetadataTokens.GetToken(m.MetadataToken) == searchTermToken;
		}
	}
}
