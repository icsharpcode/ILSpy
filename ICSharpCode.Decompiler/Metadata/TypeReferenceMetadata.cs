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
using System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.Metadata
{
#if !VSADDIN
	public sealed class TypeReferenceMetadata
	{
		readonly TypeReference entry;

		public MetadataReader Metadata { get; }
		public TypeReferenceHandle Handle { get; }

		string? name;
		public string Name {
			get {
				try
				{
					return name ??= Metadata.GetString(entry.Name);
				}
				catch (BadImageFormatException)
				{
					return name = $"TR:{Handle}";
				}
			}
		}

		string? @namespace;
		public string Namespace {
			get {
				try
				{
					return @namespace ??= Metadata.GetString(entry.Namespace);
				}
				catch (BadImageFormatException)
				{
					return @namespace = $"namespace(TR:{Handle})";
				}
			}
		}

		public EntityHandle ResolutionScope => entry.ResolutionScope;

		ImmutableArray<MemberReferenceMetadata> memberReferences;
		public ImmutableArray<MemberReferenceMetadata> MemberReferences {
			get {
				var value = memberReferences;
				if (value.IsDefault)
				{
					value = Metadata.MemberReferences
						.Select(r => new MemberReferenceMetadata(Metadata, r))
						.Where(r => r.Parent == Handle)
						.OrderBy(r => r.Name)
						.ToImmutableArray();
					memberReferences = value;
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
						.OrderBy(r => r.Name)
						.ToImmutableArray();
					typeReferences = value;
				}
				return value;
			}
		}

		public TypeReferenceMetadata(MetadataReader metadata, TypeReferenceHandle handle)
		{
			Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			Handle = handle;
			entry = metadata.GetTypeReference(handle);
		}

		public override string ToString() => $"{Namespace}::{Name}";
	}
#endif
}
