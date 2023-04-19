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

#nullable enable

using System.Diagnostics.CodeAnalysis;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Implements transforming &lt;PrivateImplementationDetails&gt;.ThrowIfNull(name, "name"); 
	/// </summary>
	class ParameterNullCheckTransform : IILTransform
	{
		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.ParameterNullCheck)
				return;
			// we only need to look at the entry-point as parameter null-checks
			// do not produce any IL control-flow instructions
			Block entryPoint = ((BlockContainer)function.Body).EntryPoint;
			int index = 0;
			// Early versions of this pattern produced call ThrowIfNull instructions after
			// state-machine initialization instead of right at the start of the method.
			// In order to support both patterns, we scan all instructions,
			// if the current function is decorated with a state-machine attribute.
			bool scanFullBlock = function.Method != null
				&& (function.Method.HasAttribute(KnownAttribute.IteratorStateMachine)
					|| function.Method.HasAttribute(KnownAttribute.AsyncIteratorStateMachine)
					|| function.Method.HasAttribute(KnownAttribute.AsyncStateMachine));
			// loop over all instructions
			while (index < entryPoint.Instructions.Count)
			{
				// The pattern does not match for the current instruction
				if (!MatchThrowIfNullCall(entryPoint.Instructions[index], out ILVariable? parameterVariable))
				{
					if (scanFullBlock)
					{
						// continue scanning
						index++;
						continue;
					}
					else
					{
						// abort
						break;
					}
				}
				// remove the call to ThrowIfNull
				entryPoint.Instructions.RemoveAt(index);
				// remember to generate !! when producing the final output.
				parameterVariable.HasNullCheck = true;
			}
		}

		// call <PrivateImplementationDetails>.ThrowIfNull(ldloc parameterVariable, ldstr "parameterVariable")
		private bool MatchThrowIfNullCall(ILInstruction instruction, [NotNullWhen(true)] out ILVariable? parameterVariable)
		{
			parameterVariable = null;
			if (instruction is not Call call)
				return false;
			if (call.Arguments.Count != 2)
				return false;
			if (!call.Method.IsStatic || !call.Method.FullNameIs("<PrivateImplementationDetails>", "ThrowIfNull"))
				return false;
			if (!(call.Arguments[0].MatchLdLoc(out parameterVariable) && parameterVariable.Kind == VariableKind.Parameter))
				return false;
			return true;
		}
	}
}
