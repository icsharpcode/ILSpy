using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !NET8_0_OR_GREATER
namespace System.Numerics
{
	internal static class BitOperations
	{
		private static ReadOnlySpan<byte> TrailingZeroCountDeBruijn => new byte[32]
{
			00, 01, 28, 02, 29, 14, 24, 03,
			30, 22, 20, 15, 25, 17, 04, 08,
			31, 27, 13, 23, 21, 19, 16, 07,
			26, 12, 18, 06, 11, 05, 10, 09
};
		public static int TrailingZeroCount(uint value)
		{
			// Unguarded fallback contract is 0->0, BSF contract is 0->undefined
			if (value == 0)
			{
				return 32;
			}

			unchecked
			{
				// uint.MaxValue >> 27 is always in range [0 - 31] so we use Unsafe.AddByteOffset to avoid bounds check
				return Unsafe.AddByteOffset(
					// Using deBruijn sequence, k=2, n=5 (2^5=32) : 0b_0000_0111_0111_1100_1011_0101_0011_0001u
					ref MemoryMarshal.GetReference(TrailingZeroCountDeBruijn),
					// uint|long -> IntPtr cast on 32-bit platforms does expensive overflow checks not needed here
					(IntPtr)(int)(((value & (uint)-(int)value) * 0x077CB531u) >> 27)); // Multi-cast mitigates redundant conv.u8
			}
		}

		public static int TrailingZeroCount(ulong value)
		{
			unchecked
			{
				uint lo = (uint)value;

				if (lo == 0)
				{
					return 32 + TrailingZeroCount((uint)(value >> 32));
				}

				return TrailingZeroCount(lo);
			}
		}
	}
}
#endif
