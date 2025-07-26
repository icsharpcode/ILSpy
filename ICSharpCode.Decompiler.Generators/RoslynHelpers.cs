using Microsoft.CodeAnalysis;

namespace ICSharpCode.Decompiler.Generators;

public static class RoslynHelpers
{
	public static IEnumerable<INamedTypeSymbol> GetTopLevelTypes(this IAssemblySymbol assembly)
	{
		foreach (var ns in TreeTraversal.PreOrder(assembly.GlobalNamespace, ns => ns.GetNamespaceMembers()))
		{
			foreach (var t in ns.GetTypeMembers())
			{
				yield return t;
			}
		}
	}

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
