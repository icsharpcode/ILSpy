namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public static class GloballyQualifiedTypeInStringInterpolation
	{
		public static string CurrentDateTime => $"Time: {(global::System.DateTime.Now)}";
	}
}
