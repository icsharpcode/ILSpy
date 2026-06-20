// Copyright (c) 2026 Siegfried Pammer
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

namespace ICSharpCode.Decompiler.Tests.TestCases.LocationsInAst
{
	// A small method body gives the location-setting output path several statements and tokens
	// to assign positions to.
	internal class LocationSample
	{
		public int Add(int a, int b)
		{
			int sum = a + b;
			return sum;
		}
	}

	// Exercises the statement headers whose sequence-point coordinates SequencePointBuilder derives
	// from token positions ('{'/'}', if/while/do-while/foreach/switch/lock '(...)', catch/when).
	internal class SequencePointSample
	{
		public int Headers(int n, int[] items)
		{
			int sum = 0;
			if (n > 0)
			{
				sum++;
			}
			else
			{
				sum--;
			}
			while (sum < n)
			{
				sum += 2;
			}
			do
			{
				sum--;
			}
			while (sum > 0);
			foreach (int item in items)
			{
				sum += item;
			}
			switch (n)
			{
				case 1:
					sum = 1;
					break;
				default:
					sum = 0;
					break;
			}
			lock (items)
			{
				sum++;
			}
			try
			{
				sum += n;
			}
			catch (Exception ex) when (ex.Message.Length > 0)
			{
				sum = -1;
			}
			catch (Exception)
			{
				sum = -2;
			}
			return sum;
		}
	}
}
