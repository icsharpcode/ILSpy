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

namespace ILSpy.Languages
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
		/// Versions selectable for this language (e.g. C# 1.0 → C# 14). Default is empty;
		/// <see cref="HasLanguageVersions"/> reports whether any are available so toolbar UI
		/// can hide the version picker for languages that don't differentiate.
		/// </summary>
		public virtual IReadOnlyList<LanguageVersion> LanguageVersions => System.Array.Empty<LanguageVersion>();

		public bool HasLanguageVersions => LanguageVersions.Count > 0;

		/// <summary>
		/// Token-to-source-line mapping for navigation. Default returns a no-op CodeMappingInfo —
		/// language subclasses with full decompilation override and produce a real one.
		/// </summary>
		public virtual CodeMappingInfo GetCodeMappingInfo(MetadataFile module, EntityHandle member)
			=> new(module, (TypeDefinitionHandle)member);

		/// <summary>
		/// Stable name string for an entity reachable only by metadata token. Falls back to the
		/// language's <see cref="EntityToString"/> path so subclasses don't have to repeat the
		/// formatting rules — overrideable when a language wants a tokenless name (e.g. C# uses a
		/// disassembler-friendly form).
		/// </summary>
		public virtual string GetEntityName(MetadataFile module, EntityHandle handle, bool fullName, bool omitGenerics)
		{
			ArgumentNullException.ThrowIfNull(module);
			var typeSystem = new DecompilerTypeSystem(module, module.GetAssemblyResolver());
			var entity = typeSystem.MainModule.ResolveEntity(handle);
			if (entity == null)
				return string.Empty;
			var flags = ConversionFlags.ShowParameterList | ConversionFlags.ShowParameterModifiers;
			if (fullName)
				flags |= ConversionFlags.UseFullyQualifiedTypeNames | ConversionFlags.UseFullyQualifiedEntityNames;
			if (!omitGenerics)
				flags |= ConversionFlags.ShowTypeParameterList;
			return entity is IType type ? TypeToString(type, flags) : EntityToString((IEntity)entity, flags);
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
