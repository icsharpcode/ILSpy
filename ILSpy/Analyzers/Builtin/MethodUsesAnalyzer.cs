using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows entities that are used by a method.
	/// </summary>
	[Export(typeof(IAnalyzer<IMethod>))]
	class MethodUsesAnalyzer : IEntityAnalyzer<IMethod>
	{
		public string Text => "Uses";

		public bool Show(IMethod entity) => true;

		public IEnumerable<IEntity> Analyze(IMethod analyzedMethod, AnalyzerContext context)
		{
			return context.CodeMappingInfo.GetMethodParts((MethodDefinitionHandle)analyzedMethod.MetadataToken)
				.SelectMany(h => ScanMethod(analyzedMethod, h, context)).Distinct();
		}

		IEnumerable<IEntity> ScanMethod(IMethod analyzedMethod, MethodDefinitionHandle handle, AnalyzerContext context)
		{
			var module = analyzedMethod.ParentAssembly.PEFile;
			var md = module.Metadata.GetMethodDefinition(handle);
			if (!md.HasBody()) yield break;

			var blob = module.Reader.GetMethodBody(md.RelativeVirtualAddress).GetILReader();
			var visitor = new TypeDefinitionCollector();

			while (blob.RemainingBytes > 0) {
				var opCode = blob.DecodeOpCode();
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
								context.TypeSystem.ResolveAsType(member).AcceptVisitor(visitor);
								break;
							case HandleKind.MethodDefinition:
							case HandleKind.MethodSpecification:
							case HandleKind.MemberReference:
							case HandleKind.FieldDefinition:
								var m = context.TypeSystem.ResolveAsMember(member);
								if (m != null)
									yield return m;
								break;
						}
						break;
					default:
						ILParser.SkipOperand(ref blob, opCode);
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
