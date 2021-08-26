// Copyright (c) 2020 AlphaSierraPapa for the SharpDevelop Team
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
using System.ComponentModel.Composition;
using System.IO;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;

namespace ILSpy.BamlDecompiler
{
	[Export(typeof(IResourceNodeFactory))]
	public sealed class BamlResourceNodeFactory : IResourceNodeFactory
	{
		public ILSpyTreeNode CreateNode(Resource resource)
		{
			if (resource.Name.EndsWith(".baml", StringComparison.OrdinalIgnoreCase))
				return new BamlResourceEntryNode(resource.Name, resource.TryOpenStream);
			else
				return null;
		}
	}

	[Export(typeof(IResourceFileHandler))]
	public sealed class BamlResourceFileHandler : IResourceFileHandler
	{
		public string EntryType => "Page";
		public bool CanHandle(string name, DecompilationOptions options) => name.EndsWith(".baml", StringComparison.OrdinalIgnoreCase);

		public string WriteResourceToFile(LoadedAssembly assembly, string fileName, Stream stream, DecompilationOptions options)
		{
			BamlDecompilerTypeSystem typeSystem = new BamlDecompilerTypeSystem(assembly.GetPEFileOrNull(), assembly.GetAssemblyResolver());
			var decompiler = new XamlDecompiler(typeSystem, new BamlDecompilerSettings() {
				ThrowOnAssemblyResolveErrors = options.DecompilerSettings.ThrowOnAssemblyResolveErrors
			});
			decompiler.CancellationToken = options.CancellationToken;
			fileName = Path.ChangeExtension(fileName, ".xaml");
			var result = decompiler.Decompile(stream);
			result.Xaml.Save(Path.Combine(options.SaveAsProjectDirectory, fileName));
			return fileName;
		}
	}
}
