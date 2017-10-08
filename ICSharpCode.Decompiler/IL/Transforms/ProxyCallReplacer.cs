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
			MethodDefinition methodDef = context.TypeSystem.GetCecil(inst.Method) as MethodDefinition;
			if (methodDef != null && methodDef.Body != null) {
				if (inst.Method.IsCompilerGeneratedOrIsInCompilerGeneratedClass()) {
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


					if (function.Children.Count != 1)
						return;
					var blockContainer = function.Children[0];
					if (blockContainer.OpCode != OpCode.BlockContainer)
						return;
					if (blockContainer.Children.Count != 1)
						return;
					var block = blockContainer.Children[0];
					if (block.OpCode != OpCode.Block)
						return;
					if (block.Children.Count > 2)
						return;
					if (block.Children.Count == 2 && block.Children[1].OpCode != OpCode.Nop)
						return;
					var leave = block.Children[0];
					if (leave.OpCode != OpCode.Leave)
						return;
					if (leave.Children.Count != 1)
						return;
					Call call = leave.Children[0] as Call;
					if (call == null)
						return;
					// check if original arguments are only correct ldloc calls
					for (int i = 0; i < call.Arguments.Count; i++) {
						var originalArg = call.Arguments[i];
						if (originalArg.OpCode != OpCode.LdLoc ||
							originalArg.Children.Count != 0 ||
							((LdLoc)originalArg).Variable.Kind != VariableKind.Parameter ||
							((LdLoc)originalArg).Variable.Index != i - 1) {
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
					if (thisArg.OpCode == OpCode.LdObj && thisArg.Children.Count > 0 && thisArg.Children[0].OpCode == OpCode.LdLoca) {
						thisArg = new LdLoc(((LdLoca)thisArg.Children[0]).Variable);
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
