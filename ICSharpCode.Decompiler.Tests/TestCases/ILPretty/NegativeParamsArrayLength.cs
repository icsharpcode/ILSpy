namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class NegativeParamsArrayLength
	{
		public static string M()
		{
			return string.Concat(new string[-1]);
		}
	}
}
