// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Globalization;
using ICSharpCode.Decompiler.CSharp.Syntax;

namespace ICSharpCode.Decompiler.TypeSystem
{
	[Serializable]
	public struct DomRegion : IEquatable<DomRegion>
	{
		public readonly static DomRegion Empty = new DomRegion();
		
		public bool IsEmpty => BeginLine <= 0;

		public string FileName { get; }

		public int BeginLine { get; }

		/// <value>
		/// if the end line is == -1 the end column is -1 too
		/// this stands for an unknwon end
		/// </value>
		public int EndLine { get; }

		public int BeginColumn { get; }

		/// <value>
		/// if the end column is == -1 the end line is -1 too
		/// this stands for an unknown end
		/// </value>
		public int EndColumn { get; }

		public TextLocation Begin => new TextLocation (BeginLine, BeginColumn);

		public TextLocation End => new TextLocation (EndLine, EndColumn);

		public DomRegion (int beginLine, int beginColumn, int endLine, int endColumn) : this (null, beginLine, beginColumn, endLine, endColumn)
		{
		}

		public DomRegion(string fileName, int beginLine, int beginColumn, int endLine, int endColumn)
		{
			this.FileName = fileName;
			this.BeginLine   = beginLine;
			this.BeginColumn = beginColumn;
			this.EndLine     = endLine;
			this.EndColumn   = endColumn;
		}
		
		public DomRegion (int beginLine, int beginColumn) : this (null, beginLine, beginColumn)
		{
		}
		
		public DomRegion (string fileName, int beginLine, int beginColumn)
		{
			this.FileName = fileName;
			this.BeginLine = beginLine;
			this.BeginColumn = beginColumn;
			this.EndLine = -1;
			this.EndColumn = -1;
		}
		
		public DomRegion (TextLocation begin, TextLocation end) : this (null, begin, end)
		{
		}
		
		public DomRegion (string fileName, TextLocation begin, TextLocation end)
		{
			this.FileName = fileName;
			this.BeginLine = begin.Line;
			this.BeginColumn = begin.Column;
			this.EndLine = end.Line;
			this.EndColumn = end.Column;
		}
		
		public DomRegion (TextLocation begin) : this (null, begin)
		{
		}
		
		public DomRegion (string fileName, TextLocation begin)
		{
			this.FileName = fileName;
			this.BeginLine = begin.Line;
			this.BeginColumn = begin.Column;
			this.EndLine = -1;
			this.EndColumn = -1;
		}
		
		/// <remarks>
		/// Returns true, if the given coordinates (line, column) are in the region.
		/// This method assumes that for an unknown end the end line is == -1
		/// </remarks>
		public bool IsInside(int line, int column)
		{
			if (IsEmpty)
				return false;
			return line >= BeginLine &&
				(line <= EndLine   || EndLine == -1) &&
				(line != BeginLine || column >= BeginColumn) &&
				(line != EndLine   || column <= EndColumn);
		}

		public bool IsInside(TextLocation location)
		{
			return IsInside(location.Line, location.Column);
		}

		/// <remarks>
		/// Returns true, if the given coordinates (line, column) are in the region.
		/// This method assumes that for an unknown end the end line is == -1
		/// </remarks>
		public bool Contains(int line, int column)
		{
			if (IsEmpty)
				return false;
			return line >= BeginLine &&
				(line <= EndLine   || EndLine == -1) &&
				(line != BeginLine || column >= BeginColumn) &&
				(line != EndLine   || column < EndColumn);
		}

		public bool Contains(TextLocation location)
		{
			return Contains(location.Line, location.Column);
		}

		public bool IntersectsWith (DomRegion region)
		{
			return region.Begin <= End && region.End >= Begin;
		}

		public bool OverlapsWith (DomRegion region)
		{
			var maxBegin = Begin > region.Begin ? Begin : region.Begin;
			var minEnd = End < region.End ? End : region.End;
			return maxBegin < minEnd;
		}

		public override string ToString()
		{
			return string.Format(
				CultureInfo.InvariantCulture,
				"[DomRegion FileName={0}, Begin=({1}, {2}), End=({3}, {4})]",
				FileName, BeginLine, BeginColumn, EndLine, EndColumn);
		}
		
		public override bool Equals(object obj)
		{
			return obj is DomRegion && Equals((DomRegion)obj);
		}
		
		public override int GetHashCode()
		{
			unchecked {
				var hashCode = FileName != null ? FileName.GetHashCode() : 0;
				hashCode ^= BeginColumn + 1100009 * BeginLine + 1200007 * EndLine + 1300021 * EndColumn;
				return hashCode;
			}
		}
		
		public bool Equals(DomRegion other)
		{
			return BeginLine == other.BeginLine && BeginColumn == other.BeginColumn
				&& EndLine == other.EndLine && EndColumn == other.EndColumn
				&& FileName == other.FileName;
		}
		
		public static bool operator ==(DomRegion left, DomRegion right)
		{
			return left.Equals(right);
		}
		
		public static bool operator !=(DomRegion left, DomRegion right)
		{
			return !left.Equals(right);
		}
	}
}
