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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class AnonymousTypes
	{
		private void SimpleTypes()
		{
			var value = new {
			};
			var anon = new {
				X = 5
			};
			var anon2 = new {
				X = 5,
				Y = 10
			};

			Console.WriteLine(value);
			Console.WriteLine(anon.X);
			Console.WriteLine(anon2.Y + anon2.X);
		}

		private void SimpleArray()
		{
			var array = new[] {
				new {
					X = 5,
					Y = 2,
					Z = -1
				},
				new {
					X = 3,
					Y = 6,
					Z = -6
				}
			};

			Console.WriteLine(array[0].X);
			Console.WriteLine(array[1].X);
		}

		private void JaggedArray()
		{
			var array = new[] {
				new {
					X = 5,
					Y = 2,
					Z = -1
				},
				new {
					X = 3,
					Y = 6,
					Z = -6
				}
			};
			var array2 = new[] {
				array,
				array
			};

			Console.WriteLine(array[0].X);
			Console.WriteLine(array[1].X);
			Console.WriteLine(array2.Length);
		}
	}
}
