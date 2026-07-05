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
using System.Reflection.Metadata;
using System.Text;

namespace ICSharpCode.Decompiler.DebugInfo
{
	public struct Variable
	{
		public Variable(int index, string name)
		{
			Index = index;
			Name = name;
		}

		public int Index { get; }
		public string Name { get; }
	}

	public struct PdbExtraTypeInfo
	{
		public string[] TupleElementNames;
		public bool[] DynamicFlags;
	}

	public interface IDebugInfoProvider
	{
		string Description { get; }
		IList<SequencePoint> GetSequencePoints(MethodDefinitionHandle method);
		IList<Variable> GetVariables(MethodDefinitionHandle method);
		bool TryGetName(MethodDefinitionHandle method, int index, out string name);
		bool TryGetExtraTypeInfo(MethodDefinitionHandle method, int index, out PdbExtraTypeInfo extraTypeInfo);
		string SourceFileName { get; }
	}
}
