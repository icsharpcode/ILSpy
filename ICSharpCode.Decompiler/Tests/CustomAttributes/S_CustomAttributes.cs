// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

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
		public static void ArrayAsAttribute1()
		{
		}
		// Boxing of each array element
		[My(new object[] {
			StringComparison.Ordinal, 
			StringComparison.CurrentCulture
		})]
		public static void ArrayAsAttribute2()
		{
		}
	}
}