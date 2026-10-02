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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;

#nullable enable

namespace ICSharpCode.ILSpyX.Symbols
{
	/// <summary>
	/// Serves the symbols of a set of assemblies under their symbol-server keys:
	/// <list type="bullet">
	/// <item>the PE file itself under its time stamp / image size key;</item>
	/// <item>for portable-PDB keys, the assembly's own PDB when one is loaded from disk and matches,
	/// otherwise a portable PDB generated on demand with the decompiled sources embedded, so a
	/// debugger can step through code that ships without symbols.</item>
	/// </list>
	/// Windows-PDB keys are not served: a generated portable PDB cannot satisfy a Windows-PDB
	/// signature.
	/// </summary>
	public sealed class DecompiledSymbolStore : ISymbolFileSource
	{
		readonly Func<IEnumerable<LoadedAssembly>> assemblies;
		readonly Func<DecompilerSettings> settings;
		readonly string cacheDirectory;
		readonly ConcurrentDictionary<string, Lazy<Task<string?>>> generated = new(StringComparer.OrdinalIgnoreCase);

		/// <param name="assemblies">The assemblies to serve; queried on every request.</param>
		/// <param name="settings">The settings used to decompile generated PDBs.</param>
		/// <param name="cacheDirectory">Where generated PDBs are written.</param>
		public DecompiledSymbolStore(Func<IEnumerable<LoadedAssembly>> assemblies,
			Func<DecompilerSettings> settings, string cacheDirectory)
		{
			this.assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
			this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
			this.cacheDirectory = cacheDirectory ?? throw new ArgumentNullException(nameof(cacheDirectory));
		}

		/// <summary>Raised when a PDB is generated, with the assembly file name and the PDB path.</summary>
		public event Action<string, string>? PdbGenerated;

		public async Task<string?> GetFileAsync(string key, CancellationToken cancellationToken)
		{
			foreach (var assembly in assemblies())
			{
				if (await assembly.GetMetadataFileOrNullAsync().ConfigureAwait(false) is not PEFile module)
					continue;
				var peKey = SymbolKey.GetPEKey(module.Reader, module.FileName);
				if (peKey != null && peKey.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
					return assembly.ParentBundle == null && File.Exists(assembly.FileName) ? assembly.FileName : null;
				foreach (var pdbKey in SymbolKey.GetPdbKeys(module.Reader))
				{
					if (pdbKey.Kind != SymbolFileKind.PortablePdb || !pdbKey.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
						continue;
					var existing = assembly.GetDebugInfoOrNull()?.SourceFileName ?? assembly.PdbFileName;
					if (existing != null && File.Exists(existing) && SymbolLocator.Matches(existing, pdbKey))
						return existing;
					return await GetOrGenerateAsync(assembly, module, pdbKey, cancellationToken).ConfigureAwait(false);
				}
			}
			return null;
		}

		Task<string?> GetOrGenerateAsync(LoadedAssembly assembly, PEFile module, SymbolKey key, CancellationToken cancellationToken)
		{
			var lazy = generated.GetOrAdd(key.Key, k => new Lazy<Task<string?>>(
				() => Task.Run(() => Generate(assembly, module, key, CancellationToken.None))));
			var task = lazy.Value;
			if (task.IsFaulted)
				generated.TryRemove(new KeyValuePair<string, Lazy<Task<string?>>>(key.Key, lazy));
			return task.WaitAsync(cancellationToken);
		}

		string? Generate(LoadedAssembly assembly, PEFile module, SymbolKey key, CancellationToken cancellationToken)
		{
			string path = Path.Combine(cacheDirectory, key.Key.Replace('/', Path.DirectorySeparatorChar));
			if (File.Exists(path) && SymbolLocator.Matches(path, key))
				return path;
			Directory.CreateDirectory(Path.GetDirectoryName(path)!);
			string tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
			try
			{
				var decompilerSettings = settings();
				using (var stream = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write))
				{
					var decompiler = new CSharpDecompiler(module, assembly.GetAssemblyResolver(), decompilerSettings) {
						CancellationToken = cancellationToken
					};
					new PortablePdbWriter().WritePdb(module, decompiler, decompilerSettings, stream);
				}
				File.Move(tempPath, path, overwrite: true);
				PdbGenerated?.Invoke(assembly.FileName, path);
				return path;
			}
			finally
			{
				if (File.Exists(tempPath))
					File.Delete(tempPath);
			}
		}
	}
}
