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

using ICSharpCode.Decompiler.IL;
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
		readonly IType[] types;
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
			types = new IType[Len()];
			for (int i = 0; i < types.Length; i++)
				types[i] = ReadTypeEntry();
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

		public IType Type() => types[Len()];

		protected IType TypeAt(int index) => types[index];

		public byte[] Raw(int count)
		{
			var result = new byte[count];
			Array.Copy(data, pos, result, 0, count);
			pos += count;
			return result;
		}

		IType[] TypeList()
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
		public const uint Version = 1;

		readonly MetadataModule mainModule;
		GenericContext context;
		ILVariable[] variables = [];
		Block[] blocks = [];
		BlockContainer[] containers = [];

		IlAstMaterializer(byte[] data, ICompilation compilation, IReadOnlyList<MetadataModule> modules)
			: base(data, Version, compilation, modules)
		{
			mainModule = modules[0];
		}

		/// <summary>Per MethodDef row: the function, or the engine's error message.</summary>
		public static Dictionary<int, (ILFunction? Function, string? Error)> ReadModule(
			byte[] wire, ICompilation compilation, IReadOnlyList<MetadataModule> modules)
		{
			var reader = new IlAstMaterializer(wire, compilation, modules);
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

		ILFunction ReadFunction(int row)
		{
			var method = mainModule.GetDefinition(MetadataTokens.MethodDefinitionHandle(row));
			context = new GenericContext(method);
			int codeSize = Len();
			int localSignatureLength = Len();
			int asyncReturnType = Len();
			variables = new ILVariable[Len()];
			for (int i = 0; i < variables.Length; i++)
				variables[i] = ReadVariable();
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
			var body = Node();
			var function = new ILFunction(method, codeSize, context, body, ILFunctionKind.TopLevelFunction);
			function.Variables.AddRange(variables);
			function.LocalVariableSignatureLength = localSignatureLength;
			if (asyncReturnType != 0)
				function.AsyncReturnType = TypeAt(asyncReturnType - 1);
			function.AddRef();
			function.SequencePointCandidates = sequencePoints;
			function.Warnings.AddRange(warnings);
			return function;
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
			return v;
		}

		ILVariable Variable() => variables[Len()];

		IMethod Method() => mainModule.ResolveMethod(MetadataTokens.EntityHandle((int)Token()), context);

		IField Field()
			=> mainModule.ResolveEntity(MetadataTokens.EntityHandle((int)Token()), context) as IField
				?? throw new InvalidOperationException("Field token does not resolve to a field");

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

		ILInstruction Node()
		{
			byte op = U8();
			var range = new Interval(Int(), Int());
			ILInstruction inst = op switch {
				0 => new InvalidBranch(Str()) { ExpectedResultType = (StackType)U8() },
				1 => new InvalidExpression(Str()) { ExpectedResultType = (StackType)U8(), Severity = Str() },
				2 => new Nop { Kind = (NopKind)U8() },
				3 => Container(),
				4 => ReadBlock(),
				5 => Binary(),
				6 => new BitNot(Node()),
				7 => new Arglist(),
				8 => new Branch(blocks[Len()]),
				9 => new Leave(containers[Len()], Node()),
				10 => new IfInstruction(Node(), Node(), Node()),
				11 => Switch(),
				12 => Section(),
				13 => TryCatch(),
				14 => Handler(),
				15 => new TryFinally(Node(), Node()),
				16 => new TryFault(Node(), Node()),
				17 => new DebugBreak(),
				18 => Comp(),
				19 => Call(new Call(Method())),
				20 => Call(new CallVirt(Method())),
				21 => Call(new NewObj(Method())),
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
				35 => new LdFtn(Method()),
				36 => LdVirtFtn(),
				37 => new LdTypeToken(Type()),
				38 => new LdMemberToken(mainModule.ResolveEntity(MetadataTokens.EntityHandle((int)Token()), context) as IMember
					?? throw new InvalidOperationException("ldtoken operand is not a member")),
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
				_ => throw new InvalidOperationException($"Unknown wire opcode {op}"),
			};
			inst.SetILRange(range);
			return inst;
		}

		ILInstruction Comp()
		{
			var kind = (ComparisonKind)U8();
			var sign = (Sign)U8();
			var input = (StackType)U8();
			return new Comp(kind, ComparisonLiftingKind.None, input, sign, Node(), Node());
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
			return new BinaryNumericInstruction(op, Node(), Node(), leftType, rightType, check, sign);
		}

		ILInstruction Switch()
		{
			var switchInst = new SwitchInstruction(Node());
			foreach (var section in Nodes())
				switchInst.Sections.Add((SwitchSection)section);
			return switchInst;
		}

		ILInstruction Section()
		{
			var intervals = new LongInterval[Len()];
			for (int i = 0; i < intervals.Length; i++)
				intervals[i] = new LongInterval(IVar(), IVar());
			return new SwitchSection { Labels = new LongSet((IEnumerable<LongInterval>)intervals), Body = Node() };
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
			return new TryCatchHandler(Node(), Node(), variable);
		}

		ILInstruction CallIndirect()
		{
			bool isInstance = Bool();
			bool hasExplicitThis = Bool();
			var signature = (FunctionPointerType)Type();
			var arguments = Nodes();
			return new CallIndirect(isInstance, hasExplicitThis, signature, Node(), arguments);
		}

		ILInstruction Conv()
		{
			var input = (StackType)U8();
			var sign = (Sign)U8();
			var target = (PrimitiveType)U8();
			bool check = Bool();
			return new Conv(Node(), input, sign, target, check);
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
			var method = Method();
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
			var array = Node();
			return new LdElema(type, array, Nodes()) { DelayExceptions = delay, IsReadOnly = isReadOnly };
		}
	}
}
