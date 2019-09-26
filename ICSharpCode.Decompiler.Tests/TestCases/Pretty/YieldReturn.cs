// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal struct StructWithYieldReturn
	{
		private int val;

		public IEnumerable<int> Count()
		{
			yield return val++;
			yield return val++;
		}
	}

	public class YieldReturnPrettyTest
	{
		private int fieldOnThis;

		public static IEnumerable<char> YieldChars {
			get {
				yield return 'a';
				yield return 'b';
				yield return 'c';
			}
		}

		internal static void Print<T>(string name, IEnumerator<T> enumerator)
		{
			Console.WriteLine(name + ": Test start");
			while (enumerator.MoveNext()) {
				Console.WriteLine(name + ": " + enumerator.Current);
			}
		}

		public static IEnumerable<string> SimpleYieldReturn()
		{
			yield return "A";
			yield return "B";
			yield return "C";
		}

		public static IEnumerator<string> SimpleYieldReturnEnumerator()
		{
			yield return "A";
			yield return "B";
			yield return "C";
		}

		public IEnumerable<int> YieldReturnParameters(int p)
		{
			yield return p;
			yield return fieldOnThis;
		}

		public IEnumerator<int> YieldReturnParametersEnumerator(int p)
		{
			yield return p;
			yield return fieldOnThis;
		}

		public static IEnumerable<int> YieldReturnInLoop()
		{
			for (int i = 0; i < 100; i++) {
				yield return i;
			}
		}

		public static IEnumerable<int> YieldReturnWithTryFinally()
		{
			yield return 0;
			try {
				yield return 1;
			} finally {
				Console.WriteLine("Finally!");
			}
			yield return 2;
		}

#if TODO
		// TODO: adjust lock-pattern for this case
		public static IEnumerable<int> YieldReturnInLock1(object o)
		{
			lock (o) {
				yield return 1;
			}
		}

		public static IEnumerable<int> YieldReturnInLock2(object o)
		{
			lock (o) {
				yield return 1;
				o = null;
				yield return 2;
			}
		}
#endif

		public static IEnumerable<string> YieldReturnWithNestedTryFinally(bool breakInMiddle)
		{
			Console.WriteLine("Start of method - 1");
			yield return "Start of method";
			Console.WriteLine("Start of method - 2");
			try {
				Console.WriteLine("Within outer try - 1");
				yield return "Within outer try";
				Console.WriteLine("Within outer try - 2");
				try {
					Console.WriteLine("Within inner try - 1");
					yield return "Within inner try";
					Console.WriteLine("Within inner try - 2");
					if (breakInMiddle) {
						Console.WriteLine("Breaking...");
						yield break;
					}
					Console.WriteLine("End of inner try - 1");
					yield return "End of inner try";
					Console.WriteLine("End of inner try - 2");
				} finally {
					Console.WriteLine("Inner Finally");
				}
				Console.WriteLine("End of outer try - 1");
				yield return "End of outer try";
				Console.WriteLine("End of outer try - 2");
			} finally {
				Console.WriteLine("Outer Finally");
			}
			Console.WriteLine("End of method - 1");
			yield return "End of method";
			Console.WriteLine("End of method - 2");
		}

		public static IEnumerable<string> YieldReturnWithTwoNonNestedFinallyBlocks(IEnumerable<string> input)
		{
			// outer try-finally block
			foreach (string line in input) {
				// nested try-finally block
				try {
					yield return line;
				} finally {
					Console.WriteLine("Processed " + line);
				}
			}
			yield return "A";
			yield return "B";
			yield return "C";
			yield return "D";
			yield return "E";
			yield return "F";
			// outer try-finally block
			foreach (string item in input) {
				yield return item.ToUpper();
			}
		}

		public static IEnumerable<Func<string>> YieldReturnWithAnonymousMethods1(IEnumerable<string> input)
		{
			foreach (string line in input) {
				yield return () => line;
			}
		}

		public static IEnumerable<Func<string>> YieldReturnWithAnonymousMethods2(IEnumerable<string> input)
		{
			foreach (string item in input) {
				string copy = item;
				yield return () => copy;
			}
		}

		public static IEnumerable<int> GetEvenNumbers(int n)
		{
			for (int i = 0; i < n; i++) {
				if (i % 2 == 0) {
					yield return i;
				}
			}
		}

		public static IEnumerable<char> ExceptionHandling()
		{
			yield return 'a';
			try {
				Console.WriteLine("1 - try");
			} catch (Exception) {
				Console.WriteLine("1 - catch");
			}
			yield return 'b';
			try {
				try {
					Console.WriteLine("2 - try");
				} finally {
					Console.WriteLine("2 - finally");
				}
				yield return 'c';
			} finally {
				Console.WriteLine("outer finally");
			}
		}

		public static IEnumerable<int> YieldBreakInCatch()
		{
			yield return 0;
			try {
				Console.WriteLine("In Try");
			} catch {
				// yield return is not allowed in catch, but yield break is
				yield break;
			}
			yield return 1;
		}
		
		public static IEnumerable<int> YieldBreakInCatchInTryFinally()
		{
			try {
				yield return 0;
				try {
					Console.WriteLine("In Try");
				} catch {
					// yield return is not allowed in catch, but yield break is
					// Note that pre-roslyn, this code triggers a compiler bug:
					// If the finally block throws an exception, it ends up getting
					// called a second time.
					yield break;
				}
				yield return 1;
			} finally {
				Console.WriteLine("Finally");
			}
		}

		public static IEnumerable<int> YieldBreakInTryCatchInTryFinally()
		{
			try {
				yield return 0;
				try {
					Console.WriteLine("In Try");
					// same compiler bug as in YieldBreakInCatchInTryFinally
					yield break;
				} catch {
					Console.WriteLine("Catch");
				}
				yield return 1;
			} finally {
				Console.WriteLine("Finally");
			}
		}

		public static IEnumerable<int> YieldBreakInTryFinallyInTryFinally(bool b)
		{
			try {
				yield return 0;
				try {
					Console.WriteLine("In Try");
					if (b) {
						// same compiler bug as in YieldBreakInCatchInTryFinally
						yield break;
					}
				} finally {
					Console.WriteLine("Inner Finally");
				}
				yield return 1;
			} finally {
				Console.WriteLine("Finally");
			}
		}

		public static IEnumerable<int> YieldBreakOnly()
		{
			yield break;
		}
		
		public static IEnumerable<int> UnconditionalThrowInTryFinally()
		{
			// Here, MoveNext() doesn't call the finally methods at all
			// (only indirectly via Dispose())
			try {
				yield return 0;
				throw new NotImplementedException();
			} finally {
				Console.WriteLine("Finally");
			}
		}

		public static IEnumerable<int> NestedTryFinallyStartingOnSamePosition()
		{
			// The first user IL instruction is already in 2 nested try blocks.
			try {
				try {
					yield return 0;
				} finally {
					Console.WriteLine("Inner Finally");
				}
			} finally {
				Console.WriteLine("Outer Finally");
			}
		}
		
		public static IEnumerable<int> LocalInFinally<T>(T a) where T : IDisposable
		{
			yield return 1;
			try {
				yield return 2;
			} finally {
				T val = a;
				val.Dispose();
				val.Dispose();
			}
			yield return 3;
		}

		public static IEnumerable<T> GenericYield<T>() where T : new()
		{
			T val = new T();
			for (int i = 0; i < 3; i++) {
				yield return val;
			}
		}

		public static IEnumerable<int> MultipleYieldBreakInTryFinally(int i)
		{
			try {
				if (i == 2) {
					yield break;
				}

				while (i < 40) {
					if (i % 2 == 0) {
						yield break;
					}
					i++;

					yield return i;
				}
			} finally {
				Console.WriteLine("finally");
			}
			Console.WriteLine("normal exit");
		}
	}
}