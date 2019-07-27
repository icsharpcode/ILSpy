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
		private readonly SwitchAnalysis analysis = new SwitchAnalysis();

		private ILTransformContext context;
		private BlockContainer currentContainer;
		private ControlFlowGraph controlFlowGraph;
		private LoopContext loopContext;

		/// <summary>
		/// When detecting a switch, it is important to distinguish Branch instructions which will
		/// eventually decompile to continue; statements. 
		/// 
		/// A LoopContext is constructed for a node and its dominator tree, as for a Branch to be a continue;
		/// statement, it must be contained within the target-loop
		/// 
		/// This class also supplies the depth of the loop targetted by a continue; statement relative to the
		/// context node, to avoid (or eventually support) labelled continues to outer loops
		/// </summary>
		public class LoopContext
		{
			private readonly IDictionary<ControlFlowNode, int> continueDepth = new Dictionary<ControlFlowNode, int>();

			public LoopContext(ControlFlowGraph cfg, ControlFlowNode contextNode)
			{
				var loopHeads = new List<ControlFlowNode>();

				void Analyze(ControlFlowNode n)
				{
					if (n.Visited)
						return;

					n.Visited = true;
					if (n.Dominates(contextNode))
						loopHeads.Add(n);
					else
						n.Successors.ForEach(Analyze);
				}
				contextNode.Successors.ForEach(Analyze);
				ResetVisited(cfg.cfg);

				int l = 1;
				foreach (var loopHead in loopHeads.OrderBy(n => n.PostOrderNumber))
					continueDepth[FindContinue(loopHead)] = l++;
			}

			private static ControlFlowNode FindContinue(ControlFlowNode loopHead)
			{
				// potential continue target
				var pred = loopHead.Predecessors.OnlyOrDefault(p => p != loopHead && loopHead.Dominates(p));
				if (pred == null)
					return loopHead;

				// match for loop increment block
				if (pred.Successors.Count == 1) {
					if (HighLevelLoopTransform.MatchIncrementBlock((Block)pred.UserData, out var target) &&target == loopHead.UserData)
						return pred;
				}

				// match do-while condition
				if (pred.Successors.Count <= 2) {
					if (HighLevelLoopTransform.MatchDoWhileConditionBlock((Block)pred.UserData, out var t1, out var t2) &&
					    (t1 == loopHead.UserData || t2 == loopHead.UserData))
						return pred;
				}

				return loopHead;
			}
			
			public bool MatchContinue(ControlFlowNode node) => MatchContinue(node, out var _);

			public bool MatchContinue(ControlFlowNode node, int depth) => 
				MatchContinue(node, out int _depth) && depth == _depth;

			public bool MatchContinue(ControlFlowNode node, out int depth) => continueDepth.TryGetValue(node, out depth);

			public int GetContinueDepth(ControlFlowNode node) => MatchContinue(node, out var depth) ? depth : 0;
		
			/// <summary>
			/// Lists all potential targets for break; statements from a domination tree,
			/// assuming the domination tree must be exited via either break; or continue;
			/// 
			/// First list all nodes in the dominator tree (excluding continue nodes)
			/// Then return the all successors not contained within said tree.
			/// 
			/// Note that node will be returned once for each outgoing edge.
			/// Labelled continue statements (depth > 1) are counted as break targets
			/// </summary>
			internal IEnumerable<ControlFlowNode> GetBreakTargets(ControlFlowNode dominator) => 
				TreeTraversal.PreOrder(dominator, n => n.DominatorTreeChildren.Where(c => !MatchContinue(c)))
					.SelectMany(n => n.Successors)
					.Where(n => !dominator.Dominates(n) && !MatchContinue(n, 1));
		}

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

					// if the original code has an unreachable switch-like condition
					// eg. if (i >= 0) { ... } else if (i == 2) { unreachable }
					// then the 'i == 2' block head gets consumed and the unreachable code needs deleting
					if (context.Settings.RemoveDeadCode)
						container.SortBlocks(deleteUnreachableBlocks: true);
					else
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
				sw.AddILRange(block.Instructions[block.Instructions.Count - 1]);
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

		const ulong MaxValuesPerSection = 100;

		/// <summary>
		/// Tests whether we should prefer a switch statement over an if statement.
		/// </summary>
		private bool UseCSharpSwitch(out KeyValuePair<LongSet, ILInstruction> defaultSection)
		{
			if (!analysis.InnerBlocks.Any()) {
				defaultSection = default;
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
			
			// heuristic to determine if a block would be better represented as an if statement rather than switch
			int ifCount = analysis.InnerBlocks.Count + 1;
			int intervalCount = analysis.Sections.Where(s => !s.Key.SetEquals(defaultSectionKey)).Sum(s => s.Key.Intervals.Length);
			if (ifCount < intervalCount)
				return false;
			
			(var flowNodes, var caseNodes) = AnalyzeControlFlow();

			// don't create switch statements with only one non-default label when the corresponding condition tree is flat
			// it may be important that the switch-like conditions be inlined
			// for example, a loop condition: while (c == '\n' || c == '\r')
			if (analysis.Sections.Count == 2 && IsSingleCondition(flowNodes, caseNodes))
				return false;
			
			// if there is no ILSwitch, there's still many control flow patterns that 
			// match a switch statement but were originally just regular if statements,
			// and converting them to switches results in poor quality code with goto statements
			// 
			// If a single break target cannot be identified, then the equivalent switch statement would require goto statements.
			// These goto statements may be "goto case x" or "goto default", but these are a hint that the original code was not a switch,
			// and that the switch statement may be very poor quality. 
			// Thus the rule of thumb is no goto statements if the original code didn't include them
			if (SwitchUsesGoto(flowNodes, caseNodes, out var breakBlock))
				return false;

			// valid switch construction, all code can be inlined
			if (breakBlock == null)
				return true;

			// The switch has a single break target and there is one more hint
			// The break target cannot be inlined, and should have the highest IL offset of everything targetted by the switch
			return breakBlock.StartILOffset >= analysis.Sections.Select(s => s.Value.MatchBranch(out var b) ? b.StartILOffset : -1).Max();
		}

		/// <summary>
		/// stloc switchValueVar(call ComputeStringHash(switchValue))
		/// </summary>
		private bool MatchRoslynSwitchOnString()
		{
			var insns = analysis.RootBlock.Instructions;
			return insns.Count >= 3 && SwitchOnStringTransform.MatchComputeStringHashCall(insns[insns.Count - 3], analysis.SwitchVariable, out var switchLdLoc);
		}

		/// <summary>
		/// Builds the control flow graph for the current container (if necessary), establishes loopContext
		/// and returns the ControlFlowNodes corresponding to the inner flow and case blocks of the potential switch
		/// </summary>
		private (List<ControlFlowNode> flowNodes, List<ControlFlowNode> caseNodes) AnalyzeControlFlow()
		{
			if (controlFlowGraph == null)
				controlFlowGraph = new ControlFlowGraph(currentContainer, context.CancellationToken);
			
			var switchHead = controlFlowGraph.GetNode(analysis.RootBlock);
			loopContext = new LoopContext(controlFlowGraph, switchHead);

			var flowNodes = new List<ControlFlowNode> { switchHead };
			flowNodes.AddRange(analysis.InnerBlocks.Select(controlFlowGraph.GetNode));

			// grab the control flow nodes for blocks targetted by each section
			var caseNodes = new List<ControlFlowNode>();
			foreach (var s in analysis.Sections) {
				if (!s.Value.MatchBranch(out var block)) 
					continue;

				if (block.Parent == currentContainer) {
					var node = controlFlowGraph.GetNode(block);
					if (!loopContext.MatchContinue(node))
						caseNodes.Add(node);
				}
			}

			AddNullCase(flowNodes, caseNodes);

			Debug.Assert(flowNodes.SelectMany(n => n.Successors)
				.All(n => flowNodes.Contains(n) || caseNodes.Contains(n) || loopContext.MatchContinue(n)));

			return (flowNodes, caseNodes);
		}

		/// <summary>
		/// Determines if the analysed switch can be constructed without any gotos
		/// </summary>
		private bool SwitchUsesGoto(List<ControlFlowNode> flowNodes, List<ControlFlowNode> caseNodes, out Block breakBlock)
		{
			// cases with predecessors that aren't part of the switch logic 
			// must either require "goto case" statements, or consist of a single "break;"
			var externalCases = caseNodes.Where(c => c.Predecessors.Any(n => !flowNodes.Contains(n))).ToList();

			breakBlock = null;
			if (externalCases.Count > 1)
				return true; // cannot have more than one break case without gotos
			
			// check that case nodes flow through a single point
			var breakTargets = caseNodes.Except(externalCases).SelectMany(n => loopContext.GetBreakTargets(n)).ToHashSet();

			// if there are multiple break targets, then gotos are required
			// if there are none, then the external case (if any) can be the break target
			if (breakTargets.Count != 1)
				return breakTargets.Count > 1;
			
			breakBlock = (Block) breakTargets.Single().UserData;

			// external case must consist of a single "break;"
			return externalCases.Count == 1 && breakBlock != externalCases.Single().UserData;
		}

		/// <summary>
		/// Does some of the analysis of SwitchOnNullableTransform to add the null case control flow
		/// to the results of SwitchAnaylsis
		/// </summary>
		private void AddNullCase(List<ControlFlowNode> flowNodes, List<ControlFlowNode> caseNodes)
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
			flowNodes.Add(controlFlowGraph.GetNode(nullableBlock));
		}
		/// <summary>
		/// Pattern matching for short circuit expressions
		///   p
		///   |\
		///   | n
		///   |/ \
		///   s   c
		/// 
		///  where
		///   p: if (a) goto n; goto s;
		///   n: if (b) goto c; goto s;
		/// 
		///  Can simplify to
		///    p|n
		///    / \
		///   s   c
		/// 
		///  where:
		///   p|n: if (a &amp;&amp; b) goto c; goto s;
		/// 
		///  Note that if n has only 1 successor, but is still a flow node, then a short circuit expression 
		///  has a target (c) with no corresponding block (leave)
		/// </summary>
		/// <param name="parent">A node with 2 successors</param>
		/// <param name="side">The successor index to consider n (the other successor will be the common sibling)</param>
		private static bool IsShortCircuit(ControlFlowNode parent, int side)
		{
			var node = parent.Successors[side];
			var sibling = parent.Successors[side ^ 1];

			if (!IsFlowNode(node) || node.Successors.Count > 2 || node.Predecessors.Count != 1)
				return false;

			return node.Successors.Contains(sibling);
		}

		/// <summary>
		/// A flow node contains only two instructions, the first of which is an IfInstruction
		/// A short circuit expression is comprised of a root block ending in an IfInstruction and one or more flow nodes
		/// </summary>
		static bool IsFlowNode(ControlFlowNode n) => ((Block)n.UserData).Instructions.FirstOrDefault() is IfInstruction;

		/// <summary>
		/// Determines whether the flowNodes are can be reduced to a single condition via short circuit operators
		/// </summary>
		private bool IsSingleCondition(List<ControlFlowNode> flowNodes, List<ControlFlowNode> caseNodes)
		{
			if (flowNodes.Count == 1)
				return true;

			var rootNode = controlFlowGraph.GetNode(analysis.RootBlock);
			rootNode.Visited = true;

			// search down the tree, marking nodes as visited while they continue the current condition
			var n = rootNode;
			while (n.Successors.Count > 0 && (n == rootNode || IsFlowNode(n))) {
				if (n.Successors.Count == 1) {
					// if there is more than one case node, then a flow node with only one successor is not part of the initial condition
					if (caseNodes.Count > 1)
						break;
					
					n = n.Successors[0];
				}
				else { // 2 successors
					if (IsShortCircuit(n, 0))
						n = n.Successors[0];
					else if (IsShortCircuit(n, 1))
						n = n.Successors[1];
					else
						break;
				}
				
				n.Visited = true;
				if (loopContext.MatchContinue(n))
					break;
			}

			var ret = flowNodes.All(f => f.Visited);
			ResetVisited(controlFlowGraph.cfg);
			return ret;
		}

		private static void ResetVisited(IEnumerable<ControlFlowNode> nodes)
		{
			foreach (var n in nodes)
				n.Visited = false;
		}
	}
}
