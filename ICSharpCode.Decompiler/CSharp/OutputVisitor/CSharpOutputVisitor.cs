﻿// Copyright (c) 2010-2020 AlphaSierraPapa for the SharpDevelop Team
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

using Attribute = ICSharpCode.Decompiler.CSharp.Syntax.Attribute;

namespace ICSharpCode.Decompiler.CSharp.OutputVisitor
{
	/// <summary>
	/// Outputs the AST.
	/// </summary>
	public class CSharpOutputVisitor : IAstVisitor
	{
		readonly protected TokenWriter writer;
		readonly protected CSharpFormattingOptions policy;
		readonly protected Stack<AstNode> containerStack = new Stack<AstNode>();

		public CSharpOutputVisitor(TextWriter textWriter, CSharpFormattingOptions formattingPolicy)
		{
			if (textWriter == null)
			{
				throw new ArgumentNullException(nameof(textWriter));
			}
			if (formattingPolicy == null)
			{
				throw new ArgumentNullException(nameof(formattingPolicy));
			}
			this.writer = TokenWriter.Create(textWriter, formattingPolicy.IndentationString);
			this.policy = formattingPolicy;
		}

		public CSharpOutputVisitor(TokenWriter writer, CSharpFormattingOptions formattingPolicy)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (formattingPolicy == null)
			{
				throw new ArgumentNullException(nameof(formattingPolicy));
			}
			this.writer = new InsertSpecialsDecorator(new InsertRequiredSpacesDecorator(writer));
			this.policy = formattingPolicy;
		}

		#region StartNode/EndNode
		protected virtual void StartNode(AstNode node)
		{
			// Ensure that nodes are visited in the proper nested order.
			// Jumps to different subtrees are allowed only for the child of a placeholder node.
			Debug.Assert(containerStack.Count == 0 || node.Parent == containerStack.Peek() || containerStack.Peek().NodeType == NodeType.Pattern);
			containerStack.Push(node);
			writer.StartNode(node);
		}

		protected virtual void EndNode(AstNode node)
		{
			Debug.Assert(node == containerStack.Peek());
			containerStack.Pop();
			writer.EndNode(node);
		}
		#endregion

		#region Comma
		/// <summary>
		/// Writes a comma.
		/// </summary>
		/// <param name="nextNode">The next node after the comma.</param>
		/// <param name="noSpaceAfterComma">When set prevents printing a space after comma.</param>
		protected virtual void Comma(AstNode nextNode, bool noSpaceAfterComma = false)
		{
			Space(policy.SpaceBeforeBracketComma);
			// TODO: Comma policy has changed.
			writer.WriteToken(Roles.Comma, ",");
			isAfterSpace = false;
			Space(!noSpaceAfterComma && policy.SpaceAfterBracketComma);
			// TODO: Comma policy has changed.
		}

		/// <summary>
		/// Writes an optional comma, e.g. at the end of an enum declaration or in an array initializer
		/// </summary>
		protected virtual void OptionalComma(AstNode pos)
		{
			// Look if there's a comma after the current node, and insert it if it exists.
			while (pos != null && pos.NodeType == NodeType.Whitespace)
			{
				pos = pos.NextSibling;
			}
			if (pos != null && pos.Role == Roles.Comma)
			{
				Comma(null, noSpaceAfterComma: true);
			}
		}

		/// <summary>
		/// Writes an optional semicolon, e.g. at the end of a type or namespace declaration.
		/// </summary>
		protected virtual void OptionalSemicolon(AstNode pos)
		{
			// Look if there's a semicolon after the current node, and insert it if it exists.
			while (pos != null && pos.NodeType == NodeType.Whitespace)
			{
				pos = pos.PrevSibling;
			}
			if (pos != null && pos.Role == Roles.Semicolon)
			{
				Semicolon();
			}
		}

		protected virtual void WriteCommaSeparatedList(IEnumerable<AstNode> list)
		{
			bool isFirst = true;
			foreach (AstNode node in list)
			{
				if (isFirst)
				{
					isFirst = false;
				}
				else
				{
					Comma(node);
				}
				node.AcceptVisitor(this);
			}
		}

		protected virtual void WriteCommaSeparatedListInParenthesis(IEnumerable<AstNode> list, bool spaceWithin)
		{
			LPar();
			if (list.Any())
			{
				Space(spaceWithin);
				WriteCommaSeparatedList(list);
				Space(spaceWithin);
			}
			RPar();
		}

		protected virtual void WriteCommaSeparatedListInBrackets(IEnumerable<ParameterDeclaration> list, bool spaceWithin)
		{
			WriteToken(Roles.LBracket);
			if (list.Any())
			{
				Space(spaceWithin);
				WriteCommaSeparatedList(list);
				Space(spaceWithin);
			}
			WriteToken(Roles.RBracket);
		}

		protected virtual void WriteCommaSeparatedListInBrackets(IEnumerable<Expression> list)
		{
			WriteToken(Roles.LBracket);
			if (list.Any())
			{
				Space(policy.SpacesWithinBrackets);
				WriteCommaSeparatedList(list);
				Space(policy.SpacesWithinBrackets);
			}
			WriteToken(Roles.RBracket);
		}
		#endregion

		#region Write tokens
		protected bool isAtStartOfLine = true;
		protected bool isAfterSpace;

		/// <summary>
		/// Writes a keyword, and all specials up to
		/// </summary>
		protected virtual void WriteKeyword(TokenRole tokenRole)
		{
			WriteKeyword(tokenRole.Token, tokenRole);
		}

		protected virtual void WriteKeyword(string token, Role tokenRole = null)
		{
			writer.WriteKeyword(tokenRole, token);
			isAtStartOfLine = false;
			isAfterSpace = false;
		}

		protected virtual void WriteIdentifier(Identifier identifier)
		{
			writer.WriteIdentifier(identifier);
			isAtStartOfLine = false;
			isAfterSpace = false;
		}

		protected virtual void WriteIdentifier(string identifier)
		{
			AstType.Create(identifier).AcceptVisitor(this);
			isAtStartOfLine = false;
			isAfterSpace = false;
		}

		protected virtual void WriteToken(TokenRole tokenRole)
		{
			WriteToken(tokenRole.Token, tokenRole);
		}

		protected virtual void WriteToken(string token, Role tokenRole)
		{
			writer.WriteToken(tokenRole, token);
			isAtStartOfLine = false;
			isAfterSpace = false;
		}

		protected virtual void LPar()
		{
			WriteToken(Roles.LPar);
		}

		protected virtual void RPar()
		{
			WriteToken(Roles.RPar);
		}

		/// <summary>
		/// Marks the end of a statement
		/// </summary>
		protected virtual void Semicolon()
		{
			// get the role of the current node
			Role role = containerStack.Peek().Role;
			if (!SkipToken())
			{
				WriteToken(Roles.Semicolon);
				if (!SkipNewLine())
					NewLine();
				else
					Space();
			}

			bool SkipToken()
			{
				return role == ForStatement.InitializerRole
					|| role == ForStatement.IteratorRole
					|| role == UsingStatement.ResourceAcquisitionRole;
			}

			bool SkipNewLine()
			{
				if (containerStack.Peek() is not Accessor accessor)
					return false;
				if (!(role == PropertyDeclaration.GetterRole || role == PropertyDeclaration.SetterRole))
					return false;
				bool isAutoProperty = accessor.Body.IsNull
					&& !accessor.Attributes.Any()
					&& policy.AutoPropertyFormatting == PropertyFormatting.SingleLine;
				return isAutoProperty;
			}
		}

		/// <summary>
		/// Writes a space depending on policy.
		/// </summary>
		protected virtual void Space(bool addSpace = true)
		{
			if (addSpace && !isAfterSpace)
			{
				writer.Space();
				isAfterSpace = true;
			}
		}

		protected virtual void NewLine()
		{
			writer.NewLine();
			isAtStartOfLine = true;
			isAfterSpace = false;
		}

		int GetCallChainLengthLimited(MemberReferenceExpression expr)
		{
			int callChainLength = 0;
			var node = expr;

			while (node.Target is InvocationExpression invocation && invocation.Target is MemberReferenceExpression mre && callChainLength < 4)
			{
				node = mre;
				callChainLength++;
			}
			return callChainLength;
		}

		int ShouldInsertNewLineWhenInMethodCallChain(MemberReferenceExpression expr)
		{
			int callChainLength = GetCallChainLengthLimited(expr);
			if (callChainLength < 3)
				return 0;
			if (expr.GetParent(n => n is Statement || n is LambdaExpression || n is InterpolatedStringContent) is InterpolatedStringContent)
				return 0;
			return callChainLength;
		}

		protected virtual bool InsertNewLineWhenInMethodCallChain(MemberReferenceExpression expr)
		{
			int callChainLength = ShouldInsertNewLineWhenInMethodCallChain(expr);
			if (callChainLength == 0)
				return false;
			if (callChainLength == 3)
				writer.Indent();
			writer.NewLine();

			isAtStartOfLine = true;
			isAfterSpace = false;
			return true;
		}

		protected virtual void OpenBrace(BraceStyle style, bool newLine = true)
		{
			switch (style)
			{
				case BraceStyle.EndOfLine:
				case BraceStyle.BannerStyle:
					if (!isAtStartOfLine)
						Space();
					WriteToken("{", Roles.LBrace);
					break;
				case BraceStyle.EndOfLineWithoutSpace:
					WriteToken("{", Roles.LBrace);
					break;
				case BraceStyle.NextLine:
					if (!isAtStartOfLine)
						NewLine();
					WriteToken("{", Roles.LBrace);
					break;
				case BraceStyle.NextLineShifted:
					NewLine();
					writer.Indent();
					WriteToken("{", Roles.LBrace);
					NewLine();
					return;
				case BraceStyle.NextLineShifted2:
					NewLine();
					writer.Indent();
					WriteToken("{", Roles.LBrace);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			if (newLine)
			{
				writer.Indent();
				NewLine();
			}
		}

		protected virtual void CloseBrace(BraceStyle style, bool unindent = true)
		{
			switch (style)
			{
				case BraceStyle.EndOfLine:
				case BraceStyle.EndOfLineWithoutSpace:
				case BraceStyle.NextLine:
					if (unindent)
						writer.Unindent();
					WriteToken("}", Roles.RBrace);
					break;
				case BraceStyle.BannerStyle:
				case BraceStyle.NextLineShifted:
					WriteToken("}", Roles.RBrace);
					if (unindent)
						writer.Unindent();
					break;
				case BraceStyle.NextLineShifted2:
					if (unindent)
						writer.Unindent();
					WriteToken("}", Roles.RBrace);
					if (unindent)
						writer.Unindent();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#endregion

		#region IsKeyword Test
		static readonly HashSet<string> unconditionalKeywords = new HashSet<string> {
			"abstract", "as", "base", "bool", "break", "byte", "case", "catch",
			"char", "checked", "class", "const", "continue", "decimal", "default", "delegate",
			"do", "double", "else", "enum", "event", "explicit", "extern", "false",
			"finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit",
			"in", "int", "interface", "internal", "is", "lock", "long", "namespace",
			"new", "null", "object", "operator", "out", "override", "params", "private",
			"protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
			"sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw",
			"true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
			"using", "virtual", "void", "volatile", "while"
		};
		static readonly HashSet<string> queryKeywords = new HashSet<string> {
			"from", "where", "join", "on", "equals", "into", "let", "orderby",
			"ascending", "descending", "select", "group", "by"
		};
		static readonly int maxKeywordLength = unconditionalKeywords.Concat(queryKeywords).Max(s => s.Length);

		/// <summary>
		/// Determines whether the specified identifier is a keyword in the given context.
		/// </summary>
		public static bool IsKeyword(string identifier, AstNode context)
		{
			// only 2-10 char lower-case identifiers can be keywords
			if (identifier.Length > maxKeywordLength || identifier.Length < 2 || identifier[0] < 'a')
			{
				return false;
			}
			if (unconditionalKeywords.Contains(identifier))
			{
				return true;
			}
			if (queryKeywords.Contains(identifier))
			{
				return context.Ancestors.Any(ancestor => ancestor is QueryExpression);
			}
			if (identifier == "await")
			{
				foreach (AstNode ancestor in context.Ancestors)
				{
					// with lambdas/anonymous methods,
					if (ancestor is LambdaExpression)
					{
						return ((LambdaExpression)ancestor).IsAsync;
					}
					if (ancestor is AnonymousMethodExpression)
					{
						return ((AnonymousMethodExpression)ancestor).IsAsync;
					}
					if (ancestor is EntityDeclaration)
					{
						return (((EntityDeclaration)ancestor).Modifiers & Modifiers.Async) == Modifiers.Async;
					}
				}
			}
			return false;
		}
		#endregion

		#region Write constructs
		protected virtual void WriteTypeArguments(IEnumerable<AstType> typeArguments)
		{
			if (typeArguments.Any())
			{
				WriteToken(Roles.LChevron);
				WriteCommaSeparatedList(typeArguments);
				WriteToken(Roles.RChevron);
			}
		}

		public virtual void WriteTypeParameters(IEnumerable<TypeParameterDeclaration> typeParameters)
		{
			if (typeParameters.Any())
			{
				WriteToken(Roles.LChevron);
				WriteCommaSeparatedList(typeParameters);
				WriteToken(Roles.RChevron);
			}
		}

		protected virtual void WriteModifiers(IEnumerable<CSharpModifierToken> modifierTokens)
		{
			foreach (CSharpModifierToken modifier in modifierTokens)
			{
				modifier.AcceptVisitor(this);
				Space();
			}
		}

		protected virtual void WriteQualifiedIdentifier(IEnumerable<Identifier> identifiers)
		{
			bool first = true;
			foreach (Identifier ident in identifiers)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					writer.WriteToken(Roles.Dot, ".");
				}
				writer.WriteIdentifier(ident);
			}
		}

		/// <summary>
		/// Writes an embedded statement.
		/// </summary>
		/// <param name="embeddedStatement">The statement to write.</param>
		/// <param name="nlp">Determines whether a trailing newline should be written following a block.
		/// Non-blocks always write a trailing newline.</param>
		/// <remarks>
		/// Blocks may or may not write a leading newline depending on StatementBraceStyle.
		/// Non-blocks always write a leading newline.
		/// </remarks>
		protected virtual void WriteEmbeddedStatement(Statement embeddedStatement, NewLinePlacement nlp = NewLinePlacement.NewLine)
		{
			if (embeddedStatement.IsNull)
			{
				NewLine();
				return;
			}
			BlockStatement block = embeddedStatement as BlockStatement;
			if (block != null)
			{
				WriteBlock(block, policy.StatementBraceStyle);
				if (nlp == NewLinePlacement.SameLine)
				{
					Space(); // if not a trailing newline, then at least a trailing space
				}
				else
				{
					NewLine();
				}
			}
			else
			{
				NewLine();
				writer.Indent();
				embeddedStatement.AcceptVisitor(this);
				writer.Unindent();
			}
		}

		protected virtual void WriteMethodBody(BlockStatement body, BraceStyle style, bool newLine = true)
		{
			if (body.IsNull)
			{
				Semicolon();
			}
			else
			{
				WriteBlock(body, style);
				NewLine();
			}
		}

		protected virtual void WriteAttributes(IEnumerable<AttributeSection> attributes)
		{
			foreach (AttributeSection attr in attributes)
			{
				attr.AcceptVisitor(this);
			}
		}

		protected virtual void WritePrivateImplementationType(AstType privateImplementationType)
		{
			if (!privateImplementationType.IsNull)
			{
				privateImplementationType.AcceptVisitor(this);
				WriteToken(Roles.Dot);
			}
		}

		#endregion

		#region Expressions
		public virtual void VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression)
		{
			StartNode(anonymousMethodExpression);
			if (anonymousMethodExpression.IsAsync)
			{
				WriteKeyword(AnonymousMethodExpression.AsyncModifierRole);
				Space();
			}
			WriteKeyword(AnonymousMethodExpression.DelegateKeywordRole);
			if (anonymousMethodExpression.HasParameterList)
			{
				Space(policy.SpaceBeforeAnonymousMethodParentheses);
				WriteCommaSeparatedListInParenthesis(anonymousMethodExpression.Parameters, policy.SpaceWithinAnonymousMethodParentheses);
			}
			WriteBlock(anonymousMethodExpression.Body, policy.AnonymousMethodBraceStyle);
			EndNode(anonymousMethodExpression);
		}

		public virtual void VisitUndocumentedExpression(UndocumentedExpression undocumentedExpression)
		{
			StartNode(undocumentedExpression);
			switch (undocumentedExpression.UndocumentedExpressionType)
			{
				case UndocumentedExpressionType.ArgList:
				case UndocumentedExpressionType.ArgListAccess:
					WriteKeyword(UndocumentedExpression.ArglistKeywordRole);
					break;
				case UndocumentedExpressionType.MakeRef:
					WriteKeyword(UndocumentedExpression.MakerefKeywordRole);
					break;
				case UndocumentedExpressionType.RefType:
					WriteKeyword(UndocumentedExpression.ReftypeKeywordRole);
					break;
				case UndocumentedExpressionType.RefValue:
					WriteKeyword(UndocumentedExpression.RefvalueKeywordRole);
					break;
			}
			if (undocumentedExpression.UndocumentedExpressionType != UndocumentedExpressionType.ArgListAccess)
			{
				Space(policy.SpaceBeforeMethodCallParentheses);
				WriteCommaSeparatedListInParenthesis(undocumentedExpression.Arguments, policy.SpaceWithinMethodCallParentheses);
			}
			EndNode(undocumentedExpression);
		}

		public virtual void VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression)
		{
			StartNode(arrayCreateExpression);
			WriteKeyword(ArrayCreateExpression.NewKeywordRole);
			arrayCreateExpression.Type.AcceptVisitor(this);
			if (arrayCreateExpression.Arguments.Count > 0)
			{
				WriteCommaSeparatedListInBrackets(arrayCreateExpression.Arguments);
			}
			foreach (var specifier in arrayCreateExpression.AdditionalArraySpecifiers)
			{
				specifier.AcceptVisitor(this);
			}
			arrayCreateExpression.Initializer.AcceptVisitor(this);
			EndNode(arrayCreateExpression);
		}

		public virtual void VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression)
		{
			StartNode(arrayInitializerExpression);
			// "new List<int> { { 1 } }" and "new List<int> { 1 }" are the same semantically.
			// We also use the same AST for both: we always use two nested ArrayInitializerExpressions
			// for collection initializers, even if the user did not write nested brackets.
			// The output visitor will output nested braces only if they are necessary,
			// or if the braces tokens exist in the AST.
			bool bracesAreOptional = arrayInitializerExpression.Elements.Count == 1
				&& IsObjectOrCollectionInitializer(arrayInitializerExpression.Parent)
				&& !CanBeConfusedWithObjectInitializer(arrayInitializerExpression.Elements.Single());
			if (bracesAreOptional && arrayInitializerExpression.LBraceToken.IsNull)
			{
				arrayInitializerExpression.Elements.Single().AcceptVisitor(this);
			}
			else
			{
				PrintInitializerElements(arrayInitializerExpression.Elements);
			}
			EndNode(arrayInitializerExpression);
		}

		protected bool CanBeConfusedWithObjectInitializer(Expression expr)
		{
			// "int a; new List<int> { a = 1 };" is an object initalizers and invalid, but
			// "int a; new List<int> { { a = 1 } };" is a valid collection initializer.
			AssignmentExpression ae = expr as AssignmentExpression;
			return ae != null && ae.Operator == AssignmentOperatorType.Assign;
		}

		protected bool IsObjectOrCollectionInitializer(AstNode node)
		{
			if (!(node is ArrayInitializerExpression))
			{
				return false;
			}
			if (node.Parent is ObjectCreateExpression)
			{
				return node.Role == ObjectCreateExpression.InitializerRole;
			}
			if (node.Parent is NamedExpression)
			{
				return node.Role == Roles.Expression;
			}
			return false;
		}

		protected virtual void PrintInitializerElements(AstNodeCollection<Expression> elements)
		{
			bool wrapAlways = policy.ArrayInitializerWrapping == Wrapping.WrapAlways
				|| (elements.Count > 1 && elements.Any(e => !IsSimpleExpression(e)))
				|| elements.Any(IsComplexExpression);
			bool wrap = wrapAlways
				|| elements.Count > 10;
			OpenBrace(wrap ? policy.ArrayInitializerBraceStyle : BraceStyle.EndOfLine, newLine: wrap);
			if (!wrap)
				Space();
			AstNode last = null;
			foreach (var (idx, node) in elements.WithIndex())
			{
				if (idx > 0)
				{
					Comma(node, noSpaceAfterComma: true);
					if (wrapAlways || idx % 10 == 0)
						NewLine();
					else
						Space();
				}
				last = node;
				node.AcceptVisitor(this);
			}
			if (last != null)
				OptionalComma(last.NextSibling);
			if (wrap)
				NewLine();
			else
				Space();
			CloseBrace(wrap ? policy.ArrayInitializerBraceStyle : BraceStyle.EndOfLine, unindent: wrap);

			bool IsSimpleExpression(Expression ex)
			{
				switch (ex)
				{
					case NullReferenceExpression _:
					case ThisReferenceExpression _:
					case PrimitiveExpression _:
					case IdentifierExpression _:
					case MemberReferenceExpression {
						Target: ThisReferenceExpression
							or IdentifierExpression
							or BaseReferenceExpression
					} _:
						return true;
					default:
						return false;
				}
			}

			bool IsComplexExpression(Expression ex)
			{
				switch (ex)
				{
					case AnonymousMethodExpression _:
					case LambdaExpression _:
					case AnonymousTypeCreateExpression _:
					case ObjectCreateExpression _:
					case NamedExpression _:
						return true;
					default:
						return false;
				}
			}
		}

		public virtual void VisitAsExpression(AsExpression asExpression)
		{
			StartNode(asExpression);
			asExpression.Expression.AcceptVisitor(this);
			Space();
			WriteKeyword(AsExpression.AsKeywordRole);
			Space();
			asExpression.Type.AcceptVisitor(this);
			EndNode(asExpression);
		}

		public virtual void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
		{
			StartNode(assignmentExpression);
			assignmentExpression.Left.AcceptVisitor(this);
			Space(policy.SpaceAroundAssignment);
			WriteToken(AssignmentExpression.GetOperatorRole(assignmentExpression.Operator));
			Space(policy.SpaceAroundAssignment);
			assignmentExpression.Right.AcceptVisitor(this);
			EndNode(assignmentExpression);
		}

		public virtual void VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression)
		{
			StartNode(baseReferenceExpression);
			WriteKeyword("base", baseReferenceExpression.Role);
			EndNode(baseReferenceExpression);
		}

		public virtual void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
		{
			StartNode(binaryOperatorExpression);
			binaryOperatorExpression.Left.AcceptVisitor(this);
			bool spacePolicy;
			switch (binaryOperatorExpression.Operator)
			{
				case BinaryOperatorType.BitwiseAnd:
				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.ExclusiveOr:
					spacePolicy = policy.SpaceAroundBitwiseOperator;
					break;
				case BinaryOperatorType.ConditionalAnd:
				case BinaryOperatorType.ConditionalOr:
					spacePolicy = policy.SpaceAroundLogicalOperator;
					break;
				case BinaryOperatorType.GreaterThan:
				case BinaryOperatorType.GreaterThanOrEqual:
				case BinaryOperatorType.LessThanOrEqual:
				case BinaryOperatorType.LessThan:
					spacePolicy = policy.SpaceAroundRelationalOperator;
					break;
				case BinaryOperatorType.Equality:
				case BinaryOperatorType.InEquality:
					spacePolicy = policy.SpaceAroundEqualityOperator;
					break;
				case BinaryOperatorType.Add:
				case BinaryOperatorType.Subtract:
					spacePolicy = policy.SpaceAroundAdditiveOperator;
					break;
				case BinaryOperatorType.Multiply:
				case BinaryOperatorType.Divide:
				case BinaryOperatorType.Modulus:
					spacePolicy = policy.SpaceAroundMultiplicativeOperator;
					break;
				case BinaryOperatorType.ShiftLeft:
				case BinaryOperatorType.ShiftRight:
				case BinaryOperatorType.UnsignedShiftRight:
					spacePolicy = policy.SpaceAroundShiftOperator;
					break;
				case BinaryOperatorType.NullCoalescing:
				case BinaryOperatorType.IsPattern:
					spacePolicy = true;
					break;
				case BinaryOperatorType.Range:
					spacePolicy = false;
					break;
				default:
					throw new NotSupportedException("Invalid value for BinaryOperatorType");
			}
			Space(spacePolicy);
			TokenRole tokenRole = BinaryOperatorExpression.GetOperatorRole(binaryOperatorExpression.Operator);
			if (tokenRole == BinaryOperatorExpression.IsKeywordRole)
			{
				WriteKeyword(tokenRole);
			}
			else
			{
				WriteToken(tokenRole);
			}
			Space(spacePolicy);
			binaryOperatorExpression.Right.AcceptVisitor(this);
			EndNode(binaryOperatorExpression);
		}

		public virtual void VisitCastExpression(CastExpression castExpression)
		{
			StartNode(castExpression);
			LPar();
			Space(policy.SpacesWithinCastParentheses);
			castExpression.Type.AcceptVisitor(this);
			Space(policy.SpacesWithinCastParentheses);
			RPar();
			Space(policy.SpaceAfterTypecast);
			castExpression.Expression.AcceptVisitor(this);
			EndNode(castExpression);
		}

		public virtual void VisitCheckedExpression(CheckedExpression checkedExpression)
		{
			StartNode(checkedExpression);
			WriteKeyword(CheckedExpression.CheckedKeywordRole);
			LPar();
			Space(policy.SpacesWithinCheckedExpressionParantheses);
			checkedExpression.Expression.AcceptVisitor(this);
			Space(policy.SpacesWithinCheckedExpressionParantheses);
			RPar();
			EndNode(checkedExpression);
		}

		public virtual void VisitConditionalExpression(ConditionalExpression conditionalExpression)
		{
			StartNode(conditionalExpression);
			conditionalExpression.Condition.AcceptVisitor(this);

			Space(policy.SpaceBeforeConditionalOperatorCondition);
			WriteToken(ConditionalExpression.QuestionMarkRole);
			Space(policy.SpaceAfterConditionalOperatorCondition);

			conditionalExpression.TrueExpression.AcceptVisitor(this);

			Space(policy.SpaceBeforeConditionalOperatorSeparator);
			WriteToken(ConditionalExpression.ColonRole);
			Space(policy.SpaceAfterConditionalOperatorSeparator);

			conditionalExpression.FalseExpression.AcceptVisitor(this);

			EndNode(conditionalExpression);
		}

		public virtual void VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression)
		{
			StartNode(defaultValueExpression);

			WriteKeyword(DefaultValueExpression.DefaultKeywordRole);
			LPar();
			Space(policy.SpacesWithinTypeOfParentheses);
			defaultValueExpression.Type.AcceptVisitor(this);
			Space(policy.SpacesWithinTypeOfParentheses);
			RPar();

			EndNode(defaultValueExpression);
		}

		public virtual void VisitDirectionExpression(DirectionExpression directionExpression)
		{
			StartNode(directionExpression);

			switch (directionExpression.FieldDirection)
			{
				case FieldDirection.Out:
					WriteKeyword(DirectionExpression.OutKeywordRole);
					break;
				case FieldDirection.Ref:
					WriteKeyword(DirectionExpression.RefKeywordRole);
					break;
				case FieldDirection.In:
					WriteKeyword(DirectionExpression.InKeywordRole);
					break;
				default:
					throw new NotSupportedException("Invalid value for FieldDirection");
			}
			Space();
			directionExpression.Expression.AcceptVisitor(this);

			EndNode(directionExpression);
		}

		public virtual void VisitDeclarationExpression(DeclarationExpression declarationExpression)
		{
			StartNode(declarationExpression);

			declarationExpression.Type.AcceptVisitor(this);
			Space();
			declarationExpression.Designation.AcceptVisitor(this);

			EndNode(declarationExpression);
		}

		public virtual void VisitRecursivePatternExpression(RecursivePatternExpression recursivePatternExpression)
		{
			StartNode(recursivePatternExpression);

			recursivePatternExpression.Type.AcceptVisitor(this);
			Space();
			if (recursivePatternExpression.IsPositional)
			{
				WriteToken(Roles.LPar);
			}
			else
			{
				WriteToken(Roles.LBrace);
			}
			Space();
			WriteCommaSeparatedList(recursivePatternExpression.SubPatterns);
			Space();
			if (recursivePatternExpression.IsPositional)
			{
				WriteToken(Roles.RPar);
			}
			else
			{
				WriteToken(Roles.RBrace);
			}
			if (!recursivePatternExpression.Designation.IsNull)
			{
				Space();
				recursivePatternExpression.Designation.AcceptVisitor(this);
			}

			EndNode(recursivePatternExpression);
		}

		public virtual void VisitOutVarDeclarationExpression(OutVarDeclarationExpression outVarDeclarationExpression)
		{
			StartNode(outVarDeclarationExpression);

			WriteKeyword(OutVarDeclarationExpression.OutKeywordRole);
			Space();
			outVarDeclarationExpression.Type.AcceptVisitor(this);
			Space();
			outVarDeclarationExpression.Variable.AcceptVisitor(this);

			EndNode(outVarDeclarationExpression);
		}

		public virtual void VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			StartNode(identifierExpression);
			WriteIdentifier(identifierExpression.IdentifierToken);
			WriteTypeArguments(identifierExpression.TypeArguments);
			EndNode(identifierExpression);
		}

		public virtual void VisitIndexerExpression(IndexerExpression indexerExpression)
		{
			StartNode(indexerExpression);
			indexerExpression.Target.AcceptVisitor(this);
			Space(policy.SpaceBeforeMethodCallParentheses);
			WriteCommaSeparatedListInBrackets(indexerExpression.Arguments);
			EndNode(indexerExpression);
		}

		public virtual void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			StartNode(invocationExpression);
			invocationExpression.Target.AcceptVisitor(this);
			Space(policy.SpaceBeforeMethodCallParentheses);
			WriteCommaSeparatedListInParenthesis(invocationExpression.Arguments, policy.SpaceWithinMethodCallParentheses);
			if (!(invocationExpression.Parent is MemberReferenceExpression))
			{
				if (invocationExpression.Target is MemberReferenceExpression mre)
				{
					if (ShouldInsertNewLineWhenInMethodCallChain(mre) >= 3)
						writer.Unindent();
				}
			}
			EndNode(invocationExpression);
		}

		public virtual void VisitIsExpression(IsExpression isExpression)
		{
			StartNode(isExpression);
			isExpression.Expression.AcceptVisitor(this);
			Space();
			WriteKeyword(IsExpression.IsKeywordRole);
			isExpression.Type.AcceptVisitor(this);
			EndNode(isExpression);
		}

		public virtual void VisitLambdaExpression(LambdaExpression lambdaExpression)
		{
			StartNode(lambdaExpression);
			WriteAttributes(lambdaExpression.Attributes);
			if (lambdaExpression.IsAsync)
			{
				WriteKeyword(LambdaExpression.AsyncModifierRole);
				Space();
			}
			if (LambdaNeedsParenthesis(lambdaExpression))
			{
				WriteCommaSeparatedListInParenthesis(lambdaExpression.Parameters, policy.SpaceWithinMethodDeclarationParentheses);
			}
			else
			{
				lambdaExpression.Parameters.Single().AcceptVisitor(this);
			}
			Space();
			WriteToken(Roles.Arrow);
			if (lambdaExpression.Body is BlockStatement)
			{
				WriteBlock((BlockStatement)lambdaExpression.Body, policy.AnonymousMethodBraceStyle);
			}
			else
			{
				Space();
				lambdaExpression.Body.AcceptVisitor(this);
			}
			EndNode(lambdaExpression);
		}

		protected bool LambdaNeedsParenthesis(LambdaExpression lambdaExpression)
		{
			if (lambdaExpression.Parameters.Count != 1)
			{
				return true;
			}
			var p = lambdaExpression.Parameters.Single();
			return !(p.Type.IsNull && p.ParameterModifier == ReferenceKind.None && !p.IsParams);
		}

		public virtual void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
		{
			StartNode(memberReferenceExpression);
			memberReferenceExpression.Target.AcceptVisitor(this);
			bool insertedNewLine = InsertNewLineWhenInMethodCallChain(memberReferenceExpression);
			WriteToken(Roles.Dot);
			WriteIdentifier(memberReferenceExpression.MemberNameToken);
			WriteTypeArguments(memberReferenceExpression.TypeArguments);
			if (insertedNewLine && !(memberReferenceExpression.Parent is InvocationExpression))
			{
				writer.Unindent();
			}
			EndNode(memberReferenceExpression);
		}

		public virtual void VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression)
		{
			StartNode(namedArgumentExpression);
			WriteIdentifier(namedArgumentExpression.NameToken);
			WriteToken(Roles.Colon);
			Space();
			namedArgumentExpression.Expression.AcceptVisitor(this);
			EndNode(namedArgumentExpression);
		}

		public virtual void VisitNamedExpression(NamedExpression namedExpression)
		{
			StartNode(namedExpression);
			WriteIdentifier(namedExpression.NameToken);
			Space();
			WriteToken(Roles.Assign);
			Space();
			namedExpression.Expression.AcceptVisitor(this);
			EndNode(namedExpression);
		}

		public virtual void VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression)
		{
			StartNode(nullReferenceExpression);
			writer.WritePrimitiveValue(null);
			isAfterSpace = false;
			EndNode(nullReferenceExpression);
		}

		public virtual void VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression)
		{
			StartNode(objectCreateExpression);
			WriteKeyword(ObjectCreateExpression.NewKeywordRole);
			objectCreateExpression.Type.AcceptVisitor(this);
			bool useParenthesis = objectCreateExpression.Arguments.Any() || objectCreateExpression.Initializer.IsNull;
			// also use parenthesis if there is an '(' token
			if (!objectCreateExpression.LParToken.IsNull)
			{
				useParenthesis = true;
			}
			if (useParenthesis)
			{
				Space(policy.SpaceBeforeMethodCallParentheses);
				WriteCommaSeparatedListInParenthesis(objectCreateExpression.Arguments, policy.SpaceWithinMethodCallParentheses);
			}
			objectCreateExpression.Initializer.AcceptVisitor(this);
			EndNode(objectCreateExpression);
		}

		public virtual void VisitAnonymousTypeCreateExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression)
		{
			StartNode(anonymousTypeCreateExpression);
			WriteKeyword(AnonymousTypeCreateExpression.NewKeywordRole);
			PrintInitializerElements(anonymousTypeCreateExpression.Initializers);
			EndNode(anonymousTypeCreateExpression);
		}

		public virtual void VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression)
		{
			StartNode(parenthesizedExpression);
			LPar();
			Space(policy.SpacesWithinParentheses);
			parenthesizedExpression.Expression.AcceptVisitor(this);
			Space(policy.SpacesWithinParentheses);
			RPar();
			EndNode(parenthesizedExpression);
		}

		public virtual void VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression)
		{
			StartNode(pointerReferenceExpression);
			pointerReferenceExpression.Target.AcceptVisitor(this);
			WriteToken(PointerReferenceExpression.ArrowRole);
			WriteIdentifier(pointerReferenceExpression.MemberNameToken);
			WriteTypeArguments(pointerReferenceExpression.TypeArguments);
			EndNode(pointerReferenceExpression);
		}

		#region VisitPrimitiveExpression
		public virtual void VisitPrimitiveExpression(PrimitiveExpression primitiveExpression)
		{
			StartNode(primitiveExpression);
			writer.WritePrimitiveValue(primitiveExpression.Value, primitiveExpression.Format);
			isAfterSpace = false;
			EndNode(primitiveExpression);
		}

		public virtual void VisitInterpolatedStringExpression(InterpolatedStringExpression interpolatedStringExpression)
		{
			StartNode(interpolatedStringExpression);

			writer.WriteToken(InterpolatedStringExpression.OpenQuote, "$\"");
			foreach (var element in interpolatedStringExpression.Content)
			{
				element.AcceptVisitor(this);
			}
			writer.WriteToken(InterpolatedStringExpression.CloseQuote, "\"");
			isAfterSpace = false;

			EndNode(interpolatedStringExpression);
		}

		public virtual void VisitInterpolation(Interpolation interpolation)
		{
			StartNode(interpolation);

			writer.WriteToken(Interpolation.LBrace, "{");
			interpolation.Expression.AcceptVisitor(this);
			if (interpolation.Alignment != 0)
			{
				writer.WriteToken(Roles.Comma, ",");
				writer.WritePrimitiveValue(interpolation.Alignment);
			}
			if (interpolation.Suffix != null)
			{
				writer.WriteToken(Roles.Colon, ":");
				writer.WriteInterpolatedText(interpolation.Suffix);
			}
			writer.WriteToken(Interpolation.RBrace, "}");

			EndNode(interpolation);
		}

		public virtual void VisitInterpolatedStringText(InterpolatedStringText interpolatedStringText)
		{
			StartNode(interpolatedStringText);
			writer.WriteInterpolatedText(interpolatedStringText.Text);
			EndNode(interpolatedStringText);
		}
		#endregion

		public virtual void VisitSizeOfExpression(SizeOfExpression sizeOfExpression)
		{
			StartNode(sizeOfExpression);

			WriteKeyword(SizeOfExpression.SizeofKeywordRole);
			LPar();
			Space(policy.SpacesWithinSizeOfParentheses);
			sizeOfExpression.Type.AcceptVisitor(this);
			Space(policy.SpacesWithinSizeOfParentheses);
			RPar();

			EndNode(sizeOfExpression);
		}

		public virtual void VisitStackAllocExpression(StackAllocExpression stackAllocExpression)
		{
			StartNode(stackAllocExpression);
			WriteKeyword(StackAllocExpression.StackallocKeywordRole);
			stackAllocExpression.Type.AcceptVisitor(this);
			WriteCommaSeparatedListInBrackets(new[] { stackAllocExpression.CountExpression });
			stackAllocExpression.Initializer.AcceptVisitor(this);
			EndNode(stackAllocExpression);
		}

		public virtual void VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression)
		{
			StartNode(thisReferenceExpression);
			WriteKeyword("this", thisReferenceExpression.Role);
			EndNode(thisReferenceExpression);
		}

		public virtual void VisitThrowExpression(ThrowExpression throwExpression)
		{
			StartNode(throwExpression);
			WriteKeyword(ThrowExpression.ThrowKeywordRole);
			Space();
			throwExpression.Expression.AcceptVisitor(this);
			EndNode(throwExpression);
		}

		public virtual void VisitTupleExpression(TupleExpression tupleExpression)
		{
			Debug.Assert(tupleExpression.Elements.Count >= 2);
			StartNode(tupleExpression);
			LPar();
			WriteCommaSeparatedList(tupleExpression.Elements);
			RPar();
			EndNode(tupleExpression);
		}

		public virtual void VisitTypeOfExpression(TypeOfExpression typeOfExpression)
		{
			StartNode(typeOfExpression);

			WriteKeyword(TypeOfExpression.TypeofKeywordRole);
			LPar();
			Space(policy.SpacesWithinTypeOfParentheses);
			typeOfExpression.Type.AcceptVisitor(this);
			Space(policy.SpacesWithinTypeOfParentheses);
			RPar();

			EndNode(typeOfExpression);
		}

		public virtual void VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression)
		{
			StartNode(typeReferenceExpression);
			typeReferenceExpression.Type.AcceptVisitor(this);
			EndNode(typeReferenceExpression);
		}

		public virtual void VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression)
		{
			StartNode(unaryOperatorExpression);
			UnaryOperatorType opType = unaryOperatorExpression.Operator;
			var opSymbol = UnaryOperatorExpression.GetOperatorRole(opType);
			if (opType is UnaryOperatorType.Await or UnaryOperatorType.PatternNot)
			{
				WriteKeyword(opSymbol);
				Space();
			}
			else if (!IsPostfixOperator(opType) && opSymbol != null)
			{
				WriteToken(opSymbol);
			}
			unaryOperatorExpression.Expression.AcceptVisitor(this);
			if (IsPostfixOperator(opType))
			{
				WriteToken(opSymbol);
			}
			EndNode(unaryOperatorExpression);
		}

		static bool IsPostfixOperator(UnaryOperatorType op)
		{
			return op == UnaryOperatorType.PostIncrement
				|| op == UnaryOperatorType.PostDecrement
				|| op == UnaryOperatorType.NullConditional
				|| op == UnaryOperatorType.SuppressNullableWarning;
		}

		public virtual void VisitUncheckedExpression(UncheckedExpression uncheckedExpression)
		{
			StartNode(uncheckedExpression);
			WriteKeyword(UncheckedExpression.UncheckedKeywordRole);
			LPar();
			Space(policy.SpacesWithinCheckedExpressionParantheses);
			uncheckedExpression.Expression.AcceptVisitor(this);
			Space(policy.SpacesWithinCheckedExpressionParantheses);
			RPar();
			EndNode(uncheckedExpression);
		}

		public virtual void VisitWithInitializerExpression(WithInitializerExpression withInitializerExpression)
		{
			StartNode(withInitializerExpression);
			withInitializerExpression.Expression.AcceptVisitor(this);
			WriteKeyword("with", WithInitializerExpression.WithKeywordRole);
			withInitializerExpression.Initializer.AcceptVisitor(this);
			EndNode(withInitializerExpression);
		}

		#endregion

		#region Query Expressions
		public virtual void VisitQueryExpression(QueryExpression queryExpression)
		{
			StartNode(queryExpression);
			if (queryExpression.Role != QueryContinuationClause.PrecedingQueryRole)
				writer.Indent();
			bool first = true;
			foreach (var clause in queryExpression.Clauses)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					if (!(clause is QueryContinuationClause))
					{
						NewLine();
					}
				}
				clause.AcceptVisitor(this);
			}
			if (queryExpression.Role != QueryContinuationClause.PrecedingQueryRole)
				writer.Unindent();
			EndNode(queryExpression);
		}

		public virtual void VisitQueryContinuationClause(QueryContinuationClause queryContinuationClause)
		{
			StartNode(queryContinuationClause);
			queryContinuationClause.PrecedingQuery.AcceptVisitor(this);
			Space();
			WriteKeyword(QueryContinuationClause.IntoKeywordRole);
			Space();
			WriteIdentifier(queryContinuationClause.IdentifierToken);
			EndNode(queryContinuationClause);
		}

		public virtual void VisitQueryFromClause(QueryFromClause queryFromClause)
		{
			StartNode(queryFromClause);
			WriteKeyword(QueryFromClause.FromKeywordRole);
			queryFromClause.Type.AcceptVisitor(this);
			Space();
			WriteIdentifier(queryFromClause.IdentifierToken);
			Space();
			WriteKeyword(QueryFromClause.InKeywordRole);
			Space();
			queryFromClause.Expression.AcceptVisitor(this);
			EndNode(queryFromClause);
		}

		public virtual void VisitQueryLetClause(QueryLetClause queryLetClause)
		{
			StartNode(queryLetClause);
			WriteKeyword(QueryLetClause.LetKeywordRole);
			Space();
			WriteIdentifier(queryLetClause.IdentifierToken);
			Space(policy.SpaceAroundAssignment);
			WriteToken(Roles.Assign);
			Space(policy.SpaceAroundAssignment);
			queryLetClause.Expression.AcceptVisitor(this);
			EndNode(queryLetClause);
		}

		public virtual void VisitQueryWhereClause(QueryWhereClause queryWhereClause)
		{
			StartNode(queryWhereClause);
			WriteKeyword(QueryWhereClause.WhereKeywordRole);
			Space();
			queryWhereClause.Condition.AcceptVisitor(this);
			EndNode(queryWhereClause);
		}

		public virtual void VisitQueryJoinClause(QueryJoinClause queryJoinClause)
		{
			StartNode(queryJoinClause);
			WriteKeyword(QueryJoinClause.JoinKeywordRole);
			queryJoinClause.Type.AcceptVisitor(this);
			Space();
			WriteIdentifier(queryJoinClause.JoinIdentifierToken);
			Space();
			WriteKeyword(QueryJoinClause.InKeywordRole);
			Space();
			queryJoinClause.InExpression.AcceptVisitor(this);
			Space();
			WriteKeyword(QueryJoinClause.OnKeywordRole);
			Space();
			queryJoinClause.OnExpression.AcceptVisitor(this);
			Space();
			WriteKeyword(QueryJoinClause.EqualsKeywordRole);
			Space();
			queryJoinClause.EqualsExpression.AcceptVisitor(this);
			if (queryJoinClause.IsGroupJoin)
			{
				Space();
				WriteKeyword(QueryJoinClause.IntoKeywordRole);
				WriteIdentifier(queryJoinClause.IntoIdentifierToken);
			}
			EndNode(queryJoinClause);
		}

		public virtual void VisitQueryOrderClause(QueryOrderClause queryOrderClause)
		{
			StartNode(queryOrderClause);
			WriteKeyword(QueryOrderClause.OrderbyKeywordRole);
			Space();
			WriteCommaSeparatedList(queryOrderClause.Orderings);
			EndNode(queryOrderClause);
		}

		public virtual void VisitQueryOrdering(QueryOrdering queryOrdering)
		{
			StartNode(queryOrdering);
			queryOrdering.Expression.AcceptVisitor(this);
			switch (queryOrdering.Direction)
			{
				case QueryOrderingDirection.Ascending:
					Space();
					WriteKeyword(QueryOrdering.AscendingKeywordRole);
					break;
				case QueryOrderingDirection.Descending:
					Space();
					WriteKeyword(QueryOrdering.DescendingKeywordRole);
					break;
			}
			EndNode(queryOrdering);
		}

		public virtual void VisitQuerySelectClause(QuerySelectClause querySelectClause)
		{
			StartNode(querySelectClause);
			WriteKeyword(QuerySelectClause.SelectKeywordRole);
			Space();
			querySelectClause.Expression.AcceptVisitor(this);
			EndNode(querySelectClause);
		}

		public virtual void VisitQueryGroupClause(QueryGroupClause queryGroupClause)
		{
			StartNode(queryGroupClause);
			WriteKeyword(QueryGroupClause.GroupKeywordRole);
			Space();
			queryGroupClause.Projection.AcceptVisitor(this);
			Space();
			WriteKeyword(QueryGroupClause.ByKeywordRole);
			Space();
			queryGroupClause.Key.AcceptVisitor(this);
			EndNode(queryGroupClause);
		}

		#endregion

		#region GeneralScope
		public virtual void VisitAttribute(Attribute attribute)
		{
			StartNode(attribute);
			attribute.Type.AcceptVisitor(this);
			if (attribute.Arguments.Count != 0 || attribute.HasArgumentList)
			{
				Space(policy.SpaceBeforeMethodCallParentheses);
				WriteCommaSeparatedListInParenthesis(attribute.Arguments, policy.SpaceWithinMethodCallParentheses);
			}
			EndNode(attribute);
		}

		public virtual void VisitAttributeSection(AttributeSection attributeSection)
		{
			StartNode(attributeSection);
			WriteToken(Roles.LBracket);
			if (!string.IsNullOrEmpty(attributeSection.AttributeTarget))
			{
				WriteKeyword(attributeSection.AttributeTarget, Roles.Identifier);
				WriteToken(Roles.Colon);
				Space();
			}
			WriteCommaSeparatedList(attributeSection.Attributes);
			WriteToken(Roles.RBracket);
			switch (attributeSection.Parent)
			{
				case ParameterDeclaration _:
					if (attributeSection.NextSibling is AttributeSection)
						Space(policy.SpaceBetweenParameterAttributeSections);
					else
						Space();
					break;
				case TypeParameterDeclaration _:
				case ComposedType _:
				case LambdaExpression _:
					Space();
					break;
				default:
					NewLine();
					break;
			}
			EndNode(attributeSection);
		}

		public virtual void VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration)
		{
			StartNode(delegateDeclaration);
			WriteAttributes(delegateDeclaration.Attributes);
			WriteModifiers(delegateDeclaration.ModifierTokens);
			WriteKeyword(Roles.DelegateKeyword);
			delegateDeclaration.ReturnType.AcceptVisitor(this);
			Space();
			WriteIdentifier(delegateDeclaration.NameToken);
			WriteTypeParameters(delegateDeclaration.TypeParameters);
			Space(policy.SpaceBeforeDelegateDeclarationParentheses);
			WriteCommaSeparatedListInParenthesis(delegateDeclaration.Parameters, policy.SpaceWithinMethodDeclarationParentheses);
			foreach (Constraint constraint in delegateDeclaration.Constraints)
			{
				constraint.AcceptVisitor(this);
			}
			Semicolon();
			EndNode(delegateDeclaration);
		}

		public virtual void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
		{
			StartNode(namespaceDeclaration);
			WriteKeyword(Roles.NamespaceKeyword);
			namespaceDeclaration.NamespaceName.AcceptVisitor(this);
			if (namespaceDeclaration.IsFileScoped)
			{
				Semicolon();
				NewLine();
			}
			else
			{
				OpenBrace(policy.NamespaceBraceStyle);
			}
			foreach (var member in namespaceDeclaration.Members)
			{
				member.AcceptVisitor(this);
				MaybeNewLinesAfterUsings(member);
			}
			if (!namespaceDeclaration.IsFileScoped)
			{
				CloseBrace(policy.NamespaceBraceStyle);
				OptionalSemicolon(namespaceDeclaration.LastChild);
				NewLine();
			}
			EndNode(namespaceDeclaration);
		}

		public virtual void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
		{
			StartNode(typeDeclaration);
			WriteAttributes(typeDeclaration.Attributes);
			WriteModifiers(typeDeclaration.ModifierTokens);
			BraceStyle braceStyle;
			switch (typeDeclaration.ClassType)
			{
				case ClassType.Enum:
					WriteKeyword(Roles.EnumKeyword);
					braceStyle = policy.EnumBraceStyle;
					break;
				case ClassType.Interface:
					WriteKeyword(Roles.InterfaceKeyword);
					braceStyle = policy.InterfaceBraceStyle;
					break;
				case ClassType.Struct:
					WriteKeyword(Roles.StructKeyword);
					braceStyle = policy.StructBraceStyle;
					break;
				case ClassType.RecordClass:
					WriteKeyword(Roles.RecordKeyword);
					braceStyle = policy.ClassBraceStyle;
					break;
				case ClassType.RecordStruct:
					WriteKeyword(Roles.RecordStructKeyword);
					WriteKeyword(Roles.StructKeyword);
					braceStyle = policy.StructBraceStyle;
					break;
				default:
					WriteKeyword(Roles.ClassKeyword);
					braceStyle = policy.ClassBraceStyle;
					break;
			}
			WriteIdentifier(typeDeclaration.NameToken);
			WriteTypeParameters(typeDeclaration.TypeParameters);
			if (typeDeclaration.PrimaryConstructorParameters.Count > 0)
			{
				Space(policy.SpaceBeforeMethodDeclarationParentheses);
				WriteCommaSeparatedListInParenthesis(typeDeclaration.PrimaryConstructorParameters, policy.SpaceWithinMethodDeclarationParentheses);
			}
			if (typeDeclaration.BaseTypes.Any())
			{
				Space();
				WriteToken(Roles.Colon);
				Space();
				WriteCommaSeparatedList(typeDeclaration.BaseTypes);
			}
			foreach (Constraint constraint in typeDeclaration.Constraints)
			{
				constraint.AcceptVisitor(this);
			}
			if (typeDeclaration.ClassType is (ClassType.RecordClass or ClassType.RecordStruct) && typeDeclaration.Members.Count == 0)
			{
				Semicolon();
			}
			else
			{
				OpenBrace(braceStyle);
				if (typeDeclaration.ClassType == ClassType.Enum)
				{
					bool first = true;
					AstNode last = null;
					foreach (var member in typeDeclaration.Members)
					{
						if (first)
						{
							first = false;
						}
						else
						{
							Comma(member, noSpaceAfterComma: true);
							NewLine();
						}
						last = member;
						member.AcceptVisitor(this);
					}
					if (last != null)
						OptionalComma(last.NextSibling);
					NewLine();
				}
				else
				{
					bool first = true;
					foreach (var member in typeDeclaration.Members)
					{
						if (!first)
						{
							for (int i = 0; i < policy.MinimumBlankLinesBetweenMembers; i++)
								NewLine();
						}
						first = false;
						member.AcceptVisitor(this);
					}
				}
				CloseBrace(braceStyle);
				OptionalSemicolon(typeDeclaration.LastChild);
				NewLine();
			}
			EndNode(typeDeclaration);
		}

		public virtual void VisitUsingAliasDeclaration(UsingAliasDeclaration usingAliasDeclaration)
		{
			StartNode(usingAliasDeclaration);
			WriteKeyword(UsingAliasDeclaration.UsingKeywordRole);
			WriteIdentifier(usingAliasDeclaration.GetChildByRole(UsingAliasDeclaration.AliasRole));
			Space(policy.SpaceAroundEqualityOperator);
			WriteToken(Roles.Assign);
			Space(policy.SpaceAroundEqualityOperator);
			usingAliasDeclaration.Import.AcceptVisitor(this);
			Semicolon();
			EndNode(usingAliasDeclaration);
		}

		public virtual void VisitUsingDeclaration(UsingDeclaration usingDeclaration)
		{
			StartNode(usingDeclaration);
			WriteKeyword(UsingDeclaration.UsingKeywordRole);
			usingDeclaration.Import.AcceptVisitor(this);
			Semicolon();
			EndNode(usingDeclaration);
		}

		public virtual void VisitExternAliasDeclaration(ExternAliasDeclaration externAliasDeclaration)
		{
			StartNode(externAliasDeclaration);
			WriteKeyword(Roles.ExternKeyword);
			Space();
			WriteKeyword(Roles.AliasKeyword);
			Space();
			WriteIdentifier(externAliasDeclaration.NameToken);
			Semicolon();
			EndNode(externAliasDeclaration);
		}

		#endregion

		#region Statements
		public virtual void VisitBlockStatement(BlockStatement blockStatement)
		{
			WriteBlock(blockStatement, policy.StatementBraceStyle);
			NewLine();
		}

		/// <summary>
		/// Writes a block statement.
		/// Similar to VisitBlockStatement() except that:
		/// 1) it allows customizing the BraceStyle
		/// 2) it does not write a trailing newline after the '}' (this job is left to the caller)
		/// </summary>
		protected virtual void WriteBlock(BlockStatement blockStatement, BraceStyle style)
		{
			StartNode(blockStatement);
			OpenBrace(style);
			foreach (var node in blockStatement.Statements)
			{
				node.AcceptVisitor(this);
			}
			CloseBrace(style);
			EndNode(blockStatement);
		}

		public virtual void VisitBreakStatement(BreakStatement breakStatement)
		{
			StartNode(breakStatement);
			WriteKeyword("break", BreakStatement.BreakKeywordRole);
			Semicolon();
			EndNode(breakStatement);
		}

		public virtual void VisitCheckedStatement(CheckedStatement checkedStatement)
		{
			StartNode(checkedStatement);
			WriteKeyword(CheckedStatement.CheckedKeywordRole);
			checkedStatement.Body.AcceptVisitor(this);
			EndNode(checkedStatement);
		}

		public virtual void VisitContinueStatement(ContinueStatement continueStatement)
		{
			StartNode(continueStatement);
			WriteKeyword("continue", ContinueStatement.ContinueKeywordRole);
			Semicolon();
			EndNode(continueStatement);
		}

		public virtual void VisitDoWhileStatement(DoWhileStatement doWhileStatement)
		{
			StartNode(doWhileStatement);
			WriteKeyword(DoWhileStatement.DoKeywordRole);
			WriteEmbeddedStatement(doWhileStatement.EmbeddedStatement, policy.WhileNewLinePlacement);
			WriteKeyword(DoWhileStatement.WhileKeywordRole);
			Space(policy.SpaceBeforeWhileParentheses);
			LPar();
			Space(policy.SpacesWithinWhileParentheses);
			doWhileStatement.Condition.AcceptVisitor(this);
			Space(policy.SpacesWithinWhileParentheses);
			RPar();
			Semicolon();
			EndNode(doWhileStatement);
		}

		public virtual void VisitEmptyStatement(EmptyStatement emptyStatement)
		{
			StartNode(emptyStatement);
			Semicolon();
			EndNode(emptyStatement);
		}

		public virtual void VisitExpressionStatement(ExpressionStatement expressionStatement)
		{
			StartNode(expressionStatement);
			expressionStatement.Expression.AcceptVisitor(this);
			Semicolon();
			EndNode(expressionStatement);
		}

		public virtual void VisitFixedStatement(FixedStatement fixedStatement)
		{
			StartNode(fixedStatement);
			WriteKeyword(FixedStatement.FixedKeywordRole);
			Space(policy.SpaceBeforeUsingParentheses);
			LPar();
			Space(policy.SpacesWithinUsingParentheses);
			fixedStatement.Type.AcceptVisitor(this);
			Space();
			WriteCommaSeparatedList(fixedStatement.Variables);
			Space(policy.SpacesWithinUsingParentheses);
			RPar();
			WriteEmbeddedStatement(fixedStatement.EmbeddedStatement);
			EndNode(fixedStatement);
		}

		public virtual void VisitForeachStatement(ForeachStatement foreachStatement)
		{
			StartNode(foreachStatement);
			if (foreachStatement.IsAsync)
				WriteKeyword(ForeachStatement.AwaitRole);
			WriteKeyword(ForeachStatement.ForeachKeywordRole);
			Space(policy.SpaceBeforeForeachParentheses);
			LPar();
			Space(policy.SpacesWithinForeachParentheses);
			foreachStatement.VariableType.AcceptVisitor(this);
			Space();
			foreachStatement.VariableDesignation.AcceptVisitor(this);
			Space();
			WriteKeyword(ForeachStatement.InKeywordRole);
			Space();
			foreachStatement.InExpression.AcceptVisitor(this);
			Space(policy.SpacesWithinForeachParentheses);
			RPar();
			WriteEmbeddedStatement(foreachStatement.EmbeddedStatement);
			EndNode(foreachStatement);
		}

		public virtual void VisitForStatement(ForStatement forStatement)
		{
			StartNode(forStatement);
			WriteKeyword(ForStatement.ForKeywordRole);
			Space(policy.SpaceBeforeForParentheses);
			LPar();
			Space(policy.SpacesWithinForParentheses);

			WriteCommaSeparatedList(forStatement.Initializers);
			Space(policy.SpaceBeforeForSemicolon);
			WriteToken(Roles.Semicolon);
			Space(policy.SpaceAfterForSemicolon);

			forStatement.Condition.AcceptVisitor(this);
			Space(policy.SpaceBeforeForSemicolon);
			WriteToken(Roles.Semicolon);
			if (forStatement.Iterators.Any())
			{
				Space(policy.SpaceAfterForSemicolon);
				WriteCommaSeparatedList(forStatement.Iterators);
			}

			Space(policy.SpacesWithinForParentheses);
			RPar();
			WriteEmbeddedStatement(forStatement.EmbeddedStatement);
			EndNode(forStatement);
		}

		public virtual void VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement)
		{
			StartNode(gotoCaseStatement);
			WriteKeyword(GotoCaseStatement.GotoKeywordRole);
			WriteKeyword(GotoCaseStatement.CaseKeywordRole);
			Space();
			gotoCaseStatement.LabelExpression.AcceptVisitor(this);
			Semicolon();
			EndNode(gotoCaseStatement);
		}

		public virtual void VisitGotoDefaultStatement(GotoDefaultStatement gotoDefaultStatement)
		{
			StartNode(gotoDefaultStatement);
			WriteKeyword(GotoDefaultStatement.GotoKeywordRole);
			WriteKeyword(GotoDefaultStatement.DefaultKeywordRole);
			Semicolon();
			EndNode(gotoDefaultStatement);
		}

		public virtual void VisitGotoStatement(GotoStatement gotoStatement)
		{
			StartNode(gotoStatement);
			WriteKeyword(GotoStatement.GotoKeywordRole);
			WriteIdentifier(gotoStatement.GetChildByRole(Roles.Identifier));
			Semicolon();
			EndNode(gotoStatement);
		}

		public virtual void VisitIfElseStatement(IfElseStatement ifElseStatement)
		{
			StartNode(ifElseStatement);
			WriteKeyword(IfElseStatement.IfKeywordRole);
			Space(policy.SpaceBeforeIfParentheses);
			LPar();
			Space(policy.SpacesWithinIfParentheses);
			ifElseStatement.Condition.AcceptVisitor(this);
			Space(policy.SpacesWithinIfParentheses);
			RPar();

			if (ifElseStatement.FalseStatement.IsNull)
			{
				WriteEmbeddedStatement(ifElseStatement.TrueStatement);
			}
			else
			{
				WriteEmbeddedStatement(ifElseStatement.TrueStatement, policy.ElseNewLinePlacement);
				WriteKeyword(IfElseStatement.ElseKeywordRole);
				if (ifElseStatement.FalseStatement is IfElseStatement)
				{
					// don't put newline between 'else' and 'if'
					ifElseStatement.FalseStatement.AcceptVisitor(this);
				}
				else
				{
					WriteEmbeddedStatement(ifElseStatement.FalseStatement);
				}
			}
			EndNode(ifElseStatement);
		}

		public virtual void VisitLabelStatement(LabelStatement labelStatement)
		{
			StartNode(labelStatement);
			WriteIdentifier(labelStatement.GetChildByRole(Roles.Identifier));
			WriteToken(Roles.Colon);
			bool foundLabelledStatement = false;
			for (AstNode tmp = labelStatement.NextSibling; tmp != null; tmp = tmp.NextSibling)
			{
				if (tmp.Role == labelStatement.Role)
				{
					foundLabelledStatement = true;
				}
			}
			if (!foundLabelledStatement)
			{
				// introduce an EmptyStatement so that the output becomes syntactically valid
				WriteToken(Roles.Semicolon);
			}
			NewLine();
			EndNode(labelStatement);
		}

		public virtual void VisitLockStatement(LockStatement lockStatement)
		{
			StartNode(lockStatement);
			WriteKeyword(LockStatement.LockKeywordRole);
			Space(policy.SpaceBeforeLockParentheses);
			LPar();
			Space(policy.SpacesWithinLockParentheses);
			lockStatement.Expression.AcceptVisitor(this);
			Space(policy.SpacesWithinLockParentheses);
			RPar();
			WriteEmbeddedStatement(lockStatement.EmbeddedStatement);
			EndNode(lockStatement);
		}

		public virtual void VisitReturnStatement(ReturnStatement returnStatement)
		{
			StartNode(returnStatement);
			WriteKeyword(ReturnStatement.ReturnKeywordRole);
			if (!returnStatement.Expression.IsNull)
			{
				Space();
				returnStatement.Expression.AcceptVisitor(this);
			}
			Semicolon();
			EndNode(returnStatement);
		}

		public virtual void VisitSwitchStatement(SwitchStatement switchStatement)
		{
			StartNode(switchStatement);
			WriteKeyword(SwitchStatement.SwitchKeywordRole);
			Space(policy.SpaceBeforeSwitchParentheses);
			LPar();
			Space(policy.SpacesWithinSwitchParentheses);
			switchStatement.Expression.AcceptVisitor(this);
			Space(policy.SpacesWithinSwitchParentheses);
			RPar();
			OpenBrace(policy.StatementBraceStyle);
			if (!policy.IndentSwitchBody)
			{
				writer.Unindent();
			}

			foreach (var section in switchStatement.SwitchSections)
			{
				section.AcceptVisitor(this);
			}

			if (!policy.IndentSwitchBody)
			{
				writer.Indent();
			}
			CloseBrace(policy.StatementBraceStyle);
			NewLine();
			EndNode(switchStatement);
		}

		public virtual void VisitSwitchSection(SwitchSection switchSection)
		{
			StartNode(switchSection);
			bool first = true;
			foreach (var label in switchSection.CaseLabels)
			{
				if (!first)
				{
					NewLine();
				}
				label.AcceptVisitor(this);
				first = false;
			}
			bool isBlock = switchSection.Statements.Count == 1 && switchSection.Statements.Single() is BlockStatement;
			if (policy.IndentCaseBody && !isBlock)
			{
				writer.Indent();
			}

			if (!isBlock)
				NewLine();

			foreach (var statement in switchSection.Statements)
			{
				statement.AcceptVisitor(this);
			}

			if (policy.IndentCaseBody && !isBlock)
			{
				writer.Unindent();
			}

			EndNode(switchSection);
		}

		public virtual void VisitCaseLabel(CaseLabel caseLabel)
		{
			StartNode(caseLabel);
			if (caseLabel.Expression.IsNull)
			{
				WriteKeyword(CaseLabel.DefaultKeywordRole);
			}
			else
			{
				WriteKeyword(CaseLabel.CaseKeywordRole);
				Space();
				caseLabel.Expression.AcceptVisitor(this);
			}
			WriteToken(Roles.Colon);
			EndNode(caseLabel);
		}

		public virtual void VisitSwitchExpression(SwitchExpression switchExpression)
		{
			StartNode(switchExpression);
			switchExpression.Expression.AcceptVisitor(this);
			Space();
			WriteKeyword(SwitchExpression.SwitchKeywordRole);
			OpenBrace(policy.ArrayInitializerBraceStyle);
			foreach (AstNode node in switchExpression.SwitchSections)
			{
				node.AcceptVisitor(this);
				Comma(node);
				NewLine();
			}
			CloseBrace(policy.ArrayInitializerBraceStyle);
			EndNode(switchExpression);
		}

		public virtual void VisitSwitchExpressionSection(SwitchExpressionSection switchExpressionSection)
		{
			StartNode(switchExpressionSection);
			switchExpressionSection.Pattern.AcceptVisitor(this);
			Space();
			WriteToken(Roles.Arrow);
			Space();
			switchExpressionSection.Body.AcceptVisitor(this);
			EndNode(switchExpressionSection);
		}

		public virtual void VisitThrowStatement(ThrowStatement throwStatement)
		{
			StartNode(throwStatement);
			WriteKeyword(ThrowStatement.ThrowKeywordRole);
			if (!throwStatement.Expression.IsNull)
			{
				Space();
				throwStatement.Expression.AcceptVisitor(this);
			}
			Semicolon();
			EndNode(throwStatement);
		}

		public virtual void VisitTryCatchStatement(TryCatchStatement tryCatchStatement)
		{
			StartNode(tryCatchStatement);
			WriteKeyword(TryCatchStatement.TryKeywordRole);
			WriteBlock(tryCatchStatement.TryBlock, policy.StatementBraceStyle);
			foreach (var catchClause in tryCatchStatement.CatchClauses)
			{
				if (policy.CatchNewLinePlacement == NewLinePlacement.SameLine)
					Space();
				else
					NewLine();
				catchClause.AcceptVisitor(this);
			}
			if (!tryCatchStatement.FinallyBlock.IsNull)
			{
				if (policy.FinallyNewLinePlacement == NewLinePlacement.SameLine)
					Space();
				else
					NewLine();
				WriteKeyword(TryCatchStatement.FinallyKeywordRole);
				WriteBlock(tryCatchStatement.FinallyBlock, policy.StatementBraceStyle);
			}
			NewLine();
			EndNode(tryCatchStatement);
		}

		public virtual void VisitCatchClause(CatchClause catchClause)
		{
			StartNode(catchClause);
			WriteKeyword(CatchClause.CatchKeywordRole);
			if (!catchClause.Type.IsNull)
			{
				Space(policy.SpaceBeforeCatchParentheses);
				LPar();
				Space(policy.SpacesWithinCatchParentheses);
				catchClause.Type.AcceptVisitor(this);
				if (!string.IsNullOrEmpty(catchClause.VariableName))
				{
					Space();
					WriteIdentifier(catchClause.VariableNameToken);
				}
				Space(policy.SpacesWithinCatchParentheses);
				RPar();
			}
			if (!catchClause.Condition.IsNull)
			{
				Space();
				WriteKeyword(CatchClause.WhenKeywordRole);
				Space(policy.SpaceBeforeIfParentheses);
				WriteToken(CatchClause.CondLPar);
				Space(policy.SpacesWithinIfParentheses);
				catchClause.Condition.AcceptVisitor(this);
				Space(policy.SpacesWithinIfParentheses);
				WriteToken(CatchClause.CondRPar);
			}
			WriteBlock(catchClause.Body, policy.StatementBraceStyle);
			EndNode(catchClause);
		}

		public virtual void VisitUncheckedStatement(UncheckedStatement uncheckedStatement)
		{
			StartNode(uncheckedStatement);
			WriteKeyword(UncheckedStatement.UncheckedKeywordRole);
			uncheckedStatement.Body.AcceptVisitor(this);
			EndNode(uncheckedStatement);
		}

		public virtual void VisitUnsafeStatement(UnsafeStatement unsafeStatement)
		{
			StartNode(unsafeStatement);
			WriteKeyword(UnsafeStatement.UnsafeKeywordRole);
			unsafeStatement.Body.AcceptVisitor(this);
			EndNode(unsafeStatement);
		}

		public virtual void VisitUsingStatement(UsingStatement usingStatement)
		{
			StartNode(usingStatement);
			if (usingStatement.IsAsync)
			{
				WriteKeyword(UsingStatement.AwaitRole);
			}
			WriteKeyword(UsingStatement.UsingKeywordRole);
			if (usingStatement.IsEnhanced)
			{
				Space();
			}
			else
			{
				Space(policy.SpaceBeforeUsingParentheses);
				LPar();
				Space(policy.SpacesWithinUsingParentheses);
			}

			usingStatement.ResourceAcquisition.AcceptVisitor(this);

			if (usingStatement.IsEnhanced)
			{
				Semicolon();
			}
			else
			{
				Space(policy.SpacesWithinUsingParentheses);
				RPar();
			}

			if (usingStatement.IsEnhanced)
			{
				if (usingStatement.EmbeddedStatement is BlockStatement blockStatement)
				{
					StartNode(blockStatement);
					foreach (var node in blockStatement.Statements)
					{
						node.AcceptVisitor(this);
					}
					EndNode(blockStatement);
				}
				else
				{
					usingStatement.EmbeddedStatement.AcceptVisitor(this);
				}
			}
			else
			{
				WriteEmbeddedStatement(usingStatement.EmbeddedStatement);
			}

			EndNode(usingStatement);
		}

		public virtual void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
		{
			StartNode(variableDeclarationStatement);
			WriteModifiers(variableDeclarationStatement.GetChildrenByRole(VariableDeclarationStatement.ModifierRole));
			variableDeclarationStatement.Type.AcceptVisitor(this);
			Space();
			WriteCommaSeparatedList(variableDeclarationStatement.Variables);
			Semicolon();
			EndNode(variableDeclarationStatement);
		}

		public virtual void VisitLocalFunctionDeclarationStatement(LocalFunctionDeclarationStatement localFunctionDeclarationStatement)
		{
			StartNode(localFunctionDeclarationStatement);
			localFunctionDeclarationStatement.Declaration.AcceptVisitor(this);
			EndNode(localFunctionDeclarationStatement);
		}

		public virtual void VisitWhileStatement(WhileStatement whileStatement)
		{
			StartNode(whileStatement);
			WriteKeyword(WhileStatement.WhileKeywordRole);
			Space(policy.SpaceBeforeWhileParentheses);
			LPar();
			Space(policy.SpacesWithinWhileParentheses);
			whileStatement.Condition.AcceptVisitor(this);
			Space(policy.SpacesWithinWhileParentheses);
			RPar();
			WriteEmbeddedStatement(whileStatement.EmbeddedStatement);
			EndNode(whileStatement);
		}

		public virtual void VisitYieldBreakStatement(YieldBreakStatement yieldBreakStatement)
		{
			StartNode(yieldBreakStatement);
			WriteKeyword(YieldBreakStatement.YieldKeywordRole);
			WriteKeyword(YieldBreakStatement.BreakKeywordRole);
			Semicolon();
			EndNode(yieldBreakStatement);
		}

		public virtual void VisitYieldReturnStatement(YieldReturnStatement yieldReturnStatement)
		{
			StartNode(yieldReturnStatement);
			WriteKeyword(YieldReturnStatement.YieldKeywordRole);
			WriteKeyword(YieldReturnStatement.ReturnKeywordRole);
			Space();
			yieldReturnStatement.Expression.AcceptVisitor(this);
			Semicolon();
			EndNode(yieldReturnStatement);
		}

		#endregion

		#region TypeMembers
		public virtual void VisitAccessor(Accessor accessor)
		{
			StartNode(accessor);
			WriteAttributes(accessor.Attributes);
			WriteModifiers(accessor.ModifierTokens);
			BraceStyle style = policy.StatementBraceStyle;
			if (accessor.Role == PropertyDeclaration.GetterRole)
			{
				WriteKeyword("get", PropertyDeclaration.GetKeywordRole);
				style = policy.PropertyGetBraceStyle;
			}
			else if (accessor.Role == PropertyDeclaration.SetterRole)
			{
				if (accessor.Keyword.Role == PropertyDeclaration.InitKeywordRole)
				{
					WriteKeyword("init", PropertyDeclaration.InitKeywordRole);
				}
				else
				{
					WriteKeyword("set", PropertyDeclaration.SetKeywordRole);
				}
				style = policy.PropertySetBraceStyle;
			}
			else if (accessor.Role == CustomEventDeclaration.AddAccessorRole)
			{
				WriteKeyword("add", CustomEventDeclaration.AddKeywordRole);
				style = policy.EventAddBraceStyle;
			}
			else if (accessor.Role == CustomEventDeclaration.RemoveAccessorRole)
			{
				WriteKeyword("remove", CustomEventDeclaration.RemoveKeywordRole);
				style = policy.EventRemoveBraceStyle;
			}
			WriteMethodBody(accessor.Body, style);
			EndNode(accessor);
		}

		public virtual void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
		{
			StartNode(constructorDeclaration);
			WriteAttributes(constructorDeclaration.Attributes);
			WriteModifiers(constructorDeclaration.ModifierTokens);
			TypeDeclaration type = constructorDeclaration.Parent as TypeDeclaration;
			if (type != null && type.Name != constructorDeclaration.Name)
				WriteIdentifier((Identifier)type.NameToken.Clone());
			else
				WriteIdentifier(constructorDeclaration.NameToken);
			Space(policy.SpaceBeforeConstructorDeclarationParentheses);
			WriteCommaSeparatedListInParenthesis(constructorDeclaration.Parameters, policy.SpaceWithinMethodDeclarationParentheses);
			if (!constructorDeclaration.Initializer.IsNull)
			{
				NewLine();
				writer.Indent();
				constructorDeclaration.Initializer.AcceptVisitor(this);
				writer.Unindent();
			}
			WriteMethodBody(constructorDeclaration.Body, policy.ConstructorBraceStyle);
			EndNode(constructorDeclaration);
		}

		public virtual void VisitConstructorInitializer(ConstructorInitializer constructorInitializer)
		{
			StartNode(constructorInitializer);
			WriteToken(Roles.Colon);
			Space();
			if (constructorInitializer.ConstructorInitializerType == ConstructorInitializerType.This)
			{
				WriteKeyword(ConstructorInitializer.ThisKeywordRole);
			}
			else
			{
				WriteKeyword(ConstructorInitializer.BaseKeywordRole);
			}
			Space(policy.SpaceBeforeMethodCallParentheses);
			WriteCommaSeparatedListInParenthesis(constructorInitializer.Arguments, policy.SpaceWithinMethodCallParentheses);
			EndNode(constructorInitializer);
		}

		public virtual void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
		{
			StartNode(destructorDeclaration);
			WriteAttributes(destructorDeclaration.Attributes);
			WriteModifiers(destructorDeclaration.ModifierTokens);
			if (destructorDeclaration.ModifierTokens.Any())
			{
				Space();
			}
			WriteToken(DestructorDeclaration.TildeRole);
			TypeDeclaration type = destructorDeclaration.Parent as TypeDeclaration;
			if (type != null && type.Name != destructorDeclaration.Name)
				WriteIdentifier((Identifier)type.NameToken.Clone());
			else
				WriteIdentifier(destructorDeclaration.NameToken);
			Space(policy.SpaceBeforeConstructorDeclarationParentheses);
			LPar();
			RPar();
			WriteMethodBody(destructorDeclaration.Body, policy.DestructorBraceStyle);
			EndNode(destructorDeclaration);
		}

		public virtual void VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration)
		{
			StartNode(enumMemberDeclaration);
			WriteAttributes(enumMemberDeclaration.Attributes);
			WriteModifiers(enumMemberDeclaration.ModifierTokens);
			WriteIdentifier(enumMemberDeclaration.NameToken);
			if (!enumMemberDeclaration.Initializer.IsNull)
			{
				Space(policy.SpaceAroundAssignment);
				WriteToken(Roles.Assign);
				Space(policy.SpaceAroundAssignment);
				enumMemberDeclaration.Initializer.AcceptVisitor(this);
			}
			EndNode(enumMemberDeclaration);
		}

		public virtual void VisitEventDeclaration(EventDeclaration eventDeclaration)
		{
			StartNode(eventDeclaration);
			WriteAttributes(eventDeclaration.Attributes);
			WriteModifiers(eventDeclaration.ModifierTokens);
			WriteKeyword(EventDeclaration.EventKeywordRole);
			eventDeclaration.ReturnType.AcceptVisitor(this);
			Space();
			WriteCommaSeparatedList(eventDeclaration.Variables);
			Semicolon();
			EndNode(eventDeclaration);
		}

		public virtual void VisitCustomEventDeclaration(CustomEventDeclaration customEventDeclaration)
		{
			StartNode(customEventDeclaration);
			WriteAttributes(customEventDeclaration.Attributes);
			WriteModifiers(customEventDeclaration.ModifierTokens);
			WriteKeyword(CustomEventDeclaration.EventKeywordRole);
			customEventDeclaration.ReturnType.AcceptVisitor(this);
			Space();
			WritePrivateImplementationType(customEventDeclaration.PrivateImplementationType);
			WriteIdentifier(customEventDeclaration.NameToken);
			OpenBrace(policy.EventBraceStyle);
			// output add/remove in their original order
			foreach (AstNode node in customEventDeclaration.Children)
			{
				if (node.Role == CustomEventDeclaration.AddAccessorRole || node.Role == CustomEventDeclaration.RemoveAccessorRole)
				{
					node.AcceptVisitor(this);
				}
			}
			CloseBrace(policy.EventBraceStyle);
			NewLine();
			EndNode(customEventDeclaration);
		}

		public virtual void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
		{
			StartNode(fieldDeclaration);
			WriteAttributes(fieldDeclaration.Attributes);
			WriteModifiers(fieldDeclaration.ModifierTokens);
			fieldDeclaration.ReturnType.AcceptVisitor(this);
			Space();
			WriteCommaSeparatedList(fieldDeclaration.Variables);
			Semicolon();
			EndNode(fieldDeclaration);
		}

		public virtual void VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration)
		{
			StartNode(fixedFieldDeclaration);
			WriteAttributes(fixedFieldDeclaration.Attributes);
			WriteModifiers(fixedFieldDeclaration.ModifierTokens);
			WriteKeyword(FixedFieldDeclaration.FixedKeywordRole);
			Space();
			fixedFieldDeclaration.ReturnType.AcceptVisitor(this);
			Space();
			WriteCommaSeparatedList(fixedFieldDeclaration.Variables);
			Semicolon();
			EndNode(fixedFieldDeclaration);
		}

		public virtual void VisitFixedVariableInitializer(FixedVariableInitializer fixedVariableInitializer)
		{
			StartNode(fixedVariableInitializer);
			WriteIdentifier(fixedVariableInitializer.NameToken);
			if (!fixedVariableInitializer.CountExpression.IsNull)
			{
				WriteToken(Roles.LBracket);
				Space(policy.SpacesWithinBrackets);
				fixedVariableInitializer.CountExpression.AcceptVisitor(this);
				Space(policy.SpacesWithinBrackets);
				WriteToken(Roles.RBracket);
			}
			EndNode(fixedVariableInitializer);
		}

		public virtual void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
		{
			StartNode(indexerDeclaration);
			WriteAttributes(indexerDeclaration.Attributes);
			WriteModifiers(indexerDeclaration.ModifierTokens);
			indexerDeclaration.ReturnType.AcceptVisitor(this);
			Space();
			WritePrivateImplementationType(indexerDeclaration.PrivateImplementationType);
			WriteKeyword(IndexerDeclaration.ThisKeywordRole);
			Space(policy.SpaceBeforeMethodDeclarationParentheses);
			WriteCommaSeparatedListInBrackets(indexerDeclaration.Parameters, policy.SpaceWithinMethodDeclarationParentheses);

			if (indexerDeclaration.ExpressionBody.IsNull)
			{
				bool isSingleLine =
					(policy.AutoPropertyFormatting == PropertyFormatting.SingleLine)
					&& (indexerDeclaration.Getter.IsNull || indexerDeclaration.Getter.Body.IsNull)
					&& (indexerDeclaration.Setter.IsNull || indexerDeclaration.Setter.Body.IsNull)
					&& !indexerDeclaration.Getter.Attributes.Any()
					&& !indexerDeclaration.Setter.Attributes.Any();
				OpenBrace(isSingleLine ? BraceStyle.EndOfLine : policy.PropertyBraceStyle, newLine: !isSingleLine);
				if (isSingleLine)
					Space();
				// output get/set in their original order
				foreach (AstNode node in indexerDeclaration.Children)
				{
					if (node.Role == IndexerDeclaration.GetterRole || node.Role == IndexerDeclaration.SetterRole)
					{
						node.AcceptVisitor(this);
					}
				}
				CloseBrace(isSingleLine ? BraceStyle.EndOfLine : policy.PropertyBraceStyle, unindent: !isSingleLine);
				NewLine();
			}
			else
			{
				Space();
				WriteToken(Roles.Arrow);
				Space();
				indexerDeclaration.ExpressionBody.AcceptVisitor(this);
				Semicolon();
			}
			EndNode(indexerDeclaration);
		}

		public virtual void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			StartNode(methodDeclaration);
			WriteAttributes(methodDeclaration.Attributes);
			WriteModifiers(methodDeclaration.ModifierTokens);
			methodDeclaration.ReturnType.AcceptVisitor(this);
			Space();
			WritePrivateImplementationType(methodDeclaration.PrivateImplementationType);
			WriteIdentifier(methodDeclaration.NameToken);
			WriteTypeParameters(methodDeclaration.TypeParameters);
			Space(policy.SpaceBeforeMethodDeclarationParentheses);
			WriteCommaSeparatedListInParenthesis(methodDeclaration.Parameters, policy.SpaceWithinMethodDeclarationParentheses);
			foreach (Constraint constraint in methodDeclaration.Constraints)
			{
				constraint.AcceptVisitor(this);
			}
			WriteMethodBody(methodDeclaration.Body, policy.MethodBraceStyle);
			EndNode(methodDeclaration);
		}

		public virtual void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
		{
			StartNode(operatorDeclaration);
			WriteAttributes(operatorDeclaration.Attributes);
			WriteModifiers(operatorDeclaration.ModifierTokens);
			if (operatorDeclaration.OperatorType == OperatorType.Explicit || operatorDeclaration.OperatorType == OperatorType.CheckedExplicit)
			{
				WriteKeyword(OperatorDeclaration.ExplicitRole);
			}
			else if (operatorDeclaration.OperatorType == OperatorType.Implicit)
			{
				WriteKeyword(OperatorDeclaration.ImplicitRole);
			}
			else
			{
				operatorDeclaration.ReturnType.AcceptVisitor(this);
			}
			Space();
			WritePrivateImplementationType(operatorDeclaration.PrivateImplementationType);
			WriteKeyword(OperatorDeclaration.OperatorKeywordRole);
			Space();
			if (OperatorDeclaration.IsChecked(operatorDeclaration.OperatorType))
			{
				WriteKeyword(OperatorDeclaration.CheckedKeywordRole);
				Space();
			}
			if (operatorDeclaration.OperatorType == OperatorType.Explicit
				|| operatorDeclaration.OperatorType == OperatorType.CheckedExplicit
				|| operatorDeclaration.OperatorType == OperatorType.Implicit)
			{
				operatorDeclaration.ReturnType.AcceptVisitor(this);
			}
			else
			{
				WriteToken(OperatorDeclaration.GetToken(operatorDeclaration.OperatorType), OperatorDeclaration.GetRole(operatorDeclaration.OperatorType));
			}
			Space(policy.SpaceBeforeMethodDeclarationParentheses);
			WriteCommaSeparatedListInParenthesis(operatorDeclaration.Parameters, policy.SpaceWithinMethodDeclarationParentheses);
			WriteMethodBody(operatorDeclaration.Body, policy.MethodBraceStyle);
			EndNode(operatorDeclaration);
		}

		public virtual void VisitParameterDeclaration(ParameterDeclaration parameterDeclaration)
		{
			StartNode(parameterDeclaration);
			WriteAttributes(parameterDeclaration.Attributes);
			if (parameterDeclaration.HasThisModifier)
			{
				WriteKeyword(ParameterDeclaration.ThisModifierRole);
				Space();
			}
			if (parameterDeclaration.IsParams)
			{
				WriteKeyword(ParameterDeclaration.ParamsModifierRole);
				Space();
			}
			if (parameterDeclaration.IsScopedRef)
			{
				WriteKeyword(ParameterDeclaration.ScopedRefRole);
				Space();
			}
			switch (parameterDeclaration.ParameterModifier)
			{
				case ReferenceKind.Ref:
					WriteKeyword(ParameterDeclaration.RefModifierRole);
					Space();
					break;
				case ReferenceKind.RefReadOnly:
					WriteKeyword(ParameterDeclaration.RefModifierRole);
					WriteKeyword(ParameterDeclaration.ReadonlyModifierRole);
					Space();
					break;
				case ReferenceKind.Out:
					WriteKeyword(ParameterDeclaration.OutModifierRole);
					Space();
					break;
				case ReferenceKind.In:
					WriteKeyword(ParameterDeclaration.InModifierRole);
					Space();
					break;
			}
			parameterDeclaration.Type.AcceptVisitor(this);
			if (!parameterDeclaration.Type.IsNull && !string.IsNullOrEmpty(parameterDeclaration.Name))
			{
				Space();
			}
			if (!string.IsNullOrEmpty(parameterDeclaration.Name))
			{
				WriteIdentifier(parameterDeclaration.NameToken);
			}
			if (parameterDeclaration.HasNullCheck)
			{
				WriteToken(Roles.DoubleExclamation);
			}
			if (!parameterDeclaration.DefaultExpression.IsNull)
			{
				Space(policy.SpaceAroundAssignment);
				WriteToken(Roles.Assign);
				Space(policy.SpaceAroundAssignment);
				parameterDeclaration.DefaultExpression.AcceptVisitor(this);
			}
			EndNode(parameterDeclaration);
		}

		public virtual void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
		{
			StartNode(propertyDeclaration);
			WriteAttributes(propertyDeclaration.Attributes);
			WriteModifiers(propertyDeclaration.ModifierTokens);
			propertyDeclaration.ReturnType.AcceptVisitor(this);
			Space();
			WritePrivateImplementationType(propertyDeclaration.PrivateImplementationType);
			WriteIdentifier(propertyDeclaration.NameToken);
			if (propertyDeclaration.ExpressionBody.IsNull)
			{
				bool isSingleLine =
					(policy.AutoPropertyFormatting == PropertyFormatting.SingleLine)
					&& (propertyDeclaration.Getter.IsNull || propertyDeclaration.Getter.Body.IsNull)
					&& (propertyDeclaration.Setter.IsNull || propertyDeclaration.Setter.Body.IsNull)
					&& !propertyDeclaration.Getter.Attributes.Any()
					&& !propertyDeclaration.Setter.Attributes.Any();
				OpenBrace(isSingleLine ? BraceStyle.EndOfLine : policy.PropertyBraceStyle, newLine: !isSingleLine);
				if (isSingleLine)
					Space();
				// output get/set in their original order
				foreach (AstNode node in propertyDeclaration.Children)
				{
					if (node.Role == IndexerDeclaration.GetterRole || node.Role == IndexerDeclaration.SetterRole)
					{
						node.AcceptVisitor(this);
					}
				}
				CloseBrace(isSingleLine ? BraceStyle.EndOfLine : policy.PropertyBraceStyle, unindent: !isSingleLine);
				if (!propertyDeclaration.Initializer.IsNull)
				{
					Space(policy.SpaceAroundAssignment);
					WriteToken(Roles.Assign);
					Space(policy.SpaceAroundAssignment);
					propertyDeclaration.Initializer.AcceptVisitor(this);
					Semicolon();
				}
				NewLine();
			}
			else
			{
				Space();
				WriteToken(Roles.Arrow);
				Space();
				propertyDeclaration.ExpressionBody.AcceptVisitor(this);
				Semicolon();
			}
			EndNode(propertyDeclaration);
		}

		#endregion

		#region Other nodes
		public virtual void VisitVariableInitializer(VariableInitializer variableInitializer)
		{
			StartNode(variableInitializer);
			WriteIdentifier(variableInitializer.NameToken);
			if (!variableInitializer.Initializer.IsNull)
			{
				Space(policy.SpaceAroundAssignment);
				WriteToken(Roles.Assign);
				Space(policy.SpaceAroundAssignment);
				variableInitializer.Initializer.AcceptVisitor(this);
			}
			EndNode(variableInitializer);
		}

		void MaybeNewLinesAfterUsings(AstNode node)
		{
			var nextSibling = node.NextSibling;

			if ((node is UsingDeclaration || node is UsingAliasDeclaration) && !(nextSibling is UsingDeclaration || nextSibling is UsingAliasDeclaration))
			{
				for (int i = 0; i < policy.MinimumBlankLinesAfterUsings; i++)
					NewLine();
			}
		}

		public virtual void VisitSyntaxTree(SyntaxTree syntaxTree)
		{
			// don't do node tracking as we visit all children directly
			foreach (AstNode node in syntaxTree.Children)
			{
				node.AcceptVisitor(this);
				MaybeNewLinesAfterUsings(node);
			}
		}

		public virtual void VisitSimpleType(SimpleType simpleType)
		{
			StartNode(simpleType);
			WriteIdentifier(simpleType.IdentifierToken);
			WriteTypeArguments(simpleType.TypeArguments);
			EndNode(simpleType);
		}

		public virtual void VisitMemberType(MemberType memberType)
		{
			StartNode(memberType);
			memberType.Target.AcceptVisitor(this);
			if (memberType.IsDoubleColon)
			{
				WriteToken(Roles.DoubleColon);
			}
			else
			{
				WriteToken(Roles.Dot);
			}
			WriteIdentifier(memberType.MemberNameToken);
			WriteTypeArguments(memberType.TypeArguments);
			EndNode(memberType);
		}

		public virtual void VisitTupleType(TupleAstType tupleType)
		{
			Debug.Assert(tupleType.Elements.Count >= 2);
			StartNode(tupleType);
			LPar();
			WriteCommaSeparatedList(tupleType.Elements);
			RPar();
			EndNode(tupleType);
		}

		public virtual void VisitTupleTypeElement(TupleTypeElement tupleTypeElement)
		{
			StartNode(tupleTypeElement);
			tupleTypeElement.Type.AcceptVisitor(this);
			if (!tupleTypeElement.NameToken.IsNull)
			{
				Space();
				tupleTypeElement.NameToken.AcceptVisitor(this);
			}
			EndNode(tupleTypeElement);
		}

		public virtual void VisitFunctionPointerType(FunctionPointerAstType functionPointerType)
		{
			StartNode(functionPointerType);
			WriteKeyword(Roles.DelegateKeyword);
			WriteToken(FunctionPointerAstType.PointerRole);
			if (functionPointerType.HasUnmanagedCallingConvention)
			{
				Space();
				WriteKeyword("unmanaged");
			}
			if (functionPointerType.CallingConventions.Any())
			{
				WriteToken(Roles.LBracket);
				WriteCommaSeparatedList(functionPointerType.CallingConventions);
				WriteToken(Roles.RBracket);
			}
			WriteToken(Roles.LChevron);
			WriteCommaSeparatedList(
				functionPointerType.Parameters.Concat<AstNode>(new[] { functionPointerType.ReturnType }));
			WriteToken(Roles.RChevron);
			EndNode(functionPointerType);
		}

		public virtual void VisitInvocationType(InvocationAstType invocationType)
		{
			StartNode(invocationType);
			invocationType.BaseType.AcceptVisitor(this);
			WriteToken(Roles.LPar);
			WriteCommaSeparatedList(invocationType.Arguments);
			WriteToken(Roles.RPar);
			EndNode(invocationType);
		}

		public virtual void VisitComposedType(ComposedType composedType)
		{
			StartNode(composedType);
			if (composedType.Attributes.Any())
			{
				foreach (var attr in composedType.Attributes)
				{
					attr.AcceptVisitor(this);
				}
			}
			if (composedType.HasRefSpecifier)
			{
				WriteKeyword(ComposedType.RefRole);
			}
			if (composedType.HasReadOnlySpecifier)
			{
				WriteKeyword(ComposedType.ReadonlyRole);
			}
			composedType.BaseType.AcceptVisitor(this);
			if (composedType.HasNullableSpecifier)
			{
				WriteToken(ComposedType.NullableRole);
			}
			for (int i = 0; i < composedType.PointerRank; i++)
			{
				WriteToken(ComposedType.PointerRole);
			}
			foreach (var node in composedType.ArraySpecifiers)
			{
				node.AcceptVisitor(this);
			}
			EndNode(composedType);
		}

		public virtual void VisitArraySpecifier(ArraySpecifier arraySpecifier)
		{
			StartNode(arraySpecifier);
			WriteToken(Roles.LBracket);
			foreach (var comma in arraySpecifier.GetChildrenByRole(Roles.Comma))
			{
				writer.WriteToken(Roles.Comma, ",");
			}
			WriteToken(Roles.RBracket);
			EndNode(arraySpecifier);
		}

		public virtual void VisitPrimitiveType(PrimitiveType primitiveType)
		{
			StartNode(primitiveType);
			writer.WritePrimitiveType(primitiveType.Keyword);
			isAfterSpace = false;
			EndNode(primitiveType);
		}

		public virtual void VisitSingleVariableDesignation(SingleVariableDesignation singleVariableDesignation)
		{
			StartNode(singleVariableDesignation);
			WriteIdentifier(singleVariableDesignation.IdentifierToken);
			EndNode(singleVariableDesignation);
		}

		public virtual void VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignation parenthesizedVariableDesignation)
		{
			StartNode(parenthesizedVariableDesignation);
			LPar();
			WriteCommaSeparatedList(parenthesizedVariableDesignation.VariableDesignations);
			RPar();
			EndNode(parenthesizedVariableDesignation);
		}

		public virtual void VisitComment(Comment comment)
		{
			writer.StartNode(comment);
			writer.WriteComment(comment.CommentType, comment.Content);
			writer.EndNode(comment);
		}

		public virtual void VisitPreProcessorDirective(PreProcessorDirective preProcessorDirective)
		{
			writer.StartNode(preProcessorDirective);
			writer.WritePreProcessorDirective(preProcessorDirective.Type, preProcessorDirective.Argument);
			writer.EndNode(preProcessorDirective);
		}

		public virtual void VisitTypeParameterDeclaration(TypeParameterDeclaration typeParameterDeclaration)
		{
			StartNode(typeParameterDeclaration);
			WriteAttributes(typeParameterDeclaration.Attributes);
			switch (typeParameterDeclaration.Variance)
			{
				case VarianceModifier.Invariant:
					break;
				case VarianceModifier.Covariant:
					WriteKeyword(TypeParameterDeclaration.OutVarianceKeywordRole);
					break;
				case VarianceModifier.Contravariant:
					WriteKeyword(TypeParameterDeclaration.InVarianceKeywordRole);
					break;
				default:
					throw new NotSupportedException("Invalid value for VarianceModifier");
			}
			WriteIdentifier(typeParameterDeclaration.NameToken);
			EndNode(typeParameterDeclaration);
		}

		public virtual void VisitConstraint(Constraint constraint)
		{
			StartNode(constraint);
			Space();
			WriteKeyword(Roles.WhereKeyword);
			constraint.TypeParameter.AcceptVisitor(this);
			Space();
			WriteToken(Roles.Colon);
			Space();
			WriteCommaSeparatedList(constraint.BaseTypes);
			EndNode(constraint);
		}

		public virtual void VisitCSharpTokenNode(CSharpTokenNode cSharpTokenNode)
		{
			CSharpModifierToken mod = cSharpTokenNode as CSharpModifierToken;
			if (mod != null)
			{
				// ITokenWriter assumes that each node processed between a
				// StartNode(parentNode)-EndNode(parentNode)-pair is a child of parentNode.
				WriteKeyword(CSharpModifierToken.GetModifierName(mod.Modifier), cSharpTokenNode.Role);
			}
			else
			{
				throw new NotSupportedException("Should never visit individual tokens");
			}
		}

		public virtual void VisitIdentifier(Identifier identifier)
		{
			// Do not call StartNode and EndNode for Identifier, because they are handled by the ITokenWriter.
			// ITokenWriter assumes that each node processed between a
			// StartNode(parentNode)-EndNode(parentNode)-pair is a child of parentNode.
			WriteIdentifier(identifier);
		}

		void IAstVisitor.VisitNullNode(AstNode nullNode)
		{
		}

		void IAstVisitor.VisitErrorNode(AstNode errorNode)
		{
			StartNode(errorNode);
			EndNode(errorNode);
		}
		#endregion

		#region Pattern Nodes
		public virtual void VisitPatternPlaceholder(AstNode placeholder, Pattern pattern)
		{
			StartNode(placeholder);
			VisitNodeInPattern(pattern);
			EndNode(placeholder);
		}

		void VisitAnyNode(AnyNode anyNode)
		{
			if (!string.IsNullOrEmpty(anyNode.GroupName))
			{
				WriteIdentifier(anyNode.GroupName);
				WriteToken(Roles.Colon);
			}
		}

		void VisitBackreference(Backreference backreference)
		{
			WriteKeyword("backreference");
			LPar();
			WriteIdentifier(backreference.ReferencedGroupName);
			RPar();
		}

		void VisitIdentifierExpressionBackreference(IdentifierExpressionBackreference identifierExpressionBackreference)
		{
			WriteKeyword("identifierBackreference");
			LPar();
			WriteIdentifier(identifierExpressionBackreference.ReferencedGroupName);
			RPar();
		}

		void VisitChoice(Choice choice)
		{
			WriteKeyword("choice");
			Space();
			LPar();
			NewLine();
			writer.Indent();
			foreach (INode alternative in choice)
			{
				VisitNodeInPattern(alternative);
				if (alternative != choice.Last())
				{
					WriteToken(Roles.Comma);
				}
				NewLine();
			}
			writer.Unindent();
			RPar();
		}

		void VisitNamedNode(NamedNode namedNode)
		{
			if (!string.IsNullOrEmpty(namedNode.GroupName))
			{
				WriteIdentifier(namedNode.GroupName);
				WriteToken(Roles.Colon);
			}
			VisitNodeInPattern(namedNode.ChildNode);
		}

		void VisitRepeat(Repeat repeat)
		{
			WriteKeyword("repeat");
			LPar();
			if (repeat.MinCount != 0 || repeat.MaxCount != int.MaxValue)
			{
				WriteIdentifier(repeat.MinCount.ToString());
				WriteToken(Roles.Comma);
				WriteIdentifier(repeat.MaxCount.ToString());
				WriteToken(Roles.Comma);
			}
			VisitNodeInPattern(repeat.ChildNode);
			RPar();
		}

		void VisitOptionalNode(OptionalNode optionalNode)
		{
			WriteKeyword("optional");
			LPar();
			VisitNodeInPattern(optionalNode.ChildNode);
			RPar();
		}

		void VisitNodeInPattern(INode childNode)
		{
			if (childNode is AstNode)
			{
				((AstNode)childNode).AcceptVisitor(this);
			}
			else if (childNode is IdentifierExpressionBackreference)
			{
				VisitIdentifierExpressionBackreference((IdentifierExpressionBackreference)childNode);
			}
			else if (childNode is Choice)
			{
				VisitChoice((Choice)childNode);
			}
			else if (childNode is AnyNode)
			{
				VisitAnyNode((AnyNode)childNode);
			}
			else if (childNode is Backreference)
			{
				VisitBackreference((Backreference)childNode);
			}
			else if (childNode is NamedNode)
			{
				VisitNamedNode((NamedNode)childNode);
			}
			else if (childNode is OptionalNode)
			{
				VisitOptionalNode((OptionalNode)childNode);
			}
			else if (childNode is Repeat)
			{
				VisitRepeat((Repeat)childNode);
			}
			else
			{
				writer.WritePrimitiveValue(childNode);
			}
		}
		#endregion

		#region Documentation Reference
		public virtual void VisitDocumentationReference(DocumentationReference documentationReference)
		{
			StartNode(documentationReference);
			if (!documentationReference.DeclaringType.IsNull)
			{
				documentationReference.DeclaringType.AcceptVisitor(this);
				if (documentationReference.SymbolKind != SymbolKind.TypeDefinition)
				{
					WriteToken(Roles.Dot);
				}
			}
			switch (documentationReference.SymbolKind)
			{
				case SymbolKind.TypeDefinition:
					// we already printed the DeclaringType
					break;
				case SymbolKind.Indexer:
					WriteKeyword(IndexerDeclaration.ThisKeywordRole);
					break;
				case SymbolKind.Operator:
					var opType = documentationReference.OperatorType;
					if (opType == OperatorType.Explicit || opType == OperatorType.CheckedExplicit)
					{
						WriteKeyword(OperatorDeclaration.ExplicitRole);
					}
					else if (opType == OperatorType.Implicit)
					{
						WriteKeyword(OperatorDeclaration.ImplicitRole);
					}
					WriteKeyword(OperatorDeclaration.OperatorKeywordRole);
					Space();
					if (OperatorDeclaration.IsChecked(opType))
					{
						WriteKeyword(OperatorDeclaration.CheckedKeywordRole);
						Space();
					}
					if (opType == OperatorType.Explicit || opType == OperatorType.Implicit || opType == OperatorType.CheckedExplicit)
					{
						documentationReference.ConversionOperatorReturnType.AcceptVisitor(this);
					}
					else
					{
						WriteToken(OperatorDeclaration.GetToken(opType), OperatorDeclaration.GetRole(opType));
					}
					break;
				default:
					WriteIdentifier(documentationReference.GetChildByRole(Roles.Identifier));
					break;
			}
			WriteTypeArguments(documentationReference.TypeArguments);
			if (documentationReference.HasParameterList)
			{
				Space(policy.SpaceBeforeMethodDeclarationParentheses);
				if (documentationReference.SymbolKind == SymbolKind.Indexer)
				{
					WriteCommaSeparatedListInBrackets(documentationReference.Parameters, policy.SpaceWithinMethodDeclarationParentheses);
				}
				else
				{
					WriteCommaSeparatedListInParenthesis(documentationReference.Parameters, policy.SpaceWithinMethodDeclarationParentheses);
				}
			}
			EndNode(documentationReference);
		}
		#endregion

		/// <summary>
		/// Converts special characters to escape sequences within the given string.
		/// </summary>
		public static string ConvertString(string text)
		{
			return TextWriterTokenWriter.ConvertString(text);
		}
	}
}
