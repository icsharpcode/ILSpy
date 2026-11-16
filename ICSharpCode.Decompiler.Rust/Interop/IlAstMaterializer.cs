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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// Reads the engine's wire format (rust/ilspy-decompiler/src/wire): the header, the string
	/// table and the type table, whose entries become link-compiled <see cref="IType"/>s.
	/// </summary>
	internal class WireReader
	{
		readonly byte[] data;
		int pos;
		readonly string[] strings;
		readonly IType?[] types;
		readonly int[] typeOffsets;
		/// <summary>The offset of each type's syntax entry; null when no body asked for them.</summary>
		readonly int[]? typeSyntaxOffsets;
		/// <summary>The entry each decoded type came from.</summary>
		readonly Dictionary<IType, int> typeIndex = new Dictionary<IType, int>(ReferenceComparer.Instance);
		protected readonly ICompilation compilation;
		protected readonly IReadOnlyList<MetadataModule> modules;

		public WireReader(byte[] data, uint expectedVersion, ICompilation compilation, IReadOnlyList<MetadataModule> modules)
		{
			this.data = data;
			this.compilation = compilation;
			this.modules = modules;
			if (data.Length < 4 || Encoding.ASCII.GetString(data, 0, 4) != "ILSW")
				throw new InvalidOperationException("Not an engine wire message");
			pos = 4;
			uint version = (uint)UVar();
			if (version != expectedVersion)
				throw new InvalidOperationException($"Wire version {version}, expected {expectedVersion}");
			strings = new string[Len()];
			for (int i = 0; i < strings.Length; i++)
			{
				int length = Len();
				strings[i] = Encoding.UTF8.GetString(data, pos, length);
				pos += length;
			}
			// Each entry is length-prefixed and decoded on first use: an entry may refer to a
			// generic context the function header only defines later.
			types = new IType?[Len()];
			typeOffsets = new int[types.Length];
			for (int i = 0; i < types.Length; i++)
			{
				int length = Len();
				typeOffsets[i] = pos;
				pos += length;
			}
			if (Bool())
			{
				typeSyntaxOffsets = new int[types.Length];
				for (int i = 0; i < types.Length; i++)
				{
					int length = Len();
					typeSyntaxOffsets[i] = pos;
					pos += length;
				}
			}
		}

		/// <summary>
		/// <c>TypeSystemAstBuilder.ConvertType(type)</c> as the engine built it for the entry
		/// <paramref name="type"/> was decoded from, with its <see cref="TypeResolveResult"/>
		/// annotations; null for a type of no entry, or one the engine does not convert.
		/// </summary>
		public CSharp.Syntax.AstType? TypeSyntax(IType type)
		{
			if (typeSyntaxOffsets == null || !typeIndex.TryGetValue(type, out int index))
				return null;
			int saved = pos;
			pos = typeSyntaxOffsets[index];
			var syntax = TypeSyntaxNode();
			pos = saved;
			return syntax;
		}

		CSharp.Syntax.AstType? TypeSyntaxNode()
		{
			CSharp.Syntax.AstType result;
			switch (U8())
			{
				case 0:
					return null;
				case 1:
					result = new CSharp.Syntax.PrimitiveType(Str());
					break;
				case 2:
				{
					var simple = new CSharp.Syntax.SimpleType(Str());
					int count = Len();
					for (int i = 0; i < count; i++)
						simple.TypeArguments.Add(TypeSyntaxNode()!);
					result = simple;
					break;
				}
				case 3:
				{
					var composed = new CSharp.Syntax.ComposedType { BaseType = TypeSyntaxNode()! };
					composed.HasNullableSpecifier = Bool();
					composed.HasRefSpecifier = Bool();
					composed.PointerRank = Len();
					int count = Len();
					for (int i = 0; i < count; i++)
						composed.ArraySpecifiers.Add(new CSharp.Syntax.ArraySpecifier(Len()));
					result = composed;
					break;
				}
				case 4:
				{
					var tuple = new CSharp.Syntax.TupleAstType();
					int count = Len();
					for (int i = 0; i < count; i++)
					{
						var element = new CSharp.Syntax.TupleTypeElement { Type = TypeSyntaxNode()! };
						int name = Len();
						if (name != 0)
							element.Name = strings[name - 1];
						tuple.Elements.Add(element);
					}
					result = tuple;
					break;
				}
				default:
					throw new InvalidOperationException("Unknown wire type syntax tag");
			}
			int annotations = Len();
			for (int i = 0; i < annotations; i++)
				result.AddAnnotation(new TypeResolveResult(Type()));
			return result;
		}

		public bool AtEnd => pos == data.Length;

		public byte U8() => data[pos++];

		public bool Bool() => U8() != 0;

		public ulong UVar()
		{
			ulong result = 0;
			int shift = 0;
			while (true)
			{
				byte b = data[pos++];
				result |= (ulong)(b & 0x7F) << shift;
				if ((b & 0x80) == 0)
					return result;
				shift += 7;
			}
		}

		public long IVar()
		{
			ulong v = UVar();
			return (long)(v >> 1) ^ -(long)(v & 1);
		}

		public int Int() => checked((int)IVar());

		public int Len() => checked((int)UVar());

		public uint Token() => checked((uint)UVar());

		public string Str() => strings[Len()];

		public IType Type() => TypeAt(Len());

		protected IType TypeAt(int index)
		{
			if (types[index] is { } type)
				return type;
			int saved = pos;
			pos = typeOffsets[index];
			type = ReadTypeEntry();
			pos = saved;
			if (!typeIndex.ContainsKey(type))
				typeIndex.Add(type, index);
			return types[index] = type;
		}

		/// <summary>Method type parameter <paramref name="index"/> of explicit generic context <paramref name="context"/>.</summary>
		protected virtual ITypeParameter ContextMethodTypeParameter(int context, int index)
			=> throw new InvalidOperationException("This message has no generic contexts");

		public byte[] Raw(int count)
		{
			var result = new byte[count];
			Array.Copy(data, pos, result, 0, count);
			pos += count;
			return result;
		}

		internal IType[] TypeList()
		{
			var result = new IType[Len()];
			for (int i = 0; i < result.Length; i++)
				result[i] = Type();
			return result;
		}

		MetadataModule Module() => modules[Len()];

		ITypeDefinition TypeDefinition()
		{
			var module = Module();
			return module.GetDefinition(MetadataTokens.TypeDefinitionHandle(Len()));
		}

		IType ReadTypeEntry()
		{
			switch (U8())
			{
				case 0:
					return TypeDefinition();
				case 1:
				{
					var top = new TopLevelTypeName(Str(), Str(), Len());
					var name = new FullTypeName(top);
					int nested = Len();
					for (int i = 0; i < nested; i++)
						name = name.NestedType(Str(), Len());
					bool? isReferenceType = U8() switch { 0 => null, 1 => false, _ => true };
					return new UnknownType(name, isReferenceType);
				}
				case 2:
					return SpecialType.UnknownType;
				case 3:
					return SpecialType.UnboundTypeArgument;
				case 4:
				{
					byte owner = U8();
					switch (owner)
					{
						case 0:
							return TypeDefinition().TypeParameters[Len()];
						case 1:
						{
							var module = Module();
							var method = module.GetDefinition(MetadataTokens.MethodDefinitionHandle(Len()));
							return method.TypeParameters[Len()];
						}
						case 4:
						{
							int context = Len();
							return ContextMethodTypeParameter(context, Len());
						}
						case 2:
							return DummyTypeParameter.GetClassTypeParameter(Len());
						default:
							return DummyTypeParameter.GetMethodTypeParameter(Len());
					}
				}
				case 5:
					return new ParameterizedType(Type(), TypeList());
				case 6:
					return new ArrayType(compilation, Type(), Len(), (Nullability)U8());
				case 7:
					return new PointerType(Type());
				case 8:
					return new ByReferenceType(Type());
				case 9:
					return new PinnedType(Type());
				case 10:
				{
					var convention = (SignatureCallingConvention)U8();
					var custom = TypeList();
					var ret = Type();
					bool retRefReadOnly = Bool();
					var parameters = TypeList();
					var refKinds = Raw(parameters.Length).Select(k => (ReferenceKind)k);
					return new FunctionPointerType((MetadataModule)compilation.MainModule, convention,
						custom.ToImmutableArray(), ret, retRefReadOnly,
						parameters.ToImmutableArray(), refKinds.ToImmutableArray());
				}
				case 11:
					return SpecialType.ArgList;
				case 12:
					return new ModifiedType(Type(), Type(), Bool());
				case 13:
					return SpecialType.Dynamic;
				case 14:
					return SpecialType.NInt;
				case 15:
					return SpecialType.NUInt;
				case 16:
				{
					int module = Len();
					var elements = TypeList();
					var names = new string?[Len()];
					for (int i = 0; i < names.Length; i++)
					{
						int n = Len();
						names[i] = n == 0 ? null : strings[n - 1];
					}
					return new TupleType(compilation, elements.ToImmutableArray(), names.Select(n => n!).ToImmutableArray(),
						module == 0 ? null : modules[module - 1]);
				}
				case 17:
					return Type().ChangeNullability((Nullability)U8());
				case 18:
					return SpecialType.NullType;
				case 19:
					return SpecialType.NoType;
				case 20:
					return IntersectionType.Create(TypeList());
				default:
					throw new InvalidOperationException("Unknown wire type tag");
			}
		}
	}

	/// <summary>
	/// Builds link-compiled <see cref="ILFunction"/>s from wire v1 (rust/ilspy-decompiler/src/wire/ilast.rs):
	/// the IL reader's output for every method body of the main module.
	/// </summary>
	internal sealed class IlAstMaterializer : WireReader
	{
		public const uint Version = 4;

		readonly MetadataModule mainModule;
		readonly DecompilerSettings settings;
		int currentRow;
		GenericContext context;
		GenericContext[] explicitContexts = [];

		protected override ITypeParameter ContextMethodTypeParameter(int context, int index)
			=> explicitContexts[context].MethodTypeParameters![index];
		ILVariable[] variables = [];
		Block[] blocks = [];
		BlockContainer[] containers = [];

		IlAstMaterializer(byte[] data, ICompilation compilation, IReadOnlyList<MetadataModule> modules, DecompilerSettings settings)
			: base(data, Version, compilation, modules)
		{
			mainModule = modules[0];
			this.settings = settings;
		}

		/// <summary>Per MethodDef row: the function, or the engine's error message.</summary>
		public static Dictionary<int, (ILFunction? Function, string? Error)> ReadModule(
			byte[] wire, ICompilation compilation, IReadOnlyList<MetadataModule> modules, DecompilerSettings settings)
		{
			var reader = new IlAstMaterializer(wire, compilation, modules, settings);
			var result = new Dictionary<int, (ILFunction?, string?)>();
			int count = reader.Len();
			for (int i = 0; i < count; i++)
			{
				int row = reader.Len();
				if (reader.U8() != 0)
					result.Add(row, (null, reader.Str()));
				else
					result.Add(row, (reader.ReadFunction(row), null));
			}
			if (!reader.AtEnd)
				throw new InvalidOperationException("Trailing bytes after the last function");
			return result;
		}

		/// <summary>
		/// A <c>resolver_query</c> message (rust/ilspy-decompiler/src/wire/ilast.rs), read in the
		/// generic context of the main module's MethodDef <paramref name="row"/>.
		/// </summary>
		public static RustResolverQuery ReadResolverQuery(byte[] wire, int row, ICompilation compilation,
			IReadOnlyList<MetadataModule> modules, DecompilerSettings settings)
		{
			var reader = new IlAstMaterializer(wire, compilation, modules, settings);
			reader.context = new GenericContext(reader.mainModule.GetDefinition(MetadataTokens.MethodDefinitionHandle(row)));
			reader.ReadContexts();
			var query = new RustResolverQuery { Kind = reader.U8() };
			if (query.Kind == RustResolverQuery.SingularizeKind)
			{
				query.Name = reader.Str();
				if (!reader.AtEnd)
					throw new InvalidOperationException("Trailing bytes after the resolver query");
				return query;
			}
			if (query.Kind == RustResolverQuery.TypesAccessibleKind)
			{
				query.Arguments = new IType[reader.Len()];
				for (int i = 0; i < query.Arguments.Length; i++)
					query.Arguments[i] = reader.Type();
				if (!reader.AtEnd)
					throw new InvalidOperationException("Trailing bytes after the resolver query");
				return query;
			}
			if (query.Kind is RustResolverQuery.MethodGroupKind or RustResolverQuery.AccessorKind or RustResolverQuery.IndexersKind or RustResolverQuery.MethodReferenceKind)
			{
				if (query.Kind == RustResolverQuery.AccessorKind)
				{
					query.Method = reader.CallMethod();
					query.CallVirt = reader.Bool();
				}
				else
				{
					query.Name = reader.Str();
				}
				query.HasTarget = reader.Bool();
				if (query.HasTarget)
				{
					query.TargetKind = reader.U8();
					if (query.TargetKind == RustResolverQuery.ThisTarget)
						query.CausesNonVirtualInvocation = reader.Bool();
					query.Target = reader.Type();
				}
				if (query.Kind is RustResolverQuery.MethodGroupKind or RustResolverQuery.IndexersKind or RustResolverQuery.MethodReferenceKind)
				{
					query.TypeArguments = new IType[reader.Len()];
					for (int i = 0; i < query.TypeArguments.Length; i++)
						query.TypeArguments[i] = reader.Type();
				}
				if (!reader.AtEnd)
					throw new InvalidOperationException("Trailing bytes after the resolver query");
				return query;
			}
			if (query.Kind is RustResolverQuery.FieldSimpleNameKind or RustResolverQuery.FieldMemberAccessKind or RustResolverQuery.FieldKindKind)
			{
				query.Field = reader.Field();
				if (query.Kind == RustResolverQuery.FieldKindKind)
					query.TargetIsThis = reader.Bool();
				if (query.Kind == RustResolverQuery.FieldMemberAccessKind)
				{
					query.TargetKind = reader.U8();
					if (query.TargetKind == RustResolverQuery.ThisTarget)
						query.CausesNonVirtualInvocation = reader.Bool();
					query.Target = reader.Type();
				}
				if (!reader.AtEnd)
					throw new InvalidOperationException("Trailing bytes after the resolver query");
				return query;
			}
			query.Method = reader.CallMethod();
			if (query.Kind == RustResolverQuery.CallKind)
			{
				query.Target = reader.Type();
				if (reader.Bool())
					query.TargetStore = (reader.Type(), reader.Type());
				query.Arguments = new IType[reader.Len()];
				for (int i = 0; i < query.Arguments.Length; i++)
					query.Arguments[i] = reader.Type();
			}
			if (query.Kind == RustResolverQuery.ExtensionSyntaxKind)
			{
				query.TypeArguments = new IType[reader.Len()];
				for (int i = 0; i < query.TypeArguments.Length; i++)
					query.TypeArguments[i] = reader.Type();
				if (reader.Bool())
					query.Method = WithMethodTypeArguments(query.Method, reader.TypeList());
				query.TargetResult = reader.QueryOperand();
				query.ArgumentResults = new ResolveResult[reader.Len()];
				for (int i = 0; i < query.ArgumentResults.Length; i++)
					query.ArgumentResults[i] = reader.QueryOperand();
				if (reader.U8() != 0)
				{
					query.ArgumentNames = new string?[query.ArgumentResults.Length];
					for (int i = 0; i < query.ArgumentNames.Length; i++)
						query.ArgumentNames[i] = reader.Bool() ? reader.Str() : null;
				}
			}
			if (!reader.AtEnd)
				throw new InvalidOperationException("Trailing bytes after the resolver query");
			return query;
		}

		/// <summary>
		/// An operand of a resolver query: 0 and the type of an expression, 1, the type and the
		/// constant, 2, the element type and the <see cref="ReferenceKind"/> of a by-reference
		/// argument, 3 and the type of an <c>out var</c>, 4 and the delegate type of a lambda; 5, a
		/// method group as a <see cref="QueryMethodGroup"/>.
		/// </summary>
		ResolveResult QueryOperand()
		{
			byte kind = U8();
			if (kind == 5)
			{
				var method = CallMethod();
				byte targetKind = U8();
				if (targetKind == 4)
				{
					// `CallBuilder.ToMethodGroup` of a local function.
					string name = Str();
					Len();
					return new MethodGroupResolveResult(null, name,
						new[] { new MethodListWithDeclaringType(method.DeclaringType, new IParameterizedMember[] { method }) },
						method.TypeArguments);
				}
				bool nonVirtual = targetKind == 1 && Bool();
				var target = targetKind == 0 ? null : Type();
				var typeArguments = new IType[Len()];
				for (int i = 0; i < typeArguments.Length; i++)
					typeArguments[i] = Type();
				return new QueryMethodGroup(method, targetKind, nonVirtual, target, typeArguments);
			}
			var type = Type();
			return kind switch {
				0 => new ResolveResult(type),
				1 => new ConstantResolveResult(type, Constant()),
				2 => new ByReferenceResolveResult(type, (ReferenceKind)U8()),
				3 => new OutVarResolveResult(type),
				4 => QueryLambda(type),
				_ => throw new InvalidOperationException("Unknown resolver query operand"),
			};
		}

		/// <summary>
		/// The lambda of a resolver query: its return type, inferred return type, parameters and
		/// type mismatch flag, over a function without body, as overload resolution sees it.
		/// </summary>
		DecompiledLambdaResolveResult QueryLambda(IType delegateType)
		{
			var returnType = Type();
			var inferredReturnType = Type();
			var parameters = new IParameter[Len()];
			for (int i = 0; i < parameters.Length; i++)
			{
				var type = Type();
				parameters[i] = new DefaultParameter(type, Str());
			}
			var function = new ILFunction(returnType, parameters, default, new Nop(), ILFunctionKind.Delegate) { DelegateType = delegateType };
			bool attemptedConversionWithTypeMismatch = Bool();
			bool isImplicitlyTyped = Bool();
			bool isAnonymousMethod = Bool();
			return new DecompiledLambdaResolveResult(function, delegateType, inferredReturnType,
				hasParameterList: !isAnonymousMethod, isAnonymousMethod: isAnonymousMethod, isImplicitlyTyped: isImplicitlyTyped) {
				AttemptedConversionWithTypeMismatch = attemptedConversionWithTypeMismatch
			};
		}

		/// <summary><c>ILFunction.helperVariableCount</c>, which <see cref="ILFunction.RegisterVariable"/> continues.</summary>
		static readonly System.Reflection.FieldInfo helperVariableCountField =
			typeof(ILFunction).GetField("helperVariableCount", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;

		ILInstruction If()
		{
			int type = Len();
			var condition = Node();
			var trueInst = Node();
			var falseInst = Node();
			return new IfInstruction(condition, trueInst, falseInst, type != 0 ? TypeAt(type - 1) : null);
		}

		ILInstruction LdMemberToken()
		{
			var token = Member(out var memberContext);
			return new LdMemberToken(mainModule.ResolveEntity(token, memberContext) as IMember
				?? throw new InvalidOperationException("ldtoken operand is not a member"));
		}

		ILInstruction GetPinnableReference()
		{
			var type = Type();
			var method = Bool() ? Method() : null;
			return new GetPinnableReference(Node(), type, method);
		}

		const byte NodeReference = 255;

		Await Await()
		{
			var getAwaiter = AwaiterMethod();
			var getResult = AwaiterMethod();
			var opCode = Bool() ? OpCode.CallVirt : OpCode.Call;
			return new Await(Node()) { GetAwaiterMethod = getAwaiter, GetResultMethod = getResult, GetAwaiterCallOpCode = opCode };
		}

		/// <summary>None, a method, or the method of a dynamic await (a <c>FakeMethod</c> on and
		/// returning <c>dynamic</c>, as <c>AsyncAwaitDecompiler.CreateDynamicAwaiterMethod</c>).</summary>
		IMethod? AwaiterMethod()
		{
			switch (U8())
			{
				case 0:
					return null;
				case 1:
					return Method();
				default:
					string name = Str();
					var fakeMethod = typeof(IMethod).Assembly.GetType("ICSharpCode.Decompiler.TypeSystem.Implementation.FakeMethod", throwOnError: true)!;
					var method = (IMethod)Activator.CreateInstance(fakeMethod, mainModule.Compilation, SymbolKind.Method)!;
					fakeMethod.GetProperty("Name")!.SetValue(method, name);
					fakeMethod.GetProperty("DeclaringType")!.SetValue(method, SpecialType.Dynamic);
					fakeMethod.GetProperty("ReturnType")!.SetValue(method, SpecialType.Dynamic);
					return method;
			}
		}

		CSharpArgumentInfo[] ArgumentInfos()
		{
			var infos = new CSharpArgumentInfo[Len()];
			for (int i = 0; i < infos.Length; i++)
			{
				var flags = (CSharpArgumentInfoFlags)(int)IVar();
				string? name = Bool() ? Str() : null;
				infos[i] = new CSharpArgumentInfo { Flags = flags, Name = name, CompileTimeType = Type() };
			}
			return infos;
		}

		IType? OptionalType()
		{
			int t = Len();
			return t == 0 ? null : TypeAt(t - 1);
		}

		ILInstruction Dynamic()
		{
			byte kind = U8();
			var binderFlags = (CSharpBinderFlags)(int)IVar();
			var context = OptionalType();
			string? name = Bool() ? Str() : null;
			var operation = (System.Linq.Expressions.ExpressionType)(int)IVar();
			var type = OptionalType();
			IType[]? typeArguments = null;
			if (Bool())
			{
				typeArguments = new IType[Len()];
				for (int i = 0; i < typeArguments.Length; i++)
					typeArguments[i] = Type();
			}
			var infos = ArgumentInfos();
			var args = Nodes();
			return kind switch {
				0 => new DynamicBinaryOperatorInstruction(binderFlags, operation, context, infos[0], args[0], infos[1], args[1]),
				1 => new DynamicUnaryOperatorInstruction(binderFlags, operation, context, infos[0], args[0]),
				2 => new DynamicConvertInstruction(binderFlags, type!, context, args[0]),
				3 => new DynamicGetMemberInstruction(binderFlags, name, context, infos[0], args[0]),
				4 => new DynamicSetMemberInstruction(binderFlags, name, context, infos[0], args[0], infos[1], args[1]),
				5 => new DynamicGetIndexInstruction(binderFlags, context, infos, args),
				6 => new DynamicSetIndexInstruction(binderFlags, context, infos, args),
				7 => new DynamicInvokeMemberInstruction(binderFlags, name!, typeArguments, context, type, infos, args),
				8 => new DynamicInvokeConstructorInstruction(binderFlags, type!, context, infos, args),
				9 => new DynamicInvokeInstruction(binderFlags, context, infos, args),
				10 => new DynamicIsEventInstruction(binderFlags, name, context, args[0]),
				11 => new DynamicLogicOperatorInstruction(binderFlags, operation, context, infos[0], args[0], infos[1], args[1]),
				_ => throw new InvalidOperationException($"Unknown dynamic instruction kind {kind}"),
			};
		}

		ILFunction ReadFunction(int row)
		{
			var method = mainModule.GetDefinition(MetadataTokens.MethodDefinitionHandle(row));
			context = new GenericContext(method);
			currentRow = row;
			transformContext = null;
			ReadContexts();
			int codeSize = Len();
			int localSignatureLength = Len();
			int asyncReturnType = Len();
			int helperVariableCount = Len();
			byte stateMachineFlags = U8();
			var moveNextMethod = Bool() ? Method() : null;
			AsyncDebugInfo? asyncDebugInfo = null;
			if (Bool())
			{
				int catchHandlerOffset = Int();
				var awaits = new AsyncDebugInfo.Await[Len()];
				for (int i = 0; i < awaits.Length; i++)
					awaits[i] = new AsyncDebugInfo.Await(Int(), Int());
				asyncDebugInfo = new AsyncDebugInfo(catchHandlerOffset, ImmutableArray.Create(awaits));
			}
			var topLevelVariables = new ILVariable[Len()];
			for (int i = 0; i < topLevelVariables.Length; i++)
				topLevelVariables[i] = ReadVariable();
			// The variables of nested functions follow; each function lists its own.
			var nestedVariables = new ILVariable[Len()];
			for (int i = 0; i < nestedVariables.Length; i++)
				nestedVariables[i] = ReadVariable();
			variables = topLevelVariables.Concat(nestedVariables).ToArray();
			var warnings = new string[Len()];
			for (int i = 0; i < warnings.Length; i++)
				warnings[i] = Str();
			var sequencePoints = new List<int>(Len());
			for (int i = sequencePoints.Capacity; i > 0; i--)
				sequencePoints.Add(Int());
			blocks = new Block[Len()];
			for (int i = 0; i < blocks.Length; i++)
			{
				blocks[i] = new Block((BlockKind)U8());
				blocks[i].SetILRange(new Interval(Int(), Int()));
			}
			containers = new BlockContainer[Len()];
			materialized = new List<ILInstruction>();
			var body = Node();
			var localFunctions = new ILFunction[Len()];
			for (int i = 0; i < localFunctions.Length; i++)
				localFunctions[i] = (ILFunction)Node();
			var function = new ILFunction(method, codeSize, context, body, ILFunctionKind.TopLevelFunction);
			function.Variables.AddRange(topLevelVariables);
			function.LocalFunctions.AddRange(localFunctions);
			// Capture scopes refer to containers, so they follow the body.
			for (int i = Len(); i > 0; i--)
			{
				var captured = Variable();
				var scope = containers[Len()];
				captured.CaptureScope = scope;
			}
			for (int i = Len(); i > 0; i--)
			{
				var localFunction = (ILFunction)materialized[Len()];
				localFunction.DeclarationScope = containers[Len()];
			}
			for (int i = Len(); i > 0; i--)
			{
				int owner = Len();
				var captured = Variable();
				(owner == 0 ? function : (ILFunction)materialized[owner - 1]).CapturedVariables.Add(captured);
			}
			function.LocalVariableSignatureLength = localSignatureLength;
			if (asyncReturnType != 0)
				function.AsyncReturnType = TypeAt(asyncReturnType - 1);
			function.AddRef();
			helperVariableCountField.SetValue(function, helperVariableCount);
			function.IsIterator = (stateMachineFlags & 1) != 0;
			function.StateMachineCompiledWithMono = (stateMachineFlags & 2) != 0;
			function.StateMachineCompiledWithLegacyVisualBasic = (stateMachineFlags & 4) != 0;
			function.MoveNextMethod = moveNextMethod;
			if (asyncDebugInfo is { } info)
				function.AsyncDebugInfo = info;
			function.SequencePointCandidates = sequencePoints;
			function.Warnings.AddRange(warnings);
			switch (U8())
			{
				case 1:
					RustSeam2.Attach(function, null, Str());
					break;
				case 2:
					RustSeam2.Attach(function, BodyNode(), null, this);
					break;
			}
			return function;
		}

		/// <summary>
		/// A node of the C# translation of the function body (rust/ilspy-decompiler/src/wire/ast.rs),
		/// with its instructions, variables and types resolved.
		/// </summary>
		RustBodyNode BodyNode()
		{
			var node = new RustBodyNode { Kind = U8() };
			node.IL = new ILInstruction[Len()];
			for (int i = 0; i < node.IL.Length; i++)
				node.IL[i] = materialized[Len()];
			node.ILBeforeResolveResult = Bool();
			node.ResolveResult = BodyResolveResult(out node.OperatorType);
			node.Call = pendingInvocation;
			pendingInvocation = null;
			node.ResolveCast = pendingResolveCast;
			pendingResolveCast = false;
			node.ImplicitConversion = pendingImplicitConversion;
			pendingImplicitConversion = false;
			node.LambdaConversion = pendingLambdaConversion;
			pendingLambdaConversion = false;
			node.ResolveOperator = pendingResolveOperator;
			pendingResolveOperator = false;
			node.MethodGroup = pendingMethodGroup;
			pendingMethodGroup = null;
			node.LookUpMember = pendingLookUpMember;
			pendingLookUpMember = false;
			int implicitReturn = Len();
			if (implicitReturn != 0)
				node.ImplicitReturn = (Leave)materialized[implicitReturn - 1];
			if (Bool())
			{
				node.DroppedCastIsImplicitOperator = U8() == 0;
				node.DroppedCastType = Type();
			}
			switch (node.Kind)
			{
				case RustBodyNode.Block:
					node.Children = new RustBodyNode[Len()];
					for (int i = 0; i < node.Children.Length; i++)
						node.Children[i] = BodyNode();
					break;
				case RustBodyNode.SwitchExpression:
				{
					// The value, then pattern and body per section.
					var switchValue = BodyNode();
					int arms = Len();
					node.Children = new RustBodyNode[1 + 2 * arms];
					node.Children[0] = switchValue;
					for (int i = 1; i < node.Children.Length; i++)
						node.Children[i] = BodyNode();
					break;
				}
				case RustBodyNode.Switch:
				{
					// The value, then the sections.
					var value = BodyNode();
					node.Children = new RustBodyNode[Len() + 1];
					node.Children[0] = value;
					for (int i = 1; i < node.Children.Length; i++)
						node.Children[i] = BodyNode();
					break;
				}
				case RustBodyNode.SwitchSection:
				{
					// Whether each label has an expression (and the expression), then the statements.
					var labels = new bool[Len()];
					var children = new List<RustBodyNode>();
					for (int i = 0; i < labels.Length; i++)
					{
						labels[i] = Bool();
						if (labels[i])
							children.Add(BodyNode());
					}
					int statements = Len();
					for (int i = 0; i < statements; i++)
						children.Add(BodyNode());
					node.Value = labels;
					node.Children = children.ToArray();
					break;
				}
				case RustBodyNode.Return:
					node.Children = Bool() ? [BodyNode()] : [];
					break;
				case RustBodyNode.ConstantValue:
				case RustBodyNode.HexConstant:
					node.Type = Type();
					node.Value = Constant();
					node.Flag = Bool();
					if (Bool())
					{
						bool complete = true;
						var syntax = ConstantSyntax(ref complete);
						node.ConstantSyntax = complete ? syntax : null;
					}
					break;
				case RustBodyNode.StringLiteral:
					node.Value = String();
					break;
				case RustBodyNode.Identifier:
				case RustBodyNode.Label:
				case RustBodyNode.Goto:
					node.Value = Str();
					break;
				case RustBodyNode.MemberReference:
					node.Children = [BodyNode()];
					node.Value = Str();
					break;
				case RustBodyNode.TypeReference:
					node.Type = Type();
					break;
				case RustBodyNode.RefDirection:
				case RustBodyNode.ExpressionStatement:
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.Assignment:
					node.Children = [BodyNode(), BodyNode()];
					break;
				case RustBodyNode.Invocation:
				{
					var target = BodyNode();
					var arguments = new RustBodyNode[Len()];
					for (int i = 0; i < arguments.Length; i++)
						arguments[i] = BodyNode();
					node.Children = [target, .. arguments];
					break;
				}
				case RustBodyNode.NamedArgument:
					node.Value = Str();
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.IfElse:
				{
					var condition = BodyNode();
					var trueStatement = BodyNode();
					node.Children = Bool() ? [condition, trueStatement, BodyNode()] : [condition, trueStatement];
					break;
				}
				case RustBodyNode.LogicNot:
				case RustBodyNode.Await:
				case RustBodyNode.ThrowExpression:
				case RustBodyNode.BitNot:
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.Throw:
					node.Children = Bool() ? [BodyNode()] : [];
					break;
				case RustBodyNode.Conditional:
					node.Flag = Bool();
					node.Children = [BodyNode(), BodyNode(), BodyNode()];
					break;
				case RustBodyNode.TypeOf:
					node.Type = Type();
					break;
				case RustBodyNode.As:
					node.Type = Type();
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.Tuple:
				{
					// The ValueTuple type, then per element its name (if any) and the element.
					node.Type = Type();
					node.Flag = Bool();
					var names = new string?[Len()];
					node.Children = new RustBodyNode[names.Length];
					for (int i = 0; i < names.Length; i++)
					{
						names[i] = Bool() ? Str() : null;
						node.Children[i] = BodyNode();
					}
					node.Value = names;
					break;
				}
				case RustBodyNode.OutDirection:
					node.Children = [BodyNode()];
					node.Flag = Bool();
					break;
				case RustBodyNode.TryCatch:
				{
					var tryBlock = BodyNode();
					var clauses = new RustBodyNode[Len()];
					for (int i = 0; i < clauses.Length; i++)
						clauses[i] = BodyNode();
					// The finally block, if any, comes last; Flag says whether there is one.
					node.Flag = Bool();
					node.Children = node.Flag ? [tryBlock, .. clauses, BodyNode()] : [tryBlock, .. clauses];
					break;
				}
				case RustBodyNode.CatchClause:
				{
					node.Value = Bool() ? Str() : null;
					node.Type = Bool() ? Type() : null;
					bool hasCondition = Bool();
					var condition = hasCondition ? BodyNode() : null;
					var body = BodyNode();
					node.Children = condition != null ? [body, condition] : [body];
					break;
				}
				case RustBodyNode.ArrayCreate:
				{
					// The element type, UseSpecialConstants for the elements, the dimensions, then
					// whether an initializer follows, and the initializer.
					node.Type = Type();
					node.Flag = Bool();
					var children = new List<RustBodyNode>();
					int dimensions = Len();
					for (int i = 0; i < dimensions; i++)
						children.Add(BodyNode());
					bool hasInitializer = Bool();
					if (hasInitializer)
						children.Add(BodyNode());
					node.Value = hasInitializer;
					node.Children = children.ToArray();
					break;
				}
				case RustBodyNode.DelegateCreate:
					node.Type = Type();
					node.Value = CallMethod();
					node.Flag = Bool();
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.Foreach:
					// Whether it is `await foreach`, the variable, the three instructions of the annotation,
					// the collection and the body.
					node.Flag = Bool();
					bool hasDesignation = Bool();
					node.Value = Variable();
					node.ForeachInstructions = [materialized[Len()], materialized[Len()], materialized[Len()]];
					node.Children = hasDesignation ? [BodyNode(), BodyNode(), BodyNode()] : [BodyNode(), BodyNode()];
					break;
				case RustBodyNode.DefaultValue:
					node.Type = Bool() ? Type() : null;
					break;
				case RustBodyNode.IsType:
					node.Type = Type();
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.IsPattern:
					node.Children = [BodyNode(), BodyNode()];
					break;
				case RustBodyNode.DeclarationPattern:
					node.Value = Variable();
					node.Flag = Bool();
					break;
				case RustBodyNode.RecursivePattern:
					// The designation, the type, then member and pattern per sub-pattern.
					node.Value = Bool() ? Variable() : null;
					node.Type = Bool() ? Type() : null;
					node.Members = new IMember[Len()];
					node.Children = new RustBodyNode[node.Members.Length];
					for (int i = 0; i < node.Members.Length; i++)
					{
						node.Members[i] = U8() == 0 ? Field() : CallMethod().AccessorOwner
							?? throw new InvalidOperationException("Sub-pattern of a method that is no accessor");
						node.Children[i] = BodyNode();
					}
					break;
				case RustBodyNode.UsingExpanded:
					// The variable, whether it is the store, whether it is an `await using`, and the
					// resource or the body.
					node.Value = Variable();
					node.ResolveOperator = Bool();
					node.Flag = Bool();
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.ParenthesizedDesignation:
					node.Children = new RustBodyNode[Len()];
					for (int i = 0; i < node.Children.Length; i++)
						node.Children[i] = BodyNode();
					break;
				case RustBodyNode.SingleDesignation:
					node.Value = Bool() ? Variable() : null;
					break;
				case RustBodyNode.InDirection:
				case RustBodyNode.GotoCase:
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.DeconstructionTuple:
					// The elements.
					node.Children = new RustBodyNode[Len()];
					for (int i = 0; i < node.Children.Length; i++)
						node.Children[i] = BodyNode();
					break;
				case RustBodyNode.PatternUnary:
					// The operator, then the pattern.
					node.Value = (int)U8();
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.NullConditional:
					node.Flag = Bool();
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.YieldReturn:
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.ArrayInitializer:
					node.Children = new RustBodyNode[Len()];
					for (int i = 0; i < node.Children.Length; i++)
						node.Children[i] = BodyNode();
					break;
				case RustBodyNode.Named:
					node.Value = Str();
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.InterpolatedString:
				{
					// Texts, and interpolations with alignment and suffix, whose expressions are the children.
					var contents = new (string? Text, int Alignment, string? Suffix)[Len()];
					var children = new List<RustBodyNode>();
					for (int i = 0; i < contents.Length; i++)
					{
						if (U8() == 0)
						{
							contents[i] = (String(), 0, null);
							continue;
						}
						int alignment = Int();
						string? suffix = Bool() ? String() : null;
						contents[i] = (null, alignment, suffix);
						children.Add(BodyNode());
					}
					node.Value = contents;
					node.Children = children.ToArray();
					break;
				}
				case RustBodyNode.LocalFunctionDeclaration:
					// The body.
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.Lambda:
					// The delegate type, the inferred return type, the type mismatch and implicit typing
					// flags, then the body.
					node.Type = Type();
					node.Value = Type();
					node.AttemptedConversionWithTypeMismatch = Bool();
					node.ImplicitlyTyped = Bool();
					node.AnonymousMethod = Bool();
					node.Flag = Bool();
					node.Children = [BodyNode()];
					break;
				case RustBodyNode.Using:
					// Whether it is `await using`, the declared variable, the resource and the body.
					node.Flag = Bool();
					node.Value = Bool() ? Variable() : null;
					node.Children = [BodyNode(), BodyNode()];
					break;
				case RustBodyNode.Lock:
					node.Children = [BodyNode(), BodyNode()];
					break;
				case RustBodyNode.DoWhile:
					node.Children = [BodyNode(), BodyNode()];
					break;
				case RustBodyNode.Indexer:
				{
					var target = BodyNode();
					var indices = new RustBodyNode[Len()];
					for (int i = 0; i < indices.Length; i++)
						indices[i] = BodyNode();
					node.Children = [target, .. indices];
					break;
				}
				case RustBodyNode.For:
				{
					// The condition, the iterators and the body last.
					var condition = BodyNode();
					var iterators = new RustBodyNode[Len()];
					for (int i = 0; i < iterators.Length; i++)
						iterators[i] = BodyNode();
					node.Children = [condition, .. iterators, BodyNode()];
					break;
				}
				case RustBodyNode.While:
				{
					// Flag: whether a condition precedes the body.
					node.Flag = Bool();
					node.Children = node.Flag ? [BodyNode(), BodyNode()] : [BodyNode()];
					break;
				}
				case RustBodyNode.Cast:
					node.Type = Type();
					node.Children = [BodyNode()];
					node.Flag = Bool();
					break;
				case RustBodyNode.GenericIdentifier:
				case RustBodyNode.GenericMemberReference:
				{
					if (node.Kind == RustBodyNode.GenericMemberReference)
						node.Children = [BodyNode()];
					node.Value = Str();
					node.TypeArguments = new IType[Len()];
					for (int i = 0; i < node.TypeArguments.Length; i++)
						node.TypeArguments[i] = Type();
					break;
				}
				case RustBodyNode.ObjectCreate:
				{
					node.Type = Type();
					var arguments = new RustBodyNode[Len()];
					for (int i = 0; i < arguments.Length; i++)
						arguments[i] = BodyNode();
					node.Children = arguments;
					if (Bool())
					{
						node.Initializer = new RustBodyNode[Len()];
						for (int i = 0; i < node.Initializer.Length; i++)
							node.Initializer[i] = BodyNode();
					}
					break;
				}
				case RustBodyNode.BinaryOperator:
					node.Value = (CSharp.Syntax.BinaryOperatorType)U8();
					node.Overflow = U8();
					node.Children = [BodyNode(), BodyNode()];
					break;
				case RustBodyNode.CompoundAssignment:
					node.Value = (CSharp.Syntax.AssignmentOperatorType)U8();
					node.Overflow = U8();
					node.Children = [BodyNode(), BodyNode()];
					break;
				case RustBodyNode.PostIncrement:
					node.Flag = Bool();
					node.Overflow = U8();
					node.Children = [BodyNode()];
					break;
			}
			return node;
		}

		/// <summary>The target and method of the invocation resolve result the last <see cref="BodyResolveResult"/> read.</summary>
		(ResolveResult? Target, IMethod Method, bool IsExtension, bool IsDelegate, bool FoundByLookup, bool IsExpanded)? pendingInvocation;

		/// <summary>
		/// The resolve result of a call's target, inside the call's: a cast's (which RustSeam2
		/// computes from the cast's node) is <see cref="RustSeam2.PendingTarget"/>, without
		/// marking the call's node as a cast.
		/// </summary>
		/// <summary>The definition of <paramref name="method"/> with its class type arguments and these method type arguments.</summary>
		static IMethod WithMethodTypeArguments(IMethod method, IType[] typeArguments)
			=> ((IMethod)method.MemberDefinition).Specialize(new TypeParameterSubstitution(method.Substitution.ClassTypeArguments, typeArguments));

		ResolveResult? NestedTargetResolveResult()
		{
			var result = BodyResolveResult(out _);
			if (pendingResolveCast || pendingImplicitConversion || pendingLambdaConversion)
			{
				pendingResolveCast = pendingImplicitConversion = pendingLambdaConversion = false;
				return RustSeam2.PendingTarget;
			}
			// A nested invocation's resolve result is completed on its own node; the one taking it
			// takes it from there.
			if (result == null && pendingInvocation != null)
			{
				pendingInvocation = null;
				return RustSeam2.PendingTarget;
			}
			return result;
		}

		/// <summary>Whether the last <see cref="BodyResolveResult"/> read was the <c>ResolveCast</c> of a cast.</summary>
		bool pendingResolveCast;

		/// <summary>Whether the last <see cref="BodyResolveResult"/> read was the implicit conversion of an operator call.</summary>
		bool pendingImplicitConversion;

		/// <summary>Whether the last <see cref="BodyResolveResult"/> read was the lambda conversion of a cast.</summary>
		bool pendingLambdaConversion;

		/// <summary>Whether the last <see cref="BodyResolveResult"/> read was a binary operator the host resolves.</summary>
		bool pendingResolveOperator;

		/// <summary>The method group the last <see cref="BodyResolveResult"/> read describes.</summary>
		(bool HasTarget, IType[] TypeArguments, IMethod Method)? pendingMethodGroup;

		/// <summary>Whether the last <see cref="BodyResolveResult"/> read names a member found by looking its name up again.</summary>
		bool pendingLookUpMember;

		/// <summary>
		/// A resolve result of the body translation; an operator's, whose operands are its
		/// children's, comes back as null with its type in <paramref name="operatorType"/>.
		/// </summary>
		ResolveResult? BodyResolveResult(out IType? operatorType)
		{
			operatorType = null;
			switch (U8())
			{
				case 0:
					return null;
				case 1:
				{
					var type = Type();
					return new ConstantResolveResult(type, Constant());
				}
				case 2:
				{
					var variable = variables[Len()];
					return new ILVariableResolveResult(variable, Type());
				}
				case 3:
				{
					var variable = variables[Len()];
					return new ThisResolveResult(variable, Type(), Bool());
				}
				case 4:
					return new TypeResolveResult(Type());
				case 5:
				{
					var target = NestedTargetResolveResult();
					return new MemberResolveResult(target, Field());
				}
				case 24:
				{
					// A tuple element: the field with the element type of the target's tuple type.
					var target = NestedTargetResolveResult();
					var field = Field();
					return new MemberResolveResult(target, field, returnTypeOverride: Type());
				}
				case 6:
					// RustSeam2 takes a cast element's resolve result from the built element.
					return new ByReferenceResolveResult(NestedTargetResolveResult()!, ReferenceKind.Ref);
				case 7:
					operatorType = Type();
					return null;
				case 9:
				{
					// The arguments are the invocation's; RustSeam2 completes the resolve result.
					var target = NestedTargetResolveResult();
					var method = CallMethod();
					bool isExtension = Bool(), isDelegate = Bool(), foundByLookup = Bool(), isExpanded = Bool();
					// The method the call resolves to, with other type arguments.
					if (Bool())
						method = WithMethodTypeArguments(method, TypeList());
					pendingInvocation = (target, method, isExtension, isDelegate, foundByLookup, isExpanded);
					return null;
				}
				case 10:
				{
					var target = NestedTargetResolveResult();
					var accessor = CallMethod();
					// RustSeam2 looks the member up again when the name binds to another one.
					pendingLookUpMember = Bool();
					return new MemberResolveResult(target, accessor.AccessorOwner
						?? throw new InvalidOperationException("Property resolve result of a method that is no accessor"));
				}
				case 11:
					// A constructor call has no target; RustSeam2 completes the resolve result.
					pendingInvocation = (null, CallMethod(), false, false, false, Bool());
					return null;
				case 12:
					return new ResolveResult(Type());
				case 13:
					// RustSeam2 resolves the cast over its operand.
					pendingResolveCast = true;
					return null;
				case 17:
					// RustSeam2 computes the implicit conversion of the operand.
					pendingImplicitConversion = true;
					return null;
				case 19:
					return new InitializedObjectResolveResult(Type());
				case 27:
					return new ThrowResolveResult();
				case 28:
				{
					// `CallBuilder.ToMethodGroup` of a local function.
					string name = String();
					var method = CallMethod();
					return new MethodGroupResolveResult(null, name,
						new[] { new MethodListWithDeclaringType(method.DeclaringType, new IParameterizedMember[] { method }) },
						method.TypeArguments);
				}
				case 26:
				{
					// RustSeam2 adds the arguments, the interpolations' resolve results.
					var type = Type();
					return new InterpolatedStringResolveResult(type, String());
				}
				case 25:
					// RustSeam2 replaces it with the TryCast conversion of the `as` node's operand.
					return new ResolveResult(Type());
				case 23:
				{
					// RustSeam2 looks the method group up again and narrows it to the method.
					bool hasTarget = Bool();
					var typeArguments = new IType[Len()];
					for (int i = 0; i < typeArguments.Length; i++)
						typeArguments[i] = Type();
					pendingMethodGroup = (hasTarget, typeArguments, CallMethod());
					return null;
				}
				case 22:
					// RustSeam2 resolves the binary operator over its operands.
					pendingResolveOperator = true;
					return null;
				case 21:
					return new DefaultLiteralResolveResult(Type());
				case 18:
					// RustSeam2 converts the lambda's resolve result.
					pendingLambdaConversion = true;
					return null;
				case 14:
				{
					var type = Type();
					return new TypeOfResolveResult(type, Type());
				}
				case 15:
					return new ByReferenceResolveResult(NestedTargetResolveResult()!, ReferenceKind.Out);
				case 30:
					return new MemberResolveResult(null, CallMethod());
				case 29:
				{
					var kind = Bool() ? ReferenceKind.RefReadOnly : ReferenceKind.In;
					return new ByReferenceResolveResult(NestedTargetResolveResult()!, kind);
				}
				case 16:
				{
					// `VisitLdLen`: the Length or LongLength property of System.Array.
					var target = NestedTargetResolveResult();
					bool isLong = Bool();
					string name = isLong ? "LongLength" : "Length";
					var arrayType = compilation.FindType(KnownTypeCode.Array);
					var member = arrayType.GetProperties(p => p.Name == name).FirstOrDefault();
					return member == null
						? new ResolveResult(compilation.FindType(isLong ? KnownTypeCode.Int64 : KnownTypeCode.Int32))
						: new MemberResolveResult(target, member);
				}
				case 8:
				{
					var target = NestedTargetResolveResult();
					var token = Member(out var memberContext);
					var ev = mainModule.ResolveEntity(token, memberContext) as IEvent
						?? throw new InvalidOperationException("Event token does not resolve to an event");
					return new MemberResolveResult(target, ev);
				}
				default:
					throw new InvalidOperationException("Unknown resolve result tag");
			}
		}

		/// <summary>A constant of the body translation, as the CLR value <see cref="ConstantResolveResult"/> holds.</summary>
		object? Constant()
		{
			switch (U8())
			{
				case 0:
					return null;
				case 1:
					return Bool();
				case 2:
					return (char)UVar();
				case 3:
					return (sbyte)IVar();
				case 4:
					return (byte)UVar();
				case 5:
					return (short)IVar();
				case 6:
					return (ushort)UVar();
				case 7:
					return (int)IVar();
				case 8:
					return (uint)UVar();
				case 9:
					return IVar();
				case 10:
					return UVar();
				case 11:
					return BitConverter.ToSingle(Raw(4), 0);
				case 12:
					return BitConverter.ToDouble(Raw(8), 0);
				case 13:
					return String();
				case 14:
				{
					var bits = Raw(16);
					return new decimal([BitConverter.ToInt32(bits, 0), BitConverter.ToInt32(bits, 4),
						BitConverter.ToInt32(bits, 8), BitConverter.ToInt32(bits, 12)]);
				}
				default:
					throw new InvalidOperationException("Unknown constant tag");
			}
		}

		ILVariable ReadVariable()
		{
			var kind = (VariableKind)U8();
			var type = Type();
			var stackType = (StackType)U8();
			int? index = U8() != 0 ? Int() : null;
			var v = new ILVariable(kind, type, stackType, index) { Name = Str() };
			byte flags = U8();
			v.HasGeneratedName = (flags & 1) != 0;
			if (kind != VariableKind.Parameter)
			{
				v.InitialValueIsInitialized = (flags & 2) != 0;
				v.UsesInitialValue = (flags & 4) != 0;
			}
			v.IsRefReadOnly = (flags & 8) != 0;
			v.RemoveIfRedundant = (flags & 16) != 0;
			v.IsScoped = (flags & 32) != 0;
			v.IsScopedWithoutInitializer = (flags & 64) != 0;
			if (Bool())
				v.StateMachineField = Field();
			return v;
		}

		ILVariable Variable() => variables[Len()];

		/// <summary>The explicit generic contexts (<c>GenericContext.explicit</c> in the engine).</summary>
		void ReadContexts()
		{
			explicitContexts = new GenericContext[Len()];
			for (int i = 0; i < explicitContexts.Length; i++)
			{
				var classTypeParameters = new ITypeParameter[Len()];
				for (int j = 0; j < classTypeParameters.Length; j++)
					classTypeParameters[j] = (ITypeParameter)Type();
				var methodTypes = new int[Len()];
				for (int j = 0; j < methodTypes.Length; j++)
					methodTypes[j] = Len();
				// From `first` on, the type parameters of a (specialized) method, whose constraints are
				// seen through its type arguments; their entries refer to this context.
				int first = methodTypes.Length;
				IMethod? source = null;
				if (Bool())
				{
					var handle = Member(out var memberContext);
					first = Len();
					source = mainModule.ResolveMethod(handle, memberContext);
				}
				var methodTypeParameters = new ITypeParameter[methodTypes.Length];
				for (int j = 0; j < methodTypeParameters.Length; j++)
					methodTypeParameters[j] = j < first ? (ITypeParameter)TypeAt(methodTypes[j]) : source!.TypeParameters[j];
				explicitContexts[i] = new GenericContext(classTypeParameters, methodTypeParameters);
			}
		}

		/// <summary>
		/// A member token and the context it resolves in: the function's own, the state machine
		/// context the yield-return and async decompilers read compiler-generated methods in (the
		/// function's class and method type parameters as class type parameters), or an explicit one.
		/// </summary>
		EntityHandle Member(out GenericContext memberContext)
		{
			ulong v = UVar();
			memberContext = context;
			if ((v & 1) != 0)
			{
				int index = Len();
				memberContext = index == 1
					? new GenericContext(
						classTypeParameters: (context.ClassTypeParameters ?? []).Concat(context.MethodTypeParameters ?? []).ToArray(),
						methodTypeParameters: null)
					: explicitContexts[index - 2];
			}
			return MetadataTokens.EntityHandle(checked((int)(v >> 1)));
		}

		IMethod Method()
		{
			var token = Member(out var memberContext);
			if (!token.IsNil)
				return mainModule.ResolveMethod(token, memberContext);
			// A method found by type lookup: its type, name and parameter types.
			var type = Type();
			string name = Str();
			var parameterTypes = new IType[Len()];
			for (int i = 0; i < parameterTypes.Length; i++)
				parameterTypes[i] = Type();
			// shortcut: top-level nullability only, enough for the exception constructors sent so far
			static IType Oblivious(IType t) => t.ChangeNullability(Nullability.Oblivious);
			var candidates = name == ".ctor"
				? type.GetConstructors(m => !m.IsStatic && m.Parameters.Count == parameterTypes.Length)
				: type.GetMethods(m => m.Name == name && m.Parameters.Count == parameterTypes.Length)
					.Concat(type.GetAccessors(m => m.Name == name && m.Parameters.Count == parameterTypes.Length));
			return candidates
				.FirstOrDefault(m => m.Parameters.Select(p => Oblivious(p.Type)).SequenceEqual(parameterTypes.Select(Oblivious)))
				?? throw new InvalidOperationException($"No method {name} of {type} matches the sent parameter types");
		}

		/// <summary>
		/// A constant as the engine wrote it (rust/ilspy-decompiler/src/csharp/constant_syntax.rs);
		/// <paramref name="complete"/> turns false where a type has no syntax entry.
		/// </summary>
		CSharp.Syntax.Expression ConstantSyntax(ref bool complete)
		{
			CSharp.Syntax.Expression result;
			byte tag = U8();
			switch (tag)
			{
				case 0:
				{
					var value = Constant()!;
					result = new CSharp.Syntax.PrimitiveExpression(value,
						Bool() ? CSharp.Syntax.LiteralFormat.HexadecimalNumber : CSharp.Syntax.LiteralFormat.None);
					break;
				}
				case 1:
					result = new CSharp.Syntax.NullReferenceExpression();
					break;
				case 2:
					result = new CSharp.Syntax.DefaultValueExpression(SyntaxOf(Type(), ref complete));
					break;
				case 3:
				{
					var type = SyntaxOf(Type(), ref complete);
					result = new CSharp.Syntax.CastExpression(type, ConstantSyntax(ref complete));
					break;
				}
				case 4:
				{
					var keyword = Str();
					result = new CSharp.Syntax.CastExpression(new CSharp.Syntax.PrimitiveType(keyword), ConstantSyntax(ref complete));
					break;
				}
				case 5:
				case 6:
				{
					bool special = tag == 5;
					var type = Type();
					var module = modules[Len()];
					var field = module.GetDefinition(MetadataTokens.FieldDefinitionHandle(Len()));
					var target = new CSharp.Syntax.TypeReferenceExpression(SyntaxOf(type, ref complete));
					if (special)
						target.AddAnnotation(new TypeResolveResult(type));
					var reference = new CSharp.Syntax.MemberReferenceExpression(target, Str());
					reference.AddAnnotation(new MemberResolveResult(
						special ? new TypeResolveResult(type) : ErrorResolveResult.UnknownError, field));
					result = reference;
					break;
				}
				case 7:
				{
					var left = ConstantSyntax(ref complete);
					result = new CSharp.Syntax.BinaryOperatorExpression(left, CSharp.Syntax.BinaryOperatorType.BitwiseOr, ConstantSyntax(ref complete));
					break;
				}
				case 8:
					result = new CSharp.Syntax.UnaryOperatorExpression(CSharp.Syntax.UnaryOperatorType.Minus, ConstantSyntax(ref complete));
					break;
				case 9:
					result = new CSharp.Syntax.UnaryOperatorExpression(CSharp.Syntax.UnaryOperatorType.BitNot, ConstantSyntax(ref complete));
					break;
				default:
					throw new InvalidOperationException("Unknown wire constant syntax tag");
			}
			switch (U8())
			{
				case 1:
				{
					var type = Type();
					result.AddAnnotation(new ConstantResolveResult(type, Constant()));
					break;
				}
				case 2:
					result.AddAnnotation(new ConstantResolveResult(SpecialType.NullType, null));
					break;
			}
			return result;
		}

		/// <summary>The engine's syntax of <paramref name="type"/>, a placeholder where it has none.</summary>
		CSharp.Syntax.AstType SyntaxOf(IType type, ref bool complete)
		{
			if (TypeSyntax(type) is { } syntax)
				return syntax;
			complete = false;
			return new CSharp.Syntax.SimpleType("?");
		}

		IField Field()
		{
			var token = Member(out var memberContext);
			return mainModule.ResolveEntity(token, memberContext) as IField
				?? throw new InvalidOperationException("Field token does not resolve to a field");
		}

		ILInstruction[] Nodes()
		{
			var result = new ILInstruction[Len()];
			for (int i = 0; i < result.Length; i++)
				result[i] = Node();
			return result;
		}

		T Call<T>(T call) where T : CallInstruction
		{
			byte flags = U8();
			call.IsTail = (flags & 1) != 0;
			call.ILStackWasEmpty = (flags & 2) != 0;
			if ((flags & 4) != 0)
				call.ConstrainedTo = Type();
			call.Arguments.AddRange(Nodes());
			return call;
		}

		/// <summary>The nodes read so far, in completion order, for <c>NodeReference</c>.</summary>
		List<ILInstruction> materialized = new List<ILInstruction>();

		ILInstruction Node()
		{
			byte op = U8();
			// A node with two parents (an instruction the managed transforms share) is sent once.
			if (op == NodeReference)
				return materialized[Len()];
			var range = new Interval(Int(), Int());
			ILInstruction inst = op switch {
				0 => new InvalidBranch(Str()) { ExpectedResultType = (StackType)U8() },
				1 => new InvalidExpression(Str()) { ExpectedResultType = (StackType)U8(), Severity = Str() },
				2 => new Nop { Kind = (NopKind)U8() },
				3 => Container(),
				4 => ReadBlock(),
				5 => Binary(),
				6 => BitNot(),
				7 => new Arglist(),
				8 => new Branch(blocks[Len()]),
				9 => new Leave(containers[Len()], Node()),
				10 => If(),
				11 => Switch(),
				12 => Section(),
				13 => TryCatch(),
				14 => Handler(),
				15 => new TryFinally(Node(), Node()),
				16 => new TryFault(Node(), Node()),
				17 => new DebugBreak(),
				18 => Comp(),
				19 => Call(new Call(CallMethod())),
				20 => Call(new CallVirt(CallMethod())),
				21 => Call(new NewObj(CallMethod())),
				22 => CallIndirect(),
				23 => new Ckfinite(Node()),
				24 => Conv(),
				25 => new LdLoc(Variable()),
				26 => new LdLoca(Variable()),
				27 => StLoc(),
				28 => AddressOf(),
				29 => new LdStr(String()),
				30 => new LdcI4(Int()),
				31 => new LdcI8(IVar()),
				32 => new LdcF4(BitConverter.ToSingle(Raw(4), 0)),
				33 => new LdcF8(BitConverter.ToDouble(Raw(8), 0)),
				34 => new LdNull(),
				35 => new LdFtn(CallMethod()),
				36 => LdVirtFtn(),
				37 => new LdTypeToken(Type()),
				38 => LdMemberToken(),
				39 => new LocAlloc(Node()),
				40 => Cpblk(),
				41 => Initblk(),
				42 => LdFlda(),
				43 => new LdsFlda(Field()),
				44 => WithType((a, t) => new CastClass(a, t)),
				45 => WithType((a, t) => new IsInst(a, t)),
				46 => LdObj(),
				47 => WithType((a, t) => new LdObjIfRef(a, t)),
				48 => StObj(),
				49 => WithType((a, t) => new Box(a, t)),
				50 => WithType((a, t) => new Unbox(a, t)),
				51 => WithType((a, t) => new UnboxAny(a, t)),
				52 => NewArr(),
				53 => new DefaultValue(Type()) { ILStackWasEmpty = Bool() },
				54 => new Throw(Node()),
				55 => new Rethrow(),
				56 => new SizeOf(Type()),
				57 => LdLen(),
				58 => LdElema(),
				59 => WithType((a, t) => new MakeRefAny(a, t)),
				60 => new RefAnyType(Node()),
				61 => WithType((a, t) => new RefAnyValue(a, t)),
				62 => new PinnedRegion(Variable(), Node(), Node()),
				63 => GetPinnableReference(),
				64 => new YieldReturn(Node()),
				65 => Await(),
				66 => Dynamic(),
				67 => new LdcDecimal(new decimal(new[] { BitConverter.ToInt32(Raw(4), 0), BitConverter.ToInt32(Raw(4), 0), BitConverter.ToInt32(Raw(4), 0), BitConverter.ToInt32(Raw(4), 0) })),
				68 => StringToInt(),
				69 => Match(),
				70 => new LockInstruction(Node(), Node()),
				71 => Using(),
				72 => NumericCompoundAssign(),
				73 => UserDefinedCompoundAssign(),
				74 => LocAllocSpan(),
				75 => LdVirtDelegate(),
				76 => LdElemaInlineArray(),
				77 => NullCoalescing(),
				78 => DynamicCompoundAssign(),
				79 => new ThreeValuedBoolAnd(Node(), Node()),
				80 => new ThreeValuedBoolOr(Node(), Node()),
				81 => NullableUnwrap(),
				82 => NullableRewrap(),
				83 => new LdStrUtf8(String()),
				84 => LambdaFunction(),
				85 => ExpressionTreeCast(),
				86 => Deconstruct(),
				87 => DeconstructResult(),
				88 => new UserDefinedLogicOperator(CallMethod(), Node(), Node()),
				_ => throw new InvalidOperationException($"Unknown wire opcode {op}"),
			};
			inst.SetILRange(range);
			materialized.Add(inst);
			return inst;
		}

		ILInstruction NumericCompoundAssign()
		{
			bool lifted = Bool();
			var op = (BinaryNumericOperator)U8();
			bool check = Bool();
			var sign = (Sign)U8();
			var leftType = (StackType)U8();
			var rightType = (StackType)U8();
			var type = Type();
			var evalMode = (CompoundEvalMode)U8();
			var targetKind = (CompoundTargetKind)U8();
			var target = Node();
			var value = Node();
			// The instruction copies its operator from a binary instruction, whose right operand is
			// the value (as in TransformAssignment): the constructor checks it for pointer targets.
			var binary = new BinaryNumericInstruction(op, new Nop(), value, leftType, rightType, check, sign, lifted);
			return new NumericCompoundAssign(binary, target, targetKind, value, type, evalMode, TransformContext);
		}

		ILInstruction LocAllocSpan()
		{
			var type = Type();
			return new LocAllocSpan(Node(), type);
		}

		ILInstruction LdVirtDelegate()
		{
			var type = Type();
			var method = Method();
			return new LdVirtDelegate(Node(), type, method);
		}

		ILInstruction LdElemaInlineArray()
		{
			var type = Type();
			bool isReadOnly = Bool();
			var array = Node();
			return new LdElemaInlineArray(type, array, Nodes()) { IsReadOnly = isReadOnly };
		}

		ILInstruction DynamicCompoundAssign()
		{
			var operation = (System.Linq.Expressions.ExpressionType)(int)IVar();
			var binderFlags = (CSharpBinderFlags)(int)IVar();
			var infos = ArgumentInfos();
			var targetKind = (CompoundTargetKind)U8();
			var target = Node();
			return new DynamicCompoundAssign(operation, binderFlags, target, infos[0], Node(), infos[1], targetKind);
		}

		ILInstruction LambdaFunction()
		{
			var kind = (ILFunctionKind)U8();
			var delegateType = OptionalType();
			var returnType = Type();
			var parameters = new IParameter[Len()];
			for (int i = 0; i < parameters.Length; i++)
			{
				var type = Type();
				parameters[i] = new DefaultParameter(type, Str());
			}
			var functionVariables = new ILVariable[Len()];
			for (int i = 0; i < functionVariables.Length; i++)
				functionVariables[i] = Variable();
			int helperVariableCount = Len();
			Action<ILFunction>? readResults = null;
			IMethod? method = null;
			GenericContext readContext = default;
			if (Bool())
			{
				// A function read from a method body: ILReader.ReadIL in an explicit context. A local
				// function without a body has the use site's method and the bare context instead.
				var operandMethod = Bool() ? CallMethod() : null;
				var definition = mainModule.GetDefinition(MetadataTokens.MethodDefinitionHandle(Len()));
				int contextIndex = Len();
				if (operandMethod != null)
				{
					readContext = contextIndex == 0 ? default : explicitContexts[contextIndex - 2];
					method = operandMethod;
				}
				else
				{
					readContext = contextIndex == 0 ? new GenericContext(definition) : explicitContexts[contextIndex - 2];
					method = contextIndex == 0 ? definition : (IMethod)definition.Specialize(readContext.ToSubstitution());
				}
				int codeSize = Len();
				int localSignatureLength = Len();
				var asyncReturnType = OptionalType();
				byte stateMachineFlags = U8();
				var moveNextMethod = Bool() ? Method() : null;
				AsyncDebugInfo? asyncDebugInfo = null;
				if (Bool())
				{
					int catchHandlerOffset = Int();
					var awaits = new AsyncDebugInfo.Await[Len()];
					for (int i = 0; i < awaits.Length; i++)
						awaits[i] = new AsyncDebugInfo.Await(Int(), Int());
					asyncDebugInfo = new AsyncDebugInfo(catchHandlerOffset, ImmutableArray.Create(awaits));
				}
				var warnings = new string[Len()];
				for (int i = 0; i < warnings.Length; i++)
					warnings[i] = Str();
				var sequencePoints = new List<int>(Len());
				for (int i = sequencePoints.Capacity; i > 0; i--)
					sequencePoints.Add(Int());
				readResults = f => {
					f.CodeSize = codeSize;
					f.LocalVariableSignatureLength = localSignatureLength;
					f.AsyncReturnType = asyncReturnType;
					f.IsIterator = (stateMachineFlags & 1) != 0;
					f.StateMachineCompiledWithMono = (stateMachineFlags & 2) != 0;
					f.StateMachineCompiledWithLegacyVisualBasic = (stateMachineFlags & 4) != 0;
					f.MoveNextMethod = moveNextMethod;
					if (asyncDebugInfo is { } info)
						f.AsyncDebugInfo = info;
					f.SequencePointCandidates = sequencePoints;
					f.Warnings.AddRange(warnings);
				};
			}
			var body = Node();
			var localFunctions = new ILFunction[Len()];
			for (int i = 0; i < localFunctions.Length; i++)
				localFunctions[i] = (ILFunction)Node();
			(bool IsStatic, int Parameters, int TypeParameters)? reduced = Bool() ? (Bool(), Len(), Len()) : null;
			string? name = Bool() ? Str() : null;
			var function = method != null
				? new ILFunction(method, 0, readContext, body, kind) { DelegateType = delegateType }
				: new ILFunction(returnType, parameters, context, body, kind) { DelegateType = delegateType };
			function.Variables.AddRange(functionVariables);
			helperVariableCountField.SetValue(function, helperVariableCount);
			readResults?.Invoke(function);
			if (name != null)
				function.Name = name;
			function.LocalFunctions.AddRange(localFunctions);
			if (reduced is var (isStaticLocal, generatedParameters, generatedTypeParameters))
				function.ReducedMethod = new LocalFunctionMethod(function.Method!, name ?? function.Method!.Name, isStaticLocal, generatedParameters, generatedTypeParameters);
			return function;
		}

		ILInstruction Deconstruct()
		{
			var init = new StLoc[Len()];
			for (int i = 0; i < init.Length; i++)
				init[i] = (StLoc)Node();
			// The init collection renumbers the pattern, conversions and assignments, so it is
			// filled once they are set.
			var deconstruct = new DeconstructInstruction {
				Pattern = (MatchInstruction)Node(),
				Conversions = (Block)Node(),
				Assignments = (Block)Node()
			};
			deconstruct.Init.AddRange(init);
			return deconstruct;
		}

		ILInstruction DeconstructResult()
		{
			int index = Int();
			var type = Type();
			var resultType = (StackType)U8();
			return new DeconstructResultInstruction(index, type, resultType, Node());
		}

		ILInstruction ExpressionTreeCast()
		{
			var type = Type();
			bool isChecked = Bool();
			return new ExpressionTreeCast(type, Node(), isChecked);
		}

		ILInstruction NullableUnwrap()
		{
			var type = Type();
			var resultType = (StackType)U8();
			bool refInput = Bool();
			return new NullableUnwrap(type, resultType, Node(), refInput);
		}

		ILInstruction NullableRewrap()
		{
			var type = Type();
			return new NullableRewrap(Node(), type);
		}

		ILInstruction NullCoalescing()
		{
			var kind = (NullCoalescingKind)U8();
			var type = Type();
			var value = Node();
			return new NullCoalescingInstruction(type, kind, value, Node());
		}

		ILInstruction UserDefinedCompoundAssign()
		{
			var method = Method();
			var evalMode = (CompoundEvalMode)U8();
			var targetKind = (CompoundTargetKind)U8();
			var target = Node();
			return new UserDefinedCompoundAssign(method, evalMode, target, targetKind, Node());
		}

		/// <summary>
		/// The context the constructors of some instructions check their operands against (in
		/// Debug builds); its function is a placeholder.
		/// </summary>
		ILTransformContext TransformContext => transformContext ??= new ILTransformContext(
			new ILFunction(mainModule.GetDefinition(MetadataTokens.MethodDefinitionHandle(currentRow)), 0, context, new Nop()),
			(IDecompilerTypeSystem)compilation, null, settings);
		ILTransformContext? transformContext;

		ILInstruction Using()
		{
			var inst = new UsingInstruction(Variable(), Node(), Node());
			byte flags = U8();
			inst.IsAsync = (flags & 1) != 0;
			inst.IsRefStruct = (flags & 2) != 0;
			return inst;
		}

		ILInstruction Match()
		{
			var variable = Variable();
			byte flags = U8();
			IMethod? method = Bool() ? Method() : null;
			var testedOperand = Node();
			return new MatchInstruction(variable, method, testedOperand, Nodes()) {
				IsDeconstructCall = (flags & 1) != 0,
				IsDeconstructTuple = (flags & 2) != 0,
				CheckType = (flags & 4) != 0,
				CheckNotNull = (flags & 8) != 0,
			};
		}

		ILInstruction StringToInt()
		{
			var type = Type();
			var map = new List<(string?, int)>();
			for (int n = Len(); n > 0; n--)
				map.Add((Bool() ? String() : null, (int)IVar()));
			return new StringToInt(Node(), map, type);
		}

		ILInstruction Comp()
		{
			var kind = (ComparisonKind)U8();
			var lifting = (ComparisonLiftingKind)U8();
			var sign = (Sign)U8();
			var input = (StackType)U8();
			return new Comp(kind, lifting, input, sign, Node(), Node());
		}

		string String()
		{
			var chars = new char[Len()];
			var bytes = Raw(chars.Length * 2);
			for (int i = 0; i < chars.Length; i++)
				chars[i] = (char)(bytes[2 * i] | bytes[2 * i + 1] << 8);
			return new string(chars);
		}

		ILInstruction WithType(Func<ILInstruction, IType, ILInstruction> create)
		{
			var type = Type();
			return create(Node(), type);
		}

		BlockContainer Container()
		{
			int index = Len();
			var kind = (ContainerKind)U8();
			int expected = Len();
			var container = new BlockContainer(kind, expected == 0 ? null : TypeAt(expected - 1));
			containers[index] = container;
			int count = Len();
			for (int i = 0; i < count; i++)
				container.Blocks.Add((Block)Node());
			return container;
		}

		Block ReadBlock()
		{
			var block = blocks[Len()];
			block.Instructions.AddRange(Nodes());
			block.FinalInstruction = Node();
			return block;
		}

		ILInstruction Binary()
		{
			var op = (BinaryNumericOperator)U8();
			bool check = Bool();
			var sign = (Sign)U8();
			var leftType = (StackType)U8();
			var rightType = (StackType)U8();
			int csharpResultType = Len();
			bool lifted = Bool();
			return new BinaryNumericInstruction(op, Node(), Node(), leftType, rightType, check, sign, lifted) {
				CSharpResultType = csharpResultType == 0 ? null : TypeAt(csharpResultType - 1)
			};
		}

		ILInstruction Switch()
		{
			bool isLifted = Bool();
			int resultType = Len();
			int type = Len();
			var switchInst = new SwitchInstruction(Node()) { IsLifted = isLifted };
			if (resultType != 0)
				switchInst.SetResultType(TypeAt(resultType - 1));
			if (type != 0)
				switchInst.Type = TypeAt(type - 1);
			foreach (var section in Nodes())
				switchInst.Sections.Add((SwitchSection)section);
			return switchInst;
		}

		ILInstruction Section()
		{
			byte flags = U8();
			var intervals = new LongInterval[Len()];
			for (int i = 0; i < intervals.Length; i++)
				intervals[i] = new LongInterval(IVar(), IVar());
			return new SwitchSection {
				HasNullLabel = (flags & 1) != 0,
				IsCompilerGeneratedDefaultSection = (flags & 2) != 0,
				Labels = new LongSet((IEnumerable<LongInterval>)intervals),
				Body = Node()
			};
		}

		ILInstruction TryCatch()
		{
			var tryCatch = new TryCatch(Node());
			foreach (var handler in Nodes())
				tryCatch.Handlers.Add((TryCatchHandler)handler);
			return tryCatch;
		}

		ILInstruction Handler()
		{
			var variable = Variable();
			var specifier = new Interval((int)IVar(), (int)IVar());
			var handler = new TryCatchHandler(Node(), Node(), variable);
			if (!specifier.IsEmpty)
				handler.AddExceptionSpecifierILRange(specifier);
			return handler;
		}

		ILInstruction CallIndirect()
		{
			bool isInstance = Bool();
			bool hasExplicitThis = Bool();
			var signature = (FunctionPointerType)Type();
			var arguments = Nodes();
			return new CallIndirect(isInstance, hasExplicitThis, signature, Node(), arguments);
		}

		ILInstruction BitNot()
		{
			if (!Bool())
				return new BitNot(Node());
			var underlying = (StackType)U8();
			return new BitNot(Node(), isLifted: true, underlying);
		}

		/// <summary>The method of a call, lifted when the engine lifted a user-defined operator.</summary>
		IMethod CallMethod()
		{
			IMethod method;
			if (Bool())
			{
				// ProxyCallReplacer: the forwarded method, read in the proxy's generic context and
				// specialized by the proxy call's substitution.
				var proxy = CallMethod();
				var saved = context;
				context = new GenericContext(proxy);
				method = Method();
				context = saved;
				method = (IMethod)method.Specialize(proxy.Substitution);
			}
			else
			{
				method = Method();
			}
			if (Bool())
			{
				method = ICSharpCode.Decompiler.CSharp.Resolver.CSharpOperators.LiftUserDefinedOperator(method)
					?? throw new InvalidOperationException($"The operator {method} cannot be lifted");
			}
			if (Bool())
			{
				var indexOrRangeType = Type();
				method = new SyntheticRangeIndexAccessor(method, indexOrRangeType, slicing: Bool());
			}
			if (Bool())
			{
				// A local function use: the reduced method of the function read from this method.
				var definition = mainModule.GetDefinition(MetadataTokens.MethodDefinitionHandle(Len()));
				int contextIndex = Len();
				IMethod read = contextIndex == 0 ? definition : (IMethod)definition.Specialize(explicitContexts[contextIndex - 2].ToSubstitution());
				bool isStatic = Bool();
				int parameters = Len();
				int typeParameters = Len();
				string name = Bool() ? Str() : read.Name;
				method = new LocalFunctionMethod(read, name, isStatic, parameters, typeParameters).Specialize(method.Substitution);
			}
			return method;
		}

		ILInstruction Conv()
		{
			var input = (StackType)U8();
			var sign = (Sign)U8();
			var target = (PrimitiveType)U8();
			bool check = Bool();
			bool lifted = Bool();
			return new Conv(Node(), input, sign, target, check, lifted);
		}

		ILInstruction StLoc()
		{
			var variable = Variable();
			byte flags = U8();
			return new StLoc(variable, Node()) {
				ILStackWasEmpty = (flags & 1) != 0,
				IsStackAdjustment = (flags & 2) != 0,
			};
		}

		ILInstruction AddressOf()
		{
			var type = Type();
			return new AddressOf(Node(), type);
		}

		ILInstruction LdVirtFtn()
		{
			var method = CallMethod();
			return new LdVirtFtn(Node(), method);
		}

		ILInstruction Cpblk()
		{
			bool isVolatile = Bool();
			byte unaligned = U8();
			return new Cpblk(Node(), Node(), Node()) { IsVolatile = isVolatile, UnalignedPrefix = unaligned };
		}

		ILInstruction Initblk()
		{
			bool isVolatile = Bool();
			byte unaligned = U8();
			return new Initblk(Node(), Node(), Node()) { IsVolatile = isVolatile, UnalignedPrefix = unaligned };
		}

		ILInstruction LdFlda()
		{
			var field = Field();
			bool delay = Bool();
			return new LdFlda(Node(), field) { DelayExceptions = delay };
		}

		ILInstruction LdObj()
		{
			var type = Type();
			bool isVolatile = Bool();
			byte unaligned = U8();
			return new LdObj(Node(), type) { IsVolatile = isVolatile, UnalignedPrefix = unaligned };
		}

		ILInstruction StObj()
		{
			var type = Type();
			bool isVolatile = Bool();
			byte unaligned = U8();
			return new StObj(Node(), Node(), type) { IsVolatile = isVolatile, UnalignedPrefix = unaligned };
		}

		ILInstruction NewArr()
		{
			var type = Type();
			return new NewArr(type, Nodes());
		}

		ILInstruction LdLen()
		{
			var type = (StackType)U8();
			return new LdLen(type, Node());
		}

		ILInstruction LdElema()
		{
			var type = Type();
			bool delay = Bool();
			bool isReadOnly = Bool();
			bool withSystemIndex = Bool();
			var array = Node();
			return new LdElema(type, array, Nodes()) { DelayExceptions = delay, IsReadOnly = isReadOnly, WithSystemIndex = withSystemIndex };
		}
	}
}
