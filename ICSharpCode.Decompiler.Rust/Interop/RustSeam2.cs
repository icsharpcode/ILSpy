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
		public const byte TryCatch = 27;
		public const byte CatchClause = 28;
		public const byte Lock = 29;
		public const byte While = 30;
		public const byte Break = 31;
		public const byte Continue = 32;
		public const byte DoWhile = 33;
		public const byte For = 34;
		public const byte Indexer = 35;
		public const byte Using = 36;
		public const byte Lambda = 37;
		public const byte InterpolatedString = 38;
		public const byte Named = 39;
		public const byte ArrayCreate = 40;
		public const byte ArrayInitializer = 41;
		public const byte YieldReturn = 42;
		public const byte YieldBreak = 43;
		public const byte NullConditional = 44;
		public const byte IsPattern = 45;
		public const byte DeclarationPattern = 46;
		public const byte RecursivePattern = 47;
		public const byte IsType = 48;
		public const byte DefaultValue = 49;
		public const byte Foreach = 50;
		public const byte DelegateCreate = 51;
		public const byte Await = 52;
		public const byte Switch = 53;
		public const byte SwitchSection = 54;
		public const byte CompoundAssignment = 55;
		public const byte PostIncrement = 56;
		public const byte As = 57;
		public const byte ThrowExpression = 58;
		public const byte BitNot = 59;
		public const byte Tuple = 60;
		public const byte HexConstant = 61;
		public const byte SwitchExpression = 62;
		public const byte LocalFunctionDeclaration = 63;
		public const byte PatternUnary = 64;
		public const byte Label = 65;
		public const byte Goto = 66;
		public const byte DeconstructionTuple = 67;

		public byte Kind;
		/// <summary>The instructions the node is annotated with, in order.</summary>
		public ILInstruction[] IL = [];
		/// <summary>Whether the instructions were annotated before the resolve result.</summary>
		public bool ILBeforeResolveResult;
		public ResolveResult? ResolveResult;
		/// <summary>For an assignment or <c>!</c>: the type of its <see cref="OperatorResolveResult"/>, whose operands are the children's.</summary>
		public IType? OperatorType;
		/// <summary>For an invocation or object creation: the target, method and extension method flag of its <see cref="CSharpInvocationResolveResult"/>, whose arguments are the node's.</summary>
		public (ResolveResult? Target, IMethod Method, bool IsExtension, bool IsDelegate, bool FoundByLookup, bool IsExpanded)? Call;
		public Leave? ImplicitReturn;
		public RustBodyNode[] Children = [];
		public IType? Type;
		/// <summary>For an arithmetic operator: its <c>AddCheckedBlocks</c> annotation, 0 none, 1 checked, 2 unchecked.</summary>
		public byte Overflow;
		/// <summary>For a member reference or name: whether its resolve result names the member the name is looked up to.</summary>
		public bool LookUpMember;
		/// <summary>For a cast: whether its resolve result is <c>ResolveCast</c> over the operand's.</summary>
		public bool ResolveCast;
		/// <summary>For a cast: whether its resolve result is the implicit conversion from the operand's type.</summary>
		public bool ImplicitConversion;
		/// <summary>For a cast: whether its resolve result is the lambda conversion of the lambda it casts.</summary>
		public bool LambdaConversion;
		/// <summary>For a binary operator: whether its resolve result is <c>ResolveBinaryOperator</c> over the operands'.</summary>
		public bool ResolveOperator;
		/// <summary>For a method reference: whether it has a target, and the type arguments and method of its <see cref="MethodGroupResolveResult"/>.</summary>
		public (bool HasTarget, IType[] TypeArguments, IMethod Method)? MethodGroup;
		/// <summary>For a lambda: <see cref="DecompiledLambdaResolveResult.IsImplicitlyTyped"/>.</summary>
		public bool ImplicitlyTyped;
		/// <summary>For a lambda: an anonymous method without parameter list.</summary>
		public bool AnonymousMethod;
		/// <summary>For a lambda: <see cref="DecompiledLambdaResolveResult.AttemptedConversionWithTypeMismatch"/>.</summary>
		public bool AttemptedConversionWithTypeMismatch;
		/// <summary>For an object creation: the elements of its initializer, if it has one.</summary>
		public RustBodyNode[]? Initializer;
		/// <summary>For a foreach statement: the GetEnumerator, MoveNext and get_Current calls of its <see cref="ForeachAnnotation"/>.</summary>
		public ILInstruction[] ForeachInstructions = [];
		/// <summary>The type of the user-defined cast <c>ConvertTo</c> dropped around this expression, for its <see cref="CSharp.ImplicitConversionAnnotation"/>.</summary>
		public IType? DroppedCastType;
		/// <summary>Whether that cast was an implicit conversion operator's (else a <c>ResolveCast</c>).</summary>
		public bool DroppedCastIsImplicitOperator;
		/// <summary>For a generic name: the type arguments written with it.</summary>
		public IType[] TypeArguments = [];
		public object? Value;
		/// <summary>For a recursive pattern: the member of each sub-pattern.</summary>
		public IMember[] Members = [];
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

		/// <summary>The checked or unchecked annotation of <see cref="RustBodyNode.Overflow"/> (1 checked, 2 unchecked).</summary>
		static T AddOverflowAnnotation<T>(T expression, RustBodyNode node) where T : Expression
		{
			if (node.Overflow != 0)
			{
				expression.AddAnnotation(node.Overflow == 1
					? CSharp.Transforms.AddCheckedBlocks.CheckedAnnotation
					: CSharp.Transforms.AddCheckedBlocks.UncheckedAnnotation);
			}
			return expression;
		}

		/// <summary>A call target's resolve result that is the one of its built target expression.</summary>
		internal static readonly ResolveResult PendingTarget = new ResolveResult(SpecialType.UnknownType);

		static AstNode Build(RustBodyNode node, ExpressionBuilder exprBuilder)
		{
			var syntax = BuildNode(node, exprBuilder);
			if (node.MethodGroup is var (hasTarget, typeArguments, groupMethod))
			{
				// `Disambiguator.LookUpName` without invocation, from the built target expression,
				// then `WithChosenMethod`.
				var resolver = exprBuilder.resolver;
				var found = !hasTarget
					? resolver.ResolveSimpleName(groupMethod.Name, typeArguments, isInvocationTarget: false)
					: new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule)
						.Lookup(((MemberReferenceExpression)syntax).Target.GetResolveResult(), groupMethod.Name, typeArguments, isInvocation: false);
				syntax.AddAnnotation(((MethodGroupResolveResult)found).WithChosenMethod(groupMethod));
			}
			if (node.DroppedCastType is { } castType)
			{
				// The conversion `ConvertTo` keeps in an `ImplicitConversionAnnotation`: the cast's
				// resolve result over the operand's.
				var operand = syntax.GetResolveResult();
				var conversion = node.DroppedCastIsImplicitOperator
					? new ConversionResolveResult(castType, operand, CSharp.Resolver.CSharpConversions.Get(exprBuilder.compilation).ImplicitConversion(operand.Type, castType))
					: (ConversionResolveResult)exprBuilder.resolver.WithCheckForOverflow(false).ResolveCast(castType, operand);
				syntax.AddAnnotation(new ImplicitConversionAnnotation(conversion));
			}
			return syntax;
		}

		static AstNode BuildNode(RustBodyNode node, ExpressionBuilder exprBuilder)
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
				case RustBodyNode.SwitchExpression:
				{
					// The value, then pattern and body per section.
					var switchExpression = new SwitchExpression { Expression = (Expression)Build(node.Children[0], exprBuilder) };
					for (int i = 1; i + 1 < node.Children.Length; i += 2)
					{
						switchExpression.SwitchSections.Add(new SwitchExpressionSection {
							Pattern = (Expression)Build(node.Children[i], exprBuilder),
							Body = (Expression)Build(node.Children[i + 1], exprBuilder)
						});
					}
					return Annotate(switchExpression, node);
				}
				case RustBodyNode.HexConstant:
				{
					// A bitwise operator's constant operand, in hexadecimal where the flag says so.
					var expression = exprBuilder.ConvertConstantValue(new ConstantResolveResult(node.Type!, node.Value),
						allowImplicitConversion: false, displayAsHex: node.Flag).Expression;
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
					// A cast target's resolve result is the one of the built target.
					if (node.ResolveResult is MemberResolveResult pending && ReferenceEquals(pending.TargetResult, PendingTarget))
					{
						node.ResolveResult = new MemberResolveResult(target.GetResolveResult(), pending.Member, pending.Type,
							pending.IsCompileTimeConstant, pending.ConstantValue, pending.IsVirtualCall);
					}
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
					var (targetResult, method, isExtension, isDelegate, foundByLookup, isExpanded) = node.Call!.Value;
					// The target of a member reference, or the invoked delegate itself.
					if (ReferenceEquals(targetResult, PendingTarget))
						targetResult = (target is MemberReferenceExpression mre ? mre.Target : target).GetResolveResult();
					// The argument resolve results, of the named arguments' expressions.
					var argumentResults = arguments
						.Select(a => (a is NamedArgumentExpression named ? named.Expression : a).GetResolveResult())
						.ToList();
					// An override the name resolves to: the overload resolution of the written call.
					if (foundByLookup)
						method = FindOverload(exprBuilder, target, method, arguments, argumentResults);
					node.ResolveResult = new CSharpInvocationResolveResult(targetResult!, method, argumentResults, isExpandedForm: isExpanded, isExtensionMethodInvocation: isExtension, isDelegateInvocation: isDelegate);
					return Annotate(new InvocationExpression(target, arguments), node);
				}
				case RustBodyNode.ObjectCreate:
				{
					var arguments = node.Children.Select(c => (Expression)Build(c, exprBuilder)).ToList();
					var argumentResults = arguments
						.Select(a => (a is NamedArgumentExpression named ? named.Expression : a).GetResolveResult())
						.ToArray();
					node.ResolveResult = new CSharpInvocationResolveResult(null!, node.Call!.Value.Method, argumentResults, isExpandedForm: node.Call!.Value.IsExpanded);
					var creation = new ObjectCreateExpression(exprBuilder.ConvertType(node.Type!), arguments);
					if (node.Initializer != null)
						creation.Initializer = new ArrayInitializerExpression(node.Initializer.Select(e => (Expression)Build(e, exprBuilder)));
					return Annotate(creation, node);
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
				case RustBodyNode.CompoundAssignment:
				{
					// `HandleCompoundAssignment`: the operator over the target's and value's resolve results.
					var op = (AssignmentOperatorType)node.Value!;
					var left = (Expression)Build(node.Children[0], exprBuilder);
					var right = (Expression)Build(node.Children[1], exprBuilder);
					// An event's `+=` or `-=` keeps the type resolve result it comes with.
					if (node.OperatorType != null)
					{
						node.ResolveResult = new OperatorResolveResult(node.OperatorType, AssignmentExpression.GetLinqNodeType(op, node.Overflow == 1),
							left.GetResolveResult(), right.GetResolveResult());
					}
					return AddOverflowAnnotation(Annotate(new AssignmentExpression(left, op, right), node), node);
				}
				case RustBodyNode.PostIncrement:
				{
					var operand = (Expression)Build(node.Children[0], exprBuilder);
					node.ResolveResult = new OperatorResolveResult(node.OperatorType!,
						node.Flag ? System.Linq.Expressions.ExpressionType.PostIncrementAssign : System.Linq.Expressions.ExpressionType.PostDecrementAssign,
						operand.GetResolveResult());
					var unary = new UnaryOperatorExpression(node.Flag ? UnaryOperatorType.PostIncrement : UnaryOperatorType.PostDecrement, operand);
					return AddOverflowAnnotation(Annotate(unary, node), node);
				}
				case RustBodyNode.Switch:
				{
					var statement = new SwitchStatement { Expression = (Expression)Build(node.Children[0], exprBuilder) };
					foreach (var section in node.Children.Skip(1))
						statement.SwitchSections.Add((ICSharpCode.Decompiler.CSharp.Syntax.SwitchSection)Build(section, exprBuilder));
					return Annotate(statement, node);
				}
				case RustBodyNode.SwitchSection:
				{
					// The labels (null for `default`), then the statements.
					var section = new ICSharpCode.Decompiler.CSharp.Syntax.SwitchSection();
					var labels = (bool[])node.Value!;
					int child = 0;
					foreach (bool hasExpression in labels)
						section.CaseLabels.Add(hasExpression ? new CaseLabel((Expression)Build(node.Children[child++], exprBuilder)) : new CaseLabel());
					while (child < node.Children.Length)
						section.Statements.Add((Statement)Build(node.Children[child++], exprBuilder));
					return Annotate(section, node);
				}
				case RustBodyNode.Tuple:
				{
					// `MatchTupleConstruction`: the elements, named where the type hint names them.
					var names = (string?[])node.Value!;
					var tuple = new TupleExpression();
					var elementResults = new List<ResolveResult>();
					for (int i = 0; i < node.Children.Length; i++)
					{
						var element = (Expression)Build(node.Children[i], exprBuilder);
						elementResults.Add(element.GetResolveResult());
						tuple.Elements.Add(names[i] is { Length: > 0 } name ? new NamedArgumentExpression(name, element) : element);
					}
					var elementNames = node.Flag && names.Any(n => n != null) ? System.Collections.Immutable.ImmutableArray.Create(names) : default;
					node.ResolveResult = new TupleResolveResult(exprBuilder.compilation, System.Collections.Immutable.ImmutableArray.CreateRange(elementResults),
						elementNames, valueTupleAssembly: node.Type!.GetDefinition()?.ParentModule);
					return Annotate(tuple, node);
				}
				case RustBodyNode.BitNot:
				{
					var operand = (Expression)Build(node.Children[0], exprBuilder);
					node.ResolveResult = exprBuilder.resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, operand.GetResolveResult());
					return Annotate(new UnaryOperatorExpression(UnaryOperatorType.BitNot, operand), node);
				}
				case RustBodyNode.ThrowExpression:
					return Annotate(new ThrowExpression((Expression)Build(node.Children[0], exprBuilder)), node);
				case RustBodyNode.Await:
					return Annotate(new UnaryOperatorExpression(UnaryOperatorType.Await, (Expression)Build(node.Children[0], exprBuilder)), node);
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
					if (node.ResolveOperator)
						node.ResolveResult = exprBuilder.resolver.ResolveBinaryOperator(op, left.GetResolveResult(), right.GetResolveResult());
					var binary = Annotate(new BinaryOperatorExpression(left, op, right), node);
					if (node.Overflow != 0)
					{
						binary.AddAnnotation(node.Overflow == 1
							? CSharp.Transforms.AddCheckedBlocks.CheckedAnnotation
							: CSharp.Transforms.AddCheckedBlocks.UncheckedAnnotation);
					}
					return binary;
				}
				case RustBodyNode.TryCatch:
				{
					var statement = new TryCatchStatement { TryBlock = (BlockStatement)Build(node.Children[0], exprBuilder) };
					int clauses = node.Children.Length - (node.Flag ? 2 : 1);
					for (int i = 1; i <= clauses; i++)
						statement.CatchClauses.Add((CatchClause)Build(node.Children[i], exprBuilder));
					if (node.Flag)
						statement.FinallyBlock = (BlockStatement)Build(node.Children[^1], exprBuilder);
					return Annotate(statement, node);
				}
				case RustBodyNode.CatchClause:
				{
					var clause = new CatchClause();
					if (node.Value != null)
						clause.VariableName = (string)node.Value;
					if (node.Type != null)
						clause.Type = exprBuilder.ConvertType(node.Type);
					if (node.Children.Length == 2)
						clause.Condition = (Expression)Build(node.Children[1], exprBuilder);
					clause.Body = (BlockStatement)Build(node.Children[0], exprBuilder);
					return Annotate(clause, node);
				}
				case RustBodyNode.While:
				{
					// `while (true)` has a plain literal without resolve result.
					var condition = node.Flag
						? (Expression)Build(node.Children[0], exprBuilder)
						: new PrimitiveExpression(true);
					var body = (Statement)Build(node.Children[^1], exprBuilder);
					return Annotate(new WhileStatement(condition, body), node);
				}
				case RustBodyNode.Indexer:
					return Annotate(new IndexerExpression((Expression)Build(node.Children[0], exprBuilder),
						node.Children.Skip(1).Select(c => (Expression)Build(c, exprBuilder))), node);
				case RustBodyNode.DoWhile:
					return Annotate(new DoWhileStatement {
						EmbeddedStatement = (Statement)Build(node.Children[0], exprBuilder),
						Condition = (Expression)Build(node.Children[1], exprBuilder)
					}, node);
				case RustBodyNode.For:
				{
					var statement = new ForStatement {
						Condition = (Expression)Build(node.Children[0], exprBuilder),
						EmbeddedStatement = (Statement)Build(node.Children[^1], exprBuilder)
					};
					for (int i = 1; i < node.Children.Length - 1; i++)
						statement.Iterators.Add((Statement)Build(node.Children[i], exprBuilder));
					return Annotate(statement, node);
				}
				case RustBodyNode.Break:
					return Annotate(new BreakStatement(), node);
				case RustBodyNode.Continue:
					return Annotate(new ContinueStatement(), node);
				case RustBodyNode.DeconstructionTuple:
				{
					var tuple = new TupleExpression();
					tuple.Elements.AddRange(node.Children.Select(c => (Expression)Build(c, exprBuilder)));
					return Annotate(tuple, node);
				}
				case RustBodyNode.Label:
					return Annotate(new LabelStatement { Label = (string)node.Value! }, node);
				case RustBodyNode.Goto:
					return Annotate(new GotoStatement((string)node.Value!), node);
				case RustBodyNode.DelegateCreate:
				{
					// `HandleDelegateConstruction`
					var reference = (Expression)Build(node.Children[0], exprBuilder);
					var method = (IMethod)node.Value!;
					node.ResolveResult = new ConversionResolveResult(node.Type!, reference.GetResolveResult(),
						Conversion.MethodGroupConversion(method, node.Flag, false));
					return Annotate(new ObjectCreateExpression(exprBuilder.ConvertType(node.Type!), reference), node);
				}
				case RustBodyNode.Foreach:
				{
					// `TransformToForeach`: the variable as the ILAst names and types it now.
					var variable = (ILVariable)node.Value!;
					var designation = new SingleVariableDesignation { Identifier = variable.Name! };
					designation.AddAnnotation(new ILVariableResolveResult(variable, variable.Type));
					var statement = new ForeachStatement {
						VariableType = exprBuilder.ConvertType(variable.Type),
						VariableDesignation = designation,
						InExpression = (Expression)Build(node.Children[0], exprBuilder),
						EmbeddedStatement = (Statement)Build(node.Children[1], exprBuilder)
					};
					var i = node.ForeachInstructions;
					statement.AddAnnotation(new ForeachAnnotation(i[0], i[1], i[2]));
					return Annotate(statement, node);
				}
				case RustBodyNode.DefaultValue:
					return Annotate(node.Type == null ? new DefaultValueExpression() : new DefaultValueExpression(exprBuilder.ConvertType(node.Type)), node);
				case RustBodyNode.IsType:
				{
					var tested = (Expression)Build(node.Children[0], exprBuilder);
					node.ResolveResult = new TypeIsResolveResult(tested.GetResolveResult(), node.Type!, exprBuilder.compilation.FindType(TypeCode.Boolean));
					return Annotate(new IsExpression(tested, exprBuilder.ConvertType(node.Type!.TupleUnderlyingTypeOrSelf())), node);
				}
				case RustBodyNode.IsPattern:
					return Annotate(new BinaryOperatorExpression((Expression)Build(node.Children[0], exprBuilder), BinaryOperatorType.IsPattern,
						(Expression)Build(node.Children[1], exprBuilder)), node);
				case RustBodyNode.DeclarationPattern:
				{
					var variable = (ILVariable)node.Value!;
					return Annotate(new DeclarationExpression {
						Type = node.Flag ? new SimpleType("var") : exprBuilder.ConvertType(variable.Type),
						Designation = Designation(variable)
					}, node);
				}
				case RustBodyNode.RecursivePattern:
				{
					var pattern = new RecursivePatternExpression();
					if (node.Type != null)
						pattern.Type = exprBuilder.ConvertType(node.Type);
					for (int i = 0; i < node.Members.Length; i++)
					{
						var member = node.Members[i];
						var sub = new NamedArgumentExpression { Name = member.Name, Expression = (Expression)Build(node.Children[i], exprBuilder) };
						sub.AddAnnotation(new MemberResolveResult(null, member));
						pattern.SubPatterns.Add(sub);
					}
					if (node.Value is ILVariable variable)
						pattern.Designation = Designation(variable);
					return Annotate(pattern, node);
				}
				case RustBodyNode.PatternUnary:
				{
					var op = (int)node.Value! switch {
						0 => UnaryOperatorType.PatternNot,
						1 => UnaryOperatorType.PatternRelationalLessThan,
						2 => UnaryOperatorType.PatternRelationalLessThanOrEqual,
						3 => UnaryOperatorType.PatternRelationalGreaterThan,
						_ => UnaryOperatorType.PatternRelationalGreaterThanOrEqual,
					};
					return Annotate(new UnaryOperatorExpression(op, (Expression)Build(node.Children[0], exprBuilder)), node);
				}
				case RustBodyNode.NullConditional:
					return Annotate(new UnaryOperatorExpression(node.Flag ? UnaryOperatorType.NullConditionalRewrap : UnaryOperatorType.NullConditional,
						(Expression)Build(node.Children[0], exprBuilder)), node);
				case RustBodyNode.YieldReturn:
					return Annotate(new YieldReturnStatement { Expression = (Expression)Build(node.Children[0], exprBuilder) }, node);
				case RustBodyNode.YieldBreak:
					return Annotate(new YieldBreakStatement(), node);
				case RustBodyNode.ArrayInitializer:
					return Annotate(new ArrayInitializerExpression(node.Children.Select(c => (Expression)Build(c, exprBuilder))), node);
				case RustBodyNode.ArrayCreate:
				{
					// `TranslateArrayInitializer`, or `VisitNewArr` without initializer.
					var elementType = node.Type!;
					bool hasInitializer = (bool)node.Value!;
					int dimensions = node.Children.Length - (hasInitializer ? 1 : 0);
					var arguments = node.Children.Take(dimensions).Select(c => (Expression)Build(c, exprBuilder)).ToList();
					var astBuilder = exprBuilder.astBuilder;
					bool useSpecialConstants = astBuilder.UseSpecialConstants;
					ArrayInitializerExpression? initializer = null;
					if (hasInitializer)
					{
						try
						{
							astBuilder.UseSpecialConstants = node.Flag;
							initializer = (ArrayInitializerExpression)Build(node.Children[dimensions], exprBuilder);
						}
						finally
						{
							astBuilder.UseSpecialConstants = useSpecialConstants;
						}
					}
					// An array of anonymous types is implicitly typed, `new[] { ... }`.
					bool anonymous = hasInitializer && exprBuilder.settings.AnonymousTypes && elementType.ContainsAnonymousType();
					AstType? typeExpression = null;
					ArraySpecifier[] additionalSpecifiers = [new ArraySpecifier()];
					if (!anonymous)
					{
						typeExpression = exprBuilder.ConvertType(elementType);
						additionalSpecifiers = [];
						if (typeExpression is ComposedType composed && composed.ArraySpecifiers.Count > 0)
						{
							additionalSpecifiers = composed.ArraySpecifiers.Select(a => (ArraySpecifier)a.Clone()).ToArray();
							composed.ArraySpecifiers.Clear();
						}
					}
					var creation = new ArrayCreateExpression { Type = typeExpression! };
					if (initializer != null)
						creation.Initializer = initializer;
					creation.AdditionalArraySpecifiers.AddRange(additionalSpecifiers);
					if (!(hasInitializer && elementType.ContainsAnonymousType()))
						creation.Arguments.AddRange(arguments);
					var elementResults = new List<ResolveResult>();
					if (initializer != null)
						CollectElements(initializer);
					node.ResolveResult = new ArrayCreateResolveResult(new ArrayType(exprBuilder.compilation, elementType, arguments.Count),
						arguments.Select(a => a.GetResolveResult()).ToArray(), elementResults);
					return Annotate(creation, node);

					void CollectElements(ArrayInitializerExpression list)
					{
						foreach (var element in list.Elements)
						{
							if (element is ArrayInitializerExpression nested)
								CollectElements(nested);
							else
								elementResults.Add(element.GetResolveResult());
						}
					}
				}
				case RustBodyNode.Named:
					return Annotate(new NamedExpression((string)node.Value!, (Expression)Build(node.Children[0], exprBuilder)), node);
				case RustBodyNode.InterpolatedString:
				{
					var content = new List<InterpolatedStringContent>();
					int child = 0;
					foreach (var (text, alignment, suffix) in ((string? Text, int Alignment, string? Suffix)[])node.Value!)
					{
						if (text != null)
							content.Add(new InterpolatedStringText(text));
						else
							content.Add(new Interpolation((Expression)Build(node.Children[child++], exprBuilder), alignment, suffix));
					}
					// A `string.Format` call: the arguments are the interpolated expressions.
					if (node.ResolveResult is InterpolatedStringResolveResult format)
					{
						node.ResolveResult = new InterpolatedStringResolveResult(format.Type, format.FormatString,
							content.OfType<Interpolation>().Select(i => i.Expression.GetResolveResult()).ToArray());
					}
					return Annotate(new InterpolatedStringExpression(content), node);
				}
				case RustBodyNode.LocalFunctionDeclaration:
				{
					// `DeclareLocalFunctions.TranslateFunction`: the declaration of the reduced method
					// with the parameters named after the function's, around the translated body.
					var function = (ILFunction)node.IL[0];
					var method = (MethodDeclaration)exprBuilder.astBuilder.ConvertEntity(function.ReducedMethod!);
					var variables = function.Variables.Where(v => v.Kind == VariableKind.Parameter).ToDictionary(v => v.Index!.Value);
					foreach (var (i, p) in method.Parameters.WithIndex())
					{
						if (variables.TryGetValue(i, out var v))
							p.Name = v.Name!;
					}
					method.Body = (BlockStatement)Build(node.Children[0], exprBuilder);
					var warningAnchor = method.Body.Statements.FirstOrDefault();
					foreach (string warning in function.Warnings)
					{
						var warningStatement = new EmptyStatement();
						warningStatement.AddTrailingTrivia(new Comment(warning));
						if (warningAnchor != null)
							method.Body.Statements.InsertBefore(warningAnchor, warningStatement);
						else
							method.Body.Statements.Add(warningStatement);
					}
					CSharpDecompiler.AddAnnotationsToDeclaration(function.ReducedMethod!, method, function);
					CSharpDecompiler.CleanUpMethodDeclaration(method, method.Body, function, function.Method!.HasBody);
					CSharpDecompiler.RemoveAttribute(method, KnownAttribute.CompilerGenerated);
					var stmt = new LocalFunctionDeclarationStatement(method);
					stmt.AddAnnotation(new MemberResolveResult(null, function.ReducedMethod));
					stmt.AddAnnotation(function);
					return stmt;
				}
				case RustBodyNode.Lambda:
				{
					// `TranslateFunction` for a lambda with a parameter list, explicitly typed unless a
					// parameter has an anonymous type.
					var function = (ILFunction)node.IL[0];
					if (node.AnonymousMethod)
					{
						// `delegate { ... }`: the parameters are not used and cannot be written.
						var anonymous = new AnonymousMethodExpression { Body = (BlockStatement)Build(node.Children[0], exprBuilder) };
						node.ResolveResult = new DecompiledLambdaResolveResult(function, node.Type!, (IType)node.Value!,
							hasParameterList: false, isAnonymousMethod: true, isImplicitlyTyped: false) {
							AttemptedConversionWithTypeMismatch = node.AttemptedConversionWithTypeMismatch
						};
						return Annotate(anonymous, node);
					}
					var lambda = new LambdaExpression();
					lambda.Parameters.AddRange(MakeParameters(function, exprBuilder));
					if (node.ImplicitlyTyped)
					{
						foreach (var pd in lambda.Parameters)
							pd.Type = null!;
					}
					lambda.Body = Build(node.Children[0], exprBuilder);
					node.ResolveResult = new DecompiledLambdaResolveResult(function, node.Type!, (IType)node.Value!,
						hasParameterList: true, isAnonymousMethod: false, isImplicitlyTyped: node.ImplicitlyTyped) {
						AttemptedConversionWithTypeMismatch = node.AttemptedConversionWithTypeMismatch
					};
					return Annotate(lambda, node);
				}
				case RustBodyNode.Using:
				{
					AstNode resourceAcquisition = Build(node.Children[0], exprBuilder);
					if (node.Value is ILVariable variable)
					{
						var declaration = new VariableDeclarationStatement(exprBuilder.ConvertType(variable.Type), variable.Name!, (Expression)resourceAcquisition);
						declaration.Variables.Single().AddAnnotation(new ILVariableResolveResult(variable, variable.Type));
						resourceAcquisition = declaration;
					}
					return Annotate(new UsingStatement {
						ResourceAcquisition = resourceAcquisition,
						EmbeddedStatement = (Statement)Build(node.Children[1], exprBuilder)
					}, node);
				}
				case RustBodyNode.Lock:
					return Annotate(new LockStatement {
						Expression = (Expression)Build(node.Children[0], exprBuilder),
						EmbeddedStatement = (Statement)Build(node.Children[1], exprBuilder)
					}, node);
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
				case RustBodyNode.As:
				{
					var operand = (Expression)Build(node.Children[0], exprBuilder);
					node.ResolveResult = new ConversionResolveResult(node.Type!, operand.GetResolveResult(), Conversion.TryCast);
					return Annotate(new AsExpression(operand, exprBuilder.ConvertType(node.Type!)), node);
				}
				case RustBodyNode.Cast:
				{
					var operand = (Expression)Build(node.Children[0], exprBuilder);
					var cast = new CastExpression(exprBuilder.ConvertType(node.Type!), operand);
					// `TranslatedExpression.ConvertTo` marks a cast to an integer type unchecked.
					if (node.Flag)
						cast.AddAnnotation(CSharp.Transforms.AddCheckedBlocks.UncheckedAnnotation);
					if (node.LambdaConversion)
						node.ResolveResult = new ConversionResolveResult(node.Type!, operand.GetResolveResult(), Semantics.LambdaConversion.Instance);
					if (node.ResolveCast)
						node.ResolveResult = exprBuilder.resolver.WithCheckForOverflow(false).ResolveCast(node.Type!, operand.GetResolveResult());
					// `HandleImplicitConversion`: the implicit conversion from the operand's type.
					if (node.ImplicitConversion)
					{
						var operandResult = operand.GetResolveResult();
						var conversion = CSharp.Resolver.CSharpConversions.Get(exprBuilder.compilation).ImplicitConversion(operandResult.Type, node.Type!);
						node.ResolveResult = new ConversionResolveResult(node.Type!, operandResult, conversion);
					}
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
					// Branches of different types: the resolver converts one to the other's type.
					// Branches converted to a target type come with a plain resolve result instead.
					if (node.Flag)
						node.ResolveResult = exprBuilder.resolver.ResolveConditional(condition.GetResolveResult(), trueExpression.GetResolveResult(), falseExpression.GetResolveResult());
					else if (node.OperatorType != null)
						node.ResolveResult = new OperatorResolveResult(node.OperatorType, System.Linq.Expressions.ExpressionType.Conditional,
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
		/// <summary>
		/// `Disambiguator.IsUnambiguousCall`'s found member: the best candidate of the overload
		/// resolution of the call as written, over the method group of its name.
		/// </summary>
		static IMethod FindOverload(ExpressionBuilder exprBuilder, Expression callee, IMethod method, List<Expression> arguments, List<ResolveResult> argumentResults)
		{
			var resolver = exprBuilder.resolver;
			var target = callee is MemberReferenceExpression mre ? mre.Target.GetResolveResult() : null;
			bool typeArgumentsWritten = callee is MemberReferenceExpression { TypeArguments.Count: > 0 }
				|| callee is IdentifierExpression { TypeArguments.Count: > 0 };
			var typeArguments = typeArgumentsWritten ? method.TypeArguments.ToArray() : Array.Empty<IType>();
			var group = target == null
				? resolver.ResolveSimpleName(method.Name, typeArguments, isInvocationTarget: true)
				: new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule)
					.Lookup(target, method.Name, typeArguments, isInvocation: true);
			var names = arguments.Any(a => a is NamedArgumentExpression)
				? arguments.Select(a => (a as NamedArgumentExpression)?.Name).ToArray()
				: null;
			var or = new OverloadResolution(resolver.Compilation, argumentResults.ToArray(), names!, typeArguments, conversions: resolver.conversions);
			or.AddMethodLists(((MethodGroupResolveResult)group).MethodsGroupedByDeclaringType.ToArray());
			return (IMethod?)or.GetBestCandidateWithSubstitutedTypeArguments()
				?? throw new RustEngineException($"The call of {method.Name} no longer resolves to an overload");
		}

		/// <summary>The designation of a pattern variable, annotated with its resolve result.</summary>
		static SingleVariableDesignation Designation(ILVariable variable)
		{
			var designation = new SingleVariableDesignation { Identifier = variable.Name! };
			designation.AddAnnotation(new ILVariableResolveResult(variable));
			return designation;
		}

		/// <summary>`ExpressionBuilder.MakeParameters`, with the parameter types written.</summary>
		static List<ParameterDeclaration> MakeParameters(ILFunction function, ExpressionBuilder exprBuilder)
		{
			var variables = function.Variables.Where(v => v.Kind == VariableKind.Parameter).ToDictionary(v => v.Index!.Value);
			var result = new List<ParameterDeclaration>(function.Parameters.Count);
			for (int i = 0; i < function.Parameters.Count; i++)
			{
				var parameter = function.Parameters[i];
				var pd = exprBuilder.astBuilder.ConvertParameter(parameter);
				if (variables.TryGetValue(i, out var v))
				{
					pd.AddAnnotation(new ILVariableResolveResult(v, parameter.Type));
					pd.Name = v.Name!;
				}
				if (string.IsNullOrEmpty(pd.Name) && !pd.Type.IsArgList())
					pd.Name = "P_" + i;
				result.Add(pd);
			}
			return result;
		}

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
