using System;

namespace TestEnum
{
	public enum BooleanEnum : bool
	{
		Min = false,
		Zero = false,
		One = true,
		Max = byte.MaxValue
	}

	public enum EnumWithNestedClass
	{
#pragma warning disable format
		// error: nested types are not permitted in C#.
		public class NestedClass
		{
		}
		,
#pragma warning enable format
		Zero,
		One
	}

	public enum NativeIntEnum : IntPtr
	{
		Zero = 0L,
		One = 1L,
		FortyTwo = 42L
	}
}
