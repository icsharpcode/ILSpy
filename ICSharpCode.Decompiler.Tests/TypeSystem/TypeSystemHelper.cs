using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.Tests.TypeSystem
{
	public static class TypeSystemHelper
	{
		public static ICompilation CreateCompilation(params IUnresolvedTypeDefinition[] unresolvedTypeDefinitions)
		{
			var unresolvedAsm = new DefaultUnresolvedAssembly("dummy");
			foreach (var typeDef in unresolvedTypeDefinitions)
				unresolvedAsm.AddTypeDefinition(typeDef);
			return new SimpleCompilation(unresolvedAsm, TypeSystemLoaderTests.Mscorlib, TypeSystemLoaderTests.SystemCore);
		}
	}
}
