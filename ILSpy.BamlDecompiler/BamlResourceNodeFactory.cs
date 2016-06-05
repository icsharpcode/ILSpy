// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy;
using System.Resources;
using System.Collections;
using System.Linq;

namespace ILSpy.BamlDecompiler
{
	[Export(typeof(IResourceNodeFactory))]
	public sealed class BamlResourceNodeFactory : IResourceNodeFactory
	{
		public ILSpyTreeNode CreateNode(Mono.Cecil.Resource resource)
		{
			return null;
		}
		
		public ILSpyTreeNode CreateNode(string key, object data)
		{
			if (key.EndsWith(".baml", StringComparison.OrdinalIgnoreCase) && data is Stream)
				return new BamlResourceEntryNode(key, (Stream)data);
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
			var document = BamlResourceEntryNode.LoadIntoDocument(assembly.GetAssemblyResolver(), assembly.AssemblyDefinition, stream);
			fileName = Path.ChangeExtension(fileName, ".xaml");
			document.Save(Path.Combine(options.SaveAsProjectDirectory, fileName));
			return fileName;
		}
	}
}
