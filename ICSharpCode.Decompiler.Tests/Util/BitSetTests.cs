// Copyright (c) 2017 Daniel Grunwald
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
	[TestFixture]
	public class BitSetTests
	{
		[Test]
		public void SetRange()
		{
			var bitset = new BitSet(302);
			bitset.Set(2, 300);
			Assert.IsFalse(bitset[0]);
			Assert.IsFalse(bitset[1]);
			for (int i = 2; i < 300; ++i)
			{
				Assert.IsTrue(bitset[i]);
			}
			Assert.IsFalse(bitset[301]);
		}

		[Test]
		public void ClearRange()
		{
			var bitset = new BitSet(300);
			bitset.Set(0, 300);
			bitset.Clear(1, 299);
			Assert.IsTrue(bitset[0]);
			for (int i = 1; i < 299; ++i)
			{
				Assert.IsFalse(bitset[i]);
			}
			Assert.IsTrue(bitset[299]);
		}

		[Test]
		public void AllInRange()
		{
			var bitset = new BitSet(300);
			bitset.Set(1, 299);
			Assert.IsTrue(bitset.All(1, 299));
			Assert.IsTrue(bitset.All(10, 290));
			Assert.IsTrue(bitset.All(100, 200));
			Assert.IsFalse(bitset.All(0, 200));
			Assert.IsFalse(bitset.All(0, 1));
			Assert.IsFalse(bitset.All(1, 300));
			bitset[200] = false;
			Assert.IsFalse(bitset.All(1, 299));
		}
	}
}
