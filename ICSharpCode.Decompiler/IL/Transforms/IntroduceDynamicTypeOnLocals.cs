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
using System.Text;

using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	public class IntroduceDynamicTypeOnLocals : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var variable in function.Variables)
			{
				if (variable.Kind != VariableKind.Local &&
					variable.Kind != VariableKind.StackSlot &&
					variable.Kind != VariableKind.ForeachLocal &&
					variable.Kind != VariableKind.UsingLocal)
				{
					continue;
				}
				if (!variable.Type.IsKnownType(KnownTypeCode.Object) || variable.LoadCount == 0)
					continue;
				foreach (var load in variable.LoadInstructions)
				{
					if (load.Parent is DynamicInstruction dynamicInstruction)
					{
						var argumentInfo = dynamicInstruction.GetArgumentInfoOfChild(load.ChildIndex);
						if (!argumentInfo.HasFlag(CSharpArgumentInfoFlags.UseCompileTimeType))
						{
							variable.Type = SpecialType.Dynamic;
						}
					}
				}
			}
		}
	}
}
