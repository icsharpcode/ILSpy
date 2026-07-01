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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.DebugSteps;
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
			Stepper.Step(description, CreateNodeInfo(near));
		}

		[Conditional("STEP")]
		[DebuggerStepThrough]
		internal void StepStartGroup(string description, AstNode? near = null)
		{
			Stepper.StartGroup(description, CreateNodeInfo(near));
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
			if (Stepper.LastStep != null && modifiedNode != null)
			{
				var marker = new DebugStepMarker();
				modifiedNode.AddAnnotation(marker);
				Stepper.LastStep.ModifiedNode = modifiedNode;
				Stepper.LastStep.RecordModifiedNode(modifiedNode, extraIdentity: marker, insertFirst: true);
			}
		}

		static DebugStepNodeInfo? CreateNodeInfo(AstNode? modifiedNode)
		{
			if (modifiedNode == null)
				return null;
			// The marker rides CopyAnnotationsFrom so a replaced node's step still resolves to the
			// emitted text; it is recorded as a second identity for the changed node. The seam
			// neighbors and ancestor chain are captured from the original node before mutation.
			var marker = new DebugStepMarker();
			modifiedNode.AddAnnotation(marker);
			return new DebugStepNodeInfo(
				modifiedNode,
				modifiedNode.NextSibling,
				modifiedNode.PrevSibling,
				Ancestors(modifiedNode),
				extraIdentity: marker);

			static IEnumerable<object> Ancestors(AstNode node)
			{
				for (var parent = node.Parent; parent != null; parent = parent.Parent)
					yield return parent;
			}
		}
	}

}
