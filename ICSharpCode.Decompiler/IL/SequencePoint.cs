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
		/// IL offset.
		/// </summary>
		public int Offset { get; set; }

		public int StartLine { get; set; }
		public int StartColumn { get; set; }
		public int EndLine { get; set; }
		public int EndColumn { get; set; }

		public bool IsHidden {
			get { return StartLine == 0xfeefee && StartLine == EndLine; }
		}
	}
}
