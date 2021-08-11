#nullable enable
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	partial class ExpressionTreeCast
	{
		public bool IsChecked { get; set; }

		public ExpressionTreeCast(IType type, ILInstruction argument, bool isChecked)
			: base(OpCode.ExpressionTreeCast, argument)
		{
			this.type = type;
			this.IsChecked = isChecked;
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			if (IsChecked)
				output.Write(".checked");
			output.Write(' ');
			type.WriteTo(output);
			output.Write('(');
			Argument.WriteTo(output, options);
			output.Write(')');
		}
	}
}
