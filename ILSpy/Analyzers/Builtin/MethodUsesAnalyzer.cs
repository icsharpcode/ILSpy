// Copyright (c) 2018 Siegfried Pammer
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
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows entities that are used by a method.
	/// </summary>
	[ExportAnalyzer(Header = "Uses", Order = 10)]
	class MethodUsesAnalyzer : IAnalyzer
	{
		public bool Show(ISymbol symbol) => symbol is IMethod method && method.HasBody;

		public IEnumerable<ISymbol> Analyze(ISymbol symbol, AnalyzerContext context)
		{
			if (symbol is IMethod method) {
				var typeSystem = context.GetOrCreateTypeSystem(method.ParentModule.PEFile);
				return context.Language.GetCodeMappingInfo(method.ParentModule.PEFile, method.MetadataToken)
					.GetMethodParts((MethodDefinitionHandle)method.MetadataToken)
					.SelectMany(h => ScanMethod(h, typeSystem)).Distinct();
			}
			throw new InvalidOperationException("Should never happen.");
		}

		IEnumerable<IEntity> ScanMethod(MethodDefinitionHandle handle, DecompilerTypeSystem typeSystem)
		{
			var module = typeSystem.MainModule;
			var md = module.PEFile.Metadata.GetMethodDefinition(handle);
			if (!md.HasBody()) yield break;

			BlobReader blob;
			try {
				blob = module.PEFile.Reader.GetMethodBody(md.RelativeVirtualAddress).GetILReader();
			} catch (BadImageFormatException) {
				yield break;
			}
			var visitor = new TypeDefinitionCollector();
			var genericContext = new Decompiler.TypeSystem.GenericContext(); // type parameters don't matter for this analyzer

			while (blob.RemainingBytes > 0) {
				ILOpCode opCode;
				try {
					opCode = blob.DecodeOpCode();
				} catch (BadImageFormatException) {
					yield break;
				}
				switch (opCode.GetOperandType()) {
					case OperandType.Field:
					case OperandType.Method:
					case OperandType.Sig:
					case OperandType.Tok:
						var member = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
						if (member.IsNil) continue;

						switch (member.Kind) {
							case HandleKind.StandaloneSignature:
								break;
							case HandleKind.TypeDefinition:
							case HandleKind.TypeReference:
							case HandleKind.TypeSpecification:
								IType ty;
								try {
									ty = module.ResolveType(member, genericContext);
								} catch (BadImageFormatException) {
									ty = null;
								}
								ty?.AcceptVisitor(visitor);
								break;
							case HandleKind.MethodDefinition:
							case HandleKind.MethodSpecification:
							case HandleKind.MemberReference:
							case HandleKind.FieldDefinition:
								IEntity m;
								try {
									m = module.ResolveEntity(member, genericContext);
								} catch (BadImageFormatException) {
									m = null;
								}
								if (m != null)
									yield return m;
								break;
						}
						break;
					default:
						try {
							ILParser.SkipOperand(ref blob, opCode);
						} catch (BadImageFormatException) {
							yield break;
						}
						break;
				}
			}

			foreach (var type in visitor.UsedTypes) {
				yield return type;
			}
		}

		class TypeDefinitionCollector : TypeVisitor
		{
			public readonly List<ITypeDefinition> UsedTypes = new List<ITypeDefinition>(); 

			public override IType VisitTypeDefinition(ITypeDefinition type)
			{
				UsedTypes.Add(type);
				return base.VisitTypeDefinition(type);
			}
		}
	}
}
