// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using System.Linq;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.PatternMatching
{
	/// <summary>
	/// Substrate-agnostic tests of the pattern-matching engine: single-node matching with captures,
	/// the special pattern nodes (AnyNode, Choice, Repeat, OptionalNode, the back-references), and the
	/// collection backtracking core. They are written against the public matching API so they hold
	/// across the engine's storage model.
	/// </summary>
	[TestFixture]
	public class PatternMatchingTests
	{
		static ExpressionStatement Stmt(string identifier)
		{
			return new ExpressionStatement(new IdentifierExpression(identifier));
		}

		static BlockStatement Block(params Statement[] statements)
		{
			var block = new BlockStatement();
			foreach (var s in statements)
				block.Add(s);
			return block;
		}

		#region single node
		[Test]
		public void IdenticalExpressions_Match()
		{
			Assert.That(new IdentifierExpression("x").IsMatch(new IdentifierExpression("x")), Is.True);
		}

		[Test]
		public void DifferentExpressions_DoNotMatch()
		{
			Assert.That(new IdentifierExpression("x").IsMatch(new IdentifierExpression("y")), Is.False);
		}

		[Test]
		public void AnyNode_MatchesNonNull_RejectsNull()
		{
			Assert.That(new AnyNode().IsMatch(new IdentifierExpression("x")), Is.True);
			Assert.That(new AnyNode().IsMatch(null), Is.False);
		}

		[Test]
		public void AnyNodeOrNull_MatchesNull()
		{
			Assert.That(new AnyNodeOrNull().IsMatch(null), Is.True);
			Assert.That(new AnyNodeOrNull().IsMatch(new IdentifierExpression("x")), Is.True);
		}

		[Test]
		public void NamedNode_CapturesTheMatchedNode()
		{
			var pattern = new NamedNode("id", new AnyNode());
			var target = new IdentifierExpression("foo");
			var m = pattern.Match(target);
			Assert.That(m.Success, Is.True);
			Assert.That(m.Get("id").Single(), Is.SameAs(target));
		}
		#endregion

		#region Choice
		[Test]
		public void Choice_MatchesAnyAlternative()
		{
			var choice = new Choice { new IdentifierExpression("a"), new IdentifierExpression("b") };
			Assert.That(choice.IsMatch(new IdentifierExpression("a")), Is.True);
			Assert.That(choice.IsMatch(new IdentifierExpression("b")), Is.True);
			Assert.That(choice.IsMatch(new IdentifierExpression("c")), Is.False);
		}

		[Test]
		public void Choice_DoesNotLeakCapturesFromFailedAlternative()
		{
			// The first alternative captures "g" then fails on the operator; the checkpoint must be
			// restored so the surviving match carries only the second alternative's capture.
			var choice = new Choice {
				new BinaryOperatorExpression {
					Left = new NamedNode("g", new IdentifierExpression("a")),
					Operator = BinaryOperatorType.Add,
					Right = new AnyNode()
				},
				new NamedNode("g", new IdentifierExpression("z"))
			};
			var m = choice.Match(new IdentifierExpression("z"));
			Assert.That(m.Success, Is.True);
			Assert.That(m.Get("g").Count(), Is.EqualTo(1));
			Assert.That(((IdentifierExpression)m.Get("g").Single()).Identifier, Is.EqualTo("z"));
		}
		#endregion

		#region back-references
		[Test]
		public void Backreference_MatchesEqualNode()
		{
			var pattern = new BinaryOperatorExpression {
				Left = new NamedNode("x", new AnyNode()),
				Operator = BinaryOperatorType.Any,
				Right = new Backreference("x")
			};
			Assert.That(pattern.IsMatch(new BinaryOperatorExpression(
				new IdentifierExpression("a"), BinaryOperatorType.Add, new IdentifierExpression("a"))), Is.True);
			Assert.That(pattern.IsMatch(new BinaryOperatorExpression(
				new IdentifierExpression("a"), BinaryOperatorType.Add, new IdentifierExpression("b"))), Is.False);
		}

		[Test]
		public void IdentifierExpressionBackreference_MatchesSameName()
		{
			var pattern = new BinaryOperatorExpression {
				Left = new NamedNode("v", new IdentifierExpression(Pattern.AnyString)),
				Operator = BinaryOperatorType.Any,
				Right = new IdentifierExpressionBackreference("v")
			};
			Assert.That(pattern.IsMatch(new BinaryOperatorExpression(
				new IdentifierExpression("i"), BinaryOperatorType.Add, new IdentifierExpression("i"))), Is.True);
			Assert.That(pattern.IsMatch(new BinaryOperatorExpression(
				new IdentifierExpression("i"), BinaryOperatorType.Add, new IdentifierExpression("j"))), Is.False);
		}

		[Test]
		public void IdentifierExpressionBackreference_RejectsTypeArguments()
		{
			var pattern = new BinaryOperatorExpression {
				Left = new NamedNode("v", new IdentifierExpression(Pattern.AnyString)),
				Operator = BinaryOperatorType.Any,
				Right = new IdentifierExpressionBackreference("v")
			};
			var rightWithTypeArgs = new IdentifierExpression("i");
			rightWithTypeArgs.TypeArguments.Add(new SimpleType("T"));
			Assert.That(pattern.IsMatch(new BinaryOperatorExpression(
				new IdentifierExpression("i"), BinaryOperatorType.Add, rightWithTypeArgs)), Is.False);
		}
		#endregion

		#region collections
		[Test]
		public void EmptyCollections_Match()
		{
			Assert.That(Block().IsMatch(Block()), Is.True);
		}

		[Test]
		public void Repeat_GreedilyMatchesAll()
		{
			var pattern = new BlockStatement { Statements = { new Repeat(new AnyNode("s")) } };
			var m = pattern.Match(Block(Stmt("a"), Stmt("b"), Stmt("c")));
			Assert.That(m.Success, Is.True);
			Assert.That(m.Get("s").Count(), Is.EqualTo(3));
		}

		[Test]
		public void Repeat_MatchesEmptyWhenMinZero()
		{
			var pattern = new BlockStatement { Statements = { new Repeat(new AnyNode()) } };
			Assert.That(pattern.IsMatch(Block()), Is.True);
		}

		[Test]
		public void Repeat_MinCountNotSatisfied_DoesNotMatch()
		{
			var pattern = new BlockStatement { Statements = { new Repeat(new AnyNode()) { MinCount = 2 } } };
			Assert.That(pattern.IsMatch(Block(Stmt("a"))), Is.False);
			Assert.That(pattern.IsMatch(Block(Stmt("a"), Stmt("b"))), Is.True);
		}

		[Test]
		public void Repeat_MaxCountLeavesTrailingUnmatched_DoesNotMatch()
		{
			var pattern = new BlockStatement { Statements = { new Repeat(new AnyNode()) { MaxCount = 1 } } };
			Assert.That(pattern.IsMatch(Block(Stmt("a"))), Is.True);
			Assert.That(pattern.IsMatch(Block(Stmt("a"), Stmt("b"))), Is.False);
		}

		[Test]
		public void Repeat_BacktracksToSatisfyTrailingFixedNode()
		{
			var pattern = new BlockStatement {
				Statements = {
					new Repeat(new AnyNode("body")),
					new NamedNode("last", Stmt("end"))
				}
			};
			var m = pattern.Match(Block(Stmt("a"), Stmt("b"), Stmt("end")));
			Assert.That(m.Success, Is.True);
			Assert.That(m.Get("body").Count(), Is.EqualTo(2));
			Assert.That(((IdentifierExpression)((ExpressionStatement)m.Get("last").Single()).Expression).Identifier,
				Is.EqualTo("end"));
		}

		[Test]
		public void Optional_MatchesWhetherPresentOrAbsent()
		{
			var pattern = new BlockStatement {
				Statements = {
					new OptionalNode(Stmt("opt")),
					new NamedNode("tail", Stmt("tail"))
				}
			};
			Assert.That(pattern.IsMatch(Block(Stmt("opt"), Stmt("tail"))), Is.True);
			Assert.That(pattern.IsMatch(Block(Stmt("tail"))), Is.True);
			Assert.That(pattern.IsMatch(Block(Stmt("other"), Stmt("tail"))), Is.False);
		}

		[Test]
		public void FixedLengthCollection_LengthMismatch_DoesNotMatch()
		{
			var pattern = Block(Stmt("a"), Stmt("b"));
			Assert.That(pattern.IsMatch(Block(Stmt("a"), Stmt("b"))), Is.True);
			Assert.That(pattern.IsMatch(Block(Stmt("a"), Stmt("b"), Stmt("c"))), Is.False);
			Assert.That(pattern.IsMatch(Block(Stmt("a"))), Is.False);
		}
		#endregion

		#region nesting
		[Test]
		public void NestedPattern_MatchesAndCaptures()
		{
			var pattern = new BinaryOperatorExpression {
				Left = new BinaryOperatorExpression {
					Left = new NamedNode("a", new AnyNode()),
					Operator = BinaryOperatorType.Multiply,
					Right = new NamedNode("b", new AnyNode())
				},
				Operator = BinaryOperatorType.Add,
				Right = new NamedNode("c", new AnyNode())
			};
			var target = new BinaryOperatorExpression(
				new BinaryOperatorExpression(new IdentifierExpression("x"), BinaryOperatorType.Multiply, new IdentifierExpression("y")),
				BinaryOperatorType.Add,
				new IdentifierExpression("z"));
			var m = pattern.Match(target);
			Assert.That(m.Success, Is.True);
			Assert.That(((IdentifierExpression)m.Get("a").Single()).Identifier, Is.EqualTo("x"));
			Assert.That(((IdentifierExpression)m.Get("b").Single()).Identifier, Is.EqualTo("y"));
			Assert.That(((IdentifierExpression)m.Get("c").Single()).Identifier, Is.EqualTo("z"));
		}
		#endregion
	}
}
