using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.Decompiler.IL
{
	public static class ILInstructionExtensions
	{
		/// <summary>
		/// Determines whether a block only consists of nop instructions or is empty.
		/// </summary>
		public static bool IsNopBlock(this Block block, ILInstruction ignoreExitPoint = null)
		{
			if (block == null)
				throw new ArgumentNullException(nameof(block));
			return block.Children.Count(i => !(i is Nop) && i != ignoreExitPoint) == 0;
		}
	}
}
