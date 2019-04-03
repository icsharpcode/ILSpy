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
using ICSharpCode.Decompiler.IL.Transforms;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using System.Diagnostics;

namespace ICSharpCode.Decompiler.IL
{
	partial class ILFunction
	{
		public readonly IMethod Method;
		public readonly GenericContext GenericContext;
		/// <summary>
		/// Size of the IL code in this function.
		/// Note: after async/await transform, this is the code size of the MoveNext function.
		/// </summary>
		public int CodeSize;
		public readonly ILVariableCollection Variables;

		/// <summary>
		/// List of warnings of ILReader.
		/// </summary>
		public List<string> Warnings { get; } = new List<string>();

		/// <summary>
		/// Gets whether this function is a decompiled iterator (is using yield).
		/// This flag gets set by the YieldReturnDecompiler.
		/// 
		/// If set, the 'return' instruction has the semantics of 'yield break;'
		/// instead of a normal return.
		/// </summary>
		public bool IsIterator;

		public bool StateMachineCompiledWithMono;

		/// <summary>
		/// Gets whether this function is async.
		/// This flag gets set by the AsyncAwaitDecompiler.
		/// </summary>
		public bool IsAsync => AsyncReturnType != null;

		/// <summary>
		/// Return element type -- if the async method returns Task{T}, this field stores T.
		/// If the async method returns Task or void, this field stores void.
		/// </summary>
		public IType AsyncReturnType;

		/// <summary>
		/// If this function is an iterator/async, this field stores the compiler-generated MoveNext() method.
		/// </summary>
		public IMethod MoveNextMethod;

		internal DebugInfo.AsyncDebugInfo AsyncDebugInfo;

		int ctorCallStart = int.MinValue;

		/// <summary>
		/// Returns the IL offset of the constructor call, -1 if this is not a constructor or no chained constructor call was found.
		/// </summary>
		internal int ChainedConstructorCallILOffset {
			get {
				if (ctorCallStart == int.MinValue) {
					if (!this.Method.IsConstructor || this.Method.IsStatic)
						ctorCallStart = -1;
					else {
						ctorCallStart = this.Descendants.FirstOrDefault(d => d is CallInstruction call && !(call is NewObj)
							&& call.Method.IsConstructor
							&& call.Method.DeclaringType.IsReferenceType == true
							&& call.Parent is Block)?.StartILOffset ?? -1;
					}
				}
				return ctorCallStart;
			}
		}

		/// <summary>
		/// If this is an expression tree or delegate, returns the expression tree type Expression{T} or T.
		/// T is the delegate type that matches the signature of this method.
		/// </summary>
		public IType DelegateType;

		public bool IsExpressionTree => DelegateType != null && DelegateType.FullName == "System.Linq.Expressions.Expression" && DelegateType.TypeParameterCount == 1;

		public readonly IType ReturnType;

		public readonly IReadOnlyList<IParameter> Parameters;

		public ILFunction(IMethod method, int codeSize, GenericContext genericContext, ILInstruction body) : base(OpCode.ILFunction)
		{
			this.Method = method;
			this.CodeSize = codeSize;
			this.GenericContext = genericContext;
			this.Body = body;
			this.ReturnType = Method?.ReturnType;
			this.Parameters = Method?.Parameters;
			this.Variables = new ILVariableCollection(this);
		}

		public ILFunction(IType returnType, IReadOnlyList<IParameter> parameters, GenericContext genericContext, ILInstruction body) : base(OpCode.ILFunction)
		{
			this.GenericContext = genericContext;
			this.Body = body;
			this.ReturnType = returnType;
			this.Parameters = parameters;
			this.Variables = new ILVariableCollection(this);
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			for (int i = 0; i < Variables.Count; i++) {
				Debug.Assert(Variables[i].Function == this);
				Debug.Assert(Variables[i].IndexInFunction == i);
				Variables[i].CheckInvariant();
			}
			base.CheckInvariant(phase);
		}

		void CloneVariables()
		{
			throw new NotSupportedException("ILFunction.CloneVariables is currently not supported!");
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			if (Method != null) {
				output.Write(' ');
				Method.WriteTo(output);
			}
			if (IsExpressionTree) {
				output.Write(".ET");
			}
			if (DelegateType != null) {
				output.Write("[");
				DelegateType.WriteTo(output);
				output.Write("]");
			}
			output.WriteLine(" {");
			output.Indent();

			if (IsAsync) {
				output.WriteLine(".async");
			}
			if (IsIterator) {
				output.WriteLine(".iterator");
			}

			output.MarkFoldStart(Variables.Count + " variable(s)", true);
			foreach (var variable in Variables) {
				variable.WriteDefinitionTo(output);
				output.WriteLine();
			}
			output.MarkFoldEnd();
			output.WriteLine();

			foreach (string warning in Warnings) {
				output.WriteLine("//" + warning);
			}

			body.WriteTo(output, options);
			output.WriteLine();

			if (options.ShowILRanges) {
				var unusedILRanges = FindUnusedILRanges();
				if (!unusedILRanges.IsEmpty) {
					output.Write("// Unused IL Ranges: ");
					output.Write(string.Join(", ", unusedILRanges.Intervals.Select(
						range => $"[{range.Start:x4}..{range.InclusiveEnd:x4}]")));
					output.WriteLine();
				}
			}

			output.Unindent();
			output.WriteLine("}");
		}
		
		LongSet FindUnusedILRanges()
		{
			var usedILRanges = new List<LongInterval>();
			MarkUsedILRanges(body);
			return new LongSet(new LongInterval(0, CodeSize)).ExceptWith(new LongSet(usedILRanges));

			void MarkUsedILRanges(ILInstruction inst)
			{
				if (CSharp.SequencePointBuilder.HasUsableILRange(inst)) {
					usedILRanges.Add(new LongInterval(inst.StartILOffset, inst.EndILOffset));
				}
				if (!(inst is ILFunction)) {
					foreach (var child in inst.Children) {
						MarkUsedILRanges(child);
					}
				}
			}
		}

		protected override InstructionFlags ComputeFlags()
		{
			// Creating a lambda may throw OutOfMemoryException
			// We intentionally don't propagate any flags from the lambda body!
			return InstructionFlags.MayThrow | InstructionFlags.ControlFlow;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow | InstructionFlags.ControlFlow;
			}
		}
		
		/// <summary>
		/// Apply a list of transforms to this function.
		/// </summary>
		public void RunTransforms(IEnumerable<IILTransform> transforms, ILTransformContext context)
		{
			this.CheckInvariant(ILPhase.Normal);
			foreach (var transform in transforms) {
				context.CancellationToken.ThrowIfCancellationRequested();
				if (transform is BlockILTransform blockTransform) {
					context.StepStartGroup(blockTransform.ToString());
				} else {
					context.StepStartGroup(transform.GetType().Name);
				}
				transform.Run(this, context);
				this.CheckInvariant(ILPhase.Normal);
				context.StepEndGroup(keepIfEmpty: true);
			}
		}

		int helperVariableCount;

		public ILVariable RegisterVariable(VariableKind kind, IType type, string name = null)
		{
			return RegisterVariable(kind, type, type.GetStackType(), name);
		}

		public ILVariable RegisterVariable(VariableKind kind, StackType stackType, string name = null)
		{
			var type = Method.Compilation.FindType(stackType.ToKnownTypeCode());
			return RegisterVariable(kind, type, stackType, name);
		}

		ILVariable RegisterVariable(VariableKind kind, IType type, StackType stackType, string name = null)
		{
			var variable = new ILVariable(kind, type, stackType);
			if (string.IsNullOrWhiteSpace(name)) {
				name = "I_" + (helperVariableCount++);
				variable.HasGeneratedName = true;
			}
			variable.Name = name;
			Variables.Add(variable);
			return variable;
		}

		/// <summary>
		/// Recombine split variables by replacing all occurrences of variable2 with variable1.
		/// </summary>
		internal void RecombineVariables(ILVariable variable1, ILVariable variable2)
		{
			Debug.Assert(ILVariableEqualityComparer.Instance.Equals(variable1, variable2));
			foreach (var ldloc in variable2.LoadInstructions.ToArray()) {
				ldloc.Variable = variable1;
			}
			foreach (var store in variable2.StoreInstructions.ToArray()) {
				store.Variable = variable1;
			}
			foreach (var ldloca in variable2.AddressInstructions.ToArray()) {
				ldloca.Variable = variable1;
			}
			bool ok = Variables.Remove(variable2);
			Debug.Assert(ok);
		}
	}
}
