namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue3442
{
	public class Class : Interface
	{
		void Interface.M<T>()
		{
		}
	}
	public interface Interface
	{
		void M<T>() where T : Interface;
	}
}