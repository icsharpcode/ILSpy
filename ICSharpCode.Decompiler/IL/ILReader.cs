// Copyright (c) 2014 Daniel Grunwald
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Diagnostics;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Cecil;
using Cil = Mono.Cecil.Cil;
using System.Collections;
using System.Threading;

namespace ICSharpCode.Decompiler.IL
{
	public class ILReader
	{
		internal static ILOpCode ReadOpCode(ref BlobReader reader)
		{
			byte b = reader.ReadByte();
			if (b == 0xfe)
				return (ILOpCode)(0x100 | reader.ReadByte());
			else
				return (ILOpCode)b;
		}

		internal static MetadataToken ReadMetadataToken(ref BlobReader reader)
		{
			return new MetadataToken(reader.ReadUInt32());
		}
		
		readonly ICompilation compilation;
		readonly IDecompilerTypeSystem typeSystem;

		public ILReader(IDecompilerTypeSystem typeSystem)
		{
			if (typeSystem == null)
				throw new ArgumentNullException("typeSystem");
			this.typeSystem = typeSystem;
			this.compilation = typeSystem.Compilation;
		}

		Cil.MethodBody body;
		StackType methodReturnStackType;
		BlobReader reader;
		ImmutableStack<ILVariable> currentStack;
		ILVariable[] parameterVariables;
		ILVariable[] localVariables;
		BitArray isBranchTarget;
		List<ILInstruction> instructionBuilder;

		// Dictionary that stores stacks for each IL instruction
		Dictionary<int, ImmutableStack<ILVariable>> stackByOffset;
		Dictionary<Cil.ExceptionHandler, ILVariable> variableByExceptionHandler;
		UnionFind<ILVariable> unionFind;
		IEnumerable<ILVariable> stackVariables;
		
		void Init(Cil.MethodBody body)
		{
			if (body == null)
				throw new ArgumentNullException("body");
			this.body = body;
			this.reader = body.GetILReader();
			this.currentStack = ImmutableStack<ILVariable>.Empty;
			this.unionFind = new UnionFind<ILVariable>();
			this.methodReturnStackType = typeSystem.Resolve(body.Method.ReturnType).GetStackType();
			InitParameterVariables();
			this.localVariables = body.Variables.SelectArray(CreateILVariable);
			if (body.InitLocals) {
				foreach (var v in localVariables) {
					v.HasInitialValue = true;
				}
			}
			this.instructionBuilder = new List<ILInstruction>();
			this.isBranchTarget = new BitArray(body.CodeSize);
			this.stackByOffset = new Dictionary<int, ImmutableStack<ILVariable>>();
			this.variableByExceptionHandler = new Dictionary<Cil.ExceptionHandler, ILVariable>();
		}

		IMetadataTokenProvider ReadAndDecodeMetadataToken()
		{
			var token = ReadMetadataToken(ref reader);
			return body.LookupToken(token);
		}

		IType ReadAndDecodeTypeReference()
		{
			var token = ReadMetadataToken(ref reader);
			var typeReference = body.LookupToken(token) as TypeReference;
			return typeSystem.Resolve(typeReference);
		}

		IMethod ReadAndDecodeMethodReference()
		{
			var token = ReadMetadataToken(ref reader);
			var methodReference = body.LookupToken(token) as MethodReference;
			return typeSystem.Resolve(methodReference);
		}

		IField ReadAndDecodeFieldReference()
		{
			var token = ReadMetadataToken(ref reader);
			var fieldReference = body.LookupToken(token) as FieldReference;
			return typeSystem.Resolve(fieldReference);
		}

		void InitParameterVariables()
		{
			parameterVariables = new ILVariable[GetPopCount(OpCode.Call, body.Method)];
			int paramIndex = 0;
			if (body.Method.HasThis)
				parameterVariables[paramIndex++] = CreateILVariable(body.ThisParameter);
			foreach (var p in body.Method.Parameters)
				parameterVariables[paramIndex++] = CreateILVariable(p);
			Debug.Assert(paramIndex == parameterVariables.Length);
		}

		ILVariable CreateILVariable(Cil.VariableDefinition v)
		{
			VariableKind kind = v.IsPinned ? VariableKind.PinnedLocal : VariableKind.Local;
			ILVariable ilVar = new ILVariable(kind, typeSystem.Resolve(v.VariableType), v.Index);
			if (string.IsNullOrEmpty(v.Name))
				ilVar.Name = "V_" + v.Index;
			else
				ilVar.Name = v.Name;
			return ilVar;
		}

		ILVariable CreateILVariable(ParameterDefinition p)
		{
			IType parameterType;
			if (p.Index == -1) {
				// Manually construct ctor parameter type due to Cecil bug:
				// https://github.com/jbevain/cecil/issues/275
				ITypeDefinition def = typeSystem.Resolve(body.Method.DeclaringType).GetDefinition();
				if (def != null && def.TypeParameterCount > 0) {
					parameterType = new ParameterizedType(def, def.TypeArguments);
					if (def.IsReferenceType == false) {
						parameterType = new NRefactory.TypeSystem.ByReferenceType(parameterType);
					}
				} else {
					parameterType = typeSystem.Resolve(p.ParameterType);
				}
			} else {
				parameterType = typeSystem.Resolve(p.ParameterType);
			}
			Debug.Assert(!parameterType.IsUnbound());
			if (parameterType.IsUnbound()) {
				// parameter types should not be unbound, the only known cause for these is a Cecil bug:
				Debug.Assert(p.Index < 0); // cecil bug occurs only for "this"
				parameterType = new ParameterizedType(parameterType.GetDefinition(), parameterType.TypeArguments);
			}
			var ilVar = new ILVariable(VariableKind.Parameter, parameterType, p.Index);
			Debug.Assert(ilVar.StoreCount == 1); // count the initial store when the method is called with an argument
			if (p.Index < 0)
				ilVar.Name = "this";
			else if (string.IsNullOrEmpty(p.Name))
				ilVar.Name = "P_" + p.Index;
			else
				ilVar.Name = p.Name;
			return ilVar;
		}
		
		/// <summary>
		/// Warn when invalid IL is detected.
		/// ILSpy should be able to handle invalid IL; but this method can be helpful for debugging the ILReader,
		/// as this method should not get called when processing valid IL.
		/// </summary>
		void Warn(string message)
		{
			Debug.Fail(string.Format("IL_{0:x4}: {1}", reader.Position, message));
		}

		void MergeStacks(ImmutableStack<ILVariable> a, ImmutableStack<ILVariable> b)
		{
			var enum1 = a.GetEnumerator();
			var enum2 = b.GetEnumerator();
			bool ok1 = enum1.MoveNext();
			bool ok2 = enum2.MoveNext();
			while (ok1 && ok2) {
				if (enum1.Current.StackType != enum2.Current.StackType) {
					Warn("Incompatible stack types: " + enum1.Current.StackType + " vs " + enum2.Current.StackType);
				}
				unionFind.Merge(enum1.Current, enum2.Current);
				ok1 = enum1.MoveNext();
				ok2 = enum2.MoveNext();
			}
			if (ok1 || ok2) {
				Warn("Incompatible stack heights: " + a.Count() + " vs " + b.Count());
			}
		}

		void StoreStackForOffset(int offset, ImmutableStack<ILVariable> stack)
		{
			ImmutableStack<ILVariable> existing;
			if (stackByOffset.TryGetValue(offset, out existing)) {
				MergeStacks(existing, stack);
			} else {
				stackByOffset.Add(offset, stack);
			}
		}

		void ReadInstructions(CancellationToken cancellationToken)
		{
			// Fill isBranchTarget and branchStackDict based on exception handlers
			foreach (var eh in body.ExceptionHandlers) {
				ImmutableStack<ILVariable> ehStack = null;
				if (eh.HandlerType == Cil.ExceptionHandlerType.Catch || eh.HandlerType == Cil.ExceptionHandlerType.Filter) {
					var v = new ILVariable(VariableKind.Exception, typeSystem.Resolve(eh.CatchType), eh.HandlerStart.Offset) {
						Name = "E_" + eh.HandlerStart.Offset
					};
					variableByExceptionHandler.Add(eh, v);
					ehStack = ImmutableStack.Create(v);
				} else {
					ehStack = ImmutableStack<ILVariable>.Empty;
				}
				if (eh.FilterStart != null) {
					isBranchTarget[eh.FilterStart.Offset] = true;
					StoreStackForOffset(eh.FilterStart.Offset, ehStack);
				}
				if (eh.HandlerStart != null) {
					isBranchTarget[eh.HandlerStart.Offset] = true;
					StoreStackForOffset(eh.HandlerStart.Offset, ehStack);
				}
			}
			
			while (reader.Position < reader.Length) {
				cancellationToken.ThrowIfCancellationRequested();
				int start = reader.Position;
				StoreStackForOffset(start, currentStack);
				ILInstruction decodedInstruction = DecodeInstruction();
				if (decodedInstruction.ResultType == StackType.Unknown)
					Warn("Unknown result type (might be due to invalid IL)");
				decodedInstruction.CheckInvariant(ILPhase.InILReader);
				decodedInstruction.ILRange = new Interval(start, reader.Position);
				UnpackPush(decodedInstruction).ILRange = decodedInstruction.ILRange;
				instructionBuilder.Add(decodedInstruction);
				if (decodedInstruction.HasDirectFlag(InstructionFlags.EndPointUnreachable)) {
					if (!stackByOffset.TryGetValue(reader.Position, out currentStack)) {
						currentStack = ImmutableStack<ILVariable>.Empty;
					}
				}
			}
			
			var visitor = new CollectStackVariablesVisitor(unionFind);
			for (int i = 0; i < instructionBuilder.Count; i++) {
				instructionBuilder[i] = instructionBuilder[i].AcceptVisitor(visitor);
			}
			stackVariables = visitor.variables;
		}

		/// <summary>
		/// Debugging helper: writes the decoded instruction stream interleaved with the inferred evaluation stack layout.
		/// </summary>
		public void WriteTypedIL(Cil.MethodBody body, ITextOutput output, CancellationToken cancellationToken = default(CancellationToken))
		{
			Init(body);
			ReadInstructions(cancellationToken);
			foreach (var inst in instructionBuilder) {
				output.Write("   [");
				bool isFirstElement = true;
				foreach (var element in stackByOffset[inst.ILRange.Start]) {
					if (isFirstElement)
						isFirstElement = false;
					else
						output.Write(", ");
					output.WriteReference(element.Name, element, isLocal: true);
					output.Write(":");
					output.Write(element.StackType);
				}
				output.Write(']');
				output.WriteLine();
				if (isBranchTarget[inst.ILRange.Start])
					output.Write('*');
				else
					output.Write(' ');
				output.WriteDefinition("IL_" + inst.ILRange.Start.ToString("x4"), inst.ILRange.Start);
				output.Write(": ");
				inst.WriteTo(output);
				output.WriteLine();
			}
			new Disassembler.MethodBodyDisassembler(output, false, cancellationToken).WriteExceptionHandlers(body);
		}

		/// <summary>
		/// Decodes the specified method body and returns an ILFunction.
		/// </summary>
		public ILFunction ReadIL(Cil.MethodBody body, CancellationToken cancellationToken = default(CancellationToken))
		{
			Init(body);
			ReadInstructions(cancellationToken);
			var blockBuilder = new BlockBuilder(body, typeSystem, variableByExceptionHandler);
			var container = blockBuilder.CreateBlocks(instructionBuilder, isBranchTarget);
			var function = new ILFunction(body.Method, container);
			function.Variables.AddRange(parameterVariables);
			function.Variables.AddRange(localVariables);
			function.Variables.AddRange(stackVariables);
			function.Variables.AddRange(variableByExceptionHandler.Values);
			function.AddRef(); // mark the root node
			foreach (var c in function.Descendants.OfType<BlockContainer>()) {
				c.SortBlocks();
			}
			return function;
		}

		static ILInstruction UnpackPush(ILInstruction inst)
		{
			ILVariable v;
			ILInstruction inner;
			if (inst.MatchStLoc(out v, out inner) && v.Kind == VariableKind.StackSlot)
				return inner;
			else
				return inst;
		}
		
		ILInstruction Neg()
		{
			switch (PeekStackType()) {
				case StackType.I4:
					return Push(new Sub(new LdcI4(0), Pop(), checkForOverflow: false, sign: Sign.None));
				case StackType.I:
					return Push(new Sub(new Conv(new LdcI4(0), PrimitiveType.I, false, Sign.None), Pop(), checkForOverflow: false, sign: Sign.None));
				case StackType.I8:
					return Push(new Sub(new LdcI8(0), Pop(), checkForOverflow: false, sign: Sign.None));
				case StackType.F:
					return Push(new Sub(new LdcF(0), Pop(), checkForOverflow: false, sign: Sign.None));
				default:
					Warn("Unsupported input type for neg.");
					goto case StackType.I4;
			}
		}
		
		ILInstruction DecodeInstruction()
		{
			var ilOpCode = ReadOpCode(ref reader);
			switch (ilOpCode) {
				case ILOpCode.Constrained:
					return DecodeConstrainedCall();
				case ILOpCode.Readonly:
					return DecodeReadonly();
				case ILOpCode.Tailcall:
					return DecodeTailCall();
				case ILOpCode.Unaligned:
					return DecodeUnaligned();
				case ILOpCode.Volatile:
					return DecodeVolatile();
				case ILOpCode.Add:
					return BinaryNumeric(OpCode.Add);
				case ILOpCode.Add_Ovf:
					return BinaryNumeric(OpCode.Add, true, Sign.Signed);
				case ILOpCode.Add_Ovf_Un:
					return BinaryNumeric(OpCode.Add, true, Sign.Unsigned);
				case ILOpCode.And:
					return BinaryNumeric(OpCode.BitAnd);
				case ILOpCode.Arglist:
					return Push(new Arglist());
				case ILOpCode.Beq:
					return DecodeComparisonBranch(false, ComparisonKind.Equality);
				case ILOpCode.Beq_S:
					return DecodeComparisonBranch(true, ComparisonKind.Equality);
				case ILOpCode.Bge:
					return DecodeComparisonBranch(false, ComparisonKind.GreaterThanOrEqual);
				case ILOpCode.Bge_S:
					return DecodeComparisonBranch(true, ComparisonKind.GreaterThanOrEqual);
				case ILOpCode.Bge_Un:
					return DecodeComparisonBranch(false, ComparisonKind.GreaterThanOrEqual, un: true);
				case ILOpCode.Bge_Un_S:
					return DecodeComparisonBranch(true, ComparisonKind.GreaterThanOrEqual, un: true);
				case ILOpCode.Bgt:
					return DecodeComparisonBranch(false, ComparisonKind.GreaterThan);
				case ILOpCode.Bgt_S:
					return DecodeComparisonBranch(true, ComparisonKind.GreaterThan);
				case ILOpCode.Bgt_Un:
					return DecodeComparisonBranch(false, ComparisonKind.GreaterThan, un: true);
				case ILOpCode.Bgt_Un_S:
					return DecodeComparisonBranch(true, ComparisonKind.GreaterThan, un: true);
				case ILOpCode.Ble:
					return DecodeComparisonBranch(false, ComparisonKind.LessThanOrEqual);
				case ILOpCode.Ble_S:
					return DecodeComparisonBranch(true, ComparisonKind.LessThanOrEqual);
				case ILOpCode.Ble_Un:
					return DecodeComparisonBranch(false, ComparisonKind.LessThanOrEqual, un: true);
				case ILOpCode.Ble_Un_S:
					return DecodeComparisonBranch(true, ComparisonKind.LessThanOrEqual, un: true);
				case ILOpCode.Blt:
					return DecodeComparisonBranch(false, ComparisonKind.LessThan);
				case ILOpCode.Blt_S:
					return DecodeComparisonBranch(true, ComparisonKind.LessThan);
				case ILOpCode.Blt_Un:
					return DecodeComparisonBranch(false, ComparisonKind.LessThan, un: true);
				case ILOpCode.Blt_Un_S:
					return DecodeComparisonBranch(true, ComparisonKind.LessThan, un: true);
				case ILOpCode.Bne_Un:
					return DecodeComparisonBranch(false, ComparisonKind.Inequality, un: true);
				case ILOpCode.Bne_Un_S:
					return DecodeComparisonBranch(true, ComparisonKind.Inequality, un: true);
				case ILOpCode.Br:
					return DecodeUnconditionalBranch(false);
				case ILOpCode.Br_S:
					return DecodeUnconditionalBranch(true);
				case ILOpCode.Break:
					return new DebugBreak();
				case ILOpCode.Brfalse:
					return DecodeConditionalBranch(false, true);
				case ILOpCode.Brfalse_S:
					return DecodeConditionalBranch(true, true);
				case ILOpCode.Brtrue:
					return DecodeConditionalBranch(false, false);
				case ILOpCode.Brtrue_S:
					return DecodeConditionalBranch(true, false);
				case ILOpCode.Call:
					return DecodeCall(OpCode.Call);
				case ILOpCode.Callvirt:
					return DecodeCall(OpCode.CallVirt);
				case ILOpCode.Calli:
					throw new NotImplementedException();
				case ILOpCode.Ceq:
					return Push(Comparison(ComparisonKind.Equality));
				case ILOpCode.Cgt:
					return Push(Comparison(ComparisonKind.GreaterThan));
				case ILOpCode.Cgt_Un:
					return Push(Comparison(ComparisonKind.GreaterThan, un: true));
				case ILOpCode.Clt:
					return Push(Comparison(ComparisonKind.LessThan));
				case ILOpCode.Clt_Un:
					return Push(Comparison(ComparisonKind.LessThan, un: true));
				case ILOpCode.Ckfinite:
					return new Ckfinite(Peek());
				case ILOpCode.Conv_I1:
					return Push(new Conv(Pop(), PrimitiveType.I1, false, Sign.None));
				case ILOpCode.Conv_I2:
					return Push(new Conv(Pop(), PrimitiveType.I2, false, Sign.None));
				case ILOpCode.Conv_I4:
					return Push(new Conv(Pop(), PrimitiveType.I4, false, Sign.None));
				case ILOpCode.Conv_I8:
					return Push(new Conv(Pop(), PrimitiveType.I8, false, Sign.None));
				case ILOpCode.Conv_R4:
					return Push(new Conv(Pop(), PrimitiveType.R4, false, Sign.Signed));
				case ILOpCode.Conv_R8:
					return Push(new Conv(Pop(), PrimitiveType.R8, false, Sign.Signed));
				case ILOpCode.Conv_U1:
					return Push(new Conv(Pop(), PrimitiveType.U1, false, Sign.None));
				case ILOpCode.Conv_U2:
					return Push(new Conv(Pop(), PrimitiveType.U2, false, Sign.None));
				case ILOpCode.Conv_U4:
					return Push(new Conv(Pop(), PrimitiveType.U4, false, Sign.None));
				case ILOpCode.Conv_U8:
					return Push(new Conv(Pop(), PrimitiveType.U8, false, Sign.None));
				case ILOpCode.Conv_I:
					return Push(new Conv(Pop(), PrimitiveType.I, false, Sign.None));
				case ILOpCode.Conv_U:
					return Push(new Conv(Pop(), PrimitiveType.U, false, Sign.None));
				case ILOpCode.Conv_R_Un:
					return Push(new Conv(Pop(), PrimitiveType.R8, false, Sign.Unsigned));
				case ILOpCode.Conv_Ovf_I1:
					return Push(new Conv(Pop(), PrimitiveType.I1, true, Sign.Signed));
				case ILOpCode.Conv_Ovf_I2:
					return Push(new Conv(Pop(), PrimitiveType.I2, true, Sign.Signed));
				case ILOpCode.Conv_Ovf_I4:
					return Push(new Conv(Pop(), PrimitiveType.I4, true, Sign.Signed));
				case ILOpCode.Conv_Ovf_I8:
					return Push(new Conv(Pop(), PrimitiveType.I8, true, Sign.Signed));
				case ILOpCode.Conv_Ovf_U1:
					return Push(new Conv(Pop(), PrimitiveType.U1, true, Sign.Signed));
				case ILOpCode.Conv_Ovf_U2:
					return Push(new Conv(Pop(), PrimitiveType.U2, true, Sign.Signed));
				case ILOpCode.Conv_Ovf_U4:
					return Push(new Conv(Pop(), PrimitiveType.U4, true, Sign.Signed));
				case ILOpCode.Conv_Ovf_U8:
					return Push(new Conv(Pop(), PrimitiveType.U8, true, Sign.Signed));
				case ILOpCode.Conv_Ovf_I:
					return Push(new Conv(Pop(), PrimitiveType.I, true, Sign.Signed));
				case ILOpCode.Conv_Ovf_U:
					return Push(new Conv(Pop(), PrimitiveType.U, true, Sign.Signed));
				case ILOpCode.Conv_Ovf_I1_Un:
					return Push(new Conv(Pop(), PrimitiveType.I1, true, Sign.Unsigned));
				case ILOpCode.Conv_Ovf_I2_Un:
					return Push(new Conv(Pop(), PrimitiveType.I2, true, Sign.Unsigned));
				case ILOpCode.Conv_Ovf_I4_Un:
					return Push(new Conv(Pop(), PrimitiveType.I4, true, Sign.Unsigned));
				case ILOpCode.Conv_Ovf_I8_Un:
					return Push(new Conv(Pop(), PrimitiveType.I8, true, Sign.Unsigned));
				case ILOpCode.Conv_Ovf_U1_Un:
					return Push(new Conv(Pop(), PrimitiveType.U1, true, Sign.Unsigned));
				case ILOpCode.Conv_Ovf_U2_Un:
					return Push(new Conv(Pop(), PrimitiveType.U2, true, Sign.Unsigned));
				case ILOpCode.Conv_Ovf_U4_Un:
					return Push(new Conv(Pop(), PrimitiveType.U4, true, Sign.Unsigned));
				case ILOpCode.Conv_Ovf_U8_Un:
					return Push(new Conv(Pop(), PrimitiveType.U8, true, Sign.Unsigned));
				case ILOpCode.Conv_Ovf_I_Un:
					return Push(new Conv(Pop(), PrimitiveType.I, true, Sign.Unsigned));
				case ILOpCode.Conv_Ovf_U_Un:
					return Push(new Conv(Pop(), PrimitiveType.U, true, Sign.Unsigned));
				case ILOpCode.Cpblk:
					throw new NotImplementedException();
				case ILOpCode.Div:
					return BinaryNumeric(OpCode.Div, false, Sign.Signed);
				case ILOpCode.Div_Un:
					return BinaryNumeric(OpCode.Div, false, Sign.Unsigned);
				case ILOpCode.Dup:
					return Push(Peek());
				case ILOpCode.Endfilter:
				case ILOpCode.Endfinally:
					return new Leave(null);
				case ILOpCode.Initblk:
					throw new NotImplementedException();
				case ILOpCode.Jmp:
					throw new NotImplementedException();
				case ILOpCode.Ldarg:
					return Push(Ldarg(reader.ReadUInt16()));
				case ILOpCode.Ldarg_S:
					return Push(Ldarg(reader.ReadByte()));
				case ILOpCode.Ldarg_0:
				case ILOpCode.Ldarg_1:
				case ILOpCode.Ldarg_2:
				case ILOpCode.Ldarg_3:
					return Push(Ldarg(ilOpCode - ILOpCode.Ldarg_0));
				case ILOpCode.Ldarga:
					return Push(Ldarga(reader.ReadUInt16()));
				case ILOpCode.Ldarga_S:
					return Push(Ldarga(reader.ReadByte()));
				case ILOpCode.Ldc_I4:
					return Push(new LdcI4(reader.ReadInt32()));
				case ILOpCode.Ldc_I8:
					return Push(new LdcI8(reader.ReadInt64()));
				case ILOpCode.Ldc_R4:
					return Push(new LdcF(reader.ReadSingle()));
				case ILOpCode.Ldc_R8:
					return Push(new LdcF(reader.ReadDouble()));
				case ILOpCode.Ldc_I4_M1:
				case ILOpCode.Ldc_I4_0:
				case ILOpCode.Ldc_I4_1:
				case ILOpCode.Ldc_I4_2:
				case ILOpCode.Ldc_I4_3:
				case ILOpCode.Ldc_I4_4:
				case ILOpCode.Ldc_I4_5:
				case ILOpCode.Ldc_I4_6:
				case ILOpCode.Ldc_I4_7:
				case ILOpCode.Ldc_I4_8:
					return Push(new LdcI4((int)ilOpCode - (int)ILOpCode.Ldc_I4_0));
				case ILOpCode.Ldc_I4_S:
					return Push(new LdcI4(reader.ReadSByte()));
				case ILOpCode.Ldnull:
					return Push(new LdNull());
				case ILOpCode.Ldstr:
					return Push(DecodeLdstr());
				case ILOpCode.Ldftn:
					return Push(new LdFtn(ReadAndDecodeMethodReference()));
				case ILOpCode.Ldind_I1:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.SByte)));
				case ILOpCode.Ldind_I2:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Int16)));
				case ILOpCode.Ldind_I4:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Int32)));
				case ILOpCode.Ldind_I8:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Int64)));
				case ILOpCode.Ldind_U1:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Byte)));
				case ILOpCode.Ldind_U2:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.UInt16)));
				case ILOpCode.Ldind_U4:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.UInt32)));
				case ILOpCode.Ldind_R4:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Single)));
				case ILOpCode.Ldind_R8:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Double)));
				case ILOpCode.Ldind_I:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.IntPtr)));
				case ILOpCode.Ldind_Ref:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Object)));
				case ILOpCode.Ldloc:
					return Push(Ldloc(reader.ReadUInt16()));
				case ILOpCode.Ldloc_S:
					return Push(Ldloc(reader.ReadByte()));
				case ILOpCode.Ldloc_0:
				case ILOpCode.Ldloc_1:
				case ILOpCode.Ldloc_2:
				case ILOpCode.Ldloc_3:
					return Push(Ldloc(ilOpCode - ILOpCode.Ldloc_0));
				case ILOpCode.Ldloca:
					return Push(Ldloca(reader.ReadUInt16()));
				case ILOpCode.Ldloca_S:
					return Push(Ldloca(reader.ReadByte()));
				case ILOpCode.Leave:
					return DecodeUnconditionalBranch(false, isLeave: true);
				case ILOpCode.Leave_S:
					return DecodeUnconditionalBranch(true, isLeave: true);
				case ILOpCode.Localloc:
					return Push(new LocAlloc(Pop()));
				case ILOpCode.Mul:
					return BinaryNumeric(OpCode.Mul, false, Sign.None);
				case ILOpCode.Mul_Ovf:
					return BinaryNumeric(OpCode.Mul, true, Sign.Signed);
				case ILOpCode.Mul_Ovf_Un:
					return BinaryNumeric(OpCode.Mul, true, Sign.Unsigned);
				case ILOpCode.Neg:
					return Neg();
				case ILOpCode.Newobj:
					return DecodeCall(OpCode.NewObj);
				case ILOpCode.Nop:
					return new Nop();
				case ILOpCode.Not:
					return Push(new BitNot(Pop()));
				case ILOpCode.Or:
					return BinaryNumeric(OpCode.BitOr);
				case ILOpCode.Pop:
					Pop();
					return new Nop();
				case ILOpCode.Rem:
					return BinaryNumeric(OpCode.Rem, false, Sign.Signed);
				case ILOpCode.Rem_Un:
					return BinaryNumeric(OpCode.Rem, false, Sign.Unsigned);
				case ILOpCode.Ret:
					return Return();
				case ILOpCode.Shl:
					return BinaryNumeric(OpCode.Shl, false, Sign.None);
				case ILOpCode.Shr:
					return BinaryNumeric(OpCode.Shr, false, Sign.Signed);
				case ILOpCode.Shr_Un:
					return BinaryNumeric(OpCode.Shr, false, Sign.Unsigned);
				case ILOpCode.Starg:
					return Starg(reader.ReadUInt16());
				case ILOpCode.Starg_S:
					return Starg(reader.ReadByte());
				case ILOpCode.Stind_I1:
					return new StObj(value: Pop(StackType.I4), target: PopPointer(), type: compilation.FindType(KnownTypeCode.SByte));
				case ILOpCode.Stind_I2:
					return new StObj(value: Pop(StackType.I4), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Int16));
				case ILOpCode.Stind_I4:
					return new StObj(value: Pop(StackType.I4), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Int32));
				case ILOpCode.Stind_I8:
					return new StObj(value: Pop(StackType.I8), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Int64));
				case ILOpCode.Stind_R4:
					return new StObj(value: Pop(StackType.F), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Single));
				case ILOpCode.Stind_R8:
					return new StObj(value: Pop(StackType.F), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Double));
				case ILOpCode.Stind_I:
					return new StObj(value: Pop(StackType.I), target: PopPointer(), type: compilation.FindType(KnownTypeCode.IntPtr));
				case ILOpCode.Stind_Ref:
					return new StObj(value: Pop(StackType.O), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Object));
				case ILOpCode.Stloc:
					return Stloc(reader.ReadUInt16());
				case ILOpCode.Stloc_S:
					return Stloc(reader.ReadByte());
				case ILOpCode.Stloc_0:
				case ILOpCode.Stloc_1:
				case ILOpCode.Stloc_2:
				case ILOpCode.Stloc_3:
					return Stloc(ilOpCode - ILOpCode.Stloc_0);
				case ILOpCode.Sub:
					return BinaryNumeric(OpCode.Sub, false, Sign.None);
				case ILOpCode.Sub_Ovf:
					return BinaryNumeric(OpCode.Sub, true, Sign.Signed);
				case ILOpCode.Sub_Ovf_Un:
					return BinaryNumeric(OpCode.Sub, true, Sign.Unsigned);
				case ILOpCode.Switch:
					return DecodeSwitch();
				case ILOpCode.Xor:
					return BinaryNumeric(OpCode.BitXor);
				case ILOpCode.Box:
					{
						var type = ReadAndDecodeTypeReference();
						return Push(new Box(Pop(type.GetStackType()), type));
					}
				case ILOpCode.Castclass:
					return Push(new CastClass(Pop(StackType.O), ReadAndDecodeTypeReference()));
				case ILOpCode.Cpobj:
					{
						var type = ReadAndDecodeTypeReference();
						var ld = new LdObj(PopPointer(), type);
						return new StObj(PopPointer(), ld, type);
					}
				case ILOpCode.Initobj:
					return InitObj(PopPointer(), ReadAndDecodeTypeReference());
				case ILOpCode.Isinst:
					return Push(new IsInst(Pop(StackType.O), ReadAndDecodeTypeReference()));
				case ILOpCode.Ldelem:
					return LdElem(ReadAndDecodeTypeReference());
				case ILOpCode.Ldelem_I1:
					return LdElem(compilation.FindType(KnownTypeCode.SByte));
				case ILOpCode.Ldelem_I2:
					return LdElem(compilation.FindType(KnownTypeCode.Int16));
				case ILOpCode.Ldelem_I4:
					return LdElem(compilation.FindType(KnownTypeCode.Int32));
				case ILOpCode.Ldelem_I8:
					return LdElem(compilation.FindType(KnownTypeCode.Int64));
				case ILOpCode.Ldelem_U1:
					return LdElem(compilation.FindType(KnownTypeCode.Byte));
				case ILOpCode.Ldelem_U2:
					return LdElem(compilation.FindType(KnownTypeCode.UInt16));
				case ILOpCode.Ldelem_U4:
					return LdElem(compilation.FindType(KnownTypeCode.UInt32));
				case ILOpCode.Ldelem_R4:
					return LdElem(compilation.FindType(KnownTypeCode.Single));
				case ILOpCode.Ldelem_R8:
					return LdElem(compilation.FindType(KnownTypeCode.Double));
				case ILOpCode.Ldelem_I:
					return LdElem(compilation.FindType(KnownTypeCode.IntPtr));
				case ILOpCode.Ldelem_Ref:
					return LdElem(compilation.FindType(KnownTypeCode.Object));
				case ILOpCode.Ldelema:
					return Push(new LdElema(indices: Pop(), array: Pop(), type: ReadAndDecodeTypeReference()));
				case ILOpCode.Ldfld:
					return Push(new LdFld(Pop(), ReadAndDecodeFieldReference()));
				case ILOpCode.Ldflda:
					return Push(new LdFlda(Pop(), ReadAndDecodeFieldReference()));
				case ILOpCode.Stfld:
					{
						var field = ReadAndDecodeFieldReference();
						return new StFld(value: Pop(field.Type.GetStackType()), target: Pop(), field: field);
					}
				case ILOpCode.Ldlen:
					return Push(new LdLen(StackType.I, Pop()));
				case ILOpCode.Ldobj:
					return Push(new LdObj(PopPointer(), ReadAndDecodeTypeReference()));
				case ILOpCode.Ldsfld:
					return Push(new LdsFld(ReadAndDecodeFieldReference()));
				case ILOpCode.Ldsflda:
					return Push(new LdsFlda(ReadAndDecodeFieldReference()));
				case ILOpCode.Stsfld:
					{
						var field = ReadAndDecodeFieldReference();
						return new StsFld(Pop(field.Type.GetStackType()), field);
					}
				case ILOpCode.Ldtoken:
					return Push(LdToken(ReadAndDecodeMetadataToken()));
				case ILOpCode.Ldvirtftn:
					return Push(new LdVirtFtn(Pop(), ReadAndDecodeMethodReference()));
				case ILOpCode.Mkrefany:
					return Push(new MakeRefAny(PopPointer(), ReadAndDecodeTypeReference()));
				case ILOpCode.Newarr:
					return Push(new NewArr(ReadAndDecodeTypeReference(), Pop()));
				case ILOpCode.Refanytype:
					return Push(new RefAnyType(Pop()));
				case ILOpCode.Refanyval:
					return Push(new RefAnyValue(Pop(), ReadAndDecodeTypeReference()));
				case ILOpCode.Rethrow:
					return new Rethrow();
				case ILOpCode.Sizeof:
					return Push(new SizeOf(ReadAndDecodeTypeReference()));
				case ILOpCode.Stelem:
					return StElem(ReadAndDecodeTypeReference());
				case ILOpCode.Stelem_I1:
					return StElem(compilation.FindType(KnownTypeCode.SByte));
				case ILOpCode.Stelem_I2:
					return StElem(compilation.FindType(KnownTypeCode.Int16));
				case ILOpCode.Stelem_I4:
					return StElem(compilation.FindType(KnownTypeCode.Int32));
				case ILOpCode.Stelem_I8:
					return StElem(compilation.FindType(KnownTypeCode.Int64));
				case ILOpCode.Stelem_R4:
					return StElem(compilation.FindType(KnownTypeCode.Single));
				case ILOpCode.Stelem_R8:
					return StElem(compilation.FindType(KnownTypeCode.Double));
				case ILOpCode.Stelem_I:
					return StElem(compilation.FindType(KnownTypeCode.IntPtr));
				case ILOpCode.Stelem_Ref:
					return StElem(compilation.FindType(KnownTypeCode.Object));
				case ILOpCode.Stobj:
					{
						var type = ReadAndDecodeTypeReference();
						return new StObj(value: Pop(type.GetStackType()), target: PopPointer(), type: type);
					}
				case ILOpCode.Throw:
					return new Throw(Pop());
				case ILOpCode.Unbox:
					return Push(new Unbox(Pop(), ReadAndDecodeTypeReference()));
				case ILOpCode.Unbox_Any:
					return Push(new UnboxAny(Pop(), ReadAndDecodeTypeReference()));
				default:
					return new InvalidInstruction("Unknown opcode: " + ilOpCode.ToString());
			}
		}

		
		StackType PeekStackType()
		{
			if (currentStack.IsEmpty)
				return StackType.Unknown;
			else
				return currentStack.Peek().StackType;
		}

		class CollectStackVariablesVisitor : ILVisitor<ILInstruction>
		{
			readonly UnionFind<ILVariable> unionFind;
			internal readonly HashSet<ILVariable> variables = new HashSet<ILVariable>();

			public CollectStackVariablesVisitor(UnionFind<ILVariable> unionFind)
			{
				Debug.Assert(unionFind != null);
				this.unionFind = unionFind;
			}

			protected override ILInstruction Default(ILInstruction inst)
			{
				foreach (var child in inst.Children) {
					var newChild = child.AcceptVisitor(this);
					if (newChild != child)
						child.ReplaceWith(newChild);
				}
				return inst;
			}

			protected internal override ILInstruction VisitLdLoc(LdLoc inst)
			{
				base.VisitLdLoc(inst);
				if (inst.Variable.Kind == VariableKind.StackSlot) {
					var variable = unionFind.Find(inst.Variable);
					if (variables.Add(variable))
						variable.Name = "S_" + (variables.Count - 1);
					return new LdLoc(variable) { ILRange = inst.ILRange };
				}
				return inst;
			}

			protected internal override ILInstruction VisitStLoc(StLoc inst)
			{
				base.VisitStLoc(inst);
				if (inst.Variable.Kind == VariableKind.StackSlot) {
					var variable = unionFind.Find(inst.Variable);
					if (variables.Add(variable))
						variable.Name = "S_" + (variables.Count - 1);
					return new StLoc(variable, inst.Value) { ILRange = inst.ILRange };
				}
				return inst;
			}
		}
		
		ILInstruction Push(ILInstruction inst)
		{
			Debug.Assert(inst.ResultType != StackType.Void);
			IType type = compilation.FindType(inst.ResultType.ToKnownTypeCode());
			var v = new ILVariable(VariableKind.StackSlot, type, inst.ResultType, inst.ILRange.Start);
			v.Name = "S_" + inst.ILRange.Start.ToString("x4");
			currentStack = currentStack.Push(v);
			return new StLoc(v, inst);
		}
		
		ILInstruction Peek()
		{
			if (currentStack.IsEmpty) {
				return new InvalidInstruction("Stack underflow") { ILRange = new Interval(reader.Position, reader.Position) };
			}
			return new LdLoc(currentStack.Peek());
		}
		
		ILInstruction Pop()
		{
			if (currentStack.IsEmpty) {
				return new InvalidInstruction("Stack underflow") { ILRange = new Interval(reader.Position, reader.Position) };
			}
			ILVariable v;
			currentStack = currentStack.Pop(out v);
			return new LdLoc(v);
		}

		ILInstruction Pop(StackType expectedType)
		{
			ILInstruction inst = Pop();
			if (expectedType != inst.ResultType) {
				if (expectedType == StackType.I && inst.ResultType == StackType.I4) {
					inst = new Conv(inst, PrimitiveType.I, false, Sign.None);
				} else if (expectedType == StackType.Ref && inst.ResultType == StackType.I) {
					// implicitly start GC tracking
				} else if (inst is InvalidInstruction) {
					((InvalidInstruction)inst).ExpectedResultType = expectedType;
				} else {
					Warn($"Expected {expectedType}, but got {inst.ResultType}");
				}
			}
			return inst;
		}
		
		ILInstruction PopPointer()
		{
			ILInstruction inst = Pop();
			switch (inst.ResultType) {
				case StackType.I4:
					return new Conv(inst, PrimitiveType.I, false, Sign.None);
				case StackType.I:
				case StackType.Ref:
				case StackType.Unknown:
					return inst;
				default:
					Warn("Expected native int or pointer, but got " + inst.ResultType);
					return inst;
			}
		}
		
		private ILInstruction Return()
		{
			if (methodReturnStackType == StackType.Void)
				return new IL.Return();
			else
				return new IL.Return(Pop(methodReturnStackType));
		}

		private ILInstruction DecodeLdstr()
		{
			var metadataToken = ReadMetadataToken(ref reader);
			return new LdStr(body.LookupStringToken(metadataToken));
		}

		private ILInstruction Ldarg(ushort v)
		{
			return new LdLoc(parameterVariables[v]);
		}

		private ILInstruction Ldarga(ushort v)
		{
			return new LdLoca(parameterVariables[v]);
		}

		private ILInstruction Starg(ushort v)
		{
			return new StLoc(parameterVariables[v], Pop(parameterVariables[v].StackType));
		}

		private ILInstruction Ldloc(ushort v)
		{
			return new LdLoc(localVariables[v]);
		}

		private ILInstruction Ldloca(ushort v)
		{
			return new LdLoca(localVariables[v]);
		}

		private ILInstruction Stloc(ushort v)
		{
			return new StLoc(localVariables[v], Pop(localVariables[v].StackType));
		}
		
		private ILInstruction LdElem(IType type)
		{
			return Push(new LdObj(new LdElema(indices: Pop(), array: Pop(), type: type) { DelayExceptions = true }, type));
		}
		
		private ILInstruction StElem(IType type)
		{
			var value = Pop(type.GetStackType());
			var index = Pop();
			var array = Pop();
			return new StObj(new LdElema(type, array, index) { DelayExceptions = true }, value, type);
		}

		ILInstruction InitObj(ILInstruction target, IType type)
		{
			return new StObj(target, new DefaultValue(type), type);
		}
		
		private ILInstruction DecodeConstrainedCall()
		{
			var typeRef = ReadAndDecodeTypeReference();
			var inst = DecodeInstruction();
			var call = UnpackPush(inst) as CallInstruction;
			if (call != null)
				call.ConstrainedTo = typeRef;
			else
				Warn("Ignored invalid 'constrained' prefix");
			return inst;
		}

		private ILInstruction DecodeTailCall()
		{
			var inst = DecodeInstruction();
			var call = UnpackPush(inst) as CallInstruction;
			if (call != null)
				call.IsTail = true;
			else
				Warn("Ignored invalid 'tail' prefix");
			return inst;
		}

		private ILInstruction DecodeUnaligned()
		{
			byte alignment = reader.ReadByte();
			var inst = DecodeInstruction();
			var sup = UnpackPush(inst) as ISupportsUnalignedPrefix;
			if (sup != null)
				sup.UnalignedPrefix = alignment;
			else
				Warn("Ignored invalid 'unaligned' prefix");
			return inst;
		}

		private ILInstruction DecodeVolatile()
		{
			var inst = DecodeInstruction();
			var svp = UnpackPush(inst) as ISupportsVolatilePrefix;
			if (svp != null)
				svp.IsVolatile = true;
			else
				Warn("Ignored invalid 'volatile' prefix");
			return inst;
		}

		private ILInstruction DecodeReadonly()
		{
			var inst = DecodeInstruction();
			var ldelema = UnpackPush(inst) as LdElema;
			if (ldelema != null)
				ldelema.IsReadOnly = true;
			else
				Warn("Ignored invalid 'readonly' prefix");
			return inst;
		}

		ILInstruction DecodeCall(OpCode opCode)
		{
			var method = ReadAndDecodeMethodReference();
			int firstArgument = (opCode != OpCode.NewObj && !method.IsStatic) ? 1 : 0;
			var arguments = new ILInstruction[firstArgument + method.Parameters.Count];
			for (int i = method.Parameters.Count - 1; i >= 0; i--) {
				arguments[firstArgument + i] = Pop(method.Parameters[i].Type.GetStackType());
			}
			if (firstArgument == 1) {
				arguments[0] = Pop();
			}
			switch (method.DeclaringType.Kind) {
				case TypeKind.Array:
					var elementType = ((ICSharpCode.NRefactory.TypeSystem.ArrayType)method.DeclaringType).ElementType;
					if (opCode == OpCode.NewObj)
						return Push(new NewArr(elementType, arguments));
					if (method.Name == "Set") {
						var target = arguments[0];
						var value = arguments.Last();
						var indices = arguments.Skip(1).Take(arguments.Length - 2).ToArray();
						return new StObj(new LdElema(elementType, target, indices), value, elementType);
					}
					if (method.Name == "Get") {
						var target = arguments[0];
						var indices = arguments.Skip(1).ToArray();
						return Push(new LdObj(new LdElema(elementType, target, indices), elementType));
					}
					if (method.Name == "Address") {
						var target = arguments[0];
						var indices = arguments.Skip(1).ToArray();
						return Push(new LdElema(elementType, target, indices));
					}
					Warn("Unknown method called on array type: " + method.Name);
					goto default;
				default:
					var call = CallInstruction.Create(opCode, method);
					call.Arguments.AddRange(arguments);
					if (call.ResultType != StackType.Void)
						return Push(call);
					return call;
			}
		}
		
		static int GetPopCount(OpCode callCode, MethodReference methodReference)
		{
			int popCount = methodReference.Parameters.Count;
			if (callCode != OpCode.NewObj && methodReference.HasThis)
				popCount++;
			return popCount;
		}

		static int GetPopCount(OpCode callCode, IMethod method)
		{
			int popCount = method.Parameters.Count;
			if (callCode != OpCode.NewObj && !method.IsStatic)
				popCount++;
			return popCount;
		}

		ILInstruction Comparison(ComparisonKind kind, bool un = false)
		{
			var right = Pop();
			var left = Pop();
			// make the implicit I4->I conversion explicit:
			if (left.ResultType == StackType.I4 && right.ResultType == StackType.I) {
				left = new Conv(left, PrimitiveType.I, false, Sign.None);
			} else if (left.ResultType == StackType.I && right.ResultType == StackType.I4) {
				right = new Conv(right, PrimitiveType.I, false, Sign.None);
			}
			
			// Based on Table 4: Binary Comparison or Branch Operation
			if (left.ResultType == StackType.F && right.ResultType == StackType.F) {
				if (un) {
					// for floats, 'un' means 'unordered'
					return new LogicNot(new Comp(kind.Negate(), Sign.None, left, right));
				} else {
					return new Comp(kind, Sign.None, left, right);
				}
			} else if (left.ResultType.IsIntegerType() && !kind.IsEqualityOrInequality()) {
				// integer comparison where the sign matters
				Debug.Assert(right.ResultType.IsIntegerType());
				return new Comp(kind, un ? Sign.Unsigned : Sign.Signed, left, right);
			} else {
				// integer equality, object reference or managed reference comparison
				return new Comp(kind, Sign.None, left, right);
			}
		}

		ILInstruction DecodeComparisonBranch(bool shortForm, ComparisonKind kind, bool un = false)
		{
			int start = reader.Position - 1;
			var condition = Comparison(kind, un);
			int target = shortForm ? reader.ReadSByte() : reader.ReadInt32();
			target += reader.Position;
			condition.ILRange = new Interval(start, reader.Position);
			MarkBranchTarget(target);
			return new IfInstruction(condition, new Branch(target));
		}

		ILInstruction DecodeConditionalBranch(bool shortForm, bool negate)
		{
			int target = shortForm ? reader.ReadSByte() : reader.ReadInt32();
			target += reader.Position;
			ILInstruction condition = Pop();
			switch (condition.ResultType) {
				case StackType.O:
					// introduce explicit comparison with null
					condition = new Comp(
						negate ? ComparisonKind.Equality : ComparisonKind.Inequality,
						Sign.None, condition, new LdNull());
					break;
				case StackType.I:
					// introduce explicit comparison with 0
					condition = new Comp(
						negate ? ComparisonKind.Equality : ComparisonKind.Inequality,
						Sign.None, condition, new Conv(new LdcI4(0), PrimitiveType.I, false, Sign.None));
					break;
				case StackType.I8:
					// introduce explicit comparison with 0
					condition = new Comp(
						negate ? ComparisonKind.Equality : ComparisonKind.Inequality,
						Sign.None, condition, new LdcI8(0));
					break;
				default:
					if (negate) {
						condition = new LogicNot(condition);
					}
					break;
			}
			MarkBranchTarget(target);
			return new IfInstruction(condition, new Branch(target));
		}

		ILInstruction DecodeUnconditionalBranch(bool shortForm, bool isLeave = false)
		{
			int target = shortForm ? reader.ReadSByte() : reader.ReadInt32();
			target += reader.Position;
			if (isLeave) {
				currentStack = currentStack.Clear();
			}
			MarkBranchTarget(target);
			return new Branch(target);
		}

		void MarkBranchTarget(int targetILOffset)
		{
			isBranchTarget[targetILOffset] = true;
			StoreStackForOffset(targetILOffset, currentStack);
		}

		ILInstruction DecodeSwitch()
		{
			uint length = reader.ReadUInt32();
			int baseOffset = 4 * (int)length + reader.Position;
			var instr = new SwitchInstruction(Pop(StackType.I4));
			
			for (uint i = 0; i < length; i++) {
				var section = new SwitchSection();
				section.Labels = new LongSet(i);
				int target = baseOffset + reader.ReadInt32();
				MarkBranchTarget(target);
				section.Body = new Branch(target);
				instr.Sections.Add(section);
			}
			
			return instr;
		}
		
		ILInstruction BinaryNumeric(OpCode opCode, bool checkForOverflow = false, Sign sign = Sign.None)
		{
			var right = Pop();
			var left = Pop();
			if (opCode != OpCode.Shl && opCode != OpCode.Shr) {
				// make the implicit I4->I conversion explicit:
				if (left.ResultType == StackType.I4 && right.ResultType == StackType.I) {
					left = new Conv(left, PrimitiveType.I, false, Sign.None);
				} else if (left.ResultType == StackType.I && right.ResultType == StackType.I4) {
					right = new Conv(right, PrimitiveType.I, false, Sign.None);
				}
			}
			return Push(BinaryNumericInstruction.Create(opCode, left, right, checkForOverflow, sign));
		}

		ILInstruction LdToken(IMetadataTokenProvider token)
		{
			if (token is TypeReference)
				return new LdTypeToken(typeSystem.Resolve((TypeReference)token));
			if (token is FieldReference)
				return new LdMemberToken(typeSystem.Resolve((FieldReference)token));
			if (token is MethodReference)
				return new LdMemberToken(typeSystem.Resolve((MethodReference)token));
			throw new NotImplementedException();
		}
	}
}
