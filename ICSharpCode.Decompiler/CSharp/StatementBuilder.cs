using ICSharpCode.Decompiler.IL;
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.CSharp
{
	class StatementBuilder
	{
		readonly ExpressionBuilder exprBuilder = new ExpressionBuilder();

		public Statement Convert(ILInstruction inst)
		{
			switch (inst.OpCode) {
				default:
					return new ExpressionStatement(exprBuilder.Convert(inst));
			}
		}
	}
}
