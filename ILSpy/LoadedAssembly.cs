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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Options;
using Mono.Cecil;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Represents an assembly loaded into ILSpy.
	/// </summary>
	public sealed class LoadedAssembly
	{
		readonly Task<ModuleDefinition> assemblyTask;
		readonly AssemblyList assemblyList;
		readonly string fileName;
		readonly string shortName;
		readonly Lazy<string> targetFrameworkId;
		readonly Dictionary<string, UnresolvedAssemblyNameReference> loadedAssemblyReferences = new Dictionary<string, UnresolvedAssemblyNameReference>();

		public LoadedAssembly(AssemblyList assemblyList, string fileName, Stream stream = null)
		{
			if (assemblyList == null)
				throw new ArgumentNullException(nameof(assemblyList));
			if (fileName == null)
				throw new ArgumentNullException(nameof(fileName));
			this.assemblyList = assemblyList;
			this.fileName = fileName;
			
			this.assemblyTask = Task.Factory.StartNew<ModuleDefinition>(LoadAssembly, stream); // requires that this.fileName is set
			this.shortName = Path.GetFileNameWithoutExtension(fileName);
			this.targetFrameworkId = new Lazy<string>(AssemblyDefinition.DetectTargetFrameworkId, false);
		}

		/// <summary>
		/// Returns a target framework identifier in the form '&lt;framework&gt;Version=v&lt;version&gt;'.
		/// </summary>
		public string TargetFrameworkId => targetFrameworkId.Value;

		public Dictionary<string, UnresolvedAssemblyNameReference> LoadedAssemblyReferencesInfo => loadedAssemblyReferences;

		/// <summary>
		/// Gets the Cecil ModuleDefinition.
		/// Can be null when there was a load error.
		/// </summary>
		public ModuleDefinition ModuleDefinition {
			get {
				try {
					return assemblyTask.Result;
				} catch (AggregateException) {
					return null;
				}
			}
		}
		
		/// <summary>
		/// Gets the Cecil AssemblyDefinition.
		/// Is null when there was a load error; or when opening a netmodule.
		/// </summary>
		public AssemblyDefinition AssemblyDefinition {
			get {
				var module = this.ModuleDefinition;
				return module != null ? module.Assembly : null;
			}
		}

		public AssemblyList AssemblyList => assemblyList;

		public string FileName => fileName;

		public string ShortName => shortName;

		public string Text {
			get {
				if (AssemblyDefinition != null) {
					return String.Format("{0} ({1})", ShortName, AssemblyDefinition.Name.Version);
				} else {
					return ShortName;
				}
			}
		}

		public bool IsLoaded => assemblyTask.IsCompleted;

		public bool HasLoadError => assemblyTask.IsFaulted;

		public bool IsAutoLoaded { get; set; }

		ModuleDefinition LoadAssembly(object state)
		{
			var stream = state as Stream;
			ModuleDefinition module;

			// runs on background thread
			ReaderParameters p = new ReaderParameters();
			p.AssemblyResolver = new MyAssemblyResolver(this);
			p.InMemory = true;

			if (stream != null)
			{
				// Read the module from a precrafted stream
				module = ModuleDefinition.ReadModule(stream, p);
			}
			else
			{
				// Read the module from disk (by default)
				module = ModuleDefinition.ReadModule(fileName, p);
			}

			if (DecompilerSettingsPanel.CurrentDecompilerSettings.UseDebugSymbols) {
				try {
					LoadSymbols(module);
				} catch (IOException) {
				} catch (UnauthorizedAccessException) {
				} catch (InvalidOperationException) {
					// ignore any errors during symbol loading
				}
			}
			return module;
		}
		
		private void LoadSymbols(ModuleDefinition module)
		{
			if (!module.HasDebugHeader) {
				return;
			}

			// search for pdb in same directory as dll
			string pdbName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".pdb");
			if (File.Exists(pdbName)) {
				using (Stream s = File.OpenRead(pdbName)) {
					module.ReadSymbols(new Mono.Cecil.Pdb.PdbReaderProvider().GetSymbolReader(module, s));
				}
				return;
			}
			
			// TODO: use symbol cache, get symbols from microsoft
		}
		
		[ThreadStatic]
		static int assemblyLoadDisableCount;
		
		public static IDisposable DisableAssemblyLoad()
		{
			assemblyLoadDisableCount++;
			return new DecrementAssemblyLoadDisableCount();
		}
		
		sealed class DecrementAssemblyLoadDisableCount : IDisposable
		{
			bool disposed;
			
			public void Dispose()
			{
				if (!disposed) {
					disposed = true;
					assemblyLoadDisableCount--;
					// clear the lookup cache since we might have stored the lookups failed due to DisableAssemblyLoad()
					MainWindow.Instance.CurrentAssemblyList.ClearCache();
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
			
			public AssemblyDefinition Resolve(AssemblyNameReference name)
			{
				var node = parent.LookupReferencedAssembly(name);
				return node != null ? node.AssemblyDefinition : null;
			}
			
			public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
			{
				var node = parent.LookupReferencedAssembly(name);
				return node != null ? node.AssemblyDefinition : null;
			}
			
			public AssemblyDefinition Resolve(string fullName)
			{
				var node = parent.LookupReferencedAssembly(fullName);
				return node != null ? node.AssemblyDefinition : null;
			}
			
			public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
			{
				var node = parent.LookupReferencedAssembly(fullName);
				return node != null ? node.AssemblyDefinition : null;
			}

			public void Dispose()
			{
			}
		}
		
		public IAssemblyResolver GetAssemblyResolver()
		{
			return new MyAssemblyResolver(this);
		}
		
		public LoadedAssembly LookupReferencedAssembly(AssemblyNameReference name)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (name.IsWindowsRuntime) {
				return assemblyList.winRTMetadataLookupCache.GetOrAdd(name.Name, LookupWinRTMetadata);
			} else {
				return assemblyList.assemblyLookupCache.GetOrAdd(name.FullName, LookupReferencedAssemblyInternal);
			}
		}
		
		public LoadedAssembly LookupReferencedAssembly(string fullName)
		{
			return assemblyList.assemblyLookupCache.GetOrAdd(fullName, LookupReferencedAssemblyInternal);
		}

		DotNetCorePathFinder dotNetCorePathFinder;
		
		LoadedAssembly LookupReferencedAssemblyInternal(string fullName)
		{
			foreach (LoadedAssembly asm in assemblyList.GetAssemblies()) {
				if (asm.AssemblyDefinition != null && fullName.Equals(asm.AssemblyDefinition.FullName, StringComparison.OrdinalIgnoreCase)) {
					return asm;
				}
			}
			if (assemblyLoadDisableCount > 0)
				return null;
			
			if (!App.Current.Dispatcher.CheckAccess()) {
				// Call this method on the GUI thread.
				return (LoadedAssembly)App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Func<string, LoadedAssembly>(LookupReferencedAssembly), fullName);
			}

			var targetFramework = TargetFrameworkId.Split(new[] { ",Version=v" }, StringSplitOptions.None);
			var name = AssemblyNameReference.Parse(fullName);
			string file = null;
			switch (targetFramework[0]) {
				case ".NETCoreApp":
				case ".NETStandard":
					if (targetFramework.Length != 2) break;
					if (dotNetCorePathFinder == null) {
						var version = targetFramework[1].Length == 3 ? targetFramework[1] + ".0" : targetFramework[1];
						dotNetCorePathFinder = new DotNetCorePathFinder(fileName, TargetFrameworkId, version, this.loadedAssemblyReferences);
					}
					file = dotNetCorePathFinder.TryResolveDotNetCore(name);
					break;
				default:
					file = GacInterop.FindAssemblyInNetGac(name);
					break;
			}
			if (file == null) {
				string dir = Path.GetDirectoryName(this.fileName);
				if (File.Exists(Path.Combine(dir, name.Name + ".dll")))
					file = Path.Combine(dir, name.Name + ".dll");
				else if (File.Exists(Path.Combine(dir, name.Name + ".exe")))
					file = Path.Combine(dir, name.Name + ".exe");
			}
			if (file != null) {
				loadedAssemblyReferences.AddMessage(fullName, MessageKind.Info, "Success - Loading from: " + file);
				return assemblyList.OpenAssembly(file, true);
			} else {
				loadedAssemblyReferences.AddMessage(fullName, MessageKind.Error, "Could not find reference: " + fullName);
				return null;
			}
		}
		
		LoadedAssembly LookupWinRTMetadata(string name)
		{
			foreach (LoadedAssembly asm in assemblyList.GetAssemblies()) {
				if (asm.AssemblyDefinition != null && name.Equals(asm.AssemblyDefinition.Name.Name, StringComparison.OrdinalIgnoreCase))
					return asm;
			}
			if (assemblyLoadDisableCount > 0)
				return null;
			if (!App.Current.Dispatcher.CheckAccess()) {
				// Call this method on the GUI thread.
				return (LoadedAssembly)App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Func<string, LoadedAssembly>(LookupWinRTMetadata), name);
			}
			
			string file = Path.Combine(Environment.SystemDirectory, "WinMetadata", name + ".winmd");
			if (File.Exists(file)) {
				return assemblyList.OpenAssembly(file, true);
			} else {
				return null;
			}
		}
		
		public Task ContinueWhenLoaded(Action<Task<ModuleDefinition>> onAssemblyLoaded, TaskScheduler taskScheduler)
		{
			return this.assemblyTask.ContinueWith(onAssemblyLoaded, taskScheduler);
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
