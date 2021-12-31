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

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.PdbProvider;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpy.Options;

using K4os.Compression.LZ4;

#nullable enable

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Represents a file loaded into ILSpy.
	/// 
	/// Note: this class is misnamed.
	/// The file is not necessarily an assembly, nor is it necessarily loaded.
	/// 
	/// A LoadedAssembly can refer to:
	///   * a .NET module (single-file) loaded into ILSpy
	///   * a non-existant file
	///   * a file of unknown format that could not be loaded
	///   * a .nupkg file or .NET core bundle
	///   * a file that is still being loaded in the background
	/// </summary>
	[DebuggerDisplay("[LoadedAssembly {shortName}]")]
	public sealed class LoadedAssembly
	{
		/// <summary>
		/// Maps from PEFile (successfully loaded .NET module) back to the LoadedAssembly instance
		/// that was used to load the module.
		/// </summary>
		internal static readonly ConditionalWeakTable<PEFile, LoadedAssembly> loadedAssemblies = new ConditionalWeakTable<PEFile, LoadedAssembly>();

		public sealed class LoadResult
		{
			public PEFile? PEFile { get; }
			public Exception? PEFileLoadException { get; }
			public LoadedPackage? Package { get; }

			public LoadResult(PEFile peFile)
			{
				this.PEFile = peFile ?? throw new ArgumentNullException(nameof(peFile));
			}
			public LoadResult(Exception peFileLoadException, LoadedPackage package)
			{
				this.PEFileLoadException = peFileLoadException ?? throw new ArgumentNullException(nameof(peFileLoadException));
				this.Package = package ?? throw new ArgumentNullException(nameof(package));
			}
		}

		readonly Task<LoadResult> loadingTask;
		readonly AssemblyList assemblyList;
		readonly string fileName;
		readonly string shortName;
		readonly IAssemblyResolver? providedAssemblyResolver;

		public LoadedAssembly? ParentBundle { get; }

		public LoadedAssembly(AssemblyList assemblyList, string fileName,
			Task<Stream?>? stream = null, IAssemblyResolver? assemblyResolver = null, string? pdbFileName = null)
		{
			this.assemblyList = assemblyList ?? throw new ArgumentNullException(nameof(assemblyList));
			this.fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
			this.PdbFileName = pdbFileName;
			this.providedAssemblyResolver = assemblyResolver;

			this.loadingTask = Task.Run(() => LoadAsync(stream)); // requires that this.fileName is set
			this.shortName = Path.GetFileNameWithoutExtension(fileName);
		}

		public LoadedAssembly(LoadedAssembly bundle, string fileName, Task<Stream?>? stream, IAssemblyResolver? assemblyResolver = null)
			: this(bundle.assemblyList, fileName, stream, assemblyResolver)
		{
			this.ParentBundle = bundle;
		}

		string? targetFrameworkId;

		/// <summary>
		/// Returns a target framework identifier in the form '&lt;framework&gt;Version=v&lt;version&gt;'.
		/// Returns an empty string if no TargetFrameworkAttribute was found
		/// or the file doesn't contain an assembly header, i.e., is only a module.
		/// 
		/// Throws an exception if the file does not contain any .NET metadata (e.g. file of unknown format).
		/// </summary>
		public async Task<string> GetTargetFrameworkIdAsync()
		{
			var value = LazyInit.VolatileRead(ref targetFrameworkId);
			if (value == null)
			{
				var assembly = await GetPEFileAsync().ConfigureAwait(false);
				value = assembly.DetectTargetFrameworkId() ?? string.Empty;
				value = LazyInit.GetOrSet(ref targetFrameworkId, value);
			}

			return value;
		}

		string? runtimePack;

		public async Task<string> GetRuntimePackAsync()
		{
			var value = LazyInit.VolatileRead(ref runtimePack);
			if (value == null)
			{
				var assembly = await GetPEFileAsync().ConfigureAwait(false);
				value = assembly.DetectRuntimePack() ?? string.Empty;
				value = LazyInit.GetOrSet(ref runtimePack, value);
			}

			return value;
		}

		public ReferenceLoadInfo LoadedAssemblyReferencesInfo { get; } = new ReferenceLoadInfo();

		IDebugInfoProvider? debugInfoProvider;

		/// <summary>
		/// Gets the <see cref="LoadResult"/>.
		/// </summary>
		public Task<LoadResult> GetLoadResultAsync()
		{
			return loadingTask;
		}

		/// <summary>
		/// Gets the <see cref="PEFile"/>.
		/// </summary>
		public async Task<PEFile> GetPEFileAsync()
		{
			var loadResult = await loadingTask.ConfigureAwait(false);
			if (loadResult.PEFile != null)
				return loadResult.PEFile;
			else
				throw loadResult.PEFileLoadException!;
		}

		/// <summary>
		/// Gets the <see cref="PEFile"/>.
		/// Returns null in case of load errors.
		/// </summary>
		public PEFile? GetPEFileOrNull()
		{
			try
			{
				var loadResult = loadingTask.GetAwaiter().GetResult();
				return loadResult.PEFile;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.TraceError(ex.ToString());
				return null;
			}
		}

		/// <summary>
		/// Gets the <see cref="PEFile"/>.
		/// Returns null in case of load errors.
		/// </summary>
		public async Task<PEFile?> GetPEFileOrNullAsync()
		{
			try
			{
				var loadResult = await loadingTask.ConfigureAwait(false);
				return loadResult.PEFile;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.TraceError(ex.ToString());
				return null;
			}
		}

		ICompilation? typeSystem;

		/// <summary>
		/// Gets a type system containing all types from this assembly + primitive types from mscorlib.
		/// Returns null in case of load errors.
		/// </summary>
		/// <remarks>
		/// This is an uncached type system.
		/// </remarks>
		public ICompilation? GetTypeSystemOrNull()
		{
			return LazyInitializer.EnsureInitialized(ref this.typeSystem, () => {
				var module = GetPEFileOrNull();
				if (module == null)
					return null;
				return new SimpleCompilation(
					module.WithOptions(TypeSystemOptions.Default | TypeSystemOptions.Uncached | TypeSystemOptions.KeepModifiers),
					MinimalCorlib.Instance);
			});
		}

		readonly object typeSystemWithOptionsLockObj = new object();
		ICompilation? typeSystemWithOptions;
		TypeSystemOptions? currentTypeSystemOptions;

		public ICompilation? GetTypeSystemOrNull(TypeSystemOptions options)
		{
			lock (typeSystemWithOptionsLockObj)
			{
				if (typeSystemWithOptions != null && options == currentTypeSystemOptions)
					return typeSystemWithOptions;
				var module = GetPEFileOrNull();
				if (module == null)
					return null;
				currentTypeSystemOptions = options;
				return typeSystemWithOptions = new SimpleCompilation(
					module.WithOptions(options | TypeSystemOptions.Uncached | TypeSystemOptions.KeepModifiers),
					MinimalCorlib.Instance);
			}
		}

		public AssemblyList AssemblyList => assemblyList;

		public string FileName => fileName;

		public string ShortName => shortName;

		public string Text {
			get {
				if (IsLoaded && !HasLoadError)
				{
					PEFile? module = GetPEFileOrNull();
					var metadata = module?.Metadata;
					string? versionOrInfo = null;
					if (metadata != null)
					{
						if (metadata.IsAssembly)
						{
							versionOrInfo = metadata.GetAssemblyDefinition().Version?.ToString();
							var tfId = GetTargetFrameworkIdAsync().Result;
							if (!string.IsNullOrEmpty(tfId))
								versionOrInfo += ", " + tfId.Replace("Version=", " ");
						}
						else
						{
							versionOrInfo = ".netmodule";
						}
					}
					if (versionOrInfo == null)
						return ShortName;
					return string.Format("{0} ({1})", ShortName, versionOrInfo);
				}
				else
				{
					return ShortName;
				}
			}
		}

		/// <summary>
		/// Gets whether loading finished for this file (either successfully or unsuccessfully).
		/// </summary>
		public bool IsLoaded => loadingTask.IsCompleted;

		/// <summary>
		/// Gets whether this file was loaded successfully as an assembly (not as a bundle).
		/// </summary>
		public bool IsLoadedAsValidAssembly {
			get {
				return loadingTask.Status == TaskStatus.RanToCompletion && loadingTask.Result.PEFile != null;
			}
		}

		/// <summary>
		/// Gets whether loading failed (file does not exist, unknown file format).
		/// Returns false for valid assemblies and valid bundles.
		/// </summary>
		public bool HasLoadError => loadingTask.IsFaulted;

		public bool IsAutoLoaded { get; set; }

		/// <summary>
		/// Gets the PDB file name or null, if no PDB was found or it's embedded.
		/// </summary>
		public string? PdbFileName { get; private set; }

		async Task<LoadResult> LoadAsync(Task<Stream?>? streamTask)
		{
			// runs on background thread
			var stream = streamTask != null ? await streamTask.ConfigureAwait(false) : null;
			if (stream != null)
			{
				// Read the module from a precrafted stream
				if (!stream.CanSeek)
				{
					var memoryStream = new MemoryStream();
					stream.CopyTo(memoryStream);
					stream.Close();
					memoryStream.Position = 0;
					stream = memoryStream;
				}
				var streamOptions = stream is MemoryStream ? PEStreamOptions.PrefetchEntireImage : PEStreamOptions.Default;
				return LoadAssembly(stream, streamOptions);
			}
			// Read the module from disk
			Exception loadAssemblyException;
			try
			{
				using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					return LoadAssembly(fileStream, PEStreamOptions.PrefetchEntireImage);
				}
			}
			catch (PEFileNotSupportedException ex)
			{
				loadAssemblyException = ex;
			}
			catch (BadImageFormatException ex)
			{
				loadAssemblyException = ex;
			}
			// Maybe its a compressed Xamarin/Mono assembly, see https://github.com/xamarin/xamarin-android/pull/4686
			try
			{
				return LoadCompressedAssembly(fileName);
			}
			catch (InvalidDataException)
			{
				// Not a compressed module, try other options below
			}
			// If it's not a .NET module, maybe it's a single-file bundle
			var bundle = LoadedPackage.FromBundle(fileName);
			if (bundle != null)
			{
				bundle.LoadedAssembly = this;
				return new LoadResult(loadAssemblyException, bundle);
			}
			// If it's not a .NET module, maybe it's a zip archive (e.g. .nupkg)
			try
			{
				var zip = LoadedPackage.FromZipFile(fileName);
				zip.LoadedAssembly = this;
				return new LoadResult(loadAssemblyException, zip);
			}
			catch (InvalidDataException)
			{
				throw loadAssemblyException;
			}
		}

		LoadResult LoadAssembly(Stream stream, PEStreamOptions streamOptions)
		{
			MetadataReaderOptions options;
			if (DecompilerSettingsPanel.CurrentDecompilerSettings.ApplyWindowsRuntimeProjections)
			{
				options = MetadataReaderOptions.ApplyWindowsRuntimeProjections;
			}
			else
			{
				options = MetadataReaderOptions.None;
			}

			PEFile module = new PEFile(fileName, stream, streamOptions, metadataOptions: options);

			debugInfoProvider = LoadDebugInfo(module);
			lock (loadedAssemblies)
			{
				loadedAssemblies.Add(module, this);
			}
			return new LoadResult(module);
		}

		LoadResult LoadCompressedAssembly(string fileName)
		{
			const uint CompressedDataMagic = 0x5A4C4158; // Magic used for Xamarin compressed module header ('XALZ', little-endian)
			using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			using (var fileReader = new BinaryReader(fileStream))
			{
				// Read compressed file header
				var magic = fileReader.ReadUInt32();
				if (magic != CompressedDataMagic)
					throw new InvalidDataException($"Xamarin compressed module header magic {magic} does not match expected {CompressedDataMagic}");
				_ = fileReader.ReadUInt32(); // skip index into descriptor table, unused
				int uncompressedLength = (int)fileReader.ReadUInt32();
				int compressedLength = (int)fileStream.Length;  // Ensure we read all of compressed data
				ArrayPool<byte> pool = ArrayPool<byte>.Shared;
				var src = pool.Rent(compressedLength);
				var dst = pool.Rent(uncompressedLength);
				try
				{
					// fileReader stream position is now at compressed module data
					fileStream.Read(src, 0, compressedLength);
					// Decompress
					LZ4Codec.Decode(src, 0, compressedLength, dst, 0, uncompressedLength);
					// Load module from decompressed data buffer
					using (var uncompressedStream = new MemoryStream(dst, writable: false))
					{
						return LoadAssembly(uncompressedStream, PEStreamOptions.PrefetchEntireImage);
					}
				}
				finally
				{
					pool.Return(dst);
					pool.Return(src);
				}
			}
		}

		IDebugInfoProvider? LoadDebugInfo(PEFile module)
		{
			if (DecompilerSettingsPanel.CurrentDecompilerSettings.UseDebugSymbols)
			{
				try
				{
					return DebugInfoUtils.FromFile(module, PdbFileName)
						?? DebugInfoUtils.LoadSymbols(module);
				}
				catch (IOException)
				{
				}
				catch (UnauthorizedAccessException)
				{
				}
				catch (InvalidOperationException)
				{
					// ignore any errors during symbol loading
				}
			}
			return null;
		}

		public async Task<IDebugInfoProvider?> LoadDebugInfo(string fileName)
		{
			this.PdbFileName = fileName;
			var assembly = await GetPEFileAsync().ConfigureAwait(false);
			debugInfoProvider = await Task.Run(() => LoadDebugInfo(assembly));
			return debugInfoProvider;
		}

		sealed class MyAssemblyResolver : IAssemblyResolver
		{
			readonly LoadedAssembly parent;
			readonly bool loadOnDemand;

			readonly IAssemblyResolver? providedAssemblyResolver;
			readonly AssemblyList assemblyList;
			readonly LoadedAssembly[] alreadyLoadedAssemblies;
			readonly Task<string> tfmTask;
			readonly ReferenceLoadInfo referenceLoadInfo;

			public MyAssemblyResolver(LoadedAssembly parent, bool loadOnDemand)
			{
				this.parent = parent;
				this.loadOnDemand = loadOnDemand;

				this.providedAssemblyResolver = parent.providedAssemblyResolver;
				this.assemblyList = parent.assemblyList;
				// Note: we cache a copy of the assembly list in the constructor, so that the
				// resolve calls only search-by-asm-name in the assemblies that were already loaded
				// at the time of the GetResolver() call.
				this.alreadyLoadedAssemblies = assemblyList.GetAssemblies();
				// If we didn't do this, we'd also search in the assemblies that we just started to load
				// in previous Resolve() calls; but we don't want to wait for those to be loaded.
				this.tfmTask = parent.GetTargetFrameworkIdAsync();
				this.referenceLoadInfo = parent.LoadedAssemblyReferencesInfo;
			}

			public PEFile? Resolve(IAssemblyReference reference)
			{
				return ResolveAsync(reference).GetAwaiter().GetResult();
			}

			Dictionary<string, PEFile>? asmLookupByFullName;
			Dictionary<string, PEFile>? asmLookupByShortName;

			/// <summary>
			/// 0) if we're inside a package, look for filename.dll in parent directories
			/// 1) try to find exact match by tfm + full asm name in loaded assemblies
			/// 2) try to find match in search paths
			/// 3) if a.deps.json is found: search %USERPROFILE%/.nuget/packages/* as well
			/// 4) look in /dotnet/shared/{runtime-pack}/{closest-version}
			/// 5) if the version is retargetable or all zeros or ones, search C:\Windows\Microsoft.NET\Framework64\v4.0.30319
			/// 6) For "mscorlib.dll" we use the exact same assembly with which ILSpy runs
			/// 7) Search the GAC
			/// 8) search C:\Windows\Microsoft.NET\Framework64\v4.0.30319
			/// 9) try to find match by asm name (no tfm/version) in loaded assemblies
			/// </summary>
			public async Task<PEFile?> ResolveAsync(IAssemblyReference reference)
			{
				PEFile? module;
				// 0) if we're inside a package, look for filename.dll in parent directories
				if (providedAssemblyResolver != null)
				{
					module = await providedAssemblyResolver.ResolveAsync(reference).ConfigureAwait(false);
					if (module != null)
						return module;
				}

				string tfm = await tfmTask.ConfigureAwait(false);

				bool isWinRT = reference.IsWindowsRuntime;
				string key = tfm + ";" + (isWinRT ? reference.Name : reference.FullName);

				// 1) try to find exact match by tfm + full asm name in loaded assemblies
				var lookup = LazyInit.VolatileRead(ref isWinRT ? ref asmLookupByShortName : ref asmLookupByFullName);
				if (lookup == null)
				{
					lookup = await CreateLoadedAssemblyLookupAsync(shortNames: isWinRT).ConfigureAwait(false);
					lookup = LazyInit.GetOrSet(ref isWinRT ? ref asmLookupByShortName : ref asmLookupByFullName, lookup);
				}
				if (lookup.TryGetValue(key, out module))
				{
					referenceLoadInfo.AddMessageOnce(reference.FullName, MessageKind.Info, "Success - Found in Assembly List");
					return module;
				}

				string? file = parent.GetUniversalResolver().FindAssemblyFile(reference);

				if (file != null)
				{
					// Load assembly from disk
					LoadedAssembly? asm;
					if (loadOnDemand)
					{
						asm = assemblyList.OpenAssembly(file, isAutoLoaded: true);
					}
					else
					{
						asm = assemblyList.FindAssembly(file);
					}
					if (asm != null)
					{
						referenceLoadInfo.AddMessage(reference.ToString(), MessageKind.Info, "Success - Loading from: " + file);
						return await asm.GetPEFileOrNullAsync().ConfigureAwait(false);
					}
					return null;
				}
				else
				{
					// Assembly not found; try to find a similar-enough already-loaded assembly
					var candidates = new List<(LoadedAssembly assembly, Version version)>();

					foreach (LoadedAssembly loaded in alreadyLoadedAssemblies)
					{
						module = await loaded.GetPEFileOrNullAsync().ConfigureAwait(false);
						var reader = module?.Metadata;
						if (reader == null || !reader.IsAssembly)
							continue;
						var asmDef = reader.GetAssemblyDefinition();
						var asmDefName = reader.GetString(asmDef.Name);
						if (reference.Name.Equals(asmDefName, StringComparison.OrdinalIgnoreCase))
						{
							candidates.Add((loaded, asmDef.Version));
						}
					}

					if (candidates.Count == 0)
					{
						referenceLoadInfo.AddMessageOnce(reference.ToString(), MessageKind.Error, "Could not find reference: " + reference);
						return null;
					}

					candidates.SortBy(c => c.version);

					var bestCandidate = candidates.FirstOrDefault(c => c.version >= reference.Version).assembly ?? candidates.Last().assembly;
					referenceLoadInfo.AddMessageOnce(reference.ToString(), MessageKind.Info, "Success - Found in Assembly List with different TFM or version: " + bestCandidate.fileName);
					return await bestCandidate.GetPEFileOrNullAsync().ConfigureAwait(false);
				}
			}

			private async Task<Dictionary<string, PEFile>> CreateLoadedAssemblyLookupAsync(bool shortNames)
			{
				var result = new Dictionary<string, PEFile>(StringComparer.OrdinalIgnoreCase);
				foreach (LoadedAssembly loaded in alreadyLoadedAssemblies)
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

			public PEFile? ResolveModule(PEFile mainModule, string moduleName)
			{
				return ResolveModuleAsync(mainModule, moduleName).GetAwaiter().GetResult();
			}

			public async Task<PEFile?> ResolveModuleAsync(PEFile mainModule, string moduleName)
			{
				if (providedAssemblyResolver != null)
				{
					var module = await providedAssemblyResolver.ResolveModuleAsync(mainModule, moduleName).ConfigureAwait(false);
					if (module != null)
						return module;
				}


				string file = Path.Combine(Path.GetDirectoryName(mainModule.FileName), moduleName);
				if (File.Exists(file))
				{
					// Load module from disk
					LoadedAssembly? asm;
					if (loadOnDemand)
					{
						asm = assemblyList.OpenAssembly(file, isAutoLoaded: true);
					}
					else
					{
						asm = assemblyList.FindAssembly(file);
					}
					if (asm != null)
					{
						return await asm.GetPEFileOrNullAsync().ConfigureAwait(false);
					}
				}
				else
				{
					// Module does not exist on disk, look for one with a matching name in the assemblylist:
					foreach (LoadedAssembly loaded in alreadyLoadedAssemblies)
					{
						var module = await loaded.GetPEFileOrNullAsync().ConfigureAwait(false);
						var reader = module?.Metadata;
						if (reader == null || reader.IsAssembly)
							continue;
						var moduleDef = reader.GetModuleDefinition();
						if (moduleName.Equals(reader.GetString(moduleDef.Name), StringComparison.OrdinalIgnoreCase))
						{
							referenceLoadInfo.AddMessageOnce(moduleName, MessageKind.Info, "Success - Found in Assembly List");
							return module;
						}
					}
				}
				return null;
			}
		}

		public IAssemblyResolver GetAssemblyResolver(bool loadOnDemand = true)
		{
			return new MyAssemblyResolver(this, loadOnDemand);
		}

		private UniversalAssemblyResolver GetUniversalResolver()
		{
			return LazyInitializer.EnsureInitialized(ref this.universalResolver, () => {
				var targetFramework = this.GetTargetFrameworkIdAsync().Result;
				var runtimePack = this.GetRuntimePackAsync().Result;

				var readerOptions = DecompilerSettingsPanel.CurrentDecompilerSettings.ApplyWindowsRuntimeProjections
					? MetadataReaderOptions.ApplyWindowsRuntimeProjections
					: MetadataReaderOptions.None;

				var rootedPath = Path.IsPathRooted(this.FileName) ? this.FileName : null;

				return new UniversalAssemblyResolver(rootedPath, throwOnError: false, targetFramework,
					runtimePack, PEStreamOptions.PrefetchEntireImage, readerOptions);
			})!;
		}

		public AssemblyReferenceClassifier GetAssemblyReferenceClassifier()
		{
			return GetUniversalResolver();
		}

		/// <summary>
		/// Returns the debug info for this assembly. Returns null in case of load errors or no debug info is available.
		/// </summary>
		public IDebugInfoProvider? GetDebugInfoOrNull()
		{
			if (GetPEFileOrNull() == null)
				return null;
			return debugInfoProvider;
		}

		UniversalAssemblyResolver? universalResolver;

		/// <summary>
		/// Wait until the assembly is loaded.
		/// Throws an AggregateException when loading the assembly fails.
		/// </summary>
		public void WaitUntilLoaded()
		{
			loadingTask.Wait();
		}
	}
}
