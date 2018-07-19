using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy
{
	public static class LoadedAssemblyExtensions
	{
		public static IAssemblyResolver GetAssemblyResolver(this PEFile file)
		{
			return GetLoadedAssembly(file).GetAssemblyResolver();
		}

		public static IDebugInfoProvider GetDebugInfoOrNull(this PEFile file)
		{
			return GetLoadedAssembly(file).GetDebugInfoOrNull();
		}

		public static ICompilation GetTypeSystemOrNull(this PEFile file)
		{
			return GetLoadedAssembly(file).GetTypeSystemOrNull();
		}

		static LoadedAssembly GetLoadedAssembly(PEFile file)
		{
			if (file == null)
				throw new ArgumentNullException(nameof(file));
			LoadedAssembly loadedAssembly;
			lock (LoadedAssembly.loadedAssemblies) {
				if (!LoadedAssembly.loadedAssemblies.TryGetValue(file, out loadedAssembly))
					throw new ArgumentException("The specified file is not associated with a LoadedAssembly!");
			}
			return loadedAssembly;
		}
	}
}
