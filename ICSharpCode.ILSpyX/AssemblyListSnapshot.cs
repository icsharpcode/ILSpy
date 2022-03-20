// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpyX.Extensions;

namespace ICSharpCode.ILSpyX
{
	class AssemblyListSnapshot
	{
		readonly ImmutableArray<LoadedAssembly> assemblies;
		Dictionary<string, PEFile>? asmLookupByFullName;
		Dictionary<string, PEFile>? asmLookupByShortName;
		Dictionary<string, List<(PEFile module, Version version)>>? asmLookupByShortNameGrouped;
		public ImmutableArray<LoadedAssembly> Assemblies => assemblies;

		public AssemblyListSnapshot(ImmutableArray<LoadedAssembly> assemblies)
		{
			this.assemblies = assemblies;
		}

		public async Task<PEFile?> TryGetModuleAsync(IAssemblyReference reference, string tfm)
		{
			bool isWinRT = reference.IsWindowsRuntime;
			if (tfm.StartsWith(".NETFramework,Version=v4.", StringComparison.Ordinal))
			{
				tfm = ".NETFramework,Version=v4";
			}
			string key = tfm + ";" + (isWinRT ? reference.Name : reference.FullName);
			var lookup = LazyInit.VolatileRead(ref isWinRT ? ref asmLookupByShortName : ref asmLookupByFullName);
			if (lookup == null)
			{
				lookup = await CreateLoadedAssemblyLookupAsync(shortNames: isWinRT).ConfigureAwait(false);
				lookup = LazyInit.GetOrSet(ref isWinRT ? ref asmLookupByShortName : ref asmLookupByFullName, lookup);
			}
			if (lookup.TryGetValue(key, out PEFile? module))
				return module;
			return null;
		}

		public async Task<PEFile?> TryGetSimilarModuleAsync(IAssemblyReference reference)
		{
			var lookup = LazyInit.VolatileRead(ref asmLookupByShortNameGrouped);
			if (lookup == null)
			{
				lookup = await CreateLoadedAssemblyShortNameGroupLookupAsync().ConfigureAwait(false);
				lookup = LazyInit.GetOrSet(ref asmLookupByShortNameGrouped, lookup);
			}

			if (!lookup.TryGetValue(reference.Name, out var candidates))
				return null;
			return candidates.FirstOrDefault(c => c.version >= reference.Version).module ?? candidates.Last().module;
		}

		private async Task<Dictionary<string, PEFile>> CreateLoadedAssemblyLookupAsync(bool shortNames)
		{
			var result = new Dictionary<string, PEFile>(StringComparer.OrdinalIgnoreCase);
			foreach (LoadedAssembly loaded in assemblies)
			{
				try
				{
					var module = await loaded.GetPEFileOrNullAsync().ConfigureAwait(false);
					if (module == null)
						continue;
					var reader = module.Metadata;
					if (reader == null || !reader.IsAssembly)
						continue;
					string tfm = await loaded.GetTargetFrameworkIdAsync().ConfigureAwait(false);
					if (tfm.StartsWith(".NETFramework,Version=v4.", StringComparison.Ordinal))
					{
						tfm = ".NETFramework,Version=v4";
					}
					string key = tfm + ";"
						+ (shortNames ? module.Name : module.FullName);
					if (!result.ContainsKey(key))
					{
						result.Add(key, module);
					}
				}
				catch (BadImageFormatException)
				{
					continue;
				}
			}
			return result;
		}

		private async Task<Dictionary<string, List<(PEFile module, Version version)>>> CreateLoadedAssemblyShortNameGroupLookupAsync()
		{
			var result = new Dictionary<string, List<(PEFile module, Version version)>>(StringComparer.OrdinalIgnoreCase);

			foreach (LoadedAssembly loaded in assemblies)
			{
				try
				{
					var module = await loaded.GetPEFileOrNullAsync().ConfigureAwait(false);
					var reader = module?.Metadata;
					if (reader == null || !reader.IsAssembly)
						continue;
					var asmDef = reader.GetAssemblyDefinition();
					var asmDefName = reader.GetString(asmDef.Name);

					var line = (module!, version: asmDef.Version);

					if (!result.TryGetValue(asmDefName, out var existing))
					{
						existing = new List<(PEFile module, Version version)>();
						result.Add(asmDefName, existing);
						existing.Add(line);
						continue;
					}

					int index = existing.BinarySearch(line.version, l => l.version);
					index = index < 0 ? ~index : index + 1;
					existing.Insert(index, line);
				}
				catch (BadImageFormatException)
				{
					continue;
				}
			}

			return result;
		}

		/// <summary>
		/// Gets all loaded assemblies recursively, including assemblies found in bundles or packages.
		/// </summary>
		public async Task<IList<LoadedAssembly>> GetAllAssembliesAsync()
		{
			var results = new List<LoadedAssembly>(assemblies.Length);

			foreach (var asm in assemblies)
			{
				LoadedAssembly.LoadResult result;
				try
				{
					result = await asm.GetLoadResultAsync().ConfigureAwait(false);
				}
				catch
				{
					results.Add(asm);
					continue;
				}
				if (result.Package != null)
				{
					AddDescendants(result.Package.RootFolder);
				}
				else if (result.PEFile != null)
				{
					results.Add(asm);
				}
			}

			void AddDescendants(PackageFolder folder)
			{
				foreach (var subFolder in folder.Folders)
				{
					AddDescendants(subFolder);
				}

				foreach (var entry in folder.Entries)
				{
					if (!entry.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
						continue;
					var asm = folder.ResolveFileName(entry.Name);
					if (asm == null)
						continue;
					results.Add(asm);
				}
			}

			return results;
		}
	}
}
