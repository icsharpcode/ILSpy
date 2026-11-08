// Copyright (c) 2026 Christoph Wille
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// Seam 1 of the forked <see cref="CSharpDecompiler"/>: per method body, the ILAst after the
	/// first IL transforms comes from the engine and the managed pipeline continues from there.
	/// A method the engine cannot serve falls back to the managed function and is counted; with
	/// <c>ILSPY_RUST_STRICT=1</c> a fallback is an error instead.
	/// </summary>
	public static class RustSeam1
	{
		static long served;
		static long fallbacks;

		/// <summary>Methods whose ILAst came from the engine.</summary>
		public static long Served => Interlocked.Read(ref served);

		/// <summary>Methods that fell back to the managed reader and transforms.</summary>
		public static long Fallbacks => Interlocked.Read(ref fallbacks);

		public static void ResetCounters()
		{
			Interlocked.Exchange(ref served, 0);
			Interlocked.Exchange(ref fallbacks, 0);
		}

		/// <summary>Whether fallback is disabled (<c>ILSPY_RUST_STRICT=1</c>).</summary>
		public static bool Strict => Environment.GetEnvironmentVariable("ILSPY_RUST_STRICT") == "1";

		/// <summary>
		/// Replaces <paramref name="function"/> (read by the managed reader, no transform run yet)
		/// with the engine's function after its IL prefix, moves the declaration's annotations
		/// and <paramref name="context"/> over to it, and returns the transforms that remain.
		/// Returns null when the method falls back to the managed pipeline unchanged.
		/// </summary>
		internal static IEnumerable<IILTransform>? Serve(IDecompilerTypeSystem typeSystem, IMethod method, EntityDeclaration entityDecl,
			int parameterOffset, IEnumerable<IILTransform> transforms, bool recordSteps, IDebugInfoProvider? debugInfo,
			ref ILFunction function, ref ILTransformContext context)
		{
			var pipeline = transforms.ToList();
			// The Debug Steps pane needs every transform to run managed, so it only gets the reader.
			int requested = recordSteps ? 0 : RustEngine.ILPrefix;
			int prefix = IlTransformPrefix.TopLevelBoundary(pipeline, requested, out int topLevelCount);
			if (!IlTransformPrefix.MatchesStandardPipeline(pipeline, topLevelCount))
				return Fallback("the IL transform list differs from the standard pipeline");
			var engine = RustEngine.For(typeSystem, out string? reason);
			if (engine == null)
				return Fallback(reason);
			var settings = context.Settings;
			var status = engine.ReadFunction((MethodDefinitionHandle)method.MetadataToken, prefix,
				settings, settings.UseDebugSymbols ? debugInfo : null,
				out var rustFunction, out reason);
			if (status != RustStatus.Ok)
				return Fallback(reason);

			entityDecl.RemoveAnnotations<ILFunction>();
			foreach (var parameter in entityDecl.GetChildren(Slots.Parameter))
				parameter.RemoveAnnotations<ILVariableResolveResult>();
			AddAnnotationsToDeclaration(method, entityDecl, rustFunction!, parameterOffset);
			function = rustFunction!;
			context = new ILTransformContext(context, function);
			Interlocked.Increment(ref served);
			return pipeline.Skip(topLevelCount);
		}

		/// <summary>
		/// <see cref="CSharpDecompiler.AddAnnotationsToDeclaration"/> for a function after its IL
		/// prefix. A state machine decompiler may have moved the parameters of the compiler-generated
		/// methods (another <c>this</c>) into the function; the reader's own parameters come first in
		/// the variable list, so the first parameter per index is the method's.
		/// </summary>
		static void AddAnnotationsToDeclaration(IMethod method, EntityDeclaration entityDecl, ILFunction function, int parameterOffset)
		{
			var parameters = new Dictionary<int, ILVariable>();
			foreach (var v in function.Variables)
			{
				if (v.Kind == VariableKind.Parameter && !parameters.ContainsKey(v.Index!.Value))
					parameters.Add(v.Index.Value, v);
			}
			int i = parameterOffset;
			foreach (var parameter in entityDecl.GetChildren(Slots.Parameter))
			{
				if (parameters.TryGetValue(i, out var v))
					parameter.AddAnnotation(new ILVariableResolveResult(v, method.Parameters[i].Type));
				i++;
			}
			entityDecl.AddAnnotation(function);
		}

		static IEnumerable<IILTransform>? Fallback(string? reason)
		{
			if (Strict)
				throw new RustEngineException("strict mode: the engine cannot serve this method: " + (reason ?? "unknown reason"));
			Interlocked.Increment(ref fallbacks);
			return null;
		}
	}
}
