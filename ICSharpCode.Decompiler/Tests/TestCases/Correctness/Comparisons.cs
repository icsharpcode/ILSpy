// Copyright (c) 2016 Daniel Grunwald
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

#pragma warning disable 652

namespace ICSharpCode.Decompiler.Tests.TestCases
{
	public class Comparisons
	{
		public static int Main()
		{
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			TestFloatOp("==", (a, b) => a == b);
			TestFloatOp("!=", (a, b) => a != b);
			TestFloatOp("<", (a, b) => a < b);
			TestFloatOp(">", (a, b) => a > b);
			TestFloatOp("<=", (a, b) => a <= b);
			TestFloatOp(">=", (a, b) => a >= b);
			TestFloatOp("!<", (a, b) => !(a < b));
			TestFloatOp("!>", (a, b) => !(a > b));
			TestFloatOp("!<=", (a, b) => !(a <= b));
			TestFloatOp("!>=", (a, b) => !(a >= b));
			
			TestUInt(0);
			TestUInt(uint.MaxValue);
			TestUShort(0);
			TestUShort(ushort.MaxValue);

			Console.WriteLine(IsNotNull(new OverloadedOperators()));
			Console.WriteLine(IsNull(new OverloadedOperators()));
			return 0;
		}
		
		static void TestFloatOp(string name, Func<float, float, bool> f)
		{
			float[] vals = { -1, 0, 3, float.PositiveInfinity, float.NaN };
			for (int i = 0; i < vals.Length; i++) {
				for (int j = 0; j < vals.Length; j++) {
					Console.WriteLine("Float: {0:r} {1} {2:r} = {3}", vals[i], name, vals[j], f(vals[i], vals[j]));
				}
			}
		}
		
		static T Id<T>(T arg)
		{
			return arg;
		}
		
		static void TestUShort(ushort i)
		{
			Console.WriteLine("ushort: {0} == ushort.MaxValue = {1}", i, i == ushort.MaxValue);
			Console.WriteLine("ushort: {0} == -1 = {1}", i, i == -1);
			Console.WriteLine("ushort: {0} == Id<short>(-1) = {1}", i, i == Id<short>(-1));
			Console.WriteLine("ushort: {0} == 0x1ffff = {1}", i, i == 0x1ffff);
		}
		
		static void TestUInt(uint i)
		{
			Console.WriteLine("uint: {0} == uint.MaxValue = {1}", i, i == uint.MaxValue);
			Console.WriteLine("uint: {0} == Id(uint.MaxValue) = {1}", i, i == Id(uint.MaxValue));
			Console.WriteLine("uint: {0} == -1 = {1}", i, i == -1);
			Console.WriteLine("uint: {0} == Id(-1) = {1}", i, i == Id(-1));
		}

		static bool IsNull(OverloadedOperators oo)
		{
			return (object)oo == null;
		}

		static bool IsNotNull(OverloadedOperators oo)
		{
			return (object)oo != null;
		}
	}

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
	class OverloadedOperators
	{
		public static bool operator ==(OverloadedOperators oo, object b)
		{
			throw new NotSupportedException("Not supported to call the user-defined operator");
		}

		public static bool operator !=(OverloadedOperators oo, object b)
		{
			throw new NotSupportedException("Not supported to call the user-defined operator");
		}
	}
}
