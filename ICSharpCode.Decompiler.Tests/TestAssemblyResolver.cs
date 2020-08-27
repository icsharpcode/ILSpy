using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.Tests
{
	sealed class TestAssemblyResolver : UniversalAssemblyResolver
	{
		readonly HashSet<string> localAssemblies = new HashSet<string>();

		public TestAssemblyResolver(string mainAssemblyFileName, string baseDir, string targetFramework)
			: base(mainAssemblyFileName, false, targetFramework, PEStreamOptions.PrefetchMetadata, MetadataReaderOptions.ApplyWindowsRuntimeProjections)
		{
			var assemblyNames = new DirectoryInfo(baseDir).EnumerateFiles("*.dll").Select(f => Path.GetFileNameWithoutExtension(f.Name));
			foreach (var name in assemblyNames)
			{
				localAssemblies.Add(name);
			}
		}

		public override bool IsGacAssembly(IAssemblyReference reference)
		{
			return reference != null && !localAssemblies.Contains(reference.Name);
		}
	}
}
