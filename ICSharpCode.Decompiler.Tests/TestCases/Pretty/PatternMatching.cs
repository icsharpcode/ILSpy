using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class PatternMatching
	{
		public bool SimpleTypePattern(object x)
		{
			Use(x is string y);
			if (x is string z)
			{
				Console.WriteLine(z);
			}
			return x is string w;
		}

		public bool SimpleTypePatternWithShortcircuit(object x)
		{
			Use(F() && x is string y && y.Contains("a"));
			if (F() && x is string z && z.Contains("a"))
			{
				Console.WriteLine(z);
			}
			return F() && x is string w && w.Contains("a");
		}

		public void SimpleTypePatternWithShortcircuitAnd(object x)
		{
			if (x is string z && z.Contains("a"))
			{
				Console.WriteLine(z);
			}
			else
			{
				Console.WriteLine();
			}
		}

		public void SimpleTypePatternWithShortcircuitOr(object x)
		{
			if (!(x is string z) || z.Contains("a"))
			{
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine(z);
			}
		}

		public void SimpleTypePatternWithShortcircuitOr2(object x)
		{
			if (F() || !(x is string z))
			{
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine(z);
			}
		}

		private bool F()
		{
			return true;
		}

		private void Use(bool x)
		{
		}
	}
}
