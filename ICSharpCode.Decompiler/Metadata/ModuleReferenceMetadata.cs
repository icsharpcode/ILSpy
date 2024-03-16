// Copyright (c) 2023 James May
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

#nullable enable

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.Metadata
{
#if !VSADDIN
	public class ModuleReferenceMetadata /* : IModuleReference*/
	{
		readonly ModuleReference entry;

		public MetadataReader Metadata { get; }
		public ModuleReferenceHandle Handle { get; }

		string? name;
		public string Name {
			get {
				if (name == null)
				{
					try
					{
						name = Metadata.GetString(entry.Name);
					}
					catch (BadImageFormatException)
					{
						name = $"AR:{Handle}";
					}
				}
				return name;
			}
		}

		ImmutableArray<CustomAttribute> attributes;
		public ImmutableArray<CustomAttribute> Attributes {
			get {
				var value = attributes;
				if (value.IsDefault)
				{
					value = entry.GetCustomAttributes().Select(Metadata.GetCustomAttribute).ToImmutableArray();
					attributes = value;
				}
				return value;
			}
		}

		ImmutableArray<TypeReferenceMetadata> typeReferences;
		public ImmutableArray<TypeReferenceMetadata> TypeReferences {
			get {
				var value = typeReferences;
				if (value.IsDefault)
				{
					value = Metadata.TypeReferences
						.Select(r => new TypeReferenceMetadata(Metadata, r))
						.Where(r => r.ResolutionScope == Handle)
						.OrderBy(r => r.Namespace)
						.ThenBy(r => r.Name)
						.ToImmutableArray();
					typeReferences = value;
				}
				return value;
			}
		}

		ImmutableArray<ExportedTypeMetadata> exportedTypes;
		public ImmutableArray<ExportedTypeMetadata> ExportedTypes {
			get {
				var value = exportedTypes;
				if (value.IsDefault)
				{
					value = Metadata.ExportedTypes
						.Select(r => new ExportedTypeMetadata(Metadata, r))
						.Where(r => r.Implementation == Handle)
						.OrderBy(r => r.Namespace)
						.ThenBy(r => r.Name)
						.ToImmutableArray();
					exportedTypes = value;
				}
				return value;
			}
		}

		public ModuleReferenceMetadata(MetadataReader metadata, ModuleReferenceHandle handle)
		{
			if (metadata == null)
				throw new ArgumentNullException(nameof(metadata));
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			Metadata = metadata;
			Handle = handle;
			entry = metadata.GetModuleReference(handle);
		}

		public ModuleReferenceMetadata(PEFile module, ModuleReferenceHandle handle)
		{
			if (module == null)
				throw new ArgumentNullException(nameof(module));
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			Metadata = module.Metadata;
			Handle = handle;
			entry = Metadata.GetModuleReference(handle);
		}

		public override string ToString()
		{
			return Name;
		}
	}
#endif
}
