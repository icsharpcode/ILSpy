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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class OutVariables
	{
		public static void OutVarInShortCircuit(Dictionary<int, string> d)
		{
			if (d.Count > 2 && d.TryGetValue(42, out var value))
			{
				Console.WriteLine(value);
			}
		}

		public static Action CapturedOutVarInShortCircuit(Dictionary<int, string> d)
		{
			// Note: needs reasoning about "definitely assigned if true"
			// to ensure that the value is initialized when the delegate is declared.
			if (d.Count > 2 && d.TryGetValue(42, out var value))
			{
				return delegate {
					Console.WriteLine(value);
				};
			}
			return null;
		}

		private bool TryGet<T>(out T result)
		{
			result = default(T);
			return true;
		}

		public void M3()
		{
			TryGet<Dictionary<int, (int, string)>>(out Dictionary<int, (int A, string B)> data);

			Test();

			int Test()
			{
				return data[0].A;
			}
		}

		public void GetObject(out object obj)
		{
			obj = null;
		}

		public void M4()
		{
			GetObject(out dynamic obj);
			obj.Method();
		}
	}
}
