using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public static class StackAllocDuplicateStore
	{
		public unsafe static int Seq(int a, int b, int c)
		{
			byte* num = stackalloc byte[12];
			*(int*)num = a;
			*(int*)num = 99;
			((int*)num)[1] = b;
			((int*)num)[2] = c;
			Span<int> span = new Span<int>(num, 3);
			return span[0] + span[1] + span[2];
		}
	}
}
