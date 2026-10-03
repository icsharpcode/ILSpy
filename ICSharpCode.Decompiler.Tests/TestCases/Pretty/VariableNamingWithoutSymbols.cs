using System;
using System.Collections.Generic;
#if EXPECTED_OUTPUT && CS70
using ICSharpCode.Decompiler.Tests.TestCases.Pretty.First;
using ICSharpCode.Decompiler.Tests.TestCases.Pretty.Second;
#endif

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

#if CS70
		private static void CapturedLocalNamingConflict(bool condition)
		{
#if EXPECTED_OUTPUT
			ICSharpCode.Decompiler.Tests.TestCases.Pretty.First.Brush brush;
			ICSharpCode.Decompiler.Tests.TestCases.Pretty.Second.Brush brush2;
			if (condition)
			{
				UseFirstBrush();
			}
			else
			{
				UseSecondBrush();
			}

			void UseFirstBrush()
			{
				brush = new ICSharpCode.Decompiler.Tests.TestCases.Pretty.First.Brush();
				Use(brush);
			}

			void UseSecondBrush()
			{
				brush2 = new ICSharpCode.Decompiler.Tests.TestCases.Pretty.Second.Brush();
				Use(brush2);
			}
#else
			if (condition)
			{
				First.Brush brush;
				UseFirstBrush();

				void UseFirstBrush()
				{
					brush = new First.Brush();
					Use(brush);
				}
			}
			else
			{
				Second.Brush brush;
				UseSecondBrush();

				void UseSecondBrush()
				{
					brush = new Second.Brush();
					Use(brush);
				}
			}
#endif
		}

		private static void Use(ICSharpCode.Decompiler.Tests.TestCases.Pretty.First.Brush brush)
		{
		}

		private static void Use(ICSharpCode.Decompiler.Tests.TestCases.Pretty.Second.Brush brush)
		{
		}
#endif
	}
}

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.First
{
	internal class Brush
	{
	}
}

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.Second
{
	internal class Brush
	{
	}
}
