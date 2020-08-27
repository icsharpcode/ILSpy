namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal class Issue959
	{
		public void Test(bool arg)
		{
			if (!arg && arg)
			{

			}
		}
	}
}
