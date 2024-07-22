﻿// Copyright (c) 2016 Daniel Grunwald
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
			CallAmbiguousOutParam();
			CallWithInParam();
			CallWithRefReadOnlyParam();
#if CS90
			NativeIntTests(new IntPtr(1), 2);
#endif
			Issue2444.M2();
			Issue2741.B.Test(new Issue2741.C());
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

		#region Out Parameter
		static void AmbiguousOutParam(out string a)
		{
			a = null;
			Console.WriteLine("AmbiguousOutParam(out string)");
		}

		static void AmbiguousOutParam(out int b)
		{
			b = 1;
			Console.WriteLine("AmbiguousOutParam(out int)");
		}

		static void CallAmbiguousOutParam()
		{
			Console.WriteLine("CallAmbiguousOutParam:");
			string a;
			int b;
			AmbiguousOutParam(out a);
			AmbiguousOutParam(out b);
		}
		#endregion

		#region Ref readonly Parameter

		static void CallWithRefReadOnlyParam()
		{
#if CS120
#pragma warning disable CS9193
			Console.WriteLine("OverloadSetWithRefReadOnlyParam:");
			OverloadSetWithRefReadOnlyParam(1);
			OverloadSetWithRefReadOnlyParam(2L);
			int i = 3;
			OverloadSetWithRefReadOnlyParam(in i);
			OverloadSetWithRefReadOnlyParam((long)4);

			Console.WriteLine("OverloadSetWithRefReadOnlyParam2:");
			OverloadSetWithRefReadOnlyParam2(1);
			OverloadSetWithRefReadOnlyParam2((object)1);

			Console.WriteLine("OverloadSetWithRefReadOnlyParam3:");
			OverloadSetWithRefReadOnlyParam3(1);
			OverloadSetWithRefReadOnlyParam3<int>(2);
			OverloadSetWithRefReadOnlyParam3((object)3);

			Console.WriteLine("RefReadOnlyVsRegularParam:");
			RefReadOnlyVsRegularParam(1);
			i = 2;
			RefReadOnlyVsRegularParam(in i);
#endif
		}

#if CS120
		static void OverloadSetWithRefReadOnlyParam(ref readonly int i)
		{
			Console.WriteLine("ref readonly int " + i);
		}
		static void OverloadSetWithRefReadOnlyParam(long l)
		{
			Console.WriteLine("long " + l);
		}
		static void OverloadSetWithRefReadOnlyParam2(ref readonly long i)
		{
			Console.WriteLine("ref readonly long " + i);
		}
		static void OverloadSetWithRefReadOnlyParam2(object o)
		{
			Console.WriteLine("object " + o);
		}
		static void OverloadSetWithRefReadOnlyParam3(ref readonly int i)
		{
			Console.WriteLine("ref readonly int " + i);
		}
		static void OverloadSetWithRefReadOnlyParam3<T>(T a)
		{
			Console.WriteLine("T " + a);
		}
		static void RefReadOnlyVsRegularParam(ref readonly int i)
		{
			Console.WriteLine("ref readonly int " + i);
		}
		static void RefReadOnlyVsRegularParam(int i)
		{
			Console.WriteLine("int " + i);
		}

#endif

		#endregion

		#region In Parameter
		static void CallWithInParam()
		{
#if CS72
			Console.WriteLine("OverloadSetWithInParam:");
			OverloadSetWithInParam(1);
			OverloadSetWithInParam(2L);
			int i = 3;
			OverloadSetWithInParam(in i);
			OverloadSetWithInParam((long)4);

			Console.WriteLine("OverloadSetWithInParam2:");
			OverloadSetWithInParam2(1);
			OverloadSetWithInParam2((object)1);

			Console.WriteLine("OverloadSetWithInParam3:");
			OverloadSetWithInParam3(1);
			OverloadSetWithInParam3<int>(2);
			OverloadSetWithInParam3((object)3);

			Console.WriteLine("InVsRegularParam:");
			InVsRegularParam(1);
			i = 2;
			InVsRegularParam(in i);
#endif
		}

#if CS72
		static void OverloadSetWithInParam(in int i)
		{
			Console.WriteLine("in int " + i);
		}
		static void OverloadSetWithInParam(long l)
		{
			Console.WriteLine("long " + l);
		}
		static void OverloadSetWithInParam2(in long i)
		{
			Console.WriteLine("in long " + i);
		}
		static void OverloadSetWithInParam2(object o)
		{
			Console.WriteLine("object " + o);
		}
		static void OverloadSetWithInParam3(in int i)
		{
			Console.WriteLine("in int " + i);
		}
		static void OverloadSetWithInParam3<T>(T a)
		{
			Console.WriteLine("T " + a);
		}
		static void InVsRegularParam(in int i)
		{
			Console.WriteLine("in int " + i);
		}
		static void InVsRegularParam(int i)
		{
			Console.WriteLine("int " + i);
		}
#endif
		#endregion

#if CS90
		static void NativeIntTests(IntPtr i1, nint i2)
		{
			Console.WriteLine("NativeIntTests(i1):");
			ObjectOrLong((object)i1);
			ObjectOrLong((long)i1);
			Console.WriteLine("NativeIntTests(i2):");
			ObjectOrLong((object)i2);
			ObjectOrLong((long)i2);
			Console.WriteLine("NativeIntTests(new IntPtr):");
			ObjectOrLong((object)new IntPtr(3));
			ObjectOrLong((long)new IntPtr(3));
			Console.WriteLine("NativeIntTests(IntPtr.Zero):");
			ObjectOrLong((object)IntPtr.Zero);
			ObjectOrLong((long)IntPtr.Zero);
		}

		static void ObjectOrLong(object o)
		{
			Console.WriteLine("object " + o);
		}

		static void ObjectOrLong(long l)
		{
			Console.WriteLine("long " + l);
		}
#endif

		#region #2444
		public struct Issue2444
		{
			public class X { }
			public class Y { }

			public static implicit operator Issue2444(X x)
			{
				Console.WriteLine("#2444: op_Implicit(X)");
				return new Issue2444();
			}

			public static implicit operator Issue2444(Y y)
			{
				Console.WriteLine("#2444: op_Implicit(Y)");
				return new Issue2444();
			}

			public static void M1(Issue2444 z)
			{
				Console.WriteLine(string.Format("#2444: M1({0})", z));
			}

			public static void M2()
			{
				Console.WriteLine("#2444: before M1");
				M1((X)null);
				Console.WriteLine("#2444: after M1");
			}
		}

		public class Issue2741
		{
			public class B
			{
				private void M()
				{
					Console.WriteLine("B::M");
				}

				protected void M2()
				{
					Console.WriteLine("B::M2");
				}

				protected void M3()
				{
					Console.WriteLine("B::M3");
				}

				protected void M4()
				{
					Console.WriteLine("B::M4");
				}

				public static void Test(C c)
				{
					((B)c).M();
					((B)c).M2();
					c.Test();
				}
			}

			public class C : B
			{
				public void M()
				{
					Console.WriteLine("C::M");
				}

				public new void M2()
				{
					Console.WriteLine("C::M2");
				}

				public new void M3()
				{
					Console.WriteLine("C::M3");
				}

				public void Test()
				{
					M3();
					base.M3();
					M4();
				}
			}
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
