// Copyright (c) 2026 Siegfried Pammer
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
using System.Linq;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Syntax
{
	/// <summary>
	/// Trivia lives outside the child-slot space: it is reachable only through
	/// <see cref="AstNode.LeadingTrivia"/>/<see cref="AstNode.TrailingTrivia"/>, navigates via
	/// Next/PrevSibling within its owning list, and never leaks into the owner's child space.
	/// </summary>
	[TestFixture]
	public class TriviaTests
	{
		static (SyntaxTree tree, ExpressionStatement stmt) MakeTreeWithStatement()
		{
			var stmt = new ExpressionStatement(new IdentifierExpression("x"));
			var tree = new SyntaxTree();
			tree.Members.Add(stmt);
			return (tree, stmt);
		}

		[Test]
		public void Attached_Trivia_Navigates_Within_Its_List()
		{
			var (_, stmt) = MakeTreeWithStatement();
			var a = new Comment(" a");
			var b = new Comment(" b");
			var c = new Comment(" c");
			stmt.AddLeadingTrivia(a);
			stmt.AddLeadingTrivia(b);
			stmt.AddLeadingTrivia(c);

			Assert.That(a.Parent, Is.SameAs(stmt));
			Assert.That(a.NextSibling, Is.SameAs(b));
			Assert.That(b.NextSibling, Is.SameAs(c));
			Assert.That(c.NextSibling, Is.Null);
			Assert.That(c.PrevSibling, Is.SameAs(b));
			Assert.That(a.PrevSibling, Is.Null);
			Assert.That(a.Slot, Is.Null);
		}

		[Test]
		public void Prepend_Reindexes_Existing_Trivia()
		{
			var (_, stmt) = MakeTreeWithStatement();
			var first = new Comment(" first");
			var second = new Comment(" second");
			stmt.AddLeadingTrivia(second);
			stmt.PrependLeadingTrivia(first);

			Assert.That(stmt.LeadingTrivia.ToArray(), Is.EqualTo(new[] { first, second }));
			Assert.That(first.NextSibling, Is.SameAs(second));
			Assert.That(second.PrevSibling, Is.SameAs(first));
			Assert.That(second.NextSibling, Is.Null);
		}

		[Test]
		public void Remove_Detaches_And_Reindexes_Remaining_Trivia()
		{
			var (_, stmt) = MakeTreeWithStatement();
			var a = new Comment(" a");
			var b = new Comment(" b");
			var c = new Comment(" c");
			stmt.AddLeadingTrivia(a);
			stmt.AddLeadingTrivia(b);
			stmt.AddLeadingTrivia(c);

			b.Remove();

			Assert.That(b.Parent, Is.Null);
			Assert.That(stmt.LeadingTrivia.ToArray(), Is.EqualTo(new[] { a, c }));
			Assert.That(a.NextSibling, Is.SameAs(c));
			Assert.That(c.PrevSibling, Is.SameAs(a));
		}

		[Test]
		public void Removed_Trivia_Can_Be_Reattached()
		{
			var (_, stmt) = MakeTreeWithStatement();
			var comment = new Comment(" migrating");
			stmt.AddLeadingTrivia(comment);
			comment.Remove();

			var other = new ExpressionStatement(new IdentifierExpression("y"));
			other.AddTrailingTrivia(comment);

			Assert.That(comment.Parent, Is.SameAs(other));
			Assert.That(other.TrailingTrivia.Single(), Is.SameAs(comment));
		}

		[Test]
		public void Adding_Attached_Trivia_Elsewhere_Throws()
		{
			var (_, stmt) = MakeTreeWithStatement();
			var comment = new Comment(" taken");
			stmt.AddLeadingTrivia(comment);

			var other = new ExpressionStatement(new IdentifierExpression("y"));
			Assert.Throws<ArgumentException>(() => other.AddLeadingTrivia(comment));
		}

		[Test]
		public void ReplaceWith_On_Attached_Trivia_Throws()
		{
			var (_, stmt) = MakeTreeWithStatement();
			var comment = new Comment(" doc", CommentType.Documentation);
			stmt.AddLeadingTrivia(comment);

			Assert.Throws<InvalidOperationException>(() => comment.ReplaceWith(new Comment(" other")));
			// The failed replacement must not have touched the owning statement's real children.
			Assert.That(stmt.Expression, Is.InstanceOf<IdentifierExpression>());
			Assert.That(stmt.LeadingTrivia.Single(), Is.SameAs(comment));
		}

		[Test]
		public void ReplaceWith_Function_On_Attached_Trivia_Throws()
		{
			var (_, stmt) = MakeTreeWithStatement();
			var comment = new Comment(" doc", CommentType.Documentation);
			stmt.AddLeadingTrivia(comment);

			Assert.Throws<InvalidOperationException>(() => comment.ReplaceWith(_ => new Comment(" other")));
			Assert.That(stmt.LeadingTrivia.Single(), Is.SameAs(comment));
		}

		[Test]
		public void GetNextNode_Stays_Within_Trivia_Space()
		{
			var (tree, stmt) = MakeTreeWithStatement();
			var second = new ExpressionStatement(new IdentifierExpression("y"));
			tree.Members.Add(second);
			var a = new Comment(" a");
			var b = new Comment(" b");
			stmt.AddLeadingTrivia(a);
			stmt.AddLeadingTrivia(b);

			Assert.That(a.GetNextNode(), Is.SameAs(b));
			// Leading trivia precedes its owner, so continuing past the end of the trivia list into
			// the owner's sibling space would skip the owner's whole subtree.
			Assert.That(b.GetNextNode(), Is.Null);
			Assert.That(a.GetPrevNode(), Is.Null);
		}

		[Test]
		public void Clone_Of_Owner_Reparents_Cloned_Trivia()
		{
			var (_, stmt) = MakeTreeWithStatement();
			var a = new Comment(" a");
			var b = new Comment(" b");
			stmt.AddLeadingTrivia(a);
			stmt.AddLeadingTrivia(b);

			var copy = (ExpressionStatement)stmt.Clone();
			var copiedTrivia = copy.LeadingTrivia.ToArray();

			Assert.That(copiedTrivia, Has.Length.EqualTo(2));
			Assert.That(copiedTrivia[0], Is.Not.SameAs(a));
			Assert.That(copiedTrivia[0].Parent, Is.SameAs(copy));
			Assert.That(copiedTrivia[0].NextSibling, Is.SameAs(copiedTrivia[1]));
			// The original's trivia must be untouched.
			Assert.That(a.Parent, Is.SameAs(stmt));
			Assert.That(a.NextSibling, Is.SameAs(b));
		}

		[Test]
		public void Clone_Of_Attached_Trivia_Is_Fully_Detached()
		{
			var (_, stmt) = MakeTreeWithStatement();
			var a = new Comment(" a");
			var b = new Comment(" b");
			stmt.AddLeadingTrivia(a);
			stmt.AddLeadingTrivia(b);

			var clone = (Comment)a.Clone();

			Assert.That(clone.Parent, Is.Null);
			Assert.That(clone.triviaSiblings, Is.Null);

			// A detached clone attached as a regular child must navigate in child space, not in the
			// source node's trivia list.
			var host = new SyntaxTree();
			host.Members.Add(clone);
			Assert.That(clone.NextSibling, Is.Null);
			Assert.That(clone.PrevSibling, Is.Null);
			clone.Remove();
			Assert.That(stmt.LeadingTrivia.ToArray(), Is.EqualTo(new[] { a, b }));
		}

		[Test]
		public void CopyAnnotationsFrom_Clones_Trivia_Instead_Of_Sharing()
		{
			var source = new ExpressionStatement(new IdentifierExpression("x"));
			var sourceComment = new Comment(" note");
			source.AddTrailingTrivia(sourceComment);

			var target = new ExpressionStatement(new IdentifierExpression("y"));
			target.CopyAnnotationsFrom(source);

			var targetComment = target.TrailingTrivia.Single();
			Assert.That(targetComment, Is.Not.SameAs(sourceComment));
			Assert.That(targetComment.Parent, Is.SameAs(target));
			Assert.That(sourceComment.Parent, Is.SameAs(source));

			// The lists must be independent: mutating one node's trivia must not affect the other.
			target.AddTrailingTrivia(new Comment(" extra"));
			Assert.That(source.TrailingTrivia.Count(), Is.EqualTo(1));
			targetComment.Remove();
			Assert.That(source.TrailingTrivia.Single(), Is.SameAs(sourceComment));
		}
	}
}
