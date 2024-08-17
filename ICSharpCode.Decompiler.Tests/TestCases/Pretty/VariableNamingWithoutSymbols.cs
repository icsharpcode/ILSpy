using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class VariableNamingWithoutSymbols
	{
		private class C
		{
			public string Name;
			public string Text;
		}

		private void Test(string text, C c)
		{
#if CS70
			_ = c.Name;
#else
			string name = c.Name;
#endif
		}

		private void Test2(string text, C c)
		{
#if CS70
			_ = c.Text;
#else
			string text2 = c.Text;
#endif
		}

		private static IDisposable GetData()
		{
			return null;
		}

		private static void UseData(IDisposable data)
		{

		}

		private static IEnumerable<int> GetItems()
		{
			throw null;
		}

		private static byte[] GetMemory()
		{
			throw null;
		}

		private static void Test(int item)
		{
			foreach (int item2 in GetItems())
			{
				Console.WriteLine(item2);
			}
		}

		private static void Test(IDisposable data)
		{
#if CS80
			using IDisposable data2 = GetData();
			UseData(data2);
#else
			using (IDisposable data2 = GetData())
			{
				UseData(data2);
			}
#endif
		}

		private unsafe static void Test(byte[] memory)
		{
			fixed (byte* memory2 = GetMemory())
			{
				Console.WriteLine(*memory2);
			}
		}

		private static void ForLoopNamingConflict(int i)
		{
			for (int j = 0; j < i; j++)
			{
				Console.WriteLine(i + " of " + j);
			}
		}
	}
}
