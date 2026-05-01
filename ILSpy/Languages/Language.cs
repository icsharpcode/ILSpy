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
using System.Diagnostics;
using System.Reflection.PortableExecutable;

using AvaloniaEdit.Highlighting;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

namespace ILSpy.Languages
{
	/// <summary>
	/// Output language for tree-node labels and decompiled views.
	/// Subclasses override formatting for their target syntax. Implementations must be
	/// thread-safe.
	/// </summary>
	public abstract class Language
	{
		public abstract string Name { get; }

		public abstract string FileExtension { get; }

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
			Debug.Assert(method.DeclaringTypeDefinition != null);
			WriteCommentLine(output, TypeToString(method.DeclaringTypeDefinition) + "." + method.Name);
		}

		public virtual void DecompileField(IField field, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(field.DeclaringTypeDefinition != null);
			WriteCommentLine(output, TypeToString(field.DeclaringTypeDefinition) + "." + field.Name);
		}

		public virtual void DecompileProperty(IProperty property, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(property.DeclaringTypeDefinition != null);
			WriteCommentLine(output, TypeToString(property.DeclaringTypeDefinition) + "." + property.Name);
		}

		public virtual void DecompileEvent(IEvent ev, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(ev.DeclaringTypeDefinition != null);
			WriteCommentLine(output, TypeToString(ev.DeclaringTypeDefinition) + "." + ev.Name);
		}

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
