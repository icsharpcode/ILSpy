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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class CompoundAssignmentTest
	{
		[Flags]
		private enum MyEnum
		{
			None = 0,
			One = 1,
			Two = 2,
			Four = 4
		}

		public enum ShortEnum : short
		{
			None,
			One,
			Two,
			Four = 4
		}

		private struct StructContainer
		{
			public bool HasIndex;
			public int Field;
		}
		
		public class MutableClass
		{
			public int Field;
			public short ShortField;
			
			public int Property {
				get;
				set;
			}

			public byte ByteProperty {
				get;
				set;
			}
			
			public uint this[string name] {
				get {
					return 0u;
				}
				set {
				}
			}
		}

		private class Item
		{
			public Item Self;
		}

		private int test1;
		private int[] array1;
		private StructContainer field1;
		private MyEnum enumField;
		private Dictionary<ushort, ushort> ushortDict = new Dictionary<ushort, ushort>();
		private ushort ushortField;
		private ShortEnum shortEnumField;
		public static int StaticField;
		public static short StaticShortField;

		public static int StaticProperty {
			get;
			set;
		}

		public static ShortEnum StaticShortProperty {
			get;
			set;
		}

		private MutableClass M()
		{
			return new MutableClass();
		}
		
		private int[,] Array()
		{
			return null;
		}
		
		private unsafe int* GetPointer()
		{
			return null;
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

		public bool IsUpperCaseA(char a)
		{
			return a == 'A';
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
		
		public int ArrayUsageWithMethods()
		{
			return this.GetArray()[this.GetIndex()]++;
		}
		
		public void NestedField()
		{
			if (this.field1.HasIndex) {
				Console.WriteLine(this.field1.Field *= 2);
				this.field1.Field++;
				Console.WriteLine(this.field1.Field++);
			}
		}
		
		public void Enum()
		{
			this.enumField |= MyEnum.Two;
			this.enumField &= ~MyEnum.Four;
			this.enumField += 2;
			this.enumField -= 3;
		}

		public void ShortEnumTest()
		{
			this.shortEnumField |= ShortEnum.Two;
			this.shortEnumField &= ShortEnum.Four;
			this.shortEnumField += 2;
			this.shortEnumField -= 3;
		}

		public int PreIncrementInAddition(int i, int j)
		{
			return i + ++j;
		}
		
		public int PreIncrementArrayElement(int[] array, int pos)
		{
			return --array[pos];
		}

		public int PostIncrementArrayElement(int[] array, int pos)
		{
			return array[pos]++;
		}

		public void IncrementArrayElement(int[] array, int pos)
		{
			array[pos]++;
		}

		public void DoubleArrayElement(int[] array, int pos)
		{
			array[pos] *= 2;
		}

		public int DoubleArrayElementAndReturn(int[] array, int pos)
		{
			return array[pos] *= 2;
		}

		public int PreIncrementArrayElementShort(short[] array, int pos)
		{
			return --array[pos];
		}

		public int PostIncrementArrayElementShort(short[] array, int pos)
		{
			return array[pos]++;
		}

		public void IncrementArrayElementShort(short[] array, int pos)
		{
			array[pos]++;
		}

		public void DoubleArrayElementShort(short[] array, int pos)
		{
			array[pos] *= 2;
		}

		public short DoubleArrayElementShortAndReturn(short[] array, int pos)
		{
			return array[pos] *= 2;
		}

		public int PreIncrementInstanceField()
		{
			return ++this.M().Field;
		}
		
		public int PostIncrementInstanceField()
		{
			return this.M().Field++;
		}

		public void IncrementInstanceField()
		{
			this.M().Field++;
		}

		public void DoubleInstanceField()
		{
			this.M().Field *= 2;
		}

		public int DoubleInstanceFieldAndReturn()
		{
			return this.M().Field *= 2;
		}

		public int PreIncrementInstanceField2(MutableClass m)
		{
			return ++m.Field;
		}
		
		public int PostIncrementInstanceField2(MutableClass m)
		{
			return m.Field++;
		}

		public void IncrementInstanceField2(MutableClass m)
		{
			m.Field++;
		}

		public int PreIncrementInstanceFieldShort()
		{
			return ++this.M().ShortField;
		}

		public int PostIncrementInstanceFieldShort()
		{
			return this.M().ShortField++;
		}

		public void IncrementInstanceFieldShort()
		{
			this.M().ShortField++;
		}

		public int PreIncrementInstanceProperty()
		{
			return ++this.M().Property;
		}

		public int PostIncrementInstanceProperty()
		{
			return this.M().Property++;
		}

		public void IncrementInstanceProperty()
		{
			this.M().Property++;
		}

		public void DoubleInstanceProperty()
		{
			this.M().Property *= 2;
		}

		public int DoubleInstancePropertyAndReturn()
		{
			return this.M().Property *= 2;
		}

		public int PreIncrementInstancePropertyByte()
		{
			return ++this.M().ByteProperty;
		}

		public int PostIncrementInstancePropertyByte()
		{
			return this.M().ByteProperty++;
		}

		public void IncrementInstancePropertyByte()
		{
			this.M().ByteProperty++;
		}

		public void DoubleInstancePropertyByte()
		{
			this.M().ByteProperty *= 2;
		}

		public int DoubleInstancePropertyByteAndReturn()
		{
			return this.M().ByteProperty *= 2;
		}

		public int PreIncrementStaticField()
		{
			return ++CompoundAssignmentTest.StaticField;
		}

		public int PostIncrementStaticField()
		{
			return CompoundAssignmentTest.StaticField++;
		}

		public void IncrementStaticField()
		{
			CompoundAssignmentTest.StaticField++;
		}

		public void DoubleStaticField()
		{
			CompoundAssignmentTest.StaticField *= 2;
		}

		public int DoubleStaticFieldAndReturn()
		{
			return CompoundAssignmentTest.StaticField *= 2;
		}

		public int PreIncrementStaticFieldShort()
		{
			return ++CompoundAssignmentTest.StaticShortField;
		}

		public int PostIncrementStaticFieldShort()
		{
			return CompoundAssignmentTest.StaticShortField++;
		}

		public void IncrementStaticFieldShort()
		{
			CompoundAssignmentTest.StaticShortField++;
		}

		public void DoubleStaticFieldShort()
		{
			CompoundAssignmentTest.StaticShortField *= 2;
		}

		public short DoubleStaticFieldAndReturnShort()
		{
			return CompoundAssignmentTest.StaticShortField *= 2;
		}

		public int PreIncrementStaticProperty()
		{
			return ++CompoundAssignmentTest.StaticProperty;
		}

		public int PostIncrementStaticProperty()
		{
			return CompoundAssignmentTest.StaticProperty++;
		}

		public void IncrementStaticProperty()
		{
			CompoundAssignmentTest.StaticProperty++;
		}

		public void DoubleStaticProperty()
		{
			CompoundAssignmentTest.StaticProperty *= 2;
		}

		public int DoubleStaticPropertyAndReturn()
		{
			return CompoundAssignmentTest.StaticProperty *= 2;
		}

		public ShortEnum PreIncrementStaticPropertyShort()
		{
			return ++CompoundAssignmentTest.StaticShortProperty;
		}

		public ShortEnum PostIncrementStaticPropertyShort()
		{
			return CompoundAssignmentTest.StaticShortProperty++;
		}

		public void IncrementStaticPropertyShort()
		{
			CompoundAssignmentTest.StaticShortProperty++;
		}
		
		private static Item GetItem(object obj)
		{
			return null;
		}

		private static void Issue882()
		{
			Item item = CompoundAssignmentTest.GetItem(null);
			item.Self = item;
		}

		private void Issue954(ref MyEnum a, MyEnum b)
		{
			// cannot decompile to: "a %= b;", because the % operator does not apply to enums
			a = (MyEnum)((int)a % (int)b);
			// same with enum field:
			this.enumField = (MyEnum)((int)this.enumField % (int)b);
		}

		private void Issue588(ushort val)
		{
			this.ushortDict.Add(this.ushortField++, val);
		}

		private void Issue1007(TimeSpan[] items, int startIndex, TimeSpan item)
		{
			int num = startIndex;
			items[num++] = item;
			items[num++] = item;
		}
	}
}
