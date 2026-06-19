using Microsoft.CodeAnalysis;

namespace ICSharpCode.Decompiler.Generators;

public static class RoslynHelpers
{
	public static bool IsDerivedFrom(this INamedTypeSymbol type, INamedTypeSymbol baseType)
	{
		INamedTypeSymbol? t = type;

		while (t != null)
		{
			if (SymbolEqualityComparer.Default.Equals(t, baseType))
				return true;

			t = t.BaseType;
		}

		return false;
	}
}
