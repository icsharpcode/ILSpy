// Copyright (c) 2014 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable enable

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using ICSharpCode.Decompiler.IL.Transforms;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// Detect suitable exit points for BlockContainers.
	/// 
	/// An "exit point" is an instruction that causes control flow
	/// to leave the container (a branch or leave instruction).
	/// 
	/// If an "exit point" instruction is placed immediately following a
	/// block container, each equivalent exit point within the container
	/// can be replaced with a "leave container" instruction.
	/// 
	/// This transform performs this replacement: any exit points
	/// equivalent to the exit point following the container are
	/// replaced with a leave instruction.
	/// Additionally, if the container is not yet followed by an exit point,
	/// but has room to introduce such an exit point (i.e. iff the container's 
	/// end point is currently unreachable), we pick one of the non-return
	/// exit points within the container, move it to the position following the
	/// container, and replace all instances within the container with a leave
	/// instruction.
	/// 
	/// This makes it easier for the following transforms to construct
	/// control flow that falls out of blocks instead of using goto/break statements.
	/// </summary>
	public class DetectExitPoints : ILVisitor, IILTransform
	{
		static readonly Nop ExitNotYetDetermined = new Nop { Comment = "ExitNotYetDetermined" };
		static readonly Nop NoExit = new Nop { Comment = "NoExit" };

		/// <summary>
		/// Gets the next instruction after <paramref name="inst"/> is executed.
		/// Returns NoExit when the next instruction cannot be identified;
		/// returns <c>null</c> when the end of a Block is reached (so that we could insert an arbitrary instruction)
		/// </summary>
		internal static ILInstruction GetExit(ILInstruction inst)
		{
			SlotInfo? slot = inst.SlotInfo;
			if (slot == Block.InstructionSlot)
			{
				Block block = (Block)inst.Parent!;
				return block.Instructions.ElementAtOrDefault(inst.ChildIndex + 1) ?? ExitNotYetDetermined;
			}
			else if (slot == TryInstruction.TryBlockSlot
			  || slot == TryCatchHandler.BodySlot
			  || slot == TryCatch.HandlerSlot
			  || slot == PinnedRegion.BodySlot
			  || slot == UsingInstruction.BodySlot
			  || slot == LockInstruction.BodySlot)
			{
				return GetExit(inst.Parent!);
			}
			return NoExit;
		}

		/// <summary>
		/// Returns true iff exit1 and exit2 are both exit instructions
		/// (branch or leave) and both represent the same exit.
		/// </summary>
		internal static bool CompatibleExitInstruction(ILInstruction exit1, ILInstruction exit2)
		{
			if (exit1 == null || exit2 == null || exit1.OpCode != exit2.OpCode)
				return false;
			switch (exit1.OpCode)
			{
				case OpCode.Branch:
					Branch br1 = (Branch)exit1;
					Branch br2 = (Branch)exit2;
					return br1.TargetBlock == br2.TargetBlock;
				case OpCode.Leave:
					Leave leave1 = (Leave)exit1;
					Leave leave2 = (Leave)exit2;
					return leave1.TargetContainer == leave2.TargetContainer && leave1.Value.MatchNop() && leave2.Value.MatchNop();
				default:
					return false;
			}
		}

		class ContainerContext
		{
			public readonly BlockContainer Container;

			/// <summary>
			/// The instruction that will be executed next after leaving the Container.
			/// <c>ExitNotYetDetermined</c> means the container is last in its parent block, and thus does not
			/// yet have any leave instructions. This means we can move any exit instruction of
			/// our choice our of the container and replace it with a leave instruction.
			/// </summary>
			public readonly ILInstruction CurrentExit;

			/// <summary>
			/// If <c>currentExit==ExitNotYetDetermined</c>, holds the list of potential exit instructions.
			/// After the currentContainer was visited completely, one of these will be selected as exit instruction.
			/// </summary>
			public readonly List<ILInstruction>? PotentialExits = null;

			public ContainerContext(BlockContainer container, ILInstruction currentExit)
			{
				this.Container = container;
				this.CurrentExit = currentExit;
				this.PotentialExits = (currentExit == ExitNotYetDetermined ? new List<ILInstruction>() : null);
			}

			public void HandleExit(ILInstruction inst)
			{
				if (this.CurrentExit == ExitNotYetDetermined && this.Container.LeaveCount == 0)
				{
					this.PotentialExits!.Add(inst);
				}
				else if (CompatibleExitInstruction(inst, this.CurrentExit))
				{
					inst.ReplaceWith(new Leave(this.Container).WithILRange(inst));
				}
			}
		}

		CancellationToken cancellationToken;
		readonly List<Block> blocksPotentiallyMadeUnreachable = new List<Block>();
		readonly Stack<ContainerContext> containerStack = new Stack<ContainerContext>();

		public void Run(ILFunction function, ILTransformContext context)
		{
			cancellationToken = context.CancellationToken;
			blocksPotentiallyMadeUnreachable.Clear();
			containerStack.Clear();
			function.AcceptVisitor(this);
			// It's possible that there are unreachable code blocks which we only
			// detect as such during exit point detection.
			// Clean them up.
			foreach (var block in blocksPotentiallyMadeUnreachable)
			{
				if (block.IncomingEdgeCount == 0 || block.IncomingEdgeCount == 1 && IsInfiniteLoop(block))
				{
					block.Remove();
				}
			}
			blocksPotentiallyMadeUnreachable.Clear();
			containerStack.Clear();
		}

		static bool IsInfiniteLoop(Block block)
		{
			return block.Instructions.Count == 1
				&& block.Instructions[0] is Branch b
				&& b.TargetBlock == block;
		}

		protected override void Default(ILInstruction inst)
		{
			foreach (var child in inst.Children)
				child.AcceptVisitor(this);
		}

		protected internal override void VisitBlockContainer(BlockContainer container)
		{
			var thisExit = GetExit(container);
			var stackEntry = new ContainerContext(container, thisExit);
			containerStack.Push(stackEntry);
			base.VisitBlockContainer(container);
			if (stackEntry.PotentialExits?.Any(i => i.IsConnected) ?? false)
			{
				// This transform determined an exit point.
				var newExit = ChooseExit(stackEntry.PotentialExits.Where(i => i.IsConnected));
				Debug.Assert(!newExit.MatchLeave(container));
				foreach (var exit in stackEntry.PotentialExits)
				{
					if (exit.IsConnected && CompatibleExitInstruction(newExit, exit))
					{
						exit.ReplaceWith(new Leave(container).WithILRange(exit));
					}
				}
				ILInstruction inst = container;
				// traverse up to the block (we'll always find one because GetExit
				// only returns ExitNotYetDetermined if there's a block)
				while (inst.Parent!.OpCode != OpCode.Block)
					inst = inst.Parent;
				Block block = (Block)inst.Parent;
				if (block.HasFlag(InstructionFlags.EndPointUnreachable))
				{
					// Special case: despite replacing the exits with leave(currentContainer),
					// we still have an unreachable endpoint.
					// The appended currentExit instruction would not be reachable!
					// This happens in test case ExceptionHandling.ThrowInFinally()
					if (newExit is Branch b)
					{
						blocksPotentiallyMadeUnreachable.Add(b.TargetBlock);
					}
				}
				else
				{
					block.Instructions.Add(newExit);
				}
			}
			if (containerStack.Pop() != stackEntry)
			{
				Debug.Fail("containerStack got imbalanced");
			}
		}

		static ILInstruction ChooseExit(IEnumerable<ILInstruction> potentialExits)
		{
			using var enumerator = potentialExits.GetEnumerator();
			enumerator.MoveNext();
			ILInstruction first = enumerator.Current;
			if (first is Leave { IsLeavingFunction: true })
			{
				while (enumerator.MoveNext())
				{
					var exit = enumerator.Current;
					if (!(exit is Leave { IsLeavingFunction: true }))
						return exit;
				}
			}
			return first;
		}

		protected internal override void VisitBlock(Block block)
		{
			cancellationToken.ThrowIfCancellationRequested();
			// Don't use foreach loop, because the children might add to the block
			for (int i = 0; i < block.Instructions.Count; i++)
			{
				block.Instructions[i].AcceptVisitor(this);
			}
		}

		protected internal override void VisitBranch(Branch inst)
		{
			foreach (var entry in containerStack)
			{
				if (inst.TargetBlock.IsDescendantOf(entry.Container))
					break;
				entry.HandleExit(inst);
			}
		}

		protected internal override void VisitLeave(Leave inst)
		{
			base.VisitLeave(inst);
			if (!inst.Value.MatchNop())
				return;
			foreach (var entry in containerStack)
			{
				if (inst.TargetContainer == entry.Container)
					break;
				if (inst.IsLeavingFunction || inst.TargetContainer.Kind != ContainerKind.Normal)
				{
					if (entry.Container.Kind == ContainerKind.Normal)
					{
						// Don't transform a `return`/`break` into a leave for try-block containers (or similar).
						// It's possible that those can be turned into fallthrough later, but
						// it might not work out and then we would be left with a `goto`.
						// But continue searching the container stack, it might be possible to
						// turn the `return` into a `break` instead.
					}
					else
					{
						// return; could turn to break;
						entry.HandleExit(inst);
						break; // but only for the innermost loop/switch
					}
				}
				else
				{
					entry.HandleExit(inst);
				}
			}
		}
	}
}
