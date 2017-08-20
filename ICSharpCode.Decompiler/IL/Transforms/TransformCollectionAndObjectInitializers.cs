using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transforms array initialization pattern of System.Runtime.CompilerServices.RuntimeHelpers.InitializeArray.
	/// For collection and object initializers see <see cref="TransformInitializers"/>
	/// </summary>
	public class TransformCollectionAndObjectInitializers : IBlockTransform
	{
		BlockTransformContext context;

		void IBlockTransform.Run(Block block, BlockTransformContext context)
		{
			this.context = context;
			for (int i = block.Instructions.Count - 1; i >= 0; i--)
			{
				DoTransform(block, i);
			}
		}

		enum InitializerType
		{
			None,
			Collection,
			Object
		}

		bool DoTransform(Block body, int pos)
		{
			ILInstruction inst = body.Instructions[pos];
			// Match stloc(v, newobj)
			if (inst.MatchStLoc(out var v, out var initInst) && v.Kind == VariableKind.StackSlot && initInst is NewObj newObj) {
				context.Step("CollectionOrObjectInitializer", inst);
				int initializerItemsCount = 0;
				var initializerType = InitializerType.None;
				// Detect initializer type by scanning the following statements
				// each must be a callvirt with ldloc v as first argument
				// if the method is a setter we're dealing with an object initializer
				// if the method is named Add and has at least 2 arguments we're dealing with a collection/dictionary initializer
				while (pos + initializerItemsCount + 1 < body.Instructions.Count
					&& (MatchesCallVirt(body.Instructions[pos + initializerItemsCount + 1], v, ref initializerType) || MatchesStObj(body.Instructions[pos + initializerItemsCount + 1], v, ref initializerType)))
				{
					initializerItemsCount++;
				}
				if (initializerItemsCount == 0)
					return false;
				Block initBlock;
				switch (initializerType) {
					case InitializerType.Collection:
						initBlock = new Block(BlockType.CollectionInitializer);
						break;
					case InitializerType.Object:
						initBlock = new Block(BlockType.ObjectInitializer);
						break;
					default: return false;
				}
				var finalSlot = context.Function.RegisterVariable(VariableKind.StackSlot, v.Type);
				initBlock.FinalInstruction = new LdLoc(finalSlot);
				initBlock.Instructions.Add(new StLoc(finalSlot, initInst.Clone()));
				for (int i = 1; i <= initializerItemsCount; i++) {
					switch (body.Instructions[i + pos]) {
						case CallVirt callVirt:
							var newCallVirt = new CallVirt(callVirt.Method);
							var newTarget = callVirt.Arguments[0].Clone();
							foreach (var load in newTarget.Descendants.OfType<LdLoc>())
								load.Variable = finalSlot;
							newCallVirt.Arguments.Add(newTarget);
							newCallVirt.Arguments.AddRange(callVirt.Arguments.Skip(1).Select(a => a.Clone()));
							initBlock.Instructions.Add(newCallVirt);
							break;
						case StObj stObj:
							var newStObj = (StObj)stObj.Clone();
							foreach (var load in newStObj.Target.Descendants.OfType<LdLoc>())
								load.Variable = finalSlot;
							initBlock.Instructions.Add(newStObj);
							break;
					}

				}
				initInst.ReplaceWith(initBlock);
				for (int i = 0; i < initializerItemsCount; i++)
					body.Instructions.RemoveAt(pos + 1);
				ILInlining.InlineIfPossible(body, ref pos, context);
			}
			return true;
		}

		bool MatchesStObj(ILInstruction current, ILVariable v, ref InitializerType initializerType)
		{
			if (current is StObj so
				&& so.Target is LdFlda ldfa
				&& ldfa.Target.MatchLdLoc(v))
				return true;
			return false;
		}

		bool MatchesCallVirt(ILInstruction current, ILVariable v, ref InitializerType initializerType)
		{
			return current is CallVirt cv
				&& cv.Arguments.Count >= 2
				&& !cv.Arguments.Skip(1).Any(a => a.Descendants.OfType<LdLoc>().Any(b => b.MatchLdLoc(v)))
				&& IsMatchingTarget(cv.Arguments[0], v)
				&& IsMatchingMethod(cv, ref initializerType);
		}

		bool IsMatchingTarget(ILInstruction target, ILVariable targetVariable)
		{
			if (target.MatchLdLoc(targetVariable))
				return true;
			if (target is LdObj lo && lo.Target is LdFlda ldfa && ldfa.Target.MatchLdLoc(targetVariable))
				return true;
			if (target is CallVirt cv && cv.Method.IsAccessor && cv.Arguments.Count == 1 && cv.Arguments[0].MatchLdLoc(targetVariable))
				return true;
			return false;
		}

		bool IsMatchingMethod(CallVirt cv, ref InitializerType type)
		{
			if (cv.Method.IsAccessor && cv.Method.AccessorOwner is IProperty p && p.Setter == cv.Method) {
				type = InitializerType.Object;
				return true;
			} else if (cv.Method.Name.Equals("Add", StringComparison.Ordinal)) {
				if (type == InitializerType.None)
					type = InitializerType.Collection;
				return true;
			}
			return false;
		}
	}
}
