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
		
		protected virtual ReflectionDisassembler CreateDisassembler(ITextOutput output, PEReader reader, DecompilationOptions options)
		{
			return new ReflectionDisassembler(output, reader, options.CancellationToken) {
				DetectControlStructure = detectControlStructure,
				ShowSequencePoints = options.DecompilerSettings.ShowDebugInfo
			};
		}

		public override void DecompileMethod(Mono.Cecil.MethodDefinition method, ITextOutput output, DecompilationOptions options)
		{
			using (var reader = new PEReader(new FileStream(method.Module.FileName, FileMode.Open, FileAccess.Read))) {
				var dis = CreateDisassembler(output, reader, options);
				dis.DisassembleMethod(MetadataTokens.MethodDefinitionHandle(method.MetadataToken.ToInt32()));
			}
		}
		
		public override void DecompileField(Mono.Cecil.FieldDefinition field, ITextOutput output, DecompilationOptions options)
		{
			using (var reader = new PEReader(new FileStream(field.Module.FileName, FileMode.Open, FileAccess.Read))) {
				var dis = CreateDisassembler(output, reader, options);
				dis.DisassembleField(MetadataTokens.FieldDefinitionHandle(field.MetadataToken.ToInt32()));
			}
		}
		
		public override void DecompileProperty(Mono.Cecil.PropertyDefinition property, ITextOutput output, DecompilationOptions options)
		{
			using (var reader = new PEReader(new FileStream(property.Module.FileName, FileMode.Open, FileAccess.Read))) {
				var dis = CreateDisassembler(output, reader, options);
				dis.DisassembleProperty(MetadataTokens.PropertyDefinitionHandle(property.MetadataToken.ToInt32()));

				if (property.GetMethod != null) {
					output.WriteLine();
					dis.DisassembleMethod(MetadataTokens.MethodDefinitionHandle(property.GetMethod.MetadataToken.ToInt32()));
				}
				if (property.SetMethod != null) {
					output.WriteLine();
					dis.DisassembleMethod(MetadataTokens.MethodDefinitionHandle(property.SetMethod.MetadataToken.ToInt32()));
				}
				foreach (var m in property.OtherMethods) {
					output.WriteLine();
					dis.DisassembleMethod(MetadataTokens.MethodDefinitionHandle(m.MetadataToken.ToInt32()));
				}
			}
		}
		
		public override void DecompileEvent(Mono.Cecil.EventDefinition ev, ITextOutput output, DecompilationOptions options)
		{
			using (var reader = new PEReader(new FileStream(ev.Module.FileName, FileMode.Open, FileAccess.Read))) {
				var dis = CreateDisassembler(output, reader, options);
				dis.DisassembleEvent(MetadataTokens.EventDefinitionHandle(ev.MetadataToken.ToInt32()));
				if (ev.AddMethod != null) {
					output.WriteLine();
					dis.DisassembleMethod(MetadataTokens.MethodDefinitionHandle(ev.AddMethod.MetadataToken.ToInt32()));
				}
				if (ev.RemoveMethod != null) {
					output.WriteLine();
					dis.DisassembleMethod(MetadataTokens.MethodDefinitionHandle(ev.RemoveMethod.MetadataToken.ToInt32()));
				}
				foreach (var m in ev.OtherMethods) {
					output.WriteLine();
					dis.DisassembleMethod(MetadataTokens.MethodDefinitionHandle(m.MetadataToken.ToInt32()));
				}
			}
		}
		
		public override void DecompileType(Mono.Cecil.TypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			using (var reader = new PEReader(new FileStream(type.Module.FileName, FileMode.Open, FileAccess.Read))) {
				var dis = CreateDisassembler(output, reader, options);
				dis.DisassembleType(MetadataTokens.TypeDefinitionHandle(type.MetadataToken.ToInt32()));
			}
		}
		
		public override void DecompileNamespace(string nameSpace, IEnumerable<Mono.Cecil.TypeDefinition> types, ITextOutput output, DecompilationOptions options)
		{
			if (!types.Any())
				return;
			using (var reader = new PEReader(new FileStream(types.First().Module.FileName, FileMode.Open, FileAccess.Read))) {
				var dis = CreateDisassembler(output, reader, options);
				dis.DisassembleNamespace(nameSpace, types.Select(t => MetadataTokens.TypeDefinitionHandle(t.MetadataToken.ToInt32())));
			}
		}
		
		public override void DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			output.WriteLine("// " + assembly.FileName);
			output.WriteLine();
			using (var reader = new PEReader(new FileStream(assembly.FileName, FileMode.Open, FileAccess.Read))) {
				var dis = CreateDisassembler(output, reader, options);
				var module = assembly.GetModuleDefinitionAsync().Result;
				if (options.FullDecompilation)
					dis.WriteAssemblyReferences();
				if (module.Assembly != null)
					dis.WriteAssemblyHeader();
				output.WriteLine();
				dis.WriteModuleHeader();
				if (options.FullDecompilation) {
					output.WriteLine();
					output.WriteLine();
					dis.WriteModuleContents(reader.GetMetadataReader().GetModuleDefinition());
				}
			}
		}
		
		public override string TypeToString(Mono.Cecil.TypeReference type, bool includeNamespace, Mono.Cecil.ICustomAttributeProvider typeAttributes = null)
		{
			PlainTextOutput output = new PlainTextOutput();
			type.WriteTo(output, includeNamespace ? ILNameSyntax.TypeName : ILNameSyntax.ShortTypeName);
			return output.ToString();
		}
	}
}
