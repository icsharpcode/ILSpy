using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class Issue3452
	{
		private struct Data
		{
			public object Obj;
		}

		private class C1(object obj)
		{
			internal Data d = new Data {
				Obj = obj
			};
		}

		private class C2(object obj)
		{
			public object Obj => obj;
		}

		private class C3(StringComparison comparison)
		{
			private StringComparison _comparison = comparison;

			internal StringComparison Test()
			{
				return comparison;
			}
		}

		private struct S1(object obj)
		{
			internal Data d = new Data {
				Obj = obj
			};
		}

		private struct S2(object obj)
		{
			public object Obj => obj;
		}
	}

}
