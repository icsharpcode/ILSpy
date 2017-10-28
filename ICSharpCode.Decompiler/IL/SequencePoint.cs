using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A sequence point produced by the decompiler.
	/// </summary>
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

		internal void SetHidden()
		{
			StartLine = EndLine = 0xfeefee;
		}
	}
}
