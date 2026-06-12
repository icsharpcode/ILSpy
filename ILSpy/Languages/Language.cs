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
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using AvaloniaEdit.Highlighting;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Abstractions;

namespace ICSharpCode.ILSpy.Languages
{
	/// <summary>
	/// Output language for tree-node labels and decompiled views.
	/// Subclasses override formatting for their target syntax. Implementations must be
	/// thread-safe.
	/// </summary>
	public abstract class Language : ILanguage
	{
		public abstract string Name { get; }

		public abstract string FileExtension { get; }

		/// <summary>
		/// Extension for the language's project file (e.g. <c>.csproj</c>). <c>null</c> when
		/// the language doesn't support multi-file project export — File → Save Code only
		/// offers single-file save in that case. <see cref="ICSharpCode.Decompiler.CSharp.ProjectDecompiler.WholeProjectDecompiler"/>
		/// runs against the live <see cref="MetadataFile"/> when this is non-null and
		/// <see cref="DecompilationOptions.SaveAsProjectDirectory"/> is set.
		/// </summary>
		public virtual string? ProjectFileExtension => null;

		/// <summary>
		/// Bracket-pair matcher used by the text view for caret-side bracket highlighting.
		/// Languages without a matching-bracket concept (IL, IL-AST) return the no-op
		/// default; <see cref="CSharpLanguage"/> overrides to <see cref="CSharpBracketSearcher"/>.
		/// </summary>
		public virtual ICSharpCode.ILSpy.TextView.IBracketSearcher BracketSearcher
			=> ICSharpCode.ILSpy.TextView.DefaultBracketSearcher.DefaultInstance;

		/// <summary>
		/// Versions selectable for this language (e.g. C# 1.0 → C# 14). Default is empty;
		/// <see cref="HasLanguageVersions"/> reports whether any are available so toolbar UI
		/// can hide the version picker for languages that don't differentiate.
		/// </summary>
		public virtual IReadOnlyList<LanguageVersion> LanguageVersions => System.Array.Empty<LanguageVersion>();

		public bool HasLanguageVersions => LanguageVersions.Count > 0;

		/// <summary>
		/// Token-to-source-line mapping for navigation. Default walks <paramref name="member"/>
		/// up to its declaring type and hands back a <see cref="CodeMappingInfo"/> keyed on
		/// that type. The <see cref="EntityHandle"/> the caller passes may name any member
		/// kind (method, field, …) — analyzers like <c>MethodUsesAnalyzer</c> pass the
		/// method's own token and expect the language to walk to the type itself, not crash
		/// trying to cast the method handle. Subclasses with full decompilation override and
		/// build the mapping from the decompiler pipeline.
		/// </summary>
		public virtual CodeMappingInfo GetCodeMappingInfo(MetadataFile module, EntityHandle member)
		{
			var declaringType = (TypeDefinitionHandle)member.GetDeclaringType(module.Metadata);
			if (declaringType.IsNil && member.Kind == HandleKind.TypeDefinition)
				declaringType = (TypeDefinitionHandle)member;
			return new CodeMappingInfo(module, declaringType);
		}

		/// <summary>
		/// Cheap name string used by <see cref="ICSharpCode.ILSpyX.Search.MemberSearchStrategy"/>
		/// and friends as a pre-filter before resolving the full entity. Called once per
		/// type / method / field / property / event handle in the module — must stay metadata-
		/// only (no <see cref="DecompilerTypeSystem"/>, no <c>IAssemblyResolver</c>) so a
		/// large assembly like mscorlib remains sub-second instead of tens of seconds.
		/// Returns <c>null</c> for handle kinds we don't recognise; the strategy skips its
		/// pre-filter for those handles and falls through to the slower resolve.
		/// </summary>
		public virtual string GetEntityName(MetadataFile module, EntityHandle handle, bool fullName, bool omitGenerics)
		{
			ArgumentNullException.ThrowIfNull(module);
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
					if (fullName && !declaringType.IsNil)
						return ILAmbience.EscapeName(declaringType.GetFullTypeName(metadata).ToILNameString(omitGenerics) + "." + metadata.GetString(ed.Name));
					return ILAmbience.EscapeName(metadata.GetString(ed.Name));
				case HandleKind.PropertyDefinition:
					var pd = metadata.GetPropertyDefinition((PropertyDefinitionHandle)handle);
					declaringType = metadata.GetMethodDefinition(pd.GetAccessors().GetAny()).GetDeclaringType();
					if (fullName && !declaringType.IsNil)
						return ILAmbience.EscapeName(declaringType.GetFullTypeName(metadata).ToILNameString(omitGenerics) + "." + metadata.GetString(pd.Name));
					return ILAmbience.EscapeName(metadata.GetString(pd.Name));
				default:
					// MemberSearchStrategy call sites explicitly tolerate null (the IsMatch
					// pre-filter is skipped when the language doesn't recognise the handle
					// kind) — see ICSharpCode.ILSpyX/Search/MemberSearchStrategy.cs.
					return null!;
			}
		}

		public virtual string TypeToString(IType type, ConversionFlags conversionFlags = ConversionFlags.UseFullyQualifiedTypeNames | ConversionFlags.UseFullyQualifiedEntityNames)
		{
			ArgumentNullException.ThrowIfNull(type);
			var ambience = new ILAmbience { ConversionFlags = conversionFlags };
			return ambience.ConvertType(type);
		}

		public virtual string EntityToString(IEntity entity, ConversionFlags conversionFlags)
		{
			ArgumentNullException.ThrowIfNull(entity);
			var ambience = new ILAmbience {
				ConversionFlags = ConversionFlags.ShowTypeParameterList
					| ConversionFlags.PlaceReturnTypeAfterParameterList
					| ConversionFlags.ShowReturnType
					| ConversionFlags.ShowParameterList
					| ConversionFlags.ShowParameterModifiers
					| conversionFlags
			};
			return ambience.ConvertSymbol(entity);
		}

		public virtual void WriteCommentLine(ITextOutput output, string comment)
		{
			output.WriteLine("// " + comment);
		}

		public virtual string GetTooltip(IEntity entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			return EntityToString(entity, ConversionFlags.UseFullyQualifiedTypeNames | ConversionFlags.UseFullyQualifiedEntityNames | ConversionFlags.ShowDeclaringType);
		}

		public virtual RichText GetRichTextTooltip(IEntity entity) => new(GetTooltip(entity));

		public virtual void DecompileType(ITypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(type));
		}

		public virtual void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, MemberDescription(method));
		}

		public virtual void DecompileField(IField field, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, MemberDescription(field));
		}

		public virtual void DecompileProperty(IProperty property, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, MemberDescription(property));
		}

		public virtual void DecompileEvent(IEvent ev, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, MemberDescription(ev));
		}

		/// <summary>
		/// Returns false for entities the active language wants to hide (compiler-generated
		/// closures, anonymous-type backing fields, async-state-machine fields, …) when the
		/// caller has opted into "everything except compiler-generated" via
		/// <see cref="ApiVisibility.PublicOnly"/> / <see cref="ApiVisibility.PublicAndInternal"/>.
		/// The default treats every entity as visible — language subclasses tighten the rule.
		/// </summary>
		public virtual bool ShowMember(IEntity member) => true;

		// FakeMember-derived entities (the decompiler's fallback when a member or reference
		// can't be resolved) can carry a null DeclaringTypeDefinition; fall back to the
		// member name on its own when that's the case.
		string MemberDescription(IMember member)
			=> (member.DeclaringTypeDefinition is { } t ? TypeToString(t) + "." : string.Empty) + member.Name;

		public virtual void DecompileNamespace(string nameSpace, IEnumerable<ITypeDefinition> types, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, nameSpace);
		}

		public virtual ProjectId? DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
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
				else if (metadata.TryGetFullAssemblyName(out string? assemblyName))
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

		public override string ToString() => Name;

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
			var corflags = headers.CorHeader!.Flags;

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
