using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public static class Issue3552
	{
		public static Issue3552_IntegerPair MakePair1(int x, int y)
		{
			Issue3552_IntegerPairBuilder issue3552_IntegerPairBuilder = new Issue3552_IntegerPairBuilder { x, y };
			return issue3552_IntegerPairBuilder.ToPair();
		}
		public static Issue3552_IntegerPair MakePair2(int x, int y)
		{
			Issue3552_IntegerPairBuilder issue3552_IntegerPairBuilder = new Issue3552_IntegerPairBuilder { x, y };
			return issue3552_IntegerPairBuilder.ToPair();
		}
		public static Issue3552_IntegerPair MakePair3(int x, int y)
		{
			Issue3552_IntegerPairBuilder issue3552_IntegerPairBuilder = new Issue3552_IntegerPairBuilder { x, y };
			return issue3552_IntegerPairBuilder.ToPair();
		}
	}
	public struct Issue3552_IntegerPair
	{
		public int X;
		public int Y;
	}
	public struct Issue3552_IntegerPairBuilder : IEnumerable<int>, IEnumerable
	{
		private int index;
		private Issue3552_IntegerPair pair;

		public readonly Issue3552_IntegerPair ToPair()
		{
			return pair;
		}

		public void Add(int value)
		{
			switch (index)
			{
				case 0:
					pair.X = value;
					break;
				case 1:
					pair.Y = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
			index++;
		}

		public IEnumerator<int> GetEnumerator()
		{
			return null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}