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

using System.IO;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Output
{
	[TestFixture]
	public class InsertParenthesesVisitorTests
	{
		CSharpFormattingOptions policy;

		[SetUp]
		public void SetUp()
		{
			policy = FormattingOptionsFactory.CreateMono();
		}

		string InsertReadable(Expression expr)
		{
			expr = expr.Clone();
			expr.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
			StringWriter w = new StringWriter();
			w.NewLine = " ";
			expr.AcceptVisitor(new CSharpOutputVisitor(new TextWriterTokenWriter(w) { IndentationString = "" }, policy));
			return w.ToString();
		}

		string InsertRequired(Expression expr)
		{
			expr = expr.Clone();
			expr.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = false });
			StringWriter w = new StringWriter();
			w.NewLine = " ";
			expr.AcceptVisitor(new CSharpOutputVisitor(new TextWriterTokenWriter(w) { IndentationString = "" }, policy));
			return w.ToString();
		}

		[Test]
		public void EqualityInAssignment()
		{
			Expression expr = new AssignmentExpression(
				new IdentifierExpression("cond"),
				new BinaryOperatorExpression(
					new IdentifierExpression("a"),
					BinaryOperatorType.Equality,
					new IdentifierExpression("b")
				)
			);

			Assert.That(InsertRequired(expr), Is.EqualTo("cond = a == b"));
			Assert.That(InsertReadable(expr), Is.EqualTo("cond = a == b"));
		}

		[Test]
		public void LambdaInAssignment()
		{
			Expression expr = new AssignmentExpression(
				new IdentifierExpression("p"),
				new LambdaExpression {
					Body = new BinaryOperatorExpression(
						new IdentifierExpression("a"),
						BinaryOperatorType.Add,
						new IdentifierExpression("b")
					)
				}
			);

			Assert.That(InsertRequired(expr), Is.EqualTo("p = () => a + b"));
			Assert.That(InsertReadable(expr), Is.EqualTo("p = () => a + b"));
		}

		[Test]
		public void LambdaInDelegateAdditionRHS()
		{
			Expression expr = new BinaryOperatorExpression {
				Left = new IdentifierExpression("p"),
				Operator = BinaryOperatorType.Add,
				Right = new LambdaExpression {
					Body = new BinaryOperatorExpression(
						new IdentifierExpression("a"),
						BinaryOperatorType.Add,
						new IdentifierExpression("b")
					)
				}
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("p + () => a + b"));
			Assert.That(InsertReadable(expr), Is.EqualTo("p + (() => a + b)"));
		}

		[Test]
		public void LambdaInDelegateAdditionLHS()
		{
			Expression expr = new BinaryOperatorExpression {
				Left = new LambdaExpression {
					Body = new BinaryOperatorExpression(
						new IdentifierExpression("a"),
						BinaryOperatorType.Add,
						new IdentifierExpression("b")
					)
				},
				Operator = BinaryOperatorType.Add,
				Right = new IdentifierExpression("p"),
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("(() => a + b) + p"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(() => a + b) + p"));
		}

		[Test]
		public void TrickyCast1()
		{
			Expression expr = new CastExpression {
				Type = new PrimitiveType("int"),
				Expression = new UnaryOperatorExpression(
					UnaryOperatorType.Minus, new IdentifierExpression("a")
				)
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("(int)-a"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(int)(-a)"));
		}

		[Test]
		public void TrickyCast2()
		{
			Expression expr = new CastExpression {
				Type = new SimpleType("MyType"),
				Expression = new UnaryOperatorExpression(
					UnaryOperatorType.Minus, new IdentifierExpression("a")
				)
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("(MyType)(-a)"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(MyType)(-a)"));
		}

		[Test]
		public void TrickyCast3()
		{
			Expression expr = new CastExpression {
				Type = new SimpleType("MyType"),
				Expression = new UnaryOperatorExpression(
					UnaryOperatorType.Not, new IdentifierExpression("a")
				)
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("(MyType)!a"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(MyType)(!a)"));
		}

		[Test]
		public void TrickyCast4()
		{
			Expression expr = new CastExpression {
				Type = new SimpleType("MyType"),
				Expression = new PrimitiveExpression(int.MinValue),
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("(MyType)(-2147483648)"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(MyType)(-2147483648)"));
		}

		[Test]
		public void TrickyCast5()
		{
			Expression expr = new CastExpression {
				Type = new SimpleType("MyType"),
				Expression = new PrimitiveExpression(-1.0),
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("(MyType)(-1.0)"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(MyType)(-1.0)"));
		}

		[Test]
		public void TrickyCast6()
		{
			Expression expr = new CastExpression {
				Type = new PrimitiveType("double"),
				Expression = new PrimitiveExpression(int.MinValue),
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("(double)-2147483648"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(double)(-2147483648)"));
		}

		[Test]
		public void CastAndInvoke()
		{
			Expression expr = new MemberReferenceExpression {
				Target = new CastExpression {
					Type = new PrimitiveType("string"),
					Expression = new IdentifierExpression("a")
				},
				MemberName = "Length"
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("((string)a).Length"));
			Assert.That(InsertReadable(expr), Is.EqualTo("((string)a).Length"));
		}

		[Test]
		public void DoubleNegation()
		{
			Expression expr = new UnaryOperatorExpression(
				UnaryOperatorType.Minus,
				new UnaryOperatorExpression(UnaryOperatorType.Minus, new IdentifierExpression("a"))
			);

			Assert.That(InsertRequired(expr), Is.EqualTo("- -a"));
			Assert.That(InsertReadable(expr), Is.EqualTo("-(-a)"));
		}

		[Test]
		public void AdditionWithConditional()
		{
			Expression expr = new BinaryOperatorExpression {
				Left = new IdentifierExpression("a"),
				Operator = BinaryOperatorType.Add,
				Right = new ConditionalExpression {
					Condition = new BinaryOperatorExpression {
						Left = new IdentifierExpression("b"),
						Operator = BinaryOperatorType.Equality,
						Right = new PrimitiveExpression(null)
					},
					TrueExpression = new IdentifierExpression("c"),
					FalseExpression = new IdentifierExpression("d")
				}
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("a + (b == null ? c : d)"));
			Assert.That(InsertReadable(expr), Is.EqualTo("a + ((b == null) ? c : d)"));
		}

		[Test]
		public void TypeTestInConditional()
		{
			Expression expr = new ConditionalExpression {
				Condition = new IsExpression {
					Expression = new IdentifierExpression("a"),
					Type = new ComposedType {
						BaseType = new PrimitiveType("int"),
						HasNullableSpecifier = true
					}
				},
				TrueExpression = new IdentifierExpression("b"),
				FalseExpression = new IdentifierExpression("c")
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("a is int? ? b : c"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(a is int?) ? b : c"));

			policy.SpaceBeforeConditionalOperatorCondition = false;
			policy.SpaceAfterConditionalOperatorCondition = false;
			policy.SpaceBeforeConditionalOperatorSeparator = false;
			policy.SpaceAfterConditionalOperatorSeparator = false;

			Assert.That(InsertRequired(expr), Is.EqualTo("a is int? ?b:c"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(a is int?)?b:c"));
		}

		[Test]
		public void MethodCallOnQueryExpression()
		{
			Expression expr = new InvocationExpression(new MemberReferenceExpression {
				Target = new QueryExpression {
					Clauses = {
					new QueryFromClause {
						Identifier = "a",
						Expression = new IdentifierExpression("b")
					},
					new QuerySelectClause {
						Expression = new InvocationExpression(new MemberReferenceExpression {
							Target = new IdentifierExpression("a"),
							MemberName = "c"
						})
					}
				}
				},
				MemberName = "ToArray"
			});

			Assert.That(InsertRequired(expr), Is.EqualTo("(from a in b select a.c ()).ToArray ()"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(from a in b select a.c ()).ToArray ()"));
		}

		[Test]
		public void SumOfQueries()
		{
			QueryExpression query = new QueryExpression {
				Clauses = {
					new QueryFromClause {
						Identifier = "a",
						Expression = new IdentifierExpression("b")
					},
					new QuerySelectClause {
						Expression = new IdentifierExpression("a")
					}
				}
			};
			Expression expr = new BinaryOperatorExpression(
				query,
				BinaryOperatorType.Add,
				query.Clone()
			);

			Assert.That(InsertRequired(expr), Is.EqualTo("(from a in b select a) + " +
							"from a in b select a"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(from a in b select a) + " +
							"(from a in b select a)"));
		}

		[Test]
		public void QueryInTypeTest()
		{
			Expression expr = new IsExpression {
				Expression = new QueryExpression {
					Clauses = {
						new QueryFromClause {
							Identifier = "a",
							Expression = new IdentifierExpression("b")
						},
						new QuerySelectClause {
							Expression = new IdentifierExpression("a")
						}
					}
				},
				Type = new PrimitiveType("int")
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("(from a in b select a) is int"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(from a in b select a) is int"));
		}

		[Test]
		public void PrePost()
		{
			Expression expr = new UnaryOperatorExpression(
				UnaryOperatorType.Increment,
				new UnaryOperatorExpression(
					UnaryOperatorType.PostIncrement,
					new IdentifierExpression("a")
				)
			);

			Assert.That(InsertRequired(expr), Is.EqualTo("++a++"));
			Assert.That(InsertReadable(expr), Is.EqualTo("++(a++)"));
		}

		[Test]
		public void PostPre()
		{
			Expression expr = new UnaryOperatorExpression(
				UnaryOperatorType.PostIncrement,
				new UnaryOperatorExpression(
					UnaryOperatorType.Increment,
					new IdentifierExpression("a")
				)
			);

			Assert.That(InsertRequired(expr), Is.EqualTo("(++a)++"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(++a)++"));
		}

		[Test]
		public void Logical1()
		{
			Expression expr = new BinaryOperatorExpression(
				new BinaryOperatorExpression(
					new IdentifierExpression("a"),
					BinaryOperatorType.ConditionalAnd,
					new IdentifierExpression("b")
				),
				BinaryOperatorType.ConditionalAnd,
				new IdentifierExpression("c")
			);

			Assert.That(InsertRequired(expr), Is.EqualTo("a && b && c"));
			Assert.That(InsertReadable(expr), Is.EqualTo("a && b && c"));
		}

		[Test]
		public void Logical2()
		{
			Expression expr = new BinaryOperatorExpression(
				new IdentifierExpression("a"),
				BinaryOperatorType.ConditionalAnd,
				new BinaryOperatorExpression(
					new IdentifierExpression("b"),
					BinaryOperatorType.ConditionalAnd,
					new IdentifierExpression("c")
				)
			);

			Assert.That(InsertRequired(expr), Is.EqualTo("a && (b && c)"));
			Assert.That(InsertReadable(expr), Is.EqualTo("a && (b && c)"));
		}

		[Test]
		public void Logical3()
		{
			Expression expr = new BinaryOperatorExpression(
				new IdentifierExpression("a"),
				BinaryOperatorType.ConditionalOr,
				new BinaryOperatorExpression(
					new IdentifierExpression("b"),
					BinaryOperatorType.ConditionalAnd,
					new IdentifierExpression("c")
				)
			);

			Assert.That(InsertRequired(expr), Is.EqualTo("a || b && c"));
			Assert.That(InsertReadable(expr), Is.EqualTo("a || (b && c)"));
		}

		[Test]
		public void Logical4()
		{
			Expression expr = new BinaryOperatorExpression(
				new IdentifierExpression("a"),
				BinaryOperatorType.ConditionalAnd,
				new BinaryOperatorExpression(
					new IdentifierExpression("b"),
					BinaryOperatorType.ConditionalOr,
					new IdentifierExpression("c")
				)
			);

			Assert.That(InsertRequired(expr), Is.EqualTo("a && (b || c)"));
			Assert.That(InsertReadable(expr), Is.EqualTo("a && (b || c)"));
		}

		[Test]
		public void ArrayCreationInIndexer()
		{
			Expression expr = new IndexerExpression {
				Target = new ArrayCreateExpression {
					Type = new PrimitiveType("int"),
					Arguments = { new PrimitiveExpression(1) }
				},
				Arguments = { new PrimitiveExpression(0) }
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("(new int[1]) [0]"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(new int[1]) [0]"));
		}

		[Test]
		public void ArrayCreationWithInitializerInIndexer()
		{
			Expression expr = new IndexerExpression {
				Target = new ArrayCreateExpression {
					Type = new PrimitiveType("int"),
					Arguments = { new PrimitiveExpression(1) },
					Initializer = new ArrayInitializerExpression {
						Elements = { new PrimitiveExpression(42) }
					}
				},
				Arguments = { new PrimitiveExpression(0) }
			};

			Assert.That(InsertRequired(expr), Is.EqualTo("new int[1] { 42 } [0]"));
			Assert.That(InsertReadable(expr), Is.EqualTo("(new int[1] { 42 }) [0]"));
		}
	}
}
