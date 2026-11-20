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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Instrumentation;
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
		public const byte InDirection = 68;
		public const byte ParenthesizedDesignation = 69;
		public const byte SingleDesignation = 70;
		public const byte UsingExpanded = 71;
		public const byte GotoCase = 72;
		public const byte GotoDefault = 73;
		public const byte PointerUnary = 120;
		public const byte SizeOf = 121;
		public const byte Fixed = 122;
		public const byte UnsafeIntrinsic = 123;
		public const byte StackAlloc = 124;
		public const byte Undocumented = 125;
		public const byte UnaryOperator = 90;
		public const byte AnonymousTypeCreate = 74;
		public const byte WithInitializer = 75;
		public const byte Utf8StringLiteral = 130;
		public const byte Error = 131;
		public const byte BlockExpression = 132;
		public const byte BoolLiteral = 140;
		public const byte InitializerIndexer = 160;
		public const byte AsRefReadOnlyHelper = 161;
		/// <summary>A comment added as leading trivia of its only child, which it builds into.</summary>
		public const byte LeadingComment = 190;
		/// <summary>A range <c>start..end</c>, a child left out being null.</summary>
		public const byte Range = 191;

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
		/// <summary>For an operator: the operator method (null for a predefined one) and lifted flag of its <see cref="OperatorResolveResult"/> (of <see cref="OperatorType"/>), and the <c>AddCheckedBlocks</c> annotation of its first child.</summary>
		public (IMethod? Method, bool IsLifted, byte OperandOverflow)? ExplicitOperator;
		/// <summary>For a binary operator: the operator method of its <see cref="InvocationResolveResult"/> without target, whose arguments are the operands'.</summary>
		public IMethod? OperatorInvocation;
		public Leave? ImplicitReturn;
		public RustBodyNode[] Children = [];
		public IType? Type;
		/// <summary>For an arithmetic operator or a cast: its <c>AddCheckedBlocks</c> annotation, 0 none, 1 checked, 2 unchecked, 3 explicitly unchecked (casts only).</summary>
		public byte Overflow;
		/// <summary>For a member reference or name: whether its resolve result names the member the name is looked up to.</summary>
		public bool LookUpMember;
		/// <summary>For a cast: whether its resolve result is <c>ResolveCast</c> over the operand's.</summary>
		public bool ResolveCast;
		/// <summary>For a cast: whether its resolve result is the implicit conversion from the operand's type.</summary>
		public bool ImplicitConversion;
		/// <summary>For a cast: whether its resolve result is the lambda conversion of the lambda it casts.</summary>
		public bool LambdaConversion;
		/// <summary>For a cast: the method whose method group conversion over the operand's resolve result is the cast's (a function pointer <c>ldftn</c>).</summary>
		public IMethod? MethodGroupConversion;
		/// <summary>For a cast: whether its resolve result is the inline array conversion of the operand's.</summary>
		public bool InlineArrayConversion;
		/// <summary>For a binary operator: whether its resolve result is <c>ResolveBinaryOperator</c> over the operands'.</summary>
		public bool ResolveOperator;
		/// <summary>For a method reference: whether it has a target, the type arguments and method of its <see cref="MethodGroupResolveResult"/>, and whether it references an extension method.</summary>
		public (bool HasTarget, IType[] TypeArguments, IMethod Method, bool Extension)? MethodGroup;
		/// <summary>For a lambda: <see cref="DecompiledLambdaResolveResult.IsImplicitlyTyped"/>.</summary>
		public bool ImplicitlyTyped;
		/// <summary>For a lambda: <see cref="DecompiledLambdaResolveResult.IsAnonymousMethod"/>.</summary>
		public bool AnonymousMethod;
		/// <summary>For a lambda: <see cref="DecompiledLambdaResolveResult.HasParameterList"/>.</summary>
		public bool HasParameterList;
		/// <summary>For a lambda: whether it is async.</summary>
		public bool IsAsync;
		/// <summary>For a lambda: whether its parameter declarations keep <c>params</c> and their default values.</summary>
		public bool KeepParamsAndDefaults;
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
		/// <summary>For a constant: the expression the engine wrote it as; null when the host converts it.</summary>
		public Expression? ConstantSyntax;
		/// <summary>For a recursive pattern: the member of each sub-pattern.</summary>
		public IMember[] Members = [];
		public bool Flag;
		/// <summary>For a dynamic operation: its resolve result, which is completed from the built node.</summary>
		public RustDynamicResult? Dynamic;
	}

	/// <summary>
	/// The resolve result of a dynamic operation (<c>Rr::Dynamic*</c> in rust/ilspy-decompiler/src/csharp/ast.rs),
	/// with the symbol <c>ExpressionBuilder.CreateDynamic*Symbol</c> synthesizes; the arguments and
	/// operands are the built node's.
	/// </summary>
	internal sealed class RustDynamicResult
	{
		/// <summary>70 member, 71 invocation or indexing, 72 constructor, 73 conversion, 74 compound operator.</summary>
		public byte Tag;
		/// <summary>The target's resolve result; <see cref="RustSeam2.PendingTarget"/> or null for the built target's.</summary>
		public ResolveResult? Target;
		public bool Indexing;
		public IMember? Symbol;
		public IType? Type;
		public bool Explicit;
		public int Operation;
	}

	/// <summary>
	/// Seam 2, at body level: the statement block of a method body comes from the engine, which
	/// translates the ILAst it handed over through seam 1. A body with constructs the engine does
	/// not translate yet falls back to the managed statement builder and is counted.
	/// </summary>
	/// <summary>
	/// The transforms before the built-in AST transforms of a request that the engine performs
	/// itself (<see cref="RustSeam2.AstRequest"/>).
	/// </summary>
	[Flags]
	public enum AstRequestFlags
	{
		None = 0,
		RemoveEmbeddedAttributes = 1,
		RemoveCompilerAttribute = 2,
		RemoveNamespaceMy = 4,
	}

	/// <summary>The engine's member walk of a type (<see cref="RustSeam2.MemberWalk"/>).</summary>
	internal sealed class MemberWalkResult
	{
		readonly Dictionary<IEntity, byte> decisions;

		readonly byte enumDisplayMode;
		readonly bool removesDefaultMember;

		internal MemberWalkResult(IEntity[] entities, Dictionary<IEntity, byte> decisions, byte enumDisplayMode, bool removesDefaultMember)
		{
			Entities = entities;
			this.decisions = decisions;
			this.enumDisplayMode = enumDisplayMode;
			this.removesDefaultMember = removesDefaultMember;
		}

		/// <summary>
		/// Whether the declaration of <paramref name="typeDef"/> drops <c>[DefaultMember]</c>, as the
		/// engine decides it from the walk; a difference from <paramref name="managed"/> is counted.
		/// </summary>
		internal bool RemovesDefaultMember(ITypeDefinition typeDef, bool managed)
		{
			if (removesDefaultMember != managed)
				RustSeam2.CountWalkDivergence(typeDef, $"DefaultMember removal {removesDefaultMember}, managed {managed}");
			return removesDefaultMember;
		}

		/// <summary>
		/// The engine's <c>DetectBestEnumValueDisplayMode</c> of an enum, or <paramref name="managed"/>
		/// where the host decides it; a difference from <paramref name="managed"/> is counted.
		/// </summary>
		internal EnumValueDisplayMode EnumDisplayMode(ITypeDefinition typeDef, EnumValueDisplayMode managed)
		{
			if (enumDisplayMode == 255)
				return managed;
			var mode = (EnumValueDisplayMode)enumDisplayMode;
			if (mode != managed)
				RustSeam2.CountWalkDivergence(typeDef, $"enum display mode {mode}, managed {managed}");
			return mode;
		}

		/// <summary><c>allOrderedEntities</c>.</summary>
		public IEntity[] Entities { get; }

		/// <summary>Whether the walk leaves <paramref name="entity"/> out without further analysis.</summary>
		public bool IsHidden(IEntity entity) => decisions[entity] == 1;

		/// <summary>
		/// Whether <paramref name="entity"/> is hidden only as the backing field of an event, which
		/// the walk decompiles when the event is not automatic.
		/// </summary>
		public bool IsHiddenUnlessEventIsCustom(IEntity entity) => decisions[entity] == 2;
	}

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

		static long typeSyntaxFallbacks;

		/// <summary>
		/// Types of a served body the engine sent no syntax for (a function pointer or intersection
		/// type, or a type no wire entry stands for), which the managed builder converted.
		/// </summary>
		public static long TypeSyntaxFallbacks => Interlocked.Read(ref typeSyntaxFallbacks);

		static long declarationsServed;
		static long declarationFallbacks;
		static readonly ConcurrentDictionary<string, int> declarationFallbackReasons = new ConcurrentDictionary<string, int>();

		/// <summary>Member declarations the engine wrote.</summary>
		public static long DeclarationsServed => Interlocked.Read(ref declarationsServed);

		/// <summary>Member declarations the engine was asked for but the managed builder wrote.</summary>
		public static long DeclarationFallbacks => Interlocked.Read(ref declarationFallbacks);

		/// <summary>Why the engine did not write the declarations that fell back, with how often.</summary>
		public static IReadOnlyDictionary<string, int> DeclarationFallbackReasons => declarationFallbackReasons;

		/// <summary>
		/// <c>typeSystemAstBuilder.ConvertEntity(field)</c> from the engine; <paramref name="managed"/>, the
		/// managed builder's, counted, where the engine does not write it.
		/// </summary>
		/// <summary>
		/// The <c>EnumMemberDeclaration</c> of the enum constant <paramref name="field"/> from the
		/// engine; <paramref name="managed"/>, counted, where the engine does not write it.
		/// </summary>
		internal static EntityDeclaration ConvertEnumMember(IDecompilerTypeSystem typeSystem, IField field, TypeSystemAstBuilder typeSystemAstBuilder, DecompilerSettings settings, EntityDeclaration managed)
		{
			if (Enabled && RustEngine.For(typeSystem, out _) is { } engine
				&& field.ParentModule == typeSystem.MainModule
				&& !field.MetadataToken.IsNil)
			{
				var decl = engine.EntitySyntax(field, typeSystemAstBuilder.UseSpecialConstants, settings, out string? reason, asEnumMember: true);
				if (decl != null)
				{
					Interlocked.Increment(ref declarationsServed);
					return decl;
				}
				ThrowIfStrict("this declaration", reason);
				Interlocked.Increment(ref declarationFallbacks);
				declarationFallbackReasons.AddOrUpdate(reason ?? "unknown", 1, (_, n) => n + 1);
			}
			return managed;
		}

		internal static EntityDeclaration ConvertField(IDecompilerTypeSystem typeSystem, IField field, TypeSystemAstBuilder typeSystemAstBuilder, DecompilerSettings settings, EntityDeclaration managed)
		{
			if (Enabled && RustEngine.For(typeSystem, out _) is { } engine
				&& field.ParentModule == typeSystem.MainModule
				&& !field.MetadataToken.IsNil)
			{
				var decl = engine.EntitySyntax(field, typeSystemAstBuilder.UseSpecialConstants, settings, out string? reason);
				if (decl != null)
				{
					Interlocked.Increment(ref declarationsServed);
					return decl;
				}
				ThrowIfStrict("this declaration", reason);
				Interlocked.Increment(ref declarationFallbacks);
				declarationFallbackReasons.AddOrUpdate(reason ?? "unknown", 1, (_, n) => n + 1);
			}
			return managed;
		}

		/// <summary>
		/// <c>typeSystemAstBuilder.ConvertEntity(method)</c> from the engine; <paramref name="managed"/>,
		/// the managed builder's, counted, where the engine does not write it.
		/// </summary>
		internal static EntityDeclaration ConvertMethod(IDecompilerTypeSystem typeSystem, IMethod method, TypeSystemAstBuilder typeSystemAstBuilder, DecompilerSettings settings, EntityDeclaration managed)
		{
			if (Enabled && RustEngine.For(typeSystem, out _) is { } engine
				&& method.ParentModule == typeSystem.MainModule
				&& !method.MetadataToken.IsNil
				&& method is not ICSharpCode.Decompiler.TypeSystem.Implementation.LocalFunctionMethod)
			{
				var decl = engine.EntitySyntax(method, typeSystemAstBuilder.UseSpecialConstants, settings, out string? reason);
				if (decl != null)
				{
					Interlocked.Increment(ref declarationsServed);
					return decl;
				}
				ThrowIfStrict("this declaration", reason);
				Interlocked.Increment(ref declarationFallbacks);
				declarationFallbackReasons.AddOrUpdate(reason ?? "unknown", 1, (_, n) => n + 1);
			}
			return managed;
		}

		/// <summary>
		/// <c>typeSystemAstBuilder.ConvertEntity(member)</c> of a type, property or event from the engine;
		/// <paramref name="managed"/>, the managed builder's, counted, where the engine does not
		/// write it.
		/// </summary>
		/// <summary>
		/// <c>typeSystemAstBuilder.ConvertExtension(group)</c> of the extension block of
		/// <paramref name="markerType"/> from the engine; <paramref name="managed"/>, counted, where
		/// the engine does not write it.
		/// </summary>
		internal static EntityDeclaration ConvertExtension(IDecompilerTypeSystem typeSystem, ITypeDefinition markerType, TypeSystemAstBuilder typeSystemAstBuilder, DecompilerSettings settings, EntityDeclaration managed)
		{
			if (Enabled && RustEngine.For(typeSystem, out _) is { } engine
				&& markerType.ParentModule == typeSystem.MainModule
				&& !markerType.MetadataToken.IsNil)
			{
				var decl = engine.EntitySyntax(markerType, typeSystemAstBuilder.UseSpecialConstants, settings, out string? reason, asExtension: true);
				if (decl != null)
				{
					Interlocked.Increment(ref declarationsServed);
					return decl;
				}
				ThrowIfStrict("this declaration", reason);
				Interlocked.Increment(ref declarationFallbacks);
				declarationFallbackReasons.AddOrUpdate(reason ?? "unknown", 1, (_, n) => n + 1);
			}
			return managed;
		}

		internal static EntityDeclaration ConvertDeclaration(IDecompilerTypeSystem typeSystem, IEntity member, TypeSystemAstBuilder typeSystemAstBuilder, DecompilerSettings settings, EntityDeclaration managed)
		{
			if (Enabled && RustEngine.For(typeSystem, out _) is { } engine
				&& member.ParentModule == typeSystem.MainModule
				&& !member.MetadataToken.IsNil)
			{
				var decl = engine.EntitySyntax(member, typeSystemAstBuilder.UseSpecialConstants, settings, out string? reason, typeSystemAstBuilder.UseCustomEvents);
				if (decl != null)
				{
					Interlocked.Increment(ref declarationsServed);
					return decl;
				}
				ThrowIfStrict("this declaration", reason);
				Interlocked.Increment(ref declarationFallbacks);
				declarationFallbackReasons.AddOrUpdate(reason ?? "unknown", 1, (_, n) => n + 1);
			}
			return managed;
		}

		static long constantSyntaxFallbacks;

		/// <summary>Constants of a served body the engine sent no syntax for, which the managed builder converted.</summary>
		public static long ConstantSyntaxFallbacks => Interlocked.Read(ref constantSyntaxFallbacks);

		public static void ResetCounters()
		{
			Interlocked.Exchange(ref served, 0);
			Interlocked.Exchange(ref fallbacks, 0);
			Interlocked.Exchange(ref typeSyntaxFallbacks, 0);
			Interlocked.Exchange(ref constantSyntaxFallbacks, 0);
			Interlocked.Exchange(ref declarationsServed, 0);
			Interlocked.Exchange(ref declarationFallbacks, 0);
			declarationFallbackReasons.Clear();
			fallbackReasons.Clear();
		}

		/// <summary>The message the body being built came in, whose type syntax it is written with.</summary>
		[ThreadStatic]
		static WireReader? currentTypes;

		/// <summary>Whether seam 2 fallback is disabled (<c>ILSPY_RUST_STRICT_SEAM2=1</c>).</summary>
		// shortcut: separate from ILSPY_RUST_STRICT while the test matrix still hits seam 2 gaps; merge once it runs green strict.
		public static bool Strict => Environment.GetEnvironmentVariable("ILSPY_RUST_STRICT_SEAM2") == "1";

		/// <summary>
		/// Under strict mode (<see cref="Strict"/>) a seam 2 fallback where the engine lacks something
		/// is an error. Request routing (<see cref="AstRequest"/>) is not such a gap.
		/// </summary>
		static void ThrowIfStrict(string what, string? reason)
		{
			if (Strict)
				throw new RustEngineException($"strict mode: the engine cannot serve {what}: {reason ?? "unknown reason"}");
		}

		/// <summary>
		/// <c>exprBuilder.ConvertType(type)</c> as the engine built it; the managed conversion,
		/// counted, for a type the engine sent no syntax for.
		/// </summary>
		/// <summary>A constant the managed builder converted, counted.</summary>
		static Expression ConstantFallback(Expression expression)
		{
			ThrowIfStrict("this constant syntax", expression.ToString());
			Interlocked.Increment(ref constantSyntaxFallbacks);
			return expression;
		}

		static AstType ConvertType(ExpressionBuilder exprBuilder, IType type)
		{
			if (currentTypes?.TypeSyntax(type) is { } syntax)
				return syntax;
			ThrowIfStrict("this type syntax", type.ToString());
			Interlocked.Increment(ref typeSyntaxFallbacks);
			return exprBuilder.ConvertType(type);
		}

		/// <summary>
		/// The syntax of the compilation's Unsafe. Where no module defines it, the compilation's
		/// is MinimalCorlib's, which the engine knows only by name (<paramref name="engineType"/>);
		/// the engine's syntax of that name then carries the compilation's type, as the managed
		/// <c>ConvertType</c> annotates it, so <c>FullyQualifyAmbiguousTypeNames</c> qualifies it alike.
		/// </summary>
		static AstType UnsafeTypeSyntax(ExpressionBuilder exprBuilder, IType engineType)
		{
			var unsafeType = exprBuilder.compilation.FindType(KnownTypeCode.Unsafe);
			if (currentTypes?.TypeSyntax(unsafeType) is { } syntax)
				return syntax;
			if (unsafeType.GetDefinition()?.ParentModule is not MetadataModule && engineType.Kind == TypeKind.Unknown
				&& engineType.FullName == unsafeType.FullName
				&& currentTypes?.TypeSyntax(engineType) is { } engineSyntax)
			{
				engineSyntax.RemoveAnnotations<TypeResolveResult>();
				engineSyntax.AddAnnotation(new TypeResolveResult(unsafeType));
				return engineSyntax;
			}
			return ConvertType(exprBuilder, unsafeType);
		}

		static long hiddenDecisions;
		static long hiddenDivergences;
		static readonly ConcurrentDictionary<string, int> hiddenDivergenceMembers = new ConcurrentDictionary<string, int>();

		/// <summary><c>MemberIsHidden</c> decisions the engine made.</summary>
		public static long HiddenDecisions => Interlocked.Read(ref hiddenDecisions);

		/// <summary>Engine <c>MemberIsHidden</c> decisions that differ from the managed one.</summary>
		public static long HiddenDivergences => Interlocked.Read(ref hiddenDivergences);

		/// <summary>The members whose <c>MemberIsHidden</c> decisions differ, with how often.</summary>
		public static IReadOnlyDictionary<string, int> HiddenDivergenceMembers => hiddenDivergenceMembers;

		/// <summary>
		/// <c>CSharpDecompiler.MemberIsHidden</c> from the engine for a member of the main module;
		/// null where the host decides it (the engine is off or the member is of another module).
		/// Each decision is checked against the managed one, and a difference is counted.
		/// </summary>
		internal static bool? MemberIsHidden(IDecompilerTypeSystem typeSystem, MetadataFile? module, EntityHandle member, DecompilerSettings settings)
		{
			if (!Enabled || module == null || member.IsNil
				|| typeSystem.MainModule is not MetadataModule { MetadataFile: var mainFile } || mainFile != module
				|| RustEngine.For(typeSystem, out _) is not { } engine)
				return null;
			bool hidden = engine.MemberIsHidden(member, settings);
			Interlocked.Increment(ref hiddenDecisions);
			if (hidden != CSharp.CSharpDecompiler.MemberIsHidden(module, member, settings))
			{
				Interlocked.Increment(ref hiddenDivergences);
				hiddenDivergenceMembers.AddOrUpdate($"{module.Name} 0x{MetadataTokens.GetToken(member):x8}", 1, (_, n) => n + 1);
			}
			return hidden;
		}

		static long walksServed;
		static long walkFallbacks;
		static readonly ConcurrentDictionary<string, int> walkFallbackReasons = new ConcurrentDictionary<string, int>();
		static readonly ConcurrentDictionary<string, int> walkDivergences = new ConcurrentDictionary<string, int>();

		/// <summary>Types whose member walk came from the engine.</summary>
		public static long WalksServed => Interlocked.Read(ref walksServed);

		/// <summary>Types whose member walk the host did, with why.</summary>
		public static IReadOnlyDictionary<string, int> WalkFallbackReasons => walkFallbackReasons;

		/// <summary>Types whose engine member walk differs from the managed one, with how often.</summary>
		public static IReadOnlyDictionary<string, int> WalkDivergences => walkDivergences;

		/// <summary>
		/// The engine's member walk of <c>DoDecompile(ITypeDefinition)</c> for
		/// <paramref name="typeDef"/>: its entities in order and, per entity, whether the walk
		/// decompiles it. Null, counted, where the host walks the type. The order and the decisions
		/// are checked against the managed <paramref name="managedOrder"/> and <c>MemberIsHidden</c>,
		/// and a difference is counted.
		/// </summary>
		internal static MemberWalkResult? MemberWalk(IDecompilerTypeSystem typeSystem, MetadataModule module, ITypeDefinition typeDef, DecompilerSettings settings, IReadOnlyList<IEntity> managedOrder)
		{
			if (!Enabled || typeDef.ParentModule != typeSystem.MainModule || typeDef.MetadataToken.IsNil
				|| RustEngine.For(typeSystem, out _) is not { } engine)
				return null;
			var walk = engine.MemberWalk(typeDef, settings, out string? reason, out byte enumDisplayMode, out bool removesDefaultMember);
			if (walk == null)
			{
				ThrowIfStrict("this type's member walk", reason);
				Interlocked.Increment(ref walkFallbacks);
				walkFallbackReasons.AddOrUpdate(reason ?? "unknown", 1, (_, n) => n + 1);
				return null;
			}
			Interlocked.Increment(ref walksServed);
			var entities = new IEntity[walk.Count];
			var decisions = new Dictionary<IEntity, byte>();
			for (int i = 0; i < walk.Count; i++)
			{
				var (handle, decision) = walk[i];
				IEntity entity = handle.Kind switch {
					HandleKind.TypeDefinition => module.GetDefinition((TypeDefinitionHandle)handle),
					HandleKind.FieldDefinition => module.GetDefinition((FieldDefinitionHandle)handle),
					HandleKind.MethodDefinition => module.GetDefinition((MethodDefinitionHandle)handle),
					HandleKind.PropertyDefinition => module.GetDefinition((PropertyDefinitionHandle)handle),
					HandleKind.EventDefinition => module.GetDefinition((EventDefinitionHandle)handle),
					_ => throw new RustEngineException($"the member walk of {typeDef.FullName} names {handle.Kind}"),
				};
				entities[i] = entity;
				decisions[entity] = decision;
			}
			// A generic record lists its fields and properties specialized by its own type parameters;
			// the engine names their definitions.
			var managed = managedOrder.Where(e => !e.MetadataToken.IsNil)
				.Select(e => e is IMember { MemberDefinition: var d } ? d : e).ToList();
			string? difference = null;
			if (!managed.SequenceEqual(entities))
				difference = "order";
			else if (managed.FirstOrDefault(e => (decisions[e] != 0) != CSharp.CSharpDecompiler.MemberIsHidden(module.MetadataFile, e.MetadataToken, settings)) is { } wrong)
				difference = $"hidden 0x{MetadataTokens.GetToken(wrong.MetadataToken):x8}";
			if (difference != null)
				CountWalkDivergence(typeDef, difference);
			return new MemberWalkResult(entities, decisions, enumDisplayMode, removesDefaultMember);
		}

		/// <summary>
		/// The engine's comment after the data of <paramref name="field"/>, a field of the main module
		/// with an RVA; <paramref name="managed"/> where the host writes it. A difference is counted.
		/// </summary>
		internal static string FieldDataComment(IDecompilerTypeSystem typeSystem, IField field, string managed)
		{
			if (!Enabled || field.ParentModule != typeSystem.MainModule || field.MetadataToken.IsNil
				|| RustEngine.For(typeSystem, out _) is not { } engine)
				return managed;
			string comment = engine.FieldDataComment(field.MetadataToken);
			if (comment != managed && field.DeclaringTypeDefinition is { } type)
				CountWalkDivergence(type, $"data comment of {field.Name}");
			return comment;
		}

		static long newModifierDecisions;
		static readonly ConcurrentDictionary<string, int> newModifierFallbackReasons = new ConcurrentDictionary<string, int>();

		/// <summary><c>SetNewModifier</c> decisions the engine made.</summary>
		public static long NewModifierDecisions => Interlocked.Read(ref newModifierDecisions);

		/// <summary>Why the host made the <c>SetNewModifier</c> decisions it made, with how often.</summary>
		public static IReadOnlyDictionary<string, int> NewModifierFallbackReasons => newModifierFallbackReasons;

		/// <summary>
		/// The engine's <c>SetNewModifier</c> decision for <paramref name="entity"/>; null, counted,
		/// where the host decides it. The managed decision (<paramref name="managed"/>) is computed
		/// beside it and a difference is counted.
		/// </summary>
		internal static bool? HidesBaseMember(IDecompilerTypeSystem typeSystem, IEntity entity, Func<bool> managed)
		{
			if (!Enabled || entity.ParentModule != typeSystem.MainModule || entity.MetadataToken.IsNil
				|| RustEngine.For(typeSystem, out _) is not { } engine)
				return null;
			var hides = engine.HidesBaseMember(entity.MetadataToken, out string? reason);
			if (hides == null)
			{
				ThrowIfStrict("this new-modifier decision", reason);
				newModifierFallbackReasons.AddOrUpdate(reason ?? "unknown", 1, (_, n) => n + 1);
				return null;
			}
			Interlocked.Increment(ref newModifierDecisions);
			if (hides != managed() && entity.DeclaringTypeDefinition is { } type)
				CountWalkDivergence(type, $"new modifier of {entity.Name}");
			return hides;
		}

		static long requestsServed;
		static readonly ConcurrentDictionary<string, int> requestFallbackReasons = new ConcurrentDictionary<string, int>();

		/// <summary>Decompile requests whose AST transform pipeline the engine may serve.</summary>
		public static long RequestsServed => Interlocked.Read(ref requestsServed);

		/// <summary>Why the AST transform pipeline of a request stays with the host, with how often.</summary>
		public static IReadOnlyDictionary<string, int> RequestFallbackReasons => requestFallbackReasons;

		/// <summary>
		/// The transforms a caller may put before the built-in AST transforms that the engine knows,
		/// by type name, and the flag each becomes.
		/// </summary>
		static readonly Dictionary<string, AstRequestFlags> recognizedEarlyTransforms = new Dictionary<string, AstRequestFlags> {
			["RemoveEmbeddedAttributes"] = AstRequestFlags.RemoveEmbeddedAttributes,
			["RemoveCompilerAttribute"] = AstRequestFlags.RemoveCompilerAttribute,
			["RemoveNamespaceMy"] = AstRequestFlags.RemoveNamespaceMy,
		};

		/// <summary>
		/// Whether the engine may serve the AST transform pipeline of a request that runs
		/// <paramref name="transforms"/>: the built-in transforms in their order, after which a
		/// caller may append any, and before which only the recognized ones, which become the
		/// returned flags. A request that records its steps, puts another transform first, or
		/// changes the built-in list stays with the host, counted per request.
		/// </summary>
		internal static AstRequestFlags? AstRequest(IReadOnlyList<CSharp.Transforms.IAstTransform> transforms, bool recordSteps)
		{
			if (!Enabled)
				return null;
			string? reason = null;
			var flags = AstRequestFlags.None;
			if (recordSteps)
			{
				reason = "a request that records its steps";
			}
			else if (DecompilerEventSource.Log.IsTransformTracingEnabled())
			{
				// AstTransformExecuted events need every transform to run managed.
				reason = "a request that traces its transforms";
			}
			else
			{
				var builtIn = CSharp.CSharpDecompiler.GetAstTransforms().Select(t => t.GetType()).ToList();
				int start = -1;
				for (int i = 0; i + builtIn.Count <= transforms.Count && start < 0; i++)
				{
					if (builtIn.Select((t, k) => transforms[i + k].GetType() == t).All(same => same))
						start = i;
				}
				if (start < 0)
				{
					reason = "a changed list of built-in AST transforms";
				}
				else
				{
					for (int i = 0; i < start && reason == null; i++)
					{
						string name = transforms[i].GetType().Name;
						if (recognizedEarlyTransforms.TryGetValue(name, out var flag))
							flags |= flag;
						else
							reason = $"the early AST transform {name}";
					}
				}
			}
			if (reason != null)
			{
				requestFallbackReasons.AddOrUpdate(reason, 1, (_, n) => n + 1);
				return null;
			}
			Interlocked.Increment(ref requestsServed);
			return flags;
		}

		static long roundTrips;

		/// <summary>Syntax trees that went through the engine (<see cref="RunAstTransforms"/>).</summary>
		public static long RoundTrips => Interlocked.Read(ref roundTrips);

		/// <summary>
		/// The AST transforms of <paramref name="transforms"/> the engine ran on <paramref name="root"/>,
		/// which the host skips: the early ones the request check turned into <paramref name="flags"/>
		/// and the first <see cref="RustEngine.ASTPrefix"/> built-in ones. The tree goes through the
		/// engine for every request it serves, so the round trip itself is under test even where it
		/// runs no transform.
		/// </summary>
		internal static HashSet<CSharp.Transforms.IAstTransform> RunAstTransforms(IDecompilerTypeSystem typeSystem, AstNode root, AstRequestFlags flags, IReadOnlyList<CSharp.Transforms.IAstTransform> transforms, CSharp.Transforms.TransformContext context)
		{
			var served = new HashSet<CSharp.Transforms.IAstTransform>();
			if (RustEngine.For(typeSystem, out _) is not { } engine)
				return served;
			int prefix = RustEngine.ASTPrefix;
			int start = BuiltInStart(transforms);
			if (prefix > 0)
			{
				// shortcut: the early transforms the request check recognized run managed ahead of the
				// engine's built-in prefix, which must see their result; port them to send flags instead.
				for (int i = 0; i < start; i++)
					transforms[i].Run(root, context);
			}
			engine.RunAstTransforms(root, prefix, AstRequestFlags.None, context: context);
			Interlocked.Increment(ref roundTrips);
			if (prefix == 0)
				return served;
			// The early transforms, then the engine's built-in prefix.
			int end = start + prefix;
			for (int i = 0; i < end; i++)
				served.Add(transforms[i]);
			return served;
		}

		static int BuiltInStart(IReadOnlyList<CSharp.Transforms.IAstTransform> transforms)
		{
			var builtIn = CSharp.CSharpDecompiler.GetAstTransforms().Select(t => t.GetType()).ToList();
			for (int i = 0; i + builtIn.Count <= transforms.Count; i++)
			{
				if (builtIn.Select((t, k) => transforms[i + k].GetType() == t).All(same => same))
					return i;
			}
			throw new InvalidOperationException("a request the check served without the built-in AST transforms");
		}

		internal static void CountWalkDivergence(ITypeDefinition typeDef, string difference)
			=> walkDivergences.AddOrUpdate($"{typeDef.ParentModule?.AssemblyName} {typeDef.FullName}: {difference}", 1, (_, n) => n + 1);

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

		[ThreadStatic]
		static string? lastFallbackReason;

		/// <summary>
		/// The reason of the last body the engine did not serve on this thread, for tests that
		/// decompile one method and ask whether the engine translated its body; tests reset it.
		/// </summary>
		public static string? LastFallbackReason {
			get => lastFallbackReason;
			set => lastFallbackReason = value;
		}

		sealed class Translation
		{
			public RustBodyNode? Body;
			public string? Reason;
			public WireReader? Types;
		}

		/// <summary>
		/// Records the engine's translation of <paramref name="function"/>'s body, or why there is
		/// none, with the message whose type syntax the body is written with.
		/// </summary>
		internal static void Attach(ILFunction function, RustBodyNode? body, string? reason, WireReader? types = null)
		{
			translations.Remove(function);
			translations.Add(function, new Translation { Body = body, Reason = reason, Types = types });
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
				lastFallbackReason = translation.Reason ?? "unknown";
				ThrowIfStrict("this body", translation.Reason);
				Interlocked.Increment(ref fallbacks);
				fallbackReasons.AddOrUpdate(translation.Reason ?? "unknown", 1, (_, n) => n + 1);
				return null;
			}
			var saved = currentTypes;
			currentTypes = translation.Types;
			Statement statement;
			try
			{
				statement = (Statement)Build(translation.Body, statementBuilder.exprBuilder);
			}
			finally
			{
				currentTypes = saved;
			}
			Interlocked.Increment(ref served);
			return statement as BlockStatement ?? new BlockStatement { statement };
		}

		/// <summary>The checked or unchecked annotation of <see cref="RustBodyNode.Overflow"/> (1 checked, 2 unchecked, 3 explicitly unchecked).</summary>
		static T AddOverflowAnnotation<T>(T expression, RustBodyNode node) where T : Expression
			=> AddOverflowAnnotation(expression, node.Overflow);

		static T AddOverflowAnnotation<T>(T expression, byte overflow) where T : Expression
		{
			if (overflow != 0)
			{
				expression.AddAnnotation(overflow switch {
					1 => CSharp.Transforms.AddCheckedBlocks.CheckedAnnotation,
					2 => CSharp.Transforms.AddCheckedBlocks.UncheckedAnnotation,
					_ => CSharp.Transforms.AddCheckedBlocks.ExplicitUncheckedAnnotation,
				});
			}
			return expression;
		}

		/// <summary>A call target's resolve result that is the one of its built target expression.</summary>
		internal static readonly ResolveResult PendingTarget = new ResolveResult(SpecialType.UnknownType);

		/// <summary>A by-reference resolve result of a cast element: the built element's resolve result.</summary>
		static void CompletePendingElement(RustBodyNode node, Expression element)
		{
			if (node.ResolveResult is ByReferenceResolveResult pending && ReferenceEquals(pending.ElementResult, PendingTarget))
				node.ResolveResult = new ByReferenceResolveResult(element.GetResolveResult(), pending.ReferenceKind);
		}

		/// <summary>The target resolve result of a dynamic operation: its own, else the built target's.</summary>
		static ResolveResult DynamicTarget(RustDynamicResult dynamicResult, Expression? builtTarget)
			=> dynamicResult.Target is null || ReferenceEquals(dynamicResult.Target, PendingTarget)
				? builtTarget?.GetResolveResult() ?? ErrorResolveResult.UnknownError
				: dynamicResult.Target;

		/// <summary>
		/// The <see cref="DynamicInvocationResolveResult"/> of a dynamic invocation or index access over
		/// these arguments (a named argument has no resolve result of its own).
		/// </summary>
		static void CompleteDynamicInvocation(RustBodyNode node, Expression? builtTarget, IEnumerable<Expression> arguments)
		{
			if (node.Dynamic is not { Tag: 71 } dynamicResult)
				return;
			node.ResolveResult = new DynamicInvocationResolveResult(DynamicTarget(dynamicResult, builtTarget),
				dynamicResult.Indexing ? DynamicInvocationType.Indexing : DynamicInvocationType.Invocation,
				arguments.Select(a => a.GetResolveResult()).ToArray(), symbol: dynamicResult.Symbol);
		}

		static AstNode Build(RustBodyNode node, ExpressionBuilder exprBuilder)
		{
			var syntax = BuildNode(node, exprBuilder);
			if (node.MethodGroup is (true, var extensionTypeArguments, var extensionMethod, true))
			{
				// `IsUnambiguousMethodReference` of an extension method: the member access on the
				// built target expression as an invocation target.
				syntax.AddAnnotation(exprBuilder.resolver.ResolveMemberAccess(((MemberReferenceExpression)syntax).Target.GetResolveResult(),
					extensionMethod.Name, extensionTypeArguments, NameLookupMode.InvocationTarget));
			}
			else if (node.MethodGroup is var (hasTarget, typeArguments, groupMethod, _))
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
					var expression = node.ConstantSyntax ?? ConstantFallback(exprBuilder.ConvertConstantValue(new ConstantResolveResult(node.Type!, node.Value), node.Flag).Expression);
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
					var expression = node.ConstantSyntax ?? ConstantFallback(exprBuilder.ConvertConstantValue(new ConstantResolveResult(node.Type!, node.Value),
						allowImplicitConversion: false, displayAsHex: node.Flag).Expression);
					foreach (var inst in node.IL)
						expression.AddAnnotation(inst);
					return expression;
				}
				case RustBodyNode.StringLiteral:
					return Annotate(new PrimitiveExpression(node.Value!), node);
				case RustBodyNode.Utf8StringLiteral:
					return Annotate(new PrimitiveExpression(node.Value!, LiteralFormat.Utf8Literal), node);
				case RustBodyNode.Error:
				{
					var error = new ErrorExpression();
					error.AddTrailingTrivia(new Comment((string)node.Value!, CommentType.MultiLine));
					node.ResolveResult = ErrorResolveResult.UnknownError;
					return Annotate(error, node);
				}
				case RustBodyNode.BlockExpression:
				{
					// `VisitBlockContainer`.
					var container = (BlockContainer)node.IL[0];
					var body = (BlockStatement)Build(node.Children[0], exprBuilder);
					var commentStatement = new EmptyStatement();
					commentStatement.AddTrailingTrivia(new Comment(" Could not convert BlockContainer to single expression"));
					body.Statements.InsertBefore(body.Statements.FirstOrDefault(), commentStatement);
					// set ILVariable.UsesInitialValue for any variables being used inside the container
					foreach (var stloc in container.Descendants.OfType<StLoc>())
						stloc.Variable.UsesInitialValue = true;
					var (delegateType, blockReturnType) = ((IType, IType))node.Value!;
					var invocationTarget = new CastExpression(ConvertType(exprBuilder, delegateType), new AnonymousMethodExpression { Body = body });
					node.ResolveResult = node.Flag
						? new ResolveResult(blockReturnType)
						: new CSharpInvocationResolveResult(new ResolveResult(delegateType), delegateType.GetDelegateInvokeMethod(), EmptyList<ResolveResult>.Instance);
					return Annotate(new InvocationExpression(new MemberReferenceExpression(invocationTarget, "Invoke")), node);
				}
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
					if (node.Dynamic is { Tag: 70 } dynamicMember)
						node.ResolveResult = new DynamicMemberResolveResult(DynamicTarget(dynamicMember, target), (string)node.Value!, dynamicMember.Symbol);
					return Annotate(new MemberReferenceExpression(target, (string)node.Value!), node);
				}
				case RustBodyNode.TypeReference:
					return Annotate(new TypeReferenceExpression(ConvertType(exprBuilder, node.Type!)), node);
				case RustBodyNode.UsingExpanded:
				{
					// `VisitUsingInstruction` where `IsValidInCSharp` fails.
					var variable = (ILVariable)node.Value!;
					// The store of the resource.
					if (node.ResolveOperator)
					{
						return Annotate(new ExpressionStatement(new AssignmentExpression(exprBuilder.ConvertVariable(variable).Expression,
							(Expression)Build(node.Children[0], exprBuilder))), node);
					}
					var function = variable.Function!;
					bool isAsync = node.Flag;
					var disposeType = exprBuilder.compilation.FindType(isAsync ? KnownTypeCode.IAsyncDisposable : KnownTypeCode.IDisposable);
					variable.Kind = VariableKind.Local;
					var disposeVariable = function.RegisterVariable(VariableKind.Local, disposeType,
						IL.Transforms.AssignVariableNames.GenerateVariableName(function, disposeType, exprBuilder.statementBuilder.decompileRun.UsingScope));
					Expression disposeInvocation = new InvocationExpression(new MemberReferenceExpression(
						exprBuilder.ConvertVariable(disposeVariable).Expression, isAsync ? "DisposeAsync" : "Dispose"));
					if (isAsync)
						disposeInvocation = new UnaryOperatorExpression { Expression = disposeInvocation, Operator = UnaryOperatorType.Await };
					var tryFinally = new TryCatchStatement {
						TryBlock = (BlockStatement)Build(node.Children[0], exprBuilder),
						FinallyBlock = new BlockStatement {
								new ExpressionStatement(new AssignmentExpression(exprBuilder.ConvertVariable(disposeVariable).Expression,
									new AsExpression(exprBuilder.ConvertVariable(variable).Expression, ConvertType(exprBuilder, disposeType)))),
								new IfElseStatement {
									Condition = new BinaryOperatorExpression(exprBuilder.ConvertVariable(disposeVariable), BinaryOperatorType.InEquality, new NullReferenceExpression()),
									TrueStatement = new ExpressionStatement(disposeInvocation)
								}
							}
					};
					return Annotate(tryFinally, node);
				}
				case RustBodyNode.ParenthesizedDesignation:
				{
					var designations = new ParenthesizedVariableDesignation();
					designations.VariableDesignations.AddRange(node.Children.Select(c => (VariableDesignation)Build(c, exprBuilder)));
					return Annotate(designations, node);
				}
				case RustBodyNode.SingleDesignation:
				{
					var single = new SingleVariableDesignation { Identifier = node.Value is ILVariable v ? v.Name! : "_" };
					if (node.Value is ILVariable designated)
						single.AddAnnotation(new ILVariableResolveResult(designated));
					return single;
				}
				case RustBodyNode.InDirection:
				{
					var element = (Expression)Build(node.Children[0], exprBuilder);
					CompletePendingElement(node, element);
					return Annotate(new DirectionExpression(FieldDirection.In, element), node);
				}
				case RustBodyNode.RefDirection:
				{
					var element = (Expression)Build(node.Children[0], exprBuilder);
					CompletePendingElement(node, element);
					return Annotate(new DirectionExpression(FieldDirection.Ref, element), node);
				}
				case RustBodyNode.ExpressionStatement:
					return Annotate(new ExpressionStatement((Expression)Build(node.Children[0], exprBuilder)), node);
				case RustBodyNode.Assignment:
				{
					var left = (Expression)Build(node.Children[0], exprBuilder);
					var right = (Expression)Build(node.Children[1], exprBuilder);
					// `VisitDeconstructInstruction` replaced the loads of the init temporaries in the
					// assignment targets with the inits' values, which the engine translated.
					if (node.IL is [DeconstructInstruction deconstruction])
						ReplaceAssignmentTargets(deconstruction);
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
					// A dynamic member invocation's target is the member reference's target; a
					// simple name's comes with the invocation.
					CompleteDynamicInvocation(node, node.Dynamic?.Symbol != null ? (target as MemberReferenceExpression)?.Target : target, arguments);
					// A function pointer call comes with a plain resolve result of its own.
					if (node.Call == null)
						return Annotate(new InvocationExpression(target, arguments), node);
					var (targetResult, method, isExtension, isDelegate, foundByLookup, isExpanded) = node.Call.Value;
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
					// `VisitDynamicInvokeConstructorInstruction`: the synthesized constructor over the arguments.
					if (node.Dynamic is { Tag: 72 } dynamicConstructor)
					{
						node.ResolveResult = new CSharpInvocationResolveResult(new ResolveResult(dynamicConstructor.Type!),
							(IMethod)dynamicConstructor.Symbol!, arguments.Select(a => a.GetResolveResult()).ToArray());
					}
					// A creation of `default(T)` or `Activator.CreateInstance<T>()` keeps its type's resolve result.
					if (node.Call is { } call)
					{
						// `HandleConstructorCall`: where native integers need no attribute, a created IntPtr is nint.
						IType? returnTypeOverride = null;
						if (exprBuilder.compilation.MainModule is MetadataModule { TypeSystemOptions: var options }
							&& options.HasFlag(TypeSystemOptions.NativeIntegersWithoutAttribute))
						{
							if (call.Method.DeclaringType.IsKnownType(KnownTypeCode.IntPtr))
								returnTypeOverride = SpecialType.NInt;
							else if (call.Method.DeclaringType.IsKnownType(KnownTypeCode.UIntPtr))
								returnTypeOverride = SpecialType.NUInt;
						}
						node.ResolveResult = new CSharpInvocationResolveResult(null!, call.Method, argumentResults,
							isExpandedForm: call.IsExpanded, returnTypeOverride: returnTypeOverride);
					}
					var creation = new ObjectCreateExpression(ConvertType(exprBuilder, node.Type!), arguments);
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
					if (node.Dynamic is { Tag: 74 } dynamicOperator)
					{
						// `VisitDynamicCompoundAssign`: the binder's operation over the target's and value's resolve results.
						node.ResolveResult = new OperatorResolveResult(SpecialType.Dynamic, (System.Linq.Expressions.ExpressionType)dynamicOperator.Operation,
							[left.GetResolveResult(), right.GetResolveResult()]);
					}
					else if (node.ExplicitOperator is var (userOperator, isLifted, operandOverflow))
					{
						// `VisitUserDefinedCompoundAssign`: the operator over the target's and value's resolve results.
						node.ResolveResult = new OperatorResolveResult(node.OperatorType!, AssignmentExpression.GetLinqNodeType(op, false),
							userOperator, isLifted, [left.GetResolveResult(), right.GetResolveResult()]);
						AddOverflowAnnotation(left, operandOverflow);
					}
					else if (node.OperatorType != null)
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
				case RustBodyNode.UnaryOperator:
				{
					var op = (UnaryOperatorType)node.Value!;
					var operand = (Expression)Build(node.Children[0], exprBuilder);
					if (node.ExplicitOperator is var (userOperator, isLifted, operandOverflow))
					{
						node.ResolveResult = new OperatorResolveResult(node.OperatorType!, UnaryOperatorExpression.GetLinqNodeType(op, false),
							userOperator, isLifted, [operand.GetResolveResult()]);
						AddOverflowAnnotation(operand, operandOverflow);
					}
					// A dynamic operator keeps the plain resolve result it comes with.
					else if (node.OperatorType != null)
					{
						node.ResolveResult = new OperatorResolveResult(node.OperatorType, UnaryOperatorExpression.GetLinqNodeType(op, node.Overflow == 1),
							operand.GetResolveResult());
					}
					return AddOverflowAnnotation(Annotate(new UnaryOperatorExpression(op, operand), node), node);
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
					// `VisitCkfinite`'s `!` comes without a resolve result.
					if (node.OperatorType != null)
						node.ResolveResult = new OperatorResolveResult(node.OperatorType, System.Linq.Expressions.ExpressionType.Not, operand.GetResolveResult());
					return Annotate(new UnaryOperatorExpression(UnaryOperatorType.Not, operand), node);
				}
				case RustBodyNode.BinaryOperator:
				{
					var op = (BinaryOperatorType)node.Value!;
					var left = (Expression)Build(node.Children[0], exprBuilder);
					var right = (Expression)Build(node.Children[1], exprBuilder);
					// A logic operator comes with a plain resolve result of its own.
					if (node.ExplicitOperator is var (operatorMethod, isLifted, _))
					{
						// `HandleThreeValuedLogic`: the lifted operator over the operands' resolve results.
						node.ResolveResult = new OperatorResolveResult(node.OperatorType!, BinaryOperatorExpression.GetLinqNodeType(op, false),
							operatorMethod, isLifted, [left.GetResolveResult(), right.GetResolveResult()]);
					}
					else if (node.OperatorType != null)
					{
						node.ResolveResult = new OperatorResolveResult(node.OperatorType, BinaryOperatorExpression.GetLinqNodeType(op, node.Overflow == 1),
							left.GetResolveResult(), right.GetResolveResult());
					}
					// `VisitUserDefinedLogicOperator`: the operator's invocation over the operands' resolve results.
					if (node.OperatorInvocation is { } invokedOperator)
						node.ResolveResult = new InvocationResolveResult(null, invokedOperator, [left.GetResolveResult(), right.GetResolveResult()]);
					if (node.ResolveOperator)
						node.ResolveResult = exprBuilder.resolver.ResolveBinaryOperator(op, left.GetResolveResult(), right.GetResolveResult());
					// A call written as an operator (`b == true` of `GetValueOrDefault`, `==` of
					// `Delegate.op_Equality`) keeps its resolve result; the operands are the arguments
					// of a call with two parameters.
					if (node.Call is var (callTarget, callMethod, _, _, _, isExpanded))
					{
						if (ReferenceEquals(callTarget, PendingTarget))
							callTarget = left.GetResolveResult();
						ResolveResult[] arguments = callMethod.Parameters.Count == 2 ? [left.GetResolveResult(), right.GetResolveResult()] : [];
						node.ResolveResult = new CSharpInvocationResolveResult(callTarget!, callMethod, arguments, isExpandedForm: isExpanded);
					}
					// A constant operation that overflows in a checked context is explicitly unchecked.
					return AddOverflowAnnotation(Annotate(new BinaryOperatorExpression(left, op, right), node), node);
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
						clause.Type = ConvertType(exprBuilder, node.Type);
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
				{
					var target = (Expression)Build(node.Children[0], exprBuilder);
					var indices = node.Children.Skip(1).Select(c => (Expression)Build(c, exprBuilder)).ToList();
					// A slice's member resolve result takes a cast or call target's from the built target.
					if (node.ResolveResult is MemberResolveResult pending && ReferenceEquals(pending.TargetResult, PendingTarget))
						node.ResolveResult = new MemberResolveResult(target.GetResolveResult(), pending.Member);
					CompleteDynamicInvocation(node, target, indices);
					return Annotate(new IndexerExpression(target, indices), node);
				}
				case RustBodyNode.BoolLiteral:
					return new PrimitiveExpression(node.Flag);
				case RustBodyNode.InitializerIndexer:
					return Annotate(new IndexerExpression(null, node.Children.Select(c => (Expression)Build(c, exprBuilder))), node);
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
				case RustBodyNode.AsRefReadOnlyHelper:
					return AsRefReadOnlyHelper(exprBuilder.settings);
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
				case RustBodyNode.GotoCase:
					return Annotate(new GotoCaseStatement { LabelExpression = (Expression)Build(node.Children[0], exprBuilder) }, node);
				case RustBodyNode.GotoDefault:
					return Annotate(new GotoDefaultStatement(), node);
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
					return Annotate(new ObjectCreateExpression(ConvertType(exprBuilder, node.Type!), reference), node);
				}
				case RustBodyNode.Foreach:
				{
					// `TransformToForeach`: the variable as the ILAst names and types it now.
					var variable = (ILVariable)node.Value!;
					// A deconstructing foreach declares its targets with `var`.
					VariableDesignation designation;
					bool useVar = node.Children.Length == 3;
					if (useVar)
					{
						designation = (VariableDesignation)Build(node.Children[2], exprBuilder);
					}
					else
					{
						designation = new SingleVariableDesignation { Identifier = variable.Name! };
						designation.AddAnnotation(new ILVariableResolveResult(variable, variable.Type));
						// `var` for a variable of an anonymous type, and for one of a tuple type,
						// which the engine only translates where it is the type `var` infers.
						useVar = exprBuilder.settings.AnonymousTypes && variable.Type.ContainsAnonymousType()
							|| TupleType.IsTupleCompatible(variable.Type, out _);
					}
					var statement = new ForeachStatement {
						IsAsync = node.Flag,
						VariableType = useVar ? new SimpleType("var") : ConvertType(exprBuilder, variable.Type),
						VariableDesignation = designation,
						InExpression = (Expression)Build(node.Children[0], exprBuilder),
						EmbeddedStatement = (Statement)Build(node.Children[1], exprBuilder)
					};
					var i = node.ForeachInstructions;
					statement.AddAnnotation(new ForeachAnnotation(i[0], i[1], i[2]));
					return Annotate(statement, node);
				}
				case RustBodyNode.DefaultValue:
					return Annotate(node.Type == null ? new DefaultValueExpression() : new DefaultValueExpression(ConvertType(exprBuilder, node.Type)), node);
				case RustBodyNode.IsType:
				{
					var tested = (Expression)Build(node.Children[0], exprBuilder);
					node.ResolveResult = new TypeIsResolveResult(tested.GetResolveResult(), node.Type!, exprBuilder.compilation.FindType(TypeCode.Boolean));
					return Annotate(new IsExpression(tested, ConvertType(exprBuilder, node.Type!.TupleUnderlyingTypeOrSelf())), node);
				}
				case RustBodyNode.IsPattern:
					return Annotate(new BinaryOperatorExpression((Expression)Build(node.Children[0], exprBuilder), BinaryOperatorType.IsPattern,
						(Expression)Build(node.Children[1], exprBuilder)), node);
				case RustBodyNode.DeclarationPattern:
				{
					var variable = (ILVariable)node.Value!;
					return Annotate(new DeclarationExpression {
						Type = node.Flag ? new SimpleType("var") : ConvertType(exprBuilder, variable.Type),
						Designation = Designation(variable)
					}, node);
				}
				case RustBodyNode.RecursivePattern:
				{
					var pattern = new RecursivePatternExpression();
					if (node.Type != null)
						pattern.Type = ConvertType(exprBuilder, node.Type);
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
				{
					var elements = node.Children.Select(c => (Expression)Build(c, exprBuilder)).ToList();
					// A collection initializer's `Add` call with several arguments.
					if (node.Call is var (callTarget, callMethod, isExtension, _, _, isExpanded))
					{
						node.ResolveResult = new CSharpInvocationResolveResult(callTarget!, callMethod,
							elements.Select(e => e.GetResolveResult()).ToArray(), isExtensionMethodInvocation: isExtension, isExpandedForm: isExpanded);
					}
					return Annotate(new ArrayInitializerExpression(elements), node);
				}
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
						typeExpression = ConvertType(exprBuilder, elementType);
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
				case RustBodyNode.AnonymousTypeCreate:
				{
					// `HandleConstructorCall` of an anonymous type. shortcut: the arguments of the
					// resolve result are the written initializers' (after the cast to a property's
					// type where one was added), not the uncast arguments; send those if a consumer
					// ever tells them apart.
					var initializers = node.Children.Select(c => (Expression)Build(c, exprBuilder)).ToList();
					if (node.Call is var (_, callMethod, _, _, _, isExpanded))
					{
						node.ResolveResult = new CSharpInvocationResolveResult(null!, callMethod,
							initializers.Select(e => (e is NamedExpression named ? named.Expression : e).GetResolveResult()).ToArray(),
							isExpandedForm: isExpanded);
					}
					var creation = new AnonymousTypeCreateExpression();
					creation.Initializers.AddRange(initializers);
					return Annotate(creation, node);
				}
				case RustBodyNode.WithInitializer:
				{
					// `TranslateWithInitializer`: the value, then the initializer's elements.
					var with = new WithInitializerExpression {
						Expression = (Expression)Build(node.Children[0], exprBuilder),
						Initializer = new ArrayInitializerExpression(node.Children.Skip(1).Select(c => (Expression)Build(c, exprBuilder))),
					};
					return Annotate(with, node);
				}
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
					if (node.Children.Length == 0)
					{
						method.Modifiers |= Modifiers.Extern;
					}
					else
					{
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
					// `TranslateFunction`: the engine decides the syntax, the parameter declarations are
					// `MakeParameters`.
					var function = (ILFunction)node.IL[0];
					var parameters = node.HasParameterList ? MakeParameters(function, exprBuilder) : [];
					foreach (var pd in parameters)
					{
						if (node.ImplicitlyTyped)
							pd.Type = null!;
						if (!node.KeepParamsAndDefaults)
						{
							pd.IsParams = false;
							pd.DefaultExpression?.Detach();
						}
					}
					node.ResolveResult = new DecompiledLambdaResolveResult(function, node.Type!, (IType)node.Value!,
						hasParameterList: node.HasParameterList, isAnonymousMethod: node.AnonymousMethod, isImplicitlyTyped: node.ImplicitlyTyped && node.AnonymousMethod) {
						AttemptedConversionWithTypeMismatch = node.AttemptedConversionWithTypeMismatch
					};
					var lambdaBody = Build(node.Children[0], exprBuilder);
					if (lambdaBody is BlockStatement block)
					{
						var warningAnchor = block.Statements.FirstOrDefault();
						foreach (string warning in function.Warnings)
						{
							var warningStatement = new EmptyStatement();
							warningStatement.AddTrailingTrivia(new Comment(warning));
							if (warningAnchor != null)
								block.Statements.InsertBefore(warningAnchor, warningStatement);
							else
								block.Statements.Add(warningStatement);
						}
					}
					if (node.AnonymousMethod)
					{
						var anonymous = new AnonymousMethodExpression { IsAsync = node.IsAsync, Body = (BlockStatement)lambdaBody };
						anonymous.Parameters.AddRange(parameters);
						return Annotate(anonymous, node);
					}
					var lambda = new LambdaExpression { IsAsync = node.IsAsync };
					var method = function.Method?.MemberDefinition as IMethod;
					foreach (var attr in method?.GetAttributes() ?? [])
					{
						if (attr.AttributeType.IsKnownType(KnownAttribute.CompilerGenerated))
							continue;
						if (function.IsAsync && (attr.AttributeType.IsKnownType(KnownAttribute.AsyncStateMachine)
							|| attr.AttributeType.IsKnownType(KnownAttribute.DebuggerStepThrough)))
						{
							continue;
						}
						lambda.Attributes.Add(new AttributeSection(exprBuilder.astBuilder.ConvertAttribute(attr)));
					}
					foreach (var attr in method?.GetReturnTypeAttributes() ?? [])
						lambda.Attributes.Add(new AttributeSection(exprBuilder.astBuilder.ConvertAttribute(attr)) { AttributeTarget = "return" });
					lambda.Parameters.AddRange(parameters);
					lambda.Body = lambdaBody;
					return Annotate(lambda, node);
				}
				case RustBodyNode.Using:
				{
					AstNode resourceAcquisition = Build(node.Children[0], exprBuilder);
					if (node.Value is ILVariable variable)
					{
						var declaration = new VariableDeclarationStatement(ConvertType(exprBuilder, variable.Type), variable.Name!, (Expression)resourceAcquisition);
						declaration.Variables.Single().AddAnnotation(new ILVariableResolveResult(variable, variable.Type));
						resourceAcquisition = declaration;
					}
					return Annotate(new UsingStatement {
						ResourceAcquisition = resourceAcquisition,
						IsAsync = node.Flag,
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
					var element = (Expression)Build(node.Children[0], exprBuilder);
					CompletePendingElement(node, element);
					var direction = Annotate(new DirectionExpression(FieldDirection.Out, element), node);
					// `GetArgumentExpressions` marks an `out` argument written as `out var`.
					if (node.Flag)
						direction.AddAnnotation(UseImplicitlyTypedOutAnnotation.Instance);
					return direction;
				}
				case RustBodyNode.TypeOf:
					return Annotate(new TypeOfExpression(ConvertType(exprBuilder, node.Type!)), node);
				case RustBodyNode.As:
				{
					var operand = (Expression)Build(node.Children[0], exprBuilder);
					node.ResolveResult = new ConversionResolveResult(node.Type!, operand.GetResolveResult(), Conversion.TryCast);
					return Annotate(new AsExpression(operand, ConvertType(exprBuilder, node.Type!)), node);
				}
				case RustBodyNode.Cast:
				{
					var operand = (Expression)Build(node.Children[0], exprBuilder);
					var cast = new CastExpression(ConvertType(exprBuilder, node.Type!), operand);
					// `TranslatedExpression.ConvertTo` marks a cast to an integer type checked or unchecked.
					AddOverflowAnnotation(cast, node);
					if (node.LambdaConversion)
						node.ResolveResult = new ConversionResolveResult(node.Type!, operand.GetResolveResult(), Semantics.LambdaConversion.Instance);
					if (node.InlineArrayConversion)
						node.ResolveResult = new ConversionResolveResult(node.Type!, operand.GetResolveResult(), Conversion.InlineArrayConversion);
					if (node.MethodGroupConversion != null)
					{
						node.ResolveResult = new ConversionResolveResult(node.Type!, operand.GetResolveResult(),
							Conversion.MethodGroupConversion(node.MethodGroupConversion, isVirtualMethodLookup: false, delegateCapturesFirstArgument: false));
					}
					if (node.ResolveCast)
						node.ResolveResult = exprBuilder.resolver.WithCheckForOverflow(node.Overflow == 1).ResolveCast(node.Type!, operand.GetResolveResult());
					// `HandleImplicitConversion`: the implicit conversion from the operand's type.
					if (node.ImplicitConversion)
					{
						var operandResult = operand.GetResolveResult();
						var conversion = CSharp.Resolver.CSharpConversions.Get(exprBuilder.compilation).ImplicitConversion(operandResult.Type, node.Type!);
						node.ResolveResult = new ConversionResolveResult(node.Type!, operandResult, conversion);
					}
					// `VisitDynamicConvertInstruction`: the dynamic conversion of the binder's flags.
					if (node.Dynamic is { Tag: 73 } dynamicConversion)
					{
						node.ResolveResult = new ConversionResolveResult(node.Type!, operand.GetResolveResult(),
							dynamicConversion.Explicit ? Conversion.ExplicitDynamicConversion : Conversion.ImplicitDynamicConversion);
					}
					return Annotate(cast, node);
				}
				case RustBodyNode.GenericIdentifier:
					return Annotate(new IdentifierExpression((string)node.Value!,
						node.TypeArguments.Select(t => ConvertType(exprBuilder, t))), node);
				case RustBodyNode.GenericMemberReference:
					return Annotate(new MemberReferenceExpression((Expression)Build(node.Children[0], exprBuilder), (string)node.Value!,
						node.TypeArguments.Select(t => ConvertType(exprBuilder, t))), node);
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
				case RustBodyNode.PointerUnary:
					return Annotate(new UnaryOperatorExpression(node.Flag ? UnaryOperatorType.AddressOf : UnaryOperatorType.Dereference,
						(Expression)Build(node.Children[0], exprBuilder)), node);
				case RustBodyNode.SizeOf:
					node.ResolveResult = new SizeOfResolveResult(exprBuilder.compilation.FindType(KnownTypeCode.Int32), node.Type!, null);
					return Annotate(new SizeOfExpression(ConvertType(exprBuilder, node.Type!)), node);
				case RustBodyNode.Fixed:
				{
					var variable = (ILVariable)node.Value!;
					var statement = new FixedStatement { Type = ConvertType(exprBuilder, variable.Type) };
					statement.Variables.Add(new VariableInitializer(variable.Name!, (Expression)Build(node.Children[0], exprBuilder)).WithILVariable(variable));
					statement.EmbeddedStatement = (Statement)Build(node.Children[1], exprBuilder);
					return Annotate(statement, node);
				}
				case RustBodyNode.UnsafeIntrinsic:
				{
					var target = new MemberReferenceExpression {
						// `CallUnsafeIntrinsic`: the compilation's Unsafe, which is an unknown (fully qualified) type
						// where the module does not reference it.
						Target = new TypeReferenceExpression(UnsafeTypeSyntax(exprBuilder, node.Type!)),
						MemberName = (string)node.Value!
					};
					target.TypeArguments.AddRange(node.TypeArguments.Select(t => ConvertType(exprBuilder, t)));
					return Annotate(new InvocationExpression(target, node.Children.Select(c => (Expression)Build(c, exprBuilder))), node);
				}
				case RustBodyNode.StackAlloc:
				{
					var stackAlloc = new StackAllocExpression { Type = ConvertType(exprBuilder, node.Type!) };
					int first = 0;
					if (node.Flag)
						stackAlloc.CountExpression = (Expression)Build(node.Children[first++], exprBuilder);
					if ((bool)node.Value!)
					{
						var initializer = stackAlloc.Initializer = new ArrayInitializerExpression();
						for (int i = first; i < node.Children.Length; i++)
							initializer.Elements.Add((Expression)Build(node.Children[i], exprBuilder));
					}
					return Annotate(stackAlloc, node);
				}
				case RustBodyNode.Undocumented:
				{
					var undocumented = new UndocumentedExpression { UndocumentedExpressionType = (UndocumentedExpressionType)node.Value! };
					undocumented.Arguments.AddRange(node.Children.Select(c => (Expression)Build(c, exprBuilder)));
					return Annotate(undocumented, node);
				}
				case RustBodyNode.Range:
				{
					var start = node.Children[0] is { } s ? (Expression)Build(s, exprBuilder) : null;
					var end = node.Children[1] is { } e ? (Expression)Build(e, exprBuilder) : null;
					return Annotate(new BinaryOperatorExpression(start!, BinaryOperatorType.Range, end!), node);
				}
				case RustBodyNode.LeadingComment:
				{
					var commented = Build(node.Children[0], exprBuilder);
					commented.AddLeadingTrivia((Comment)node.Value!);
					return commented;
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

		/// <summary>`ConstructAssignmentTarget`'s `ReplaceAssignmentTarget` over the deconstruction's assignments.</summary>
		static void ReplaceAssignmentTargets(DeconstructInstruction inst)
		{
			var inits = inst.Init;
			int initPos = 0;
			foreach (var assignment in inst.Assignments.Instructions.ToArray())
			{
				switch (assignment)
				{
					case CallInstruction call:
						for (int i = 0; i < call.Arguments.Count - 1; i++)
							ReplaceAssignmentTarget(call.Arguments[i]);
						break;
					case StObj stobj:
						var target = stobj.Target;
						while (target.MatchLdFlda(out var nestedTarget, out _))
							target = nestedTarget;
						ReplaceAssignmentTarget(target);
						break;
				}
			}

			void ReplaceAssignmentTarget(ILInstruction target)
			{
				if (target.MatchLdLoc(out var v) && v.Kind == VariableKind.DeconstructionInitTemporary)
				{
					target.ReplaceWith(inits[initPos].Value);
					initPos++;
				}
			}
		}

		/// <summary>The <c>ILSpyHelper_AsRefReadOnly</c> local function <c>ConvertBlockContainer</c> declares
		/// where <c>EmitAsRefReadOnly</c> is set.</summary>
		static LocalFunctionDeclarationStatement AsRefReadOnlyHelper(DecompilerSettings settings)
		{
			var methodDecl = new MethodDeclaration();
			if (settings.StaticLocalFunctions)
			{
				methodDecl.Modifiers = Modifiers.Static;
			}

			methodDecl.ReturnType = new ComposedType() { HasReadOnlySpecifier = true, HasRefSpecifier = true, BaseType = new SimpleType("T") };
			methodDecl.Name = "ILSpyHelper_AsRefReadOnly";
			methodDecl.TypeParameters.Add(new TypeParameterDeclaration("T"));
			methodDecl.Parameters.Add(new ParameterDeclaration { ParameterModifier = ReferenceKind.In, Type = new SimpleType("T"), Name = "temp" });

			methodDecl.Body = new BlockStatement();
			var commentStatement = new EmptyStatement();
			commentStatement.AddTrailingTrivia(new Comment(
				"ILSpy generated this function to help ensure overload resolution can pick the overload using 'in'"));
			methodDecl.Body.Add(commentStatement);
			methodDecl.Body.Add(new ReturnStatement(new DirectionExpression(FieldDirection.Ref, new IdentifierExpression("temp"))));
			return new LocalFunctionDeclarationStatement(methodDecl);
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
