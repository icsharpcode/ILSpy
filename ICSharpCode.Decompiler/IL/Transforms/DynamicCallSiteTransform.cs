// Copyright (c) 2018 Siegfried Pammer
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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transforms the "callsite initialization pattern" into DynamicInstructions.
	/// </summary>
	public class DynamicCallSiteTransform : IILTransform
	{
		ILTransformContext context;

		const string CallSiteTypeName = "System.Runtime.CompilerServices.CallSite";
		const string CSharpBinderTypeName = "Microsoft.CSharp.RuntimeBinder.Binder";

		public void Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.Dynamic)
				return;

			this.context = context;

			Dictionary<IField, CallSiteInfo> callsites = new Dictionary<IField, CallSiteInfo>();
			HashSet<BlockContainer> modifiedContainers = new HashSet<BlockContainer>();

			foreach (var block in function.Descendants.OfType<Block>()) {
				if (block.Instructions.Count < 2) continue;
				// Check if, we deal with a callsite cache field null check:
				// if (comp(ldsfld <>p__3 == ldnull)) br IL_000c
				// br IL_002b
				if (!(block.Instructions.SecondToLastOrDefault() is IfInstruction ifInst)) continue;
				if (!(block.Instructions.LastOrDefault() is Branch branchAfterInit)) continue;
				if (!MatchCallSiteCacheNullCheck(ifInst.Condition, out var callSiteCacheField, out var callSiteDelegate, out bool invertBranches))
					continue;
				if (!ifInst.TrueInst.MatchBranch(out var trueBlock))
					continue;
				Block callSiteInitBlock, targetBlockAfterInit;
				if (invertBranches) {
					callSiteInitBlock = branchAfterInit.TargetBlock;
					targetBlockAfterInit = trueBlock;
				} else {
					callSiteInitBlock = trueBlock;
					targetBlockAfterInit = branchAfterInit.TargetBlock;
				}
				if (!ScanCallSiteInitBlock(callSiteInitBlock, callSiteCacheField, callSiteDelegate, out var callSiteInfo, out var blockAfterInit))
					continue;
				if (targetBlockAfterInit != blockAfterInit)
					continue;
				callSiteInfo.DelegateType = callSiteDelegate;
				callSiteInfo.ConditionalJumpToInit = ifInst;
				callSiteInfo.Inverted = invertBranches;
				callSiteInfo.BranchAfterInit = branchAfterInit;
				callsites.Add(callSiteCacheField, callSiteInfo);
			}

			var storesToRemove = new List<StLoc>();

			foreach (var invokeCall in function.Descendants.OfType<CallVirt>()) {
				if (invokeCall.Method.DeclaringType.Kind != TypeKind.Delegate || invokeCall.Method.Name != "Invoke" || invokeCall.Arguments.Count == 0)
					continue;
				var firstArgument = invokeCall.Arguments[0];
				if (firstArgument.MatchLdLoc(out var stackSlot) && stackSlot.Kind == VariableKind.StackSlot && stackSlot.IsSingleDefinition) {
					firstArgument = ((StLoc)stackSlot.StoreInstructions[0]).Value;
				}
				if (!firstArgument.MatchLdFld(out var cacheFieldLoad, out var targetField))
					continue;
				if (!cacheFieldLoad.MatchLdsFld(out var cacheField))
					continue;
				if (!callsites.TryGetValue(cacheField, out var callsite))
					continue;
				context.Stepper.Step("Transform callsite for " + callsite.MemberName);
				var deadArguments = new List<ILInstruction>();
				ILInstruction replacement = MakeDynamicInstruction(callsite, invokeCall, deadArguments);
				if (replacement == null)
					continue;
				invokeCall.ReplaceWith(replacement);
				Debug.Assert(callsite.ConditionalJumpToInit?.Parent is Block);
				var block = ((Block)callsite.ConditionalJumpToInit.Parent);
				if (callsite.Inverted) {
					block.Instructions.Remove(callsite.ConditionalJumpToInit);
					callsite.BranchAfterInit.ReplaceWith(callsite.ConditionalJumpToInit.TrueInst);
				} else {
					block.Instructions.Remove(callsite.ConditionalJumpToInit);
				}
				foreach (var arg in deadArguments) {
					if (arg.MatchLdLoc(out var temporary) && temporary.Kind == VariableKind.StackSlot && temporary.IsSingleDefinition && temporary.LoadCount == 0) {
						StLoc stLoc = (StLoc)temporary.StoreInstructions[0];
						if (stLoc.Parent is Block storeParentBlock) {
							var value = stLoc.Value;
							if (value.MatchLdsFld(out var cacheFieldCopy) && cacheFieldCopy.Equals(cacheField))
								storesToRemove.Add(stLoc);
							if (value.MatchLdFld(out cacheFieldLoad, out var targetFieldCopy) && cacheFieldLoad.MatchLdsFld(out cacheFieldCopy) && cacheField.Equals(cacheFieldCopy) && targetField.Equals(targetFieldCopy))
								storesToRemove.Add(stLoc);
						}
					}
				}
				modifiedContainers.Add((BlockContainer)block.Parent);
			}

			foreach (var inst in storesToRemove) {
				Block parentBlock = (Block)inst.Parent;
				parentBlock.Instructions.RemoveAt(inst.ChildIndex);
			}

			foreach (var container in modifiedContainers)
				container.SortBlocks(deleteUnreachableBlocks: true);
		}

		ILInstruction MakeDynamicInstruction(CallSiteInfo callsite, CallVirt targetInvokeCall, List<ILInstruction> deadArguments)
		{
			switch (callsite.Kind) {
				case BinderMethodKind.BinaryOperation:
					deadArguments.AddRange(targetInvokeCall.Arguments.Take(2));
					return new DynamicBinaryOperatorInstruction(
						binderFlags: callsite.Flags,
						operation: callsite.Operation,
						context: callsite.Context,
						leftArgumentInfo: callsite.ArgumentInfos[0],
						left: targetInvokeCall.Arguments[2],
						rightArgumentInfo: callsite.ArgumentInfos[1],
						right: targetInvokeCall.Arguments[3]
					);
				case BinderMethodKind.Convert:
					deadArguments.AddRange(targetInvokeCall.Arguments.Take(2));
					ILInstruction result = new DynamicConvertInstruction(
						binderFlags: callsite.Flags,
						context: callsite.Context,
						type: callsite.ConvertTargetType,
						argument: targetInvokeCall.Arguments[2]
					);
					if (result.ResultType == StackType.Unknown) {
						// if references are missing, we need to coerce the primitive type to None.
						// Otherwise we will get loads of assertions.
						result = new Conv(result, PrimitiveType.None, ((DynamicConvertInstruction)result).IsChecked, Sign.None);
					}
					return result;
				case BinderMethodKind.GetIndex:
					deadArguments.AddRange(targetInvokeCall.Arguments.Take(2));
					return new DynamicGetIndexInstruction(
						binderFlags: callsite.Flags,
						context: callsite.Context,
						argumentInfo: callsite.ArgumentInfos,
						arguments: targetInvokeCall.Arguments.Skip(2).ToArray()
					);
				case BinderMethodKind.GetMember:
					deadArguments.AddRange(targetInvokeCall.Arguments.Take(2));
					return new DynamicGetMemberInstruction(
						binderFlags: callsite.Flags,
						name: callsite.MemberName,
						context: callsite.Context,
						targetArgumentInfo: callsite.ArgumentInfos[0],
						target: targetInvokeCall.Arguments[2]
					);
				case BinderMethodKind.Invoke:
					deadArguments.AddRange(targetInvokeCall.Arguments.Take(2));
					return new DynamicInvokeInstruction(
						binderFlags: callsite.Flags,
						context: callsite.Context,
						argumentInfo: callsite.ArgumentInfos,
						arguments: targetInvokeCall.Arguments.Skip(2).ToArray()
					);
				case BinderMethodKind.InvokeConstructor:
					var arguments = targetInvokeCall.Arguments.Skip(2).ToArray();
					// Extract type information from targetInvokeCall:
					// Must either be an inlined type or
					// a reference to a variable that is initialized with a type.
					if (!TransformExpressionTrees.MatchGetTypeFromHandle(arguments[0], out var type)) {
						if (!(arguments[0].MatchLdLoc(out var temp) && temp.IsSingleDefinition && temp.StoreInstructions.FirstOrDefault() is StLoc initStore))
							return null;
						if (!TransformExpressionTrees.MatchGetTypeFromHandle(initStore.Value, out type))
							return null;
					}
					deadArguments.AddRange(targetInvokeCall.Arguments.Take(2));
					return new DynamicInvokeConstructorInstruction(
						binderFlags: callsite.Flags,
						type: type ?? SpecialType.UnknownType,
						context: callsite.Context,
						argumentInfo: callsite.ArgumentInfos,
						arguments: arguments
					);
				case BinderMethodKind.InvokeMember:
					deadArguments.AddRange(targetInvokeCall.Arguments.Take(2));
					return new DynamicInvokeMemberInstruction(
						binderFlags: callsite.Flags,
						name: callsite.MemberName,
						typeArguments: callsite.TypeArguments,
						context: callsite.Context,
						argumentInfo: callsite.ArgumentInfos,
						arguments: targetInvokeCall.Arguments.Skip(2).ToArray()
					);
				case BinderMethodKind.IsEvent:
					deadArguments.AddRange(targetInvokeCall.Arguments.Take(2));
					return new DynamicIsEventInstruction(
						binderFlags: callsite.Flags,
						name: callsite.MemberName,
						context: callsite.Context,
						argument: targetInvokeCall.Arguments[2]
					);
				case BinderMethodKind.SetIndex:
					deadArguments.AddRange(targetInvokeCall.Arguments.Take(2));
					return new DynamicSetIndexInstruction(
						binderFlags: callsite.Flags,
						context: callsite.Context,
						argumentInfo: callsite.ArgumentInfos,
						arguments: targetInvokeCall.Arguments.Skip(2).ToArray()
					);
				case BinderMethodKind.SetMember:
					deadArguments.AddRange(targetInvokeCall.Arguments.Take(2));
					return new DynamicSetMemberInstruction(
						binderFlags: callsite.Flags,
						name: callsite.MemberName,
						context: callsite.Context,
						targetArgumentInfo: callsite.ArgumentInfos[0],
						target: targetInvokeCall.Arguments[2],
						valueArgumentInfo: callsite.ArgumentInfos[1],
						value: targetInvokeCall.Arguments[3]
					);
				case BinderMethodKind.UnaryOperation:
					deadArguments.AddRange(targetInvokeCall.Arguments.Take(2));
					return new DynamicUnaryOperatorInstruction(
						binderFlags: callsite.Flags,
						operation: callsite.Operation,
						context: callsite.Context,
						operandArgumentInfo: callsite.ArgumentInfos[0],
						operand: targetInvokeCall.Arguments[2]
					);
				default:
					throw new ArgumentOutOfRangeException($"Value {callsite.Kind} is not supported!");
			}
		}

		bool ScanCallSiteInitBlock(Block callSiteInitBlock, IField callSiteCacheField, IType callSiteDelegateType, out CallSiteInfo callSiteInfo, out Block blockAfterInit)
		{
			callSiteInfo = default(CallSiteInfo);
			blockAfterInit = null;
			int instCount = callSiteInitBlock.Instructions.Count;
			if (callSiteInitBlock.IncomingEdgeCount != 1 || instCount < 2)
				return false;
			if (!callSiteInitBlock.Instructions[instCount - 1].MatchBranch(out blockAfterInit))
				return false;
			if (!callSiteInitBlock.Instructions[instCount - 2].MatchStsFld(out var field, out var value) || !field.Equals(callSiteCacheField))
				return false;
			if (!(value is Call createBinderCall) || createBinderCall.Method.TypeArguments.Count != 0 || createBinderCall.Arguments.Count != 1 || createBinderCall.Method.Name != "Create" || createBinderCall.Method.DeclaringType.FullName != CallSiteTypeName || createBinderCall.Method.DeclaringType.TypeArguments.Count != 1)
				return false;
			if (!(createBinderCall.Arguments[0] is Call binderCall) || binderCall.Method.DeclaringType.FullName != CSharpBinderTypeName || binderCall.Method.DeclaringType.TypeParameterCount != 0)
				return false;
			callSiteInfo.DelegateType = callSiteDelegateType;
			callSiteInfo.InitBlock = callSiteInitBlock;
			switch (binderCall.Method.Name) {
				case "IsEvent":
					callSiteInfo.Kind = BinderMethodKind.IsEvent;
					// In the case of Binder.IsEvent all arguments should already be properly inlined, as there is no array initializer:
					// Scan arguments: binder flags, member name, context type
					if (binderCall.Arguments.Count != 3)
						return false;
					if (!binderCall.Arguments[0].MatchLdcI4(out int binderFlagsInteger))
						return false;
					callSiteInfo.Flags = (CSharpBinderFlags)binderFlagsInteger;
					if (!binderCall.Arguments[1].MatchLdStr(out string name))
						return false;
					callSiteInfo.MemberName = name;
					if (!TransformExpressionTrees.MatchGetTypeFromHandle(binderCall.Arguments[2], out var contextType))
						return false;
					callSiteInfo.Context = contextType;
					return true;
				case "Convert":
					callSiteInfo.Kind = BinderMethodKind.Convert;
					// In the case of Binder.Convert all arguments should already be properly inlined, as there is no array initializer:
					// Scan arguments: binder flags, target type, context type
					if (binderCall.Arguments.Count != 3)
						return false;
					if (!binderCall.Arguments[0].MatchLdcI4(out binderFlagsInteger))
						return false;
					callSiteInfo.Flags = (CSharpBinderFlags)binderFlagsInteger;
					if (!TransformExpressionTrees.MatchGetTypeFromHandle(binderCall.Arguments[1], out var targetType))
						return false;
					callSiteInfo.ConvertTargetType = targetType;
					if (!TransformExpressionTrees.MatchGetTypeFromHandle(binderCall.Arguments[2], out contextType))
						return false;
					callSiteInfo.Context = contextType;
					return true;
				case "InvokeMember":
					callSiteInfo.Kind = BinderMethodKind.InvokeMember;
					if (binderCall.Arguments.Count != 5)
						return false;
					// First argument: binder flags
					// The value must be a single ldc.i4 instruction.
					if (!binderCall.Arguments[0].MatchLdLoc(out var variable))
						return false;
					if (!callSiteInitBlock.Instructions[0].MatchStLoc(variable, out value))
						return false;
					if (!value.MatchLdcI4(out binderFlagsInteger))
						return false;
					callSiteInfo.Flags = (CSharpBinderFlags)binderFlagsInteger;
					// Second argument: method name
					// The value must be a single ldstr instruction.
					if (!binderCall.Arguments[1].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[1].MatchStLoc(variable, out value))
						return false;
					if (!value.MatchLdStr(out name))
						return false;
					callSiteInfo.MemberName = name;
					// Third argument: type arguments
					// The value must be either ldnull (no type arguments) or an array initializer pattern.
					if (!binderCall.Arguments[2].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[2].MatchStLoc(out var variableOrTemporary, out value))
						return false;
					int numberOfTypeArguments = 0;
					if (!value.MatchLdNull()) {
						if (value is NewArr typeArgsNewArr && typeArgsNewArr.Type.IsKnownType(KnownTypeCode.Type) && typeArgsNewArr.Indices.Count == 1 && typeArgsNewArr.Indices[0].MatchLdcI4(out numberOfTypeArguments)) {
							if (!TransformArrayInitializers.HandleSimpleArrayInitializer(context.Function, callSiteInitBlock, 3, variableOrTemporary, typeArgsNewArr.Type, new[] { numberOfTypeArguments }, out var typeArguments, out _))
								return false;
							int i = 0;
							callSiteInfo.TypeArguments = new IType[numberOfTypeArguments];
							foreach (var (_, typeArg) in typeArguments) {
								if (!TransformExpressionTrees.MatchGetTypeFromHandle(typeArg, out var type))
									return false;
								callSiteInfo.TypeArguments[i] = type;
								i++;
							}
						} else {
							return false;
						}
					}
					int typeArgumentsOffset = numberOfTypeArguments;
					// Special case for csc array initializers:
					if (variableOrTemporary != variable) {
						// store temporary from array initializer in variable
						if (!callSiteInitBlock.Instructions[3 + typeArgumentsOffset].MatchStLoc(variable, out value))
							return false;
						if (!value.MatchLdLoc(variableOrTemporary))
							return false;
						typeArgumentsOffset++;
					}
					// Fourth argument: context type
					if (!binderCall.Arguments[3].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[3 + typeArgumentsOffset].MatchStLoc(variable, out value))
						return false;
					if (!TransformExpressionTrees.MatchGetTypeFromHandle(value, out contextType))
						return false;
					callSiteInfo.Context = contextType;
					// Fifth argument: call parameter info
					if (!binderCall.Arguments[4].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[4 + typeArgumentsOffset].MatchStLoc(variable, out value))
						return false;
					if (!ExtractArgumentInfo(value, ref callSiteInfo, 5 + typeArgumentsOffset, variable))
						return false;
					return true;
				case "GetMember":
				case "SetMember":
					callSiteInfo.Kind = binderCall.Method.Name == "GetMember" ? BinderMethodKind.GetMember : BinderMethodKind.SetMember;
					if (binderCall.Arguments.Count != 4)
						return false;
					// First argument: binder flags
					// The value must be a single ldc.i4 instruction.
					if (!binderCall.Arguments[0].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[0].MatchStLoc(variable, out value))
						return false;
					if (!value.MatchLdcI4(out binderFlagsInteger))
						return false;
					callSiteInfo.Flags = (CSharpBinderFlags)binderFlagsInteger;
					// Second argument: method name
					// The value must be a single ldstr instruction.
					if (!binderCall.Arguments[1].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[1].MatchStLoc(variable, out value))
						return false;
					if (!value.MatchLdStr(out name))
						return false;
					callSiteInfo.MemberName = name;
					// Third argument: context type
					if (!binderCall.Arguments[2].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[2].MatchStLoc(variable, out value))
						return false;
					if (!TransformExpressionTrees.MatchGetTypeFromHandle(value, out contextType))
						return false;
					callSiteInfo.Context = contextType;
					// Fourth argument: call parameter info
					if (!binderCall.Arguments[3].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[3].MatchStLoc(variable, out value))
						return false;
					if (!ExtractArgumentInfo(value, ref callSiteInfo, 4, variable))
						return false;
					return true;
				case "GetIndex":
				case "SetIndex":
				case "InvokeConstructor":
				case "Invoke":
					switch (binderCall.Method.Name) {
						case "GetIndex":
							callSiteInfo.Kind = BinderMethodKind.GetIndex;
							break;
						case "SetIndex":
							callSiteInfo.Kind = BinderMethodKind.SetIndex;
							break;
						case "InvokeConstructor":
							callSiteInfo.Kind = BinderMethodKind.InvokeConstructor;
							break;
						case "Invoke":
							callSiteInfo.Kind = BinderMethodKind.Invoke;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					if (binderCall.Arguments.Count != 3)
						return false;
					// First argument: binder flags
					// The value must be a single ldc.i4 instruction.
					if (!binderCall.Arguments[0].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[0].MatchStLoc(variable, out value))
						return false;
					if (!value.MatchLdcI4(out binderFlagsInteger))
						return false;
					callSiteInfo.Flags = (CSharpBinderFlags)binderFlagsInteger;
					// Second argument: context type
					if (!binderCall.Arguments[1].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[1].MatchStLoc(variable, out value))
						return false;
					if (!TransformExpressionTrees.MatchGetTypeFromHandle(value, out contextType))
						return false;
					callSiteInfo.Context = contextType;
					// Third argument: call parameter info
					if (!binderCall.Arguments[2].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[2].MatchStLoc(variable, out value))
						return false;
					if (!ExtractArgumentInfo(value, ref callSiteInfo, 3, variable))
						return false;
					return true;
				case "UnaryOperation":
				case "BinaryOperation":
					callSiteInfo.Kind = binderCall.Method.Name == "BinaryOperation" ? BinderMethodKind.BinaryOperation : BinderMethodKind.UnaryOperation;
					if (binderCall.Arguments.Count != 4)
						return false;
					// First argument: binder flags
					// The value must be a single ldc.i4 instruction.
					if (!binderCall.Arguments[0].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[0].MatchStLoc(variable, out value))
						return false;
					if (!value.MatchLdcI4(out binderFlagsInteger))
						return false;
					callSiteInfo.Flags = (CSharpBinderFlags)binderFlagsInteger;
					// Second argument: operation
					// The value must be a single ldc.i4 instruction.
					if (!binderCall.Arguments[1].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[1].MatchStLoc(variable, out value))
						return false;
					if (!value.MatchLdcI4(out int operation))
						return false;
					callSiteInfo.Operation = (ExpressionType)operation;
					// Third argument: context type
					if (!binderCall.Arguments[2].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[2].MatchStLoc(variable, out value))
						return false;
					if (!TransformExpressionTrees.MatchGetTypeFromHandle(value, out contextType))
						return false;
					callSiteInfo.Context = contextType;
					// Fourth argument: call parameter info
					if (!binderCall.Arguments[3].MatchLdLoc(out variable))
						return false;
					if (!callSiteInitBlock.Instructions[3].MatchStLoc(variable, out value))
						return false;
					if (!ExtractArgumentInfo(value, ref callSiteInfo, 4, variable))
						return false;
					return true;
				default:
					return false;
			}
		}

		bool ExtractArgumentInfo(ILInstruction value, ref CallSiteInfo callSiteInfo, int instructionOffset, ILVariable variable)
		{
			if (!(value is NewArr newArr2 && newArr2.Type.FullName == "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" && newArr2.Indices.Count == 1 && newArr2.Indices[0].MatchLdcI4(out var numberOfArguments)))
				return false;
			if (!TransformArrayInitializers.HandleSimpleArrayInitializer(context.Function, callSiteInfo.InitBlock, instructionOffset, variable, newArr2.Type, new[] { numberOfArguments }, out var arguments, out _))
				return false;
			int i = 0;
			callSiteInfo.ArgumentInfos = new CSharpArgumentInfo[numberOfArguments];
			IMethod invokeMethod = callSiteInfo.DelegateType.GetDelegateInvokeMethod();
			if (invokeMethod == null)
				return false;
			var compileTimeTypes = invokeMethod.Parameters.SelectReadOnlyArray(p => p.Type);
			foreach (var (_, arg) in arguments) {
				if (!(arg is Call createCall))
					return false;
				if (!(createCall.Method.Name == "Create" && createCall.Method.DeclaringType.FullName == "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" && createCall.Arguments.Count == 2))
					return false;
				if (!createCall.Arguments[0].MatchLdcI4(out var argumentInfoFlags))
					return false;
				if (!createCall.Arguments[1].MatchLdStr(out string argumentName))
					if (!createCall.Arguments[1].MatchLdNull())
						return false;
				callSiteInfo.ArgumentInfos[i] = new CSharpArgumentInfo { Flags = (CSharpArgumentInfoFlags)argumentInfoFlags, Name = argumentName, CompileTimeType = compileTimeTypes[i + 1] };
				i++;
			}
			return true;
		}

		bool MatchCallSiteCacheNullCheck(ILInstruction condition, out IField callSiteCacheField, out IType callSiteDelegate, out bool invertBranches)
		{
			callSiteCacheField = null;
			callSiteDelegate = null;
			invertBranches = false;
			if (!condition.MatchCompEqualsNull(out var argument)) {
				if (!condition.MatchCompNotEqualsNull(out argument))
					return false;
				invertBranches = true;
			}
			if (!argument.MatchLdsFld(out callSiteCacheField) || callSiteCacheField.ReturnType.TypeArguments.Count != 1 || callSiteCacheField.ReturnType.FullName != CallSiteTypeName)
				return false;
			callSiteDelegate = callSiteCacheField.ReturnType.TypeArguments[0];
			if (callSiteDelegate.Kind != TypeKind.Delegate)
				return false;
			return true;
		}

		struct CallSiteInfo
		{
			public bool Inverted;
			public ILInstruction BranchAfterInit;
			public IfInstruction ConditionalJumpToInit;
			public Block InitBlock;
			public IType DelegateType;
			public BinderMethodKind Kind;
			public CSharpBinderFlags Flags;
			public ExpressionType Operation;
			public IType Context;
			public IType ConvertTargetType;
			public IType[] TypeArguments;
			public CSharpArgumentInfo[] ArgumentInfos;
			public string MemberName;
		}

		enum BinderMethodKind
		{
			BinaryOperation,
			Convert,
			GetIndex,
			GetMember,
			Invoke,
			InvokeConstructor,
			InvokeMember,
			IsEvent,
			SetIndex,
			SetMember,
			UnaryOperation
		}
	}
}
