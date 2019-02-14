using System.Diagnostics;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	public sealed class NullabilityAnnotatedType : DecoratedType, IType
	{
		readonly Nullability nullability;

		internal NullabilityAnnotatedType(IType type, Nullability nullability)
			: base(type)
		{
			Debug.Assert(nullability != Nullability.Oblivious);
			Debug.Assert(!(type is ParameterizedType || type is NullabilityAnnotatedType));
			this.nullability = nullability;
		}

		public Nullability Nullability => nullability;

		public IType ElementType => baseType;

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
				return new NullabilityAnnotatedType(newBase, nullability);
			else
				return this;
		}

		public override string ToString()
		{
			return baseType.ToString() + (nullability == Nullability.Nullable ? "?" : "!");
		}
	}
}
