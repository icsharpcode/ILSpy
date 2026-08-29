namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class MissingBaseConstructor : MissingBase
	{
		public MissingBaseConstructor(MissingRole role, string content)
			: base(role, content)
		{
		}
	}
}
