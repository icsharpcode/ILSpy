// Copyright (c) 2017 Siegfried Pammer
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

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transforms collection and object initialization patterns.
	/// </summary>
	public class TransformCollectionAndObjectInitializers : IStatementTransform
	{
		StatementTransformContext context;

		void IStatementTransform.Run(Block block, int pos, StatementTransformContext context)
		{
			this.context = context;
			try {
				DoTransform(block, pos);
			} finally {
				this.context = null;
			}
		}

		bool DoTransform(Block body, int pos)
		{
			ILInstruction inst = body.Instructions[pos];
			
			// Match callvirt set_Content(ldloc obj, newobj)
			if (inst is CallInstruction callInst) {
				(var kind, var path, var values, var targetVariable) = AccessPathElement.GetAccessPath(callInst, null, new Dictionary<ILVariable, (int Index, ILInstruction Value)>());
				if (kind != AccessPathKind.Setter)
					return false;
				string propertyName = callInst.Method.FullName.Replace("set", "");
				int initializerItemsCount = 0;
				while (pos + initializerItemsCount + 1 < body.Instructions.Count
					&& CheckSetter(body.Instructions[pos + initializerItemsCount + 1], propertyName, callInst.Arguments[0])) {
					initializerItemsCount++;
				}
				if (initializerItemsCount == 0)
					return false;
				Block initializerBlock = new Block(BlockType.ObjectInitializer);
				IType instType = null;
				switch (callInst.Arguments[1]) {
					case NewObj newObjInst:
						instType = newObjInst.Method.DeclaringType;
						break;
					case DefaultValue defaultVal:
						instType = defaultVal.Type;
						break;
					case Block existingInitializer:
						if (existingInitializer.Type == BlockType.CollectionInitializer || existingInitializer.Type == BlockType.ObjectInitializer) {
							initializerBlock = existingInitializer;
							var value = ((StLoc)initializerBlock.Instructions[0]).Value;
							if (value is NewObj no)
								instType = no.Method.DeclaringType;
							else
								instType = ((DefaultValue)value).Type;
							break;
						}
						return false;
					default:
						return false;
				}
				var finalSlot = context.Function.RegisterVariable(VariableKind.InitializerTarget, instType);
				initializerBlock.FinalInstruction = new LdLoc(finalSlot);
				initializerBlock.Instructions.Add(new StLoc(finalSlot, callInst.Arguments[1].Clone()));

				for (int i = 1; i <= initializerItemsCount; i++) {
					CallInstruction c = body.Instructions[i + pos].Clone() as CallInstruction;
					foreach (CallInstruction call in c.Descendants.OfType<CallInstruction>()) {
						if (call.Method.FullName.Replace("get", "").Equals(propertyName)) {
							// callvirt get_Content(ldloc obj)
							if (SpecialToString(callInst.Arguments[0]).Equals(SpecialToString(callInst.Arguments[0]))) {
								call.ReplaceWith(new LdLoc(finalSlot));
							}
						}
					}
					initializerBlock.Instructions.Add(c);
				}

				callInst.Arguments[1].ReplaceWith(initializerBlock);
				body.Instructions.RemoveRange(pos + 1, initializerItemsCount);
				ILInlining.InlineIfPossible(body, pos, context);
				return true;
			}

			// Match stloc(v, newobj)
			if (inst.MatchStLoc(out var v, out var initInst) && (v.Kind == VariableKind.Local || v.Kind == VariableKind.StackSlot)) {
				Block initializerBlock = null;
				IType instType;
				switch (initInst) {
					case NewObj newObjInst:
						if (newObjInst.ILStackWasEmpty && v.Kind == VariableKind.Local && !context.Function.Method.IsConstructor) {
							// on statement level (no other expressions on IL stack),
							// prefer to keep local variables (but not stack slots),
							// unless we are in a constructor (where inlining object initializers might be critical
							// for the base ctor call)
							return false;
						}
						// Do not try to transform display class usages or delegate construction.
						// DelegateConstruction transform cannot deal with this.
						if (DelegateConstruction.IsSimpleDisplayClass(newObjInst.Method.DeclaringType))
							return false;
						if (DelegateConstruction.IsDelegateConstruction(newObjInst) || DelegateConstruction.IsPotentialClosure(context, newObjInst))
							return false;
						instType = newObjInst.Method.DeclaringType;
						break;
					case DefaultValue defaultVal:
						instType = defaultVal.Type;
						break;
					case Block existingInitializer:
						if (existingInitializer.Type == BlockType.CollectionInitializer || existingInitializer.Type == BlockType.ObjectInitializer) {
							initializerBlock = existingInitializer;
							var value = ((StLoc)initializerBlock.Instructions[0]).Value;
							if (value is NewObj no)
								instType = no.Method.DeclaringType;
							else
								instType = ((DefaultValue)value).Type;
							break;
						}
						return false;
					default:
						return false;
				}
				int initializerItemsCount = 0;
				var blockType = initializerBlock?.Type ?? BlockType.CollectionInitializer;
				var possibleIndexVariables = new Dictionary<ILVariable, (int Index, ILInstruction Value)>();
				// Detect initializer type by scanning the following statements
				// each must be a callvirt with ldloc v as first argument
				// if the method is a setter we're dealing with an object initializer
				// if the method is named Add and has at least 2 arguments we're dealing with a collection/dictionary initializer
				while (pos + initializerItemsCount + 1 < body.Instructions.Count
					&& IsPartOfInitializer(body.Instructions, pos + initializerItemsCount + 1, v, instType, ref blockType, possibleIndexVariables)) {
					initializerItemsCount++;
				}
				var index = possibleIndexVariables.Where(info => info.Value.Index > -1).Min(info => (int?)info.Value.Index);
				if (index != null) {
					initializerItemsCount = index.Value - pos - 1;
				}
				if (initializerItemsCount <= 0)
					return false;
				context.Step("CollectionOrObjectInitializer", inst);
				ILVariable finalSlot;
				if (initializerBlock == null) {
					initializerBlock = new Block(blockType);
					finalSlot = context.Function.RegisterVariable(VariableKind.InitializerTarget, v.Type);
					initializerBlock.FinalInstruction = new LdLoc(finalSlot);
					initializerBlock.Instructions.Add(new StLoc(finalSlot, initInst.Clone()));
				} else {
					finalSlot = ((LdLoc)initializerBlock.FinalInstruction).Variable;
				}
				for (int i = 1; i <= initializerItemsCount; i++) {
					switch (body.Instructions[i + pos]) {
						case CallInstruction call:
							if (!(call is CallVirt || call is Call)) continue;
							var newCall = (CallInstruction)call.Clone();
							var newTarget = newCall.Arguments[0];
							foreach (var load in newTarget.Descendants.OfType<IInstructionWithVariableOperand>())
								if ((load is LdLoc || load is LdLoca) && load.Variable == v)
									load.Variable = finalSlot;
							initializerBlock.Instructions.Add(newCall);
							break;
						case StObj stObj:
							var newStObj = (StObj)stObj.Clone();
							foreach (var load in newStObj.Target.Descendants.OfType<IInstructionWithVariableOperand>())
								if ((load is LdLoc || load is LdLoca) && load.Variable == v)
									load.Variable = finalSlot;
							initializerBlock.Instructions.Add(newStObj);
							break;
						case StLoc stLoc:
							var newStLoc = (StLoc)stLoc.Clone();
							initializerBlock.Instructions.Add(newStLoc);
							break;
					}
				}
				initInst.ReplaceWith(initializerBlock);
				body.Instructions.RemoveRange(pos + 1, initializerItemsCount);
				ILInlining.InlineIfPossible(body, pos, context);
			}
			return true;
		}

		bool IsPartOfInitializer(InstructionCollection<ILInstruction> instructions, int pos, ILVariable target, IType rootType, ref BlockType blockType, Dictionary<ILVariable, (int Index, ILInstruction Value)> possibleIndexVariables)
		{
			if (instructions[pos] is StLoc stloc && stloc.Variable.Kind == VariableKind.Local && stloc.Variable.IsSingleDefinition) {
				if (stloc.Value.Descendants.OfType<IInstructionWithVariableOperand>().Any(ld => ld.Variable == target && (ld is LdLoc || ld is LdLoca)))
					return false;
				possibleIndexVariables.Add(stloc.Variable, (stloc.ChildIndex, stloc.Value));
				return true;
			}
			(var kind, var path, var values, var targetVariable) = AccessPathElement.GetAccessPath(instructions[pos], rootType, possibleIndexVariables);
			switch (kind) {
				case AccessPathKind.Adder:
					return target == targetVariable;
				case AccessPathKind.Setter:
					if (values.Count == 1 && target == targetVariable) {
						blockType = BlockType.ObjectInitializer;
						return true;
					}
					return false;
				default:
					return false;
			}
		}

		private string SpecialToString(ILInstruction instruction)
		{
			var output = new PlainTextOutput();
			instruction.WriteTo(output, new ILAstWritingOptions());
			return output.ToString();
		}

		private bool CheckSetter(ILInstruction inst, string propertyName, ILInstruction onObject)
		{
			// inst: callvirt set...(callvirt get...(callvirt get_Content(ldloc obj)), newobj)
			// propertyName: Content
			// onObject: ldloc obj
			foreach (CallInstruction callInst in inst.Descendants.OfType<CallInstruction>()) {
				if (callInst.Method.FullName.Replace("get", "").Equals(propertyName)) {
					// callvirt get_Content(ldloc obj)
					if (SpecialToString(callInst.Arguments[0]).Equals(SpecialToString(onObject)))
						return true;
				}
			}
			return false;
		}
	}

	public enum AccessPathKind
	{
		Invalid,
		Setter,
		Adder
	}

	public struct AccessPathElement : IEquatable<AccessPathElement>
	{
		public AccessPathElement(IMember member, ILInstruction[] indices = null)
		{
			this.Member = member;
			this.Indices = indices;
		}

		public readonly IMember Member;
		public readonly ILInstruction[] Indices;

		public override string ToString() => $"[{Member}, {Indices}]";

		public static (AccessPathKind Kind, List<AccessPathElement> Path, List<ILInstruction> Values, ILVariable Target) GetAccessPath(ILInstruction instruction, IType rootType, Dictionary<ILVariable, (int Index, ILInstruction Value)> possibleIndexVariables = null)
		{
			List<AccessPathElement> path = new List<AccessPathElement>();
			ILVariable target = null;
			AccessPathKind kind = AccessPathKind.Invalid;
			List<ILInstruction> values = null;
			IMethod method;
			var inst = instruction;
			while (instruction != null) {
				switch (instruction) {
					case CallInstruction call:
						if (!(call is CallVirt || call is Call)) goto default;
						method = call.Method;
						if (!IsMethodApplicable(method, call.Arguments, rootType)) goto default;
						instruction = call.Arguments[0];
						if (method.IsAccessor) {
							var property = method.AccessorOwner as IProperty;
							var isGetter = property?.Getter == method;
							var indices = call.Arguments.Skip(1).Take(call.Arguments.Count - (isGetter ? 1 : 2)).ToArray();
							if (possibleIndexVariables != null) {
								foreach (var index in indices.OfType<IInstructionWithVariableOperand>()) {
									if (possibleIndexVariables.TryGetValue(index.Variable, out var info))
										possibleIndexVariables[index.Variable] = (-1, info.Value);
								}
							}
							path.Insert(0, new AccessPathElement(method.AccessorOwner, indices));
						} else {
							path.Insert(0, new AccessPathElement(method));
						}
						if (values == null) {
							if (method.IsAccessor) {
								kind = AccessPathKind.Setter;
								values = new List<ILInstruction> { call.Arguments.Last() };
							} else {
								kind = AccessPathKind.Adder;
								values = new List<ILInstruction>(call.Arguments.Skip(1));
								if (values.Count == 0)
									goto default;
							}
						}
						break;
					case LdObj ldobj:
						if (ldobj.Target is LdFlda ldflda) {
							path.Insert(0, new AccessPathElement(ldflda.Field));
							instruction = ldflda.Target;
							break;
						}
						goto default;
					case StObj stobj:
						if (stobj.Target is LdFlda ldflda2) {
							path.Insert(0, new AccessPathElement(ldflda2.Field));
							instruction = ldflda2.Target;
							if (values == null) {
								values = new List<ILInstruction>(new[] { stobj.Value });
								kind = AccessPathKind.Setter;
							}
							break;
						}
						goto default;
					case LdLoc ldloc:
						target = ldloc.Variable;
						instruction = null;
						break;
					case LdLoca ldloca:
						target = ldloca.Variable;
						instruction = null;
						break;
					default:
						kind = AccessPathKind.Invalid;
						instruction = null;
						break;
				}
			}
			if (kind != AccessPathKind.Invalid && values.SelectMany(v => v.Descendants).OfType<IInstructionWithVariableOperand>().Any(ld => ld.Variable == target && (ld is LdLoc || ld is LdLoca)))
				kind = AccessPathKind.Invalid;
			return (kind, path, values, target);
		}

		static bool IsMethodApplicable(IMethod method, IList<ILInstruction> arguments, IType rootType)
		{
			if (!method.IsExtensionMethod && method.IsStatic)
				return false;
			if (method.IsAccessor)
				return true;
			if (!"Add".Equals(method.Name, StringComparison.Ordinal) || arguments.Count == 0)
				return false;
			var targetType = GetReturnTypeFromInstruction(arguments[0]) ?? rootType;
			if (targetType == null)
				return false;
			return targetType.GetAllBaseTypes().Any(i => i.IsKnownType(KnownTypeCode.IEnumerable) || i.IsKnownType(KnownTypeCode.IEnumerableOfT));
		}

		static IType GetReturnTypeFromInstruction(ILInstruction instruction)
		{
			switch (instruction) {
				case CallInstruction call:
					if (!(call is CallVirt || call is Call)) goto default;
					return call.Method.ReturnType;
				case LdObj ldobj:
					if (ldobj.Target is LdFlda ldflda)
						return ldflda.Field.ReturnType;
					goto default;
				case StObj stobj:
					if (stobj.Target is LdFlda ldflda2)
						return ldflda2.Field.ReturnType;
					goto default;
				default:
					return null;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is AccessPathElement)
				return Equals((AccessPathElement)obj);
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (Member != null)
					hashCode += 1000000007 * Member.GetHashCode();
			}
			return hashCode;
		}

		public bool Equals(AccessPathElement other)
		{
			return other.Member.Equals(this.Member) && (other.Indices == this.Indices || other.Indices.SequenceEqual(this.Indices, ILInstructionMatchComparer.Instance));
		}

		public static bool operator ==(AccessPathElement lhs, AccessPathElement rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(AccessPathElement lhs, AccessPathElement rhs)
		{
			return !(lhs == rhs);
		}
	}

	class ILInstructionMatchComparer : IEqualityComparer<ILInstruction>
	{
		public static readonly ILInstructionMatchComparer Instance = new ILInstructionMatchComparer();

		public bool Equals(ILInstruction x, ILInstruction y)
		{
			if (x == y)
				return true;
			if (x == null || y == null)
				return false;
			return x.Match(y).Success;
		}

		public int GetHashCode(ILInstruction obj)
		{
			return obj.GetHashCode();
		}
	}
}
