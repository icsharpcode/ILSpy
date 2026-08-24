namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class LateBaseConstructorCall
	{
		private static void Initialize()
		{
		}

		public LateBaseConstructorCall()
		{
			Initialize();
		}
	}
}
