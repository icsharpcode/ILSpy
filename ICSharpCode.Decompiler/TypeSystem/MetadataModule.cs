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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Type system implementation for Metadata.PEFile.
	/// </summary>
	[DebuggerDisplay("<MetadataModule: {AssemblyName}>")]
	public class MetadataModule : IModule
	{
		public ICompilation Compilation { get; }
		internal readonly MetadataReader metadata;
		readonly TypeSystemOptions options;
		internal readonly TypeProvider TypeProvider;
		internal readonly Nullability NullableContext;

		readonly MetadataNamespace rootNamespace;
		readonly MetadataTypeDefinition[] typeDefs;
		readonly MetadataField[] fieldDefs;
		readonly MetadataMethod[] methodDefs;
		readonly MetadataProperty[] propertyDefs;
		readonly MetadataEvent[] eventDefs;

		internal MetadataModule(ICompilation compilation, Metadata.PEFile peFile, TypeSystemOptions options)
		{
			this.Compilation = compilation;
			this.PEFile = peFile;
			this.metadata = peFile.Metadata;
			this.options = options;
			this.TypeProvider = new TypeProvider(this);

			// assembly metadata
			if (metadata.IsAssembly) {
				var asmdef = metadata.GetAssemblyDefinition();
				this.AssemblyName = metadata.GetString(asmdef.Name);
				this.FullAssemblyName = metadata.GetFullAssemblyName();
			} else {
				var moddef = metadata.GetModuleDefinition();
				this.AssemblyName = metadata.GetString(moddef.Name);
				this.FullAssemblyName = this.AssemblyName;
			}
			this.NullableContext = metadata.GetModuleDefinition().GetCustomAttributes().GetNullableContext(metadata) ?? Nullability.Oblivious;
			this.rootNamespace = new MetadataNamespace(this, null, string.Empty, metadata.GetNamespaceDefinitionRoot());

			if (!options.HasFlag(TypeSystemOptions.Uncached)) {
				// create arrays for resolved entities, indexed by row index
				this.typeDefs = new MetadataTypeDefinition[metadata.TypeDefinitions.Count + 1];
				this.fieldDefs = new MetadataField[metadata.FieldDefinitions.Count + 1];
				this.methodDefs = new MetadataMethod[metadata.MethodDefinitions.Count + 1];
				this.propertyDefs = new MetadataProperty[metadata.PropertyDefinitions.Count + 1];
				this.eventDefs = new MetadataEvent[metadata.EventDefinitions.Count + 1];
			}
		}

		internal string GetString(StringHandle name)
		{
			return metadata.GetString(name);
		}

		public TypeSystemOptions TypeSystemOptions => options;

		#region IAssembly interface
		public PEFile PEFile { get; }

		public bool IsMainModule => this == Compilation.MainModule;

		public string AssemblyName { get; }
		public string FullAssemblyName { get; }
		string ISymbol.Name => AssemblyName;
		SymbolKind ISymbol.SymbolKind => SymbolKind.Module;

		public INamespace RootNamespace => rootNamespace;

		public IEnumerable<ITypeDefinition> TopLevelTypeDefinitions => TypeDefinitions.Where(td => td.DeclaringTypeDefinition == null);
		
		public ITypeDefinition GetTypeDefinition(TopLevelTypeName topLevelTypeName)
		{
			var typeDefHandle = PEFile.GetTypeDefinition(topLevelTypeName);
			if (typeDefHandle.IsNil) {
				var forwarderHandle = PEFile.GetTypeForwarder(topLevelTypeName);
				if (!forwarderHandle.IsNil) {
					var forwarder = metadata.GetExportedType(forwarderHandle);
					return ResolveForwardedType(forwarder).GetDefinition();
				}
			}
			return GetDefinition(typeDefHandle);
		}
		#endregion

		#region InternalsVisibleTo
		public bool InternalsVisibleTo(IModule module)
		{
			if (this == module)
				return true;
			foreach (string shortName in GetInternalsVisibleTo()) {
				if (string.Equals(module.AssemblyName, shortName, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}

		string[] internalsVisibleTo;

		string[] GetInternalsVisibleTo()
		{
			var result = LazyInit.VolatileRead(ref this.internalsVisibleTo);
			if (result != null) {
				return result;
			}
			if (metadata.IsAssembly) {
				var list = new List<string>();
				foreach (var attrHandle in metadata.GetAssemblyDefinition().GetCustomAttributes()) {
					var attr = metadata.GetCustomAttribute(attrHandle);
					if (attr.IsKnownAttribute(metadata, KnownAttribute.InternalsVisibleTo)) {
						var attrValue = attr.DecodeValue(this.TypeProvider);
						if (attrValue.FixedArguments.Length == 1) {
							if (attrValue.FixedArguments[0].Value is string s) {
								list.Add(GetShortName(s));
							}
						}
					}
				}
				result = list.ToArray();
			} else {
				result = Empty<string>.Array;
			}
			return LazyInit.GetOrSet(ref this.internalsVisibleTo, result);
		}

		static string GetShortName(string fullAssemblyName)
		{
			if (fullAssemblyName == null)
				return null;
			int pos = fullAssemblyName.IndexOf(',');
			if (pos < 0)
				return fullAssemblyName;
			else
				return fullAssemblyName.Substring(0, pos);
		}
		#endregion

		#region GetDefinition
		/// <summary>
		/// Gets all types in the assembly, including nested types.
		/// </summary>
		public IEnumerable<ITypeDefinition> TypeDefinitions {
			get {
				foreach (var tdHandle in metadata.TypeDefinitions) {
					yield return GetDefinition(tdHandle);
				}
			}
		}

		public ITypeDefinition GetDefinition(TypeDefinitionHandle handle)
		{
			if (handle.IsNil)
				return null;
			if (typeDefs == null)
				return new MetadataTypeDefinition(this, handle);
			int row = MetadataTokens.GetRowNumber(handle);
			if (row >= typeDefs.Length)
				HandleOutOfRange(handle);
			var typeDef = LazyInit.VolatileRead(ref typeDefs[row]);
			if (typeDef != null)
				return typeDef;
			typeDef = new MetadataTypeDefinition(this, handle);
			return LazyInit.GetOrSet(ref typeDefs[row], typeDef);
		}

		public IField GetDefinition(FieldDefinitionHandle handle)
		{
			if (handle.IsNil)
				return null;
			if (fieldDefs == null)
				return new MetadataField(this, handle);
			int row = MetadataTokens.GetRowNumber(handle);
			if (row >= fieldDefs.Length)
				HandleOutOfRange(handle);
			var field = LazyInit.VolatileRead(ref fieldDefs[row]);
			if (field != null)
				return field;
			field = new MetadataField(this, handle);
			return LazyInit.GetOrSet(ref fieldDefs[row], field);
		}

		public IMethod GetDefinition(MethodDefinitionHandle handle)
		{
			if (handle.IsNil)
				return null;
			if (methodDefs == null)
				return new MetadataMethod(this, handle);
			int row = MetadataTokens.GetRowNumber(handle);
			Debug.Assert(row != 0);
			if (row >= methodDefs.Length)
				HandleOutOfRange(handle);
			var method = LazyInit.VolatileRead(ref methodDefs[row]);
			if (method != null)
				return method;
			method = new MetadataMethod(this, handle);
			return LazyInit.GetOrSet(ref methodDefs[row], method);
		}

		public IProperty GetDefinition(PropertyDefinitionHandle handle)
		{
			if (handle.IsNil)
				return null;
			if (propertyDefs == null)
				return new MetadataProperty(this, handle);
			int row = MetadataTokens.GetRowNumber(handle);
			Debug.Assert(row != 0);
			if (row >= methodDefs.Length)
				HandleOutOfRange(handle);
			var property = LazyInit.VolatileRead(ref propertyDefs[row]);
			if (property != null)
				return property;
			property = new MetadataProperty(this, handle);
			return LazyInit.GetOrSet(ref propertyDefs[row], property);
		}

		public IEvent GetDefinition(EventDefinitionHandle handle)
		{
			if (handle.IsNil)
				return null;
			if (eventDefs == null)
				return new MetadataEvent(this, handle);
			int row = MetadataTokens.GetRowNumber(handle);
			Debug.Assert(row != 0);
			if (row >= methodDefs.Length)
				HandleOutOfRange(handle);
			var ev = LazyInit.VolatileRead(ref eventDefs[row]);
			if (ev != null)
				return ev;
			ev = new MetadataEvent(this, handle);
			return LazyInit.GetOrSet(ref eventDefs[row], ev);
		}

		void HandleOutOfRange(EntityHandle handle)
		{
			throw new BadImageFormatException("Handle with invalid row number.");
		}
		#endregion

		#region Resolve Type
		public IType ResolveType(EntityHandle typeRefDefSpec, GenericContext context, CustomAttributeHandleCollection? typeAttributes = null, Nullability nullableContext = Nullability.Oblivious)
		{
			return ResolveType(typeRefDefSpec, context, options, typeAttributes, nullableContext);
		}

		public IType ResolveType(EntityHandle typeRefDefSpec, GenericContext context, TypeSystemOptions customOptions, CustomAttributeHandleCollection? typeAttributes = null, Nullability nullableContext = Nullability.Oblivious)
		{
			if (typeRefDefSpec.IsNil)
				return SpecialType.UnknownType;
			IType ty;
			switch (typeRefDefSpec.Kind) {
				case HandleKind.TypeDefinition:
					ty = TypeProvider.GetTypeFromDefinition(metadata, (TypeDefinitionHandle)typeRefDefSpec, 0);
					break;
				case HandleKind.TypeReference:
					ty = TypeProvider.GetTypeFromReference(metadata, (TypeReferenceHandle)typeRefDefSpec, 0);
					break;
				case HandleKind.TypeSpecification:
					var typeSpec = metadata.GetTypeSpecification((TypeSpecificationHandle)typeRefDefSpec);
					ty = typeSpec.DecodeSignature(TypeProvider, context);
					break;
				case HandleKind.ExportedType:
					return ResolveForwardedType(metadata.GetExportedType((ExportedTypeHandle)typeRefDefSpec));
				default:
					throw new BadImageFormatException("Not a type handle");
			}
			ty = ApplyAttributeTypeVisitor.ApplyAttributesToType(ty, Compilation, typeAttributes, metadata, customOptions, nullableContext);
			return ty;
		}

		IType ResolveDeclaringType(EntityHandle declaringTypeReference, GenericContext context)
		{
			// resolve without substituting dynamic/tuple types
			var ty = ResolveType(declaringTypeReference, context,
				options & ~(TypeSystemOptions.Dynamic | TypeSystemOptions.Tuple | TypeSystemOptions.NullabilityAnnotations));
			// but substitute tuple types in type arguments:
			ty = ApplyAttributeTypeVisitor.ApplyAttributesToType(ty, Compilation, null, metadata, options, Nullability.Oblivious, typeChildrenOnly: true);
			return ty;
		}

		IType IntroduceTupleTypes(IType ty)
		{
			// run ApplyAttributeTypeVisitor without attributes, in order to introduce tuple types
			return ApplyAttributeTypeVisitor.ApplyAttributesToType(ty, Compilation, null, metadata, options, Nullability.Oblivious);
		}
		#endregion

		#region Resolve Method
		public IMethod ResolveMethod(EntityHandle methodReference, GenericContext context)
		{
			if (methodReference.IsNil)
				throw new ArgumentNullException(nameof(methodReference));
			switch (methodReference.Kind) {
				case HandleKind.MethodDefinition:
					return ResolveMethodDefinition((MethodDefinitionHandle)methodReference, expandVarArgs: true);
				case HandleKind.MemberReference:
					return ResolveMethodReference((MemberReferenceHandle)methodReference, context, expandVarArgs: true);
				case HandleKind.MethodSpecification:
					return ResolveMethodSpecification((MethodSpecificationHandle)methodReference, context, expandVarArgs: true);
				default:
					throw new BadImageFormatException("Metadata token must be either a methoddef, memberref or methodspec");
			}
		}

		IMethod ResolveMethodDefinition(MethodDefinitionHandle methodDefHandle, bool expandVarArgs)
		{
			var method = GetDefinition(methodDefHandle);
			if (expandVarArgs && method.Parameters.LastOrDefault()?.Type.Kind == TypeKind.ArgList) {
				method = new VarArgInstanceMethod(method, EmptyList<IType>.Instance);
			}
			return method;
		}

		IMethod ResolveMethodSpecification(MethodSpecificationHandle methodSpecHandle, GenericContext context, bool expandVarArgs)
		{
			var methodSpec = metadata.GetMethodSpecification(methodSpecHandle);
			var methodTypeArgs = methodSpec.DecodeSignature(TypeProvider, context)
				.SelectReadOnlyArray(IntroduceTupleTypes);
			IMethod method;
			if (methodSpec.Method.Kind == HandleKind.MethodDefinition) {
				// generic instance of a methoddef (=generic method in non-generic class in current assembly)
				method = ResolveMethodDefinition((MethodDefinitionHandle)methodSpec.Method, expandVarArgs);
				method = method.Specialize(new TypeParameterSubstitution(null, methodTypeArgs));
			} else {
				method = ResolveMethodReference((MemberReferenceHandle)methodSpec.Method, context, methodTypeArgs, expandVarArgs);
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
		IMethod ResolveMethodReference(MemberReferenceHandle memberRefHandle, GenericContext context, IReadOnlyList<IType> methodTypeArguments = null, bool expandVarArgs = true)
		{
			var memberRef = metadata.GetMemberReference(memberRefHandle);
			Debug.Assert(memberRef.GetKind() == MemberReferenceKind.Method);
			MethodSignature<IType> signature;
			IReadOnlyList<IType> classTypeArguments = null;
			IMethod method;
			if (memberRef.Parent.Kind == HandleKind.MethodDefinition) {
				method = ResolveMethodDefinition((MethodDefinitionHandle)memberRef.Parent, expandVarArgs: false);
				signature = memberRef.DecodeMethodSignature(TypeProvider, context);
			} else {
				var declaringType = ResolveDeclaringType(memberRef.Parent, context);
				var declaringTypeDefinition = declaringType.GetDefinition();
				if (declaringType.TypeArguments.Count > 0) {
					classTypeArguments = declaringType.TypeArguments;
				}
				// Note: declaringType might be parameterized, but the signature is for the original method definition.
				// We'll have to search the member directly on declaringTypeDefinition.
				string name = metadata.GetString(memberRef.Name);
				signature = memberRef.DecodeMethodSignature(TypeProvider,
					new GenericContext(declaringTypeDefinition?.TypeParameters));
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
					if (signature.Header.CallingConvention == SignatureCallingConvention.VarArgs) {
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
			if (expandVarArgs && signature.Header.CallingConvention == SignatureCallingConvention.VarArgs) {
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
		IMethod CreateFakeMethod(IType declaringType, string name, MethodSignature<IType> signature)
		{
			SymbolKind symbolKind = SymbolKind.Method;
			if (name == ".ctor" || name == ".cctor")
				symbolKind = SymbolKind.Constructor;
			var m = new FakeMethod(Compilation, symbolKind);
			m.DeclaringType = declaringType;
			m.Name = name;
			m.ReturnType = signature.ReturnType;
			m.IsStatic = !signature.Header.IsInstance;

			TypeParameterSubstitution substitution = null;
			if (signature.GenericParameterCount > 0) {
				var typeParameters = new List<ITypeParameter>();
				for (int i = 0; i < signature.GenericParameterCount; i++) {
					typeParameters.Add(new DefaultTypeParameter(m, i));
				}
				m.TypeParameters = typeParameters;
				substitution = new TypeParameterSubstitution(declaringType.TypeArguments, typeParameters);
			} else if (declaringType.TypeArguments.Count > 0) {
				substitution = declaringType.GetSubstitution();
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

		#region Resolve Entity
		/// <summary>
		/// Resolves a symbol.
		/// </summary>
		/// <remarks>
		/// * Types are resolved to their definition, as IType does not implement ISymbol.
		///    * types without definition will resolve to <c>null</c>
		///    * use ResolveType() to properly resolve types
		/// * When resolving methods, varargs signatures are not expanded.
		///    * use ResolveMethod() instead to get an IMethod instance suitable for call-sites
		/// * May return specialized members, where generics are involved.
		/// * Other types of handles that don't correspond to TS entities, will return <c>null</c>.
		/// </remarks>
		public IEntity ResolveEntity(EntityHandle entityHandle, GenericContext context = default)
		{
			switch (entityHandle.Kind) {
				case HandleKind.TypeReference:
				case HandleKind.TypeDefinition:
				case HandleKind.TypeSpecification:
				case HandleKind.ExportedType:
					return ResolveType(entityHandle, context).GetDefinition();
				case HandleKind.MemberReference:
					var memberReferenceHandle = (MemberReferenceHandle)entityHandle;
					switch (metadata.GetMemberReference(memberReferenceHandle).GetKind()) {
						case MemberReferenceKind.Method:
							// for consistency with the MethodDefinition case, never expand varargs
							return ResolveMethodReference(memberReferenceHandle, context, expandVarArgs: false);
						case MemberReferenceKind.Field:
							return ResolveFieldReference(memberReferenceHandle, context);
						default:
							throw new BadImageFormatException("Unknown MemberReferenceKind");
					}
				case HandleKind.MethodDefinition:
					return GetDefinition((MethodDefinitionHandle)entityHandle);
				case HandleKind.MethodSpecification:
					return ResolveMethodSpecification((MethodSpecificationHandle)entityHandle, context, expandVarArgs: false);
				case HandleKind.FieldDefinition:
					return GetDefinition((FieldDefinitionHandle)entityHandle);
				case HandleKind.EventDefinition:
					return GetDefinition((EventDefinitionHandle)entityHandle);
				case HandleKind.PropertyDefinition:
					return GetDefinition((PropertyDefinitionHandle)entityHandle);
				default:
					return null;
			}
		}

		IField ResolveFieldReference(MemberReferenceHandle memberReferenceHandle, GenericContext context)
		{
			var memberRef = metadata.GetMemberReference(memberReferenceHandle);
			var declaringType = ResolveDeclaringType(memberRef.Parent, context);
			var declaringTypeDefinition = declaringType.GetDefinition();
			string name = metadata.GetString(memberRef.Name);
			// field signature is for the definition, not the generic instance
			var signature = memberRef.DecodeFieldSignature(TypeProvider,
				new GenericContext(declaringTypeDefinition?.TypeParameters));
			// 'f' in the predicate is also the definition, even if declaringType is a ParameterizedType
			var field = declaringType.GetFields(f => f.Name == name && CompareTypes(f.ReturnType, signature),
				GetMemberOptions.IgnoreInheritedMembers).FirstOrDefault();
			if (field == null) {
				// If it's a field in a generic type, we need to substitute the type arguments:
				if (declaringType.TypeArguments.Count > 0) {
					signature = signature.AcceptVisitor(declaringType.GetSubstitution());
				}
				field = new FakeField(Compilation) {
					ReturnType = signature,
					Name = name,
					DeclaringType = declaringType,
				};
			}
			return field;
		}
		#endregion

		#region Decode Standalone Signature
		public MethodSignature<IType> DecodeMethodSignature(StandaloneSignatureHandle handle, GenericContext genericContext)
		{
			var standaloneSignature = metadata.GetStandaloneSignature(handle);
			if (standaloneSignature.GetKind() != StandaloneSignatureKind.Method)
				throw new BadImageFormatException("Expected Method signature");
			var sig = standaloneSignature.DecodeMethodSignature(TypeProvider, genericContext);
			return new MethodSignature<IType>(
				sig.Header,
				IntroduceTupleTypes(sig.ReturnType),
				sig.RequiredParameterCount,
				sig.GenericParameterCount,
				ImmutableArray.CreateRange(
					sig.ParameterTypes, IntroduceTupleTypes
				)
			);
		}

		public ImmutableArray<IType> DecodeLocalSignature(StandaloneSignatureHandle handle, GenericContext genericContext)
		{
			var standaloneSignature = metadata.GetStandaloneSignature(handle);
			if (standaloneSignature.GetKind() != StandaloneSignatureKind.LocalVariables)
				throw new BadImageFormatException("Expected LocalVariables signature");
			var types = standaloneSignature.DecodeLocalSignature(TypeProvider, genericContext);
			return ImmutableArray.CreateRange(types, IntroduceTupleTypes);
		}
		#endregion

		#region Module / Assembly attributes
		/// <summary>
		/// Gets the list of all assembly attributes in the project.
		/// </summary>
		public IEnumerable<IAttribute> GetAssemblyAttributes()
		{
			var b = new AttributeListBuilder(this);
			if (metadata.IsAssembly) {
				var assembly = metadata.GetAssemblyDefinition();
				b.Add(metadata.GetCustomAttributes(Handle.AssemblyDefinition), SymbolKind.Module);
				b.AddSecurityAttributes(assembly.GetDeclarativeSecurityAttributes());

				// AssemblyVersionAttribute
				if (assembly.Version != null) {
					b.Add(KnownAttribute.AssemblyVersion, KnownTypeCode.String, assembly.Version.ToString());
				}

				AddTypeForwarderAttributes(ref b);
			}
			return b.Build();
		}

		/// <summary>
		/// Gets the list of all module attributes in the project.
		/// </summary>
		public IEnumerable<IAttribute> GetModuleAttributes()
		{
			var b = new AttributeListBuilder(this);
			b.Add(metadata.GetCustomAttributes(Handle.ModuleDefinition), SymbolKind.Module);
			if (!metadata.IsAssembly) {
				AddTypeForwarderAttributes(ref b);
			}
			return b.Build();
		}

		void AddTypeForwarderAttributes(ref AttributeListBuilder b)
		{
			foreach (ExportedTypeHandle t in metadata.ExportedTypes) {
				var type = metadata.GetExportedType(t);
				if (type.IsForwarder) {
					b.Add(KnownAttribute.TypeForwardedTo, KnownTypeCode.Type, ResolveForwardedType(type));
				}
			}
		}

		IType ResolveForwardedType(ExportedType forwarder)
		{
			IModule module = ResolveModule(forwarder);
			var typeName = forwarder.GetFullTypeName(metadata);
			if (module == null)
				return new UnknownType(typeName);
			using (var busyLock = BusyManager.Enter(this)) {
				if (busyLock.Success) {
					var td = module.GetTypeDefinition(typeName);
					if (td != null) {
						return td;
					}
				}
			}
			return new UnknownType(typeName);

			IModule ResolveModule(ExportedType type)
			{
				switch (type.Implementation.Kind) {
					case HandleKind.AssemblyFile:
						// TODO : Resolve assembly file (module)...
						return this;
					case HandleKind.ExportedType:
						var outerType = metadata.GetExportedType((ExportedTypeHandle)type.Implementation);
						return ResolveModule(outerType);
					case HandleKind.AssemblyReference:
						var asmRef = metadata.GetAssemblyReference((AssemblyReferenceHandle)type.Implementation);
						string shortName = metadata.GetString(asmRef.Name);
						foreach (var asm in Compilation.Modules) {
							if (string.Equals(asm.AssemblyName, shortName, StringComparison.OrdinalIgnoreCase)) {
								return asm;
							}
						}
						return null;
					default:
						throw new BadImageFormatException("Expected implementation to be either an AssemblyFile, ExportedType or AssemblyReference.");
				}
			}
		}
		#endregion

		#region Attribute Helpers
		/// <summary>
		/// Cache for parameterless known attribute types.
		/// </summary>
		readonly IType[] knownAttributeTypes = new IType[KnownAttributes.Count];

		internal IType GetAttributeType(KnownAttribute attr)
		{
			var ty = LazyInit.VolatileRead(ref knownAttributeTypes[(int)attr]);
			if (ty != null)
				return ty;
			ty = Compilation.FindType(attr.GetTypeName());
			return LazyInit.GetOrSet(ref knownAttributeTypes[(int)attr], ty);
		}

		/// <summary>
		/// Cache for parameterless known attributes.
		/// </summary>
		readonly IAttribute[] knownAttributes = new IAttribute[KnownAttributes.Count];

		/// <summary>
		/// Construct a builtin attribute.
		/// </summary>
		internal IAttribute MakeAttribute(KnownAttribute type)
		{
			var attr = LazyInit.VolatileRead(ref knownAttributes[(int)type]);
			if (attr != null)
				return attr;
			attr = new DefaultAttribute(GetAttributeType(type),
				ImmutableArray.Create<CustomAttributeTypedArgument<IType>>(), 
				ImmutableArray.Create<CustomAttributeNamedArgument<IType>>());
			return LazyInit.GetOrSet(ref knownAttributes[(int)type], attr);
		}
		#endregion

		#region Visibility Filter
		internal bool IncludeInternalMembers => (options & TypeSystemOptions.OnlyPublicAPI) == 0;

		internal bool IsVisible(FieldAttributes att)
		{
			att &= FieldAttributes.FieldAccessMask;
			return IncludeInternalMembers
				|| att == FieldAttributes.Public
				|| att == FieldAttributes.Family
				|| att == FieldAttributes.FamORAssem;
		}

		internal bool IsVisible(MethodAttributes att)
		{
			att &= MethodAttributes.MemberAccessMask;
			return IncludeInternalMembers
				|| att == MethodAttributes.Public
				|| att == MethodAttributes.Family
				|| att == MethodAttributes.FamORAssem;
		}
		#endregion
	}
}
