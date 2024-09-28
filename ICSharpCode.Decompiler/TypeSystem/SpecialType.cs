﻿// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Contains static implementations of special types.
	/// </summary>
	[Serializable]
	public sealed class SpecialType : AbstractType, ITypeReference
	{
		/// <summary>
		/// Gets the type representing resolve errors.
		/// </summary>
		public readonly static SpecialType UnknownType = new SpecialType(TypeKind.Unknown, "?", isReferenceType: null);

		/// <summary>
		/// The null type is used as type of the null literal. It is a reference type without any members; and it is a subtype of all reference types.
		/// </summary>
		public readonly static SpecialType NullType = new SpecialType(TypeKind.Null, "null", isReferenceType: true);

		/// <summary>
		/// Used for expressions without type, e.g. method groups or lambdas.
		/// </summary>
		public readonly static SpecialType NoType = new SpecialType(TypeKind.None, "?", isReferenceType: null);

		/// <summary>
		/// Type representing the C# 'dynamic' type.
		/// </summary>
		public readonly static SpecialType Dynamic = new SpecialType(TypeKind.Dynamic, "dynamic", isReferenceType: true);

		/// <summary>
		/// Type representing the C# 9 'nint' type.
		/// </summary>
		public readonly static SpecialType NInt = new SpecialType(TypeKind.NInt, "nint", isReferenceType: false);

		/// <summary>
		/// Type representing the C# 9 'nuint' type.
		/// </summary>
		public readonly static SpecialType NUInt = new SpecialType(TypeKind.NUInt, "nuint", isReferenceType: false);

		/// <summary>
		/// Type representing the result of the C# '__arglist()' expression.
		/// </summary>
		public readonly static SpecialType ArgList = new SpecialType(TypeKind.ArgList, "__arglist", isReferenceType: null);

		/// <summary>
		/// A type used for unbound type arguments in partially parameterized types.
		/// </summary>
		/// <see cref="IType.GetNestedTypes(Predicate{ITypeDefinition}, GetMemberOptions)"/>
		public readonly static SpecialType UnboundTypeArgument = new SpecialType(TypeKind.UnboundTypeArgument, "", isReferenceType: null);

		readonly TypeKind kind;
		readonly string name;
		readonly bool? isReferenceType;

		private SpecialType(TypeKind kind, string name, bool? isReferenceType)
		{
			this.kind = kind;
			this.name = name;
			this.isReferenceType = isReferenceType;
		}

		public override string Name {
			get { return name; }
		}

		public override TypeKind Kind {
			get { return kind; }
		}

		public override bool? IsReferenceType {
			get { return isReferenceType; }
		}

		IType ITypeReference.Resolve(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			return this;
		}

#pragma warning disable 809
		[Obsolete("Please compare special types using the kind property instead.")]
		public override bool Equals(IType? other)
		{
			// We consider a special types equal when they have equal types.
			// However, an unknown type with additional information is not considered to be equal to the SpecialType with TypeKind.Unknown.
			return other is SpecialType && other.Kind == kind;
		}

		public override int GetHashCode()
		{
			return 81625621 ^ (int)kind;
		}

		public override IType ChangeNullability(Nullability nullability)
		{
			if (nullability == base.Nullability || Kind is not TypeKind.Dynamic)
				return this;
			else
				return new NullabilityAnnotatedType(this, nullability);
		}
	}
}
