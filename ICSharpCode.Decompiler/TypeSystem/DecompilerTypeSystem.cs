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
using SRM = System.Reflection.Metadata;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

using static ICSharpCode.Decompiler.Metadata.MetadataExtensions;
using System.Diagnostics;
using System.Collections.Immutable;
using ICSharpCode.Decompiler.Metadata;

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
		/// Default settings: all features enabled.
		/// </summary>
		Default = Dynamic | Tuple | ExtensionMethods
	}

	/// <summary>
	/// Manages the NRefactory type system for the decompiler.
	/// </summary>
	/// <remarks>
	/// This class is thread-safe.
	/// </remarks>
	public class DecompilerTypeSystem : IDecompilerTypeSystem
	{
		readonly Metadata.PEFile moduleDefinition;
		readonly ICompilation compilation;
		readonly IAssemblyResolver assemblyResolver;
		readonly TypeSystemOptions typeSystemOptions;
		readonly MetadataAssembly mainAssembly;

		public DecompilerTypeSystem(Metadata.PEFile moduleDefinition, IAssemblyResolver assemblyResolver)
			: this(moduleDefinition, assemblyResolver, new DecompilerSettings())
		{
		}

		public DecompilerTypeSystem(PEFile moduleDefinition, IAssemblyResolver assemblyResolver, DecompilerSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));
			this.moduleDefinition = moduleDefinition ?? throw new ArgumentNullException(nameof(moduleDefinition));
			this.assemblyResolver = assemblyResolver ?? throw new ArgumentNullException(nameof(assemblyResolver));
			typeSystemOptions = TypeSystemOptions.None;
			if (settings.Dynamic)
				typeSystemOptions |= TypeSystemOptions.Dynamic;
			if (settings.TupleTypes)
				typeSystemOptions |= TypeSystemOptions.Tuple;
			if (settings.ExtensionMethods)
				typeSystemOptions |= TypeSystemOptions.ExtensionMethods;
			var mainAssembly = moduleDefinition.WithOptions(typeSystemOptions);
			// Load referenced assemblies and type-forwarder references.
			// This is necessary to make .NET Core/PCL binaries work better.
			var referencedAssemblies = new List<IAssemblyReference>();
			var assemblyReferenceQueue = new Queue<Metadata.AssemblyReference>(moduleDefinition.AssemblyReferences);
			var processedAssemblyReferences = new HashSet<Metadata.AssemblyReference>(KeyComparer.Create((Metadata.AssemblyReference reference) => reference.FullName));
			while (assemblyReferenceQueue.Count > 0) {
				var asmRef = assemblyReferenceQueue.Dequeue();
				if (!processedAssemblyReferences.Add(asmRef))
					continue;
				var asm = assemblyResolver.Resolve(asmRef);
				if (asm != null) {
					referencedAssemblies.Add(asm.WithOptions(typeSystemOptions));
					var metadata = asm.Metadata;
					foreach (var h in metadata.ExportedTypes) {
						var forwarder = metadata.GetExportedType(h);
						if (!forwarder.IsForwarder || forwarder.Implementation.Kind != SRM.HandleKind.AssemblyReference) continue;
						assemblyReferenceQueue.Enqueue(new Metadata.AssemblyReference(asm, (SRM.AssemblyReferenceHandle)forwarder.Implementation));
					}
				}
			}
			compilation = new SimpleCompilation(mainAssembly, referencedAssemblies);
			// Primitive types are necessary to avoid assertions in ILReader.
			// Fallback to MinimalCorlib to provide the primitive types.
			if (compilation.FindType(KnownTypeCode.Void).Kind == TypeKind.Unknown || compilation.FindType(KnownTypeCode.Int32).Kind == TypeKind.Unknown) {
				referencedAssemblies.Add(MinimalCorlib.Instance);
				compilation = new SimpleCompilation(mainAssembly, referencedAssemblies);
			}
			this.mainAssembly = (MetadataAssembly)compilation.MainAssembly;
		}

		public ICompilation Compilation {
			get { return compilation; }
		}
		
		public MetadataAssembly MainAssembly {
			get { return mainAssembly; }
		}

		public Metadata.PEFile ModuleDefinition {
			get { return moduleDefinition; }
		}

		public SRM.MetadataReader GetMetadata() => moduleDefinition.Metadata;

		public Metadata.PEFile GetModuleDefinition(IAssembly assembly)
		{
			if (assembly is MetadataAssembly asm) {
				return asm.PEFile;
			}
			return null;
		}
		
		public IMember ResolveAsMember(SRM.EntityHandle memberReference)
		{
			switch (memberReference.Kind) {
				case SRM.HandleKind.FieldDefinition:
					return ResolveAsField(memberReference);
				case SRM.HandleKind.MethodDefinition:
					return ResolveAsMethod(memberReference);
				case SRM.HandleKind.MemberReference:
					var mr = moduleDefinition.Metadata.GetMemberReference((SRM.MemberReferenceHandle)memberReference);
					switch (mr.GetKind()) {
						case SRM.MemberReferenceKind.Method:
							return ResolveAsMethod(memberReference);
						case SRM.MemberReferenceKind.Field:
							return ResolveAsField(memberReference);
					}
					throw new NotSupportedException();
				case SRM.HandleKind.EventDefinition:
					return ResolveAsEvent(memberReference);
				case SRM.HandleKind.PropertyDefinition:
					return ResolveAsProperty(memberReference);
				case SRM.HandleKind.MethodSpecification:
					return ResolveAsMethod(memberReference);
				default:
					throw new NotSupportedException();
			}
		}

		public IType ResolveAsType(SRM.EntityHandle typeReference)
		{
			return mainAssembly.ResolveType(typeReference, new GenericContext());
		}

		public IMethod ResolveAsMethod(SRM.EntityHandle methodReference)
		{
			return mainAssembly.ResolveMethod(methodReference);
		}

		public IField ResolveAsField(SRM.EntityHandle fieldReference)
		{
			return mainAssembly.ResolveEntity(fieldReference, new GenericContext()) as IField;
		}

		public IProperty ResolveAsProperty(SRM.EntityHandle propertyReference)
		{
			return mainAssembly.ResolveEntity(propertyReference, new GenericContext()) as IProperty;
		}

		public IEvent ResolveAsEvent(SRM.EntityHandle eventReference)
		{
			return mainAssembly.ResolveEntity(eventReference, new GenericContext()) as IEvent;
		}

		public SRM.MethodSignature<IType> DecodeMethodSignature(SRM.StandaloneSignatureHandle handle)
		{
			var standaloneSignature = moduleDefinition.Metadata.GetStandaloneSignature(handle);
			if (standaloneSignature.GetKind() != SRM.StandaloneSignatureKind.Method)
				throw new InvalidOperationException("Expected Method signature");
			var sig = standaloneSignature.DecodeMethodSignature(
				mainAssembly.TypeProvider,
				new GenericContext()
			);
			return new SRM.MethodSignature<IType>(
				sig.Header,
				ApplyAttributesToType(sig.ReturnType),
				sig.RequiredParameterCount,
				sig.GenericParameterCount,
				ImmutableArray.CreateRange(
					sig.ParameterTypes, ApplyAttributesToType
				)
			);
		}

		public ImmutableArray<IType> DecodeLocalSignature(SRM.StandaloneSignatureHandle handle)
		{
			var standaloneSignature = moduleDefinition.Metadata.GetStandaloneSignature(handle);
			if (standaloneSignature.GetKind() != SRM.StandaloneSignatureKind.LocalVariables)
				throw new InvalidOperationException("Expected Local signature");
			var types = standaloneSignature.DecodeLocalSignature(
				mainAssembly.TypeProvider,
				new GenericContext()
			);
			return ImmutableArray.CreateRange(types, ApplyAttributesToType);
		}

		IType ApplyAttributesToType(IType t)
		{
			return ApplyAttributeTypeVisitor.ApplyAttributesToType(t, compilation, null,
				moduleDefinition.Metadata, typeSystemOptions);
		}

		public IDecompilerTypeSystem GetSpecializingTypeSystem(TypeParameterSubstitution substitution)
		{
			if (substitution.Equals(TypeParameterSubstitution.Identity)) {
				return this;
			} else {
				return new SpecializingDecompilerTypeSystem(this, substitution);
			}
		}
	}
}
