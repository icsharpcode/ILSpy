namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class Issue3541
	{
		private void Test(string format)
		{
			TestLocal();

			void TestLocal(int a = 0)
			{
				a.ToString(format);
			}
		}
	}
}
