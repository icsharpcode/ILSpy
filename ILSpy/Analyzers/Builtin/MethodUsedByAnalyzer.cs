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
	/// Shows entities that are used by a method.
	/// </summary>
	[Export(typeof(IAnalyzer<IMethod>))]
	class MethodUsedByAnalyzer : IMethodBodyAnalyzer<IMethod>
	{
		public string Text => "Used By";

		public bool Show(IMethod entity) => true;

		public IEnumerable<IEntity> Analyze(IMethod analyzedMethod, IMethod method, MethodBodyBlock methodBody, AnalyzerContext context)
		{
			var blob = methodBody.GetILReader();

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
							case HandleKind.MethodDefinition:
							case HandleKind.MethodSpecification:
							case HandleKind.MemberReference:
								var m = context.TypeSystem.ResolveAsMember(member)?.MemberDefinition;
								if (m.MetadataToken == analyzedMethod.MetadataToken && m.ParentAssembly.PEFile == analyzedMethod.ParentAssembly.PEFile) {
									yield return method;
									yield break;
								}
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
