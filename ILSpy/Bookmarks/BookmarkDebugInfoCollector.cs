// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.Bookmarks
{
	/// <summary>
	/// Harvests the IL-offset/text-line map for body bookmarks during the single C# formatting pass.
	/// As the output visitor writes each AST node, the node still carries the
	/// <see cref="ILInstruction"/>(s) it was built from; pairing an instruction's IL offset with the
	/// document line the node lands on yields the same line/offset map a body anchor needs.
	///
	/// Because the line numbers are read from the very output being displayed, the map needs no
	/// second formatting pass and no separate sequence-point generation, and there is no cross-pass
	/// "the two writers must break lines identically" assumption to hold.
	/// </summary>
	sealed class BookmarkDebugInfoCollector : DecoratingTokenWriter
	{
		readonly AvaloniaEditTextOutput output;
		// Per top-level function: the lowest IL offset of any instruction whose node starts on a given
		// document line. Lowest-on-the-line matches MethodDebugInfo.TryGetOffsetForLine's contract.
		readonly Dictionary<ILFunction, Dictionary<int, int>> offsetByLinePerFunction = new();

		public BookmarkDebugInfoCollector(TokenWriter decoratedWriter, AvaloniaEditTextOutput output)
			: base(decoratedWriter)
		{
			this.output = output;
		}

		public override void StartNode(AstNode node)
		{
			base.StartNode(node);
			// CurrentLine is the line this node's first token will land on: the preceding newline has
			// already advanced it, and indentation/tokens for this node are only written afterwards.
			int line = output.CurrentLine;
			foreach (var inst in node.Annotations.OfType<ILInstruction>())
			{
				if (!HasUsableILRange(inst))
					continue;
				// The instruction's nearest enclosing function owns the IL offset. Only a top-level
				// function has a metadata token that resolves to a navigable member; an offset inside a
				// lambda/local function is skipped, so a click there falls back to a member (token) anchor.
				var function = inst.Parent!.Ancestors.OfType<ILFunction>().FirstOrDefault();
				if (function is not { Kind: ILFunctionKind.TopLevelFunction, Method: not null })
					continue;
				if (!offsetByLinePerFunction.TryGetValue(function, out var offsetByLine))
					offsetByLinePerFunction[function] = offsetByLine = new Dictionary<int, int>();
				int offset = inst.StartILOffset;
				if (!offsetByLine.TryGetValue(line, out int existing) || offset < existing)
					offsetByLine[line] = offset;
			}
		}

		// Mirrors SequencePointBuilder.HasUsableILRange: an instruction contributes a position only when
		// it has a non-empty IL range, is connected to the tree, and is not a whole-body (block) node.
		static bool HasUsableILRange(ILInstruction inst)
			=> !inst.ILRangeIsEmpty && inst.Parent != null && inst is not (BlockContainer or Block);

		/// <summary>
		/// Registers a <see cref="MethodDebugInfo"/> for each captured function with the output. Call
		/// once, after the formatting pass has visited the whole syntax tree.
		/// </summary>
		public void Publish()
		{
			foreach (var (function, offsetByLine) in offsetByLinePerFunction)
			{
				var method = function.Method!;
				var file = method.ParentModule?.MetadataFile;
				if (file == null)
					continue;
				var lines = offsetByLine.Select(entry => (Line: entry.Key, ILOffset: entry.Value)).ToList();
				uint token = (uint)MetadataTokens.GetToken(method.MetadataToken);
				string moduleName = string.IsNullOrEmpty(file.FileName) ? file.Name : Path.GetFileName(file.FileName);
				output.AddMethodDebugInfo(new MethodDebugInfo(
					token, file.FileName, file.FullName, moduleName, method.FullName, lines));
			}
		}
	}
}
