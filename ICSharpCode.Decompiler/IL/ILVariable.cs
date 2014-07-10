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

	class ILVariable(VariableKind kind, TypeReference type, int index)
	{
		public readonly VariableKind Kind = kind;
		public readonly TypeReference Type = type;
		public readonly int Index = index;

		readonly object CecilObject;
		
		public ILVariable(VariableDefinition v)
			: this(VariableKind.Local, v.VariableType, v.Index)
		{
			this.CecilObject = v;
		}

		public ILVariable(ParameterDefinition p)
			: this(VariableKind.Parameter, p.ParameterType, p.Index)
		{
			this.CecilObject = p;
		}

		public override string ToString()
		{
			switch (Kind) {
				case VariableKind.Local:
					return "V_" + Index.ToString();
				case VariableKind.Parameter:
					if (Index == -1)
						return "this";
					return "P_" + Index.ToString();
				default:
					return Kind.ToString();
			}
		}

		internal void WriteTo(ITextOutput output)
		{
			output.WriteReference(this.ToString(), CecilObject ?? this, isLocal: true);
		}
	}
}
