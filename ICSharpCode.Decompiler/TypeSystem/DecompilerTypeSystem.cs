using System;
using System.Collections.Generic;
using System.Linq;
using SRM = System.Reflection.Metadata;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

using static ICSharpCode.Decompiler.Metadata.MetadataExtensions;
using System.Diagnostics;

namespace ICSharpCode.Decompiler.TypeSystem
{
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

		/// <summary>
		/// CecilLoader used for converting cecil type references to ITypeReference.
		/// May only be accessed within lock(typeReferenceCecilLoader).
		/// </summary>
		readonly MetadataLoader typeReferenceCecilLoader = new MetadataLoader();

		/// <summary>
		/// Dictionary for NRefactory->Cecil lookup.
		/// May only be accessed within lock(entityDict)
		/// </summary>
		Dictionary<IUnresolvedEntity, SRM.EntityHandle> entityDict = new Dictionary<IUnresolvedEntity, SRM.EntityHandle>();

		Dictionary<SRM.EntityHandle, IField> fieldLookupCache = new Dictionary<SRM.EntityHandle, IField>();
		Dictionary<SRM.EntityHandle, IProperty> propertyLookupCache = new Dictionary<SRM.EntityHandle, IProperty>();
		Dictionary<SRM.EntityHandle, IMethod> methodLookupCache = new Dictionary<SRM.EntityHandle, IMethod>();
		Dictionary<SRM.EntityHandle, IEvent> eventLookupCache = new Dictionary<SRM.EntityHandle, IEvent>();
		
		public DecompilerTypeSystem(Metadata.PEFile moduleDefinition)
		{
			if (moduleDefinition == null)
				throw new ArgumentNullException(nameof(moduleDefinition));
			this.moduleDefinition = moduleDefinition;
			MetadataLoader cecilLoader = new MetadataLoader { IncludeInternalMembers = true, LazyLoad = true, OnEntityLoaded = StoreMemberReference, ShortenInterfaceImplNames = false };
			typeReferenceCecilLoader.SetCurrentModule(moduleDefinition.GetMetadataReader());
			IUnresolvedAssembly mainAssembly = cecilLoader.LoadModule(moduleDefinition.GetMetadataReader());
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
					referencedAssemblies.Add(cecilLoader.LoadModule(asm.GetMetadataReader()));
					var metadata = asm.GetMetadataReader();
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
		
		public IAssembly MainAssembly {
			get { return compilation.MainAssembly; }
		}

		public Metadata.PEFile ModuleDefinition {
			get { return moduleDefinition; }
		}

		void StoreMemberReference(IUnresolvedEntity entity, SRM.EntityHandle mr)
		{
			// This is a callback from the type system, which is multi-threaded and may be accessed externally
			lock (entityDict)
				entityDict[entity] = mr;
		}

		/*
		Metadata.IMetadataEntity GetMetadata(IUnresolvedEntity member)
		{
			if (member == null)
				return null;
			lock (entityDict) {
				if (entityDict.TryGetValue(member, out var mr))
					return mr;
				return null;
			}
		}

		/// <summary>
		/// Retrieves the Cecil member definition for the specified member.
		/// </summary>
		/// <remarks>
		/// Returns null if the member is not defined in the module being decompiled.
		/// </remarks>
		public Metadata.IMetadataEntity GetMetadata(IMember member)
		{
			if (member == null)
				return null;
			return GetMetadata(member.UnresolvedMember);
		}


		/// <summary>
		/// Retrieves the Cecil type definition.
		/// </summary>
		/// <remarks>
		/// Returns null if the type is not defined in the module being decompiled.
		/// </remarks>
		public Metadata.TypeDefinition GetMetadata(ITypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
				return default;
			return (Metadata.TypeDefinition)GetMetadata(typeDefinition.Parts[0]);
		}
				*/


		public IMember ResolveAsMember(SRM.EntityHandle memberReference)
		{
			throw new NotImplementedException();
		}

		#region Resolve Type
		public IType ResolveAsType(SRM.EntityHandle typeReference)
		{
			if (typeReference.IsNil)
				return SpecialType.UnknownType;
			ITypeReference typeRef;
			lock (typeReferenceCecilLoader)
				typeRef = typeReferenceCecilLoader.ReadTypeReference(typeReference);
			return typeRef.Resolve(context);
		}
		#endregion

		#region Resolve Field
		public IField ResolveAsField(SRM.EntityHandle fieldReference)
		{
			if (fieldReference.IsNil)
				throw new ArgumentNullException(nameof(fieldReference));
			if (fieldReference.Kind != SRM.HandleKind.FieldDefinition && fieldReference.Kind != SRM.HandleKind.MemberReference)
				throw new ArgumentException("HandleKind must be either FieldDefinition or MemberReference", nameof(fieldReference));
			lock (fieldLookupCache) {
				IField field;
				if (!fieldLookupCache.TryGetValue(fieldReference, out field)) {
					var metadata = moduleDefinition.GetMetadataReader();
					IType declaringType;
					ITypeReference returnType;
					switch (fieldReference.Kind) {
						case SRM.HandleKind.FieldDefinition:
							var fieldDef = metadata.GetFieldDefinition((SRM.FieldDefinitionHandle)fieldReference);
							declaringType = ResolveAsType(fieldDef.GetDeclaringType());
							returnType = fieldDef.DecodeSignature(new TypeReferenceSignatureDecoder(), default);
							var declaringTypeDefinition = declaringType.GetDefinition();
							if (declaringTypeDefinition == null)
								field = CreateFakeField(declaringType, metadata.GetString(fieldDef.Name), returnType);
							else {
								field = declaringTypeDefinition.GetFields(f => f.MetadataToken == fieldReference, GetMemberOptions.IgnoreInheritedMembers).FirstOrDefault()
									?? CreateFakeField(declaringType, metadata.GetString(fieldDef.Name), returnType);
							}
							break;
						case SRM.HandleKind.MemberReference:
							var memberRef = metadata.GetMemberReference((SRM.MemberReferenceHandle)fieldReference);
							switch (memberRef.Parent.Kind) {
								case SRM.HandleKind.TypeReference:
									declaringType = ResolveAsType(memberRef.Parent);
									field = FindNonGenericField(metadata, memberRef, declaringType);
									break;
								case SRM.HandleKind.TypeSpecification:
									throw new NotImplementedException();
								/*if (fieldReference.DeclaringType.IsGenericInstance) {
								var git = (GenericInstanceType)fieldReference.DeclaringType;
								var typeArguments = git.GenericArguments.SelectArray(Resolve);
								field = (IField)field.Specialize(new TypeParameterSubstitution(typeArguments, null));
								}*/
								default:
									throw new NotSupportedException();
							}
							break;
					}
					fieldLookupCache.Add(fieldReference, field);
				}
				return field;
			}
		}

		IField FindNonGenericField(SRM.MetadataReader metadata, SRM.MemberReference memberRef, IType declaringType)
		{
			string name = metadata.GetString(memberRef.Name);
			ITypeDefinition typeDef = declaringType.GetDefinition();
			ITypeReference returnType = memberRef.DecodeFieldSignature(new TypeReferenceSignatureDecoder(), default);

			if (typeDef == null)
				return CreateFakeField(declaringType, name, returnType);
			foreach (IField field in typeDef.Fields)
				if (field.Name == name)
					return field;
			return CreateFakeField(declaringType, name, returnType);
		}

		IField CreateFakeField(IType declaringType, string name, ITypeReference returnType)
		{
			var f = new DefaultUnresolvedField();
			f.Name = name;
			f.ReturnType = returnType;
			return new ResolvedFakeField(f, context.WithCurrentTypeDefinition(declaringType.GetDefinition()), declaringType);
		}

		class ResolvedFakeField : DefaultResolvedField
		{
			readonly IType declaringType;

			public ResolvedFakeField(DefaultUnresolvedField unresolved, ITypeResolveContext parentContext, IType declaringType)
				: base(unresolved, parentContext)
			{
				this.declaringType = declaringType;
			}

			public override IType DeclaringType
			{
				get { return declaringType; }
			}
		}
		#endregion

		#region Resolve Method
		public IMethod ResolveAsMethod(SRM.EntityHandle methodReference)
		{
			if (methodReference.IsNil)
				throw new ArgumentNullException(nameof(methodReference));
			if (methodReference.Kind != SRM.HandleKind.MethodDefinition && methodReference.Kind != SRM.HandleKind.MemberReference && methodReference.Kind != SRM.HandleKind.MethodSpecification)
				throw new ArgumentException("HandleKind must be either a MethodDefinition, MemberReference or MethodSpecification", nameof(methodReference));
			lock (methodLookupCache) {
				IMethod method;
				if (!methodLookupCache.TryGetValue(methodReference, out method)) {
					var metadata = moduleDefinition.GetMetadataReader();
					IType declaringType;
					switch (methodReference.Kind) {
						case SRM.HandleKind.MethodDefinition:
							var methodDef = metadata.GetMethodDefinition((SRM.MethodDefinitionHandle)methodReference);
							declaringType = ResolveAsType(methodDef.GetDeclaringType());
							var declaringTypeDefinition = declaringType.GetDefinition();
							if (declaringTypeDefinition == null)
								method = CreateFakeMethod(declaringType, metadata.GetString(methodDef.Name), methodDef.DecodeSignature(new TypeReferenceSignatureDecoder(), default));
							else {
								method = (IMethod)declaringTypeDefinition.GetMembers(m => m.MetadataToken == methodReference, GetMemberOptions.IgnoreInheritedMembers).FirstOrDefault()
									?? CreateFakeMethod(declaringType, metadata.GetString(methodDef.Name), methodDef.DecodeSignature(new TypeReferenceSignatureDecoder(), default));
							}
							break;
						case SRM.HandleKind.MemberReference:
							var memberRef = metadata.GetMemberReference((SRM.MemberReferenceHandle)methodReference);
							break;
						case SRM.HandleKind.MethodSpecification:
							break;
					}

					/*method = FindNonGenericMethod(metadata, new Metadata.Entity(moduleDefinition, methodReference).ResolveAsMethod(), signature);
					switch (methodReference.Kind) {
						case SRM.HandleKind.StandaloneSignature:
							var standaloneSignature = metadata.GetStandaloneSignature((SRM.StandaloneSignatureHandle)methodReference);
							Debug.Assert(standaloneSignature.GetKind() == SRM.StandaloneSignatureKind.Method);
							var signature = standaloneSignature.DecodeMethodSignature(new TypeReferenceSignatureDecoder(), default);
						method = new VarArgInstanceMethod(
							method,
								signature.ParameterTypes.Skip(signature.RequiredParameterCount).Select(p => p.Resolve(context))
						);
							break;
						case SRM.HandleKind.MethodSpecification:
						IReadOnlyList<IType> classTypeArguments = null;
						IReadOnlyList<IType> methodTypeArguments = null;
							var methodSpec = metadata.GetMethodSpecification((SRM.MethodSpecificationHandle)methodReference);
							var typeArguments = methodSpec.DecodeSignature(new TypeReferenceSignatureDecoder(), default);
							if (typeArguments.Length > 0) {
								methodTypeArguments = typeArguments.SelectArray(arg => arg.Resolve(context));
						}
							var methodmethodSpec.Method.ResolveAsMethod(moduleDefinition);
						if (methodReference.DeclaringType.IsGenericInstance) {
							var git = (GenericInstanceType)methodReference.DeclaringType;
							classTypeArguments = git.GenericArguments.SelectArray(Resolve);
						}
						method = method.Specialize(new TypeParameterSubstitution(classTypeArguments, methodTypeArguments));
							break;

					}*/
					methodLookupCache.Add(methodReference, method);
				}
				return method;
			}
		}

		/*IMethod FindNonGenericMethod(SRM.MetadataReader metadata, Metadata.MethodDefinition method)
		{
			var methodDefinition = metadata.GetMethodDefinition(method.Handle);
			ITypeDefinition typeDef = ResolveAsType(methodDefinition.GetDeclaringType()).GetDefinition();
			if (typeDef == null)
				return CreateFakeMethod(methodDefinition);
			IEnumerable<IMethod> methods;
			var name = metadata.GetString(methodDefinition.Name);
			if (name == ".ctor") {
				methods = typeDef.GetConstructors();
			} else if (name == ".cctor") {
				return typeDef.Methods.FirstOrDefault(m => m.IsConstructor && m.IsStatic);
			} else {
				methods = typeDef.GetMethods(m => m.Name == name, GetMemberOptions.IgnoreInheritedMembers)
					.Concat(typeDef.GetAccessors(m => m.Name == name, GetMemberOptions.IgnoreInheritedMembers));
			}
			foreach (var m in methods) {
				if (GetMetadata(m).AsMethod() == method)
					return m;
			}
			IType[] parameterTypes;
			var signature = methodDefinition.DecodeSignature(new TypeReferenceSignatureDecoder(), default);
			if (signature.Header.CallingConvention == SRM.SignatureCallingConvention.VarArgs) {
				parameterTypes = signature.ParameterTypes
					.Take(signature.RequiredParameterCount)
					.Select(p => p.Resolve(context))
					.Concat(new[] { SpecialType.ArgList })
					.ToArray();
			} else {
				parameterTypes = signature.ParameterTypes.SelectArray(p => p.Resolve(context));
			}
			var returnType = signature.ReturnType.Resolve(context);
			foreach (var m in methods) {
				if (m.TypeParameters.Count != signature.GenericParameterCount)
					continue;
				if (!CompareSignatures(m.Parameters, parameterTypes) || !CompareTypes(m.ReturnType, returnType))
					continue;
				return m;
			}
			return CreateFakeMethod(methodDefinition);
		}*/
		
		static bool CompareTypes(IType a, IType b)
		{
			IType type1 = DummyTypeParameter.NormalizeAllTypeParameters(a);
			IType type2 = DummyTypeParameter.NormalizeAllTypeParameters(b);
			return type1.Equals(type2);
		}
		
		static bool IsVarArgMethod(IMethod method)
		{
			return method.Parameters.Count > 0 && method.Parameters[method.Parameters.Count - 1].Type.Kind == TypeKind.ArgList;
		}
		
		static bool CompareSignatures(IReadOnlyList<IParameter> parameters, IType[] parameterTypes)
		{
			if (parameterTypes.Length != parameters.Count)
				return false;
			for (int i = 0; i < parameterTypes.Length; i++) {
				if (!CompareTypes(parameterTypes[i], parameters[i].Type))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Create a dummy IMethod from the specified MethodReference
		/// </summary>
		IMethod CreateFakeMethod(IType declaringType, string name, SRM.MethodSignature<ITypeReference> signature)
		{
			var m = new DefaultUnresolvedMethod();
			if (name == ".ctor" || name == ".cctor")
				m.SymbolKind = SymbolKind.Constructor;
			m.Name = name;
			m.ReturnType = signature.ReturnType;
			m.IsStatic = !signature.Header.IsInstance;
			
			/*ITypeReference declaringTypeReference;
			lock (typeReferenceCecilLoader) {
				var metadata = moduleDefinition.GetMetadataReader();
				declaringTypeReference = typeReferenceCecilLoader.ReadTypeReference(methodDefinition.GetDeclaringType());
				string name = metadata.GetString(methodDefinition.Name);
				var gps = methodDefinition.GetGenericParameters();
				for (int i = 0; i < signature.GenericParameterCount; i++) {
					var gp = metadata.GetGenericParameter(gps[i]);
					m.TypeParameters.Add(new DefaultUnresolvedTypeParameter(SymbolKind.Method, i, metadata.GetString(gp.Name)));
				}
				var ps = methodDefinition.GetParameters();
				for (int i = 0; i < ps.Count; i++) {
					var p = metadata.GetParameter(ps.At(metadata, i));
					m.Parameters.Add(new DefaultUnresolvedParameter(signature.ParameterTypes[i], metadata.GetString(p.Name)));
				}
			}
			var type = declaringTypeReference.Resolve(context);*/
			return new ResolvedFakeMethod(m, context.WithCurrentTypeDefinition(declaringType.GetDefinition()), declaringType);
		}

		class ResolvedFakeMethod : DefaultResolvedMethod
		{
			readonly IType declaringType;

			public ResolvedFakeMethod(DefaultUnresolvedMethod unresolved, ITypeResolveContext parentContext, IType declaringType)
				: base(unresolved, parentContext)
			{
				this.declaringType = declaringType;
			}

			public override IType DeclaringType
			{
				get { return declaringType; }
			}
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
					var metadata = moduleDefinition.GetMetadataReader();
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
			ITypeDefinition typeDef = ResolveAsType(declaringType).GetDefinition();
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
					var metadata = moduleDefinition.GetMetadataReader();
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
			ITypeDefinition typeDef = ResolveAsType(declaringType).GetDefinition();
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
