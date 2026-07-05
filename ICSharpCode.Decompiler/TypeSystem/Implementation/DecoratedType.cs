// Copyright (c) 2019 Daniel Grunwald
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
using System.Text;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	public abstract class DecoratedType : IType
	{
		protected readonly IType baseType;

		protected DecoratedType(IType baseType)
		{
			this.baseType = baseType;
		}

		TypeKind IType.Kind => baseType.Kind;

		bool? IType.IsReferenceType => baseType.IsReferenceType;

		bool IType.IsByRefLike => baseType.IsByRefLike;

		Nullability IType.Nullability => baseType.Nullability;
		public abstract IType ChangeNullability(Nullability nullability);

		IType IType.DeclaringType => baseType.DeclaringType;

		int IType.TypeParameterCount => baseType.TypeParameterCount;

		IReadOnlyList<ITypeParameter> IType.TypeParameters => baseType.TypeParameters;

		IReadOnlyList<IType> IType.TypeArguments => baseType.TypeArguments;

		IEnumerable<IType> IType.DirectBaseTypes => baseType.DirectBaseTypes;

		string INamedElement.FullName => baseType.FullName;

		string INamedElement.Name => baseType.Name;

		string INamedElement.ReflectionName => baseType.ReflectionName;

		string INamedElement.Namespace => baseType.Namespace;

		public abstract IType AcceptVisitor(TypeVisitor visitor);

		public abstract bool Equals(IType other);

		IEnumerable<IMethod> IType.GetAccessors(Predicate<IMethod> filter, GetMemberOptions options)
		{
			return baseType.GetAccessors(filter, options);
		}

		IEnumerable<IMethod> IType.GetConstructors(Predicate<IMethod> filter, GetMemberOptions options)
		{
			return baseType.GetConstructors(filter, options);
		}

		ITypeDefinition IType.GetDefinition()
		{
			return baseType.GetDefinition();
		}

		ITypeDefinitionOrUnknown IType.GetDefinitionOrUnknown()
		{
			return baseType.GetDefinitionOrUnknown();
		}

		IEnumerable<IEvent> IType.GetEvents(Predicate<IEvent> filter, GetMemberOptions options)
		{
			return baseType.GetEvents(filter, options);
		}

		IEnumerable<IField> IType.GetFields(Predicate<IField> filter, GetMemberOptions options)
		{
			return baseType.GetFields(filter, options);
		}

		IEnumerable<IMember> IType.GetMembers(Predicate<IMember> filter, GetMemberOptions options)
		{
			return baseType.GetMembers(filter, options);
		}

		IEnumerable<IMethod> IType.GetMethods(Predicate<IMethod> filter, GetMemberOptions options)
		{
			return baseType.GetMethods(filter, options);
		}

		IEnumerable<IMethod> IType.GetMethods(IReadOnlyList<IType> typeArguments, Predicate<IMethod> filter, GetMemberOptions options)
		{
			return baseType.GetMethods(typeArguments, filter, options);
		}

		IEnumerable<IType> IType.GetNestedTypes(Predicate<ITypeDefinition> filter, GetMemberOptions options)
		{
			return baseType.GetNestedTypes(filter, options);
		}

		IEnumerable<IType> IType.GetNestedTypes(IReadOnlyList<IType> typeArguments, Predicate<ITypeDefinition> filter, GetMemberOptions options)
		{
			return baseType.GetNestedTypes(typeArguments, filter, options);
		}

		IEnumerable<IProperty> IType.GetProperties(Predicate<IProperty> filter, GetMemberOptions options)
		{
			return baseType.GetProperties(filter, options);
		}

		TypeParameterSubstitution IType.GetSubstitution()
		{
			return baseType.GetSubstitution();
		}

		public abstract IType VisitChildren(TypeVisitor visitor);
	}
}
