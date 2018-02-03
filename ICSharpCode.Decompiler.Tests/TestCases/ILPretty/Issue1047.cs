namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class Issue1047
	{
		private static bool dummy;

		private void ProblemMethod()
		{
			IL_0000:
			while (!Issue1047.dummy) {
			}
			return;
			IL_0014:
			goto IL_0000;
		}
	}
}