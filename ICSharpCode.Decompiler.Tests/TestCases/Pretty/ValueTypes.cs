// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public static class ValueTypes
	{
		public struct S
		{
			public int Field;

			public S(int field)
			{
				Field = field;
			}

			public void SetField()
			{
				Field = 5;
			}

			public void MethodCalls()
			{
				SetField();
				Test(this);
				Test(ref this);
			}

			private static void Test(S byVal)
			{
			}

			private static void Test(ref S byRef)
			{
			}
		}

#if ROSLYN && OPT
		// Roslyn optimizes out the explicit default-initialization
		private static readonly S ReadOnlyS;
		private static S MutableS;
#else
		private static readonly S ReadOnlyS = default(S);
		private static S MutableS = default(S);
#endif
		private static volatile int VolatileInt;

		public static void CallMethodViaField()
		{
			ReadOnlyS.SetField();
			MutableS.SetField();
			S mutableS = MutableS;
			mutableS.SetField();
		}

#if !(ROSLYN && OPT) || COPY_PROPAGATION_FIXED
		public static S InitObj1()
		{
			S result = default(S);
			MakeArray();
			return result;
		}
#endif

		public static S InitObj2()
		{
			return default(S);
		}

		public static void InitObj3(out S p)
		{
			p = default(S);
		}

		public static S CallValueTypeCtor()
		{
			return new S(10);
		}
		
		public static S Copy1(S p)
		{
			return p;
		}

		public static S Copy2(ref S p)
		{
			return p;
		}

		public static void Copy3(S p, out S o)
		{
			o = p;
		}

		public static void Copy4(ref S p, out S o)
		{
			o = p;
		}

		public static void Copy4b(ref S p, out S o)
		{
			// test passing through by-ref arguments
			Copy4(ref p, out o);
		}

		public static void Issue56(int i, out string str)
		{
			str = "qq";
			str += i.ToString();
		}

		public static void CopyAroundAndModifyField(S s)
		{
			S s2 = s;
			s2.Field += 10;
			s = s2;
		}

		private static int[] MakeArray()
		{
			return null;
		}

		public static void IncrementArrayLocation()
		{
			MakeArray()[Environment.TickCount]++;
		}

		public static bool Is(object obj)
		{
			return obj is S;
		}

		public static bool IsNullable(object obj)
		{
			return obj is S?;
		}

		public static S? As(object obj)
		{
			return obj as S?;
		}

		public static S OnlyChangeTheCopy(S p)
		{
			S s = p;
			s.SetField();
			return p;
		}

		public static void UseRefBoolInCondition(ref bool x)
		{
			if (x) {
				Console.WriteLine("true");
			}
		}

		public static void CompareNotEqual0IsReallyNotEqual(IComparable<int> a)
		{
			if (a.CompareTo(0) != 0) {
				Console.WriteLine("true");
			}
		}

		public static void CompareEqual0IsReallyEqual(IComparable<int> a)
		{
			if (a.CompareTo(0) == 0) {
				Console.WriteLine("true");
			}
		}
	}
}