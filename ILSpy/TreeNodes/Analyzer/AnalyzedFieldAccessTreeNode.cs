// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Dom;

using ILOpCode = System.Reflection.Metadata.ILOpCode;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	internal sealed class AnalyzedFieldAccessTreeNode : AnalyzerSearchTreeNode
	{
		private readonly bool showWrites; // true: show writes; false: show read access
		private readonly FieldDefinition analyzedField;
		private Lazy<Hashtable> foundMethods;
		private readonly object hashLock = new object();

		public AnalyzedFieldAccessTreeNode(FieldDefinition analyzedField, bool showWrites)
		{
			if (analyzedField.IsNil)
				throw new ArgumentNullException(nameof(analyzedField));

			this.analyzedField = analyzedField;
			this.showWrites = showWrites;
		}

		public override object Text
		{
			get { return showWrites ? "Assigned By" : "Read By"; }
		}

		protected override IEnumerable<AnalyzerTreeNode> FetchChildren(CancellationToken ct)
		{
			foundMethods = new Lazy<Hashtable>(LazyThreadSafetyMode.ExecutionAndPublication);

			var analyzer = new ScopedWhereUsedAnalyzer<AnalyzerTreeNode>(analyzedField, FindReferencesInType);
			foreach (var child in analyzer.PerformAnalysis(ct).OrderBy(n => n.Text)) {
				yield return child;
			}

			foundMethods = null;
		}

		private IEnumerable<AnalyzerTreeNode> FindReferencesInType(TypeDefinition type)
		{
			string name = analyzedField.Name;

			foreach (MethodDefinition method in type.Methods) {
				bool found = false;
				if (!method.HasBody)
					continue;
				var blob = method.Body.GetILReader();
				while (blob.RemainingBytes > 0) {
					var opCode = ILParser.DecodeOpCode(ref blob);
					if (!CanBeReference(opCode)) {
						ILParser.SkipOperand(ref blob, opCode);
						continue;
					}
					var field = ILParser.DecodeMemberToken(ref blob, method.Module);
					if (field == null || field.Name != name)
						continue;
					var definition = field.GetDefinition() as FieldDefinition?;
					if (definition?.DeclaringType.FullName != analyzedField.DeclaringType.FullName)
						continue;
					found = true;
					break;
				}

				if (found) {
					MethodDefinition? codeLocation = this.Language.GetOriginalCodeLocation(method) as MethodDefinition?;
					if (codeLocation != null && !HasAlreadyBeenFound(codeLocation.Value)) {
						var node = new AnalyzedMethodTreeNode(codeLocation.Value);
						node.Language = this.Language;
						yield return node;
					}
				}
			}
		}

		private bool CanBeReference(ILOpCode code)
		{
			switch (code) {
				case ILOpCode.Ldfld:
				case ILOpCode.Ldsfld:
					return !showWrites;
				case ILOpCode.Stfld:
				case ILOpCode.Stsfld:
					return showWrites;
				case ILOpCode.Ldflda:
				case ILOpCode.Ldsflda:
					return true; // always show address-loading
				default:
					return false;
			}
		}

		private bool HasAlreadyBeenFound(MethodDefinition method)
		{
			Hashtable hashtable = foundMethods.Value;
			lock (hashLock) {
				if (hashtable.Contains(method)) {
					return true;
				} else {
					hashtable.Add(method, null);
					return false;
				}
			}
		}
	}
}
