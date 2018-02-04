using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	public class NoArrayInitializers
	{
		public int[] LiteralArray()
		{
			int[] obj = new int[3];
			RuntimeHelpers.InitializeArray(obj, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			return obj;
		}

		public int[] VariableArray(int a, int b)
		{
			int[] obj = new int[2];
			obj[0] = a;
			obj[1] = b;
			return obj;
		}
	}
}
[CompilerGenerated]
internal sealed class _003CPrivateImplementationDetails_003E
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 12)]
	private struct __StaticArrayInitTypeSize_003D12
	{
	}
	internal static readonly __StaticArrayInitTypeSize_003D12 E429CCA3F703A39CC5954A6572FEC9086135B34E/* Not supported: data(01 00 00 00 02 00 00 00 03 00 00 00) */;
}