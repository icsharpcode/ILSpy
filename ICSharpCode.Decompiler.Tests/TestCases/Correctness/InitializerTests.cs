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
using System.Globalization;
using System.Linq;
using System.Threading;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	public class InitializerTests
	{
		public static int Main()
		{
			int[,] test = new int[2, 3];
			test[0, 0] = 0;
			test[0, 1] = 1;
			test[0, 2] = 2;
			int result = test.Length + test[0, 0] + test[0, 2];
			Console.WriteLine(result);
			return 0;
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
			public List<InitializerTests.MyEnum2> FieldList = new List<InitializerTests.MyEnum2>();
			public InitializerTests.MyEnum a {
				get;
				set;
			}
			public InitializerTests.MyEnum b {
				get;
				set;
			}
			public List<InitializerTests.MyEnum2> PropertyList {
				get;
				set;
			}

			public InitializerTests.Data MoreData {
				get;
				set;
			}

			public InitializerTests.StructData NestedStruct {
				get;
				set;
			}

			public InitializerTests.Data this[int i] {
				get {
					return null;
				}
				set { }
			}

			public InitializerTests.Data this[int i, string j] {
				get {
					return null;
				}
				set { }
			}
		}

		private struct StructData
		{
			public int Field;
			public int Property {
				get;
				set;
			}

			public InitializerTests.Data MoreData {
				get;
				set;
			}

			public StructData(int initialValue)
			{
				this = default(InitializerTests.StructData);
				this.Field = initialValue;
				this.Property = initialValue;
			}
		}

		// Helper methods used to ensure initializers used within expressions work correctly
		private static void X(object a, object b)
		{
		}

		private static object Y()
		{
			return null;
		}

		public static void CollectionInitializerList()
		{
			InitializerTests.X(InitializerTests.Y(), new List<int>
				{
					1,
					2,
					3
				});
		}

		public static object RecursiveCollectionInitializer()
		{
			List<object> list = new List<object>();
			list.Add(list);
			return list;
		}

		public static void CollectionInitializerDictionary()
		{
			InitializerTests.X(InitializerTests.Y(), new Dictionary<string, int>
		{
			{
				"First",
				1
			},
			{
				"Second",
				2
			},
			{
				"Third",
				3
			}
		  });
		}

		public static void CollectionInitializerDictionaryWithEnumTypes()
		{
			InitializerTests.X(InitializerTests.Y(), new Dictionary<InitializerTests.MyEnum, InitializerTests.MyEnum2>
		{
			{
				InitializerTests.MyEnum.a,
				InitializerTests.MyEnum2.c
			},
			{
				InitializerTests.MyEnum.b,
				InitializerTests.MyEnum2.d
			}
		  });
		}

		public static void NotACollectionInitializer()
		{
			List<int> list = new List<int>();
			list.Add(1);
			list.Add(2);
			list.Add(3);
			InitializerTests.X(InitializerTests.Y(), list);
		}

		public static void ObjectInitializer()
		{
			InitializerTests.X(InitializerTests.Y(), new InitializerTests.Data {
				a = InitializerTests.MyEnum.a
			});
		}

		public static void NotAnObjectInitializer()
		{
			InitializerTests.Data data = new InitializerTests.Data();
			data.a = InitializerTests.MyEnum.a;
			InitializerTests.X(InitializerTests.Y(), data);
		}

		public static void ObjectInitializerAssignCollectionToField()
		{
			InitializerTests.X(InitializerTests.Y(), new InitializerTests.Data {
				a = InitializerTests.MyEnum.a,
				FieldList = new List<InitializerTests.MyEnum2>
				{
					InitializerTests.MyEnum2.c,
					InitializerTests.MyEnum2.d
				}
			});
		}

		public static void ObjectInitializerAddToCollectionInField()
		{
			InitializerTests.X(InitializerTests.Y(), new InitializerTests.Data {
				a = InitializerTests.MyEnum.a,
				FieldList =
				{
					InitializerTests.MyEnum2.c,
					InitializerTests.MyEnum2.d
				}
			});
		}

		public static void ObjectInitializerAssignCollectionToProperty()
		{
			InitializerTests.X(InitializerTests.Y(), new InitializerTests.Data {
				a = InitializerTests.MyEnum.a,
				PropertyList = new List<InitializerTests.MyEnum2>
				{
					InitializerTests.MyEnum2.c,
					InitializerTests.MyEnum2.d
				}
			});
		}

		public static void ObjectInitializerAddToCollectionInProperty()
		{
			InitializerTests.X(InitializerTests.Y(), new InitializerTests.Data {
				a = InitializerTests.MyEnum.a,
				PropertyList =
				{
					InitializerTests.MyEnum2.c,
					InitializerTests.MyEnum2.d
				}
			});
		}

		public static void ObjectInitializerWithInitializationOfNestedObjects()
		{
			InitializerTests.X(InitializerTests.Y(), new InitializerTests.Data {
				MoreData =
				{
					a = InitializerTests.MyEnum.a,
					MoreData = {
						a = InitializerTests.MyEnum.b
					}
				}
			});
		}

		static int GetInt()
		{
			return 1;
		}

		static string GetString()
		{
			return "Test";
		}

#if CS60
		public static void SimpleDictInitializer()
		{
			InitializerTests.X(InitializerTests.Y(), new InitializerTests.Data {
				MoreData =
				{
					a = InitializerTests.MyEnum.a,
					[2] = (Data)null
				}
			});
		}

		public static void MixedObjectAndDictInitializer()
		{
			InitializerTests.X(InitializerTests.Y(), new InitializerTests.Data {
				MoreData =
				{
					a = InitializerTests.MyEnum.a,
					[GetInt()] = {
						a = InitializerTests.MyEnum.b,
						FieldList = { MyEnum2.c },
						[GetInt(), GetString()] = new Data(),
						[2] = (Data)null
					}
				}
			});
		}
#endif

		public static void ObjectInitializerWithInitializationOfDeeplyNestedObjects()
		{
			InitializerTests.X(InitializerTests.Y(), new InitializerTests.Data {
				a = InitializerTests.MyEnum.b,
				MoreData =
			  {
				  a = InitializerTests.MyEnum.a,
				  MoreData = { MoreData = { MoreData = { MoreData = { MoreData = { MoreData = { a = MyEnum.b } } } } } }
			  }
			});
		}

		public static void CollectionInitializerInsideObjectInitializers()
		{
			InitializerTests.Data castPattern = new InitializerTests.Data {
				MoreData = new InitializerTests.Data {
					a = InitializerTests.MyEnum.a,
					b = InitializerTests.MyEnum.b,
					PropertyList = { InitializerTests.MyEnum2.c }
				}
			};
		}

		public static void NotAStructInitializer_DefaultConstructor()
		{
			InitializerTests.StructData data = new InitializerTests.StructData();
			data.Field = 1;
			data.Property = 2;
			InitializerTests.X(InitializerTests.Y(), data);
		}

		public static void StructInitializer_DefaultConstructor()
		{
			InitializerTests.X(InitializerTests.Y(), new InitializerTests.StructData {
				Field = 1,
				Property = 2
			});
		}

		public static void NotAStructInitializer_ExplicitConstructor()
		{
			InitializerTests.StructData data = new InitializerTests.StructData(0);
			data.Field = 1;
			data.Property = 2;
			InitializerTests.X(InitializerTests.Y(), data);
		}

		public static void StructInitializer_ExplicitConstructor()
		{
			InitializerTests.X(InitializerTests.Y(), new InitializerTests.StructData(0) {
				Field = 1,
				Property = 2
			});
		}

		public static void StructInitializerWithInitializationOfNestedObjects()
		{
			InitializerTests.X(InitializerTests.Y(), new InitializerTests.StructData {
				MoreData =
				{
					a = InitializerTests.MyEnum.a,
					FieldList =
					{
						InitializerTests.MyEnum2.c,
						InitializerTests.MyEnum2.d
					}
				}
			});
		}

		public static void StructInitializerWithinObjectInitializer()
		{
			InitializerTests.X(InitializerTests.Y(), new InitializerTests.Data {
				NestedStruct = new InitializerTests.StructData(2) {
					Field = 1,
					Property = 2
				}
			});
		}

		public static void Bug270_NestedInitialisers()
		{
			NumberFormatInfo[] numberFormats = null;

			Thread t = new Thread(Bug270_NestedInitialisers) {
				Priority = ThreadPriority.BelowNormal,
				CurrentCulture = new CultureInfo(0) {
					DateTimeFormat = new DateTimeFormatInfo {
						ShortDatePattern = "ddmmyy"
					},
					NumberFormat = (from format in numberFormats where format.CurrencySymbol == "$" select format).First()
				}
			};

		}

#if CS60
		class Issue2622a
		{
			public class C
			{
				public ServiceHost M()
				{
					return new ServiceHost(typeof(EWSService), null) {
						Description = { Endpoints = { [0] = { Behaviors = { new EwsWebHttpBehavior() } } } }
					};
				}
			}

			class EWSService { }

			public class ServiceHost
			{
				public ServiceHost(Type type, object x) { }

				public Descr Description { get; }
			}

			public class Descr
			{
				public List<EP> Endpoints { get; }
			}

			public class EP
			{
				public List<Beh> Behaviors { get; }
			}

			public abstract class Beh { }

			public class EwsWebHttpBehavior : Beh { }
		}
#endif

		class Issue855
		{
			class Data
			{
				public object Obj;
			}

			class Items
			{
				public void SetItem(int i, object item) { }
			}

			object Item(string s, Data d)
			{
				return new object();
			}

			void Test()
			{
				Items items = null;

				int num = 0;

				for (int i = 0; i < 2; i++)
				{
					if (num < 10)
						items.SetItem(num, Item(string.Empty, new Data { Obj = null }));
				}
			}
		}
	}
}