// Copyright (c) 2019 Siegfried Pammer
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
using System.Text;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	internal static class ILInstructionExtensions
	{
		public static T WithILRange<T>(this T target, ILInstruction sourceInstruction) where T : ILInstruction
		{
			target.AddILRange(sourceInstruction);
			return target;
		}

		public static T WithILRange<T>(this T target, Interval range) where T : ILInstruction
		{
			target.AddILRange(range);
			return target;
		}

		public static ILInstruction? GetNextSibling(this ILInstruction? instruction)
		{
			if (instruction?.Parent == null)
				return null;
			if (instruction.ChildIndex + 1 >= instruction.Parent.Children.Count)
				return null;
			return instruction.Parent.Children[instruction.ChildIndex + 1];
		}
	}
}
