// Copyright (c) 2018 Daniel Grunwald
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
	public class TupleTypes
	{
		public ValueTuple VT0;
		public ValueTuple<int> VT1;
		public ValueTuple<int, int, int, int, int, int, int, ValueTuple> VT7EmptyRest;

		public (int, uint) Unnamed2;
		public (int, int, int) Unnamed3;
		public (int, int, int, int) Unnamed4;
		public (int, int, int, int, int) Unnamed5;
		public (int, int, int, int, int, int) Unnamed6;
		public (int, int, int, int, int, int, int) Unnamed7;
		public (int, int, int, int, int, int, int, int) Unnamed8;

		public (int a, uint b) Named2;
		public (int a, uint b)[] Named2Array;
		public (int a, int b, int c, int d, int e, int f, int g, int h) Named8;

		public (int, int a, int, int b, int) PartiallyNamed;

		public ((int a, int b) x, (int, int) y, (int c, int d) z) Nested1;
		public ((object a, dynamic b), dynamic, (dynamic c, object d)) Nested2;
		public (ValueTuple a, (int x1, int x2), ValueTuple<int> b, (int y1, int y2), (int, int) c) Nested3;
		public (int a, int b, int c, int d, int e, int f, int g, int h, (int i, int j)) Nested4;
	}
}
