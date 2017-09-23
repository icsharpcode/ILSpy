using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	partial class LockInstruction
	{
		protected override InstructionFlags ComputeFlags()
		{
			return Body.Flags | OnExpression.Flags | InstructionFlags.ControlFlow | InstructionFlags.SideEffect;
		}

		public override InstructionFlags DirectFlags => InstructionFlags.ControlFlow | InstructionFlags.SideEffect;

		public override StackType ResultType => StackType.Void;

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			output.Write("lock (");
			OnExpression.WriteTo(output, options);
			output.WriteLine(") ");
			Body.WriteTo(output, options);
		}
	}
}
