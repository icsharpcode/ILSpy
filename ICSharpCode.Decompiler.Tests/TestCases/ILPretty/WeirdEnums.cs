using System;
using System.Runtime.InteropServices;

namespace TestEnum
{
	[StructLayout(LayoutKind.Auto)]
	public struct ArrayEnum : Enum
	{
		public int[] value__;
	}

	public enum BooleanEnum : bool
	{
		Min = false,
		Zero = false,
		One = true,
		Max = byte.MaxValue
	}

	public enum DoubleEnum : double
	{
		Half = 0.5,
		One = 1.0
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

	public enum FloatEnum : float
	{
		Half = 0.5f,
		One = 1f
	}

	public enum NativeIntEnum : IntPtr
	{
		Zero = 0L,
		One = 1L,
		FortyTwo = 42L
	}
}
