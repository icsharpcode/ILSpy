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
		readonly ITypeResolveContext context;
		readonly TypeSystemOptions typeSystemOptions;
		readonly MetadataAssembly mainAssembly;

		Dictionary<SRM.EntityHandle, IField> fieldLookupCache = new Dictionary<SRM.EntityHandle, IField>();
		Dictionary<SRM.EntityHandle, IProperty> propertyLookupCache = new Dictionary<SRM.EntityHandle, IProperty>();
		Dictionary<SRM.EntityHandle, IMethod> methodLookupCache = new Dictionary<SRM.EntityHandle, IMethod>();
		Dictionary<SRM.EntityHandle, IEvent> eventLookupCache = new Dictionary<SRM.EntityHandle, IEvent>();

		Dictionary<IUnresolvedAssembly, Metadata.PEFile> moduleLookup = new Dictionary<IUnresolvedAssembly, Metadata.PEFile>();
		
		public DecompilerTypeSystem(Metadata.PEFile moduleDefinition) : this(moduleDefinition, new DecompilerSettings())
		{
		}

		public DecompilerTypeSystem(Metadata.PEFile moduleDefinition, DecompilerSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));
			this.moduleDefinition = moduleDefinition;
			typeSystemOptions = TypeSystemOptions.None;
			if (settings.Dynamic)
				typeSystemOptions |= TypeSystemOptions.Dynamic;
			if (settings.TupleTypes)
				typeSystemOptions |= TypeSystemOptions.Tuple;
			if (settings.ExtensionMethods)
				typeSystemOptions |= TypeSystemOptions.ExtensionMethods;
			MetadataLoader loader = new MetadataLoader {
				IncludeInternalMembers = true,
				ShortenInterfaceImplNames = false,
				Options = typeSystemOptions,
			};
			IUnresolvedAssembly mainAssembly = loader.LoadModule(moduleDefinition);
			// Load referenced assemblies and type-forwarder references.
			// This is necessary to make .NET Core/PCL binaries work better.
			var referencedAssemblies = new List<IUnresolvedAssembly>();
			var assemblyReferenceQueue = new Queue<Metadata.AssemblyReference>(moduleDefinition.AssemblyReferences);
			var processedAssemblyReferences = new HashSet<Metadata.AssemblyReference>(KeyComparer.Create((Metadata.AssemblyReference reference) => reference.FullName));
			while (assemblyReferenceQueue.Count > 0) {
				var asmRef = assemblyReferenceQueue.Dequeue();
				if (!processedAssemblyReferences.Add(asmRef))
					continue;
				var asm = moduleDefinition.AssemblyResolver.Resolve(asmRef);
				if (asm != null) {
					IUnresolvedAssembly unresolvedAsm = loader.LoadModule(asm);
					referencedAssemblies.Add(unresolvedAsm);
					moduleLookup.Add(unresolvedAsm, asm);
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
			context = new SimpleTypeResolveContext(compilation.MainAssembly);
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

		#region Resolve Type
		public IType ResolveAsType(SRM.EntityHandle typeReference)
		{
			return MetadataTypeReference.Resolve(
				typeReference,
				moduleDefinition.Metadata,
				mainAssembly.TypeProvider,
				new GenericContext(),
				typeSystemOptions
			);
		}

		IType ResolveDeclaringType(SRM.EntityHandle declaringTypeReference)
		{
			// resolve without substituting dynamic/tuple types
			return MetadataTypeReference.Resolve(
				declaringTypeReference,
				moduleDefinition.Metadata,
				mainAssembly.TypeProvider,
				new GenericContext(context),
				typeSystemOptions & ~(TypeSystemOptions.Dynamic | TypeSystemOptions.Tuple)
			);
		}
		#endregion
		
		public SRM.MethodSignature<IType> DecodeMethodSignature(SRM.StandaloneSignatureHandle handle)
		{
			var standaloneSignature = moduleDefinition.Metadata.GetStandaloneSignature(handle);
			if (standaloneSignature.GetKind() != SRM.StandaloneSignatureKind.Method)
				throw new InvalidOperationException("Expected Method signature");
			return standaloneSignature.DecodeMethodSignature(
				mainAssembly.TypeProvider,
				new GenericContext(context)
			);
		}

		public ImmutableArray<IType> DecodeLocalSignature(SRM.StandaloneSignatureHandle handle)
		{
			var standaloneSignature = moduleDefinition.Metadata.GetStandaloneSignature(handle);
			if (standaloneSignature.GetKind() != SRM.StandaloneSignatureKind.LocalVariables)
				throw new InvalidOperationException("Expected Local signature");
			return standaloneSignature.DecodeLocalSignature(
				mainAssembly.TypeProvider,
				new GenericContext(context)
			);
		}

		#region Resolve Field
		public IField ResolveAsField(SRM.EntityHandle fieldReference)
		{
			/*if (fieldReference.IsNil)
				throw new ArgumentNullException(nameof(fieldReference));
			if (fieldReference.Kind != SRM.HandleKind.FieldDefinition && fieldReference.Kind != SRM.HandleKind.MemberReference)
				throw new ArgumentException("HandleKind must be either FieldDefinition or MemberReference", nameof(fieldReference));
			lock (fieldLookupCache) {
				if (!fieldLookupCache.TryGetValue(fieldReference, out IField field)) {
					var metadata = moduleDefinition.Metadata;
					IType declaringType;
					switch (fieldReference.Kind) {
						case SRM.HandleKind.FieldDefinition:
							var fieldDefHandle = (SRM.FieldDefinitionHandle)fieldReference;
							field = mainAssembly.GetDefinition(fieldDefHandle);
							if (field == null) {
								throw new NotImplementedException();
							}
							break;
						case SRM.HandleKind.MemberReference:
							var memberRefHandle = (SRM.MemberReferenceHandle)fieldReference;
							var memberRef = metadata.GetMemberReference(memberRefHandle);
							Debug.Assert(memberRef.GetKind() == SRM.MemberReferenceKind.Field);
							declaringType = ResolveDeclaringType(memberRef.Parent);
							field = FindNonGenericField(metadata, memberRefHandle, declaringType);
							break;
						default:
							throw new NotSupportedException();
					}
					if (declaringType.TypeArguments.Count > 0) {
						field = (IField)field.Specialize(new TypeParameterSubstitution(declaringType.TypeArguments, null));
					}
					fieldLookupCache.Add(fieldReference, field);
				}
				return field;
			}*/
			throw new NotImplementedException();
		}

		/*
		IField FindNonGenericField(SRM.MetadataReader metadata, SRM.MemberReferenceHandle memberRefHandle, IType declaringType)
		{
			var memberRef = metadata.GetMemberReference(memberRefHandle);
			string name = metadata.GetString(memberRef.Name);
			ITypeDefinition typeDef = declaringType.GetDefinition();

			if (typeDef != null) {
				foreach (IField field in typeDef.Fields) {
					if (field.Name == name)
						return field;
				}
			}
			var returnType = memberRef.DecodeFieldSignature(mainAssembly.TypeProvider, new GenericContext(context));
			return new FakeField(compilation) {
				DeclaringType = declaringType,
				Name = name,
				ReturnType = returnType
			};
		}
		*/
		#endregion

		#region Resolve Method
		public IMethod ResolveAsMethod(SRM.EntityHandle methodReference)
		{
			return MainAssembly.ResolveMethod(methodReference);
		}
		
		#endregion

		#region Resolve Property
		public IProperty ResolveAsProperty(SRM.EntityHandle propertyReference)
		{
			if (propertyReference.IsNil)
				throw new ArgumentNullException(nameof(propertyReference));
			if (propertyReference.Kind != SRM.HandleKind.PropertyDefinition)
				throw new ArgumentException("HandleKind must be PropertyDefinition", nameof(propertyReference));
			lock (propertyLookupCache) {
				IProperty property;
				if (!propertyLookupCache.TryGetValue(propertyReference, out property)) {
					var metadata = moduleDefinition.Metadata;
					property = FindNonGenericProperty(metadata, (SRM.PropertyDefinitionHandle)propertyReference);
					/*if (propertyReference.DeclaringType.IsGenericInstance) {
						var git = (GenericInstanceType)propertyReference.DeclaringType;
						var typeArguments = git.GenericArguments.SelectArray(Resolve);
						property = (IProperty)property.Specialize(new TypeParameterSubstitution(typeArguments, null));
					}*/
					propertyLookupCache.Add(propertyReference, property);
				}
				return property;
			}
		}

		IProperty FindNonGenericProperty(SRM.MetadataReader metadata, SRM.PropertyDefinitionHandle handle)
		{
			var propertyDefinition = metadata.GetPropertyDefinition(handle);
			var declaringType = metadata.GetMethodDefinition(propertyDefinition.GetAccessors().GetAny()).GetDeclaringType();
			ITypeDefinition typeDef = ResolveDeclaringType(declaringType).GetDefinition();
			if (typeDef == null)
				return null;
			foreach (IProperty property in typeDef.Properties) {
				if (property.MetadataToken == handle)
					return property;
			}
			return null;
		}
		#endregion

		#region Resolve Event
		public IEvent ResolveAsEvent(SRM.EntityHandle eventReference)
		{
			if (eventReference.IsNil)
				throw new ArgumentNullException(nameof(eventReference));
			if (eventReference.Kind != SRM.HandleKind.EventDefinition)
				throw new ArgumentException("HandleKind must be EventDefinition", nameof(eventReference));
			lock (eventLookupCache) {
				IEvent ev;
				if (!eventLookupCache.TryGetValue(eventReference, out ev)) {
					var metadata = moduleDefinition.Metadata;
					ev = FindNonGenericEvent(metadata, (SRM.EventDefinitionHandle)eventReference);
					/*if (eventReference.DeclaringType.IsGenericInstance) {
						var git = (GenericInstanceType)eventReference.DeclaringType;
						var typeArguments = git.GenericArguments.SelectArray(Resolve);
						ev = (IEvent)ev.Specialize(new TypeParameterSubstitution(typeArguments, null));
					}*/
					eventLookupCache.Add(eventReference, ev);
				}
				return ev;
			}
		}

		IEvent FindNonGenericEvent(SRM.MetadataReader metadata, SRM.EventDefinitionHandle handle)
		{
			var eventDefinition = metadata.GetEventDefinition(handle);
			var declaringType = metadata.GetMethodDefinition(eventDefinition.GetAccessors().GetAny()).GetDeclaringType();
			ITypeDefinition typeDef = ResolveDeclaringType(declaringType).GetDefinition();
			if (typeDef == null)
				return null;
			var returnType = ResolveAsType(eventDefinition.Type);
			string name = metadata.GetString(eventDefinition.Name);
			foreach (IEvent ev in typeDef.Events) {
				if (ev.MetadataToken == handle)
					return ev;
			}
			return null;
		}
		#endregion

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
