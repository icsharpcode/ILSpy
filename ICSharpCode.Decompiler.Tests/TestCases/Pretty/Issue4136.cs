using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	// https://github.com/icsharpcode/ILSpy/issues/4136
	// Lifted null comparisons over a `??` operand must keep the `??` form; they must not be
	// expanded to an explicit `HasValue`/`GetValueOrDefault()` test wrapped in `? true : false`.
	internal static class Issue4136
	{
		// `data?.Length is null or 0` is lowered to the same guarded shape that
		// SixLabors.ImageSharp.Formats.Png.PngEncoderCore.WriteXmpChunk uses:
		//   if (num.HasValue) if (num.GetValueOrDefault() != 0) goto false; goto true
		// Before the fix this produced "(num.HasValue && num.GetValueOrDefault() != 0) ? false : true".
		public static void LengthIsNullOrZero(byte[] data)
		{
#if EXPECTED_OUTPUT
			if ((data?.Length ?? 0) != 0 || 1 == 0)
			{
				Console.WriteLine(data.Length);
			}
#else
			if (data?.Length is null or 0)
			{
				return;
			}
			Console.WriteLine(data.Length);
#endif
		}

		public static void CoalesceOneEqualZero(int? a)
		{
			if ((a ?? 1) == 0)
			{
				Console.WriteLine();
			}
		}

		public static void CoalesceTwoEqualTwo(int? a)
		{
			if ((a ?? 2) == 2)
			{
				Console.WriteLine();
			}
		}

		public static void CoalesceOneNotEqualZero(int? a)
		{
			if ((a ?? 1) != 0)
			{
				Console.WriteLine();
			}
		}
	}
}
