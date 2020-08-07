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
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class CompoundAssignment
	{
		static void Main()
		{
			PreIncrementProperty();
			PreIncrementIndexer();
			CallTwice();
			UnsignedShiftRightInstanceField();
			UnsignedShiftRightStaticProperty();
			DivideByBigValue();
			Overflow();
			IntPtr_CompoundAssign();
		}

		static void Test(int a, int b)
		{
			Console.WriteLine("{0} {1}", a, b);
		}

		static int x;

		static int X()
		{
			Console.Write("X ");
			return ++x;
		}

		static int instanceCount;
		int instanceNumber = ++instanceCount;
		
		int instanceField;

		public int InstanceProperty
		{
			get {
				Console.WriteLine("In {0}.get_InstanceProperty", instanceNumber);
				return instanceField;
			}
			set {
				Console.WriteLine("In {0}.set_InstanceProperty, value=" + value, instanceNumber);
				instanceField = value;
			}
		}

		static int staticField;

		public static int StaticProperty
		{
			get {
				Console.WriteLine("In get_StaticProperty");
				return staticField;
			}
			set {
				Console.WriteLine("In set_StaticProperty, value=" + value);
				staticField = value;
			}
		}

		static short shortField;

		public static short ShortProperty {
			get {
				Console.WriteLine("In get_ShortProperty");
				return shortField;
			}
			set {
				Console.WriteLine("In set_ShortProperty, value={0}", value);
				shortField = value;
			}
		}

		static byte byteField;

		public static byte ByteProperty {
			get {
				Console.WriteLine("In get_ByteProperty");
				return byteField;
			}
			set {
				Console.WriteLine("In set_ByteProperty, value={0}", value);
				byteField = value;
			}
		}

		IntPtr intPtrField = new IntPtr(IntPtr.Size == 8 ? long.MaxValue : int.MaxValue);

		public IntPtr IntPtrProperty {
			get {
				Console.WriteLine("In {0}.get_IntPtrProperty", instanceNumber);
				return intPtrField;
			}
			set {
				Console.WriteLine("In {0}.set_IntPtrProperty, value={1}", instanceNumber, value);
				intPtrField = value;
			}
		}

		public static Dictionary<string, int> GetDict()
		{
			Console.WriteLine("In GetDict()");
			return new Dictionary<string, int>() { { GetString(), 5 } };
		}

		static CompoundAssignment GetObject()
		{
			var obj = new CompoundAssignment();
			Console.WriteLine("In GetObject() (instance #{0})", obj.instanceNumber);
			return obj;
		}

		static string GetString()
		{
			Console.WriteLine("In GetString()");
			return "the string";
		}

		static void PreIncrementProperty()
		{
			Console.WriteLine("PreIncrementProperty:");
			Test(X(), ++new CompoundAssignment().InstanceProperty);
			Test(X(), ++StaticProperty);
		}

		static void PreIncrementIndexer()
		{
			Console.WriteLine("PreIncrementIndexer:");
			Test(X(), ++GetDict()[GetString()]);
		}

		static void CallTwice()
		{
			Console.WriteLine("CallTwice: instanceField:");
			GetObject().instanceField = GetObject().instanceField + 1;
			Test(X(), GetObject().instanceField = GetObject().instanceField + 1);
			Console.WriteLine("CallTwice: InstanceProperty:");
			GetObject().InstanceProperty = GetObject().InstanceProperty + 1;
			Test(X(), GetObject().InstanceProperty = GetObject().InstanceProperty + 1);
			Console.WriteLine("CallTwice: dict indexer:");
			GetDict()[GetString()] = GetDict()[GetString()] + 1;
			Test(X(), GetDict()[GetString()] = GetDict()[GetString()] + 1);
		}

		static void UnsignedShiftRightInstanceField()
		{
#if CS70
			ref int f = ref new CompoundAssignment().instanceField;
			Test(X(), f = (int)((uint)f >> 2));
#endif
		}

		static void UnsignedShiftRightStaticProperty()
		{
			Console.WriteLine("UnsignedShiftRightStaticProperty:");
			StaticProperty = -15;
			Test(X(), StaticProperty = (int)((uint)StaticProperty >> 2));

			ShortProperty = -20;
			ShortProperty = (short)((uint)StaticProperty >> 2);

			ShortProperty = -30;
			ShortProperty = (short)((ushort)StaticProperty >> 2);
		}

		static void DivideByBigValue()
		{
			Console.WriteLine("DivideByBigValue:");
			ByteProperty = 5;
			// can't use "ByteProperty /= (byte)(byte.MaxValue + 3)" because that would be division by 2.
			ByteProperty = (byte)(ByteProperty / (byte.MaxValue + 3));

			ByteProperty = 200;
			ByteProperty = (byte)(ByteProperty / Id(byte.MaxValue + 3));

			ShortProperty = short.MaxValue;
			ShortProperty = (short)(ShortProperty / (short.MaxValue + 3));
		}

		static void Overflow()
		{
			Console.WriteLine("Overflow:");
			ByteProperty = 0;
			ByteProperty = (byte)checked(ByteProperty + 300);
			try {
				ByteProperty = checked((byte)(ByteProperty + 300));
			} catch (OverflowException) {
				Console.WriteLine("Overflow OK");
			}

			ByteProperty = 200;
			ByteProperty = (byte)checked(ByteProperty + 100);
			ByteProperty = 201;
			try {
				ByteProperty = checked((byte)(ByteProperty + 100));
			} catch (OverflowException) {
				Console.WriteLine("Overflow OK");
			}
		}

		static T Id<T>(T val)
		{
			return val;
		}

		static void IntPtr_CompoundAssign()
		{
			Console.WriteLine("IntPtr_CompoundAssign:");
#if !MCS
			GetObject().IntPtrProperty -= 2;
			GetObject().IntPtrProperty += 2;
#endif
		}
	}
}