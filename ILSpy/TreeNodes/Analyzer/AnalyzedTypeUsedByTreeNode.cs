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
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	internal sealed class AnalyzedTypeUsedByTreeNode : AnalyzerSearchTreeNode
	{
		readonly Decompiler.Metadata.PEFile module;
		readonly TypeDefinitionHandle analyzedType;
		readonly FullTypeName analyzedTypeName;

		public AnalyzedTypeUsedByTreeNode(Decompiler.Metadata.PEFile module, TypeDefinitionHandle analyzedType)
		{
			if (analyzedType.IsNil)
				throw new ArgumentNullException(nameof(analyzedType));

			this.module = module;
			this.analyzedType = analyzedType;
			this.analyzedTypeName = analyzedType.GetFullTypeName(module.Metadata);
		}

		public override object Text => "Used By";

		protected override IEnumerable<AnalyzerTreeNode> FetchChildren(CancellationToken ct)
		{
			var analyzer = new ScopedWhereUsedAnalyzer<AnalyzerEntityTreeNode>(this.Language, module, analyzedType, provideTypeSystem: true, FindTypeUsage);
			return analyzer.PerformAnalysis(ct).Distinct(AnalyzerEntityTreeNodeComparer.Instance).OrderBy(n => n.Text);
		}

		IEnumerable<AnalyzerEntityTreeNode> FindTypeUsage(Decompiler.Metadata.PEFile module, TypeDefinitionHandle type, CodeMappingInfo codeMapping, IDecompilerTypeSystem typeSystem)
		{
			if (type == this.analyzedType && module == this.module)
				yield break;

			// TODO : cache / optimize this per assembly
			var analyzedTypeDefinition = typeSystem.Compilation.FindType(analyzedTypeName).GetDefinition();
			var typeDefinition = typeSystem.ResolveAsType(type).GetDefinition();

			if (analyzedTypeDefinition == null || typeDefinition == null)
				yield break;

			var visitor = new TypeDefinitionUsedVisitor(analyzedTypeDefinition);

			if (typeDefinition.DirectBaseTypes.Any(bt => analyzedTypeDefinition.Equals(bt.GetDefinition())))
				yield return new AnalyzedTypeTreeNode(new Decompiler.Metadata.TypeDefinition(module, type)) { Language = Language };

			foreach (var field in typeDefinition.Fields.Where(f => IsUsedInField(f, visitor)))
				yield return new AnalyzedFieldTreeNode(module, (FieldDefinitionHandle)field.MetadataToken) { Language = Language };

			foreach (var method in typeDefinition.Methods.Where(m => IsUsedInMethodDefinition(m, visitor, typeSystem, module)))
				yield return new AnalyzedMethodTreeNode(module, (MethodDefinitionHandle)method.MetadataToken) { Language = Language };

			foreach (var property in typeDefinition.Properties.Where(p => IsUsedInProperty(p, visitor, typeSystem, module)))
				yield return new AnalyzedPropertyTreeNode(module, (PropertyDefinitionHandle)property.MetadataToken) { Language = Language };
		}

		bool IsUsedInField(IField field, TypeDefinitionUsedVisitor visitor)
		{
			visitor.Found = false;
			field.ReturnType.AcceptVisitor(visitor);
			return visitor.Found;
		}

		bool IsUsedInProperty(IProperty property, TypeDefinitionUsedVisitor visitor, IDecompilerTypeSystem typeSystem, PEFile module)
		{
			visitor.Found = false;
			property.ReturnType.AcceptVisitor(visitor);
			for (int i = 0; i < property.Parameters.Count && !visitor.Found; i++)
				property.Parameters[i].Type.AcceptVisitor(visitor);
			return visitor.Found
				|| (property.CanGet && IsUsedInMethodBody(module, visitor, typeSystem, (MethodDefinitionHandle)property.Getter.MetadataToken))
				|| (property.CanSet && IsUsedInMethodBody(module, visitor, typeSystem, (MethodDefinitionHandle)property.Setter.MetadataToken));
		}

		bool IsUsedInMethodDefinition(IMethod method, TypeDefinitionUsedVisitor visitor, IDecompilerTypeSystem typeSystem, PEFile module)
		{
			visitor.Found = false;
			method.ReturnType.AcceptVisitor(visitor);
			for (int i = 0; i < method.Parameters.Count && !visitor.Found; i++)
				method.Parameters[i].Type.AcceptVisitor(visitor);
			return visitor.Found || IsUsedInMethodBody(module, visitor, typeSystem, (MethodDefinitionHandle)method.MetadataToken);
		}

		bool IsUsedInMethod(IMethod method, TypeDefinitionUsedVisitor visitor)
		{
			visitor.Found = false;
			method.ReturnType.AcceptVisitor(visitor);
			for (int i = 0; i < method.Parameters.Count && !visitor.Found; i++)
				method.Parameters[i].Type.AcceptVisitor(visitor);
			return visitor.Found;
		}

		bool IsUsedInMethodBody(PEFile module, TypeDefinitionUsedVisitor visitor, IDecompilerTypeSystem typeSystem, MethodDefinitionHandle method)
		{
			if (method.IsNil)
				return false;
			var md = module.Metadata.GetMethodDefinition(method);
			if (!md.HasBody())
				return false;

			var blob = module.Reader.GetMethodBody(md.RelativeVirtualAddress).GetILReader();
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
							case HandleKind.TypeReference:
							case HandleKind.TypeSpecification:
								var resolvedType = typeSystem.ResolveAsType(member);
								resolvedType.AcceptVisitor(visitor);
								if (visitor.Found)
									return true;
								break;

							case HandleKind.TypeDefinition:
								if (this.module != module)
									break;
								if (member == analyzedType)
									return true;
								break;

							case HandleKind.FieldDefinition:
								if (this.module != module)
									break;
								var resolvedField = typeSystem.ResolveAsField(member);
								if (IsUsedInField(resolvedField, visitor))
									return true;
								break;

							case HandleKind.MethodDefinition:
								var resolvedMethod = typeSystem.ResolveAsMethod(member);
								if (resolvedMethod == null)
									break;
								if (IsUsedInMethod(resolvedMethod, visitor))
									return true;
								break;

							case HandleKind.MemberReference:
								var resolvedMember = typeSystem.ResolveAsMember(member);
								if (resolvedMember == null)
									break;
								if (resolvedMember is IField f && IsUsedInField(f, visitor))
									return true;
								if (resolvedMember is IMethod m && IsUsedInMethod(m, visitor))
									return true;
								break;

							case HandleKind.MethodSpecification:
								resolvedMethod = typeSystem.ResolveAsMethod(member);
								if (resolvedMethod == null)
									break;
								if (IsUsedInMethod(resolvedMethod, visitor))
									return true;
								break;

							default:
								break;
						}
						break;
					default:
						blob.SkipOperand(opCode);
						break;
				}
			}

			return false;
		}

		public static bool CanShow(Decompiler.Metadata.PEFile module, TypeDefinitionHandle type)
		{
			return !type.IsNil;
		}
	}

	class AnalyzerEntityTreeNodeComparer : IEqualityComparer<AnalyzerEntityTreeNode>
	{
		public static readonly AnalyzerEntityTreeNodeComparer Instance = new AnalyzerEntityTreeNodeComparer();

		public bool Equals(AnalyzerEntityTreeNode x, AnalyzerEntityTreeNode y)
		{
			return x.Member == y.Member;
		}

		public int GetHashCode(AnalyzerEntityTreeNode obj)
		{
			return obj.Member.GetHashCode();
		}
	}

	class TypeDefinitionUsedVisitor : TypeVisitor
	{
		readonly ITypeDefinition typeDefinition;

		public bool Found { get; set; }

		public TypeDefinitionUsedVisitor(ITypeDefinition definition)
		{
			this.typeDefinition = definition;
		}

		public override IType VisitTypeDefinition(ITypeDefinition type)
		{
			Found |= typeDefinition.Equals(type);
			return base.VisitTypeDefinition(type);
		}
	}
}
