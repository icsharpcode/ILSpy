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

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class Using
	{
		class PrintOnDispose : IDisposable
		{
			private string v;

			public PrintOnDispose(string v)
			{
				this.v = v;
			}

			public void Dispose()
			{
				Console.WriteLine(this.v);
			}
		}

		static void Main()
		{
			SimpleUsingNullStatement();
			NoUsing();
			NoUsing2();
			ThisIsNotAUsingBlock();
			UsingObject();
		}

		/// <summary>
		/// Special case: Roslyn eliminates the try-finally altogether.
		/// </summary>
		public static void SimpleUsingNullStatement()
		{
			Console.WriteLine("before using");
			using (null) {
				Console.WriteLine("using (null)");
			}
			Console.WriteLine("after using");
		}

		public static void NoUsing()
		{
			PrintOnDispose printOnDispose = new PrintOnDispose("Wrong");
			try {
				printOnDispose = new PrintOnDispose("Correct");
			} finally {
				printOnDispose.Dispose();
			}
		}

		public static void NoUsing2()
		{
			PrintOnDispose printOnDispose = new PrintOnDispose("NoUsing(): Wrong");
			try {
				printOnDispose = new PrintOnDispose("NoUsing(): Correct");
			} finally {
				IDisposable disposable = (object)printOnDispose as IDisposable;
				if (disposable != null) {
					disposable.Dispose();
				}
			}
		}

		public static void ThisIsNotAUsingBlock()
		{
			var obj = new System.IO.StringWriter();
			IDisposable disposable;
			try {
				obj.WriteLine("ThisIsNotAUsingBlock");
			} finally {
				disposable = (object)obj as IDisposable;
				if (disposable != null) {
					disposable.Dispose();
				}
			}
			Console.WriteLine(disposable);
		}

		public static void UsingObject()
		{
			object obj = new object();
			try {
				Console.WriteLine("UsingObject: {0}", obj);
			} finally {
				IDisposable disposable = obj as IDisposable;
				if (disposable != null) {
					disposable.Dispose();
				}
			}
		}
	}
}
