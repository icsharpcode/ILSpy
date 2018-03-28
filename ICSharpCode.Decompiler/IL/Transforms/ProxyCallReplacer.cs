using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class ProxyCallReplacer : IILTransform
	{
		ILTransformContext context;

		public void Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;
			var module = context.TypeSystem.ModuleDefinition;
			var metadata = context.TypeSystem.GetMetadata();
			foreach (var inst in function.Descendants.OfType<CallInstruction>()) {
				var handle = (MethodDefinitionHandle)inst.Method.MetadataToken;
				if (handle.IsNil
					|| !IsDefinedInCurrentOrOuterClass(inst.Method, context.Function.Method.DeclaringTypeDefinition)
					|| !inst.Method.IsCompilerGeneratedOrIsInCompilerGeneratedClass()) {
					continue;
				}
				MethodDefinition methodDef = metadata.GetMethodDefinition((MethodDefinitionHandle)inst.Method.MetadataToken);
				if (!methodDef.HasBody()) continue;
				// partially copied from CSharpDecompiler
				var specializingTypeSystem = this.context.TypeSystem.GetSpecializingTypeSystem(inst.Method.Substitution);
				var ilReader = new ILReader(specializingTypeSystem);
				System.Threading.CancellationToken cancellationToken = new System.Threading.CancellationToken();
				var proxyFunction = ilReader.ReadIL(module, handle, module.Reader.GetMethodBody(methodDef.RelativeVirtualAddress), cancellationToken);
				var transformContext = new ILTransformContext(proxyFunction, specializingTypeSystem, this.context.Settings) {
					CancellationToken = cancellationToken,
					DecompileRun = context.DecompileRun
				};
				foreach (var transform in CSharp.CSharpDecompiler.GetILTransforms()) {
					if (transform.GetType() != typeof(ProxyCallReplacer)) { // don't call itself on itself
						cancellationToken.ThrowIfCancellationRequested();
						transform.Run(proxyFunction, transformContext);
					}
				}

				if (!(proxyFunction.Body is BlockContainer blockContainer))
					return;
				if (blockContainer.Blocks.Count != 1)
					return;
				var block = blockContainer.Blocks[0];
				Call call = null;
				if (block.Instructions.Count == 1) {
					// leave IL_0000 (call Test(ldloc this, ldloc A_1))
					if (!block.Instructions[0].MatchLeave(blockContainer, out ILInstruction returnValue))
						return;
					call = returnValue as Call;
				} else if (block.Instructions.Count == 2) {
					// call Test(ldloc this, ldloc A_1)
					// leave IL_0000(nop)
					call = block.Instructions[0] as Call;
					if (!block.Instructions[1].MatchLeave(blockContainer, out ILInstruction returnValue))
						return;
					if (!returnValue.MatchNop())
						return;
				}
				if (call == null) {
					return;
				}
				if (call.Method.IsConstructor)
					return;

				// check if original arguments are only correct ldloc calls
				for (int i = 0; i < call.Arguments.Count; i++) {
					var originalArg = call.Arguments[i];
					if (!originalArg.MatchLdLoc(out ILVariable var) ||
						var.Kind != VariableKind.Parameter ||
						var.Index != i - 1) {
						return;
					}
				}

				Call newInst = (Call)call.Clone();

				newInst.Arguments.ReplaceList(inst.Arguments);
				inst.ReplaceWith(newInst);
			}
		}

		static bool IsDefinedInCurrentOrOuterClass(IMethod method, ITypeDefinition declaringTypeDefinition)
		{
			while (declaringTypeDefinition != null) {
				if (method.DeclaringTypeDefinition == declaringTypeDefinition)
					return true;
				declaringTypeDefinition = declaringTypeDefinition.DeclaringTypeDefinition;
			}
			return false;
		}
	}
}
