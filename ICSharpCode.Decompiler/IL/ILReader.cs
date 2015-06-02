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
			InitParameterVariables();
			this.localVariables = body.Variables.SelectArray(CreateILVariable);
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
			var ilVar = new ILVariable(VariableKind.Local, typeSystem.Resolve(v.VariableType), v.Index);
			if (string.IsNullOrEmpty(v.Name))
				ilVar.Name = "V_" + v.Index;
			else
				ilVar.Name = v.Name;
			return ilVar;
		}

		ILVariable CreateILVariable(ParameterDefinition p)
		{
			var variableKind = p.Index == -1 ? VariableKind.This : VariableKind.Parameter;
			var ilVar = new ILVariable(variableKind, typeSystem.Resolve(p.ParameterType), p.Index);
			ilVar.StoreCount = 1; // count the initial store when the method is called with an argument
			if (variableKind == VariableKind.This)
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
			Debug.Fail(message);
		}

		void MergeStacks(ImmutableStack<ILVariable> a, ImmutableStack<ILVariable> b)
		{
			Debug.Assert(a.Count() == b.Count());
			var enum1 = a.GetEnumerator();
			var enum2 = b.GetEnumerator();
			while (enum1.MoveNext() && enum2.MoveNext()) {
				unionFind.Merge(enum1.Current, enum2.Current);
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
					var v = new ILVariable(VariableKind.Exception, typeSystem.Resolve(eh.CatchType), eh.HandlerStart.Offset);
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
				decodedInstruction.CheckInvariant();
				decodedInstruction.ILRange = new Interval(start, reader.Position);
				UnpackPush(decodedInstruction).ILRange = decodedInstruction.ILRange;
				instructionBuilder.Add(decodedInstruction);
				if (decodedInstruction.HasFlag(InstructionFlags.EndPointUnreachable)) {
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
			var container = new BlockBuilder(body, typeSystem, variableByExceptionHandler).CreateBlocks(instructionBuilder, isBranchTarget);
			var function = new ILFunction(body.Method, container);
			function.Variables.AddRange(parameterVariables);
			function.Variables.AddRange(localVariables);
			function.Variables.AddRange(stackVariables);
			function.AddRef(); // mark the root node
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
				case StackType.I:
					return Push(new Sub(new LdcI4(0), Pop(), checkForOverflow: false, sign: Sign.None));
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
					return DecodeComparisonBranch(false, OpCode.Ceq, OpCode.Ceq, false);
				case ILOpCode.Beq_S:
					return DecodeComparisonBranch(true, OpCode.Ceq, OpCode.Ceq, false);
				case ILOpCode.Bge:
					return DecodeComparisonBranch(false, OpCode.Clt, OpCode.Clt_Un, true);
				case ILOpCode.Bge_S:
					return DecodeComparisonBranch(true, OpCode.Clt, OpCode.Clt_Un, true);
				case ILOpCode.Bge_Un:
					return DecodeComparisonBranch(false, OpCode.Clt_Un, OpCode.Clt, true);
				case ILOpCode.Bge_Un_S:
					return DecodeComparisonBranch(true, OpCode.Clt_Un, OpCode.Clt, true);
				case ILOpCode.Bgt:
					return DecodeComparisonBranch(false, OpCode.Cgt, OpCode.Cgt, false);
				case ILOpCode.Bgt_S:
					return DecodeComparisonBranch(true, OpCode.Cgt, OpCode.Cgt, false);
				case ILOpCode.Bgt_Un:
					return DecodeComparisonBranch(false, OpCode.Cgt_Un, OpCode.Clt_Un, false);
				case ILOpCode.Bgt_Un_S:
					return DecodeComparisonBranch(true, OpCode.Cgt_Un, OpCode.Clt_Un, false);
				case ILOpCode.Ble:
					return DecodeComparisonBranch(false, OpCode.Cgt, OpCode.Cgt_Un, true);
				case ILOpCode.Ble_S:
					return DecodeComparisonBranch(true, OpCode.Cgt, OpCode.Cgt_Un, true);
				case ILOpCode.Ble_Un:
					return DecodeComparisonBranch(false, OpCode.Cgt_Un, OpCode.Cgt, true);
				case ILOpCode.Ble_Un_S:
					return DecodeComparisonBranch(true, OpCode.Cgt_Un, OpCode.Cgt, true);
				case ILOpCode.Blt:
					return DecodeComparisonBranch(false, OpCode.Clt, OpCode.Clt, false);
				case ILOpCode.Blt_S:
					return DecodeComparisonBranch(true, OpCode.Clt, OpCode.Clt, false);
				case ILOpCode.Blt_Un:
					return DecodeComparisonBranch(false, OpCode.Clt_Un, OpCode.Clt_Un, false);
				case ILOpCode.Blt_Un_S:
					return DecodeComparisonBranch(true, OpCode.Clt_Un, OpCode.Clt_Un, false);
				case ILOpCode.Bne_Un:
					return DecodeComparisonBranch(false, OpCode.Ceq, OpCode.Ceq, true);
				case ILOpCode.Bne_Un_S:
					return DecodeComparisonBranch(true, OpCode.Ceq, OpCode.Ceq, true);
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
					return Push(Comparison(OpCode.Ceq, OpCode.Ceq));
				case ILOpCode.Cgt:
					return Push(Comparison(OpCode.Cgt, OpCode.Cgt));
				case ILOpCode.Cgt_Un:
					return Push(Comparison(OpCode.Cgt_Un, OpCode.Cgt_Un));
				case ILOpCode.Clt:
					return Push(Comparison(OpCode.Clt, OpCode.Clt));
				case ILOpCode.Clt_Un:
					return Push(Comparison(OpCode.Clt_Un, OpCode.Clt_Un));
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
					return Push(new LdObj(Pop(), compilation.FindType(KnownTypeCode.SByte)));
				case ILOpCode.Ldind_I2:
					return Push(new LdObj(Pop(), compilation.FindType(KnownTypeCode.Int16)));
				case ILOpCode.Ldind_I4:
					return Push(new LdObj(Pop(), compilation.FindType(KnownTypeCode.Int32)));
				case ILOpCode.Ldind_I8:
					return Push(new LdObj(Pop(), compilation.FindType(KnownTypeCode.Int64)));
				case ILOpCode.Ldind_U1:
					return Push(new LdObj(Pop(), compilation.FindType(KnownTypeCode.Byte)));
				case ILOpCode.Ldind_U2:
					return Push(new LdObj(Pop(), compilation.FindType(KnownTypeCode.UInt16)));
				case ILOpCode.Ldind_U4:
					return Push(new LdObj(Pop(), compilation.FindType(KnownTypeCode.UInt32)));
				case ILOpCode.Ldind_R4:
					return Push(new LdObj(Pop(), compilation.FindType(KnownTypeCode.Single)));
				case ILOpCode.Ldind_R8:
					return Push(new LdObj(Pop(), compilation.FindType(KnownTypeCode.Double)));
				case ILOpCode.Ldind_I:
					return Push(new LdObj(Pop(), compilation.FindType(KnownTypeCode.IntPtr)));
				case ILOpCode.Ldind_Ref:
					return Push(new LdObj(Pop(), compilation.FindType(KnownTypeCode.Object)));
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
					return new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.SByte));
				case ILOpCode.Stind_I2:
					return new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.Int16));
				case ILOpCode.Stind_I4:
					return new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.Int32));
				case ILOpCode.Stind_I8:
					return new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.Int64));
				case ILOpCode.Stind_R4:
					return new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.Single));
				case ILOpCode.Stind_R8:
					return new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.Double));
				case ILOpCode.Stind_I:
					return new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.IntPtr));
				case ILOpCode.Stind_Ref:
					return new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.Object));
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
					return Push(new Box(Pop(), ReadAndDecodeTypeReference()));
				case ILOpCode.Castclass:
					return Push(new CastClass(Pop(), ReadAndDecodeTypeReference()));
				case ILOpCode.Cpobj:
					{
						var type = ReadAndDecodeTypeReference();
						var ld = new LdObj(Pop(), type);
						return new StObj(Pop(), ld, type);
					}
				case ILOpCode.Initobj:
					return Push(InitObj(Pop(), ReadAndDecodeTypeReference()));
				case ILOpCode.Isinst:
					return Push(new IsInst(Pop(), ReadAndDecodeTypeReference()));
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
					return Push(new LdElema(index: Pop(), array: Pop(), type: ReadAndDecodeTypeReference()));
				case ILOpCode.Ldfld:
					return Push(new LdFld(Pop(), ReadAndDecodeFieldReference()));
				case ILOpCode.Ldflda:
					return Push(new LdFlda(Pop(), ReadAndDecodeFieldReference()));
				case ILOpCode.Stfld:
					return new StFld(value: Pop(), target: Pop(), field: ReadAndDecodeFieldReference());
				case ILOpCode.Ldlen:
					return Push(new LdLen(Pop()));
				case ILOpCode.Ldobj:
					return Push(new LdObj(Pop(), ReadAndDecodeTypeReference()));
				case ILOpCode.Ldsfld:
					return Push(new LdsFld(ReadAndDecodeFieldReference()));
				case ILOpCode.Ldsflda:
					return Push(new LdsFlda(ReadAndDecodeFieldReference()));
				case ILOpCode.Stsfld:
					return new StsFld(Pop(), ReadAndDecodeFieldReference());
				case ILOpCode.Ldtoken:
					return Push(LdToken(ReadAndDecodeMetadataToken()));
				case ILOpCode.Ldvirtftn:
					return Push(new LdVirtFtn(Pop(), ReadAndDecodeMethodReference()));
				case ILOpCode.Mkrefany:
					throw new NotImplementedException();
				case ILOpCode.Newarr:
					return Push(new NewArr(ReadAndDecodeTypeReference(), Pop()));
				case ILOpCode.Refanytype:
					throw new NotImplementedException();
				case ILOpCode.Refanyval:
					throw new NotImplementedException();
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
					return new StObj(value: Pop(), target: Pop(), type: ReadAndDecodeTypeReference());
				case ILOpCode.Throw:
					return new Throw(Pop());
				case ILOpCode.Unbox:
					return Push(new Unbox(Pop(), ReadAndDecodeTypeReference()));
				case ILOpCode.Unbox_Any:
					return Push(new UnboxAny(Pop(), ReadAndDecodeTypeReference()));
				default:
					throw new NotImplementedException(ilOpCode.ToString());
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
		
		LdLoc Peek()
		{
			// TODO: handle stack underflow?
			return new LdLoc(currentStack.Peek());
		}
		
		LdLoc Pop()
		{
			// TODO: handle stack underflow?
			ILVariable v;
			currentStack = currentStack.Pop(out v);
			return new LdLoc(v);
		}

		private ILInstruction Return()
		{
			if (body.Method.ReturnType.GetStackType() == StackType.Void)
				return new IL.Return();
			else
				return new IL.Return(Pop());
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
			return new StLoc(parameterVariables[v], Pop());
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
			return new StLoc(localVariables[v], Pop());
		}
		
		private ILInstruction LdElem(IType type)
		{
			return Push(new LdObj(new LdElema(index: Pop(), array: Pop(), type: type), type));
		}
		
		private ILInstruction StElem(IType type)
		{
			var value = Pop();
			var index = Pop();
			var array = Pop();
			return new StObj(new LdElema(array, index, type), value, type);
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
			var arguments = new ILInstruction[GetPopCount(opCode, method)];
			for (int i = arguments.Length - 1; i >= 0; i--) {
				arguments[i] = Pop();
			}
			var call = CallInstruction.Create(opCode, method);
			call.Arguments.AddRange(arguments);
			if (call.ResultType != StackType.Void)
				return Push(call);
			else
				return call;
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

		ILInstruction Comparison(OpCode opCode_I, OpCode opCode_F)
		{
			var right = Pop();
			var left = Pop();
			// Based on Table 4: Binary Comparison or Branch Operation
			OpCode opCode;
			if (left.ResultType == StackType.F && right.ResultType == StackType.F)
				opCode = opCode_F;
			else
				opCode = opCode_I;
			return BinaryComparisonInstruction.Create(opCode, left, right);
		}

		ILInstruction DecodeComparisonBranch(bool shortForm, OpCode comparisonOpCodeForInts, OpCode comparisonOpCodeForFloats, bool negate)
		{
			int start = reader.Position - 1;
			var condition = Comparison(comparisonOpCodeForInts, comparisonOpCodeForFloats);
			int target = shortForm ? reader.ReadSByte() : reader.ReadInt32();
			target += reader.Position;
			condition.ILRange = new Interval(start, reader.Position);
			if (negate) {
				condition = new LogicNot(condition);
			}
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
					condition = new Ceq(condition, new LdNull());
					negate = !negate;
					break;
				case StackType.I:
					// introduce explicit comparison with 0
					condition = new Ceq(condition, new LdcI4(0));
					negate = !negate;
					break;
				case StackType.I8:
					// introduce explicit comparison with 0
					condition = new Ceq(condition, new LdcI8(0));
					negate = !negate;
					break;
			}
			if (negate) {
				condition = new LogicNot(condition);
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
			var instr = new SwitchInstruction(Pop());
			
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
