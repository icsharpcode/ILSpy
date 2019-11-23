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

	public enum NativeIntEnum : IntPtr
	{
		Zero = 0L,
		One = 1L,
		FortyTwo = 42L
	}
}
