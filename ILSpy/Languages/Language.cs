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
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
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

		public virtual ProjectId DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, assembly.FileName);
			var asm = assembly.GetPEFileOrNull();
			if (asm == null) return null;
			var metadata = asm.Metadata;
			if (metadata.IsAssembly) {
				var name = metadata.GetAssemblyDefinition();
				if ((name.Flags & System.Reflection.AssemblyFlags.WindowsRuntime) != 0) {
					WriteCommentLine(output, metadata.GetString(name.Name) + " [WinRT]");
				} else {
					WriteCommentLine(output, metadata.GetFullAssemblyName());
				}
			} else {
				WriteCommentLine(output, metadata.GetString(metadata.GetModuleDefinition().Name));
			}
			return null;
		}

		public virtual void WriteCommentLine(ITextOutput output, string comment)
		{
			output.WriteLine("// " + comment);
		}

		#region TypeToString
		/// <summary>
		/// Converts a type definition, reference or specification into a string. This method is used by tree nodes and search results.
		/// </summary>
		public virtual string TypeToString(IType type, bool includeNamespace)
		{
			var visitor = new TypeToStringVisitor(includeNamespace);
			type.AcceptVisitor(visitor);
			return visitor.ToString();
		}

		class TypeToStringVisitor : TypeVisitor
		{
			readonly bool includeNamespace;
			readonly StringBuilder builder;

			public override string ToString()
			{
				return builder.ToString();
			}

			public TypeToStringVisitor(bool includeNamespace)
			{
				this.includeNamespace = includeNamespace;
				this.builder = new StringBuilder();
			}

			public override IType VisitArrayType(ArrayType type)
			{
				base.VisitArrayType(type);
				builder.Append('[');
				builder.Append(',', type.Dimensions - 1);
				builder.Append(']');
				return type;
			}

			public override IType VisitByReferenceType(ByReferenceType type)
			{
				base.VisitByReferenceType(type);
				builder.Append('&');
				return type;
			}

			public override IType VisitModOpt(ModifiedType type)
			{
				type.ElementType.AcceptVisitor(this);
				builder.Append(" modopt(");
				type.Modifier.AcceptVisitor(this);
				builder.Append(")");
				return type;
			}

			public override IType VisitModReq(ModifiedType type)
			{
				type.ElementType.AcceptVisitor(this);
				builder.Append(" modreq(");
				type.Modifier.AcceptVisitor(this);
				builder.Append(")");
				return type;
			}

			public override IType VisitPointerType(PointerType type)
			{
				base.VisitPointerType(type);
				builder.Append('*');
				return type;
			}

			public override IType VisitTypeParameter(ITypeParameter type)
			{
				base.VisitTypeParameter(type);
				EscapeName(builder, type.Name);
				return type;
			}

			public override IType VisitParameterizedType(ParameterizedType type)
			{
				type.GenericType.AcceptVisitor(this);
				builder.Append('<');
				for (int i = 0; i < type.TypeArguments.Count; i++) {
					if (i > 0)
						builder.Append(',');
					type.TypeArguments[i].AcceptVisitor(this);
				}
				builder.Append('>');
				return type;
			}

			public override IType VisitTupleType(TupleType type)
			{
				type.UnderlyingType.AcceptVisitor(this);
				return type;
			}

			public override IType VisitOtherType(IType type)
			{
				WriteType(type);
				return type;
			}

			private void WriteType(IType type)
			{
				if (includeNamespace)
					EscapeName(builder, type.FullName);
				else
					EscapeName(builder, type.Name);
				if (type.TypeParameterCount > 0) {
					builder.Append('`');
					builder.Append(type.TypeParameterCount);
				}
			}

			public override IType VisitTypeDefinition(ITypeDefinition type)
			{
				switch (type.KnownTypeCode) {
					case KnownTypeCode.Object:
						builder.Append("object");
						break;
					case KnownTypeCode.Boolean:
						builder.Append("bool");
						break;
					case KnownTypeCode.Char:
						builder.Append("char");
						break;
					case KnownTypeCode.SByte:
						builder.Append("int8");
						break;
					case KnownTypeCode.Byte:
						builder.Append("uint8");
						break;
					case KnownTypeCode.Int16:
						builder.Append("int16");
						break;
					case KnownTypeCode.UInt16:
						builder.Append("uint16");
						break;
					case KnownTypeCode.Int32:
						builder.Append("int32");
						break;
					case KnownTypeCode.UInt32:
						builder.Append("uint32");
						break;
					case KnownTypeCode.Int64:
						builder.Append("int64");
						break;
					case KnownTypeCode.UInt64:
						builder.Append("uint64");
						break;
					case KnownTypeCode.Single:
						builder.Append("float32");
						break;
					case KnownTypeCode.Double:
						builder.Append("float64");
						break;
					case KnownTypeCode.String:
						builder.Append("string");
						break;
					case KnownTypeCode.Void:
						builder.Append("void");
						break;
					case KnownTypeCode.IntPtr:
						builder.Append("native int");
						break;
					case KnownTypeCode.UIntPtr:
						builder.Append("native uint");
						break;
					case KnownTypeCode.TypedReference:
						builder.Append("typedref");
						break;
					default:
						WriteType(type);
						break;
				}
				return type;
			}
		}
		#endregion

		/// <summary>
		/// Converts a member signature to a string.
		/// This is used for displaying the tooltip on a member reference.
		/// </summary>
		public virtual string GetTooltip(IEntity entity)
		{
			return GetDisplayName(entity, true, true, true);
		}

		public virtual string FieldToString(IField field, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			if (field == null)
				throw new ArgumentNullException(nameof(field));
			return GetDisplayName(field, includeDeclaringTypeName, includeNamespace, includeNamespaceOfDeclaringTypeName) + " : " + TypeToString(field.ReturnType, includeNamespace);
		}

		public virtual string PropertyToString(IProperty property, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			if (property == null)
				throw new ArgumentNullException(nameof(property));
			return GetDisplayName(property, includeDeclaringTypeName, includeNamespace, includeNamespaceOfDeclaringTypeName) + " : " + TypeToString(property.ReturnType, includeNamespace);
		}

		public virtual string MethodToString(IMethod method, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			int i = 0;
			var buffer = new StringBuilder();
			buffer.Append(GetDisplayName(method, includeDeclaringTypeName, includeNamespace, includeNamespaceOfDeclaringTypeName));
			var typeParameters = method.TypeParameters;
			if (typeParameters.Count > 0) {
				buffer.Append("``");
				buffer.Append(typeParameters.Count);
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
			if (!method.IsConstructor) {
				buffer.Append(" : ");
				buffer.Append(TypeToString(method.ReturnType, includeNamespace));
			}
			return buffer.ToString();
		}

		public virtual string EventToString(IEvent @event, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			if (@event == null)
				throw new ArgumentNullException(nameof(@event));
			var buffer = new StringBuilder();
			buffer.Append(GetDisplayName(@event, includeDeclaringTypeName, includeNamespace, includeNamespaceOfDeclaringTypeName));
			buffer.Append(" : ");
			buffer.Append(TypeToString(@event.ReturnType, includeNamespace));
			return buffer.ToString();
		}

		protected string GetDisplayName(IEntity entity, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			string entityName;
			if (entity is ITypeDefinition t && !t.MetadataToken.IsNil) {
				MetadataReader metadata = t.ParentModule.PEFile.Metadata;
				var typeDef = metadata.GetTypeDefinition((TypeDefinitionHandle)t.MetadataToken);
				entityName = EscapeName(metadata.GetString(typeDef.Name));
			} else {
				entityName = EscapeName(entity.Name);
			}
			if (includeNamespace || includeDeclaringTypeName) {
				if (entity.DeclaringTypeDefinition != null)
					return TypeToString(entity.DeclaringTypeDefinition, includeNamespaceOfDeclaringTypeName) + "." + entityName;
				return EscapeName(entity.Namespace) + "." + entityName;
			} else {
				return entityName;
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

		/// <summary>
		/// This should produce a string representation of the entity for search to match search strings against.
		/// </summary>
		public virtual string GetEntityName(PEFile module, EntityHandle handle, bool fullName)
		{
			MetadataReader metadata = module.Metadata;
			switch (handle.Kind) {
				case HandleKind.TypeDefinition:
					if (fullName)
						return EscapeName(((TypeDefinitionHandle)handle).GetFullTypeName(metadata).ToILNameString());
					var td = metadata.GetTypeDefinition((TypeDefinitionHandle)handle);
					return EscapeName(metadata.GetString(td.Name));
				case HandleKind.FieldDefinition:
					var fd = metadata.GetFieldDefinition((FieldDefinitionHandle)handle);
					var declaringType = fd.GetDeclaringType();
					if (fullName)
						return EscapeName(fd.GetDeclaringType().GetFullTypeName(metadata).ToILNameString() + "." + metadata.GetString(fd.Name));
					return EscapeName(metadata.GetString(fd.Name));
				case HandleKind.MethodDefinition:
					var md = metadata.GetMethodDefinition((MethodDefinitionHandle)handle);
					declaringType = md.GetDeclaringType();
					string methodName = metadata.GetString(md.Name);
					int genericParamCount = md.GetGenericParameters().Count;
					if (genericParamCount > 0)
						methodName += "``" + genericParamCount;
					if (fullName)
						return EscapeName(md.GetDeclaringType().GetFullTypeName(metadata).ToILNameString() + "." + methodName);
					return EscapeName(methodName);
				case HandleKind.EventDefinition:
					var ed = metadata.GetEventDefinition((EventDefinitionHandle)handle);
					declaringType = metadata.GetMethodDefinition(ed.GetAccessors().GetAny()).GetDeclaringType();
					if (fullName)
						return EscapeName(declaringType.GetFullTypeName(metadata).ToILNameString() + "." + metadata.GetString(ed.Name));
					return EscapeName(metadata.GetString(ed.Name));
				case HandleKind.PropertyDefinition:
					var pd = metadata.GetPropertyDefinition((PropertyDefinitionHandle)handle);
					declaringType = metadata.GetMethodDefinition(pd.GetAccessors().GetAny()).GetDeclaringType();
					if (fullName)
						return EscapeName(declaringType.GetFullTypeName(metadata).ToILNameString() + "." + metadata.GetString(pd.Name));
					return EscapeName(metadata.GetString(pd.Name));
				default:
					return null;
			}
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
			var headers = module.Reader.PEHeaders;
			var architecture = headers.CoffHeader.Machine;
			var characteristics = headers.CoffHeader.Characteristics;
			var corflags = headers.CorHeader.Flags;
			switch (architecture) {
				case Machine.I386:
					if ((corflags & CorFlags.Prefers32Bit) != 0)
						return "AnyCPU (32-bit preferred)";
					if ((corflags & CorFlags.Requires32Bit) != 0)
						return "x86";
					// According to ECMA-335, II.25.3.3.1 CorFlags.Requires32Bit and Characteristics.Bit32Machine must be in sync
					// for assemblies containing managed code. However, this is not true for C++/CLI assemblies.
					if ((corflags & CorFlags.ILOnly) == 0 && (characteristics & Characteristics.Bit32Machine) != 0)
						return "x86";
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

		/// <summary>
		/// Escape characters that cannot be displayed in the UI.
		/// </summary>
		public static StringBuilder EscapeName(StringBuilder sb, string name)
		{
			foreach (char ch in name) {
				if (char.IsWhiteSpace(ch) || char.IsControl(ch) || char.IsSurrogate(ch))
					sb.AppendFormat("\\u{0:x4}", (int)ch);
				else
					sb.Append(ch);
			}
			return sb;
		}

		/// <summary>
		/// Escape characters that cannot be displayed in the UI.
		/// </summary>
		public static string EscapeName(string name)
		{
			return EscapeName(new StringBuilder(name.Length), name).ToString();
		}
	}
}
