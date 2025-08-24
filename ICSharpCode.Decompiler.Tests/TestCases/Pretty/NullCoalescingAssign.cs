// Copyright (c) 2018 Daniel Grunwald
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
	// Note: for null coalescing without assignment (??), see
	// LiftedOperators.cs, NullPropagation.cs and ThrowExpressions.cs.
	internal class NullCoalescingAssign
	{
		private class MyClass
		{
			public static string? StaticField1;
			public static int? StaticField2;
			public string? InstanceField1;
			public int? InstanceField2;

			public static string? StaticProperty1 { get; set; }
			public static int? StaticProperty2 { get; set; }

			public string? InstanceProperty1 { get; set; }
			public int? InstanceProperty2 { get; set; }

			public string? this[string name] {
				get {
					return null;
				}
				set {
				}
			}

			public int? this[int index] {
				get {
					return null;
				}
				set {
				}
			}
		}

		private struct MyStruct
		{
			public string? InstanceField1;
			public int? InstanceField2;
			public string? InstanceProperty1 { get; set; }
			public int? InstanceProperty2 { get; set; }

			public string? this[string name] {
				get {
					return null;
				}
				set {
				}
			}

			public int? this[int index] {
				get {
					return null;
				}
				set {
				}
			}
		}

		private static T Get<T>()
		{
			return default(T);
		}

		private static ref T GetRef<T>()
		{
			throw null;
		}

		private static void Use<T>(T t)
		{
		}

		public static void Locals()
		{
			string? local1 = null;
			int? local2 = null;
			local1 ??= "Hello";
			local2 ??= 42;
			Use(local1 ??= "World");
			Use(local2 ??= 42);
		}

		public static void StaticFields()
		{
			MyClass.StaticField1 ??= "Hello";
			MyClass.StaticField2 ??= 42;
			Use(MyClass.StaticField1 ??= "World");
			Use(MyClass.StaticField2 ??= 42);
		}

		public static void InstanceFields()
		{
			Get<MyClass>().InstanceField1 ??= "Hello";
			Get<MyClass>().InstanceField2 ??= 42;
			Use(Get<MyClass>().InstanceField1 ??= "World");
			Use(Get<MyClass>().InstanceField2 ??= 42);
		}

		public static void ArrayElements()
		{
			Get<string[]>()[Get<int>()] ??= "Hello";
			Get<int?[]>()[Get<int>()] ??= 42;
			Use(Get<string[]>()[Get<int>()] ??= "World");
			Use(Get<int?[]>()[Get<int>()] ??= 42);
		}

		public static void InstanceProperties()
		{
			Get<MyClass>().InstanceProperty1 ??= "Hello";
			Get<MyClass>().InstanceProperty2 ??= 42;
			Use(Get<MyClass>().InstanceProperty1 ??= "World");
			Use(Get<MyClass>().InstanceProperty2 ??= 42);
		}

		public static void StaticProperties()
		{
			MyClass.StaticProperty1 ??= "Hello";
			MyClass.StaticProperty2 ??= 42;
			Use(MyClass.StaticProperty1 ??= "World");
			Use(MyClass.StaticProperty2 ??= 42);
		}

		public static void RefReturn()
		{
			GetRef<string>() ??= "Hello";
			GetRef<int?>() ??= 42;
			Use(GetRef<string>() ??= "World");
			Use(GetRef<int?>() ??= 42);
		}

		public static void Dynamic()
		{
			Get<dynamic>().X ??= "Hello";
			Get<dynamic>().Y ??= 42;
			Use(Get<dynamic>().X ??= "Hello");
			Use(Get<dynamic>().Y ??= 42);
		}
	}
}
