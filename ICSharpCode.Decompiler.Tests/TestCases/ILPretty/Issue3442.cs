namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3442
{
	public class Class : Interface
	{
		private void M<T>() where T : Interface
		{
		}

		void Interface.M<T>()
		{
			//ILSpy generated this explicit interface implementation from .override directive in M
			this.M<T>();
		}
	}
	public interface Interface
	{
		void M<T>() where T : Interface;
	}
}
