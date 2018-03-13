using System;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{

	/// <summary>
	/// A New line node represents a line break in the text.
	/// </summary>
	public sealed class NewLineNode : AstNode
	{
		public override NodeType NodeType => NodeType.Whitespace;

		const uint newLineMask = 0xfu << AstNodeFlagsUsedBits;
		static readonly UnicodeNewline[] newLineTypes = {
			UnicodeNewline.Unknown,
			UnicodeNewline.LF,
			UnicodeNewline.CRLF,
			UnicodeNewline.CR,
			UnicodeNewline.NEL,
			UnicodeNewline.VT,
			UnicodeNewline.FF,
			UnicodeNewline.LS,
			UnicodeNewline.PS
		};
		
		public UnicodeNewline NewLineType {
			get => newLineTypes[(flags & newLineMask) >> AstNodeFlagsUsedBits];
			set {
				ThrowIfFrozen();
				var pos = Array.IndexOf(newLineTypes, value);
				if (pos < 0)
					pos = 0;
				flags &= ~newLineMask; // clear old newline type
				flags |= (uint)pos << AstNodeFlagsUsedBits;
			}
		}

		TextLocation startLocation;
		public override TextLocation StartLocation => startLocation;

		public override TextLocation EndLocation => new TextLocation (startLocation.Line + 1, 1);

		public NewLineNode() : this (TextLocation.Empty)
		{
		}

		public NewLineNode(TextLocation startLocation)
		{
			this.startLocation = startLocation;
		}

		public sealed override string ToString(CSharpFormattingOptions formattingOptions)
		{
			return NewLine.GetString (NewLineType);
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitNewLine (this);
		}
			
		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitNewLine (this);
		}
		
		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitNewLine (this, data);
		}
		
		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			return other is NewLineNode;
		}
	}
}

