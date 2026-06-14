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

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Syntax;

namespace ICSharpCode.Decompiler.CSharp.OutputVisitor
{
	class InsertMissingTokensDecorator : DecoratingTokenWriter
	{
		readonly Stack<List<AstNode>> nodes = new Stack<List<AstNode>>();
		List<AstNode> currentList;
		readonly ILocatable locationProvider;

		// Nodes that have been started but whose first token has not been written yet. The start
		// location of a node is the position of its first printed token (not the StartNode position,
		// which precedes any leading newline/indentation), so it is assigned lazily on the next write.
		readonly List<AstNode> nodesAwaitingStart = new List<AstNode>();

		// Position immediately after the most recently written token. A node's end location is the end
		// of its last token (not the EndNode position, which follows the trailing newline/indentation).
		TextLocation lastTokenEnd;

		public InsertMissingTokensDecorator(TokenWriter writer, ILocatable locationProvider)
			: base(writer)
		{
			this.locationProvider = locationProvider;
			currentList = new List<AstNode>();
		}

		void AssignPendingStartLocations()
		{
			if (nodesAwaitingStart.Count == 0)
				return;
			TextLocation location = locationProvider.Location;
			foreach (var node in nodesAwaitingStart)
				node.StorePrintStart(location);
			nodesAwaitingStart.Clear();
		}

		public override void StartNode(AstNode node)
		{
			// ignore whitespace: these don't need to be processed.
			// StartNode/EndNode is only called for them to support folding of comments.
			if (node.NodeType != NodeType.Whitespace)
			{
				currentList.Add(node);
				nodes.Push(currentList);
				currentList = new List<AstNode>();
				nodesAwaitingStart.Add(node);
			}
			else if (node is Comment comment)
			{
				comment.SetStartLocation(locationProvider.Location);
			}
			if (node is ErrorExpression error)
			{
				error.Location = locationProvider.Location;
			}
			base.StartNode(node);
		}

		public override void EndNode(AstNode node)
		{
			// ignore whitespace: these don't need to be processed.
			// StartNode/EndNode is only called for them to support folding of comments.
			if (node.NodeType != NodeType.Whitespace)
			{
				// A node that printed no tokens of its own collapses to a zero-width span here.
				if (nodesAwaitingStart.Remove(node))
					node.StorePrintStart(lastTokenEnd);
				node.StorePrintEnd(lastTokenEnd);
				System.Diagnostics.Debug.Assert(currentList != null);
				foreach (var child in currentList)
				{
					System.Diagnostics.Debug.Assert(child.Parent == null || node == child.Parent);
					child.Remove();
					node.AddChildWithExistingRole(child);
				}
				currentList = nodes.Pop();
			}
			else if (node is Comment comment)
			{
				comment.SetEndLocation(locationProvider.Location);
			}
			base.EndNode(node);
		}

		public override void WriteToken(TokenRole role, string token)
		{
			AssignPendingStartLocations();
			switch (nodes.Peek().LastOrDefault())
			{
				case EmptyStatement emptyStatement:
					emptyStatement.Location = locationProvider.Location;
					break;
				case ErrorExpression errorExpression:
					errorExpression.Location = locationProvider.Location;
					break;
			}
			base.WriteToken(role, token);
			lastTokenEnd = locationProvider.Location;
		}

		public override void WriteKeyword(TokenRole role, string keyword)
		{
			AssignPendingStartLocations();
			TextLocation start = locationProvider.Location;
			if (keyword == "this")
			{
				ThisReferenceExpression node = nodes.Peek().LastOrDefault() as ThisReferenceExpression;
				if (node != null)
					node.Location = start;
			}
			else if (keyword == "base")
			{
				BaseReferenceExpression node = nodes.Peek().LastOrDefault() as BaseReferenceExpression;
				if (node != null)
					node.Location = start;
			}
			base.WriteKeyword(role, keyword);
			lastTokenEnd = locationProvider.Location;
		}

		public override void WriteIdentifier(Identifier identifier)
		{
			AssignPendingStartLocations();
			if (!identifier.IsNull)
				identifier.SetStartLocation(locationProvider.Location);
			currentList.Add(identifier);
			base.WriteIdentifier(identifier);
			lastTokenEnd = locationProvider.Location;
		}

		public override void WritePrimitiveValue(object value, LiteralFormat format = LiteralFormat.None)
		{
			AssignPendingStartLocations();
			Expression node = nodes.Peek().LastOrDefault() as Expression;
			var startLocation = locationProvider.Location;
			base.WritePrimitiveValue(value, format);
			if (node is PrimitiveExpression)
			{
				((PrimitiveExpression)node).SetLocation(startLocation, locationProvider.Location);
			}
			if (node is NullReferenceExpression)
			{
				((NullReferenceExpression)node).SetStartLocation(startLocation);
			}
			lastTokenEnd = locationProvider.Location;
		}

		public override void WritePrimitiveType(string type)
		{
			AssignPendingStartLocations();
			PrimitiveType node = nodes.Peek().LastOrDefault() as PrimitiveType;
			if (node != null)
				node.SetStartLocation(locationProvider.Location);
			base.WritePrimitiveType(type);
			lastTokenEnd = locationProvider.Location;
		}
	}
}
