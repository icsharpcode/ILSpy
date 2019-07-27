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
			Debug.Assert(nullability != type.Nullability);
			// Due to IType -> concrete type casts all over the type system, we can insert
			// the NullabilityAnnotatedType wrapper only in some limited places.
			Debug.Assert(type is ITypeDefinition
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
			if (newBase != baseType) {
				if (newBase.Nullability == Nullability.Nullable) {
					// `T!` with substitution T=`U?` becomes `U?`
					// This happens during type substitution for generic methods.
					return newBase;
				}
				if (newBase.Kind == TypeKind.TypeParameter || newBase.IsReferenceType == true) {
					return newBase.ChangeNullability(nullability);
				} else {
					// `T!` with substitution T=`int` becomes `int`, not `int!`
					return newBase;
				}
			} else {
				return this;
			}
		}

		public override string ToString()
		{
			switch (nullability) {
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
		Nullability ITypeParameter.NullabilityConstraint => baseType.NullabilityConstraint;
		SymbolKind ISymbol.SymbolKind => SymbolKind.TypeParameter;
		IEnumerable<IAttribute> ITypeParameter.GetAttributes() => baseType.GetAttributes();
	}
}
