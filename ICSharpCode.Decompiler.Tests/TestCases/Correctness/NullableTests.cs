// Copyright (c) 2017 Daniel Grunwald
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
	class NullableTests
	{
		static void Main()
		{
			AvoidLifting();
			BitNot();
			FieldAccessOrderOfEvaluation(null);
			ArrayAccessOrderOfEvaluation();
		}

		static void AvoidLifting()
		{
			Console.WriteLine("MayThrow:");
			Console.WriteLine(MayThrow(10, 2, 3));
			Console.WriteLine(MayThrow(10, 0, null));

			Console.WriteLine("NotUsingAllInputs:");
			Console.WriteLine(NotUsingAllInputs(5, 3));
			Console.WriteLine(NotUsingAllInputs(5, null));

			Console.WriteLine("UsingUntestedValue:");
			Console.WriteLine(UsingUntestedValue(5, 3));
			Console.WriteLine(UsingUntestedValue(5, null));
		}

		static int? MayThrow(int? a, int? b, int? c)
		{
			// cannot be lifted without changing the exception behavior
			return a.HasValue & b.HasValue & c.HasValue ? a.GetValueOrDefault() / b.GetValueOrDefault() + c.GetValueOrDefault() : default(int?);
		}

		static int? NotUsingAllInputs(int? a, int? b)
		{
			// cannot be lifted because the value differs if b == null
			return a.HasValue & b.HasValue ? a.GetValueOrDefault() + a.GetValueOrDefault() : default(int?);
		}

		static int? UsingUntestedValue(int? a, int? b)
		{
			// cannot be lifted because the value differs if b == null
			return a.HasValue ? a.GetValueOrDefault() + b.GetValueOrDefault() : default(int?);
		}

		static void BitNot()
		{
			UInt32? value = 0;
			Assert(~value == UInt32.MaxValue);
			UInt64? value2 = 0;
			Assert(~value2 == UInt64.MaxValue);
			UInt16? value3 = 0;
			Assert((UInt16)~value3 == (UInt16)UInt16.MaxValue);
			UInt32 value4 = 0;
			Assert(~value4 == UInt32.MaxValue);
			UInt64 value5 = 0;
			Assert(~value5 == UInt64.MaxValue);
			UInt16 value6 = 0;
			Assert((UInt16)~value6 == UInt16.MaxValue);
		}

		static void Assert(bool b)
		{
			if (!b)
				throw new InvalidOperationException();
		}


		static T GetValue<T>()
		{
			Console.WriteLine("GetValue");
			return default(T);
		}

		int intField;

		static void FieldAccessOrderOfEvaluation(NullableTests c)
		{
			Console.WriteLine("GetInt, then NRE:");
			try {
				c.intField = GetValue<int>();
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
			Console.WriteLine("NRE before GetInt:");
			try {
#if CS60
				ref int i = ref c.intField;
				i = GetValue<int>();
#endif
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

		static T[] GetArray<T>()
		{
			Console.WriteLine("GetArray");
			return null;
		}

		static int GetIndex()
		{
			Console.WriteLine("GetIndex");
			return 0;
		}

		static void ArrayAccessOrderOfEvaluation()
		{
			Console.WriteLine("GetArray direct:");
			try {
				GetArray<int>()[GetIndex()] = GetValue<int>();
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
			Console.WriteLine("GetArray with ref:");
			try {
#if CS60
				ref int elem = ref GetArray<int>()[GetIndex()];
				elem = GetValue<int>();
#endif
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
			Console.WriteLine("GetArray direct with value-type:");
			try {
				// This line is mis-compiled by legacy csc:
				// with the legacy compiler the NRE is thrown before the GetValue call;
				// with Roslyn the NRE is thrown after the GetValue call.
				GetArray<TimeSpan>()[GetIndex()] = GetValue<TimeSpan>();
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}
	}
}
