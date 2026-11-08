// Copyright (c) 2026 Piero Viano
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

using Avalonia;
using Avalonia.Media;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy.FileStructure
{
	/// <summary>
	/// One type or member defined in a decompiled document, as listed by the File Structure pane
	/// and the Go to File Member popup.
	/// </summary>
	/// <param name="Name">The name as written at the definition (a constructor shows its type's name).</param>
	/// <param name="Detail">Extra text that tells overloads apart (a method's parameter types), or empty.</param>
	/// <param name="Icon">The same icon the assembly tree shows for the entity.</param>
	/// <param name="Offset">Document offset of the definition's name; navigation puts the caret here.</param>
	/// <param name="Depth">Nesting level: 0 for a top-level type, one more per enclosing type.</param>
	/// <param name="Entity">The resolved entity, or null for an unresolved metadata definition (IL view).</param>
	public sealed record FileMemberEntry(string Name, string Detail, IImage? Icon, int Offset, int Depth, IEntity? Entity)
	{
		/// <summary>Name plus detail, the text a row shows.</summary>
		public string DisplayText => Detail.Length == 0 ? Name : Name + Detail;

		/// <summary>Left indentation of a row, proportional to <see cref="Depth"/>.</summary>
		public Thickness Indent => new(Depth * 16, 0, 0, 0);
	}

	/// <summary>
	/// Builds the member outline of a decompiled document from the definition references the
	/// decompiler recorded while writing it, so the outline matches the text exactly and costs no
	/// extra decompilation.
	/// </summary>
	public static class FileMemberCollector
	{
		public static IReadOnlyList<FileMemberEntry> Collect(DecompilerTabPageModel? document)
		{
			if (document?.References is not { } references || string.IsNullOrEmpty(document.Text))
				return Array.Empty<FileMemberEntry>();
			string text = document.Text;
			var entries = new List<FileMemberEntry>();
			var seen = new HashSet<(object Module, int Token)>();
			foreach (var segment in references)
			{
				if (!segment.IsDefinition || segment.Kind != ReferenceMode.Link || segment.EndOffset > text.Length)
					continue;
				var entry = segment.Reference switch {
					IEntity entity => FromEntity(entity, segment, text),
					EntityReference metadataReference => FromMetadata(metadataReference, segment, text),
					_ => null,
				};
				if (entry == null)
					continue;
				if (Identity(segment.Reference!) is { } identity && !seen.Add(identity))
					continue;
				entries.Add(entry);
			}
			entries.Sort((a, b) => a.Offset.CompareTo(b.Offset));
			int minDepth = entries.Count == 0 ? 0 : entries.Min(e => e.Depth);
			return minDepth == 0 ? entries : entries.Select(e => e with { Depth = e.Depth - minDepth }).ToList();
		}

		static (object, int)? Identity(object reference) => reference switch {
			IEntity { MetadataToken.IsNil: false } entity when entity.ParentModule?.MetadataFile is { } file
				=> (file, System.Reflection.Metadata.Ecma335.MetadataTokens.GetToken(entity.MetadataToken)),
			EntityReference r => (r.Module, System.Reflection.Metadata.Ecma335.MetadataTokens.GetToken(r.Handle)),
			_ => null,
		};

		static FileMemberEntry? FromEntity(IEntity entity, ReferenceSegment segment, string text)
		{
			// Accessors are listed through their property or event.
			if (entity is IMethod { AccessorOwner: not null })
				return null;
			var icon = GetIcon(entity);
			if (icon == null)
				return null;
			string name = text.Substring(segment.StartOffset, segment.Length);
			string detail = entity is IMethod method
				? "(" + string.Join(", ", method.Parameters.Select(p => p.Type.Name)) + ")"
				: string.Empty;
			return new FileMemberEntry(name, detail, icon, segment.StartOffset, NestingDepth(entity), entity);
		}

		static IImage? GetIcon(IEntity entity) => entity switch {
			ITypeDefinition type => TypeTreeNode.GetIcon(type),
			IMethod method => MethodTreeNode.GetIcon(method),
			IProperty property => PropertyTreeNode.GetIcon(property),
			IField field => FieldTreeNode.GetIcon(field),
			IEvent @event => EventTreeNode.GetIcon(@event),
			_ => null,
		};

		// A type nested N levels deep sits at depth N; its members one level further in.
		static int NestingDepth(IEntity entity)
		{
			int depth = entity is ITypeDefinition ? 0 : 1;
			for (var declaring = entity.DeclaringTypeDefinition; declaring != null; declaring = declaring.DeclaringTypeDefinition)
				depth++;
			return depth;
		}

		// The IL view writes definitions as unresolved (module, handle) references whose text is
		// the directive keyword (".method", ".class", ...); the name comes from the metadata.
		static FileMemberEntry? FromMetadata(EntityReference reference, ReferenceSegment segment, string text)
		{
			if (reference.Protocol != "decompile" || reference.Handle.IsNil)
				return null;
			var metadataFile = ResolveMetadata(reference);
			if (metadataFile == null)
				return null;
			var metadata = metadataFile.Metadata;
			try
			{
				switch (reference.Handle.Kind)
				{
					case HandleKind.TypeDefinition:
					{
						var handle = (TypeDefinitionHandle)reference.Handle;
						var definition = metadata.GetTypeDefinition(handle);
						int depth = 0;
						for (var declaring = definition.GetDeclaringType(); !declaring.IsNil; declaring = metadata.GetTypeDefinition(declaring).GetDeclaringType())
							depth++;
						return new FileMemberEntry(StripArity(metadata.GetString(definition.Name)), string.Empty,
							Images.Class, segment.StartOffset, depth, null);
					}
					case HandleKind.MethodDefinition:
					{
						var definition = metadata.GetMethodDefinition((MethodDefinitionHandle)reference.Handle);
						return new FileMemberEntry(metadata.GetString(definition.Name), string.Empty, Images.Method,
							segment.StartOffset, MemberDepth(metadata, definition.GetDeclaringType()), null);
					}
					case HandleKind.FieldDefinition:
					{
						var definition = metadata.GetFieldDefinition((FieldDefinitionHandle)reference.Handle);
						return new FileMemberEntry(metadata.GetString(definition.Name), string.Empty, Images.Field,
							segment.StartOffset, MemberDepth(metadata, definition.GetDeclaringType()), null);
					}
					case HandleKind.PropertyDefinition:
					{
						var handle = (PropertyDefinitionHandle)reference.Handle;
						var definition = metadata.GetPropertyDefinition(handle);
						var declaring = metadata.GetMethodDefinition(definition.GetAccessors().Getter.IsNil
							? definition.GetAccessors().Setter : definition.GetAccessors().Getter).GetDeclaringType();
						return new FileMemberEntry(metadata.GetString(definition.Name), string.Empty, Images.Property,
							segment.StartOffset, MemberDepth(metadata, declaring), null);
					}
					case HandleKind.EventDefinition:
					{
						var definition = metadata.GetEventDefinition((EventDefinitionHandle)reference.Handle);
						var declaring = metadata.GetMethodDefinition(definition.GetAccessors().Adder).GetDeclaringType();
						return new FileMemberEntry(metadata.GetString(definition.Name), string.Empty, Images.Event,
							segment.StartOffset, MemberDepth(metadata, declaring), null);
					}
					default:
						return null;
				}
			}
			catch (BadImageFormatException)
			{
				return null;
			}
		}

		static int MemberDepth(MetadataReader metadata, TypeDefinitionHandle declaringType)
		{
			int depth = 1;
			for (var declaring = declaringType; !declaring.IsNil; declaring = metadata.GetTypeDefinition(declaring).GetDeclaringType())
			{
				if (!metadata.GetTypeDefinition(declaring).GetDeclaringType().IsNil)
					depth++;
			}
			return depth;
		}

		static string StripArity(string name)
		{
			int tick = name.IndexOf('`');
			return tick > 0 ? name.Substring(0, tick) : name;
		}

		static MetadataFile? ResolveMetadata(EntityReference reference)
		{
			var assemblyList = AppEnv.AppComposition.TryGetExport<AssemblyTree.AssemblyTreeModel>()?.AssemblyList;
			return assemblyList == null ? null : reference.ResolveAssembly(assemblyList);
		}
	}
}
