namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal class BaseClass
	{
		public int importsClausePosition;
	}
	
	internal class Issue1681 : BaseClass
	{
		public void Test()
		{
			_ = importsClausePosition;
		}
	}
}
