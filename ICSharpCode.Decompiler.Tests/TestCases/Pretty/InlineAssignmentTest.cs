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
using System.IO;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class InlineAssignmentTest
	{
		private int field1;
		private static InlineAssignmentTest field2;
		private int[] field3;
		private short field4;

		public int InstanceProperty {
			get;
			set;
		}

		public static int StaticProperty {
			get;
			set;
		}

		public bool BoolProperty {
			get;
			set;
		}

		public void SimpleInlineWithLocals()
		{
			int index;
			Console.WriteLine(GetFormat(), index = GetIndex());
			Console.WriteLine(index);
			InlineAssignmentTest value;
			Console.WriteLine(GetFormat(), value = new InlineAssignmentTest());
			Console.WriteLine(value);
		}

		public void SimpleInlineWithFields()
		{
			Console.WriteLine(field1 = 5);
			Console.WriteLine(field2 = new InlineAssignmentTest());
		}

		public void SimpleInlineWithFields2()
		{
			Console.WriteLine(field1 = 5);
			Console.WriteLine(field1);
			Console.WriteLine(field2 = new InlineAssignmentTest());
			Console.WriteLine(field2);
			UseShort(field4 = 6);
			UseShort(field4 = -10000);
			UseShort(field4 = (short)field1);
			UseShort(field4 = UseShort(0));
			Console.WriteLine(field4);
		}

		public short UseShort(short s)
		{
			Console.WriteLine(s);
			return s;
		}

		public void ReadLoop1(TextReader r)
		{
			string value;
			while ((value = r.ReadLine()) != null)
			{
				Console.WriteLine(value);
			}
		}

		public void AccessArray(int[] a)
		{
			int num;
			Console.WriteLine(num = a[0]);
			Console.WriteLine(a[num] = num);
		}

		public int Return(ref int a)
		{
			return a = 3;
		}

		public int Array(int[] a, int i)
		{
			return a[i] = i;
		}

		public int Array2(int i)
		{
			return field3[i] = 1;
		}

		public int GetIndex()
		{
			return new Random().Next(0, 100);
		}

		public int[] GetArray()
		{
			throw new NotImplementedException();
		}

		public string GetFormat()
		{
			return "{0}";
		}

		public int GetValue(int value)
		{
			return value;
		}

		public int ArrayUsageWithMethods()
		{
			return GetArray()[GetIndex()] = GetValue(GetIndex());
		}

		public int StaticPropertyTest()
		{
			return StaticProperty = GetIndex();
		}

		public int InstancePropertyTest()
		{
			return InstanceProperty = GetIndex();
		}

		public bool BoolPropertyTest(object x)
		{
			return BoolProperty = (x != null);
		}
	}
}
