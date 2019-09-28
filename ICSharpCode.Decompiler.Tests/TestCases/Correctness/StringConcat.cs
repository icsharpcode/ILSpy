using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class StringConcat
	{
		private class C
		{
			readonly int i;

			public C(int i)
			{
				Console.WriteLine("  new C(" + i + ")");
				this.i = i;
			}

			public override string ToString()
			{
				Console.WriteLine("  C(" + i + ").ToString()");
				return i.ToString();
			}
		}

		private struct S
		{
			readonly int i;

			public S(int i)
			{
				Console.WriteLine("  new C(" + i + ")");
				this.i = i;
			}

			public override string ToString()
			{
				Console.WriteLine("  S(" + i + ").ToString()");
				return i.ToString();
			}
		}

		static string Space()
		{
			Console.WriteLine("  Space()");
			return " ";
		}

		static void TestClass()
		{
			Console.WriteLine("string + C:");
			Console.WriteLine(Space() + new C(1));

			Console.WriteLine("C + string:");
			Console.WriteLine(new C(2) + Space());

			Console.WriteLine("C + string + C:");
			Console.WriteLine(new C(3) + Space() + new C(4));

			Console.WriteLine("string + C + C:");
			Console.WriteLine(Space() + new C(5) + new C(6));

			Console.WriteLine("string.Concat(C, string, C):");
			Console.WriteLine(string.Concat(new C(10), Space(), new C(11)));

			Console.WriteLine("string.Concat(string.Concat(C, string), C):");
			Console.WriteLine(string.Concat(string.Concat(new C(15), Space()), new C(16)));

			Console.WriteLine("string.Concat(C, string.Concat(string, C)):");
			Console.WriteLine(string.Concat(new C(20), string.Concat(Space(), new C(21))));

			Console.WriteLine("string.Concat(C, string) + C:");
			Console.WriteLine(string.Concat(new C(30), Space()) + new C(31));
		}

		static void TestStruct()
		{
			Console.WriteLine("string + S:");
			Console.WriteLine(Space() + new S(1));

			Console.WriteLine("S + string:");
			Console.WriteLine(new S(2) + Space());

			Console.WriteLine("S + string + S:");
			Console.WriteLine(new S(3) + Space() + new S(4));

			Console.WriteLine("string + S + S:");
			Console.WriteLine(Space() + new S(5) + new S(6));

			Console.WriteLine("string.Concat(S, string, S):");
			Console.WriteLine(string.Concat(new S(10), Space(), new S(11)));

			Console.WriteLine("string.Concat(string.Concat(S, string), S):");
			Console.WriteLine(string.Concat(string.Concat(new S(15), Space()), new S(16)));

			Console.WriteLine("string.Concat(S, string.Concat(string, S)):");
			Console.WriteLine(string.Concat(new S(20), string.Concat(Space(), new S(21))));

			Console.WriteLine("string.Concat(S, string) + S:");
			Console.WriteLine(string.Concat(new S(30), Space()) + new S(31));
		}

		static void Main()
		{
			TestClass();
			TestStruct();
		}
	}
}
