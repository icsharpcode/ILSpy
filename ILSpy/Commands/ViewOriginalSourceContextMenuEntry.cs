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
using System.Composition;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.Symbols;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Symbols;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click a type or member -> "View Original Source". Opens the source documents the
	/// member's PDB records, one tab each: the source embedded in the PDB when present, otherwise
	/// the file its Source Link map points at. Shown only when the PDB can supply at least one.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.ViewOriginalSource), Category = "Debug", Order = 420)]
	[Shared]
	public sealed class ViewOriginalSourceContextMenuEntry : IContextMenuEntry
	{
		const int MaxDocuments = 20;

		readonly DockWorkspace dockWorkspace;
		readonly SymbolService symbolService;

		[ImportingConstructor]
		public ViewOriginalSourceContextMenuEntry(DockWorkspace dockWorkspace, SymbolService symbolService)
		{
			this.dockWorkspace = dockWorkspace;
			this.symbolService = symbolService;
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
		{
			var (sources, documents) = GetDocuments(context);
			return sources != null
				&& documents.Any(d => sources.GetSourceLinkUri(d) != null || sources.GetEmbeddedSource(d) != null);
		}

		public void Execute(TextViewContext context)
		{
			var (sources, documents) = GetDocuments(context);
			if (sources == null || documents.Count == 0)
				return;
			ExecuteAsync(sources, documents).HandleExceptions();
		}

		async Task ExecuteAsync(OriginalSourceProvider sources, IReadOnlyList<DocumentHandle> documents)
		{
			var failed = new List<string>();
			foreach (var document in documents.Take(MaxDocuments))
			{
				var source = await sources.GetSourceAsync(document).ConfigureAwait(true);
				if (source == null)
				{
					failed.Add(sources.GetDocumentPath(document));
					continue;
				}
				dockWorkspace.ShowTextInNewTab(SymbolKey.GetFileName(source.DocumentPath), CreateOutput(source));
			}
			if (failed.Count > 0)
			{
				var output = new AvaloniaEditTextOutput { Title = Resources.ViewOriginalSource };
				output.WriteLine("The original source of these documents could not be retrieved:");
				foreach (var path in failed)
					output.WriteLine("  " + path);
				dockWorkspace.ShowTextInNewTab(Resources.ViewOriginalSource, output);
			}
		}

		static AvaloniaEditTextOutput CreateOutput(OriginalSource source)
		{
			string extension = Path.GetExtension(SymbolKey.GetFileName(source.DocumentPath)).ToLowerInvariant();
			var output = new AvaloniaEditTextOutput {
				Title = SymbolKey.GetFileName(source.DocumentPath),
				SyntaxExtensionOverride = extension,
			};
			string? commentPrefix = extension switch {
				".cs" or ".fs" or ".c" or ".cpp" or ".h" => "// ",
				".vb" => "' ",
				_ => null,
			};
			if (commentPrefix != null)
			{
				output.WriteLine(commentPrefix + source.DocumentPath);
				output.WriteLine(commentPrefix + (source.Origin == OriginalSourceOrigin.Embedded
					? "Source: embedded in the PDB"
					: "Source: " + source.Uri));
				if (source.ChecksumMatches == false)
					output.WriteLine(commentPrefix + "Warning: the checksum differs from the one recorded in the PDB (line endings or a different revision).");
				output.WriteLine();
			}
			output.Write(source.Text);
			return output;
		}

		(OriginalSourceProvider? Sources, IReadOnlyList<DocumentHandle> Documents) GetDocuments(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: 1 } nodes || nodes[0] is not IMemberTreeNode { Member: { } entity })
				return (null, Array.Empty<DocumentHandle>());
			if (entity.ParentModule?.MetadataFile is not MetadataFile module)
				return (null, Array.Empty<DocumentHandle>());
			var sources = symbolService.CreateOriginalSourceProvider(module.GetDebugInfoOrNull());
			if (sources == null)
				return (null, Array.Empty<DocumentHandle>());
			try
			{
				return (sources, sources.GetDocuments(GetMethods(module.Metadata, entity)));
			}
			catch (BadImageFormatException)
			{
				return (null, Array.Empty<DocumentHandle>());
			}
		}

		static IEnumerable<MethodDefinitionHandle> GetMethods(MetadataReader metadata, IEntity entity)
		{
			switch (entity)
			{
				case ITypeDefinition type when !type.MetadataToken.IsNil:
					return GetTypeMethods(metadata, (TypeDefinitionHandle)type.MetadataToken);
				case IMethod method when !method.MetadataToken.IsNil:
					return new[] { (MethodDefinitionHandle)method.MetadataToken };
				case IProperty property:
					return Accessors(property.Getter, property.Setter);
				case IEvent @event:
					return Accessors(@event.AddAccessor, @event.RemoveAccessor, @event.InvokeAccessor);
				default:
					return Array.Empty<MethodDefinitionHandle>();
			}

			static IEnumerable<MethodDefinitionHandle> Accessors(params IMethod?[] accessors)
				=> accessors.Where(a => a != null && !a.MetadataToken.IsNil).Select(a => (MethodDefinitionHandle)a!.MetadataToken);
		}

		static IEnumerable<MethodDefinitionHandle> GetTypeMethods(MetadataReader metadata, TypeDefinitionHandle handle)
		{
			var type = metadata.GetTypeDefinition(handle);
			foreach (var method in type.GetMethods())
				yield return method;
			foreach (var nested in type.GetNestedTypes())
			{
				foreach (var method in GetTypeMethods(metadata, nested))
					yield return method;
			}
		}
	}
}
