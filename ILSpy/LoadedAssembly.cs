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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.PdbProvider;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.ILSpy.Options;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Represents an assembly loaded into ILSpy.
	/// </summary>
	[DebuggerDisplay("[LoadedAssembly {shortName}]")]
	public sealed class LoadedAssembly
	{
		internal static readonly ConditionalWeakTable<PEFile, LoadedAssembly> loadedAssemblies = new ConditionalWeakTable<PEFile, LoadedAssembly>();

		readonly Task<PEFile> assemblyTask;
		readonly AssemblyList assemblyList;
		readonly string fileName;
		readonly string shortName;

		public LoadedAssembly(AssemblyList assemblyList, string fileName, Stream stream = null)
		{
			this.assemblyList = assemblyList ?? throw new ArgumentNullException(nameof(assemblyList));
			this.fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));

			this.assemblyTask = Task.Factory.StartNew(LoadAssembly, stream); // requires that this.fileName is set
			this.shortName = Path.GetFileNameWithoutExtension(fileName);
			this.resolver = new MyAssemblyResolver(this);
		}

		/// <summary>
		/// Returns a target framework identifier in the form '&lt;framework&gt;Version=v&lt;version&gt;'.
		/// Returns an empty string if no TargetFrameworkAttribute was found or the file doesn't contain an assembly header, i.e., is only a module.
		/// </summary>
		public async Task<string> GetTargetFrameworkIdAsync()
		{
			var assembly = await GetPEFileAsync().ConfigureAwait(false);
			return assembly.Reader.DetectTargetFrameworkId() ?? string.Empty;
		}

		public ReferenceLoadInfo LoadedAssemblyReferencesInfo { get; } = new ReferenceLoadInfo();

		IDebugInfoProvider debugInfoProvider;

		/// <summary>
		/// Gets the <see cref="PEFile"/>.
		/// </summary>
		public Task<PEFile> GetPEFileAsync()
		{
			return assemblyTask;
		}

		/// <summary>
		/// Gets the <see cref="PEFile"/>.
		/// Returns null in case of load errors.
		/// </summary>
		public PEFile GetPEFileOrNull()
		{
			try {
				return GetPEFileAsync().GetAwaiter().GetResult();
			} catch (Exception ex) {
				System.Diagnostics.Trace.TraceError(ex.ToString());
				return null;
			}
		}

		ICompilation typeSystem;

		/// <summary>
		/// Gets a type system containing all types from this assembly + primitve types from mscorlib.
		/// Returns null in case of load errors.
		/// </summary>
		/// <remarks>
		/// This is an uncached type system.
		/// </remarks>
		public ICompilation GetTypeSystemOrNull()
		{
			if (typeSystem != null)
				return typeSystem;
			var module = GetPEFileOrNull();
			if (module == null)
				return null;
			return typeSystem = new SimpleCompilation(
				module.WithOptions(TypeSystemOptions.Default | TypeSystemOptions.Uncached | TypeSystemOptions.KeepModifiers),
				MinimalCorlib.Instance);
		}

		ICompilation typeSystemWithOptions;
		TypeSystemOptions currentTypeSystemOptions;

		public ICompilation GetTypeSystemOrNull(TypeSystemOptions options)
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

		public AssemblyList AssemblyList => assemblyList;

		public string FileName => fileName;

		public string ShortName => shortName;

		public string Text {
			get {
				if (IsLoaded && !HasLoadError) {
					var metadata = GetPEFileOrNull()?.Metadata;
					string versionOrInfo = null;
					if (metadata != null) {
						if (metadata.IsAssembly) {
							versionOrInfo = metadata.GetAssemblyDefinition().Version?.ToString();
						} else {
							versionOrInfo = ".netmodule";
						}
					}
					if (versionOrInfo == null)
						return ShortName;
					return string.Format("{0} ({1})", ShortName, versionOrInfo);
				} else {
					return ShortName;
				}
			}
		}

		public bool IsLoaded => assemblyTask.IsCompleted;

		public bool HasLoadError => assemblyTask.IsFaulted;

		public bool IsAutoLoaded { get; set; }

		public string PdbFileOverride { get; set; }

		PEFile LoadAssembly(object state)
		{
			MetadataReaderOptions options;
			if (DecompilerSettingsPanel.CurrentDecompilerSettings.ApplyWindowsRuntimeProjections) {
				options = MetadataReaderOptions.ApplyWindowsRuntimeProjections;
			} else {
				options = MetadataReaderOptions.None;
			}

			PEFile module;
			// runs on background thread
			if (state is Stream stream) {
				// Read the module from a precrafted stream
				var streamOptions = stream is MemoryStream ? PEStreamOptions.PrefetchEntireImage : PEStreamOptions.Default;
				module = new PEFile(fileName, stream, streamOptions, metadataOptions: options);
			} else {
				// Read the module from disk (by default)
				stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				module = new PEFile(fileName, stream, PEStreamOptions.PrefetchEntireImage,
					metadataOptions: options);
			}

			if (DecompilerSettingsPanel.CurrentDecompilerSettings.UseDebugSymbols) {
				try {
					debugInfoProvider = DebugInfoUtils.FromFile(module, PdbFileOverride)
						?? DebugInfoUtils.LoadSymbols(module);
				} catch (IOException) {
				} catch (UnauthorizedAccessException) {
				} catch (InvalidOperationException) {
					// ignore any errors during symbol loading
				}
			}
			lock (loadedAssemblies) {
				loadedAssemblies.Add(module, this);
			}
			return module;
		}

		[ThreadStatic]
		static int assemblyLoadDisableCount;

		public static IDisposable DisableAssemblyLoad(AssemblyList assemblyList)
		{
			assemblyLoadDisableCount++;
			return new DecrementAssemblyLoadDisableCount(assemblyList);
		}

		public static IDisposable DisableAssemblyLoad()
		{
			assemblyLoadDisableCount++;
			return new DecrementAssemblyLoadDisableCount(MainWindow.Instance.CurrentAssemblyList);
		}

		sealed class DecrementAssemblyLoadDisableCount : IDisposable
		{
			bool disposed;
			AssemblyList assemblyList;

			public DecrementAssemblyLoadDisableCount(AssemblyList assemblyList)
			{
				this.assemblyList = assemblyList;
			}

			public void Dispose()
			{
				if (!disposed) {
					disposed = true;
					assemblyLoadDisableCount--;
					// clear the lookup cache since we might have stored the lookups failed due to DisableAssemblyLoad()
					assemblyList.ClearCache();
				}
			}
		}

		sealed class MyAssemblyResolver : IAssemblyResolver
		{
			readonly LoadedAssembly parent;

			public MyAssemblyResolver(LoadedAssembly parent)
			{
				this.parent = parent;
			}

			public PEFile Resolve(Decompiler.Metadata.IAssemblyReference reference)
			{
				return parent.LookupReferencedAssembly(reference)?.GetPEFileOrNull();
			}

			public PEFile ResolveModule(PEFile mainModule, string moduleName)
			{
				return parent.LookupReferencedModule(mainModule, moduleName)?.GetPEFileOrNull();
			}
		}

		readonly MyAssemblyResolver resolver;

		public IAssemblyResolver GetAssemblyResolver()
		{
			return resolver;
		}

		/// <summary>
		/// Returns the debug info for this assembly. Returns null in case of load errors or no debug info is available.
		/// </summary>
		public IDebugInfoProvider GetDebugInfoOrNull()
		{
			if (GetPEFileOrNull() == null)
				return null;
			return debugInfoProvider;
		}

		public LoadedAssembly LookupReferencedAssembly(Decompiler.Metadata.IAssemblyReference reference)
		{
			if (reference == null)
				throw new ArgumentNullException(nameof(reference));
			if (reference.IsWindowsRuntime) {
				return assemblyList.assemblyLookupCache.GetOrAdd((reference.Name, true), key => LookupReferencedAssemblyInternal(reference, true));
			} else {
				return assemblyList.assemblyLookupCache.GetOrAdd((reference.FullName, false), key => LookupReferencedAssemblyInternal(reference, false));
			}
		}

		public LoadedAssembly LookupReferencedModule(PEFile mainModule, string moduleName)
		{
			if (mainModule == null)
				throw new ArgumentNullException(nameof(mainModule));
			if (moduleName == null)
				throw new ArgumentNullException(nameof(moduleName));
			return assemblyList.moduleLookupCache.GetOrAdd(mainModule.FileName + ";" + moduleName, _ => LookupReferencedModuleInternal(mainModule, moduleName));
		}

		class MyUniversalResolver : UniversalAssemblyResolver
		{
			public MyUniversalResolver(LoadedAssembly assembly)
				: base(assembly.FileName, false, assembly.GetTargetFrameworkIdAsync().Result, PEStreamOptions.PrefetchEntireImage, DecompilerSettingsPanel.CurrentDecompilerSettings.ApplyWindowsRuntimeProjections ? MetadataReaderOptions.ApplyWindowsRuntimeProjections : MetadataReaderOptions.None)
			{
			}
		}

		static readonly Dictionary<string, LoadedAssembly> loadingAssemblies = new Dictionary<string, LoadedAssembly>();
		MyUniversalResolver universalResolver;

		LoadedAssembly LookupReferencedAssemblyInternal(Decompiler.Metadata.IAssemblyReference fullName, bool isWinRT)
		{
			string GetName(Decompiler.Metadata.IAssemblyReference name) => isWinRT ? name.Name : name.FullName;

			string file;
			LoadedAssembly asm;
			lock (loadingAssemblies) {
				foreach (LoadedAssembly loaded in assemblyList.GetAssemblies()) {
					var reader = loaded.GetPEFileOrNull()?.Metadata;
					if (reader == null || !reader.IsAssembly) continue;
					var asmDef = reader.GetAssemblyDefinition();
					var asmDefName = isWinRT ? reader.GetString(asmDef.Name) : reader.GetFullAssemblyName();
					if (GetName(fullName).Equals(asmDefName, StringComparison.OrdinalIgnoreCase)) {
						LoadedAssemblyReferencesInfo.AddMessageOnce(fullName.FullName, MessageKind.Info, "Success - Found in Assembly List");
						return loaded;
					}
				}

				if (universalResolver == null) {
					universalResolver = new MyUniversalResolver(this);
				}

				file = universalResolver.FindAssemblyFile(fullName);

				foreach (LoadedAssembly loaded in assemblyList.GetAssemblies()) {
					if (loaded.FileName.Equals(file, StringComparison.OrdinalIgnoreCase)) {
						return loaded;
					}
				}

				if (file != null && loadingAssemblies.TryGetValue(file, out asm))
					return asm;

				if (assemblyLoadDisableCount > 0)
					return null;

				if (file != null) {
					LoadedAssemblyReferencesInfo.AddMessage(fullName.ToString(), MessageKind.Info, "Success - Loading from: " + file);
					asm = new LoadedAssembly(assemblyList, file) { IsAutoLoaded = true };
				} else {
					LoadedAssemblyReferencesInfo.AddMessageOnce(fullName.ToString(), MessageKind.Error, "Could not find reference: " + fullName);
					return null;
				}
				loadingAssemblies.Add(file, asm);
			}
			App.Current.Dispatcher.BeginInvoke((Action)delegate () {
				lock (assemblyList.assemblies) {
					assemblyList.assemblies.Add(asm);
				}
				lock (loadingAssemblies) {
					loadingAssemblies.Remove(file);
				}
			});
			return asm;
		}

		LoadedAssembly LookupReferencedModuleInternal(PEFile mainModule, string moduleName)
		{
			string file;
			LoadedAssembly asm;
			lock (loadingAssemblies) {
				foreach (LoadedAssembly loaded in assemblyList.GetAssemblies()) {
					var reader = loaded.GetPEFileOrNull()?.Metadata;
					if (reader == null || reader.IsAssembly) continue;
					var moduleDef = reader.GetModuleDefinition();
					if (moduleName.Equals(reader.GetString(moduleDef.Name), StringComparison.OrdinalIgnoreCase)) {
						LoadedAssemblyReferencesInfo.AddMessageOnce(moduleName, MessageKind.Info, "Success - Found in Assembly List");
						return loaded;
					}
				}

				file = Path.Combine(Path.GetDirectoryName(mainModule.FileName), moduleName);
				if (!File.Exists(file))
					return null;

				foreach (LoadedAssembly loaded in assemblyList.GetAssemblies()) {
					if (loaded.FileName.Equals(file, StringComparison.OrdinalIgnoreCase)) {
						return loaded;
					}
				}

				if (file != null && loadingAssemblies.TryGetValue(file, out asm))
					return asm;

				if (assemblyLoadDisableCount > 0)
					return null;

				if (file != null) {
					LoadedAssemblyReferencesInfo.AddMessage(moduleName, MessageKind.Info, "Success - Loading from: " + file);
					asm = new LoadedAssembly(assemblyList, file) { IsAutoLoaded = true };
				} else {
					LoadedAssemblyReferencesInfo.AddMessageOnce(moduleName, MessageKind.Error, "Could not find reference: " + moduleName);
					return null;
				}
				loadingAssemblies.Add(file, asm);
			}
			App.Current.Dispatcher.BeginInvoke((Action)delegate () {
				lock (assemblyList.assemblies) {
					assemblyList.assemblies.Add(asm);
				}
				lock (loadingAssemblies) {
					loadingAssemblies.Remove(file);
				}
			});
			return asm;
		}

		public Task ContinueWhenLoaded(Action<Task<PEFile>> onAssemblyLoaded, TaskScheduler taskScheduler)
		{
			return this.assemblyTask.ContinueWith(onAssemblyLoaded, default(CancellationToken), TaskContinuationOptions.RunContinuationsAsynchronously, taskScheduler);
		}

		/// <summary>
		/// Wait until the assembly is loaded.
		/// Throws an AggregateException when loading the assembly fails.
		/// </summary>
		public void WaitUntilLoaded()
		{
			assemblyTask.Wait();
		}

	}
}
