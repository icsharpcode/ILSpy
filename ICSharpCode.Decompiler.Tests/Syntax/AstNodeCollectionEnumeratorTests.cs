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

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Syntax;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Syntax
{
	/// <summary>
	/// Pins the mutation-during-enumeration contract of <see cref="AstNodeCollection{T}.Enumerator"/>:
	/// the cursor follows node identity hand-over-hand (like the old linked-list enumerator), so a transform
	/// may remove or replace the current element mid-foreach, and inserting or removing other elements only
	/// shifts the backing-list indices without skipping or re-yielding a surviving element. Each case also
	/// exercises the enumerator's DEBUG position guard, which asserts when the cursor mislocates.
	/// </summary>
	[TestFixture]
	public class AstNodeCollectionEnumeratorTests
	{
		static ExpressionStatement Stmt(string name) => new ExpressionStatement(new IdentifierExpression(name));

		static string NameOf(Statement statement) => ((IdentifierExpression)((ExpressionStatement)statement).Expression).Identifier;

		// Walks the collection, invoking 'mutate' once when the named statement is the current element, and
		// returns the names yielded in order. The foreach uses the struct enumerator under test.
		static List<string> EnumerateWith(BlockStatement block, string mutateAt, System.Action<BlockStatement, Statement> mutate)
		{
			var visited = new List<string>();
			foreach (Statement statement in block.Statements)
			{
				visited.Add(NameOf(statement));
				if (mutate != null && NameOf(statement) == mutateAt)
					mutate(block, statement);
			}
			return visited;
		}

		static BlockStatement Block(params string[] names)
		{
			var block = new BlockStatement();
			foreach (string name in names)
				block.Statements.Add(Stmt(name));
			return block;
		}

		[Test]
		public void PlainEnumeration_YieldsAllInOrder()
		{
			var block = Block("a", "b", "c", "d");
			Assert.That(EnumerateWith(block, null, null), Is.EqualTo(new[] { "a", "b", "c", "d" }));
		}

		[Test]
		public void RemovingCurrent_DoesNotSkipTheSuccessor()
		{
			var block = Block("a", "b", "c", "d");
			var visited = EnumerateWith(block, "a", (b, current) => current.Remove());
			Assert.That(visited, Is.EqualTo(new[] { "a", "b", "c", "d" }));
			Assert.That(block.Statements.Select(NameOf), Is.EqualTo(new[] { "b", "c", "d" }));
		}

		[Test]
		public void ReplacingCurrent_AdvancesPastTheReplacement()
		{
			var block = Block("a", "b", "c", "d");
			var visited = EnumerateWith(block, "b", (b, current) => current.ReplaceWith(Stmt("b2")));
			// The replacement takes the current slot but is not re-visited.
			Assert.That(visited, Is.EqualTo(new[] { "a", "b", "c", "d" }));
			Assert.That(block.Statements.Select(NameOf), Is.EqualTo(new[] { "a", "b2", "c", "d" }));
		}

		[Test]
		public void RemovingAnAlreadyVisitedElement_DoesNotDisturbTheRemainder()
		{
			var block = Block("a", "b", "c", "d");
			// Removing 'a' while 'c' is current shifts 'd' down one index; the identity resume absorbs it.
			var visited = EnumerateWith(block, "c", (b, current) => b.Statements.First(s => NameOf(s) == "a").Remove());
			Assert.That(visited, Is.EqualTo(new[] { "a", "b", "c", "d" }));
		}

		[Test]
		public void InsertingBeforeTheCursor_DoesNotSkipSurvivingElements()
		{
			var block = Block("a", "b", "c", "d");
			// Inserting 'x' before the current 'b' shifts every later index up one; the cursor still resumes
			// on its captured successor 'c'. The freshly inserted node is not visited (it is behind the cursor).
			var visited = EnumerateWith(block, "b", (b, current) => b.Statements.InsertBefore((Statement)current, Stmt("x")));
			Assert.That(visited, Is.EqualTo(new[] { "a", "b", "c", "d" }));
			Assert.That(block.Statements.Select(NameOf), Is.EqualTo(new[] { "a", "x", "b", "c", "d" }));
		}

		[Test]
		public void RemovingTheCapturedSuccessor_StopsEnumeration()
		{
			var block = Block("a", "b", "c", "d");
			// 'c' is the successor captured when 'b' is yielded; removing it leaves the cursor with no node to
			// resume on, so enumeration stops -- the one mutation the identity walk cannot follow.
			var visited = EnumerateWith(block, "b", (b, current) => b.Statements.First(s => NameOf(s) == "c").Remove());
			Assert.That(visited, Is.EqualTo(new[] { "a", "b" }));
		}
	}
}
