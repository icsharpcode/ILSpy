// Copyright (c) 2026 Siegfried Pammer
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

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.DebugSteps;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	/// <summary>
	/// The Debug Steps view replays a decompilation by index, so a single <see cref="Stepper"/> has to
	/// span the whole pipeline: the IL transforms, the ILAst-to-C# conversion, and the C# AST
	/// transforms. These tests pin the IL half, which the C# path used to discard.
	/// </summary>
	[TestFixture]
	public class DebugStepRecordingTests
	{
		const string RecordedStep = "recorded IL step";

		static readonly FullTypeName SampleType =
			new FullTypeName("ICSharpCode.Decompiler.CSharp.ProjectDecompiler.WholeProjectDecompiler");

		[Test]
		public void ILTransformStepsLandOnTheDecompilerStepper()
		{
			var decompiler = CreateDecompiler();
			decompiler.RecordILTransformSteps = true;
			decompiler.ILTransforms.Add(new RecordingILTransform());

			decompiler.DecompileTypeAsString(SampleType);

			Assert.That(Descriptions(decompiler.Stepper.Steps), Has.Some.Contains(RecordedStep));
		}

		/// <summary>
		/// Retaining the IL steps of every member of a type is affordable for a step view and not for a
		/// whole-module decompile, which is why it is opt-in - and why callers that never opted in must
		/// keep the throwaway per-member stepper they have always had.
		/// </summary>
		[Test]
		public void ILTransformStepsAreNotRecordedWithoutOptIn()
		{
			var decompiler = CreateDecompiler();
			decompiler.ILTransforms.Add(new RecordingILTransform());

			decompiler.DecompileTypeAsString(SampleType);

			Assert.That(Descriptions(decompiler.Stepper.Steps), Has.None.Contains(RecordedStep));
		}

		/// <summary>
		/// Halting in the IL phase leaves no C# to show, so the caller needs the ILAst the pipeline
		/// stopped in - and must not be told the member failed to decompile.
		/// </summary>
		[Test]
		public void AStepLimitInTheILPhaseHaltsWithThePartiallyTransformedFunction()
		{
			var decompiler = CreateDecompiler();
			decompiler.RecordILTransformSteps = true;
			decompiler.ILTransforms.Insert(0, new RecordingILTransform());
			decompiler.Stepper.StepLimit = 0;

			string code = decompiler.DecompileTypeAsString(SampleType);

			var astTransformNames = CSharpDecompiler.GetAstTransforms().Select(t => t.GetType().Name).ToArray();
			using (Assert.EnterMultipleScope())
			{
				Assert.That(decompiler.StepLimitHaltedFunction, Is.Not.Null, "the caller renders this as ILAst");
				Assert.That(decompiler.Stepper.LimitReachedStep, Is.Not.Null);
				Assert.That(decompiler.Errors, Is.Empty, "a deliberate halt is not a decompilation failure");
				Assert.That(code, Does.Not.Contain(CSharpDecompiler.DecompilationErrorReportUrl));
				// The C# AST phase must not run after an IL-phase halt. It would hit the same limit
				// again - the step counter never advances past it - and the second hit would replace
				// LimitReachedStep with an AST node, so the position the halted step is highlighted at
				// would be lost.
				Assert.That(astTransformNames, Has.None.Matches<string>(
					name => decompiler.Stepper.LimitReachedStep!.Description.Contains(name)),
					"the halt must still report the IL step it stopped on");
			}
		}

		/// <summary>
		/// The C# AST phase keeps its existing behaviour: the partially transformed tree is still
		/// printed, and nothing claims the run halted in the IL phase.
		/// </summary>
		[Test]
		public void AStepLimitInTheAstPhaseStillProducesPartialCSharp()
		{
			var decompiler = CreateDecompiler();
			decompiler.Stepper.StepLimit = 1;

			string code = decompiler.DecompileTypeAsString(SampleType);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(decompiler.StepLimitHaltedFunction, Is.Null);
				Assert.That(code, Does.Contain("DecompileProject"), "the members are still written");
			}
		}

		/// <summary>
		/// A member whose transform throws must not swallow the members after it: their steps belong
		/// beside it in the tree, not inside the group it abandoned.
		/// </summary>
		[Test]
		public void AFailingTransformDoesNotNestLaterMembersUnderIt()
		{
			if (!Stepper.SteppingAvailable)
				Assert.Ignore("Transform stepping is compiled out without the STEP symbol, so there are no groups to check.");

			var decompiler = CreateDecompiler();
			decompiler.RecordILTransformSteps = true;
			decompiler.ILTransforms.Add(new ThrowingILTransform("CleanUpFileName"));

			decompiler.DecompileTypeAsString(SampleType);

			var crashed = decompiler.Stepper.Steps.Single(n => n.Description.Contains("CleanUpFileName"));
			using (Assert.EnterMultipleScope())
			{
				Assert.That(crashed.EndStep, Is.GreaterThan(crashed.BeginStep + 1), "the abandoned group was closed at the step it stopped on");
				Assert.That(decompiler.Stepper.Steps, Has.Some.Matches<Stepper.Node>(n => n.Description.Contains("DecompileProject")),
					"the members after the failing one stay top-level siblings");
			}
		}

		static IEnumerable<string> Descriptions(IEnumerable<Stepper.Node> nodes)
		{
			foreach (var node in nodes)
			{
				yield return node.Description;
				foreach (var description in Descriptions(node.Children))
					yield return description;
			}
		}

		static CSharpDecompiler CreateDecompiler()
		{
			var module = new PEFile("ICSharpCode.Decompiler.dll");
			var settings = new DecompilerSettings();
			var typeSystem = new DecompilerTypeSystem(module, new UniversalAssemblyResolver(null, false, null), settings);
			return new CSharpDecompiler(typeSystem, settings);
		}

		/// <summary>
		/// Records one step per top-level function through <see cref="Stepper.Step"/> rather than
		/// <c>context.Step</c>: the latter is <c>[Conditional("STEP")]</c>, and that is resolved where
		/// the call is compiled - this assembly, which never defines STEP - so a <c>context.Step</c>
		/// call would vanish and leave the tests passing vacuously in every configuration.
		/// </summary>
		sealed class RecordingILTransform : IILTransform
		{
			public void Run(ILFunction function, ILTransformContext context)
			{
				if (function.Parent == null)
					context.Stepper.Step(RecordedStep);
			}
		}

		sealed class ThrowingILTransform(string methodName) : IILTransform
		{
			public void Run(ILFunction function, ILTransformContext context)
			{
				if (function.Parent == null && function.Method?.Name == methodName)
					throw new InvalidOperationException("Simulated transform failure");
			}
		}
	}
}
