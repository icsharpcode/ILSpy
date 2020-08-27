// Copyright (c) 2014 Daniel Grunwald
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

using ICSharpCode.Decompiler.Util;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Util
{
	public class IntervalTests
	{
		[Test]
		public void DefaultIsEmpty()
		{
			Assert.IsTrue(default(Interval).IsEmpty);
			Assert.IsFalse(default(Interval).Contains(-1));
			Assert.IsFalse(default(Interval).Contains(0));
			Assert.IsFalse(default(Interval).Contains(1));
		}
		[Test]
		public void EmptyAt1()
		{
			Interval i = new Interval(1, 1);
			Assert.IsTrue(default(Interval).IsEmpty);
			Assert.IsFalse(default(Interval).Contains(-1));
			Assert.IsFalse(default(Interval).Contains(0));
			Assert.IsFalse(default(Interval).Contains(1));
			Assert.IsFalse(default(Interval).Contains(2));
		}

		[Test]
		public void OneToThree()
		{
			Interval i = new Interval(1, 3);
			Assert.IsFalse(i.IsEmpty);
			Assert.IsFalse(i.Contains(0));
			Assert.IsTrue(i.Contains(1));
			Assert.IsTrue(i.Contains(2));
			Assert.IsFalse(i.Contains(3));
		}

		[Test]
		public void FullInterval()
		{
			Interval full = new Interval(int.MinValue, int.MinValue);
			Assert.IsFalse(full.IsEmpty);
			Assert.IsTrue(full.Contains(int.MinValue));
			Assert.IsTrue(full.Contains(0));
			Assert.IsTrue(full.Contains(int.MaxValue));
		}

		[Test]
		public void NonNegativeIntegers()
		{
			Interval i = new Interval(0, int.MinValue);
			Assert.IsFalse(i.IsEmpty);
			Assert.IsTrue(i.Contains(0));
			Assert.IsTrue(i.Contains(1000));
			Assert.IsTrue(i.Contains(int.MaxValue));
			Assert.IsFalse(i.Contains(-1));
			Assert.IsFalse(i.Contains(-1000));
			Assert.IsFalse(i.Contains(int.MinValue));
		}

		[Test]
		public void Intersection()
		{
			Interval empty = new Interval(0, 0);
			Interval emptyAtOne = new Interval(0, 0);
			Interval zero = new Interval(0, 1);
			Interval full = new Interval(int.MinValue, int.MinValue);
			Interval nonneg = new Interval(0, int.MinValue);
			Interval nonpos = new Interval(int.MinValue, 1);
			Interval maxval = new Interval(int.MaxValue, int.MinValue);
			Assert.AreEqual(nonneg, full.Intersect(nonneg));
			Assert.AreEqual(nonneg, nonneg.Intersect(full));
			Assert.AreEqual(zero, nonneg.Intersect(zero));
			Assert.AreEqual(zero, nonneg.Intersect(nonpos));
			Assert.AreEqual(maxval, nonneg.Intersect(maxval));
			Assert.AreEqual(empty, nonpos.Intersect(maxval));
		}
	}
}
