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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>A <c>CanTransformToExtensionMethodCall</c> question the engine asks.</summary>
	internal sealed class RustResolverQuery
	{
		/// <summary><c>NullPropagationTransform.CanTransformToExtensionMethodCall(call)</c>.</summary>
		public const byte CallKind = 0;
		/// <summary><c>CanTransformToExtensionMethodCall(method, ignoreTypeArguments: true)</c>.</summary>
		public const byte MethodKind = 1;
		/// <summary><c>Disambiguator.ProbeField</c> for the unqualified field name.</summary>
		public const byte FieldSimpleNameKind = 2;
		/// <summary><c>Disambiguator.ProbeField</c> for <c>target.field</c>.</summary>
		public const byte FieldMemberAccessKind = 3;
		/// <summary>The branch <c>ExpressionBuilder.ConvertField</c> takes for the field.</summary>
		public const byte FieldKindKind = 4;

		/// <summary><see cref="TargetKind"/>: <c>this</c> or <c>base</c>.</summary>
		public const byte ThisTarget = 0;
		/// <summary><see cref="TargetKind"/>: a type name.</summary>
		public const byte TypeTarget = 1;

		public byte Kind;
		public IMethod Method = null!;
		public IField Field = null!;
		/// <summary>For <see cref="FieldMemberAccessKind"/>: what the target is (0 this or base, 1 a type, 2 another expression).</summary>
		public byte TargetKind;
		public bool CausesNonVirtualInvocation;
		/// <summary>For <see cref="FieldKindKind"/>: whether the field's target is the <c>this</c> instance or absent.</summary>
		public bool TargetIsThis;
		public IType? Target;
		/// <summary>The target variable's type and its single store's value type.</summary>
		public (IType Variable, IType Value)? TargetStore;
		public IType[] Arguments = [];
	}

	/// <summary>
	/// What the questions of the engine's C# body translation are answered from: the resolver the
	/// statement builder's ExpressionBuilder creates, and the context it decompiles the body in.
	/// </summary>
	internal sealed class RustBodyContext
	{
		public RustBodyContext(CSharpResolver resolver, ITypeResolveContext decompilationContext,
			DecompileRun decompileRun, IDecompilerTypeSystem typeSystem)
		{
			Resolver = resolver;
			DecompilationContext = decompilationContext;
			DecompileRun = decompileRun;
			TypeSystem = typeSystem;
		}

		public CSharpResolver Resolver { get; }
		public ITypeResolveContext DecompilationContext { get; }
		public DecompileRun DecompileRun { get; }
		public IDecompilerTypeSystem TypeSystem { get; }
	}

	/// <summary>
	/// Serves a <see cref="CSharpResolver"/> to the engine through <c>ResolverCallback</c>
	/// (rust/ilspy-decompiler-ffi/src/lib.rs) while it reads the main module's MethodDef
	/// <c>row</c>. An exception cannot cross the native frames: it is captured and rethrown by
	/// <see cref="ThrowIfFailed"/> once the engine call has returned.
	/// </summary>
	internal sealed class RustResolver
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Callback(IntPtr context, byte* query, UIntPtr length);

		readonly CSharpResolver resolver;
		/// <summary>What the C# body translation's questions are answered from.</summary>
		readonly RustBodyContext? body;
		readonly int row;
		readonly ICompilation compilation;
		readonly IReadOnlyList<MetadataModule> modules;
		readonly DecompilerSettings settings;
		ExceptionDispatchInfo? failure;

		/// <summary>The delegate to pass to the engine; keep this object alive for the call.</summary>
		public Callback Delegate { get; }

		public unsafe RustResolver(CSharpResolver resolver, int row, ICompilation compilation,
			IReadOnlyList<MetadataModule> modules, DecompilerSettings settings, RustBodyContext? body = null)
		{
			this.resolver = resolver;
			this.body = body;
			this.row = row;
			this.compilation = compilation;
			this.modules = modules;
			this.settings = settings;
			Delegate = Answer;
		}

		unsafe int Answer(IntPtr context, byte* query, UIntPtr length)
		{
			try
			{
				var wire = new ReadOnlySpan<byte>(query, checked((int)length)).ToArray();
				var q = IlAstMaterializer.ReadResolverQuery(wire, row, compilation, modules, settings);
				if (q.Kind == RustResolverQuery.FieldKindKind)
					return ClassifyField(q.Field, q.TargetIsThis);
				return Answer(q) ? 1 : 0;
			}
			catch (Exception ex)
			{
				failure ??= ExceptionDispatchInfo.Capture(ex);
				return -1;
			}
		}

		bool Answer(RustResolverQuery query)
		{
			if (query.Kind is RustResolverQuery.FieldSimpleNameKind or RustResolverQuery.FieldMemberAccessKind)
				return FieldReferenceBinds(query);
			if (query.Kind == RustResolverQuery.MethodKind)
				return resolver.CanTransformToExtensionMethodCall(query.Method, ignoreTypeArguments: true);
			if (query.Kind != RustResolverQuery.CallKind)
				throw new InvalidOperationException($"Unknown resolver query kind {query.Kind}");
			if (!CanTransform(query.Target!))
				return false;
			if (query.TargetStore is var (variable, value))
				return variable.Equals(value) && CanTransform(value);
			return true;

			bool CanTransform(IType target) => resolver.CanTransformToExtensionMethodCall(query.Method, Array.Empty<IType>(),
				new ResolveResult(target), query.Arguments.Select(a => new ResolveResult(a)).ToArray(), argumentNames: null);
		}

		/// <summary>
		/// <c>Disambiguator.ProbeField</c>: whether the field name, unqualified or looked up on the
		/// target, resolves back to the field.
		/// </summary>
		bool FieldReferenceBinds(RustResolverQuery query)
		{
			var resolver = (body ?? throw new InvalidOperationException("The engine asked about a C# body without the body's context")).Resolver;
			MemberResolveResult? result;
			if (query.Kind == RustResolverQuery.FieldSimpleNameKind)
			{
				result = resolver.ResolveSimpleName(query.Field.Name, EmptyList<IType>.Instance, isInvocationTarget: false) as MemberResolveResult;
			}
			else
			{
				var type = query.Target!;
				ResolveResult target = query.TargetKind switch {
					// The lookup only asks whether the target is a ThisResolveResult.
					RustResolverQuery.ThisTarget => new ThisResolveResult(new ILVariable(VariableKind.Parameter, type, -1), type, query.CausesNonVirtualInvocation),
					RustResolverQuery.TypeTarget => new TypeResolveResult(type),
					_ => new ResolveResult(type),
				};
				var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule);
				result = lookup.Lookup(target, query.Field.Name, EmptyList<IType>.Instance, isInvocation: false) as MemberResolveResult;
			}
			return result != null && !result.IsError && result.Member.Equals(query.Field, NormalizeTypeVisitor.TypeErasure);
		}

		/// <summary>
		/// The branch <c>ExpressionBuilder.ConvertField</c> takes before it builds an ordinary field
		/// reference, as the engine's <c>FieldKind</c> answer: 0 an ordinary field, 1 the C# 14
		/// <c>field</c> keyword, 3 a field named "field" in a property accessor,
		/// <c>0x2000_0000 | row</c> the backing field of the automatic property at Property row,
		/// <c>0x1000_0000 | row</c> that of the automatic event at Event row.
		/// </summary>
		int ClassifyField(IField field, bool targetIsThis)
		{
			var context = body ?? throw new InvalidOperationException("The engine asked about a C# body without the body's context");
			var currentMember = context.DecompilationContext.CurrentMember;
			if (settings.AutomaticEvents && IsBackingFieldOfAutomaticEvent(field, out var ev))
				return 0x1000_0000 | MetadataTokens.GetRowNumber(ev.MetadataToken);
			if (CanUseFieldKeyword())
				return 1;
			if (settings.AutomaticProperties
				&& PatternStatementTransform.IsBackingFieldOfAutomaticProperty(field, out var property)
				&& !PatternStatementTransform.HasAccessedThroughPropertyAttribute(field)
				&& currentMember != property
				&& (property.CanSet || settings.GetterOnlyAutomaticProperties))
			{
				return 0x2000_0000 | MetadataTokens.GetRowNumber(property.MetadataToken);
			}
			if (settings.FieldKeyword && field.Name == "field" && currentMember is IProperty { Parameters.Count: 0 })
				return 3;
			return 0;

			bool CanUseFieldKeyword()
			{
				if (!settings.FieldKeyword)
					return false;
				if (currentMember is not IProperty prop || prop.Parameters.Count != 0)
					return false;
				if (!prop.CanSet && !settings.GetterOnlyAutomaticProperties)
					return false;
				if (!PatternStatementTransform.TryGetBackingField(prop, out var backingField)
					|| PatternStatementTransform.HasAccessedThroughPropertyAttribute(backingField)
					|| !field.MemberDefinition.Equals(backingField.MemberDefinition))
				{
					return false;
				}
				return field.IsStatic || targetIsThis;
			}

			bool IsBackingFieldOfAutomaticEvent(IField f, [NotNullWhen(true)] out IEvent? e)
			{
				e = null;
				if (f.MetadataToken.IsNil || f.ParentModule is not MetadataModule module)
					return false;
				if (!module.MetadataFile.PropertyAndEventBackingFieldLookup.IsEventBackingField((FieldDefinitionHandle)f.MetadataToken, out var eventHandle))
					return false;
				e = module.GetDefinition(eventHandle);
				if (currentMember is IMethod { AccessorOwner: IEvent owner } && owner.Equals(e))
				{
					e = null;
					return false;
				}
				if (!AutoEventDecompiler.IsAutomaticEvent(context.TypeSystem, e, context.DecompileRun, CancellationToken.None, out var backingField)
					|| !backingField.Equals(f.MemberDefinition))
				{
					e = null;
					return false;
				}
				return true;
			}
		}

		/// <summary>Rethrows the first exception the resolver threw during the engine call.</summary>
		public void ThrowIfFailed() => failure?.Throw();
	}
}
