using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler
{
	/// <summary>
	/// Represents a half-closed interval.
	/// The start position is inclusive; but the end position is exclusive.
	/// </summary>
	/// <remarks>
	/// Start &lt;= unchecked(End - 1): normal interval
	/// Start == End: empty interval
	/// Special case: Start == End == int.MinValue: interval containing all integers, not an empty interval!
	/// </remarks>
	public struct Interval : IEquatable<Interval>
	{
		/// <summary>
		/// Gets the inclusive start of the interval.
		/// </summary>
		public readonly int Start;

		/// <summary>
		/// Gets the exclusive end of the interval.
		/// </summary>
		/// <remarks>
		/// Note that an End of int.MinValue is a special case, and stands
		/// for an actual End of int.MaxValue+1.
		/// If possible, prefer using InclusiveEnd for comparisons, as that does not have an overflow problem.
		/// </remarks>
		public readonly int End;
		
		/// <summary>
		/// Creates a new interval.
		/// </summary>
		/// <param name="start">Start position (inclusive)</param>
		/// <param name="end">End position (exclusive).
		/// Note that it is possible to create an interval that includes int.MaxValue
		/// by using end==int.MaxValue+1==int.MinValue.</param>
		public Interval(int start, int end)
		{
			if (!(start <= unchecked(end - 1) || start == end))
				throw new ArgumentException("The end must be after the start", "end");
			this.Start = start;
			this.End = end;
		}
		
		/// <summary>
		/// Gets the inclusive end of the interval. (End - 1)
		/// For empty intervals, this returns Start - 1.
		/// </summary>
		/// <remarks>
		/// Because there is no empty interval at int.MinValue,
		/// (Start==End==int.MinValue is a special case referring to [int.MinValue..int.MaxValue]),
		/// integer overflow is not a problem here.
		/// </remarks>
		public int InclusiveEnd {
			get {
				return unchecked(End - 1);
			}
		}

		public bool IsEmpty {
			get {
				return Start > InclusiveEnd;
			}
		}
		
		public bool Contains(int val)
		{
			// Use 'val <= InclusiveEnd' instead of 'val < End' to allow intervals to include int.MaxValue.
			return Start <= val && val <= InclusiveEnd;
		}
		
		/// <summary>
		/// Calculates the intersection between this interval and the other interval.
		/// </summary>
		public Interval Intersect(Interval other)
		{
			int start = Math.Max(this.Start, other.Start);
			int inclusiveEnd = Math.Min(this.InclusiveEnd, other.InclusiveEnd);
			if (start <= inclusiveEnd)
				return new Interval(start, unchecked(inclusiveEnd + 1));
			else
				return default(Interval);
		}
		
		public override string ToString()
		{
			if (End == int.MinValue)
				return string.Format("[{0}..int.MaxValue]", Start);
			else
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
			return Start ^ End ^ (End << 7);
			}

		public static bool operator ==(Interval lhs, Interval rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Interval lhs, Interval rhs)
		{
			return !(lhs == rhs);
		}
		#endregion
	}

	/// <summary>
	/// Represents a half-closed interval.
	/// The start position is inclusive; but the end position is exclusive.
	/// </summary>
	/// <remarks>
	/// Start &lt;= unchecked(End - 1): normal interval
	/// Start == End: empty interval
	/// Special case: Start == End == int.MinValue: interval containing all integers, not an empty interval!
	/// </remarks>
	public struct LongInterval : IEquatable<LongInterval>
	{
		/// <summary>
		/// Gets the inclusive start of the interval.
		/// </summary>
		public readonly long Start;

		/// <summary>
		/// Gets the exclusive end of the interval.
		/// </summary>
		/// <remarks>
		/// Note that an End of long.MinValue is a special case, and stands
		/// for an actual End of long.MaxValue+1.
		/// If possible, prefer using InclusiveEnd for comparisons, as that does not have an overflow problem.
		/// </remarks>
		public readonly long End;
		
		/// <summary>
		/// Creates a new interval.
		/// </summary>
		/// <param name="start">Start position (inclusive)</param>
		/// <param name="end">End position (exclusive).
		/// Note that it is possible to create an interval that includes int.MaxValue
		/// by using end==int.MaxValue+1==int.MinValue.</param>
		public LongInterval(long start, long end)
		{
			if (!(start <= unchecked(end - 1) || start == end))
				throw new ArgumentException("The end must be after the start", "end");
			this.Start = start;
			this.End = end;
		}
		
		/// <summary>
		/// Gets the inclusive end of the interval. (End - 1)
		/// For empty intervals, this returns Start - 1.
		/// </summary>
		/// <remarks>
		/// Because there is no empty interval at int.MinValue,
		/// (Start==End==int.MinValue is a special case referring to [int.MinValue..int.MaxValue]),
		/// integer overflow is not a problem here.
		/// </remarks>
		public long InclusiveEnd {
			get {
				return unchecked(End - 1);
			}
		}

		public bool IsEmpty {
			get {
				return Start > InclusiveEnd;
			}
		}
		
		public bool Contains(long val)
		{
			// Use 'val <= InclusiveEnd' instead of 'val < End' to allow intervals to include int.MaxValue.
			return Start <= val && val <= InclusiveEnd;
		}
		
		/// <summary>
		/// Calculates the intersection between this interval and the other interval.
		/// </summary>
		public LongInterval Intersect(LongInterval other)
		{
			long start = Math.Max(this.Start, other.Start);
			long inclusiveEnd = Math.Min(this.InclusiveEnd, other.InclusiveEnd);
			if (start <= inclusiveEnd)
				return new LongInterval(start, unchecked(inclusiveEnd + 1));
			else
				return default(LongInterval);
		}
		
		/// <summary>
		/// Returns an enumerator over all values in this interval.
		/// </summary>
		public IEnumerable<long> Range()
		{
			if (End == long.MinValue) {
				long i = Start;
				while (true) {
					yield return i;
					if (i == long.MaxValue)
						break;
					i++;
				}
			} else {
				for (long i = Start; i < End; i++)
					yield return i;
			}
		}
		
		public override string ToString()
		{
			if (End == long.MinValue)
				return string.Format("[{0}..long.MaxValue]", Start);
			else
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
			return (Start ^ End ^ (End << 7)).GetHashCode();
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
