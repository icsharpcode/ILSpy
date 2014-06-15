using Mono.Cecil;
using Mono.Cecil.Cil;
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

	class ILVariable(public readonly VariableKind Kind, public readonly TypeReference Type, public readonly int Index)
	{
		public ILVariable(VariableDefinition v)
			: this(VariableKind.Local, v.VariableType, v.Index)
		{
		}

		public ILVariable(ParameterDefinition p)
			: this(VariableKind.Parameter, p.ParameterType, p.Index)
		{
		}

		public override string ToString()
		{
			switch (Kind) {
				case VariableKind.Local:
					return "V_" + Index.ToString();
				case VariableKind.Parameter:
					return "P_" + Index.ToString();
				default:
					return Kind.ToString();
			}
		}
	}
}
