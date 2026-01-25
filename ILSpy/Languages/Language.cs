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
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Abstractions;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Base class for language-specific decompiler implementations.
	/// </summary>
	/// <remarks>
	/// Implementations of this class must be thread-safe.
	/// </remarks>
	public abstract class Language : ILanguage
	{
		protected static SettingsService SettingsService { get; } = App.ExportProvider.GetExportedValue<SettingsService>();

		protected static AssemblyTreeModel AssemblyTreeModel { get; } = App.ExportProvider.GetExportedValue<AssemblyTreeModel>();

		protected static ICollection<IResourceFileHandler> ResourceFileHandlers { get; } = App.ExportProvider.GetExportedValues<IResourceFileHandler>().ToArray();

		/// <summary>
		/// Gets the name of the language (as shown in the UI)
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the file extension used by source code files in this language.
		/// </summary>
		public abstract string FileExtension { get; }

		public virtual string ProjectFileExtension {
			get { return null; }
		}

		public virtual IReadOnlyList<LanguageVersion> LanguageVersions {
			get { return EmptyList<LanguageVersion>.Instance; }
		}

		public bool HasLanguageVersions => LanguageVersions.Count > 0;

		/// <summary>
		/// Gets the syntax highlighting used for this language.
		/// </summary>
		public virtual IHighlightingDefinition SyntaxHighlighting {
			get {
				return HighlightingManager.Instance.GetDefinitionByExtension(FileExtension);
			}
		}

		public virtual TextView.IBracketSearcher BracketSearcher {
			get {
				return TextView.DefaultBracketSearcher.DefaultInstance;
			}
		}

		public virtual void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(method.DeclaringTypeDefinition) + "." + method.Name);
		}

		public virtual void DecompileProperty(IProperty property, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(property.DeclaringTypeDefinition) + "." + property.Name);
		}

		public virtual void DecompileField(IField field, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(field.DeclaringTypeDefinition) + "." + field.Name);
		}

		public virtual void DecompileEvent(IEvent @event, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(@event.DeclaringTypeDefinition) + "." + @event.Name);
		}

		public virtual void DecompileType(ITypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(type));
		}

		public virtual void DecompileNamespace(string nameSpace, IEnumerable<ITypeDefinition> types, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, nameSpace);
		}

		public virtual ProjectId DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, assembly.FileName);
			var asm = assembly.GetMetadataFileOrNull();
			if (asm == null)
				return null;
			if (options.FullDecompilation && options.SaveAsProjectDirectory != null)
			{
				throw new NotSupportedException($"Language '{Name}' does not support exporting assemblies as projects!");
			}
			var metadata = asm.Metadata;
			if (metadata.IsAssembly)
			{
				var name = metadata.GetAssemblyDefinition();
				if ((name.Flags & System.Reflection.AssemblyFlags.WindowsRuntime) != 0)
				{
					WriteCommentLine(output, metadata.GetString(name.Name) + " [WinRT]");
				}
				else if (metadata.TryGetFullAssemblyName(out string assemblyName))
				{
					WriteCommentLine(output, assemblyName);
				}
				else
				{
					WriteCommentLine(output, "ERR: Could not read assembly name");
				}
			}
			else
			{
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
		public virtual string TypeToString(IType type, ConversionFlags conversionFlags = ConversionFlags.UseFullyQualifiedTypeNames | ConversionFlags.UseFullyQualifiedEntityNames)
		{
			return new ILAmbience() { ConversionFlags = conversionFlags }.ConvertType(type);
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

		/// <summary>
		/// Converts a member signature to a string.
		/// This is used for displaying the tooltip on a member reference.
		/// </summary>
		public virtual RichText GetRichTextTooltip(IEntity entity)
		{
			return GetTooltip(entity);
		}

		[Obsolete("Use EntityToString instead")]
		public virtual string FieldToString(IField field, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			if (field == null)
				throw new ArgumentNullException(nameof(field));
			var flags = ConversionFlags.ShowTypeParameterList
				| ConversionFlags.PlaceReturnTypeAfterParameterList
				| ConversionFlags.ShowReturnType
				| ConversionFlags.ShowParameterList
				| ConversionFlags.ShowParameterModifiers;
			if (includeDeclaringTypeName)
				flags |= ConversionFlags.ShowDeclaringType;
			if (includeNamespace)
				flags |= ConversionFlags.UseFullyQualifiedTypeNames;
			if (includeNamespaceOfDeclaringTypeName)
				flags |= ConversionFlags.UseFullyQualifiedEntityNames;
			return EntityToString(field, flags);
		}

		[Obsolete("Use EntityToString instead")]
		public virtual string PropertyToString(IProperty property, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			if (property == null)
				throw new ArgumentNullException(nameof(property));
			var flags = ConversionFlags.ShowTypeParameterList
				| ConversionFlags.PlaceReturnTypeAfterParameterList
				| ConversionFlags.ShowReturnType
				| ConversionFlags.ShowParameterList
				| ConversionFlags.ShowParameterModifiers;
			if (includeDeclaringTypeName)
				flags |= ConversionFlags.ShowDeclaringType;
			if (includeNamespace)
				flags |= ConversionFlags.UseFullyQualifiedTypeNames;
			if (includeNamespaceOfDeclaringTypeName)
				flags |= ConversionFlags.UseFullyQualifiedEntityNames;
			return EntityToString(property, flags);
		}

		[Obsolete("Use EntityToString instead")]
		public virtual string MethodToString(IMethod method, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));
			var flags = ConversionFlags.ShowTypeParameterList
				| ConversionFlags.PlaceReturnTypeAfterParameterList
				| ConversionFlags.ShowReturnType
				| ConversionFlags.ShowParameterList
				| ConversionFlags.ShowParameterModifiers;
			if (includeDeclaringTypeName)
				flags |= ConversionFlags.ShowDeclaringType;
			if (includeNamespace)
				flags |= ConversionFlags.UseFullyQualifiedTypeNames;
			if (includeNamespaceOfDeclaringTypeName)
				flags |= ConversionFlags.UseFullyQualifiedEntityNames;
			return EntityToString(method, flags);
		}

		[Obsolete("Use EntityToString instead")]
		public virtual string EventToString(IEvent @event, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			if (@event == null)
				throw new ArgumentNullException(nameof(@event));
			var flags = ConversionFlags.ShowTypeParameterList
				| ConversionFlags.PlaceReturnTypeAfterParameterList
				| ConversionFlags.ShowReturnType
				| ConversionFlags.ShowParameterList
				| ConversionFlags.ShowParameterModifiers;
			if (includeDeclaringTypeName)
				flags |= ConversionFlags.ShowDeclaringType;
			if (includeNamespace)
				flags |= ConversionFlags.UseFullyQualifiedTypeNames;
			if (includeNamespaceOfDeclaringTypeName)
				flags |= ConversionFlags.UseFullyQualifiedEntityNames;
			return EntityToString(@event, flags);
		}

		public virtual string EntityToString(IEntity entity, ConversionFlags conversionFlags)
		{
			var ambience = new ILAmbience();
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList
				| ConversionFlags.PlaceReturnTypeAfterParameterList
				| ConversionFlags.ShowReturnType
				| ConversionFlags.ShowParameterList
				| ConversionFlags.ShowParameterModifiers
				| conversionFlags;
			return ambience.ConvertSymbol(entity);
		}

		protected string GetDisplayName(IEntity entity, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			string entityName;
			if (entity is ITypeDefinition t && !t.MetadataToken.IsNil)
			{
				MetadataReader metadata = t.ParentModule.MetadataFile.Metadata;
				var typeDef = metadata.GetTypeDefinition((TypeDefinitionHandle)t.MetadataToken);
				entityName = ILAmbience.EscapeName(metadata.GetString(typeDef.Name));
			}
			else
			{
				entityName = ILAmbience.EscapeName(entity.Name);
			}
			if (includeNamespace || includeDeclaringTypeName)
			{
				if (entity.DeclaringTypeDefinition != null)
					return TypeToString(entity.DeclaringTypeDefinition, ConversionFlags.ShowDeclaringType | ConversionFlags.UseFullyQualifiedEntityNames) + "." + entityName;
				return ILAmbience.EscapeName(entity.Namespace) + "." + entityName;
			}
			else
			{
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
		public virtual string GetEntityName(MetadataFile module, EntityHandle handle, bool fullName, bool omitGenerics)
		{
			MetadataReader metadata = module.Metadata;
			switch (handle.Kind)
			{
				case HandleKind.TypeDefinition:
					if (fullName)
						return ILAmbience.EscapeName(((TypeDefinitionHandle)handle).GetFullTypeName(metadata).ToILNameString(omitGenerics));
					var td = metadata.GetTypeDefinition((TypeDefinitionHandle)handle);
					return ILAmbience.EscapeName(metadata.GetString(td.Name));
				case HandleKind.FieldDefinition:
					var fd = metadata.GetFieldDefinition((FieldDefinitionHandle)handle);
					if (fullName)
						return ILAmbience.EscapeName(fd.GetDeclaringType().GetFullTypeName(metadata).ToILNameString(omitGenerics) + "." + metadata.GetString(fd.Name));
					return ILAmbience.EscapeName(metadata.GetString(fd.Name));
				case HandleKind.MethodDefinition:
					var md = metadata.GetMethodDefinition((MethodDefinitionHandle)handle);
					string methodName = metadata.GetString(md.Name);
					if (!omitGenerics)
					{
						int genericParamCount = md.GetGenericParameters().Count;
						if (genericParamCount > 0)
							methodName += "``" + genericParamCount;
					}
					if (fullName)
						return ILAmbience.EscapeName(md.GetDeclaringType().GetFullTypeName(metadata).ToILNameString(omitGenerics) + "." + methodName);
					return ILAmbience.EscapeName(methodName);
				case HandleKind.EventDefinition:
					var ed = metadata.GetEventDefinition((EventDefinitionHandle)handle);
					var declaringType = metadata.GetMethodDefinition(ed.GetAccessors().GetAny()).GetDeclaringType();
					if (fullName)
						return ILAmbience.EscapeName(declaringType.GetFullTypeName(metadata).ToILNameString(omitGenerics) + "." + metadata.GetString(ed.Name));
					return ILAmbience.EscapeName(metadata.GetString(ed.Name));
				case HandleKind.PropertyDefinition:
					var pd = metadata.GetPropertyDefinition((PropertyDefinitionHandle)handle);
					declaringType = metadata.GetMethodDefinition(pd.GetAccessors().GetAny()).GetDeclaringType();
					if (fullName)
						return ILAmbience.EscapeName(declaringType.GetFullTypeName(metadata).ToILNameString(omitGenerics) + "." + metadata.GetString(pd.Name));
					return ILAmbience.EscapeName(metadata.GetString(pd.Name));
				default:
					return null;
			}
		}

		public virtual CodeMappingInfo GetCodeMappingInfo(MetadataFile module, EntityHandle member)
		{
			var declaringType = (TypeDefinitionHandle)member.GetDeclaringType(module.Metadata);

			if (declaringType.IsNil && member.Kind == HandleKind.TypeDefinition)
			{
				declaringType = (TypeDefinitionHandle)member;
			}

			return new CodeMappingInfo(module, declaringType);
		}

		static readonly IReadOnlyDictionary<Machine, string> osMachineLookup = new Dictionary<Machine, string>
		{
			{ (Machine)0x4644, "MacOS" },
			{ (Machine)0x7b79, "Linux" },
			{ (Machine)0xadc4, "FreeBSD" },
			{ (Machine)0x1993, "NetBSD" },
			{ (Machine)0x1992, "Sun" },
		};

		public static string GetPlatformDisplayName(PEFile module)
		{
			var headers = module.Reader.PEHeaders;
			var architecture = headers.CoffHeader.Machine;
			var characteristics = headers.CoffHeader.Characteristics;
			var corflags = headers.CorHeader.Flags;

			var modifier = string.Empty;

			if (!Enum.IsDefined(architecture))
			{
				foreach (var (osEnum, osText) in osMachineLookup)
				{
					var candidate = architecture ^ osEnum;
					if (Enum.IsDefined(candidate))
					{
						modifier = osText + " ";
						architecture = candidate;
						break;
					}
				}
			}

			switch (architecture)
			{
				case Machine.I386:
					if ((corflags & CorFlags.Prefers32Bit) != 0)
						return modifier + "AnyCPU (32-bit preferred)";
					if ((corflags & CorFlags.Requires32Bit) != 0)
						return modifier + "x86";
					// According to ECMA-335, II.25.3.3.1 CorFlags.Requires32Bit and Characteristics.Bit32Machine must be in sync
					// for assemblies containing managed code. However, this is not true for C++/CLI assemblies.
					if ((corflags & CorFlags.ILOnly) == 0 && (characteristics & Characteristics.Bit32Machine) != 0)
						return modifier + "x86";
					return modifier + "AnyCPU (64-bit preferred)";
				case Machine.Amd64:
					return modifier + "x64";
				case Machine.IA64:
					return modifier + "Itanium";
				default:
					return architecture.ToString();
			}
		}

		public static string GetRuntimeDisplayName(MetadataFile module)
		{
			return module.Metadata.MetadataVersion;
		}
	}
}
