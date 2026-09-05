// Copyright (c) 2018 Daniel Grunwald
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
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Instrumentation;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

using static ICSharpCode.Decompiler.Metadata.MetadataExtensions;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Options that control how metadata is represented in the type system.
	/// </summary>
	[Flags]
	public enum TypeSystemOptions
	{
		/// <summary>
		/// No options enabled; stay as close to the metadata as possible.
		/// </summary>
		None = 0,
		/// <summary>
		/// [DynamicAttribute] is used to replace 'object' types with the 'dynamic' type.
		/// 
		/// If this option is not active, the 'dynamic' type is not used, and the attribute is preserved.
		/// </summary>
		Dynamic = 1,
		/// <summary>
		/// Tuple types are represented using the TupleType class.
		/// [TupleElementNames] is used to name the tuple elements.
		/// 
		/// If this option is not active, the tuples are represented using their underlying type, and the attribute is preserved.
		/// </summary>
		Tuple = 2,
		/// <summary>
		/// If this option is active, [ExtensionAttribute] is removed and methods are marked as IsExtensionMethod.
		/// Otherwise, the attribute is preserved but the methods are not marked.
		/// </summary>
		ExtensionMethods = 4,
		/// <summary>
		/// Only load the public API into the type system.
		/// </summary>
		OnlyPublicAPI = 8,
		/// <summary>
		/// Do not cache accessed entities.
		/// In a normal type system (without this option), every type or member definition has exactly one ITypeDefinition/IMember
		/// instance. This instance is kept alive until the whole type system can be garbage-collected.
		/// When this option is specified, the type system avoids these caches.
		/// This reduces the memory usage in many cases, but increases the number of allocations.
		/// Also, some code in the decompiler expects to be able to compare type/member definitions by reference equality,
		/// and thus will fail with uncached type systems.
		/// </summary>
		Uncached = 0x10,
		/// <summary>
		/// If this option is active, [DecimalConstantAttribute] is removed and constant values are transformed into simple decimal literals.
		/// </summary>
		DecimalConstants = 0x20,
		/// <summary>
		/// If this option is active, modopt and modreq types are preserved in the type system.
		/// 
		/// Note: the decompiler currently does not support handling modified types;
		/// activating this option may lead to incorrect decompilation or internal errors.
		/// </summary>
		KeepModifiers = 0x40,
		/// <summary>
		/// If this option is active, [IsReadOnlyAttribute] on parameters+structs is removed
		/// and parameters are marked as in, structs as readonly.
		/// Otherwise, the attribute is preserved but the parameters and structs are not marked.
		/// </summary>
		ReadOnlyStructsAndParameters = 0x80,
		/// <summary>
		/// If this option is active, [IsByRefLikeAttribute] is removed and structs are marked as ref.
		/// Otherwise, the attribute is preserved but the structs are not marked.
		/// </summary>
		RefStructs = 0x100,
		/// <summary>
		/// If this option is active, [IsUnmanagedAttribute] is removed from type parameters,
		/// and HasUnmanagedConstraint is set instead.
		/// </summary>
		UnmanagedConstraints = 0x200,
		/// <summary>
		/// If this option is active, [NullableAttribute] is removed and reference types with
		/// nullability annotations are used instead.
		/// </summary>
		NullabilityAnnotations = 0x400,
		/// <summary>
		/// If this option is active, [IsReadOnlyAttribute] on methods is removed
		/// and the method marked as ThisIsRefReadOnly.
		/// </summary>
		ReadOnlyMethods = 0x800,
		/// <summary>
		/// [NativeIntegerAttribute] is used to replace 'IntPtr' types with the 'nint' type.
		/// </summary>
		NativeIntegers = 0x1000,
		/// <summary>
		/// Allow function pointer types. If this option is not enabled, function pointers are
		/// replaced with the 'IntPtr' type.
		/// </summary>
		FunctionPointers = 0x2000,
		/// <summary>
		/// Allow C# 11 scoped annotation. If this option is not enabled, ScopedRefAttribute
		/// will be reported as custom attribute.
		/// </summary>
		ScopedRef = 0x4000,
		/// <summary>
		/// Replace 'IntPtr' types with the 'nint' type even in absence of [NativeIntegerAttribute].
		/// Note: DecompilerTypeSystem constructor removes this setting from the options if
		/// not targeting .NET 7 or later.
		/// </summary>
		NativeIntegersWithoutAttribute = 0x8000,
		/// <summary>
		/// If this option is active, [RequiresLocationAttribute] on parameters is removed
		/// and parameters are marked as ref readonly.
		/// Otherwise, the attribute is preserved but the parameters are not marked
		/// as if it was a ref parameter without any attributes.
		/// </summary>
		RefReadOnlyParameters = 0x10000,
		/// <summary>
		/// If this option is active, [ParamCollectionAttribute] on parameters is removed
		/// and parameters are marked as params.
		/// Otherwise, the attribute is preserved but the parameters are not marked
		/// as if it was a normal parameter without any attributes.
		/// </summary>
		ParamsCollections = 0x20000,
		/// <summary>
		/// If this option is active, span types (Span&lt;T&gt; and ReadOnlySpan&lt;T&gt;) are treated like
		/// built-in types and language rules of C# 14 and later are applied.
		/// </summary>
		FirstClassSpanTypes = 0x40000,
		/// <summary>
		/// If this option is active, extension member groups are detected, otherwise the compiler-generated nested classes are left as-is.
		/// </summary>
		ExtensionMembers = 0x80000,
		/// <summary>
		/// If this option is active, methods with the MethodImplAttribute(MethodImplOptions.Async) are treated as async methods.
		/// </summary>
		RuntimeAsync = 0x100000,
		/// <summary>
		/// Default settings: typical options for the decompiler, with all C# language features enabled.
		/// </summary>
		Default = Dynamic | Tuple | ExtensionMethods | DecimalConstants | ReadOnlyStructsAndParameters
			| RefStructs | UnmanagedConstraints | NullabilityAnnotations | ReadOnlyMethods
			| NativeIntegers | FunctionPointers | ScopedRef | NativeIntegersWithoutAttribute
			| RefReadOnlyParameters | ParamsCollections | FirstClassSpanTypes | ExtensionMembers
			| RuntimeAsync
	}

	/// <summary>
	/// Manages the NRefactory type system for the decompiler.
	/// </summary>
	/// <remarks>
	/// This class is thread-safe.
	/// </remarks>
	public class DecompilerTypeSystem : SimpleCompilation, IDecompilerTypeSystem
	{
		public static TypeSystemOptions GetOptions(DecompilerSettings settings)
		{
			var typeSystemOptions = TypeSystemOptions.None;
			if (settings.Dynamic)
				typeSystemOptions |= TypeSystemOptions.Dynamic;
			if (settings.TupleTypes)
				typeSystemOptions |= TypeSystemOptions.Tuple;
			if (settings.ExtensionMethods)
				typeSystemOptions |= TypeSystemOptions.ExtensionMethods;
			if (settings.DecimalConstants)
				typeSystemOptions |= TypeSystemOptions.DecimalConstants;
			if (settings.IntroduceRefModifiersOnStructs)
				typeSystemOptions |= TypeSystemOptions.RefStructs;
			if (settings.IntroduceReadonlyAndInModifiers)
				typeSystemOptions |= TypeSystemOptions.ReadOnlyStructsAndParameters;
			if (settings.IntroduceUnmanagedConstraint)
				typeSystemOptions |= TypeSystemOptions.UnmanagedConstraints;
			if (settings.NullableReferenceTypes)
				typeSystemOptions |= TypeSystemOptions.NullabilityAnnotations;
			if (settings.ReadOnlyMethods)
				typeSystemOptions |= TypeSystemOptions.ReadOnlyMethods;
			if (settings.NativeIntegers)
				typeSystemOptions |= TypeSystemOptions.NativeIntegers;
			if (settings.FunctionPointers)
				typeSystemOptions |= TypeSystemOptions.FunctionPointers;
			if (settings.ScopedRef)
				typeSystemOptions |= TypeSystemOptions.ScopedRef;
			if (settings.NumericIntPtr)
				typeSystemOptions |= TypeSystemOptions.NativeIntegersWithoutAttribute;
			if (settings.RefReadOnlyParameters)
				typeSystemOptions |= TypeSystemOptions.RefReadOnlyParameters;
			if (settings.ParamsCollections)
				typeSystemOptions |= TypeSystemOptions.ParamsCollections;
			if (settings.FirstClassSpanTypes)
				typeSystemOptions |= TypeSystemOptions.FirstClassSpanTypes;
			if (settings.ExtensionMembers)
				typeSystemOptions |= TypeSystemOptions.ExtensionMembers;
			if (settings.AsyncAwait)
				typeSystemOptions |= TypeSystemOptions.RuntimeAsync;
			return typeSystemOptions;
		}

		public static Task<DecompilerTypeSystem> CreateAsync(PEFile mainModule, IAssemblyResolver assemblyResolver)
		{
			return CreateAsync(mainModule, assemblyResolver, TypeSystemOptions.Default);
		}

		public static Task<DecompilerTypeSystem> CreateAsync(PEFile mainModule, IAssemblyResolver assemblyResolver, DecompilerSettings settings)
		{
			return CreateAsync(mainModule, assemblyResolver, GetOptions(settings ?? throw new ArgumentNullException(nameof(settings))));
		}

		public static async Task<DecompilerTypeSystem> CreateAsync(PEFile mainModule, IAssemblyResolver assemblyResolver, TypeSystemOptions typeSystemOptions)
		{
			if (mainModule == null)
				throw new ArgumentNullException(nameof(mainModule));
			if (assemblyResolver == null)
				throw new ArgumentNullException(nameof(assemblyResolver));
			var ts = new DecompilerTypeSystem(typeSystemOptions);
			await ts.InitializeAsync(mainModule, assemblyResolver)
				.ConfigureAwait(false);
			return ts;
		}

		private MetadataModule mainModule;
		private TypeSystemOptions typeSystemOptions;

		private DecompilerTypeSystem(TypeSystemOptions typeSystemOptions)
		{
			this.typeSystemOptions = typeSystemOptions;
		}

		public DecompilerTypeSystem(MetadataFile mainModule, IAssemblyResolver assemblyResolver)
			: this(mainModule, assemblyResolver, TypeSystemOptions.Default)
		{
		}

		public DecompilerTypeSystem(MetadataFile mainModule, IAssemblyResolver assemblyResolver, DecompilerSettings settings)
			: this(mainModule, assemblyResolver, GetOptions(settings ?? throw new ArgumentNullException(nameof(settings))))
		{
		}

		public DecompilerTypeSystem(MetadataFile mainModule, IAssemblyResolver assemblyResolver, TypeSystemOptions typeSystemOptions)
			: this(typeSystemOptions)
		{
			if (mainModule == null)
				throw new ArgumentNullException(nameof(mainModule));
			if (assemblyResolver == null)
				throw new ArgumentNullException(nameof(assemblyResolver));
			InitializeAsync(mainModule, assemblyResolver).GetAwaiter().GetResult();
		}

		static readonly string[] implicitReferences = new[] {
			"System.Runtime.InteropServices",
			"System.Runtime.CompilerServices.Unsafe"
		};

		/// <summary>
		/// Where the resolver keeps one, the log that records how each reference was resolved. The
		/// type system reports the forwarder chains it cannot follow there, next to the resolution
		/// messages for the same reference.
		/// </summary>
		internal ReferenceLoadInfo ReferenceLoadInfo { get; private set; }

		private async Task InitializeAsync(MetadataFile mainModule, IAssemblyResolver assemblyResolver)
		{
			ReferenceLoadInfo = (assemblyResolver as IReferenceLoadInfoProvider)?.LoadInfo;
			DecompilerEventSource.Log.TypeSystemInitStart(mainModule.Name);
			int referencedAssembliesResolved = 0;
			try
			{
				// The whole reference closure is resolved here, and every reference in it asks the
				// same framework directories the same questions. A resolver that can hold what it
				// read answers them once for this build and forgets it again afterwards.
				using (assemblyResolver.BeginSnapshot())
				{
					referencedAssembliesResolved = await InitializeCoreAsync(mainModule, assemblyResolver).ConfigureAwait(false);
				}
			}
			finally
			{
				DecompilerEventSource.Log.TypeSystemInitStop(mainModule.Name, referencedAssembliesResolved);
			}
		}

		/// <summary>
		/// Walks the type forwarders of every loaded assembly and looks for chains that come back to
		/// an assembly they already passed through. Such a chain never reaches a definition, so the
		/// type it forwards is lost. It is walked a second time with each hop resolved next to the
		/// assembly forwarding it, which keeps it inside the framework it reached, and the assembly
		/// that ends the repaired chain is returned so the caller can load it.
		/// </summary>
		/// <remarks>
		/// Only chains that are already broken are walked twice: a chain that reaches a definition is
		/// left exactly as it resolved, so nothing that decompiles correctly today changes.
		/// </remarks>
		static async Task<HashSet<MetadataFile>> RepairCyclicTypeForwardersAsync(
			List<MetadataFile> referencedAssemblies, IAssemblyResolver assemblyResolver)
		{
			// The chain is followed the way the type system follows it: by assembly short name, over
			// the assemblies that are loaded, keeping the highest version of each name.
			var loadedByName = new Dictionary<string, MetadataFile>(StringComparer.OrdinalIgnoreCase);
			foreach (var file in referencedAssemblies)
			{
				if (!file.IsAssembly)
					continue;
				if (!loadedByName.TryGetValue(file.Name, out var existing)
					|| file.Metadata.GetAssemblyDefinition().Version > existing.Metadata.GetAssemblyDefinition().Version)
				{
					loadedByName[file.Name] = file;
				}
			}

			var repaired = new HashSet<MetadataFile>();
			// The same chain carries every type a facade forwards; walking one of them settles it.
			var alreadyWalked = new HashSet<(MetadataFile, string)>();
			foreach (var file in referencedAssemblies.ToArray())
			{
				var metadata = file.Metadata;
				foreach (var handle in metadata.ExportedTypes)
				{
					var exportedType = metadata.GetExportedType(handle);
					// Only a row that names another assembly can start a chain that leaves this one.
					// A row implemented by an AssemblyFile stays inside this assembly - the type lives
					// in one of its other modules - and a nested type is implemented by its enclosing
					// exported type, so it travels with the chain of the enclosing name.
					if (exportedType.Implementation.Kind != SRM.HandleKind.AssemblyReference)
						continue;
					var typeName = exportedType.GetFullTypeName(metadata);
					var reference = (SRM.AssemblyReferenceHandle)exportedType.Implementation;
					string targetName = metadata.GetString(metadata.GetAssemblyReference(reference).Name);
					if (!alreadyWalked.Add((file, targetName)))
						continue;
					if (!ChainIsCyclic(file, typeName, targetName, loadedByName))
						continue;
					var definition = await FollowChainNextToForwardersAsync(file, typeName, reference, assemblyResolver)
						.ConfigureAwait(false);
					if (definition != null && !loadedByName.ContainsValue(definition))
					{
						repaired.Add(definition);
					}
				}
			}
			return repaired;
		}

		/// <summary>
		/// Whether following <paramref name="typeName"/> through the loaded assemblies returns to one
		/// it already passed through. A chain that ends anywhere else - at an assembly that does not
		/// forward the type onwards, or at a name nothing resolves to - is not this method's business.
		/// </summary>
		static bool ChainIsCyclic(MetadataFile start, FullTypeName typeName, string targetName,
			Dictionary<string, MetadataFile> loadedByName)
		{
			var visited = new HashSet<MetadataFile> { start };
			for (int hop = 0; hop < MaxTypeForwarderHops; hop++)
			{
				if (!loadedByName.TryGetValue(targetName, out var next))
					return false;
				if (!visited.Add(next))
					return true;
				var forwarder = next.GetTypeForwarder(typeName);
				if (forwarder.IsNil)
					return false;
				var exportedType = next.Metadata.GetExportedType(forwarder);
				// Anything but another assembly ends the chain here: an AssemblyFile row puts the
				// type in a sibling module of this assembly, and a nested type row points back at
				// its enclosing type rather than onwards.
				if (exportedType.Implementation.Kind != SRM.HandleKind.AssemblyReference)
					return false;
				var reference = (SRM.AssemblyReferenceHandle)exportedType.Implementation;
				targetName = next.Metadata.GetString(next.Metadata.GetAssemblyReference(reference).Name);
			}
			return false;
		}

		/// <summary>
		/// Follows the chain again with every hop resolved next to the assembly that forwards it, and
		/// returns the assembly the chain ends at - the one that holds the definition, where the
		/// repair worked. Null where it still leads nowhere.
		/// </summary>
		static async Task<MetadataFile> FollowChainNextToForwardersAsync(MetadataFile start,
			FullTypeName typeName, SRM.AssemblyReferenceHandle reference, IAssemblyResolver assemblyResolver)
		{
			var current = start;
			var visited = new HashSet<MetadataFile> { start };
			for (int hop = 0; hop < MaxTypeForwarderHops; hop++)
			{
				MetadataFile next;
				try
				{
					next = await assemblyResolver.ResolveAsync(
						new AssemblyReference(current, reference, preferNextToReferencingModule: true))
						.ConfigureAwait(false);
				}
				catch (Exception ex) when (!(ex is OperationCanceledException))
				{
					return null;
				}
				if (next == null || !visited.Add(next))
					return null;
				var forwarder = next.GetTypeForwarder(typeName);
				if (forwarder.IsNil)
				{
					// The chain ends here, which is only worth anything if the type is really
					// declared here: an assembly that neither forwards nor defines it would
					// otherwise be loaded, and displace the assembly it shares its name with.
					return DefinesType(next, typeName) ? next : null;
				}
				var exportedType = next.Metadata.GetExportedType(forwarder);
				if (exportedType.Implementation.Kind == SRM.HandleKind.AssemblyFile)
				{
					// The type is declared in another module of this assembly, which the loader pulls
					// in along with it, so the chain ends here and ends well.
					return next;
				}
				if (exportedType.Implementation.Kind != SRM.HandleKind.AssemblyReference)
				{
					// A nested type, implemented by its enclosing exported type. The chain that
					// matters is the enclosing type's, and that one is walked in its own right.
					return null;
				}
				current = next;
				reference = (SRM.AssemblyReferenceHandle)exportedType.Implementation;
			}
			return null;
		}

		/// <summary>
		/// Whether the file declares the type itself. Walking every type definition is affordable
		/// here because it only happens for a chain that is already known to be broken.
		/// </summary>
		static bool DefinesType(MetadataFile file, FullTypeName typeName)
		{
			var metadata = file.Metadata;
			foreach (var handle in metadata.TypeDefinitions)
			{
				if (handle.GetFullTypeName(metadata) == typeName)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Chains are a handful of hops long in practice; the cap only stops a malformed assembly
		/// from walking forever.
		/// </summary>
		const int MaxTypeForwarderHops = 16;

		/// <returns>The number of references in the final set passed to Init(): distinct
		/// resolved assemblies (same-name lower-version duplicates dropped) plus resolved
		/// non-assembly modules.</returns>
		private async Task<int> InitializeCoreAsync(MetadataFile mainModule, IAssemblyResolver assemblyResolver)
		{
			// Load referenced assemblies and type-forwarder references.
			// This is necessary to make .NET Core/PCL binaries work better.
			var referencedAssemblies = new List<MetadataFile>();
			var assemblyReferenceQueue = new Queue<(bool IsAssembly, MetadataFile MainModule, object Reference, Task<MetadataFile> ResolveTask)>();
			var comparer = KeyComparer.Create(((bool IsAssembly, MetadataFile MainModule, object Reference) reference) =>
				reference.IsAssembly ? "A:" + ((IAssemblyReference)reference.Reference).FullName :
									   "M:" + reference.Reference);
			var assemblyReferencesInQueue = new HashSet<(bool IsAssembly, MetadataFile Parent, object Reference)>(comparer);
			var mainMetadata = mainModule.Metadata;
			var tfm = mainModule.DetectTargetFrameworkId();
			var (identifier, version) = UniversalAssemblyResolver.ParseTargetFramework(tfm);
			foreach (var h in mainMetadata.GetModuleReferences())
			{
				try
				{
					var moduleRef = mainMetadata.GetModuleReference(h);
					var moduleName = mainMetadata.GetString(moduleRef.Name);
					foreach (var fileHandle in mainMetadata.AssemblyFiles)
					{
						var file = mainMetadata.GetAssemblyFile(fileHandle);
						if (mainMetadata.StringComparer.Equals(file.Name, moduleName) && file.ContainsMetadata)
						{
							AddToQueue(false, mainModule, moduleName);
							break;
						}
					}
				}
				catch (BadImageFormatException)
				{
				}
			}
			foreach (var refs in mainModule.AssemblyReferences)
			{
				AddToQueue(true, mainModule, refs);
			}
			while (assemblyReferenceQueue.Count > 0)
			{
				var asmRef = assemblyReferenceQueue.Dequeue();
				var asm = await asmRef.ResolveTask.ConfigureAwait(false);
				if (asm != null)
				{
					referencedAssemblies.Add(asm);
					var metadata = asm.Metadata;
					foreach (var h in metadata.ExportedTypes)
					{
						var exportedType = metadata.GetExportedType(h);
						switch (exportedType.Implementation.Kind)
						{
							case SRM.HandleKind.AssemblyReference:
								AddToQueue(true, asm, new AssemblyReference(asm, (SRM.AssemblyReferenceHandle)exportedType.Implementation));
								break;
							case SRM.HandleKind.AssemblyFile:
								var file = metadata.GetAssemblyFile((SRM.AssemblyFileHandle)exportedType.Implementation);
								AddToQueue(false, asm, metadata.GetString(file.Name));
								break;
						}
					}
				}
				if (assemblyReferenceQueue.Count == 0)
				{
					// For .NET Core and .NET 5 and newer, we need to pull in implicit references which are not included in the metadata,
					// as they contain compile-time-only types, such as System.Runtime.InteropServices.dll (for DllImport, MarshalAs, etc.)
					switch (identifier)
					{
						case TargetFrameworkIdentifier.NETCoreApp:
						case TargetFrameworkIdentifier.NETStandard:
						case TargetFrameworkIdentifier.NET:
							foreach (var item in implicitReferences)
							{
								var existing = referencedAssemblies.FirstOrDefault(asm => asm.Name == item);
								if (existing == null)
								{
									AddToQueue(true, mainModule, AssemblyNameReference.Parse(item + ", Version=" + version.ToString(3) + ".0, Culture=neutral"));
								}
							}
							break;
					}

				}
			}
			// A chain of type forwarders is followed by assembly name, and every name is resolved
			// relative to the assembly being decompiled - so a chain that leaves for another
			// framework can be pulled straight back and end up at an assembly it already visited.
			// Nothing in the closure defines the type then, and it is lost (issue #2054). Such a
			// chain is already broken, so walking it again costs nothing: this time each hop is
			// resolved next to the assembly that forwards it, and whatever that turns up is added.
			var repairedFiles = await RepairCyclicTypeForwardersAsync(referencedAssemblies, assemblyResolver)
				.ConfigureAwait(false);
			referencedAssemblies.AddRange(repairedFiles);

			if (!(identifier == TargetFrameworkIdentifier.NET && version >= new Version(7, 0)))
			{
				typeSystemOptions &= ~TypeSystemOptions.NativeIntegersWithoutAttribute;
			}
			var mainModuleWithOptions = mainModule.WithOptions(typeSystemOptions);
			// create IModuleReferences for all references
			var referencedAssembliesWithOptions = new List<IModuleReference>(referencedAssemblies.Count);
			Dictionary<string, (Version version, int insertionIndex, bool repaired)> referenceAssemblyVersionMap = new();
			foreach (var file in referencedAssemblies)
			{
				// if the file is an assembly, we need to make sure to deduplicate all assemblies,
				// with the same name, but different version. We keep the highest version number.
				if (file.IsAssembly)
				{
					var newFileVersion = file.Metadata.GetAssemblyDefinition().Version;
					// A file the forwarder repair found holds the definition the chain was looking
					// for, which the assembly it shares its name with does not - version order says
					// nothing about that, so it wins outright.
					bool isRepaired = repairedFiles.Contains(file);
					if (referenceAssemblyVersionMap.TryGetValue(file.Name, out var info))
					{
						if (isRepaired || (newFileVersion >= info.version && !info.repaired))
						{
							referencedAssembliesWithOptions[info.insertionIndex] = file.WithOptions(typeSystemOptions);
							referenceAssemblyVersionMap[file.Name] = (newFileVersion, info.insertionIndex, isRepaired);
						}
						continue;
					}
					else
					{
						referenceAssemblyVersionMap[file.Name] = (newFileVersion, referencedAssembliesWithOptions.Count, isRepaired);
					}
				}
				referencedAssembliesWithOptions.Add(file.WithOptions(typeSystemOptions));
			}
			// Primitive types are necessary to avoid assertions in ILReader.
			// Other known types are necessary in order for transforms to work (e.g. Task<T> for async transform).
			// Figure out which known types are missing from our type system so far:
			var missingKnownTypes = KnownTypeReference.AllKnownTypes.Where(IsMissing).ToList();
			if (missingKnownTypes.Count > 0)
			{
				Init(mainModuleWithOptions, referencedAssembliesWithOptions.Concat(new[] { MinimalCorlib.CreateWithTypes(missingKnownTypes) }));
			}
			else
			{
				Init(mainModuleWithOptions, referencedAssembliesWithOptions);
			}
			this.mainModule = (MetadataModule)base.MainModule;
			return referencedAssembliesWithOptions.Count;

			void AddToQueue(bool isAssembly, MetadataFile mainModule, object reference)
			{
				if (assemblyReferencesInQueue.Add((isAssembly, mainModule, reference)))
				{
					// Immediately start loading the referenced module as we add the entry to the queue.
					// This allows loading multiple modules in parallel.
					Task<MetadataFile> asm;
					if (isAssembly)
					{
						asm = assemblyResolver.ResolveAsync((IAssemblyReference)reference);
					}
					else
					{
						asm = assemblyResolver.ResolveModuleAsync(mainModule, (string)reference);
					}
					assemblyReferenceQueue.Enqueue((isAssembly, mainModule, reference, asm));
				}
			}

			bool IsMissing(KnownTypeReference knownType)
			{
				var name = knownType.TypeName;
				if (!mainModule.GetTypeDefinition(name).IsNil)
					return false;
				foreach (var file in referencedAssemblies)
				{
					if (!file.GetTypeDefinition(name).IsNil)
						return false;
				}
				return true;
			}
		}

		public new MetadataModule MainModule => mainModule;

		public override TypeSystemOptions TypeSystemOptions => typeSystemOptions;
	}
}
