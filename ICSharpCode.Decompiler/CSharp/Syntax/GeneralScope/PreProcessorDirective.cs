// 
// PreProcessorDirective.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#nullable enable

using System.Linq;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public enum PreProcessorDirectiveType : byte
	{
		Invalid = 0,
		Region = 1,
		Endregion = 2,

		If = 3,
		Endif = 4,
		Elif = 5,
		Else = 6,

		Define = 7,
		Undef = 8,
		Error = 9,
		Warning = 10,
		Pragma = 11,
		Line = 12
	}

	/// <summary>
	/// <c>line_directive ::= '#' 'line' ( decimal_digit+ string_literal? | 'default' | 'hidden' )</c> (C# lexical grammar)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class LinePreprocessorDirective : PreProcessorDirective
	{
		public LinePreprocessorDirective(TextLocation startLocation, TextLocation endLocation) : base(PreProcessorDirectiveType.Line, startLocation, endLocation)
		{
		}

		public LinePreprocessorDirective(string? argument = null) : base(PreProcessorDirectiveType.Line, argument)
		{
		}
	}

	/// <summary>
	/// <c>pragma_warning_directive ::= '#' 'pragma' 'warning' ( 'disable' | 'restore' ) expression*</c> (C# lexical grammar)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class PragmaWarningPreprocessorDirective : PreProcessorDirective
	{
		[Slot("Warning")]
		public partial AstNodeCollection<PrimitiveExpression> Warnings { get; }

		public override TextLocation EndLocation {
			get {
				var child = LastChild;
				if (child == null)
					return base.EndLocation;
				return child.EndLocation;
			}
		}

		public PragmaWarningPreprocessorDirective(TextLocation startLocation, TextLocation endLocation) : base(PreProcessorDirectiveType.Pragma, startLocation, endLocation)
		{
		}

		public PragmaWarningPreprocessorDirective(string? argument = null) : base(PreProcessorDirectiveType.Pragma, argument)
		{
		}

		public bool IsDefined(int pragmaWarning)
		{
			return Warnings.Select(w => (int)w.Value).Any(n => n == pragmaWarning);
		}
	}

	/// <summary>
	/// <c>pp_directive ::= '#' pp_kind new_line</c> (C# lexical grammar §6.5.1)
	/// </summary>
	[DecompilerAstNode]
	public partial class PreProcessorDirective : Trivia
	{
		public PreProcessorDirectiveType Type { get; set; }

		public string? Argument { get; set; }

		public PreProcessorDirective(PreProcessorDirectiveType type, TextLocation startLocation, TextLocation endLocation) : base(startLocation, endLocation)
		{
			this.Type = type;
		}

		public PreProcessorDirective(PreProcessorDirectiveType type, string? argument = null)
		{
			this.Type = type;
			this.Argument = argument;
		}

		protected internal override bool DoMatch(AstNode? other, PatternMatching.Match match)
		{
			PreProcessorDirective? o = other as PreProcessorDirective;
			return o != null && Type == o.Type && MatchString(Argument, o.Argument);
		}
	}
}
