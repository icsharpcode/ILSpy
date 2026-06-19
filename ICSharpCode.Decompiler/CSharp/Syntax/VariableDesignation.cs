// Copyright (c) 2020 Siegfried Pammer
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable enable

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	[DecompilerAstNode]
	public abstract partial class VariableDesignation : AstNode
	{
	}

	/// <summary>
	/// <c>single_variable_designation ::= identifier</c> (C# grammar §11.2.2)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class SingleVariableDesignation : VariableDesignation
	{
		[Slot("Identifier")]
		public partial string Identifier { get; set; }
	}

	/// <summary>
	/// <c>tuple_designation ::= '(' designations? ')'</c> (C# grammar §11.2.4)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class ParenthesizedVariableDesignation : VariableDesignation
	{
		[Slot("VariableDesignation")]
		public partial AstNodeCollection<VariableDesignation> VariableDesignations { get; }
	}
}
