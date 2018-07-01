// Copyright (c) 2018 Siegfried Pammer
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
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Metadata
{
	public class PEFile : IDisposable, TypeSystem.IAssemblyReference
	{
		public string FileName { get; }
		public PEReader Reader { get; }
		public MetadataReader Metadata { get; }
		public IAssemblyResolver AssemblyResolver { get; }
		public IAssemblyDocumentationResolver DocumentationResolver { get; set; }
		public IDebugInfoProvider DebugInfo { get; set; }

		public PEFile(string fileName, Stream stream, bool throwOnResolveError = false, PEStreamOptions options = PEStreamOptions.Default)
		{
			this.FileName = fileName;
			this.Reader = new PEReader(stream, options);
			this.Metadata = Reader.GetMetadataReader();
			this.AssemblyResolver = new UniversalAssemblyResolver(fileName, throwOnResolveError, Reader.DetectTargetFrameworkId(), options);
		}

		public PEFile(string fileName, Stream stream, IAssemblyResolver assemblyResolver, PEStreamOptions options = PEStreamOptions.Default)
		{
			this.FileName = fileName;
			this.Reader = new PEReader(stream, options);
			this.Metadata = Reader.GetMetadataReader();
			this.AssemblyResolver = assemblyResolver;
		}

		public bool IsAssembly => Metadata.IsAssembly;
		public string Name => GetName();
		public string FullName => IsAssembly ? Metadata.GetFullAssemblyName() : Name;

		public TargetRuntime GetRuntime()
		{
			string version = Metadata.MetadataVersion;
			switch (version[1]) {
				case '1':
					if (version[3] == 1)
						return TargetRuntime.Net_1_0;
					else
						return TargetRuntime.Net_1_1;
				case '2':
					return TargetRuntime.Net_2_0;
				case '4':
					return TargetRuntime.Net_4_0;
				default:
					return TargetRuntime.Unknown;
			}

		}

		string GetName()
		{
			var metadata = Metadata;
			if (metadata.IsAssembly)
				return metadata.GetString(metadata.GetAssemblyDefinition().Name);
			return metadata.GetString(metadata.GetModuleDefinition().Name);
		}

		public ImmutableArray<AssemblyReference> AssemblyReferences => Metadata.AssemblyReferences.Select(r => new AssemblyReference(this, r)).ToImmutableArray();
		public ImmutableArray<Resource> Resources => GetResources().ToImmutableArray();

		IEnumerable<Resource> GetResources()
		{
			var metadata = Metadata;
			foreach (var h in metadata.ManifestResources) {
				yield return new Resource(this, h);
			}
		}

		public void Dispose()
		{
			Reader.Dispose();
		}

		Dictionary<TopLevelTypeName, TypeDefinitionHandle> typeLookup;

		/// <summary>
		/// Finds the top-level-type with the specified name.
		/// </summary>
		public TypeDefinitionHandle GetTypeDefinition(TopLevelTypeName typeName)
		{
			var lookup = LazyInit.VolatileRead(ref typeLookup);
			if (lookup == null) {
				lookup = new Dictionary<TopLevelTypeName, TypeDefinitionHandle>();
				foreach (var handle in Metadata.TypeDefinitions) {
					var td = Metadata.GetTypeDefinition(handle);
					if (!td.GetDeclaringType().IsNil) {
						continue; // nested type
					}
					var nsHandle = td.Namespace;
					string ns = nsHandle.IsNil ? string.Empty : Metadata.GetString(nsHandle);
					string name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(Metadata.GetString(td.Name), out int typeParameterCount);
					lookup[new TopLevelTypeName(ns, name, typeParameterCount)] = handle;
				}
				lookup = LazyInit.GetOrSet(ref typeLookup, lookup);
			}
			if (lookup.TryGetValue(typeName, out var resultHandle))
				return resultHandle;
			else
				return default;
		}

		Dictionary<FullTypeName, ExportedTypeHandle> typeForwarderLookup;

		/// <summary>
		/// Finds the type forwarder with the specified name.
		/// </summary>
		public ExportedTypeHandle GetTypeForwarder(FullTypeName typeName)
		{
			var lookup = LazyInit.VolatileRead(ref typeForwarderLookup);
			if (lookup == null) {
				lookup = new Dictionary<FullTypeName, ExportedTypeHandle>();
				foreach (var handle in Metadata.ExportedTypes) {
					var td = Metadata.GetExportedType(handle);
					lookup[td.GetFullTypeName(Metadata)] = handle;
				}
				lookup = LazyInit.GetOrSet(ref typeForwarderLookup, lookup);
			}
			if (lookup.TryGetValue(typeName, out var resultHandle))
				return resultHandle;
			else
				return default;
		}

		MethodSemanticsLookup methodSemanticsLookup;

		internal MethodSemanticsLookup MethodSemanticsLookup {
			get {
				var r = LazyInit.VolatileRead(ref methodSemanticsLookup);
				if (r != null)
					return r;
				else
					return LazyInit.GetOrSet(ref methodSemanticsLookup, new MethodSemanticsLookup(Metadata));
			}
		}

		public TypeSystem.IAssemblyReference WithOptions(TypeSystemOptions options)
		{
			return new PEFileWithOptions(this, options);
		}

		IAssembly TypeSystem.IAssemblyReference.Resolve(ITypeResolveContext context)
		{
			return new MetadataAssembly(context.Compilation, this, TypeSystemOptions.Default);
		}

		private class PEFileWithOptions : TypeSystem.IAssemblyReference
		{
			readonly PEFile peFile;
			readonly TypeSystemOptions options;

			public PEFileWithOptions(PEFile peFile, TypeSystemOptions options)
			{
				this.peFile = peFile;
				this.options = options;
			}

			IAssembly TypeSystem.IAssemblyReference.Resolve(ITypeResolveContext context)
			{
				return new MetadataAssembly(context.Compilation, peFile, options);
			}
		}
	}
}
