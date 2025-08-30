namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class Issue3466<T>
	{
		public static implicit operator Issue3466<T>(T t)
		{
			return null;
		}
		public static bool M(Issue3466<object> x)
		{
			return x != null;
		}
	}
}
