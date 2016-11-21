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
				bool needBlockCleanup = false;
				foreach (var block in container.Blocks) {
					if (analysis.AnalyzeBlock(block) && UseCSharpSwitch(analysis)) {
						var hugeSection = analysis.Sections.Single(s => s.Key.Count() > 50);

						var sw = new SwitchInstruction(new LdLoc(analysis.SwitchVariable));
						foreach (var section in analysis.Sections) {
							if (!section.Key.SetEquals(hugeSection.Key)) {
								sw.Sections.Add(new SwitchSection
								{
									Labels = section.Key,
									Body = section.Value
								});
							}
						}
						block.Instructions[block.Instructions.Count - 2] = sw;
						block.Instructions[block.Instructions.Count - 1] = hugeSection.Value;
						// mark all inner blocks that were converted to the switch statement for deletion
						foreach (var innerBlock in analysis.InnerBlocks) {
							Debug.Assert(innerBlock.Parent == container);
							Debug.Assert(innerBlock != container.EntryPoint);
							innerBlock.Instructions.Clear();
						}
						needBlockCleanup = true;
					}
				}
				if (needBlockCleanup) {
					Debug.Assert(container.Blocks.All(b => b.Instructions.Count != 0 || b.IncomingEdgeCount == 0));
					container.Blocks.RemoveAll(b => b.Instructions.Count == 0);
				}
			}
		}
		
		const ulong MaxValuesPerSection = 50;

		/// <summary>
		/// Tests whether we should prefer a switch statement over an if statement.
		/// </summary>
		static bool UseCSharpSwitch(SwitchAnalysis analysis)
		{
			return analysis.InnerBlocks.Any()
				&& analysis.Sections.Count(s => s.Key.Count() > MaxValuesPerSection) == 1;
		}
	}
}
