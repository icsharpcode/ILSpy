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
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
		SwitchAnalysis analysis = new SwitchAnalysis();

		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var container in function.Descendants.OfType<BlockContainer>()) {
				bool blockContainerNeedsCleanup = false;
				foreach (var block in container.Blocks) {
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
			if (analysisSuccess && UseCSharpSwitch(analysis, out defaultSection)) {
				// complex multi-block switch that can be combined into a single SwitchInstruction

				var sw = new SwitchInstruction(new LdLoc(analysis.SwitchVariable));
				foreach (var section in analysis.Sections) {
					if (!section.Key.SetEquals(defaultSection.Key)) {
						sw.Sections.Add(new SwitchSection
						{
							Labels = section.Key,
							Body = section.Value
						});
					}
				}
				block.Instructions[block.Instructions.Count - 2] = sw;
				block.Instructions[block.Instructions.Count - 1] = defaultSection.Value;
				// mark all inner blocks that were converted to the switch statement for deletion
				foreach (var innerBlock in analysis.InnerBlocks) {
					Debug.Assert(innerBlock.Parent == block.Parent);
					Debug.Assert(innerBlock != ((BlockContainer)block.Parent).EntryPoint);
					innerBlock.Instructions.Clear();
				}
				blockContainerNeedsCleanup = true;
			} else {
				// 2nd pass of SimplifySwitchInstruction (after duplicating return blocks),
				// (1st pass was in ControlFlowSimplification)
				SimplifySwitchInstruction(block);
			}
		}

		internal static void SimplifySwitchInstruction(Block block)
		{
			// due to our of of basic blocks at this point,
			// switch instructions can only appear as second-to-last insturction
			var sw = block.Instructions.SecondToLastOrDefault() as SwitchInstruction;
			if (sw == null)
				return;

			// ControlFlowSimplification runs early (before any other control flow transforms).
			// Any switch instructions will only have branch instructions in the sections.

			// Combine sections with identical branch target:
			Block defaultTarget;
			block.Instructions.Last().MatchBranch(out defaultTarget);
			var dict = new Dictionary<Block, SwitchSection>(); // branch target -> switch section
			sw.Sections.RemoveAll(
				section => {
					Block target;
					if (section.Body.MatchBranch(out target)) {
						SwitchSection primarySection;
						if (target == defaultTarget) {
							// This section is just an alternative for 'default'.
							Debug.Assert(sw.DefaultBody is Nop);
							return true; // remove this section
						} else if (dict.TryGetValue(target, out primarySection)) {
							primarySection.Labels = primarySection.Labels.UnionWith(section.Labels);
							return true; // remove this section
						} else {
							dict.Add(target, section);
						}
					}
					return false;
				});
		}

		const ulong MaxValuesPerSection = 50;

		/// <summary>
		/// Tests whether we should prefer a switch statement over an if statement.
		/// </summary>
		static bool UseCSharpSwitch(SwitchAnalysis analysis, out KeyValuePair<LongSet, ILInstruction> defaultSection)
		{
			if (!analysis.InnerBlocks.Any()) {
				defaultSection = default(KeyValuePair<LongSet, ILInstruction>);
				return false;
			}
			defaultSection = analysis.Sections.FirstOrDefault(s => s.Key.Count() > MaxValuesPerSection);
			if (defaultSection.Key.IsEmpty) {
				return false;
			}
			ulong valuePerSectionLimit = MaxValuesPerSection;
			if (!analysis.ContainsILSwitch) {
				// If there's no IL switch involved, limit the number of keys per section
				// much more drastically to avoid generating switches where an if condition
				// would be shorter.
				valuePerSectionLimit = Math.Min(
					valuePerSectionLimit,
					(ulong)analysis.InnerBlocks.Count);
			}
			var defaultSectionKey = defaultSection.Key;
			if (analysis.Sections.Any(s => !s.Key.SetEquals(defaultSectionKey)
										&& s.Key.Count() > valuePerSectionLimit)) {
				return false;
			}
			return analysis.InnerBlocks.Any();
		}
	}
}
