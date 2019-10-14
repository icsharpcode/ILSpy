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
using System.Linq;
using System.Text;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	class IntroduceRefReadOnlyModifierOnLocals : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var variable in function.Variables) {
				if (variable.Type.Kind != TypeKind.ByReference || variable.Kind == VariableKind.Parameter)
					continue;
				// ref readonly
				if (IsUsedAsRefReadonly(variable)) {
					variable.IsRefReadOnly = true;
					continue;
				}
			}
		}

		/// <summary>
		/// Infer ref readonly type from usage:
		/// An ILVariable should be marked as readonly,
		/// if it's a "by-ref-like" type and the initialized value is known to be readonly.
		/// </summary>
		bool IsUsedAsRefReadonly(ILVariable variable)
		{
			foreach (var store in variable.StoreInstructions.OfType<StLoc>()) {
				if (ILInlining.IsReadonlyReference(store.Value))
					return true;
			}
			return false;
		}
	}
}
