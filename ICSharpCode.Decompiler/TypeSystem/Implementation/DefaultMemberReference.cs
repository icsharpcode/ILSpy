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
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// References an entity by its type and name.
	/// This class can be used to refer to all members except for constructors and explicit interface implementations.
	/// </summary>
	/// <remarks>
	/// Resolving a DefaultMemberReference requires a context that provides enough information for resolving the declaring type reference
	/// and the parameter types references.
	/// </remarks>
	[Serializable]
	public sealed class DefaultMemberReference : IMemberReference, ISupportsInterning
	{
		readonly SymbolKind symbolKind;
		readonly string name;
		readonly int typeParameterCount;
		readonly IList<ITypeReference> parameterTypes;
		
		public DefaultMemberReference(SymbolKind symbolKind, ITypeReference typeReference, string name, int typeParameterCount = 0, IList<ITypeReference> parameterTypes = null)
		{
			if (typeParameterCount != 0 && symbolKind != SymbolKind.Method)
				throw new ArgumentException("Type parameter count > 0 is only supported for methods.");
			this.symbolKind = symbolKind;
			this.DeclaringTypeReference = typeReference ?? throw new ArgumentNullException("typeReference");
			this.name = name ?? throw new ArgumentNullException("name");
			this.typeParameterCount = typeParameterCount;
			this.parameterTypes = parameterTypes ?? EmptyList<ITypeReference>.Instance;
		}
		
		public ITypeReference DeclaringTypeReference { get; }

		public IMember Resolve(ITypeResolveContext context)
		{
			var type = DeclaringTypeReference.Resolve(context);
			IEnumerable<IMember> members;
			if (symbolKind == SymbolKind.Accessor) {
				members = type.GetAccessors(
					m => m.Name == name && !m.IsExplicitInterfaceImplementation,
					GetMemberOptions.IgnoreInheritedMembers);
			} else if (symbolKind == SymbolKind.Method) {
				members = type.GetMethods(
					m => m.Name == name && m.SymbolKind == SymbolKind.Method
					&& m.TypeParameters.Count == typeParameterCount && !m.IsExplicitInterfaceImplementation,
					GetMemberOptions.IgnoreInheritedMembers);
			} else {
				members = type.GetMembers(
					m => m.Name == name && m.SymbolKind == symbolKind && !m.IsExplicitInterfaceImplementation,
					GetMemberOptions.IgnoreInheritedMembers);
			}
			var resolvedParameterTypes = parameterTypes.Resolve(context);
			foreach (var member in members) {
				var parameterizedMember = member as IParameterizedMember;
				if (parameterizedMember == null) {
					if (parameterTypes.Count == 0)
						return member;
				} else if (parameterTypes.Count == parameterizedMember.Parameters.Count) {
					var signatureMatches = true;
					for (var i = 0; i < parameterTypes.Count; i++) {
						var type1 = DummyTypeParameter.NormalizeAllTypeParameters(resolvedParameterTypes[i]);
						var type2 = DummyTypeParameter.NormalizeAllTypeParameters(parameterizedMember.Parameters[i].Type);
						if (!type1.Equals(type2)) {
							signatureMatches = false;
							break;
						}
					}
					if (signatureMatches)
						return member;
				}
			}
			return null;
		}
		
		ISymbol ISymbolReference.Resolve(ITypeResolveContext context)
		{
			return ((IMemberReference)this).Resolve(context);
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			return (int)symbolKind ^ DeclaringTypeReference.GetHashCode() ^ name.GetHashCode() ^ parameterTypes.GetHashCode();
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			var o = other as DefaultMemberReference;
			return o != null && symbolKind == o.symbolKind && DeclaringTypeReference == o.DeclaringTypeReference && name == o.name && parameterTypes == o.parameterTypes;
		}
	}
}
