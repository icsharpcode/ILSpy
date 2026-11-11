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
using System.Runtime.CompilerServices;
using System.Threading;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;

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

		public byte Kind;
		/// <summary>The instructions the node is annotated with, in order.</summary>
		public ILInstruction[] IL = [];
		public ResolveResult? ResolveResult;
		public Leave? ImplicitReturn;
		public RustBodyNode[] Children = [];
		public IType? Type;
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
					return WithIL(block, node);
				}
				case RustBodyNode.Return:
				{
					var statement = node.Children.Length == 1
						? new ReturnStatement((Expression)Build(node.Children[0], exprBuilder))
						: new ReturnStatement();
					return WithIL(statement, node);
				}
				case RustBodyNode.Empty:
					return WithIL(new EmptyStatement(), node);
				case RustBodyNode.ConstantValue:
				{
					var expression = exprBuilder.ConvertConstantValue(new ConstantResolveResult(node.Type!, node.Value), node.Flag).Expression;
					return WithIL(expression, node);
				}
				case RustBodyNode.StringLiteral:
					return WithRR(WithIL(new PrimitiveExpression(node.Value!), node), node);
				case RustBodyNode.Null:
					return WithIL(WithRR(new NullReferenceExpression(), node), node);
				case RustBodyNode.Identifier:
					return WithIL(WithRR(new IdentifierExpression((string)node.Value!), node), node);
				case RustBodyNode.This:
					return WithIL(WithRR(new ThisReferenceExpression(), node), node);
				default:
					throw new RustEngineException($"unknown C# node kind {node.Kind}");
			}
		}

		static T WithIL<T>(T syntax, RustBodyNode node) where T : AstNode
		{
			foreach (var inst in node.IL)
				syntax.AddAnnotation(inst);
			return syntax;
		}

		static T WithRR<T>(T syntax, RustBodyNode node) where T : AstNode
		{
			if (node.ResolveResult != null)
				syntax.AddAnnotation(node.ResolveResult);
			return syntax;
		}
	}
}
