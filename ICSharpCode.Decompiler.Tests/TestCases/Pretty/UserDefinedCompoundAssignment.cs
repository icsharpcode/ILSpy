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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class UserDefinedCompoundAssignment
	{
		public class CompoundClass
		{
			public int Value;

			public void operator +=(int rhs)
			{
				Value += rhs;
			}

			public void operator checked +=(int rhs)
			{
				checked
				{
					Value += rhs;
				}
			}

			public void operator -=(int rhs)
			{
				Value -= rhs;
			}

			public void operator checked -=(int rhs)
			{
				checked
				{
					Value -= rhs;
				}
			}

			public void operator *=(int rhs)
			{
				Value *= rhs;
			}

			public void operator checked *=(int rhs)
			{
				checked
				{
					Value *= rhs;
				}
			}

			public void operator /=(int rhs)
			{
				Value /= rhs;
			}

			public void operator checked /=(int rhs)
			{
				Value /= rhs;
			}

			public void operator %=(int rhs)
			{
				Value %= rhs;
			}

			public void operator &=(int rhs)
			{
				Value &= rhs;
			}

			public void operator |=(int rhs)
			{
				Value |= rhs;
			}

			public void operator ^=(int rhs)
			{
				Value ^= rhs;
			}

			public void operator <<=(int rhs)
			{
				Value <<= rhs;
			}

			public void operator >>=(int rhs)
			{
				Value >>= rhs;
			}

			public void operator >>>=(int rhs)
			{
				Value >>>= rhs;
			}

			public void operator ++()
			{
				Value++;
			}

			public void operator checked ++()
			{
				checked
				{
					Value++;
				}
			}

			public void operator --()
			{
				Value--;
			}

			public void operator checked --()
			{
				checked
				{
					Value--;
				}
			}

			public virtual void operator +=(long rhs)
			{
				Value += (int)rhs;
			}

			// "this" is not an assignable variable in a class, so the copy is what makes the
			// operator form legal; it has to survive decompilation.
			public void AddViaThisCopy(int n)
			{
				CompoundClass compoundClass = this;
				compoundClass += n;
			}

			public void IncrementViaThisCopy()
			{
				CompoundClass compoundClass = this;
				compoundClass++;
			}
		}

		public struct CompoundStruct
		{
			public int Value;

			public void operator +=(int rhs)
			{
				Value += rhs;
			}

			public readonly void operator -=(int rhs)
			{
				Console.WriteLine(Value - rhs);
			}

			public void operator ++()
			{
				Value++;
			}

			public void AddViaThis()
			{
				this += 10;
			}
		}

		public class DerivedCompoundClass : CompoundClass
		{
		}

		public class OverridingCompoundClass : CompoundClass
		{
			public override void operator +=(long rhs)
			{
				Value += (int)(rhs * 2);
			}
		}

		public class ShadowingCompoundClass : CompoundClass
		{
			public new void operator +=(int rhs)
			{
				Value += rhs * 2;
			}
		}

		public sealed class DisposableCompound : IDisposable
		{
			public int Value;

			public void operator +=(int rhs)
			{
				Value += rhs;
			}

			public void Dispose()
			{
			}
		}

		public class ReadonlyFieldHolder
		{
			public static readonly CompoundClass Shared;

			private readonly CompoundClass instance = new CompoundClass();

			static ReadonlyFieldHolder()
			{
				Shared = new CompoundClass();
				// A readonly field is a variable inside the matching constructor, so the
				// operator form stays legal here.
				Shared += 1;
			}

			public ReadonlyFieldHolder()
			{
				instance += 2;
			}
		}

		// A value-type element makes csc address the storage once with ldelema, which is a
		// different shape again from the reference-type case.
		public struct StaticOnlyStruct
		{
			public int Value;

			public static StaticOnlyStruct operator +(StaticOnlyStruct lhs, int rhs)
			{
				return new StaticOnlyStruct {
					Value = lhs.Value + rhs
				};
			}
		}

		// No instance operator, so "x += n" reaches the static one and csc emits the compound
		// assignment shape rather than a load and a store.
		public class StaticOnlyOperators
		{
			public int Value;

			public static StaticOnlyOperators operator +(StaticOnlyOperators lhs, int rhs)
			{
				return new StaticOnlyOperators {
					Value = lhs.Value + rhs
				};
			}

			public static StaticOnlyOperators operator ++(StaticOnlyOperators x)
			{
				return new StaticOnlyOperators {
					Value = x.Value + 1
				};
			}
		}

		public class BothOperators
		{
			public int Value;

			public static BothOperators operator +(BothOperators lhs, int rhs)
			{
				return new BothOperators {
					Value = lhs.Value + rhs
				};
			}

			public void operator +=(int rhs)
			{
				Value += rhs;
			}

			public static BothOperators operator ++(BothOperators x)
			{
				return new BothOperators {
					Value = x.Value + 1
				};
			}

			public void operator ++()
			{
				Value++;
			}
		}

		public class MixedSignatureOperators
		{
			public int Value;

			public static MixedSignatureOperators operator +(MixedSignatureOperators lhs, long rhs)
			{
				return new MixedSignatureOperators {
					Value = lhs.Value + (int)rhs
				};
			}

			public void operator +=(int rhs)
			{
				Value += rhs;
			}
		}

		public interface ICompound<T>
		{
			void operator +=(T rhs);
			void operator ++();
		}

		public class ExplicitCompound : ICompound<int>
		{
			public int Value;

			void ICompound<int>.operator +=(int rhs)
			{
				Value += rhs;
			}

			void ICompound<int>.operator ++()
			{
				Value++;
			}
		}

		private static CompoundClass staticField = new CompoundClass();

		private CompoundClass instanceField = new CompoundClass();

		private BothOperators bothField = new BothOperators();

		private BothOperators[] bothArray = new BothOperators[4];

		private static CompoundClass refReturnTarget = new CompoundClass();

		private static CompoundStruct refReturnStructTarget;

		public BothOperators BothProperty { get; set; }

		public BothOperators this[int index] {
			get {
				return bothArray[index];
			}
			set {
				bothArray[index] = value;
			}
		}

		public static void UseClass(CompoundClass c, int n)
		{
			c += n;
			c += 1;
			c -= n;
			c *= n;
			c /= n;
			c %= n;
			c &= n;
			c |= n;
			c ^= n;
			c <<= n;
			c >>= n;
			c >>>= n;
			c += 2L;
			c++;
			c--;
			checked
			{
				c += n;
				c -= n;
				c *= n;
				c /= n;
				c++;
				c--;
			}
		}

		public static void UseStruct(CompoundStruct s, int n)
		{
			s += n;
			s -= n;
			s++;
		}

		public static void UseOtherTargets(CompoundClass[] arr, ref CompoundClass rc, ref CompoundStruct rs, UserDefinedCompoundAssignment inst, int n)
		{
			staticField += n;
			inst.instanceField += n;
			arr[0] += n;
			rc += n;
			rs += n;
			rs++;
		}

		public static void UseGeneric<T>(T x, int n) where T : ICompound<int>
		{
			x += n;
			x++;
		}

		public static void UseInheritedOperator(DerivedCompoundClass d, int n)
		{
			d += n;
			d++;
		}

		public static void UseShadowedOperator(ShadowingCompoundClass s, int n)
		{
			s += n;
		}

		public static void UseOverriddenOperator(OverridingCompoundClass o, long n)
		{
			o += n;
		}

		public static BothOperators UsePostfixIncrementResultUsed(BothOperators b)
		{
			return b++;
		}

		public static BothOperators UseStaticOperator(BothOperators b, int n)
		{
			// The IL calls the static operator, so the decompiled code has to keep calling it:
			// "b += n" and "b++" would bind to the instance operators instead.
			b = b + n;
			b = b + 1;
			return b;
		}

		public static void UseCompoundAssignmentOnArrayElement(StaticOnlyOperators[] arr, int n)
		{
			arr[0] += n;
			arr[0]++;
		}

		public static void UseCompoundAssignmentOnStructArrayElement(StaticOnlyStruct[] arr, int n)
		{
			arr[0] += n;
		}

		public static void UseStaticOperatorOnArrayElement(BothOperators[] arr, int n)
		{
			// The element type declares an instance operator too, so the static call has to stay
			// spelled out; array elements take a separate path through the decompiler to locals.
			arr[0] = arr[0] + n;
			arr[0] = arr[0] + 1;
		}

		public static void UseStaticOperatorOnField(UserDefinedCompoundAssignment inst, int n)
		{
			inst.bothField = inst.bothField + n;
		}

		public static void UseBaseTypedLocalOfShadowingClass(ShadowingCompoundClass s, int n)
		{
			// The local is what makes "c += n" bind the base operator rather than the shadowing
			// one, so it has to survive even where the compiler optimized it away.
			CompoundClass compoundClass = s;
			compoundClass += n;
		}

		public static void UseStaticOperatorOnPropertyAndIndexer(UserDefinedCompoundAssignment inst, int n)
		{
			// A property and an indexer are not variables, so the instance operator is never a
			// candidate for them: "x += n" binds the static operator, which is the one the IL calls.
			inst.BothProperty += n;
			inst[0] += n;
		}

		public static void UseStaticOperatorWhenInstanceDoesNotApply(MixedSignatureOperators m, MixedSignatureOperators[] arr, long n)
		{
			// The instance operator takes an int, so it is not a candidate for a long argument:
			// "x += n" binds the static operator here, which is the one the IL calls.
			m += n;
			arr[0] += n;
		}

		public static void UseInstanceOperator(BothOperators b, int n)
		{
			b += n;
			b++;
		}

		public static ref CompoundClass GetRefTarget()
		{
			return ref refReturnTarget;
		}

		public static ref CompoundStruct GetRefStructTarget()
		{
			return ref refReturnStructTarget;
		}

		public static void UseRefReturnTarget(int n)
		{
			// A ref-returning invocation is a variable, so the operator form binds here.
			GetRefTarget() += n;
			GetRefTarget()++;
			GetRefStructTarget() += n;
			GetRefStructTarget()++;
		}

		public unsafe static void UsePointerReceiver(CompoundStruct* p, int n)
		{
			// A pointer indirection is a variable, so the operator form binds here.
			*p += n;
			(*p)++;
		}

		public static void UseForeachVariableCopy(List<CompoundClass> list, int n)
		{
			// The iteration variable is read-only, so the operator is applied to a copy; the
			// copy has to survive even where the compiler optimized it away.
			foreach (CompoundClass item in list)
			{
				CompoundClass compoundClass = item;
				compoundClass += n;
			}
		}

		public static void UseUsingVariableCopy(int n)
		{
			// The using variable is read-only, so the operator is applied to a copy; the copy has
			// to survive even where the compiler optimized it away.
			using DisposableCompound disposableCompound = new DisposableCompound();
			DisposableCompound disposableCompound2 = disposableCompound;
			disposableCompound2 += n;
		}

		public static void UseExplicitInterfaceOperator(ExplicitCompound e, int n)
		{
			// The interface-typed variable is the only receiver that can bind an explicitly
			// implemented operator, so the copy has to survive decompilation.
			ICompound<int> compound = e;
			compound += n;
			ICompound<int> compound2 = e;
			compound2++;
		}
	}
}
