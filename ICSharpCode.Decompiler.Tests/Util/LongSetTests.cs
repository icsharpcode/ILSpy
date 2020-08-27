// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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

using System.Collections.Immutable;
using System.Linq;

using ICSharpCode.Decompiler.Util;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Util
{
	[TestFixture]
	public class LongSetTests
	{
		[Test]
		public void UpperBound()
		{
			var longSet = new LongSet(new[] { new LongInterval(1, 5), new LongInterval(6, 7) }.ToImmutableArray());
			Assert.AreEqual(0, longSet.upper_bound(0));
			for (int i = 1; i <= 5; i++)
				Assert.AreEqual(1, longSet.upper_bound(i));
			for (int i = 6; i <= 10; i++)
				Assert.AreEqual(2, longSet.upper_bound(i));
		}

		[Test]
		public void UniverseContainsAll()
		{
			Assert.IsTrue(LongSet.Universe.Contains(long.MinValue));
			Assert.IsTrue(LongSet.Universe.Contains(1));
			Assert.IsTrue(LongSet.Universe.Contains(long.MaxValue));
			Assert.IsFalse(LongSet.Universe.IsEmpty);
		}

		[Test]
		public void IntersectUniverse()
		{
			Assert.AreEqual(LongSet.Universe, LongSet.Universe.IntersectWith(LongSet.Universe));
			Assert.AreEqual(LongSet.Empty, LongSet.Universe.IntersectWith(LongSet.Empty));
			Assert.AreEqual(new LongSet(long.MaxValue), LongSet.Universe.IntersectWith(new LongSet(long.MaxValue)));
			var longSet = new LongSet(new[] { new LongInterval(1, 5), new LongInterval(6, 7) }.ToImmutableArray());
			Assert.AreEqual(longSet, longSet.IntersectWith(LongSet.Universe));
		}

		[Test]
		public void UnionUniverse()
		{
			Assert.AreEqual(LongSet.Universe, LongSet.Universe.UnionWith(LongSet.Universe));
			Assert.AreEqual(LongSet.Universe, LongSet.Universe.UnionWith(LongSet.Empty));
			Assert.AreEqual(LongSet.Universe, LongSet.Universe.UnionWith(new LongSet(long.MaxValue)));
			var longSet = new LongSet(new[] { new LongInterval(1, 5), new LongInterval(6, 7) }.ToImmutableArray());
			Assert.AreEqual(LongSet.Universe, longSet.UnionWith(LongSet.Universe));
		}

		[Test]
		public void ExceptWithUniverse()
		{
			Assert.AreEqual(LongSet.Universe, LongSet.Universe.ExceptWith(LongSet.Empty));
			Assert.AreEqual(LongSet.Empty, LongSet.Universe.ExceptWith(LongSet.Universe));
			Assert.AreEqual(LongSet.Empty, LongSet.Empty.ExceptWith(LongSet.Universe));
			Assert.AreEqual(LongSet.Empty, LongSet.Empty.ExceptWith(LongSet.Empty));
		}

		[Test]
		public void UnionWith()
		{
			Assert.AreEqual(new LongSet(new LongInterval(0, 2)),
				new LongSet(0).UnionWith(new LongSet(1)));

			Assert.AreEqual(LongSet.Universe, new LongSet(0).Invert().UnionWith(new LongSet(0)));
		}

		[Test]
		public void AddTo()
		{
			Assert.AreEqual(new LongSet(1), new LongSet(0).AddOffset(1));
			Assert.AreEqual(new LongSet(long.MinValue), new LongSet(long.MaxValue).AddOffset(1));

			TestAddTo(new LongSet(new LongInterval(-10, 10)), 5);
			TestAddTo(new LongSet(new LongInterval(-10, 10)), long.MaxValue);
			Assert.AreEqual(new LongSet(10).Invert(), new LongSet(0).Invert().AddOffset(10));
			Assert.AreEqual(new LongSet(20).Invert(), new LongSet(30).Invert().AddOffset(-10));
		}

		void TestAddTo(LongSet input, long constant)
		{
			Assert.AreEqual(
				input.Values.Select(e => unchecked(e + constant)).OrderBy(e => e).ToList(),
				input.AddOffset(constant).Values.ToList());
		}

		[Test]
		public void Values()
		{
			Assert.IsFalse(LongSet.Empty.Values.Any());
			Assert.IsTrue(LongSet.Universe.Values.Any());
			Assert.AreEqual(new[] { 1, 2, 3 }, new LongSet(LongInterval.Inclusive(1, 3)).Values.ToArray());
		}

		[Test]
		public void ValueCount()
		{
			Assert.AreEqual(0, LongSet.Empty.Count());
			Assert.AreEqual(ulong.MaxValue, new LongSet(3).Invert().Count());
			Assert.AreEqual(ulong.MaxValue, LongSet.Universe.Count());
			Assert.AreEqual(long.MaxValue + 2ul, new LongSet(LongInterval.Inclusive(-1, long.MaxValue)).Count());
		}
	}
}
