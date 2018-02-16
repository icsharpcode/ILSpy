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
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

using static System.Reflection.Metadata.PEReaderExtensions;
using SRM = System.Reflection.Metadata;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Base class for language-specific decompiler implementations.
	/// </summary>
	public abstract class Language
	{
		/// <summary>
		/// Gets the name of the language (as shown in the UI)
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the file extension used by source code files in this language.
		/// </summary>
		public abstract string FileExtension { get; }

		public virtual string ProjectFileExtension
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the syntax highlighting used for this language.
		/// </summary>
		public virtual ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition SyntaxHighlighting
		{
			get
			{
				return ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension(this.FileExtension);
			}
		}

		public virtual void DecompileMethod(MethodDefinition method, ITextOutput output, DecompilationOptions options)
		{
			var metadata = method.Module.GetMetadataReader();
			var methodDefinition = metadata.GetMethodDefinition(method.Handle);
			WriteCommentLine(output, TypeToString(new Entity(method.Module, methodDefinition.GetDeclaringType()), true) + "." + metadata.GetString(methodDefinition.Name));
		}

		public virtual void DecompileProperty(PropertyDefinition property, ITextOutput output, DecompilationOptions options)
		{
			var metadata = property.Module.GetMetadataReader();
			var propertyDefinition = metadata.GetPropertyDefinition(property.Handle);
			var declaringType = metadata.GetMethodDefinition(propertyDefinition.GetAccessors().GetAny()).GetDeclaringType();
			WriteCommentLine(output, TypeToString(new Entity(property.Module, declaringType), true) + "." + metadata.GetString(propertyDefinition.Name));
		}

		public virtual void DecompileField(FieldDefinition field, ITextOutput output, DecompilationOptions options)
		{
			var metadata = field.Module.GetMetadataReader();
			var fieldDefinition = metadata.GetFieldDefinition(field.Handle);
			WriteCommentLine(output, TypeToString(new Entity(field.Module, fieldDefinition.GetDeclaringType()), true) + "." + metadata.GetString(fieldDefinition.Name));
		}

		public virtual void DecompileEvent(EventDefinition ev, ITextOutput output, DecompilationOptions options)
		{
			var metadata = ev.Module.GetMetadataReader();
			var eventDefinition = metadata.GetEventDefinition(ev.Handle);
			var declaringType = metadata.GetMethodDefinition(eventDefinition.GetAccessors().GetAny()).GetDeclaringType();
			WriteCommentLine(output, TypeToString(new Entity(ev.Module, declaringType), true) + "." + metadata.GetString(eventDefinition.Name));
		}

		public virtual void DecompileType(TypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(type, true));
		}

		public virtual void DecompileNamespace(string nameSpace, IEnumerable<TypeDefinition> types, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, nameSpace);
		}

		public virtual void DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, assembly.FileName);
			var asm = assembly.GetPEFileOrNull();
			if (asm == null) return;
			var reader = asm.GetMetadataReader();
			if (reader.IsAssembly) {
				var name = reader.GetAssemblyDefinition();
				if ((name.Flags & System.Reflection.AssemblyFlags.WindowsRuntime) != 0) {
					WriteCommentLine(output, name.Name + " [WinRT]");
				} else {
					WriteCommentLine(output, reader.GetFullAssemblyName());
				}
			} else {
				WriteCommentLine(output, reader.GetString(reader.GetModuleDefinition().Name));
			}
		}

		public virtual void WriteCommentLine(ITextOutput output, string comment)
		{
			output.WriteLine("// " + comment);
		}

		/// <summary>
		/// Converts a type reference into a string. This method is used by the member tree node for parameter and return types.
		/// </summary>
		public virtual string TypeToString(Entity type, bool includeNamespace, SRM.CustomAttributeHandleCollection typeAttributes = default(SRM.CustomAttributeHandleCollection))
		{
			var metadata = type.Module.GetMetadataReader();
			var fullName = type.Handle.GetFullTypeName(metadata);
			if (includeNamespace)
				return fullName.ToString();
			else
				return fullName.Name;
		}

		public virtual SRM.ISignatureTypeProvider<string, GenericContext> CreateSignatureTypeProvider(bool includeNamespace)
		{
			return new ILSignatureProvider(includeNamespace);
		}

		/// <summary>
		/// Converts a member signature to a string.
		/// This is used for displaying the tooltip on a member reference.
		/// </summary>
		public virtual string GetTooltip(Entity entity)
		{
			var metadata = entity.Module.GetMetadataReader();
			switch (entity.Handle.Kind) {
				case SRM.HandleKind.TypeReference:
				case SRM.HandleKind.TypeDefinition:
				case SRM.HandleKind.TypeSpecification:
					return entity.Handle.GetFullTypeName(metadata).ToString();
				case SRM.HandleKind.FieldDefinition:
					var fieldDefinition = metadata.GetFieldDefinition((SRM.FieldDefinitionHandle)entity.Handle);
					string fieldType = fieldDefinition.DecodeSignature(CreateSignatureTypeProvider(false), new GenericContext(fieldDefinition.GetDeclaringType(), entity.Module));
					return fieldType + " " + fieldDefinition.GetDeclaringType().GetFullTypeName(metadata) +  "." + metadata.GetString(fieldDefinition.Name);
				case SRM.HandleKind.MethodDefinition:
					return TreeNodes.MethodTreeNode.GetText(entity, this).ToString();
				case SRM.HandleKind.EventDefinition:
					return TreeNodes.EventTreeNode.GetText(entity, this).ToString();
				case SRM.HandleKind.PropertyDefinition:
					return TreeNodes.PropertyTreeNode.GetText(entity, this).ToString();
				default:
					throw new NotSupportedException();
			}
		}

		public virtual string FormatFieldName(FieldDefinition field)
		{
			if (field.Handle.IsNil)
				throw new ArgumentNullException(nameof(field));
			var metadata = field.Module.GetMetadataReader();
			return metadata.GetString(metadata.GetFieldDefinition(field.Handle).Name);
		}

		public virtual string FormatPropertyName(PropertyDefinition property, bool? isIndexer = null)
		{
			if (property.Handle.IsNil)
				throw new ArgumentNullException(nameof(property));
			var metadata = property.Module.GetMetadataReader();
			return metadata.GetString(metadata.GetPropertyDefinition(property.Handle).Name);
		}

		public virtual string FormatMethodName(MethodDefinition method)
		{
			if (method.Handle.IsNil)
				throw new ArgumentNullException(nameof(method));
			var metadata = method.Module.GetMetadataReader();
			return metadata.GetString(metadata.GetMethodDefinition(method.Handle).Name);
		}

		public virtual string FormatEventName(EventDefinition @event)
		{
			if (@event.Handle.IsNil)
				throw new ArgumentNullException(nameof(@event));
			var metadata = @event.Module.GetMetadataReader();
			return metadata.GetString(metadata.GetEventDefinition(@event.Handle).Name);
		}

		public virtual string FormatTypeName(TypeDefinition type)
		{
			if (type.Handle.IsNil)
				throw new ArgumentNullException(nameof(type));
			var metadata = type.Module.GetMetadataReader();
			return metadata.GetString(metadata.GetTypeDefinition(type.Handle).Name);
		}

		/// <summary>
		/// Used for WPF keyboard navigation.
		/// </summary>
		public override string ToString()
		{
			return Name;
		}

		public virtual bool ShowMember(IMetadataEntity member)
		{
			return true;
		}

		/// <summary>
		/// Used by the analyzer to map compiler generated code back to the original code's location
		/// </summary>
		public virtual IMetadataEntity GetOriginalCodeLocation(IMetadataEntity member)
		{
			return member;
		}

		public static string GetPlatformDisplayName(PEFile module)
		{
			var architecture = module.Reader.PEHeaders.CoffHeader.Machine;
			var flags = module.Reader.PEHeaders.CorHeader.Flags;
			switch (architecture) {
				case Machine.I386:
					if ((flags & CorFlags.Prefers32Bit) != 0)
						return "AnyCPU (32-bit preferred)";
					else if ((flags & CorFlags.Requires32Bit) != 0)
						return "x86";
					else
						return "AnyCPU (64-bit preferred)";
				case Machine.Amd64:
					return "x64";
				case Machine.IA64:
					return "Itanium";
				default:
					return architecture.ToString();
			}
		}

		public static string GetRuntimeDisplayName(PEFile module)
		{
			string version = module.GetMetadataReader().MetadataVersion;
			switch (version[1]) {
				case '1':
					if (version[3] == 1)
						return ".NET 1.1";
					else
						return ".NET 1.0";
				case '2':
					return ".NET 2.0";
				case '4':
					return ".NET 4.0";
			}
			return null;
		}
	}

	class ILSignatureProvider : SRM.ISignatureTypeProvider<string, GenericContext>
	{
		bool includeNamespace;

		public ILSignatureProvider(bool includeNamespace)
		{
			this.includeNamespace = includeNamespace;
		}

		public string GetArrayType(string elementType, SRM.ArrayShape shape)
		{
			string printedShape = "";
			for (int i = 0; i < shape.Rank; i++) {
				if (i > 0)
					printedShape += ", ";
				if (i < shape.LowerBounds.Length || i < shape.Sizes.Length) {
					int lower = 0;
					if (i < shape.LowerBounds.Length) {
						lower = shape.LowerBounds[i];
						printedShape += lower.ToString();
					}
					printedShape += "...";
					if (i < shape.Sizes.Length)
						printedShape += (lower + shape.Sizes[i] - 1).ToString();
				}
			}
			return $"{elementType}[{printedShape}]";
		}

		public string GetByReferenceType(string elementType)
		{
			return elementType + "&";
		}

		public string GetFunctionPointerType(SRM.MethodSignature<string> signature)
		{
			throw new NotImplementedException();
		}

		public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments)
		{
			return genericType + "<" + string.Join(", ", typeArguments) + ">";
		}

		public string GetGenericMethodParameter(GenericContext genericContext, int index)
		{
			return "!!" + genericContext.GetGenericMethodTypeParameterName(index);
		}

		public string GetGenericTypeParameter(GenericContext genericContext, int index)
		{
			return "!" + genericContext.GetGenericTypeParameterName(index);
		}

		public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired)
		{
			throw new NotImplementedException();
		}

		public string GetPinnedType(string elementType)
		{
			throw new NotImplementedException();
		}

		public string GetPointerType(string elementType)
		{
			return elementType + "*";
		}

		public string GetPrimitiveType(SRM.PrimitiveTypeCode typeCode)
		{
			switch (typeCode) {
				case SRM.PrimitiveTypeCode.Boolean:
					return "bool";
				case SRM.PrimitiveTypeCode.Byte:
					return "uint8";
				case SRM.PrimitiveTypeCode.SByte:
					return "int8";
				case SRM.PrimitiveTypeCode.Char:
					return "char";
				case SRM.PrimitiveTypeCode.Int16:
					return "int16";
				case SRM.PrimitiveTypeCode.UInt16:
					return "uint16";
				case SRM.PrimitiveTypeCode.Int32:
					return "int32";
				case SRM.PrimitiveTypeCode.UInt32:
					return "uint32";
				case SRM.PrimitiveTypeCode.Int64:
					return "int64";
				case SRM.PrimitiveTypeCode.UInt64:
					return "uint64";
				case SRM.PrimitiveTypeCode.Single:
					return "float32";
				case SRM.PrimitiveTypeCode.Double:
					return "float64";
				case SRM.PrimitiveTypeCode.IntPtr:
					return "native int";
				case SRM.PrimitiveTypeCode.UIntPtr:
					break;
				case SRM.PrimitiveTypeCode.Object:
					return "object";
				case SRM.PrimitiveTypeCode.String:
					return "string";
				case SRM.PrimitiveTypeCode.TypedReference:
					break;
				case SRM.PrimitiveTypeCode.Void:
					return "void";
				default:
					break;
			}
			throw new NotImplementedException();
		}

		public string GetSZArrayType(string elementType)
		{
			return elementType + "[]";
		}

		public string GetTypeFromDefinition(SRM.MetadataReader reader, SRM.TypeDefinitionHandle handle, byte rawTypeKind)
		{
			if (!includeNamespace) {
				return Decompiler.Disassembler.DisassemblerHelpers.Escape(handle.GetFullTypeName(reader).Name);
			}

			return handle.GetFullTypeName(reader).ToILNameString();
		}

		public string GetTypeFromReference(SRM.MetadataReader reader, SRM.TypeReferenceHandle handle, byte rawTypeKind)
		{
			if (!includeNamespace) {
				return Decompiler.Disassembler.DisassemblerHelpers.Escape(handle.GetFullTypeName(reader).Name);
			}

			return handle.GetFullTypeName(reader).ToILNameString();
		}

		public string GetTypeFromSpecification(SRM.MetadataReader reader, GenericContext genericContext, SRM.TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
		}
	}
}
