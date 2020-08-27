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
using System.Reflection;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class TypeAnalysisTests
	{
		private class @_
		{
		}

		private byte[] byteArray;

		public byte SubtractFrom256(byte b)
		{
			return (byte)(256 - b);
		}

		#region Shift
		public int LShiftInteger(int num1, int num2)
		{
			return num1 << num2;
		}

		public uint LShiftUnsignedInteger(uint num1, uint num2)
		{
			return num1 << (int)num2;
		}

		public long LShiftLong(long num1, long num2)
		{
			return num1 << (int)num2;
		}

		public ulong LShiftUnsignedLong(ulong num1, ulong num2)
		{
			return num1 << (int)num2;
		}

		public int RShiftInteger(int num1, int num2)
		{
			return num1 >> num2;
		}

		public uint RShiftUnsignedInteger(uint num1, int num2)
		{
			return num1 >> num2;
		}

		public long RShiftLong(long num1, long num2)
		{
			return num1 >> (int)num2;
		}

		public ulong RShiftUnsignedLong(ulong num1, ulong num2)
		{
			return num1 >> (int)num2;
		}

		public int ShiftByte(byte num)
		{
			return num << 8;
		}

		public int RShiftByte(byte num)
		{
			return num >> 8;
		}

		public uint RShiftByteWithZeroExtension(byte num)
		{
			return (uint)num >> 8;
		}

		public int RShiftByteAsSByte(byte num)
		{
			return (sbyte)num >> 8;
		}

		public int RShiftSByte(sbyte num)
		{
			return num >> 8;
		}

		public uint RShiftSByteWithZeroExtension(sbyte num)
		{
			return (uint)num >> 8;
		}

		public int RShiftSByteAsByte(sbyte num)
		{
			return (byte)num >> 8;
		}
		#endregion

		public int GetHashCode(long num)
		{
			return (int)num ^ (int)(num >> 32);
		}

		public void TernaryOp(Random a, Random b, bool c)
		{
			if ((c ? a : b) == null)
			{
				Console.WriteLine();
			}
		}

		public void OperatorIs(object o)
		{
			Console.WriteLine(o is Random);
			Console.WriteLine(!(o is Random));
			// If we didn't escape the '_' identifier here, this would look like a discard pattern
			Console.WriteLine(o is @_);
		}

		public byte[] CreateArrayWithInt(int length)
		{
			return new byte[length];
		}

		public byte[] CreateArrayWithLong(long length)
		{
			return new byte[length];
		}

		public byte[] CreateArrayWithUInt(uint length)
		{
			return new byte[length];
		}

		public byte[] CreateArrayWithULong(ulong length)
		{
			return new byte[length];
		}

		public byte[] CreateArrayWithShort(short length)
		{
			return new byte[length];
		}

		public byte[] CreateArrayWithUShort(ushort length)
		{
			return new byte[length];
		}

		public byte UseArrayWithInt(int i)
		{
			return byteArray[i];
		}

		public byte UseArrayWithUInt(uint i)
		{
			return byteArray[i];
		}

		public byte UseArrayWithLong(long i)
		{
			return byteArray[i];
		}

		public byte UseArrayWithULong(ulong i)
		{
			return byteArray[i];
		}

		public byte UseArrayWithShort(short i)
		{
			return byteArray[i];
		}

		public byte UseArrayWithUShort(ushort i)
		{
			return byteArray[i];
		}

		public byte UseArrayWithCastToUShort(int i)
		{
			// Unchecked cast = truncate to 16 bits
			return byteArray[(ushort)i];
		}

		public StringComparison EnumDiffNumber(StringComparison data)
		{
			return data - 1;
		}

		public int EnumDiff(StringComparison a, StringComparison b)
		{
			return Math.Abs(a - b);
		}

		public bool CompareDelegatesByValue(Action a, Action b)
		{
			return a == b;
		}

		public bool CompareDelegatesByReference(Action a, Action b)
		{
			return (object)a == b;
		}

		public bool CompareDelegateWithNull(Action a)
		{
			return a == null;
		}

		public bool CompareStringsByValue(string a, string b)
		{
			return a == b;
		}

		public bool CompareStringsByReference(string a, string b)
		{
			return (object)a == b;
		}

		public bool CompareStringWithNull(string a)
		{
			return a == null;
		}

		public bool CompareType(Type a, Type b)
		{
			return a == b;
		}

		public bool CompareTypeByReference(Type a, Type b)
		{
			return (object)a == b;
		}

		public bool CompareTypeWithNull(Type t)
		{
			return t == null;
		}

		public Attribute CallExtensionMethodViaBaseClass(Type type)
		{
			return type.GetCustomAttribute<AttributeUsageAttribute>();
		}

		public decimal ImplicitConversionToDecimal(byte v)
		{
			return v;
		}

		public decimal ImplicitConversionToDecimal(ulong v)
		{
			return v;
		}

		public bool EnumInConditionalOperator(bool b)
		{
			return string.Equals("", "", b ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
		}

		public bool MethodCallOnEnumConstant()
		{
			return AttributeTargets.All.HasFlag(AttributeTargets.Assembly);
		}

		public static string ImpossibleCast1(int i)
		{
			return (string)(object)i;
		}

		public static string ImpossibleCast2(Action a)
		{
			return (string)(object)a;
		}
	}
}
