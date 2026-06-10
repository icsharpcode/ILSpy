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

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

namespace ILSpy.Languages
{
	[Export(typeof(Language))]
	[Shared]
	public class ILLanguage : Language
	{
		protected bool detectControlStructure = true;

		public override string Name => "IL";

		public override string FileExtension => ".il";

		// Disassembled IL uses the same `{}/()/[]` bracket conventions plus C#-style
		// `//` comments and `"..."` strings — reuse CSharpBracketSearcher's logic.
		public override ILSpy.TextView.IBracketSearcher BracketSearcher { get; } = new CSharpBracketSearcher();

		// DisplaySettings (ShowMetadataTokens / ShowRawRVAOffsetAndBytes /
		// DecodeCustomAttributeBlobs / ShowMetadataTokensInBase10) aren't wired yet — once
		// they are, plumb them in here. All four default to false.
		protected virtual ReflectionDisassembler CreateDisassembler(ITextOutput output, DecompilationOptions options)
		{
			output.IndentationString = options.DecompilerSettings.CSharpFormattingOptions.IndentationString;
			return new ReflectionDisassembler(output, options.CancellationToken) {
				DetectControlStructure = detectControlStructure,
				ShowSequencePoints = options.DecompilerSettings.ShowDebugInfo,
				ShowMetadataTokens = false,
				ShowMetadataTokensInBase10 = false,
				ShowRawRVAOffsetAndBytes = false,
				ExpandMemberDefinitions = options.DecompilerSettings.ExpandMemberDefinitions,
				DecodeCustomAttributeBlobs = false,
			};
		}

		/// <summary>
		/// Creates a disassembler wired to <paramref name="module"/>'s assembly resolver and
		/// debug info — the setup shared by every member-level Decompile* entry point.
		/// </summary>
		ReflectionDisassembler CreateDisassembler(ITextOutput output, DecompilationOptions options, MetadataFile module)
		{
			var dis = CreateDisassembler(output, options);
			dis.AssemblyResolver = module.GetAssemblyResolver();
			dis.DebugInfo = module.GetDebugInfoOrNull();
			return dis;
		}

		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile module = method.ParentModule!.MetadataFile!;
			var dis = CreateDisassembler(output, options, module);
			dis.DisassembleMethod(module, (MethodDefinitionHandle)method.MetadataToken);
		}

		public override void DecompileField(IField field, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile module = field.ParentModule!.MetadataFile!;
			var dis = CreateDisassembler(output, options, module);
			dis.DisassembleField(module, (FieldDefinitionHandle)field.MetadataToken);
		}

		public override void DecompileProperty(IProperty property, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile module = property.ParentModule!.MetadataFile!;
			var dis = CreateDisassembler(output, options, module);
			dis.DisassembleProperty(module, (PropertyDefinitionHandle)property.MetadataToken);
			var pd = module.Metadata.GetPropertyDefinition((PropertyDefinitionHandle)property.MetadataToken);
			var accessors = pd.GetAccessors();

			if (!accessors.Getter.IsNil)
			{
				output.WriteLine();
				dis.DisassembleMethod(module, accessors.Getter);
			}
			if (!accessors.Setter.IsNil)
			{
				output.WriteLine();
				dis.DisassembleMethod(module, accessors.Setter);
			}
		}

		public override void DecompileEvent(IEvent ev, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile module = ev.ParentModule!.MetadataFile!;
			var dis = CreateDisassembler(output, options, module);
			dis.DisassembleEvent(module, (EventDefinitionHandle)ev.MetadataToken);

			var ed = module.Metadata.GetEventDefinition((EventDefinitionHandle)ev.MetadataToken);
			var accessors = ed.GetAccessors();
			if (!accessors.Adder.IsNil)
			{
				output.WriteLine();
				dis.DisassembleMethod(module, accessors.Adder);
			}
			if (!accessors.Remover.IsNil)
			{
				output.WriteLine();
				dis.DisassembleMethod(module, accessors.Remover);
			}
			if (!accessors.Raiser.IsNil)
			{
				output.WriteLine();
				dis.DisassembleMethod(module, accessors.Raiser);
			}
		}

		public override void DecompileType(ITypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile module = type.ParentModule!.MetadataFile!;
			var dis = CreateDisassembler(output, options, module);
			dis.DisassembleType(module, (TypeDefinitionHandle)type.MetadataToken);
		}

		public override void DecompileNamespace(string nameSpace, IEnumerable<ITypeDefinition> types, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile? module = types.FirstOrDefault()?.ParentModule!.MetadataFile;
			if (module == null)
				return;
			var dis = CreateDisassembler(output, options, module);
			dis.DisassembleNamespace(nameSpace, module, types.Select(t => (TypeDefinitionHandle)t.MetadataToken));
		}

		public override ProjectId? DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			output.WriteLine("// " + assembly.FileName);
			output.WriteLine();
			var module = assembly.GetMetadataFileOrNull();
			if (module == null)
				return null;
			if (options.FullDecompilation && options.SaveAsProjectDirectory != null)
				throw new NotSupportedException($"Language '{Name}' does not support exporting assemblies as projects!");

			var metadata = module.Metadata;
			var dis = CreateDisassembler(output, options);
			dis.AssemblyResolver = module.GetAssemblyResolver(loadOnDemand: options.FullDecompilation);
			dis.DebugInfo = module.GetDebugInfoOrNull();
			if (options.FullDecompilation)
				dis.WriteAssemblyReferences(metadata);
			if (metadata.IsAssembly)
				dis.WriteAssemblyHeader(module);
			output.WriteLine();
			dis.WriteModuleHeader(module);
			if (options.FullDecompilation)
			{
				output.WriteLine();
				output.WriteLine();
				dis.WriteModuleContents(module);
			}
			return null;
		}
	}
}
