using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;
using Mono.Cecil;

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
		readonly ModuleDefinition moduleDefinition;
		readonly ICompilation compilation;
		readonly ITypeResolveContext context;

		/// <summary>
		/// CecilLoader used for converting cecil type references to ITypeReference.
		/// May only be accessed within lock(typeReferenceCecilLoader).
		/// </summary>
		readonly CecilLoader typeReferenceCecilLoader = new CecilLoader();

		/// <summary>
		/// Dictionary for NRefactory->Cecil lookup.
		/// May only be accessed within lock(entityDict)
		/// </summary>
		Dictionary<IUnresolvedEntity, MemberReference> entityDict = new Dictionary<IUnresolvedEntity, MemberReference>();

		Dictionary<FieldReference, IField> fieldLookupCache = new Dictionary<FieldReference, IField>();
		Dictionary<PropertyReference, IProperty> propertyLookupCache = new Dictionary<PropertyReference, IProperty>();
		Dictionary<MethodReference, IMethod> methodLookupCache = new Dictionary<MethodReference, IMethod>();
		Dictionary<EventReference, IEvent> eventLookupCache = new Dictionary<EventReference, IEvent>();
		
		public DecompilerTypeSystem(ModuleDefinition moduleDefinition)
		{
			if (moduleDefinition == null)
				throw new ArgumentNullException(nameof(moduleDefinition));
			this.moduleDefinition = moduleDefinition;
			CecilLoader cecilLoader = new CecilLoader { IncludeInternalMembers = true, LazyLoad = true, OnEntityLoaded = StoreMemberReference, ShortenInterfaceImplNames = false };
			typeReferenceCecilLoader.SetCurrentModule(moduleDefinition);
			IUnresolvedAssembly mainAssembly = cecilLoader.LoadModule(moduleDefinition);
			// Load referenced assemblies and type-forwarder references.
			// This is necessary to make .NET Core/PCL binaries work better.
			var referencedAssemblies = new List<IUnresolvedAssembly>();
			var assemblyReferenceQueue = new Queue<AssemblyNameReference>(moduleDefinition.AssemblyReferences);
			var processedAssemblyReferences = new HashSet<AssemblyNameReference>(KeyComparer.Create((AssemblyNameReference reference) => reference.FullName));
			while (assemblyReferenceQueue.Count > 0) {
				var asmRef = assemblyReferenceQueue.Dequeue();
				if (!processedAssemblyReferences.Add(asmRef))
					continue;
				var asm = moduleDefinition.AssemblyResolver.Resolve(asmRef);
				if (asm != null) {
					referencedAssemblies.Add(cecilLoader.LoadAssembly(asm));
					foreach (var forwarder in asm.MainModule.ExportedTypes) {
						if (!forwarder.IsForwarder || !(forwarder.Scope is AssemblyNameReference forwarderRef)) continue;
						assemblyReferenceQueue.Enqueue(forwarderRef);
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

		public ModuleDefinition ModuleDefinition {
			get { return moduleDefinition; }
		}

		void StoreMemberReference(IUnresolvedEntity entity, MemberReference mr)
		{
			// This is a callback from the type system, which is multi-threaded and may be accessed externally
			lock (entityDict)
				entityDict[entity] = mr;
		}

		MemberReference GetCecil(IUnresolvedEntity member)
		{
			lock (entityDict) {
				MemberReference mr;
				if (member != null && entityDict.TryGetValue(member, out mr))
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
		public MemberReference GetCecil(IMember member)
		{
			if (member == null)
				return null;
			return GetCecil(member.UnresolvedMember);
		}

		/// <summary>
		/// Retrieves the Cecil type definition.
		/// </summary>
		/// <remarks>
		/// Returns null if the type is not defined in the module being decompiled.
		/// </remarks>
		public TypeDefinition GetCecil(ITypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
				return null;
			return GetCecil(typeDefinition.Parts[0]) as TypeDefinition;
		}

		#region Resolve Type
		public IType Resolve(TypeReference typeReference)
		{
			if (typeReference == null)
				return SpecialType.UnknownType;
			// We need to skip SentinelType and PinnedType.
			// But PinnedType can be nested within modopt, so we'll also skip those.
			while (typeReference is OptionalModifierType || typeReference is RequiredModifierType) {
				typeReference = ((TypeSpecification)typeReference).ElementType;
			}
			if (typeReference is SentinelType || typeReference is PinnedType) {
				typeReference = ((TypeSpecification)typeReference).ElementType;
			}
			ITypeReference typeRef;
			lock (typeReferenceCecilLoader)
				typeRef = typeReferenceCecilLoader.ReadTypeReference(typeReference);
			return typeRef.Resolve(context);
		}
		#endregion

		#region Resolve Field
		public IField Resolve(FieldReference fieldReference)
		{
			if (fieldReference == null)
				throw new ArgumentNullException(nameof(fieldReference));
			lock (fieldLookupCache) {
				IField field;
				if (!fieldLookupCache.TryGetValue(fieldReference, out field)) {
					field = FindNonGenericField(fieldReference);
					if (fieldReference.DeclaringType.IsGenericInstance) {
						var git = (GenericInstanceType)fieldReference.DeclaringType;
						var typeArguments = git.GenericArguments.SelectArray(Resolve);
						field = (IField)field.Specialize(new TypeParameterSubstitution(typeArguments, null));
					}
					fieldLookupCache.Add(fieldReference, field);
				}
				return field;
			}
		}

		IField FindNonGenericField(FieldReference fieldReference)
		{
			ITypeDefinition typeDef = Resolve(fieldReference.DeclaringType).GetDefinition();
			if (typeDef == null)
				return CreateFakeField(fieldReference);
			foreach (IField field in typeDef.Fields)
				if (field.Name == fieldReference.Name)
					return field;
			return CreateFakeField(fieldReference);
		}

		IField CreateFakeField(FieldReference fieldReference)
		{
			var declaringType = Resolve(fieldReference.DeclaringType);
			var f = new DefaultUnresolvedField();
			f.Name = fieldReference.Name;
			lock (typeReferenceCecilLoader) {
				f.ReturnType = typeReferenceCecilLoader.ReadTypeReference(fieldReference.FieldType);
			}
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
		public IMethod Resolve(MethodReference methodReference)
		{
			if (methodReference == null)
				throw new ArgumentNullException(nameof(methodReference));
			lock (methodLookupCache) {
				IMethod method;
				if (!methodLookupCache.TryGetValue(methodReference, out method)) {
					method = FindNonGenericMethod(methodReference.GetElementMethod());
					if (methodReference.CallingConvention == MethodCallingConvention.VarArg) {
						method = new VarArgInstanceMethod(
							method,
							methodReference.Parameters.SkipWhile(p => !p.ParameterType.IsSentinel).Select(p => Resolve(p.ParameterType))
						);
					} else if (methodReference.IsGenericInstance || methodReference.DeclaringType.IsGenericInstance) {
						IList<IType> classTypeArguments = null;
						IList<IType> methodTypeArguments = null;
						if (methodReference.IsGenericInstance) {
							var gim = ((GenericInstanceMethod)methodReference);
							methodTypeArguments = gim.GenericArguments.SelectArray(Resolve);
						}
						if (methodReference.DeclaringType.IsGenericInstance) {
							var git = (GenericInstanceType)methodReference.DeclaringType;
							classTypeArguments = git.GenericArguments.SelectArray(Resolve);
						}
						method = method.Specialize(new TypeParameterSubstitution(classTypeArguments, methodTypeArguments));
					}
					methodLookupCache.Add(methodReference, method);
				}
				return method;
			}
		}

		IMethod FindNonGenericMethod(MethodReference methodReference)
		{
			ITypeDefinition typeDef = Resolve(methodReference.DeclaringType).GetDefinition();
			if (typeDef == null)
				return CreateFakeMethod(methodReference);
			IEnumerable<IMethod> methods;
			if (methodReference.Name == ".ctor") {
				methods = typeDef.GetConstructors();
			} else if (methodReference.Name == ".cctor") {
				return typeDef.Methods.FirstOrDefault(m => m.IsConstructor && m.IsStatic);
			} else {
				methods = typeDef.GetMethods(m => m.Name == methodReference.Name, GetMemberOptions.IgnoreInheritedMembers)
					.Concat(typeDef.GetAccessors(m => m.Name == methodReference.Name, GetMemberOptions.IgnoreInheritedMembers));
			}
			foreach (var method in methods) {
				if (GetCecil(method) == methodReference)
					return method;
			}
			IType[] parameterTypes;
			if (methodReference.CallingConvention == MethodCallingConvention.VarArg) {
				parameterTypes = methodReference.Parameters
					.TakeWhile(p => !p.ParameterType.IsSentinel)
					.Select(p => Resolve(p.ParameterType))
					.Concat(new[] { SpecialType.ArgList })
					.ToArray();
			} else {
				parameterTypes = methodReference.Parameters.SelectArray(p => Resolve(p.ParameterType));
			}
			var returnType = Resolve(methodReference.ReturnType);
			foreach (var method in methods) {
				if (method.TypeParameters.Count != methodReference.GenericParameters.Count)
					continue;
				if (!CompareSignatures(method.Parameters, parameterTypes) || !CompareTypes(method.ReturnType, returnType))
					continue;
				return method;
			}
			return CreateFakeMethod(methodReference);
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
		
		static bool CompareSignatures(IList<IParameter> parameters, IType[] parameterTypes)
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
		IMethod CreateFakeMethod(MethodReference methodReference)
		{
			var m = new DefaultUnresolvedMethod();
			ITypeReference declaringTypeReference;
			lock (typeReferenceCecilLoader) {
				declaringTypeReference = typeReferenceCecilLoader.ReadTypeReference(methodReference.DeclaringType);
				if (methodReference.Name == ".ctor" || methodReference.Name == ".cctor")
					m.SymbolKind = SymbolKind.Constructor;
				m.Name = methodReference.Name;
				m.ReturnType = typeReferenceCecilLoader.ReadTypeReference(methodReference.ReturnType);
				m.IsStatic = !methodReference.HasThis;
				for (int i = 0; i < methodReference.GenericParameters.Count; i++) {
					m.TypeParameters.Add(new DefaultUnresolvedTypeParameter(SymbolKind.Method, i, methodReference.GenericParameters[i].Name));
				}
				foreach (var p in methodReference.Parameters) {
					m.Parameters.Add(new DefaultUnresolvedParameter(typeReferenceCecilLoader.ReadTypeReference(p.ParameterType), p.Name));
				}
			}
			var type = declaringTypeReference.Resolve(context);
			return new ResolvedFakeMethod(m, context.WithCurrentTypeDefinition(type.GetDefinition()), type);
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
		public IProperty Resolve(PropertyReference propertyReference)
		{
			if (propertyReference == null)
				throw new ArgumentNullException(nameof(propertyReference));
			lock (propertyLookupCache) {
				IProperty property;
				if (!propertyLookupCache.TryGetValue(propertyReference, out property)) {
					property = FindNonGenericProperty(propertyReference);
					if (propertyReference.DeclaringType.IsGenericInstance) {
						var git = (GenericInstanceType)propertyReference.DeclaringType;
						var typeArguments = git.GenericArguments.SelectArray(Resolve);
						property = (IProperty)property.Specialize(new TypeParameterSubstitution(typeArguments, null));
					}
					propertyLookupCache.Add(propertyReference, property);
				}
				return property;
			}
		}

		IProperty FindNonGenericProperty(PropertyReference propertyReference)
		{
			ITypeDefinition typeDef = Resolve(propertyReference.DeclaringType).GetDefinition();
			if (typeDef == null)
				return null;
			var parameterTypes = propertyReference.Parameters.SelectArray(p => Resolve(p.ParameterType));
			var returnType = Resolve(propertyReference.PropertyType);
			foreach (IProperty property in typeDef.Properties) {
				if (property.Name == propertyReference.Name
				    && CompareTypes(property.ReturnType, returnType)
				    && CompareSignatures(property.Parameters, parameterTypes))
					return property;
			}
			return null;
		}
		#endregion

		#region Resolve Event
		public IEvent Resolve(EventReference eventReference)
		{
			if (eventReference == null)
				throw new ArgumentNullException("propertyReference");
			lock (eventLookupCache) {
				IEvent ev;
				if (!eventLookupCache.TryGetValue(eventReference, out ev)) {
					ev = FindNonGenericEvent(eventReference);
					if (eventReference.DeclaringType.IsGenericInstance) {
						var git = (GenericInstanceType)eventReference.DeclaringType;
						var typeArguments = git.GenericArguments.SelectArray(Resolve);
						ev = (IEvent)ev.Specialize(new TypeParameterSubstitution(typeArguments, null));
					}
					eventLookupCache.Add(eventReference, ev);
				}
				return ev;
			}
		}

		IEvent FindNonGenericEvent(EventReference eventReference)
		{
			ITypeDefinition typeDef = Resolve(eventReference.DeclaringType).GetDefinition();
			if (typeDef == null)
				return null;
			var returnType = Resolve(eventReference.EventType);
			foreach (IEvent ev in typeDef.Events) {
				if (ev.Name == eventReference.Name && CompareTypes(ev.ReturnType, returnType))
					return ev;
			}
			return null;
		}
		#endregion
	}
}
