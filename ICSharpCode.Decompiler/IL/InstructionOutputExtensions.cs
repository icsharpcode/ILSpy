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

using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	static partial class InstructionOutputExtensions
	{
		public static void Write(this ITextOutput output, OpCode opCode)
		{
			output.Write(originalOpCodeNames[(int)opCode]);
		}

		public static void Write(this ITextOutput output, StackType stackType)
		{
			output.Write(stackType.ToString().ToLowerInvariant());
		}

		public static void Write(this ITextOutput output, PrimitiveType primitiveType)
		{
			output.Write(primitiveType.ToString().ToLowerInvariant());
		}
		
		public static void WriteTo(this IType type, ITextOutput output, ILNameSyntax nameSyntax = ILNameSyntax.ShortTypeName)
		{
			output.WriteReference(type.ReflectionName, type);
		}
		
		public static void WriteTo(this ISymbol symbol, ITextOutput output)
		{
			if (symbol is IMethod method && method.IsConstructor)
				output.WriteReference(method.DeclaringType?.Name + "." + method.Name, symbol);
			else
				output.WriteReference(symbol.Name, symbol);
		}

		public static void WriteTo(this Interval interval, ITextOutput output, ILAstWritingOptions options)
		{
			if (!options.ShowILRanges)
				return;
			if (interval.IsEmpty)
				output.Write("[empty] ");
			else
				output.Write($"[{interval.Start:x4}..{interval.InclusiveEnd:x4}] ");
		}
	}
}
