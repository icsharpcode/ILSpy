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
						if (block.Instructions.Count != 1)
							return;
						if (!block.Instructions[0].MatchLeave(blockContainer, out ILInstruction returnValue))
							return;
						if (!(returnValue is Call call))
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

						newInst.Arguments.Clear();

						ILInstruction thisArg = inst.Arguments[0];

						// special handling for first argument (this) - the underlying issue may be somewhere else
						// normally
						// leave IL_0000(await(callvirt<> n__0(ldobj xxHandler(ldloca this), ldobj System.Net.Http.HttpRequestMessage(ldloca request), ldobj System.Threading.CancellationToken(ldloca cancellationToken))))
						// would be decompiled to
						// return await((DelegatingHandler)this).SendAsync(request, cancellationToken);
						// this changes it to
						// return await base.SendAsync(request, cancellationToken);
						if (thisArg.MatchLdObj(out ILInstruction loadedObject, out IType objectType) &&
							loadedObject.MatchLdLoca(out ILVariable loadedVar)) {
							thisArg = new LdLoc(loadedVar);
						}

						newInst.Arguments.Add(thisArg);

						// add everything except first argument
						for (int i = 1; i < inst.Arguments.Count; i++) {
							newInst.Arguments.Add(inst.Arguments[i]);
						}
						inst.ReplaceWith(newInst);
					}
				}
			}
		}
	}
}
