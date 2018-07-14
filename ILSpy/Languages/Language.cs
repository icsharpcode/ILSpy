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
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

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
	/// <remarks>
	/// Implementations of this class must be thread-safe.
	/// </remarks>
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

		public virtual void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(method.DeclaringTypeDefinition, includeNamespace: true) + "." + method.Name);
		}

		public virtual void DecompileProperty(IProperty property, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(property.DeclaringTypeDefinition, includeNamespace: true) + "." + property.Name);
		}

		public virtual void DecompileField(IField field, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(field.DeclaringTypeDefinition, includeNamespace: true) + "." + field.Name);
		}

		public virtual void DecompileEvent(IEvent @event, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(@event.DeclaringTypeDefinition, includeNamespace: true) + "." + @event.Name);
		}

		public virtual void DecompileType(ITypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(type, includeNamespace: true));
		}

		public virtual void DecompileNamespace(string nameSpace, IEnumerable<ITypeDefinition> types, ITextOutput output, DecompilationOptions options)
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
		/// Converts a type definition, reference or specification into a string. This method is used by tree nodes and search results.
		/// </summary>
		public virtual string TypeToString(IType type, bool includeNamespace)
		{
			if (includeNamespace)
				return type.ReflectionName;
			else {
				int index = type.ReflectionName.LastIndexOf('.');
				if (index > 0) {
					return type.ReflectionName.Substring(index + 1);
				}
				return type.ReflectionName;
			}
		}

		/// <summary>
		/// Converts a member signature to a string.
		/// This is used for displaying the tooltip on a member reference.
		/// </summary>
		public virtual string GetTooltip(IEntity entity)
		{
			return GetDisplayName(entity, true, true);
		}

		public virtual string FieldToString(IField field, bool includeTypeName, bool includeNamespace)
		{
			if (field == null)
				throw new ArgumentNullException(nameof(field));
			return GetDisplayName(field, includeTypeName, includeNamespace) + " : " + TypeToString(field.ReturnType, includeNamespace);
		}

		public virtual string PropertyToString(IProperty property, bool includeTypeName, bool includeNamespace)
		{
			if (property == null)
				throw new ArgumentNullException(nameof(property));
			return GetDisplayName(property, includeTypeName, includeNamespace) + " : " + TypeToString(property.ReturnType, includeNamespace);
		}

		public virtual string MethodToString(IMethod method, bool includeTypeName, bool includeNamespace)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			int i = 0;
			var buffer = new System.Text.StringBuilder();
			buffer.Append(GetDisplayName(method, includeTypeName, includeNamespace));
			var typeParameters = method.TypeParameters;
			if (typeParameters.Count > 0) {
				buffer.Append('<');
				foreach (var tp in typeParameters) {
					if (i > 0)
						buffer.Append(", ");
					buffer.Append(tp.Name);
					i++;
				}
				buffer.Append('>');
			}
			buffer.Append('(');

			i = 0;
			var parameters = method.Parameters;
			foreach (var param in parameters) {
				if (i > 0)
					buffer.Append(", ");
				buffer.Append(TypeToString(param.Type, includeNamespace));
				i++;
			}
			buffer.Append(')');
			buffer.Append(" : ");
			buffer.Append(TypeToString(method.ReturnType, includeNamespace));
			return buffer.ToString();
		}

		public virtual string EventToString(IEvent @event, bool includeTypeName, bool includeNamespace)
		{
			if (@event == null)
				throw new ArgumentNullException(nameof(@event));
			var buffer = new System.Text.StringBuilder();
			buffer.Append(GetDisplayName(@event, includeTypeName, includeNamespace));
			buffer.Append(" : ");
			buffer.Append(TypeToString(@event.ReturnType, includeNamespace));
			return buffer.ToString();
		}

		protected string GetDisplayName(IEntity entity, bool includeTypeName, bool includeNamespace)
		{
			if (includeTypeName && entity.DeclaringTypeDefinition != null) {
				string name;
				if (includeNamespace) {
					name = entity.DeclaringTypeDefinition.FullName;
				} else {
					name = entity.DeclaringTypeDefinition.Name;
				}
				return name + "." + entity.Name;
			} else {
				if (includeNamespace)
					return entity.FullName;
				return entity.Name;
			}
		}

		/// <summary>
		/// Used for WPF keyboard navigation.
		/// </summary>
		public override string ToString()
		{
			return Name;
		}

		public virtual bool ShowMember(IEntity member)
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

			return new CodeMappingInfo(module, declaringType);
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
