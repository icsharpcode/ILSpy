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
			Assert.That(longSet.upper_bound(0), Is.EqualTo(0));
			for (int i = 1; i <= 5; i++)
				Assert.That(longSet.upper_bound(i), Is.EqualTo(1));
			for (int i = 6; i <= 10; i++)
				Assert.That(longSet.upper_bound(i), Is.EqualTo(2));
		}

		[Test]
		public void UniverseContainsAll()
		{
			Assert.That(LongSet.Universe.Contains(long.MinValue));
			Assert.That(LongSet.Universe.Contains(1));
			Assert.That(LongSet.Universe.Contains(long.MaxValue));
			Assert.That(!LongSet.Universe.IsEmpty);
		}

		[Test]
		public void IntersectUniverse()
		{
			Assert.That(LongSet.Universe.IntersectWith(LongSet.Universe), Is.EqualTo(LongSet.Universe));
			Assert.That(LongSet.Universe.IntersectWith(LongSet.Empty), Is.EqualTo(LongSet.Empty));
			Assert.That(LongSet.Universe.IntersectWith(new LongSet(long.MaxValue)), Is.EqualTo(new LongSet(long.MaxValue)));
			var longSet = new LongSet(new[] { new LongInterval(1, 5), new LongInterval(6, 7) }.ToImmutableArray());
			Assert.That(longSet.IntersectWith(LongSet.Universe), Is.EqualTo(longSet));
		}

		[Test]
		public void UnionUniverse()
		{
			Assert.That(LongSet.Universe.UnionWith(LongSet.Universe), Is.EqualTo(LongSet.Universe));
			Assert.That(LongSet.Universe.UnionWith(LongSet.Empty), Is.EqualTo(LongSet.Universe));
			Assert.That(LongSet.Universe.UnionWith(new LongSet(long.MaxValue)), Is.EqualTo(LongSet.Universe));
			var longSet = new LongSet(new[] { new LongInterval(1, 5), new LongInterval(6, 7) }.ToImmutableArray());
			Assert.That(longSet.UnionWith(LongSet.Universe), Is.EqualTo(LongSet.Universe));
		}

		[Test]
		public void ExceptWithUniverse()
		{
			Assert.That(LongSet.Universe.ExceptWith(LongSet.Empty), Is.EqualTo(LongSet.Universe));
			Assert.That(LongSet.Universe.ExceptWith(LongSet.Universe), Is.EqualTo(LongSet.Empty));
			Assert.That(LongSet.Empty.ExceptWith(LongSet.Universe), Is.EqualTo(LongSet.Empty));
			Assert.That(LongSet.Empty.ExceptWith(LongSet.Empty), Is.EqualTo(LongSet.Empty));
		}

		[Test]
		public void UnionWith()
		{
			Assert.That(new LongSet(0).UnionWith(new LongSet(1)), Is.EqualTo(new LongSet(new LongInterval(0, 2))));

			Assert.That(new LongSet(0).Invert().UnionWith(new LongSet(0)), Is.EqualTo(LongSet.Universe));
		}

		[Test]
		public void AddTo()
		{
			Assert.That(new LongSet(0).AddOffset(1), Is.EqualTo(new LongSet(1)));
			Assert.That(new LongSet(long.MaxValue).AddOffset(1), Is.EqualTo(new LongSet(long.MinValue)));

			TestAddTo(new LongSet(new LongInterval(-10, 10)), 5);
			TestAddTo(new LongSet(new LongInterval(-10, 10)), long.MaxValue);
			Assert.That(new LongSet(0).Invert().AddOffset(10), Is.EqualTo(new LongSet(10).Invert()));
			Assert.That(new LongSet(30).Invert().AddOffset(-10), Is.EqualTo(new LongSet(20).Invert()));
		}

		void TestAddTo(LongSet input, long constant)
		{
			Assert.That(
				input.AddOffset(constant).Values.ToList(), Is.EqualTo(input.Values.Select(e => unchecked(e + constant)).OrderBy(e => e).ToList()));
		}

		[Test]
		public void Values()
		{
			Assert.That(!LongSet.Empty.Values.Any());
			Assert.That(LongSet.Universe.Values.Any());
			Assert.That(new LongSet(LongInterval.Inclusive(1, 3)).Values.ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));
		}

		[Test]
		public void ValueCount()
		{
			Assert.That(LongSet.Empty.Count(), Is.EqualTo(0));
			Assert.That(new LongSet(3).Invert().Count(), Is.EqualTo(ulong.MaxValue));
			Assert.That(LongSet.Universe.Count(), Is.EqualTo(ulong.MaxValue));
			Assert.That(new LongSet(LongInterval.Inclusive(-1, long.MaxValue)).Count(), Is.EqualTo(long.MaxValue + 2ul));
		}
	}
}
