using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Library1;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public record Issue4295 : BaseRecord
	{
		public int B {
			[CompilerGenerated]
			get {
				return B__BackingField;
			}
			[CompilerGenerated]
			init {
				B__BackingField = value;
			}
		}

		public Issue4295(int A, int B)
		{
			this.B__BackingField = B;
			((BaseRecord)this)._002Ector(A);
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Issue4295");
			stringBuilder.Append(" { ");
			if (((BaseRecord)this).PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		protected override bool PrintMembers(StringBuilder builder)
		{
			if (((BaseRecord)this).PrintMembers(builder))
			{
				builder.Append(", ");
			}
			builder.Append("B = ");
			builder.Append(B.ToString());
			return true;
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((BaseRecord)this).GetHashCode() * -1521134295 + EqualityComparer<int>.Default.GetHashCode(B__BackingField);
		}

		[CompilerGenerated]
		public sealed override bool Equals(BaseRecord? other)
		{
			return ((object)this).Equals((object)other);
		}

		[CompilerGenerated]
		public virtual bool Equals(Issue4295? other)
		{
			if ((object)this != other)
			{
				if (((BaseRecord)this).Equals((BaseRecord)(object)other))
				{
					return EqualityComparer<int>.Default.Equals(B__BackingField, other.B__BackingField);
				}
				return false;
			}
			return true;
		}

		[CompilerGenerated]
		protected Issue4295(Issue4295 original)
			: base((BaseRecord)(object)original)
		{
			B__BackingField = original.B__BackingField;
		}

		[CompilerGenerated]
		public void Deconstruct(out int A, out int B)
		{
			A = ((BaseRecord)this).A;
			B = this.B;
		}

		[CompilerGenerated]
		private readonly int B__BackingField;
	}
}
