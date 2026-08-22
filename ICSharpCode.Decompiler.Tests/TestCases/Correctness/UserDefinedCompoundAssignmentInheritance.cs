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

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	// Every combination of where a static "operator +" and an instance "operator +=" are
	// declared across a two-level hierarchy, used through both a base-typed and a
	// derived-typed variable. The class name suffix says what each level declares:
	// N nothing, S static only, I instance only, X both - so Base_IS declares an instance
	// operator and its derived class declares a static one.
	//
	// Which operator runs depends on the declared type of the left operand, not on the
	// runtime type, and an instance operator reachable from that type wins over any static
	// one. "x = x + n" therefore has to survive decompilation as-is wherever both forms
	// exist: folding it to "x += n" would call a different operator.
	class UserDefinedCompoundAssignmentInheritance
	{
		static void Main()
		{
			Combo_NS();
			Combo_NI();
			Combo_NX();
			Combo_SN();
			Combo_SS();
			Combo_SI();
			Combo_SX();
			Combo_IN();
			Combo_IS();
			Combo_II();
			Combo_IX();
			Combo_XN();
			Combo_XS();
			Combo_XI();
			Combo_XX();
			Combo_BaseStaticReturnsDerived();
			Combo_InstanceSignatureDoesNotApply();
			Combo_DerivedOverloadHidesBase();
		}

		static void Combo_NS()
		{
			Console.WriteLine("NS derived-typed +=:");
			Derived_NS asDerived = new Derived_NS { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
		}

		static void Combo_NI()
		{
			Console.WriteLine("NI derived-typed +=:");
			Derived_NI asDerived = new Derived_NI { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
		}

		static void Combo_NX()
		{
			Console.WriteLine("NX derived-typed +=:");
			Derived_NX asDerived = new Derived_NX { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
			Console.WriteLine("NX derived-typed x = x + n:");
			Derived_NX sDerived = new Derived_NX { Value = 1 };
			sDerived = sDerived + 4;
			Console.WriteLine("  -> " + sDerived.Value);
		}

		static void Combo_SN()
		{
			Console.WriteLine("SN base-typed +=:");
			Base_SN asBase = new Derived_SN { Value = 1 };
			asBase += 2;
			Console.WriteLine("  -> " + asBase.Value);
		}

		static void Combo_SS()
		{
			Console.WriteLine("SS base-typed +=:");
			Base_SS asBase = new Derived_SS { Value = 1 };
			asBase += 2;
			Console.WriteLine("  -> " + asBase.Value);
			Console.WriteLine("SS derived-typed +=:");
			Derived_SS asDerived = new Derived_SS { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
		}

		static void Combo_SI()
		{
			Console.WriteLine("SI base-typed +=:");
			Base_SI asBase = new Derived_SI { Value = 1 };
			asBase += 2;
			Console.WriteLine("  -> " + asBase.Value);
			Console.WriteLine("SI derived-typed +=:");
			Derived_SI asDerived = new Derived_SI { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
		}

		static void Combo_SX()
		{
			Console.WriteLine("SX base-typed +=:");
			Base_SX asBase = new Derived_SX { Value = 1 };
			asBase += 2;
			Console.WriteLine("  -> " + asBase.Value);
			Console.WriteLine("SX derived-typed +=:");
			Derived_SX asDerived = new Derived_SX { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
			Console.WriteLine("SX derived-typed x = x + n:");
			Derived_SX sDerived = new Derived_SX { Value = 1 };
			sDerived = sDerived + 4;
			Console.WriteLine("  -> " + sDerived.Value);
		}

		// The static operator is declared on the base but returns the derived type, so
		// "x = x + n" is expressible on a derived-typed variable. Binary "+" only ever
		// considers static operators, but "x += n" would find the derived class's instance
		// operator first, so the assignment has to stay spelled out.
		static void Combo_BaseStaticReturnsDerived()
		{
			Console.WriteLine("base static returning derived, derived-typed x = x + n:");
			Derived_SR asDerived = new Derived_SR { Value = 1 };
			asDerived = asDerived + 4;
			Console.WriteLine("  -> " + asDerived.Value);
			Console.WriteLine("base static returning derived, array element x = x + n:");
			Derived_SR[] arr = new Derived_SR[1] { new Derived_SR { Value = 1 } };
			arr[0] = arr[0] + 4;
			Console.WriteLine("  -> " + arr[0].Value);
		}

		// Overload resolution removes base-type candidates when an applicable candidate exists
		// in a more derived type: on a derived-typed target even an int argument binds the
		// derived operator taking long. The base-typed variable is what makes the int overload
		// reachable, so it has to survive decompilation.
		static void Combo_DerivedOverloadHidesBase()
		{
			Console.WriteLine("overload base-typed +=:");
			Derived_Overload target = new Derived_Overload { Value = 1 };
			Base_Overload asBase = target;
			asBase += 2;
			Console.WriteLine("  -> " + target.Value);
			Console.WriteLine("overload derived-typed +=:");
			Derived_Overload asDerived = new Derived_Overload { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
		}

		// The instance operator takes an int, so it is not a candidate for a long argument
		// and "x += n" binds the static operator: the assignment can be folded.
		static void Combo_InstanceSignatureDoesNotApply()
		{
			Console.WriteLine("instance operator not applicable, long argument:");
			MixedSignature asLocal = new MixedSignature { Value = 1 };
			asLocal += 4L;
			Console.WriteLine("  -> " + asLocal.Value);
			Console.WriteLine("instance operator applicable, int argument:");
			MixedSignature asInt = new MixedSignature { Value = 1 };
			asInt += 4;
			Console.WriteLine("  -> " + asInt.Value);
		}

		static void Combo_IN()
		{
			Console.WriteLine("IN base-typed +=:");
			Base_IN asBase = new Derived_IN { Value = 1 };
			asBase += 2;
			Console.WriteLine("  -> " + asBase.Value);
			Console.WriteLine("IN derived-typed +=:");
			Derived_IN asDerived = new Derived_IN { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
		}

		static void Combo_IS()
		{
			Console.WriteLine("IS base-typed +=:");
			Base_IS asBase = new Derived_IS { Value = 1 };
			asBase += 2;
			Console.WriteLine("  -> " + asBase.Value);
			Console.WriteLine("IS derived-typed +=:");
			Derived_IS asDerived = new Derived_IS { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
			Console.WriteLine("IS derived-typed x = x + n:");
			Derived_IS sDerived = new Derived_IS { Value = 1 };
			sDerived = sDerived + 4;
			Console.WriteLine("  -> " + sDerived.Value);
		}

		static void Combo_II()
		{
			Console.WriteLine("II base-typed +=:");
			Base_II asBase = new Derived_II { Value = 1 };
			asBase += 2;
			Console.WriteLine("  -> " + asBase.Value);
			Console.WriteLine("II derived-typed +=:");
			Derived_II asDerived = new Derived_II { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
		}

		static void Combo_IX()
		{
			Console.WriteLine("IX base-typed +=:");
			Base_IX asBase = new Derived_IX { Value = 1 };
			asBase += 2;
			Console.WriteLine("  -> " + asBase.Value);
			Console.WriteLine("IX derived-typed +=:");
			Derived_IX asDerived = new Derived_IX { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
			Console.WriteLine("IX derived-typed x = x + n:");
			Derived_IX sDerived = new Derived_IX { Value = 1 };
			sDerived = sDerived + 4;
			Console.WriteLine("  -> " + sDerived.Value);
		}

		static void Combo_XN()
		{
			Console.WriteLine("XN base-typed +=:");
			Base_XN asBase = new Derived_XN { Value = 1 };
			asBase += 2;
			Console.WriteLine("  -> " + asBase.Value);
			Console.WriteLine("XN base-typed x = x + n:");
			Base_XN sBase = new Derived_XN { Value = 1 };
			sBase = sBase + 2;
			Console.WriteLine("  -> " + sBase.Value);
			Console.WriteLine("XN derived-typed +=:");
			Derived_XN asDerived = new Derived_XN { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
		}

		static void Combo_XS()
		{
			Console.WriteLine("XS base-typed +=:");
			Base_XS asBase = new Derived_XS { Value = 1 };
			asBase += 2;
			Console.WriteLine("  -> " + asBase.Value);
			Console.WriteLine("XS base-typed x = x + n:");
			Base_XS sBase = new Derived_XS { Value = 1 };
			sBase = sBase + 2;
			Console.WriteLine("  -> " + sBase.Value);
			Console.WriteLine("XS derived-typed +=:");
			Derived_XS asDerived = new Derived_XS { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
			Console.WriteLine("XS derived-typed x = x + n:");
			Derived_XS sDerived = new Derived_XS { Value = 1 };
			sDerived = sDerived + 4;
			Console.WriteLine("  -> " + sDerived.Value);
		}

		static void Combo_XI()
		{
			Console.WriteLine("XI base-typed +=:");
			Base_XI asBase = new Derived_XI { Value = 1 };
			asBase += 2;
			Console.WriteLine("  -> " + asBase.Value);
			Console.WriteLine("XI base-typed x = x + n:");
			Base_XI sBase = new Derived_XI { Value = 1 };
			sBase = sBase + 2;
			Console.WriteLine("  -> " + sBase.Value);
			Console.WriteLine("XI derived-typed +=:");
			Derived_XI asDerived = new Derived_XI { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
		}

		static void Combo_XX()
		{
			Console.WriteLine("XX base-typed +=:");
			Base_XX asBase = new Derived_XX { Value = 1 };
			asBase += 2;
			Console.WriteLine("  -> " + asBase.Value);
			Console.WriteLine("XX base-typed x = x + n:");
			Base_XX sBase = new Derived_XX { Value = 1 };
			sBase = sBase + 2;
			Console.WriteLine("  -> " + sBase.Value);
			Console.WriteLine("XX derived-typed +=:");
			Derived_XX asDerived = new Derived_XX { Value = 1 };
			asDerived += 4;
			Console.WriteLine("  -> " + asDerived.Value);
			Console.WriteLine("XX derived-typed x = x + n:");
			Derived_XX sDerived = new Derived_XX { Value = 1 };
			sDerived = sDerived + 4;
			Console.WriteLine("  -> " + sDerived.Value);
		}

		class Base_NS
		{
			public int Value;
		}

		class Derived_NS : Base_NS
		{
			public static Derived_NS operator +(Derived_NS lhs, int rhs)
			{
				Console.WriteLine("  Derived_NS.op_Addition({0})", rhs);
				return new Derived_NS { Value = lhs.Value + rhs };
			}
		}

		class Base_NI
		{
			public int Value;
		}

		class Derived_NI : Base_NI
		{
			public void operator +=(int rhs)
			{
				Console.WriteLine("  Derived_NI.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Base_NX
		{
			public int Value;
		}

		class Derived_NX : Base_NX
		{
			public static Derived_NX operator +(Derived_NX lhs, int rhs)
			{
				Console.WriteLine("  Derived_NX.op_Addition({0})", rhs);
				return new Derived_NX { Value = lhs.Value + rhs };
			}

			public void operator +=(int rhs)
			{
				Console.WriteLine("  Derived_NX.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Base_SN
		{
			public int Value;

			public static Base_SN operator +(Base_SN lhs, int rhs)
			{
				Console.WriteLine("  Base_SN.op_Addition({0})", rhs);
				return new Base_SN { Value = lhs.Value + rhs };
			}
		}

		class Derived_SN : Base_SN
		{
		}

		class Base_SS
		{
			public int Value;

			public static Base_SS operator +(Base_SS lhs, int rhs)
			{
				Console.WriteLine("  Base_SS.op_Addition({0})", rhs);
				return new Base_SS { Value = lhs.Value + rhs };
			}
		}

		class Derived_SS : Base_SS
		{
			public static Derived_SS operator +(Derived_SS lhs, int rhs)
			{
				Console.WriteLine("  Derived_SS.op_Addition({0})", rhs);
				return new Derived_SS { Value = lhs.Value + rhs };
			}
		}

		class Base_SI
		{
			public int Value;

			public static Base_SI operator +(Base_SI lhs, int rhs)
			{
				Console.WriteLine("  Base_SI.op_Addition({0})", rhs);
				return new Base_SI { Value = lhs.Value + rhs };
			}
		}

		class Derived_SI : Base_SI
		{
			public void operator +=(int rhs)
			{
				Console.WriteLine("  Derived_SI.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Base_SX
		{
			public int Value;

			public static Base_SX operator +(Base_SX lhs, int rhs)
			{
				Console.WriteLine("  Base_SX.op_Addition({0})", rhs);
				return new Base_SX { Value = lhs.Value + rhs };
			}
		}

		class Derived_SX : Base_SX
		{
			public static Derived_SX operator +(Derived_SX lhs, int rhs)
			{
				Console.WriteLine("  Derived_SX.op_Addition({0})", rhs);
				return new Derived_SX { Value = lhs.Value + rhs };
			}

			public void operator +=(int rhs)
			{
				Console.WriteLine("  Derived_SX.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Base_IN
		{
			public int Value;

			public void operator +=(int rhs)
			{
				Console.WriteLine("  Base_IN.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Derived_IN : Base_IN
		{
		}

		class Base_IS
		{
			public int Value;

			public void operator +=(int rhs)
			{
				Console.WriteLine("  Base_IS.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Derived_IS : Base_IS
		{
			public static Derived_IS operator +(Derived_IS lhs, int rhs)
			{
				Console.WriteLine("  Derived_IS.op_Addition({0})", rhs);
				return new Derived_IS { Value = lhs.Value + rhs };
			}
		}

		class Base_II
		{
			public int Value;

			public void operator +=(int rhs)
			{
				Console.WriteLine("  Base_II.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Derived_II : Base_II
		{
			public new void operator +=(int rhs)
			{
				Console.WriteLine("  Derived_II.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Base_IX
		{
			public int Value;

			public void operator +=(int rhs)
			{
				Console.WriteLine("  Base_IX.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Derived_IX : Base_IX
		{
			public static Derived_IX operator +(Derived_IX lhs, int rhs)
			{
				Console.WriteLine("  Derived_IX.op_Addition({0})", rhs);
				return new Derived_IX { Value = lhs.Value + rhs };
			}

			public new void operator +=(int rhs)
			{
				Console.WriteLine("  Derived_IX.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Base_XN
		{
			public int Value;

			public static Base_XN operator +(Base_XN lhs, int rhs)
			{
				Console.WriteLine("  Base_XN.op_Addition({0})", rhs);
				return new Base_XN { Value = lhs.Value + rhs };
			}

			public void operator +=(int rhs)
			{
				Console.WriteLine("  Base_XN.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Derived_XN : Base_XN
		{
		}

		class Base_XS
		{
			public int Value;

			public static Base_XS operator +(Base_XS lhs, int rhs)
			{
				Console.WriteLine("  Base_XS.op_Addition({0})", rhs);
				return new Base_XS { Value = lhs.Value + rhs };
			}

			public void operator +=(int rhs)
			{
				Console.WriteLine("  Base_XS.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Derived_XS : Base_XS
		{
			public static Derived_XS operator +(Derived_XS lhs, int rhs)
			{
				Console.WriteLine("  Derived_XS.op_Addition({0})", rhs);
				return new Derived_XS { Value = lhs.Value + rhs };
			}
		}

		class Base_XI
		{
			public int Value;

			public static Base_XI operator +(Base_XI lhs, int rhs)
			{
				Console.WriteLine("  Base_XI.op_Addition({0})", rhs);
				return new Base_XI { Value = lhs.Value + rhs };
			}

			public void operator +=(int rhs)
			{
				Console.WriteLine("  Base_XI.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Derived_XI : Base_XI
		{
			public new void operator +=(int rhs)
			{
				Console.WriteLine("  Derived_XI.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Base_XX
		{
			public int Value;

			public static Base_XX operator +(Base_XX lhs, int rhs)
			{
				Console.WriteLine("  Base_XX.op_Addition({0})", rhs);
				return new Base_XX { Value = lhs.Value + rhs };
			}

			public void operator +=(int rhs)
			{
				Console.WriteLine("  Base_XX.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Derived_XX : Base_XX
		{
			public static Derived_XX operator +(Derived_XX lhs, int rhs)
			{
				Console.WriteLine("  Derived_XX.op_Addition({0})", rhs);
				return new Derived_XX { Value = lhs.Value + rhs };
			}

			public new void operator +=(int rhs)
			{
				Console.WriteLine("  Derived_XX.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Base_SR
		{
			public int Value;

			public static Derived_SR operator +(Base_SR lhs, int rhs)
			{
				Console.WriteLine("  Base_SR.op_Addition({0})", rhs);
				return new Derived_SR { Value = lhs.Value + rhs };
			}
		}

		class Derived_SR : Base_SR
		{
			public void operator +=(int rhs)
			{
				Console.WriteLine("  Derived_SR.op_AdditionAssignment({0})", rhs);
				Value += rhs * 100;
			}
		}

		class MixedSignature
		{
			public int Value;

			public static MixedSignature operator +(MixedSignature lhs, long rhs)
			{
				Console.WriteLine("  MixedSignature.op_Addition({0})", rhs);
				return new MixedSignature { Value = lhs.Value + (int)rhs };
			}

			public void operator +=(int rhs)
			{
				Console.WriteLine("  MixedSignature.op_AdditionAssignment({0})", rhs);
				Value += rhs * 100;
			}
		}

		class Base_Overload
		{
			public int Value;

			public void operator +=(int rhs)
			{
				Console.WriteLine("  Base_Overload.op_AdditionAssignment({0})", rhs);
				Value += rhs;
			}
		}

		class Derived_Overload : Base_Overload
		{
			public void operator +=(long rhs)
			{
				Console.WriteLine("  Derived_Overload.op_AdditionAssignment({0}L)", rhs);
				Value += (int)rhs * 100;
			}
		}
	}
}
