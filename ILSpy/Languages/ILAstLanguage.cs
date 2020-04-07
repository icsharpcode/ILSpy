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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

using SRM = System.Reflection.Metadata;
using static System.Reflection.Metadata.PEReaderExtensions;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy
{
#if DEBUG
	/// <summary>
	/// Represents the ILAst "language" used for debugging purposes.
	/// </summary>
	abstract class ILAstLanguage : Language
	{
		public event EventHandler StepperUpdated;

		protected virtual void OnStepperUpdated(EventArgs e = null)
		{
			StepperUpdated?.Invoke(this, e ?? new EventArgs());
		}

		public Stepper Stepper { get; set; } = new Stepper();

		readonly string name;
		
		protected ILAstLanguage(string name)
		{
			this.name = name;
		}
		
		public override string Name { get { return name; } }

		internal static IEnumerable<ILAstLanguage> GetDebugLanguages()
		{
			yield return new TypedIL();
			yield return new BlockIL(CSharpDecompiler.GetILTransforms());
		}
		
		public override string FileExtension {
			get {
				return ".il";
			}
		}

		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			base.DecompileMethod(method, output, options);
			new ReflectionDisassembler(output, options.CancellationToken)
				.DisassembleMethodHeader(method.ParentModule.PEFile, (SRM.MethodDefinitionHandle)method.MetadataToken);
			output.WriteLine();
			output.WriteLine();
		}

		class TypedIL : ILAstLanguage
		{
			public TypedIL() : base("Typed IL") {}
			
			public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
			{
				base.DecompileMethod(method, output, options);
				var module = method.ParentModule.PEFile;
				var methodDef = module.Metadata.GetMethodDefinition((SRM.MethodDefinitionHandle)method.MetadataToken);
				if (!methodDef.HasBody())
					return;
				var typeSystem = new DecompilerTypeSystem(module, module.GetAssemblyResolver());
				ILReader reader = new ILReader(typeSystem.MainModule);
				var methodBody = module.Reader.GetMethodBody(methodDef.RelativeVirtualAddress);
				reader.WriteTypedIL((SRM.MethodDefinitionHandle)method.MetadataToken, methodBody, output, cancellationToken: options.CancellationToken);
			}
		}

		class BlockIL : ILAstLanguage
		{
			readonly IReadOnlyList<IILTransform> transforms;

			public BlockIL(IReadOnlyList<IILTransform> transforms) : base("ILAst")
			{
				this.transforms = transforms;
			}

			public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
			{
				base.DecompileMethod(method, output, options);
				var module = method.ParentModule.PEFile;
				var metadata = module.Metadata;
				var methodDef = metadata.GetMethodDefinition((SRM.MethodDefinitionHandle)method.MetadataToken);
				if (!methodDef.HasBody())
					return;
				IAssemblyResolver assemblyResolver = module.GetAssemblyResolver();
				var typeSystem = new DecompilerTypeSystem(module, assemblyResolver);
				var reader = new ILReader(typeSystem.MainModule);
				reader.UseDebugSymbols = options.DecompilerSettings.UseDebugSymbols;
				var methodBody = module.Reader.GetMethodBody(methodDef.RelativeVirtualAddress);
				ILFunction il = reader.ReadIL((SRM.MethodDefinitionHandle)method.MetadataToken, methodBody, kind: ILFunctionKind.TopLevelFunction, cancellationToken: options.CancellationToken);
				var decompiler = new CSharpDecompiler(typeSystem, options.DecompilerSettings) { CancellationToken = options.CancellationToken };
				ILTransformContext context = decompiler.CreateILTransformContext(il);
				context.Stepper.StepLimit = options.StepLimit;
				context.Stepper.IsDebug = options.IsDebug;
				try {
					il.RunTransforms(transforms, context);
				} catch (StepLimitReachedException) {
				} catch (Exception ex) {
					output.WriteLine(ex.ToString());
					output.WriteLine();
					output.WriteLine("ILAst after the crash:");
				} finally {
					// update stepper even if a transform crashed unexpectedly
					if (options.StepLimit == int.MaxValue) {
						Stepper = context.Stepper;
						OnStepperUpdated(new EventArgs());
					}
				}
				(output as ISmartTextOutput)?.AddButton(Images.ViewCode, "Show Steps", delegate {
					Docking.DockWorkspace.Instance.ShowToolPane(DebugStepsPaneModel.PaneContentId);
				});
				output.WriteLine();
				il.WriteTo(output, DebugSteps.Options);
			}
		}
	}
	#endif
}
