#nullable enable
// Copyright (c) 2014-2016 Daniel Grunwald
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A block consists of a list of IL instructions.
	/// 
	/// <para>
	/// Note: if execution reaches the end of the instruction list,
	/// the FinalInstruction (which is not part of the list) will be executed.
	/// The block returns returns the result value of the FinalInstruction.
	/// For blocks returning void, the FinalInstruction will usually be 'nop'.
	/// </para>
	/// 
	/// There are three different uses for blocks:
	/// 1) Blocks in block containers. Used as targets for Branch instructions.
	/// 2) Blocks to group a bunch of statements, e.g. the TrueInst of an IfInstruction.
	/// 3) Inline blocks that evaluate to a value, e.g. for array initializers.
	/// </summary>
	partial class Block : ILInstruction
	{
		public static readonly SlotInfo InstructionSlot = new SlotInfo("Instruction", isCollection: true);
		public static readonly SlotInfo FinalInstructionSlot = new SlotInfo("FinalInstruction");

		public readonly BlockKind Kind;
		public readonly InstructionCollection<ILInstruction> Instructions;
		ILInstruction finalInstruction = null!;

		/// <summary>
		/// For blocks in a block container, this field holds
		/// the number of incoming control flow edges to this block.
		/// </summary>
		/// <remarks>
		/// This variable is automatically updated when adding/removing branch instructions from the ILAst,
		/// or when adding the block as an entry point to a BlockContainer.
		/// </remarks>
		public int IncomingEdgeCount { get; internal set; }

		/// <summary>
		/// A 'final instruction' that gets executed after the <c>Instructions</c> collection.
		/// Provides the return value for the block.
		/// </summary>
		/// <remarks>
		/// Blocks in containers must have 'Nop' as a final instruction.
		/// 
		/// Note that the FinalInstruction is included in Block.Children,
		/// but not in Block.Instructions!
		/// </remarks>
		public ILInstruction FinalInstruction {
			get {
				return finalInstruction;
			}
			set {
				ValidateChild(value);
				SetChildInstruction(ref finalInstruction, value, Instructions.Count);
			}
		}

		protected internal override void InstructionCollectionUpdateComplete()
		{
			base.InstructionCollectionUpdateComplete();
			if (finalInstruction.Parent == this)
				finalInstruction.ChildIndex = Instructions.Count;
		}

		public Block(BlockKind kind = BlockKind.ControlFlow) : base(OpCode.Block)
		{
			this.Kind = kind;
			this.Instructions = new InstructionCollection<ILInstruction>(this, 0);
			this.FinalInstruction = new Nop();
		}

		public override ILInstruction Clone()
		{
			Block clone = new Block(Kind);
			clone.AddILRange(this);
			clone.Instructions.AddRange(this.Instructions.Select(inst => inst.Clone()));
			clone.FinalInstruction = this.FinalInstruction.Clone();
			return clone;
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			for (int i = 0; i < Instructions.Count - 1; i++)
			{
				// only the last instruction may have an unreachable endpoint
				Debug.Assert(!Instructions[i].HasFlag(InstructionFlags.EndPointUnreachable));
			}
			switch (this.Kind)
			{
				case BlockKind.ControlFlow:
					Debug.Assert(finalInstruction.OpCode == OpCode.Nop);
					break;
				case BlockKind.CallInlineAssign:
					Debug.Assert(MatchInlineAssignBlock(out _, out _));
					break;
				case BlockKind.CallWithNamedArgs:
					Debug.Assert(finalInstruction is CallInstruction);
					foreach (var inst in Instructions)
					{
						var stloc = inst as StLoc;
						Debug.Assert(stloc != null, "Instructions in CallWithNamedArgs must be assignments");
						Debug.Assert(stloc!.Variable.Kind == VariableKind.NamedArgument);
						Debug.Assert(stloc.Variable.IsSingleDefinition && stloc.Variable.LoadCount == 1);
						Debug.Assert(stloc.Variable.LoadInstructions.Single().Parent == finalInstruction);
					}
					var call = (CallInstruction)finalInstruction;
					if (call.IsInstanceCall)
					{
						// special case: with instance calls, Instructions[0] must be for the this parameter
						ILVariable v = ((StLoc)Instructions[0]).Variable;
						Debug.Assert(call.Arguments[0].MatchLdLoc(v));
					}
					break;
				case BlockKind.ArrayInitializer:
					var final = finalInstruction as LdLoc;
					Debug.Assert(final != null && final.Variable.IsSingleDefinition && final.Variable.Kind == VariableKind.InitializerTarget);
					IType? type = null;
					Debug.Assert(Instructions[0].MatchStLoc(final!.Variable, out var init) && init.MatchNewArr(out type));
					for (int i = 1; i < Instructions.Count; i++)
					{
						Debug.Assert(Instructions[i].MatchStObj(out ILInstruction? target, out _, out var t) && type != null && type.Equals(t));
						Debug.Assert(target.MatchLdElema(out t, out ILInstruction? array) && type!.Equals(t));
						Debug.Assert(array.MatchLdLoc(out ILVariable? v) && v == final.Variable);
					}
					break;
				case BlockKind.CollectionInitializer:
				case BlockKind.ObjectInitializer:
					var final2 = finalInstruction as LdLoc;
					Debug.Assert(final2 != null);
					var initVar2 = final2!.Variable;
					Debug.Assert(initVar2.StoreCount == 1 && initVar2.Kind == VariableKind.InitializerTarget);
					IType? type2 = null;
					bool condition = Instructions[0].MatchStLoc(final2.Variable, out var init2);
					Debug.Assert(condition);
					Debug.Assert(init2 is NewObj
						|| init2 is DefaultValue
						|| (init2 is CallInstruction c && c.Method.FullNameIs("System.Activator", "CreateInstance") && c.Method.TypeArguments.Count == 1)
						|| (init2 is Block named && named.Kind == BlockKind.CallWithNamedArgs));
					switch (init2)
					{
						case NewObj newObj:
							type2 = newObj.Method.DeclaringType;
							break;
						case DefaultValue defaultValue:
							type2 = defaultValue.Type;
							break;
						case Block callWithNamedArgs when callWithNamedArgs.Kind == BlockKind.CallWithNamedArgs:
							type2 = ((CallInstruction)callWithNamedArgs.FinalInstruction).Method.ReturnType;
							break;
						case CallInstruction ci2 when TransformCollectionAndObjectInitializers.IsRecordCloneMethodCall(ci2):
							type2 = ci2.Method.DeclaringType;
							break;
						case Call c2 when c2.Method.FullNameIs("System.Activator", "CreateInstance") && c2.Method.TypeArguments.Count == 1:
							type2 = c2.Method.TypeArguments[0];
							break;
						default:
							Debug.Assert(false);
							break;
					}
					for (int i = 1; i < Instructions.Count; i++)
					{
						Debug.Assert(Instructions[i] is StLoc || AccessPathElement.GetAccessPath(Instructions[i], type2).Kind != AccessPathKind.Invalid);
					}
					break;
				case BlockKind.DeconstructionConversions:
					Debug.Assert(this.SlotInfo == DeconstructInstruction.ConversionsSlot);
					break;
				case BlockKind.DeconstructionAssignments:
					Debug.Assert(this.SlotInfo == DeconstructInstruction.AssignmentsSlot);
					break;
			}
		}

		public override StackType ResultType {
			get {
				return finalInstruction.ResultType;
			}
		}

		/// <summary>
		/// Gets the name of this block.
		/// </summary>
		public string Label {
			get { return Disassembler.DisassemblerHelpers.OffsetToString(this.StartILOffset); }
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write("Block ");
			output.WriteLocalReference(Label, this, isDefinition: true);
			if (Kind != BlockKind.ControlFlow)
				output.Write($" ({Kind})");
			if (Parent is BlockContainer)
				output.Write(" (incoming: {0})", IncomingEdgeCount);
			output.Write(' ');
			output.MarkFoldStart("{...}");
			output.WriteLine("{");
			output.Indent();
			int index = 0;
			foreach (var inst in Instructions)
			{
				if (options.ShowChildIndexInBlock)
				{
					output.Write("[" + index + "] ");
					index++;
				}
				inst.WriteTo(output, options);
				output.WriteLine();
			}
			if (finalInstruction.OpCode != OpCode.Nop)
			{
				output.Write("final: ");
				finalInstruction.WriteTo(output, options);
				output.WriteLine();
			}
			output.Unindent();
			output.Write("}");
			output.MarkFoldEnd();
		}

		protected override int GetChildCount()
		{
			return Instructions.Count + 1;
		}

		protected override ILInstruction GetChild(int index)
		{
			if (index == Instructions.Count)
				return finalInstruction;
			return Instructions[index];
		}

		protected override void SetChild(int index, ILInstruction value)
		{
			if (index == Instructions.Count)
				FinalInstruction = value;
			else
				Instructions[index] = value;
		}

		protected override SlotInfo GetChildSlot(int index)
		{
			if (index == Instructions.Count)
				return FinalInstructionSlot;
			else
				return InstructionSlot;
		}

		protected override InstructionFlags ComputeFlags()
		{
			var flags = InstructionFlags.None;
			foreach (var inst in Instructions)
			{
				flags |= inst.Flags;
			}
			flags |= FinalInstruction.Flags;
			return flags;
		}

		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.None;
			}
		}

		/// <summary>
		/// Deletes this block from its parent container.
		/// This may cause the indices of other blocks in that container to change.
		/// 
		/// It is an error to call this method on blocks that are not directly within a container.
		/// It is also an error to call this method on the entry-point block.
		/// </summary>
		public void Remove()
		{
			Debug.Assert(ChildIndex > 0);
			var container = (BlockContainer)Parent!;
			Debug.Assert(container.Blocks[ChildIndex] == this);
			container.Blocks.SwapRemoveAt(ChildIndex);
		}

		/// <summary>
		/// Apply a list of transforms to this function.
		/// </summary>
		public void RunTransforms(IEnumerable<IBlockTransform> transforms, BlockTransformContext context)
		{
			this.CheckInvariant(ILPhase.Normal);
			foreach (var transform in transforms)
			{
				context.CancellationToken.ThrowIfCancellationRequested();
				context.StepStartGroup(transform.GetType().Name);
				transform.Run(this, context);
				this.CheckInvariant(ILPhase.Normal);
				context.StepEndGroup();
			}
		}

		/// <summary>
		/// Gets the predecessor of the given instruction.
		/// Returns null if inst.Parent is not a block.
		/// </summary>
		public static ILInstruction? GetPredecessor(ILInstruction inst)
		{
			if (inst.Parent is Block block && inst.ChildIndex > 0)
			{
				return block.Instructions[inst.ChildIndex - 1];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// If inst is a block consisting of a single instruction, returns that instruction.
		/// Otherwise, returns the input instruction.
		/// </summary>
		[return: NotNullIfNotNull("inst")]
		public static ILInstruction? Unwrap(ILInstruction? inst)
		{
			if (inst is Block block)
			{
				if (block.Instructions.Count == 1 && block.finalInstruction.MatchNop())
					return block.Instructions[0];
			}
			return inst;
		}

		/// <summary>
		/// Gets the closest parent Block.
		/// Returns null, if the instruction is not a descendant of a Block.
		/// </summary>
		public static Block? FindClosestBlock(ILInstruction? inst)
		{
			var curr = inst;
			while (curr != null)
			{
				if (curr is Block b)
					return b;
				curr = curr.Parent;
			}
			return null;
		}

		public bool MatchInlineAssignBlock([NotNullWhen(true)] out CallInstruction? call, [NotNullWhen(true)] out ILInstruction? value)
		{
			call = null;
			value = null;
			if (this.Kind != BlockKind.CallInlineAssign)
				return false;
			if (this.Instructions.Count != 1)
				return false;
			call = this.Instructions[0] as CallInstruction;
			if (call == null || call.Arguments.Count == 0)
				return false;
			if (!call.Arguments.Last().MatchStLoc(out var tmp, out value))
				return false;
			if (!(tmp.IsSingleDefinition && tmp.LoadCount == 1))
				return false;
			return this.FinalInstruction.MatchLdLoc(tmp);
		}

		public bool MatchIfAtEndOfBlock([NotNullWhen(true)] out ILInstruction? condition, [NotNullWhen(true)] out ILInstruction? trueInst, [NotNullWhen(true)] out ILInstruction? falseInst)
		{
			condition = null;
			trueInst = null;
			falseInst = null;
			if (Instructions.Count < 2)
				return false;
			if (Instructions[Instructions.Count - 2].MatchIfInstruction(out condition, out trueInst))
			{
				// Swap trueInst<>falseInst for every logic.not in the condition.
				falseInst = Instructions.Last();
				while (condition.MatchLogicNot(out var arg))
				{
					condition = arg;
					(trueInst, falseInst) = (falseInst, trueInst);
				}
				return true;
			}
			return false;
		}
	}

	public enum BlockKind
	{
		/// <summary>
		/// Block is used for control flow.
		/// All blocks in block containers must have this type.
		/// Control flow blocks cannot evaluate to a value (FinalInstruction must be Nop).
		/// </summary>
		ControlFlow,
		/// <summary>
		/// Block is used for array initializers, e.g. `new int[] { expr1, expr2 }`.
		/// </summary>
		ArrayInitializer,
		CollectionInitializer,
		ObjectInitializer,
		StackAllocInitializer,
		/// <summary>
		/// Block is used for using the result of a property setter inline.
		/// Example: <code>Use(this.Property = value);</code>
		/// This is only for inline assignments to property or indexers; other inline assignments work
		/// by using the result value of the stloc/stobj instructions.
		/// 
		/// Constructed by TransformAssignment.
		/// Can be deconstructed using Block.MatchInlineAssignBlock().
		/// </summary>
		/// <example>
		/// Block {
		///   call setter(..., stloc s(...))
		///   final: ldloc s
		/// }
		/// </example>
		CallInlineAssign,
		/// <summary>
		/// Call using named arguments.
		/// </summary>
		/// <remarks>
		/// Each instruction is assigning to a new local.
		/// The final instruction is a call.
		/// The locals for this block have exactly one store and
		/// exactly one load, which must be an immediate argument to the call.
		/// </remarks>
		/// <example>
		/// Block {
		///   stloc arg0 = ...
		///   stloc arg1 = ...
		///   final: call M(..., arg1, arg0, ...)
		/// }
		/// </example>
		CallWithNamedArgs,
		/// <summary>
		/// <see cref="DeconstructInstruction"/>
		/// </summary>
		DeconstructionConversions,
		/// <summary>
		/// <see cref="DeconstructInstruction"/>
		/// </summary>
		DeconstructionAssignments,
		WithInitializer,
	}
}
