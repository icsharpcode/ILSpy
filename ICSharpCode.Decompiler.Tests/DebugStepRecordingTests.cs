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
using ICSharpCode.Decompiler.Util;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	/// <summary>
	/// The Debug Steps view replays a decompilation by index, so a single <see cref="Stepper"/> has to
	/// span the whole pipeline: the IL transforms, the ILAst-to-C# conversion, and the C# AST
	/// transforms. These tests pin the IL half of that recording.
	/// </summary>
	[TestFixture]
	public class DebugStepRecordingTests
	{
		const string RecordedStep = "recorded IL step";
		const string DetachedStep = "step on a detached function";

		const string SeamStep = "C# AST built from ILAst";

		static readonly FullTypeName SampleType =
			new FullTypeName("ICSharpCode.Decompiler.CSharp.ProjectDecompiler.WholeProjectDecompiler");

		[Test]
		public void ILTransformStepsLandOnTheDecompilerStepper()
		{
			var decompiler = CreateRecordingDecompiler();
			decompiler.ILTransforms.Add(new RecordingILTransform());

			decompiler.DecompileTypeAsString(SampleType);

			Assert.That(TreeTraversal.PreOrder(decompiler.Stepper.Steps, n => n.Children).Select(n => n.Description), Has.Some.Contains(RecordedStep));
		}

		/// <summary>
		/// Retaining the IL steps of every member of a type is affordable for a step view and not for a
		/// whole-module decompile, which is why it is opt-in - and why callers that never opted in must
		/// keep the throwaway per-member stepper they have always had.
		/// </summary>
		[Test]
		public void ILTransformStepsAreNotRecordedWithoutOptIn()
		{
			var decompiler = StepperTesting.CreateDecompiler();
			decompiler.ILTransforms.Add(new RecordingILTransform());

			decompiler.DecompileTypeAsString(SampleType);

			Assert.That(TreeTraversal.PreOrder(decompiler.Stepper.Steps, n => n.Children).Select(n => n.Description), Has.None.Contains(RecordedStep));
		}

		/// <summary>
		/// Halting in the IL phase leaves no C# to show, so the caller needs the ILAst the pipeline
		/// stopped in - and must not be told the member failed to decompile.
		/// </summary>
		[Test]
		public void AStepLimitInTheILPhaseHaltsWithThePartiallyTransformedFunction()
		{
			var decompiler = CreateRecordingDecompiler();
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
		/// A limit in the C# AST phase prints the partially transformed tree, and nothing claims the
		/// run halted in the IL phase.
		/// </summary>
		[Test]
		public void AStepLimitInTheAstPhaseStillProducesPartialCSharp()
		{
			var decompiler = StepperTesting.CreateDecompiler();
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

			var decompiler = CreateRecordingDecompiler();
			decompiler.ILTransforms.Add(new StepperTesting.ThrowingILTransform("CleanUpFileName"));

			decompiler.DecompileTypeAsString(SampleType);

			var crashed = decompiler.Stepper.Steps.Single(n => n.Description.Contains("CleanUpFileName"));
			using (Assert.EnterMultipleScope())
			{
				Assert.That(crashed.EndStep, Is.GreaterThan(crashed.BeginStep + 1), "the abandoned group was closed at the step it stopped on");
				Assert.That(decompiler.Stepper.Steps, Has.Some.Matches<Stepper.Node>(n => n.Description.Contains("SanitizeFileName")),
					"the members after the failing one stay top-level siblings");
			}
		}

		/// <summary>
		/// A step index only means anything against the run that produced it, so there must not be a
		/// second numbering to confuse it with: with recording off the pipeline counts nothing at all,
		/// neither the IL transforms nor the C# AST transforms that used to be recorded regardless.
		/// </summary>
		[Test]
		public void NothingIsRecordedWhileRecordingIsOff()
		{
			var decompiler = StepperTesting.CreateDecompiler();

			decompiler.DecompileTypeAsString(SampleType);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(decompiler.RecordSteps, Is.False, "recording is off unless a caller asks for it");
				Assert.That(decompiler.Stepper.Steps, Is.Empty, "no step may be recorded while recording is off");
				Assert.That(decompiler.Stepper.CurrentStep, Is.Zero, "the step counter must not advance either");
			}
		}

		/// <summary>
		/// The crashed-member attribution compares the step counter against the limit, which is only
		/// meaningful while the counter is actually counting. With recording off it sits at zero, so a
		/// limit of zero would otherwise match at every throwing transform and hand the pane an
		/// unrelated member's ILAst.
		/// </summary>
		[Test]
		public void ACrashWhileRecordingIsOffAttributesNoHaltedFunction()
		{
			var decompiler = StepperTesting.CreateDecompiler();
			decompiler.ILTransforms.Add(new StepperTesting.ThrowingILTransform("CleanUpFileName"));
			decompiler.Stepper.StepLimit = 0;

			decompiler.DecompileTypeAsString(SampleType);

			Assert.That(decompiler.StepLimitHaltedFunction, Is.Null,
				"a step limit cannot be reached by a run that records no steps");
		}

		/// <summary>
		/// Closing the groups an unwind abandoned must stop at the depth the member started from: a
		/// group opened around the decompile still belongs to whoever opened it, and swallowing it
		/// files every later step as that caller's sibling instead of its child.
		/// </summary>
		[Test]
		public void AFailingTransformLeavesAGroupOpenedAroundItAlone()
		{
			if (!Stepper.SteppingAvailable)
				Assert.Ignore("Transform stepping is compiled out without the STEP symbol, so there are no groups to check.");

			var decompiler = CreateRecordingDecompiler();
			decompiler.ILTransforms.Add(new StepperTesting.ThrowingILTransform("CleanUpFileName"));

			var outer = decompiler.Stepper.StartGroup("outer");
			decompiler.DecompileTypeAsString(SampleType);
			var afterwards = decompiler.Stepper.Step("after the decompile");

			using (Assert.EnterMultipleScope())
			{
				Assert.That(outer.Children, Does.Contain(afterwards),
					"the group opened around the decompile must still be the one collecting steps");
				Assert.That(decompiler.Stepper.Steps, Does.Not.Contain(afterwards),
					"a step recorded inside the outer group is not a top-level sibling of it");
			}
		}

		/// <summary>
		/// The plain ExpressionBuilder/StatementBuilder output is a state worth looking at, so it has
		/// a step of its own at the top level rather than being reachable only as "before the first
		/// AST transform". Halting there has to print C#: the whole type is converted by then, so
		/// nothing is left in ILAst.
		/// </summary>
		[Test]
		public void TheStateAtTheSeamIsUntransformedCSharp()
		{
			if (!Stepper.SteppingAvailable)
				Assert.Ignore("Transform stepping is compiled out without the STEP symbol, so there is no seam step.");

			var decompiler = CreateRecordingDecompiler();
			string full = decompiler.DecompileTypeAsString(SampleType);

			var seam = decompiler.Stepper.Steps.SingleOrDefault(n => n.Description.Contains(SeamStep));
			Assert.That(seam, Is.Not.Null, $"'{SeamStep}' must be recorded once, at the top level");

			var replay = CreateRecordingDecompiler();
			replay.Stepper.StepLimit = seam!.BeginStep;
			string atSeam = replay.DecompileTypeAsString(SampleType);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(replay.StepLimitHaltedFunction, Is.Null,
					"the seam is past every member's IL phase, so there is no halted function to dump");
				Assert.That(atSeam, Does.Contain("class WholeProjectDecompiler"),
					"halting at the seam still prints C#");
				Assert.That(atSeam, Is.Not.EqualTo(full),
					"the AST transforms have not run yet, so this cannot equal the finished output");
			}
		}

		/// <summary>
		/// A member group ends where the next member's group begins, so the state after its last step is
		/// the state that member finished in - not the untouched IL of the member the halt unwinds from.
		/// </summary>
		[Test]
		public void TheStateAfterAMemberGroupIsThatMembersTransformedFunction()
		{
			if (!Stepper.SteppingAvailable)
				Assert.Ignore("Transform stepping is compiled out without the STEP symbol, so there are no groups to replay.");

			int endStep = MemberGroup("CleanUpFileName", CreateRecordingDecompiler()).EndStep;

			var replay = CreateRecordingDecompiler();
			replay.Stepper.StepLimit = endStep;
			replay.DecompileTypeAsString(SampleType);

			Assert.That(replay.StepLimitHaltedFunction?.Name, Is.EqualTo("CleanUpFileName"));
		}

		/// <summary>
		/// Replaying the state after a group whose transform threw stops on the exception, never on a
		/// step: the ILAst the crash left half-transformed is the whole point of looking at it.
		/// </summary>
		[Test]
		public void TheStateAfterACrashedGroupIsTheFunctionTheTransformCrashedIn()
		{
			if (!Stepper.SteppingAvailable)
				Assert.Ignore("Transform stepping is compiled out without the STEP symbol, so there are no groups to replay.");

			var decompiler = CreateRecordingDecompiler();
			decompiler.ILTransforms.Add(new StepperTesting.ThrowingILTransform("CleanUpFileName"));
			decompiler.DecompileTypeAsString(SampleType);
			int endStep = decompiler.Stepper.Steps.Single(n => n.Description.Contains("CleanUpFileName")).EndStep;

			var replay = CreateRecordingDecompiler();
			replay.ILTransforms.Add(new StepperTesting.ThrowingILTransform("CleanUpFileName"));
			replay.Stepper.StepLimit = endStep;
			replay.DecompileTypeAsString(SampleType);

			Assert.That(replay.StepLimitHaltedFunction?.Name, Is.EqualTo("CleanUpFileName"));
		}

		/// <summary>
		/// Some transforms build a helper function and work on it before attaching it to the member
		/// (ProxyCallReplacer, the nested-function decompilers). A halt in there has to render the tree
		/// that holds the halted instruction; the member's function does not contain it yet.
		/// </summary>
		[Test]
		public void AHaltOnADetachedFunctionRendersThatFunction()
		{
			var detachedStep = new DetachedFunctionILTransform("CleanUpFileName");
			var decompiler = CreateRecordingDecompiler();
			decompiler.ILTransforms.Add(detachedStep);
			decompiler.DecompileTypeAsString(SampleType);
			int haltAt = TreeTraversal.PreOrder(decompiler.Stepper.Steps, n => n.Children).Single(n => n.Description.Contains(DetachedStep)).BeginStep;

			var replay = CreateRecordingDecompiler();
			var replayStep = new DetachedFunctionILTransform("CleanUpFileName");
			replay.ILTransforms.Add(replayStep);
			replay.Stepper.StepLimit = haltAt;
			replay.DecompileTypeAsString(SampleType);

			Assert.That(replay.StepLimitHaltedFunction, Is.SameAs(replayStep.DetachedFunction));
		}

		/// <summary>
		/// Runs the decompiler once and returns the top-level group recording the named member's IL phase.
		/// </summary>
		static Stepper.Node MemberGroup(string methodName, CSharpDecompiler decompiler)
		{
			decompiler.DecompileTypeAsString(SampleType);
			return decompiler.Stepper.Steps.Single(n => n.Description.EndsWith("." + methodName, StringComparison.Ordinal));
		}

		static CSharpDecompiler CreateRecordingDecompiler()
		{
			var decompiler = StepperTesting.CreateDecompiler();
			decompiler.RecordSteps = true;
			return decompiler;
		}

		/// <summary>
		/// Records one step per top-level function through <see cref="Stepper.Step"/> rather than
		/// <c>context.Step</c>: the latter is <c>[Conditional("STEP")]</c>, and that is resolved where
		/// the call is compiled - this assembly, which never defines STEP - so a <c>context.Step</c>
		/// call would vanish and leave the tests passing vacuously in every configuration.
		/// </summary>
		/// <summary>
		/// Records one step on an instruction of a function that hangs off nothing, standing in for the
		/// helper functions the real transforms build before attaching them.
		/// </summary>
		sealed class DetachedFunctionILTransform(string methodName) : IILTransform
		{
			public ILFunction? DetachedFunction { get; private set; }

			public void Run(ILFunction function, ILTransformContext context)
			{
				if (function.Parent != null || function.Method?.Name != methodName)
					return;
				DetachedFunction = new ILFunction(function.Method!, function.CodeSize, function.GenericContext,
					new Nop(), ILFunctionKind.LocalFunction);
				context.Stepper.Step(DetachedStep, new DebugStepNodeInfo(DetachedFunction.Body));
			}
		}

		sealed class RecordingILTransform : IILTransform
		{
			public void Run(ILFunction function, ILTransformContext context)
			{
				if (function.Parent == null)
					context.Stepper.Step(RecordedStep);
			}
		}

	}
}
