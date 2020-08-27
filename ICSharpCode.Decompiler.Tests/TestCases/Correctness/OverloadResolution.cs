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

using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	static class OverloadResolution
	{
		static void Main()
		{
			CallOverloadedMethod();
			TestBoxing();
			TestIssue180();
			TestExtensionMethod();
			TestParamsMethod();
			Generics();
			ConstructorTest();
			TestIndexer();
			Issue1281();
			Issue1747();
		}

		#region ConstructorTest
		static void ConstructorTest()
		{
			new CtorTestObj(1);
			new CtorTestObj((short)2);
			new CtorTestObj(3, null);
			new CtorTestObj(4, null, null);
		}

		class CtorTestObj
		{
			public CtorTestObj(int i)
			{
				Console.WriteLine("CtorTestObj(int = " + i + ")");
			}

			public CtorTestObj(short s)
			{
				Console.WriteLine("CtorTestObj(short = " + s + ")");
			}

			public CtorTestObj(int i, object item1, object item2)
				: this(i, new object[] { item1, item2 })
			{
				Console.WriteLine("CtorTestObj(int = " + i + ", item1 = " + item1 + ", item2 = " + item2);
			}

			public CtorTestObj(int i, params object[] items)
			{
				Console.WriteLine("CtorTestObj(int = " + i + ", items = " + (items == null ? "null" : items.Length.ToString()) + ")");
			}
		}
		#endregion

		#region params with nulls
		static void TestParamsMethod()
		{
			TestCall(1, null, (NullReferenceException)null);
			TestCall(2, null, (AccessViolationException)null);
			TestCall(3, null);
			TestCall(3, null, null, null);
		}

		static void TestCall(int v, Type p1, NullReferenceException p2)
		{
			Console.WriteLine("TestCall without params");
		}

		static void TestCall(int v, params AccessViolationException[] p2)
		{
			Console.WriteLine("TestCall with params: " + (p2 == null ? "null" : p2.Length.ToString()));
		}

		static void Issue1281()
		{
			var arg = new object[0];
			TestCallIssue1281(arg);
			TestCallIssue1281((object)arg);
			TestCallIssue1281(new[] { arg });
		}

		static void TestCallIssue1281(params object[] args)
		{
			Console.Write("TestCallIssue1281: count = " + args.Length + ": ");
			foreach (var arg in args)
			{
				Console.Write(arg);
				Console.Write(", ");
			}
			Console.WriteLine();
		}
		#endregion

		#region Simple Overloaded Method
		static void CallOverloadedMethod()
		{
			OverloadedMethod("(string)");
			OverloadedMethod((object)"(object)");
			OverloadedMethod(5);
			OverloadedMethod((object)5);
			OverloadedMethod(5L);
			OverloadedMethod((object)null);
			OverloadedMethod((string)null);
			OverloadedMethod((int?)null);
		}

		static void OverloadedMethod(object a)
		{
			Console.WriteLine("OverloadedMethod(object={0}, object.GetType()={1})", a, a != null ? a.GetType().Name : "null");
		}

		static void OverloadedMethod(int? a)
		{
			Console.WriteLine("OverloadedMethod(int?={0})", a);
		}

		static void OverloadedMethod(string a)
		{
			Console.WriteLine("OverloadedMethod(string={0})", a);
		}
		#endregion

		#region Boxing
		static void TestBoxing()
		{
			Print(1);
			Print((ushort)1);
			Print(null);
		}

		static void Print(object obj)
		{
			if (obj == null)
				Console.WriteLine("null");
			else
				Console.WriteLine("{0}: {1}", obj.GetType().Name, obj);
		}
		#endregion

		#region #180
		static void TestIssue180()
		{
			Issue180(null);
			Issue180(new object[1]);
			Issue180((object)new object[1]);
		}

		static void Issue180(object obj)
		{
			Console.WriteLine("#180: object");
		}

		static void Issue180(params object[] objs)
		{
			Console.WriteLine("#180: params object[]");
		}
		#endregion

		#region Extension Method
		static void TestExtensionMethod()
		{
			new object().ExtensionMethod();
			ExtensionMethod(null); // issue #167
		}

		public static void ExtensionMethod(this object obj)
		{
			Console.WriteLine("ExtensionMethod(obj)");
		}
		#endregion

		#region Generics
		static void Generics()
		{
			GenericsTest<int>(null);
			GenericsTest<long>((object)null);
		}

		static void GenericsTest<T>(string x) where T : struct
		{
			Console.WriteLine("GenericsTest<" + typeof(T).Name + ">(string: " + x + ");");
		}

		static void GenericsTest<T>(object x) where T : struct
		{
			Console.WriteLine("GenericsTest<" + typeof(T).Name + ">(object: " + x + ");");
		}
		#endregion

		#region NullableValueTypes
		private static void Issue1747()
		{
			Console.WriteLine("Issue1747:");
			M1747(null);
			M1747(true);
			M1747(false);
			M1747((bool?)true);
			M1747((bool?)false);
			Console.WriteLine("Issue1747, non-constant:");
			bool b = Get<bool>();
			M1747(b);
			M1747((bool?)b);
		}

		private static void M1747(bool b)
		{
			Console.WriteLine("bool=" + b);
		}

		private static void M1747(bool? b)
		{
			Console.WriteLine("bool?=" + b);
		}

		static T Get<T>()
		{
			return default(T);
		}
		#endregion

		#region IndexerTests
		static void TestIndexer()
		{
			var obj = new IndexerTests();
			Console.WriteLine(obj[(object)5]);
			obj[(object)5] = null;
			Console.WriteLine(obj[5]);
			obj[5] = null;
		}
		#endregion
	}

	class IndexerTests
	{
		public object this[object key] {
			get {
				Console.WriteLine("IndexerTests.get_Item(object key)");
				return new object();
			}
			set {
				Console.WriteLine("IndexerTests.set_Item(object key, object value)");
			}
		}

		public object this[int key] {
			get {
				Console.WriteLine("IndexerTests.get_Item(int key)");
				return new object();
			}
			set {
				Console.WriteLine("IndexerTests.set_Item(int key, object value)");
			}
		}
	}
}
