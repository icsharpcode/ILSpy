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
			typeReferenceCecilLoader.SetCurrentModule(moduleDefinition);
			IUnresolvedAssembly mainAssembly = cecilLoader.LoadModule(moduleDefinition);
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
					referencedAssemblies.Add(cecilLoader.LoadModule(asm));
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

		public SRM.MetadataReader GetMetadata() => moduleDefinition.GetMetadataReader();

		void StoreMemberReference(IUnresolvedEntity entity, SRM.EntityHandle mr)
		{
			// This is a callback from the type system, which is multi-threaded and may be accessed externally
			lock (entityDict)
				entityDict[entity] = mr;
		}

		public IType ResolveFromSignature(ITypeReference typeReference)
		{
			return typeReference.Resolve(context);
		}

		public IMember ResolveAsMember(SRM.EntityHandle memberReference)
		{
			switch (memberReference.Kind) {
				case SRM.HandleKind.FieldDefinition:
					return ResolveAsField(memberReference);
				case SRM.HandleKind.MethodDefinition:
					return ResolveAsMethod(memberReference);
				case SRM.HandleKind.MemberReference:
					var mr = moduleDefinition.GetMetadataReader().GetMemberReference((SRM.MemberReferenceHandle)memberReference);
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
							returnType = DynamicAwareTypeReference.Create(fieldDef.DecodeSignature(TypeReferenceSignatureDecoder.Instance, default), fieldDef.GetCustomAttributes(), metadata);
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
							Debug.Assert(memberRef.GetKind() == SRM.MemberReferenceKind.Field);
							declaringType = ResolveAsType(memberRef.Parent);
							field = FindNonGenericField(metadata, memberRef, declaringType);
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
			}
		}

		IField FindNonGenericField(SRM.MetadataReader metadata, SRM.MemberReference memberRef, IType declaringType)
		{
			string name = metadata.GetString(memberRef.Name);
			ITypeDefinition typeDef = declaringType.GetDefinition();
			ITypeReference returnType = memberRef.DecodeFieldSignature(TypeReferenceSignatureDecoder.Instance, default);

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
				IType declaringType;
				IReadOnlyList<IType> classTypeArguments = null;
				IReadOnlyList<IType> methodTypeArguments = null;
				SRM.MethodSignature<ITypeReference>? signature = null;
				if (!methodLookupCache.TryGetValue(methodReference, out method)) {
					var metadata = moduleDefinition.GetMetadataReader();
					switch (methodReference.Kind) {
						case SRM.HandleKind.MethodDefinition:
							var methodDef = metadata.GetMethodDefinition((SRM.MethodDefinitionHandle)methodReference);
							signature = methodDef.DecodeSignature(TypeReferenceSignatureDecoder.Instance, default);
							method = FindNonGenericMethod(metadata, methodReference, out declaringType);
							break;
						case SRM.HandleKind.MemberReference:
							var memberRef = metadata.GetMemberReference((SRM.MemberReferenceHandle)methodReference);
							Debug.Assert(memberRef.GetKind() == SRM.MemberReferenceKind.Method);
							signature = memberRef.DecodeMethodSignature(TypeReferenceSignatureDecoder.Instance, default);
							var elementMethod = methodReference;
							if (memberRef.Parent.Kind == SRM.HandleKind.MethodDefinition) {
								elementMethod = (SRM.MethodDefinitionHandle)memberRef.Parent;
							}
							method = FindNonGenericMethod(metadata, elementMethod, out declaringType);
							break;
						case SRM.HandleKind.MethodSpecification:
							var methodSpec = metadata.GetMethodSpecification((SRM.MethodSpecificationHandle)methodReference);
							method = FindNonGenericMethod(metadata, methodSpec.Method, out declaringType);
							var typeArguments = methodSpec.DecodeSignature(TypeReferenceSignatureDecoder.Instance, default);
							if (typeArguments.Length > 0) {
								methodTypeArguments = typeArguments.SelectArray(arg => arg.Resolve(context));
							}
							break;
						default:
							throw new NotSupportedException();
					}
					if (signature?.Header.CallingConvention == SRM.SignatureCallingConvention.VarArgs) {
						method = new VarArgInstanceMethod(method, signature.Value.ParameterTypes.Skip(signature.Value.RequiredParameterCount).Select(p => p.Resolve(context)));
					}
					if (declaringType.TypeArguments.Count > 0) {
						classTypeArguments = declaringType.TypeArguments.ToList();
					}
					if (classTypeArguments != null || methodTypeArguments != null) {
						method = method.Specialize(new TypeParameterSubstitution(classTypeArguments, methodTypeArguments));
					}

					methodLookupCache.Add(methodReference, method);
				}
				return method;
			}
		}

		IMethod FindNonGenericMethod(SRM.MetadataReader metadata, SRM.EntityHandle methodReference, out IType declaringType)
		{
			ITypeDefinition declaringTypeDefinition;
			string name;
			switch (methodReference.Kind) {
				case SRM.HandleKind.MethodDefinition:
					var methodDef = metadata.GetMethodDefinition((SRM.MethodDefinitionHandle)methodReference);
					declaringType = ResolveAsType(methodDef.GetDeclaringType());
					declaringTypeDefinition = declaringType.GetDefinition();
					if (declaringTypeDefinition == null) {
						return CreateFakeMethod(declaringType, metadata.GetString(methodDef.Name), methodDef.DecodeSignature(TypeReferenceSignatureDecoder.Instance, default));
					}
					name = metadata.GetString(methodDef.Name);
					IMethod method;
					if (name == ".ctor") {
						method = declaringTypeDefinition.GetConstructors(m => m.MetadataToken == methodReference, GetMemberOptions.IgnoreInheritedMembers).FirstOrDefault();
					} else if (name == ".cctor") {
						method = declaringTypeDefinition.Methods.FirstOrDefault(m => m.MetadataToken == methodReference);
					} else {
						method = declaringTypeDefinition.GetMethods(m => m.MetadataToken == methodReference, GetMemberOptions.IgnoreInheritedMembers)
							.Concat(declaringTypeDefinition.GetAccessors(m => m.MetadataToken == methodReference, GetMemberOptions.IgnoreInheritedMembers)).FirstOrDefault();
					}
					return method ?? CreateFakeMethod(declaringType, metadata.GetString(methodDef.Name), methodDef.DecodeSignature(TypeReferenceSignatureDecoder.Instance, default));
				case SRM.HandleKind.MemberReference:
					var memberRef = metadata.GetMemberReference((SRM.MemberReferenceHandle)methodReference);
					Debug.Assert(memberRef.GetKind() == SRM.MemberReferenceKind.Method);
					// TODO : Support other handles
					switch (memberRef.Parent.Kind) {
						case SRM.HandleKind.MethodDefinition:
							FindNonGenericMethod(metadata, memberRef.Parent, out declaringType);
							break;
						default:
							declaringType = ResolveAsType(memberRef.Parent);
							break;
					}
					declaringTypeDefinition = declaringType.GetDefinition();
					var signature = memberRef.DecodeMethodSignature(TypeReferenceSignatureDecoder.Instance, default);
					if (declaringTypeDefinition == null) {
						return CreateFakeMethod(declaringType, metadata.GetString(memberRef.Name), signature);
					}
					IEnumerable<IMethod> methods;
					name = metadata.GetString(memberRef.Name);
					if (name == ".ctor") {
						methods = declaringTypeDefinition.GetConstructors();
					} else if (name == ".cctor") {
						return declaringTypeDefinition.Methods.FirstOrDefault(m => m.IsConstructor && m.IsStatic);
					} else {
						methods = declaringTypeDefinition.GetMethods(m => m.Name == name, GetMemberOptions.IgnoreInheritedMembers)
							.Concat(declaringTypeDefinition.GetAccessors(m => m.Name == name, GetMemberOptions.IgnoreInheritedMembers));
					}
					IType[] parameterTypes;
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
					return CreateFakeMethod(declaringType, metadata.GetString(memberRef.Name), signature);
				default:
					throw new NotSupportedException();
			}
		}
		
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
			m.MetadataToken = default(SRM.MethodDefinitionHandle);
			m.ReturnType = signature.ReturnType;
			m.IsStatic = !signature.Header.IsInstance;
			
			lock (typeReferenceCecilLoader) {
				var metadata = moduleDefinition.GetMetadataReader();
				for (int i = 0; i < signature.GenericParameterCount; i++) {
					m.TypeParameters.Add(new DefaultUnresolvedTypeParameter(SymbolKind.Method, i, ""));
				}
				for (int i = 0; i < signature.ParameterTypes.Length; i++) {
					m.Parameters.Add(new DefaultUnresolvedParameter(signature.ParameterTypes[i], ""));
				}
			}
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
