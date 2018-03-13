namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class VariableNamingWithoutSymbols
	{
		private class C
		{
			public string Name;
			public string Text;
		}

		private void Test(string text, C c)
		{
			var name = c.Name;
		}

		private void Test2(string text, C c)
		{
			var text2 = c.Text;
		}
	}
}
