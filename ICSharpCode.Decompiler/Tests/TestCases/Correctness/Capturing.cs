using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class Capturing
	{
		static void Main(string[] args)
		{
			TestCase1();
		}

		static void TestCase1()
		{
			Console.WriteLine("TestCase1");
			for (int i = 0; i < 10; i++)
				Console.WriteLine(i);
			// i no longer declared
			List<Action> actions = new List<Action>();
			int max = 5;
			string line;
			while (ReadLine(out line, ref max)) {
				actions.Add(() => Console.WriteLine(line));
			}
			// line still declared
			line = null;
			Console.WriteLine("----");
			foreach (var action in actions)
				action();
		}

		private static bool ReadLine(out string line, ref int v)
		{
			line = v + " line";
			return --v > 0;
		}
	}
}
