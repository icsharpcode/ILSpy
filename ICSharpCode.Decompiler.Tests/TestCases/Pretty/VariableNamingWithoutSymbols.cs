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
#if ROSLYN
			_ = c.Name;
#else
			string name = c.Name;
#endif
		}

		private void Test2(string text, C c)
		{
#if ROSLYN
			_ = c.Text;
#else
			string text2 = c.Text;
#endif
		}
	}
}
