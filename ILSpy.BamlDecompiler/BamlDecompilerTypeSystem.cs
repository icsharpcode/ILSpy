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

namespace ILSpy.BamlDecompiler
{
	class BamlDecompilerTypeSystem : SimpleCompilation, IDecompilerTypeSystem
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

		public BamlDecompilerTypeSystem(PEFile mainModule, IAssemblyResolver assemblyResolver)
		{
			if (mainModule == null)
				throw new ArgumentNullException(nameof(mainModule));
			if (assemblyResolver == null)
				throw new ArgumentNullException(nameof(assemblyResolver));
			// Load referenced assemblies and type-forwarder references.
			// This is necessary to make .NET Core/PCL binaries work better.
			var referencedAssemblies = new List<PEFile>();
			var assemblyReferenceQueue = new Queue<(bool IsAssembly, PEFile MainModule, object Reference)>();
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
			foreach (var bamlReference in defaultBamlReferences)
			{
				assemblyReferenceQueue.Enqueue((true, mainModule, AssemblyNameReference.Parse(bamlReference)));
			}
			var comparer = KeyComparer.Create(((bool IsAssembly, PEFile MainModule, object Reference) reference) =>
				reference.IsAssembly ? "A:" + ((IAssemblyReference)reference.Reference).FullName :
									   "M:" + reference.Reference);
			var processedAssemblyReferences = new HashSet<(bool IsAssembly, PEFile Parent, object Reference)>(comparer);
			while (assemblyReferenceQueue.Count > 0)
			{
				var asmRef = assemblyReferenceQueue.Dequeue();
				if (!processedAssemblyReferences.Add(asmRef))
					continue;
				PEFile asm;
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
				if (mainModule.GetTypeDefinition(name) != null)
					return true;
				foreach (var file in referencedAssemblies)
				{
					if (file.GetTypeDefinition(name) != null)
						return true;
				}
				return false;
			}
		}

		public new MetadataModule MainModule { get; }
	}
}