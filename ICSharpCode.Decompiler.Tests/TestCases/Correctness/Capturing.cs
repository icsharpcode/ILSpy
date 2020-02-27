using System;
using System.Collections.Generic;
using System.Linq;

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
			OutsideLoop();
			InsideLoop();
			OutsideLoopOverArray();
			OutsideLoopOverArray2();
			InsideLoopOverArray2();
			NotWhileDueToVariableInsideLoop();
			NotDoWhileDueToVariableInsideLoop();
			Issue1936();
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

		static void OutsideLoop()
		{
			Console.WriteLine("OutsideLoop");
			var list = new List<int> { 1, 2, 3 };
			var functions = new List<Func<int>>();
			using (var e = list.GetEnumerator()) {
				int val; // declared outside loop
						 // The decompiler cannot convert this to a foreach-loop without
						 // changing the lambda capture semantics.
				while (e.MoveNext()) {
					val = e.Current;
					functions.Add(() => val);
				}
			}
			foreach (var func in functions) {
				Console.WriteLine(func());
			}
		}

		static void InsideLoop()
		{
			Console.WriteLine("InsideLoop");
			var list = new List<int> { 1, 2, 3 };
			var functions = new List<Func<int>>();
			using (var e = list.GetEnumerator()) {
				while (e.MoveNext()) {
					int val = e.Current;
					functions.Add(() => val);
				}
			}
			foreach (var func in functions) {
				Console.WriteLine(func());
			}
		}

		static void OutsideLoopOverArray()
		{
			Console.WriteLine("OutsideLoopOverArray:");
			var functions = new List<Func<int>>();
			var array = new int[] { 1, 2, 3 };
			int val; // declared outside loop
					 // The decompiler cannot convert this to a foreach-loop without
					 // changing the lambda capture semantics.
			for (int i = 0; i < array.Length; ++i) {
				val = array[i];
				functions.Add(() => val);
			}
			foreach (var func in functions) {
				Console.WriteLine(func());
			}
		}

		static void OutsideLoopOverArray2()
		{
			Console.WriteLine("OutsideLoopOverArray2:");
			var functions = new List<Func<int>>();
			var array = new int[] { 1, 2, 3 };
			int val; // declared outside loop
					 // The decompiler can convert this to a foreach-loop, but the 'val'
					 // variable must be declared outside.
			for (int i = 0; i < array.Length; ++i) {
				int element = array[i];
				val = element * 2;
				functions.Add(() => val);
			}
			foreach (var func in functions) {
				Console.WriteLine(func());
			}
		}

		static void InsideLoopOverArray2()
		{
			Console.WriteLine("InsideLoopOverArray2:");
			var functions = new List<Func<int>>();
			var array = new int[] { 1, 2, 3 };
			for (int i = 0; i < array.Length; ++i) {
				int element = array[i];
				int val = element * 2;
				functions.Add(() => val);
			}
			foreach (var func in functions) {
				Console.WriteLine(func());
			}
		}

		static int nextVal;

		static int GetVal()
		{
			return ++nextVal & 7;
		}

		static void NotWhileDueToVariableInsideLoop()
		{
			Console.WriteLine("NotWhileDueToVariableInsideLoop:");
			var functions = new List<Func<int>>();
			while (true) {
				int v;
				if ((v = GetVal()) == 0)
					break;
				functions.Add(() => v);
			}
			foreach (var f in functions) {
				Console.WriteLine(f());
			}
		}

		static void NotDoWhileDueToVariableInsideLoop()
		{
			Console.WriteLine("NotDoWhileDueToVariableInsideLoop:");
			var functions = new List<Func<int>>();
			while (true) {
				int v = GetVal();
				functions.Add(() => v);
				if (v == 0)
					break;
			}
			foreach (var f in functions) {
				Console.WriteLine(f());
			}
		}

		public static void Issue1936()
		{
			IEnumerable<object> outerCapture = null;
			for (int i = 0; i < 10; i++) {
				int innerCapture = 0;
				Action a = (delegate {
					List<object> list = new List<object>();
					Console.WriteLine("before inc: " + innerCapture);
					++innerCapture;
					Console.WriteLine("after inc: " + innerCapture);
					Console.WriteLine("before assign: " + outerCapture);
					outerCapture = outerCapture == null ? list : outerCapture.Concat(list);
					Console.WriteLine("after assign: " + outerCapture);
				});
				a.Invoke();
			}
		}
	}
}
