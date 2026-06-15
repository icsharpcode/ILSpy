// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>array_creation_expression ::= 'new' type '[' expression* ']' array_specifier* array_initializer?</c> (C# grammar §12.8.17.5)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class ArrayCreateExpression : Expression
	{
		public readonly static TokenRole NewKeywordRole = new TokenRole("new");
		public readonly static Role<ArraySpecifier> AdditionalArraySpecifierRole = new Role<ArraySpecifier>("AdditionalArraySpecifier", null);
		public readonly static Role<ArrayInitializerExpression> InitializerRole = new Role<ArrayInitializerExpression>("Initializer", ArrayInitializerExpression.Null);

		[Slot("Roles.Type")]
		public partial AstType Type { get; set; }

		[Slot("Roles.Argument")]
		public partial AstNodeCollection<Expression> Arguments { get; }

		/// <summary>
		/// Gets additional array ranks (those without size info).
		/// Empty for "new int[5,1]"; will contain a single element for "new int[5][]".
		/// </summary>
		[Slot("AdditionalArraySpecifierRole")]
		public partial AstNodeCollection<ArraySpecifier> AdditionalArraySpecifiers { get; }

		[Slot("InitializerRole")]
		public partial ArrayInitializerExpression Initializer { get; set; }
	}
}
