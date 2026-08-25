using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class OptionalArgumentsDisabled
	{
		public int this[int x, int y = 10] {
			get {
				return x + y;
			}
			set {
			}
		}

		public void Test()
		{
			MixedArguments("123", 0, 0);
			OnlyOptionalArguments(0, 0);
		}

		public void MixedArguments(string msg, int a = 0, int b = 0)
		{
		}

		public void OnlyOptionalArguments(int a = 0, int b = 0)
		{
		}

		public void TestIndexer()
		{
			Console.WriteLine(this[1, 10]);
			this[1, 10] = 5;
		}
	}
}
