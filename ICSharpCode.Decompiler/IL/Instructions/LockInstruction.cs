using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	partial class LockInstruction
	{
		public override void WriteTo(ITextOutput output)
		{
			output.Write("lock (");
			OnExpression.WriteTo(output);
			output.WriteLine(") {");
			output.Indent();
			Body.WriteTo(output);
			output.Unindent();
			output.WriteLine();
			output.Write("}");
		}
	}

	partial class UsingInstruction
	{
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			output.Write("using (");
			ResourceExpression.WriteTo(output);
			output.WriteLine(") {");
			output.Indent();
			Body.WriteTo(output);
			output.Unindent();
			output.WriteLine();
			output.Write("}");
		}
	}
}
