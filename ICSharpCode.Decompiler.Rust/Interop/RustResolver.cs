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
		/// <summary><c>Disambiguator.LookUpName</c> for an invocation: the method group of <see cref="Name"/>.</summary>
		public const byte MethodGroupKind = 5;
		/// <summary><c>Disambiguator.IsUnambiguousAccess</c> for the property of the accessor <see cref="Method"/>.</summary>
		public const byte AccessorKind = 6;
		/// <summary>The accessible constructors of the declaring type of <see cref="Method"/>.</summary>
		public const byte ConstructorsKind = 7;
		/// <summary><c>CanTransformToExtensionMethodCall</c> with the body's resolver, for <c>TryUseExtensionMethodSyntax</c>.</summary>
		public const byte ExtensionSyntaxKind = 8;
		/// <summary><c>Vocabularies.Default.Singularize(<see cref="Name"/>)</c>, for foreach variable names.</summary>
		public const byte SingularizeKind = 9;
		/// <summary><c>MemberLookup.LookupIndexers</c> on the target, each indexer as its getter (or setter).</summary>
		public const byte IndexersKind = 10;
		/// <summary><c>Disambiguator.LookUpName</c> for a method reference: the method group of <see cref="Name"/>, not as an invocation target.</summary>
		public const byte MethodReferenceKind = 11;
		/// <summary><c>ExpressionBuilder.ParameterTypesAreAccessible</c> for the types in <see cref="Arguments"/>.</summary>
		public const byte TypesAccessibleKind = 12;
		/// <summary><c>ResolveMemberAccess</c> of <see cref="Name"/> on the target as an invocation target: the method group with its extension methods.</summary>
		public const byte ExtensionMethodGroupKind = 100;

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
		/// <summary>For <see cref="MethodGroupKind"/>: the name, and whether <see cref="Target"/> is set.</summary>
		public string Name = "";
		public bool HasTarget;
		/// <summary>For <see cref="AccessorKind"/>: whether the accessor is called with <c>callvirt</c>.</summary>
		public bool CallVirt;
		public IType? Target;
		/// <summary>The target variable's type and its single store's value type.</summary>
		public (IType Variable, IType Value)? TargetStore;
		public IType[] Arguments = [];
		/// <summary>For <see cref="MethodGroupKind"/>: the type arguments written with the name.</summary>
		public IType[] TypeArguments = [];
		/// <summary>For <see cref="ExtensionSyntaxKind"/>: the target, the other arguments and their names.</summary>
		public ResolveResult? TargetResult;
		public ResolveResult[] ArgumentResults = [];
		public string?[]? ArgumentNames;
	}

	/// <summary>
	/// A method group argument of an extension method question, which the body's resolver looks
	/// up again as <c>Disambiguator.LookUpName</c> does without invocation, then narrows to the
	/// method reference's method.
	/// </summary>
	internal sealed class QueryMethodGroup : ResolveResult
	{
		readonly IMethod method;
		readonly byte targetKind;
		readonly bool nonVirtual;
		readonly IType? target;
		readonly IType[] typeArguments;
		readonly bool extension;

		public QueryMethodGroup(IMethod method, byte targetKind, bool nonVirtual, IType? target, IType[] typeArguments, bool extension)
			: base(SpecialType.UnknownType)
		{
			this.method = method;
			this.targetKind = targetKind;
			this.nonVirtual = nonVirtual;
			this.target = target;
			this.typeArguments = typeArguments;
			this.extension = extension;
		}

		/// <summary>The method group, or null where the name does not find one.</summary>
		public MethodGroupResolveResult? LookUp(CSharpResolver resolver)
		{
			if (extension)
			{
				// `IsUnambiguousMethodReference` with `isExtensionMethodReference`: the member
				// access as an invocation target, extension methods included, not narrowed.
				if (target == null)
					return null;
				return resolver.ResolveMemberAccess(TargetResult(), method.Name, typeArguments, NameLookupMode.InvocationTarget) as MethodGroupResolveResult;
			}
			var found = target == null
				? resolver.ResolveSimpleName(method.Name, typeArguments, isInvocationTarget: false)
				: new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule)
					.Lookup(TargetResult(), method.Name, typeArguments, isInvocation: false);
			return (found as MethodGroupResolveResult)?.WithChosenMethod(method);
		}

		ResolveResult TargetResult() => targetKind switch {
			1 => new ThisResolveResult(new ILVariable(VariableKind.Parameter, target!, -1), target!, nonVirtual),
			2 => new TypeResolveResult(target!),
			_ => new ResolveResult(target!),
		};
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
		internal unsafe delegate int Callback(IntPtr context, byte* query, UIntPtr length, byte* answer, UIntPtr capacity);

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

		unsafe int Answer(IntPtr context, byte* query, UIntPtr length, byte* answer, UIntPtr capacity)
		{
			try
			{
				var wire = new ReadOnlySpan<byte>(query, checked((int)length)).ToArray();
				var q = IlAstMaterializer.ReadResolverQuery(wire, row, compilation, modules, settings);
				if (q.Kind == RustResolverQuery.FieldKindKind)
					return ClassifyField(q.Field, q.TargetIsThis);
				if (q.Kind == RustResolverQuery.ExtensionSyntaxKind)
					return CanUseExtensionMethodSyntax(q);
				if (q.Kind == RustResolverQuery.AccessorKind)
					return AccessorReference(q);
				if (q.Kind == RustResolverQuery.TypesAccessibleKind)
					return TypesAccessible(q.Arguments) ? 1 : 0;
				if (q.Kind is RustResolverQuery.MethodGroupKind or RustResolverQuery.ConstructorsKind or RustResolverQuery.SingularizeKind or RustResolverQuery.IndexersKind or RustResolverQuery.MethodReferenceKind or RustResolverQuery.ExtensionMethodGroupKind)
				{
					// The full answer's length; the engine asks again when it does not fit.
					var data = q.Kind switch {
						RustResolverQuery.ExtensionMethodGroupKind => ExtensionMethodGroup(q),
						RustResolverQuery.ConstructorsKind => Constructors(q),
						RustResolverQuery.IndexersKind => Indexers(q),
						RustResolverQuery.SingularizeKind => System.Text.Encoding.UTF8.GetBytes(Humanizer.Inflections.Vocabularies.Default.Singularize(q.Name, inputIsKnownToBePlural: false) ?? ""),
						RustResolverQuery.MethodReferenceKind => MethodGroup(q, invocation: false),
						_ => MethodGroup(q),
					};
					data.AsSpan(0, Math.Min(data.Length, checked((int)capacity))).CopyTo(new Span<byte>(answer, checked((int)capacity)));
					return data.Length;
				}
				return Answer(q) ? 1 : 0;
			}
			catch (Exception ex)
			{
				failure ??= ExceptionDispatchInfo.Capture(ex);
				return -1;
			}
		}

		/// <summary>`ExpressionBuilder.ParameterTypesAreAccessible` over the given types.</summary>
		bool TypesAccessible(IType[] types)
		{
			var bodyResolver = (body ?? throw new InvalidOperationException("The engine asked about a C# body without the body's context")).Resolver;
			var currentTypeDefinition = bodyResolver.CurrentTypeDefinition;
			if (currentTypeDefinition == null)
				return true;
			var lookup = new MemberLookup(currentTypeDefinition, currentTypeDefinition.ParentModule);
			return types.All(IsAccessible);

			bool IsAccessible(IType type)
			{
				switch (type)
				{
					case ParameterizedType pt:
						return IsAccessible(pt.GenericType) && pt.TypeArguments.All(IsAccessible);
					case ICSharpCode.Decompiler.TypeSystem.Implementation.TypeWithElementType t:
						return IsAccessible(t.ElementType);
					default:
						for (var td = type.GetDefinition(); td != null; td = td.DeclaringTypeDefinition)
						{
							if (!lookup.IsAccessible(td, allowProtectedAccess: true))
								return false;
						}
						return true;
				}
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

		/// <summary>
		/// <c>Disambiguator.LookUpName(target, name, [], invocation: true)</c>: 0 when the name is no
		/// method group, else 1 and its method lists (each the declaring type definition and the
		/// method definitions, as module index and metadata row), as unsigned LEB128.
		/// </summary>
		byte[] MethodGroup(RustResolverQuery query, bool invocation = true)
		{
			var context = body ?? throw new InvalidOperationException("The engine asked about a C# body without the body's context");
			var resolver = context.Resolver;
			ResolveResult? result;
			if (!query.HasTarget)
			{
				result = resolver.ResolveSimpleName(query.Name, query.TypeArguments, isInvocationTarget: invocation);
			}
			else
			{
				var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule);
				result = lookup.Lookup(TargetResolveResult(query), query.Name, query.TypeArguments, isInvocation: invocation);
			}
			if (result is not MethodGroupResolveResult group)
				return [0];
			return EncodeMethodLists(group.MethodsGroupedByDeclaringType
				.Select(l => (l.DeclaringType, (IReadOnlyList<IParameterizedMember>)l)).ToList());
		}

		/// <summary>
		/// <c>ResolveMemberAccess(target, name, typeArguments, NameLookupMode.InvocationTarget)</c>,
		/// as <c>IsUnambiguousMethodReference</c> resolves an extension method reference: the
		/// method lists encoded as for <see cref="MethodGroup"/>, then the number of extension
		/// method groups and per group the number of methods and each definition's module index
		/// and MethodDef row, as unsigned LEB128; 0 for no method group or a method without row.
		/// </summary>
		byte[] ExtensionMethodGroup(RustResolverQuery query)
		{
			var context = body ?? throw new InvalidOperationException("The engine asked about a C# body without the body's context");
			var result = context.Resolver.ResolveMemberAccess(TargetResolveResult(query), query.Name, query.TypeArguments, NameLookupMode.InvocationTarget);
			if (result is not MethodGroupResolveResult group)
				return [0];
			var lists = EncodeMethodLists(group.MethodsGroupedByDeclaringType
				.Select(l => (l.DeclaringType, (IReadOnlyList<IParameterizedMember>)l)).ToList());
			if (lists.Length == 1)
				return [0];
			var output = new List<byte>(lists);
			var groups = group.GetExtensionMethods().Select(g => g.ToList()).ToList();
			UVar(groups.Count);
			foreach (var methods in groups)
			{
				UVar(methods.Count);
				foreach (var method in methods)
				{
					var definition = (IMethod)method.MemberDefinition;
					int module = -1;
					for (int i = 0; i < modules.Count; i++)
					{
						if (modules[i] == definition.ParentModule)
							module = i;
					}
					if (module < 0 || definition.MetadataToken.Kind != HandleKind.MethodDefinition)
						return [0];
					UVar(module);
					UVar(MetadataTokens.GetRowNumber(definition.MetadataToken));
				}
			}
			return output.ToArray();

			void UVar(int value)
			{
				uint v = checked((uint)value);
				while (v >= 0x80)
				{
					output.Add((byte)((v & 0x7F) | 0x80));
					v >>= 7;
				}
				output.Add((byte)v);
			}
		}

		/// <summary>
		/// <c>Disambiguator.IsUnambiguousAccess</c>'s candidates for an indexer: the indexers of the
		/// target, each as its getter (or its setter), encoded as for <see cref="MethodGroup"/>.
		/// </summary>
		byte[] Indexers(RustResolverQuery query)
		{
			var context = body ?? throw new InvalidOperationException("The engine asked about a C# body without the body's context");
			var resolver = context.Resolver;
			var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule);
			var lists = new List<(IType, IReadOnlyList<IParameterizedMember>)>();
			foreach (var list in lookup.LookupIndexers(TargetResolveResult(query)))
			{
				var accessors = new List<IParameterizedMember>();
				foreach (var member in list)
				{
					if (member is not IProperty { } property || (property.Getter ?? property.Setter) is not { } accessor)
						return [0];
					accessors.Add(accessor);
				}
				lists.Add((list.DeclaringType, accessors));
			}
			return EncodeMethodLists(lists);
		}

		/// <summary>
		/// <c>Disambiguator.IsUnambiguousCall</c>'s candidates for <c>newobj</c>: the constructors
		/// of the declaring type the decompiled method can access, encoded as for
		/// <see cref="MethodGroup"/>.
		/// </summary>
		byte[] Constructors(RustResolverQuery query)
		{
			var context = body ?? throw new InvalidOperationException("The engine asked about a C# body without the body's context");
			var resolver = context.Resolver;
			var method = query.Method;
			var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule);
			bool allowProtectedAccess = resolver.CurrentTypeDefinition == method.DeclaringTypeDefinition;
			// A value type's synthesized parameterless constructor has no metadata row; it cannot
			// apply to a call with arguments, so leaving it out does not change the resolution.
			bool takesArguments = method.Parameters.Count > 0 && !method.Parameters.Any(p => p.IsParams);
			var constructors = method.DeclaringType.GetConstructors()
				.Where(c => lookup.IsAccessible(c, allowProtectedAccess))
				.Where(c => !(takesArguments && c.Parameters.Count == 0 && c.MetadataToken.IsNil))
				.ToList<IParameterizedMember>();
			// The candidates are added one by one, so no constructors (as of an unknown type,
			// which has no definition to encode) are no method lists.
			if (constructors.Count == 0)
				return EncodeMethodLists([]);
			return EncodeMethodLists([(method.DeclaringType, constructors)]);
		}

		/// <summary>
		/// Method lists as unsigned LEB128: 1, the number of lists and per list the declaring type
		/// definition and the method definitions, as module index and metadata row; 0 when one of
		/// them has no metadata row.
		/// </summary>
		byte[] EncodeMethodLists(List<(IType DeclaringType, IReadOnlyList<IParameterizedMember> Methods)> lists)
		{
			var output = new List<byte>();
			UVar(1);
			UVar(lists.Count);
			foreach (var list in lists)
			{
				var declaring = list.DeclaringType.GetDefinition();
				int declaringModule = declaring == null ? -1 : ModuleIndex(declaring.ParentModule);
				if (declaringModule < 0)
					return [0];
				UVar(declaringModule);
				UVar(MetadataTokens.GetRowNumber(declaring!.MetadataToken));
				UVar(list.Methods.Count);
				foreach (var member in list.Methods)
				{
					var definition = (IMethod)member.MemberDefinition;
					int module = ModuleIndex(definition.ParentModule);
					if (module < 0 || definition.MetadataToken.Kind != HandleKind.MethodDefinition)
						return [0];
					UVar(module);
					UVar(MetadataTokens.GetRowNumber(definition.MetadataToken));
				}
			}
			return output.ToArray();

			void UVar(int value)
			{
				uint v = checked((uint)value);
				while (v >= 0x80)
				{
					output.Add((byte)((v & 0x7F) | 0x80));
					v >>= 7;
				}
				output.Add((byte)v);
			}

			int ModuleIndex(IModule? module)
			{
				for (int i = 0; i < modules.Count; i++)
				{
					if (modules[i] == module)
						return i;
				}
				return -1;
			}
		}

		/// <summary>
		/// <c>Disambiguator.IsUnambiguousAccess</c> for a property without parameters: 0 when its
		/// name binds to no member the accessor call may write, 1 when it binds to the accessor's
		/// property, 2 when it binds to another member the call may write.
		/// </summary>
		int AccessorReference(RustResolverQuery query)
		{
			var context = body ?? throw new InvalidOperationException("The engine asked about a C# body without the body's context");
			var resolver = context.Resolver;
			var owner = query.Method.AccessorOwner ?? throw new InvalidOperationException("The engine asked about an accessor of no property");
			ResolveResult? result;
			if (!query.HasTarget)
			{
				result = resolver.ResolveSimpleName(owner.Name, EmptyList<IType>.Instance, isInvocationTarget: false);
			}
			else
			{
				var lookup = new MemberLookup(resolver.CurrentTypeDefinition, resolver.CurrentTypeDefinition.ParentModule);
				result = lookup.Lookup(TargetResolveResult(query), owner.Name, EmptyList<IType>.Instance, isInvocation: false);
			}
			if (result is not MemberResolveResult { IsError: false } resolved)
				return 0;
			var details = new ExpectedTargetDetails { CallOpCode = query.CallVirt ? OpCode.CallVirt : OpCode.Call };
			if (!Disambiguator.IsAppropriateCallTarget(details, owner, resolved.Member))
				return 0;
			return resolved.Member.Equals(owner) ? 1 : 2;
		}

		/// <summary><c>CallBuilder.TryUseExtensionMethodSyntax</c>'s question to the expression builder's resolver.
		/// Bit 0 is the answer, bit 1 + i the type mismatch flag of the i-th lambda argument after it.</summary>
		int CanUseExtensionMethodSyntax(RustResolverQuery query)
		{
			var context = body ?? throw new InvalidOperationException("The engine asked about a C# body without the body's context");
			for (int i = 0; i < query.ArgumentResults.Length; i++)
			{
				if (query.ArgumentResults[i] is QueryMethodGroup group)
				{
					if (group.LookUp(context.Resolver) is not { } found)
						return -1;
					query.ArgumentResults[i] = found;
				}
			}
			int answer = context.Resolver.CanTransformToExtensionMethodCall(query.Method, query.TypeArguments,
				query.TargetResult!, query.ArgumentResults, query.ArgumentNames!) ? 1 : 0;
			int bit = 2;
			foreach (var lambda in query.ArgumentResults.OfType<DecompiledLambdaResolveResult>())
			{
				if (lambda.AttemptedConversionWithTypeMismatch)
					answer |= bit;
				bit <<= 1;
			}
			return answer;
		}

		/// <summary>The resolve result a member lookup sees for the query's target.</summary>
		static ResolveResult TargetResolveResult(RustResolverQuery query)
		{
			var type = query.Target!;
			return query.TargetKind switch {
				// The lookup only asks whether the target is a ThisResolveResult.
				RustResolverQuery.ThisTarget => new ThisResolveResult(new ILVariable(VariableKind.Parameter, type, -1), type, query.CausesNonVirtualInvocation),
				RustResolverQuery.TypeTarget => new TypeResolveResult(type),
				_ => new ResolveResult(type),
			};
		}

		/// <summary>Rethrows the first exception the resolver threw during the engine call.</summary>
		public void ThrowIfFailed() => failure?.Throw();
	}
}
