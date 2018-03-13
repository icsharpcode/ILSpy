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
	/// Reference to a qualified type or namespace name.
	/// </summary>
	[Serializable]
	public sealed class MemberTypeOrNamespaceReference : TypeOrNamespaceReference, ISupportsInterning
	{
		public MemberTypeOrNamespaceReference(TypeOrNamespaceReference target, string identifier, IList<ITypeReference> typeArguments, NameLookupMode lookupMode = NameLookupMode.Type)
		{
			this.Target = target ?? throw new ArgumentNullException("target");
			this.Identifier = identifier ?? throw new ArgumentNullException("identifier");
			this.TypeArguments = typeArguments ?? EmptyList<ITypeReference>.Instance;
			this.LookupMode = lookupMode;
		}
		
		public string Identifier { get; }

		public TypeOrNamespaceReference Target { get; }

		public IList<ITypeReference> TypeArguments { get; }

		public NameLookupMode LookupMode { get; }

		/// <summary>
		/// Adds a suffix to the identifier.
		/// Does not modify the existing type reference, but returns a new one.
		/// </summary>
		public MemberTypeOrNamespaceReference AddSuffix(string suffix)
		{
			return new MemberTypeOrNamespaceReference(Target, Identifier + suffix, TypeArguments, LookupMode);
		}
		
		public override ResolveResult Resolve(CSharpResolver resolver)
		{
			var targetRR = Target.Resolve(resolver);
			if (targetRR.IsError)
				return targetRR;
			var typeArgs = TypeArguments.Resolve(resolver.CurrentTypeResolveContext);
			return resolver.ResolveMemberAccess(targetRR, Identifier, typeArgs, LookupMode);
		}
		
		public override IType ResolveType(CSharpResolver resolver)
		{
			var trr = Resolve(resolver) as TypeResolveResult;
			return trr != null ? trr.Type : new UnknownType(null, Identifier, TypeArguments.Count);
		}
		
		public override string ToString()
		{
			if (TypeArguments.Count == 0)
				return Target.ToString() + "." + Identifier;
			else
				return Target.ToString() + "." + Identifier + "<" + string.Join(",", TypeArguments) + ">";
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			var hashCode = 0;
			unchecked {
				hashCode += 1000000007 * Target.GetHashCode();
				hashCode += 1000000033 * Identifier.GetHashCode();
				hashCode += 1000000087 * TypeArguments.GetHashCode();
				hashCode += 1000000021 * (int)LookupMode;
			}
			return hashCode;
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			var o = other as MemberTypeOrNamespaceReference;
			return o != null && this.Target == o.Target
				&& this.Identifier == o.Identifier && this.TypeArguments == o.TypeArguments
				&& this.LookupMode == o.LookupMode;
		}
	}
}
