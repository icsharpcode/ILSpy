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
#if CS60
using System.IO;
#endif
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public abstract class ExceptionHandling
	{
		public abstract bool B(int i);
		public abstract Task<bool> T();
		public abstract void M(int i);

		public bool ConditionalReturnInThrow()
		{
			try
			{
				if (B(0))
				{
					return B(1);
				}
			}
			catch
			{
			}
			return false;
		}

		public bool SimpleTryCatchException()
		{
			try
			{
				Console.WriteLine("Try");
				return B(new Random().Next());
			}
			catch (Exception)
			{
				Console.WriteLine("CatchException");
			}
			return false;
		}

		public bool SimpleTryCatchExceptionWithName()
		{
			try
			{
				Console.WriteLine("Try");
				return B(new Random().Next());
			}
			catch (Exception ex)
			{
				Console.WriteLine("CatchException ex: " + ex.ToString());
			}
			return false;
		}

#if CS60
		public bool SimpleTryCatchExceptionWithNameAndCondition()
		{
			try
			{
				Console.WriteLine("Try");
				return B(new Random().Next());
			}
			catch (Exception ex) when (ex.Message.Contains("test"))
			{
				Console.WriteLine("CatchException ex: " + ex.ToString());
			}
			return false;
		}

		public bool SimpleTryCatchExceptionWithNameAndConditionWithOr()
		{
			try
			{
				Console.WriteLine("Try");
				return B(new Random().Next());
			}
			catch (Exception ex) when (ex is ArgumentException || ex is IOException)
			{
				Console.WriteLine("CatchException ex: " + ex.ToString());
			}
			return false;
		}

		public async Task<bool> SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr()
		{
			try
			{
				Console.WriteLine("Try");
				return await T();
			}
			catch (Exception ex) when (ex is ArgumentException || ex is IOException)
			{
				Console.WriteLine("CatchException ex: " + ex.ToString());
			}
			return false;
		}

		public void CatchWhenWithConditionWithoutExceptionVar()
		{
			int num = 0;
			try
			{
				throw new Exception();
			}
			catch (Exception) when (num == 0)
			{
				Console.WriteLine("jo");
			}
		}

#endif

		public bool SimpleTryFinally()
		{
			try
			{
				Console.WriteLine("Try");
			}
			finally
			{
				Console.WriteLine("Finally");
			}
			return false;
		}

		public void MethodEndingWithEndFinally()
		{
			try
			{
				throw null;
			}
			finally
			{
				Console.WriteLine();
			}
		}

		public void MethodEndingWithRethrow()
		{
			try
			{
				throw null;
			}
			catch
			{
				throw;
			}
		}

		public void TryCatchFinally()
		{
			try
			{
				Console.WriteLine("Try");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				Console.WriteLine("Finally");
			}
		}

		public void TryCatchMultipleHandlers()
		{
			try
			{
				Console.WriteLine("Try");
			}
			catch (InvalidOperationException ex)
			{
				Console.WriteLine(ex.Message);
			}
			catch (SystemException ex2)
			{
				Console.WriteLine(ex2.Message);
			}
			catch
			{
				Console.WriteLine("other");
			}
		}

		//public void TwoCatchBlocksWithSameVariable()
		//{
		//	try {
		//		Console.WriteLine("Try1");
		//	} catch (Exception ex) {
		//		Console.WriteLine(ex.Message);
		//	}
		//	try {
		//		Console.WriteLine("Try2");
		//	} catch (Exception ex) {
		//		Console.WriteLine(ex.Message);
		//	}
		//}

		public void NoUsingStatementBecauseTheVariableIsAssignedTo()
		{
			CancellationTokenSource cancellationTokenSource = null;
			try
			{
				cancellationTokenSource = new CancellationTokenSource();
			}
			finally
			{
				if (cancellationTokenSource != null)
				{
					cancellationTokenSource.Dispose();
				}
			}
		}

		public void ThrowInFinally()
		{
			try
			{
			}
			finally
			{
				throw new Exception();
			}
		}

		internal void EarlyReturnInTryBlock(bool a, bool b)
		{
			try
			{
				if (a)
				{
					Console.WriteLine("a");
				}
				else if (b)
				{
					// #2379: The only goto-free way of representing this code is to use a return statement
					return;
				}

				Console.WriteLine("a || !b");
			}
			finally
			{
				Console.WriteLine("finally");
			}
		}

#if ROSLYN || !OPT
		// TODO Non-Roslyn compilers create a second while loop inside the try, by inverting the if
		// This is fixed in the non-optimised version by the enabling the RemoveDeadCode flag
		//public bool EarlyExitInLoopTry()
		//{
		//	while (true) {
		//		try {
		//			while (B(0)) {
		//				Console.WriteLine();
		//			}
		//
		//			return false;
		//		} catch {
		//		}
		//	}
		//}
		public bool EarlyExitInLoopTry()
		{
			while (true)
			{
				try
				{
					if (!B(0))
					{
						return false;
					}

					Console.WriteLine();
				}
				catch
				{
				}
			}
		}
#endif

		public bool ComplexConditionalReturnInThrow()
		{
			try
			{
				if (B(0))
				{
					if (B(1))
					{
						Console.WriteLine("0 && 1");
						return B(2);
					}

					if (B(3))
					{
						Console.WriteLine("0 && 3");
						return !B(2);
					}

					Console.WriteLine("0");
				}

				Console.WriteLine("End Try");

			}
			catch
			{
				try
				{
					try
					{
						if (((B(0) || B(1)) && B(2)) || B(3))
						{
							return B(4) && !B(5);
						}
						if (B(6) || B(7))
						{
							return B(8) || B(9);
						}
					}
					catch
					{
						Console.WriteLine("Catch2");
					}
					return B(10) && B(11);
				}
				catch
				{
					Console.WriteLine("Catch");
				}
				finally
				{
					Console.WriteLine("Finally");
				}
			}
			return false;
		}

		public void AppropriateLockExit()
		{
			int num = 0;
			lock (this)
			{
				if (num <= 256)
				{
					Console.WriteLine(0);
				}
				else if (num <= 1024)
				{
					Console.WriteLine(1);
				}
				else if (num <= 16384)
				{
					Console.WriteLine(2);
				}
			}
		}

		public void ReassignExceptionVar()
		{
			try
			{
				Console.WriteLine("ReassignExceptionVar");
			}
			catch (Exception innerException)
			{
				if (innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
				}
				Console.WriteLine(innerException);
			}
		}

		public int UseExceptionVarOutsideCatch()
		{
			Exception ex2;
			try
			{
				return 1;
			}
			catch (Exception ex)
			{
				ex2 = ex;
			}
			Console.WriteLine(ex2 != null);
			return 2;
		}

		public void GenericException<TException>(int input) where TException : Exception
		{
			try
			{
				Console.WriteLine(input);
			}
			catch (TException val)
			{
				Console.WriteLine(val.Message);
				throw;
			}
		}

		public void GenericException2<T>() where T : Exception
		{
			try
			{
				Console.WriteLine("CatchT");
#if ROSLYN
			}
			catch (T val)
			{
				Console.WriteLine("{0} {1}", val, val.ToString());
			}
#else
			}
			catch (T arg)
			{
				Console.WriteLine("{0} {1}", arg, arg.ToString());
			}
#endif
		}

#if CS60
		public void GenericExceptionWithCondition<TException>(int input) where TException : Exception
		{
			try
			{
				Console.WriteLine(input);
			}
			catch (TException val) when (val.Message.Contains("Test"))
			{
				Console.WriteLine(val.Message);
				throw;
			}
		}

		public void GenericException2WithCondition<TException>(int input) where TException : Exception
		{
			try
			{
				Console.WriteLine(input);
			}
			catch (TException val) when (val.Message.Contains("Test"))
			{
				Console.WriteLine("{0} {1}", val, val.ToString());
			}
		}

		public void XXX1()
		{
			try
			{
				Console.WriteLine();
			}
			catch (Exception ex) when (ex.Data.IsFixedSize)
			{
				Console.WriteLine(ex.ToString());
#pragma warning disable CA2200 // Rethrow to preserve stack details
				throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
			}
		}

		public void XXX2()
		{
			try
			{
				Console.WriteLine();
			}
			catch (Exception ex) when (ex is InternalBufferOverflowException)
			{
				Console.WriteLine(ex.ToString());
#pragma warning disable CA2200 // Rethrow to preserve stack details
				throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
			}
		}
#endif
	}
}
