using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	[Flags]
	public enum InstructionFlags
	{
		None = 0,
		MayPop   = 0x01,
		MayPeek  = 0x02,
		/// <summary>
		/// The instruction may throw an exception.
		/// </summary>
		MayThrow = 0x04,
		/// <summary>
		/// The instruction may read from local variables.
		/// </summary>
		MayReadLocals  = 0x08,
		/// <summary>
		/// The instruction may write to local variables.
		/// </summary>
		MayWriteLocals = 0x10,
		/// <summary>
		/// The instruction may exit with a jump or return.
		/// </summary>
		MayJump = 0x20,
		/// <summary>
		/// The instruction may have side effects, such as writing to heap memory,
		/// performing system calls, writing to local variables through pointers, etc.
		/// </summary>
		SideEffects = 0x40,
	}
}
