namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class Comparisons
	{
		private class A
		{
		}

		private class B
		{
		}

		private bool CompareUnrelatedNeedsCast(A a, B b)
		{
			return (object)a == b;
		}
	}
}
