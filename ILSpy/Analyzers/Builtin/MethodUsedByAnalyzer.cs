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

		public bool Show(IMethod entity) => !entity.IsVirtual;

		public IEnumerable<IEntity> Analyze(IMethod analyzedMethod, IMethod method, MethodBodyBlock methodBody, AnalyzerContext context)
		{
			var blob = methodBody.GetILReader();

			var baseMethod = InheritanceHelper.GetBaseMember(analyzedMethod);

			while (blob.RemainingBytes > 0) {
				var opCode = blob.DecodeOpCode();
				if (opCode != ILOpCode.Call && opCode != ILOpCode.Callvirt) {
					ILParser.SkipOperand(ref blob, opCode);
					continue;
				}
				var member = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
				if (member.IsNil || !member.Kind.IsMemberKind()) continue;

				var m = context.TypeSystem.ResolveAsMember(member)?.MemberDefinition;
				if (m == null) continue;

				if (opCode == ILOpCode.Call) {
					if (IsSameMember(analyzedMethod, m)) {
						yield return method;
						yield break;
					}
				}

				if (opCode == ILOpCode.Callvirt && baseMethod != null) {
					if (IsSameMember(baseMethod, m)) {
						yield return method;
						yield break;
					}
				}
			}
		}

		static bool IsSameMember(IMember analyzedMethod, IMember m)
		{
			return m.MetadataToken == analyzedMethod.MetadataToken
				&& m.ParentAssembly.PEFile == analyzedMethod.ParentAssembly.PEFile;
		}
	}
}
