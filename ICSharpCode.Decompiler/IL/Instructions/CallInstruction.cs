using ICSharpCode.Decompiler.Disassembler;
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
		public readonly ILInstruction[] Operands = InitOperands(opCode, methodReference);

		static ILInstruction[] InitOperands(OpCode opCode, MethodReference mr)
		{
			int popCount = mr.Parameters.Count;
			if (opCode != OpCode.NewObj && mr.HasThis)
				popCount++;
			ILInstruction[] operands = new ILInstruction[popCount];
			for (int i = 0; i < operands.Length; i++) {
				operands[i] = Pop;
			}
			return operands;
		}

		public override bool IsPeeking { get { return Operands.Length > 0 && Operands[0].IsPeeking; } }

		public override bool NoResult
		{
			get
			{
				return OpCode != OpCode.NewObj && Method.ReturnType.GetStackType() == StackType.Void;
			}
		}

		public override void TransformChildren(Func<ILInstruction, ILInstruction> transformFunc)
		{
			for (int i = 0; i < Operands.Length; i++) {
				Operands[i] = transformFunc(Operands[i]);
			}
		}

		/// <summary>
		/// Gets/Sets whether the call has the 'tail.' prefix.
		/// </summary>
		public bool IsTail;

		/// <summary>
		/// Gets/Sets the type specified in the 'constrained.' prefix.
		/// Returns null if no 'constrained.' prefix exists for this call.
		/// </summary>
		public TypeReference ConstrainedTo;

		public override void WriteTo(ITextOutput output)
		{
			if (ConstrainedTo != null) {
				output.Write("constrained.");
				ConstrainedTo.WriteTo(output, ILNameSyntax.ShortTypeName);
			}
			if (IsTail)
				output.Write("tail.");
			output.Write(OpCode);
			output.Write(' ');
			Method.WriteTo(output);
			output.Write('(');
			for (int i = 0; i < Operands.Length; i++) {
				if (i > 0)
					output.Write(", ");
				Operands[i].WriteTo(output);
			}
			output.Write(')');
		}
	}
}
