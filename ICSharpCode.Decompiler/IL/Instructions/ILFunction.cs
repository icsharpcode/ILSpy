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
using System.Diagnostics;
using System.Linq;
using System.Collections.Immutable;

using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	partial class ILFunction
	{
		/// <summary>
		/// Gets the method definition from metadata.
		/// May be null for functions that were not constructed from metadata,
		/// e.g., expression trees.
		/// </summary>
		public readonly IMethod Method;

		/// <summary>
		/// Gets the generic context of this function.
		/// </summary>
		public readonly GenericContext GenericContext;

		/// <summary>
		/// Gets the name of this function, usually this returns the name from metadata.
		/// <para>
		/// For local functions:
		/// This is the name that is used to declare and use the function.
		/// It may not conflict with the names of local variables of ancestor functions
		/// and may be overwritten by the AssignVariableNames step.
		/// 
		/// For top-level functions, delegates and expressions trees modifying this usually
		/// has no effect, as the name should not be used in the final AST construction.
		/// </para>
		/// </summary>
		public string Name;

		/// <summary>
		/// Size of the IL code in this function.
		/// Note: after async/await transform, this is the code size of the MoveNext function.
		/// </summary>
		public int CodeSize;

		/// <summary>
		/// List of ILVariables used in this function.
		/// </summary>
		public readonly ILVariableCollection Variables;

		/// <summary>
		/// Gets the scope in which the local function is declared.
		/// Returns null, if this is not a local function.
		/// </summary>
		public BlockContainer DeclarationScope { get; internal set; }

		/// <summary>
		/// Gets the set of captured variables by this ILFunction.
		/// </summary>
		/// <remarks>This is populated by the <see cref="TransformDisplayClassUsage" /> step.</remarks>
		public HashSet<ILVariable> CapturedVariables { get; } = new HashSet<ILVariable>(ILVariableEqualityComparer.Instance);

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

		/// <summary>
		/// Gets whether the YieldReturnDecompiler determined that the Mono C# compiler was used to compile this function.
		/// </summary>
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

		/// <summary>
		/// If this function is a local function, this field stores the reduced version of the function.
		/// </summary>
		internal TypeSystem.Implementation.LocalFunctionMethod ReducedMethod;

		public DebugInfo.AsyncDebugInfo AsyncDebugInfo;

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
		/// Otherwise this must be null.
		/// </summary>
		public IType DelegateType;

		ILFunctionKind kind;

		/// <summary>
		/// Gets which kind of function this is.
		/// </summary>
		public ILFunctionKind Kind {
			get => kind;
			internal set {
				if (kind == ILFunctionKind.TopLevelFunction || kind == ILFunctionKind.LocalFunction)
					throw new InvalidOperationException("ILFunction.Kind of a top-level or local function may not be changed.");
				kind = value;
			}
		}

		/// <summary>
		/// Return type of this function.
		/// Might be null, if this function was not created from metadata.
		/// </summary>
		public readonly IType ReturnType;

		/// <summary>
		/// List of parameters of this function.
		/// Might be null, if this function was not created from metadata.
		/// </summary>
		public readonly IReadOnlyList<IParameter> Parameters;

		/// <summary>
		/// List of candidate locations for sequence points. Includes any offset
		/// where the stack is empty, nop instructions, and the instruction following
		/// a call instruction
		/// </summary>
		public List<int> SequencePointCandidates { get; set; }

		/// <summary>
		/// Constructs a new ILFunction from the given metadata and with the given ILAst body.
		/// </summary>
		/// <remarks>
		/// Use <see cref="ILReader"/> to create ILAst.
		/// <paramref name="method"/> may be null.
		/// </remarks>
		public ILFunction(IMethod method, int codeSize, GenericContext genericContext, ILInstruction body, ILFunctionKind kind = ILFunctionKind.TopLevelFunction) : base(OpCode.ILFunction)
		{
			this.Method = method;
			this.Name = Method?.Name;
			this.CodeSize = codeSize;
			this.GenericContext = genericContext;
			this.Body = body;
			this.ReturnType = Method?.ReturnType;
			this.Parameters = Method?.Parameters;
			this.Variables = new ILVariableCollection(this);
			this.LocalFunctions = new InstructionCollection<ILFunction>(this, 1);
			this.kind = kind;
		}

		/// <summary>
		/// This constructor is only to be used by the TransformExpressionTrees step.
		/// </summary>
		internal ILFunction(IType returnType, IReadOnlyList<IParameter> parameters, GenericContext genericContext, ILInstruction body) : base(OpCode.ILFunction)
		{
			this.GenericContext = genericContext;
			this.Body = body;
			this.ReturnType = returnType;
			this.Parameters = parameters;
			this.Variables = new ILVariableCollection(this);
			this.LocalFunctions = new InstructionCollection<ILFunction>(this, 1);
			this.kind = ILFunctionKind.ExpressionTree;
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			switch (kind) {
				case ILFunctionKind.TopLevelFunction:
					Debug.Assert(Parent == null);
					Debug.Assert(DelegateType == null);
					Debug.Assert(DeclarationScope == null);
					Debug.Assert(Method != null);
					break;
				case ILFunctionKind.Delegate:
					Debug.Assert(DelegateType != null);
					Debug.Assert(DeclarationScope == null);
					Debug.Assert(!(DelegateType?.FullName == "System.Linq.Expressions.Expression" && DelegateType.TypeParameterCount == 1));
					break;
				case ILFunctionKind.ExpressionTree:
					Debug.Assert(DelegateType != null);
					Debug.Assert(DeclarationScope == null);
					Debug.Assert(DelegateType?.FullName == "System.Linq.Expressions.Expression" && DelegateType.TypeParameterCount == 1);
					break;
				case ILFunctionKind.LocalFunction:
					Debug.Assert(Parent is ILFunction && SlotInfo == ILFunction.LocalFunctionsSlot);
					Debug.Assert(DeclarationScope != null);
					Debug.Assert(DelegateType == null);
					Debug.Assert(Method != null);
					break;
			}
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
			switch (kind) {
				case ILFunctionKind.ExpressionTree:
					output.Write(".ET");
					break;
				case ILFunctionKind.LocalFunction:
					output.Write(".local");
					break;
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
			if (DeclarationScope != null) {
				output.Write("declared as " + Name + " in ");
				output.WriteLocalReference(DeclarationScope.EntryPoint.Label, DeclarationScope);
				output.WriteLine();
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

			foreach (var localFunction in LocalFunctions) {
				output.WriteLine();
				localFunction.WriteTo(output, options);
			}

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
			if (variable1 == variable2)
				return;
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

	public enum ILFunctionKind
	{
		/// <summary>
		/// ILFunction is a "top-level" function, i.e., method, accessor, constructor, destructor or operator.
		/// </summary>
		TopLevelFunction,
		/// <summary>
		/// ILFunction is a delegate or lambda expression.
		/// </summary>
		/// <remarks>
		/// This kind is introduced by the DelegateConstruction and TransformExpressionTrees steps in the decompiler pipeline.
		/// </remarks>
		Delegate,
		/// <summary>
		/// ILFunction is an expression tree lambda.
		/// </summary>
		/// <remarks>
		/// This kind is introduced by the TransformExpressionTrees step in the decompiler pipeline.
		/// </remarks>
		ExpressionTree,
		/// <summary>
		/// ILFunction is a C# 7.0 local function.
		/// </summary>
		/// <remarks>
		/// This kind is introduced by the LocalFunctionDecompiler step in the decompiler pipeline.
		/// </remarks>
		LocalFunction
	}
}
