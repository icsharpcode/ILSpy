﻿// Copyright (c) 2017 Siegfried Pammer
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

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.TypeSystem;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transforms collection and object initialization patterns.
	/// </summary>
	public class TransformCollectionAndObjectInitializers : IStatementTransform
	{
		void IStatementTransform.Run(Block block, int pos, StatementTransformContext context)
		{
			if (!context.Settings.ObjectOrCollectionInitializers)
				return;
			ILInstruction inst = block.Instructions[pos];
			// Match stloc(v, newobj)
			if (!inst.MatchStLoc(out var v, out var initInst) || v.Kind != VariableKind.Local && v.Kind != VariableKind.StackSlot)
				return;
			IType instType;
			var blockKind = BlockKind.CollectionInitializer;
			var insertionPos = initInst.ChildIndex;
			var siblings = initInst.Parent!.Children;
			IMethod currentMethod = context.Function.Method!;
			switch (initInst)
			{
				case NewObj newObjInst:
					if (newObjInst.ILStackWasEmpty && v.Kind == VariableKind.Local
						&& !currentMethod.IsConstructor
						&& !currentMethod.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
					{
						// on statement level (no other expressions on IL stack),
						// prefer to keep local variables (but not stack slots),
						// unless we are in a constructor (where inlining object initializers might be critical
						// for the base ctor call) or a compiler-generated delegate method, which might be used in a query expression.
						return;
					}
					// Do not try to transform delegate construction.
					// DelegateConstruction transform cannot deal with this.
					if (DelegateConstruction.MatchDelegateConstruction(newObjInst, out _, out _, out _)
						|| TransformDisplayClassUsage.IsPotentialClosure(context, newObjInst))
						return;
					// Cannot build a collection/object initializer attached to an AnonymousTypeCreateExpression
					// anon = new { A = 5 } { 3,4,5 } is invalid syntax.
					if (newObjInst.Method.DeclaringType.ContainsAnonymousType())
						return;
					instType = newObjInst.Method.DeclaringType;
					break;
				case DefaultValue defaultVal:
					instType = defaultVal.Type;
					break;
				case Call c when c.Method.FullNameIs("System.Activator", "CreateInstance") && c.Method.TypeArguments.Count == 1:
					instType = c.Method.TypeArguments[0];
					blockKind = BlockKind.ObjectInitializer;
					break;
				case CallInstruction ci when context.Settings.WithExpressions && IsRecordCloneMethodCall(ci):
					instType = ci.Method.DeclaringType;
					blockKind = BlockKind.WithInitializer;
					initInst = ci.Arguments.Single();
					break;
				default:
					var typeDef = v.Type.GetDefinition();
					if (context.Settings.WithExpressions && typeDef?.IsReferenceType == false && typeDef.IsRecord)
					{
						instType = v.Type;
						blockKind = BlockKind.WithInitializer;
						break;
					}
					return;
			}
			int initializerItemsCount = 0;
			bool initializerContainsInitOnlyItems = false;
			possibleIndexVariables.Clear();
			currentPath.Clear();
			isCollection = false;
			pathStack.Clear();
			pathStack.Push(new HashSet<AccessPathElement>());
			// Detect initializer type by scanning the following statements
			// each must be a callvirt with ldloc v as first argument
			// if the method is a setter we're dealing with an object initializer
			// if the method is named Add and has at least 2 arguments we're dealing with a collection/dictionary initializer
			while (pos + initializerItemsCount + 1 < block.Instructions.Count
				&& IsPartOfInitializer(block.Instructions, pos + initializerItemsCount + 1, v, instType, ref blockKind, ref initializerContainsInitOnlyItems, context))
			{
				initializerItemsCount++;
			}
			// Do not convert the statements into an initializer if there's an incompatible usage of the initializer variable
			// directly after the possible initializer.
			if (!initializerContainsInitOnlyItems && IsMethodCallOnVariable(block.Instructions[pos + initializerItemsCount + 1], v))
				return;
			// Calculate the correct number of statements inside the initializer:
			// All index variables that were used in the initializer have Index set to -1.
			// We fetch the first unused variable from the list and remove all instructions after its
			// first usage (i.e. the init store) from the initializer.
			var index = possibleIndexVariables.Where(info => info.Value.Index > -1).Min(info => (int?)info.Value.Index);
			if (index != null)
			{
				initializerItemsCount = index.Value - pos - 1;
			}
			// The initializer would be empty, there's nothing to do here.
			if (initializerItemsCount <= 0)
				return;
			context.Step("CollectionOrObjectInitializer", inst);
			// Create a new block and final slot (initializer target variable)
			var initializerBlock = new Block(blockKind);
			ILVariable finalSlot = context.Function.RegisterVariable(VariableKind.InitializerTarget, instType);
			initializerBlock.FinalInstruction = new LdLoc(finalSlot);
			initializerBlock.Instructions.Add(new StLoc(finalSlot, initInst));
			// Move all instructions to the initializer block.
			for (int i = 1; i <= initializerItemsCount; i++)
			{
				switch (block.Instructions[i + pos])
				{
					case CallInstruction call:
						if (!(call is CallVirt || call is Call))
							continue;
						var newCall = call;
						var newTarget = newCall.Arguments[0];
						foreach (var load in newTarget.Descendants.OfType<IInstructionWithVariableOperand>())
							if ((load is LdLoc || load is LdLoca) && load.Variable == v)
								load.Variable = finalSlot;
						initializerBlock.Instructions.Add(newCall);
						break;
					case StObj stObj:
						var newStObj = stObj;
						foreach (var load in newStObj.Target.Descendants.OfType<IInstructionWithVariableOperand>())
							if ((load is LdLoc || load is LdLoca) && load.Variable == v)
								load.Variable = finalSlot;
						initializerBlock.Instructions.Add(newStObj);
						break;
					case StLoc stLoc:
						var newStLoc = stLoc;
						initializerBlock.Instructions.Add(newStLoc);
						break;
				}
			}
			block.Instructions.RemoveRange(pos + 1, initializerItemsCount);
			siblings[insertionPos] = initializerBlock;
			ILInlining.InlineIfPossible(block, pos, context);
		}

		internal static bool IsRecordCloneMethodCall(CallInstruction ci)
		{
			if (ci.Method.DeclaringTypeDefinition?.IsRecord != true)
				return false;
			if (ci.Method.Name != "<Clone>$")
				return false;
			if (ci.Arguments.Count != 1)
				return false;

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

		readonly Dictionary<ILVariable, (int Index, ILInstruction Value)> possibleIndexVariables = new Dictionary<ILVariable, (int Index, ILInstruction Value)>();
		readonly List<AccessPathElement> currentPath = new List<AccessPathElement>();
		bool isCollection;
		readonly Stack<HashSet<AccessPathElement>> pathStack = new Stack<HashSet<AccessPathElement>>();

		bool IsPartOfInitializer(InstructionCollection<ILInstruction> instructions, int pos, ILVariable target, IType rootType, ref BlockKind blockKind, ref bool initializerContainsInitOnlyItems, StatementTransformContext context)
		{
			// Include any stores to local variables that are single-assigned and do not reference the initializer-variable
			// in the list of possible index variables.
			// Index variables are used to implement dictionary initializers.
			if (instructions[pos] is StLoc stloc && stloc.Variable.Kind == VariableKind.Local && stloc.Variable.IsSingleDefinition)
			{
				if (!context.Settings.DictionaryInitializers)
					return false;
				if (stloc.Value.Descendants.OfType<IInstructionWithVariableOperand>().Any(ld => ld.Variable == target && (ld is LdLoc || ld is LdLoca)))
					return false;
				possibleIndexVariables.Add(stloc.Variable, (stloc.ChildIndex, stloc.Value));
				return true;
			}
			var resolveContext = new CSharpTypeResolveContext(context.TypeSystem.MainModule, context.UsingScope);
			(var kind, var newPath, var values, var targetVariable) = AccessPathElement.GetAccessPath(instructions[pos], rootType, context.Settings, resolveContext, possibleIndexVariables);
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
			while (currentPath.Count > firstDifferenceIndex)
			{
				isCollection = false;
				currentPath.RemoveAt(currentPath.Count - 1);
				pathStack.Pop();
			}
			while (currentPath.Count < newPath.Count)
			{
				AccessPathElement newElement = newPath[currentPath.Count];
				currentPath.Add(newElement);
				if (isCollection || !pathStack.Peek().Add(newElement))
					return false;
				pathStack.Push(new HashSet<AccessPathElement>());
			}
			switch (kind)
			{
				case AccessPathKind.Adder:
					isCollection = true;
					if (pathStack.Peek().Count != 0)
						return false;
					return true;
				case AccessPathKind.Setter:
					if (isCollection || !pathStack.Peek().Add(lastElement))
						return false;
					if (values?.Count != 1 || !IsValidObjectInitializerTarget(currentPath))
						return false;
					if (blockKind != BlockKind.ObjectInitializer && blockKind != BlockKind.WithInitializer)
						blockKind = BlockKind.ObjectInitializer;
					initializerContainsInitOnlyItems |= lastElement.Member is IProperty { Setter.IsInitOnly: true };
					return true;
				default:
					return false;
			}
		}

		bool IsValidObjectInitializerTarget(List<AccessPathElement> path)
		{
			if (path.Count == 0)
				return true;
			var element = path.Last();
			var previous = path.SkipLast(1).LastOrDefault();
			if (element.Member is not IProperty p)
				return true;
			if (!p.IsIndexer)
				return true;
			if (previous != default)
			{
				return NormalizeTypeVisitor.IgnoreNullabilityAndTuples
					.EquivalentTypes(previous.Member.ReturnType, element.Member.DeclaringType);
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
		public AccessPathElement(OpCode opCode, IMember member, ILInstruction[]? indices = null)
		{
			this.OpCode = opCode;
			this.Member = member;
			this.Indices = indices;
		}

		public readonly OpCode OpCode;
		public readonly IMember Member;
		public readonly ILInstruction[]? Indices;

		public override string ToString() => $"[{Member}, {Indices}]";

		public static (AccessPathKind Kind, List<AccessPathElement> Path, List<ILInstruction>? Values, ILVariable? Target) GetAccessPath(
			ILInstruction instruction, IType rootType, DecompilerSettings? settings = null,
			CSharpTypeResolveContext? resolveContext = null,
			Dictionary<ILVariable, (int Index, ILInstruction Value)>? possibleIndexVariables = null)
		{
			List<AccessPathElement> path = new List<AccessPathElement>();
			ILVariable? target = null;
			AccessPathKind kind = AccessPathKind.Invalid;
			List<ILInstruction>? values = null;
			IMethod method;
			ILInstruction? inst = instruction;
			while (inst != null)
			{
				switch (inst)
				{
					case CallInstruction call:
						if (!(call is CallVirt || call is Call))
							goto default;
						method = call.Method;
						if (resolveContext != null && !IsMethodApplicable(method, call.Arguments, rootType, resolveContext, settings))
							goto default;
						inst = call.Arguments[0];
						if (method.IsAccessor)
						{
							if (method.AccessorOwner is IProperty property &&
								!CanBeUsedInInitializer(property, resolveContext, kind))
							{
								goto default;
							}

							var isGetter = method.AccessorKind == System.Reflection.MethodSemanticsAttributes.Getter;
							var indices = call.Arguments.Skip(1).Take(call.Arguments.Count - (isGetter ? 1 : 2)).ToArray();
							if (indices.Length > 0 && settings?.DictionaryInitializers == false)
								goto default;
							if (possibleIndexVariables != null)
							{
								// Mark all index variables as used
								foreach (var index in indices.OfType<IInstructionWithVariableOperand>())
								{
									if (possibleIndexVariables.TryGetValue(index.Variable, out var info))
										possibleIndexVariables[index.Variable] = (-1, info.Value);
								}
							}
							path.Insert(0, new AccessPathElement(call.OpCode, method.AccessorOwner, indices));
						}
						else
						{
							path.Insert(0, new AccessPathElement(call.OpCode, method));
						}
						if (values == null)
						{
							if (method.IsAccessor)
							{
								kind = AccessPathKind.Setter;
								values = new List<ILInstruction> { call.Arguments.Last() };
							}
							else
							{
								kind = AccessPathKind.Adder;
								values = new List<ILInstruction>(call.Arguments.Skip(1));
								if (values.Count == 0)
									goto default;
							}
						}
						break;
					case LdObj ldobj:
					{
						if (ldobj.Target is LdFlda ldflda && (kind != AccessPathKind.Setter || !ldflda.Field.IsReadOnly))
						{
							path.Insert(0, new AccessPathElement(ldobj.OpCode, ldflda.Field));
							inst = ldflda.Target;
							break;
						}
						goto default;
					}
					case StObj stobj:
					{
						if (stobj.Target is LdFlda ldflda)
						{
							path.Insert(0, new AccessPathElement(stobj.OpCode, ldflda.Field));
							inst = ldflda.Target;
							if (values == null)
							{
								values = new List<ILInstruction>(new[] { stobj.Value });
								kind = AccessPathKind.Setter;
							}
							break;
						}
						goto default;
					}
					case LdLoc ldloc:
						target = ldloc.Variable;
						inst = null;
						break;
					case LdLoca ldloca:
						target = ldloca.Variable;
						inst = null;
						break;
					case LdFlda ldflda:
						path.Insert(0, new AccessPathElement(ldflda.OpCode, ldflda.Field));
						inst = ldflda.Target;
						break;
					default:
						kind = AccessPathKind.Invalid;
						inst = null;
						break;
				}
			}
			if (kind != AccessPathKind.Invalid && values != null && values.SelectMany(v => v.Descendants).OfType<IInstructionWithVariableOperand>().Any(ld => ld.Variable == target && (ld is LdLoc || ld is LdLoca)))
				kind = AccessPathKind.Invalid;
			return (kind, path, values, target);
		}

		private static bool CanBeUsedInInitializer(IProperty property, CSharpTypeResolveContext? resolveContext, AccessPathKind kind)
		{
			if (property.CanSet && (property.Accessibility == property.Setter.Accessibility || IsAccessorAccessible(property.Setter, resolveContext)))
				return true;
			return kind != AccessPathKind.Setter;
		}

		private static bool IsAccessorAccessible(IMethod setter, CSharpTypeResolveContext? resolveContext)
		{
			if (resolveContext == null)
				return true;
			var lookup = new MemberLookup(resolveContext.CurrentTypeDefinition, resolveContext.CurrentModule);
			return lookup.IsAccessible(setter, allowProtectedAccess: setter.DeclaringTypeDefinition == resolveContext.CurrentTypeDefinition);
		}

		static bool IsMethodApplicable(IMethod method, IReadOnlyList<ILInstruction> arguments, IType rootType, CSharpTypeResolveContext resolveContext, DecompilerSettings? settings)
		{
			if (method.IsStatic && !method.IsExtensionMethod)
				return false;
			if (method.AccessorOwner is IProperty)
				return true;
			if (!"Add".Equals(method.Name, StringComparison.Ordinal) || arguments.Count == 0)
				return false;
			if (method.IsExtensionMethod)
			{
				if (settings?.ExtensionMethodsInCollectionInitializers == false)
					return false;
				if (!CSharp.Transforms.IntroduceExtensionMethods.CanTransformToExtensionMethodCall(method, resolveContext, ignoreTypeArguments: true))
					return false;
			}
			var targetType = GetReturnTypeFromInstruction(arguments[0]) ?? rootType;
			if (targetType == null)
				return false;
			if (!targetType.GetAllBaseTypes().Any(i => i.IsKnownType(KnownTypeCode.IEnumerable) || i.IsKnownType(KnownTypeCode.IEnumerableOfT)))
				return false;
			return CanInferTypeArgumentsFromParameters(method);

			bool CanInferTypeArgumentsFromParameters(IMethod method)
			{
				if (method.TypeParameters.Count == 0)
					return true;
				// always use unspecialized member, otherwise type inference fails
				method = (IMethod)method.MemberDefinition;
				new TypeInference(resolveContext.Compilation)
					.InferTypeArguments(
						method.TypeParameters,
						// TODO : this is not entirely correct... we need argument type information to resolve Add methods properly
						method.Parameters.SelectReadOnlyArray(p => new ResolveResult(p.Type)),
						method.Parameters.SelectReadOnlyArray(p => p.Type),
						out bool success
					);
				return success;
			}
		}

		static IType? GetReturnTypeFromInstruction(ILInstruction instruction)
		{
			switch (instruction)
			{
				case CallInstruction call:
					if (!(call is CallVirt || call is Call))
						goto default;
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

		public override bool Equals(object? obj)
		{
			if (obj is AccessPathElement)
				return Equals((AccessPathElement)obj);
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked
			{
				if (Member != null)
					hashCode += 1000000007 * Member.GetHashCode();
			}
			return hashCode;
		}

		public bool Equals(AccessPathElement other)
		{
			return (other.Member == this.Member
				|| this.Member.Equals(other.Member))
				&& (other.Indices == this.Indices
				|| (other.Indices != null && this.Indices != null && this.Indices.SequenceEqual(other.Indices, ILInstructionMatchComparer.Instance)));
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

		public bool Equals(ILInstruction? x, ILInstruction? y)
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
