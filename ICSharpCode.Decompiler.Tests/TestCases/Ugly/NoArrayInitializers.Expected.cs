using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[CompilerGenerated]
internal sealed class _003CPrivateImplementationDetails_003E
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 12)]
	private struct __StaticArrayInitTypeSize_003D12
	{
	}
	internal static readonly __StaticArrayInitTypeSize_003D12 E429CCA3F703A39CC5954A6572FEC9086135B34E/* Not supported: data(01 00 00 00 02 00 00 00 03 00 00 00) */;
}

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	public class NoArrayInitializers
	{
		public int[] LiteralArray()
		{
			int[] array = new int[3];
			RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			return array;
		}

		public int[] VariableArray(int a, int b)
		{
			int[] array = new int[2];
			array[0] = a;
			array[1] = b;
			return array;
		}
	}
}
