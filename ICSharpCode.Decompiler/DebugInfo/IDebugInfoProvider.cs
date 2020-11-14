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

	public interface IDebugInfoProvider
	{
		string Description { get; }
		IList<SequencePoint> GetSequencePoints(MethodDefinitionHandle method);
		IList<Variable> GetVariables(MethodDefinitionHandle method);
		bool TryGetName(MethodDefinitionHandle method, int index, out string name);
		string SourceFileName { get; }
	}
}
