namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public record Empty
	{
	}

	public record Fields
	{
		public int A;
		public double B = 1.0;
		public object C;
		public dynamic D;
		public string S = "abc";
	}

	public record Pair<A, B>
	{
		public A First { get; init; }
		public B Second { get; init; }
	}

	public record Properties
	{
		public int A { get; set; }
		public int B { get; }
		public int C => 43;
		public object O { get; set; }
		public string S { get; set; }
		public dynamic D { get; set; }

		public Properties()
		{
			B = 42;
		}
	}

	public class WithExpressionTests
	{
		public Fields Test(Fields input)
		{
			return input with {
				A = 42,
				B = 3.141,
				C = input
			};
		}
		public Fields Test2(Fields input)
		{
			return input with {
				A = 42,
				B = 3.141,
				C = input with {
					A = 43
				}
			};
		}
	}

	public abstract record WithNestedRecords
	{
		public record A : WithNestedRecords
		{
			public override string AbstractProp => "A";
		}

		public record B : WithNestedRecords
		{
			public override string AbstractProp => "B";

			public int? Value { get; set; }
		}

		public record DerivedGeneric<T> : Pair<T, T?> where T : struct
		{
			public bool Flag;
		}

		public abstract string AbstractProp { get; }
	}
}
namespace System.Runtime.CompilerServices
{
	internal class IsExternalInit
	{
	}
}
