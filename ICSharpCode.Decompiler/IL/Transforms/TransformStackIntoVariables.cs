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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ICSharpCode.Decompiler.IL
{
	public class ILTransformContext
	{
		public IDecompilerTypeSystem TypeSystem { get; set; }
		public CancellationToken CancellationToken { get; set; }
	}

	public interface IILTransform
	{
		void Run(ILFunction function, ILTransformContext context);
	}
	
	public class TransformStackIntoVariables : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			var state = new TransformStackIntoVariablesState();
			state.TypeSystem = context.TypeSystem;
			function.TransformStackIntoVariables(state);
			HashSet<ILVariable> variables = new HashSet<ILVariable>();
			function.TransformChildren(new CollectStackVariablesVisitor(state, variables));
			function.Variables.AddRange(variables);
		}

		class CollectStackVariablesVisitor : ILVisitor<ILInstruction>
		{
			readonly TransformStackIntoVariablesState state;

			readonly HashSet<ILVariable> variables;

			public CollectStackVariablesVisitor(TransformStackIntoVariablesState state, HashSet<ILVariable> variables)
			{
				this.state = state;
				this.variables = variables;
			}

			protected override ILInstruction Default(ILInstruction inst)
			{
				inst.TransformChildren(this);
				return inst;
			}

			protected internal override ILInstruction VisitLdLoc(LdLoc inst)
			{
				if (inst.Variable.Kind == VariableKind.StackSlot) {
					var variable = state.UnionFind.Find(inst.Variable);
					if (variables.Add(variable))
						variable.Name = "S_" + (variables.Count - 1);
					inst = new LdLoc(variable);
				}
				return base.VisitLdLoc(inst);
			}

			protected internal override ILInstruction VisitStLoc(StLoc inst)
			{
				if (inst.Variable.Kind == VariableKind.StackSlot) {
					var variable = state.UnionFind.Find(inst.Variable);
					if (variables.Add(variable))
						variable.Name = "S_" + (variables.Count - 1);
					inst = new StLoc(inst.Value, variable);
				}
				return base.VisitStLoc(inst);
			}
		}
	}
}


