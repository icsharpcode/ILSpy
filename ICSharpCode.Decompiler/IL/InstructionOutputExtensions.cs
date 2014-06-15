using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	static class InstructionOutputExtensions
	{
		public static void Write(this ITextOutput output, OpCode opCode)
		{
			output.Write(opCode.ToString().ToLowerInvariant());
		}

		public static void Write(this ITextOutput output, StackType stackType)
		{
			output.Write(stackType.ToString().ToLowerInvariant());
		}

		public static void Write(this ITextOutput output, PrimitiveType primitiveType)
		{
			output.Write(primitiveType.ToString().ToLowerInvariant());
		}

		public static void WriteSuffix(this ITextOutput output, OverflowMode mode)
		{
			if ((mode & OverflowMode.Ovf) != 0)
				output.Write(".ovf");
			if ((mode & OverflowMode.Un) != 0)
				output.Write(".un");
		}
	}
}
