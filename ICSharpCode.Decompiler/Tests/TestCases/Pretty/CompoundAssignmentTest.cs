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
	public class CompoundAssignmentTest
	{
		private struct StructContainer
		{
			public bool HasIndex;
			public int Field;
		}
		
		private int test1;
		private int[] array1;
		private StructContainer field1;
		
		public static void Main()
		{
			
		}
		
		public void Int32_Local_Add(int i)
		{
			i++;
			Console.WriteLine(i++);
			Console.WriteLine(++i);
			i += 5;
			Console.WriteLine(i += 5);
		}
		
		public void Int32_Local_Sub(int i)
		{
			i--;
			Console.WriteLine(i--);
			Console.WriteLine(--i);
			i -= 5;
			Console.WriteLine(i -= 5);
		}
		
		public void Int32_Local_Mul(int i)
		{
			i *= 5;
			Console.WriteLine(i *= 5);
		}
		
		public void Int32_Local_Div(int i)
		{
			i /= 5;
			Console.WriteLine(i /= 5);
		}
		
		public void Int32_Local_Rem(int i)
		{
			i %= 5;
			Console.WriteLine(i %= 5);
		}
		
		public void Int32_Local_BitAnd(int i)
		{
			i &= 5;
			Console.WriteLine(i &= 5);
		}
		
		public void Int32_Local_BitOr(int i)
		{
			i |= 5;
			Console.WriteLine(i |= 5);
		}
		
		public void Int32_Local_BitXor(int i)
		{
			i ^= 5;
			Console.WriteLine(i ^= 5);
		}
		
		public void Int32_Local_ShiftLeft(int i)
		{
			i <<= 5;
			Console.WriteLine(i <<= 5);
		}
		
		public void Int32_Local_ShiftRight(int i)
		{
			i >>= 5;
			Console.WriteLine(i >>= 5);
		}
		
		public void IntegerWithInline(int i)
		{
			Console.WriteLine(i += 5);
			Console.WriteLine(i);
		}
		
		public void IntegerField(int i)
		{
			Console.WriteLine(this.test1 += i);
			Console.WriteLine(this.test1);
			Console.WriteLine(this.test1 -= i);
			Console.WriteLine(this.test1);
		}
		
		public void Array(int i)
		{
			Console.WriteLine(this.array1[i] += i);
			Console.WriteLine(this.array1[i * 2] += i * 2);
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
			return this.GetArray()[this.GetIndex()]++;
		}
		
		public void NestedField()
		{
			if (this.field1.HasIndex) {
				Console.WriteLine(this.field1.Field++);
			}
		}
	}
}
