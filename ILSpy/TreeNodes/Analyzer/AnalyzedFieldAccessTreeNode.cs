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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.TypeSystem;
using ILOpCode = System.Reflection.Metadata.ILOpCode;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	sealed class AnalyzedFieldAccessTreeNode : AnalyzerSearchTreeNode
	{
		readonly bool showWrites; // true: show writes; false: show read access
		readonly Decompiler.Metadata.PEFile module;
		readonly FieldDefinitionHandle analyzedField;
		readonly FullTypeName analyzedTypeName;
		readonly string analyzedFieldName;
		Lazy<HashSet<Decompiler.Metadata.MethodDefinition>> foundMethods;
		readonly object hashLock = new object();

		public AnalyzedFieldAccessTreeNode(Decompiler.Metadata.PEFile module, FieldDefinitionHandle analyzedField, bool showWrites)
		{
			if (analyzedField.IsNil)
				throw new ArgumentNullException(nameof(analyzedField));

			this.module = module;
			this.analyzedField = analyzedField;
			var fd = module.Metadata.GetFieldDefinition(analyzedField);
			this.analyzedTypeName = fd.GetDeclaringType().GetFullTypeName(module.Metadata);
			this.analyzedFieldName = module.Metadata.GetString(fd.Name);
			this.showWrites = showWrites;
		}

		public override object Text => showWrites ? "Assigned By" : "Read By";

		protected override IEnumerable<AnalyzerTreeNode> FetchChildren(CancellationToken ct)
		{
			foundMethods = new Lazy<HashSet<Decompiler.Metadata.MethodDefinition>>(LazyThreadSafetyMode.ExecutionAndPublication);

			var analyzer = new ScopedWhereUsedAnalyzer<AnalyzerTreeNode>(module, analyzedField, provideTypeSystem: false, FindReferencesInType);
			foreach (var child in analyzer.PerformAnalysis(ct).OrderBy(n => n.Text)) {
				yield return child;
			}

			foundMethods = null;
		}

		private IEnumerable<AnalyzerTreeNode> FindReferencesInType(Decompiler.Metadata.PEFile module, TypeDefinitionHandle type, IDecompilerTypeSystem typeSystem)
		{
			var td = module.Metadata.GetTypeDefinition(type);
			foreach (var h in td.GetMethods()) {
				bool found = false;

				var method = module.Metadata.GetMethodDefinition(h);
				if (!method.HasBody())
					continue;

				var blob = module.Reader.GetMethodBody(method.RelativeVirtualAddress).GetILReader();
				while (!found && blob.RemainingBytes > 0) {
					var opCode = blob.DecodeOpCode();
					if (!CanBeReference(opCode)) {
						blob.SkipOperand(opCode);
						continue;
					}
					var member = MetadataTokens.EntityHandle(blob.ReadInt32());
					switch (member.Kind) {
						case HandleKind.FieldDefinition:
							// check whether we're looking at the defining assembly
							found = member == analyzedField && module == this.module;
							break;
						case HandleKind.MemberReference:
							var mr = module.Metadata.GetMemberReference((MemberReferenceHandle)member);
							// safety-check: should always be a field
							if (mr.GetKind() != MemberReferenceKind.Field)
								break;
							if (!module.Metadata.StringComparer.Equals(mr.Name, analyzedFieldName))
								break;
							var typeName = mr.Parent.GetFullTypeName(module.Metadata);
							found = typeName == analyzedTypeName;
							break;
					}
				}

				if (found) {
					var md = new Decompiler.Metadata.MethodDefinition(module, h);
					if (IsNewResult(md)) {
						var node = new AnalyzedMethodTreeNode(module, h);
						node.Language = this.Language;
						yield return node;
					}
				}
			}
		}

		bool CanBeReference(ILOpCode code)
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

		bool IsNewResult(Decompiler.Metadata.MethodDefinition method)
		{
			var hashSet = foundMethods.Value;
			lock (hashLock) {
				return hashSet.Add(method);
			}
		}
	}
}
