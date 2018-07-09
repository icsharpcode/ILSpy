using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection.Metadata;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows entities that use a type.
	/// </summary>
	[Export(typeof(IAnalyzer<ITypeDefinition>))]
	class TypeUsedByAnalyzer : IMethodBodyAnalyzer<ITypeDefinition>, ITypeDefinitionAnalyzer<ITypeDefinition>
	{
		public string Text => "Used By";

		public IEnumerable<IEntity> Analyze(ITypeDefinition analyzedEntity, IMethod method, MethodBodyBlock methodBody, AnalyzerContext context)
		{
			var typeSystem = context.TypeSystem;
			var visitor = new TypeDefinitionUsedVisitor(analyzedEntity);

			if (!methodBody.LocalSignature.IsNil) {
				foreach (var type in typeSystem.DecodeLocalSignature(methodBody.LocalSignature)) {
					type.AcceptVisitor(visitor);

					if (visitor.Found) {
						yield return method;
						yield break;
					}
				}
			}

			var blob = methodBody.GetILReader();

			while (!visitor.Found && blob.RemainingBytes > 0) {
				var opCode = blob.DecodeOpCode();
				switch (opCode.GetOperandType()) {
					case OperandType.Field:
					case OperandType.Method:
					case OperandType.Sig:
					case OperandType.Tok:
					case OperandType.Type:
						var member = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
						if (member.IsNil) continue;
						switch (member.Kind) {
							case HandleKind.TypeReference:
							case HandleKind.TypeSpecification:
							case HandleKind.TypeDefinition:
								typeSystem.ResolveAsType(member).AcceptVisitor(visitor);
								if (visitor.Found) {
									yield return method;
									yield break;
								}
								break;

							case HandleKind.FieldDefinition:
							case HandleKind.MethodDefinition:
							case HandleKind.MemberReference:
							case HandleKind.MethodSpecification:
								VisitMember(visitor, typeSystem.ResolveAsMember(member));

								if (visitor.Found) {
									yield return method;
									yield break;
								}
								break;

							case HandleKind.StandaloneSignature:
								var signature = typeSystem.DecodeMethodSignature((StandaloneSignatureHandle)member);
								foreach (var type in signature.ParameterTypes) {
									type.AcceptVisitor(visitor);
								}

								signature.ReturnType.AcceptVisitor(visitor);

								if (visitor.Found) {
									yield return method;
									yield break;
								}
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
		}

		void VisitMember(TypeDefinitionUsedVisitor visitor, IMember member)
		{
			switch (member) {
				case IField field:
					field.ReturnType.AcceptVisitor(visitor);
					break;
				case IMethod method:
					foreach (var p in method.Parameters) {
						p.Type.AcceptVisitor(visitor);
					}

					method.ReturnType.AcceptVisitor(visitor);
					break;
				case IProperty property:
					foreach (var p in property.Parameters) {
						p.Type.AcceptVisitor(visitor);
					}

					property.ReturnType.AcceptVisitor(visitor);
					break;

				case IEvent @event:
					@event.ReturnType.AcceptVisitor(visitor);
					break;
			}
		}

		public IEnumerable<IEntity> Analyze(ITypeDefinition analyzedEntity, ITypeDefinition type, AnalyzerContext context)
		{
			var typeSystem = context.TypeSystem;
			var visitor = new TypeDefinitionUsedVisitor(analyzedEntity);

			foreach (var bt in type.DirectBaseTypes) {
				bt.AcceptVisitor(visitor);
			}

			if (visitor.Found)
				yield return type;

			foreach (var member in type.Members) {
				visitor.Found = false;
				VisitMember(visitor, member);
				if (visitor.Found)
					yield return member;
			}
		}

		bool CanBeReference(ILOpCode opCode)
		{
			return opCode == ILOpCode.Newobj || opCode == ILOpCode.Initobj;
		}

		public bool Show(ITypeDefinition entity) => !entity.IsAbstract && !entity.IsStatic;
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
			Found |= typeDefinition.MetadataToken == type.MetadataToken
				&& typeDefinition.ParentAssembly.PEFile == type.ParentAssembly.PEFile;
			return base.VisitTypeDefinition(type);
		}
	}
}
