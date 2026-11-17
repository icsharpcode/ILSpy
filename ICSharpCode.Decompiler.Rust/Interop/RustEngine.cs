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
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>An error or panic reported by the Rust engine.</summary>
	public sealed class RustEngineException : Exception
	{
		public RustEngineException(string message) : base(message)
		{
		}
	}

	/// <summary>
	/// The engine session of one type system: its compilation is loaded once and shared by every
	/// decompiler on that type system, also across threads.
	/// </summary>
	internal sealed class RustEngine
	{
		sealed class Entry
		{
			public RustEngine? Engine;
			public string? UnsupportedReason;
		}

		static readonly ConditionalWeakTable<IDecompilerTypeSystem, Entry> engines = new();

		/// <summary>The number of IL transforms the engine runs (<c>IL_PREFIX</c>).</summary>
		public static readonly int ILPrefix = (int)NativeMethods.ilspy_il_prefix();

		readonly IDecompilerTypeSystem typeSystem;
		readonly RustSession session;
		readonly IReadOnlyList<MetadataModule> modules;

		RustEngine(IDecompilerTypeSystem typeSystem)
		{
			this.typeSystem = typeSystem;
			session = RustSession.Create();
			using var table = new RustModuleTable(typeSystem);
			table.Load(session);
			modules = table.Modules;
		}

		/// <summary>
		/// The engine for <paramref name="typeSystem"/>, or null with the reason when the engine
		/// cannot describe it (a main module without a PE image, such as a synthetic one).
		/// </summary>
		public static RustEngine? For(IDecompilerTypeSystem typeSystem, out string? unsupportedReason)
		{
			var entry = engines.GetValue(typeSystem, Create);
			unsupportedReason = entry.UnsupportedReason;
			return entry.Engine;
		}

		static Entry Create(IDecompilerTypeSystem typeSystem)
		{
			if (typeSystem.MainModule is not MetadataModule { MetadataFile: PEFile file })
				return new Entry { UnsupportedReason = "the main module has no PE image" };
			if (!file.Reader.IsEntireImageAvailable)
				return new Entry { UnsupportedReason = "the main module's PE image is not in memory" };
			return new Entry { Engine = new RustEngine(typeSystem) };
		}

		/// <summary>
		/// The function of <paramref name="method"/> after the first <paramref name="ilPrefix"/>
		/// IL transforms, asking <paramref name="resolver"/> the C# resolver questions. The names AssignVariableNames
		/// reserves around the method come from the type system and <paramref name="usingScope"/>.
		/// <paramref name="combineExits"/> runs CombineExitsTransform after the prefix.
		/// <paramref name="translateBody"/> also has the engine translate the body into C#, which
		/// <see cref="RustSeam2.ServeBody"/> hands to the decompiler. <paramref name="body"/> answers the
		/// body translation's name lookups as the method's ExpressionBuilder resolves them.
		/// <see cref="RustStatus.Unsupported"/> leaves <paramref name="function"/>
		/// null; an engine error or panic throws <see cref="RustEngineException"/>.
		/// </summary>
		public RustStatus ReadFunction(MethodDefinitionHandle method, int ilPrefix, DecompilerSettings settings,
			IDebugInfoProvider? debugInfo, out ILFunction? function, out string? unsupportedReason, CSharpResolver? resolver = null, UsingScope? usingScope = null, bool combineExits = false, bool translateBody = false, RustBodyContext? body = null)
		{
			function = null;
			unsupportedReason = null;
			uint flags = settings.UseRefLocalsForAccurateOrderOfEvaluation ? 1u : 0u;
			if (combineExits)
				flags |= 2;
			if (translateBody)
				flags |= 4;
			var bridge = debugInfo == null ? null : new RustDebugInfo(debugInfo);
			var resolverBridge = resolver == null ? null
				: new RustResolver(resolver, MetadataTokens.GetRowNumber(method), typeSystem, modules, settings, body);
			var declaringType = ((MetadataModule)typeSystem.MainModule).GetDefinition(method).DeclaringTypeDefinition;
			byte[] rootNames = declaringType == null ? [] : RootNames(declaringType, usingScope);
			var (settingsLow, settingsHigh) = RustSettings.Bits(settings);
			var status = NativeMethods.ilspy_session_read_function(session, (uint)MetadataTokens.GetToken(method), (uint)ilPrefix,
				flags, settingsLow, settingsHigh, IntPtr.Zero, bridge?.Delegate, IntPtr.Zero, resolverBridge?.Delegate,
				rootNames, (UIntPtr)rootNames.Length, out var buffer);
			GC.KeepAlive(bridge);
			GC.KeepAlive(resolverBridge);
			byte[] bytes = NativeMethods.TakeBuffer(buffer);
			bridge?.ThrowIfFailed();
			resolverBridge?.ThrowIfFailed();
			switch (status)
			{
				case RustStatus.Ok:
					var (read, error) = IlAstMaterializer.ReadModule(bytes, typeSystem, modules, settings)[MetadataTokens.GetRowNumber(method)];
					function = read ?? throw new RustEngineException(error ?? "no function");
					return status;
				case RustStatus.Unsupported:
					unsupportedReason = NativeMethods.LastError;
					return status;
				case RustStatus.Cancelled:
					throw new OperationCanceledException();
				default:
					throw new RustEngineException($"{status}: {NativeMethods.LastError}");
			}
		}

		/// <summary>
		/// <c>TypeSystemAstBuilder.ConvertEntity(entity)</c> of a field or method as the engine writes
		/// it with the options <c>CSharpDecompiler.CreateAstBuilder</c> gives the builder; null with
		/// <paramref name="unsupportedReason"/> where the engine does not write the declaration.
		/// </summary>
		public CSharp.Syntax.EntityDeclaration? EntitySyntax(IEntity entity, bool useSpecialConstants, DecompilerSettings settings, out string? unsupportedReason, bool useCustomEvents = false, bool asExtension = false, bool asEnumMember = false)
		{
			unsupportedReason = null;
			uint flags = (useSpecialConstants ? 1u : 0u)
				| (settings.SortCustomAttributes ? 2u : 0u)
				| (settings.IntroducePrivateProtectedAccessibility ? 4u : 0u)
				| (settings.LiftNullables ? 8u : 0u)
				| (useCustomEvents ? 16u : 0u)
				| (settings.InitAccessors ? 32u : 0u)
				| (settings.RecordClasses ? 64u : 0u)
				| (settings.RecordStructs ? 128u : 0u)
				| (asExtension ? 256u : 0u)
				| (asEnumMember ? 512u : 0u)
				| (settings.RequiredMembers ? 1024u : 0u)
				| (settings.FixedBuffers ? 2048u : 0u)
				| (settings.IntroduceRefModifiersOnStructs ? 4096u : 0u);
			// A member of an extension block comes specialized with the block's type parameters,
			// which belong to the block's marker type (`ExtensionInfo.GetMembersOfGroup`).
			uint contextToken = 0;
			if (entity is IMember { Substitution.ClassTypeArguments: [ITypeParameter { Owner: ITypeDefinition marker }, ..] })
				contextToken = (uint)MetadataTokens.GetToken(marker.MetadataToken);
			var status = NativeMethods.ilspy_session_entity_syntax(session, (uint)MetadataTokens.GetToken(entity.MetadataToken), flags, contextToken, out var buffer);
			byte[] bytes = NativeMethods.TakeBuffer(buffer);
			switch (status)
			{
				case RustStatus.Ok:
					var reader = new EntitySyntaxReader(bytes, typeSystem, modules);
					return entity switch {
						IField field when asEnumMember => reader.EnumMember(field, out unsupportedReason),
						IField field => reader.Field(field, out unsupportedReason),
						IMethod method => reader.Method(method, settings, out unsupportedReason),
						IProperty or IEvent => reader.Property((IMember)entity, out unsupportedReason),
						ITypeDefinition when asExtension => reader.Extension(out unsupportedReason),
						ITypeDefinition type => reader.TypeDefinition(type, out unsupportedReason),
						_ => throw new ArgumentException("An entity request for a field or method", nameof(entity)),
					};
				case RustStatus.Cancelled:
					throw new OperationCanceledException();
				default:
					throw new RustEngineException($"{status}: {NativeMethods.LastError}");
			}
		}

		/// <summary>
		/// <c>CSharpDecompiler.MemberIsHidden</c> of a type, method or field of the main module as the
		/// engine decides it.
		/// </summary>
		public bool MemberIsHidden(EntityHandle member, DecompilerSettings settings)
		{
			var status = NativeMethods.ilspy_session_member_is_hidden(session, (uint)MetadataTokens.GetToken(member), HiddenFlags(settings), out byte hidden);
			switch (status)
			{
				case RustStatus.Ok:
					return hidden != 0;
				case RustStatus.Cancelled:
					throw new OperationCanceledException();
				default:
					throw new RustEngineException($"{status}: {NativeMethods.LastError}");
			}
		}

		/// <summary>
		/// The comment <c>DoDecompile(IField)</c> writes after a field of the main module with an RVA.
		/// </summary>
		public string FieldDataComment(EntityHandle field)
		{
			var status = NativeMethods.ilspy_session_field_data_comment(session, (uint)MetadataTokens.GetToken(field), out var buffer);
			byte[] bytes = NativeMethods.TakeBuffer(buffer);
			switch (status)
			{
				case RustStatus.Ok:
					return System.Text.Encoding.UTF8.GetString(bytes);
				case RustStatus.Cancelled:
					throw new OperationCanceledException();
				default:
					throw new RustEngineException($"{status}: {NativeMethods.LastError}");
			}
		}

		/// <summary>
		/// <c>SetNewModifier</c>: whether the declaration of an entity of the main module takes
		/// <c>new</c>; null with <paramref name="unsupportedReason"/> where the host decides it.
		/// </summary>
		public bool? HidesBaseMember(EntityHandle entity, out string? unsupportedReason)
		{
			unsupportedReason = null;
			var status = NativeMethods.ilspy_session_new_modifier(session, (uint)MetadataTokens.GetToken(entity), out var buffer);
			byte[] bytes = NativeMethods.TakeBuffer(buffer);
			switch (status)
			{
				case RustStatus.Ok:
					if (bytes[0] == 2)
					{
						unsupportedReason = System.Text.Encoding.UTF8.GetString(bytes, 1, bytes.Length - 1);
						return null;
					}
					return bytes[0] != 0;
				case RustStatus.Cancelled:
					throw new OperationCanceledException();
				default:
					throw new RustEngineException($"{status}: {NativeMethods.LastError}");
			}
		}

		/// <summary>The settings <c>MemberIsHidden</c> reads, as <c>csharp::member_hidden::HiddenSettings</c>.</summary>
		static uint HiddenFlags(DecompilerSettings settings)
		{
			return (settings.LocalFunctions ? 1u << 0 : 0u)
				| (settings.AnonymousMethods ? 1u << 1 : 0u)
				| (settings.YieldReturn ? 1u << 2 : 0u)
				| (settings.AsyncAwait ? 1u << 3 : 0u)
				| (settings.AsyncEnumerator ? 1u << 4 : 0u)
				| (settings.FixedBuffers ? 1u << 5 : 0u)
				| (settings.InlineArrays ? 1u << 6 : 0u)
				| (settings.ExtensionMembers ? 1u << 7 : 0u)
				| (settings.ArrayInitializers ? 1u << 8 : 0u)
				| (settings.AnonymousTypes ? 1u << 9 : 0u)
				| (settings.Dynamic ? 1u << 10 : 0u)
				| (settings.SwitchStatementOnString ? 1u << 11 : 0u)
				| (settings.UsePrimaryConstructorSyntaxForNonRecordTypes ? 1u << 12 : 0u)
				| (settings.AutomaticProperties ? 1u << 13 : 0u)
				| (settings.FieldKeyword ? 1u << 14 : 0u)
				| (settings.GetterOnlyAutomaticProperties ? 1u << 15 : 0u)
				| (settings.AutomaticEvents ? 1u << 16 : 0u);
		}

		/// <summary>
		/// The member walk of <c>DoDecompile(ITypeDefinition)</c> for a type of the main module: its
		/// entities in <c>allOrderedEntities</c> order with the walk's decision for each
		/// (0 decompiled, 1 hidden, 2 hidden unless it backs an event that is not automatic), and
		/// for an enum <paramref name="enumDisplayMode"/> (<c>EnumValueDisplayMode</c>, 255 where the
		/// host decides it), and whether the declaration drops <c>[DefaultMember]</c>
		/// (<paramref name="removesDefaultMember"/>); null
		/// with <paramref name="unsupportedReason"/> where the engine does not walk the type.
		/// </summary>
		public List<(EntityHandle Entity, byte Decision)>? MemberWalk(ITypeDefinition type, DecompilerSettings settings, out string? unsupportedReason, out byte enumDisplayMode, out bool removesDefaultMember)
		{
			unsupportedReason = null;
			enumDisplayMode = 255;
			removesDefaultMember = false;
			uint flags = HiddenFlags(settings)
				| (settings.RecordClasses ? 1u << 17 : 0u)
				| (settings.RecordStructs ? 1u << 18 : 0u)
				| (settings.AlwaysShowEnumMemberValues ? 1u << 19 : 0u);
			var status = NativeMethods.ilspy_session_member_walk(session, (uint)MetadataTokens.GetToken(type.MetadataToken), flags, out var buffer);
			byte[] bytes = NativeMethods.TakeBuffer(buffer);
			switch (status)
			{
				case RustStatus.Ok:
					if (bytes[0] != 0)
					{
						unsupportedReason = System.Text.Encoding.UTF8.GetString(bytes, 1, bytes.Length - 1);
						return null;
					}
					enumDisplayMode = bytes[1];
					removesDefaultMember = bytes[2] != 0;
					var walk = new List<(EntityHandle, byte)>((bytes.Length - 3) / 5);
					for (int i = 3; i + 5 <= bytes.Length; i += 5)
						walk.Add((MetadataTokens.EntityHandle(BitConverter.ToInt32(bytes, i)), bytes[i + 4]));
					return walk;
				case RustStatus.Cancelled:
					throw new OperationCanceledException();
				default:
					throw new RustEngineException($"{status}: {NativeMethods.LastError}");
			}
		}

		/// <summary>
		/// The names AssignVariableNames' root scope collects around a method of <paramref name="type"/>, which
		/// the managed type system and using scope list: first the lower-case member names (inherited ones
		/// included), which it only avoids, then the names it also reserves (the parameters of a primary
		/// constructor, the lower-case types of the namespace, of the enclosing types and of the using scope).
		/// Each list is a count and then length-prefixed UTF-8 strings, as unsigned LEB128.
		/// </summary>
		static byte[] RootNames(ITypeDefinition type, UsingScope? usingScope)
		{
			static bool IsLowerCase(string name) => name.Length > 0 && char.ToLowerInvariant(name[0]) == name[0];
			var members = type.GetMembers(m => IsLowerCase(m.Name)).Select(m => m.Name).ToList();
			var reserved = new List<string>();
			foreach (IField field in type.Fields)
			{
				if (TransformFieldAndConstructorInitializers.IsGeneratedPrimaryConstructorBackingField(field))
					reserved.Add(field.Name.Substring(1, field.Name.Length - 3));
			}
			var ns = type.ParentModule!.Compilation.GetNamespaceByFullName(type.Namespace);
			reserved.AddRange(ns!.Types.Select(t => t.Name).Where(IsLowerCase));
			for (var current = type; current != null; current = current.DeclaringTypeDefinition)
				reserved.AddRange(current.NestedTypes.Select(t => t.Name).Where(IsLowerCase));
			if (usingScope != null)
				reserved.AddRange(usingScope.Usings.SelectMany(n => n.Types).Select(t => t.Name).Where(IsLowerCase));

			var stream = new System.IO.MemoryStream();
			void UVar(ulong v)
			{
				while (v >= 0x80)
				{
					stream.WriteByte((byte)((v & 0x7F) | 0x80));
					v >>= 7;
				}
				stream.WriteByte((byte)v);
			}
			foreach (var list in new[] { members, reserved })
			{
				UVar((ulong)list.Count);
				foreach (string name in list)
				{
					byte[] bytes = System.Text.Encoding.UTF8.GetBytes(name);
					UVar((ulong)bytes.Length);
					stream.Write(bytes, 0, bytes.Length);
				}
			}
			return stream.ToArray();
		}
	}
}
