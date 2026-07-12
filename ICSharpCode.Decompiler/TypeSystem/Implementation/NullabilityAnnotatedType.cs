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

using System.Collections.Generic;
using System.Diagnostics;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// A decorator that annotates the nullability status for a type.
	/// Note: ArrayType does not use a decorator, but has direct support for nullability.
	/// </summary>
	public class NullabilityAnnotatedType : DecoratedType, IType
	{
		readonly Nullability nullability;

		internal NullabilityAnnotatedType(IType type, Nullability nullability)
			: base(type)
		{
			Debug.Assert(type.Nullability == Nullability.Oblivious);
			Debug.Assert(nullability != Nullability.Oblivious);
			// Due to IType -> concrete type casts all over the type system, we can insert
			// the NullabilityAnnotatedType wrapper only in some limited places.
			Debug.Assert(type is ITypeDefinition { IsReferenceType: not false }
				|| type.Kind == TypeKind.Dynamic
				|| type.Kind == TypeKind.Unknown
				|| (type is ITypeParameter && this is ITypeParameter));
			this.nullability = nullability;
		}

		public Nullability Nullability => nullability;

		public IType TypeWithoutAnnotation => baseType;

		public override IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitNullabilityAnnotatedType(this);
		}

		public override bool Equals(IType other)
		{
			return other is NullabilityAnnotatedType nat
				&& nat.nullability == nullability
				&& nat.baseType.Equals(baseType);
		}

		public override IType ChangeNullability(Nullability nullability)
		{
			if (nullability == this.nullability)
				return this;
			else
				return baseType.ChangeNullability(nullability);
		}

		public override IType VisitChildren(TypeVisitor visitor)
		{
			IType newBase = baseType.AcceptVisitor(visitor);
			if (newBase != baseType)
			{
				if (newBase.Nullability == Nullability.Nullable)
				{
					// `T!` with substitution T=`U?` becomes `U?`
					// This happens during type substitution for generic methods.
					return newBase;
				}
				if (newBase.Kind == TypeKind.TypeParameter || newBase.IsReferenceType == true)
				{
					return newBase.ChangeNullability(nullability);
				}
				else
				{
					// `T!` with substitution T=`int` becomes `int`, not `int!`
					return newBase;
				}
			}
			else
			{
				return this;
			}
		}

		public override string ToString()
		{
			switch (nullability)
			{
				case Nullability.Nullable:
					return $"{baseType.ToString()}?";
				case Nullability.NotNullable:
					return $"{baseType.ToString()}!";
				default:
					Debug.Assert(nullability == Nullability.Oblivious);
					return $"{baseType.ToString()}~";
			}
		}
	}

	public sealed class NullabilityAnnotatedTypeParameter : NullabilityAnnotatedType, ITypeParameter
	{
		readonly new ITypeParameter baseType;

		internal NullabilityAnnotatedTypeParameter(ITypeParameter type, Nullability nullability)
			: base(type, nullability)
		{
			this.baseType = type;
		}

		public ITypeParameter OriginalTypeParameter => baseType;

		SymbolKind ITypeParameter.OwnerType => baseType.OwnerType;
		IEntity ITypeParameter.Owner => baseType.Owner;
		int ITypeParameter.Index => baseType.Index;
		string ITypeParameter.Name => baseType.Name;
		string ISymbol.Name => baseType.Name;
		VarianceModifier ITypeParameter.Variance => baseType.Variance;
		IType ITypeParameter.EffectiveBaseClass => baseType.EffectiveBaseClass;
		IReadOnlyCollection<IType> ITypeParameter.EffectiveInterfaceSet => baseType.EffectiveInterfaceSet;
		bool ITypeParameter.HasDefaultConstructorConstraint => baseType.HasDefaultConstructorConstraint;
		bool ITypeParameter.HasReferenceTypeConstraint => baseType.HasReferenceTypeConstraint;
		bool ITypeParameter.HasValueTypeConstraint => baseType.HasValueTypeConstraint;
		bool ITypeParameter.HasUnmanagedConstraint => baseType.HasUnmanagedConstraint;
		bool ITypeParameter.AllowsRefLikeType => baseType.AllowsRefLikeType;
		Nullability ITypeParameter.NullabilityConstraint => baseType.NullabilityConstraint;
		IReadOnlyList<TypeConstraint> ITypeParameter.TypeConstraints => baseType.TypeConstraints;
		SymbolKind ISymbol.SymbolKind => SymbolKind.TypeParameter;
		IEnumerable<IAttribute> ITypeParameter.GetAttributes() => baseType.GetAttributes();
	}
}
