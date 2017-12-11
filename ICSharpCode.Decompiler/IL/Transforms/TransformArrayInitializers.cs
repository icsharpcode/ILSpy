// Copyright (c) 2015 Siegfried Pammer
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
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transforms array initialization pattern of System.Runtime.CompilerServices.RuntimeHelpers.InitializeArray.
	/// For collection and object initializers see <see cref="TransformCollectionAndObjectInitializers"/>
	/// </summary>
	public class TransformArrayInitializers : IStatementTransform
	{
		StatementTransformContext context;
		
		void IStatementTransform.Run(Block block, int pos, StatementTransformContext context)
		{
			this.context = context;
			try {
				if (!DoTransform(block, pos))
					DoTransformMultiDim(block, pos);
			} finally {
				this.context = null;
			}
		}

		bool DoTransform(Block body, int pos)
		{
			if (pos >= body.Instructions.Count - 2)
				return false;
			ILInstruction inst = body.Instructions[pos];
			ILVariable v;
			ILInstruction newarrExpr;
			IType elementType;
			int[] arrayLength;
			if (inst.MatchStLoc(out v, out newarrExpr) && MatchNewArr(newarrExpr, out elementType, out arrayLength)) {
				ILInstruction[] values;
				int initArrayPos;
				if (ForwardScanInitializeArrayRuntimeHelper(body, pos + 1, v, elementType, arrayLength, out values, out initArrayPos)) {
					context.Step("ForwardScanInitializeArrayRuntimeHelper", inst);
					var tempStore = context.Function.RegisterVariable(VariableKind.InitializerTarget, v.Type);
					var block = BlockFromInitializer(tempStore, elementType, arrayLength, values);
					body.Instructions[pos] = new StLoc(v, block);
					body.Instructions.RemoveAt(initArrayPos);
					ILInlining.InlineIfPossible(body, pos, context);
					return true;
				}
				if (arrayLength.Length == 1) {
					int instructionsToRemove;
					if (HandleSimpleArrayInitializer(body, pos + 1, v, elementType, arrayLength[0], out values, out instructionsToRemove)) {
						context.Step("HandleSimpleArrayInitializer", inst);
						var block = new Block(BlockKind.ArrayInitializer);
						var tempStore = context.Function.RegisterVariable(VariableKind.InitializerTarget, v.Type);
						block.Instructions.Add(new StLoc(tempStore, new NewArr(elementType, arrayLength.Select(l => new LdcI4(l)).ToArray())));
						block.Instructions.AddRange(values.SelectWithIndex(
							(i, value) => {
								if (value == null)
									value = GetNullExpression(elementType);
								return StElem(new LdLoc(tempStore), new[] { new LdcI4(i) }, value, elementType);
							}
						));
						block.FinalInstruction = new LdLoc(tempStore);
						body.Instructions[pos] = new StLoc(v, block);
						body.Instructions.RemoveRange(pos + 1, instructionsToRemove);
						ILInlining.InlineIfPossible(body, pos, context);
						return true;
					}
					if (HandleJaggedArrayInitializer(body, pos + 1, v, elementType, arrayLength[0], out ILVariable finalStore, out values, out instructionsToRemove)) {
						context.Step("HandleJaggedArrayInitializer", inst);
						var block = new Block(BlockKind.ArrayInitializer);
						var tempStore = context.Function.RegisterVariable(VariableKind.InitializerTarget, v.Type);
						block.Instructions.Add(new StLoc(tempStore, new NewArr(elementType, arrayLength.Select(l => new LdcI4(l)).ToArray())));
						block.Instructions.AddRange(values.SelectWithIndex((i, value) => StElem(new LdLoc(tempStore), new[] { new LdcI4(i) }, value, elementType)));
						block.FinalInstruction = new LdLoc(tempStore);
						body.Instructions[pos] = new StLoc(finalStore, block);
						body.Instructions.RemoveRange(pos + 1, instructionsToRemove);
						ILInlining.InlineIfPossible(body, pos, context);
						return true;
					}
				}
				// Put in a limit so that we don't consume too much memory if the code allocates a huge array
				// and populates it extremely sparsly. However, 255 "null" elements in a row actually occur in the Mono C# compiler!
//				const int maxConsecutiveDefaultValueExpressions = 300;
//				var operands = new List<ILInstruction>();
//				int numberOfInstructionsToRemove = 0;
//				for (int j = pos + 1; j < body.Instructions.Count; j++) {
//					var nextExpr = body.Instructions[j] as Void;
//					int arrayPos;
//					if (nextExpr != null && nextExpr is a.IsStoreToArray() &&
//					    nextExpr.Arguments[0].Match(ILCode.Ldloc, out v3) &&
//					    v == v3 &&
//					    nextExpr.Arguments[1].Match(ILCode.Ldc_I4, out arrayPos) &&
//					    arrayPos >= operands.Count &&
//					    arrayPos <= operands.Count + maxConsecutiveDefaultValueExpressions &&
//					    !nextExpr.Arguments[2].ContainsReferenceTo(v3))
//					{
//						while (operands.Count < arrayPos)
//							operands.Add(new ILExpression(ILCode.DefaultValue, elementType));
//						operands.Add(nextExpr.Arguments[2]);
//						numberOfInstructionsToRemove++;
//					} else {
//						break;
//					}
//				}

			}
			return false;
		}

		ILInstruction GetNullExpression(IType elementType)
		{
			ITypeDefinition typeDef = elementType.GetEnumUnderlyingType().GetDefinition();
			if (typeDef == null)
				return new DefaultValue(elementType);
			switch (typeDef.KnownTypeCode) {
				case KnownTypeCode.Boolean:
				case KnownTypeCode.Char:
				case KnownTypeCode.SByte:
				case KnownTypeCode.Byte:
				case KnownTypeCode.Int16:
				case KnownTypeCode.UInt16:
				case KnownTypeCode.Int32:
				case KnownTypeCode.UInt32:
					return new LdcI4(0);
				case KnownTypeCode.Int64:
				case KnownTypeCode.UInt64:
					return new LdcI8(0);
				case KnownTypeCode.Single:
					return new LdcF4(0);
				case KnownTypeCode.Double:
					return new LdcF8(0);
				case KnownTypeCode.Decimal:
					return new LdcDecimal(0);
				case KnownTypeCode.Void:
					throw new ArgumentException("void is not a valid element type!");
				case KnownTypeCode.IntPtr:
				case KnownTypeCode.UIntPtr:
				default:
					return new DefaultValue(elementType);
			}
		}
		
		/// <summary>
		/// Handle simple case where RuntimeHelpers.InitializeArray is not used.
		/// </summary>
		bool HandleSimpleArrayInitializer(Block block, int pos, ILVariable store, IType elementType, int length, out ILInstruction[] values, out int instructionsToRemove)
		{
			instructionsToRemove = 0;
			values = null;
			values = new ILInstruction[length];
			int index = 0;
			int elementCount = 0;
			for (int i = pos; i < block.Instructions.Count; i++) {
				if (index >= length)
					break;
				if (!block.Instructions[i].MatchStObj(out ILInstruction target, out ILInstruction value, out IType type) || value.Descendants.OfType<IInstructionWithVariableOperand>().Any(inst => inst.Variable == store))
					break;
				var ldelem = target as LdElema;
				if (ldelem == null || !ldelem.Array.MatchLdLoc(store) || ldelem.Indices.Count != 1 || !ldelem.Indices[0].MatchLdcI4(out index))
					break;
				values[index] = value;
				index++;
				elementCount++;
				instructionsToRemove++;
			}
			if (pos + elementCount >= block.Instructions.Count)
				return false;
			return elementCount > 0;
		}

		bool HandleJaggedArrayInitializer(Block block, int pos, ILVariable store, IType elementType, int length, out ILVariable finalStore, out ILInstruction[] values, out int instructionsToRemove)
		{
			instructionsToRemove = 0;
			finalStore = null;
			values = new ILInstruction[length];
			
			ILInstruction initializer;
			IType type;
			for (int i = 0; i < length; i++) {
				// 1. Instruction: (optional) temporary copy of store
				bool hasTemporaryCopy = block.Instructions[pos].MatchStLoc(out ILVariable temp, out ILInstruction storeLoad) && storeLoad.MatchLdLoc(store);
				if (hasTemporaryCopy) {
					if (!MatchJaggedArrayStore(block, pos + 1, temp, i, out initializer, out type))
						return false;
				} else {
					if (!MatchJaggedArrayStore(block, pos, store, i, out initializer, out type))
						return false;
				}
				values[i] = initializer;
				int inc = hasTemporaryCopy ? 3 : 2;
				pos += inc;
				instructionsToRemove += inc;
			}
			ILInstruction array;
			// In case there is an extra copy of the store variable
			// Remove it and use its value instead.
			if (block.Instructions[pos].MatchStLoc(out finalStore, out array)) {
				instructionsToRemove++;
				return array.MatchLdLoc(store);
			}
			finalStore = store;
			return true;
		}

		bool MatchJaggedArrayStore(Block block, int pos, ILVariable store, int index, out ILInstruction initializer, out IType type)
		{
			initializer = null;
			type = null;
			// 3. Instruction: stobj(ldelema(ldloc temp, ldc.i4 0), ldloc tempArrayLoad)
			var finalInstruction = block.Instructions.ElementAtOrDefault(pos + 1);
			ILInstruction tempAccess, tempArrayLoad;
			ILVariable initializerStore;
			if (finalInstruction == null || !finalInstruction.MatchStObj(out tempAccess, out tempArrayLoad, out type) || !tempArrayLoad.MatchLdLoc(out initializerStore))
				return false;
			var elemLoad = tempAccess as LdElema;
			if (elemLoad == null || !elemLoad.Array.MatchLdLoc(store) || elemLoad.Indices.Count != 1 || !elemLoad.Indices[0].MatchLdcI4(index))
				return false;
			// 2. Instruction: stloc(temp) with block (array initializer)
			var nextInstruction = block.Instructions.ElementAtOrDefault(pos);
			return nextInstruction != null && nextInstruction.MatchStLoc(initializerStore, out initializer) && initializer.OpCode == OpCode.Block;
		}

		bool DoTransformMultiDim(Block body, int pos)
		{
			if (pos >= body.Instructions.Count - 2)
				return false;
			ILVariable v;
			ILInstruction newarrExpr;
			IType arrayType;
			int[] length;
			ILInstruction instr = body.Instructions[pos];
			if (instr.MatchStLoc(out v, out newarrExpr) && MatchNewArr(newarrExpr, out arrayType, out length)) {
				ILInstruction[] values;
				int initArrayPos;
				if (ForwardScanInitializeArrayRuntimeHelper(body, pos + 1, v, arrayType, length, out values, out initArrayPos)) {
					var block = BlockFromInitializer(v, arrayType, length, values);
					body.Instructions[pos].ReplaceWith(new StLoc(v, block));
					body.Instructions.RemoveAt(initArrayPos);
					ILInlining.InlineIfPossible(body, pos, context);
					return true;
				}
			}
			return false;
		}
		
		Block BlockFromInitializer(ILVariable v, IType elementType, int[] arrayLength, ILInstruction[] values)
		{
			var block = new Block(BlockKind.ArrayInitializer);
			block.Instructions.Add(new StLoc(v, new NewArr(elementType, arrayLength.Select(l => new LdcI4(l)).ToArray())));
			int step = arrayLength.Length + 1;
			for (int i = 0; i < values.Length / step; i++) {
				// values array is filled backwards
				var value = values[step * i];
				var indices = new List<ILInstruction>();
				for (int j = step - 1; j >= 1; j--) {
					indices.Add(values[step * i + j]);
				}
				block.Instructions.Add(StElem(new LdLoc(v), indices.ToArray(), value, elementType));
			}
			block.FinalInstruction = new LdLoc(v);
			return block;
		}
		
		static bool CompareTypes(IType a, IType b)
		{
			IType type1 = DummyTypeParameter.NormalizeAllTypeParameters(a);
			IType type2 = DummyTypeParameter.NormalizeAllTypeParameters(b);
			return type1.Equals(type2);
		}
		
		static bool CompareSignatures(IList<IParameter> parameters, IList<IParameter> otherParameters)
		{
			if (otherParameters.Count != parameters.Count)
				return false;
			for (int i = 0; i < otherParameters.Count; i++) {
				if (!CompareTypes(otherParameters[i].Type, parameters[i].Type))
					return false;
			}
			return true;
		}
		
		internal static bool MatchNewArr(ILInstruction instruction, out IType arrayType, out int[] length)
		{
			NewArr newArr = instruction as NewArr;
			length = null;
			arrayType = null;
			if (newArr == null)
				return false;
			arrayType = newArr.Type;
			var args = newArr.Indices;
			length = new int[args.Count];
			for (int i = 0; i < args.Count; i++) {
				int value;
				if (!args[i].MatchLdcI4(out value) || value <= 0) return false;
				length[i] = value;
			}
			return true;
		}
		
		bool MatchInitializeArrayCall(ILInstruction instruction, out IMethod method, out ILVariable array, out Mono.Cecil.FieldReference field)
		{
			method = null;
			array = null;
			field = null;
			Call call = instruction as Call;
			if (call == null || call.Arguments.Count != 2)
				return false;
			method = call.Method;
			if (method.DeclaringTypeDefinition == null || method.DeclaringTypeDefinition.FullName != "System.Runtime.CompilerServices.RuntimeHelpers")
				return false;
			if (method.Name != "InitializeArray")
				return false;
			if (!call.Arguments[0].MatchLdLoc(out array))
				return false;
			IMember member;
			if (!call.Arguments[1].MatchLdMemberToken(out member))
				return false;
			field = context.TypeSystem.GetCecil(member) as Mono.Cecil.FieldReference;
			if (field == null)
				return false;
			return true;
		}

		bool ForwardScanInitializeArrayRuntimeHelper(Block body, int pos, ILVariable array, IType arrayType, int[] arrayLength, out ILInstruction[] values, out int foundPos)
		{
			ILVariable v2;
			IMethod method;
			Mono.Cecil.FieldReference field;
			if (MatchInitializeArrayCall(body.Instructions[pos], out method, out v2, out field) && array == v2) {
				var fieldDef = field.ResolveWithinSameModule();
				if (fieldDef != null && fieldDef.InitialValue != null) {
					var valuesList = new List<ILInstruction>();
					if (DecodeArrayInitializer(arrayType, array, fieldDef.InitialValue, arrayLength, valuesList)) {
						values = valuesList.ToArray();
						foundPos = pos;
						return true;
					}
				}
			}
			values = null;
			foundPos = -1;
			return false;
		}

		static bool DecodeArrayInitializer(IType type, ILVariable array, byte[] initialValue, int[] arrayLength, List<ILInstruction> output)
		{
			TypeCode typeCode = ReflectionHelper.GetTypeCode(type);
			switch (typeCode) {
				case TypeCode.Boolean:
				case TypeCode.Byte:
					return DecodeArrayInitializer(initialValue, array, arrayLength, output, typeCode, type, (d, i) => new LdcI4((int)d[i]));
				case TypeCode.SByte:
					return DecodeArrayInitializer(initialValue, array, arrayLength, output, typeCode, type, (d, i) => new LdcI4((int)unchecked((sbyte)d[i])));
				case TypeCode.Int16:
					return DecodeArrayInitializer(initialValue, array, arrayLength, output, typeCode, type, (d, i) => new LdcI4((int)BitConverter.ToInt16(d, i)));
				case TypeCode.Char:
				case TypeCode.UInt16:
					return DecodeArrayInitializer(initialValue, array, arrayLength, output, typeCode, type, (d, i) => new LdcI4((int)BitConverter.ToUInt16(d, i)));
				case TypeCode.Int32:
				case TypeCode.UInt32:
					return DecodeArrayInitializer(initialValue, array, arrayLength, output, typeCode, type, (d, i) => new LdcI4(BitConverter.ToInt32(d, i)));
				case TypeCode.Int64:
				case TypeCode.UInt64:
					return DecodeArrayInitializer(initialValue, array, arrayLength, output, typeCode, type, (d, i) => new LdcI8(BitConverter.ToInt64(d, i)));
				case TypeCode.Single:
					return DecodeArrayInitializer(initialValue, array, arrayLength, output, typeCode, type, (d, i) => new LdcF4(BitConverter.ToSingle(d, i)));
				case TypeCode.Double:
					return DecodeArrayInitializer(initialValue, array, arrayLength, output, typeCode, type, (d, i) => new LdcF8(BitConverter.ToDouble(d, i)));
				case TypeCode.Object:
				case TypeCode.Empty:
					var typeDef = type.GetDefinition();
					if (typeDef != null && typeDef.Kind == TypeKind.Enum)
						return DecodeArrayInitializer(typeDef.EnumUnderlyingType, array, initialValue, arrayLength, output);
					return false;
				default:
					return false;
			}
		}

		static bool DecodeArrayInitializer(byte[] initialValue, ILVariable array, int[] arrayLength, List<ILInstruction> output, TypeCode elementType, IType type, Func<byte[], int, ILInstruction> decoder)
		{
			int elementSize = ElementSizeOf(elementType);
			var totalLength = arrayLength.Aggregate(1, (t, l) => t * l);
			if (initialValue.Length < (totalLength * elementSize))
				return false;

			for (int i = 0; i < totalLength; i++) {
				output.Add(decoder(initialValue, i * elementSize));
				int next = i;
				for (int j = arrayLength.Length - 1; j >= 0; j--) {
					output.Add(new LdcI4(next % arrayLength[j]));
					next = next / arrayLength[j];
				}
			}

			return true;
		}
		
		static ILInstruction StElem(ILInstruction array, ILInstruction[] indices, ILInstruction value, IType type)
		{
			if (type.GetStackType() != value.ResultType) {
				value = new Conv(value, type.ToPrimitiveType(), false, Sign.None); 
			}
			return new StObj(new LdElema(type, array, indices), value, type);
		}

		static int ElementSizeOf(TypeCode elementType)
		{
			switch (elementType) {
				case TypeCode.Boolean:
				case TypeCode.Byte:
				case TypeCode.SByte:
					return 1;
				case TypeCode.Char:
				case TypeCode.Int16:
				case TypeCode.UInt16:
					return 2;
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Single:
					return 4;
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Double:
					return 8;
				default:
					throw new ArgumentOutOfRangeException(nameof(elementType));
			}
		}
	}
}
