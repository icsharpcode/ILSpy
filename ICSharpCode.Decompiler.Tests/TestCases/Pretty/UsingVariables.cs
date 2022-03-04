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
#if !NET40
using System.Threading.Tasks;
#endif

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class UsingVariables
	{
		public IDisposable GetDisposable()
		{
			return null;
		}

		private void Use(IDisposable disposable)
		{

		}

#if !NET40
		public IAsyncDisposable GetAsyncDisposable()
		{
			return null;
		}

		private void Use(IAsyncDisposable asyncDisposable)
		{

		}
#endif

		public void SimpleUsingVar()
		{
			Console.WriteLine("before using");
			using IDisposable disposable = GetDisposable();
			Console.WriteLine("inside using");
			Use(disposable);
		}

		public void NotAUsingVar()
		{
			Console.WriteLine("before using");
			using (IDisposable disposable = GetDisposable())
			{
				Console.WriteLine("inside using");
				Use(disposable);
			}
			Console.WriteLine("outside using");
		}

		public void UsingVarInNestedBlocks(bool condition)
		{
			if (condition)
			{
				using IDisposable disposable = GetDisposable();
				Console.WriteLine("inside using");
				Use(disposable);
			}
			Console.WriteLine("outside using");
		}

		public void MultipleUsingVars(IDisposable other)
		{
			Console.WriteLine("before using");
			using IDisposable disposable = GetDisposable();
			Console.WriteLine("inside outer using");
			using IDisposable disposable2 = other;
			Console.WriteLine("inside inner using");
			Use(disposable);
			Use(disposable2);
		}
#if !NET40
		public async Task SimpleUsingVarAsync()
		{
			Console.WriteLine("before using");
			await using IAsyncDisposable asyncDisposable = GetAsyncDisposable();
			Console.WriteLine("inside using");
			Use(asyncDisposable);
		}
#endif
	}
}
