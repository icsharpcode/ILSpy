// Copyright (c) 2026 Christoph Wille
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// A node of the engine's C# translation of a function body
	/// (rust/ilspy-decompiler/src/wire/ast.rs), with its instructions, variables and types
	/// resolved; <see cref="RustSeam2"/> turns it into syntax.
	/// </summary>
	internal sealed class RustBodyNode
	{
		public const byte Block = 0;
		public const byte Return = 1;
		public const byte Empty = 2;
		public const byte ConstantValue = 3;
		public const byte StringLiteral = 4;
		public const byte Null = 5;
		public const byte Identifier = 6;
		public const byte This = 7;
		public const byte Base = 8;
		public const byte MemberReference = 9;
		public const byte TypeReference = 10;
		public const byte RefDirection = 11;
		public const byte Assignment = 12;
		public const byte ExpressionStatement = 13;
		public const byte Invocation = 14;
		public const byte NamedArgument = 15;
		public const byte IfElse = 16;
		public const byte LogicNot = 17;
		public const byte BinaryOperator = 18;
		public const byte Throw = 19;
		public const byte ObjectCreate = 20;
		public const byte Conditional = 21;
		public const byte GenericIdentifier = 22;
		public const byte GenericMemberReference = 23;
		public const byte Cast = 24;
		public const byte TypeOf = 25;
		public const byte OutDirection = 26;

		public byte Kind;
		/// <summary>The instructions the node is annotated with, in order.</summary>
		public ILInstruction[] IL = [];
		/// <summary>Whether the instructions were annotated before the resolve result.</summary>
		public bool ILBeforeResolveResult;
		public ResolveResult? ResolveResult;
		/// <summary>For an assignment or <c>!</c>: the type of its <see cref="OperatorResolveResult"/>, whose operands are the children's.</summary>
		public IType? OperatorType;
		/// <summary>For an invocation or object creation: the target, method and extension method flag of its <see cref="CSharpInvocationResolveResult"/>, whose arguments are the node's.</summary>
		public (ResolveResult? Target, IMethod Method, bool IsExtension)? Call;
		public Leave? ImplicitReturn;
		public RustBodyNode[] Children = [];
		public IType? Type;
		/// <summary>For an arithmetic operator: its <c>AddCheckedBlocks</c> annotation, 0 none, 1 checked, 2 unchecked.</summary>
		public byte Overflow;
		/// <summary>For a member reference or name: whether its resolve result names the member the name is looked up to.</summary>
		public bool LookUpMember;
		/// <summary>For a cast: whether its resolve result is <c>ResolveCast</c> over the operand's.</summary>
		public bool ResolveCast;
		/// <summary>For a generic name: the type arguments written with it.</summary>
		public IType[] TypeArguments = [];
		public object? Value;
		public bool Flag;
	}

	/// <summary>
	/// Seam 2, at body level: the statement block of a method body comes from the engine, which
	/// translates the ILAst it handed over through seam 1. A body with constructs the engine does
	/// not translate yet falls back to the managed statement builder and is counted.
	/// </summary>
	public static class RustSeam2
	{
		static long served;
		static long fallbacks;

		/// <summary>Bodies whose statements came from the engine.</summary>
		public static long Served => Interlocked.Read(ref served);

		/// <summary>Bodies the engine was asked to translate but the statement builder translated.</summary>
		public static long Fallbacks => Interlocked.Read(ref fallbacks);

		static readonly ConcurrentDictionary<string, int> fallbackReasons = new ConcurrentDictionary<string, int>();

		/// <summary>Why the engine did not translate the bodies that fell back, with how often.</summary>
		public static IReadOnlyDictionary<string, int> FallbackReasons => fallbackReasons;

		public static void ResetCounters()
		{
			Interlocked.Exchange(ref served, 0);
			Interlocked.Exchange(ref fallbacks, 0);
			fallbackReasons.Clear();
		}

		static readonly AsyncLocal<bool> disabled = new AsyncLocal<bool>();

		/// <summary>
		/// Whether body translations are asked for on this flow of execution; the AST equality test
		/// turns them off to decompile the managed side.
		/// </summary>
		public static bool Enabled {
			get => !disabled.Value;
			set => disabled.Value = !value;
		}

		static readonly ConditionalWeakTable<ILFunction, Translation> translations = new ConditionalWeakTable<ILFunction, Translation>();

		sealed class Translation
		{
			public RustBodyNode? Body;
			public string? Reason;
		}

		/// <summary>Records the engine's translation of <paramref name="function"/>'s body, or why there is none.</summary>
		internal static void Attach(ILFunction function, RustBodyNode? body, string? reason)
		{
			translations.Remove(function);
			translations.Add(function, new Translation { Body = body, Reason = reason });
		}

		/// <summary>
		/// <c>statementBuilder.ConvertAsBlock(function.Body)</c> from the engine's translation; null
		/// when the engine was not asked (the statement builder translates the body) or did not
		/// translate it (counted as a fallback).
		/// </summary>
		internal static BlockStatement? ServeBody(ILFunction function, StatementBuilder statementBuilder)
		{
			if (!translations.TryGetValue(function, out var translation))
				return null;
			translations.Remove(function);
			if (translation.Body == null)
			{
				Interlocked.Increment(ref fallbacks);
				fallbackReasons.AddOrUpdate(translation.Reason ?? "unknown", 1, (_, n) => n + 1);
				return null;
			}
			var statement = (Statement)Build(translation.Body, statementBuilder.exprBuilder);
			Interlocked.Increment(ref served);
			return statement as BlockStatement ?? new BlockStatement { statement };
		}

		static AstNode Build(RustBodyNode node, ExpressionBuilder exprBuilder)
		{
			switch (node.Kind)
			{
				case RustBodyNode.Block:
				{
					var block = new BlockStatement();
					foreach (var child in node.Children)
						block.Add((Statement)Build(child, exprBuilder));
					if (node.ImplicitReturn != null)
						block.AddAnnotation(new ImplicitReturnAnnotation(node.ImplicitReturn));
					return Annotate(block, node);
				}
				case RustBodyNode.Return:
				{
					var statement = node.Children.Length == 1
						? new ReturnStatement((Expression)Build(node.Children[0], exprBuilder))
						: new ReturnStatement();
					return Annotate(statement, node);
				}
				case RustBodyNode.Empty:
					return Annotate(new EmptyStatement(), node);
				case RustBodyNode.ConstantValue:
				{
					// The AST builder annotates the constant itself.
					var expression = exprBuilder.ConvertConstantValue(new ConstantResolveResult(node.Type!, node.Value), node.Flag).Expression;
					foreach (var inst in node.IL)
						expression.AddAnnotation(inst);
					return expression;
				}
				case RustBodyNode.StringLiteral:
					return Annotate(new PrimitiveExpression(node.Value!), node);
				case RustBodyNode.Null:
					return Annotate(new NullReferenceExpression(), node);
				case RustBodyNode.Identifier:
					if (node.LookUpMember)
						node.ResolveResult = LookUpMember(exprBuilder, (MemberResolveResult)node.ResolveResult!, null);
					return Annotate(new IdentifierExpression((string)node.Value!), node);
				case RustBodyNode.This:
					return Annotate(new ThisReferenceExpression(), node);
				case RustBodyNode.Base:
					return Annotate(new BaseReferenceExpression(), node);
				case RustBodyNode.MemberReference:
				{
					var target = (Expression)Build(node.Children[0], exprBuilder);
					if (node.LookUpMember)
						node.ResolveResult = LookUpMember(exprBuilder, (MemberResolveResult)node.ResolveResult!, target.GetResolveResult());
					return Annotate(new MemberReferenceExpression(target, (string)node.Value!), node);
				}
				case RustBodyNode.TypeReference:
					return Annotate(new TypeReferenceExpression(exprBuilder.ConvertType(node.Type!)), node);
				case RustBodyNode.RefDirection:
					return Annotate(new DirectionExpression(FieldDirection.Ref, (Expression)Build(node.Children[0], exprBuilder)), node);
				case RustBodyNode.ExpressionStatement:
					return Annotate(new ExpressionStatement((Expression)Build(node.Children[0], exprBuilder)), node);
				case RustBodyNode.Assignment:
				{
					var left = (Expression)Build(node.Children[0], exprBuilder);
					var right = (Expression)Build(node.Children[1], exprBuilder);
					// A property setter's assignment comes with a resolve result of its own.
					if (node.OperatorType != null)
					{
						node.ResolveResult = new OperatorResolveResult(node.OperatorType, System.Linq.Expressions.ExpressionType.Assign,
							left.GetResolveResult(), right.GetResolveResult());
					}
					return Annotate(new AssignmentExpression(left, right), node);
				}
				case RustBodyNode.Invocation:
				{
					var target = (Expression)Build(node.Children[0], exprBuilder);
					var arguments = node.Children.Skip(1).Select(c => (Expression)Build(c, exprBuilder)).ToList();
					var (targetResult, method, isExtension) = node.Call!.Value;
					// The argument resolve results, of the named arguments' expressions.
					var argumentResults = arguments
						.Select(a => (a is NamedArgumentExpression named ? named.Expression : a).GetResolveResult())
						.ToList();
					node.ResolveResult = new CSharpInvocationResolveResult(targetResult!, method, argumentResults, isExtensionMethodInvocation: isExtension);
					return Annotate(new InvocationExpression(target, arguments), node);
				}
				case RustBodyNode.ObjectCreate:
				{
					var arguments = node.Children.Select(c => (Expression)Build(c, exprBuilder)).ToList();
					var argumentResults = arguments
						.Select(a => (a is NamedArgumentExpression named ? named.Expression : a).GetResolveResult())
						.ToArray();
					node.ResolveResult = new CSharpInvocationResolveResult(null!, node.Call!.Value.Method, argumentResults);
					return Annotate(new ObjectCreateExpression(exprBuilder.ConvertType(node.Type!), arguments), node);
				}
				case RustBodyNode.Throw:
				{
					var statement = node.Children.Length == 1
						? new ThrowStatement((Expression)Build(node.Children[0], exprBuilder))
						: new ThrowStatement();
					return Annotate(statement, node);
				}
				case RustBodyNode.NamedArgument:
					return Annotate(new NamedArgumentExpression((string)node.Value!, (Expression)Build(node.Children[0], exprBuilder)), node);
				case RustBodyNode.IfElse:
				{
					var condition = (Expression)Build(node.Children[0], exprBuilder);
					var trueStatement = (Statement)Build(node.Children[1], exprBuilder);
					var falseStatement = node.Children.Length == 3 ? (Statement)Build(node.Children[2], exprBuilder) : null;
					return Annotate(new IfElseStatement(condition, trueStatement, falseStatement), node);
				}
				case RustBodyNode.LogicNot:
				{
					var operand = (Expression)Build(node.Children[0], exprBuilder);
					node.ResolveResult = new OperatorResolveResult(node.OperatorType!, System.Linq.Expressions.ExpressionType.Not, operand.GetResolveResult());
					return Annotate(new UnaryOperatorExpression(UnaryOperatorType.Not, operand), node);
				}
				case RustBodyNode.BinaryOperator:
				{
					var op = (BinaryOperatorType)node.Value!;
					var left = (Expression)Build(node.Children[0], exprBuilder);
					var right = (Expression)Build(node.Children[1], exprBuilder);
					// A logic operator comes with a plain resolve result of its own.
					if (node.OperatorType != null)
					{
						node.ResolveResult = new OperatorResolveResult(node.OperatorType, BinaryOperatorExpression.GetLinqNodeType(op, node.Overflow == 1),
							left.GetResolveResult(), right.GetResolveResult());
					}
					var binary = Annotate(new BinaryOperatorExpression(left, op, right), node);
					if (node.Overflow != 0)
					{
						binary.AddAnnotation(node.Overflow == 1
							? CSharp.Transforms.AddCheckedBlocks.CheckedAnnotation
							: CSharp.Transforms.AddCheckedBlocks.UncheckedAnnotation);
					}
					return binary;
				}
				case RustBodyNode.OutDirection:
				{
					var direction = Annotate(new DirectionExpression(FieldDirection.Out, (Expression)Build(node.Children[0], exprBuilder)), node);
					// `GetArgumentExpressions` marks an `out` argument written as `out var`.
					if (node.Flag)
						direction.AddAnnotation(UseImplicitlyTypedOutAnnotation.Instance);
					return direction;
				}
				case RustBodyNode.TypeOf:
					return Annotate(new TypeOfExpression(exprBuilder.ConvertType(node.Type!)), node);
				case RustBodyNode.Cast:
				{
					var operand = (Expression)Build(node.Children[0], exprBuilder);
					var cast = new CastExpression(exprBuilder.ConvertType(node.Type!), operand);
					// `TranslatedExpression.ConvertTo` marks a cast to an integer type unchecked.
					if (node.Flag)
						cast.AddAnnotation(CSharp.Transforms.AddCheckedBlocks.UncheckedAnnotation);
					if (node.ResolveCast)
						node.ResolveResult = exprBuilder.resolver.WithCheckForOverflow(false).ResolveCast(node.Type!, operand.GetResolveResult());
					return Annotate(cast, node);
				}
				case RustBodyNode.GenericIdentifier:
					return Annotate(new IdentifierExpression((string)node.Value!,
						node.TypeArguments.Select(exprBuilder.ConvertType)), node);
				case RustBodyNode.GenericMemberReference:
					return Annotate(new MemberReferenceExpression((Expression)Build(node.Children[0], exprBuilder), (string)node.Value!,
						node.TypeArguments.Select(exprBuilder.ConvertType)), node);
				case RustBodyNode.Conditional:
				{
					var condition = (Expression)Build(node.Children[0], exprBuilder);
					var trueExpression = (Expression)Build(node.Children[1], exprBuilder);
					var falseExpression = (Expression)Build(node.Children[2], exprBuilder);
					node.ResolveResult = new OperatorResolveResult(node.OperatorType!, System.Linq.Expressions.ExpressionType.Conditional,
						condition.GetResolveResult(), trueExpression.GetResolveResult(), falseExpression.GetResolveResult());
					return Annotate(new ConditionalExpression(condition, trueExpression, falseExpression), node);
				}
				default:
					throw new RustEngineException($"unknown C# node kind {node.Kind}");
			}
		}

		/// <summary>
		/// The resolve result <c>HandleAccessorCall</c> gives a property access whose name binds to another
		/// member than the accessor's property: the member the name is looked up to, unqualified
		/// or on <paramref name="target"/>, as <c>Disambiguator.IsUnambiguousAccess</c> found it.
		/// </summary>
		static ResolveResult LookUpMember(ExpressionBuilder exprBuilder, MemberResolveResult result, ResolveResult? target)
		{
			var resolver = exprBuilder.resolver;
			var found = target == null
				? resolver.ResolveSimpleName(result.Member.Name, EmptyList<IType>.Instance, isInvocationTarget: false)
				: new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule)
					.Lookup(target, result.Member.Name, EmptyList<IType>.Instance, isInvocation: false);
			if (found is not MemberResolveResult { IsError: false } member)
				throw new RustEngineException($"The name {result.Member.Name} no longer binds to a member");
			return new MemberResolveResult(result.TargetResult, member.Member);
		}

		/// <summary>The node's instructions and resolve result, in the order the managed builder adds them.</summary>
		static T Annotate<T>(T syntax, RustBodyNode node) where T : AstNode
		{
			if (!node.ILBeforeResolveResult && node.ResolveResult != null)
				syntax.AddAnnotation(node.ResolveResult);
			foreach (var inst in node.IL)
				syntax.AddAnnotation(inst);
			if (node.ILBeforeResolveResult && node.ResolveResult != null)
				syntax.AddAnnotation(node.ResolveResult);
			return syntax;
		}
	}
}
