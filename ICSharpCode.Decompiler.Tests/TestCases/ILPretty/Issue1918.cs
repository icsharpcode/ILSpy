using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal class Issue1918
	{
		public static Guid[] NullVal;

		public unsafe void ProblemFunction(Guid[] A_0, int A_1)
		{
			fixed (Guid* ptr = A_0)
			{
				void* ptr2 = ptr;
				UIntPtr* ptr3 = (UIntPtr*)((byte*)ptr2 - sizeof(UIntPtr));
				UIntPtr uIntPtr = *ptr3;
				try
				{
					*ptr3 = (UIntPtr)(ulong)A_1;
				}
				finally
				{
					*ptr3 = uIntPtr;
				}
			}
			fixed (Guid[] ptr = NullVal)
			{
			}
		}
	}
}
