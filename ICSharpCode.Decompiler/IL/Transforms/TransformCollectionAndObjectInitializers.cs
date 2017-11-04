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
using ICSharpCode.Decompiler.Util;

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
			// Match stloc(v, newobj)
			if (inst.MatchStLoc(out var v, out var initInst) && (v.Kind == VariableKind.Local || v.Kind == VariableKind.StackSlot)) {
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
						if (defaultVal.ILStackWasEmpty && v.Kind == VariableKind.Local && !context.Function.Method.IsConstructor) {
							// on statement level (no other expressions on IL stack),
							// prefer to keep local variables (but not stack slots),
							// unless we are in a constructor (where inlining object initializers might be critical
							// for the base ctor call)
							return false;
						}
						instType = defaultVal.Type;
						break;
					default:
						return false;
				}
				int initializerItemsCount = 0;
				var blockKind = BlockKind.CollectionInitializer;
				possibleIndexVariables = new Dictionary<ILVariable, (int Index, ILInstruction Value)>();
				currentPath = new List<AccessPathElement>();
				isCollection = false;
				pathStack = new Stack<HashSet<AccessPathElement>>();
				pathStack.Push(new HashSet<AccessPathElement>());
				// Detect initializer type by scanning the following statements
				// each must be a callvirt with ldloc v as first argument
				// if the method is a setter we're dealing with an object initializer
				// if the method is named Add and has at least 2 arguments we're dealing with a collection/dictionary initializer
				while (pos + initializerItemsCount + 1 < body.Instructions.Count
					&& IsPartOfInitializer(body.Instructions, pos + initializerItemsCount + 1, v, instType, ref blockKind)) {
					initializerItemsCount++;
				}
				// Do not convert the statements into an initializer if there's an incompatible usage of the initializer variable
				// directly after the possible initializer.
				if (IsMethodCallOnVariable(body.Instructions[pos + initializerItemsCount + 1], v))
					return false;
				// Calculate the correct number of statements inside the initializer:
				// All index variables that were used in the initializer have Index set to -1.
				// We fetch the first unused variable from the list and remove all instructions after its
				// first usage (i.e. the init store) from the initializer.
				var index = possibleIndexVariables.Where(info => info.Value.Index > -1).Min(info => (int?)info.Value.Index);
				if (index != null) {
					initializerItemsCount = index.Value - pos - 1;
				}
				// The initializer would be empty, there's nothing to do here.
				if (initializerItemsCount <= 0)
					return false;
				context.Step("CollectionOrObjectInitializer", inst);
				// Create a new block and final slot (initializer target variable)
				var initializerBlock = new Block(blockKind);
				ILVariable finalSlot = context.Function.RegisterVariable(VariableKind.InitializerTarget, v.Type);
				initializerBlock.FinalInstruction = new LdLoc(finalSlot);
				initializerBlock.Instructions.Add(new StLoc(finalSlot, initInst.Clone()));
				// Move all instructions to the initializer block.
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

		bool IsMethodCallOnVariable(ILInstruction inst, ILVariable variable)
		{
			if (inst.MatchLdLocRef(variable))
				return true;
			if (inst is CallInstruction call && call.Arguments.Count > 0 && !call.Method.IsStatic)
				return IsMethodCallOnVariable(call.Arguments[0], variable);
			if (inst.MatchLdFld(out var target, out _) || inst.MatchStFld(out target, out _, out _) || inst.MatchLdFlda(out target, out _))
				return IsMethodCallOnVariable(target, variable);
			return false;
		}

		Dictionary<ILVariable, (int Index, ILInstruction Value)> possibleIndexVariables;
		List<AccessPathElement> currentPath;
		bool isCollection;
		Stack<HashSet<AccessPathElement>> pathStack;

		bool IsPartOfInitializer(InstructionCollection<ILInstruction> instructions, int pos, ILVariable target, IType rootType, ref BlockKind blockKind)
		{
			// Include any stores to local variables that are single-assigned and do not reference the initializer-variable
			// in the list of possible index variables.
			// Index variables are used to implement dictionary initializers.
			if (instructions[pos] is StLoc stloc && stloc.Variable.Kind == VariableKind.Local && stloc.Variable.IsSingleDefinition) {
				if (stloc.Value.Descendants.OfType<IInstructionWithVariableOperand>().Any(ld => ld.Variable == target && (ld is LdLoc || ld is LdLoca)))
					return false;
				possibleIndexVariables.Add(stloc.Variable, (stloc.ChildIndex, stloc.Value));
				return true;
			}
			(var kind, var newPath, var values, var targetVariable) = AccessPathElement.GetAccessPath(instructions[pos], rootType, possibleIndexVariables);
			if (kind == AccessPathKind.Invalid || target != targetVariable)
				return false;
			// Treat last element separately:
			// Can either be an Add method call or property setter.
			var lastElement = newPath.Last();
			newPath.RemoveLast();
			// Compare new path with current path:
			int minLen = Math.Min(currentPath.Count, newPath.Count);
			int firstDifferenceIndex = 0;
			while (firstDifferenceIndex < minLen && newPath[firstDifferenceIndex] == currentPath[firstDifferenceIndex])
				firstDifferenceIndex++;
			while (currentPath.Count > firstDifferenceIndex) {
				isCollection = false;
				currentPath.RemoveAt(currentPath.Count - 1);
				pathStack.Pop();
			}
			while (currentPath.Count < newPath.Count) {
				AccessPathElement newElement = newPath[currentPath.Count];
				currentPath.Add(newElement);
				if (isCollection || !pathStack.Peek().Add(newElement))
					return false;
				pathStack.Push(new HashSet<AccessPathElement>());
			}
			switch (kind) {
				case AccessPathKind.Adder:
					isCollection = true;
					if (pathStack.Peek().Count != 0)
						return false;
					return true;
				case AccessPathKind.Setter:
					if (isCollection || !pathStack.Peek().Add(lastElement))
						return false;
					if (values.Count == 1) {
						blockKind = BlockKind.ObjectInitializer;
						return true;
					}
					return false;
				default:
					return false;
			}
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

		public static (AccessPathKind Kind, List<AccessPathElement> Path, List<ILInstruction> Values, ILVariable Target) GetAccessPath(
			ILInstruction instruction, IType rootType, Dictionary<ILVariable, (int Index, ILInstruction Value)> possibleIndexVariables = null)
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
								// Mark all index variables as used
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
					case LdObj ldobj: {
						if (ldobj.Target is LdFlda ldflda) {
							path.Insert(0, new AccessPathElement(ldflda.Field));
							instruction = ldflda.Target;
							break;
						}
						goto default;
					}
					case StObj stobj: {
						if (stobj.Target is LdFlda ldflda) {
							path.Insert(0, new AccessPathElement(ldflda.Field));
							instruction = ldflda.Target;
							if (values == null) {
								values = new List<ILInstruction>(new[] { stobj.Value });
								kind = AccessPathKind.Setter;
							}
							break;
						}
						goto default;
					}
					case LdLoc ldloc:
						target = ldloc.Variable;
						instruction = null;
						break;
					case LdLoca ldloca:
						target = ldloca.Variable;
						instruction = null;
						break;
					case LdFlda ldflda:
						path.Insert(0, new AccessPathElement(ldflda.Field));
						instruction = ldflda.Target;
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
			return other.Member.Equals(this.Member)
				&& (other.Indices == this.Indices || other.Indices.SequenceEqual(this.Indices, ILInstructionMatchComparer.Instance));
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
			return SemanticHelper.IsPure(x.Flags)
				&& SemanticHelper.IsPure(y.Flags)
				&& x.Match(y).Success;
		}

		public int GetHashCode(ILInstruction obj)
		{
			throw new NotSupportedException();
		}
	}
}
