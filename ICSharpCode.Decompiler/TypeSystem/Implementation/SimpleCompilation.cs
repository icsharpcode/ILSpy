// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Simple compilation implementation.
	/// </summary>
	public class SimpleCompilation : ICompilation
	{
		readonly ITypeResolveContext context;
		readonly CacheManager cacheManager = new CacheManager();
		readonly KnownTypeCache knownTypeCache;
		readonly IModule mainModule;
		readonly IList<IModule> assemblies;
		readonly IList<IModule> referencedAssemblies;
		INamespace rootNamespace;
		
		public SimpleCompilation(IModuleReference mainAssembly, params IModuleReference[] assemblyReferences)
			: this(mainAssembly, (IEnumerable<IModuleReference>)assemblyReferences)
		{
		}
		
		public SimpleCompilation(IModuleReference mainAssembly, IEnumerable<IModuleReference> assemblyReferences)
		{
			if (mainAssembly == null)
				throw new ArgumentNullException("mainAssembly");
			if (assemblyReferences == null)
				throw new ArgumentNullException("assemblyReferences");
			this.context = new SimpleTypeResolveContext(this);
			this.mainModule = mainAssembly.Resolve(context);
			List<IModule> assemblies = new List<IModule>();
			assemblies.Add(this.mainModule);
			List<IModule> referencedAssemblies = new List<IModule>();
			foreach (var asmRef in assemblyReferences) {
				IModule asm;
				try {
					asm = asmRef.Resolve(context);
				} catch (InvalidOperationException) {
					throw new InvalidOperationException("Tried to initialize compilation with an invalid assembly reference. (Forgot to load the assembly reference ? - see CecilLoader)");
				}
				if (asm != null && !assemblies.Contains(asm))
					assemblies.Add(asm);
				if (asm != null && !referencedAssemblies.Contains(asm))
					referencedAssemblies.Add(asm);
			}
			this.assemblies = assemblies.AsReadOnly();
			this.referencedAssemblies = referencedAssemblies.AsReadOnly();
			this.knownTypeCache = new KnownTypeCache(this);
		}
		
		public IModule MainModule {
			get {
				if (mainModule == null)
					throw new InvalidOperationException("Compilation isn't initialized yet");
				return mainModule;
			}
		}
		
		public IList<IModule> Modules {
			get {
				if (assemblies == null)
					throw new InvalidOperationException("Compilation isn't initialized yet");
				return assemblies;
			}
		}
		
		public IList<IModule> ReferencedModules {
			get {
				if (referencedAssemblies == null)
					throw new InvalidOperationException("Compilation isn't initialized yet");
				return referencedAssemblies;
			}
		}
		
		public ITypeResolveContext TypeResolveContext {
			get { return context; }
		}
		
		public INamespace RootNamespace {
			get {
				INamespace ns = LazyInit.VolatileRead(ref this.rootNamespace);
				if (ns != null) {
					return ns;
				} else {
					if (referencedAssemblies == null)
						throw new InvalidOperationException("Compilation isn't initialized yet");
					return LazyInit.GetOrSet(ref this.rootNamespace, CreateRootNamespace());
				}
			}
		}
		
		protected virtual INamespace CreateRootNamespace()
		{
			// SimpleCompilation does not support extern aliases; but derived classes might.
			// CreateRootNamespace() is virtual so that derived classes can change the global namespace.
			INamespace[] namespaces = new INamespace[referencedAssemblies.Count + 1];
			namespaces[0] = mainModule.RootNamespace;
			for (int i = 0; i < referencedAssemblies.Count; i++) {
				namespaces[i + 1] = referencedAssemblies[i].RootNamespace;
			}
			return new MergedNamespace(this, namespaces);
		}
		
		public CacheManager CacheManager {
			get { return cacheManager; }
		}
		
		public virtual INamespace GetNamespaceForExternAlias(string alias)
		{
			if (string.IsNullOrEmpty(alias))
				return this.RootNamespace;
			// SimpleCompilation does not support extern aliases; but derived classes might.
			return null;
		}
		
		public IType FindType(KnownTypeCode typeCode)
		{
			return knownTypeCache.FindType(typeCode);
		}
		
		public StringComparer NameComparer {
			get { return StringComparer.Ordinal; }
		}
		
		public override string ToString()
		{
			return "[SimpleCompilation " + mainModule.AssemblyName + "]";
		}
	}
}
