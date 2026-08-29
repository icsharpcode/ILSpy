// Copyright (c) 2026 Siegfried Pammer
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

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class UserDefinedCompoundAssignment
	{
		static void Main()
		{
			InstanceOperator();
			InheritedOperator();
			ShadowedOperator();
			StaticOperatorStaysStatic();
			StaticOperatorOnRefLocalStaysStatic();
			StaticTypeSelectsTheOperator();
			IncrementDependsOnWhetherTheResultIsUsed();
			StructOperator();
			GenericOperator();
			ForeachTarget();
			UsingTarget();
			InParameterTarget(new Base { Value = 40 });
			ReadonlyFieldTarget();
		}

		class Base
		{
			public int Value;

			public void operator +=(int rhs)
			{
				Console.WriteLine("Base.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}

			public void operator ++()
			{
				Console.WriteLine("Base.op_IncrementAssignment()");
				Value++;
			}
		}

		class Derived : Base
		{
		}

		// Declares the same signature as Base, so "s += n" must bind here and not to Base.
		class Shadowing : Base
		{
			public new void operator +=(int rhs)
			{
				Console.WriteLine("Shadowing.op_AdditionAssignment({0})", rhs);
				Value += rhs * 100;
			}
		}

		// Declaring both forms means "b = b + n" and "b += n" do different things: the static
		// operator produces a new instance, the instance one mutates the receiver.
		class Both
		{
			public int Value;

			public static Both operator +(Both lhs, int rhs)
			{
				Console.WriteLine("Both.op_Addition({0})", rhs);
				return new Both { Value = lhs.Value + rhs };
			}

			public void operator +=(int rhs)
			{
				Console.WriteLine("Both.op_AdditionAssignment({0})", rhs);
				Value += rhs * 1000;
			}

			public static Both operator ++(Both x)
			{
				Console.WriteLine("Both.op_Increment()");
				return new Both { Value = x.Value + 1 };
			}

			public void operator ++()
			{
				Console.WriteLine("Both.op_IncrementAssignment()");
				Value += 1000;
			}
		}

		// Only the derived type declares an instance operator. Which operator "x += y" binds to
		// therefore depends on the declared type of x, not on the runtime type of the object.
		class StaticBase
		{
			public int Value;

			public static StaticBase operator +(StaticBase lhs, int rhs)
			{
				Console.WriteLine("StaticBase.op_Addition({0})", rhs);
				return new StaticBase { Value = lhs.Value + rhs };
			}
		}

		class InstanceDerived : StaticBase
		{
			public void operator +=(int rhs)
			{
				Console.WriteLine("InstanceDerived.op_AdditionAssignment({0})", rhs);
				Value += rhs * 10;
			}
		}

		struct Counter
		{
			public int Value;

			public void operator +=(int rhs)
			{
				Console.WriteLine("Counter.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		interface ICompound
		{
			void operator +=(int rhs);
		}

		class ViaInterface : ICompound
		{
			public int Value;

			public void operator +=(int rhs)
			{
				Console.WriteLine("ViaInterface.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		static void InstanceOperator()
		{
			Base b = new Base { Value = 1 };
			b += 2;
			b++;
			Console.WriteLine("InstanceOperator: " + b.Value);
		}

		static void InheritedOperator()
		{
			Derived d = new Derived { Value = 1 };
			d += 2;
			d++;
			Console.WriteLine("InheritedOperator: " + d.Value);
		}

		static void StaticOperatorOnRefLocalStaysStatic()
		{
			Both[] arr = new Both[1] { new Both { Value = 1 } };
			ref Both r = ref arr[0];
			r = r + 5;
			Console.WriteLine("StaticOperatorOnRefLocalStaysStatic: " + arr[0].Value);
		}

		static void ShadowedOperator()
		{
			Shadowing s = new Shadowing { Value = 1 };
			s += 2;
			Console.WriteLine("ShadowedOperator: " + s.Value);
		}

		static void StaticOperatorStaysStatic()
		{
			Both b = new Both { Value = 1 };
			Both original = b;
			b = b + 2;
			Console.WriteLine("StaticOperatorStaysStatic: {0} {1} {2}", b.Value, original.Value, ReferenceEquals(b, original));

			b += 2;
			Console.WriteLine("InstanceOperatorMutatesInPlace: " + b.Value);
		}

		static void StaticTypeSelectsTheOperator()
		{
			StaticBase asBase = new InstanceDerived { Value = 1 };
			asBase += 2;
			Console.WriteLine("AsBase: " + asBase.Value);

			InstanceDerived asDerived = new InstanceDerived { Value = 1 };
			asDerived += 4;
			Console.WriteLine("AsDerived: " + asDerived.Value);
		}

		// A postfix increment whose result is used cannot call an operator that mutates in place,
		// so it takes the static one even though an instance operator is declared. Discarding the
		// result, or writing the prefix form, reaches the instance operator instead.
		static void IncrementDependsOnWhetherTheResultIsUsed()
		{
			Both a = new Both { Value = 1 };
			Both old = a++;
			Console.WriteLine("PostfixResultUsed: {0} {1} {2}", a.Value, old.Value, ReferenceEquals(a, old));

			Both b = new Both { Value = 1 };
			b++;
			Console.WriteLine("PostfixDiscarded: " + b.Value);

			Both c = new Both { Value = 1 };
			Both n = ++c;
			Console.WriteLine("PrefixResultUsed: {0} {1} {2}", c.Value, n.Value, ReferenceEquals(c, n));
		}

		static void StructOperator()
		{
			Counter c = default(Counter);
			c += 3;
			c += 4;
			Console.WriteLine("StructOperator: " + c.Value);
		}

		static void UseGeneric<T>(T x, int n) where T : ICompound
		{
			x += n;
		}

		static void GenericOperator()
		{
			ViaInterface v = new ViaInterface { Value = 1 };
			UseGeneric(v, 5);
			Console.WriteLine("GenericOperator: " + v.Value);
		}

		sealed class DisposableHolder : IDisposable
		{
			public int Value;

			public void operator +=(int rhs)
			{
				Console.WriteLine("DisposableHolder.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}

			public void Dispose()
			{
				Console.WriteLine("Dispose({0})", Value);
			}
		}

		static readonly Base readonlyField = new Base { Value = 10 };
		readonly Base readonlyInstanceField;

		static UserDefinedCompoundAssignment()
		{
			// A readonly field is a variable inside the matching constructor, so the
			// operator form is legal here and has to stay.
			readonlyField += 1;
		}

		UserDefinedCompoundAssignment()
		{
			readonlyInstanceField = new Base { Value = 20 };
			readonlyInstanceField += 2;
		}

		static void ForeachTarget()
		{
			// The iteration variable is read-only, so the operator can only be applied
			// to a copy; the copy has to survive decompilation.
			List<Base> list = new List<Base> { new Base(), new Base() };
			foreach (Base b in list)
			{
				Base copy = b;
				copy += 3;
			}
			foreach (Base b in list)
			{
				Console.WriteLine("ForeachTarget: {0}", b.Value);
			}
			Base[] array = { new Base { Value = 30 } };
			foreach (Base b in array)
			{
				Base copy = b;
				copy += 4;
			}
			Console.WriteLine("ForeachTargetArray: {0}", array[0].Value);
		}

		static void UsingTarget()
		{
			// Same for a using variable.
			using (DisposableHolder d = new DisposableHolder())
			{
				DisposableHolder copy = d;
				copy += 5;
			}
		}

		static void InParameterTarget(in Base b)
		{
			// And for an "in" parameter.
			Base copy = b;
			copy += 6;
			Console.WriteLine("InParameterTarget: {0}", b.Value);
		}

		static void ReadonlyFieldTarget()
		{
			// Outside the constructor the readonly field is not a variable, so the
			// copy has to survive here as well.
			Base copy = readonlyField;
			copy += 7;
			Console.WriteLine("ReadonlyFieldTarget: {0}", readonlyField.Value);
			Console.WriteLine("ReadonlyInstanceField: {0}", new UserDefinedCompoundAssignment().readonlyInstanceField.Value);
		}
	}
}
