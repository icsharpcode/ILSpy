namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class RefLocalsAndReturns
	{
		public ref struct RefStruct
		{
			private int dummy;
		}

		public readonly ref struct ReadOnlyRefStruct
		{
			private readonly int dummy;
		}

		public readonly struct ReadOnlyStruct
		{
			private readonly int dummy;
		}
	}
}
