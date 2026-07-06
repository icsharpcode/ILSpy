using System.Text;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class SealedRecordToString
	{
		public abstract record AbstractBase(int X);

		public record Middle(int X, int Y) : AbstractBase(X)
		{
			public sealed override string ToString()
			{
				return "middle";
			}
		}

		public record Leaf(int X, int Y, int Z) : Middle(X, Y);

		public record WithSealedToString(int A)
		{
			public sealed override string ToString()
			{
				return "custom";
			}
		}

		public record DerivedAddsMembers(int A, string B) : WithSealedToString(A);

		public record DerivedAddsNoMembers(int A) : WithSealedToString(A);

		public record GenericDerived<T>(int A, T Extra) : WithSealedToString(A);

		public record WithNonSealedToString(int A)
		{
			public override string ToString()
			{
				return "custom";
			}
		}

		public record DerivedFromNonSealed(int A, string B) : WithNonSealedToString(A);

		public record WithSealedToStringAndPrintMembers(int A)
		{
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				builder.Append("CustomA = ");
				builder.Append(A);
				return true;
			}

			public sealed override string ToString()
			{
				return "custom";
			}
		}

		public record EquivalentPrintMembers(int A)
		{
			// A user-written PrintMembers whose body is IL-equivalent to the
			// compiler-generated one is folded into the synthesized record members
			// and therefore not printed. Recompilation regenerates an identical
			// PrintMembers, so semantics are preserved.
#if !EXPECTED_OUTPUT
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				builder.Append("A = ");
				builder.Append(A);
				return true;
			}
#endif

			public sealed override string ToString()
			{
				return "custom";
			}
		}

		public record MimicsGeneratedToString(int A)
		{
			public sealed override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("MimicsGeneratedToString");
				stringBuilder.Append(" { ");
				if (PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}
		}

		public sealed record SealedRecordWithSealedToString(int A)
		{
			public sealed override string ToString()
			{
				return "sealed";
			}
		}

		public record GenericWithSealedToString<T>(T Value)
		{
			public sealed override string ToString()
			{
				return typeof(T).Name;
			}
		}

		public record struct StructWithToString(int A)
		{
			public override string ToString()
			{
				return "structcustom";
			}
		}
	}
}
