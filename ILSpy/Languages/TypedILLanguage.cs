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

using System.Composition;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.ILSpy.Languages
{
	/// <summary>
	/// Debug-only language rendering raw IL with the type the reader inferred for each instruction.
	/// It runs no transforms, so it is a sanity-check view for the IL reader itself rather than a
	/// stage of the decompiler pipeline - the pipeline is walked in the Debug Steps pane instead,
	/// under the C# language.
	/// Compiled only when DEBUG is defined; the language list is identical to Release otherwise.
	/// </summary>
	[Export(typeof(Language))]
	[Shared]
	public sealed class TypedILLanguage : Language
	{
		public override string Name => "Typed IL";

		public override string FileExtension => ".il";

		// Typed IL output uses the same `{}/()/[]` bracket conventions as C#, plus C#-style
		// `//` comments and `"..."` strings. Reusing CSharpBracketSearcher gives the
		// language correct bracket highlighting without a per-grammar implementation.
		public override ICSharpCode.ILSpy.TextView.IBracketSearcher BracketSearcher { get; } = new CSharpBracketSearcher();

		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			base.DecompileMethod(method, output, options);
			var module = method.ParentModule!.MetadataFile!;
			new ReflectionDisassembler(output, options.CancellationToken)
				.DisassembleMethodHeader(module, (SRM.MethodDefinitionHandle)method.MetadataToken);
			output.WriteLine();
			output.WriteLine();
			var methodDef = module.Metadata.GetMethodDefinition((SRM.MethodDefinitionHandle)method.MetadataToken);
			if (!methodDef.HasBody())
				return;
			var typeSystem = new DecompilerTypeSystem(module, module.GetAssemblyResolver());
			var reader = new ILReader(typeSystem.MainModule);
			var methodBody = module.GetMethodBody(methodDef.RelativeVirtualAddress);
			reader.WriteTypedIL((SRM.MethodDefinitionHandle)method.MetadataToken, methodBody, output, cancellationToken: options.CancellationToken);
		}
	}
}

#endif
