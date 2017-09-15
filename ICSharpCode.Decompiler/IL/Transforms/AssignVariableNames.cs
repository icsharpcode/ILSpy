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
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class AssignVariableNames : IILTransform
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
		string[] currentFieldNames;
		Dictionary<string, int> reservedVariableNames;
		HashSet<ILVariable> loopCounters;
		const char maxLoopVariableName = 'n';

		public void Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;
			currentFieldNames = function.Method.DeclaringType.Fields.Select(f => f.Name).ToArray();
			reservedVariableNames = new Dictionary<string, int>();
			loopCounters = CollectLoopCounters(function);
			foreach (var p in function.Descendants.OfType<ILFunction>().Select(f => f.Method).SelectMany(m => m.Parameters))
				AddExistingName(p.Name);
			foreach (ILFunction f in function.Descendants.OfType<ILFunction>().Reverse()) {
				PerformAssignment(f);
			}
		}

		void PerformAssignment(ILFunction function)
		{
			// remove unused variables before assigning names
			function.Variables.RemoveDead();
			foreach (var v in function.Variables) {
				switch (v.Kind) {
					case VariableKind.Parameter: // ignore
						break;
					case VariableKind.InitializerTarget: // keep generated names
						AddExistingName(v.Name);
						break;
					default:
						if (v.HasGeneratedName || !IsValidName(v.Name)) {
							// don't use the name from the debug symbols if it looks like a generated name
							v.Name = null;
						} else {
							// use the name from the debug symbols
							// (but ensure we don't use the same name for two variables)
							v.Name = GetAlternativeName(v.Name);
						}
						break;
				}
			}
			// Now generate names:
			var mapping = new Dictionary<ILVariable, string>(ILVariableEqualityComparer.Instance);
			foreach (var inst in function.Descendants.OfType<IInstructionWithVariableOperand>()) {
				var v = inst.Variable;
				if (!mapping.TryGetValue(v, out string name)) {
					if (string.IsNullOrEmpty(v.Name))
						v.Name = GenerateNameForVariable(v, function.Body);
					mapping.Add(v, v.Name);
				} else {
					v.Name = name;
				}
			}
		}

		static bool IsValidName(string varName)
		{
			if (string.IsNullOrEmpty(varName))
				return false;
			if (!(char.IsLetter(varName[0]) || varName[0] == '_'))
				return false;
			for (int i = 1; i < varName.Length; i++) {
				if (!(char.IsLetterOrDigit(varName[i]) || varName[i] == '_'))
					return false;
			}
			return true;
		}

		public string GetAlternativeName(string oldVariableName)
		{
			if (oldVariableName.Length == 1 && oldVariableName[0] >= 'i' && oldVariableName[0] <= maxLoopVariableName) {
				for (char c = 'i'; c <= maxLoopVariableName; c++) {
					if (!reservedVariableNames.ContainsKey(c.ToString())) {
						reservedVariableNames.Add(c.ToString(), 1);
						return c.ToString();
					}
				}
			}

			int number;
			string nameWithoutDigits = SplitName(oldVariableName, out number);

			if (!reservedVariableNames.ContainsKey(nameWithoutDigits)) {
				reservedVariableNames.Add(nameWithoutDigits, number - 1);
			}
			int count = ++reservedVariableNames[nameWithoutDigits];
			if (count != 1) {
				return nameWithoutDigits + count.ToString();
			} else {
				return nameWithoutDigits;
			}
		}

		HashSet<ILVariable> CollectLoopCounters(ILFunction function)
		{
			var loopCounters = new HashSet<ILVariable>();
			
			foreach (BlockContainer possibleLoop in function.Descendants.OfType<BlockContainer>()) {
				if (possibleLoop.EntryPoint.IncomingEdgeCount == 1) continue;
				var loop = DetectedLoop.DetectLoop(possibleLoop);
				if (loop.Kind != LoopKind.For || loop.IncrementTarget == null) continue;
				loopCounters.Add(loop.IncrementTarget);
			}

			return loopCounters;
		}

		string GenerateNameForVariable(ILVariable variable, ILInstruction methodBody)
		{
			string proposedName = null;
			if (variable.Type.IsKnownType(KnownTypeCode.Int32)) {
				// test whether the variable might be a loop counter
				if (loopCounters.Contains(variable)) {
					// For loop variables, use i,j,k,l,m,n
					for (char c = 'i'; c <= maxLoopVariableName; c++) {
						if (!reservedVariableNames.ContainsKey(c.ToString())) {
							proposedName = c.ToString();
							break;
						}
					}
				}
			}
			if (string.IsNullOrEmpty(proposedName)) {
				var proposedNameForStores = variable.StoreInstructions.OfType<StLoc>()
					.Select(expr => GetNameFromInstruction(expr.Value))
					.Except(currentFieldNames).ToList();
				if (proposedNameForStores.Count == 1) {
					proposedName = proposedNameForStores[0];
				}
			}
			if (string.IsNullOrEmpty(proposedName)) {
				var proposedNameForLoads = variable.LoadInstructions
					.Select(arg => GetNameForArgument(arg.Parent, arg.ChildIndex))
					.Except(currentFieldNames).ToList();
				if (proposedNameForLoads.Count == 1) {
					proposedName = proposedNameForLoads[0];
				}
			}
			if (string.IsNullOrEmpty(proposedName)) {
				proposedName = GetNameByType(variable.Type);
			}

			// remove any numbers from the proposed name
			int number;
			proposedName = SplitName(proposedName, out number);

			if (!reservedVariableNames.ContainsKey(proposedName)) {
				reservedVariableNames.Add(proposedName, 0);
			}
			int count = ++reservedVariableNames[proposedName];
			if (count > 1) {
				return proposedName + count.ToString();
			} else {
				return proposedName;
			}
		}

		static string GetNameFromInstruction(ILInstruction inst)
		{
			switch (inst) {
				case LdObj ldobj:
					IField field;
					if (ldobj.Target is LdFlda ldflda)
						field = ldflda.Field;
					else if (ldobj.Target is LdsFlda ldsflda)
						field = ldsflda.Field;
					else
						break;
					return CleanUpVariableName(field.Name);
				case CallInstruction call:
					if (call is NewObj) break;
					IMethod m = call.Method;
					if (m.Name.StartsWith("get_", StringComparison.OrdinalIgnoreCase) && m.Parameters.Count == 0) {
						// use name from properties, but not from indexers
						return CleanUpVariableName(m.Name.Substring(4));
					} else if (m.Name.StartsWith("Get", StringComparison.OrdinalIgnoreCase) && m.Name.Length >= 4 && char.IsUpper(m.Name[3])) {
						// use name from Get-methods
						return CleanUpVariableName(m.Name.Substring(3));
					}
					break;
			}
			return null;
		}

		static string GetNameForArgument(ILInstruction parent, int i)
		{
			switch (parent) {
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
					if (m.Parameters.Count == 1 && i == call.Arguments.Count - 1) {
						// argument might be value of a setter
						if (m.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase)) {
							return CleanUpVariableName(m.Name.Substring(4));
						} else if (m.Name.StartsWith("Set", StringComparison.OrdinalIgnoreCase) && m.Name.Length >= 4 && char.IsUpper(m.Name[3])) {
							return CleanUpVariableName(m.Name.Substring(3));
						}
					}
					var p = m.Parameters.ElementAtOrDefault((!(call is NewObj) && !m.IsStatic) ? i - 1 : i);
					if (p != null && !string.IsNullOrEmpty(p.Name))
						return CleanUpVariableName(p.Name);
					break;
				case Return ret:
					return "result";
			}
			return null;
		}

		string GetNameByType(IType type)
		{
			var git = type as ParameterizedType;
			if (git != null && git.FullName == "System.Nullable`1" && git.TypeArguments.Count == 1) {
				type = git.TypeArguments[0];
			}

			string name;
			if (type is ArrayType) {
				name = "array";
			} else if (type is PointerType) {
				name = "ptr";
			} else if (type.IsAnonymousType()) {
				name = "anon";
			} else if (type.Name.EndsWith("Exception", StringComparison.Ordinal)) {
				name = "ex";
			} else if (!typeNameToVariableNameDict.TryGetValue(type.FullName, out name)) {
				name = type.Name;
				// remove the 'I' for interfaces
				if (name.Length >= 3 && name[0] == 'I' && char.IsUpper(name[1]) && char.IsLower(name[2]))
					name = name.Substring(1);
				name = CleanUpVariableName(name);
			}
			return name;
		}

		void AddExistingName(string name)
		{
			if (string.IsNullOrEmpty(name))
				return;
			string nameWithoutDigits = SplitName(name, out int number);
			if (reservedVariableNames.TryGetValue(nameWithoutDigits, out int existingNumber)) {
				reservedVariableNames[nameWithoutDigits] = Math.Max(number, existingNumber);
			} else {
				reservedVariableNames.Add(nameWithoutDigits, number);
			}
		}

		static string SplitName(string name, out int number)
		{
			// First, identify whether the name already ends with a number:
			int pos = name.Length;
			while (pos > 0 && name[pos - 1] >= '0' && name[pos - 1] <= '9')
				pos--;
			if (pos < name.Length) {
				if (int.TryParse(name.Substring(pos), out number)) {
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

			if (name.Length == 0)
				return "obj";
			else
				return char.ToLower(name[0]) + name.Substring(1);
		}
	}
}
