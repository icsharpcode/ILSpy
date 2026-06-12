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
using System.Composition;
using System.IO;

using ICSharpCode.BamlDecompiler;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Abstractions;

using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TreeNodes;

using IResourceFileHandler = ICSharpCode.ILSpy.Languages.IResourceFileHandler;
using ResourceFileHandlerContext = ICSharpCode.ILSpy.Languages.ResourceFileHandlerContext;

namespace ICSharpCode.ILSpy.Baml
{
	/// <summary>
	/// Tree-node factory that claims <c>.baml</c> resource entries inside an embedded
	/// <c>.resources</c> file and wraps them in a <see cref="BamlResourceEntryNode"/>.
	/// Discovered by MEF via <see cref="ILSpyTreeNode.ResourceNodeFactories"/>.
	/// </summary>
	[Export(typeof(IResourceNodeFactory))]
	[Shared]
	public sealed class BamlResourceNodeFactory : IResourceNodeFactory
	{
		public ITreeNode? CreateNode(Resource resource)
		{
			if (resource.Name.EndsWith(".baml", StringComparison.OrdinalIgnoreCase))
				return new BamlResourceEntryNode(resource.Name, () => resource.TryOpenStream() ?? Stream.Null);
			return null;
		}
	}

	/// <summary>
	/// Project-export resource handler that intercepts <c>.baml</c> entries during
	/// "Save Code" / project export, decompiles them to XAML, and writes them as
	/// MSBuild <c>&lt;Page&gt;</c> items. The accompanying generated code-behind
	/// partial-class members are surfaced via <see cref="PartialTypeInfo"/> so the
	/// CSharp project decompiler emits matching <c>InitializeComponent</c> stubs.
	/// </summary>
	[Export(typeof(IResourceFileHandler))]
	[Shared]
	public sealed class BamlResourceFileHandler : IResourceFileHandler
	{
		public string EntryType => "Page";

		public bool CanHandle(string name, ResourceFileHandlerContext context)
			=> name.EndsWith(".baml", StringComparison.OrdinalIgnoreCase);

		public string WriteResourceToFile(LoadedAssembly assembly, string fileName, Stream stream, ResourceFileHandlerContext context)
		{
			var typeSystem = new BamlDecompilerTypeSystem(
				assembly.GetMetadataFileOrNull()!,
				assembly.GetAssemblyResolver());
			var decompiler = new XamlDecompiler(typeSystem, new BamlDecompilerSettings {
				ThrowOnAssemblyResolveErrors = context.DecompilationOptions.DecompilerSettings.ThrowOnAssemblyResolveErrors,
			}) {
				CancellationToken = context.DecompilationOptions.CancellationToken,
			};
			var result = decompiler.Decompile(stream);
			// If the BAML root names a CLR partial-class type, prefer the type's reflection name
			// for the .xaml file so it lines up with the matching .xaml.cs the C# project writer
			// emits. Otherwise just swap extensions on the existing resource name.
			var typeDefinition = result.TypeName.HasValue
				? typeSystem.MainModule.GetTypeDefinition(result.TypeName.Value.TopLevelTypeName)
				: null;
			if (typeDefinition != null)
			{
				fileName = WholeProjectDecompiler.SanitizeFileName(typeDefinition.ReflectionName + ".xaml");
				var partialTypeInfo = new PartialTypeInfo(typeDefinition);
				foreach (var member in result.GeneratedMembers)
					partialTypeInfo.AddDeclaredMember(member);
				context.AddPartialTypeInfo(partialTypeInfo);
			}
			else
			{
				fileName = Path.ChangeExtension(fileName, ".xaml");
			}
			context.AdditionalProperties.Add("Generator", "MSBuild:Compile");
			context.AdditionalProperties.Add("SubType", "Designer");
			var saveFileName = Path.Combine(context.DecompilationOptions.SaveAsProjectDirectory!, fileName);
			Directory.CreateDirectory(Path.GetDirectoryName(saveFileName)!);
			result.Xaml.Save(saveFileName);
			return fileName;
		}
	}
}
