using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	enum VariableKind
	{
		Local,
		Parameter,
	}

	class ILVariable(public readonly VariableKind Kind, public readonly int Index)
	{
		
	}
}
