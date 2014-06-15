using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	class CallInstruction(OpCode opCode, MethodReference methodReference) : ILInstruction(opCode)
	{
		public readonly MethodReference Method = methodReference;
		public readonly ILInstruction[] Operands = InitOperands(methodReference);

		static ILInstruction[] InitOperands(MethodReference mr)
		{
			ILInstruction[] operands = new ILInstruction[mr.Parameters.Count];
			for (int i = 0; i < operands.Length; i++) {
				operands[i] = Pop;
			}
			return operands;
		}

		public override bool IsPeeking { get { return Operands.Length > 0 && Operands[0].IsPeeking; } }

		/// <summary>
		/// Gets/Sets whether the call has the 'tail.' prefix.
		/// </summary>
		public bool IsTail;

		/// <summary>
		/// Gets/Sets the type specified in the 'constrained.' prefix.
		/// Returns null if no 'constrained.' prefix exists for this call.
		/// </summary>
		public TypeReference ConstrainedTo;
	}
}
