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
		readonly TypeAttributeOptions typeAttributeOptions;

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
			typeAttributeOptions = TypeAttributeOptions.None;
			if (settings.Dynamic)
				typeAttributeOptions |= TypeAttributeOptions.Dynamic;
			if (settings.TupleTypes)
				typeAttributeOptions |= TypeAttributeOptions.Tuple;
			MetadataLoader loader = new MetadataLoader {
				IncludeInternalMembers = true,
				ShortenInterfaceImplNames = false,
				UseDynamicType = settings.Dynamic,
				UseTupleTypes = settings.TupleTypes,
				UseExtensionMethods = settings.ExtensionMethods,
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
		
		public IAssembly MainAssembly {
			get { return compilation.MainAssembly; }
		}

		public Metadata.PEFile ModuleDefinition {
			get { return moduleDefinition; }
		}

		public SRM.MetadataReader GetMetadata() => moduleDefinition.Metadata;

		public Metadata.PEFile GetModuleDefinition(IAssembly assembly)
		{
			if (assembly == MainAssembly)
				return ModuleDefinition;
			if (!moduleLookup.TryGetValue(assembly.UnresolvedAssembly, out var file))
				return null;
			return file;
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
				context,
				typeAttributes: null,
				typeAttributeOptions 
			);
		}

		IType ResolveDeclaringType(SRM.EntityHandle declaringTypeReference)
		{
			// resolve without substituting dynamic/tuple types
			return MetadataTypeReference.Resolve(
				declaringTypeReference,
				moduleDefinition.Metadata,
				context,
				typeAttributes: null,
				attributeOptions: TypeAttributeOptions.None
			);
		}
		#endregion
		
		public SRM.MethodSignature<IType> DecodeMethodSignature(SRM.StandaloneSignatureHandle handle)
		{
			var standaloneSignature = moduleDefinition.Metadata.GetStandaloneSignature(handle);
			if (standaloneSignature.GetKind() != SRM.StandaloneSignatureKind.Method)
				throw new InvalidOperationException("Expected Method signature");
			return standaloneSignature.DecodeMethodSignature(
				new TypeProvider(compilation.MainAssembly),
				context
			);
		}

		public ImmutableArray<IType> DecodeLocalSignature(SRM.StandaloneSignatureHandle handle)
		{
			var standaloneSignature = moduleDefinition.Metadata.GetStandaloneSignature(handle);
			if (standaloneSignature.GetKind() != SRM.StandaloneSignatureKind.LocalVariables)
				throw new InvalidOperationException("Expected Local signature");
			return standaloneSignature.DecodeLocalSignature(
				new TypeProvider(compilation.MainAssembly),
				context
			);
		}

		#region Resolve Field
		public IField ResolveAsField(SRM.EntityHandle fieldReference)
		{
			if (fieldReference.IsNil)
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
							var fieldDef = metadata.GetFieldDefinition(fieldDefHandle);
							declaringType = ResolveDeclaringType(fieldDef.GetDeclaringType());
							var declaringTypeDefinition = declaringType.GetDefinition();
							if (declaringTypeDefinition != null) {
								field = declaringTypeDefinition.GetFields(f => f.MetadataToken == fieldReference, GetMemberOptions.IgnoreInheritedMembers).FirstOrDefault();
							} else {
								field = null;
							}
							if (field == null) {
								field = new FakeField(compilation) {
									DeclaringType = declaringType,
									Name = metadata.GetString(fieldDef.Name),
									ReturnType = FieldTypeReference.Resolve(fieldDefHandle, metadata, context, typeAttributeOptions),
									IsStatic = (fieldDef.Attributes & System.Reflection.FieldAttributes.Static) != 0,
								};
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
			}
		}

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
			var returnType = memberRef.DecodeFieldSignature(new TypeProvider(context.CurrentAssembly), context);
			return new FakeField(compilation) {
				DeclaringType = declaringType,
				Name = name,
				ReturnType = returnType
			};
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
				if (!methodLookupCache.TryGetValue(methodReference, out IMethod method)) {
					var metadata = moduleDefinition.Metadata;
					switch (methodReference.Kind) {
						case SRM.HandleKind.MethodDefinition:
							method = ResolveMethodDefinition(metadata, (SRM.MethodDefinitionHandle)methodReference);
							break;
						case SRM.HandleKind.MemberReference:
							method = ResolveMethodReference(metadata, (SRM.MemberReferenceHandle)methodReference);
							break;
						case SRM.HandleKind.MethodSpecification:
							var methodSpec = metadata.GetMethodSpecification((SRM.MethodSpecificationHandle)methodReference);
							var methodTypeArgs = methodSpec.DecodeSignature(new TypeProvider(context.CurrentAssembly), context);
							if (methodSpec.Method.Kind == SRM.HandleKind.MethodDefinition) {
								// generic instance of a methoddef (=generic method in non-generic class in current assembly)
								method = ResolveMethodDefinition(metadata, (SRM.MethodDefinitionHandle)methodSpec.Method);
								method = method.Specialize(new TypeParameterSubstitution(null, methodTypeArgs));
							} else {
								method = ResolveMethodReference(metadata, (SRM.MemberReferenceHandle)methodSpec.Method, methodTypeArgs);
							}
							break;
						default:
							throw new NotSupportedException();
					}
					methodLookupCache.Add(methodReference, method);
				}
				return method;
			}
		}

		IMethod ResolveMethodDefinition(SRM.MetadataReader metadata, SRM.MethodDefinitionHandle methodDefHandle, bool expandVarArgs = true)
		{
			var methodDef = metadata.GetMethodDefinition(methodDefHandle);
			var declaringType = ResolveDeclaringType(methodDef.GetDeclaringType());
			var declaringTypeDefinition = declaringType.GetDefinition();
			string name = metadata.GetString(methodDef.Name);
			IMethod method;
			if (declaringTypeDefinition != null) {
				if (name == ".ctor") {
					method = declaringTypeDefinition.GetConstructors(m => m.MetadataToken == methodDefHandle, GetMemberOptions.IgnoreInheritedMembers).FirstOrDefault();
				} else if (name == ".cctor") {
					method = declaringTypeDefinition.Methods.FirstOrDefault(m => m.MetadataToken == methodDefHandle);
				} else {
					method = declaringTypeDefinition.GetMethods(m => m.MetadataToken == methodDefHandle, GetMemberOptions.IgnoreInheritedMembers)
						.Concat(declaringTypeDefinition.GetAccessors(m => m.MetadataToken == methodDefHandle, GetMemberOptions.IgnoreInheritedMembers)).FirstOrDefault();
				}
			} else {
				method = null;
			}
			if (method == null) {
				var signature = methodDef.DecodeSignature(new TypeProvider(context.CurrentAssembly),
					context.WithCurrentTypeDefinition(declaringTypeDefinition));
				method = CreateFakeMethod(declaringType, metadata.GetString(methodDef.Name), signature);
			}
			if (expandVarArgs && method.Parameters.LastOrDefault()?.Type.Kind == TypeKind.ArgList) {
				method = new VarArgInstanceMethod(method, EmptyList<IType>.Instance);
			}
			return method;
		}
		
		/// <summary>
		/// Resolves a method reference.
		/// </summary>
		/// <remarks>
		/// Class type arguments are provided by the declaring type stored in the memberRef.
		/// Method type arguments are provided by the caller.
		/// </remarks>
		IMethod ResolveMethodReference(SRM.MetadataReader metadata, SRM.MemberReferenceHandle memberRefHandle, IReadOnlyList<IType> methodTypeArguments = null)
		{
			var memberRef = metadata.GetMemberReference(memberRefHandle);
			Debug.Assert(memberRef.GetKind() == SRM.MemberReferenceKind.Method);
			SRM.MethodSignature<IType> signature;
			IReadOnlyList<IType> classTypeArguments = null;
			IMethod method;
			if (memberRef.Parent.Kind == SRM.HandleKind.MethodDefinition) {
				method = ResolveMethodDefinition(metadata, (SRM.MethodDefinitionHandle)memberRef.Parent, expandVarArgs: false);
				signature = memberRef.DecodeMethodSignature(new TypeProvider(context.CurrentAssembly), context);
			} else {
				var declaringType = ResolveDeclaringType(memberRef.Parent);
				var declaringTypeDefinition = declaringType.GetDefinition();
				if (declaringType.TypeArguments.Count > 0) {
					classTypeArguments = declaringType.TypeArguments;
				}
				// Note: declaringType might be parameterized, but the signature is for the original method definition.
				// We'll have to search the member directly on declaringTypeDefinition.
				string name = metadata.GetString(memberRef.Name);
				signature = memberRef.DecodeMethodSignature(new TypeProvider(context.CurrentAssembly),
					context.WithCurrentTypeDefinition(declaringTypeDefinition));
				if (declaringTypeDefinition != null) {
					// Find the set of overloads to search:
					IEnumerable<IMethod> methods;
					if (name == ".ctor") {
						methods = declaringTypeDefinition.GetConstructors();
					} else if (name == ".cctor") {
						methods = declaringTypeDefinition.Methods.Where(m => m.IsConstructor && m.IsStatic);
					} else {
						methods = declaringTypeDefinition.GetMethods(m => m.Name == name, GetMemberOptions.IgnoreInheritedMembers)
							.Concat(declaringTypeDefinition.GetAccessors(m => m.Name == name, GetMemberOptions.IgnoreInheritedMembers));
					}
					// Determine the expected parameters from the signature:
					ImmutableArray<IType> parameterTypes;
					if (signature.Header.CallingConvention == SRM.SignatureCallingConvention.VarArgs) {
						parameterTypes = signature.ParameterTypes
							.Take(signature.RequiredParameterCount)
							.Concat(new[] { SpecialType.ArgList })
							.ToImmutableArray();
					} else {
						parameterTypes = signature.ParameterTypes;
					}
					// Search for the matching method:
					method = null;
					foreach (var m in methods) {
						if (m.TypeParameters.Count != signature.GenericParameterCount)
							continue;
						if (CompareSignatures(m.Parameters, parameterTypes) && CompareTypes(m.ReturnType, signature.ReturnType)) {
							method = m;
							break;
						}
					}
				} else {
					method = null;
				}
				if (method == null) {
					method = CreateFakeMethod(declaringType, name, signature);
				}
			}
			if (classTypeArguments != null || methodTypeArguments != null) {
				method = method.Specialize(new TypeParameterSubstitution(classTypeArguments, methodTypeArguments));
			}
			if (signature.Header.CallingConvention == SRM.SignatureCallingConvention.VarArgs) {
				method = new VarArgInstanceMethod(method, signature.ParameterTypes.Skip(signature.RequiredParameterCount));
			}
			return method;
		}
		
		static readonly NormalizeTypeVisitor normalizeTypeVisitor = new NormalizeTypeVisitor {
			ReplaceClassTypeParametersWithDummy = true,
			ReplaceMethodTypeParametersWithDummy = true,
		};

		static bool CompareTypes(IType a, IType b)
		{
			IType type1 = a.AcceptVisitor(normalizeTypeVisitor);
			IType type2 = b.AcceptVisitor(normalizeTypeVisitor);
			return type1.Equals(type2);
		}
		
		static bool IsVarArgMethod(IMethod method)
		{
			return method.Parameters.Count > 0 && method.Parameters[method.Parameters.Count - 1].Type.Kind == TypeKind.ArgList;
		}
		
		static bool CompareSignatures(IReadOnlyList<IParameter> parameters, ImmutableArray<IType> parameterTypes)
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
		IMethod CreateFakeMethod(IType declaringType, string name, SRM.MethodSignature<IType> signature)
		{
			SymbolKind symbolKind = SymbolKind.Method;
			if (name == ".ctor" || name == ".cctor")
				symbolKind = SymbolKind.Constructor;
			var m = new FakeMethod(compilation, symbolKind);
			m.DeclaringType = declaringType;
			m.Name = name;
			m.ReturnType = signature.ReturnType;
			m.IsStatic = !signature.Header.IsInstance;

			var metadata = moduleDefinition.Metadata;
			TypeParameterSubstitution substitution = null;
			if (signature.GenericParameterCount > 0) {
				var typeParameters = new List<ITypeParameter>();
				for (int i = 0; i < signature.GenericParameterCount; i++) {
					typeParameters.Add(new DefaultTypeParameter(m, i));
				}
				m.TypeParameters = typeParameters;
				substitution = new TypeParameterSubstitution(null, typeParameters);
			}
			var parameters = new List<IParameter>();
			for (int i = 0; i < signature.RequiredParameterCount; i++) {
				var type = signature.ParameterTypes[i];
				if (substitution != null) {
					// replace the dummy method type parameters with the owned instances we just created
					type = type.AcceptVisitor(substitution);
				}
				parameters.Add(new DefaultParameter(type, ""));
			}
			m.Parameters = parameters;
			return m;
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
