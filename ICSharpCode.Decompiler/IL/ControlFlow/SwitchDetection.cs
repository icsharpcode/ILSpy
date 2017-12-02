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
using ICSharpCode.Decompiler.Util;

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
			if (analysisSuccess && UseCSharpSwitch(analysis, out defaultSection)) {
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
		static bool UseCSharpSwitch(SwitchAnalysis analysis, out KeyValuePair<LongSet, ILInstruction> defaultSection)
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
				// Only the default section is allowed to have tons of keys.
				// C# doesn't support "case 1 to 100000000", and we don't want to generate
				// gigabytes of case labels.
				return false;
			}
			return true;
		}
	}
}
