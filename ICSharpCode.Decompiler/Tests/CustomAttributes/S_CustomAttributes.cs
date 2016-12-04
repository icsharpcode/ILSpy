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

namespace aa
{
	public static class CustomAttributes
	{
		[Flags]
		public enum EnumWithFlag
		{
			All = 15,
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
			MaxUInt64 = 18446744073709551615uL
		}
		[My(EnumWithFlag.Item1 | EnumWithFlag.Item2)]
		private static int field;
		[My(EnumWithFlag.All)]
		public static string Property
		{
			get
			{
				return "aa";
			}
		}
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
			null
		})]
		public static void BoxedLiteralsArray()
		{
		}
	}
}
