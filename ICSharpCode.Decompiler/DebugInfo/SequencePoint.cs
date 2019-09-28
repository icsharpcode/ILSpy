// Copyright (c) 2018 Daniel Grunwald
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
using System.Diagnostics;

namespace ICSharpCode.Decompiler.DebugInfo
{
	/// <summary>
	/// A sequence point read from a PDB file or produced by the decompiler.
	/// </summary>
	[DebuggerDisplay("SequencePoint IL_{Offset,h}-IL_{EndOffset,h}, {StartLine}:{StartColumn}-{EndLine}:{EndColumn}, IsHidden={IsHidden}")]
	public struct SequencePoint
	{
		/// <summary>
		/// IL start offset.
		/// </summary>
		public int Offset { get; set; }

		/// <summary>
		/// IL end offset.
		/// </summary>
		/// <remarks>
		/// This does not get stored in debug information;
		/// it is used internally to create hidden sequence points
		/// for the IL fragments not covered by any sequence point.
		/// </remarks>
		public int EndOffset { get; set; }

		public int StartLine { get; set; }
		public int StartColumn { get; set; }
		public int EndLine { get; set; }
		public int EndColumn { get; set; }

		public bool IsHidden {
			get { return StartLine == 0xfeefee && StartLine == EndLine; }
		}

		public string DocumentUrl { get; set; }

		internal void SetHidden()
		{
			StartLine = EndLine = 0xfeefee;
		}
	}
}
