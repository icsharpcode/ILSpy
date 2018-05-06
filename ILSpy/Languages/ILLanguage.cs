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

using System.Collections.Generic;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using System.ComponentModel.Composition;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
using ICSharpCode.Decompiler.Metadata;

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
			return new ReflectionDisassembler(output, options.CancellationToken) {
				DetectControlStructure = detectControlStructure,
				ShowSequencePoints = options.DecompilerSettings.ShowDebugInfo,
				ExpandMemberDefinitions = options.DecompilerSettings.ExpandMemberDefinitions
			};
		}

		public override void DecompileMethod(Decompiler.Metadata.MethodDefinition method, ITextOutput output, DecompilationOptions options)
		{
			var dis = CreateDisassembler(output, options);
			dis.DisassembleMethod(method);
		}
		
		public override void DecompileField(Decompiler.Metadata.FieldDefinition field, ITextOutput output, DecompilationOptions options)
		{
			var dis = CreateDisassembler(output, options);
			dis.DisassembleField(field);
		}
		
		public override void DecompileProperty(Decompiler.Metadata.PropertyDefinition property, ITextOutput output, DecompilationOptions options)
		{
			var dis = CreateDisassembler(output, options);
			dis.DisassembleProperty(property);

			var accessors = property.This().GetAccessors();

			if (!accessors.Getter.IsNil) {
				output.WriteLine();
				dis.DisassembleMethod(new Decompiler.Metadata.MethodDefinition(property.Module, accessors.Getter));
			}
			if (!accessors.Setter.IsNil) {
				output.WriteLine();
				dis.DisassembleMethod(new Decompiler.Metadata.MethodDefinition(property.Module, accessors.Setter));
			}
			/*foreach (var m in property.OtherMethods) {
				output.WriteLine();
				dis.DisassembleMethod(m);
			}*/
		}
		
		public override void DecompileEvent(Decompiler.Metadata.EventDefinition ev, ITextOutput output, DecompilationOptions options)
		{
			var dis = CreateDisassembler(output, options);
			dis.DisassembleEvent(ev);
			var accessors = ev.This().GetAccessors();
			if (!accessors.Adder.IsNil) {
				output.WriteLine();
				dis.DisassembleMethod(new Decompiler.Metadata.MethodDefinition(ev.Module, accessors.Adder));
			}
			if (!accessors.Remover.IsNil) {
				output.WriteLine();
				dis.DisassembleMethod(new Decompiler.Metadata.MethodDefinition(ev.Module, accessors.Remover));
			}
			if (!accessors.Raiser.IsNil) {
				output.WriteLine();
				dis.DisassembleMethod(new Decompiler.Metadata.MethodDefinition(ev.Module, accessors.Raiser));
			}
			/*foreach (var m in ev.OtherMethods) {
				output.WriteLine();
				dis.DisassembleMethod(m);
			}*/
		}
		
		public override void DecompileType(Decompiler.Metadata.TypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			var dis = CreateDisassembler(output, options);
			dis.DisassembleType(type);
		}
		
		public override void DecompileNamespace(string nameSpace, IEnumerable<Decompiler.Metadata.TypeDefinition> types, ITextOutput output, DecompilationOptions options)
		{
			var dis = CreateDisassembler(output, options);
			dis.DisassembleNamespace(nameSpace, types);
		}
		
		public override void DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			output.WriteLine("// " + assembly.FileName);
			output.WriteLine();
			var module = assembly.GetPEFileOrNull();
			var metadata = module.GetMetadataReader();
			var dis = CreateDisassembler(output, options);
			if (options.FullDecompilation)
				dis.WriteAssemblyReferences(metadata);
			if (metadata.IsAssembly)
				dis.WriteAssemblyHeader(module);
			output.WriteLine();
			dis.WriteModuleHeader(module);
			if (options.FullDecompilation) {
				output.WriteLine();
				output.WriteLine();
				dis.WriteModuleContents(module);
			}
		}

		public override string TypeDefinitionToString(Decompiler.Metadata.TypeDefinition type, bool includeNamespace)
		{
			PlainTextOutput output = new PlainTextOutput();
			type.WriteTo(output, includeNamespace ? ILNameSyntax.TypeName : ILNameSyntax.ShortTypeName);
			return output.ToString();
		}
	}
}
