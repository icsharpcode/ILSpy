using System;
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class NegativeReadOnlySpanLength
	{
#if EXPECTED_OUTPUT
		internal static readonly int F/* Not supported: data() */;
#else
		internal static int F;
#endif

		public unsafe static ReadOnlySpan<byte> M()
		{
			return new ReadOnlySpan<byte>(System.Runtime.CompilerServices.Unsafe.AsPointer(ref F), -1);
		}
	}
}
