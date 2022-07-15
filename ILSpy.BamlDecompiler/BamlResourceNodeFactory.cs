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
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Abstractions;

namespace ILSpy.BamlDecompiler
{
	[Export(typeof(IResourceNodeFactory))]
	public sealed class BamlResourceNodeFactory : IResourceNodeFactory
	{
		public ITreeNode CreateNode(Resource resource)
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
		public bool CanHandle(string name, ResourceFileHandlerContext context) => name.EndsWith(".baml", StringComparison.OrdinalIgnoreCase);

		public string WriteResourceToFile(LoadedAssembly assembly, string fileName, Stream stream, ResourceFileHandlerContext context)
		{
			BamlDecompilerTypeSystem typeSystem = new BamlDecompilerTypeSystem(assembly.GetPEFileOrNull(), assembly.GetAssemblyResolver());
			var decompiler = new XamlDecompiler(typeSystem, new BamlDecompilerSettings() {
				ThrowOnAssemblyResolveErrors = context.DecompilationOptions.DecompilerSettings.ThrowOnAssemblyResolveErrors
			});
			decompiler.CancellationToken = context.DecompilationOptions.CancellationToken;
			var result = decompiler.Decompile(stream);
			var typeDefinition = result.TypeName.HasValue ? typeSystem.MainModule.GetTypeDefinition(result.TypeName.Value.TopLevelTypeName) : null;
			if (typeDefinition != null)
			{
				fileName = WholeProjectDecompiler.CleanUpPath(typeDefinition.ReflectionName) + ".xaml";
				var partialTypeInfo = new PartialTypeInfo(typeDefinition);
				foreach (var member in result.GeneratedMembers)
				{
					partialTypeInfo.AddDeclaredMember(member);
				}
				context.AddPartialTypeInfo(partialTypeInfo);
			}
			else
			{
				fileName = Path.ChangeExtension(fileName, ".xaml");
			}
			string saveFileName = Path.Combine(context.DecompilationOptions.SaveAsProjectDirectory, fileName);
			Directory.CreateDirectory(Path.GetDirectoryName(saveFileName));
			result.Xaml.Save(saveFileName);
			return fileName;
		}
	}
}
