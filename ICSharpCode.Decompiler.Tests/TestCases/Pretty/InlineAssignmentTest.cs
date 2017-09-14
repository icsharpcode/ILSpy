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
	public class InlineAssignmentTest
	{
		private int field1;
		private static InlineAssignmentTest field2;
		private int[] field3;
		
		public void SimpleInlineWithLocals()
		{
			int value;
			Console.WriteLine(value = 5);
			Console.WriteLine(value);
			InlineAssignmentTest value2;
			Console.WriteLine(value2 = new InlineAssignmentTest());
			Console.WriteLine(value2);
		}
		
		public void SimpleInlineWithFields()
		{
			Console.WriteLine(this.field1 = 5);
			Console.WriteLine(InlineAssignmentTest.field2 = new InlineAssignmentTest());
		}
		
		public void SimpleInlineWithFields2()
		{
			Console.WriteLine(this.field1 = 5);
			Console.WriteLine(this.field1);
			Console.WriteLine(InlineAssignmentTest.field2 = new InlineAssignmentTest());
			Console.WriteLine(InlineAssignmentTest.field2);
		}
		
//		public void ReadLoop1(TextReader r)
//		{
//			string V_0;
//			while ((V_0 = r.ReadLine()) != null)
//			{
//				Console.WriteLine(V_0);
//			}
//		}
		
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
			return this.field3[i] = 1;
		}
		
		public int GetIndex()
		{
			return new Random().Next(0, 100);
		}
		
		public int[] GetArray()
		{
			throw new NotImplementedException();
		}
		
		public int GetValue(int value)
		{
			return value;
		}
		
		public int ArrayUsageWithMethods()
		{
			return this.GetArray()[this.GetIndex()] = this.GetValue(this.GetIndex());
		}
	}
}
