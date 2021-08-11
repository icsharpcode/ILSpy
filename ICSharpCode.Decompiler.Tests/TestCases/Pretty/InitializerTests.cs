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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests
{
	public static class Extensions
	{
		public static void Add(this TestCases.CustomList<int> inst, string a, string b)
		{
		}

		public static void Add<T>(this IList<KeyValuePair<string, string>> collection, string key, T value, Func<T, string> convert = null)
		{
		}
	}

	public class TestCases
	{
		#region Types
		public class CustomList<T> : IEnumerable<T>, IEnumerable
		{
			public IEnumerator<T> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			public void Add<T2>(string name)
			{
				new Dictionary<string, Type>().Add(name, typeof(T2));
			}

			public void Add(params int[] ints)
			{
			}
		}

		public class C
		{
			public int Z;
			public S Y;
			public List<S> L;

			public S this[int index] {
				get {
					return default(S);
				}
				set {
				}
			}

			public S this[object key] {
				get {
					return default(S);
				}
				set {
				}
			}
		}

		public struct S
		{
			public int A;
			public int B;

			public S(int a)
			{
				A = a;
				B = 0;
			}
		}

		private enum MyEnum
		{
			a,
			b
		}

		private enum MyEnum2
		{
			c,
			d
		}

		private class Data
		{
			public List<MyEnum2> FieldList = new List<MyEnum2>();
			public MyEnum a { get; set; }
			public MyEnum b { get; set; }
			public List<MyEnum2> PropertyList { get; set; }
#if CS60
			public List<MyEnum2> ReadOnlyPropertyList { get; }
#endif
			public Data MoreData { get; set; }

			public StructData NestedStruct { get; set; }

			public Data this[int i] {
				get {
					return null;
				}
				set {
				}
			}

			public Data this[int i, string j] {
				get {
					return null;
				}
				set {
				}
			}

			public event EventHandler TestEvent;
		}

		private struct StructData
		{
			public int Field;
			public int Property { get; set; }

			public Data MoreData { get; set; }

			public StructData(int initialValue)
			{
				this = default(StructData);
				Field = initialValue;
				Property = initialValue;
			}
		}

		public class Item
		{
			public string Text { get; set; }

			public decimal Value { get; set; }

			public decimal Value2 { get; set; }

			public string Value3 { get; set; }

			public string Value4 { get; set; }

			public string Value5 { get; set; }

			public string Value6 { get; set; }

#if CS90
			public Fields Value7 { get; set; }
#endif
		}

		public class OtherItem
		{
			public decimal Value { get; set; }

			public decimal Value2 { get; set; }

			public decimal? Nullable { get; set; }

			public decimal? Nullable2 { get; set; }

			public decimal? Nullable3 { get; set; }

			public decimal? Nullable4 { get; set; }
		}

		public class OtherItem2
		{
			public readonly OtherItem Data;

			public OtherItem Data2 { get; private set; }
#if CS60
			public OtherItem Data3 { get; }
#endif
		}

		public class V3f
		{
			private float x;
			private float y;
			private float z;

			public V3f(float _x, float _y, float _z)
			{
				x = _x;
				y = _y;
				z = _z;
			}
		}

#if CS90
		public record Fields
		{
			public int A;
			public double B = 1.0;
			public object C;
			public dynamic D;
			public string S = "abc";
			public Item I;
		}
#endif

		public interface IData
		{
			int Property { get; set; }
		}
		#endregion

		private S s1;
		private S s2;

		#region Field initializer tests
		private static V3f[] Issue1336_rg0 = new V3f[3] {
			new V3f(1f, 1f, 1f),
			new V3f(2f, 2f, 2f),
			new V3f(3f, 3f, 3f)
		};

		private static V3f[,] Issue1336_rg1 = new V3f[3, 3] {
			{
				new V3f(1f, 1f, 1f),
				new V3f(2f, 2f, 2f),
				new V3f(3f, 3f, 3f)
			},
			{
				new V3f(2f, 2f, 2f),
				new V3f(3f, 3f, 3f),
				new V3f(4f, 4f, 4f)
			},
			{
				new V3f(3f, 3f, 3f),
				new V3f(4f, 4f, 4f),
				new V3f(5f, 5f, 5f)
			}
		};

		private static V3f[][] Issue1336_rg1b = new V3f[3][] {
			new V3f[3] {
				new V3f(1f, 1f, 1f),
				new V3f(2f, 2f, 2f),
				new V3f(3f, 3f, 3f)
			},
			new V3f[3] {
				new V3f(2f, 2f, 2f),
				new V3f(3f, 3f, 3f),
				new V3f(4f, 4f, 4f)
			},
			new V3f[3] {
				new V3f(3f, 3f, 3f),
				new V3f(4f, 4f, 4f),
				new V3f(5f, 5f, 5f)
			}
		};

		private static V3f[,][] Issue1336_rg1c = new V3f[3, 3][] {
			{
				new V3f[3] {
					new V3f(1f, 1f, 1f),
					new V3f(2f, 2f, 2f),
					new V3f(3f, 3f, 3f)
				},
				new V3f[3] {
					new V3f(2f, 2f, 2f),
					new V3f(3f, 3f, 3f),
					new V3f(4f, 4f, 4f)
				},
				new V3f[3] {
					new V3f(3f, 3f, 3f),
					new V3f(4f, 4f, 4f),
					new V3f(5f, 5f, 5f)
				}
			},
			{
				new V3f[3] {
					new V3f(1f, 1f, 1f),
					new V3f(2f, 2f, 2f),
					new V3f(3f, 3f, 3f)
				},
				new V3f[3] {
					new V3f(2f, 2f, 2f),
					new V3f(3f, 3f, 3f),
					new V3f(4f, 4f, 4f)
				},
				new V3f[3] {
					new V3f(3f, 3f, 3f),
					new V3f(4f, 4f, 4f),
					new V3f(5f, 5f, 5f)
				}
			},
			{
				new V3f[3] {
					new V3f(1f, 1f, 1f),
					new V3f(2f, 2f, 2f),
					new V3f(3f, 3f, 3f)
				},
				new V3f[3] {
					new V3f(2f, 2f, 2f),
					new V3f(3f, 3f, 3f),
					new V3f(4f, 4f, 4f)
				},
				new V3f[3] {
					new V3f(3f, 3f, 3f),
					new V3f(4f, 4f, 4f),
					new V3f(5f, 5f, 5f)
				}
			}
		};

		private static V3f[][,] Issue1336_rg1d = new V3f[2][,] {
			new V3f[3, 3] {
				{
					new V3f(1f, 1f, 1f),
					new V3f(2f, 2f, 2f),
					new V3f(3f, 3f, 3f)
				},
				{
					new V3f(2f, 2f, 2f),
					new V3f(3f, 3f, 3f),
					new V3f(4f, 4f, 4f)
				},
				{
					new V3f(3f, 3f, 3f),
					new V3f(4f, 4f, 4f),
					new V3f(5f, 5f, 5f)
				}
			},
			new V3f[3, 3] {
				{
					new V3f(1f, 1f, 1f),
					new V3f(2f, 2f, 2f),
					new V3f(3f, 3f, 3f)
				},
				{
					new V3f(2f, 2f, 2f),
					new V3f(3f, 3f, 3f),
					new V3f(4f, 4f, 4f)
				},
				{
					new V3f(3f, 3f, 3f),
					new V3f(4f, 4f, 4f),
					new V3f(5f, 5f, 5f)
				}
			}
		};

		private static int[,] Issue1336_rg2 = new int[3, 3] {
			{ 1, 1, 1 },
			{ 1, 1, 1 },
			{ 1, 1, 1 }
		};

#if CS73
		public static ReadOnlySpan<byte> StaticData1 => new byte[1] { 0 };

		public static ReadOnlySpan<byte> StaticData3 => new byte[3] { 1, 2, 3 };

		public static Span<byte> StaticData3Span => new byte[3] { 1, 2, 3 };
#endif
		#endregion

		#region Helper methods used to ensure initializers used within expressions work correctly
		private static void X(object a, object b)
		{
		}

		private static object Y()
		{
			return null;
		}

		public static void TestCall(int a, Thread thread)
		{

		}

		public static C TestCall(int a, C c)
		{
			return c;
		}

		private static int GetInt()
		{
			return 1;
		}

		private static string GetString()
		{
			return "Test";
		}

		private static void NoOp(Guid?[] array)
		{

		}

		private void Data_TestEvent(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Array initializers
		public static void Array1()
		{
			X(Y(), new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
		}

		public static void Array2(int a, int b, int c)
		{
			X(Y(), new int[5] { a, 0, b, 0, c });
		}

		public static void NestedArray(int a, int b, int c)
		{
			X(Y(), new int[3][] {
				new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
				new int[3] { a, b, c },
				new int[6] { 1, 2, 3, 4, 5, 6 }
			});
		}

		public static void NestedNullableArray(int a, int b, int c)
		{
			X(Y(), new int?[3][] {
				new int?[11] {
					1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
					null
				},
				new int?[4] { a, b, c, null },
				new int?[7] { 1, 2, 3, 4, 5, 6, null }
			  });
		}

		public unsafe static void NestedPointerArray(int a, int b, int c)
		{
			X(Y(), new void*[3][] {
				new void*[1] { null },
				new void*[2] {
					(void*)200,
					null
				},
				new void*[2] {
					(void*)100,
					null
				}
			});
		}

		public static void ArrayBoolean()
		{
			X(Y(), new bool[8] { true, false, true, false, false, false, true, true });
		}

		public static void ArrayByte()
		{
			X(Y(), new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 254, 255 });
		}

		public static void ArraySByte()
		{
			X(Y(), new sbyte[8] { -128, -127, 0, 1, 2, 3, 4, 127 });
		}

		public static void ArrayShort()
		{
			X(Y(), new short[5] { -32768, -1, 0, 1, 32767 });
		}

		public static void ArrayUShort()
		{
			X(Y(), new ushort[6] { 0, 1, 32767, 32768, 65534, 65535 });
		}

		public static void ArrayInt()
		{
			X(Y(), new int[10] { 1, -2, 2000000000, 4, 5, -6, 7, 8, 9, 10 });
		}

		public static void ArrayUInt()
		{
			X(Y(), new uint[10] { 1u, 2000000000u, 3000000000u, 4u, 5u, 6u, 7u, 8u, 9u, 10u });
		}

		public static void ArrayLong()
		{
			X(Y(), new long[5] { -4999999999999999999L, -1L, 0L, 1L, 4999999999999999999L });
		}

		public static void ArrayULong()
		{
			X(Y(), new ulong[10] { 1uL, 2000000000uL, 3000000000uL, 4uL, 5uL, 6uL, 7uL, 8uL, 4999999999999999999uL, 9999999999999999999uL });
		}

		public static void ArrayFloat()
		{
			X(Y(), new float[6] {
				-1.5f,
				0f,
				1.5f,
				float.NegativeInfinity,
				float.PositiveInfinity,
				float.NaN
			});
		}

		public static void ArrayDouble()
		{
			X(Y(), new double[6] {
				-1.5,
				0.0,
				1.5,
				double.NegativeInfinity,
				double.PositiveInfinity,
				double.NaN
			});
		}

		public static void ArrayDecimal()
		{
			X(Y(), new decimal[6] { -100m, 0m, 100m, -79228162514264337593543950335m, 79228162514264337593543950335m, 0.0000001m });
		}

		public static void ArrayString()
		{
			X(Y(), new string[4] { "", null, "Hello", "World" });
		}

		public static void ArrayEnum()
		{
			X(Y(), new MyEnum[4] {
				MyEnum.a,
				MyEnum.b,
				MyEnum.a,
				MyEnum.b
			});
		}

		public int[,] MultidimensionalInit()
		{
			return new int[16, 4] {
				{ 0, 0, 0, 0 },
				{ 1, 1, 1, 1 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 1, 0 },
				{ 0, 0, 1, 0 },
				{ 0, 0, 1, 0 },
				{ 0, 0, 1, 0 },
				{ 0, 0, 0, 0 },
				{ 1, 1, 1, 1 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 1, 0 },
				{ 0, 0, 1, 0 },
				{ 0, 0, 1, 0 },
				{ 0, 0, 1, 0 }
			};
		}

		public int[][,] MultidimensionalInit2()
		{
			return new int[4][,] {
				new int[4, 4] {
					{ 0, 0, 0, 0 },
					{ 1, 1, 1, 1 },
					{ 0, 0, 0, 0 },
					{ 0, 0, 0, 0 }
				},
				new int[4, 4] {
					{ 0, 0, 0, 0 },
					{ 1, 1, 1, 1 },
					{ 0, 0, 0, 0 },
					{ 0, 0, 0, 0 }
				},
				new int[4, 4] {
					{ 0, 0, 1, 0 },
					{ 0, 0, 1, 0 },
					{ 0, 0, 1, 0 },
					{ 0, 0, 1, 0 }
				},
				new int[4, 4] {
					{ 0, 0, 1, 0 },
					{ 0, 0, 1, 0 },
					{ 0, 0, 1, 0 },
					{ 0, 0, 1, 0 }
				}
			};
		}

		public int[][,,] ArrayOfArrayOfArrayInit()
		{
			return new int[2][,,] {
				new int[2, 3, 3] {
					{
						{ 1, 2, 3 },
						{ 4, 5, 6 },
						{ 7, 8, 9 }
					},
					{
						{ 11, 12, 13 },
						{ 14, 15, 16 },
						{ 17, 18, 19 }
					}
				},
				new int[2, 3, 3] {
					{
						{ 21, 22, 23 },
						{ 24, 25, 26 },
						{ 27, 28, 29 }
					},
					{
						{ 31, 32, 33 },
						{ 34, 35, 36 },
						{ 37, 38, 39 }
					}
				}
			};
		}

		public static void RecursiveArrayInitializer()
		{
			int[] array = new int[3];
			array[0] = 1;
			array[1] = 2;
			array[2] = array[1] + 1;
			array[0] = 0;
		}

		public static void InvalidIndices(int a)
		{
			int[] array = new int[1];
			array[1] = a;
			X(Y(), array);
		}

		public static void InvalidIndices2(int a)
		{
#pragma warning disable 251
			int[] array = new int[1];
			array[-1] = a;
			X(Y(), array);
#pragma warning restore
		}

		public static void IndicesInWrongOrder(int a, int b)
		{
			int[] array = new int[5];
			array[2] = b;
			array[1] = a;
			X(Y(), array);
		}

		public static byte[] ReverseInitializer(int i)
		{
			byte[] array = new byte[4];
			array[3] = (byte)i;
			array[2] = (byte)(i >> 8);
			array[1] = (byte)(i >> 16);
			array[0] = (byte)(i >> 24);
			return array;
		}

		public static void Issue953_MissingNullableSpecifierForArrayInitializer()
		{
			NoOp(new Guid?[1] { Guid.Empty });
		}

		private void Issue907_Test3(string text)
		{
			X(Y(), new Dictionary<string, object> { { "", text } });
		}

		private int[] Issue1383(int i, int[] array)
		{
			array = new int[4];
			array[i++] = 1;
			array[i++] = 2;
			return array;
		}

		private string[,] Issue1382a()
		{
			return new string[4, 4] {
				{ null, "test", "hello", "world" },
				{ "test", null, "hello", "world" },
				{ "test", "hello", null, "world" },
				{ "test", "hello", "world", null }
			};
		}

		private string[,] Issue1382b()
		{
			return new string[4, 4] {
				{ "test", "hello", "world", null },
				{ "test", "hello", null, "world" },
				{ "test", null, "hello", "world" },
				{ null, "test", "hello", "world" }
			};
		}
		#endregion

		#region Object initializers
		public C Test1()
		{
			C c = new C();
			c.L = new List<S>();
			c.L.Add(new S(1));
			return c;
		}

		public C Test1Alternative()
		{
			return TestCall(1, new C {
				L = new List<S> {
					new S(1)
				}
			});
		}

		public C Test2()
		{
			C c = new C();
			c.Z = 1;
			c.Z = 2;
			return c;
		}

		public C Test3()
		{
			C c = new C();
			c.Y = new S(1);
			c.Y.A = 2;
			return c;
		}

		public C Test3b()
		{
			return TestCall(0, new C {
				Z = 1,
				Y = {
					A = 2
				}
			});
		}

		public C Test4()
		{
			C c = new C();
			c.Y.A = 1;
			c.Z = 2;
			c.Y.B = 3;
			return c;
		}

		public static void ObjectInitializer()
		{
			X(Y(), new Data {
				a = MyEnum.a
			});
		}

		public static void NotAnObjectInitializer()
		{
			Data data = new Data();
			data.a = MyEnum.a;
			X(Y(), data);
		}

		public static void NotAnObjectInitializerWithEvent()
		{
			Data data = new Data();
			data.TestEvent += delegate {
				Console.WriteLine();
			};
			X(Y(), data);
		}

		public static void ObjectInitializerAssignCollectionToField()
		{
			X(Y(), new Data {
				a = MyEnum.a,
				FieldList = new List<MyEnum2> {
					MyEnum2.c,
					MyEnum2.d
				}
			});
		}

		public static void ObjectInitializerAddToCollectionInField()
		{
			X(Y(), new Data {
				a = MyEnum.a,
				FieldList = {
					MyEnum2.c,
					MyEnum2.d
				}
			});
		}

		public static void ObjectInitializerAssignCollectionToProperty()
		{
			X(Y(), new Data {
				a = MyEnum.a,
				PropertyList = new List<MyEnum2> {
					MyEnum2.c,
					MyEnum2.d
				}
			});
		}

		public static void ObjectInitializerAddToCollectionInProperty()
		{
			X(Y(), new Data {
				a = MyEnum.a,
				PropertyList = {
					MyEnum2.c,
					MyEnum2.d
				}
			});
		}

		public static void ObjectInitializerWithInitializationOfNestedObjects()
		{
			X(Y(), new Data {
				MoreData = {
					a = MyEnum.a,
					MoreData = {
						a = MyEnum.b
					}
				}
			});
		}

		public static void ObjectInitializerWithInitializationOfDeeplyNestedObjects()
		{
			X(Y(), new Data {
				a = MyEnum.b,
				MoreData = {
				  a = MyEnum.a,
				  MoreData = {
						MoreData = {
							MoreData = {
								MoreData = {
									MoreData = {
										MoreData = {
											a = MyEnum.b
										}
									}
								}
							}
						}
					}
			  }
			});
		}

		public static void CollectionInitializerInsideObjectInitializers()
		{
			X(Y(), new Data {
				MoreData = new Data {
					a = MyEnum.a,
					b = MyEnum.b,
					PropertyList = { MyEnum2.c }
				}
			});
		}

		public static void NotAStructInitializer_DefaultConstructor()
		{
			StructData structData = default(StructData);
			structData.Field = 1;
			structData.Property = 2;
			X(Y(), structData);
		}

		public static void StructInitializer_DefaultConstructor()
		{
			X(Y(), new StructData {
				Field = 1,
				Property = 2
			});
		}

		public void InliningOfStFldTarget()
		{
			s1 = new S {
				A = 24,
				B = 42
			};
			s2 = new S {
				A = 42,
				B = 24
			};
		}

		public static void NotAStructInitializer_ExplicitConstructor()
		{
			StructData structData = new StructData(0);
			structData.Field = 1;
			structData.Property = 2;
			X(Y(), structData);
		}

		public static void StructInitializer_ExplicitConstructor()
		{
			X(Y(), new StructData(0) {
				Field = 1,
				Property = 2
			});
		}

		public static void StructInitializerWithInitializationOfNestedObjects()
		{
			X(Y(), new StructData {
				MoreData = {
					a = MyEnum.a,
					FieldList = {
						MyEnum2.c,
						MyEnum2.d
					}
				}
			});
		}

		public static void StructInitializerWithinObjectInitializer()
		{
			X(Y(), new Data {
				NestedStruct = new StructData(2) {
					Field = 1,
					Property = 2
				}
			});
		}

		public static void Issue270_NestedInitialisers()
		{
			NumberFormatInfo[] source = null;

			TestCall(0, new Thread(Issue270_NestedInitialisers) {
				Priority = ThreadPriority.BelowNormal,
				CurrentCulture = new CultureInfo(0) {
					DateTimeFormat = new DateTimeFormatInfo {
						ShortDatePattern = "ddmmyy"
					},
					NumberFormat = source.Where((NumberFormatInfo format) => format.CurrencySymbol == "$").First()
				}
			});
		}

		public OtherItem2 Issue1345()
		{
			OtherItem2 otherItem = new OtherItem2();
			otherItem.Data.Nullable = 3m;
			return otherItem;
		}

		public OtherItem2 Issue1345b()
		{
			OtherItem2 otherItem = new OtherItem2();
			otherItem.Data2.Nullable = 3m;
			return otherItem;
		}
#if CS60
		public OtherItem2 Issue1345c()
		{
			OtherItem2 otherItem = new OtherItem2();
			otherItem.Data3.Nullable = 3m;
			return otherItem;
		}

		private Data Issue1345_FalsePositive()
		{
			return new Data {
				ReadOnlyPropertyList = {
					MyEnum2.c,
					MyEnum2.d
				}
			};
		}
#endif

		private void Issue1250_Test1(MyEnum value)
		{
			X(Y(), new C {
				Z = (int)value
			});
		}

		private byte[] Issue1314()
		{
			return new byte[4] { 0, 1, 2, 255 };
		}

		private void Issue1251_Test(List<Item> list, OtherItem otherItem)
		{
			list.Add(new Item {
				Text = "Text",
				Value = otherItem.Value,
				Value2 = otherItem.Value2,
				Value3 = otherItem.Nullable.ToString(),
				Value4 = otherItem.Nullable2.ToString(),
				Value5 = otherItem.Nullable3.ToString(),
				Value6 = otherItem.Nullable4.ToString()
			});
		}

		private Data Issue1279(int p)
		{
			if (p == 1)
			{
				Data data = new Data();
				data.a = MyEnum.a;
				data.TestEvent += Data_TestEvent;
				return data;
			}
			return null;
		}

#if CS90
		private Fields RecordWithNestedClass(Fields input)
		{
			return input with {
				A = 42,
				I = new Item {
					Value7 = input with {
						A = 43
					}
				}
			}
		}
#endif

		private TData GenericObjectInitializer<TData>() where TData : IData, new()
		{
			return new TData {
				Property = 42
			};
		}
		#endregion

		#region Collection initializer

		public static void ExtensionMethodInCollectionInitializer()
		{
#if CS60
			X(Y(), new CustomList<int> { { "1", "2" } });
#else
			CustomList<int> customList = new CustomList<int>();
			customList.Add("1", "2");
			X(Y(), customList);
#endif
		}

		public static void NoCollectionInitializerBecauseOfTypeArguments()
		{
			CustomList<int> customList = new CustomList<int>();
			customList.Add<int>("int");
			Console.WriteLine(customList);
		}

		public static void CollectionInitializerWithParamsMethod()
		{
			X(Y(), new CustomList<int> { { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } });
		}

		public static void CollectionInitializerList()
		{
			X(Y(), new List<int> { 1, 2, 3 });
		}

		public static object RecursiveCollectionInitializer()
		{
			List<object> list = new List<object>();
			list.Add(list);
			return list;
		}

		public static void CollectionInitializerDictionary()
		{
			X(Y(), new Dictionary<string, int> {
				{ "First", 1 },
				{ "Second", 2 },
				{ "Third", 3 }
		  });
		}

		public static void CollectionInitializerDictionaryWithEnumTypes()
		{
			X(Y(), new Dictionary<MyEnum, MyEnum2> {
			{
				MyEnum.a,
				MyEnum2.c
			},
			{
				MyEnum.b,
				MyEnum2.d
			}
		  });
		}

		public static void NotACollectionInitializer()
		{
			List<int> list = new List<int>();
			list.Add(1);
			list.Add(2);
			list.Add(3);
			X(Y(), list);
		}

#if CS60
		public static void SimpleDictInitializer()
		{
			X(Y(), new Data {
				MoreData = {
					a = MyEnum.a,
					[2] = null
				}
			});
		}

		public static void MixedObjectAndDictInitializer()
		{
			X(Y(), new Data {
				MoreData = {
					a = MyEnum.a,
					[GetInt()] = {
						a = MyEnum.b,
						FieldList = { MyEnum2.c },
						[GetInt(), GetString()] = new Data(),
						[2] = null
					}
				}
			});
		}

		private List<List<int>> NestedListWithIndexInitializer(MyEnum myEnum)
		{
			return new List<List<int>> {
				[0] = { 1, 2, 3 },
				[1] = { (int)myEnum }
			};
		}

		private void Issue1250_Test2(MyEnum value)
		{
			X(Y(), new C { [(int)value] = new S((int)value) });
		}

		private void Issue1250_Test3(int value)
		{
			X(Y(), new C { [value] = new S(value) });
		}

		private void Issue1250_Test4(int value)
		{
			X(Y(), new C { [(object)value] = new S(value) });
		}

		public static List<KeyValuePair<string, string>> Issue1390(IEnumerable<string> tokens, bool alwaysAllowAdministrators, char wireDelimiter)
		{
			return new List<KeyValuePair<string, string>> {
			{
				"tokens",
					string.Join(wireDelimiter.ToString(), tokens),
					(Func<string, string>)null
				},
				{
					"alwaysAllowAdministrators",
					alwaysAllowAdministrators.ToString(),
					(Func<string, string>)null
				},
				{
					"delimiter",
					wireDelimiter.ToString(),
					(Func<string, string>)null
				}
			};
		}

#endif
		#endregion
	}
}