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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	/// <summary>
	/// Finds members where this method is referenced.
	/// </summary>
	internal sealed class AnalyzedMethodUsedByTreeNode : AnalyzerSearchTreeNode
	{
		readonly Decompiler.Metadata.PEFile module;
		readonly MethodDefinitionHandle analyzedMethod;

		public AnalyzedMethodUsedByTreeNode(Decompiler.Metadata.PEFile module, MethodDefinitionHandle analyzedMethod)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			if (analyzedMethod.IsNil)
				throw new ArgumentNullException(nameof(analyzedMethod));
			this.analyzedMethod = analyzedMethod;
		}

		public override object Text => "Used By";

		protected override IEnumerable<AnalyzerTreeNode> FetchChildren(CancellationToken ct)
		{
			var analyzer = new ScopedWhereUsedAnalyzer<AnalyzerTreeNode>(this.Language, module, analyzedMethod, provideTypeSystem: false, FindReferencesInType);
			foreach (var child in analyzer.PerformAnalysis(ct).OrderBy(n => n.Text)) {
				yield return child;
			}
		}

		IEnumerable<AnalyzerTreeNode> FindReferencesInType(Decompiler.Metadata.PEFile module, TypeDefinitionHandle type, CodeMappingInfo codeMapping, IDecompilerTypeSystem typeSystem)
		{
			var methodDef = this.module.Metadata.GetMethodDefinition(analyzedMethod);
			var name = this.module.Metadata.GetString(methodDef.Name);

			var td = module.Metadata.GetTypeDefinition(type);

			HashSet<MethodDefinitionHandle> alreadyFoundMethods = new HashSet<MethodDefinitionHandle>();

			foreach (var method in td.GetMethods()) {
				var currentMethod = module.Metadata.GetMethodDefinition(method);
				if (!currentMethod.HasBody())
					continue;
				if (ScanMethodBody(module, currentMethod.RelativeVirtualAddress)) {
					var parentMethod = codeMapping.GetParentMethod(method);
					if (!parentMethod.IsNil && alreadyFoundMethods.Add(parentMethod)) {
						var node = new AnalyzedMethodTreeNode(module, parentMethod);
						node.Language = this.Language;
						yield return node;
					}
				}
			}
		}

		bool ScanMethodBody(PEFile module, int rva)
		{
			var blob = module.Reader.GetMethodBody(rva).GetILReader();
			while (blob.RemainingBytes > 0) {
				var opCode = blob.DecodeOpCode();
				switch (opCode.GetOperandType()) {
					case OperandType.Field:
					case OperandType.Method:
					case OperandType.Sig:
					case OperandType.Tok:
					case OperandType.Type:
						var member = MetadataTokens.EntityHandle(blob.ReadInt32());
						switch (member.Kind) {
							case HandleKind.MethodDefinition:
								if (this.module != module || analyzedMethod != (MethodDefinitionHandle)member)
									break;
								return true;
							case HandleKind.MemberReference:
								var mr = module.Metadata.GetMemberReference((MemberReferenceHandle)member);
								if (mr.GetKind() != MemberReferenceKind.Method)
									break;
								return Helpers.IsSameMethod(analyzedMethod, this.module, member, module);
							case HandleKind.MethodSpecification:
								return Helpers.IsSameMethod(analyzedMethod, this.module, member, module);
						}
						break;
					default:
						blob.SkipOperand(opCode);
						break;
				}
			}

			return false;
		}
	}
}
