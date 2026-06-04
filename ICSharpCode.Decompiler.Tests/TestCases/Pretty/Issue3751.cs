using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class Issue3751
	{
		private static bool Cond;

		private static T Infer<T>(Func<T> factory)
		{
			return factory();
		}

		public object Trigger()
		{
			return Infer(delegate {
				if (Cond)
				{
					Console.WriteLine();
					return null;
				}
				return new {
					Value = 1
				};
			});
		}
	}
}
