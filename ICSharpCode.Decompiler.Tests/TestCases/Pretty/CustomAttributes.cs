// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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

namespace CustomAttributes
{
	public static class CustomAttributes
	{
		[Flags]
		public enum EnumWithFlag
		{
			All = 0xF,
			None = 0,
			Item1 = 1,
			Item2 = 2,
			Item3 = 4,
			Item4 = 8
		}
		[AttributeUsage(AttributeTargets.All)]
		public class MyAttribute : Attribute
		{
			public MyAttribute(object val)
			{
			}
		}
		[My(ULongEnum.MaxUInt64)]
		public enum ULongEnum : ulong
		{
			[My(null)]
			MaxUInt64 = ulong.MaxValue
		}
		[AttributeUsage(AttributeTargets.Field)]
		public class TypesAttribute : Attribute
		{
			public TypesAttribute(Type type)
			{
			}
		}
		private class SomeType<T>
		{
		}
		private class SomeType<K, V>
		{
		}
		private struct DataType
		{
			private int i;
		}
		[Types(typeof(int))]
		private static int typeattr_int;
		[Types(null)]
		private static int typeattr_null;
		[Types(typeof(List<int>))]
		private static int typeattr_list_of_int;
		[Types(typeof(List<>))]
		private static int typeattr_list_unbound;
		[Types(typeof(SomeType<DataType>))]
		private static int typeattr_sometype_of_datatype;
		[Types(typeof(SomeType<DataType, DataType>))]
		private static int typeattr_sometype_of_datatype2;
		[Types(typeof(SomeType<DataType, int>))]
		private static int typeattr_sometype_of_datatype_and_int;
		[Types(typeof(SomeType<DataType[], int>))]
		private static int typeattr_sometype_of_datatype_array_and_int;
		[Types(typeof(SomeType<SomeType<DataType>, int>))]
		private static int typeattr_sometype_of_nested_sometype;
		[Types(typeof(SomeType<int, DataType>))]
		private static int typeattr_sometype_of_int_and_datatype;
		[Types(typeof(int[]))]
		private static int typeattr_array_of_int;
		[Types(typeof(int[,,,][,]))]
		private static int typeattr_multidim_array_of_int;

		[My(EnumWithFlag.Item1 | EnumWithFlag.Item2)]
		private static int field;
		[My(EnumWithFlag.All)]
#if ROSLYN
		public static string Property => "aa";
#else
		public static string Property {
			get {
				return "aa";
			}
		}
#endif
		[Obsolete("some message")]
		public static void ObsoletedMethod()
		{
		}
		// No Boxing
		[My(new StringComparison[] {
			StringComparison.Ordinal,
			StringComparison.CurrentCulture
		})]
		public static void EnumArray()
		{
		}
		// Boxing of each array element
		[My(new object[] {
			StringComparison.Ordinal,
			StringComparison.CurrentCulture
		})]
		public static void BoxedEnumArray()
		{
		}
		[My(new object[] {
			1,
			2u,
			3L,
			4uL,
			// Ensure the decompiler doesn't leave these casts out:
			(short)5,
			(ushort)6,
			(byte)7,
			(sbyte)8,
			'a',
			'\0',
			'\ufeff',
			'\uffff',
			1f,
			2.0,
			"text",
			null,
			typeof(int),
			new object[] { 1 },
			new int[] { 1 }
		})]
		public static void BoxedLiteralsArray()
		{
		}
	}
}
