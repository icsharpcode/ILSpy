using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace ICSharpCode.ILSpy.AddIn
{
	static class SyntaxNodeExtensions
	{
		public static IEnumerable<SyntaxNode> GetAncestors(this SyntaxNode node)
		{
			var current = node.Parent;

			while (current != null)
			{
				yield return current;

				current = current is IStructuredTriviaSyntax
					? ((IStructuredTriviaSyntax)current).ParentTrivia.Token.Parent
					: current.Parent;
			}
		}

		public static IEnumerable<TNode> GetAncestors<TNode>(this SyntaxNode node)
			where TNode : SyntaxNode
		{
			var current = node.Parent;
			while (current != null)
			{
				if (current is TNode)
				{
					yield return (TNode)current;
				}

				current = current is IStructuredTriviaSyntax
					? ((IStructuredTriviaSyntax)current).ParentTrivia.Token.Parent
					: current.Parent;
			}
		}

		public static TNode GetAncestor<TNode>(this SyntaxNode node)
			where TNode : SyntaxNode
		{
			if (node == null)
			{
				return default(TNode);
			}

			return node.GetAncestors<TNode>().FirstOrDefault();
		}

		public static TNode GetAncestorOrThis<TNode>(this SyntaxNode node)
			where TNode : SyntaxNode
		{
			if (node == null)
			{
				return default(TNode);
			}

			return node.GetAncestorsOrThis<TNode>().FirstOrDefault();
		}

		public static IEnumerable<TNode> GetAncestorsOrThis<TNode>(this SyntaxNode node)
			where TNode : SyntaxNode
		{
			var current = node;
			while (current != null)
			{
				if (current is TNode)
				{
					yield return (TNode)current;
				}

				current = current is IStructuredTriviaSyntax
					? ((IStructuredTriviaSyntax)current).ParentTrivia.Token.Parent
					: current.Parent;
			}
		}

		public static bool HasAncestor<TNode>(this SyntaxNode node)
			where TNode : SyntaxNode
		{
			return node.GetAncestors<TNode>().Any();
		}

		public static bool CheckParent<T>(this SyntaxNode node, Func<T, bool> valueChecker) where T : SyntaxNode
		{
			if (node == null)
			{
				return false;
			}

			var parentNode = node.Parent as T;
			if (parentNode == null)
			{
				return false;
			}

			return valueChecker(parentNode);
		}

		/// <summary>
		/// Returns true if is a given token is a child token of of a certain type of parent node.
		/// </summary>
		/// <typeparam name="TParent">The type of the parent node.</typeparam>
		/// <param name="node">The node that we are testing.</param>
		/// <param name="childGetter">A function that, when given the parent node, returns the child token we are interested in.</param>
		public static bool IsChildNode<TParent>(this SyntaxNode node, Func<TParent, SyntaxNode> childGetter)
			where TParent : SyntaxNode
		{
			var ancestor = node.GetAncestor<TParent>();
			if (ancestor == null)
			{
				return false;
			}

			var ancestorNode = childGetter(ancestor);

			return node == ancestorNode;
		}

		/// <summary>
		/// Returns true if this node is found underneath the specified child in the given parent.
		/// </summary>
		public static bool IsFoundUnder<TParent>(this SyntaxNode node, Func<TParent, SyntaxNode> childGetter)
			where TParent : SyntaxNode
		{
			var ancestor = node.GetAncestor<TParent>();
			if (ancestor == null)
			{
				return false;
			}

			var child = childGetter(ancestor);

			// See if node passes through child on the way up to ancestor.
			return node.GetAncestorsOrThis<SyntaxNode>().Contains(child);
		}

		public static SyntaxNode GetCommonRoot(this SyntaxNode node1, SyntaxNode node2)
		{
			//Contract.ThrowIfTrue(node1.RawKind == 0 || node2.RawKind == 0);

			// find common starting node from two nodes.
			// as long as two nodes belong to same tree, there must be at least one common root (Ex, compilation unit)
			var ancestors = node1.GetAncestorsOrThis<SyntaxNode>();
			var set = new HashSet<SyntaxNode>(node2.GetAncestorsOrThis<SyntaxNode>());

			return ancestors.First(set.Contains);
		}

		public static int Width(this SyntaxNode node)
		{
			return node.Span.Length;
		}

		public static int FullWidth(this SyntaxNode node)
		{
			return node.FullSpan.Length;
		}

		public static SyntaxNode FindInnermostCommonNode(
			this IEnumerable<SyntaxNode> nodes,
			Func<SyntaxNode, bool> predicate)
		{
			IEnumerable<SyntaxNode> blocks = null;
			foreach (var node in nodes)
			{
				blocks = blocks == null
					? node.AncestorsAndSelf().Where(predicate)
					: blocks.Intersect(node.AncestorsAndSelf().Where(predicate));
			}

			return blocks == null ? null : blocks.First();
		}

		public static TSyntaxNode FindInnermostCommonNode<TSyntaxNode>(this IEnumerable<SyntaxNode> nodes)
			where TSyntaxNode : SyntaxNode
		{
			return (TSyntaxNode)nodes.FindInnermostCommonNode(n => n is TSyntaxNode);
		}

		/// <summary>
		/// create a new root node from the given root after adding annotations to the tokens
		/// 
		/// tokens should belong to the given root
		/// </summary>
		public static SyntaxNode AddAnnotations(this SyntaxNode root, IEnumerable<Tuple<SyntaxToken, SyntaxAnnotation>> pairs)
		{
			//			Contract.ThrowIfNull(root);
			//			Contract.ThrowIfNull(pairs);

			var tokenMap = pairs.GroupBy(p => p.Item1, p => p.Item2).ToDictionary(g => g.Key, g => g.ToArray());
			return root.ReplaceTokens(tokenMap.Keys, (o, n) => o.WithAdditionalAnnotations(tokenMap[o]));
		}

		/// <summary>
		/// create a new root node from the given root after adding annotations to the nodes
		/// 
		/// nodes should belong to the given root
		/// </summary>
		public static SyntaxNode AddAnnotations(this SyntaxNode root, IEnumerable<Tuple<SyntaxNode, SyntaxAnnotation>> pairs)
		{
			//			Contract.ThrowIfNull(root);
			//			Contract.ThrowIfNull(pairs);

			var tokenMap = pairs.GroupBy(p => p.Item1, p => p.Item2).ToDictionary(g => g.Key, g => g.ToArray());
			return root.ReplaceNodes(tokenMap.Keys, (o, n) => o.WithAdditionalAnnotations(tokenMap[o]));
		}

		public static TextSpan GetContainedSpan(this IEnumerable<SyntaxNode> nodes)
		{
			//			Contract.ThrowIfNull(nodes);
			//			Contract.ThrowIfFalse(nodes.Any());

			TextSpan fullSpan = nodes.First().Span;
			foreach (var node in nodes)
			{
				fullSpan = TextSpan.FromBounds(
					Math.Min(fullSpan.Start, node.SpanStart),
					Math.Max(fullSpan.End, node.Span.End));
			}

			return fullSpan;
		}

		public static IEnumerable<TextSpan> GetContiguousSpans(
			this IEnumerable<SyntaxNode> nodes, Func<SyntaxNode, SyntaxToken> getLastToken = null)
		{
			SyntaxNode lastNode = null;
			TextSpan? textSpan = null;
			foreach (var node in nodes)
			{
				if (lastNode == null)
				{
					textSpan = node.Span;
				}
				else
				{
					var lastToken = getLastToken == null
						? lastNode.GetLastToken()
						: getLastToken(lastNode);
					if (lastToken.GetNextToken(includeDirectives: true) == node.GetFirstToken())
					{
						// Expand the span
						textSpan = TextSpan.FromBounds(textSpan.Value.Start, node.Span.End);
					}
					else
					{
						// Return the last span, and start a new one
						yield return textSpan.Value;
						textSpan = node.Span;
					}
				}

				lastNode = node;
			}

			if (textSpan.HasValue)
			{
				yield return textSpan.Value;
			}
		}

		//public static bool OverlapsHiddenPosition(this SyntaxNode node, CancellationToken cancellationToken)
		//{
		//	return node.OverlapsHiddenPosition(node.Span, cancellationToken);
		//}

		//public static bool OverlapsHiddenPosition(this SyntaxNode node, TextSpan span, CancellationToken cancellationToken)
		//{
		//	return node.SyntaxTree.OverlapsHiddenPosition(span, cancellationToken);
		//}

		//public static bool OverlapsHiddenPosition(this SyntaxNode declaration, SyntaxNode startNode, SyntaxNode endNode, CancellationToken cancellationToken)
		//{
		//	var start = startNode.Span.End;
		//	var end = endNode.SpanStart;

		//	var textSpan = TextSpan.FromBounds(start, end);
		//	return declaration.OverlapsHiddenPosition(textSpan, cancellationToken);
		//}

		public static IEnumerable<T> GetAnnotatedNodes<T>(this SyntaxNode node, SyntaxAnnotation syntaxAnnotation) where T : SyntaxNode
		{
			return node.GetAnnotatedNodesAndTokens(syntaxAnnotation).Select(n => n.AsNode()).OfType<T>();
		}

		public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
		{
			if (node == null)
			{
				return false;
			}

			var csharpKind = node.Kind();
			return csharpKind == kind1 || csharpKind == kind2;
		}

		public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
		{
			if (node == null)
			{
				return false;
			}

			var csharpKind = node.Kind();
			return csharpKind == kind1 || csharpKind == kind2 || csharpKind == kind3;
		}

		public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
		{
			if (node == null)
			{
				return false;
			}

			var csharpKind = node.Kind();
			return csharpKind == kind1 || csharpKind == kind2 || csharpKind == kind3 || csharpKind == kind4;
		}

		public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
		{
			if (node == null)
			{
				return false;
			}

			var csharpKind = node.Kind();
			return csharpKind == kind1 || csharpKind == kind2 || csharpKind == kind3 || csharpKind == kind4 || csharpKind == kind5;
		}

		/// <summary>
		/// Returns the list of using directives that affect <paramref name="node"/>. The list will be returned in
		/// top down order.  
		/// </summary>
		public static IEnumerable<UsingDirectiveSyntax> GetEnclosingUsingDirectives(this SyntaxNode node)
		{
			return node.GetAncestorOrThis<CompilationUnitSyntax>().Usings
				.Concat(node.GetAncestorsOrThis<NamespaceDeclarationSyntax>()
					.Reverse()
					.SelectMany(n => n.Usings));
		}

		public static bool IsUnsafeContext(this SyntaxNode node)
		{
			if (node.GetAncestor<UnsafeStatementSyntax>() != null)
			{
				return true;
			}

			return node.GetAncestors<MemberDeclarationSyntax>().Any(
				m => m.GetModifiers().Any(SyntaxKind.UnsafeKeyword));
		}

		public static bool IsInStaticContext(this SyntaxNode node)
		{
			// this/base calls are always static.
			if (node.FirstAncestorOrSelf<ConstructorInitializerSyntax>() != null)
			{
				return true;
			}

			var memberDeclaration = node.FirstAncestorOrSelf<MemberDeclarationSyntax>();
			if (memberDeclaration == null)
			{
				return false;
			}

			switch (memberDeclaration.Kind())
			{
				case SyntaxKind.MethodDeclaration:
				case SyntaxKind.ConstructorDeclaration:
				case SyntaxKind.PropertyDeclaration:
				case SyntaxKind.EventDeclaration:
				case SyntaxKind.IndexerDeclaration:
					return memberDeclaration.GetModifiers().Any(SyntaxKind.StaticKeyword);

				case SyntaxKind.FieldDeclaration:
					// Inside a field one can only access static members of a type.
					return true;

				case SyntaxKind.DestructorDeclaration:
					return false;
			}

			// Global statements are not a static context.
			if (node.FirstAncestorOrSelf<GlobalStatementSyntax>() != null)
			{
				return false;
			}

			// any other location is considered static
			return true;
		}

		public static NamespaceDeclarationSyntax GetInnermostNamespaceDeclarationWithUsings(this SyntaxNode contextNode)
		{
			var usingDirectiveAncestor = contextNode.GetAncestor<UsingDirectiveSyntax>();
			if (usingDirectiveAncestor == null)
			{
				return contextNode.GetAncestorsOrThis<NamespaceDeclarationSyntax>().FirstOrDefault(n => n.Usings.Count > 0);
			}
			else
			{
				// We are inside a using directive. In this case, we should find and return the first 'parent' namespace with usings.
				var containingNamespace = usingDirectiveAncestor.GetAncestor<NamespaceDeclarationSyntax>();
				if (containingNamespace == null)
				{
					// We are inside a top level using directive (i.e. one that's directly in the compilation unit).
					return null;
				}
				else
				{
					return containingNamespace.GetAncestors<NamespaceDeclarationSyntax>().FirstOrDefault(n => n.Usings.Count > 0);
				}
			}
		}

		/// <summary>
		/// Returns all of the trivia to the left of this token up to the previous token (concatenates
		/// the previous token's trailing trivia and this token's leading trivia).
		/// </summary>
		public static IEnumerable<SyntaxTrivia> GetAllPrecedingTriviaToPreviousToken(this SyntaxToken token)
		{
			var prevToken = token.GetPreviousToken(includeSkipped: true);
			if (prevToken.Kind() == SyntaxKind.None)
			{
				return token.LeadingTrivia;
			}

			return prevToken.TrailingTrivia.Concat(token.LeadingTrivia);
		}

		public static bool IsBreakableConstruct(this SyntaxNode node)
		{
			switch (node.Kind())
			{
				case SyntaxKind.DoStatement:
				case SyntaxKind.WhileStatement:
				case SyntaxKind.SwitchStatement:
				case SyntaxKind.ForStatement:
				case SyntaxKind.ForEachStatement:
					return true;
			}

			return false;
		}

		public static bool IsContinuableConstruct(this SyntaxNode node)
		{
			switch (node.Kind())
			{
				case SyntaxKind.DoStatement:
				case SyntaxKind.WhileStatement:
				case SyntaxKind.ForStatement:
				case SyntaxKind.ForEachStatement:
					return true;
			}

			return false;
		}

		public static bool IsReturnableConstruct(this SyntaxNode node)
		{
			switch (node.Kind())
			{
				case SyntaxKind.AnonymousMethodExpression:
				case SyntaxKind.SimpleLambdaExpression:
				case SyntaxKind.ParenthesizedLambdaExpression:
				case SyntaxKind.MethodDeclaration:
				case SyntaxKind.ConstructorDeclaration:
				case SyntaxKind.DestructorDeclaration:
				case SyntaxKind.GetAccessorDeclaration:
				case SyntaxKind.SetAccessorDeclaration:
				case SyntaxKind.OperatorDeclaration:
				case SyntaxKind.AddAccessorDeclaration:
				case SyntaxKind.RemoveAccessorDeclaration:
					return true;
			}

			return false;
		}

		public static bool IsAnyArgumentList(this SyntaxNode node)
		{
			return node.IsKind(SyntaxKind.ArgumentList) ||
				node.IsKind(SyntaxKind.AttributeArgumentList) ||
				node.IsKind(SyntaxKind.BracketedArgumentList) ||
				node.IsKind(SyntaxKind.TypeArgumentList);
		}

		public static bool IsAnyLambda(this SyntaxNode node)
		{
			return
				node.IsKind(SyntaxKind.ParenthesizedLambdaExpression) ||
				node.IsKind(SyntaxKind.SimpleLambdaExpression);
		}

		public static bool IsAnyLambdaOrAnonymousMethod(this SyntaxNode node)
		{
			return node.IsAnyLambda() || node.IsKind(SyntaxKind.AnonymousMethodExpression);
		}

		public static bool IsAnyAssignExpression(this SyntaxNode node)
		{
			return SyntaxFacts.IsAssignmentExpression(node.Kind());
		}

		public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind)
		{
			return node != null && node.Parent.IsKind(kind);
		}

		public static bool IsParentKind(this SyntaxToken node, SyntaxKind kind)
		{
			return node.Parent != null && node.Parent.IsKind(kind);
		}

		public static bool IsCompoundAssignExpression(this SyntaxNode node)
		{
			switch (node.Kind())
			{
				case SyntaxKind.AddAssignmentExpression:
				case SyntaxKind.SubtractAssignmentExpression:
				case SyntaxKind.MultiplyAssignmentExpression:
				case SyntaxKind.DivideAssignmentExpression:
				case SyntaxKind.ModuloAssignmentExpression:
				case SyntaxKind.AndAssignmentExpression:
				case SyntaxKind.ExclusiveOrAssignmentExpression:
				case SyntaxKind.OrAssignmentExpression:
				case SyntaxKind.LeftShiftAssignmentExpression:
				case SyntaxKind.RightShiftAssignmentExpression:
					return true;
			}

			return false;
		}

		public static bool IsLeftSideOfAssignExpression(this SyntaxNode node)
		{
			return node.IsParentKind(SyntaxKind.SimpleAssignmentExpression) &&
				((AssignmentExpressionSyntax)node.Parent).Left == node;
		}

		public static bool IsLeftSideOfAnyAssignExpression(this SyntaxNode node)
		{
			return node.Parent.IsAnyAssignExpression() &&
				((AssignmentExpressionSyntax)node.Parent).Left == node;
		}

		public static bool IsRightSideOfAnyAssignExpression(this SyntaxNode node)
		{
			return node.Parent.IsAnyAssignExpression() &&
				((AssignmentExpressionSyntax)node.Parent).Right == node;
		}

		public static bool IsVariableDeclaratorValue(this SyntaxNode node)
		{
			return
				node.IsParentKind(SyntaxKind.EqualsValueClause) &&
				node.Parent.IsParentKind(SyntaxKind.VariableDeclarator) &&
				((EqualsValueClauseSyntax)node.Parent).Value == node;
		}

		public static BlockSyntax FindInnermostCommonBlock(this IEnumerable<SyntaxNode> nodes)
		{
			return nodes.FindInnermostCommonNode<BlockSyntax>();
		}

		public static IEnumerable<SyntaxNode> GetAncestorsOrThis(this SyntaxNode node, Func<SyntaxNode, bool> predicate)
		{
			var current = node;
			while (current != null)
			{
				if (predicate(current))
				{
					yield return current;
				}

				current = current.Parent;
			}
		}

		public static SyntaxNode GetParent(this SyntaxNode node)
		{
			return node != null ? node.Parent : null;
		}

		public static ValueTuple<SyntaxToken, SyntaxToken> GetBraces(this SyntaxNode node)
		{
			var namespaceNode = node as NamespaceDeclarationSyntax;
			if (namespaceNode != null)
			{
				return ValueTuple.Create(namespaceNode.OpenBraceToken, namespaceNode.CloseBraceToken);
			}

			var baseTypeNode = node as BaseTypeDeclarationSyntax;
			if (baseTypeNode != null)
			{
				return ValueTuple.Create(baseTypeNode.OpenBraceToken, baseTypeNode.CloseBraceToken);
			}

			var accessorListNode = node as AccessorListSyntax;
			if (accessorListNode != null)
			{
				return ValueTuple.Create(accessorListNode.OpenBraceToken, accessorListNode.CloseBraceToken);
			}

			var blockNode = node as BlockSyntax;
			if (blockNode != null)
			{
				return ValueTuple.Create(blockNode.OpenBraceToken, blockNode.CloseBraceToken);
			}

			var switchStatementNode = node as SwitchStatementSyntax;
			if (switchStatementNode != null)
			{
				return ValueTuple.Create(switchStatementNode.OpenBraceToken, switchStatementNode.CloseBraceToken);
			}

			var anonymousObjectCreationExpression = node as AnonymousObjectCreationExpressionSyntax;
			if (anonymousObjectCreationExpression != null)
			{
				return ValueTuple.Create(anonymousObjectCreationExpression.OpenBraceToken, anonymousObjectCreationExpression.CloseBraceToken);
			}

			var initializeExpressionNode = node as InitializerExpressionSyntax;
			if (initializeExpressionNode != null)
			{
				return ValueTuple.Create(initializeExpressionNode.OpenBraceToken, initializeExpressionNode.CloseBraceToken);
			}

			return new ValueTuple<SyntaxToken, SyntaxToken>();
		}

		public static SyntaxTokenList GetModifiers(this SyntaxNode member)
		{
			if (member != null)
			{
				switch (member.Kind())
				{
					case SyntaxKind.EnumDeclaration:
						return ((EnumDeclarationSyntax)member).Modifiers;
					case SyntaxKind.ClassDeclaration:
					case SyntaxKind.InterfaceDeclaration:
					case SyntaxKind.StructDeclaration:
						return ((TypeDeclarationSyntax)member).Modifiers;
					case SyntaxKind.DelegateDeclaration:
						return ((DelegateDeclarationSyntax)member).Modifiers;
					case SyntaxKind.FieldDeclaration:
						return ((FieldDeclarationSyntax)member).Modifiers;
					case SyntaxKind.EventFieldDeclaration:
						return ((EventFieldDeclarationSyntax)member).Modifiers;
					case SyntaxKind.ConstructorDeclaration:
						return ((ConstructorDeclarationSyntax)member).Modifiers;
					case SyntaxKind.DestructorDeclaration:
						return ((DestructorDeclarationSyntax)member).Modifiers;
					case SyntaxKind.PropertyDeclaration:
						return ((PropertyDeclarationSyntax)member).Modifiers;
					case SyntaxKind.EventDeclaration:
						return ((EventDeclarationSyntax)member).Modifiers;
					case SyntaxKind.IndexerDeclaration:
						return ((IndexerDeclarationSyntax)member).Modifiers;
					case SyntaxKind.OperatorDeclaration:
						return ((OperatorDeclarationSyntax)member).Modifiers;
					case SyntaxKind.ConversionOperatorDeclaration:
						return ((ConversionOperatorDeclarationSyntax)member).Modifiers;
					case SyntaxKind.MethodDeclaration:
						return ((MethodDeclarationSyntax)member).Modifiers;
					case SyntaxKind.GetAccessorDeclaration:
					case SyntaxKind.SetAccessorDeclaration:
					case SyntaxKind.AddAccessorDeclaration:
					case SyntaxKind.RemoveAccessorDeclaration:
						return ((AccessorDeclarationSyntax)member).Modifiers;
				}
			}

			return default(SyntaxTokenList);
		}

		public static SyntaxNode WithModifiers(this SyntaxNode member, SyntaxTokenList modifiers)
		{
			if (member != null)
			{
				switch (member.Kind())
				{
					case SyntaxKind.EnumDeclaration:
						return ((EnumDeclarationSyntax)member).WithModifiers(modifiers);
					case SyntaxKind.ClassDeclaration:
					case SyntaxKind.InterfaceDeclaration:
					case SyntaxKind.StructDeclaration:
						return ((TypeDeclarationSyntax)member).WithModifiers(modifiers);
					case SyntaxKind.DelegateDeclaration:
						return ((DelegateDeclarationSyntax)member).WithModifiers(modifiers);
					case SyntaxKind.FieldDeclaration:
						return ((FieldDeclarationSyntax)member).WithModifiers(modifiers);
					case SyntaxKind.EventFieldDeclaration:
						return ((EventFieldDeclarationSyntax)member).WithModifiers(modifiers);
					case SyntaxKind.ConstructorDeclaration:
						return ((ConstructorDeclarationSyntax)member).WithModifiers(modifiers);
					case SyntaxKind.DestructorDeclaration:
						return ((DestructorDeclarationSyntax)member).WithModifiers(modifiers);
					case SyntaxKind.PropertyDeclaration:
						return ((PropertyDeclarationSyntax)member).WithModifiers(modifiers);
					case SyntaxKind.EventDeclaration:
						return ((EventDeclarationSyntax)member).WithModifiers(modifiers);
					case SyntaxKind.IndexerDeclaration:
						return ((IndexerDeclarationSyntax)member).WithModifiers(modifiers);
					case SyntaxKind.OperatorDeclaration:
						return ((OperatorDeclarationSyntax)member).WithModifiers(modifiers);
					case SyntaxKind.ConversionOperatorDeclaration:
						return ((ConversionOperatorDeclarationSyntax)member).WithModifiers(modifiers);
					case SyntaxKind.MethodDeclaration:
						return ((MethodDeclarationSyntax)member).WithModifiers(modifiers);
					case SyntaxKind.GetAccessorDeclaration:
					case SyntaxKind.SetAccessorDeclaration:
					case SyntaxKind.AddAccessorDeclaration:
					case SyntaxKind.RemoveAccessorDeclaration:
						return ((AccessorDeclarationSyntax)member).WithModifiers(modifiers);
				}
			}

			return null;
		}

		public static TypeDeclarationSyntax WithModifiers(
			this TypeDeclarationSyntax node, SyntaxTokenList modifiers)
		{
			switch (node.Kind())
			{
				case SyntaxKind.ClassDeclaration:
					return ((ClassDeclarationSyntax)node).WithModifiers(modifiers);
				case SyntaxKind.InterfaceDeclaration:
					return ((InterfaceDeclarationSyntax)node).WithModifiers(modifiers);
				case SyntaxKind.StructDeclaration:
					return ((StructDeclarationSyntax)node).WithModifiers(modifiers);
			}

			throw new InvalidOperationException();
		}

		public static bool CheckTopLevel(this SyntaxNode node, TextSpan span)
		{
			var block = node as BlockSyntax;
			if (block != null)
			{
				return block.ContainsInBlockBody(span);
			}

			var field = node as FieldDeclarationSyntax;
			if (field != null)
			{
				foreach (var variable in field.Declaration.Variables)
				{
					if (variable.Initializer != null && variable.Initializer.Span.Contains(span))
					{
						return true;
					}
				}
			}

			var global = node as GlobalStatementSyntax;
			if (global != null)
			{
				return true;
			}

			var constructorInitializer = node as ConstructorInitializerSyntax;
			if (constructorInitializer != null)
			{
				return constructorInitializer.ContainsInArgument(span);
			}

			return false;
		}

		public static bool ContainsInArgument(this ConstructorInitializerSyntax initializer, TextSpan textSpan)
		{
			if (initializer == null)
			{
				return false;
			}

			return initializer.ArgumentList.Arguments.Any(a => a.Span.Contains(textSpan));
		}

		public static bool ContainsInBlockBody(this BlockSyntax block, TextSpan textSpan)
		{
			if (block == null)
			{
				return false;
			}

			var blockSpan = TextSpan.FromBounds(block.OpenBraceToken.Span.End, block.CloseBraceToken.SpanStart);
			return blockSpan.Contains(textSpan);
		}

		public static bool IsDelegateOrConstructorOrMethodParameterList(this SyntaxNode node)
		{
			if (!node.IsKind(SyntaxKind.ParameterList))
			{
				return false;
			}

			return
				node.IsParentKind(SyntaxKind.MethodDeclaration) ||
				node.IsParentKind(SyntaxKind.ConstructorDeclaration) ||
				node.IsParentKind(SyntaxKind.DelegateDeclaration);
		}

	}
}
