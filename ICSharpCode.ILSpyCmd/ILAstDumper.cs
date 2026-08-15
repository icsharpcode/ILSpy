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

#if DEBUG

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpyCmd
{
	/// <summary>
	/// Writes the decompiler's intermediate representation (ILAst) of a method body, optionally
	/// stopping the IL transform pipeline after a chosen transform. This is the command-line
	/// counterpart of the UI's "ILAst" language, and makes transform output diffable.
	/// </summary>
	class ILAstDumper
	{
		readonly IReadOnlyList<IILTransform> transforms = CSharpDecompiler.GetILTransforms();
		readonly ILAstWritingOptions writingOptions = new ILAstWritingOptions();

		/// <summary>
		/// Names of the IL transforms, in the order they run. Transforms nested inside a
		/// BlockILTransform (LoopDetection, ConditionDetection, the statement transforms, ...)
		/// are not listed: they run as part of their containing entry and cannot be stopped
		/// after individually.
		/// </summary>
		public static IReadOnlyList<string> TransformNames { get; } =
			CSharpDecompiler.GetILTransforms().Select(t => t.GetType().Name).ToArray();

		public static int TransformCount => TransformNames.Count;

		/// <summary>
		/// The pipeline as displayed to the user: one transform per line, prefixed by the
		/// 1-based index that <c>--after-transform</c> accepts.
		/// </summary>
		public static string DescribePipeline()
		{
			return string.Join(Environment.NewLine,
				TransformNames.Select((name, index) => $"  {index + 1,3}  {name}"));
		}

		/// <summary>
		/// Maps the value of <c>--after-transform</c> to the number of transforms to run.
		/// Accepts a 1-based pipeline index, or a transform name if it occurs exactly once;
		/// names that run repeatedly (SplitVariables, ControlFlowSimplification, ...) have to
		/// be selected by index.
		/// </summary>
		public static bool TryResolveTransformCount(string nameOrIndex, out int count, out string error)
		{
			count = 0;
			error = null;
			string trimmed = nameOrIndex.Trim();

			if (int.TryParse(trimmed, NumberStyles.None, CultureInfo.InvariantCulture, out int index))
			{
				if (index < 1 || index > TransformCount)
				{
					error = $"'{trimmed}' is out of range; the pipeline has {TransformCount} transforms:{Environment.NewLine}{DescribePipeline()}";
					return false;
				}
				count = index;
				return true;
			}

			var occurrences = TransformNames
				.Select((name, i) => (name, position: i + 1))
				.Where(t => string.Equals(t.name, trimmed, StringComparison.OrdinalIgnoreCase))
				.Select(t => t.position)
				.ToArray();

			if (occurrences.Length == 0)
			{
				error = $"Unknown transform '{trimmed}'. Pass one of these names, or its index:{Environment.NewLine}{DescribePipeline()}";
				return false;
			}
			if (occurrences.Length > 1)
			{
				error = $"'{trimmed}' runs {occurrences.Length} times, at index {string.Join(", ", occurrences)}. Pass the index of the occurrence to stop after.";
				return false;
			}

			count = occurrences[0];
			return true;
		}

		/// <summary>
		/// Writes the ILAst of a single method, or nothing at all if the method has no body
		/// (abstract, extern or a runtime-provided implementation).
		/// </summary>
		public void WriteMethod(CSharpDecompiler decompiler, DecompilerSettings settings, IMethod method,
			int transformCount, ITextOutput output, CancellationToken cancellationToken)
		{
			if (method.MetadataToken.IsNil || method.MetadataToken.Kind != HandleKind.MethodDefinition)
				return;
			var metadataFile = decompiler.TypeSystem.MainModule.MetadataFile;
			var handle = (MethodDefinitionHandle)method.MetadataToken;
			var methodDefinition = metadataFile.Metadata.GetMethodDefinition(handle);
			if (!methodDefinition.HasBody())
				return;

			output.WriteLine($"// {method.FullName}");
			output.WriteLine($"// ILAst after {transformCount} of {TransformCount} transforms ({TransformNames[transformCount - 1]})");

			var reader = new ILReader(decompiler.TypeSystem.MainModule) {
				UseDebugSymbols = settings.UseDebugSymbols,
				UseRefLocalsForAccurateOrderOfEvaluation = settings.UseRefLocalsForAccurateOrderOfEvaluation,
			};
			var body = metadataFile.GetMethodBody(methodDefinition.RelativeVirtualAddress);
			ILFunction function = reader.ReadIL(handle, body, kind: ILFunctionKind.TopLevelFunction,
				cancellationToken: cancellationToken);
			ILTransformContext context = decompiler.CreateILTransformContext(function);
			try
			{
				function.RunTransforms(transforms.Take(transformCount), context);
			}
			catch (Exception ex)
			{
				// Showing how far the pipeline got is the point of this command, so a crashing
				// transform prints its exception and then the partially transformed function
				// rather than aborting the whole dump.
				output.WriteLine(ex.ToString());
				output.WriteLine("// ILAst after the crash:");
			}
			function.WriteTo(output, writingOptions);
			output.WriteLine();
			output.WriteLine();
		}
	}
}

#endif
