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
