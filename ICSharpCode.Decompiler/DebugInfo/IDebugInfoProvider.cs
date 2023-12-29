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
