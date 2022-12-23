using System;
using System.Collections.Generic;
#if LEGACY_VBC
using System.Diagnostics;
#endif

using Microsoft.VisualBasic.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.VBPretty
{
	internal struct StructWithYieldReturn
	{
		private int val;

		public IEnumerable<int> Count()
		{
			yield return val;
			yield return val;
		}
	}

	public class YieldReturnPrettyTest
	{
		private int fieldOnThis;

#if LEGACY_VBC
		[DebuggerStepThrough]
#endif
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
			while (enumerator.MoveNext())
			{
				Console.WriteLine(name + ": " + enumerator.Current.ToString());
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
			int num = 0;
			do
			{
				yield return num;
				num = checked(num + 1);
			} while (num <= 99);
		}

		public static IEnumerable<int> YieldReturnWithTryFinally()
		{
			yield return 0;
			try
			{
				yield return 1;
			}
			finally
			{
				Console.WriteLine("Finally!");
			}
			yield return 2;
		}

		public static IEnumerable<string> YieldReturnWithNestedTryFinally(bool breakInMiddle)
		{
			Console.WriteLine("Start of method - 1");
			yield return "Start of method";
			Console.WriteLine("Start of method - 2");
			try
			{
				Console.WriteLine("Within outer try - 1");
				yield return "Within outer try";
				Console.WriteLine("Within outer try - 2");
				try
				{
					Console.WriteLine("Within inner try - 1");
					yield return "Within inner try";
					Console.WriteLine("Within inner try - 2");
					if (breakInMiddle)
					{
						Console.WriteLine("Breaking...");
						yield break;
					}
					Console.WriteLine("End of inner try - 1");
					yield return "End of inner try";
					Console.WriteLine("End of inner try - 2");
				}
				finally
				{
					Console.WriteLine("Inner Finally");
				}
				Console.WriteLine("End of outer try - 1");
				yield return "End of outer try";
				Console.WriteLine("End of outer try - 2");
			}
			finally
			{
				Console.WriteLine("Outer Finally");
			}
			Console.WriteLine("End of method - 1");
			yield return "End of method";
			Console.WriteLine("End of method - 2");
		}

		public static IEnumerable<string> YieldReturnWithTwoNonNestedFinallyBlocks(IEnumerable<string> input)
		{
			// outer try-finally block
			foreach (string item in input)
			{
				// nested try-finally block
				try
				{
					yield return item;
				}
				finally
				{
					Console.WriteLine("Processed " + item);
				}
			}
			yield return "A";
			yield return "B";
			yield return "C";
			yield return "D";
			yield return "E";
			yield return "F";
			// outer try-finally block
			foreach (string item2 in input)
			{
				yield return item2.ToUpper();
			}
		}

		public static IEnumerable<int> GetEvenNumbers(int n)
		{
			int num = checked(n - 1);
			for (int i = 0; i <= num; i = checked(i + 1))
			{
				if (i % 2 == 0)
				{
					yield return i;
				}
			}
		}

		public static IEnumerable<char> ExceptionHandling()
		{
			yield return 'a';
			try
			{
				Console.WriteLine("1 - try");
			}
			catch (Exception projectError)
			{
				ProjectData.SetProjectError(projectError);
				Console.WriteLine("1 - catch");
				ProjectData.ClearProjectError();
			}
			yield return 'b';
			try
			{
				try
				{
					Console.WriteLine("2 - try");
				}
				finally
				{
					Console.WriteLine("2 - finally");
				}
				yield return 'c';
			}
			finally
			{
				Console.WriteLine("outer finally");
			}
		}

		public static IEnumerable<int> YieldBreakInCatch()
		{
			yield return 0;
			try
			{
				Console.WriteLine("In Try");
			}
			catch (Exception projectError)
			{
				ProjectData.SetProjectError(projectError);
				ProjectData.ClearProjectError();
				// yield return is not allowed in catch, but yield break is
				yield break;
			}
			yield return 1;
		}

		public static IEnumerable<int> YieldBreakInCatchInTryFinally()
		{
			try
			{
				yield return 0;
				try
				{
					Console.WriteLine("In Try");
				}
				catch (Exception projectError)
				{
					ProjectData.SetProjectError(projectError);
					ProjectData.ClearProjectError();
					// yield return is not allowed in catch, but yield break is
					// Note that pre-roslyn, this code triggers a compiler bug:
					// If the finally block throws an exception, it ends up getting
					// called a second time.
					yield break;
				}
				yield return 1;
			}
			finally
			{
				Console.WriteLine("Finally");
			}
		}

		public static IEnumerable<int> YieldBreakInTryCatchInTryFinally()
		{
			try
			{
				yield return 0;
				try
				{
					Console.WriteLine("In Try");
					// same compiler bug as in YieldBreakInCatchInTryFinally
					yield break;
				}
				catch (Exception projectError)
				{
					ProjectData.SetProjectError(projectError);
					Console.WriteLine("Catch");
					ProjectData.ClearProjectError();
				}
				yield return 1;
			}
			finally
			{
				Console.WriteLine("Finally");
			}
		}

		public static IEnumerable<int> YieldBreakInTryFinallyInTryFinally(bool b)
		{
			try
			{
				yield return 0;
				try
				{
					Console.WriteLine("In Try");
					if (b)
					{
						// same compiler bug as in YieldBreakInCatchInTryFinally
						yield break;
					}
				}
				finally
				{
					Console.WriteLine("Inner Finally");
				}
				yield return 1;
			}
			finally
			{
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
			try
			{
				yield return 0;
				throw new NotImplementedException();
			}
			finally
			{
				Console.WriteLine("Finally");
			}
		}

		public static IEnumerable<int> NestedTryFinallyStartingOnSamePosition()
		{
			// The first user IL instruction is already in 2 nested try blocks.
#if ((ROSLYN2 && !ROSLYN4) && OPT)
			int num = -1;
#endif
			try
			{
#if ((ROSLYN2 && !ROSLYN4) && OPT)
				_ = num - 1;
#endif
				try
				{
					yield return 0;
				}
				finally
				{
					Console.WriteLine("Inner Finally");
				}
			}
			finally
			{
				Console.WriteLine("Outer Finally");
			}
		}

#if ROSLYN
		public static IEnumerable<int> LocalInFinally<T>(T a) where T : IDisposable
		{
			yield return 1;
			try
			{
				yield return 2;
			}
			finally
			{
				T val = a;
				val.Dispose();
				val.Dispose();
			}
			yield return 3;
		}
#endif

		public static IEnumerable<T> GenericYield<T>() where T : new()
		{
			T val = new T();
			int num = 0;
			do
			{
				yield return val;
				num = checked(num + 1);
			} while (num <= 2);
		}

		public static IEnumerable<int> MultipleYieldBreakInTryFinally(int i)
		{
			try
			{
				if (i == 2)
				{
					yield break;
				}

				while (i < 40)
				{
					if (i % 2 == 0)
					{
						yield break;
					}
					i = checked(i + 1);

					yield return i;
				}
			}
			finally
			{
				Console.WriteLine("finally");
			}
			Console.WriteLine("normal exit");
		}

		internal IEnumerable<int> ForLoopWithYieldReturn(int end, int evil)
		{
			// This loop needs to pick the implicit "yield break;" as exit point
			// in order to produce pretty code; not the "throw" which would
			// be a less-pretty option.
			checked
			{
				int num = end - 1;
				for (int i = 0; i <= num; i++)
				{
					if (i == evil)
					{
						throw new InvalidOperationException("Found evil number");
					}
					yield return i;
				}
			}
		}
	}
}
