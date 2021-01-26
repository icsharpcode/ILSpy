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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	sealed class MetadataMethod : IMethod
	{
		readonly MetadataModule module;
		readonly MethodDefinitionHandle handle;

		// eagerly loaded fields:
		readonly MethodAttributes attributes;
		readonly SymbolKind symbolKind;
		readonly ITypeParameter[] typeParameters;
		readonly EntityHandle accessorOwner;
		public MethodSemanticsAttributes AccessorKind { get; }
		public bool IsExtensionMethod { get; }
		bool IMethod.IsLocalFunction => false;

		// lazy-loaded fields:
		ITypeDefinition declaringType;
		string name;
		IParameter[] parameters;
		IType returnType;
		byte returnTypeIsRefReadonly = ThreeState.Unknown;
		byte thisIsRefReadonly = ThreeState.Unknown;
		bool isInitOnly;

		internal MetadataMethod(MetadataModule module, MethodDefinitionHandle handle)
		{
			Debug.Assert(module != null);
			Debug.Assert(!handle.IsNil);
			this.module = module;
			this.handle = handle;
			var metadata = module.metadata;
			var def = metadata.GetMethodDefinition(handle);
			this.attributes = def.Attributes;

			this.symbolKind = SymbolKind.Method;
			var (accessorOwner, semanticsAttribute) = module.PEFile.MethodSemanticsLookup.GetSemantics(handle);
			const MethodAttributes finalizerAttributes = (MethodAttributes.Virtual | MethodAttributes.Family | MethodAttributes.HideBySig);
			if (semanticsAttribute != 0)
			{
				this.symbolKind = SymbolKind.Accessor;
				this.accessorOwner = accessorOwner;
				this.AccessorKind = semanticsAttribute;
			}
			else if ((attributes & (MethodAttributes.SpecialName | MethodAttributes.RTSpecialName)) != 0)
			{
				string name = this.Name;
				if (name == ".cctor" || name == ".ctor")
					this.symbolKind = SymbolKind.Constructor;
				else if (name.StartsWith("op_", StringComparison.Ordinal))
					this.symbolKind = SymbolKind.Operator;
			}
			else if ((attributes & finalizerAttributes) == finalizerAttributes)
			{
				if (Name == "Finalize" && Parameters.Count == 0 && ReturnType.IsKnownType(KnownTypeCode.Void)
					&& (DeclaringTypeDefinition as MetadataTypeDefinition)?.Kind == TypeKind.Class)
				{
					this.symbolKind = SymbolKind.Destructor;
				}
			}
			this.typeParameters = MetadataTypeParameter.Create(module, this, def.GetGenericParameters());
			this.IsExtensionMethod = (attributes & MethodAttributes.Static) == MethodAttributes.Static
				&& (module.TypeSystemOptions & TypeSystemOptions.ExtensionMethods) == TypeSystemOptions.ExtensionMethods
				&& def.GetCustomAttributes().HasKnownAttribute(metadata, KnownAttribute.Extension);
		}

		public EntityHandle MetadataToken => handle;

		public override string ToString()
		{
			return $"{MetadataTokens.GetToken(handle):X8} {DeclaringType?.ReflectionName}.{Name}";
		}

		public string Name {
			get {
				string name = LazyInit.VolatileRead(ref this.name);
				if (name != null)
					return name;
				var metadata = module.metadata;
				var methodDef = metadata.GetMethodDefinition(handle);
				return LazyInit.GetOrSet(ref this.name, metadata.GetString(methodDef.Name));
			}
		}

		public IReadOnlyList<ITypeParameter> TypeParameters => typeParameters;
		IReadOnlyList<IType> IMethod.TypeArguments => typeParameters;

		public SymbolKind SymbolKind => symbolKind;
		public bool IsConstructor => symbolKind == SymbolKind.Constructor;
		public bool IsDestructor => symbolKind == SymbolKind.Destructor;
		public bool IsOperator => symbolKind == SymbolKind.Operator;
		public bool IsAccessor => symbolKind == SymbolKind.Accessor;

		public bool HasBody => module.metadata.GetMethodDefinition(handle).HasBody();


		public IMember AccessorOwner {
			get {
				if (accessorOwner.IsNil)
					return null;
				if (accessorOwner.Kind == HandleKind.PropertyDefinition)
					return module.GetDefinition((PropertyDefinitionHandle)accessorOwner);
				else if (accessorOwner.Kind == HandleKind.EventDefinition)
					return module.GetDefinition((EventDefinitionHandle)accessorOwner);
				else
					return null;
			}
		}

		#region Signature (ReturnType + Parameters)
		public IReadOnlyList<IParameter> Parameters {
			get {
				var parameters = LazyInit.VolatileRead(ref this.parameters);
				if (parameters != null)
					return parameters;
				DecodeSignature();
				return this.parameters;
			}
		}

		public IType ReturnType {
			get {
				var returnType = LazyInit.VolatileRead(ref this.returnType);
				if (returnType != null)
					return returnType;
				DecodeSignature();
				return this.returnType;
			}
		}

		public bool IsInitOnly {
			get {
				var returnType = LazyInit.VolatileRead(ref this.returnType);
				if (returnType == null)
					DecodeSignature();
				return this.isInitOnly;
			}
		}

		internal Nullability NullableContext {
			get {
				var methodDef = module.metadata.GetMethodDefinition(handle);
				return methodDef.GetCustomAttributes().GetNullableContext(module.metadata) ?? DeclaringTypeDefinition.NullableContext;
			}
		}

		private void DecodeSignature()
		{
			var methodDef = module.metadata.GetMethodDefinition(handle);
			var genericContext = new GenericContext(DeclaringType.TypeParameters, this.TypeParameters);
			IType returnType;
			IParameter[] parameters;
			ModifiedType mod;
			try
			{
				var nullableContext = methodDef.GetCustomAttributes().GetNullableContext(module.metadata) ?? DeclaringTypeDefinition.NullableContext;
				var signature = methodDef.DecodeSignature(module.TypeProvider, genericContext);
				(returnType, parameters, mod) = DecodeSignature(module, this, signature,
					methodDef.GetParameters(), nullableContext, module.OptionsForEntity(this));
			}
			catch (BadImageFormatException)
			{
				returnType = SpecialType.UnknownType;
				parameters = Empty<IParameter>.Array;
				mod = null;
			}
			this.isInitOnly = mod is { Modifier: { Name: "IsExternalInit", Namespace: "System.Runtime.CompilerServices" } };
			LazyInit.GetOrSet(ref this.returnType, returnType);
			LazyInit.GetOrSet(ref this.parameters, parameters);
		}

		internal static (IType returnType, IParameter[] parameters, ModifiedType returnTypeModifier) DecodeSignature(
			MetadataModule module, IParameterizedMember owner,
			MethodSignature<IType> signature, ParameterHandleCollection? parameterHandles,
			Nullability nullableContext, TypeSystemOptions typeSystemOptions,
			CustomAttributeHandleCollection? returnTypeAttributes = null)
		{
			var metadata = module.metadata;
			int i = 0;
			IParameter[] parameters = new IParameter[signature.RequiredParameterCount
				+ (signature.Header.CallingConvention == SignatureCallingConvention.VarArgs ? 1 : 0)];
			IType parameterType;
			if (parameterHandles != null)
			{
				foreach (var parameterHandle in parameterHandles)
				{
					var par = metadata.GetParameter(parameterHandle);
					if (par.SequenceNumber == 0)
					{
						// "parameter" holds return type attributes.
						// Note: for properties, the attributes normally stored on a method's return type
						// are instead stored as normal attributes on the property.
						// So MetadataProperty provides a non-null value for returnTypeAttributes,
						// which then should be preferred over the attributes on the accessor's parameters.
						if (returnTypeAttributes == null)
						{
							returnTypeAttributes = par.GetCustomAttributes();
						}
					}
					else if (par.SequenceNumber > 0 && i < signature.RequiredParameterCount)
					{
						// "Successive rows of the Param table that are owned by the same method shall be
						// ordered by increasing Sequence value - although gaps in the sequence are allowed"
						Debug.Assert(i < par.SequenceNumber);
						// Fill gaps in the sequence with non-metadata parameters:
						while (i < par.SequenceNumber - 1)
						{
							parameterType = ApplyAttributeTypeVisitor.ApplyAttributesToType(
								signature.ParameterTypes[i], module.Compilation, null, metadata, typeSystemOptions, nullableContext);
							parameters[i] = new DefaultParameter(parameterType, name: string.Empty, owner,
								referenceKind: parameterType.Kind == TypeKind.ByReference ? ReferenceKind.Ref : ReferenceKind.None);
							i++;
						}
						parameterType = ApplyAttributeTypeVisitor.ApplyAttributesToType(
							signature.ParameterTypes[i], module.Compilation,
							par.GetCustomAttributes(), metadata, typeSystemOptions, nullableContext);
						parameters[i] = new MetadataParameter(module, owner, parameterType, parameterHandle);
						i++;
					}
				}
			}
			while (i < signature.RequiredParameterCount)
			{
				parameterType = ApplyAttributeTypeVisitor.ApplyAttributesToType(
					signature.ParameterTypes[i], module.Compilation, null, metadata, typeSystemOptions, nullableContext);
				parameters[i] = new DefaultParameter(parameterType, name: string.Empty, owner,
					referenceKind: parameterType.Kind == TypeKind.ByReference ? ReferenceKind.Ref : ReferenceKind.None);
				i++;
			}
			if (signature.Header.CallingConvention == SignatureCallingConvention.VarArgs)
			{
				parameters[i] = new DefaultParameter(SpecialType.ArgList, name: string.Empty, owner);
				i++;
			}
			Debug.Assert(i == parameters.Length);
			var returnType = ApplyAttributeTypeVisitor.ApplyAttributesToType(signature.ReturnType,
				module.Compilation, returnTypeAttributes, metadata, typeSystemOptions, nullableContext,
				isSignatureReturnType: true);
			return (returnType, parameters, signature.ReturnType as ModifiedType);
		}
		#endregion

		public bool IsExplicitInterfaceImplementation {
			get {
				if (Name.IndexOf('.') < 0)
					return false;
				var typeDef = ((MetadataTypeDefinition)DeclaringTypeDefinition);
				return typeDef.HasOverrides(handle);
			}
		}

		public IEnumerable<IMember> ExplicitlyImplementedInterfaceMembers {
			get {
				var typeDef = ((MetadataTypeDefinition)DeclaringTypeDefinition);
				return typeDef.GetOverrides(handle);
			}
		}

		IMember IMember.MemberDefinition => this;
		IMethod IMethod.ReducedFrom => null;
		TypeParameterSubstitution IMember.Substitution => TypeParameterSubstitution.Identity;

		public ITypeDefinition DeclaringTypeDefinition {
			get {
				var declType = LazyInit.VolatileRead(ref this.declaringType);
				if (declType != null)
				{
					return declType;
				}
				else
				{
					var def = module.metadata.GetMethodDefinition(handle);
					return LazyInit.GetOrSet(ref this.declaringType,
						module.GetDefinition(def.GetDeclaringType()));
				}
			}
		}

		public IType DeclaringType => DeclaringTypeDefinition;

		public IModule ParentModule => module;
		public ICompilation Compilation => module.Compilation;

		#region Attributes
		IType FindInteropType(string name)
		{
			return module.Compilation.FindType(new TopLevelTypeName(
				"System.Runtime.InteropServices", name, 0
			));
		}

		public IEnumerable<IAttribute> GetAttributes()
		{
			var b = new AttributeListBuilder(module);

			var metadata = module.metadata;
			var def = metadata.GetMethodDefinition(handle);
			MethodImplAttributes implAttributes = def.ImplAttributes & ~MethodImplAttributes.CodeTypeMask;

			#region DllImportAttribute
			var info = def.GetImport();
			if ((attributes & MethodAttributes.PinvokeImpl) == MethodAttributes.PinvokeImpl && !info.Module.IsNil)
			{
				var dllImport = new AttributeBuilder(module, KnownAttribute.DllImport);
				dllImport.AddFixedArg(KnownTypeCode.String,
					metadata.GetString(metadata.GetModuleReference(info.Module).Name));

				var importAttrs = info.Attributes;
				if ((importAttrs & MethodImportAttributes.BestFitMappingDisable) == MethodImportAttributes.BestFitMappingDisable)
					dllImport.AddNamedArg("BestFitMapping", KnownTypeCode.Boolean, false);
				if ((importAttrs & MethodImportAttributes.BestFitMappingEnable) == MethodImportAttributes.BestFitMappingEnable)
					dllImport.AddNamedArg("BestFitMapping", KnownTypeCode.Boolean, true);

				CallingConvention callingConvention;
				switch (info.Attributes & MethodImportAttributes.CallingConventionMask)
				{
					case 0:
						Debug.WriteLine($"P/Invoke calling convention not set on: {this}");
						callingConvention = 0;
						break;
					case MethodImportAttributes.CallingConventionCDecl:
						callingConvention = CallingConvention.Cdecl;
						break;
					case MethodImportAttributes.CallingConventionFastCall:
						callingConvention = CallingConvention.FastCall;
						break;
					case MethodImportAttributes.CallingConventionStdCall:
						callingConvention = CallingConvention.StdCall;
						break;
					case MethodImportAttributes.CallingConventionThisCall:
						callingConvention = CallingConvention.ThisCall;
						break;
					case MethodImportAttributes.CallingConventionWinApi:
						callingConvention = CallingConvention.Winapi;
						break;
					default:
						throw new NotSupportedException("unknown calling convention");
				}
				if (callingConvention != CallingConvention.Winapi)
				{
					var callingConventionType = FindInteropType(nameof(CallingConvention));
					dllImport.AddNamedArg("CallingConvention", callingConventionType, (int)callingConvention);
				}

				CharSet charSet = CharSet.None;
				switch (info.Attributes & MethodImportAttributes.CharSetMask)
				{
					case MethodImportAttributes.CharSetAnsi:
						charSet = CharSet.Ansi;
						break;
					case MethodImportAttributes.CharSetAuto:
						charSet = CharSet.Auto;
						break;
					case MethodImportAttributes.CharSetUnicode:
						charSet = CharSet.Unicode;
						break;
				}
				if (charSet != CharSet.None)
				{
					var charSetType = FindInteropType(nameof(CharSet));
					dllImport.AddNamedArg("CharSet", charSetType, (int)charSet);
				}

				if (!info.Name.IsNil && info.Name != def.Name)
				{
					dllImport.AddNamedArg("EntryPoint", KnownTypeCode.String, metadata.GetString(info.Name));
				}

				if ((info.Attributes & MethodImportAttributes.ExactSpelling) == MethodImportAttributes.ExactSpelling)
				{
					dllImport.AddNamedArg("ExactSpelling", KnownTypeCode.Boolean, true);
				}

				if ((implAttributes & MethodImplAttributes.PreserveSig) == MethodImplAttributes.PreserveSig)
				{
					implAttributes &= ~MethodImplAttributes.PreserveSig;
				}
				else
				{
					dllImport.AddNamedArg("PreserveSig", KnownTypeCode.Boolean, false);
				}

				if ((info.Attributes & MethodImportAttributes.SetLastError) == MethodImportAttributes.SetLastError)
					dllImport.AddNamedArg("SetLastError", KnownTypeCode.Boolean, true);

				if ((info.Attributes & MethodImportAttributes.ThrowOnUnmappableCharDisable) == MethodImportAttributes.ThrowOnUnmappableCharDisable)
					dllImport.AddNamedArg("ThrowOnUnmappableChar", KnownTypeCode.Boolean, false);
				if ((info.Attributes & MethodImportAttributes.ThrowOnUnmappableCharEnable) == MethodImportAttributes.ThrowOnUnmappableCharEnable)
					dllImport.AddNamedArg("ThrowOnUnmappableChar", KnownTypeCode.Boolean, true);

				b.Add(dllImport.Build());
			}
			#endregion

			#region PreserveSigAttribute
			if (implAttributes == MethodImplAttributes.PreserveSig)
			{
				b.Add(KnownAttribute.PreserveSig);
				implAttributes = 0;
			}
			#endregion

			#region MethodImplAttribute
			if (implAttributes != 0)
			{
				b.Add(KnownAttribute.MethodImpl,
					new TopLevelTypeName("System.Runtime.CompilerServices", nameof(MethodImplOptions)),
					(int)implAttributes
				);
			}
			#endregion

			b.Add(def.GetCustomAttributes(), symbolKind);
			b.AddSecurityAttributes(def.GetDeclarativeSecurityAttributes());

			return b.Build();
		}
		#endregion

		#region Return type attributes
		public IEnumerable<IAttribute> GetReturnTypeAttributes()
		{
			var b = new AttributeListBuilder(module);
			var metadata = module.metadata;
			var methodDefinition = metadata.GetMethodDefinition(handle);
			var parameters = methodDefinition.GetParameters();
			if (parameters.Count > 0)
			{
				var retParam = metadata.GetParameter(parameters.First());
				if (retParam.SequenceNumber == 0)
				{
					b.AddMarshalInfo(retParam.GetMarshallingDescriptor());
					b.Add(retParam.GetCustomAttributes(), SymbolKind.ReturnType);
				}
			}
			return b.Build();
		}

		public bool ReturnTypeIsRefReadOnly {
			get {
				if (returnTypeIsRefReadonly != ThreeState.Unknown)
				{
					return returnTypeIsRefReadonly == ThreeState.True;
				}
				var metadata = module.metadata;
				var methodDefinition = metadata.GetMethodDefinition(handle);
				var parameters = methodDefinition.GetParameters();
				bool hasReadOnlyAttr = false;
				if (parameters.Count > 0)
				{
					var retParam = metadata.GetParameter(parameters.First());
					if (retParam.SequenceNumber == 0)
					{
						hasReadOnlyAttr = retParam.GetCustomAttributes().HasKnownAttribute(metadata, KnownAttribute.IsReadOnly);
					}
				}
				this.returnTypeIsRefReadonly = ThreeState.From(hasReadOnlyAttr);
				return hasReadOnlyAttr;
			}
		}

		public bool ThisIsRefReadOnly {
			get {
				if (thisIsRefReadonly != ThreeState.Unknown)
				{
					return thisIsRefReadonly == ThreeState.True;
				}
				var metadata = module.metadata;
				var methodDefinition = metadata.GetMethodDefinition(handle);
				bool hasReadOnlyAttr = DeclaringTypeDefinition?.IsReadOnly ?? false;
				if ((module.TypeSystemOptions & TypeSystemOptions.ReadOnlyMethods) != 0)
				{
					hasReadOnlyAttr |= methodDefinition.GetCustomAttributes().HasKnownAttribute(metadata, KnownAttribute.IsReadOnly);
				}
				this.thisIsRefReadonly = ThreeState.From(hasReadOnlyAttr);
				return hasReadOnlyAttr;
			}
		}

		#endregion

		public Accessibility Accessibility => GetAccessibility(attributes);

		internal static Accessibility GetAccessibility(MethodAttributes attr)
		{
			switch (attr & MethodAttributes.MemberAccessMask)
			{
				case MethodAttributes.Public:
					return Accessibility.Public;
				case MethodAttributes.Assembly:
					return Accessibility.Internal;
				case MethodAttributes.Private:
					return Accessibility.Private;
				case MethodAttributes.Family:
					return Accessibility.Protected;
				case MethodAttributes.FamANDAssem:
					return Accessibility.ProtectedAndInternal;
				case MethodAttributes.FamORAssem:
					return Accessibility.ProtectedOrInternal;
				default:
					return Accessibility.None;
			}
		}

		public bool IsStatic => (attributes & MethodAttributes.Static) != 0;
		public bool IsAbstract => (attributes & MethodAttributes.Abstract) != 0;
		public bool IsSealed => (attributes & (MethodAttributes.Abstract | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Static)) == MethodAttributes.Final;
		public bool IsVirtual => (attributes & (MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final)) == (MethodAttributes.Virtual | MethodAttributes.NewSlot);
		public bool IsOverride => (attributes & (MethodAttributes.NewSlot | MethodAttributes.Virtual)) == MethodAttributes.Virtual;
		public bool IsOverridable
			=> (attributes & (MethodAttributes.Abstract | MethodAttributes.Virtual)) != 0
			&& (attributes & MethodAttributes.Final) == 0;

		public string FullName => $"{DeclaringType?.FullName}.{Name}";
		public string ReflectionName => $"{DeclaringType?.ReflectionName}.{Name}";
		public string Namespace => DeclaringType?.Namespace ?? string.Empty;

		public override bool Equals(object obj)
		{
			if (obj is MetadataMethod m)
			{
				return handle == m.handle && module.PEFile == m.module.PEFile;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 0x5a00d671 ^ module.PEFile.GetHashCode() ^ handle.GetHashCode();
		}

		bool IMember.Equals(IMember obj, TypeVisitor typeNormalization)
		{
			return Equals(obj);
		}

		public IMethod Specialize(TypeParameterSubstitution substitution)
		{
			return SpecializedMethod.Create(this, substitution);
		}

		IMember IMember.Specialize(TypeParameterSubstitution substitution)
		{
			return SpecializedMethod.Create(this, substitution);
		}
	}
}
