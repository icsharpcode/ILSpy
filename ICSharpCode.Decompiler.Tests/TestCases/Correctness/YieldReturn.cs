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

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	public class YieldReturnTest
	{
		static void Main()
		{
			Print("SimpleYieldReturn", SimpleYieldReturn().GetEnumerator());
			Print("SimpleYieldReturnEnumerator", SimpleYieldReturnEnumerator());
			Print("YieldReturnParameters",
				new YieldReturnTest { fieldOnThis = 1 }.YieldReturnParameters(2).GetEnumerator());
			Print("YieldReturnParametersEnumerator",
				new YieldReturnTest { fieldOnThis = 1 }.YieldReturnParametersEnumerator(2));
			Print("YieldReturnInLoop", YieldReturnInLoop().GetEnumerator());
			Print("YieldReturnWithTryFinally", YieldReturnWithTryFinally().GetEnumerator());
			Print("YieldReturnInLock1", YieldReturnInLock1(new object()).GetEnumerator());
			Print("YieldReturnInLock2", YieldReturnInLock2(new object()).GetEnumerator());
			Print("YieldReturnWithNestedTryFinally(false)", YieldReturnWithNestedTryFinally(false).GetEnumerator());
			Print("YieldReturnWithNestedTryFinally(true)", YieldReturnWithNestedTryFinally(true).GetEnumerator());
			Print("YieldReturnWithTwoNonNestedFinallyBlocks", YieldReturnWithTwoNonNestedFinallyBlocks(SimpleYieldReturn()).GetEnumerator());
			// TODO: check anon methods
			Print("GetEvenNumbers", GetEvenNumbers(3).GetEnumerator());
			Print("YieldChars", YieldChars.GetEnumerator());
			Print("ExceptionHandling", ExceptionHandling().GetEnumerator());
			Print("YieldBreakInCatch", YieldBreakInCatch().GetEnumerator());
			Print("YieldBreakInCatchInTryFinally", YieldBreakInCatchInTryFinally().GetEnumerator());
			Print("YieldBreakInTryCatchInTryFinally", YieldBreakInTryCatchInTryFinally().GetEnumerator());
			Print("YieldBreakInTryFinallyInTryFinally(false)", YieldBreakInTryFinallyInTryFinally(false).GetEnumerator());
			Print("YieldBreakInTryFinallyInTryFinally(true)", YieldBreakInTryFinallyInTryFinally(true).GetEnumerator());
			try {
				Print("UnconditionalThrowInTryFinally()", UnconditionalThrowInTryFinally().GetEnumerator());
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
			Print("NestedTryFinallyStartingOnSamePosition", NestedTryFinallyStartingOnSamePosition().GetEnumerator());
			Print("TryFinallyWithTwoExitPoints(false)", TryFinallyWithTwoExitPoints(false).GetEnumerator());
			Print("TryFinallyWithTwoExitPoints(true)", TryFinallyWithTwoExitPoints(true).GetEnumerator());
#if !LEGACY_CSC
			Print("YieldBreakInNestedTryFinally()", YieldBreakInNestedTryFinally().GetEnumerator());
			Print("TryFinallyWithTwoExitPointsInNestedTry(false)", TryFinallyWithTwoExitPointsInNestedTry(false).GetEnumerator());
			Print("TryFinallyWithTwoExitPointsInNestedTry(true)", TryFinallyWithTwoExitPointsInNestedTry(true).GetEnumerator());
			Print("TryFinallyWithTwoExitPointsInNestedCatch(false)", TryFinallyWithTwoExitPointsInNestedCatch(false).GetEnumerator());
			Print("TryFinallyWithTwoExitPointsInNestedCatch(true)", TryFinallyWithTwoExitPointsInNestedCatch(true).GetEnumerator());
#endif
			Print("GenericYield<int>()", GenericYield<int>().GetEnumerator());
			StructWithYieldReturn.Run();
		}

		internal static void Print<T>(string name, IEnumerator<T> enumerator)
		{
			Console.WriteLine(name + ": Test start");
			while (enumerator.MoveNext()) {
				Console.WriteLine(name + ": " + enumerator.Current);
			}
		}

		int fieldOnThis;

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
			foreach (string line in input)
				yield return line.ToUpper();
		}

		public static IEnumerable<Func<string>> YieldReturnWithAnonymousMethods1(IEnumerable<string> input)
		{
			foreach (string line in input) {
				yield return () => line;
			}
		}

		public static IEnumerable<Func<string>> YieldReturnWithAnonymousMethods2(IEnumerable<string> input)
		{
			foreach (string line in input) {
				string copy = line;
				yield return () => copy;
			}
		}

		public static IEnumerable<int> GetEvenNumbers(int n)
		{
			for (int i = 0; i < n; i++) {
				if (i % 2 == 0)
					yield return i;
			}
		}

		public static IEnumerable<char> YieldChars
		{
			get {
				yield return 'a';
				yield return 'b';
				yield return 'c';
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
					yield break; // same compiler bug as in YieldBreakInCatchInTryFinally
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
					if (b)
						yield break; // same compiler bug as in YieldBreakInCatchInTryFinally
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


		public static IEnumerable<int> TryFinallyWithTwoExitPoints(bool b)
		{
			// Uses goto for multiple non-exceptional exits out of try-finally.
			try {
				if (b) {
					yield return 1;
					goto exit1;
				} else {
					yield return 2;
					goto exit2;
				}
			} finally {
				Console.WriteLine("Finally");
			}
			exit1:
			Console.WriteLine("Exit1");
			yield break;
			exit2:
			Console.WriteLine("Exit2");
		}

#if !LEGACY_CSC
		public static IEnumerable<int> YieldBreakInNestedTryFinally()
		{
			try {
				yield return 1;
				try {
					// Compiler bug: pre-Roslyn, the finally blocks will execute in the wrong order
					yield break;
				} finally {
					Console.WriteLine("Inner Finally");
				}
			} finally {
				Console.WriteLine("Outer Finally");
			}
		}

		// Legacy csc has a compiler bug with this type of code:
		// If the goto statements triggers a finally block, and the finally block throws an exception,
		// that exception gets caught by the catch block.
		public static IEnumerable<int> TryFinallyWithTwoExitPointsInNestedTry(bool b)
		{
			try {
				yield return 1;
				try {
					if (b)
						goto exit1;
					else
						goto exit2;
				} catch {
					Console.WriteLine("Catch");
				}
			} finally {
				Console.WriteLine("Finally");
			}
			exit1:
			Console.WriteLine("Exit1");
			yield break;
			exit2:
			Console.WriteLine("Exit2");
		}

		public static IEnumerable<int> TryFinallyWithTwoExitPointsInNestedCatch(bool b)
		{
			// The first user IL instruction is already in 2 nested try blocks.
			try {
				yield return 1;
				try {
					Console.WriteLine("Nested Try");
				} catch {
					if (b)
						goto exit1;
					else
						goto exit2;
				}
			} finally {
				Console.WriteLine("Finally");
			}
			exit1:
			Console.WriteLine("Exit1");
			yield break;
			exit2:
			Console.WriteLine("Exit2");
		}
#endif

		public static IEnumerable<int> LocalInFinally<T>(T a) where T : IDisposable
		{
			yield return 1;
			try {
				yield return 2;
			} finally {
				T b = a;
				b.Dispose();
				b.Dispose();
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
	}

	struct StructWithYieldReturn
	{
		public static void Run()
		{
			var s = new StructWithYieldReturn { val = 2 };
			var count = s.Count();
			YieldReturnTest.Print("StructWithYieldReturn", count.GetEnumerator());
			YieldReturnTest.Print("StructWithYieldReturn (again)", count.GetEnumerator());
		}

		int val;

		public IEnumerable<int> Count()
		{
			yield return val++;
			yield return val++;
		}
	}
}