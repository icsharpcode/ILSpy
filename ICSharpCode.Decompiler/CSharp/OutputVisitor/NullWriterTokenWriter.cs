using System;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.Decompiler.CSharp.Syntax;

namespace ICSharpCode.Decompiler.CSharp.OutputVisitor
{
	/// <summary>
	/// Just a quick stub class when checksum is being calculated.
	/// </summary>
	public class NullWriterTokenWriter : TokenWriter, ILocatable
	{
		public TextLocation Location => throw new NotImplementedException();

		public int Length => throw new NotImplementedException();

		public override void EndNode(AstNode node)
		{
		}

		public override void Indent()
		{
		}

		public override void NewLine()
		{
		}

		public override void Space()
		{
		}

		public override void StartNode(AstNode node)
		{
		}

		public override void Unindent()
		{
		}

		public override void WriteComment(CommentType commentType, string content)
		{
		}

		public override void WriteIdentifier(Identifier identifier)
		{
		}

		public override void WriteInterpolatedText(string text)
		{
		}

		public override void WriteKeyword(Role role, string keyword)
		{
		}

		public override void WritePreProcessorDirective(PreProcessorDirectiveType type, string argument)
		{
		}

		public override void WritePrimitiveType(string type)
		{
		}

		public override void WritePrimitiveValue(object value, LiteralFormat format = LiteralFormat.None)
		{
		}

		public override void WriteToken(Role role, string token)
		{
		}
	}
}
