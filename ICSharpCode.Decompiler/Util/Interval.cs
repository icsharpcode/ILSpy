#nullable enable
// Copyright (c) 2016 Daniel Grunwald
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
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Util
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
				throw new ArgumentException("The end must be after the start", nameof(end));
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
		public override bool Equals(object? obj)
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
	/// Special case: Start == End == long.MinValue: interval containing all integers, not an empty interval!
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
		/// Note that it is possible to create an interval that includes long.MaxValue
		/// by using end==long.MaxValue+1==long.MinValue.</param>
		/// <remarks>
		/// This method can be used to create an empty interval by specifying start==end,
		/// however this is error-prone due to the special case of
		/// start==end==long.MinValue being interpreted as the full interval [long.MinValue,long.MaxValue].
		/// </remarks>
		public LongInterval(long start, long end)
		{
			if (!(start <= unchecked(end - 1) || start == end))
				throw new ArgumentException("The end must be after the start", nameof(end));
			this.Start = start;
			this.End = end;
		}

		/// <summary>
		/// Creates a new interval from start to end.
		/// Unlike the constructor where the end position is exclusive,
		/// this method interprets the end position as inclusive.
		/// 
		/// This method cannot be used to construct an empty interval.
		/// </summary>
		public static LongInterval Inclusive(long start, long inclusiveEnd)
		{
			if (!(start <= inclusiveEnd))
				throw new ArgumentException();
			return new LongInterval(start, unchecked(inclusiveEnd + 1));
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
			// Use 'val <= InclusiveEnd' instead of 'val < End' to allow intervals to include long.MaxValue.
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
			if (End == long.MinValue)
			{
				long i = Start;
				while (true)
				{
					yield return i;
					if (i == long.MaxValue)
						break;
					i++;
				}
			}
			else
			{
				for (long i = Start; i < End; i++)
					yield return i;
			}
		}

		public override string ToString()
		{
			if (End == long.MinValue)
			{
				if (Start == long.MinValue)
					return "[long.MinValue..long.MaxValue]";
				else
					return $"[{Start}..long.MaxValue]";
			}
			else if (Start == long.MinValue)
			{
				return $"[long.MinValue..{End})";
			}
			else
			{
				return $"[{Start}..{End})";
			}
		}

		#region Equals and GetHashCode implementation
		public override bool Equals(object? obj)
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
}
