using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	internal class NoForEachStatement
	{
		public static void SimpleNonGenericForeach(IEnumerable enumerable)
		{
			IEnumerator enumerator = enumerable.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
#if ROSLYN && OPT
					Console.WriteLine(enumerator.Current);
#else
					object current = enumerator.Current;
					Console.WriteLine(current);
#endif
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{ 
					disposable.Dispose();
				}
			}
		}

		public static void SimpleForeachOverInts(IEnumerable<int> enumerable)
		{
			using (IEnumerator<int> enumerator = enumerable.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
#if ROSLYN && OPT
					Console.WriteLine(enumerator.Current);
#else
					int current = enumerator.Current;
					Console.WriteLine(current);
#endif
				}
			}
		}
	}
}
