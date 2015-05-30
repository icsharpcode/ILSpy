using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler
{
	/// <summary>
	/// Represents a half-open interval.
	/// The start position is inclusive; but the end position is exclusive.
	/// </summary>
	public struct Interval : IEquatable<Interval>
	{
		/// <summary>
		/// Gets the inclusive start of the interval.
		/// </summary>
		public readonly int Start;

		/// <summary>
		/// Gets the exclusive end of the interval.
		/// </summary>
		public readonly int End;

		public Interval(int start, int end)
		{
			if (start > end && end != int.MinValue)
				throw new ArgumentException("The end must be after the start", "end");
			this.Start = start;
			this.End = end;
		}

		public bool Contains(int val)
		{
			// Use 'val <= End-1' instead of 'val < End' to allow intervals to include int.MaxValue.
			return Start <= val && val <= unchecked(End - 1);
		}

		public override string ToString()
		{
			return string.Format("[{0}..{1})", Start, End);
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return (obj is Interval) && Equals((Interval)obj);
		}
		
		public bool Equals(Interval other)
		{
			return this.Start == other.Start && this.End == other.End;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * Start.GetHashCode();
				hashCode += 1000000009 * End.GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(Interval lhs, Interval rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Interval lhs, Interval rhs) {
			return !(lhs == rhs);
		}
		#endregion
	}

	/// <summary>
	/// Represents a half-open interval.
	/// The start position is inclusive; but the end position is exclusive.
	/// </summary>
	public struct LongInterval : IEquatable<LongInterval>
	{
		/// <summary>
		/// Gets the inclusive start of the interval.
		/// </summary>
		public readonly long Start;

		/// <summary>
		/// Gets the exclusive end of the interval.
		/// </summary>
		public readonly long End;

		public LongInterval(long start, long end)
		{
			if (start > end && end != long.MinValue)
				throw new ArgumentException("The end must be after the start", "end");
			this.Start = start;
			this.End = end;
		}

		public bool Contains(long val)
		{
			// Use 'val <= End-1' instead of 'val < End' to allow intervals to include int.MaxValue.
			return Start <= val && val <= unchecked(End - 1);
		}

		public override string ToString()
		{
			return string.Format("[{0}..{1})", Start, End);
		}

		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return (obj is LongInterval) && Equals((LongInterval)obj);
		}

		public bool Equals(LongInterval other)
		{
			return this.Start == other.Start && this.End == other.End;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked
			{
				hashCode += 1000000007 * Start.GetHashCode();
				hashCode += 1000000009 * End.GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(LongInterval lhs, LongInterval rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(LongInterval lhs, LongInterval rhs)
		{
			return !(lhs == rhs);
		}
		#endregion
		
		public IEnumerable<long> Range()
		{
			for (long i = Start; i < End; i++)
				yield return i;
		}
	}

	/// <summary>
	/// An immutable set of longs, that is implemented as a list of intervals.
	/// </summary>
	public struct LongSet
	{
		public readonly ImmutableArray<LongInterval> Intervals;

		public LongSet(ImmutableArray<LongInterval> intervals)
		{
			this.Intervals = intervals;
		}
		
		public LongSet(long value)
			: this(ImmutableArray.Create(new LongInterval(value, unchecked(value + 1))))
		{
		}
		
		public bool IsEmpty
		{
			get { return Intervals.IsDefaultOrEmpty; }
		}

		public bool Contains(long val)
		{
			int index = upper_bound(val);
			return index > 0 && Intervals[index - 1].Contains(val);
		}
		
		internal int upper_bound(long val)
		{
			int min = 0, max = Intervals.Length - 1;
			while (max >= min) {
				int m = min + (max - min) / 2;
				LongInterval i = Intervals[m];
				if (val < i.Start) {
					max = m - 1;
					continue;
				}
				if (val > i.End) {
					min = m + 1;
					continue;
				}
				return m + 1;
			}
			return min;
		}

		public IEnumerable<long> Range()
		{
			return Intervals.SelectMany(i => i.Range());
		}
		
		public override string ToString()
		{
			return string.Join(",", Intervals);
		}
	}
}
