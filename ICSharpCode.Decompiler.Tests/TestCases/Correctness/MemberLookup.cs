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
	public class MemberLookup
	{
		static readonly Action delegateConstruction = (new Child1() as Base1).TestAction;

		public static int Main()
		{
			Console.WriteLine((new Child1() as Base1).Field);
			Child1.Test();
			delegateConstruction();
			new Child2b().CallTestMethod();
			return 0;
		}

		class Base1
		{
			public int Field = 1;

			protected virtual void TestMethod()
			{
				Property = 5;
				Console.WriteLine("Base1.TestMethod()");
				Console.WriteLine(Property);
			}

			public void TestAction()
			{
				Console.WriteLine("Base1.TestAction()");
			}

			public int Property { get; set; }

			public virtual int VirtProp {
				get {
					return 3;
				}
			}
		}

		class Child1 : Base1
		{
			Child1 child;
			new public int Field = 2;

			public static void Test()
			{
				var o = new Child1();
				o.child = new Child1();
				o.TestMethod();

				Console.WriteLine(((Base1)o).Property);
				Console.WriteLine(o.Property);
				Console.WriteLine(((Base1)o).VirtProp);
				Console.WriteLine(o.VirtProp);
			}

			protected override void TestMethod()
			{
				Property = 10;
				base.TestMethod();
				if (child != null)
					child.TestMethod();
				Console.WriteLine("Child1.TestMethod()");
				Console.WriteLine("Property = " + Property + " " + base.Property);
				Console.WriteLine("Field = " + Field);
				Console.WriteLine("base.Field = " + base.Field);
			}

			new public void TestAction()
			{
				Console.WriteLine("Child1.TestAction()");
			}

			new public int Property { get; set; }

			public override int VirtProp {
				get {
					return base.VirtProp * 2;
				}
			}
		}

		class Child2 : Base1
		{
			public void CallTestMethod()
			{
				Console.WriteLine("Child2 calling this.TestMethod():");
				this.TestMethod();
				Console.WriteLine("Child2 calling base.TestMethod():");
				base.TestMethod();
			}
		}

		class Child2b : Child2
		{
			protected override void TestMethod()
			{
				Console.WriteLine("Child2b.TestMethod");
			}
		}
	}
}
