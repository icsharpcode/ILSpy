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
	internal class Generics
	{
		private class GenericClass<T>
		{
			private readonly T issue1760;

			public void M(out GenericClass<T> self)
			{
				self = this;
			}

			public void Issue1760()
			{
				Console.WriteLine(", " + issue1760);
			}
		}

		public class BaseClass
		{
		}

		public class DerivedClass : BaseClass
		{
		}

		public class MyArray<T>
		{
			public class NestedClass<Y>
			{
				public T Item1;
				public Y Item2;
			}

			public enum NestedEnum
			{
				A,
				B
			}

			private T[] arr;

			public MyArray(int capacity)
			{
				arr = new T[capacity];
			}

			public void Size(int capacity)
			{
				Array.Resize(ref arr, capacity);
			}

			public void Grow(int capacity)
			{
				if (capacity >= arr.Length)
				{
					Size(capacity);
				}
			}
		}

		public interface IInterface
		{
			void Method1<T>() where T : class;
			void Method2<T>() where T : class;
		}

		public abstract class Base : IInterface
		{
			// constraints must be repeated on implicit interface implementation
			public abstract void Method1<T>() where T : class;

			// constraints must not be specified on explicit interface implementation
			void IInterface.Method2<T>()
			{
			}
		}

		public class Derived : Base
		{
			// constraints are inherited automatically and must not be specified
			public override void Method1<T>()
			{
			}
		}

		private const MyArray<string>.NestedEnum enumVal = MyArray<string>.NestedEnum.A;
		private static Type type1 = typeof(List<>);
		private static Type type2 = typeof(MyArray<>);
		private static Type type3 = typeof(List<>.Enumerator);
		private static Type type4 = typeof(MyArray<>.NestedClass<>);
		private static Type type5 = typeof(List<int>[]);
		private static Type type6 = typeof(MyArray<>.NestedEnum);

		public T CastToTypeParameter<T>(DerivedClass d) where T : BaseClass
		{
			return (T)(BaseClass)d;
		}

		public TTarget GenericAsGeneric<TSource, TTarget>(TSource source) where TTarget : class
		{
			return source as TTarget;
		}

		public TTarget? GenericAsNullable<TSource, TTarget>(TSource source) where TTarget : struct
		{
			return source as TTarget?;
		}

		public TTarget ObjectAsGeneric<TTarget>(object source) where TTarget : class
		{
			return source as TTarget;
		}

		public TTarget? ObjectAsNullable<TTarget>(object source) where TTarget : struct
		{
			return source as TTarget?;
		}

		public TTarget IntAsGeneric<TTarget>(int source) where TTarget : class
		{
			return source as TTarget;
		}

		public TTarget? IntAsNullable<TTarget>(int source) where TTarget : struct
		{
			return source as TTarget?;
		}

		public T New<T>() where T : new()
		{
			return new T();
		}

		public T NotNew<T>()
		{
			return Activator.CreateInstance<T>();
		}

		public bool IsNull<T>(T t)
		{
			return t == null;
		}

		public T[] NewArray<T>(int size)
		{
			return new T[size];
		}

		public T[,] NewArray<T>(int size1, int size2)
		{
			return new T[size1, size2];
		}

		public Type[] TestTypeOf()
		{
			return new Type[8] {
				typeof(int),
				typeof(int[]),
				typeof(GenericClass<>),
				typeof(GenericClass<int>),
				typeof(GenericClass<int[]>),
				typeof(Dictionary<, >),
				typeof(List<int>.Enumerator),
				typeof(List<>.Enumerator)
			};
		}

		public static void MethodWithConstraint<T, S>() where T : class, S where S : ICloneable, new()
		{
		}

		public static void MethodWithStructConstraint<T>() where T : struct
		{
		}

		private static void MultidimensionalArray<T>(T[,] array)
		{
			array[0, 0] = array[0, 1];
		}

		public static Dictionary<string, string>.KeyCollection.Enumerator GetEnumerator(Dictionary<string, string> d, MyArray<string>.NestedClass<int> nc)
		{
			// Tests references to inner classes in generic classes
			return d.Keys.GetEnumerator();
		}

		public static bool IsString<T>(T input)
		{
			return input is string;
		}

		public static string AsString<T>(T input)
		{
			return input as string;
		}

		public static string CastToString<T>(T input)
		{
			return (string)(object)input;
		}

		public static T CastFromString<T>(string input)
		{
			return (T)(object)input;
		}

		public static bool IsInt<T>(T input)
		{
			return input is int;
		}

		public static int CastToInt<T>(T input)
		{
			return (int)(object)input;
		}

		public static T CastFromInt<T>(int input)
		{
			return (T)(object)input;
		}

		public static bool IsNullableInt<T>(T input)
		{
			return input is int?;
		}

		public static int? AsNullableInt<T>(T input)
		{
			return input as int?;
		}

		public static int? CastToNullableInt<T>(T input)
		{
			return (int?)(object)input;
		}

		public static T CastFromNullableInt<T>(int? input)
		{
			return (T)(object)input;
		}

#if CS73
		public static object CallDelegate<T>(T input) where T : Delegate
		{
			return input.DynamicInvoke();
		}

		public static int CountEnumerators<T>() where T : Enum
		{
			return typeof(T).GetEnumValues().Length;
		}

		public unsafe static int UnmanagedConstraint<T>() where T : unmanaged
		{
			return sizeof(T);
		}
#endif

		public static void Issue1959(int a, int b, int? c)
		{
			// This line requires parentheses around `a < b` to avoid a grammar ambiguity.
			Console.WriteLine("{}, {}", (a < b), a > (c ?? b));
			// But here there's no ambiguity:
			Console.WriteLine("{}, {}", a < b, a > b);
			Console.WriteLine("{}, {}", a < Environment.GetLogicalDrives().Length, a > (c ?? b));
		}

		public static Type Issue2231<T>()
		{
			return default(T).GetType();
		}

		public static string Issue2231b<T>()
		{
			return default(T).ToString();
		}
	}
}
