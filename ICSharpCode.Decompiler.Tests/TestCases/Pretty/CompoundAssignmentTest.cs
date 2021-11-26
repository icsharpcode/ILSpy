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
			None = 0,
			One = 1,
			Two = 2,
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

			public int Property { get; set; }

			public byte ByteProperty { get; set; }

			public bool BoolProperty { get; set; }

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

		public class CustomClass
		{
			public byte ByteField;
			public sbyte SbyteField;
			public short ShortField;
			public ushort UshortField;
			public int IntField;
			public uint UintField;
			public long LongField;
			public ulong UlongField;
			public CustomClass CustomClassField;
			public CustomStruct CustomStructField;

			public byte ByteProp { get; set; }
			public sbyte SbyteProp { get; set; }
			public short ShortProp { get; set; }
			public ushort UshortProp { get; set; }
			public int IntProp { get; set; }
			public uint UintProp { get; set; }
			public long LongProp { get; set; }
			public ulong UlongProp { get; set; }
			public string StringProp { get; set; }

			public CustomClass CustomClassProp { get; set; }
			public CustomStruct CustomStructProp { get; set; }

			public static CustomClass operator +(CustomClass lhs, CustomClass rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomClass operator +(CustomClass lhs, int rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomClass operator -(CustomClass lhs, CustomClass rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomClass operator *(CustomClass lhs, CustomClass rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomClass operator /(CustomClass lhs, CustomClass rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomClass operator %(CustomClass lhs, CustomClass rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomClass operator <<(CustomClass lhs, int rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomClass operator >>(CustomClass lhs, int rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomClass operator &(CustomClass lhs, CustomClass rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomClass operator |(CustomClass lhs, CustomClass rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomClass operator ^(CustomClass lhs, CustomClass rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomClass operator ++(CustomClass lhs)
			{
				throw new NotImplementedException();
			}
			public static CustomClass operator --(CustomClass lhs)
			{
				throw new NotImplementedException();
			}
		}

		public struct CustomStruct
		{
			public byte ByteField;
			public sbyte SbyteField;
			public short ShortField;
			public ushort UshortField;
			public int IntField;
			public uint UintField;
			public long LongField;
			public ulong UlongField;
			public CustomClass CustomClassField;

			public CustomClass CustomClassProp { get; set; }
			public byte ByteProp { get; set; }
			public sbyte SbyteProp { get; set; }
			public short ShortProp { get; set; }
			public ushort UshortProp { get; set; }
			public int IntProp { get; set; }
			public uint UintProp { get; set; }
			public long LongProp { get; set; }
			public ulong UlongProp { get; set; }

			public static CustomStruct operator +(CustomStruct lhs, CustomStruct rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomStruct operator -(CustomStruct lhs, CustomStruct rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomStruct operator *(CustomStruct lhs, CustomStruct rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomStruct operator /(CustomStruct lhs, CustomStruct rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomStruct operator %(CustomStruct lhs, CustomStruct rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomStruct operator <<(CustomStruct lhs, int rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomStruct operator >>(CustomStruct lhs, int rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomStruct operator &(CustomStruct lhs, CustomStruct rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomStruct operator |(CustomStruct lhs, CustomStruct rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomStruct operator ^(CustomStruct lhs, CustomStruct rhs)
			{
				throw new NotImplementedException();
			}
			public static CustomStruct operator ++(CustomStruct lhs)
			{
				throw new NotImplementedException();
			}
			public static CustomStruct operator --(CustomStruct lhs)
			{
				throw new NotImplementedException();
			}
		}

		public struct CustomStruct2
		{
			public CustomClass CustomClassField;
			public CustomStruct CustomStructField;

			public byte ByteField;
			public sbyte SbyteField;
			public short ShortField;
			public ushort UshortField;
			public int IntField;
			public uint UintField;
			public long LongField;
			public ulong UlongField;

			public CustomClass CustomClassProp { get; set; }
			public CustomStruct CustomStructProp { get; set; }
			public byte ByteProp { get; set; }
			public sbyte SbyteProp { get; set; }
			public short ShortProp { get; set; }
			public ushort UshortProp { get; set; }
			public int IntProp { get; set; }
			public uint UintProp { get; set; }
			public long LongProp { get; set; }
			public ulong UlongProp { get; set; }
		}

		private int test1;
		private int[] array1;
		private StructContainer field1;
		private MyEnum enumField;
		private Dictionary<ushort, ushort> ushortDict = new Dictionary<ushort, ushort>();
		private ShortEnum shortEnumField;
		public static int StaticField;
		public static short StaticShortField;

		private static CustomClass customClassField;
		private static CustomStruct customStructField;
		private static CustomStruct2 otherCustomStructField;
		private static byte byteField;
		private static sbyte sbyteField;
		private static short shortField;
		private static ushort ushortField;
		private static int intField;
		private static uint uintField;
		private static long longField;
		private static ulong ulongField;

		private static CustomClass CustomClassProp { get; set; }
		private static CustomStruct CustomStructProp { get; set; }
		private static byte ByteProp { get; set; }
		private static sbyte SbyteProp { get; set; }
		private static short ShortProp { get; set; }
		private static ushort UshortProp { get; set; }
		private static int IntProp { get; set; }
		private static uint UintProp { get; set; }
		private static long LongProp { get; set; }
		private static ulong UlongProp { get; set; }

		public static int StaticProperty { get; set; }

		public static ShortEnum StaticShortProperty { get; set; }

		public static string StaticStringProperty { get; set; }

		private static CustomStruct2 GetStruct()
		{
			throw new NotImplementedException();
		}
#if CS70
		private static ref CustomStruct2 GetRefStruct()
		{
			throw new NotImplementedException();
		}

		private static ref CustomStruct GetRefCustomStruct()
		{
			throw new NotImplementedException();
		}

		private static ref CustomClass GetRefCustomClass()
		{
			throw new NotImplementedException();
		}

		private static ref byte GetRefByte()
		{
			throw new NotImplementedException();
		}

		private static ref sbyte GetRefSbyte()
		{
			throw new NotImplementedException();
		}

		private static ref short GetRefShort()
		{
			throw new NotImplementedException();
		}

		private static ref int GetRefInt()
		{
			throw new NotImplementedException();
		}

		private static ref long GetRefLong()
		{
			throw new NotImplementedException();
		}

		private static ref ushort GetRefUshort()
		{
			throw new NotImplementedException();
		}

		private static ref uint GetRefUint()
		{
			throw new NotImplementedException();
		}

		private static ref ulong GetRefUlong()
		{
			throw new NotImplementedException();
		}
#endif

		private static CustomClass GetClass()
		{
			throw new NotImplementedException();
		}

		private static void X<T>(T result)
		{

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
			Console.WriteLine(test1 += i);
			Console.WriteLine(test1);
			Console.WriteLine(test1 -= i);
			Console.WriteLine(test1);
		}

		public void Array(int i)
		{
			Console.WriteLine(array1[i] += i);
			Console.WriteLine(array1[i * 2] += i * 2);
		}

		public int ArrayUsageWithMethods()
		{
			return GetArray()[GetIndex()]++;
		}

		public void NestedField()
		{
			if (field1.HasIndex)
			{
				Console.WriteLine(field1.Field *= 2);
				field1.Field++;
				Console.WriteLine(field1.Field++);
			}
		}

		public void Enum()
		{
			enumField |= MyEnum.Two;
			enumField &= ~MyEnum.Four;
			enumField += 2;
			enumField -= 3;
		}

		public void ShortEnumTest()
		{
			shortEnumField |= ShortEnum.Two;
			shortEnumField &= ShortEnum.Four;
			shortEnumField += 2;
			shortEnumField -= 3;
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
			return ++M().Field;
		}

		public int PostIncrementInstanceField()
		{
			return M().Field++;
		}

		public void IncrementInstanceField()
		{
			M().Field++;
		}

		public void DoubleInstanceField()
		{
			M().Field *= 2;
		}

		public int DoubleInstanceFieldAndReturn()
		{
			return M().Field *= 2;
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
			return ++M().ShortField;
		}

		public int PostIncrementInstanceFieldShort()
		{
			return M().ShortField++;
		}

		public void IncrementInstanceFieldShort()
		{
			M().ShortField++;
		}

		public int PreIncrementInstanceProperty()
		{
			return ++M().Property;
		}

		public int PostIncrementInstanceProperty()
		{
			return M().Property++;
		}

		public void IncrementInstanceProperty()
		{
			M().Property++;
		}

		public void DoubleInstanceProperty()
		{
			M().Property *= 2;
		}

		public int DoubleInstancePropertyAndReturn()
		{
			return M().Property *= 2;
		}

		public int PreIncrementInstancePropertyByte()
		{
			return ++M().ByteProperty;
		}

		public int PostIncrementInstancePropertyByte()
		{
			return M().ByteProperty++;
		}

		public void IncrementInstancePropertyByte()
		{
			M().ByteProperty++;
		}

		public void DoubleInstancePropertyByte()
		{
			M().ByteProperty *= 2;
		}

		public int DoubleInstancePropertyByteAndReturn()
		{
			return M().ByteProperty *= 2;
		}

		public void BitManipBoolProperty(bool b)
		{
			M().BoolProperty |= b;
			M().BoolProperty &= b;
			M().BoolProperty ^= b;
		}

		public bool BitOrBoolPropertyAndReturn(bool b)
		{
			return M().BoolProperty |= b;
		}

		public bool BitAndBoolPropertyAndReturn(bool b)
		{
			return M().BoolProperty &= b;
		}

		public int PreIncrementStaticField()
		{
			return ++StaticField;
		}

		public int PostIncrementStaticField()
		{
			return StaticField++;
		}

		public void IncrementStaticField()
		{
			StaticField++;
		}

		public void DoubleStaticField()
		{
			StaticField *= 2;
		}

		public int DoubleStaticFieldAndReturn()
		{
			return StaticField *= 2;
		}

		public int PreIncrementStaticFieldShort()
		{
			return ++StaticShortField;
		}

		public int PostIncrementStaticFieldShort()
		{
			return StaticShortField++;
		}

		public void IncrementStaticFieldShort()
		{
			StaticShortField++;
		}

		public void DoubleStaticFieldShort()
		{
			StaticShortField *= 2;
		}

		public short DoubleStaticFieldAndReturnShort()
		{
			return StaticShortField *= 2;
		}

		public int PreIncrementStaticProperty()
		{
			return ++StaticProperty;
		}

		public int PostIncrementStaticProperty()
		{
			return StaticProperty++;
		}

		public void IncrementStaticProperty()
		{
			StaticProperty++;
		}

		public void DoubleStaticProperty()
		{
			StaticProperty *= 2;
		}

		public int DoubleStaticPropertyAndReturn()
		{
			return StaticProperty *= 2;
		}

		public ShortEnum PreIncrementStaticPropertyShort()
		{
			return ++StaticShortProperty;
		}

		public ShortEnum PostIncrementStaticPropertyShort()
		{
			return StaticShortProperty++;
		}

		public void IncrementStaticPropertyShort()
		{
			StaticShortProperty++;
		}

		#region Generated Tests

		public static void ByteAddTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//p += 5;
			//l += 5;
			byteField += 5;
			ByteProp += 5;
			c.ByteField += 5;
			c.ByteProp += 5;
			s.ByteField += 5;
			s.ByteProp += 5;
			customClassField.ByteField += 5;
			customClassField.ByteProp += 5;
			otherCustomStructField.ByteField += 5;
			otherCustomStructField.ByteProp += 5;
			CustomClassProp.ByteField += 5;
			CustomClassProp.ByteProp += 5;
			GetClass().ByteField += 5;
			GetClass().ByteProp += 5;
#if CS70
			GetRefStruct().ByteField += 5;
			GetRefStruct().ByteProp += 5;
			GetRefByte() += 5;
#endif
		}

		public static void ByteSubtractTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//p -= 5;
			//l -= 5;
			byteField -= 5;
			ByteProp -= 5;
			c.ByteField -= 5;
			c.ByteProp -= 5;
			s.ByteField -= 5;
			s.ByteProp -= 5;
			customClassField.ByteField -= 5;
			customClassField.ByteProp -= 5;
			otherCustomStructField.ByteField -= 5;
			otherCustomStructField.ByteProp -= 5;
			CustomClassProp.ByteField -= 5;
			CustomClassProp.ByteProp -= 5;
			GetClass().ByteField -= 5;
			GetClass().ByteProp -= 5;
#if CS70
			GetRefStruct().ByteField -= 5;
			GetRefStruct().ByteProp -= 5;
			GetRefByte() -= 5;
#endif
		}

		public static void ByteMultiplyTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//p *= 5;
			//l *= 5;
			byteField *= 5;
			ByteProp *= 5;
			c.ByteField *= 5;
			c.ByteProp *= 5;
			s.ByteField *= 5;
			s.ByteProp *= 5;
			customClassField.ByteField *= 5;
			customClassField.ByteProp *= 5;
			otherCustomStructField.ByteField *= 5;
			otherCustomStructField.ByteProp *= 5;
			CustomClassProp.ByteField *= 5;
			CustomClassProp.ByteProp *= 5;
			GetClass().ByteField *= 5;
			GetClass().ByteProp *= 5;
#if CS70
			GetRefStruct().ByteField *= 5;
			GetRefStruct().ByteProp *= 5;
			GetRefByte() *= 5;
#endif
		}

		public static void ByteDivideTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//p /= 5;
			//l /= 5;
			byteField /= 5;
			ByteProp /= 5;
			c.ByteField /= 5;
			c.ByteProp /= 5;
			s.ByteField /= 5;
			s.ByteProp /= 5;
			customClassField.ByteField /= 5;
			customClassField.ByteProp /= 5;
			otherCustomStructField.ByteField /= 5;
			otherCustomStructField.ByteProp /= 5;
			CustomClassProp.ByteField /= 5;
			CustomClassProp.ByteProp /= 5;
			GetClass().ByteField /= 5;
			GetClass().ByteProp /= 5;
#if CS70
			GetRefStruct().ByteField /= 5;
			GetRefStruct().ByteProp /= 5;
			GetRefByte() /= 5;
#endif
		}

		public static void ByteModulusTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//p %= 5;
			//l %= 5;
			byteField %= 5;
			ByteProp %= 5;
			c.ByteField %= 5;
			c.ByteProp %= 5;
			s.ByteField %= 5;
			s.ByteProp %= 5;
			customClassField.ByteField %= 5;
			customClassField.ByteProp %= 5;
			otherCustomStructField.ByteField %= 5;
			otherCustomStructField.ByteProp %= 5;
			CustomClassProp.ByteField %= 5;
			CustomClassProp.ByteProp %= 5;
			GetClass().ByteField %= 5;
			GetClass().ByteProp %= 5;
#if CS70
			GetRefStruct().ByteField %= 5;
			GetRefStruct().ByteProp %= 5;
			GetRefByte() %= 5;
#endif
		}

		public static void ByteLeftShiftTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//p <<= 5;
			//l <<= 5;
			byteField <<= 5;
			ByteProp <<= 5;
			c.ByteField <<= 5;
			c.ByteProp <<= 5;
			s.ByteField <<= 5;
			s.ByteProp <<= 5;
			customClassField.ByteField <<= 5;
			customClassField.ByteProp <<= 5;
			otherCustomStructField.ByteField <<= 5;
			otherCustomStructField.ByteProp <<= 5;
			CustomClassProp.ByteField <<= 5;
			CustomClassProp.ByteProp <<= 5;
			GetClass().ByteField <<= 5;
			GetClass().ByteProp <<= 5;
#if CS70
			GetRefStruct().ByteField <<= 5;
			GetRefStruct().ByteProp <<= 5;
			GetRefByte() <<= 5;
#endif
		}

		public static void ByteRightShiftTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//p >>= 5;
			//l >>= 5;
			byteField >>= 5;
			ByteProp >>= 5;
			c.ByteField >>= 5;
			c.ByteProp >>= 5;
			s.ByteField >>= 5;
			s.ByteProp >>= 5;
			customClassField.ByteField >>= 5;
			customClassField.ByteProp >>= 5;
			otherCustomStructField.ByteField >>= 5;
			otherCustomStructField.ByteProp >>= 5;
			CustomClassProp.ByteField >>= 5;
			CustomClassProp.ByteProp >>= 5;
			GetClass().ByteField >>= 5;
			GetClass().ByteProp >>= 5;
#if CS70
			GetRefStruct().ByteField >>= 5;
			GetRefStruct().ByteProp >>= 5;
			GetRefByte() >>= 5;
#endif
		}

		public static void ByteBitAndTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//p &= 5;
			//l &= 5;
			byteField &= 5;
			ByteProp &= 5;
			c.ByteField &= 5;
			c.ByteProp &= 5;
			s.ByteField &= 5;
			s.ByteProp &= 5;
			customClassField.ByteField &= 5;
			customClassField.ByteProp &= 5;
			otherCustomStructField.ByteField &= 5;
			otherCustomStructField.ByteProp &= 5;
			CustomClassProp.ByteField &= 5;
			CustomClassProp.ByteProp &= 5;
			GetClass().ByteField &= 5;
			GetClass().ByteProp &= 5;
#if CS70
			GetRefStruct().ByteField &= 5;
			GetRefStruct().ByteProp &= 5;
			GetRefByte() &= 5;
#endif
		}

		public static void ByteBitOrTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//p |= 5;
			//l |= 5;
			byteField |= 5;
			ByteProp |= 5;
			c.ByteField |= 5;
			c.ByteProp |= 5;
			s.ByteField |= 5;
			s.ByteProp |= 5;
			customClassField.ByteField |= 5;
			customClassField.ByteProp |= 5;
			otherCustomStructField.ByteField |= 5;
			otherCustomStructField.ByteProp |= 5;
			CustomClassProp.ByteField |= 5;
			CustomClassProp.ByteProp |= 5;
			GetClass().ByteField |= 5;
			GetClass().ByteProp |= 5;
#if CS70
			GetRefStruct().ByteField |= 5;
			GetRefStruct().ByteProp |= 5;
			GetRefByte() |= 5;
#endif
		}

		public static void ByteBitXorTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//p ^= 5;
			//l ^= 5;
			byteField ^= 5;
			ByteProp ^= 5;
			c.ByteField ^= 5;
			c.ByteProp ^= 5;
			s.ByteField ^= 5;
			s.ByteProp ^= 5;
			customClassField.ByteField ^= 5;
			customClassField.ByteProp ^= 5;
			otherCustomStructField.ByteField ^= 5;
			otherCustomStructField.ByteProp ^= 5;
			CustomClassProp.ByteField ^= 5;
			CustomClassProp.ByteProp ^= 5;
			GetClass().ByteField ^= 5;
			GetClass().ByteProp ^= 5;
#if CS70
			GetRefStruct().ByteField ^= 5;
			GetRefStruct().ByteProp ^= 5;
			GetRefByte() ^= 5;
#endif
		}

		public static void BytePostIncTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//X(p++);
			//X(l++);
			X(byteField++);
			X(ByteProp++);
			X(c.ByteField++);
			X(c.ByteProp++);
			X(s.ByteField++);
			X(s.ByteProp++);
			X(customClassField.ByteField++);
			X(customClassField.ByteProp++);
			X(otherCustomStructField.ByteField++);
			X(otherCustomStructField.ByteProp++);
			X(CustomClassProp.ByteField++);
			X(CustomClassProp.ByteProp++);
			X(GetClass().ByteField++);
			X(GetClass().ByteProp++);
#if CS70
			X(GetRefStruct().ByteField++);
			X(GetRefStruct().ByteProp++);
			X(GetRefByte()++);
#endif
		}

		public static void BytePreIncTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//X(++p);
			//X(++l);
			X(++byteField);
			X(++ByteProp);
			X(++c.ByteField);
			X(++c.ByteProp);
			X(++s.ByteField);
			X(++s.ByteProp);
			X(++customClassField.ByteField);
			X(++customClassField.ByteProp);
			X(++otherCustomStructField.ByteField);
			X(++otherCustomStructField.ByteProp);
			X(++CustomClassProp.ByteField);
			X(++CustomClassProp.ByteProp);
			X(++GetClass().ByteField);
			X(++GetClass().ByteProp);
#if CS70
			X(++GetRefStruct().ByteField);
			X(++GetRefStruct().ByteProp);
			X(++GetRefByte());
#endif
		}
		public static void BytePostDecTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//X(p--);
			//X(l--);
			X(byteField--);
			X(ByteProp--);
			X(c.ByteField--);
			X(c.ByteProp--);
			X(s.ByteField--);
			X(s.ByteProp--);
			X(customClassField.ByteField--);
			X(customClassField.ByteProp--);
			X(otherCustomStructField.ByteField--);
			X(otherCustomStructField.ByteProp--);
			X(CustomClassProp.ByteField--);
			X(CustomClassProp.ByteProp--);
			X(GetClass().ByteField--);
			X(GetClass().ByteProp--);
#if CS70
			X(GetRefStruct().ByteField--);
			X(GetRefStruct().ByteProp--);
			X(GetRefByte()--);
#endif
		}

		public static void BytePreDecTest(byte p, CustomClass c, CustomStruct2 s)
		{
			//byte l = 0;
			//X(--p);
			//X(--l);
			X(--byteField);
			X(--ByteProp);
			X(--c.ByteField);
			X(--c.ByteProp);
			X(--s.ByteField);
			X(--s.ByteProp);
			X(--customClassField.ByteField);
			X(--customClassField.ByteProp);
			X(--otherCustomStructField.ByteField);
			X(--otherCustomStructField.ByteProp);
			X(--CustomClassProp.ByteField);
			X(--CustomClassProp.ByteProp);
			X(--GetClass().ByteField);
			X(--GetClass().ByteProp);
#if CS70
			X(--GetRefStruct().ByteField);
			X(--GetRefStruct().ByteProp);
			X(--GetRefByte());
#endif
		}
		public static void SbyteAddTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//p += 5;
			//l += 5;
			sbyteField += 5;
			SbyteProp += 5;
			c.SbyteField += 5;
			c.SbyteProp += 5;
			s.SbyteField += 5;
			s.SbyteProp += 5;
			customClassField.SbyteField += 5;
			customClassField.SbyteProp += 5;
			otherCustomStructField.SbyteField += 5;
			otherCustomStructField.SbyteProp += 5;
			CustomClassProp.SbyteField += 5;
			CustomClassProp.SbyteProp += 5;
			GetClass().SbyteField += 5;
			GetClass().SbyteProp += 5;
#if CS70
			GetRefStruct().SbyteField += 5;
			GetRefStruct().SbyteProp += 5;
			GetRefSbyte() += 5;
#endif
		}

		public static void SbyteSubtractTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//p -= 5;
			//l -= 5;
			sbyteField -= 5;
			SbyteProp -= 5;
			c.SbyteField -= 5;
			c.SbyteProp -= 5;
			s.SbyteField -= 5;
			s.SbyteProp -= 5;
			customClassField.SbyteField -= 5;
			customClassField.SbyteProp -= 5;
			otherCustomStructField.SbyteField -= 5;
			otherCustomStructField.SbyteProp -= 5;
			CustomClassProp.SbyteField -= 5;
			CustomClassProp.SbyteProp -= 5;
			GetClass().SbyteField -= 5;
			GetClass().SbyteProp -= 5;
#if CS70
			GetRefStruct().SbyteField -= 5;
			GetRefStruct().SbyteProp -= 5;
			GetRefSbyte() -= 5;
#endif
		}

		public static void SbyteMultiplyTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//p *= 5;
			//l *= 5;
			sbyteField *= 5;
			SbyteProp *= 5;
			c.SbyteField *= 5;
			c.SbyteProp *= 5;
			s.SbyteField *= 5;
			s.SbyteProp *= 5;
			customClassField.SbyteField *= 5;
			customClassField.SbyteProp *= 5;
			otherCustomStructField.SbyteField *= 5;
			otherCustomStructField.SbyteProp *= 5;
			CustomClassProp.SbyteField *= 5;
			CustomClassProp.SbyteProp *= 5;
			GetClass().SbyteField *= 5;
			GetClass().SbyteProp *= 5;
#if CS70
			GetRefStruct().SbyteField *= 5;
			GetRefStruct().SbyteProp *= 5;
			GetRefSbyte() *= 5;
#endif
		}

		public static void SbyteDivideTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//p /= 5;
			//l /= 5;
			sbyteField /= 5;
			SbyteProp /= 5;
			c.SbyteField /= 5;
			c.SbyteProp /= 5;
			s.SbyteField /= 5;
			s.SbyteProp /= 5;
			customClassField.SbyteField /= 5;
			customClassField.SbyteProp /= 5;
			otherCustomStructField.SbyteField /= 5;
			otherCustomStructField.SbyteProp /= 5;
			CustomClassProp.SbyteField /= 5;
			CustomClassProp.SbyteProp /= 5;
			GetClass().SbyteField /= 5;
			GetClass().SbyteProp /= 5;
#if CS70
			GetRefStruct().SbyteField /= 5;
			GetRefStruct().SbyteProp /= 5;
			GetRefSbyte() /= 5;
#endif
		}

		public static void SbyteModulusTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//p %= 5;
			//l %= 5;
			sbyteField %= 5;
			SbyteProp %= 5;
			c.SbyteField %= 5;
			c.SbyteProp %= 5;
			s.SbyteField %= 5;
			s.SbyteProp %= 5;
			customClassField.SbyteField %= 5;
			customClassField.SbyteProp %= 5;
			otherCustomStructField.SbyteField %= 5;
			otherCustomStructField.SbyteProp %= 5;
			CustomClassProp.SbyteField %= 5;
			CustomClassProp.SbyteProp %= 5;
			GetClass().SbyteField %= 5;
			GetClass().SbyteProp %= 5;
#if CS70
			GetRefStruct().SbyteField %= 5;
			GetRefStruct().SbyteProp %= 5;
			GetRefSbyte() %= 5;
#endif
		}

		public static void SbyteLeftShiftTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//p <<= 5;
			//l <<= 5;
			sbyteField <<= 5;
			SbyteProp <<= 5;
			c.SbyteField <<= 5;
			c.SbyteProp <<= 5;
			s.SbyteField <<= 5;
			s.SbyteProp <<= 5;
			customClassField.SbyteField <<= 5;
			customClassField.SbyteProp <<= 5;
			otherCustomStructField.SbyteField <<= 5;
			otherCustomStructField.SbyteProp <<= 5;
			CustomClassProp.SbyteField <<= 5;
			CustomClassProp.SbyteProp <<= 5;
			GetClass().SbyteField <<= 5;
			GetClass().SbyteProp <<= 5;
#if CS70
			GetRefStruct().SbyteField <<= 5;
			GetRefStruct().SbyteProp <<= 5;
			GetRefSbyte() <<= 5;
#endif
		}

		public static void SbyteRightShiftTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//p >>= 5;
			//l >>= 5;
			sbyteField >>= 5;
			SbyteProp >>= 5;
			c.SbyteField >>= 5;
			c.SbyteProp >>= 5;
			s.SbyteField >>= 5;
			s.SbyteProp >>= 5;
			customClassField.SbyteField >>= 5;
			customClassField.SbyteProp >>= 5;
			otherCustomStructField.SbyteField >>= 5;
			otherCustomStructField.SbyteProp >>= 5;
			CustomClassProp.SbyteField >>= 5;
			CustomClassProp.SbyteProp >>= 5;
			GetClass().SbyteField >>= 5;
			GetClass().SbyteProp >>= 5;
#if CS70
			GetRefStruct().SbyteField >>= 5;
			GetRefStruct().SbyteProp >>= 5;
			GetRefSbyte() >>= 5;
#endif
		}

		public static void SbyteBitAndTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//p &= 5;
			//l &= 5;
			sbyteField &= 5;
			SbyteProp &= 5;
			c.SbyteField &= 5;
			c.SbyteProp &= 5;
			s.SbyteField &= 5;
			s.SbyteProp &= 5;
			customClassField.SbyteField &= 5;
			customClassField.SbyteProp &= 5;
			otherCustomStructField.SbyteField &= 5;
			otherCustomStructField.SbyteProp &= 5;
			CustomClassProp.SbyteField &= 5;
			CustomClassProp.SbyteProp &= 5;
			GetClass().SbyteField &= 5;
			GetClass().SbyteProp &= 5;
#if CS70
			GetRefStruct().SbyteField &= 5;
			GetRefStruct().SbyteProp &= 5;
			GetRefSbyte() &= 5;
#endif
		}

		public static void SbyteBitOrTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//p |= 5;
			//l |= 5;
			sbyteField |= 5;
			SbyteProp |= 5;
			c.SbyteField |= 5;
			c.SbyteProp |= 5;
			s.SbyteField |= 5;
			s.SbyteProp |= 5;
			customClassField.SbyteField |= 5;
			customClassField.SbyteProp |= 5;
			otherCustomStructField.SbyteField |= 5;
			otherCustomStructField.SbyteProp |= 5;
			CustomClassProp.SbyteField |= 5;
			CustomClassProp.SbyteProp |= 5;
			GetClass().SbyteField |= 5;
			GetClass().SbyteProp |= 5;
#if CS70
			GetRefStruct().SbyteField |= 5;
			GetRefStruct().SbyteProp |= 5;
			GetRefSbyte() |= 5;
#endif
		}

		public static void SbyteBitXorTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//p ^= 5;
			//l ^= 5;
			sbyteField ^= 5;
			SbyteProp ^= 5;
			c.SbyteField ^= 5;
			c.SbyteProp ^= 5;
			s.SbyteField ^= 5;
			s.SbyteProp ^= 5;
			customClassField.SbyteField ^= 5;
			customClassField.SbyteProp ^= 5;
			otherCustomStructField.SbyteField ^= 5;
			otherCustomStructField.SbyteProp ^= 5;
			CustomClassProp.SbyteField ^= 5;
			CustomClassProp.SbyteProp ^= 5;
			GetClass().SbyteField ^= 5;
			GetClass().SbyteProp ^= 5;
#if CS70
			GetRefStruct().SbyteField ^= 5;
			GetRefStruct().SbyteProp ^= 5;
			GetRefSbyte() ^= 5;
#endif
		}

		public static void SbytePostIncTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//X(p++);
			//X(l++);
			X(sbyteField++);
			X(SbyteProp++);
			X(c.SbyteField++);
			X(c.SbyteProp++);
			X(s.SbyteField++);
			X(s.SbyteProp++);
			X(customClassField.SbyteField++);
			X(customClassField.SbyteProp++);
			X(otherCustomStructField.SbyteField++);
			X(otherCustomStructField.SbyteProp++);
			X(CustomClassProp.SbyteField++);
			X(CustomClassProp.SbyteProp++);
			X(GetClass().SbyteField++);
			X(GetClass().SbyteProp++);
#if CS70
			X(GetRefStruct().SbyteField++);
			X(GetRefStruct().SbyteProp++);
			X(GetRefSbyte()++);
#endif
		}

		public static void SbytePreIncTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//X(++p);
			//X(++l);
			X(++sbyteField);
			X(++SbyteProp);
			X(++c.SbyteField);
			X(++c.SbyteProp);
			X(++s.SbyteField);
			X(++s.SbyteProp);
			X(++customClassField.SbyteField);
			X(++customClassField.SbyteProp);
			X(++otherCustomStructField.SbyteField);
			X(++otherCustomStructField.SbyteProp);
			X(++CustomClassProp.SbyteField);
			X(++CustomClassProp.SbyteProp);
			X(++GetClass().SbyteField);
			X(++GetClass().SbyteProp);
#if CS70
			X(++GetRefStruct().SbyteField);
			X(++GetRefStruct().SbyteProp);
			X(++GetRefSbyte());
#endif
		}
		public static void SbytePostDecTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//X(p--);
			//X(l--);
			X(sbyteField--);
			X(SbyteProp--);
			X(c.SbyteField--);
			X(c.SbyteProp--);
			X(s.SbyteField--);
			X(s.SbyteProp--);
			X(customClassField.SbyteField--);
			X(customClassField.SbyteProp--);
			X(otherCustomStructField.SbyteField--);
			X(otherCustomStructField.SbyteProp--);
			X(CustomClassProp.SbyteField--);
			X(CustomClassProp.SbyteProp--);
			X(GetClass().SbyteField--);
			X(GetClass().SbyteProp--);
#if CS70
			X(GetRefStruct().SbyteField--);
			X(GetRefStruct().SbyteProp--);
			X(GetRefSbyte()--);
#endif
		}

		public static void SbytePreDecTest(sbyte p, CustomClass c, CustomStruct2 s)
		{
			//sbyte l = 0;
			//X(--p);
			//X(--l);
			X(--sbyteField);
			X(--SbyteProp);
			X(--c.SbyteField);
			X(--c.SbyteProp);
			X(--s.SbyteField);
			X(--s.SbyteProp);
			X(--customClassField.SbyteField);
			X(--customClassField.SbyteProp);
			X(--otherCustomStructField.SbyteField);
			X(--otherCustomStructField.SbyteProp);
			X(--CustomClassProp.SbyteField);
			X(--CustomClassProp.SbyteProp);
			X(--GetClass().SbyteField);
			X(--GetClass().SbyteProp);
#if CS70
			X(--GetRefStruct().SbyteField);
			X(--GetRefStruct().SbyteProp);
			X(--GetRefSbyte());
#endif
		}
		public static void ShortAddTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//p += 5;
			//l += 5;
			shortField += 5;
			ShortProp += 5;
			c.ShortField += 5;
			c.ShortProp += 5;
			s.ShortField += 5;
			s.ShortProp += 5;
			customClassField.ShortField += 5;
			customClassField.ShortProp += 5;
			otherCustomStructField.ShortField += 5;
			otherCustomStructField.ShortProp += 5;
			CustomClassProp.ShortField += 5;
			CustomClassProp.ShortProp += 5;
			GetClass().ShortField += 5;
			GetClass().ShortProp += 5;
#if CS70
			GetRefStruct().ShortField += 5;
			GetRefStruct().ShortProp += 5;
			GetRefShort() += 5;
#endif
		}

		public static void ShortSubtractTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//p -= 5;
			//l -= 5;
			shortField -= 5;
			ShortProp -= 5;
			c.ShortField -= 5;
			c.ShortProp -= 5;
			s.ShortField -= 5;
			s.ShortProp -= 5;
			customClassField.ShortField -= 5;
			customClassField.ShortProp -= 5;
			otherCustomStructField.ShortField -= 5;
			otherCustomStructField.ShortProp -= 5;
			CustomClassProp.ShortField -= 5;
			CustomClassProp.ShortProp -= 5;
			GetClass().ShortField -= 5;
			GetClass().ShortProp -= 5;
#if CS70
			GetRefStruct().ShortField -= 5;
			GetRefStruct().ShortProp -= 5;
			GetRefShort() -= 5;
#endif
		}

		public static void ShortMultiplyTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//p *= 5;
			//l *= 5;
			shortField *= 5;
			ShortProp *= 5;
			c.ShortField *= 5;
			c.ShortProp *= 5;
			s.ShortField *= 5;
			s.ShortProp *= 5;
			customClassField.ShortField *= 5;
			customClassField.ShortProp *= 5;
			otherCustomStructField.ShortField *= 5;
			otherCustomStructField.ShortProp *= 5;
			CustomClassProp.ShortField *= 5;
			CustomClassProp.ShortProp *= 5;
			GetClass().ShortField *= 5;
			GetClass().ShortProp *= 5;
#if CS70
			GetRefStruct().ShortField *= 5;
			GetRefStruct().ShortProp *= 5;
			GetRefShort() *= 5;
#endif
		}

		public static void ShortDivideTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//p /= 5;
			//l /= 5;
			shortField /= 5;
			ShortProp /= 5;
			c.ShortField /= 5;
			c.ShortProp /= 5;
			s.ShortField /= 5;
			s.ShortProp /= 5;
			customClassField.ShortField /= 5;
			customClassField.ShortProp /= 5;
			otherCustomStructField.ShortField /= 5;
			otherCustomStructField.ShortProp /= 5;
			CustomClassProp.ShortField /= 5;
			CustomClassProp.ShortProp /= 5;
			GetClass().ShortField /= 5;
			GetClass().ShortProp /= 5;
#if CS70
			GetRefStruct().ShortField /= 5;
			GetRefStruct().ShortProp /= 5;
			GetRefShort() /= 5;
#endif
		}

		public static void ShortModulusTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//p %= 5;
			//l %= 5;
			shortField %= 5;
			ShortProp %= 5;
			c.ShortField %= 5;
			c.ShortProp %= 5;
			s.ShortField %= 5;
			s.ShortProp %= 5;
			customClassField.ShortField %= 5;
			customClassField.ShortProp %= 5;
			otherCustomStructField.ShortField %= 5;
			otherCustomStructField.ShortProp %= 5;
			CustomClassProp.ShortField %= 5;
			CustomClassProp.ShortProp %= 5;
			GetClass().ShortField %= 5;
			GetClass().ShortProp %= 5;
#if CS70
			GetRefStruct().ShortField %= 5;
			GetRefStruct().ShortProp %= 5;
			GetRefShort() %= 5;
#endif
		}

		public static void ShortLeftShiftTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//p <<= 5;
			//l <<= 5;
			shortField <<= 5;
			ShortProp <<= 5;
			c.ShortField <<= 5;
			c.ShortProp <<= 5;
			s.ShortField <<= 5;
			s.ShortProp <<= 5;
			customClassField.ShortField <<= 5;
			customClassField.ShortProp <<= 5;
			otherCustomStructField.ShortField <<= 5;
			otherCustomStructField.ShortProp <<= 5;
			CustomClassProp.ShortField <<= 5;
			CustomClassProp.ShortProp <<= 5;
			GetClass().ShortField <<= 5;
			GetClass().ShortProp <<= 5;
#if CS70
			GetRefStruct().ShortField <<= 5;
			GetRefStruct().ShortProp <<= 5;
			GetRefShort() <<= 5;
#endif
		}

		public static void ShortRightShiftTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//p >>= 5;
			//l >>= 5;
			shortField >>= 5;
			ShortProp >>= 5;
			c.ShortField >>= 5;
			c.ShortProp >>= 5;
			s.ShortField >>= 5;
			s.ShortProp >>= 5;
			customClassField.ShortField >>= 5;
			customClassField.ShortProp >>= 5;
			otherCustomStructField.ShortField >>= 5;
			otherCustomStructField.ShortProp >>= 5;
			CustomClassProp.ShortField >>= 5;
			CustomClassProp.ShortProp >>= 5;
			GetClass().ShortField >>= 5;
			GetClass().ShortProp >>= 5;
#if CS70
			GetRefStruct().ShortField >>= 5;
			GetRefStruct().ShortProp >>= 5;
			GetRefShort() >>= 5;
#endif
		}

		public static void ShortBitAndTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//p &= 5;
			//l &= 5;
			shortField &= 5;
			ShortProp &= 5;
			c.ShortField &= 5;
			c.ShortProp &= 5;
			s.ShortField &= 5;
			s.ShortProp &= 5;
			customClassField.ShortField &= 5;
			customClassField.ShortProp &= 5;
			otherCustomStructField.ShortField &= 5;
			otherCustomStructField.ShortProp &= 5;
			CustomClassProp.ShortField &= 5;
			CustomClassProp.ShortProp &= 5;
			GetClass().ShortField &= 5;
			GetClass().ShortProp &= 5;
#if CS70
			GetRefStruct().ShortField &= 5;
			GetRefStruct().ShortProp &= 5;
			GetRefShort() &= 5;
#endif
		}

		public static void ShortBitOrTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//p |= 5;
			//l |= 5;
			shortField |= 5;
			ShortProp |= 5;
			c.ShortField |= 5;
			c.ShortProp |= 5;
			s.ShortField |= 5;
			s.ShortProp |= 5;
			customClassField.ShortField |= 5;
			customClassField.ShortProp |= 5;
			otherCustomStructField.ShortField |= 5;
			otherCustomStructField.ShortProp |= 5;
			CustomClassProp.ShortField |= 5;
			CustomClassProp.ShortProp |= 5;
			GetClass().ShortField |= 5;
			GetClass().ShortProp |= 5;
#if CS70
			GetRefStruct().ShortField |= 5;
			GetRefStruct().ShortProp |= 5;
			GetRefShort() |= 5;
#endif
		}

		public static void ShortBitXorTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//p ^= 5;
			//l ^= 5;
			shortField ^= 5;
			ShortProp ^= 5;
			c.ShortField ^= 5;
			c.ShortProp ^= 5;
			s.ShortField ^= 5;
			s.ShortProp ^= 5;
			customClassField.ShortField ^= 5;
			customClassField.ShortProp ^= 5;
			otherCustomStructField.ShortField ^= 5;
			otherCustomStructField.ShortProp ^= 5;
			CustomClassProp.ShortField ^= 5;
			CustomClassProp.ShortProp ^= 5;
			GetClass().ShortField ^= 5;
			GetClass().ShortProp ^= 5;
#if CS70
			GetRefStruct().ShortField ^= 5;
			GetRefStruct().ShortProp ^= 5;
			GetRefShort() ^= 5;
#endif
		}

		public static void ShortPostIncTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//X(p++);
			//X(l++);
			X(shortField++);
			X(ShortProp++);
			X(c.ShortField++);
			X(c.ShortProp++);
			X(s.ShortField++);
			X(s.ShortProp++);
			X(customClassField.ShortField++);
			X(customClassField.ShortProp++);
			X(otherCustomStructField.ShortField++);
			X(otherCustomStructField.ShortProp++);
			X(CustomClassProp.ShortField++);
			X(CustomClassProp.ShortProp++);
			X(GetClass().ShortField++);
			X(GetClass().ShortProp++);
#if CS70
			X(GetRefStruct().ShortField++);
			X(GetRefStruct().ShortProp++);
			X(GetRefShort()++);
#endif
		}

		public static void ShortPreIncTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//X(++p);
			//X(++l);
			X(++shortField);
			X(++ShortProp);
			X(++c.ShortField);
			X(++c.ShortProp);
			X(++s.ShortField);
			X(++s.ShortProp);
			X(++customClassField.ShortField);
			X(++customClassField.ShortProp);
			X(++otherCustomStructField.ShortField);
			X(++otherCustomStructField.ShortProp);
			X(++CustomClassProp.ShortField);
			X(++CustomClassProp.ShortProp);
			X(++GetClass().ShortField);
			X(++GetClass().ShortProp);
#if CS70
			X(++GetRefStruct().ShortField);
			X(++GetRefStruct().ShortProp);
			X(++GetRefShort());
#endif
		}
		public static void ShortPostDecTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//X(p--);
			//X(l--);
			X(shortField--);
			X(ShortProp--);
			X(c.ShortField--);
			X(c.ShortProp--);
			X(s.ShortField--);
			X(s.ShortProp--);
			X(customClassField.ShortField--);
			X(customClassField.ShortProp--);
			X(otherCustomStructField.ShortField--);
			X(otherCustomStructField.ShortProp--);
			X(CustomClassProp.ShortField--);
			X(CustomClassProp.ShortProp--);
			X(GetClass().ShortField--);
			X(GetClass().ShortProp--);
#if CS70
			X(GetRefStruct().ShortField--);
			X(GetRefStruct().ShortProp--);
			X(GetRefShort()--);
#endif
		}

		public static void ShortPreDecTest(short p, CustomClass c, CustomStruct2 s)
		{
			//short l = 0;
			//X(--p);
			//X(--l);
			X(--shortField);
			X(--ShortProp);
			X(--c.ShortField);
			X(--c.ShortProp);
			X(--s.ShortField);
			X(--s.ShortProp);
			X(--customClassField.ShortField);
			X(--customClassField.ShortProp);
			X(--otherCustomStructField.ShortField);
			X(--otherCustomStructField.ShortProp);
			X(--CustomClassProp.ShortField);
			X(--CustomClassProp.ShortProp);
			X(--GetClass().ShortField);
			X(--GetClass().ShortProp);
#if CS70
			X(--GetRefStruct().ShortField);
			X(--GetRefStruct().ShortProp);
			X(--GetRefShort());
#endif
		}
		public static void UshortAddTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//p += 5;
			//l += 5;
			ushortField += 5;
			UshortProp += 5;
			c.UshortField += 5;
			c.UshortProp += 5;
			s.UshortField += 5;
			s.UshortProp += 5;
			customClassField.UshortField += 5;
			customClassField.UshortProp += 5;
			otherCustomStructField.UshortField += 5;
			otherCustomStructField.UshortProp += 5;
			CustomClassProp.UshortField += 5;
			CustomClassProp.UshortProp += 5;
			GetClass().UshortField += 5;
			GetClass().UshortProp += 5;
#if CS70
			GetRefStruct().UshortField += 5;
			GetRefStruct().UshortProp += 5;
			GetRefUshort() += 5;
#endif
		}

		public static void UshortSubtractTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//p -= 5;
			//l -= 5;
			ushortField -= 5;
			UshortProp -= 5;
			c.UshortField -= 5;
			c.UshortProp -= 5;
			s.UshortField -= 5;
			s.UshortProp -= 5;
			customClassField.UshortField -= 5;
			customClassField.UshortProp -= 5;
			otherCustomStructField.UshortField -= 5;
			otherCustomStructField.UshortProp -= 5;
			CustomClassProp.UshortField -= 5;
			CustomClassProp.UshortProp -= 5;
			GetClass().UshortField -= 5;
			GetClass().UshortProp -= 5;
#if CS70
			GetRefStruct().UshortField -= 5;
			GetRefStruct().UshortProp -= 5;
			GetRefUshort() -= 5;
#endif
		}

		public static void UshortMultiplyTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//p *= 5;
			//l *= 5;
			ushortField *= 5;
			UshortProp *= 5;
			c.UshortField *= 5;
			c.UshortProp *= 5;
			s.UshortField *= 5;
			s.UshortProp *= 5;
			customClassField.UshortField *= 5;
			customClassField.UshortProp *= 5;
			otherCustomStructField.UshortField *= 5;
			otherCustomStructField.UshortProp *= 5;
			CustomClassProp.UshortField *= 5;
			CustomClassProp.UshortProp *= 5;
			GetClass().UshortField *= 5;
			GetClass().UshortProp *= 5;
#if CS70
			GetRefStruct().UshortField *= 5;
			GetRefStruct().UshortProp *= 5;
			GetRefUshort() *= 5;
#endif
		}

		public static void UshortDivideTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//p /= 5;
			//l /= 5;
			ushortField /= 5;
			UshortProp /= 5;
			c.UshortField /= 5;
			c.UshortProp /= 5;
			s.UshortField /= 5;
			s.UshortProp /= 5;
			customClassField.UshortField /= 5;
			customClassField.UshortProp /= 5;
			otherCustomStructField.UshortField /= 5;
			otherCustomStructField.UshortProp /= 5;
			CustomClassProp.UshortField /= 5;
			CustomClassProp.UshortProp /= 5;
			GetClass().UshortField /= 5;
			GetClass().UshortProp /= 5;
#if CS70
			GetRefStruct().UshortField /= 5;
			GetRefStruct().UshortProp /= 5;
			GetRefUshort() /= 5;
#endif
		}

		public static void UshortModulusTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//p %= 5;
			//l %= 5;
			ushortField %= 5;
			UshortProp %= 5;
			c.UshortField %= 5;
			c.UshortProp %= 5;
			s.UshortField %= 5;
			s.UshortProp %= 5;
			customClassField.UshortField %= 5;
			customClassField.UshortProp %= 5;
			otherCustomStructField.UshortField %= 5;
			otherCustomStructField.UshortProp %= 5;
			CustomClassProp.UshortField %= 5;
			CustomClassProp.UshortProp %= 5;
			GetClass().UshortField %= 5;
			GetClass().UshortProp %= 5;
#if CS70
			GetRefStruct().UshortField %= 5;
			GetRefStruct().UshortProp %= 5;
			GetRefUshort() %= 5;
#endif
		}

		public static void UshortLeftShiftTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//p <<= 5;
			//l <<= 5;
			ushortField <<= 5;
			UshortProp <<= 5;
			c.UshortField <<= 5;
			c.UshortProp <<= 5;
			s.UshortField <<= 5;
			s.UshortProp <<= 5;
			customClassField.UshortField <<= 5;
			customClassField.UshortProp <<= 5;
			otherCustomStructField.UshortField <<= 5;
			otherCustomStructField.UshortProp <<= 5;
			CustomClassProp.UshortField <<= 5;
			CustomClassProp.UshortProp <<= 5;
			GetClass().UshortField <<= 5;
			GetClass().UshortProp <<= 5;
#if CS70
			GetRefStruct().UshortField <<= 5;
			GetRefStruct().UshortProp <<= 5;
			GetRefUshort() <<= 5;
#endif
		}

		public static void UshortRightShiftTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//p >>= 5;
			//l >>= 5;
			ushortField >>= 5;
			UshortProp >>= 5;
			c.UshortField >>= 5;
			c.UshortProp >>= 5;
			s.UshortField >>= 5;
			s.UshortProp >>= 5;
			customClassField.UshortField >>= 5;
			customClassField.UshortProp >>= 5;
			otherCustomStructField.UshortField >>= 5;
			otherCustomStructField.UshortProp >>= 5;
			CustomClassProp.UshortField >>= 5;
			CustomClassProp.UshortProp >>= 5;
			GetClass().UshortField >>= 5;
			GetClass().UshortProp >>= 5;
#if CS70
			GetRefStruct().UshortField >>= 5;
			GetRefStruct().UshortProp >>= 5;
			GetRefUshort() >>= 5;
#endif
		}

		public static void UshortBitAndTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//p &= 5;
			//l &= 5;
			ushortField &= 5;
			UshortProp &= 5;
			c.UshortField &= 5;
			c.UshortProp &= 5;
			s.UshortField &= 5;
			s.UshortProp &= 5;
			customClassField.UshortField &= 5;
			customClassField.UshortProp &= 5;
			otherCustomStructField.UshortField &= 5;
			otherCustomStructField.UshortProp &= 5;
			CustomClassProp.UshortField &= 5;
			CustomClassProp.UshortProp &= 5;
			GetClass().UshortField &= 5;
			GetClass().UshortProp &= 5;
#if CS70
			GetRefStruct().UshortField &= 5;
			GetRefStruct().UshortProp &= 5;
			GetRefUshort() &= 5;
#endif
		}

		public static void UshortBitOrTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//p |= 5;
			//l |= 5;
			ushortField |= 5;
			UshortProp |= 5;
			c.UshortField |= 5;
			c.UshortProp |= 5;
			s.UshortField |= 5;
			s.UshortProp |= 5;
			customClassField.UshortField |= 5;
			customClassField.UshortProp |= 5;
			otherCustomStructField.UshortField |= 5;
			otherCustomStructField.UshortProp |= 5;
			CustomClassProp.UshortField |= 5;
			CustomClassProp.UshortProp |= 5;
			GetClass().UshortField |= 5;
			GetClass().UshortProp |= 5;
#if CS70
			GetRefStruct().UshortField |= 5;
			GetRefStruct().UshortProp |= 5;
			GetRefUshort() |= 5;
#endif
		}

		public static void UshortBitXorTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//p ^= 5;
			//l ^= 5;
			ushortField ^= 5;
			UshortProp ^= 5;
			c.UshortField ^= 5;
			c.UshortProp ^= 5;
			s.UshortField ^= 5;
			s.UshortProp ^= 5;
			customClassField.UshortField ^= 5;
			customClassField.UshortProp ^= 5;
			otherCustomStructField.UshortField ^= 5;
			otherCustomStructField.UshortProp ^= 5;
			CustomClassProp.UshortField ^= 5;
			CustomClassProp.UshortProp ^= 5;
			GetClass().UshortField ^= 5;
			GetClass().UshortProp ^= 5;
#if CS70
			GetRefStruct().UshortField ^= 5;
			GetRefStruct().UshortProp ^= 5;
			GetRefUshort() ^= 5;
#endif
		}

		public static void UshortPostIncTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//X(p++);
			//X(l++);
			X(ushortField++);
			X(UshortProp++);
			X(c.UshortField++);
			X(c.UshortProp++);
			X(s.UshortField++);
			X(s.UshortProp++);
			X(customClassField.UshortField++);
			X(customClassField.UshortProp++);
			X(otherCustomStructField.UshortField++);
			X(otherCustomStructField.UshortProp++);
			X(CustomClassProp.UshortField++);
			X(CustomClassProp.UshortProp++);
			X(GetClass().UshortField++);
			X(GetClass().UshortProp++);
#if CS70
			X(GetRefStruct().UshortField++);
			X(GetRefStruct().UshortProp++);
			X(GetRefUshort()++);
#endif
		}

		public static void UshortPreIncTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//X(++p);
			//X(++l);
			X(++ushortField);
			X(++UshortProp);
			X(++c.UshortField);
			X(++c.UshortProp);
			X(++s.UshortField);
			X(++s.UshortProp);
			X(++customClassField.UshortField);
			X(++customClassField.UshortProp);
			X(++otherCustomStructField.UshortField);
			X(++otherCustomStructField.UshortProp);
			X(++CustomClassProp.UshortField);
			X(++CustomClassProp.UshortProp);
			X(++GetClass().UshortField);
			X(++GetClass().UshortProp);
#if CS70
			X(++GetRefStruct().UshortField);
			X(++GetRefStruct().UshortProp);
			X(++GetRefUshort());
#endif
		}
		public static void UshortPostDecTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//X(p--);
			//X(l--);
			X(ushortField--);
			X(UshortProp--);
			X(c.UshortField--);
			X(c.UshortProp--);
			X(s.UshortField--);
			X(s.UshortProp--);
			X(customClassField.UshortField--);
			X(customClassField.UshortProp--);
			X(otherCustomStructField.UshortField--);
			X(otherCustomStructField.UshortProp--);
			X(CustomClassProp.UshortField--);
			X(CustomClassProp.UshortProp--);
			X(GetClass().UshortField--);
			X(GetClass().UshortProp--);
#if CS70
			X(GetRefStruct().UshortField--);
			X(GetRefStruct().UshortProp--);
			X(GetRefUshort()--);
#endif
		}

		public static void UshortPreDecTest(ushort p, CustomClass c, CustomStruct2 s)
		{
			//ushort l = 0;
			//X(--p);
			//X(--l);
			X(--ushortField);
			X(--UshortProp);
			X(--c.UshortField);
			X(--c.UshortProp);
			X(--s.UshortField);
			X(--s.UshortProp);
			X(--customClassField.UshortField);
			X(--customClassField.UshortProp);
			X(--otherCustomStructField.UshortField);
			X(--otherCustomStructField.UshortProp);
			X(--CustomClassProp.UshortField);
			X(--CustomClassProp.UshortProp);
			X(--GetClass().UshortField);
			X(--GetClass().UshortProp);
#if CS70
			X(--GetRefStruct().UshortField);
			X(--GetRefStruct().UshortProp);
			X(--GetRefUshort());
#endif
		}
		public static void IntAddTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//p += 5;
			//l += 5;
			intField += 5;
			IntProp += 5;
			c.IntField += 5;
			c.IntProp += 5;
			s.IntField += 5;
			s.IntProp += 5;
			customClassField.IntField += 5;
			customClassField.IntProp += 5;
			otherCustomStructField.IntField += 5;
			otherCustomStructField.IntProp += 5;
			CustomClassProp.IntField += 5;
			CustomClassProp.IntProp += 5;
			GetClass().IntField += 5;
			GetClass().IntProp += 5;
#if CS70
			GetRefStruct().IntField += 5;
			GetRefStruct().IntProp += 5;
			GetRefInt() += 5;
#endif
		}

		public static void IntSubtractTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//p -= 5;
			//l -= 5;
			intField -= 5;
			IntProp -= 5;
			c.IntField -= 5;
			c.IntProp -= 5;
			s.IntField -= 5;
			s.IntProp -= 5;
			customClassField.IntField -= 5;
			customClassField.IntProp -= 5;
			otherCustomStructField.IntField -= 5;
			otherCustomStructField.IntProp -= 5;
			CustomClassProp.IntField -= 5;
			CustomClassProp.IntProp -= 5;
			GetClass().IntField -= 5;
			GetClass().IntProp -= 5;
#if CS70
			GetRefStruct().IntField -= 5;
			GetRefStruct().IntProp -= 5;
			GetRefInt() -= 5;
#endif
		}

		public static void IntMultiplyTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//p *= 5;
			//l *= 5;
			intField *= 5;
			IntProp *= 5;
			c.IntField *= 5;
			c.IntProp *= 5;
			s.IntField *= 5;
			s.IntProp *= 5;
			customClassField.IntField *= 5;
			customClassField.IntProp *= 5;
			otherCustomStructField.IntField *= 5;
			otherCustomStructField.IntProp *= 5;
			CustomClassProp.IntField *= 5;
			CustomClassProp.IntProp *= 5;
			GetClass().IntField *= 5;
			GetClass().IntProp *= 5;
#if CS70
			GetRefStruct().IntField *= 5;
			GetRefStruct().IntProp *= 5;
			GetRefInt() *= 5;
#endif
		}

		public static void IntDivideTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//p /= 5;
			//l /= 5;
			intField /= 5;
			IntProp /= 5;
			c.IntField /= 5;
			c.IntProp /= 5;
			s.IntField /= 5;
			s.IntProp /= 5;
			customClassField.IntField /= 5;
			customClassField.IntProp /= 5;
			otherCustomStructField.IntField /= 5;
			otherCustomStructField.IntProp /= 5;
			CustomClassProp.IntField /= 5;
			CustomClassProp.IntProp /= 5;
			GetClass().IntField /= 5;
			GetClass().IntProp /= 5;
#if CS70
			GetRefStruct().IntField /= 5;
			GetRefStruct().IntProp /= 5;
			GetRefInt() /= 5;
#endif
		}

		public static void IntModulusTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//p %= 5;
			//l %= 5;
			intField %= 5;
			IntProp %= 5;
			c.IntField %= 5;
			c.IntProp %= 5;
			s.IntField %= 5;
			s.IntProp %= 5;
			customClassField.IntField %= 5;
			customClassField.IntProp %= 5;
			otherCustomStructField.IntField %= 5;
			otherCustomStructField.IntProp %= 5;
			CustomClassProp.IntField %= 5;
			CustomClassProp.IntProp %= 5;
			GetClass().IntField %= 5;
			GetClass().IntProp %= 5;
#if CS70
			GetRefStruct().IntField %= 5;
			GetRefStruct().IntProp %= 5;
			GetRefInt() %= 5;
#endif
		}

		public static void IntLeftShiftTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//p <<= 5;
			//l <<= 5;
			intField <<= 5;
			IntProp <<= 5;
			c.IntField <<= 5;
			c.IntProp <<= 5;
			s.IntField <<= 5;
			s.IntProp <<= 5;
			customClassField.IntField <<= 5;
			customClassField.IntProp <<= 5;
			otherCustomStructField.IntField <<= 5;
			otherCustomStructField.IntProp <<= 5;
			CustomClassProp.IntField <<= 5;
			CustomClassProp.IntProp <<= 5;
			GetClass().IntField <<= 5;
			GetClass().IntProp <<= 5;
#if CS70
			GetRefStruct().IntField <<= 5;
			GetRefStruct().IntProp <<= 5;
			GetRefInt() <<= 5;
#endif
		}

		public static void IntRightShiftTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//p >>= 5;
			//l >>= 5;
			intField >>= 5;
			IntProp >>= 5;
			c.IntField >>= 5;
			c.IntProp >>= 5;
			s.IntField >>= 5;
			s.IntProp >>= 5;
			customClassField.IntField >>= 5;
			customClassField.IntProp >>= 5;
			otherCustomStructField.IntField >>= 5;
			otherCustomStructField.IntProp >>= 5;
			CustomClassProp.IntField >>= 5;
			CustomClassProp.IntProp >>= 5;
			GetClass().IntField >>= 5;
			GetClass().IntProp >>= 5;
#if CS70
			GetRefStruct().IntField >>= 5;
			GetRefStruct().IntProp >>= 5;
			GetRefInt() >>= 5;
#endif
		}

		public static void IntBitAndTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//p &= 5;
			//l &= 5;
			intField &= 5;
			IntProp &= 5;
			c.IntField &= 5;
			c.IntProp &= 5;
			s.IntField &= 5;
			s.IntProp &= 5;
			customClassField.IntField &= 5;
			customClassField.IntProp &= 5;
			otherCustomStructField.IntField &= 5;
			otherCustomStructField.IntProp &= 5;
			CustomClassProp.IntField &= 5;
			CustomClassProp.IntProp &= 5;
			GetClass().IntField &= 5;
			GetClass().IntProp &= 5;
#if CS70
			GetRefStruct().IntField &= 5;
			GetRefStruct().IntProp &= 5;
			GetRefInt() &= 5;
#endif
		}

		public static void IntBitOrTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//p |= 5;
			//l |= 5;
			intField |= 5;
			IntProp |= 5;
			c.IntField |= 5;
			c.IntProp |= 5;
			s.IntField |= 5;
			s.IntProp |= 5;
			customClassField.IntField |= 5;
			customClassField.IntProp |= 5;
			otherCustomStructField.IntField |= 5;
			otherCustomStructField.IntProp |= 5;
			CustomClassProp.IntField |= 5;
			CustomClassProp.IntProp |= 5;
			GetClass().IntField |= 5;
			GetClass().IntProp |= 5;
#if CS70
			GetRefStruct().IntField |= 5;
			GetRefStruct().IntProp |= 5;
			GetRefInt() |= 5;
#endif
		}

		public static void IntBitXorTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//p ^= 5;
			//l ^= 5;
			intField ^= 5;
			IntProp ^= 5;
			c.IntField ^= 5;
			c.IntProp ^= 5;
			s.IntField ^= 5;
			s.IntProp ^= 5;
			customClassField.IntField ^= 5;
			customClassField.IntProp ^= 5;
			otherCustomStructField.IntField ^= 5;
			otherCustomStructField.IntProp ^= 5;
			CustomClassProp.IntField ^= 5;
			CustomClassProp.IntProp ^= 5;
			GetClass().IntField ^= 5;
			GetClass().IntProp ^= 5;
#if CS70
			GetRefStruct().IntField ^= 5;
			GetRefStruct().IntProp ^= 5;
			GetRefInt() ^= 5;
#endif
		}

		public static void IntPostIncTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//X(p++);
			//X(l++);
			X(intField++);
			X(IntProp++);
			X(c.IntField++);
			X(c.IntProp++);
			X(s.IntField++);
			X(s.IntProp++);
			X(customClassField.IntField++);
			X(customClassField.IntProp++);
			X(otherCustomStructField.IntField++);
			X(otherCustomStructField.IntProp++);
			X(CustomClassProp.IntField++);
			X(CustomClassProp.IntProp++);
			X(GetClass().IntField++);
			X(GetClass().IntProp++);
#if CS70
			X(GetRefStruct().IntField++);
			X(GetRefStruct().IntProp++);
			X(GetRefInt()++);
#endif
		}

		public static void IntPreIncTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//X(++p);
			//X(++l);
			X(++intField);
			X(++IntProp);
			X(++c.IntField);
			X(++c.IntProp);
			X(++s.IntField);
			X(++s.IntProp);
			X(++customClassField.IntField);
			X(++customClassField.IntProp);
			X(++otherCustomStructField.IntField);
			X(++otherCustomStructField.IntProp);
			X(++CustomClassProp.IntField);
			X(++CustomClassProp.IntProp);
			X(++GetClass().IntField);
			X(++GetClass().IntProp);
#if CS70
			X(++GetRefStruct().IntField);
			X(++GetRefStruct().IntProp);
			X(++GetRefInt());
#endif
		}
		public static void IntPostDecTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//X(p--);
			//X(l--);
			X(intField--);
			X(IntProp--);
			X(c.IntField--);
			X(c.IntProp--);
			X(s.IntField--);
			X(s.IntProp--);
			X(customClassField.IntField--);
			X(customClassField.IntProp--);
			X(otherCustomStructField.IntField--);
			X(otherCustomStructField.IntProp--);
			X(CustomClassProp.IntField--);
			X(CustomClassProp.IntProp--);
			X(GetClass().IntField--);
			X(GetClass().IntProp--);
#if CS70
			X(GetRefStruct().IntField--);
			X(GetRefStruct().IntProp--);
			X(GetRefInt()--);
#endif
		}

		public static void IntPreDecTest(int p, CustomClass c, CustomStruct2 s)
		{
			//int l = 0;
			//X(--p);
			//X(--l);
			X(--intField);
			X(--IntProp);
			X(--c.IntField);
			X(--c.IntProp);
			X(--s.IntField);
			X(--s.IntProp);
			X(--customClassField.IntField);
			X(--customClassField.IntProp);
			X(--otherCustomStructField.IntField);
			X(--otherCustomStructField.IntProp);
			X(--CustomClassProp.IntField);
			X(--CustomClassProp.IntProp);
			X(--GetClass().IntField);
			X(--GetClass().IntProp);
#if CS70
			X(--GetRefStruct().IntField);
			X(--GetRefStruct().IntProp);
			X(--GetRefInt());
#endif
		}
		public static void UintAddTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//p += 5u;
			//l += 5u;
			uintField += 5u;
			UintProp += 5u;
			c.UintField += 5u;
			c.UintProp += 5u;
			s.UintField += 5u;
			s.UintProp += 5u;
			customClassField.UintField += 5u;
			customClassField.UintProp += 5u;
			otherCustomStructField.UintField += 5u;
			otherCustomStructField.UintProp += 5u;
			CustomClassProp.UintField += 5u;
			CustomClassProp.UintProp += 5u;
			GetClass().UintField += 5u;
			GetClass().UintProp += 5u;
#if CS70
			GetRefStruct().UintField += 5u;
			GetRefStruct().UintProp += 5u;
			GetRefUint() += 5u;
#endif
		}

		public static void UintSubtractTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//p -= 5u;
			//l -= 5u;
			uintField -= 5u;
			UintProp -= 5u;
			c.UintField -= 5u;
			c.UintProp -= 5u;
			s.UintField -= 5u;
			s.UintProp -= 5u;
			customClassField.UintField -= 5u;
			customClassField.UintProp -= 5u;
			otherCustomStructField.UintField -= 5u;
			otherCustomStructField.UintProp -= 5u;
			CustomClassProp.UintField -= 5u;
			CustomClassProp.UintProp -= 5u;
			GetClass().UintField -= 5u;
			GetClass().UintProp -= 5u;
#if CS70
			GetRefStruct().UintField -= 5u;
			GetRefStruct().UintProp -= 5u;
			GetRefUint() -= 5u;
#endif
		}

		public static void UintMultiplyTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//p *= 5u;
			//l *= 5u;
			uintField *= 5u;
			UintProp *= 5u;
			c.UintField *= 5u;
			c.UintProp *= 5u;
			s.UintField *= 5u;
			s.UintProp *= 5u;
			customClassField.UintField *= 5u;
			customClassField.UintProp *= 5u;
			otherCustomStructField.UintField *= 5u;
			otherCustomStructField.UintProp *= 5u;
			CustomClassProp.UintField *= 5u;
			CustomClassProp.UintProp *= 5u;
			GetClass().UintField *= 5u;
			GetClass().UintProp *= 5u;
#if CS70
			GetRefStruct().UintField *= 5u;
			GetRefStruct().UintProp *= 5u;
			GetRefUint() *= 5u;
#endif
		}

		public static void UintDivideTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//p /= 5u;
			//l /= 5u;
			uintField /= 5u;
			UintProp /= 5u;
			c.UintField /= 5u;
			c.UintProp /= 5u;
			s.UintField /= 5u;
			s.UintProp /= 5u;
			customClassField.UintField /= 5u;
			customClassField.UintProp /= 5u;
			otherCustomStructField.UintField /= 5u;
			otherCustomStructField.UintProp /= 5u;
			CustomClassProp.UintField /= 5u;
			CustomClassProp.UintProp /= 5u;
			GetClass().UintField /= 5u;
			GetClass().UintProp /= 5u;
#if CS70
			GetRefStruct().UintField /= 5u;
			GetRefStruct().UintProp /= 5u;
			GetRefUint() /= 5u;
#endif
		}

		public static void UintModulusTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//p %= 5u;
			//l %= 5u;
			uintField %= 5u;
			UintProp %= 5u;
			c.UintField %= 5u;
			c.UintProp %= 5u;
			s.UintField %= 5u;
			s.UintProp %= 5u;
			customClassField.UintField %= 5u;
			customClassField.UintProp %= 5u;
			otherCustomStructField.UintField %= 5u;
			otherCustomStructField.UintProp %= 5u;
			CustomClassProp.UintField %= 5u;
			CustomClassProp.UintProp %= 5u;
			GetClass().UintField %= 5u;
			GetClass().UintProp %= 5u;
#if CS70
			GetRefStruct().UintField %= 5u;
			GetRefStruct().UintProp %= 5u;
			GetRefUint() %= 5u;
#endif
		}

		public static void UintLeftShiftTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//p <<= 5;
			//l <<= 5;
			uintField <<= 5;
			UintProp <<= 5;
			c.UintField <<= 5;
			c.UintProp <<= 5;
			s.UintField <<= 5;
			s.UintProp <<= 5;
			customClassField.UintField <<= 5;
			customClassField.UintProp <<= 5;
			otherCustomStructField.UintField <<= 5;
			otherCustomStructField.UintProp <<= 5;
			CustomClassProp.UintField <<= 5;
			CustomClassProp.UintProp <<= 5;
			GetClass().UintField <<= 5;
			GetClass().UintProp <<= 5;
#if CS70
			GetRefStruct().UintField <<= 5;
			GetRefStruct().UintProp <<= 5;
			GetRefUint() <<= 5;
#endif
		}

		public static void UintRightShiftTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//p >>= 5;
			//l >>= 5;
			uintField >>= 5;
			UintProp >>= 5;
			c.UintField >>= 5;
			c.UintProp >>= 5;
			s.UintField >>= 5;
			s.UintProp >>= 5;
			customClassField.UintField >>= 5;
			customClassField.UintProp >>= 5;
			otherCustomStructField.UintField >>= 5;
			otherCustomStructField.UintProp >>= 5;
			CustomClassProp.UintField >>= 5;
			CustomClassProp.UintProp >>= 5;
			GetClass().UintField >>= 5;
			GetClass().UintProp >>= 5;
#if CS70
			GetRefStruct().UintField >>= 5;
			GetRefStruct().UintProp >>= 5;
			GetRefUint() >>= 5;
#endif
		}

		public static void UintBitAndTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//p &= 5u;
			//l &= 5u;
			uintField &= 5u;
			UintProp &= 5u;
			c.UintField &= 5u;
			c.UintProp &= 5u;
			s.UintField &= 5u;
			s.UintProp &= 5u;
			customClassField.UintField &= 5u;
			customClassField.UintProp &= 5u;
			otherCustomStructField.UintField &= 5u;
			otherCustomStructField.UintProp &= 5u;
			CustomClassProp.UintField &= 5u;
			CustomClassProp.UintProp &= 5u;
			GetClass().UintField &= 5u;
			GetClass().UintProp &= 5u;
#if CS70
			GetRefStruct().UintField &= 5u;
			GetRefStruct().UintProp &= 5u;
			GetRefUint() &= 5u;
#endif
		}

		public static void UintBitOrTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//p |= 5u;
			//l |= 5u;
			uintField |= 5u;
			UintProp |= 5u;
			c.UintField |= 5u;
			c.UintProp |= 5u;
			s.UintField |= 5u;
			s.UintProp |= 5u;
			customClassField.UintField |= 5u;
			customClassField.UintProp |= 5u;
			otherCustomStructField.UintField |= 5u;
			otherCustomStructField.UintProp |= 5u;
			CustomClassProp.UintField |= 5u;
			CustomClassProp.UintProp |= 5u;
			GetClass().UintField |= 5u;
			GetClass().UintProp |= 5u;
#if CS70
			GetRefStruct().UintField |= 5u;
			GetRefStruct().UintProp |= 5u;
			GetRefUint() |= 5u;
#endif
		}

		public static void UintBitXorTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//p ^= 5u;
			//l ^= 5u;
			uintField ^= 5u;
			UintProp ^= 5u;
			c.UintField ^= 5u;
			c.UintProp ^= 5u;
			s.UintField ^= 5u;
			s.UintProp ^= 5u;
			customClassField.UintField ^= 5u;
			customClassField.UintProp ^= 5u;
			otherCustomStructField.UintField ^= 5u;
			otherCustomStructField.UintProp ^= 5u;
			CustomClassProp.UintField ^= 5u;
			CustomClassProp.UintProp ^= 5u;
			GetClass().UintField ^= 5u;
			GetClass().UintProp ^= 5u;
#if CS70
			GetRefStruct().UintField ^= 5u;
			GetRefStruct().UintProp ^= 5u;
			GetRefUint() ^= 5u;
#endif
		}

		public static void UintPostIncTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//X(p++);
			//X(l++);
			X(uintField++);
			X(UintProp++);
			X(c.UintField++);
			X(c.UintProp++);
			X(s.UintField++);
			X(s.UintProp++);
			X(customClassField.UintField++);
			X(customClassField.UintProp++);
			X(otherCustomStructField.UintField++);
			X(otherCustomStructField.UintProp++);
			X(CustomClassProp.UintField++);
			X(CustomClassProp.UintProp++);
			X(GetClass().UintField++);
			X(GetClass().UintProp++);
#if CS70
			X(GetRefStruct().UintField++);
			X(GetRefStruct().UintProp++);
			X(GetRefUint()++);
#endif
		}

		public static void UintPreIncTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//X(++p);
			//X(++l);
			X(++uintField);
			X(++UintProp);
			X(++c.UintField);
			X(++c.UintProp);
			X(++s.UintField);
			X(++s.UintProp);
			X(++customClassField.UintField);
			X(++customClassField.UintProp);
			X(++otherCustomStructField.UintField);
			X(++otherCustomStructField.UintProp);
			X(++CustomClassProp.UintField);
			X(++CustomClassProp.UintProp);
			X(++GetClass().UintField);
			X(++GetClass().UintProp);
#if CS70
			X(++GetRefStruct().UintField);
			X(++GetRefStruct().UintProp);
			X(++GetRefUint());
#endif
		}
		public static void UintPostDecTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//X(p--);
			//X(l--);
			X(uintField--);
			X(UintProp--);
			X(c.UintField--);
			X(c.UintProp--);
			X(s.UintField--);
			X(s.UintProp--);
			X(customClassField.UintField--);
			X(customClassField.UintProp--);
			X(otherCustomStructField.UintField--);
			X(otherCustomStructField.UintProp--);
			X(CustomClassProp.UintField--);
			X(CustomClassProp.UintProp--);
			X(GetClass().UintField--);
			X(GetClass().UintProp--);
#if CS70
			X(GetRefStruct().UintField--);
			X(GetRefStruct().UintProp--);
			X(GetRefUint()--);
#endif
		}

		public static void UintPreDecTest(uint p, CustomClass c, CustomStruct2 s)
		{
			//uint l = 0;
			//X(--p);
			//X(--l);
			X(--uintField);
			X(--UintProp);
			X(--c.UintField);
			X(--c.UintProp);
			X(--s.UintField);
			X(--s.UintProp);
			X(--customClassField.UintField);
			X(--customClassField.UintProp);
			X(--otherCustomStructField.UintField);
			X(--otherCustomStructField.UintProp);
			X(--CustomClassProp.UintField);
			X(--CustomClassProp.UintProp);
			X(--GetClass().UintField);
			X(--GetClass().UintProp);
#if CS70
			X(--GetRefStruct().UintField);
			X(--GetRefStruct().UintProp);
			X(--GetRefUint());
#endif
		}
		public static void LongAddTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//p += 5L;
			//l += 5L;
			longField += 5L;
			LongProp += 5L;
			c.LongField += 5L;
			c.LongProp += 5L;
			s.LongField += 5L;
			s.LongProp += 5L;
			customClassField.LongField += 5L;
			customClassField.LongProp += 5L;
			otherCustomStructField.LongField += 5L;
			otherCustomStructField.LongProp += 5L;
			CustomClassProp.LongField += 5L;
			CustomClassProp.LongProp += 5L;
			GetClass().LongField += 5L;
			GetClass().LongProp += 5L;
#if CS70
			GetRefStruct().LongField += 5L;
			GetRefStruct().LongProp += 5L;
			GetRefLong() += 5L;
#endif
		}

		public static void LongSubtractTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//p -= 5L;
			//l -= 5L;
			longField -= 5L;
			LongProp -= 5L;
			c.LongField -= 5L;
			c.LongProp -= 5L;
			s.LongField -= 5L;
			s.LongProp -= 5L;
			customClassField.LongField -= 5L;
			customClassField.LongProp -= 5L;
			otherCustomStructField.LongField -= 5L;
			otherCustomStructField.LongProp -= 5L;
			CustomClassProp.LongField -= 5L;
			CustomClassProp.LongProp -= 5L;
			GetClass().LongField -= 5L;
			GetClass().LongProp -= 5L;
#if CS70
			GetRefStruct().LongField -= 5L;
			GetRefStruct().LongProp -= 5L;
			GetRefLong() -= 5L;
#endif
		}

		public static void LongMultiplyTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//p *= 5L;
			//l *= 5L;
			longField *= 5L;
			LongProp *= 5L;
			c.LongField *= 5L;
			c.LongProp *= 5L;
			s.LongField *= 5L;
			s.LongProp *= 5L;
			customClassField.LongField *= 5L;
			customClassField.LongProp *= 5L;
			otherCustomStructField.LongField *= 5L;
			otherCustomStructField.LongProp *= 5L;
			CustomClassProp.LongField *= 5L;
			CustomClassProp.LongProp *= 5L;
			GetClass().LongField *= 5L;
			GetClass().LongProp *= 5L;
#if CS70
			GetRefStruct().LongField *= 5L;
			GetRefStruct().LongProp *= 5L;
			GetRefLong() *= 5L;
#endif
		}

		public static void LongDivideTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//p /= 5L;
			//l /= 5L;
			longField /= 5L;
			LongProp /= 5L;
			c.LongField /= 5L;
			c.LongProp /= 5L;
			s.LongField /= 5L;
			s.LongProp /= 5L;
			customClassField.LongField /= 5L;
			customClassField.LongProp /= 5L;
			otherCustomStructField.LongField /= 5L;
			otherCustomStructField.LongProp /= 5L;
			CustomClassProp.LongField /= 5L;
			CustomClassProp.LongProp /= 5L;
			GetClass().LongField /= 5L;
			GetClass().LongProp /= 5L;
#if CS70
			GetRefStruct().LongField /= 5L;
			GetRefStruct().LongProp /= 5L;
			GetRefLong() /= 5L;
#endif
		}

		public static void LongModulusTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//p %= 5L;
			//l %= 5L;
			longField %= 5L;
			LongProp %= 5L;
			c.LongField %= 5L;
			c.LongProp %= 5L;
			s.LongField %= 5L;
			s.LongProp %= 5L;
			customClassField.LongField %= 5L;
			customClassField.LongProp %= 5L;
			otherCustomStructField.LongField %= 5L;
			otherCustomStructField.LongProp %= 5L;
			CustomClassProp.LongField %= 5L;
			CustomClassProp.LongProp %= 5L;
			GetClass().LongField %= 5L;
			GetClass().LongProp %= 5L;
#if CS70
			GetRefStruct().LongField %= 5L;
			GetRefStruct().LongProp %= 5L;
			GetRefLong() %= 5L;
#endif
		}

		public static void LongLeftShiftTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//p <<= 5;
			//l <<= 5;
			longField <<= 5;
			LongProp <<= 5;
			c.LongField <<= 5;
			c.LongProp <<= 5;
			s.LongField <<= 5;
			s.LongProp <<= 5;
			customClassField.LongField <<= 5;
			customClassField.LongProp <<= 5;
			otherCustomStructField.LongField <<= 5;
			otherCustomStructField.LongProp <<= 5;
			CustomClassProp.LongField <<= 5;
			CustomClassProp.LongProp <<= 5;
			GetClass().LongField <<= 5;
			GetClass().LongProp <<= 5;
#if CS70
			GetRefStruct().LongField <<= 5;
			GetRefStruct().LongProp <<= 5;
			GetRefLong() <<= 5;
#endif
		}

		public static void LongRightShiftTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//p >>= 5;
			//l >>= 5;
			longField >>= 5;
			LongProp >>= 5;
			c.LongField >>= 5;
			c.LongProp >>= 5;
			s.LongField >>= 5;
			s.LongProp >>= 5;
			customClassField.LongField >>= 5;
			customClassField.LongProp >>= 5;
			otherCustomStructField.LongField >>= 5;
			otherCustomStructField.LongProp >>= 5;
			CustomClassProp.LongField >>= 5;
			CustomClassProp.LongProp >>= 5;
			GetClass().LongField >>= 5;
			GetClass().LongProp >>= 5;
#if CS70
			GetRefStruct().LongField >>= 5;
			GetRefStruct().LongProp >>= 5;
			GetRefLong() >>= 5;
#endif
		}

		public static void LongBitAndTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//p &= 5L;
			//l &= 5L;
			longField &= 5L;
			LongProp &= 5L;
			c.LongField &= 5L;
			c.LongProp &= 5L;
			s.LongField &= 5L;
			s.LongProp &= 5L;
			customClassField.LongField &= 5L;
			customClassField.LongProp &= 5L;
			otherCustomStructField.LongField &= 5L;
			otherCustomStructField.LongProp &= 5L;
			CustomClassProp.LongField &= 5L;
			CustomClassProp.LongProp &= 5L;
			GetClass().LongField &= 5L;
			GetClass().LongProp &= 5L;
#if CS70
			GetRefStruct().LongField &= 5L;
			GetRefStruct().LongProp &= 5L;
			GetRefLong() &= 5L;
#endif
		}

		public static void LongBitOrTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//p |= 5L;
			//l |= 5L;
			longField |= 5L;
			LongProp |= 5L;
			c.LongField |= 5L;
			c.LongProp |= 5L;
			s.LongField |= 5L;
			s.LongProp |= 5L;
			customClassField.LongField |= 5L;
			customClassField.LongProp |= 5L;
			otherCustomStructField.LongField |= 5L;
			otherCustomStructField.LongProp |= 5L;
			CustomClassProp.LongField |= 5L;
			CustomClassProp.LongProp |= 5L;
			GetClass().LongField |= 5L;
			GetClass().LongProp |= 5L;
#if CS70
			GetRefStruct().LongField |= 5L;
			GetRefStruct().LongProp |= 5L;
			GetRefLong() |= 5L;
#endif
		}

		public static void LongBitXorTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//p ^= 5L;
			//l ^= 5L;
			longField ^= 5L;
			LongProp ^= 5L;
			c.LongField ^= 5L;
			c.LongProp ^= 5L;
			s.LongField ^= 5L;
			s.LongProp ^= 5L;
			customClassField.LongField ^= 5L;
			customClassField.LongProp ^= 5L;
			otherCustomStructField.LongField ^= 5L;
			otherCustomStructField.LongProp ^= 5L;
			CustomClassProp.LongField ^= 5L;
			CustomClassProp.LongProp ^= 5L;
			GetClass().LongField ^= 5L;
			GetClass().LongProp ^= 5L;
#if CS70
			GetRefStruct().LongField ^= 5L;
			GetRefStruct().LongProp ^= 5L;
			GetRefLong() ^= 5L;
#endif
		}

		public static void LongPostIncTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//X(p++);
			//X(l++);
			X(longField++);
			X(LongProp++);
			X(c.LongField++);
			X(c.LongProp++);
			X(s.LongField++);
			X(s.LongProp++);
			X(customClassField.LongField++);
			X(customClassField.LongProp++);
			X(otherCustomStructField.LongField++);
			X(otherCustomStructField.LongProp++);
			X(CustomClassProp.LongField++);
			X(CustomClassProp.LongProp++);
			X(GetClass().LongField++);
			X(GetClass().LongProp++);
#if CS70
			X(GetRefStruct().LongField++);
			X(GetRefStruct().LongProp++);
			X(GetRefLong()++);
#endif
		}

		public static void LongPreIncTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//X(++p);
			//X(++l);
			X(++longField);
			X(++LongProp);
			X(++c.LongField);
			X(++c.LongProp);
			X(++s.LongField);
			X(++s.LongProp);
			X(++customClassField.LongField);
			X(++customClassField.LongProp);
			X(++otherCustomStructField.LongField);
			X(++otherCustomStructField.LongProp);
			X(++CustomClassProp.LongField);
			X(++CustomClassProp.LongProp);
			X(++GetClass().LongField);
			X(++GetClass().LongProp);
#if CS70
			X(++GetRefStruct().LongField);
			X(++GetRefStruct().LongProp);
			X(++GetRefLong());
#endif
		}
		public static void LongPostDecTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//X(p--);
			//X(l--);
			X(longField--);
			X(LongProp--);
			X(c.LongField--);
			X(c.LongProp--);
			X(s.LongField--);
			X(s.LongProp--);
			X(customClassField.LongField--);
			X(customClassField.LongProp--);
			X(otherCustomStructField.LongField--);
			X(otherCustomStructField.LongProp--);
			X(CustomClassProp.LongField--);
			X(CustomClassProp.LongProp--);
			X(GetClass().LongField--);
			X(GetClass().LongProp--);
#if CS70
			X(GetRefStruct().LongField--);
			X(GetRefStruct().LongProp--);
			X(GetRefLong()--);
#endif
		}

		public static void LongPreDecTest(long p, CustomClass c, CustomStruct2 s)
		{
			//long l = 0;
			//X(--p);
			//X(--l);
			X(--longField);
			X(--LongProp);
			X(--c.LongField);
			X(--c.LongProp);
			X(--s.LongField);
			X(--s.LongProp);
			X(--customClassField.LongField);
			X(--customClassField.LongProp);
			X(--otherCustomStructField.LongField);
			X(--otherCustomStructField.LongProp);
			X(--CustomClassProp.LongField);
			X(--CustomClassProp.LongProp);
			X(--GetClass().LongField);
			X(--GetClass().LongProp);
#if CS70
			X(--GetRefStruct().LongField);
			X(--GetRefStruct().LongProp);
			X(--GetRefLong());
#endif
		}
		public static void UlongAddTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//p += 5uL;
			//l += 5uL;
			ulongField += 5uL;
			UlongProp += 5uL;
			c.UlongField += 5uL;
			c.UlongProp += 5uL;
			s.UlongField += 5uL;
			s.UlongProp += 5uL;
			customClassField.UlongField += 5uL;
			customClassField.UlongProp += 5uL;
			otherCustomStructField.UlongField += 5uL;
			otherCustomStructField.UlongProp += 5uL;
			CustomClassProp.UlongField += 5uL;
			CustomClassProp.UlongProp += 5uL;
			GetClass().UlongField += 5uL;
			GetClass().UlongProp += 5uL;
#if CS70
			GetRefStruct().UlongField += 5uL;
			GetRefStruct().UlongProp += 5uL;
			GetRefUlong() += 5uL;
#endif
		}

		public static void UlongSubtractTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//p -= 5uL;
			//l -= 5uL;
			ulongField -= 5uL;
			UlongProp -= 5uL;
			c.UlongField -= 5uL;
			c.UlongProp -= 5uL;
			s.UlongField -= 5uL;
			s.UlongProp -= 5uL;
			customClassField.UlongField -= 5uL;
			customClassField.UlongProp -= 5uL;
			otherCustomStructField.UlongField -= 5uL;
			otherCustomStructField.UlongProp -= 5uL;
			CustomClassProp.UlongField -= 5uL;
			CustomClassProp.UlongProp -= 5uL;
			GetClass().UlongField -= 5uL;
			GetClass().UlongProp -= 5uL;
#if CS70
			GetRefStruct().UlongField -= 5uL;
			GetRefStruct().UlongProp -= 5uL;
			GetRefUlong() -= 5uL;
#endif
		}

		public static void UlongMultiplyTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//p *= 5uL;
			//l *= 5uL;
			ulongField *= 5uL;
			UlongProp *= 5uL;
			c.UlongField *= 5uL;
			c.UlongProp *= 5uL;
			s.UlongField *= 5uL;
			s.UlongProp *= 5uL;
			customClassField.UlongField *= 5uL;
			customClassField.UlongProp *= 5uL;
			otherCustomStructField.UlongField *= 5uL;
			otherCustomStructField.UlongProp *= 5uL;
			CustomClassProp.UlongField *= 5uL;
			CustomClassProp.UlongProp *= 5uL;
			GetClass().UlongField *= 5uL;
			GetClass().UlongProp *= 5uL;
#if CS70
			GetRefStruct().UlongField *= 5uL;
			GetRefStruct().UlongProp *= 5uL;
			GetRefUlong() *= 5uL;
#endif
		}

		public static void UlongDivideTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//p /= 5uL;
			//l /= 5uL;
			ulongField /= 5uL;
			UlongProp /= 5uL;
			c.UlongField /= 5uL;
			c.UlongProp /= 5uL;
			s.UlongField /= 5uL;
			s.UlongProp /= 5uL;
			customClassField.UlongField /= 5uL;
			customClassField.UlongProp /= 5uL;
			otherCustomStructField.UlongField /= 5uL;
			otherCustomStructField.UlongProp /= 5uL;
			CustomClassProp.UlongField /= 5uL;
			CustomClassProp.UlongProp /= 5uL;
			GetClass().UlongField /= 5uL;
			GetClass().UlongProp /= 5uL;
#if CS70
			GetRefStruct().UlongField /= 5uL;
			GetRefStruct().UlongProp /= 5uL;
			GetRefUlong() /= 5uL;
#endif
		}

		public static void UlongModulusTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//p %= 5uL;
			//l %= 5uL;
			ulongField %= 5uL;
			UlongProp %= 5uL;
			c.UlongField %= 5uL;
			c.UlongProp %= 5uL;
			s.UlongField %= 5uL;
			s.UlongProp %= 5uL;
			customClassField.UlongField %= 5uL;
			customClassField.UlongProp %= 5uL;
			otherCustomStructField.UlongField %= 5uL;
			otherCustomStructField.UlongProp %= 5uL;
			CustomClassProp.UlongField %= 5uL;
			CustomClassProp.UlongProp %= 5uL;
			GetClass().UlongField %= 5uL;
			GetClass().UlongProp %= 5uL;
#if CS70
			GetRefStruct().UlongField %= 5uL;
			GetRefStruct().UlongProp %= 5uL;
			GetRefUlong() %= 5uL;
#endif
		}

		public static void UlongLeftShiftTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//p <<= 5;
			//l <<= 5;
			ulongField <<= 5;
			UlongProp <<= 5;
			c.UlongField <<= 5;
			c.UlongProp <<= 5;
			s.UlongField <<= 5;
			s.UlongProp <<= 5;
			customClassField.UlongField <<= 5;
			customClassField.UlongProp <<= 5;
			otherCustomStructField.UlongField <<= 5;
			otherCustomStructField.UlongProp <<= 5;
			CustomClassProp.UlongField <<= 5;
			CustomClassProp.UlongProp <<= 5;
			GetClass().UlongField <<= 5;
			GetClass().UlongProp <<= 5;
#if CS70
			GetRefStruct().UlongField <<= 5;
			GetRefStruct().UlongProp <<= 5;
			GetRefUlong() <<= 5;
#endif
		}

		public static void UlongRightShiftTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//p >>= 5;
			//l >>= 5;
			ulongField >>= 5;
			UlongProp >>= 5;
			c.UlongField >>= 5;
			c.UlongProp >>= 5;
			s.UlongField >>= 5;
			s.UlongProp >>= 5;
			customClassField.UlongField >>= 5;
			customClassField.UlongProp >>= 5;
			otherCustomStructField.UlongField >>= 5;
			otherCustomStructField.UlongProp >>= 5;
			CustomClassProp.UlongField >>= 5;
			CustomClassProp.UlongProp >>= 5;
			GetClass().UlongField >>= 5;
			GetClass().UlongProp >>= 5;
#if CS70
			GetRefStruct().UlongField >>= 5;
			GetRefStruct().UlongProp >>= 5;
			GetRefUlong() >>= 5;
#endif
		}

		public static void UlongBitAndTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//p &= 5uL;
			//l &= 5uL;
			ulongField &= 5uL;
			UlongProp &= 5uL;
			c.UlongField &= 5uL;
			c.UlongProp &= 5uL;
			s.UlongField &= 5uL;
			s.UlongProp &= 5uL;
			customClassField.UlongField &= 5uL;
			customClassField.UlongProp &= 5uL;
			otherCustomStructField.UlongField &= 5uL;
			otherCustomStructField.UlongProp &= 5uL;
			CustomClassProp.UlongField &= 5uL;
			CustomClassProp.UlongProp &= 5uL;
			GetClass().UlongField &= 5uL;
			GetClass().UlongProp &= 5uL;
#if CS70
			GetRefStruct().UlongField &= 5uL;
			GetRefStruct().UlongProp &= 5uL;
			GetRefUlong() &= 5uL;
#endif
		}

		public static void UlongBitOrTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//p |= 5uL;
			//l |= 5uL;
			ulongField |= 5uL;
			UlongProp |= 5uL;
			c.UlongField |= 5uL;
			c.UlongProp |= 5uL;
			s.UlongField |= 5uL;
			s.UlongProp |= 5uL;
			customClassField.UlongField |= 5uL;
			customClassField.UlongProp |= 5uL;
			otherCustomStructField.UlongField |= 5uL;
			otherCustomStructField.UlongProp |= 5uL;
			CustomClassProp.UlongField |= 5uL;
			CustomClassProp.UlongProp |= 5uL;
			GetClass().UlongField |= 5uL;
			GetClass().UlongProp |= 5uL;
#if CS70
			GetRefStruct().UlongField |= 5uL;
			GetRefStruct().UlongProp |= 5uL;
			GetRefUlong() |= 5uL;
#endif
		}

		public static void UlongBitXorTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//p ^= 5uL;
			//l ^= 5uL;
			ulongField ^= 5uL;
			UlongProp ^= 5uL;
			c.UlongField ^= 5uL;
			c.UlongProp ^= 5uL;
			s.UlongField ^= 5uL;
			s.UlongProp ^= 5uL;
			customClassField.UlongField ^= 5uL;
			customClassField.UlongProp ^= 5uL;
			otherCustomStructField.UlongField ^= 5uL;
			otherCustomStructField.UlongProp ^= 5uL;
			CustomClassProp.UlongField ^= 5uL;
			CustomClassProp.UlongProp ^= 5uL;
			GetClass().UlongField ^= 5uL;
			GetClass().UlongProp ^= 5uL;
#if CS70
			GetRefStruct().UlongField ^= 5uL;
			GetRefStruct().UlongProp ^= 5uL;
			GetRefUlong() ^= 5uL;
#endif
		}

		public static void UlongPostIncTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//X(p++);
			//X(l++);
			X(ulongField++);
			X(UlongProp++);
			X(c.UlongField++);
			X(c.UlongProp++);
			X(s.UlongField++);
			X(s.UlongProp++);
			X(customClassField.UlongField++);
			X(customClassField.UlongProp++);
			X(otherCustomStructField.UlongField++);
			X(otherCustomStructField.UlongProp++);
			X(CustomClassProp.UlongField++);
			X(CustomClassProp.UlongProp++);
			X(GetClass().UlongField++);
			X(GetClass().UlongProp++);
#if CS70
			X(GetRefStruct().UlongField++);
			X(GetRefStruct().UlongProp++);
			X(GetRefUlong()++);
#endif
		}

		public static void UlongPreIncTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//X(++p);
			//X(++l);
			X(++ulongField);
			X(++UlongProp);
			X(++c.UlongField);
			X(++c.UlongProp);
			X(++s.UlongField);
			X(++s.UlongProp);
			X(++customClassField.UlongField);
			X(++customClassField.UlongProp);
			X(++otherCustomStructField.UlongField);
			X(++otherCustomStructField.UlongProp);
			X(++CustomClassProp.UlongField);
			X(++CustomClassProp.UlongProp);
			X(++GetClass().UlongField);
			X(++GetClass().UlongProp);
#if CS70
			X(++GetRefStruct().UlongField);
			X(++GetRefStruct().UlongProp);
			X(++GetRefUlong());
#endif
		}
		public static void UlongPostDecTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//X(p--);
			//X(l--);
			X(ulongField--);
			X(UlongProp--);
			X(c.UlongField--);
			X(c.UlongProp--);
			X(s.UlongField--);
			X(s.UlongProp--);
			X(customClassField.UlongField--);
			X(customClassField.UlongProp--);
			X(otherCustomStructField.UlongField--);
			X(otherCustomStructField.UlongProp--);
			X(CustomClassProp.UlongField--);
			X(CustomClassProp.UlongProp--);
			X(GetClass().UlongField--);
			X(GetClass().UlongProp--);
#if CS70
			X(GetRefStruct().UlongField--);
			X(GetRefStruct().UlongProp--);
			X(GetRefUlong()--);
#endif
		}

		public static void UlongPreDecTest(ulong p, CustomClass c, CustomStruct2 s)
		{
			//ulong l = 0;
			//X(--p);
			//X(--l);
			X(--ulongField);
			X(--UlongProp);
			X(--c.UlongField);
			X(--c.UlongProp);
			X(--s.UlongField);
			X(--s.UlongProp);
			X(--customClassField.UlongField);
			X(--customClassField.UlongProp);
			X(--otherCustomStructField.UlongField);
			X(--otherCustomStructField.UlongProp);
			X(--CustomClassProp.UlongField);
			X(--CustomClassProp.UlongProp);
			X(--GetClass().UlongField);
			X(--GetClass().UlongProp);
#if CS70
			X(--GetRefStruct().UlongField);
			X(--GetRefStruct().UlongProp);
			X(--GetRefUlong());
#endif
		}
		public static void CustomClassAddTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//p += (CustomClass)null;
			//l += (CustomClass)null;
			customClassField += (CustomClass)null;
			CustomClassProp += (CustomClass)null;
			c.CustomClassField += (CustomClass)null;
			c.CustomClassProp += (CustomClass)null;
			s.CustomClassField += (CustomClass)null;
			s.CustomClassProp += (CustomClass)null;
			customClassField.CustomClassField += (CustomClass)null;
			customClassField.CustomClassProp += (CustomClass)null;
			otherCustomStructField.CustomClassField += (CustomClass)null;
			otherCustomStructField.CustomClassProp += (CustomClass)null;
			CustomClassProp.CustomClassField += (CustomClass)null;
			CustomClassProp.CustomClassProp += (CustomClass)null;
			GetClass().CustomClassField += (CustomClass)null;
			GetClass().CustomClassProp += (CustomClass)null;
#if CS70
			GetRefStruct().CustomClassField += (CustomClass)null;
			GetRefStruct().CustomClassProp += (CustomClass)null;
			GetRefCustomClass() += (CustomClass)null;
#endif
		}

		public static void CustomClassSubtractTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//p -= (CustomClass)null;
			//l -= (CustomClass)null;
			customClassField -= (CustomClass)null;
			CustomClassProp -= (CustomClass)null;
			c.CustomClassField -= (CustomClass)null;
			c.CustomClassProp -= (CustomClass)null;
			s.CustomClassField -= (CustomClass)null;
			s.CustomClassProp -= (CustomClass)null;
			customClassField.CustomClassField -= (CustomClass)null;
			customClassField.CustomClassProp -= (CustomClass)null;
			otherCustomStructField.CustomClassField -= (CustomClass)null;
			otherCustomStructField.CustomClassProp -= (CustomClass)null;
			CustomClassProp.CustomClassField -= (CustomClass)null;
			CustomClassProp.CustomClassProp -= (CustomClass)null;
			GetClass().CustomClassField -= (CustomClass)null;
			GetClass().CustomClassProp -= (CustomClass)null;
#if CS70
			GetRefStruct().CustomClassField -= (CustomClass)null;
			GetRefStruct().CustomClassProp -= (CustomClass)null;
			GetRefCustomClass() -= (CustomClass)null;
#endif
		}

		public static void CustomClassMultiplyTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//p *= (CustomClass)null;
			//l *= (CustomClass)null;
			customClassField *= (CustomClass)null;
			CustomClassProp *= (CustomClass)null;
			c.CustomClassField *= (CustomClass)null;
			c.CustomClassProp *= (CustomClass)null;
			s.CustomClassField *= (CustomClass)null;
			s.CustomClassProp *= (CustomClass)null;
			customClassField.CustomClassField *= (CustomClass)null;
			customClassField.CustomClassProp *= (CustomClass)null;
			otherCustomStructField.CustomClassField *= (CustomClass)null;
			otherCustomStructField.CustomClassProp *= (CustomClass)null;
			CustomClassProp.CustomClassField *= (CustomClass)null;
			CustomClassProp.CustomClassProp *= (CustomClass)null;
			GetClass().CustomClassField *= (CustomClass)null;
			GetClass().CustomClassProp *= (CustomClass)null;
#if CS70
			GetRefStruct().CustomClassField *= (CustomClass)null;
			GetRefStruct().CustomClassProp *= (CustomClass)null;
			GetRefCustomClass() *= (CustomClass)null;
#endif
		}

		public static void CustomClassDivideTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//p /= (CustomClass)null;
			//l /= (CustomClass)null;
			customClassField /= (CustomClass)null;
			CustomClassProp /= (CustomClass)null;
			c.CustomClassField /= (CustomClass)null;
			c.CustomClassProp /= (CustomClass)null;
			s.CustomClassField /= (CustomClass)null;
			s.CustomClassProp /= (CustomClass)null;
			customClassField.CustomClassField /= (CustomClass)null;
			customClassField.CustomClassProp /= (CustomClass)null;
			otherCustomStructField.CustomClassField /= (CustomClass)null;
			otherCustomStructField.CustomClassProp /= (CustomClass)null;
			CustomClassProp.CustomClassField /= (CustomClass)null;
			CustomClassProp.CustomClassProp /= (CustomClass)null;
			GetClass().CustomClassField /= (CustomClass)null;
			GetClass().CustomClassProp /= (CustomClass)null;
#if CS70
			GetRefStruct().CustomClassField /= (CustomClass)null;
			GetRefStruct().CustomClassProp /= (CustomClass)null;
			GetRefCustomClass() /= (CustomClass)null;
#endif
		}

		public static void CustomClassModulusTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//p %= (CustomClass)null;
			//l %= (CustomClass)null;
			customClassField %= (CustomClass)null;
			CustomClassProp %= (CustomClass)null;
			c.CustomClassField %= (CustomClass)null;
			c.CustomClassProp %= (CustomClass)null;
			s.CustomClassField %= (CustomClass)null;
			s.CustomClassProp %= (CustomClass)null;
			customClassField.CustomClassField %= (CustomClass)null;
			customClassField.CustomClassProp %= (CustomClass)null;
			otherCustomStructField.CustomClassField %= (CustomClass)null;
			otherCustomStructField.CustomClassProp %= (CustomClass)null;
			CustomClassProp.CustomClassField %= (CustomClass)null;
			CustomClassProp.CustomClassProp %= (CustomClass)null;
			GetClass().CustomClassField %= (CustomClass)null;
			GetClass().CustomClassProp %= (CustomClass)null;
#if CS70
			GetRefStruct().CustomClassField %= (CustomClass)null;
			GetRefStruct().CustomClassProp %= (CustomClass)null;
			GetRefCustomClass() %= (CustomClass)null;
#endif
		}

		public static void CustomClassLeftShiftTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//p <<= 5;
			//l <<= 5;
			customClassField <<= 5;
			CustomClassProp <<= 5;
			c.CustomClassField <<= 5;
			c.CustomClassProp <<= 5;
			s.CustomClassField <<= 5;
			s.CustomClassProp <<= 5;
			customClassField.CustomClassField <<= 5;
			customClassField.CustomClassProp <<= 5;
			otherCustomStructField.CustomClassField <<= 5;
			otherCustomStructField.CustomClassProp <<= 5;
			CustomClassProp.CustomClassField <<= 5;
			CustomClassProp.CustomClassProp <<= 5;
			GetClass().CustomClassField <<= 5;
			GetClass().CustomClassProp <<= 5;
#if CS70
			GetRefStruct().CustomClassField <<= 5;
			GetRefStruct().CustomClassProp <<= 5;
			GetRefCustomClass() <<= 5;
#endif
		}

		public static void CustomClassRightShiftTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//p >>= 5;
			//l >>= 5;
			customClassField >>= 5;
			CustomClassProp >>= 5;
			c.CustomClassField >>= 5;
			c.CustomClassProp >>= 5;
			s.CustomClassField >>= 5;
			s.CustomClassProp >>= 5;
			customClassField.CustomClassField >>= 5;
			customClassField.CustomClassProp >>= 5;
			otherCustomStructField.CustomClassField >>= 5;
			otherCustomStructField.CustomClassProp >>= 5;
			CustomClassProp.CustomClassField >>= 5;
			CustomClassProp.CustomClassProp >>= 5;
			GetClass().CustomClassField >>= 5;
			GetClass().CustomClassProp >>= 5;
#if CS70
			GetRefStruct().CustomClassField >>= 5;
			GetRefStruct().CustomClassProp >>= 5;
			GetRefCustomClass() >>= 5;
#endif
		}

		public static void CustomClassBitAndTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//p &= (CustomClass)null;
			//l &= (CustomClass)null;
			customClassField &= (CustomClass)null;
			CustomClassProp &= (CustomClass)null;
			c.CustomClassField &= (CustomClass)null;
			c.CustomClassProp &= (CustomClass)null;
			s.CustomClassField &= (CustomClass)null;
			s.CustomClassProp &= (CustomClass)null;
			customClassField.CustomClassField &= (CustomClass)null;
			customClassField.CustomClassProp &= (CustomClass)null;
			otherCustomStructField.CustomClassField &= (CustomClass)null;
			otherCustomStructField.CustomClassProp &= (CustomClass)null;
			CustomClassProp.CustomClassField &= (CustomClass)null;
			CustomClassProp.CustomClassProp &= (CustomClass)null;
			GetClass().CustomClassField &= (CustomClass)null;
			GetClass().CustomClassProp &= (CustomClass)null;
#if CS70
			GetRefStruct().CustomClassField &= (CustomClass)null;
			GetRefStruct().CustomClassProp &= (CustomClass)null;
			GetRefCustomClass() &= (CustomClass)null;
#endif
		}

		public static void CustomClassBitOrTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//p |= (CustomClass)null;
			//l |= (CustomClass)null;
			customClassField |= (CustomClass)null;
			CustomClassProp |= (CustomClass)null;
			c.CustomClassField |= (CustomClass)null;
			c.CustomClassProp |= (CustomClass)null;
			s.CustomClassField |= (CustomClass)null;
			s.CustomClassProp |= (CustomClass)null;
			customClassField.CustomClassField |= (CustomClass)null;
			customClassField.CustomClassProp |= (CustomClass)null;
			otherCustomStructField.CustomClassField |= (CustomClass)null;
			otherCustomStructField.CustomClassProp |= (CustomClass)null;
			CustomClassProp.CustomClassField |= (CustomClass)null;
			CustomClassProp.CustomClassProp |= (CustomClass)null;
			GetClass().CustomClassField |= (CustomClass)null;
			GetClass().CustomClassProp |= (CustomClass)null;
#if CS70
			GetRefStruct().CustomClassField |= (CustomClass)null;
			GetRefStruct().CustomClassProp |= (CustomClass)null;
			GetRefCustomClass() |= (CustomClass)null;
#endif
		}

		public static void CustomClassBitXorTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//p ^= (CustomClass)null;
			//l ^= (CustomClass)null;
			customClassField ^= (CustomClass)null;
			CustomClassProp ^= (CustomClass)null;
			c.CustomClassField ^= (CustomClass)null;
			c.CustomClassProp ^= (CustomClass)null;
			s.CustomClassField ^= (CustomClass)null;
			s.CustomClassProp ^= (CustomClass)null;
			customClassField.CustomClassField ^= (CustomClass)null;
			customClassField.CustomClassProp ^= (CustomClass)null;
			otherCustomStructField.CustomClassField ^= (CustomClass)null;
			otherCustomStructField.CustomClassProp ^= (CustomClass)null;
			CustomClassProp.CustomClassField ^= (CustomClass)null;
			CustomClassProp.CustomClassProp ^= (CustomClass)null;
			GetClass().CustomClassField ^= (CustomClass)null;
			GetClass().CustomClassProp ^= (CustomClass)null;
#if CS70
			GetRefStruct().CustomClassField ^= (CustomClass)null;
			GetRefStruct().CustomClassProp ^= (CustomClass)null;
			GetRefCustomClass() ^= (CustomClass)null;
#endif
		}

		public static void CustomClassPostIncTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//X(p++);
			//X(l++);
			X(customClassField++);
			X(CustomClassProp++);
			X(c.CustomClassField++);
			X(c.CustomClassProp++);
			X(s.CustomClassField++);
			X(s.CustomClassProp++);
			X(customClassField.CustomClassField++);
			X(customClassField.CustomClassProp++);
			X(otherCustomStructField.CustomClassField++);
			X(otherCustomStructField.CustomClassProp++);
			X(CustomClassProp.CustomClassField++);
			X(CustomClassProp.CustomClassProp++);
			X(GetClass().CustomClassField++);
			X(GetClass().CustomClassProp++);
#if CS70
			X(GetRefStruct().CustomClassField++);
			X(GetRefStruct().CustomClassProp++);
			X(GetRefCustomClass()++);
#endif
		}

		public static void CustomClassPreIncTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//X(++p);
			//X(++l);
			X(++customClassField);
			X(++CustomClassProp);
			X(++c.CustomClassField);
			X(++c.CustomClassProp);
			X(++s.CustomClassField);
			X(++s.CustomClassProp);
			X(++customClassField.CustomClassField);
			X(++customClassField.CustomClassProp);
			X(++otherCustomStructField.CustomClassField);
			X(++otherCustomStructField.CustomClassProp);
			X(++CustomClassProp.CustomClassField);
			X(++CustomClassProp.CustomClassProp);
			X(++GetClass().CustomClassField);
			X(++GetClass().CustomClassProp);
#if CS70
			X(++GetRefStruct().CustomClassField);
			X(++GetRefStruct().CustomClassProp);
			X(++GetRefCustomClass());
#endif
		}
		public static void CustomClassPostDecTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//X(p--);
			//X(l--);
			X(customClassField--);
			X(CustomClassProp--);
			X(c.CustomClassField--);
			X(c.CustomClassProp--);
			X(s.CustomClassField--);
			X(s.CustomClassProp--);
			X(customClassField.CustomClassField--);
			X(customClassField.CustomClassProp--);
			X(otherCustomStructField.CustomClassField--);
			X(otherCustomStructField.CustomClassProp--);
			X(CustomClassProp.CustomClassField--);
			X(CustomClassProp.CustomClassProp--);
			X(GetClass().CustomClassField--);
			X(GetClass().CustomClassProp--);
#if CS70
			X(GetRefStruct().CustomClassField--);
			X(GetRefStruct().CustomClassProp--);
			X(GetRefCustomClass()--);
#endif
		}

		public static void CustomClassPreDecTest(CustomClass p, CustomClass c, CustomStruct2 s)
		{
			//CustomClass l = null;
			//X(--p);
			//X(--l);
			X(--customClassField);
			X(--CustomClassProp);
			X(--c.CustomClassField);
			X(--c.CustomClassProp);
			X(--s.CustomClassField);
			X(--s.CustomClassProp);
			X(--customClassField.CustomClassField);
			X(--customClassField.CustomClassProp);
			X(--otherCustomStructField.CustomClassField);
			X(--otherCustomStructField.CustomClassProp);
			X(--CustomClassProp.CustomClassField);
			X(--CustomClassProp.CustomClassProp);
			X(--GetClass().CustomClassField);
			X(--GetClass().CustomClassProp);
#if CS70
			X(--GetRefStruct().CustomClassField);
			X(--GetRefStruct().CustomClassProp);
			X(--GetRefCustomClass());
#endif
		}
		public static void CustomStructAddTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//p += default(CustomStruct);
			//l += default(CustomStruct);
			customStructField += default(CustomStruct);
			CustomStructProp += default(CustomStruct);
			c.CustomStructField += default(CustomStruct);
			c.CustomStructProp += default(CustomStruct);
			s.CustomStructField += default(CustomStruct);
			s.CustomStructProp += default(CustomStruct);
			customClassField.CustomStructField += default(CustomStruct);
			customClassField.CustomStructProp += default(CustomStruct);
			otherCustomStructField.CustomStructField += default(CustomStruct);
			otherCustomStructField.CustomStructProp += default(CustomStruct);
			CustomClassProp.CustomStructField += default(CustomStruct);
			CustomClassProp.CustomStructProp += default(CustomStruct);
			GetClass().CustomStructField += default(CustomStruct);
			GetClass().CustomStructProp += default(CustomStruct);
#if CS70
			GetRefStruct().CustomStructField += default(CustomStruct);
			GetRefStruct().CustomStructProp += default(CustomStruct);
			GetRefCustomStruct() += default(CustomStruct);
#endif
		}

		public static void CustomStructSubtractTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//p -= default(CustomStruct);
			//l -= default(CustomStruct);
			customStructField -= default(CustomStruct);
			CustomStructProp -= default(CustomStruct);
			c.CustomStructField -= default(CustomStruct);
			c.CustomStructProp -= default(CustomStruct);
			s.CustomStructField -= default(CustomStruct);
			s.CustomStructProp -= default(CustomStruct);
			customClassField.CustomStructField -= default(CustomStruct);
			customClassField.CustomStructProp -= default(CustomStruct);
			otherCustomStructField.CustomStructField -= default(CustomStruct);
			otherCustomStructField.CustomStructProp -= default(CustomStruct);
			CustomClassProp.CustomStructField -= default(CustomStruct);
			CustomClassProp.CustomStructProp -= default(CustomStruct);
			GetClass().CustomStructField -= default(CustomStruct);
			GetClass().CustomStructProp -= default(CustomStruct);
#if CS70
			GetRefStruct().CustomStructField -= default(CustomStruct);
			GetRefStruct().CustomStructProp -= default(CustomStruct);
			GetRefCustomStruct() -= default(CustomStruct);
#endif
		}

		public static void CustomStructMultiplyTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//p *= default(CustomStruct);
			//l *= default(CustomStruct);
			customStructField *= default(CustomStruct);
			CustomStructProp *= default(CustomStruct);
			c.CustomStructField *= default(CustomStruct);
			c.CustomStructProp *= default(CustomStruct);
			s.CustomStructField *= default(CustomStruct);
			s.CustomStructProp *= default(CustomStruct);
			customClassField.CustomStructField *= default(CustomStruct);
			customClassField.CustomStructProp *= default(CustomStruct);
			otherCustomStructField.CustomStructField *= default(CustomStruct);
			otherCustomStructField.CustomStructProp *= default(CustomStruct);
			CustomClassProp.CustomStructField *= default(CustomStruct);
			CustomClassProp.CustomStructProp *= default(CustomStruct);
			GetClass().CustomStructField *= default(CustomStruct);
			GetClass().CustomStructProp *= default(CustomStruct);
#if CS70
			GetRefStruct().CustomStructField *= default(CustomStruct);
			GetRefStruct().CustomStructProp *= default(CustomStruct);
			GetRefCustomStruct() *= default(CustomStruct);
#endif
		}

		public static void CustomStructDivideTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//p /= default(CustomStruct);
			//l /= default(CustomStruct);
			customStructField /= default(CustomStruct);
			CustomStructProp /= default(CustomStruct);
			c.CustomStructField /= default(CustomStruct);
			c.CustomStructProp /= default(CustomStruct);
			s.CustomStructField /= default(CustomStruct);
			s.CustomStructProp /= default(CustomStruct);
			customClassField.CustomStructField /= default(CustomStruct);
			customClassField.CustomStructProp /= default(CustomStruct);
			otherCustomStructField.CustomStructField /= default(CustomStruct);
			otherCustomStructField.CustomStructProp /= default(CustomStruct);
			CustomClassProp.CustomStructField /= default(CustomStruct);
			CustomClassProp.CustomStructProp /= default(CustomStruct);
			GetClass().CustomStructField /= default(CustomStruct);
			GetClass().CustomStructProp /= default(CustomStruct);
#if CS70
			GetRefStruct().CustomStructField /= default(CustomStruct);
			GetRefStruct().CustomStructProp /= default(CustomStruct);
			GetRefCustomStruct() /= default(CustomStruct);
#endif
		}

		public static void CustomStructModulusTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//p %= default(CustomStruct);
			//l %= default(CustomStruct);
			customStructField %= default(CustomStruct);
			CustomStructProp %= default(CustomStruct);
			c.CustomStructField %= default(CustomStruct);
			c.CustomStructProp %= default(CustomStruct);
			s.CustomStructField %= default(CustomStruct);
			s.CustomStructProp %= default(CustomStruct);
			customClassField.CustomStructField %= default(CustomStruct);
			customClassField.CustomStructProp %= default(CustomStruct);
			otherCustomStructField.CustomStructField %= default(CustomStruct);
			otherCustomStructField.CustomStructProp %= default(CustomStruct);
			CustomClassProp.CustomStructField %= default(CustomStruct);
			CustomClassProp.CustomStructProp %= default(CustomStruct);
			GetClass().CustomStructField %= default(CustomStruct);
			GetClass().CustomStructProp %= default(CustomStruct);
#if CS70
			GetRefStruct().CustomStructField %= default(CustomStruct);
			GetRefStruct().CustomStructProp %= default(CustomStruct);
			GetRefCustomStruct() %= default(CustomStruct);
#endif
		}

		public static void CustomStructLeftShiftTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//p <<= 5;
			//l <<= 5;
			customStructField <<= 5;
			CustomStructProp <<= 5;
			c.CustomStructField <<= 5;
			c.CustomStructProp <<= 5;
			s.CustomStructField <<= 5;
			s.CustomStructProp <<= 5;
			customClassField.CustomStructField <<= 5;
			customClassField.CustomStructProp <<= 5;
			otherCustomStructField.CustomStructField <<= 5;
			otherCustomStructField.CustomStructProp <<= 5;
			CustomClassProp.CustomStructField <<= 5;
			CustomClassProp.CustomStructProp <<= 5;
			GetClass().CustomStructField <<= 5;
			GetClass().CustomStructProp <<= 5;
#if CS70
			GetRefStruct().CustomStructField <<= 5;
			GetRefStruct().CustomStructProp <<= 5;
			GetRefCustomStruct() <<= 5;
#endif
		}

		public static void CustomStructRightShiftTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//p >>= 5;
			//l >>= 5;
			customStructField >>= 5;
			CustomStructProp >>= 5;
			c.CustomStructField >>= 5;
			c.CustomStructProp >>= 5;
			s.CustomStructField >>= 5;
			s.CustomStructProp >>= 5;
			customClassField.CustomStructField >>= 5;
			customClassField.CustomStructProp >>= 5;
			otherCustomStructField.CustomStructField >>= 5;
			otherCustomStructField.CustomStructProp >>= 5;
			CustomClassProp.CustomStructField >>= 5;
			CustomClassProp.CustomStructProp >>= 5;
			GetClass().CustomStructField >>= 5;
			GetClass().CustomStructProp >>= 5;
#if CS70
			GetRefStruct().CustomStructField >>= 5;
			GetRefStruct().CustomStructProp >>= 5;
			GetRefCustomStruct() >>= 5;
#endif
		}

		public static void CustomStructBitAndTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//p &= default(CustomStruct);
			//l &= default(CustomStruct);
			customStructField &= default(CustomStruct);
			CustomStructProp &= default(CustomStruct);
			c.CustomStructField &= default(CustomStruct);
			c.CustomStructProp &= default(CustomStruct);
			s.CustomStructField &= default(CustomStruct);
			s.CustomStructProp &= default(CustomStruct);
			customClassField.CustomStructField &= default(CustomStruct);
			customClassField.CustomStructProp &= default(CustomStruct);
			otherCustomStructField.CustomStructField &= default(CustomStruct);
			otherCustomStructField.CustomStructProp &= default(CustomStruct);
			CustomClassProp.CustomStructField &= default(CustomStruct);
			CustomClassProp.CustomStructProp &= default(CustomStruct);
			GetClass().CustomStructField &= default(CustomStruct);
			GetClass().CustomStructProp &= default(CustomStruct);
#if CS70
			GetRefStruct().CustomStructField &= default(CustomStruct);
			GetRefStruct().CustomStructProp &= default(CustomStruct);
			GetRefCustomStruct() &= default(CustomStruct);
#endif
		}

		public static void CustomStructBitOrTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//p |= default(CustomStruct);
			//l |= default(CustomStruct);
			customStructField |= default(CustomStruct);
			CustomStructProp |= default(CustomStruct);
			c.CustomStructField |= default(CustomStruct);
			c.CustomStructProp |= default(CustomStruct);
			s.CustomStructField |= default(CustomStruct);
			s.CustomStructProp |= default(CustomStruct);
			customClassField.CustomStructField |= default(CustomStruct);
			customClassField.CustomStructProp |= default(CustomStruct);
			otherCustomStructField.CustomStructField |= default(CustomStruct);
			otherCustomStructField.CustomStructProp |= default(CustomStruct);
			CustomClassProp.CustomStructField |= default(CustomStruct);
			CustomClassProp.CustomStructProp |= default(CustomStruct);
			GetClass().CustomStructField |= default(CustomStruct);
			GetClass().CustomStructProp |= default(CustomStruct);
#if CS70
			GetRefStruct().CustomStructField |= default(CustomStruct);
			GetRefStruct().CustomStructProp |= default(CustomStruct);
			GetRefCustomStruct() |= default(CustomStruct);
#endif
		}

		public static void CustomStructBitXorTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//p ^= default(CustomStruct);
			//l ^= default(CustomStruct);
			customStructField ^= default(CustomStruct);
			CustomStructProp ^= default(CustomStruct);
			c.CustomStructField ^= default(CustomStruct);
			c.CustomStructProp ^= default(CustomStruct);
			s.CustomStructField ^= default(CustomStruct);
			s.CustomStructProp ^= default(CustomStruct);
			customClassField.CustomStructField ^= default(CustomStruct);
			customClassField.CustomStructProp ^= default(CustomStruct);
			otherCustomStructField.CustomStructField ^= default(CustomStruct);
			otherCustomStructField.CustomStructProp ^= default(CustomStruct);
			CustomClassProp.CustomStructField ^= default(CustomStruct);
			CustomClassProp.CustomStructProp ^= default(CustomStruct);
			GetClass().CustomStructField ^= default(CustomStruct);
			GetClass().CustomStructProp ^= default(CustomStruct);
#if CS70
			GetRefStruct().CustomStructField ^= default(CustomStruct);
			GetRefStruct().CustomStructProp ^= default(CustomStruct);
			GetRefCustomStruct() ^= default(CustomStruct);
#endif
		}

		public static void CustomStructPostIncTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//X(p++);
			//X(l++);
			X(customStructField++);
			X(CustomStructProp++);
			X(c.CustomStructField++);
			X(c.CustomStructProp++);
			X(s.CustomStructField++);
			X(s.CustomStructProp++);
			X(customClassField.CustomStructField++);
			X(customClassField.CustomStructProp++);
			X(otherCustomStructField.CustomStructField++);
			X(otherCustomStructField.CustomStructProp++);
			X(CustomClassProp.CustomStructField++);
			X(CustomClassProp.CustomStructProp++);
			X(GetClass().CustomStructField++);
			X(GetClass().CustomStructProp++);
#if CS70
			X(GetRefStruct().CustomStructField++);
			X(GetRefStruct().CustomStructProp++);
			X(GetRefCustomStruct()++);
#endif
		}

		public static void CustomStructPreIncTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//X(++p);
			//X(++l);
			X(++customStructField);
			X(++CustomStructProp);
			X(++c.CustomStructField);
			X(++c.CustomStructProp);
			X(++s.CustomStructField);
			X(++s.CustomStructProp);
			X(++customClassField.CustomStructField);
			X(++customClassField.CustomStructProp);
			X(++otherCustomStructField.CustomStructField);
			X(++otherCustomStructField.CustomStructProp);
			X(++CustomClassProp.CustomStructField);
			X(++CustomClassProp.CustomStructProp);
			X(++GetClass().CustomStructField);
			X(++GetClass().CustomStructProp);
#if CS70
			X(++GetRefStruct().CustomStructField);
			X(++GetRefStruct().CustomStructProp);
			X(++GetRefCustomStruct());
#endif
		}
		public static void CustomStructPostDecTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//X(p--);
			//X(l--);
			X(customStructField--);
			X(CustomStructProp--);
			X(c.CustomStructField--);
			X(c.CustomStructProp--);
			X(s.CustomStructField--);
			X(s.CustomStructProp--);
			X(customClassField.CustomStructField--);
			X(customClassField.CustomStructProp--);
			X(otherCustomStructField.CustomStructField--);
			X(otherCustomStructField.CustomStructProp--);
			X(CustomClassProp.CustomStructField--);
			X(CustomClassProp.CustomStructProp--);
			X(GetClass().CustomStructField--);
			X(GetClass().CustomStructProp--);
#if CS70
			X(GetRefStruct().CustomStructField--);
			X(GetRefStruct().CustomStructProp--);
			X(GetRefCustomStruct()--);
#endif
		}

		public static void CustomStructPreDecTest(CustomStruct p, CustomClass c, CustomStruct2 s)
		{
			//CustomStruct l = default(CustomStruct);
			//X(--p);
			//X(--l);
			X(--customStructField);
			X(--CustomStructProp);
			X(--c.CustomStructField);
			X(--c.CustomStructProp);
			X(--s.CustomStructField);
			X(--s.CustomStructProp);
			X(--customClassField.CustomStructField);
			X(--customClassField.CustomStructProp);
			X(--otherCustomStructField.CustomStructField);
			X(--otherCustomStructField.CustomStructProp);
			X(--CustomClassProp.CustomStructField);
			X(--CustomClassProp.CustomStructProp);
			X(--GetClass().CustomStructField);
			X(--GetClass().CustomStructProp);
#if CS70
			X(--GetRefStruct().CustomStructField);
			X(--GetRefStruct().CustomStructProp);
			X(--GetRefCustomStruct());
#endif
		}
		#endregion

		public static void AddOneToCustomClass(ref CustomClass c)
		{
			// This should not be turned into post-increment:
			c += 1;
			c.CustomClassProp += 1;
		}

		private static Item GetItem(object obj)
		{
			return null;
		}

		private static void Issue882()
		{
			Item item = GetItem(null);
			item.Self = item;
		}

		private void Issue954(ref MyEnum a, MyEnum b)
		{
			// cannot decompile to: "a %= b;", because the % operator does not apply to enums
			a = (MyEnum)((int)a % (int)b);
			// same with enum field:
			enumField = (MyEnum)((int)enumField % (int)b);
		}

		private void Issue588(ushort val)
		{
			ushortDict.Add(ushortField++, val);
		}

		private void Issue1007(TimeSpan[] items, int startIndex, TimeSpan item)
		{
			int num = startIndex;
			items[num++] = item;
			items[num++] = item;
		}

#if !LEGACY_CSC
		// Legacy csc generates a slightly different pattern for string compound assignment
		// as for all other compound assignments. We'll ignore that edge case.
		// Note: it's possible that the pre-CopyPropagation run of TransformAssignments is causing trouble there,
		// and that the compound assignment transform would be fine if it didn't get disrupted.

		private static void Issue1082(string[] strings, List<char> chars, bool flag, int i)
		{
			// The 'chars[i]' result is stored in a temporary, and both branches use the
			// same temporary. In order to inline the generated value-type temporary, we
			// need to split it, even though it has the address taken for the ToString() call.
			if (flag)
			{
				strings[1] += chars[i];
			}
			else
			{
				strings[0] += chars[i];
			}
		}
#endif

		private static void StringPropertyCompoundAssign()
		{
			StaticStringProperty += "a";
			StaticStringProperty += 1;
			new CustomClass().StringProp += "a";
			new CustomClass().StringProp += 1;
		}

		public uint PreIncrementIndexer(string name)
		{
			return ++M()[name];
		}

		public int PreIncrementByRef(ref int i)
		{
			return ++i;
		}

		public unsafe int PreIncrementByPointer()
		{
			return ++(*GetPointer());
		}

		public unsafe int PreIncrementOfPointer(int* ptr)
		{
			return *(++ptr);
		}

		public int PreIncrement2DArray()
		{
			return ++Array()[1, 2];
		}

		public int CompoundAssignInstanceField()
		{
			return M().Field *= 10;
		}

		public int CompoundAssignInstanceProperty()
		{
			return M().Property *= 10;
		}

		public int CompoundAssignStaticField()
		{
			return StaticField ^= 100;
		}

		public int CompoundAssignStaticProperty()
		{
			return StaticProperty &= 10;
		}

		public int CompoundAssignArrayElement1(int[] array, int pos)
		{
			return array[pos] *= 10;
		}

		public int CompoundAssignArrayElement2(int[] array)
		{
			return array[Environment.TickCount] *= 10;
		}

		public uint CompoundAssignIndexer(string name)
		{
			return M()[name] -= 2u;
		}

		public uint CompoundAssignIndexerComplexIndex()
		{
			return M()[ToString()] -= 2u;
		}

		public int CompoundAssignIncrement2DArray()
		{
			return Array()[1, 2] %= 10;
		}

		public int CompoundAssignByRef(ref int i)
		{
			return i <<= 2;
		}

		public unsafe int* CompoundAssignOfPointer(int* ptr)
		{
			return ptr += 10;
		}

		public unsafe double CompoundAssignByPointer(double* ptr)
		{
			return *ptr /= 1.5;
		}

		public void CompoundAssignEnum()
		{
			enumField |= MyEnum.Two;
			enumField &= ~MyEnum.Four;
		}

		public int PostIncrementInAddition(int i, int j)
		{
			return i++ + j;
		}

		public void PostIncrementInlineLocalVariable(Func<int, int> f)
		{
			int num = 0;
			f(num++);
		}

		public int PostDecrementArrayElement(int[] array, int pos)
		{
			return array[pos]--;
		}

		public uint PostIncrementIndexer(string name)
		{
			return M()[name]++;
		}

#if false
		public unsafe int PostIncrementOfPointer(int* ptr)
		{
			return *(ptr++);
		}
#endif

		public int PostDecrementInstanceField()
		{
			return M().Field--;
		}

		public int PostDecrementInstanceProperty()
		{
			return M().Property--;
		}

		public int PostIncrement2DArray()
		{
			return Array()[StaticField, StaticProperty]++;
		}

		public int PostIncrementByRef(ref int i)
		{
			return i++;
		}

		public unsafe int PostIncrementByPointer()
		{
			return (*GetPointer())++;
		}

		public float PostIncrementFloat(float f)
		{
			return f++;
		}

		public double PostIncrementDouble(double d)
		{
			return d++;
		}

		public void Issue1552Pre(CustomStruct a, CustomStruct b)
		{
			CustomStruct customStruct = a + b;
			Console.WriteLine(++customStruct);
		}

		public void Issue1552Stmt(CustomStruct a, CustomStruct b)
		{
			CustomStruct customStruct = a + b;
			++customStruct;
		}

		public void Issue1552StmtUseLater(CustomStruct a, CustomStruct b)
		{
			CustomStruct customStruct = a + b;
			++customStruct;
			Console.WriteLine();
			Console.WriteLine(customStruct * b);
		}

		public void Issue1552Decimal(decimal a)
		{
			// Legacy csc compiles this using op_Increment,
			// ensure we don't misdetect this as an invalid pre-increment "++(a * 10m)"
			Console.WriteLine(a * 10m + 1m);
		}

#if !(ROSLYN && OPT)
		// Roslyn opt no longer has a detectable post-increment pattern
		// due to optimizing out some of the stores.
		// Our emitted code is valid but has some additional temporaries.
		public void Issue1552Post(CustomStruct a, CustomStruct b)
		{
			CustomStruct customStruct = a + b;
			Console.WriteLine(customStruct++);
		}

		public void Issue1552StmtTwice(CustomStruct a, CustomStruct b)
		{
			CustomStruct customStruct = a + b;
			++customStruct;
			++customStruct;
		}
#endif

		public void Issue1779(int value)
		{
			CustomStruct2 @struct = GetStruct();
			@struct.IntProp += value;
		}
	}
}
