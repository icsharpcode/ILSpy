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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

using Humanizer.Inflections;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class AssignVariableNames : ILVisitor<AssignVariableNames.VariableScope, Unit>, IILTransform
	{
		static readonly Dictionary<string, string> typeNameToVariableNameDict = new Dictionary<string, string> {
			{ "System.Boolean", "flag" },
			{ "System.Byte", "b" },
			{ "System.SByte", "b" },
			{ "System.Int16", "num" },
			{ "System.Int32", "num" },
			{ "System.Int64", "num" },
			{ "System.UInt16", "num" },
			{ "System.UInt32", "num" },
			{ "System.UInt64", "num" },
			{ "System.Single", "num" },
			{ "System.Double", "num" },
			{ "System.Decimal", "num" },
			{ "System.String", "text" },
			{ "System.Object", "obj" },
			{ "System.Char", "c" }
		};

		ILTransformContext context;
		Queue<(ILFunction function, VariableScope parentScope)> workList;
		const char maxLoopVariableName = 'n';

		public class VariableScope
		{
			readonly ILTransformContext context;
			readonly VariableScope parentScope;
			readonly ILFunction function;
			readonly Dictionary<MethodDefinitionHandle, string> localFunctions = new();
			readonly Dictionary<ILVariable, string> variableMapping = new(ILVariableEqualityComparer.Instance);
			readonly string[] assignedLocalSignatureIndices;

			IImmutableSet<string> currentLowerCaseTypeOrMemberNames;
			Dictionary<string, int> reservedVariableNames;
			HashSet<ILVariable> loopCounters;
			int numDisplayClassLocals;

			public VariableScope(ILFunction function, ILTransformContext context, VariableScope parentScope = null)
			{
				this.function = function;
				this.context = context;
				this.parentScope = parentScope;

				numDisplayClassLocals = 0;
				assignedLocalSignatureIndices = new string[function.LocalVariableSignatureLength];
				reservedVariableNames = new Dictionary<string, int>();

				// find all loop counters in the current function
				loopCounters = new HashSet<ILVariable>();
				foreach (var inst in TreeTraversal.PreOrder((ILInstruction)function, i => i.Children))
				{
					if (inst is ILFunction && inst != function)
						break;
					if (inst is BlockContainer { Kind: ContainerKind.For, Blocks: [.., var incrementBlock] })
					{
						foreach (var i in incrementBlock.Instructions)
						{
							if (HighLevelLoopTransform.MatchIncrement(i, out var variable))
								loopCounters.Add(variable);
						}
					}
				}

				// if this is the root scope, we also collect all lower-case type and member names
				// and fixed parameter names to avoid conflicts when naming local variables.
				if (parentScope == null)
				{
					var currentLowerCaseTypeOrMemberNames = new HashSet<string>(StringComparer.Ordinal);
					foreach (var name in CollectAllLowerCaseMemberNames(function.Method.DeclaringTypeDefinition))
						currentLowerCaseTypeOrMemberNames.Add(name);
					foreach (var name in CollectAllLowerCaseTypeNames(function.Method.DeclaringTypeDefinition))
					{
						currentLowerCaseTypeOrMemberNames.Add(name);
						AddExistingName(reservedVariableNames, name);
					}
					this.currentLowerCaseTypeOrMemberNames = currentLowerCaseTypeOrMemberNames.ToImmutableHashSet();

					// handle implicit parameters of set or event accessors
					if (function.Method != null && IsSetOrEventAccessor(function.Method) && function.Parameters.Count > 0)
					{
						for (int i = 0; i < function.Method.Parameters.Count - 1; i++)
						{
							AddExistingName(reservedVariableNames, function.Method.Parameters[i].Name);
						}
						var lastParameter = function.Method.Parameters.Last();
						switch (function.Method.AccessorOwner)
						{
							case IProperty prop:
								if (function.Method.AccessorKind == MethodSemanticsAttributes.Setter)
								{
									if (prop.Parameters.Any(p => p.Name == "value"))
									{
										function.Warnings.Add("Parameter named \"value\" already present in property signature!");
										break;
									}
									var variableForLastParameter = function.Variables.FirstOrDefault(v => v.Function == function
										&& v.Kind == VariableKind.Parameter
										&& v.Index == function.Method.Parameters.Count - 1);
									if (variableForLastParameter == null)
									{
										AddExistingName(reservedVariableNames, lastParameter.Name);
									}
									else
									{
										if (variableForLastParameter.Name != "value")
										{
											variableForLastParameter.Name = "value";
										}
										AddExistingName(reservedVariableNames, variableForLastParameter.Name);
									}
								}
								break;
							case IEvent ev:
								if (function.Method.AccessorKind != MethodSemanticsAttributes.Raiser)
								{
									var variableForLastParameter = function.Variables.FirstOrDefault(v => v.Function == function
										&& v.Kind == VariableKind.Parameter
										&& v.Index == function.Method.Parameters.Count - 1);
									if (variableForLastParameter == null)
									{
										AddExistingName(reservedVariableNames, lastParameter.Name);
									}
									else
									{
										if (variableForLastParameter.Name != "value")
										{
											variableForLastParameter.Name = "value";
										}
										AddExistingName(reservedVariableNames, variableForLastParameter.Name);
									}
								}
								break;
							default:
								AddExistingName(reservedVariableNames, lastParameter.Name);
								break;
						}
					}
					else
					{
						var variables = function.Variables.Where(v => v.Kind == VariableKind.Parameter && v.Index >= 0).ToDictionary(v => v.Index);
						foreach (var (i, p) in function.Parameters.WithIndex())
						{
							string name = p.Name;
							if (string.IsNullOrWhiteSpace(name) && p.Type != SpecialType.ArgList)
							{
								// needs to be consistent with logic in ILReader.CreateILVariable
								name = "P_" + i;
							}
							if (variables.TryGetValue(i, out var v))
								variableMapping[v] = name;
							AddExistingName(reservedVariableNames, name);
						}
					}

					static bool IsSetOrEventAccessor(IMethod method)
					{
						switch (method.AccessorKind)
						{
							case MethodSemanticsAttributes.Setter:
							case MethodSemanticsAttributes.Adder:
							case MethodSemanticsAttributes.Remover:
								return true;
							default:
								return false;
						}
					}
				}
				else
				{
					this.currentLowerCaseTypeOrMemberNames = parentScope.currentLowerCaseTypeOrMemberNames;
					var variables = function.Variables.Where(v => v.Kind == VariableKind.Parameter).ToDictionary(v => v.Index);

					foreach (var (i, p) in function.Parameters.WithIndex())
					{
						string name = string.IsNullOrWhiteSpace(p.Name) ? variables[i].Name : p.Name;

						if (function.Kind is ILFunctionKind.Delegate or ILFunctionKind.ExpressionTree
							&& CSharpDecompiler.IsTransparentIdentifier(name))
						{
							AddExistingName(reservedVariableNames, name);
							if (variables.TryGetValue(i, out var v))
								variableMapping[v] = name;
						}
						string nameWithoutNumber = SplitName(name, out int newIndex);
						if (!parentScope.IsReservedVariableName(nameWithoutNumber, out _))
						{
							AddExistingName(reservedVariableNames, name);
							if (variables.TryGetValue(i, out var v))
								variableMapping[v] = name;
						}
						else if (variables.TryGetValue(i, out var v))
						{
							v.HasGeneratedName = true;
						}
					}
				}
			}

			public void Add(MethodDefinitionHandle localFunction, string name)
			{
				this.localFunctions[localFunction] = name;
			}

			public string TryGetExistingName(MethodDefinitionHandle localFunction)
			{
				if (localFunctions.TryGetValue(localFunction, out var name))
					return name;
				return parentScope?.TryGetExistingName(localFunction);
			}

			public string TryGetExistingName(ILVariable v)
			{
				if (variableMapping.TryGetValue(v, out var name))
					return name;
				return parentScope?.TryGetExistingName(v);
			}

			public string TryGetExistingName(ILFunction function, int index)
			{
				if (this.function == function)
				{
					return this.assignedLocalSignatureIndices[index];
				}
				else
				{
					return parentScope?.TryGetExistingName(function, index);
				}
			}

			public void AssignNameToLocalSignatureIndex(ILFunction function, int index, string name)
			{
				var scope = this;
				while (scope != null && scope.function != function)
					scope = scope.parentScope;
				Debug.Assert(scope != null);
				scope.assignedLocalSignatureIndices[index] = name;
			}

			public bool IsReservedVariableName(string name, out int index)
			{
				if (reservedVariableNames.TryGetValue(name, out index))
					return true;
				return parentScope?.IsReservedVariableName(name, out index) ?? false;
			}

			public void ReserveVariableName(string name, int index = 1)
			{
				reservedVariableNames[name] = index;
			}

			public string NextDisplayClassLocal()
			{
				return parentScope?.NextDisplayClassLocal() ?? "CS$<>8__locals" + (numDisplayClassLocals++);
			}

			public bool IsLoopCounter(ILVariable v)
			{
				return loopCounters.Contains(v) || (parentScope?.IsLoopCounter(v) == true);
			}

			public string AssignNameIfUnassigned(ILVariable v)
			{
				if (variableMapping.TryGetValue(v, out var name))
					return name;
				return AssignName(v);
			}

			public string AssignName(ILVariable v)
			{
				// variable has no valid name
				string newName = v.Name;
				if (v.HasGeneratedName || !IsValidName(newName))
				{
					// don't use the name from the debug symbols if it looks like a generated name
					// generate a new one based on how the variable is used
					newName = GenerateNameForVariable(v);
				}
				// use the existing name and update index appended to future conflicts
				string nameWithoutNumber = SplitName(newName, out int newIndex);
				if (IsReservedVariableName(nameWithoutNumber, out int lastUsedIndex))
				{
					if (v.Type.IsKnownType(KnownTypeCode.Int32) && IsLoopCounter(v))
					{
						// special case for loop counters,
						// we don't want them to be named i, i2, ..., but i, j, ...
						newName = GenerateNameForVariable(v);
						nameWithoutNumber = newName;
						newIndex = 1;
					}
				}
				if (IsReservedVariableName(nameWithoutNumber, out lastUsedIndex))
				{
					// name without number was already used
					if (newIndex > lastUsedIndex)
					{
						// new index is larger than last, so we can use it
					}
					else
					{
						// new index is smaller or equal, so we use the next value
						newIndex = lastUsedIndex + 1;
					}
					// resolve conflicts by appending the index to the new name:
					newName = nameWithoutNumber + newIndex.ToString();
				}
				// update the last used index
				ReserveVariableName(nameWithoutNumber, newIndex);
				variableMapping[v] = newName;
				return newName;
			}

			string GenerateNameForVariable(ILVariable variable)
			{
				string proposedName = null;
				if (variable.Type.IsKnownType(KnownTypeCode.Int32))
				{
					// test whether the variable might be a loop counter
					if (loopCounters.Contains(variable))
					{
						// For loop variables, use i,j,k,l,m,n
						for (char c = 'i'; c <= maxLoopVariableName; c++)
						{
							if (!IsReservedVariableName(c.ToString(), out _))
							{
								proposedName = c.ToString();
								break;
							}
						}
					}
				}
				// The ComponentResourceManager inside InitializeComponent must be named "resources",
				// otherwise the WinForms designer won't load the Form.
				if (CSharp.CSharpDecompiler.IsWindowsFormsInitializeComponentMethod(context.Function.Method) && variable.Type.FullName == "System.ComponentModel.ComponentResourceManager")
				{
					proposedName = "resources";
				}
				if (string.IsNullOrEmpty(proposedName))
				{
					var proposedNameForAddress = variable.AddressInstructions.OfType<LdLoca>()
						.Select(arg => arg.Parent is CallInstruction c ? c.GetParameter(arg.ChildIndex)?.Name : null)
						.Where(arg => !string.IsNullOrWhiteSpace(arg))
						.Except(currentLowerCaseTypeOrMemberNames).ToList();
					if (proposedNameForAddress.Count > 0)
					{
						proposedName = proposedNameForAddress[0];
					}
				}
				if (string.IsNullOrEmpty(proposedName))
				{
					var proposedNameForStores = new HashSet<string>();
					foreach (var store in variable.StoreInstructions)
					{
						if (store is StLoc stloc)
						{
							var name = GetNameFromInstruction(stloc.Value);
							if (!currentLowerCaseTypeOrMemberNames.Contains(name))
								proposedNameForStores.Add(name);
						}
						else if (store is MatchInstruction match && match.SlotInfo == MatchInstruction.SubPatternsSlot)
						{
							var name = GetNameFromInstruction(match.TestedOperand);
							if (!currentLowerCaseTypeOrMemberNames.Contains(name))
								proposedNameForStores.Add(name);
						}
						else if (store is PinnedRegion pinnedRegion)
						{
							var name = GetNameFromInstruction(pinnedRegion.Init);
							if (!currentLowerCaseTypeOrMemberNames.Contains(name))
								proposedNameForStores.Add(name);
						}
					}
					if (proposedNameForStores.Count == 1)
					{
						proposedName = proposedNameForStores.Single();
					}
				}
				if (string.IsNullOrEmpty(proposedName))
				{
					var proposedNameForLoads = variable.LoadInstructions
						.Select(arg => GetNameForArgument(arg.Parent, arg.ChildIndex))
						.Except(currentLowerCaseTypeOrMemberNames).ToList();
					if (proposedNameForLoads.Count == 1)
					{
						proposedName = proposedNameForLoads[0];
					}
				}
				if (string.IsNullOrEmpty(proposedName) && variable.Kind == VariableKind.StackSlot)
				{
					var proposedNameForStoresFromNewObj = variable.StoreInstructions.OfType<StLoc>()
						.Select(expr => GetNameByType(GuessType(variable.Type, expr.Value, context)))
						.Except(currentLowerCaseTypeOrMemberNames).ToList();
					if (proposedNameForStoresFromNewObj.Count == 1)
					{
						proposedName = proposedNameForStoresFromNewObj[0];
					}
				}
				if (string.IsNullOrEmpty(proposedName))
				{
					proposedName = GetNameByType(variable.Type);
				}

				// for generated names remove number-suffixes
				return SplitName(proposedName, out _);
			}
		}

		public void Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;
			this.workList ??= new Queue<(ILFunction function, VariableScope parentScope)>();
			this.workList.Clear();
			this.workList.Enqueue((function, null));

			while (this.workList.Count > 0)
			{
				var (currentFunction, parentContext) = this.workList.Dequeue();

				var nestedContext = new VariableScope(currentFunction, this.context, parentContext);

				foreach (var localFunction in currentFunction.LocalFunctions)
				{
					AssignNameToLocalFunction(localFunction, nestedContext);
					workList.Enqueue((localFunction, nestedContext));
				}

				currentFunction.Body.AcceptVisitor(this, nestedContext);

				if (currentFunction.Kind != ILFunctionKind.TopLevelFunction)
				{
					foreach (var p in currentFunction.Variables.Where(v => v.Kind == VariableKind.Parameter && v.Index >= 0))
					{
						p.Name = nestedContext.AssignNameIfUnassigned(p);
					}
				}
			}
		}

		private static void AssignNameToLocalFunction(ILFunction function, VariableScope parentContext)
		{
			if (!LocalFunctionDecompiler.ParseLocalFunctionName(function.Name, out _, out var newName) || !IsValidName(newName))
				newName = null;
			string nameWithoutNumber;
			int number;
			if (!string.IsNullOrEmpty(newName))
			{
				nameWithoutNumber = SplitName(newName, out number);
			}
			else
			{
				nameWithoutNumber = "f";
				number = 1;
			}
			int count;
			if (!parentContext.IsReservedVariableName(nameWithoutNumber, out int currentIndex))
			{
				count = 1;
			}
			else
			{
				if (currentIndex < number)
					count = number;
				else
					count = Math.Max(number, currentIndex) + 1;
			}
			parentContext.ReserveVariableName(nameWithoutNumber, count);
			if (count > 1)
			{
				newName = nameWithoutNumber + count.ToString();
			}
			else
			{
				newName = nameWithoutNumber;
			}
			function.Name = newName;
			function.ReducedMethod.Name = newName;
			parentContext.Add((MethodDefinitionHandle)function.ReducedMethod.MetadataToken, newName);
		}

		Unit VisitChildren(ILInstruction inst, VariableScope context)
		{
			foreach (var child in inst.Children)
			{
				child.AcceptVisitor(this, context);
			}

			return default;
		}

		protected override Unit Default(ILInstruction inst, VariableScope context)
		{
			if (inst is IInstructionWithVariableOperand { Variable: var v })
			{
				// if there is already a valid name for the variable slot, just use it
				string name = context.TryGetExistingName(v);
				if (!string.IsNullOrEmpty(name))
				{
					v.Name = name;
					return VisitChildren(inst, context);
				}

				switch (v.Kind)
				{
					case VariableKind.Parameter when !v.HasGeneratedName && v.Function.Kind == ILFunctionKind.TopLevelFunction:
						// Parameter names of top-level functions are handled in ILReader.CreateILVariable
						// and CSharpDecompiler.FixParameterNames
						break;
					case VariableKind.InitializerTarget: // keep generated names
					case VariableKind.NamedArgument:
						context.ReserveVariableName(v.Name);
						break;
					case VariableKind.UsingLocal when v.AddressCount == 0 && v.LoadCount == 0:
						// using variables that are not read, will not be declared in source source
						break;
					case VariableKind.DisplayClassLocal:
						v.Name = context.NextDisplayClassLocal();
						break;
					case VariableKind.Local when v.Index != null:
						name = context.TryGetExistingName(v.Function, v.Index.Value);
						if (name != null)
						{
							// make sure all local ILVariables that refer to the same slot in the locals signature
							// are assigned the same name.
							v.Name = name;
						}
						else
						{
							v.Name = context.AssignName(v);
							context.AssignNameToLocalSignatureIndex(v.Function, v.Index.Value, v.Name);
						}
						break;
					default:
						v.Name = context.AssignName(v);
						break;
				}
			}

			return VisitChildren(inst, context);
		}

		protected internal override Unit VisitILFunction(ILFunction function, VariableScope context)
		{
			workList.Enqueue((function, context));
			return default;
		}

		protected internal override Unit VisitCall(Call inst, VariableScope context)
		{
			if (inst.Method is LocalFunctionMethod m)
			{
				string name = context.TryGetExistingName((MethodDefinitionHandle)m.MetadataToken);
				if (!string.IsNullOrEmpty(name))
					m.Name = name;
			}

			return base.VisitCall(inst, context);
		}

		protected internal override Unit VisitLdFtn(LdFtn inst, VariableScope context)
		{
			if (inst.Method is LocalFunctionMethod m)
			{
				string name = context.TryGetExistingName((MethodDefinitionHandle)m.MetadataToken);
				if (!string.IsNullOrEmpty(name))
					m.Name = name;
			}

			return base.VisitLdFtn(inst, context);
		}

		static IEnumerable<string> CollectAllLowerCaseMemberNames(ITypeDefinition type)
		{
			foreach (var item in type.GetMembers(m => IsLowerCase(m.Name)))
				yield return item.Name;
		}

		static IEnumerable<string> CollectAllLowerCaseTypeNames(ITypeDefinition type)
		{
			var ns = type.ParentModule.Compilation.GetNamespaceByFullName(type.Namespace);
			foreach (var item in ns.Types)
			{
				if (IsLowerCase(item.Name))
					yield return item.Name;
			}
		}

		static bool IsLowerCase(string name)
		{
			return name.Length > 0 && char.ToLower(name[0]) == name[0];
		}

		/// <remarks>
		/// Must be in sync with <see cref="GetNameFromInstruction" />.
		/// </remarks>
		internal static bool IsSupportedInstruction(object arg)
		{
			switch (arg)
			{
				case GetPinnableReference _:
				case LdObj _:
				case LdFlda _:
				case LdsFlda _:
				case CallInstruction _:
					return true;
				default:
					return false;
			}
		}

		internal static bool IsValidName(string varName)
		{
			if (string.IsNullOrWhiteSpace(varName))
				return false;
			if (!(char.IsLetter(varName[0]) || varName[0] == '_'))
				return false;
			for (int i = 1; i < varName.Length; i++)
			{
				if (!(char.IsLetterOrDigit(varName[i]) || varName[i] == '_'))
					return false;
			}
			return true;
		}

		static string GetNameFromInstruction(ILInstruction inst)
		{
			switch (inst)
			{
				case GetPinnableReference getPinnableReference:
					return GetNameFromInstruction(getPinnableReference.Argument);
				case LdObj ldobj:
					return GetNameFromInstruction(ldobj.Target);
				case LdFlda ldflda:
					if (ldflda.Field.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
						return GetNameFromInstruction(ldflda.Target);
					return CleanUpVariableName(ldflda.Field.Name);
				case LdsFlda ldsflda:
					return CleanUpVariableName(ldsflda.Field.Name);
				case CallInstruction call:
					if (call is NewObj)
						break;
					IMethod m = call.Method;
					if (ExcludeMethodFromCandidates(m))
						break;
					if (m.Name.StartsWith("get_", StringComparison.OrdinalIgnoreCase) && m.Parameters.Count == 0)
					{
						// use name from properties, but not from indexers
						return CleanUpVariableName(m.Name.Substring(4));
					}
					else if (m.Name.StartsWith("Get", StringComparison.OrdinalIgnoreCase) && m.Name.Length >= 4 && char.IsUpper(m.Name[3]))
					{
						// use name from Get-methods
						return CleanUpVariableName(m.Name.Substring(3));
					}
					break;
				case DynamicInvokeMemberInstruction dynInvokeMember:
					if (dynInvokeMember.Name.StartsWith("Get", StringComparison.OrdinalIgnoreCase)
						&& dynInvokeMember.Name.Length >= 4 && char.IsUpper(dynInvokeMember.Name[3]))
					{
						// use name from Get-methods
						return CleanUpVariableName(dynInvokeMember.Name.Substring(3));
					}
					break;
			}
			return null;
		}

		static string GetNameForArgument(ILInstruction parent, int i)
		{
			switch (parent)
			{
				case StObj stobj:
					IField field;
					if (stobj.Target is LdFlda ldflda)
						field = ldflda.Field;
					else if (stobj.Target is LdsFlda ldsflda)
						field = ldsflda.Field;
					else
						break;
					return CleanUpVariableName(field.Name);
				case CallInstruction call:
					IMethod m = call.Method;
					if (ExcludeMethodFromCandidates(m))
						return null;
					if (m.Parameters.Count == 1 && i == call.Arguments.Count - 1)
					{
						// argument might be value of a setter
						if (m.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
						{
							return CleanUpVariableName(m.Name.Substring(4));
						}
						else if (m.Name.StartsWith("Set", StringComparison.OrdinalIgnoreCase) && m.Name.Length >= 4 && char.IsUpper(m.Name[3]))
						{
							return CleanUpVariableName(m.Name.Substring(3));
						}
					}
					var p = call.GetParameter(i);
					if (p != null && !string.IsNullOrEmpty(p.Name))
						return CleanUpVariableName(p.Name);
					break;
				case Leave _:
					return "result";
			}
			return null;
		}

		static bool ExcludeMethodFromCandidates(IMethod m)
		{
			if (m.SymbolKind == SymbolKind.Operator)
				return true;
			if (m.Name == "ToString")
				return true;
			if (m.Name == "Concat" && m.DeclaringType.IsKnownType(KnownTypeCode.String))
				return true;
			if (m.Name == "GetPinnableReference")
				return true;
			return false;
		}

		static string GetNameByType(IType type)
		{
			type = NullableType.GetUnderlyingType(type);
			while (type is ModifiedType || type is PinnedType)
			{
				type = NullableType.GetUnderlyingType(((TypeWithElementType)type).ElementType);
			}

			string name;
			if (type.IsAnonymousType())
			{
				name = "anon";
			}
			else if (type.Name.EndsWith("Exception", StringComparison.Ordinal))
			{
				name = "ex";
			}
			else if (type.Name.EndsWith("EventArgs", StringComparison.Ordinal))
			{
				name = "e";
			}
			else if (type.IsCSharpNativeIntegerType())
			{
				name = "num";
			}
			else if (!typeNameToVariableNameDict.TryGetValue(type.FullName, out name))
			{
				name = type.Kind switch {
					TypeKind.Array => "array",
					TypeKind.Pointer => "ptr",
					TypeKind.TypeParameter => "val",
					TypeKind.Unknown => "val",
					TypeKind.Dynamic => "val",
					TypeKind.ByReference => "reference",
					TypeKind.Tuple => "tuple",
					TypeKind.NInt => "num",
					TypeKind.NUInt => "num",
					_ => type.Name
				};
				// remove the 'I' for interfaces
				if (name.Length >= 3 && name[0] == 'I' && char.IsUpper(name[1]) && char.IsLower(name[2]))
					name = name.Substring(1);
				name = CleanUpVariableName(name) ?? "obj";
			}
			return name;
		}

		static void AddExistingName(Dictionary<string, int> reservedVariableNames, string name)
		{
			if (string.IsNullOrEmpty(name))
				return;
			string nameWithoutDigits = SplitName(name, out int number);
			if (reservedVariableNames.TryGetValue(nameWithoutDigits, out int existingNumber))
			{
				reservedVariableNames[nameWithoutDigits] = Math.Max(number, existingNumber);
			}
			else
			{
				reservedVariableNames.Add(nameWithoutDigits, number);
			}
		}

		static string SplitName(string name, out int number)
		{
			// First, identify whether the name already ends with a number:
			int pos = name.Length;
			while (pos > 0 && name[pos - 1] >= '0' && name[pos - 1] <= '9')
				pos--;
			if (pos < name.Length)
			{
				if (int.TryParse(name.Substring(pos), out number))
				{
					return name.Substring(0, pos);
				}
			}
			number = 1;
			return name;
		}

		static string CleanUpVariableName(string name)
		{
			// remove the backtick (generics)
			int pos = name.IndexOf('`');
			if (pos >= 0)
				name = name.Substring(0, pos);

			// remove field prefix:
			if (name.Length > 2 && name.StartsWith("m_", StringComparison.Ordinal))
				name = name.Substring(2);
			else if (name.Length > 1 && name[0] == '_' && (char.IsLetter(name[1]) || name[1] == '_'))
				name = name.Substring(1);

			if (TextWriterTokenWriter.ContainsNonPrintableIdentifierChar(name))
			{
				return null;
			}

			if (name.Length == 0)
				return "obj";
			string lowerCaseName = char.ToLower(name[0]) + name.Substring(1);
			if (CSharp.OutputVisitor.CSharpOutputVisitor.IsKeyword(lowerCaseName))
				return null;
			return lowerCaseName;
		}

		static IType GuessType(IType variableType, ILInstruction inst, ILTransformContext context)
		{
			if (!variableType.IsKnownType(KnownTypeCode.Object))
				return variableType;

			IType inferredType = inst.InferType(context.TypeSystem);
			if (inferredType.Kind != TypeKind.Unknown)
				return inferredType;
			else
				return variableType;
		}

		static Dictionary<string, int> CollectReservedVariableNames(ILFunction function,
			ILVariable existingVariable, bool mustResolveConflicts)
		{
			var reservedVariableNames = new Dictionary<string, int>();
			var rootFunction = function.Ancestors.OfType<ILFunction>().Single(f => f.Parent == null);
			foreach (var f in rootFunction.Descendants.OfType<ILFunction>())
			{
				foreach (var p in rootFunction.Parameters)
				{
					AddExistingName(reservedVariableNames, p.Name);
				}
				foreach (var v in f.Variables.Where(v => v.Kind != VariableKind.Parameter))
				{
					if (v != existingVariable)
						AddExistingName(reservedVariableNames, v.Name);
				}
			}
			if (mustResolveConflicts)
			{
				var memberNames = CollectAllLowerCaseMemberNames(function.Method.DeclaringTypeDefinition)
					.Concat(CollectAllLowerCaseTypeNames(function.Method.DeclaringTypeDefinition));
				foreach (var name in memberNames)
					AddExistingName(reservedVariableNames, name);
			}
			return reservedVariableNames;
		}

		internal static string GenerateForeachVariableName(ILFunction function, ILInstruction valueContext,
			ILVariable existingVariable = null, bool mustResolveConflicts = false)
		{
			if (function == null)
				throw new ArgumentNullException(nameof(function));
			if (existingVariable != null && !existingVariable.HasGeneratedName)
			{
				return existingVariable.Name;
			}
			var reservedVariableNames = CollectReservedVariableNames(function, existingVariable, mustResolveConflicts);

			string baseName = GetNameFromInstruction(valueContext);
			if (string.IsNullOrEmpty(baseName))
			{
				if (valueContext is LdLoc ldloc && ldloc.Variable.Kind == VariableKind.Parameter)
				{
					baseName = ldloc.Variable.Name;
				}
			}
			string proposedName = "item";

			if (!string.IsNullOrEmpty(baseName))
			{
				if (!IsPlural(baseName, ref proposedName))
				{
					if (baseName.Length > 4 && baseName.EndsWith("List", StringComparison.Ordinal))
					{
						proposedName = baseName.Substring(0, baseName.Length - 4);
					}
					else if (baseName.Equals("list", StringComparison.OrdinalIgnoreCase))
					{
						proposedName = "item";
					}
					else if (baseName.EndsWith("children", StringComparison.OrdinalIgnoreCase))
					{
						proposedName = baseName.Remove(baseName.Length - 3);
					}
				}
			}

			// remove any numbers from the proposed name
			proposedName = SplitName(proposedName, out int number);

			if (!reservedVariableNames.ContainsKey(proposedName))
			{
				reservedVariableNames.Add(proposedName, 0);
			}
			int count = ++reservedVariableNames[proposedName];
			Debug.Assert(!string.IsNullOrWhiteSpace(proposedName));
			if (count > 1)
			{
				return proposedName + count.ToString();
			}
			else
			{
				return proposedName;
			}
		}

		internal static string GenerateVariableName(ILFunction function, IType type,
			ILInstruction valueContext = null, ILVariable existingVariable = null,
			bool mustResolveConflicts = false)
		{
			if (function == null)
				throw new ArgumentNullException(nameof(function));
			var reservedVariableNames = CollectReservedVariableNames(function, existingVariable, mustResolveConflicts);

			string baseName = valueContext != null ? GetNameFromInstruction(valueContext) ?? GetNameByType(type) : GetNameByType(type);
			string proposedName = "obj";

			if (!string.IsNullOrEmpty(baseName))
			{
				if (!IsPlural(baseName, ref proposedName))
				{
					if (baseName.Length > 4 && baseName.EndsWith("List", StringComparison.Ordinal))
					{
						proposedName = baseName.Substring(0, baseName.Length - 4);
					}
					else if (baseName.Equals("list", StringComparison.OrdinalIgnoreCase))
					{
						proposedName = "item";
					}
					else if (baseName.EndsWith("children", StringComparison.OrdinalIgnoreCase))
					{
						proposedName = baseName.Remove(baseName.Length - 3);
					}
					else
					{
						proposedName = baseName;
					}
				}
			}

			// remove any numbers from the proposed name
			proposedName = SplitName(proposedName, out int number);

			if (!reservedVariableNames.ContainsKey(proposedName))
			{
				reservedVariableNames.Add(proposedName, 0);
			}
			int count = ++reservedVariableNames[proposedName];
			Debug.Assert(!string.IsNullOrWhiteSpace(proposedName));
			if (count > 1)
			{
				return proposedName + count.ToString();
			}
			else
			{
				return proposedName;
			}
		}

		private static bool IsPlural(string baseName, ref string proposedName)
		{
			var newName = Vocabularies.Default.Singularize(baseName, inputIsKnownToBePlural: false);
			if (string.IsNullOrWhiteSpace(newName) || newName == baseName)
				return false;
			proposedName = newName;
			return true;
		}
	}
}
