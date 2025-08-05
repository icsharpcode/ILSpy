namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class OptionalArgumentsDisabled
	{
		public void Test()
		{
			MixedArguments("123", 0, 0);
			OnlyOptionalArguments(0, 0);
		}

		public void MixedArguments(string msg, int a = 0, int b = 0)
		{
		}

		public void OnlyOptionalArguments(int a = 0, int b = 0)
		{
		}
	}
}
