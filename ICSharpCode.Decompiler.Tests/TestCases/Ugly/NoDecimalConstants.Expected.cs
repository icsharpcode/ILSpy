using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	public class NoDecimalConstants
	{
		[DecimalConstant(1, 0, 0u, 0u, 10u)]
		private static readonly decimal constant = 1.0m;
		private void MethodWithOptionalParameter([Optional][DecimalConstant(1, 0, 0u, 0u, 10u)] decimal parameter)
		{
		}
	}
}
