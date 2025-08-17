// Copyright (c) 2025 Siegfried Pammer
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	// call ThrowInvalidOperationException(...)
	// leave IL_0000 (ldloc temp)
	//
	// -or-
	//
	// call ThrowSwitchExpressionException(...)
	// leave IL_0000 (ldloc temp)
	// 
	// -to-
	//
	// throw(newobj SwitchExpressionException(...))
	class SwitchExpressionDefaultCaseTransform : IILTransform
	{
		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			IMethod? FindConstructor(string fullTypeName, params Type[] argumentTypes)
			{
				IType exceptionType = context.TypeSystem.FindType(new FullTypeName(fullTypeName));
				var types = argumentTypes.SelectArray(context.TypeSystem.FindType);

				foreach (var ctor in exceptionType.GetConstructors(m => !m.IsStatic && m.Parameters.Count == argumentTypes.Length))
				{
					bool found = true;
					foreach (var pair in ctor.Parameters.Select(p => p.Type).Zip(types))
					{
						if (!NormalizeTypeVisitor.IgnoreNullability.EquivalentTypes(pair.Item1, pair.Item2))
						{
							found = false;
							break;
						}
					}
					if (found)
						return ctor;
				}

				return null;
			}

			IMethod[] exceptionCtorTable = new IMethod[2];

			exceptionCtorTable[0] = FindConstructor("System.InvalidOperationException")!;
			exceptionCtorTable[1] = FindConstructor("System.Runtime.CompilerServices.SwitchExpressionException", typeof(object))!;

			if (exceptionCtorTable[0] == null && exceptionCtorTable[1] == null)
				return;

			bool MatchThrowHelperCall(ILInstruction inst, [NotNullWhen(true)] out IMethod? exceptionCtor, out ILInstruction? value)
			{
				exceptionCtor = null;
				value = null;
				if (inst is not Call call)
					return false;
				if (call.Method.DeclaringType.FullName != "<PrivateImplementationDetails>")
					return false;
				switch (call.Arguments.Count)
				{
					case 0:
						if (call.Method.Name != "ThrowInvalidOperationException")
							return false;
						exceptionCtor = exceptionCtorTable[0];
						break;
					case 1:
						if (call.Method.Name != "ThrowSwitchExpressionException")
							return false;
						exceptionCtor = exceptionCtorTable[1];
						value = call.Arguments[0];
						break;
					default:
						return false;
				}
				return exceptionCtor != null;
			}

			foreach (var block in function.Descendants.OfType<Block>())
			{
				if (block.Parent is not BlockContainer container)
					continue;
				if (container.EntryPoint is not { IncomingEdgeCount: 1, Instructions: [SwitchInstruction inst] })
					continue;
				if (block.Instructions.Count != 2 || block.IncomingEdgeCount != 1 || !block.FinalInstruction.MatchNop())
					continue;
				var defaultSection = inst.GetDefaultSection();
				if (defaultSection.Body != block && (defaultSection.Body is not Branch b || b.TargetBlock != block))
					continue;
				if (!MatchThrowHelperCall(block.Instructions[0], out IMethod? exceptionCtor, out ILInstruction? value))
					continue;
				if (block.Instructions[1] is not Leave { Value: LdLoc { Variable: { Kind: VariableKind.Local or VariableKind.StackSlot, LoadCount: 1, InitialValueIsInitialized: true } } })
				{
					continue;
				}
				context.Step("SwitchExpressionDefaultCaseTransform", block.Instructions[0]);
				var newObj = new NewObj(exceptionCtor);
				if (value != null)
					newObj.Arguments.Add(value);
				block.Instructions[0] = new Throw(newObj).WithILRange(block.Instructions[0]).WithILRange(block.Instructions[1]);
				block.Instructions.RemoveAt(1);
				defaultSection.IsCompilerGeneratedDefaultSection = true;
			}
		}
	}
}
