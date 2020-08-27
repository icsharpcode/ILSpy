using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler
{
	internal class DecompileRun
	{
		public HashSet<string> DefinedSymbols { get; private set; } = new HashSet<string>();
		public HashSet<string> Namespaces { get; private set; } = new HashSet<string>();
		public CancellationToken CancellationToken { get; set; }
		public DecompilerSettings Settings { get; }
		public IDocumentationProvider DocumentationProvider { get; set; }

		Lazy<CSharp.TypeSystem.UsingScope> usingScope =>
			new Lazy<CSharp.TypeSystem.UsingScope>(() => CreateUsingScope(Namespaces));

		public CSharp.TypeSystem.UsingScope UsingScope => usingScope.Value;

		public DecompileRun(DecompilerSettings settings)
		{
			this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
		}

		CSharp.TypeSystem.UsingScope CreateUsingScope(HashSet<string> requiredNamespacesSuperset)
		{
			var usingScope = new CSharp.TypeSystem.UsingScope();
			foreach (var ns in requiredNamespacesSuperset)
			{
				string[] parts = ns.Split('.');
				AstType nsType = new SimpleType(parts[0]);
				for (int i = 1; i < parts.Length; i++)
				{
					nsType = new MemberType { Target = nsType, MemberName = parts[i] };
				}
				var reference = nsType.ToTypeReference(CSharp.Resolver.NameLookupMode.TypeInUsingDeclaration)
					as CSharp.TypeSystem.TypeOrNamespaceReference;
				if (reference != null)
					usingScope.Usings.Add(reference);
			}
			return usingScope;
		}
	}
}
