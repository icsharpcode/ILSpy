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
using System.Reflection.Metadata;
using System.Text;

using ICSharpCode.Decompiler.TypeSystem;
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
			if (!context.Settings.ArrayInitializers)
				return;
			this.context = context;
			try
			{
				if (DoTransform(context.Function, block, pos))
					return;
				if (DoTransformMultiDim(context.Function, block, pos))
					return;
				if (context.Settings.StackAllocInitializers && DoTransformStackAllocInitializer(block, pos))
					return;
				if (DoTransformInlineRuntimeHelpersInitializeArray(block, pos))
					return;
			}
			finally
			{
				this.context = null;
			}
		}

		bool DoTransform(ILFunction function, Block body, int pos)
		{
			if (pos >= body.Instructions.Count - 2)
				return false;
			ILInstruction inst = body.Instructions[pos];
			if (inst.MatchStLoc(out var v, out var newarrExpr) && MatchNewArr(newarrExpr, out var elementType, out var arrayLength))
			{
				if (HandleRuntimeHelpersInitializeArray(body, pos + 1, v, elementType, arrayLength, out var values, out var initArrayPos))
				{
					context.Step("HandleRuntimeHelperInitializeArray: single-dim", inst);
					var tempStore = context.Function.RegisterVariable(VariableKind.InitializerTarget, v.Type);
					var block = BlockFromInitializer(tempStore, elementType, arrayLength, values);
					body.Instructions[pos] = new StLoc(v, block);
					body.Instructions.RemoveAt(initArrayPos);
					ILInlining.InlineIfPossible(body, pos, context);
					return true;
				}
				if (arrayLength.Length == 1)
				{
					if (HandleSimpleArrayInitializer(function, body, pos + 1, v, arrayLength, out var arrayValues, out var instructionsToRemove))
					{
						context.Step("HandleSimpleArrayInitializer: single-dim", inst);
						var block = new Block(BlockKind.ArrayInitializer);
						var tempStore = context.Function.RegisterVariable(VariableKind.InitializerTarget, v.Type);
						block.Instructions.Add(new StLoc(tempStore, new NewArr(elementType, arrayLength.Select(l => new LdcI4(l)).ToArray())));
						block.Instructions.AddRange(arrayValues.Select(
							t => {
								var (indices, value) = t;
								if (value == null)
									value = GetNullExpression(elementType);
								return StElem(new LdLoc(tempStore), indices, value, elementType);
							}
						));
						block.FinalInstruction = new LdLoc(tempStore);
						body.Instructions[pos] = new StLoc(v, block);
						body.Instructions.RemoveRange(pos + 1, instructionsToRemove);
						ILInlining.InlineIfPossible(body, pos, context);
						return true;
					}
					if (HandleJaggedArrayInitializer(body, pos + 1, v, elementType, arrayLength[0], out ILVariable finalStore, out values, out instructionsToRemove))
					{
						context.Step("HandleJaggedArrayInitializer: single-dim", inst);
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
			}
			return false;
		}

		internal static bool TransformSpanTArrayInitialization(NewObj inst, StatementTransformContext context, out ILInstruction replacement)
		{
			replacement = null;
			if (!context.Settings.ArrayInitializers)
				return false;
			if (MatchSpanTCtorWithPointerAndSize(inst, context, out var elementType, out var field, out var size))
			{
				if (field.HasFlag(System.Reflection.FieldAttributes.HasFieldRVA))
				{
					var valuesList = new List<ILInstruction>();
					var initialValue = field.GetInitialValue(context.PEFile, context.TypeSystem);
					if (context.Settings.Utf8StringLiterals &&
						elementType.IsKnownType(KnownTypeCode.Byte) &&
						DecodeUTF8String(initialValue, size, out string text))
					{
						replacement = new LdStrUtf8(text);
						return true;
					}
					if (DecodeArrayInitializer(elementType, initialValue, new[] { size }, valuesList))
					{
						var tempStore = context.Function.RegisterVariable(VariableKind.InitializerTarget, new ArrayType(context.TypeSystem, elementType));
						replacement = BlockFromInitializer(tempStore, elementType, new[] { size }, valuesList.ToArray());
						return true;
					}
				}
			}
			return false;
		}

		private static unsafe bool DecodeUTF8String(BlobReader blob, int size, out string text)
		{
			if (size > blob.RemainingBytes)
			{
				text = null;
				return false;
			}
			for (int i = 0; i < size; i++)
			{
				byte val = blob.CurrentPointer[i];
				// If the string has control characters, it's probably binary data and not a string.
				if (val < 0x20 && val is not ((byte)'\r' or (byte)'\n' or (byte)'\t'))
				{
					text = null;
					return false;
				}
			}
			text = Encoding.UTF8.GetString(blob.CurrentPointer, size);
			// Only use UTF8 string literal if we can perfectly roundtrip the data
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return MemoryExtensions.SequenceEqual(bytes, new ReadOnlySpan<byte>(blob.CurrentPointer, size));
		}

		static bool MatchSpanTCtorWithPointerAndSize(NewObj newObj, StatementTransformContext context, out IType elementType, out FieldDefinition field, out int size)
		{
			field = default;
			size = default;
			elementType = null;
			IType type = newObj.Method.DeclaringType;
			if (!type.IsKnownType(KnownTypeCode.ReadOnlySpanOfT))
				return false;
			if (newObj.Arguments.Count != 2 || type.TypeArguments.Count != 1)
				return false;
			elementType = type.TypeArguments[0];
			if (!newObj.Arguments[0].UnwrapConv(ConversionKind.StopGCTracking).MatchLdsFlda(out var member))
				return false;
			if (member.MetadataToken.IsNil)
				return false;
			if (!newObj.Arguments[1].MatchLdcI4(out size))
				return false;
			field = context.PEFile.Metadata.GetFieldDefinition((FieldDefinitionHandle)member.MetadataToken);
			return true;
		}

		bool DoTransformMultiDim(ILFunction function, Block body, int pos)
		{
			if (pos >= body.Instructions.Count - 2)
				return false;
			ILInstruction inst = body.Instructions[pos];
			if (inst.MatchStLoc(out var v, out var newarrExpr) && MatchNewArr(newarrExpr, out var elementType, out var length))
			{
				if (HandleRuntimeHelpersInitializeArray(body, pos + 1, v, elementType, length, out var values, out var initArrayPos))
				{
					context.Step("HandleRuntimeHelpersInitializeArray: multi-dim", inst);
					var block = BlockFromInitializer(v, elementType, length, values);
					body.Instructions[pos].ReplaceWith(new StLoc(v, block));
					body.Instructions.RemoveAt(initArrayPos);
					ILInlining.InlineIfPossible(body, pos, context);
					return true;
				}
				if (HandleSimpleArrayInitializer(function, body, pos + 1, v, length, out var arrayValues, out var instructionsToRemove))
				{
					context.Step("HandleSimpleArrayInitializer: multi-dim", inst);
					var block = new Block(BlockKind.ArrayInitializer);
					var tempStore = context.Function.RegisterVariable(VariableKind.InitializerTarget, v.Type);
					block.Instructions.Add(new StLoc(tempStore, new NewArr(elementType, length.Select(l => new LdcI4(l)).ToArray())));
					block.Instructions.AddRange(arrayValues.Select(
						t => {
							var (indices, value) = t;
							if (value == null)
								value = GetNullExpression(elementType);
							return StElem(new LdLoc(tempStore), indices, value, elementType);
						}
					));
					block.FinalInstruction = new LdLoc(tempStore);
					body.Instructions[pos] = new StLoc(v, block);
					body.Instructions.RemoveRange(pos + 1, instructionsToRemove);
					ILInlining.InlineIfPossible(body, pos, context);
					return true;
				}
			}
			return false;
		}

		bool DoTransformStackAllocInitializer(Block body, int pos)
		{
			if (pos >= body.Instructions.Count - 2)
				return false;
			ILInstruction inst = body.Instructions[pos];
			if (inst.MatchStLoc(out var v, out var locallocExpr) && locallocExpr.MatchLocAlloc(out var lengthInst))
			{
				if (lengthInst.MatchLdcI(out var lengthInBytes) && HandleCpblkInitializer(body, pos + 1, v, lengthInBytes, out var blob, out var elementType))
				{
					context.Step("HandleCpblkInitializer", inst);
					var block = new Block(BlockKind.StackAllocInitializer);
					var tempStore = context.Function.RegisterVariable(VariableKind.InitializerTarget, new PointerType(elementType));
					block.Instructions.Add(new StLoc(tempStore, locallocExpr));

					while (blob.RemainingBytes > 0)
					{
						block.Instructions.Add(StElemPtr(tempStore, blob.Offset, ReadElement(ref blob, elementType), elementType));
					}

					block.FinalInstruction = new LdLoc(tempStore);
					body.Instructions[pos] = new StLoc(v, block);
					body.Instructions.RemoveAt(pos + 1);
					ILInlining.InlineIfPossible(body, pos, context);
					ExpressionTransforms.RunOnSingleStatement(body.Instructions[pos], context);
					return true;
				}
				if (HandleSequentialLocAllocInitializer(body, pos + 1, v, locallocExpr, out elementType, out StObj[] values, out int instructionsToRemove))
				{
					context.Step("HandleSequentialLocAllocInitializer", inst);
					var block = new Block(BlockKind.StackAllocInitializer);
					var tempStore = context.Function.RegisterVariable(VariableKind.InitializerTarget, new PointerType(elementType));
					block.Instructions.Add(new StLoc(tempStore, locallocExpr));
					block.Instructions.AddRange(values.Where(value => value != null).Select(value => RewrapStore(tempStore, value, elementType)));
					block.FinalInstruction = new LdLoc(tempStore);
					body.Instructions[pos] = new StLoc(v, block);
					body.Instructions.RemoveRange(pos + 1, instructionsToRemove);
					ILInlining.InlineIfPossible(body, pos, context);
					ExpressionTransforms.RunOnSingleStatement(body.Instructions[pos], context);
					return true;
				}
			}
			return false;
		}

		private ILInstruction ReadElement(ref BlobReader blob, IType elementType)
		{
			switch (elementType.GetSize())
			{
				case 1:
					return new LdcI4(blob.ReadByte());
				case 2:
					return new LdcI4(blob.ReadInt16());
				case 4:
					return new LdcI4(blob.ReadInt32());
				case 8:
					return new LdcI8(blob.ReadInt64());
				default:
					throw new NotSupportedException();
			}
		}

		bool HandleCpblkInitializer(Block block, int pos, ILVariable v, long length, out BlobReader blob, out IType elementType)
		{
			blob = default;
			elementType = null;
			if (!block.Instructions[pos].MatchCpblk(out var dest, out var src, out var size))
				return false;
			if (!dest.MatchLdLoc(v) || !MatchGetStaticFieldAddress(src, out var field) || !size.MatchLdcI4((int)length))
				return false;
			if (!(v.IsSingleDefinition && v.LoadCount == 2))
				return false;
			if (field.MetadataToken.IsNil)
				return false;
			if (!block.Instructions[pos + 1].MatchStLoc(out var finalStore, out var value))
			{
				var otherLoadOfV = v.LoadInstructions.FirstOrDefault(l => !(l.Parent is Cpblk));
				if (otherLoadOfV == null)
					return false;
				finalStore = otherLoadOfV.Parent.Extract(context);
				if (finalStore == null)
					return false;
				value = ((StLoc)finalStore.StoreInstructions[0]).Value;
			}
			var fd = context.PEFile.Metadata.GetFieldDefinition((FieldDefinitionHandle)field.MetadataToken);
			if (!fd.HasFlag(System.Reflection.FieldAttributes.HasFieldRVA))
				return false;
			if (value.MatchLdLoc(v))
			{
				elementType = ((PointerType)finalStore.Type).ElementType;
			}
			else if (value is NewObj { Arguments: { Count: 2 } } newObj
				  && newObj.Method.DeclaringType.IsKnownType(KnownTypeCode.SpanOfT)
				  && newObj.Arguments[0].MatchLdLoc(v)
				  && newObj.Arguments[1].MatchLdcI4(out var elementCount))
			{
				elementType = ((ParameterizedType)newObj.Method.DeclaringType).TypeArguments[0];
				if (elementCount != length / elementType.GetSize())
					return false;
			}
			else
			{
				return false;
			}
			blob = fd.GetInitialValue(context.PEFile, context.TypeSystem);
			return true;
		}

		bool MatchGetStaticFieldAddress(ILInstruction input, out IField field)
		{
			if (input.MatchLdsFlda(out field))
				return true;
			// call get_Item(addressof System.ReadOnlySpan`1[[T]](call CreateSpan(ldmembertoken field)), ldc.i4 0)
			if (input is not Call { Method.Name: "get_Item", Arguments.Count: 2 } call)
				return false;
			if (!call.Method.DeclaringType.IsKnownType(KnownTypeCode.ReadOnlySpanOfT))
				return false;
			if (!call.Arguments[1].MatchLdcI4(0))
				return false;
			if (call.Arguments[0] is not AddressOf addressOf)
				return false;
			if (addressOf.Value is not Call { Method.Name: "CreateSpan", Arguments.Count: 1 } createSpanCall)
				return false;
			if (!IsRuntimeHelpers(createSpanCall.Method.DeclaringType))
				return false;
			if (!createSpanCall.Arguments[0].MatchLdMemberToken(out var member))
				return false;
			field = member as IField;
			return field != null;
		}

		static bool IsRuntimeHelpers(IType type) => type is { Name: "RuntimeHelpers", Namespace: "System.Runtime.CompilerServices" };

		unsafe bool HandleSequentialLocAllocInitializer(Block block, int pos, ILVariable store, ILInstruction locAllocInstruction, out IType elementType, out StObj[] values, out int instructionsToRemove)
		{
			int elementCount = 0;
			long minExpectedOffset = 0;
			values = null;
			elementType = null;
			instructionsToRemove = 0;

			if (!locAllocInstruction.MatchLocAlloc(out var lengthInstruction))
				return false;

			BlobReader blob = default;

			if (lengthInstruction.MatchLdcI(out long byteCount))
			{
				if (block.Instructions[pos].MatchInitblk(out var dest, out var value, out var size))
				{
					if (!dest.MatchLdLoc(store) || !size.MatchLdcI(byteCount))
						return false;
					instructionsToRemove++;
					pos++;
				}
				else if (block.Instructions[pos].MatchCpblk(out dest, out var src, out size))
				{
					if (!dest.MatchLdLoc(store) || !size.MatchLdcI(byteCount))
						return false;
					if (!MatchGetStaticFieldAddress(src, out var field))
						return false;
					var fd = context.PEFile.Metadata.GetFieldDefinition((FieldDefinitionHandle)field.MetadataToken);
					if (!fd.HasFlag(System.Reflection.FieldAttributes.HasFieldRVA))
						return false;
					blob = fd.GetInitialValue(context.PEFile, context.TypeSystem);
					instructionsToRemove++;
					pos++;
				}
			}

			for (int i = pos; i < block.Instructions.Count; i++)
			{
				// match the basic stobj pattern
				if (!block.Instructions[i].MatchStObj(out ILInstruction target, out var value, out var currentType)
					|| value.Descendants.OfType<IInstructionWithVariableOperand>().Any(inst => inst.Variable == store))
					break;
				// first
				if (elementType == null)
				{
					elementType = currentType;
					if (blob.StartPointer != null)
					{
						var countInstruction = PointerArithmeticOffset.Detect(lengthInstruction, elementType, checkForOverflow: true);
						if (countInstruction == null || !countInstruction.MatchLdcI(out long valuesLength) || valuesLength < 1)
							return false;
						values = new StObj[(int)valuesLength];
						int valueIndex = 0;
						while (blob.RemainingBytes > 0 && valueIndex < values.Length)
						{
							values[valueIndex] = StElemPtr(store, blob.Offset, ReadElement(ref blob, elementType), elementType);
							valueIndex++;
						}
					}
				}
				else if (!currentType.Equals(elementType))
				{
					break;
				}
				// match the target
				// should be either ldloc store (at offset 0)
				// or binary.add(ldloc store, offset) where offset is either 'elementSize' or 'i * elementSize'
				if (!target.MatchLdLoc(store))
				{
					if (!target.MatchBinaryNumericInstruction(BinaryNumericOperator.Add, out var left, out var right))
						return false;
					if (!left.MatchLdLoc(store))
						break;
					var offsetInst = PointerArithmeticOffset.Detect(right, elementType, ((BinaryNumericInstruction)target).CheckForOverflow);
					if (offsetInst == null)
						return false;
					if (!offsetInst.MatchLdcI(out long offset) || offset < 0 || offset < minExpectedOffset)
						break;
					minExpectedOffset = offset;
				}
				if (values == null)
				{
					var countInstruction = PointerArithmeticOffset.Detect(lengthInstruction, elementType, checkForOverflow: true);
					if (countInstruction == null || !countInstruction.MatchLdcI(out long valuesLength) || valuesLength < 1)
						return false;
					values = new StObj[(int)valuesLength];
				}
				if (minExpectedOffset >= values.Length)
					break;
				values[minExpectedOffset] = (StObj)block.Instructions[i];
				elementCount++;
			}

			if (values == null || store.Kind != VariableKind.StackSlot || store.StoreCount != 1
				|| store.AddressCount != 0 || store.LoadCount > values.Length + 1)
				return false;

			if (store.LoadInstructions.Last().Parent is StLoc finalStore)
			{
				elementType = ((PointerType)finalStore.Variable.Type).ElementType;
			}
			instructionsToRemove += elementCount;

			return elementCount <= values.Length;
		}

		ILInstruction RewrapStore(ILVariable target, StObj storeInstruction, IType type)
		{
			ILInstruction targetInst;
			if (storeInstruction.Target.MatchLdLoc(out _))
				targetInst = new LdLoc(target);
			else if (storeInstruction.Target.MatchBinaryNumericInstruction(BinaryNumericOperator.Add, out var left, out var right))
			{
				var old = (BinaryNumericInstruction)storeInstruction.Target;
				targetInst = new BinaryNumericInstruction(BinaryNumericOperator.Add, new LdLoc(target), right,
					old.CheckForOverflow, old.Sign);
			}
			else
				throw new NotSupportedException("This should never happen: Bug in HandleSequentialLocAllocInitializer!");

			return new StObj(targetInst, storeInstruction.Value, storeInstruction.Type);
		}

		StObj StElemPtr(ILVariable target, int offset, ILInstruction value, IType type)
		{
			var targetInst = offset == 0 ? (ILInstruction)new LdLoc(target) : new BinaryNumericInstruction(
				BinaryNumericOperator.Add,
				new LdLoc(target),
				new Conv(new LdcI4(offset), PrimitiveType.I, false, Sign.Signed),
				false,
				Sign.None
			);
			return new StObj(targetInst, value, type);
		}

		/// <summary>
		/// Handle simple case where RuntimeHelpers.InitializeArray is not used.
		/// </summary>
		internal static bool HandleSimpleArrayInitializer(ILFunction function, Block block, int pos, ILVariable store, int[] arrayLength, out (ILInstruction[] Indices, ILInstruction Value)[] values, out int instructionsToRemove)
		{
			instructionsToRemove = 0;
			int elementCount = 0;
			int length = arrayLength.Aggregate(1, (t, l) => t * l);
			// Cannot pre-allocate the result array, because we do not know yet,
			// whether there is in fact an array initializer.
			// To prevent excessive allocations, use min(|block|, arraySize) als initial capacity.
			// This should prevent list-resizing as well as out-of-memory errors.
			values = null;
			var valuesList = new List<(ILInstruction[] Indices, ILInstruction Value)>(Math.Min(block.Instructions.Count, length));

			int[] nextMinimumIndex = new int[arrayLength.Length];

			ILInstruction[] CalculateNextIndices(InstructionCollection<ILInstruction> indices, out bool exactMatch)
			{
				var nextIndices = new ILInstruction[arrayLength.Length];
				exactMatch = true;
				if (indices == null)
				{
					for (int k = 0; k < nextIndices.Length; k++)
					{
						nextIndices[k] = new LdcI4(nextMinimumIndex[k]);
					}
				}
				else
				{
					bool previousComponentWasGreater = false;
					for (int k = 0; k < indices.Count; k++)
					{
						if (!indices[k].MatchLdcI4(out int index))
							return null;
						// index must be in range [0..length[ and must be greater than or equal to nextMinimumIndex
						// to avoid running out of bounds or accidentally reordering instructions or overwriting previous instructions.
						// However, leaving array slots empty is allowed, as those are filled with default values when the
						// initializer block is generated.
						if (index < 0 || index >= arrayLength[k] || (!previousComponentWasGreater && index < nextMinimumIndex[k]))
							return null;
						nextIndices[k] = new LdcI4(nextMinimumIndex[k]);
						if (index != nextMinimumIndex[k])
						{
							exactMatch = false;
							// this flag allows us to check whether the whole set of indices is smaller:
							// [3, 3] should be smaller than [4, 0] even though 3 > 0.
							if (index > nextMinimumIndex[k])
								previousComponentWasGreater = true;
						}
					}
				}

				for (int k = nextMinimumIndex.Length - 1; k >= 0; k--)
				{
					nextMinimumIndex[k]++;
					if (nextMinimumIndex[k] < arrayLength[k] || k == 0)
						break;
					nextMinimumIndex[k] = 0;
				}

				return nextIndices;
			}

			int i = pos;
			int step;
			while (i < block.Instructions.Count)
			{
				InstructionCollection<ILInstruction> indices;
				// stobj elementType(ldelema elementType(ldloc store, indices), value)
				if (block.Instructions[i].MatchStObj(out ILInstruction target, out ILInstruction value, out IType type))
				{
					if (!(target is LdElema ldelem && ldelem.Array.MatchLdLoc(store)))
						break;
					indices = ldelem.Indices;
					step = 1;
					// stloc s(ldelema elementType(ldloc store, indices))
					// stobj elementType(ldloc s, value)
				}
				else if (block.Instructions[i].MatchStLoc(out var addressTemporary, out var addressValue))
				{
					if (!(addressTemporary.IsSingleDefinition && addressTemporary.LoadCount == 1))
						break;
					if (!(addressValue is LdElema ldelem && ldelem.Array.MatchLdLoc(store)))
						break;
					if (!(i + 1 < block.Instructions.Count))
						break;
					if (!block.Instructions[i + 1].MatchStObj(out target, out value, out type))
						break;
					if (!(target.MatchLdLoc(addressTemporary) && ldelem.Array.MatchLdLoc(store)))
						break;
					indices = ldelem.Indices;
					step = 2;
				}
				else
				{
					break;
				}
				if (value.Descendants.OfType<IInstructionWithVariableOperand>().Any(inst => inst.Variable == store))
					break;
				if (indices.Count != arrayLength.Length)
					break;
				bool exact;
				if (length <= 0)
					break;
				do
				{
					var nextIndices = CalculateNextIndices(indices, out exact);
					if (nextIndices == null)
						return false;
					if (exact)
					{
						valuesList.Add((nextIndices, value));
						elementCount++;
						instructionsToRemove += step;
					}
					else
					{
						valuesList.Add((nextIndices, null));
					}
				} while (valuesList.Count < length && !exact);
				i += step;
			}
			if (i < block.Instructions.Count)
			{
				if (block.Instructions[i].MatchStObj(out ILInstruction target, out ILInstruction value, out IType type))
				{
					// An element of the array is modified directly after the initializer:
					// Abort transform, so that partial initializers are not constructed. 
					if (target is LdElema ldelem && ldelem.Array.MatchLdLoc(store))
						return false;
				}
			}
			if (pos + instructionsToRemove >= block.Instructions.Count)
				return false;
			if (elementCount == 0)
				return false;

			bool mustTransform = ILInlining.IsCatchWhenBlock(block) || ILInlining.IsInConstructorInitializer(function, block.Instructions[pos]);
			// If there are not enough user-defined elements after scanning all instructions, we can abort the transform.
			// This avoids unnecessary allocations and OOM crashes (in case of int.MaxValue).
			if (elementCount < length / 3 - 5)
			{
				if (!mustTransform)
					return false;
				// .NET does not allow allocation of arrays >= 2 GB.
				if (length >= int.MaxValue)
					return false;
			}

			for (int j = valuesList.Count; j < length; j++)
			{
				var nextIndices = CalculateNextIndices(null, out _);
				if (nextIndices == null)
					return false;
				valuesList.Add((nextIndices, null));
			}
			values = valuesList.ToArray();
			return true;
		}

		bool HandleJaggedArrayInitializer(Block block, int pos, ILVariable store, IType elementType, int length, out ILVariable finalStore, out ILInstruction[] values, out int instructionsToRemove)
		{
			instructionsToRemove = 0;
			finalStore = null;
			// Cannot pre-allocate the result array, because we do not know yet,
			// whether there is in fact an array initializer.
			// To prevent excessive allocations, use min(|block|, arraySize) als initial capacity.
			// This should prevent list-resizing as well as out-of-memory errors.
			values = null;
			var valuesList = new List<ILInstruction>(Math.Min(block.Instructions.Count, length));

			for (int i = 0; i < length; i++)
			{
				// 1. Instruction: (optional) temporary copy of store
				bool hasTemporaryCopy = block.Instructions[pos].MatchStLoc(out var temp, out var storeLoad) && storeLoad.MatchLdLoc(store);
				ILInstruction initializer;
				if (hasTemporaryCopy)
				{
					if (!MatchJaggedArrayStore(block, pos + 1, temp, i, out initializer, out _))
						return false;
				}
				else
				{
					if (!MatchJaggedArrayStore(block, pos, store, i, out initializer, out _))
						return false;
				}
				valuesList.Add(initializer);
				int inc = hasTemporaryCopy ? 3 : 2;
				pos += inc;
				instructionsToRemove += inc;
			}
			// In case there is an extra copy of the store variable
			// Remove it and use its value instead.
			if (block.Instructions[pos].MatchStLoc(out finalStore, out var array))
			{
				if (!array.MatchLdLoc(store))
					return false;
				instructionsToRemove++;
			}
			else
			{
				finalStore = store;
			}
			values = valuesList.ToArray();
			return true;
		}

		bool MatchJaggedArrayStore(Block block, int pos, ILVariable store, int index, out ILInstruction initializer, out IType type)
		{
			initializer = null;
			type = null;
			// 3. Instruction: stobj(ldelema(ldloc temp, ldc.i4 0), ldloc tempArrayLoad)
			var finalInstruction = block.Instructions.ElementAtOrDefault(pos + 1);
			if (finalInstruction == null || !finalInstruction.MatchStObj(out var tempAccess, out var tempArrayLoad, out type)
				|| !tempArrayLoad.MatchLdLoc(out var initializerStore))
			{
				return false;
			}
			if (!(tempAccess is LdElema elemLoad) || !elemLoad.Array.MatchLdLoc(store) || elemLoad.Indices.Count != 1
				|| !elemLoad.Indices[0].MatchLdcI4(index))
			{
				return false;
			}
			// 2. Instruction: stloc(temp) with block (array initializer)
			var nextInstruction = block.Instructions.ElementAtOrDefault(pos);
			return nextInstruction != null
				&& nextInstruction.MatchStLoc(initializerStore, out initializer)
				&& initializer.OpCode == OpCode.Block;
		}

		static Block BlockFromInitializer(ILVariable v, IType elementType, int[] arrayLength, ILInstruction[] values)
		{
			var block = new Block(BlockKind.ArrayInitializer);
			block.Instructions.Add(new StLoc(v, new NewArr(elementType, arrayLength.Select(l => new LdcI4(l)).ToArray())));
			int step = arrayLength.Length + 1;

			var indices = new List<ILInstruction>();
			for (int i = 0; i < values.Length / step; i++)
			{
				// values array is filled backwards
				var value = values[step * i];

				indices.EnsureCapacity(step - 1);
				for (int j = step - 1; j >= 1; j--)
				{
					indices.Add(values[step * i + j]);
				}

				block.Instructions.Add(StElem(new LdLoc(v), indices.ToArray(), value, elementType));
				indices.Clear();
			}
			block.FinalInstruction = new LdLoc(v);
			return block;
		}

		static bool MatchNewArr(ILInstruction instruction, out IType arrayType, out int[] length)
		{
			length = null;
			arrayType = null;
			if (!(instruction is NewArr newArr))
				return false;
			arrayType = newArr.Type;
			var args = newArr.Indices;
			length = new int[args.Count];
			for (int i = 0; i < args.Count; i++)
			{
				if (!args[i].MatchLdcI4(out int value) || value <= 0)
					return false;
				length[i] = value;
			}
			return true;
		}

		bool MatchInitializeArrayCall(ILInstruction instruction, out ILInstruction array, out FieldDefinition field)
		{
			array = null;
			field = default;
			if (!(instruction is Call call) || call.Arguments.Count != 2)
				return false;
			IMethod method = call.Method;
			if (!method.IsStatic || method.Name != "InitializeArray" || method.DeclaringTypeDefinition == null)
				return false;
			if (!IsRuntimeHelpers(method.DeclaringType))
				return false;
			array = call.Arguments[0];
			if (!call.Arguments[1].MatchLdMemberToken(out var member))
				return false;
			if (member.MetadataToken.IsNil)
				return false;
			field = context.PEFile.Metadata.GetFieldDefinition((FieldDefinitionHandle)member.MetadataToken);
			return true;
		}

		bool HandleRuntimeHelpersInitializeArray(Block body, int pos, ILVariable array, IType arrayType, int[] arrayLength, out ILInstruction[] values, out int foundPos)
		{
			if (MatchInitializeArrayCall(body.Instructions[pos], out var arrayInst, out var field) && arrayInst.MatchLdLoc(array))
			{
				if (field.HasFlag(System.Reflection.FieldAttributes.HasFieldRVA))
				{
					var valuesList = new List<ILInstruction>();
					var initialValue = field.GetInitialValue(context.PEFile, context.TypeSystem);
					if (DecodeArrayInitializer(arrayType, initialValue, arrayLength, valuesList))
					{
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

		/// <summary>
		/// call InitializeArray(newarr T(size), ldmembertoken fieldToken)
		/// =>
		/// Block (ArrayInitializer) {
		///		stloc i(newarr T(size))
		///		stobj T(ldelema T(... indices ...), value)
		///		final: ldloc i
		/// }
		/// </summary>
		bool DoTransformInlineRuntimeHelpersInitializeArray(Block body, int pos)
		{
			var inst = body.Instructions[pos];
			if (!MatchInitializeArrayCall(inst, out var arrayInst, out var field))
				return false;
			if (!MatchNewArr(arrayInst, out var elementType, out var arrayLength))
				return false;
			if (!field.HasFlag(System.Reflection.FieldAttributes.HasFieldRVA))
				return false;
			var valuesList = new List<ILInstruction>();
			var initialValue = field.GetInitialValue(context.PEFile, context.TypeSystem);
			if (!DecodeArrayInitializer(elementType, initialValue, arrayLength, valuesList))
				return false;
			context.Step("InlineRuntimeHelpersInitializeArray: single-dim", inst);
			var tempStore = context.Function.RegisterVariable(VariableKind.InitializerTarget, new ArrayType(context.TypeSystem, elementType, arrayLength.Length));
			var block = BlockFromInitializer(tempStore, elementType, arrayLength, valuesList.ToArray());
			body.Instructions[pos] = block;
			ILInlining.InlineIfPossible(body, pos, context);
			return true;
		}

		static bool DecodeArrayInitializer(IType type, BlobReader initialValue, int[] arrayLength, List<ILInstruction> output)
		{
			TypeCode typeCode = ReflectionHelper.GetTypeCode(type);
			switch (typeCode)
			{
				case TypeCode.Boolean:
				case TypeCode.Byte:
					return DecodeArrayInitializer(initialValue, arrayLength, output, typeCode, (ref BlobReader r) => new LdcI4(r.ReadByte()));
				case TypeCode.SByte:
					return DecodeArrayInitializer(initialValue, arrayLength, output, typeCode, (ref BlobReader r) => new LdcI4(r.ReadSByte()));
				case TypeCode.Int16:
					return DecodeArrayInitializer(initialValue, arrayLength, output, typeCode, (ref BlobReader r) => new LdcI4(r.ReadInt16()));
				case TypeCode.Char:
				case TypeCode.UInt16:
					return DecodeArrayInitializer(initialValue, arrayLength, output, typeCode, (ref BlobReader r) => new LdcI4(r.ReadUInt16()));
				case TypeCode.Int32:
				case TypeCode.UInt32:
					return DecodeArrayInitializer(initialValue, arrayLength, output, typeCode, (ref BlobReader r) => new LdcI4(r.ReadInt32()));
				case TypeCode.Int64:
				case TypeCode.UInt64:
					return DecodeArrayInitializer(initialValue, arrayLength, output, typeCode, (ref BlobReader r) => new LdcI8(r.ReadInt64()));
				case TypeCode.Single:
					return DecodeArrayInitializer(initialValue, arrayLength, output, typeCode, (ref BlobReader r) => new LdcF4(r.ReadSingle()));
				case TypeCode.Double:
					return DecodeArrayInitializer(initialValue, arrayLength, output, typeCode, (ref BlobReader r) => new LdcF8(r.ReadDouble()));
				case TypeCode.Object:
				case TypeCode.Empty:
					var typeDef = type.GetDefinition();
					if (typeDef != null && typeDef.Kind == TypeKind.Enum)
						return DecodeArrayInitializer(typeDef.EnumUnderlyingType, initialValue, arrayLength, output);
					return false;
				default:
					return false;
			}
		}

		delegate ILInstruction ValueDecoder(ref BlobReader reader);

		static bool DecodeArrayInitializer(BlobReader initialValue, int[] arrayLength,
			List<ILInstruction> output, TypeCode elementType, ValueDecoder decoder)
		{
			int elementSize = ElementSizeOf(elementType);
			var totalLength = arrayLength.Aggregate(1, (t, l) => t * l);
			if (initialValue.RemainingBytes < (totalLength * elementSize))
				return false;

			output.EnsureCapacity(totalLength + totalLength * arrayLength.Length);

			for (int i = 0; i < totalLength; i++)
			{
				output.Add(decoder(ref initialValue));
				int next = i;
				for (int j = arrayLength.Length - 1; j >= 0; j--)
				{
					output.Add(new LdcI4(next % arrayLength[j]));
					next /= arrayLength[j];
				}
			}

			return true;
		}

		static ILInstruction StElem(ILInstruction array, ILInstruction[] indices, ILInstruction value, IType type)
		{
			if (type.GetStackType() != value.ResultType)
			{
				value = new Conv(value, type.ToPrimitiveType(), false, Sign.None);
			}
			return new StObj(new LdElema(type, array, indices) { DelayExceptions = true }, value, type);
		}

		internal static ILInstruction GetNullExpression(IType elementType)
		{
			ITypeDefinition typeDef = elementType.GetEnumUnderlyingType().GetDefinition();
			if (typeDef == null)
				return new DefaultValue(elementType);
			switch (typeDef.KnownTypeCode)
			{
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

		static int ElementSizeOf(TypeCode elementType)
		{
			switch (elementType)
			{
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
