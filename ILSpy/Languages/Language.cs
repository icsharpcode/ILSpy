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
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

using static System.Reflection.Metadata.PEReaderExtensions;
using SRM = System.Reflection.Metadata;

namespace ICSharpCode.ILSpy
{
	public struct LanguageVersion : IEquatable<LanguageVersion>
	{
		public string Version { get; }
		public string DisplayName { get; }

		public LanguageVersion(string version, string name = null)
		{
			this.Version = version ?? "";
			this.DisplayName = name ?? version.ToString();
		}

		public bool Equals(LanguageVersion other)
		{
			return other.Version == this.Version && other.DisplayName == this.DisplayName;
		}

		public override bool Equals(object obj)
		{
			return obj is LanguageVersion version && Equals(version);
		}

		public override int GetHashCode()
		{
			return unchecked(982451629 * Version.GetHashCode() + 982451653 * DisplayName.GetHashCode());
		}

		public static bool operator ==(LanguageVersion lhs, LanguageVersion rhs) => lhs.Equals(rhs);
		public static bool operator !=(LanguageVersion lhs, LanguageVersion rhs) => !lhs.Equals(rhs);
	}

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

		public virtual IReadOnlyList<LanguageVersion> LanguageVersions {
			get { return EmptyList<LanguageVersion>.Instance; }
		}

		public bool HasLanguageVersions => LanguageVersions.Count > 0;

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
			var metadata = method.Module.Metadata;
			var methodDefinition = metadata.GetMethodDefinition(method.Handle);
			WriteCommentLine(output, TypeToString(new TypeDefinition(method.Module, methodDefinition.GetDeclaringType()), includeNamespace: true) + "." + metadata.GetString(methodDefinition.Name));
		}

		public virtual void DecompileProperty(PropertyDefinition property, ITextOutput output, DecompilationOptions options)
		{
			var metadata = property.Module.Metadata;
			var propertyDefinition = metadata.GetPropertyDefinition(property.Handle);
			var declaringType = metadata.GetMethodDefinition(propertyDefinition.GetAccessors().GetAny()).GetDeclaringType();
			WriteCommentLine(output, TypeToString(new TypeDefinition(property.Module, declaringType), includeNamespace: true) + "." + metadata.GetString(propertyDefinition.Name));
		}

		public virtual void DecompileField(FieldDefinition field, ITextOutput output, DecompilationOptions options)
		{
			var metadata = field.Module.Metadata;
			var fieldDefinition = metadata.GetFieldDefinition(field.Handle);
			WriteCommentLine(output, TypeToString(new TypeDefinition(field.Module, fieldDefinition.GetDeclaringType()), includeNamespace: true) + "." + metadata.GetString(fieldDefinition.Name));
		}

		public virtual void DecompileEvent(EventDefinition ev, ITextOutput output, DecompilationOptions options)
		{
			var metadata = ev.Module.Metadata;
			var eventDefinition = metadata.GetEventDefinition(ev.Handle);
			var declaringType = metadata.GetMethodDefinition(eventDefinition.GetAccessors().GetAny()).GetDeclaringType();
			WriteCommentLine(output, TypeToString(new TypeDefinition(ev.Module, declaringType), includeNamespace: true) + "." + metadata.GetString(eventDefinition.Name));
		}

		public virtual void DecompileType(TypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(type, includeNamespace: true));
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
			var metadata = asm.Metadata;
			if (metadata.IsAssembly) {
				var name = metadata.GetAssemblyDefinition();
				if ((name.Flags & System.Reflection.AssemblyFlags.WindowsRuntime) != 0) {
					WriteCommentLine(output, name.Name + " [WinRT]");
				} else {
					WriteCommentLine(output, metadata.GetFullAssemblyName());
				}
			} else {
				WriteCommentLine(output, metadata.GetString(metadata.GetModuleDefinition().Name));
			}
		}

		public virtual void WriteCommentLine(ITextOutput output, string comment)
		{
			output.WriteLine("// " + comment);
		}

		/// <summary>
		/// Converts a type definition, reference or specification into a string. This method is used by the type tree nodes and search results.
		/// </summary>
		public virtual string TypeToString(Entity type, GenericContext genericContext = null, bool includeNamespace = true)
		{
			var provider = includeNamespace ? ILSignatureProvider.WithNamespace : ILSignatureProvider.WithoutNamespace;
			var metadata = type.Module.Metadata;
			switch (type.Handle.Kind) {
				case SRM.HandleKind.TypeReference:
					return provider.GetTypeFromReference(metadata, (SRM.TypeReferenceHandle)type.Handle, 0);
				case SRM.HandleKind.TypeDefinition:
					var td = metadata.GetTypeDefinition((SRM.TypeDefinitionHandle)type.Handle);
					var genericParams = td.GetGenericParameters();

					var buffer = new System.Text.StringBuilder();

					var name = td.GetFullTypeName(metadata);

					if (includeNamespace)
						buffer.Append(name.ToString());
					else
						buffer.Append(name.Name);

					if (genericParams.Count > 0) {
						buffer.Append('<');
						int i = 0;
						foreach (var h in genericParams) {
							var gp = metadata.GetGenericParameter(h);
							if (i > 0)
								buffer.Append(", ");
							buffer.Append(metadata.GetString(gp.Name));
							i++;
						}
						buffer.Append('>');
					}

					return buffer.ToString();
				case SRM.HandleKind.TypeSpecification:
					return provider.GetTypeFromSpecification(metadata, genericContext ?? GenericContext.Empty, (SRM.TypeSpecificationHandle)type.Handle, 0);
				default:
					throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Converts a member signature to a string.
		/// This is used for displaying the tooltip on a member reference.
		/// </summary>
		public virtual string GetTooltip(Entity entity)
		{
			var metadata = entity.Module.Metadata;
			switch (entity.Handle.Kind) {
				case SRM.HandleKind.TypeReference:
				case SRM.HandleKind.TypeDefinition:
				case SRM.HandleKind.TypeSpecification:
					return entity.Handle.GetFullTypeName(metadata).ToString();
				case SRM.HandleKind.FieldDefinition:
					var fieldDefinition = metadata.GetFieldDefinition((SRM.FieldDefinitionHandle)entity.Handle);
					string fieldType = fieldDefinition.DecodeSignature(ILSignatureProvider.WithoutNamespace, new GenericContext(fieldDefinition.GetDeclaringType(), entity.Module));
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

		public virtual string FieldToString(FieldDefinition field, bool includeTypeName, bool includeNamespace)
		{
			if (field.Handle.IsNil)
				throw new ArgumentNullException(nameof(field));
			var metadata = field.Module.Metadata;
			var fd = metadata.GetFieldDefinition(field.Handle);
			string fieldType = fd.DecodeSignature(ILSignatureProvider.WithoutNamespace, new GenericContext(fd.GetDeclaringType(), field.Module));
			string simple = metadata.GetString(fd.Name) + " : " + fieldType;
			if (!includeTypeName)
				return simple;
			var typeName = fd.GetDeclaringType().GetFullTypeName(metadata);
			if (!includeNamespace)
				return typeName.Name + "." + simple;
			return typeName + "." + simple;
		}

		public virtual string PropertyToString(PropertyDefinition property, bool includeTypeName, bool includeNamespace, bool? isIndexer = null)
		{
			if (property.Handle.IsNil)
				throw new ArgumentNullException(nameof(property));
			var metadata = property.Module.Metadata;
			var pd = metadata.GetPropertyDefinition(property.Handle);
			var declaringType = metadata.GetMethodDefinition(pd.GetAccessors().GetAny()).GetDeclaringType();
			var signature = pd.DecodeSignature(!includeNamespace ? ILSignatureProvider.WithoutNamespace : ILSignatureProvider.WithNamespace, new GenericContext(declaringType, property.Module));
			string simple = metadata.GetString(metadata.GetPropertyDefinition(property.Handle).Name) + " : " + signature.ReturnType;
			if (!includeTypeName)
				return simple;
			var typeName = declaringType.GetFullTypeName(metadata);
			if (!includeNamespace)
				return typeName.Name + "." + simple;
			return typeName + "." + simple;
		}

		public virtual string MethodToString(MethodDefinition method, bool includeTypeName, bool includeNamespace)
		{
			if (method.IsNil)
				throw new ArgumentNullException(nameof(method));
			var metadata = method.Module.Metadata;
			var md = metadata.GetMethodDefinition(method.Handle);
			string name;
			if (includeTypeName) {
				if (includeNamespace) {
					name = md.GetDeclaringType().GetFullTypeName(metadata) + ".";
				} else {
					name = md.GetDeclaringType().GetFullTypeName(metadata).Name + ".";
				}
				name += metadata.GetString(md.Name);
			} else {
				name = metadata.GetString(md.Name);
			}
			var signature = md.DecodeSignature(includeNamespace ? ILSignatureProvider.WithNamespace : ILSignatureProvider.WithoutNamespace, new GenericContext(method));

			int i = 0;
			var buffer = new System.Text.StringBuilder(name);
			var genericParams = md.GetGenericParameters();
			if (genericParams.Count > 0) {
				buffer.Append('<');
				foreach (var h in genericParams) {
					var gp = metadata.GetGenericParameter(h);
					if (i > 0)
						buffer.Append(", ");
					buffer.Append(metadata.GetString(gp.Name));
					i++;
				}
				buffer.Append('>');
			}
			buffer.Append('(');

			i = 0;
			var parameterHandles = md.GetParameters();
			if (signature.RequiredParameterCount > parameterHandles.Count) {
				foreach (var type in signature.ParameterTypes) {
					if (i > 0)
						buffer.Append(", ");
					buffer.Append(signature.ParameterTypes[i]);
					i++;
				}
			} else {
				foreach (var h in parameterHandles) {
					var p = metadata.GetParameter(h);
					if (p.SequenceNumber > 0 && i < signature.ParameterTypes.Length) {
						if (i > 0)
							buffer.Append(", ");
						buffer.Append(signature.ParameterTypes[i]);
						i++;
					}
				}
			}
			if (signature.Header.CallingConvention == SRM.SignatureCallingConvention.VarArgs) {
				if (signature.ParameterTypes.Length > 0)
					buffer.Append(", ");
				buffer.Append("...");
			}
			buffer.Append(')');
			buffer.Append(" : ");
			buffer.Append(signature.ReturnType);
			return buffer.ToString();
		}

		public virtual string EventToString(EventDefinition @event, bool includeTypeName, bool includeNamespace)
		{
			if (@event.IsNil)
				throw new ArgumentNullException(nameof(@event));
			var metadata = @event.Module.Metadata;
			var ed = metadata.GetEventDefinition(@event.Handle);
			var accessorHandle = ed.GetAccessors().GetAny();
			var signature = ed.DecodeSignature(metadata, includeNamespace ? ILSignatureProvider.WithNamespace : ILSignatureProvider.WithoutNamespace, new GenericContext(accessorHandle, @event.Module));
			var buffer = new System.Text.StringBuilder();
			buffer.Append(metadata.GetString(ed.Name));
			buffer.Append(" : ");
			buffer.Append(signature);
			return buffer.ToString();
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

		public virtual CodeMappingInfo GetCodeMappingInfo(PEFile module, SRM.EntityHandle member)
		{
			var parts = new Dictionary<SRM.MethodDefinitionHandle, SRM.MethodDefinitionHandle[]>();
			var locations = new Dictionary<SRM.EntityHandle, SRM.MethodDefinitionHandle>();

			var declaringType = member.GetDeclaringType(module.Metadata);

			if (declaringType.IsNil && member.Kind == SRM.HandleKind.TypeDefinition) {
				declaringType = (SRM.TypeDefinitionHandle)member;
			}

			return new CodeMappingInfo(this, module, declaringType);
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
			return module.Metadata.MetadataVersion;
		}
	}
}
