using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// C# switch statements are not necessarily compiled into IL switch instructions.
	/// For example, when the label values are not contiguous, the C# compiler
	/// will generate if statements similar to a binary search.
	/// 
	/// This class analyses such sequences of if statements to reconstruct the original switch.
	/// </summary>
	/// <remarks>
	/// This analysis expects to be run on basic blocks (not extended basic blocks).
	/// </remarks>
	class SwitchAnalysis
	{
		/// <summary>
		/// The variable that is used to represent the switch expression.
		/// <c>null</c> while analyzing the first block.
		/// </summary>
		ILVariable switchVar;

		/// <summary>
		/// The variable to be used as the argument of the switch instruction.
		/// </summary>
		public ILVariable SwitchVariable
		{
			get { return switchVar; }
		}

		/// <summary>
		/// Whether at least one the analyzed blocks contained an IL switch constructors.
		/// </summary>
		public bool ContainsILSwitch { get; private set; }

		/// <summary>
		/// Gets the sections that were detected by the previoous AnalyzeBlock() call.
		/// </summary>
		public readonly List<KeyValuePair<LongSet, ILInstruction>> Sections = new List<KeyValuePair<LongSet, ILInstruction>>();

		readonly Dictionary<Block, int> targetBlockToSectionIndex = new Dictionary<Block, int>();

		/// <summary>
		/// Blocks that can be deleted if the tail of the initial block is replaced with a switch instruction.
		/// </summary>
		public readonly List<Block> InnerBlocks = new List<Block>();

		Block rootBlock;

		/// <summary>
		/// Analyze the last two statements in the block and see if they can be turned into a
		/// switch instruction.
		/// </summary>
		/// <returns>true if the block could be analyzed successfully; false otherwise</returns>
		public bool AnalyzeBlock(Block block)
		{
			switchVar = null;
			rootBlock = block;
			targetBlockToSectionIndex.Clear();
			Sections.Clear();
			InnerBlocks.Clear();
			ContainsILSwitch = false;
			return AnalyzeBlock(block, LongSet.Universe, tailOnly: true);
		}

		/// <summary>
		/// Analyzes the tail end (last two instructions) of a block.
		/// </summary>
		/// <remarks>
		/// Sets <c>switchVar</c> and <c>defaultInstruction</c> if they are null,
		/// and adds found sections to <c>sectionLabels</c> and <c>sectionInstructions</c>.
		/// 
		/// If the function returns false, <c>sectionLabels</c> and <c>sectionInstructions</c> are unmodified.
		/// </remarks>
		/// <param name="block">The block to analyze.</param>
		/// <param name="inputValues">The possible values of the "interesting" variable
		/// when control flow reaches this block.</param>
		/// <param name="tailOnly">If true, analyze only the tail (last two instructions).
		/// If false, analyze the whole block.</param>
		bool AnalyzeBlock(Block block, LongSet inputValues, bool tailOnly = false)
		{
			if (tailOnly) {
				Debug.Assert(block == rootBlock);
				if (block.Instructions.Count < 2)
					return false;
			} else {
				Debug.Assert(switchVar != null); // switchVar should always be determined by the top-level call
				if (block.IncomingEdgeCount != 1 || block == rootBlock)
					return false; // for now, let's only consider if-structures that form a tree
				if (block.Instructions.Count != 2)
					return false;
				if (block.Parent != rootBlock.Parent)
					return false; // all blocks should belong to the same container
			}
			var inst = block.Instructions[block.Instructions.Count - 2];
			ILInstruction condition, trueInst;
			LongSet trueValues;
			if (inst.MatchIfInstruction(out condition, out trueInst)
				&& AnalyzeCondition(condition, out trueValues)
			) {
				trueValues = trueValues.IntersectWith(inputValues);
				Block trueBlock;
				if (trueInst.MatchBranch(out trueBlock) && AnalyzeBlock(trueBlock, trueValues)) {
					// OK, true block was further analyzed.
					InnerBlocks.Add(trueBlock);
				} else {
					// Create switch section for trueInst.
					AddSection(trueValues, trueInst);
				}
			} else if (inst.OpCode == OpCode.SwitchInstruction) {
				if (AnalyzeSwitch((SwitchInstruction)inst, inputValues, out trueValues)) {
					ContainsILSwitch = true; // OK
				} else { // switch analysis failed (e.g. switchVar mismatch)
					return false;
				}
			} else { // unknown inst
				return false;
			}

			var remainingValues = inputValues.ExceptWith(trueValues);
			ILInstruction falseInst = block.Instructions.Last();
			Block falseBlock;
			if (falseInst.MatchBranch(out falseBlock) && AnalyzeBlock(falseBlock, remainingValues)) {
				// OK, false block was further analyzed.
				InnerBlocks.Add(falseBlock);
			} else {
				// Create switch section for falseInst.
				AddSection(remainingValues, falseInst);
			}
			return true;
		}

		private bool AnalyzeSwitch(SwitchInstruction inst, LongSet inputValues, out LongSet anyMatchValues)
		{
			Debug.Assert(inst.DefaultBody is Nop);
			anyMatchValues = LongSet.Empty;
			long offset;
			if (MatchSwitchVar(inst.Value)) {
				offset = 0;
			} else if (inst.Value.OpCode == OpCode.BinaryNumericInstruction) {
				var bop = (BinaryNumericInstruction)inst.Value;
				if (bop.CheckForOverflow)
					return false;
				long val;
				if (MatchSwitchVar(bop.Left) && MatchLdcI(bop.Right, out val)) {
					switch (bop.Operator) {
						case BinaryNumericOperator.Add:
							offset = -val;
							break;
						case BinaryNumericOperator.Sub:
							offset = val;
							break;
						default: // unknown bop.Operator
							return false;
					}
				} else { // unknown bop.Left
					return false;
				}
			} else { // unknown inst.Value
				return false;
			}
			foreach (var section in inst.Sections) {
				var matchValues = section.Labels.AddOffset(offset).IntersectWith(inputValues);
				AddSection(matchValues, section.Body);
				anyMatchValues = anyMatchValues.UnionWith(matchValues);
			}
			return true;
		}
		
		/// <summary>
		/// Adds a new section to the Sections list.
		/// 
		/// If the instruction is a branch instruction, unify the new section with an existing section
		/// that also branches to the same target.
		/// </summary>
		void AddSection(LongSet values, ILInstruction inst)
		{
			Block targetBlock;
			if (inst.MatchBranch(out targetBlock)) {
				int index;
				if (targetBlockToSectionIndex.TryGetValue(targetBlock, out index)) {
					Sections[index] = new KeyValuePair<LongSet, ILInstruction>(
						Sections[index].Key.UnionWith(values),
						inst
					);
				} else {
					targetBlockToSectionIndex.Add(targetBlock, Sections.Count);
					Sections.Add(new KeyValuePair<LongSet, ILInstruction>(values, inst));
				}
			} else {
				Sections.Add(new KeyValuePair<LongSet, ILInstruction>(values, inst));
			}
		}

		bool MatchSwitchVar(ILInstruction inst)
		{
			if (switchVar != null)
				return inst.MatchLdLoc(switchVar);
			else
				return inst.MatchLdLoc(out switchVar);
		}

		bool MatchLdcI(ILInstruction inst, out long val)
		{
			if (inst.MatchLdcI8(out val))
				return true;
			int intVal;
			if (inst.MatchLdcI4(out intVal)) {
				val = intVal;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Analyzes the boolean condition, returning the set of values of the interesting
		/// variable for which the condition evaluates to true.
		/// </summary>
		private bool AnalyzeCondition(ILInstruction condition, out LongSet trueValues)
		{
			ILInstruction arg;
			Comp comp = condition as Comp;
			long val;
			if (comp != null && MatchSwitchVar(comp.Left) && MatchLdcI(comp.Right, out val)) {
				// if (comp(V OP val))
				switch (comp.Kind) {
					case ComparisonKind.Equality:
						trueValues = new LongSet(val);
						return true;
					case ComparisonKind.Inequality:
						trueValues = new LongSet(val).Invert();
						return true;
					case ComparisonKind.LessThan:
						trueValues = MakeGreaterThanOrEqualSet(val, comp.Sign).Invert();
						return true;
					case ComparisonKind.LessThanOrEqual:
						trueValues = MakeLessThanOrEqualSet(val, comp.Sign);
						return true;
					case ComparisonKind.GreaterThan:
						trueValues = MakeLessThanOrEqualSet(val, comp.Sign).Invert();
						return true;
					case ComparisonKind.GreaterThanOrEqual:
						trueValues = MakeGreaterThanOrEqualSet(val, comp.Sign);
						return true;
					default:
						trueValues = LongSet.Empty;
						return false;
				}
			} else if (MatchSwitchVar(condition)) {
				// if (ldloc V) --> branch for all values except 0
				trueValues = new LongSet(0).Invert();
				return true;
			} else if (condition.MatchLogicNot(out arg)) {
				// if (logic.not(X)) --> branch for all values where if (X) does not branch
				LongSet falseValues;
				bool res = AnalyzeCondition(arg, out falseValues);
				trueValues = falseValues.Invert();
				return res;
			} else {
				trueValues = LongSet.Empty;
				return false;
			}
		}

		private LongSet MakeGreaterThanOrEqualSet(long val, Sign sign)
		{
			if (sign == Sign.Signed) {
				return new LongSet(LongInterval.Inclusive(val, long.MaxValue));
			} else {
				Debug.Assert(sign == Sign.Unsigned);
				if (val >= 0) {
					// The range val to ulong.MaxValue expressed with signed longs
					// is not a single contiguous range, but two ranges:
					return new LongSet(LongInterval.Inclusive(val, long.MaxValue))
						.UnionWith(new LongSet(new LongInterval(long.MinValue, 0)));
				} else {
					return new LongSet(new LongInterval(val, 0));
				}
			}
		}

		private LongSet MakeLessThanOrEqualSet(long val, Sign sign)
		{
			if (sign == Sign.Signed) {
				return new LongSet(LongInterval.Inclusive(long.MinValue, val));
			} else {
				Debug.Assert(sign == Sign.Unsigned);
				if (val >= 0) {
					return new LongSet(LongInterval.Inclusive(0, val));
				} else {
					// The range 0 to (ulong)val expressed with signed longs
					// is not a single contiguous range, but two ranges:
					return new LongSet(LongInterval.Inclusive(0, long.MaxValue))
						.UnionWith(new LongSet(LongInterval.Inclusive(long.MinValue, val)));
				}
			}
		}
	}
}
