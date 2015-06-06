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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	public class TransformValueTypeInitializers : IILTransform
	{
		ILTransformContext context;

		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;
			foreach (var block in function.Descendants.OfType<Block>()) {
				for (int i = block.Instructions.Count - 1; i >= 0; i--) {
					DoTransform(block, i);
				}
			}
		}

		bool DoTransform(Block block, int pos)
		{
			Call call = block.Instructions[pos] as Call;
			if (call == null || !call.Method.IsConstructor || call.Method.DeclaringType.Kind != TypeKind.Struct)
				return false;
			if (call.Arguments.Count != call.Method.Parameters.Count + 1)
				return false;
			var newObj = new NewObj(call.Method);
			newObj.Arguments.AddRange(call.Arguments.Skip(1));
			var expr = new StObj(call.Arguments[0], newObj, call.Method.DeclaringType);
			block.Instructions[pos].ReplaceWith(expr);
			return true;
		}
	}
}


