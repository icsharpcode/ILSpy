namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class Issue1047
	{
		private static bool dummy;

		private void ProblemMethod()
		{
			while (!dummy)
			{
			}
		}
	}
}