using System;
#if CS100
using System.Runtime.InteropServices;
#endif

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class RecordClasses
	{
		public record Base(string A);

		public record CopyCtor(string A)
		{
			protected CopyCtor(CopyCtor _)
			{
			}
		}

		public record Derived(int B) : Base(B.ToString());

		public record Empty;

		public record Fields
		{
			public int A;
			public double B = 1.0;
			public object C;
			public dynamic D;
			public string S = "abc";
		}

		public record Interface(int B) : IRecord;

		public interface IRecord
		{
		}

		public record Pair<A, B>
		{
			public A First { get; init; }
			public B Second { get; init; }
		}

		public record PairWithPrimaryCtor<A, B>(A First, B Second);

		public record PrimaryCtor(int A, string B);
		public record PrimaryCtorWithAttribute([RecordTest("param")][property: RecordTest("property")][field: RecordTest("field")] int a);
		public record PrimaryCtorWithField(int A, string B)
		{
			public double C = 1.0;
			public string D = A + B;
		}
		public record PrimaryCtorWithInParameter(in int A, in string B);
		public record PrimaryCtorWithProperty(int A, string B)
		{
			public double C { get; init; } = 1.0;
			public string D { get; } = A + B;
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

		[AttributeUsage(AttributeTargets.All)]
		public class RecordTestAttribute : Attribute
		{
			public RecordTestAttribute(string name)
			{
			}
		}

		public sealed record Sealed(string A);

		public sealed record SealedDerived(int B) : Base(B.ToString());

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


		public abstract record BaseRecord
		{
			public string Name { get; }
			public object Value { get; }
			public bool Encode { get; }

			protected BaseRecord(string name, object value, bool encode)
			{
				Name = name;
				Value = value;
				Encode = encode;
			}
		}

		public record DerivedRecord(string name, object value, bool encode = true) : BaseRecord(string.IsNullOrEmpty(name) ? "name" : name, value, encode);

		public record DefaultValuesRecord() : DerivedRecord("default", 42, encode: false);

		public record RecordWithProtectedMember(int Value)
		{
			protected int Double => Value * 2;
		}

		public record InheritedRecordWithAdditionalMember(int Value) : RecordWithProtectedMember(Value)
		{
			public int MoreData { get; set; }
		}

		public record InheritedRecordWithAdditionalParameter(int Value, int Value2) : RecordWithProtectedMember(Value);

		public record BaseWithString(string S);

		public record DerivedWithAdditionalInt(int I) : BaseWithString(I.ToString());

		public record DerivedWithNoAdditionalProperty(string S) : BaseWithString(S);

		public record DerivedWithAdditionalProperty(string S2) : BaseWithString(S2);

		public record DerivedWithAdditionalPropertyDifferentAccessor(string S) : BaseWithString(S)
		{
			public string S2 { get; set; } = S;
		}

		public record MultipleCtorsChainedNoPrimaryCtor
		{
			public int A { get; init; }
			public string B { get; init; }

			public double C { get; init; }

			public MultipleCtorsChainedNoPrimaryCtor(double c)
			{
				A = 0;
				B = null;
				C = c;
			}

			public MultipleCtorsChainedNoPrimaryCtor(int a)
				: this(3.14)
			{
				A = a;
			}

			public MultipleCtorsChainedNoPrimaryCtor(string b)
				: this(4.13)
			{
				B = b;
			}
		}

		public record UnexpectedCodeInCtor
		{
			public int A { get; init; }
			public string B { get; init; }

			public UnexpectedCodeInCtor(int A, string B)
			{
				this.A = A;
				this.B = B;
				Console.WriteLine();
			}

			public UnexpectedCodeInCtor(int A)
				: this(A, null)
			{
				this.A = A;
			}
		}
	}

#if CS100
	internal class RecordStructs
	{
		public record struct Base(string A);

		public record CopyCtor(string A)
		{
			protected CopyCtor(CopyCtor _)
			{
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public record struct Empty;

		public record struct Fields
		{
			public int A;
			public double B;
			public object C;
			public dynamic D;
			public string S;
		}

		public record struct Interface(int B) : IRecord;

		public interface IRecord
		{
		}

		public record struct Pair<A, B>
		{
			public A First { get; init; }
			public B Second { get; init; }
		}

		public record struct PairWithPrimaryCtor<A, B>(A First, B Second);

		public record struct PrimaryCtor(int A, string B);

		public record struct MultipleCtorsNoPrimaryCtor
		{
			public Guid Id { get; }

			public MultipleCtorsNoPrimaryCtor()
			{
				Id = Guid.NewGuid();
			}

			public MultipleCtorsNoPrimaryCtor(Guid id)
			{
				Id = id;
			}
		}

		public record struct MultipleCtorsChainedNoPrimaryCtor
		{
			public int A { get; init; }
			public string B { get; init; }

			public double C { get; init; }

			public MultipleCtorsChainedNoPrimaryCtor(double c)
			{
				A = 0;
				B = null;
				C = c;
			}

			public MultipleCtorsChainedNoPrimaryCtor(int a)
				: this(3.14)
			{
				A = a;
			}

			public MultipleCtorsChainedNoPrimaryCtor(string b)
				: this(4.13)
			{
				B = b;
			}
		}

		public record struct PrimaryCtorWithAttribute([RecordTest("param")][property: RecordTest("property")][field: RecordTest("field")] int a);
		public record struct PrimaryCtorWithField(int A, string B)
		{
			public double C = 1.0;
			public string D = A + B;
		}
		public record struct PrimaryCtorWithInParameter(in int A, in string B);
		public record struct PrimaryCtorWithProperty(int A, string B)
		{
			public double C { get; init; } = 1.0;
			public string D { get; } = A + B;
		}

		public record struct Properties
		{
			public int A { get; set; }
			public int B { get; }
			public int C => 43;
			public object O { get; set; }
			public string S { get; set; }
			public dynamic D { get; set; }

			public Properties()
			{
				A = 41;
				B = 42;
				O = null;
				S = "Hello";
				D = null;
			}
		}

		public record struct PropertiesWithInitializers()
		{
			public int A { get; set; } = 41;
			public int B { get; } = 42;
			public int C => 43;
			public object O { get; set; } = null;
			public string S { get; set; } = "Hello";
			public dynamic D { get; set; } = null;
		}

		[AttributeUsage(AttributeTargets.All)]
		public class RecordTestAttribute : Attribute
		{
			public RecordTestAttribute(string name)
			{
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
#if CS110
		public record struct WithRequiredMembers
		{
			public int A { get; set; }
			public required double B { get; set; }
			public object C;
			public required dynamic D;
		}
#endif
		public record struct RecordWithMultipleCtors
		{
			public int A { get; set; }

			public RecordWithMultipleCtors()
			{
				A = 42;
			}

			public RecordWithMultipleCtors(int A)
			{
				this.A = A;
			}

			public RecordWithMultipleCtors(string A)
			{
				this.A = int.Parse(A);
			}
		}
	}
#endif
}
