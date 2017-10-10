using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class ProxyCallReplacer : IILTransform
	{
		ILTransformContext context;

		public void Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;
			foreach (var inst in function.Descendants.OfType<CallInstruction>()) {
				MethodDefinition methodDef = context.TypeSystem.GetCecil(inst.Method) as MethodDefinition;
				if (methodDef != null && methodDef.Body != null) {
					if (inst.Method.IsCompilerGeneratedOrIsInCompilerGeneratedClass()) {
						// partially copied from CSharpDecompiler
						var specializingTypeSystem = this.context.TypeSystem.GetSpecializingTypeSystem(this.context.TypeSystem.Compilation.TypeResolveContext);
						var ilReader = new ILReader(specializingTypeSystem);
						System.Threading.CancellationToken cancellationToken = new System.Threading.CancellationToken();
						var proxyFunction = ilReader.ReadIL(methodDef.Body, cancellationToken);
						var transformContext = new ILTransformContext(proxyFunction, specializingTypeSystem, this.context.Settings) {
							CancellationToken = cancellationToken
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
			}
		}
	}
}
