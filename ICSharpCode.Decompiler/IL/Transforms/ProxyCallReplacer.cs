using System.Diagnostics;
using ICSharpCode.Decompiler.TypeSystem;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class ProxyCallReplacer : ILVisitor, IILTransform
	{
		ILTransformContext context;

		public void Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;
			function.AcceptVisitor(this);
		}

		protected override void Default(ILInstruction inst)
		{
			foreach (var child in inst.Children) {
				child.AcceptVisitor(this);
			}
		}

		protected internal override void VisitCallVirt(CallVirt inst)
		{
			VisitCally(inst);
		}

		protected internal override void VisitCall(Call inst)
		{
			VisitCally(inst);
		}

		protected internal void VisitCally(CallInstruction inst)
		{
			if (inst.Method.DeclaringTypeDefinition == null) // TODO: investigate why
				return;
			foreach (IMethod method in inst.Method.DeclaringTypeDefinition.Methods) {
				if (method.FullName.Equals(inst.Method.FullName)) {
					MethodDefinition methodDef = context.TypeSystem.GetCecil(method) as MethodDefinition;
					if (methodDef != null && methodDef.Body != null) {
						if (method.IsCompilerGeneratedOrIsInCompilerGeneratedClass()) {
							// partially copied from CSharpDecompiler
							var specializingTypeSystem = this.context.TypeSystem.GetSpecializingTypeSystem(this.context.TypeSystem.Compilation.TypeResolveContext);
							var ilReader = new ILReader(specializingTypeSystem);
							System.Threading.CancellationToken cancellationToken = new System.Threading.CancellationToken();
							var function = ilReader.ReadIL(methodDef.Body, cancellationToken);
							var context = new ILTransformContext(function, specializingTypeSystem, this.context.Settings) {
								CancellationToken = cancellationToken
							};
							foreach (var transform in CSharp.CSharpDecompiler.GetILTransforms()) {
								if (transform.GetType() != typeof(ProxyCallReplacer)) { // don't call itself on itself
									cancellationToken.ThrowIfCancellationRequested();
									transform.Run(function, context);
								}
							}
							Call currentCall = new ProxyMethodVisitor().GetCalledMethod(function, context);
							if (currentCall != null) {
								Call newInst = (Call)currentCall.Clone();
								
								// check if original arguments are only correct ldloc calls
								for (int i = 0; i < currentCall.Arguments.Count; i++) {
									var originalArg = currentCall.Arguments.ElementAtOrDefault(i);
									if (originalArg.OpCode != OpCode.LdLoc ||
										originalArg.Children.Count != 0 ||
										((LdLoc)originalArg).Variable.Kind != VariableKind.Parameter ||
										((LdLoc)originalArg).Variable.Index != i-1) {
										return;
									}
								}
								newInst.Arguments.Clear();

								ILInstruction thisArg = inst.Arguments.ElementAtOrDefault(0).Clone();

								// special handling for first argument (this) - the underlying issue may be somewhere else
								// normally
								// leave IL_0000(await(callvirt<> n__0(ldobj xxHandler(ldloca this), ldobj System.Net.Http.HttpRequestMessage(ldloca request), ldobj System.Threading.CancellationToken(ldloca cancellationToken))))
								// would be decompiled to
								// return await((DelegatingHandler)this).SendAsync(request, cancellationToken);
								// this changes it to
								// return await base.SendAsync(request, cancellationToken);
								if (thisArg.OpCode == OpCode.LdObj && thisArg.Children.Count > 0 && thisArg.Children[0].OpCode == OpCode.LdLoca) {
									thisArg = new LdLoc(((LdLoca)thisArg.Children[0]).Variable);
								}

								newInst.Arguments.Add(thisArg);

								// add everything except first argument
								for (int i = 1; i < inst.Arguments.Count; i++) {
									newInst.Arguments.Add(inst.Arguments.ElementAtOrDefault(i).Clone());
								}
								inst.ReplaceWith(newInst);
							}
						}
					}
				}
			}
		}

		// Checks if the method is a proxy method by checking if it only contains a call instruction and if it has special compiler attributes.
		private class ProxyMethodVisitor : ILVisitor
		{
			ILTransformContext context;
			int invalidInstructions = 0;
			Call currentCall;

			public Call GetCalledMethod(ILFunction function, ILTransformContext context)
			{
				this.context = context;
				function.AcceptVisitor(this);
				if (invalidInstructions == 0) {
					return currentCall;
				}
				return null;
			}

			protected override void Default(ILInstruction inst)
			{
				if (inst.OpCode != OpCode.ILFunction &&
					inst.OpCode != OpCode.BlockContainer &&
					inst.OpCode != OpCode.Block &&
					inst.OpCode != OpCode.Leave &&
					inst.OpCode != OpCode.Nop) {
					invalidInstructions++;
				}
				foreach (var child in inst.Children) {
					child.AcceptVisitor(this);
				}
			}

			protected internal override void VisitCall(Call inst)
			{
				if (currentCall == null) {
					currentCall = inst;
				} else {
					invalidInstructions++; // more than one call in the function
				}
			}
		}
	}
}
