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
	public class InlineAssignmentTest
	{
		private int field1;
		private InlineAssignmentTest field2;
		
		public static void Main()
		{
			
		}
		
		public void SimpleInlineWithLocals()
		{
			int V_0;
			Console.WriteLine(V_0 = 5);
			Console.WriteLine(V_0);
			InlineAssignmentTest V_1;
			Console.WriteLine((object)(V_1 = new InlineAssignmentTest()));
			Console.WriteLine((object)V_1);
		}
		
		public void SimpleInlineWithFields()
		{
			Console.WriteLine(this.field1 = 5);
			Console.WriteLine((object)(this.field2 = new InlineAssignmentTest()));
		}
		
		public void SimpleInlineWithFields2()
		{
			Console.WriteLine(this.field1 = 5);
			Console.WriteLine(this.field1);
			Console.WriteLine((object)(this.field2 = new InlineAssignmentTest()));
			Console.WriteLine((object)this.field2);
		}
	}
}
