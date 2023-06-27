// Copyright (c) 2018 AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Represents a modopt or modreq type.
	/// </summary>
	public class ModifiedType : TypeWithElementType, IType
	{
		readonly TypeKind kind;
		readonly IType modifier;

		public ModifiedType(IType modifier, IType unmodifiedType, bool isRequired) : base(unmodifiedType)
		{
			this.kind = isRequired ? TypeKind.ModReq : TypeKind.ModOpt;
			this.modifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
		}

		public IType Modifier => modifier;
		public override TypeKind Kind => kind;

		public override string NameSuffix => (kind == TypeKind.ModReq ? " modreq" : " modopt") + $"({modifier.FullName})";

		public override bool? IsReferenceType => elementType.IsReferenceType;
		public override bool IsByRefLike => elementType.IsByRefLike;
		public override Nullability Nullability => elementType.Nullability;

		public override IType ChangeNullability(Nullability nullability)
		{
			IType newElementType = elementType.ChangeNullability(nullability);
			if (newElementType == elementType)
				return this;
			else
				return new ModifiedType(modifier, newElementType, kind == TypeKind.ModReq);
		}

		public override ITypeDefinition GetDefinition()
		{
			return elementType.GetDefinition();
		}

		public override ITypeDefinitionOrUnknown GetDefinitionOrUnknown()
		{
			return elementType.GetDefinitionOrUnknown();
		}

		public override IEnumerable<IMethod> GetAccessors(Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return elementType.GetAccessors(filter, options);
		}

		public override IEnumerable<IMethod> GetConstructors(Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.IgnoreInheritedMembers)
		{
			return elementType.GetConstructors(filter, options);
		}

		public override IEnumerable<IEvent> GetEvents(Predicate<IEvent> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return elementType.GetEvents(filter, options);
		}

		public override IEnumerable<IField> GetFields(Predicate<IField> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return elementType.GetFields(filter, options);
		}

		public override IEnumerable<IMember> GetMembers(Predicate<IMember> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return elementType.GetMembers(filter, options);
		}

		public override IEnumerable<IMethod> GetMethods(IReadOnlyList<IType> typeArguments, Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return elementType.GetMethods(typeArguments, filter, options);
		}

		public override IEnumerable<IMethod> GetMethods(Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return elementType.GetMethods(filter, options);
		}

		public override IEnumerable<IType> GetNestedTypes(IReadOnlyList<IType> typeArguments, Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return elementType.GetNestedTypes(typeArguments, filter, options);
		}

		public override IEnumerable<IType> GetNestedTypes(Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return elementType.GetNestedTypes(filter, options);
		}

		public override IEnumerable<IProperty> GetProperties(Predicate<IProperty> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return elementType.GetProperties(filter, options);
		}

		public override IType VisitChildren(TypeVisitor visitor)
		{
			var newElementType = elementType.AcceptVisitor(visitor);
			var newModifier = modifier.AcceptVisitor(visitor);
			if (newModifier != modifier || newElementType != elementType)
			{
				return new ModifiedType(newModifier, newElementType, kind == TypeKind.ModReq);
			}
			return this;
		}

		public override IType AcceptVisitor(TypeVisitor visitor)
		{
			if (kind == TypeKind.ModReq)
				return visitor.VisitModReq(this);
			else
				return visitor.VisitModOpt(this);
		}

		public override bool Equals(IType other)
		{
			return other is ModifiedType o && kind == o.kind && modifier.Equals(o.modifier) && elementType.Equals(o.elementType);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (int)kind ^ (elementType.GetHashCode() * 1344795899) ^ (modifier.GetHashCode() * 901375117);
			}
		}
	}
}
