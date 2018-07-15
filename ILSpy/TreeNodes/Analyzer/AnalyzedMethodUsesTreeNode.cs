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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	/// <summary>
	/// Shows the methods that are used by this method.
	/// </summary>
	internal sealed class AnalyzedMethodUsesTreeNode : AnalyzerSearchTreeNode
	{
		readonly Decompiler.Metadata.PEFile module;
		readonly MethodDefinitionHandle analyzedMethod;

		public AnalyzedMethodUsesTreeNode(Decompiler.Metadata.PEFile module, MethodDefinitionHandle analyzedMethod)
		{
			if (analyzedMethod.IsNil)
				throw new ArgumentNullException(nameof(analyzedMethod));

			this.module = module;
			this.analyzedMethod = analyzedMethod;
		}

		public override object Text => "Uses";

		protected override IEnumerable<AnalyzerTreeNode> FetchChildren(CancellationToken ct)
		{
			var mapping = Language.GetCodeMappingInfo(module, analyzedMethod);
			foreach (var part in mapping.GetMethodParts(analyzedMethod)) {
				foreach (var node in ScanMethod(part)) {
					node.Language = this.Language;
					yield return node;
				}
			}

			IEnumerable<AnalyzerTreeNode> ScanMethod(MethodDefinitionHandle handle)
			{
				var md = module.Metadata.GetMethodDefinition(handle);
				if (!md.HasBody()) yield break;

				var blob = module.Reader.GetMethodBody(md.RelativeVirtualAddress).GetILReader();
				var resolveContext = new SimpleMetadataResolveContext(module);

				while (blob.RemainingBytes > 0) {
					var opCode = ILParser.DecodeOpCode(ref blob);
					Decompiler.Metadata.MethodDefinition method;
					switch (opCode.GetOperandType()) {
						case OperandType.Field:
						case OperandType.Method:
						case OperandType.Sig:
						case OperandType.Tok:
							var member = MetadataTokens.EntityHandle(blob.ReadInt32());
							switch (member.Kind) {
								case HandleKind.FieldDefinition:
									if (!Language.ShowMember(new Decompiler.Metadata.TypeDefinition(module, member.GetDeclaringType(module.Metadata))) || !Language.ShowMember(new Decompiler.Metadata.FieldDefinition(module, (FieldDefinitionHandle)member))) break;
									yield return new AnalyzedFieldTreeNode(module, (FieldDefinitionHandle)member);
									break;
								case HandleKind.MethodDefinition:
									if (!Language.ShowMember(new Decompiler.Metadata.TypeDefinition(module, member.GetDeclaringType(module.Metadata))) || !Language.ShowMember(new Decompiler.Metadata.MethodDefinition(module, (MethodDefinitionHandle)member))) break;
									yield return new AnalyzedMethodTreeNode(module, (MethodDefinitionHandle)member);
									break;
								case HandleKind.MemberReference:
									var mr = module.Metadata.GetMemberReference((MemberReferenceHandle)member);
									switch (mr.GetKind()) {
										case MemberReferenceKind.Method:
											method = MetadataResolver.ResolveAsMethod(member, resolveContext);
											if (method.IsNil || !Language.ShowMember(new Decompiler.Metadata.TypeDefinition(method.Module, method.Handle.GetDeclaringType(method.Module.Metadata))) || !Language.ShowMember(method)) break;
											yield return new AnalyzedMethodTreeNode(method.Module, method.Handle);
											break;
										case MemberReferenceKind.Field:
											var field = MetadataResolver.ResolveAsField(member, resolveContext);
											if (field.IsNil || !Language.ShowMember(new Decompiler.Metadata.TypeDefinition(field.Module, field.Handle.GetDeclaringType(field.Module.Metadata))) || !Language.ShowMember(field)) break;
											yield return new AnalyzedFieldTreeNode(field.Module, field.Handle);
											break;
										default:
											throw new ArgumentOutOfRangeException();
									}
									break;
								case HandleKind.MethodSpecification:
									method = MetadataResolver.ResolveAsMethod(member, resolveContext);
									if (method.IsNil || !Language.ShowMember(new Decompiler.Metadata.TypeDefinition(method.Module, method.Handle.GetDeclaringType(method.Module.Metadata))) || !Language.ShowMember(method)) break;
									yield return new AnalyzedMethodTreeNode(method.Module, method.Handle);
									break;
							}
							break;
						default:
							ILParser.SkipOperand(ref blob, opCode);
							break;
					}
				}
			}
		}
	}
}
