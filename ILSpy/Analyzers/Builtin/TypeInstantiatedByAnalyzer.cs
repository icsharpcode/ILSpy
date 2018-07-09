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
	/// Shows methods that instantiate a type.
	/// </summary>
	[Export(typeof(IAnalyzer<ITypeDefinition>))]
	class TypeInstantiatedByAnalyzer : IMethodBodyAnalyzer<ITypeDefinition>
	{
		public string Text => "Instantiated By";

		public IEnumerable<IEntity> Analyze(ITypeDefinition analyzedEntity, IMethod method, MethodBodyBlock methodBody, AnalyzerContext context)
		{
			bool found = false;
			var blob = methodBody.GetILReader();

			while (!found && blob.RemainingBytes > 0) {
				var opCode = blob.DecodeOpCode();
				if (!CanBeReference(opCode)) {
					blob.SkipOperand(opCode);
					continue;
				}
				EntityHandle methodHandle = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
				if (!methodHandle.Kind.IsMemberKind())
					continue;
				var ctor = context.TypeSystem.ResolveAsMethod(methodHandle);
				if (ctor == null || !ctor.IsConstructor)
					continue;

				found = ctor.DeclaringTypeDefinition?.MetadataToken == analyzedEntity.MetadataToken
					&& ctor.ParentAssembly.PEFile == analyzedEntity.ParentAssembly.PEFile;
			}

			if (found) {
				yield return method;
			}
		}

		bool CanBeReference(ILOpCode opCode)
		{
			return opCode == ILOpCode.Newobj || opCode == ILOpCode.Initobj;
		}

		public bool Show(ITypeDefinition entity) => !entity.IsAbstract && !entity.IsStatic;
	}
}
