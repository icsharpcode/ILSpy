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
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class Using
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct UsingStruct : IDisposable
		{
			public UsingStruct(int i)
			{
				Console.WriteLine(i);
			}

			void IDisposable.Dispose()
			{
				throw new NotImplementedException();
			}
		}

#if CS80
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public ref struct UsingRefStruct
		{
			public int i;

			public UsingRefStruct(int i)
			{
				this.i = i;
				Console.WriteLine(i);
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}
		}
#endif

#if LEGACY_CSC
		// roslyn optimizes out the try-finally; mcs has a compiler bug on using(null-literal)
		public void SimpleUsingNullStatement()
		{
			using (null) {
				Console.WriteLine("using (null)");
			}
		}
#endif

		public void SimpleUsingExpressionStatement()
		{
			using (new MemoryStream()) {
				Console.WriteLine("using-body");
			}
		}

		public void SimpleUsingExpressionStatementWithDeclaration()
		{
			using (MemoryStream memoryStream = new MemoryStream()) {
				memoryStream.WriteByte(42);
				Console.WriteLine("using-body: " + memoryStream.Position);
			}
		}

		public void UsingStatementThatChangesTheVariable()
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			using (cancellationTokenSource) {
				cancellationTokenSource = new CancellationTokenSource();
			}
			cancellationTokenSource.Cancel();
		}

		public void UsingStatementOnStruct()
		{
			using (new UsingStruct(1)) {
				Console.WriteLine("using-body");
			}
		}

		public void UsingStatementOnStructWithVariable()
		{
			using (UsingStruct usingStruct = new UsingStruct(2)) {
				Console.WriteLine("using-body: " + usingStruct);
			}
		}

		private void UsingStatementOnNullableStruct(UsingStruct? us)
		{
			using (us) {
				Console.WriteLine("using-body: " + us.ToString());
			}
		}

		public void GenericUsing<T>(T t) where T : IDisposable
		{
			using (t) {
				Console.WriteLine(t);
			}
		}

		public void GenericStructUsing<T>(T t) where T : struct, IDisposable
		{
			using (t) {
				Console.WriteLine(t);
			}
		}

		public void GenericClassUsing<T>(T t) where T : class, IDisposable
		{
			using (t) {
				Console.WriteLine(t);
			}
		}

		public void GenericNullableUsing<T>(T? t) where T : struct, IDisposable
		{
			using (t) {
				Console.WriteLine(t);
			}
		}

#if CS80
		public void UsingRefStruct1(UsingRefStruct s)
		{
			using (s) {
				Console.WriteLine(s.i);
			}
		}
#endif
	}
}
