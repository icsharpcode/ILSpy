// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Disassembler
{
	/// <summary>
	/// Specifies the type of an IL structure.
	/// </summary>
	public enum ILStructureType
	{
		/// <summary>
		/// The root block of the method
		/// </summary>
		Root,
		/// <summary>
		/// A nested control structure representing a loop.
		/// </summary>
		Loop,
		/// <summary>
		/// A nested control structure representing a try block.
		/// </summary>
		Try,
		/// <summary>
		/// A nested control structure representing a catch, finally, or fault block.
		/// </summary>
		Handler,
		/// <summary>
		/// A nested control structure representing an exception filter block.
		/// </summary>
		Filter
	}

	/// <summary>
	/// An IL structure.
	/// </summary>
	public class ILStructure
	{
		public readonly PEFile Module;
		public readonly MethodDefinitionHandle MethodHandle;
		public readonly GenericContext GenericContext;
		public readonly ILStructureType Type;

		/// <summary>
		/// Start position of the structure.
		/// </summary>
		public readonly int StartOffset;

		/// <summary>
		/// End position of the structure. (exclusive)
		/// </summary>
		public readonly int EndOffset;

		/// <summary>
		/// The exception handler associated with the Try, Filter or Handler block.
		/// </summary>
		public readonly ExceptionRegion ExceptionHandler;

		/// <summary>
		/// The loop's entry point.
		/// </summary>
		public readonly int LoopEntryPointOffset;

		/// <summary>
		/// The list of child structures.
		/// </summary>
		public readonly List<ILStructure> Children = new List<ILStructure>();

		public ILStructure(PEFile module, MethodDefinitionHandle handle, GenericContext genericContext, MethodBodyBlock body)
			: this(module, handle, genericContext, ILStructureType.Root, 0, body.GetILReader().Length)
		{
			// Build the tree of exception structures:
			for (int i = 0; i < body.ExceptionRegions.Length; i++)
			{
				ExceptionRegion eh = body.ExceptionRegions[i];
				if (!body.ExceptionRegions.Take(i).Any(oldEh => oldEh.TryOffset == eh.TryOffset && oldEh.TryLength == eh.TryLength))
					AddNestedStructure(new ILStructure(module, handle, genericContext, ILStructureType.Try, eh.TryOffset, eh.TryOffset + eh.TryLength, eh));
				if (eh.Kind == ExceptionRegionKind.Filter)
					AddNestedStructure(new ILStructure(module, handle, genericContext, ILStructureType.Filter, eh.FilterOffset, eh.HandlerOffset, eh));
				AddNestedStructure(new ILStructure(module, handle, genericContext, ILStructureType.Handler, eh.HandlerOffset, eh.HandlerOffset + eh.HandlerLength, eh));
			}
			// Very simple loop detection: look for backward branches
			(var allBranches, var isAfterUnconditionalBranch) = FindAllBranches(body.GetILReader());
			// We go through the branches in reverse so that we find the biggest possible loop boundary first (think loops with "continue;")
			for (int i = allBranches.Count - 1; i >= 0; i--)
			{
				int loopEnd = allBranches[i].Source.End;
				int loopStart = allBranches[i].Target;
				if (loopStart < loopEnd)
				{
					// We found a backward branch. This is a potential loop.
					// Check that is has only one entry point:
					int entryPoint = -1;

					// entry point is first instruction in loop if prev inst isn't an unconditional branch
					if (loopStart > 0 && !isAfterUnconditionalBranch[loopStart])
						entryPoint = allBranches[i].Target;

					bool multipleEntryPoints = false;
					foreach (var branch in allBranches)
					{
						if (branch.Source.Start < loopStart || branch.Source.Start >= loopEnd)
						{
							if (loopStart <= branch.Target && branch.Target < loopEnd)
							{
								// jump from outside the loop into the loop
								if (entryPoint < 0)
									entryPoint = branch.Target;
								else if (branch.Target != entryPoint)
									multipleEntryPoints = true;
							}
						}
					}
					if (!multipleEntryPoints)
					{
						AddNestedStructure(new ILStructure(module, handle, genericContext, ILStructureType.Loop, loopStart, loopEnd, entryPoint));
					}
				}
			}
			SortChildren();
		}

		public ILStructure(PEFile module, MethodDefinitionHandle handle, GenericContext genericContext, ILStructureType type, int startOffset, int endOffset, ExceptionRegion handler = default)
		{
			Debug.Assert(startOffset < endOffset);
			this.Module = module;
			this.MethodHandle = handle;
			this.GenericContext = genericContext;
			this.Type = type;
			this.StartOffset = startOffset;
			this.EndOffset = endOffset;
			this.ExceptionHandler = handler;
		}

		public ILStructure(PEFile module, MethodDefinitionHandle handle, GenericContext genericContext, ILStructureType type, int startOffset, int endOffset, int loopEntryPoint)
		{
			Debug.Assert(startOffset < endOffset);
			this.Module = module;
			this.MethodHandle = handle;
			this.GenericContext = genericContext;
			this.Type = type;
			this.StartOffset = startOffset;
			this.EndOffset = endOffset;
			this.LoopEntryPointOffset = loopEntryPoint;
		}

		bool AddNestedStructure(ILStructure newStructure)
		{
			// special case: don't consider the loop-like structure of "continue;" statements to be nested loops
			if (this.Type == ILStructureType.Loop && newStructure.Type == ILStructureType.Loop && newStructure.StartOffset == this.StartOffset)
				return false;

			// use <= for end-offset comparisons because both end and EndOffset are exclusive
			Debug.Assert(StartOffset <= newStructure.StartOffset && newStructure.EndOffset <= EndOffset);
			foreach (ILStructure child in this.Children)
			{
				if (child.StartOffset <= newStructure.StartOffset && newStructure.EndOffset <= child.EndOffset)
				{
					return child.AddNestedStructure(newStructure);
				}
				else if (!(child.EndOffset <= newStructure.StartOffset || newStructure.EndOffset <= child.StartOffset))
				{
					// child and newStructure overlap
					if (!(newStructure.StartOffset <= child.StartOffset && child.EndOffset <= newStructure.EndOffset))
					{
						// Invalid nesting, can't build a tree. -> Don't add the new structure.
						return false;
					}
				}
			}
			// Move existing structures into the new structure:
			for (int i = 0; i < this.Children.Count; i++)
			{
				ILStructure child = this.Children[i];
				if (newStructure.StartOffset <= child.StartOffset && child.EndOffset <= newStructure.EndOffset)
				{
					this.Children.RemoveAt(i--);
					newStructure.Children.Add(child);
				}
			}
			// Add the structure here:
			this.Children.Add(newStructure);
			return true;
		}

		struct Branch
		{
			public Interval Source;
			public int Target;

			public Branch(int start, int end, int target)
			{
				this.Source = new Interval(start, end);
				this.Target = target;
			}

			public override string ToString()
			{
				return $"[Branch Source={Source}, Target={Target}]";
			}
		}

		/// <summary>
		/// Finds all branches. Returns list of source offset->target offset mapping.
		/// Multiple entries for the same source offset are possible (switch statements).
		/// The result is sorted by source offset.
		/// </summary>
		(List<Branch> Branches, BitSet IsAfterUnconditionalBranch) FindAllBranches(BlobReader body)
		{
			var result = new List<Branch>();
			var bitset = new BitSet(body.Length + 1);
			body.Reset();
			int target;
			while (body.RemainingBytes > 0)
			{
				var offset = body.Offset;
				int endOffset;
				var thisOpCode = body.DecodeOpCode();
				switch (thisOpCode.GetOperandType())
				{
					case OperandType.BrTarget:
					case OperandType.ShortBrTarget:
						target = ILParser.DecodeBranchTarget(ref body, thisOpCode);
						endOffset = body.Offset;
						result.Add(new Branch(offset, endOffset, target));
						bitset[endOffset] = IsUnconditionalBranch(thisOpCode);
						break;
					case OperandType.Switch:
						var targets = ILParser.DecodeSwitchTargets(ref body);
						foreach (int t in targets)
							result.Add(new Branch(offset, body.Offset, t));
						break;
					default:
						ILParser.SkipOperand(ref body, thisOpCode);
						bitset[body.Offset] = IsUnconditionalBranch(thisOpCode);
						break;
				}
			}
			return (result, bitset);
		}

		static bool IsUnconditionalBranch(ILOpCode opCode)
		{
			switch (opCode)
			{
				case ILOpCode.Br:
				case ILOpCode.Br_s:
				case ILOpCode.Ret:
				case ILOpCode.Endfilter:
				case ILOpCode.Endfinally:
				case ILOpCode.Throw:
				case ILOpCode.Rethrow:
				case ILOpCode.Leave:
				case ILOpCode.Leave_s:
					return true;
				default:
					return false;
			}
		}

		void SortChildren()
		{
			Children.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
			foreach (ILStructure child in Children)
				child.SortChildren();
		}

		/// <summary>
		/// Gets the innermost structure containing the specified offset.
		/// </summary>
		public ILStructure GetInnermost(int offset)
		{
			Debug.Assert(StartOffset <= offset && offset < EndOffset);
			foreach (ILStructure child in this.Children)
			{
				if (child.StartOffset <= offset && offset < child.EndOffset)
					return child.GetInnermost(offset);
			}
			return this;
		}
	}
}
