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

using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Parameters for IAstTransform.
	/// </summary>
	public class TransformContext
	{
		public readonly IDecompilerTypeSystem TypeSystem;
		public readonly CancellationToken CancellationToken;
		public readonly TypeSystemAstBuilder TypeSystemAstBuilder;
		public readonly DecompilerSettings Settings;
		internal readonly DecompileRun DecompileRun;
		public Stepper Stepper { get; set; }

		readonly ITypeResolveContext decompilationContext;

		/// <summary>
		/// Returns the current member; or null if a whole type or module is being decompiled.
		/// </summary>
		public IMember? CurrentMember => decompilationContext.CurrentMember;

		/// <summary>
		/// Returns the current type definition; or null if a module is being decompiled.
		/// </summary>
		public ITypeDefinition? CurrentTypeDefinition => decompilationContext.CurrentTypeDefinition;

		/// <summary>
		/// Returns the module that is being decompiled.
		/// </summary>
		public IModule? CurrentModule => decompilationContext.CurrentModule;

		/// <summary>
		/// Returns the max possible set of namespaces that will be used during decompilation.
		/// </summary>
		public IImmutableSet<string> RequiredNamespacesSuperset => DecompileRun.Namespaces.ToImmutableHashSet();

		internal TransformContext(IDecompilerTypeSystem typeSystem, DecompileRun decompileRun, ITypeResolveContext decompilationContext, TypeSystemAstBuilder typeSystemAstBuilder)
		{
			this.TypeSystem = typeSystem;
			this.DecompileRun = decompileRun;
			this.decompilationContext = decompilationContext;
			this.TypeSystemAstBuilder = typeSystemAstBuilder;
			this.CancellationToken = decompileRun.CancellationToken;
			this.Settings = decompileRun.Settings;
			this.Stepper = new Stepper();
		}

		/// <summary>
		/// Call this method immediately before performing a transform step.
		/// Unlike <c>context.Stepper.Step()</c>, calls to this method are only compiled in debug builds.
		/// </summary>
		[Conditional("STEP")]
		[DebuggerStepThrough]
		internal void Step(string description, AstNode? near = null)
		{
			TrackModifiedNode(Stepper.Step(description, modifiedNode: near), near);
		}

		[Conditional("STEP")]
		[DebuggerStepThrough]
		internal void StepStartGroup(string description, AstNode? near = null)
		{
			TrackModifiedNode(Stepper.StartGroup(description, modifiedNode: near), near);
		}

		[Conditional("STEP")]
		internal void StepEndGroup(bool keepIfEmpty = false)
		{
			Stepper.EndGroup(keepIfEmpty);
		}

		/// <summary>
		/// Points the most recently recorded step at the node its mutation produced.
		/// Call this after a <see cref="Step"/> whose modified node only comes into existence
		/// during the mutation (e.g. the result of a ReplaceWith or a freshly inserted node).
		/// </summary>
		[Conditional("STEP")]
		internal void EndStep(AstNode? modifiedNode)
		{
			if (Stepper.LastStep != null)
			{
				Stepper.LastStep.ModifiedNode = modifiedNode;
				TrackModifiedNode(Stepper.LastStep, modifiedNode, insertFirst: true);
			}
		}

		static void TrackModifiedNode(Stepper.Node step, AstNode? modifiedNode, bool insertFirst = false)
		{
			if (modifiedNode == null)
				return;
			AddCandidate(step, modifiedNode, insertFirst);
			var marker = new DebugStepMarker();
			modifiedNode.AddAnnotation(marker);
			AddCandidate(step, marker, insertFirst: false);
			// insertFirst marks the produced-node update from EndStep; the seam neighbours and
			// ancestor chain are recorded once, from the original node captured before the mutation.
			if (!insertFirst)
			{
				if (modifiedNode.NextSibling is { } nextSibling)
					step.SeamAnchors.Add((nextSibling, false));
				if (modifiedNode.PrevSibling is { } prevSibling)
					step.SeamAnchors.Add((prevSibling, true));
				for (var parent = modifiedNode.Parent; parent != null; parent = parent.Parent)
				{
					step.AncestorCandidates.Add(parent);
				}
			}
		}

		static void AddCandidate(Stepper.Node step, object candidate, bool insertFirst)
		{
			if (step.ModifiedNodeCandidates.Contains(candidate))
				return;
			if (insertFirst)
				step.ModifiedNodeCandidates.Insert(0, candidate);
			else
				step.ModifiedNodeCandidates.Add(candidate);
		}

		sealed class DebugStepMarker
		{
		}
	}
}
