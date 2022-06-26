// 
// Roles.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin <http://xamarin.com>
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


namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public static class Roles
	{
		public static readonly Role<AstNode> Root = AstNode.RootRole;

		// some pre defined constants for common roles
		public static readonly Role<Identifier> Identifier = new("Identifier", Syntax.Identifier.Null);
		public static readonly Role<BlockStatement> Body = new("Body", BlockStatement.Null);
		public static readonly Role<ParameterDeclaration> Parameter = new("Parameter", null);
		public static readonly Role<Expression> Argument = new("Argument", Syntax.Expression.Null);
		public static readonly Role<AstType> Type = new("Type", AstType.Null);
		public static readonly Role<Expression> Expression = new("Expression", Syntax.Expression.Null);
		public static readonly Role<Expression> TargetExpression = new("Target", Syntax.Expression.Null);
		public readonly static Role<Expression> Condition = new("Condition", Syntax.Expression.Null);
		public static readonly Role<TypeParameterDeclaration> TypeParameter = new("TypeParameter", null);
		public static readonly Role<AstType> TypeArgument = new("TypeArgument", AstType.Null);
		public readonly static Role<Constraint> Constraint = new("Constraint", null);
		public static readonly Role<VariableInitializer> Variable = new("Variable", VariableInitializer.Null);
		public static readonly Role<Statement> EmbeddedStatement = new("EmbeddedStatement", Statement.Null);
		public readonly static Role<EntityDeclaration> TypeMemberRole = new("TypeMember", null);

		public static readonly Role<VariableDesignation> VariableDesignationRole = new("VariableDesignation", VariableDesignation.Null);

		//			public static readonly TokenRole Keyword = new TokenRole ("Keyword", CSharpTokenNode.Null);
		//			public static readonly TokenRole InKeyword = new TokenRole ("InKeyword", CSharpTokenNode.Null);

		// some pre defined constants for most used punctuation
		public static readonly TokenRole LPar = new("(");
		public static readonly TokenRole RPar = new(")");
		public static readonly TokenRole LBracket = new("[");
		public static readonly TokenRole RBracket = new("]");
		public static readonly TokenRole LBrace = new("{");
		public static readonly TokenRole RBrace = new("}");
		public static readonly TokenRole LChevron = new("<");
		public static readonly TokenRole RChevron = new(">");
		public static readonly TokenRole Comma = new(",");
		public static readonly TokenRole Dot = new(".");
		public static readonly TokenRole Semicolon = new(";");
		public static readonly TokenRole Assign = new("=");
		public static readonly TokenRole Colon = new(":");
		public static readonly TokenRole DoubleColon = new("::");
		public static readonly TokenRole Arrow = new("=>");
		public static readonly TokenRole DoubleExclamation = new("!!");
		public static readonly Role<Comment> Comment = new("Comment", null);
		public static readonly Role<PreProcessorDirective> PreProcessorDirective = new("PreProcessorDirective", null);

		public readonly static Role<AstType> BaseType = new("BaseType", AstType.Null);

		public static readonly Role<Attribute> Attribute = new("Attribute", null);
		public static readonly Role<CSharpTokenNode> AttributeTargetRole = new("AttributeTarget", CSharpTokenNode.Null);

		public readonly static TokenRole WhereKeyword = new("where");
		public readonly static Role<SimpleType> ConstraintTypeParameter = new("TypeParameter", SimpleType.Null);
		public readonly static TokenRole DelegateKeyword = new("delegate");
		public static readonly TokenRole ExternKeyword = new("extern");
		public static readonly TokenRole AliasKeyword = new("alias");
		public static readonly TokenRole NamespaceKeyword = new("namespace");

		public static readonly TokenRole EnumKeyword = new("enum");
		public static readonly TokenRole InterfaceKeyword = new("interface");
		public static readonly TokenRole StructKeyword = new("struct");
		public static readonly TokenRole ClassKeyword = new("class");
		public static readonly TokenRole RecordKeyword = new("record");
		public static readonly TokenRole RecordStructKeyword = new("record");

	}
}
