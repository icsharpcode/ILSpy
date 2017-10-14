using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using Mono.Cecil;

namespace ICSharpCode.ILSpy
{
	[Export(typeof(Language))]
	class CSharpILMixedLanguage : Language
	{
		private readonly bool detectControlStructure = true;

		public override string Name => "IL with C#";

		public override string FileExtension => ".il";

		public override void DecompileMethod(MethodDefinition method, ITextOutput output, DecompilationOptions options)
		{
			var dis = new ReflectionDisassembler(output, detectControlStructure, options.CancellationToken);
			dis.DisassembleMethod(method);
		}
	}
}
