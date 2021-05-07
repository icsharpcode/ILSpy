// Copyright (c) 2008 Daniel Grunwald
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
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class MemberTests
	{
		public class IndexerNonDefaultName
		{
			[IndexerName("Foo")]
#if ROSLYN
			public int this[int index] => 0;
#else
			#pragma warning disable format
			public int this[int index] {
				get {
					return 0;
				}
			}
			#pragma warning restore format
#endif
		}

		[DefaultMember("Bar")]
		public class NoDefaultMember
		{
		}

		public const int IntConstant = 1;
		public const decimal DecimalConstant = 2m;

		private volatile int volatileField = 3;
		private static volatile int staticVolatileField = 4;

		public extern int ExternGetOnly { get; }
		public extern int ExternSetOnly { set; }
		public extern int ExternProperty { get; set; }

		public extern event EventHandler Event;

		public void UseVolatileFields()
		{
			Console.WriteLine(volatileField + staticVolatileField);
			volatileField++;
			staticVolatileField++;
		}

		public extern void ExternMethod();
	}
}
