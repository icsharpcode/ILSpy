// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.TypeSystem
{
	/// <summary>
	/// Represents a simple C# name. (a single non-qualified identifier with an optional list of type arguments)
	/// </summary>
	[Serializable]
	public sealed class SimpleTypeOrNamespaceReference : TypeOrNamespaceReference, ISupportsInterning
	{
		public SimpleTypeOrNamespaceReference(string identifier, IList<ITypeReference> typeArguments, NameLookupMode lookupMode = NameLookupMode.Type)
		{
			this.Identifier = identifier ?? throw new ArgumentNullException("identifier");
			this.TypeArguments = typeArguments ?? EmptyList<ITypeReference>.Instance;
			this.LookupMode = lookupMode;
		}
		
		public string Identifier { get; }

		public IList<ITypeReference> TypeArguments { get; }

		public NameLookupMode LookupMode { get; }

		/// <summary>
		/// Adds a suffix to the identifier.
		/// Does not modify the existing type reference, but returns a new one.
		/// </summary>
		public SimpleTypeOrNamespaceReference AddSuffix(string suffix)
		{
			return new SimpleTypeOrNamespaceReference(Identifier + suffix, TypeArguments, LookupMode);
		}
		
		public override ResolveResult Resolve(CSharpResolver resolver)
		{
			var typeArgs = TypeArguments.Resolve(resolver.CurrentTypeResolveContext);
			return resolver.LookupSimpleNameOrTypeName(Identifier, typeArgs, LookupMode);
		}
		
		public override IType ResolveType(CSharpResolver resolver)
		{
			var trr = Resolve(resolver) as TypeResolveResult;
			return trr != null ? trr.Type : new UnknownType(null, Identifier, TypeArguments.Count);
		}
		
		public override string ToString()
		{
			if (TypeArguments.Count == 0)
				return Identifier;
			else
				return Identifier + "<" + string.Join(",", TypeArguments) + ">";
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			var hashCode = 0;
			unchecked {
				hashCode += 1000000021 * Identifier.GetHashCode();
				hashCode += 1000000033 * TypeArguments.GetHashCode();
				hashCode += 1000000087 * (int)LookupMode;
			}
			return hashCode;
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			var o = other as SimpleTypeOrNamespaceReference;
			return o != null && this.Identifier == o.Identifier
				&& this.TypeArguments == o.TypeArguments && this.LookupMode == o.LookupMode;
		}
	}
}
