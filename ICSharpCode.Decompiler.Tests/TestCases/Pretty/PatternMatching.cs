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

		public void SimpleTypePatternValueTypesCondition(object x)
		{
			if (x is int i)
			{
				Console.WriteLine("Integer: " + i);
			}
			else
			{
				Console.WriteLine("else");
			}
		}

		public void SimpleTypePatternValueTypesCondition2()
		{
			if (GetObject() is int i)
			{
				Console.WriteLine("Integer: " + i);
			}
			else
			{
				Console.WriteLine("else");
			}
		}

		public void SimpleTypePatternValueTypesWithShortcircuitAnd(object x)
		{
			if (x is int i && i.GetHashCode() > 0)
			{
				Console.WriteLine("Positive integer: " + i);
			}
			else
			{
				Console.WriteLine("else");
			}
		}

		public void SimpleTypePatternValueTypesWithShortcircuitOr(object x)
		{
			if (!(x is int z) || z.GetHashCode() > 0)
			{
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine(z);
			}
		}

		public void SimpleTypePatternValueTypesWithShortcircuitOr2(object x)
		{
			if (F() || !(x is int z))
			{
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine(z);
			}
		}

#if CS71
		public void SimpleTypePatternGenerics<T>(object x)
		{
			if (x is T t)
			{
				Console.WriteLine(typeof(T).FullName + ": " + t);
			}
			else
			{
				Console.WriteLine("not a " + typeof(T).FullName);
			}
		}

		public void SimpleTypePatternGenericRefType<T>(object x) where T : class
		{
			if (x is T t)
			{
				Console.WriteLine(typeof(T).FullName + ": " + t);
			}
			else
			{
				Console.WriteLine("not a " + typeof(T).FullName);
			}
		}

		public void SimpleTypePatternGenericValType<T>(object x) where T : struct
		{
			if (x is T t)
			{
				Console.WriteLine(typeof(T).FullName + ": " + t);
			}
			else
			{
				Console.WriteLine("not a " + typeof(T).FullName);
			}
		}
#endif

		public void SimpleTypePatternValueTypesWithShortcircuitAndMultiUse(object x)
		{
			if (x is int i && i.GetHashCode() > 0 && i % 2 == 0)
			{
				Console.WriteLine("Positive integer: " + i);
			}
			else
			{
				Console.WriteLine("else");
			}
		}

		public void SimpleTypePatternValueTypesWithShortcircuitAndMultiUse2(object x)
		{
			if ((x is int i && i.GetHashCode() > 0 && i % 2 == 0) || F())
			{
				Console.WriteLine("true");
			}
			else
			{
				Console.WriteLine("else");
			}
		}

		public void SimpleTypePatternValueTypesWithShortcircuitAndMultiUse3(object x)
		{
			if (F() || (x is int i && i.GetHashCode() > 0 && i % 2 == 0))
			{
				Console.WriteLine("true");
			}
			else
			{
				Console.WriteLine("else");
			}
		}

		public void SimpleTypePatternValueTypes()
		{
			Use(F() && GetObject() is int y && y.GetHashCode() > 0 && y % 2 == 0);
		}

		private bool F()
		{
			return true;
		}

		private object GetObject()
		{
			throw new NotImplementedException();
		}

		private void Use(bool x)
		{
		}
	}
}
