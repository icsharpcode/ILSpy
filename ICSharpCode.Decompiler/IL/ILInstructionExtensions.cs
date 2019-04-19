using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	internal static class ILInstructionExtensions
	{
		public static T WithILRange<T>(this T target, ILInstruction sourceInstruction) where T : ILInstruction
		{
			target.AddILRange(sourceInstruction);
			return target;
		}

		public static T WithILRange<T>(this T target, Interval range) where T : ILInstruction
		{
			target.AddILRange(range);
			return target;
		}
	}
}
