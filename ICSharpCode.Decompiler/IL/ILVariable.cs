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

using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.Disassembler;

namespace ICSharpCode.Decompiler.IL
{
	public enum VariableKind
	{
		Local,
		Parameter,
		/// <summary>
		/// The 'this' parameter
		/// </summary>
		This,
		/// <summary>
		/// Variable created for exception handler
		/// </summary>
		Exception
	}

	public class ILVariable
	{
		public readonly VariableKind Kind;
		public readonly TypeReference Type;
		public readonly int Index;
		
		public string Name;

		/// <summary>
		/// Number of ldloc instructions referencing this variable.
		/// </summary>
		public int LoadCount;
		
		/// <summary>
		/// Number of stloc instructions referencing this variable.
		/// </summary>
		public int StoreCount;
		
		/// <summary>
		/// Number of ldloca instructions referencing this variable.
		/// </summary>
		public int AddressCount;
		
		readonly object CecilObject;
		
		public ILVariable(VariableKind kind, TypeReference type, int index)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			this.Kind = kind;
			this.Type = type;
			this.Index = index;
		}
		
		public ILVariable(VariableDefinition v)
			: this(VariableKind.Local, v.VariableType, v.Index)
		{
			this.CecilObject = v;
			if (string.IsNullOrEmpty(v.Name))
				this.Name = "V_" + v.Index;
			else
				this.Name = v.Name;
		}

		public ILVariable(ParameterDefinition p)
			: this(p.Index == -1 ? VariableKind.This : VariableKind.Parameter, p.ParameterType, p.Index)
		{
			this.CecilObject = p;
			this.StoreCount = 1; // count the initial store when the method is called with an argument
			if (string.IsNullOrEmpty(p.Name))
				this.Name = "P_" + this.Index;
			else
				this.Name = p.Name;
		}
		
		public override string ToString()
		{
			return Name;
		}
		
		internal void WriteDefinitionTo(ITextOutput output)
		{
			output.WriteDefinition(this.Name, CecilObject ?? this, isLocal: true);
			output.Write(" : ");
			Type.WriteTo(output);
			output.Write("({0} ldloc, {1} ldloca, {2} stloc)", LoadCount, AddressCount, StoreCount);
		}
		
		internal void WriteTo(ITextOutput output)
		{
			output.WriteReference(this.Name, CecilObject ?? this, isLocal: true);
		}
	}
}
