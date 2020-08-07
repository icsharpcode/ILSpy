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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class MultidimensionalArray
	{
		internal class Generic<T, S> where T : new()
		{
			private T[,] a = new T[20, 20];
			private S[,][] b = new S[20, 20][];

			public T this[int i, int j] {
				get {
					return a[i, j];
				}
				set {
					a[i, j] = value;
				}
			}

			public void TestB(S x, ref S y)
			{
				b[5, 3] = new S[10];
				b[5, 3][0] = default(S);
				b[5, 3][1] = x;
				b[5, 3][2] = y;
			}

			public void PassByReference(ref T arr)
			{
				PassByReference(ref a[10, 10]);
			}
		}

		public int[][,] MakeArray()
		{
			return new int[10][,];
		}
	}
}