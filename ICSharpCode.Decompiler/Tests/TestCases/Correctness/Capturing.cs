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
			TestCase2();
			TestCase3();
			TestCase4("TestCase4");
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

		static void TestCase2()
		{
			Console.WriteLine("TestCase2");
			List<Action> actions = new List<Action>();
			int max = 5;
			string line;
			while (ReadLine(out line, ref max)) {
				string capture = line;
				actions.Add(() => Console.WriteLine(capture));
			}
			// line still declared
			line = null;
			Console.WriteLine("----");
			foreach (var action in actions)
				action();
		}

		static void TestCase3()
		{
			Console.WriteLine("TestCase3");
			List<Action> actions = new List<Action>();
			int max = 5;
			string line, capture;
			while (ReadLine(out line, ref max)) {
				capture = line;
				actions.Add(() => Console.WriteLine(capture));
			}
			// line still declared
			line = null;
			Console.WriteLine("----");
			foreach (var action in actions)
				action();
		}

		static void TestCase4(string capture)
		{
			Console.WriteLine("TestCase4");
			List<Action> actions = new List<Action>();
			actions.Add(() => Console.WriteLine(capture));
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
