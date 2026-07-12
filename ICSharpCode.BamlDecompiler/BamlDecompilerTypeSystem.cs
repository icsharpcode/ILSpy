// Copyright (c) 2021 Siegfried Pammer
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
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.BamlDecompiler
{
	public class BamlDecompilerTypeSystem : SimpleCompilation, IDecompilerTypeSystem
	{
		string[] defaultBamlReferences = new[] {
				"mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
				"System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
				"WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
				"PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
				"PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
				"PresentationUI, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
				"System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
			};

		// The WPF assemblies whose types serialize under the presentation XML namespace. When one of
		// these has to be synthesized (e.g. inspecting a WPF binary on a non-Windows machine), the
		// synthetic module reproduces its XmlnsDefinitionAttribute mapping so known types still emit
		// the clean presentation xmlns rather than a clr-namespace fallback.
		static readonly HashSet<string> presentationXmlnsAssemblies = new(StringComparer.OrdinalIgnoreCase) {
				"WindowsBase", "PresentationCore", "PresentationFramework", "PresentationUI"
			};

		public BamlDecompilerTypeSystem(MetadataFile mainModule, IAssemblyResolver assemblyResolver)
		{
			if (mainModule == null)
				throw new ArgumentNullException(nameof(mainModule));
			if (assemblyResolver == null)
				throw new ArgumentNullException(nameof(assemblyResolver));
			// Load referenced assemblies and type-forwarder references.
			// This is necessary to make .NET Core/PCL binaries work better.
			var referencedAssemblies = new List<MetadataFile>();
			var assemblyReferenceQueue = new Queue<(bool IsAssembly, MetadataFile MainModule, object Reference)>();
			var mainMetadata = mainModule.Metadata;
			foreach (var h in mainMetadata.GetModuleReferences())
			{
				var moduleRef = mainMetadata.GetModuleReference(h);
				var moduleName = mainMetadata.GetString(moduleRef.Name);
				foreach (var fileHandle in mainMetadata.AssemblyFiles)
				{
					var file = mainMetadata.GetAssemblyFile(fileHandle);
					if (mainMetadata.StringComparer.Equals(file.Name, moduleName) && file.ContainsMetadata)
					{
						assemblyReferenceQueue.Enqueue((false, mainModule, moduleName));
						break;
					}
				}
			}
			foreach (var refs in mainModule.AssemblyReferences)
			{
				assemblyReferenceQueue.Enqueue((true, mainModule, refs));
			}
			var defaultReferences = defaultBamlReferences.Select(AssemblyNameReference.Parse).ToArray();
			foreach (var bamlReference in defaultReferences)
			{
				assemblyReferenceQueue.Enqueue((true, mainModule, bamlReference));
			}
			var comparer = KeyComparer.Create(((bool IsAssembly, MetadataFile MainModule, object Reference) reference) =>
				reference.IsAssembly ? "A:" + ((IAssemblyReference)reference.Reference).FullName :
									   "M:" + reference.Reference);
			var processedAssemblyReferences = new HashSet<(bool IsAssembly, MetadataFile Parent, object Reference)>(comparer);
			while (assemblyReferenceQueue.Count > 0)
			{
				var asmRef = assemblyReferenceQueue.Dequeue();
				if (!processedAssemblyReferences.Add(asmRef))
					continue;
				MetadataFile asm;
				if (asmRef.IsAssembly)
				{
					asm = assemblyResolver.Resolve((IAssemblyReference)asmRef.Reference);
				}
				else
				{
					asm = assemblyResolver.ResolveModule(asmRef.MainModule, (string)asmRef.Reference);
				}
				if (asm != null)
				{
					referencedAssemblies.Add(asm);
					var metadata = asm.Metadata;
					foreach (var h in metadata.ExportedTypes)
					{
						var exportedType = metadata.GetExportedType(h);
						switch (exportedType.Implementation.Kind)
						{
							case HandleKind.AssemblyReference:
								assemblyReferenceQueue.Enqueue((true, asm, new ICSharpCode.Decompiler.Metadata.AssemblyReference(asm, (AssemblyReferenceHandle)exportedType.Implementation)));
								break;
							case HandleKind.AssemblyFile:
								var file = metadata.GetAssemblyFile((AssemblyFileHandle)exportedType.Implementation);
								assemblyReferenceQueue.Enqueue((false, asm, metadata.GetString(file.Name)));
								break;
						}
					}
				}
			}
			var mainModuleWithOptions = mainModule.WithOptions(TypeSystemOptions.Default);
			var referencedAssembliesWithOptions = referencedAssemblies.Select(file => file.WithOptions(TypeSystemOptions.Default));
			// Substitute a synthetic stand-in for every well-known BAML assembly that could not be
			// resolved (e.g. WPF assemblies when inspecting a WPF binary on a machine without WPF).
			// KnownThings assumes these assemblies are always present; the stand-in upholds that
			// invariant so the BAML decompiler degrades gracefully instead of failing outright.
			var resolvedAssemblyNames = new HashSet<string>(
				referencedAssemblies.Select(file => file.Name), StringComparer.OrdinalIgnoreCase);
			resolvedAssemblyNames.Add(mainModule.Name);
			var syntheticReferences = new List<IModuleReference>();
			foreach (var reference in defaultReferences)
			{
				if (resolvedAssemblyNames.Contains(reference.Name))
					continue;
				string presentationXmlns = presentationXmlnsAssemblies.Contains(reference.Name)
					? XamlContext.KnownNamespace_Presentation
					: null;
				syntheticReferences.Add(SyntheticWpfModule.CreateReference(reference, presentationXmlns));
			}
			referencedAssembliesWithOptions = referencedAssembliesWithOptions.Concat(syntheticReferences);
			// Primitive types are necessary to avoid assertions in ILReader.
			// Fallback to MinimalCorlib to provide the primitive types.
			if (!HasType(KnownTypeCode.Void) || !HasType(KnownTypeCode.Int32))
			{
				Init(mainModule.WithOptions(TypeSystemOptions.Default), referencedAssembliesWithOptions.Concat(new[] { MinimalCorlib.Instance }));
			}
			else
			{
				Init(mainModuleWithOptions, referencedAssembliesWithOptions);
			}
			this.MainModule = (MetadataModule)base.MainModule;

			bool HasType(KnownTypeCode code)
			{
				TopLevelTypeName name = KnownTypeReference.Get(code).TypeName;
				if (!mainModule.GetTypeDefinition(name).IsNil)
					return true;
				foreach (var file in referencedAssemblies)
				{
					if (!file.GetTypeDefinition(name).IsNil)
						return true;
				}
				return false;
			}
		}

		public new MetadataModule MainModule { get; }
	}
}