namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	internal class NoGenericTypeInstantiation<TOnType> where TOnType : new()
	{
		public static TOnType CreateTOnType()
		{
			return new TOnType();
		}

		public static TOnMethod CreateTOnMethod<TOnMethod>() where TOnMethod : new()
		{
			return new TOnMethod();
		}
	}
}
