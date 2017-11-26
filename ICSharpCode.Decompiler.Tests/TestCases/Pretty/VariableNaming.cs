namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class VariableNaming
	{
		private class C
		{
			public string Name;
			public string Text;
		}

		private void Test(string text, C c)
		{
			string name = c.Name;
		}

		private void Test2(string text, C c)
		{
			string text2 = c.Text;
		}
	}
}
