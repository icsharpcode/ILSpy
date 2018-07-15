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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	sealed class AnalyzedTypeInstantiationsTreeNode : AnalyzerSearchTreeNode
	{
		readonly Decompiler.Metadata.PEFile module;
		readonly TypeDefinitionHandle analyzedType;
		readonly FullTypeName analyzedTypeName;

		public AnalyzedTypeInstantiationsTreeNode(Decompiler.Metadata.PEFile module, TypeDefinitionHandle analyzedType)
		{
			if (analyzedType.IsNil)
				throw new ArgumentNullException(nameof(analyzedType));

			this.module = module;
			this.analyzedType = analyzedType;
			this.analyzedTypeName = analyzedType.GetFullTypeName(module.Metadata);
		}

		public override object Text => "Instantiated By";

		protected override IEnumerable<AnalyzerTreeNode> FetchChildren(CancellationToken ct)
		{
			var analyzer = new ScopedWhereUsedAnalyzer<AnalyzerTreeNode>(this.Language, module, analyzedType, provideTypeSystem: false, FindReferencesInType);
			return analyzer.PerformAnalysis(ct).OrderBy(n => n.Text);
		}

		IEnumerable<AnalyzerTreeNode> FindReferencesInType(Decompiler.Metadata.PEFile module, TypeDefinitionHandle type, CodeMappingInfo codeMapping, IDecompilerTypeSystem typeSystem)
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
					switch (opCode) {

						case ILOpCode.Newobj:
							var member = MetadataTokens.EntityHandle(blob.ReadInt32());
							switch (member.Kind) {
								case HandleKind.MethodDefinition:
									// check whether we're looking at the defining assembly:
									if (module != this.module)
										break;
									var md = module.Metadata.GetMethodDefinition((MethodDefinitionHandle)member);
									if (!module.Metadata.StringComparer.Equals(md.Name, ".ctor"))
										break;
									found = md.GetDeclaringType() == analyzedType;
									break;
								case HandleKind.MemberReference:
									var mr = module.Metadata.GetMemberReference((MemberReferenceHandle)member);
									// safety-check: should always be a method
									if (mr.GetKind() != MemberReferenceKind.Method)
										break;
									if (!module.Metadata.StringComparer.Equals(mr.Name, ".ctor"))
										break;
									switch (mr.Parent.Kind) {
										case HandleKind.MethodDefinition: // varargs method
											var parentMD = module.Metadata.GetMethodDefinition((MethodDefinitionHandle)mr.Parent);
											found = parentMD.GetDeclaringType() == analyzedType;
											break;
										case HandleKind.ModuleReference: // global function
											throw new NotSupportedException();
										default:
											var typeName = mr.Parent.GetFullTypeName(module.Metadata);
											found = typeName == analyzedTypeName;
											break;
									}
									break;
								case HandleKind.MethodSpecification: // do we need to handle these?
									throw new NotSupportedException();
								default:
									throw new ArgumentOutOfRangeException();
							}
							break;

						case ILOpCode.Initobj:
							var referencedType = MetadataTokens.EntityHandle(blob.ReadInt32());
							switch (referencedType.Kind) {
								case HandleKind.TypeDefinition:
									// check whether we're looking at the defining assembly:
									if (module != this.module)
										break;
									found = referencedType == analyzedType;
									break;
								case HandleKind.TypeReference:
								case HandleKind.TypeSpecification:
									var referencedTypeName = referencedType.GetFullTypeName(module.Metadata);
									found = referencedTypeName == analyzedTypeName;
									break;
								default:
									throw new ArgumentOutOfRangeException();
							}
							break;

						default:
							blob.SkipOperand(opCode);
							break;
					}
				}

				if (found) {
					var node = new AnalyzedMethodTreeNode(module, h);
					node.Language = this.Language;
					yield return node;
				}
			}
		}

		public static bool CanShow(MetadataReader metadata, TypeDefinitionHandle handle)
		{
			var td = metadata.GetTypeDefinition(handle);
			return (td.Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Class
				&& !((td.Attributes & TypeAttributes.Abstract) != 0 && (td.Attributes & TypeAttributes.Sealed) != 0)
				&& !handle.IsEnum(metadata);
		}
	}
}
