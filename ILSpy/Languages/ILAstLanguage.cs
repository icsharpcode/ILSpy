// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

#if DEBUG

using System;
using System.Collections.Generic;
using System.Composition;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.ILSpy.Languages
{
	/// <summary>
	/// Debug-only language that surfaces the decompiler pipeline's intermediate state.
	/// Two concrete variants ship: <see cref="TypedIL"/> renders raw IL with type
	/// annotations; <see cref="BlockIL"/> runs the decompiler's IL transforms with a
	/// <see cref="Stepper"/> attached so the Debug Steps pane can replay each transform.
	/// Compiled only when DEBUG is defined — the language list is identical to Release
	/// otherwise.
	/// </summary>
	public abstract class ILAstLanguage : Language, IDebugStepProvider
	{
		readonly string name;

		protected ILAstLanguage(string name)
		{
			this.name = name;
		}

		// ILAst output uses the same `{}/()/[]` bracket conventions as C#, plus C#-style
		// `//` comments and `"..."` strings. Reusing CSharpBracketSearcher gives the
		// language correct bracket highlighting without a per-grammar implementation.
		public override ICSharpCode.ILSpy.TextView.IBracketSearcher BracketSearcher { get; } = new CSharpBracketSearcher();

		/// <summary>
		/// Fires after a <see cref="DecompileMethod"/> run installs a fresh <see cref="Stepper"/>.
		/// The Debug Steps pane subscribes here so it can rebind its TreeView to the new
		/// step list whenever the user reruns decompilation.
		/// </summary>
		public event EventHandler? StepperUpdated;

		protected virtual void OnStepperUpdated(EventArgs? e = null)
			=> StepperUpdated?.Invoke(this, e ?? EventArgs.Empty);

		public Stepper Stepper { get; set; } = new();

		// ILAst contributes the shared writing-options checkboxes to the Debug Steps pane.
		public object? StepOptions => DebugStepsPaneModel.WritingOptions;

		public override string Name => name;

		public override string FileExtension => ".il";

		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			base.DecompileMethod(method, output, options);
			new ReflectionDisassembler(output, options.CancellationToken)
				.DisassembleMethodHeader(method.ParentModule!.MetadataFile, (SRM.MethodDefinitionHandle)method.MetadataToken);
			output.WriteLine();
			output.WriteLine();
		}
	}

	/// <summary>
	/// Raw IL with type annotations on each instruction. No transforms, no stepper — the
	/// Debug Steps pane stays empty under this language because there's nothing to step
	/// through. Useful as a sanity-check view for the IL reader itself.
	/// </summary>
	[Export(typeof(Language))]
	[Shared]
	public sealed class TypedILLanguage : ILAstLanguage
	{
		public TypedILLanguage() : base("Typed IL") { }

		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			base.DecompileMethod(method, output, options);
			var module = method.ParentModule!.MetadataFile!;
			var methodDef = module.Metadata.GetMethodDefinition((SRM.MethodDefinitionHandle)method.MetadataToken);
			if (!methodDef.HasBody())
				return;
			var typeSystem = new DecompilerTypeSystem(module, module.GetAssemblyResolver());
			var reader = new ILReader(typeSystem.MainModule);
			var methodBody = module.GetMethodBody(methodDef.RelativeVirtualAddress);
			reader.WriteTypedIL((SRM.MethodDefinitionHandle)method.MetadataToken, methodBody, output, cancellationToken: options.CancellationToken);
		}
	}

	/// <summary>
	/// Runs the full C# decompiler's IL transforms and writes the resulting <see cref="ILFunction"/>.
	/// Each transform is recorded by a <see cref="Stepper"/> hooked into the
	/// <see cref="ILTransformContext"/>; the resulting step tree is what the Debug Steps
	/// pane visualises. The "Show Steps" button on the output reveals the pane.
	/// </summary>
	[Export(typeof(Language))]
	[Shared]
	public sealed class BlockILLanguage : ILAstLanguage
	{
		readonly IReadOnlyList<IILTransform> transforms;

		public BlockILLanguage() : base("ILAst")
		{
			this.transforms = CSharpDecompiler.GetILTransforms();
		}

		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			base.DecompileMethod(method, output, options);
			var module = method.ParentModule!.MetadataFile!;
			var metadata = module.Metadata;
			var methodDef = metadata.GetMethodDefinition((SRM.MethodDefinitionHandle)method.MetadataToken);
			if (!methodDef.HasBody())
				return;
			IAssemblyResolver assemblyResolver = module.GetAssemblyResolver();
			var typeSystem = new DecompilerTypeSystem(module, assemblyResolver);
			var reader = new ILReader(typeSystem.MainModule) {
				UseDebugSymbols = options.DecompilerSettings.UseDebugSymbols,
				UseRefLocalsForAccurateOrderOfEvaluation = options.DecompilerSettings.UseRefLocalsForAccurateOrderOfEvaluation,
			};
			var methodBody = module.GetMethodBody(methodDef.RelativeVirtualAddress);
			ILFunction il = reader.ReadIL((SRM.MethodDefinitionHandle)method.MetadataToken, methodBody,
				kind: ILFunctionKind.TopLevelFunction, cancellationToken: options.CancellationToken);
			var decompiler = new CSharpDecompiler(typeSystem, options.DecompilerSettings) { CancellationToken = options.CancellationToken };
			ILTransformContext context = decompiler.CreateILTransformContext(il);
			context.Stepper.StepLimit = options.StepLimit;
			context.Stepper.IsDebug = options.IsDebug;
			try
			{
				il.RunTransforms(transforms, context);
			}
			catch (StepLimitReachedException)
			{
				// Expected when the Debug Steps pane asked to halt after a specific step.
			}
			catch (Exception ex)
			{
				output.WriteLine(ex.ToString());
				output.WriteLine();
				output.WriteLine("ILAst after the crash:");
			}
			finally
			{
				// Capture the populated stepper even when a transform crashed, so the user
				// can see the partial step tree leading up to the failure point. Only when
				// the run was full-fidelity (no StepLimit) — partial runs leave the stepper
				// alone so the previously-shown tree stays visible.
				if (options.StepLimit == int.MaxValue)
				{
					Stepper = context.Stepper;
					OnStepperUpdated();
				}
			}
			// DockWorkspace is resolved lazily here, not via [ImportingConstructor]: it imports
			// LanguageService, which imports the registered Languages, so a constructor import would
			// form a composition cycle. The lazy lookup costs one MEF resolve per "Show Steps" click.
			(output as ISmartTextOutput)?.AddButton(Images.ViewCode, "Show Steps", delegate {
				AppComposition.TryGetExport<DockWorkspace>()?.ShowToolPane(DebugStepsPaneModel.PaneContentId);
			});
			output.WriteLine();
			il.WriteTo(output, DebugStepsPaneModel.WritingOptions);
		}
	}
}

#endif
