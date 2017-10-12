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
#if !LEGACY_CSC
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
			try {
				if (this.B(0)) {
					return this.B(1);
				}
			} catch {
			}
			return false;
		}

		public bool SimpleTryCatchException()
		{
			try {
				Console.WriteLine("Try");
				return this.B(new Random().Next());
			} catch (Exception) {
				Console.WriteLine("CatchException");
			}
			return false;
		}

		public bool SimpleTryCatchExceptionWithName()
		{
			try {
				Console.WriteLine("Try");
				return this.B(new Random().Next());
			} catch (Exception ex) {
				Console.WriteLine("CatchException ex: " + ex.ToString());
			}
			return false;
		}

#if !LEGACY_CSC
		public bool SimpleTryCatchExceptionWithNameAndCondition()
		{
			try {
				Console.WriteLine("Try");
				return this.B(new Random().Next());
			} catch (Exception ex) when (ex.Message.Contains("test")) {
				Console.WriteLine("CatchException ex: " + ex.ToString());
			}
			return false;
		}

		public bool SimpleTryCatchExceptionWithNameAndConditionWithOr()
		{
			try {
				Console.WriteLine("Try");
				return this.B(new Random().Next());
			} catch (Exception ex) when (ex is ArgumentException || ex is IOException) {
				Console.WriteLine("CatchException ex: " + ex.ToString());
			}
			return false;
		}

		public async Task<bool> SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr()
		{
			try {
				Console.WriteLine("Try");
				return await this.T();
			} catch (Exception ex) when (ex is ArgumentException || ex is IOException) {
				Console.WriteLine("CatchException ex: " + ex.ToString());
			}
			return false;
		}

		public void CatchWhenWithConditionWithoutExceptionVar()
		{
			int num = 0;
			try {
				throw new Exception();
			} catch (Exception) when (num == 0) {
				Console.WriteLine("jo");
			}
		}

#endif

		public bool SimpleTryFinally()
		{
			try {
				Console.WriteLine("Try");
			} finally {
				Console.WriteLine("Finally");
			}
			return false;
		}

		public void MethodEndingWithEndFinally()
		{
			try {
				throw null;
			} finally {
				Console.WriteLine();
			}
		}

		public void MethodEndingWithRethrow()
		{
			try {
				throw null;
			} catch {
				throw;
			}
		}

		public void TryCatchFinally()
		{
			try {
				Console.WriteLine("Try");
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			} finally {
				Console.WriteLine("Finally");
			}
		}

		public void TryCatchMultipleHandlers()
		{
			try {
				Console.WriteLine("Try");
			} catch (InvalidOperationException ex) {
				Console.WriteLine(ex.Message);
			} catch (SystemException ex2) {
				Console.WriteLine(ex2.Message);
			} catch {
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
			try {
				cancellationTokenSource = new CancellationTokenSource();
			} finally {
				if (cancellationTokenSource != null) {
					cancellationTokenSource.Dispose();
				}
			}
		}


	}
}