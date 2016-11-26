// Copyright (c) 2014 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Execution context for phase 1: replace pop and peek instructions with evaluation stack values.
	/// </summary>
	interface IInlineContext
	{
		/// <summary>
		/// Peeks at the top value on the evaluation stack, returning an instruction that represents
		/// that value.
		/// <see cref="ILInstruction.Inline"/> will replace <see cref="Peek"/> instructions
		/// with the value returned by this function.
		/// </summary>
		/// <param name="flagsBefore">Combined instruction flags of the instructions
		/// that the instruction getting inlined would get moved over.</param>
		/// <remarks>
		/// This method may return <c>null</c> when the evaluation stack is empty or the contents
		/// are unknown. In this case, the peek instruction will not be replaced.
		/// </remarks>
		ILInstruction Peek(InstructionFlags flagsBefore);
		
		/// <summary>
		/// Pops the top value on the evaluation stack, returning an instruction that represents
		/// that value.
		/// <see cref="ILInstruction.Inline"/> will replace <see cref="Pop"/> instructions
		/// with the value returned by this function.
		/// </summary>
		/// <param name="flagsBefore">Combined instruction flags of the instructions
		/// that the instruction getting inlined would get moved over.</param>
		/// <remarks>
		/// This method may return <c>null</c> when the evaluation stack is empty or the contents
		/// are unknown. In this case, the pop instruction will not be replaced.
		/// </remarks>
		ILInstruction Pop(InstructionFlags flagsBefore);
	}
}
