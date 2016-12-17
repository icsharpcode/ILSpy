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
			var V_0 = new {
			};
			var V_1 = new {
				X = 5
			};
			var V_2 = new {
				X = 5,
				Y = 10
			};

			Console.WriteLine((object)V_0);
			Console.WriteLine(V_1.X);
			Console.WriteLine(V_2.Y + V_2.X);
		}

		private void SimpleArray()
		{
			var V_0 = new[] {
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

			Console.WriteLine(V_0[0].X);
			Console.WriteLine(V_0[1].X);
		}

		private void JaggedArray()
		{
			var V_0 = new[] {
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
			var V_1 = new[] {
				V_0,
				V_0
			};

			Console.WriteLine(V_0[0].X);
			Console.WriteLine(V_0[1].X);
			Console.WriteLine(V_1.Length);
		}
	}
}
