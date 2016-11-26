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

// #include "../../../Util/CSharpPrimitiveCast.cs"

using System;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Tests.TestCases
{
	public class Conversions
	{
		static readonly TypeCode[] targetTypes = {
			TypeCode.Char,
			TypeCode.SByte,
			TypeCode.Byte,
			TypeCode.Int16,
			TypeCode.UInt16,
			TypeCode.Int32,
			TypeCode.UInt32,
			TypeCode.Int64,
			TypeCode.UInt64,
			TypeCode.Single,
			TypeCode.Double,
			//TypeCode.Decimal
		};
		
		static object[] inputValues = {
			'\0',
			'a',
			'\uFFFE',
			
			sbyte.MinValue,
			sbyte.MaxValue,
			(sbyte)-1,
			(sbyte)1,
			
			byte.MinValue,
			byte.MaxValue,
			(byte)1,
			
			short.MinValue,
			short.MaxValue,
			(short)-1,
			(short)1,
			
			ushort.MinValue,
			ushort.MaxValue,
			(ushort)1,
			
			int.MinValue,
			int.MaxValue,
			(int)-1,
			(int)1,
			
			uint.MinValue,
			uint.MaxValue,
			(uint)1,
			
			long.MinValue,
			long.MaxValue,
			(long)-1,
			(long)1,
			
			ulong.MinValue,
			ulong.MaxValue,
			(ulong)1,
			
			-1.1f,
			1.1f,
			float.MinValue,
			float.MaxValue,
			float.NegativeInfinity,
			float.PositiveInfinity,
			float.NaN,
			
			-1.1,
			1.1,
			double.MinValue,
			double.MaxValue,
			double.NegativeInfinity,
			double.PositiveInfinity,
			double.NaN,
			
			decimal.MinValue,
			decimal.MaxValue,
			decimal.MinusOne,
			decimal.One
		};
		
		static void Main(string[] args)
		{
			RunTest(checkForOverflow: false);
			RunTest(checkForOverflow: true);
			
			Console.WriteLine(ReadZeroTerminatedString("Hello World!".Length));
		}
		
		static void RunTest(bool checkForOverflow)
		{
			string mode = checkForOverflow ? "checked" : "unchecked";
			foreach (object input in inputValues) {
				string inputType = input.GetType().Name;
				foreach (var targetType in targetTypes) {
					try {
						object result = CSharpPrimitiveCast.Cast(targetType, input, checkForOverflow);
						Console.WriteLine("{0} ({1})({2}){3} = ({4}){5}", mode, targetType, inputType, input,
						                  result.GetType().Name, result);
					} catch (Exception ex) {
						Console.WriteLine("{0} ({1})({2}){3} = {4}", mode, targetType, inputType, input,
						                  ex.GetType().Name);
					}
				}
			}
		}
		
		static object MM(sbyte c)
		{
			checked {
				return (UInt64)c;
			}
		}
		
		static string ReadZeroTerminatedString (int length)
		{
			int read = 0;
			var buffer = new char [length];
			var bytes = ReadBytes (length);
			while (read < length) {
				var current = bytes [read];
				if (current == 0)
					break;

				buffer [read++] = (char) current;
			}

			return new string (buffer, 0, read);
		}
		
		static byte[] ReadBytes(int length) {
			return System.Text.Encoding.ASCII.GetBytes("Hello World!");
		}
	}
}
