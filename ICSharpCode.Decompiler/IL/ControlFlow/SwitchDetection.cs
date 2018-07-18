// Copyright (c) 2016 Daniel Grunwald
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

using ICSharpCode.Decompiler.IL.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// C# switch statements are not necessarily compiled into
	/// IL switch instructions (e.g. when the integer values are non-contiguous).
	/// 
	/// Detect sequences of conditional branches that all test a single integer value,
	/// and simplify them into a ILAst switch instruction (which like C# does not require contiguous values).
	/// </summary>
	class SwitchDetection : IILTransform
	{
		private ILTransformContext context;
		private BlockContainer currentContainer;
		private ControlFlowGraph controlFlowGraph;

		SwitchAnalysis analysis = new SwitchAnalysis();

		public void Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;

			foreach (var container in function.Descendants.OfType<BlockContainer>()) {
				currentContainer = container;
				controlFlowGraph = null;

				bool blockContainerNeedsCleanup = false;
				foreach (var block in container.Blocks) {
					context.CancellationToken.ThrowIfCancellationRequested();
					ProcessBlock(block, ref blockContainerNeedsCleanup);
				}
				if (blockContainerNeedsCleanup) {
					Debug.Assert(container.Blocks.All(b => b.Instructions.Count != 0 || b.IncomingEdgeCount == 0));
					container.Blocks.RemoveAll(b => b.Instructions.Count == 0);
				}
			}
		}

		void ProcessBlock(Block block, ref bool blockContainerNeedsCleanup)
		{
			bool analysisSuccess = analysis.AnalyzeBlock(block);
			KeyValuePair<LongSet, ILInstruction> defaultSection;
			if (analysisSuccess && UseCSharpSwitch(out defaultSection)) {
				// complex multi-block switch that can be combined into a single SwitchInstruction
				ILInstruction switchValue = new LdLoc(analysis.SwitchVariable);
				if (switchValue.ResultType == StackType.Unknown) {
					// switchValue must have a result type of either I4 or I8
					switchValue = new Conv(switchValue, PrimitiveType.I8, false, TypeSystem.Sign.Signed);
				}
				var sw = new SwitchInstruction(switchValue);
				foreach (var section in analysis.Sections) {
					sw.Sections.Add(new SwitchSection {
						Labels = section.Key,
						Body = section.Value
					});
				}
				if (block.Instructions.Last() is SwitchInstruction) {
					// we'll replace the switch
				} else {
					Debug.Assert(block.Instructions.SecondToLastOrDefault() is IfInstruction);
					// Remove branch/leave after if; it's getting moved into a section.
					block.Instructions.RemoveAt(block.Instructions.Count - 1);
				}
				block.Instructions[block.Instructions.Count - 1] = sw;
				
				// mark all inner blocks that were converted to the switch statement for deletion
				foreach (var innerBlock in analysis.InnerBlocks) {
					Debug.Assert(innerBlock.Parent == block.Parent);
					Debug.Assert(innerBlock != ((BlockContainer)block.Parent).EntryPoint);
					innerBlock.Instructions.Clear();
				}

				controlFlowGraph = null; // control flow graph is no-longer valid
				blockContainerNeedsCleanup = true;
				SortSwitchSections(sw);
			} else {
				// 2nd pass of SimplifySwitchInstruction (after duplicating return blocks),
				// (1st pass was in ControlFlowSimplification)
				SimplifySwitchInstruction(block);
			}
		}

		internal static void SimplifySwitchInstruction(Block block)
		{
			// due to our of of basic blocks at this point,
			// switch instructions can only appear as last insturction
			var sw = block.Instructions.LastOrDefault() as SwitchInstruction;
			if (sw == null)
				return;

			// ControlFlowSimplification runs early (before any other control flow transforms).
			// Any switch instructions will only have branch instructions in the sections.

			// Combine sections with identical branch target:
			var dict = new Dictionary<Block, SwitchSection>(); // branch target -> switch section
			sw.Sections.RemoveAll(
				section => {
					if (section.Body.MatchBranch(out Block target)) {
						if (dict.TryGetValue(target, out SwitchSection primarySection)) {
							primarySection.Labels = primarySection.Labels.UnionWith(section.Labels);
							primarySection.HasNullLabel |= section.HasNullLabel;
							return true; // remove this section
						} else {
							dict.Add(target, section);
						}
					}
					return false;
				});
			AdjustLabels(sw);
			SortSwitchSections(sw);
		}

		static void SortSwitchSections(SwitchInstruction sw)
		{
			sw.Sections.ReplaceList(sw.Sections.OrderBy(s => (s.Body as Branch)?.TargetILOffset).ThenBy(s => s.Labels.Values.FirstOrDefault()));
		}

		static void AdjustLabels(SwitchInstruction sw)
		{
			if (sw.Value is BinaryNumericInstruction bop && !bop.CheckForOverflow && bop.Right.MatchLdcI(out long val)) {
				// Move offset into labels:
				long offset;
				switch (bop.Operator) {
					case BinaryNumericOperator.Add:
						offset = unchecked(-val);
						break;
					case BinaryNumericOperator.Sub:
						offset = val;
						break;
					default: // unknown bop.Operator
						return;
				}
				sw.Value = bop.Left;
				foreach (var section in sw.Sections) {
					section.Labels = section.Labels.AddOffset(offset);
				}
			}
		}

		const ulong MaxValuesPerSection = 50;

		/// <summary>
		/// Tests whether we should prefer a switch statement over an if statement.
		/// </summary>
		private bool UseCSharpSwitch(out KeyValuePair<LongSet, ILInstruction> defaultSection)
		{
			if (!analysis.InnerBlocks.Any()) {
				defaultSection = default(KeyValuePair<LongSet, ILInstruction>);
				return false;
			}
			defaultSection = analysis.Sections.FirstOrDefault(s => s.Key.Count() > MaxValuesPerSection);
			if (defaultSection.Value == null) {
				// no default section found?
				// This should never happen, as we'd need 2^64/MaxValuesPerSection sections to hit this case...
				return false;
			}
			var defaultSectionKey = defaultSection.Key;
			if (analysis.Sections.Any(s => !s.Key.SetEquals(defaultSectionKey) && s.Key.Count() > MaxValuesPerSection)) {
				// Only the default section is allowed to have tons of keys.
				// C# doesn't support "case 1 to 100000000", and we don't want to generate
				// gigabytes of case labels.
				return false;
			}

			// good enough indicator that the surrounding code also forms a switch statement
			if (analysis.ContainsILSwitch || MatchRoslynSwitchOnString())
				return true;

			int ifCount = analysis.InnerBlocks.Count + 1;
			int labelCount = analysis.Sections.Where(s => !s.Key.SetEquals(defaultSectionKey)).Sum(s => s.Key.Intervals.Length);
			// heuristic to determine if a block would be better represented as an if statement rather than a case statement
			if (ifCount < labelCount)
				return false;
			
			// if there is no ILSwitch, there's still many control flow patterns that 
			// match a switch statement but were originally just regular if statements,
			// and converting them to switches results in poor quality code with goto statements
			// 
			// If a single break target cannot be identified, then the equivalent switch statement would require goto statements.
			// These goto statements may be "goto case x" or "goto default", but these are a hint that the original code was not a switch,
			// and that the switch statement may be very poor quality. 
			// Thus the rule of thumb is no goto statements if the original code didn't include them
			return !SwitchUsesGoto();
		}

		/// <summary>
		/// stloc switchValueVar(call ComputeStringHash(switchValue))
		/// 
		/// Previously, the roslyn case block heads were added to the flowBlocks for case control flow analysis.
		/// This forbade goto case statements (as is the purpose of ValidatePotentialSwitchFlow)
		/// Identifying the roslyn string switch head is a better indicator for UseCSharpSwitch
		/// </summary>
		private bool MatchRoslynSwitchOnString()
		{
			var insns = analysis.RootBlock.Instructions;
			return insns.Count >= 3 && SwitchOnStringTransform.MatchComputeStringHashCall(insns[insns.Count - 3], analysis.SwitchVariable, out var switchLdLoc);
		}

		/// <summary>
		/// Determines if the analysed switch can be constructed without any gotos
		/// </summary>
		private bool SwitchUsesGoto()
		{
			if (controlFlowGraph == null)
				controlFlowGraph = new ControlFlowGraph(currentContainer, context.CancellationToken);

			// grab the control flow nodes for blocks targetted by each section
			var caseNodes = new List<ControlFlowNode>();
			ControlFlowNode defaultNode = null;

			foreach (var s in analysis.Sections) {
				// leave sections not considered
				if (s.Value.MatchBranch(out var block) && !IsContinue(block)) {
					if (s.Key.Count() > MaxValuesPerSection)
						defaultNode = controlFlowGraph.GetNode(block);
					else
						caseNodes.Add(controlFlowGraph.GetNode(block));
				}
			}

			var flowBlocks = analysis.InnerBlocks.Concat(new[] {analysis.RootBlock}).ToHashSet();
			
			AddNullCase(flowBlocks, caseNodes);
			
			// all predecessors of case blocks must be part of the switch logic (to avoid gotos)
			foreach (var caseNode in caseNodes)
				if (caseNode.Predecessors.Any(n => !flowBlocks.Contains(n.UserData)))
					return true;
			
			if (!FindBreakTarget(caseNodes, defaultNode, out var breakTarget))
				return true; // more than one exit point, requires goto statements

			// if the switch has a single break target there is one more hint
			// The break target cannot be inlined, and should have a higher IL offset than anything in the switch body
			return breakTarget?.UserIndex < caseNodes.Select(c => c.UserIndex).Max();
		}

		/// <summary>
		/// Does some of the analysis of SwitchOnNullableTransform to add the null case control flow
		/// to the results of SwitchAnaylsis
		/// </summary>
		private void AddNullCase(HashSet<Block> flowBlocks, List<ControlFlowNode> caseNodes)
		{
			if (analysis.RootBlock.IncomingEdgeCount != 1)
				return;
			
			// if (comp(logic.not(call get_HasValue(ldloca nullableVar))) br NullCase
			// br RootBlock
			var nullableBlock = (Block)controlFlowGraph.GetNode(analysis.RootBlock).Predecessors.SingleOrDefault()?.UserData;
			if (nullableBlock == null ||
			    nullableBlock.Instructions.Count < 2 ||
			    !nullableBlock.Instructions.Last().MatchBranch(analysis.RootBlock) ||
			    !nullableBlock.Instructions.SecondToLastOrDefault().MatchIfInstruction(out var cond, out var trueInst) ||
			    !cond.MatchLogicNot(out var getHasValue) ||
			    !NullableLiftingTransform.MatchHasValueCall(getHasValue, out ILInstruction nullableInst))
				return;
			
			// could check that nullableInst is ldloc or ldloca and that the switch variable matches a GetValueOrDefault
			// but the effect of adding an incorrect block to the flowBlock list would only be disasterous if it branched directly
			// to a candidate case block

			// must branch to a case label, otherwise we can proceed fine and let SwitchOnNullableTransform do all the work
			if (!trueInst.MatchBranch(out var nullBlock) || !caseNodes.Exists(n => n.UserData == nullBlock))
				return;

			//add the null case logic to the incoming flow blocks
			flowBlocks.Add(nullableBlock);
		}

		internal static bool IsContinue(Block block) => false;
		
		/// <summary>
		/// Enumerates all nodes via which a domination tree can be exited. (Not distinct)
		/// That is, all nodes with at least one dominated predecessor which are not dominated themselves
		/// 
		/// Note that while this construction appears less efficient than a recursive solution, 
		/// a recursive solution would require the use of the Visted flag to avoid loops in the dominator tree
		/// </summary>
		internal static IEnumerable<ControlFlowNode> GetDominatorTreeExits(ControlFlowNode dominator) =>
			dominator.DominatorTreeChildren.Concat(new[] {dominator})
				.SelectMany(n => n.Successors)
				.Where(n => !dominator.Dominates(n));

		/// <summary>
		/// Lists all potential targets for break; statements from a domination tree,
		/// assuming the domination tree must be exited via either break; or continue;
		/// </summary>
		internal static IEnumerable<ControlFlowNode> GetBreakTargets(ControlFlowNode dominator) =>
			GetDominatorTreeExits(dominator).Where(n => !IsContinue((Block)n.UserData));
		
		/// <summary>
		/// Finds the target node of all branches leaving the dominator tree of the cases of a switch block.
		/// Returns true if a single target was found, or if no break statements are required (breakTarget = null)
		/// If the defaultNode is the breakTarget, then it should not be inlined.
		/// 
		/// Note that branches to case labels ("goto case x") are considered break targets, and will fail this method
		/// 
		/// defaultNode may be null if there is no control block associated with the default case. 
		/// </summary>
		/// <returns></returns>
		internal static bool FindBreakTarget(List<ControlFlowNode> caseNodes, ControlFlowNode defaultNode, out ControlFlowNode breakTarget)
		{
			breakTarget = null;
			var breakTargets = caseNodes.SelectMany(GetBreakTargets).ToHashSet();

			// no cases require break statements, inlining of default case doesn't matter
			if (breakTargets.Count == 0)
				return true;
			
			if (defaultNode != null) {
				// default case is break target, don't inline
				if (breakTargets.Count == 1 && breakTargets.Single() == defaultNode) {
					breakTarget = defaultNode;
					return true;
				}
				
				// default case requires inlining, ensure break target matches that of case statements
				breakTargets.AddRange(GetBreakTargets(defaultNode));
			}

			// single break target found
			if (breakTargets.Count == 1) {
				breakTarget = breakTargets.Single();
				return true;
			}

			// more than 1 break target found
			return false;
		}
	}
}
