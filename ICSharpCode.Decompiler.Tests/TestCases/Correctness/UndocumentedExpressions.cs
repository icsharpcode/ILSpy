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
	public class UndocumentedExpressions
	{
		static void Main(string[] args)
		{
			MakeTypedRef("abc");
			VarArgs(1, __arglist());
			VarArgs(__arglist(1));
			VarArgs(1, __arglist("abc", 2, true));
			VarArgs(1, __arglist((object)"abc", 2, true));
			VarArgs(1, __arglist((short)1));
			VarArgs(1, __arglist(ConsoleColor.Red));
		}

		public static void VarArgs(int normalArg, __arglist)
		{
			ArgIterator argIterator = new ArgIterator(__arglist);
			Console.WriteLine("Called with {0} arguments", argIterator.GetRemainingCount());
			int pos = 0;
			while (argIterator.GetRemainingCount() > 0)
			{
				TypedReference tr = argIterator.GetNextArg();
				object val;
				try
				{
					val = __refvalue(tr, object);
				}
				catch (Exception ex)
				{
					val = ex.GetType().Name;
				}
				Console.WriteLine("{0} : {1} = {2}", pos++, __reftype(tr).Name, val);
			}
		}

		public static void VarArgs(__arglist)
		{
			Console.WriteLine("The other varargs overload");
		}

		public static void MakeTypedRef(object o)
		{
			TypedReference tr = __makeref(o);
			UndocumentedExpressions.AcceptTypedRef(tr);
		}

		private static void AcceptTypedRef(TypedReference tr)
		{
			Console.WriteLine("Value is: " + __refvalue(tr, object).ToString());
			Console.WriteLine("Type is: " + __reftype(tr).Name);
			__refvalue(tr, object) = 1;
		}
	}
}