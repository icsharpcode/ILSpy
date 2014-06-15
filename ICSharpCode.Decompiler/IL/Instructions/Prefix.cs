using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	interface ISupportsMemoryPrefix
	{
		/// <summary>
		/// Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.
		/// </summary>
		byte UnalignedPrefix { get; set; }

		/// <summary>
		/// Gets/Sets whether the memory access is volatile.
		/// </summary>
		bool IsVolatile { get; set; }
	}
}
