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
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// IL language support.
	/// </summary>
	/// <remarks>
	/// Currently comes in two versions:
	/// flat IL (detectControlStructure=false) and structured IL (detectControlStructure=true).
	/// </remarks>
	[Export(typeof(Language))]
	public class ILLanguage : Language
	{
		protected bool detectControlStructure = true;

		public override string Name {
			get { return "IL"; }
		}

		public override string FileExtension {
			get { return ".il"; }
		}

		protected virtual ReflectionDisassembler CreateDisassembler(ITextOutput output, DecompilationOptions options)
		{
			output.IndentationString = options.DecompilerSettings.CSharpFormattingOptions.IndentationString;
			return new ReflectionDisassembler(output, options.CancellationToken) {
				DetectControlStructure = detectControlStructure,
				ShowSequencePoints = options.DecompilerSettings.ShowDebugInfo,
				ShowMetadataTokens = Options.DisplaySettingsPanel.CurrentDisplaySettings.ShowMetadataTokens,
				ShowMetadataTokensInBase10 = Options.DisplaySettingsPanel.CurrentDisplaySettings.ShowMetadataTokensInBase10,
				ShowRawRVAOffsetAndBytes = Options.DisplaySettingsPanel.CurrentDisplaySettings.ShowRawOffsetsAndBytesBeforeInstruction,
				ExpandMemberDefinitions = options.DecompilerSettings.ExpandMemberDefinitions
			};
		}

		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			var dis = CreateDisassembler(output, options);
			PEFile module = method.ParentModule.PEFile;
			dis.AssemblyResolver = module.GetAssemblyResolver();
			dis.DebugInfo = module.GetDebugInfoOrNull();
			dis.DisassembleMethod(module, (MethodDefinitionHandle)method.MetadataToken);
		}

		public override void DecompileField(IField field, ITextOutput output, DecompilationOptions options)
		{
			var dis = CreateDisassembler(output, options);
			PEFile module = field.ParentModule.PEFile;
			dis.AssemblyResolver = module.GetAssemblyResolver();
			dis.DebugInfo = module.GetDebugInfoOrNull();
			dis.DisassembleField(module, (FieldDefinitionHandle)field.MetadataToken);
		}

		public override void DecompileProperty(IProperty property, ITextOutput output, DecompilationOptions options)
		{
			var dis = CreateDisassembler(output, options);
			PEFile module = property.ParentModule.PEFile;
			dis.AssemblyResolver = module.GetAssemblyResolver();
			dis.DebugInfo = module.GetDebugInfoOrNull();
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
			/*foreach (var m in property.OtherMethods) {
				output.WriteLine();
				dis.DisassembleMethod(m);
			}*/
		}

		public override void DecompileEvent(IEvent ev, ITextOutput output, DecompilationOptions options)
		{
			var dis = CreateDisassembler(output, options);
			PEFile module = ev.ParentModule.PEFile;
			dis.AssemblyResolver = module.GetAssemblyResolver();
			dis.DebugInfo = module.GetDebugInfoOrNull();
			dis.DisassembleEvent(module, (EventDefinitionHandle)ev.MetadataToken);

			var ed = ((MetadataReader)module.Metadata).GetEventDefinition((EventDefinitionHandle)ev.MetadataToken);
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
			/*foreach (var m in ev.OtherMethods) {
				output.WriteLine();
				dis.DisassembleMethod(m);
			}*/
		}

		public override void DecompileType(ITypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			var dis = CreateDisassembler(output, options);
			PEFile module = type.ParentModule.PEFile;
			dis.AssemblyResolver = module.GetAssemblyResolver();
			dis.DebugInfo = module.GetDebugInfoOrNull();
			dis.DisassembleType(module, (TypeDefinitionHandle)type.MetadataToken);
		}

		public override void DecompileNamespace(string nameSpace, IEnumerable<ITypeDefinition> types, ITextOutput output, DecompilationOptions options)
		{
			var dis = CreateDisassembler(output, options);
			PEFile module = types.FirstOrDefault()?.ParentModule.PEFile;
			dis.AssemblyResolver = module.GetAssemblyResolver();
			dis.DebugInfo = module.GetDebugInfoOrNull();
			dis.DisassembleNamespace(nameSpace, module, types.Select(t => (TypeDefinitionHandle)t.MetadataToken));
		}

		public override ProjectId DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			output.WriteLine("// " + assembly.FileName);
			output.WriteLine();
			var module = assembly.GetPEFileAsync().GetAwaiter().GetResult();
			var metadata = module.Metadata;
			var dis = CreateDisassembler(output, options);

			if (options.FullDecompilation && options.SaveAsProjectDirectory != null)
			{
				throw new NotSupportedException($"Language '{Name}' does not support exporting assemblies as projects!");
			}

			// don't automatically load additional assemblies when an assembly node is selected in the tree view
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

		public override RichText GetRichTextTooltip(IEntity entity)
		{
			var output = new AvalonEditTextOutput() { IgnoreNewLineAndIndent = true };
			var disasm = CreateDisassembler(output, new DecompilationOptions());
			PEFile module = entity.ParentModule?.PEFile;
			if (module == null)
			{
				return null;
			}

			switch (entity.SymbolKind)
			{
				case SymbolKind.TypeDefinition:
					disasm.DisassembleTypeHeader(module, (TypeDefinitionHandle)entity.MetadataToken);
					break;
				case SymbolKind.Field:
					disasm.DisassembleFieldHeader(module, (FieldDefinitionHandle)entity.MetadataToken);
					break;
				case SymbolKind.Property:
				case SymbolKind.Indexer:
					disasm.DisassemblePropertyHeader(module, (PropertyDefinitionHandle)entity.MetadataToken);
					break;
				case SymbolKind.Event:
					disasm.DisassembleEventHeader(module, (EventDefinitionHandle)entity.MetadataToken);
					break;
				case SymbolKind.Method:
				case SymbolKind.Operator:
				case SymbolKind.Constructor:
				case SymbolKind.Destructor:
				case SymbolKind.Accessor:
					disasm.DisassembleMethodHeader(module, (MethodDefinitionHandle)entity.MetadataToken);
					break;
				default:
					output.Write(GetDisplayName(entity, true, true, true));
					break;
			}

			return new DocumentHighlighter(output.GetDocument(), base.SyntaxHighlighting).HighlightLine(1).ToRichText();
		}
	}
}
