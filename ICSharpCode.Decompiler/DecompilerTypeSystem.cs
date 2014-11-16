using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Mono.Cecil;

namespace ICSharpCode.Decompiler
{
	/// <summary>
	/// Manages the NRefactory type system for the decompiler.
	/// This class is thread-safe.
	/// </summary>
	public class DecompilerTypeSystem
	{
		readonly ModuleDefinition moduleDefinition;
		readonly ICompilation compilation;
		readonly ITypeResolveContext context;

		/// <summary>
		/// CecilLoader used for converting cecil type references to ITypeReference.
		/// May only be accessed within lock(cecilLoader).
		/// </summary>
		CecilLoader typeReferenceCecilLoader = new CecilLoader();

		/// <summary>
		/// Dictionary for NRefactory->Cecil lookup. Only contains entities from the main module.
		/// May only be accessed within lock(entityDict)
		/// </summary>
		Dictionary<IUnresolvedEntity, MemberReference> entityDict = new Dictionary<IUnresolvedEntity, MemberReference>();

		Dictionary<FieldReference, IField> fieldLookupCache = new Dictionary<FieldReference, IField>();
		Dictionary<MethodReference, IMethod> methodLookupCache = new Dictionary<MethodReference, IMethod>();
		
		public DecompilerTypeSystem(ModuleDefinition moduleDefinition)
		{
			if (moduleDefinition == null)
				throw new ArgumentNullException("moduleDefinition");
			this.moduleDefinition = moduleDefinition;
			CecilLoader mainAssemblyCecilLoader = new CecilLoader { IncludeInternalMembers = true, LazyLoad = true, OnEntityLoaded = StoreMemberReference };
			CecilLoader referencedAssemblyCecilLoader = new CecilLoader { IncludeInternalMembers = true, LazyLoad = true };
			typeReferenceCecilLoader.SetCurrentModule(moduleDefinition);
			IUnresolvedAssembly mainAssembly = mainAssemblyCecilLoader.LoadModule(moduleDefinition);
			var referencedAssemblies = new List<IUnresolvedAssembly>();
			foreach (var asmRef in moduleDefinition.AssemblyReferences) {
				var asm = moduleDefinition.AssemblyResolver.Resolve(asmRef);
				if (asm != null)
					referencedAssemblies.Add(referencedAssemblyCecilLoader.LoadAssembly(asm));
			}
			compilation = new SimpleCompilation(mainAssembly, referencedAssemblies);
			context = new SimpleTypeResolveContext(compilation.MainAssembly);
		}

		public ICompilation Compilation
		{
			get { return compilation; }
		}

		public ModuleDefinition ModuleDefinition
		{
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
		/// <summary>
		/// Retrieves a type definition for a type defined in the compilation's main assembly.
		/// </summary>
		public IType GetType(TypeReference typeReference)
		{
			if (typeReference == null)
				return SpecialType.UnknownType;
			ITypeReference typeRef;
			lock (typeReferenceCecilLoader)
				typeRef = typeReferenceCecilLoader.ReadTypeReference(typeReference);
			return typeRef.Resolve(context);
		}
		#endregion

		#region Resolve Field
		public IField GetField(FieldReference fieldReference)
		{
			if (fieldReference == null)
				throw new ArgumentNullException("fieldReference");
			lock (fieldLookupCache) {
				IField field;
				if (!fieldLookupCache.TryGetValue(fieldReference, out field)) {
					field = GetNonGenericField(fieldReference);
					// TODO: specialize the field if necessary
					fieldLookupCache[fieldReference] = field;
				}
				return field;
			}
		}

		IField GetNonGenericField(FieldReference fieldReference)
		{
			Debug.Assert(Monitor.IsEntered(fieldLookupCache));
			IField field;
			if (!fieldLookupCache.TryGetValue(fieldReference, out field)) {
				field = FindNonGenericField(fieldReference);
				fieldLookupCache.Add(fieldReference, field);
			}
			return field;
		}

		IField FindNonGenericField(FieldReference fieldReference)
		{
			ITypeDefinition typeDef = GetType(fieldReference.DeclaringType).GetDefinition();
			if (typeDef == null)
				return CreateFakeField(fieldReference);
			foreach (IField field in typeDef.Fields)
				if (field.Name == fieldReference.Name)
					return field;
			return CreateFakeField(fieldReference);
		}

		IField CreateFakeField(FieldReference fieldReference)
		{
			var declaringType = GetType(fieldReference.DeclaringType);
			var f = new DefaultUnresolvedField();
			f.Name = fieldReference.Name;
			f.ReturnType = typeReferenceCecilLoader.ReadTypeReference(fieldReference.FieldType);
			return new ResolvedFakeField(f, context, declaringType);
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
		public IMethod GetMethod(MethodReference methodReference)
		{
			if (methodReference == null)
				throw new ArgumentNullException("methodReference");
			lock (methodLookupCache) {
				IMethod method;
				if (!methodLookupCache.TryGetValue(methodReference, out method)) {
					method = GetNonGenericMethod(methodReference.GetElementMethod());
					// TODO: specialize the method

					methodLookupCache[methodReference] = method;
				}
				return method;
			}
		}

		IMethod GetNonGenericMethod(MethodReference methodReference)
		{
			Debug.Assert(Monitor.IsEntered(methodLookupCache));
			IMethod method;
			if (!methodLookupCache.TryGetValue(methodReference, out method)) {
				method = FindNonGenericMethod(methodReference);
				methodLookupCache.Add(methodReference, method);
			}
			return method;
		}

		IMethod FindNonGenericMethod(MethodReference methodReference)
		{
			ITypeDefinition typeDef = GetType(methodReference.DeclaringType).GetDefinition();
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
			var parameterTypes = methodReference.Parameters.SelectArray(p => GetType(p.ParameterType));
			foreach (var method in methods) {
				if (CompareSignatures(method.Parameters, parameterTypes))
					return method;
			}
			return CreateFakeMethod(methodReference);
		}

		static bool CompareSignatures(IList<IParameter> parameters, IType[] parameterTypes)
		{
			if (parameterTypes.Length != parameters.Count)
				return false;
			for (int i = 0; i < parameterTypes.Length; i++) {
				IType type1 = DummyTypeParameter.NormalizeAllTypeParameters(parameterTypes[i]);
				IType type2 = DummyTypeParameter.NormalizeAllTypeParameters(parameters[i].Type);
				if (!type1.Equals(type2)) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Create a dummy IMethod from the specified MethodReference
		/// </summary>
		IMethod CreateFakeMethod(MethodReference methodReference)
		{
			var declaringTypeReference = typeReferenceCecilLoader.ReadTypeReference(methodReference.DeclaringType);
			var m = new DefaultUnresolvedMethod();
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
			return new ResolvedFakeMethod(m, context, declaringTypeReference.Resolve(context));
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
	}
}
