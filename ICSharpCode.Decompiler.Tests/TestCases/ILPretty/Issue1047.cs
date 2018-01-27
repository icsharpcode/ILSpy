namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class Issue1047
	{
		private void ProblemMethod()
		{
			IL_0000:
			do {
				if (/*Error near IL_0001: Stack underflow*/ >= /*Error near IL_0001: Stack underflow*/) {
					if ((int)/*Error near IL_0007: Stack underflow*/ == 0) {
						return;
					}
					return;
				}
			} while ((int)/*Error near IL_0014: Stack underflow*/ == 0);
			if (/*Error near IL_0020: Stack underflow*/ > /*Error near IL_0020: Stack underflow*/&& (int)/*Error near IL_0026: Stack underflow*/ != 0 && (int)/*Error near IL_002c: Stack underflow*/ == 0) {
				return;
			}
			return;
			IL_0037:
			goto IL_0000;
		}
	}
}
