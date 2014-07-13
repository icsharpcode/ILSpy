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
	/// An immutable set of integers, that is implemented as a list of intervals.
	/// </summary>
	struct IntegerSet
	{
		public readonly ImmutableArray<Interval> Intervals;

		public IntegerSet(ImmutableArray<Interval> intervals)
		{
			this.Intervals = intervals;
		}
		
		public bool IsEmpty
		{
			get { return Intervals.IsDefaultOrEmpty; }
		}

		public bool Contains(int val)
		{
			// TODO: use binary search
			foreach (Interval v in Intervals) {
				if (v.Start <= val && val <= v.End)
					return true;
			}
			return false;
		}

		public override string ToString()
		{
			return string.Join(",", Intervals);
		}
	}
}
