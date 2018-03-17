// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

using static ICSharpCode.Decompiler.Metadata.MetadataExtensions;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Allows loading an IProjectContent from an already compiled assembly.
	/// </summary>
	/// <remarks>Instance methods are not thread-safe; you need to create multiple instances of CecilLoader
	/// if you want to load multiple project contents in parallel.</remarks>
	public sealed class MetadataLoader
	{
		#region Options

		/// <summary>
		/// Specifies whether to include internal members. The default is false.
		/// </summary>
		public bool IncludeInternalMembers { get; set; }

		/// <summary>
		/// Gets/Sets the cancellation token used by the assembly loader.
		/// </summary>
		public CancellationToken CancellationToken { get; set; }

		InterningProvider interningProvider = new SimpleInterningProvider();

		/// <summary>
		/// Gets/Sets the interning provider.
		/// </summary>
		public InterningProvider InterningProvider {
			get { return interningProvider; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				interningProvider = value;
			}
		}

		/// <summary>
		/// Specifies whether to use lazy loading. The default is false.
		/// If this property is set to true, the CecilLoader will not copy all the relevant information
		/// out of the Cecil object model, but will maintain references to the Cecil objects.
		/// This speeds up the loading process and avoids loading unnecessary information, but it causes
		/// the Cecil objects to stay in memory (which can significantly increase memory usage).
		/// It also prevents serialization of the Cecil-loaded type system.
		/// </summary>
		/// <remarks>
		/// Because the type system can be used on multiple threads, but Cecil is not
		/// thread-safe for concurrent read access, the CecilLoader will lock on the <see cref="ModuleDefinition"/> instance
		/// for every delay-loading operation.
		/// If you access the Cecil objects directly in your application, you may need to take the same lock.
		/// </remarks>
		public bool LazyLoad { get; set; }

		/// <summary>
		/// This delegate gets executed whenever an entity was loaded.
		/// </summary>
		/// <remarks>
		/// This callback may be to build a dictionary that maps between
		/// entities and cecil objects.
		/// Warning: if delay-loading is used and the type system is accessed by multiple threads,
		/// the callback may be invoked concurrently on multiple threads.
		/// </remarks>
		public Action<IUnresolvedEntity, Metadata.IMetadataEntity> OnEntityLoaded { get; set; }
		
		/// <summary>
		/// Gets a value indicating whether this instance stores references to the cecil objects.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has references to the cecil objects; otherwise, <c>false</c>.
		/// </value>
		public bool HasCecilReferences { get { return typeSystemTranslationTable != null; } }
		
		bool shortenInterfaceImplNames = true;
		
		/// <summary>
		/// Specifies whether method names of explicit interface-implementations should be shortened.
		/// </summary>
		/// <remarks>This is important when working with parser-initialized type-systems in order to be consistent.</remarks>
		public bool ShortenInterfaceImplNames {
			get {
				return shortenInterfaceImplNames;
			}
			set {
				shortenInterfaceImplNames = value;
			}
		}
		#endregion

		Metadata.PEFile currentModule;
		MetadataReader currentModuleMetadata;
		DefaultUnresolvedAssembly currentAssembly;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MetadataLoader"/> class.
		/// </summary>
		public MetadataLoader()
		{
		}
		
		/// <summary>
		/// Creates a nested CecilLoader for lazy-loading.
		/// </summary>
		private MetadataLoader(MetadataLoader loader)
		{
			// use a shared typeSystemTranslationTable
			this.typeSystemTranslationTable = loader.typeSystemTranslationTable;
			this.IncludeInternalMembers = loader.IncludeInternalMembers;
			this.LazyLoad = loader.LazyLoad;
			this.OnEntityLoaded = loader.OnEntityLoaded;
			this.ShortenInterfaceImplNames = loader.ShortenInterfaceImplNames;
			this.currentModule = loader.currentModule;
			this.currentModuleMetadata = loader.currentModuleMetadata;
			this.currentAssembly = loader.currentAssembly;
			// don't use interning - the interning provider is most likely not thread-safe
			this.interningProvider = InterningProvider.Dummy;
			// don't use cancellation for delay-loaded members
		}

		#region Load From AssemblyDefinition
		/// <summary>
		/// Loads a module definition into a project content.
		/// </summary>
		/// <returns>Unresolved type system representing the assembly</returns>
		public IUnresolvedAssembly LoadModule(Metadata.PEFile module)
		{
			this.currentModule = module;
			this.currentModuleMetadata = module.GetMetadataReader();

			// Read assembly and module attributes
			IList<IUnresolvedAttribute> assemblyAttributes = new List<IUnresolvedAttribute>();
			IList<IUnresolvedAttribute> moduleAttributes = new List<IUnresolvedAttribute>();
			AssemblyDefinition assemblyDefinition = default(AssemblyDefinition);
			if (currentModuleMetadata.IsAssembly) {
				AddAttributes(currentModuleMetadata.GetAssemblyDefinition(), assemblyAttributes);
			}
			AddAttributes(Handle.ModuleDefinition, moduleAttributes);

			assemblyAttributes = interningProvider.InternList(assemblyAttributes);
			moduleAttributes = interningProvider.InternList(moduleAttributes);

			this.currentAssembly = new DefaultUnresolvedAssembly(currentModuleMetadata.IsAssembly ? currentModuleMetadata.GetFullAssemblyName() : currentModuleMetadata.GetString(currentModuleMetadata.GetModuleDefinition().Name));
			currentAssembly.AssemblyAttributes.AddRange(assemblyAttributes);
			currentAssembly.ModuleAttributes.AddRange(assemblyAttributes);

			// Register type forwarders:
			foreach (ExportedTypeHandle t in currentModuleMetadata.ExportedTypes) {
				var type = currentModuleMetadata.GetExportedType(t);
				if (type.IsForwarder) {/*
					switch (type.Implementation.Kind) {
						case HandleKind.AssemblyFile:
							throw new NotImplementedException(); // type is defined in another module.
						case HandleKind.ExportedType:

							break;
						case HandleKind.AssemblyReference:
							break;
					}
					int typeParameterCount;
					string ns = currentModuleMetadata.GetString(type.Namespace);
					string name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(currentModuleMetadata.GetString(type.Name), out typeParameterCount);
					ns = interningProvider.Intern(ns);
					name = interningProvider.Intern(name);

					var typeRef = new GetClassTypeReference(, ns, name, typeParameterCount);
					typeRef = interningProvider.Intern(typeRef);
					var key = new TopLevelTypeName(ns, name, typeParameterCount);
					currentAssembly.AddTypeForwarder(key, typeRef);*/
				}
			}

			// Create and register all types:
			MetadataLoader cecilLoaderCloneForLazyLoading = LazyLoad ? new MetadataLoader(this) : null;
			List<TypeDefinitionHandle> cecilTypeDefs = new List<TypeDefinitionHandle>();
			List<DefaultUnresolvedTypeDefinition> typeDefs = new List<DefaultUnresolvedTypeDefinition>();
			foreach (TypeDefinitionHandle h in currentModuleMetadata.TypeDefinitions) {
				this.CancellationToken.ThrowIfCancellationRequested();
				var td = currentModuleMetadata.GetTypeDefinition(h);
				if (this.IncludeInternalMembers || (td.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.Public) {
					string name = currentModuleMetadata.GetString(td.Name);
					if (name.Length == 0)
						continue;

					if (this.LazyLoad) {
						var t = new LazySRMTypeDefinition(cecilLoaderCloneForLazyLoading, new Metadata.TypeDefinition(currentModule, h));
						currentAssembly.AddTypeDefinition(t);
						RegisterCecilObject(t, new Metadata.TypeDefinition(currentModule, h));
					} else {
						var t = CreateTopLevelTypeDefinition(td);
						cecilTypeDefs.Add(h);
						typeDefs.Add(t);
						currentAssembly.AddTypeDefinition(t);
						// The registration will happen after the members are initialized
					}
				}
			}
			// Initialize the type's members:
			for (int i = 0; i < typeDefs.Count; i++) {
				InitTypeDefinition(cecilTypeDefs[i], typeDefs[i]);
			}

			//AddToTypeSystemTranslationTable(this.currentAssembly, assemblyDefinition);
			// Freezing the assembly here is important:
			// otherwise it will be frozen when a compilation is first created
			// from it. But freezing has the effect of changing some collection instances
			// (to ReadOnlyCollection). This hidden mutation was causing a crash
			// when the FastSerializer was saving the assembly at the same time as
			// the first compilation was created from it.
			// By freezing the assembly now, we ensure it is usable on multiple
			// threads without issues.
			currentAssembly.Freeze();

			var result = this.currentAssembly;
			this.currentAssembly = null;
			this.currentModule = null;
			this.currentModuleMetadata = null;
			return result;
		}
		
		/// <summary>
		/// Sets the current module.
		/// This causes ReadTypeReference() to use <see cref="DefaultAssemblyReference.CurrentAssembly"/> for references
		/// in that module.
		/// </summary>
		public void SetCurrentModule(Metadata.PEFile module)
		{
			this.currentModule = module;
			this.currentModuleMetadata = module.GetMetadataReader();
		}
		
		/// <summary>
		/// Loads a type from Cecil.
		/// </summary>
		/// <param name="typeDefinition">The Cecil TypeDefinition.</param>
		/// <returns>ITypeDefinition representing the Cecil type.</returns>
		public IUnresolvedTypeDefinition LoadType(TypeDefinitionHandle typeDefinition)
		{
			if (typeDefinition.IsNil)
				throw new ArgumentNullException(nameof(typeDefinition));
			var td = CreateTopLevelTypeDefinition(currentModuleMetadata.GetTypeDefinition(typeDefinition));
			InitTypeDefinition(typeDefinition, td);
			return td;
		}
		#endregion
		
		#region Load Assembly From Disk
		public IUnresolvedAssembly LoadAssemblyFile(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException(nameof(fileName));
			return LoadModule(Metadata.UniversalAssemblyResolver.LoadMainModule(fileName));
		}
		#endregion
		
		#region Read Type Reference
		/// <summary>
		/// Reads a type reference.
		/// </summary>
		/// <param name="type">The Cecil type reference that should be converted into
		/// a type system type reference.</param>
		/// <param name="typeAttributes">Attributes associated with the Cecil type reference.
		/// This is used to support the 'dynamic' type.</param>
		public ITypeReference ReadTypeReference(EntityHandle type, CustomAttributeHandleCollection typeAttributes = default)
		{
			int typeIndex = 0;
			return CreateType(type, typeAttributes, ref typeIndex);
		}

		class DynamicTypeVisitor : TypeVisitor
		{
			public override IType VisitPointerType(PointerType type)
			{
				return base.VisitPointerType(type);
			}
		}
		
		ITypeReference CreateType(EntityHandle type, CustomAttributeHandleCollection typeAttributes, ref int typeIndex)
		{
			ITypeReference CreateTypeReference(TypeReferenceHandle handle)
			{
				var t = currentModuleMetadata.GetTypeReference(handle);
				var asmref = handle.GetDeclaringAssembly(currentModuleMetadata);
				if (asmref.IsNil)
					return new GetClassTypeReference(handle.GetFullTypeName(currentModuleMetadata), DefaultAssemblyReference.CurrentAssembly);
				var asm = currentModuleMetadata.GetAssemblyReference(asmref);
				return new GetClassTypeReference(handle.GetFullTypeName(currentModuleMetadata), new DefaultAssemblyReference(currentModuleMetadata.GetString(asm.Name)));
			}

			switch (type.Kind) {
				case HandleKind.TypeSpecification:
					return new SignatureTypeReference((TypeSpecificationHandle)type, currentModuleMetadata);
				case HandleKind.TypeReference:
					return CreateTypeReference((TypeReferenceHandle)type);
				case HandleKind.TypeDefinition:
					return new DefaultUnresolvedTypeDefinition(type.GetFullTypeName(currentModuleMetadata).ReflectionName);
				default:
					throw new NotSupportedException();
			}
		}
		#endregion

		#region Read Attributes
		#region Assembly Attributes
		static readonly ITypeReference assemblyVersionAttributeTypeRef = typeof(System.Reflection.AssemblyVersionAttribute).ToTypeReference();
		
		void AddAttributes(AssemblyDefinition assembly, IList<IUnresolvedAttribute> outputList)
		{
			AddCustomAttributes(currentModuleMetadata.GetCustomAttributes((EntityHandle)Handle.AssemblyDefinition), outputList);
			AddSecurityAttributes(assembly.GetDeclarativeSecurityAttributes(), outputList);
			
			// AssemblyVersionAttribute
			if (assembly.Version != null) {
				var assemblyVersion = new DefaultUnresolvedAttribute(assemblyVersionAttributeTypeRef, new[] { KnownTypeReference.String });
				assemblyVersion.PositionalArguments.Add(CreateSimpleConstantValue(KnownTypeReference.String, assembly.Version.ToString()));
				outputList.Add(interningProvider.Intern(assemblyVersion));
			}
		}
		
		IConstantValue CreateSimpleConstantValue(ITypeReference type, object value)
		{
			return interningProvider.Intern(new SimpleConstantValue(type, interningProvider.InternValue(value)));
		}
		#endregion
		
		#region Module Attributes
		void AddAttributes(ModuleDefinitionHandle module, IList<IUnresolvedAttribute> outputList)
		{
			AddCustomAttributes(currentModuleMetadata.GetCustomAttributes(module), outputList);
		}
		#endregion
		
		#region Parameter Attributes
		static readonly IUnresolvedAttribute inAttribute = new DefaultUnresolvedAttribute(typeof(InAttribute).ToTypeReference());
		static readonly IUnresolvedAttribute outAttribute = new DefaultUnresolvedAttribute(typeof(OutAttribute).ToTypeReference());
		
		void AddAttributes(Parameter parameter, DefaultUnresolvedParameter targetParameter)
		{
			if (!targetParameter.IsOut) {
				if ((parameter.Attributes & ParameterAttributes.In) == ParameterAttributes.In)
					targetParameter.Attributes.Add(inAttribute);
				if ((parameter.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out)
					targetParameter.Attributes.Add(outAttribute);
			}
			AddCustomAttributes(parameter.GetCustomAttributes(), targetParameter.Attributes);
			AddMarshalInfo(parameter.GetMarshallingDescriptor(), targetParameter.Attributes);
		}
		#endregion
		
		#region Method Attributes
		static readonly ITypeReference dllImportAttributeTypeRef = typeof(DllImportAttribute).ToTypeReference();
		static readonly SimpleConstantValue trueValue = new SimpleConstantValue(KnownTypeReference.Boolean, true);
		static readonly SimpleConstantValue falseValue = new SimpleConstantValue(KnownTypeReference.Boolean, false);
		static readonly ITypeReference callingConventionTypeRef = typeof(CallingConvention).ToTypeReference();
		static readonly IUnresolvedAttribute preserveSigAttribute = new DefaultUnresolvedAttribute(typeof(PreserveSigAttribute).ToTypeReference());
		static readonly ITypeReference methodImplAttributeTypeRef = typeof(MethodImplAttribute).ToTypeReference();
		static readonly ITypeReference methodImplOptionsTypeRef = typeof(MethodImplOptions).ToTypeReference();
		
		bool HasAnyAttributes(MethodDefinition methodDefinition)
		{
			if ((methodDefinition.Attributes & MethodAttributes.PinvokeImpl) == MethodAttributes.PinvokeImpl)
				return true;
			if ((methodDefinition.ImplAttributes & ~MethodImplAttributes.CodeTypeMask) != 0)
				return true;
			if (methodDefinition.GetParameters().Count > 0) {
				var retParam = currentModuleMetadata.GetParameter(methodDefinition.GetParameters().First());

				if (retParam.GetCustomAttributes().Count > 0)
					return true;
				if ((retParam.Attributes & ParameterAttributes.HasFieldMarshal) == ParameterAttributes.HasFieldMarshal)
					return true;
			}
			return methodDefinition.GetCustomAttributes().Count > 0;
		}
		
		void AddAttributes(MethodDefinition methodDefinition, IList<IUnresolvedAttribute> attributes, IList<IUnresolvedAttribute> returnTypeAttributes)
		{
			MethodImplAttributes implAttributes = methodDefinition.ImplAttributes & ~MethodImplAttributes.CodeTypeMask;

			#region DllImportAttribute
			var info = methodDefinition.GetImport();
			if ((methodDefinition.Attributes & MethodAttributes.PinvokeImpl) == MethodAttributes.PinvokeImpl && !info.Module.IsNil) {
				var dllImport = new DefaultUnresolvedAttribute(dllImportAttributeTypeRef, new[] { KnownTypeReference.String });
				dllImport.PositionalArguments.Add(CreateSimpleConstantValue(KnownTypeReference.String, currentModuleMetadata.GetString(currentModuleMetadata.GetModuleReference(info.Module).Name)));
				
				if ((info.Attributes & MethodImportAttributes.BestFitMappingDisable) == MethodImportAttributes.BestFitMappingDisable)
					dllImport.AddNamedFieldArgument("BestFitMapping", falseValue);
				if ((info.Attributes & MethodImportAttributes.BestFitMappingEnable) == MethodImportAttributes.BestFitMappingEnable)
					dllImport.AddNamedFieldArgument("BestFitMapping", trueValue);
				
				CallingConvention callingConvention;
				switch (info.Attributes & MethodImportAttributes.CallingConventionMask) {
					case 0:
						Debug.WriteLine ($"P/Invoke calling convention not set on: {methodDefinition.GetDeclaringType().GetFullTypeName(currentModuleMetadata).ToString()}.{currentModuleMetadata.GetString(methodDefinition.Name)}");
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
					dllImport.AddNamedFieldArgument("CallingConvention", CreateSimpleConstantValue(callingConventionTypeRef, (int)callingConvention));
				
				CharSet charSet = CharSet.None;
				switch (info.Attributes & MethodImportAttributes.CharSetMask) {
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
					dllImport.AddNamedFieldArgument("CharSet", CreateSimpleConstantValue(charSetTypeRef, (int)charSet));
				
				if (!info.Name.IsNil && info.Name != methodDefinition.Name)
					dllImport.AddNamedFieldArgument("EntryPoint", CreateSimpleConstantValue(KnownTypeReference.String, currentModuleMetadata.GetString(info.Name)));
				
				if ((info.Attributes & MethodImportAttributes.ExactSpelling) == MethodImportAttributes.ExactSpelling)
					dllImport.AddNamedFieldArgument("ExactSpelling", trueValue);
				
				if ((implAttributes & MethodImplAttributes.PreserveSig) == MethodImplAttributes.PreserveSig)
					implAttributes &= ~MethodImplAttributes.PreserveSig;
				else
					dllImport.AddNamedFieldArgument("PreserveSig", falseValue);
				
				if ((info.Attributes & MethodImportAttributes.SetLastError) == MethodImportAttributes.SetLastError)
					dllImport.AddNamedFieldArgument("SetLastError", trueValue);
				
				if ((info.Attributes & MethodImportAttributes.ThrowOnUnmappableCharDisable) == MethodImportAttributes.ThrowOnUnmappableCharDisable)
					dllImport.AddNamedFieldArgument("ThrowOnUnmappableChar", falseValue);
				if ((info.Attributes & MethodImportAttributes.ThrowOnUnmappableCharEnable) == MethodImportAttributes.ThrowOnUnmappableCharEnable)
					dllImport.AddNamedFieldArgument("ThrowOnUnmappableChar", trueValue);
				
				attributes.Add(interningProvider.Intern(dllImport));
			}
			#endregion
			
			#region PreserveSigAttribute
			if (implAttributes == MethodImplAttributes.PreserveSig) {
				attributes.Add(preserveSigAttribute);
				implAttributes = 0;
			}
			#endregion
			
			#region MethodImplAttribute
			if (implAttributes != 0) {
				var methodImpl = new DefaultUnresolvedAttribute(methodImplAttributeTypeRef, new[] { methodImplOptionsTypeRef });
				methodImpl.PositionalArguments.Add(CreateSimpleConstantValue(methodImplOptionsTypeRef, (int)implAttributes));
				attributes.Add(interningProvider.Intern(methodImpl));
			}
			#endregion
			
			AddCustomAttributes(methodDefinition.GetCustomAttributes(), attributes);
			AddSecurityAttributes(methodDefinition.GetDeclarativeSecurityAttributes(), attributes);
			if (methodDefinition.GetParameters().Count > 0) {
				var retParam = currentModuleMetadata.GetParameter(methodDefinition.GetParameters().First());
				var marshallingDesc = retParam.GetMarshallingDescriptor();

				if (!marshallingDesc.IsNil) {
					returnTypeAttributes.Add(ConvertMarshalInfo(currentModuleMetadata.GetBlobReader(marshallingDesc)));
				}

				AddCustomAttributes(retParam.GetCustomAttributes(), returnTypeAttributes);
			}
		}
		#endregion
		
		#region Type Attributes
		static readonly DefaultUnresolvedAttribute serializableAttribute = new DefaultUnresolvedAttribute(typeof(SerializableAttribute).ToTypeReference());
		static readonly DefaultUnresolvedAttribute comImportAttribute = new DefaultUnresolvedAttribute(typeof(ComImportAttribute).ToTypeReference());
		static readonly ITypeReference structLayoutAttributeTypeRef = typeof(StructLayoutAttribute).ToTypeReference();
		static readonly ITypeReference layoutKindTypeRef = typeof(LayoutKind).ToTypeReference();
		static readonly ITypeReference charSetTypeRef = typeof(CharSet).ToTypeReference();
		
		void AddAttributes(TypeDefinition typeDefinition, IUnresolvedTypeDefinition targetEntity)
		{
			// SerializableAttribute
			if ((typeDefinition.Attributes & TypeAttributes.Serializable) != 0)
				targetEntity.Attributes.Add(serializableAttribute);
			
			// ComImportAttribute
			if ((typeDefinition.Attributes & TypeAttributes.Import) != 0)
				targetEntity.Attributes.Add(comImportAttribute);
			
			#region StructLayoutAttribute
			LayoutKind layoutKind = LayoutKind.Auto;
			switch (typeDefinition.Attributes & TypeAttributes.LayoutMask) {
				case TypeAttributes.SequentialLayout:
					layoutKind = LayoutKind.Sequential;
					break;
				case TypeAttributes.ExplicitLayout:
					layoutKind = LayoutKind.Explicit;
					break;
			}
			CharSet charSet = CharSet.None;
			switch (typeDefinition.Attributes & TypeAttributes.StringFormatMask) {
				case TypeAttributes.AnsiClass:
					charSet = CharSet.Ansi;
					break;
				case TypeAttributes.AutoClass:
					charSet = CharSet.Auto;
					break;
				case TypeAttributes.UnicodeClass:
					charSet = CharSet.Unicode;
					break;
			}
			var layout = typeDefinition.GetLayout();
			LayoutKind defaultLayoutKind = (typeDefinition.IsValueType(currentModuleMetadata) && !typeDefinition.IsEnum(currentModuleMetadata)) ? LayoutKind.Sequential : LayoutKind.Auto;
			if (layoutKind != defaultLayoutKind || charSet != CharSet.Ansi || layout.PackingSize > 0 || layout.Size > 0) {
				DefaultUnresolvedAttribute structLayout = new DefaultUnresolvedAttribute(structLayoutAttributeTypeRef, new[] { layoutKindTypeRef });
				structLayout.PositionalArguments.Add(CreateSimpleConstantValue(layoutKindTypeRef, (int)layoutKind));
				if (charSet != CharSet.Ansi) {
					structLayout.AddNamedFieldArgument("CharSet", CreateSimpleConstantValue(charSetTypeRef, (int)charSet));
				}
				if (layout.PackingSize > 0) {
					structLayout.AddNamedFieldArgument("Pack", CreateSimpleConstantValue(KnownTypeReference.Int32, (int)layout.PackingSize));
				}
				if (layout.Size > 0) {
					structLayout.AddNamedFieldArgument("Size", CreateSimpleConstantValue(KnownTypeReference.Int32, (int)layout.Size));
				}
				targetEntity.Attributes.Add(interningProvider.Intern(structLayout));
			}
			#endregion
			
			AddCustomAttributes(typeDefinition.GetCustomAttributes(), targetEntity.Attributes);
			AddSecurityAttributes(typeDefinition.GetDeclarativeSecurityAttributes(), targetEntity.Attributes);
		}
		#endregion
		
		#region Field Attributes
		static readonly ITypeReference fieldOffsetAttributeTypeRef = typeof(FieldOffsetAttribute).ToTypeReference();
		static readonly IUnresolvedAttribute nonSerializedAttribute = new DefaultUnresolvedAttribute(typeof(NonSerializedAttribute).ToTypeReference());
		
		void AddAttributes(FieldDefinition fieldDefinition, IUnresolvedEntity targetEntity)
		{
			// FieldOffsetAttribute
			int offset = fieldDefinition.GetOffset();
			if (offset != -1) {
				DefaultUnresolvedAttribute fieldOffset = new DefaultUnresolvedAttribute(fieldOffsetAttributeTypeRef, new[] { KnownTypeReference.Int32 });
				fieldOffset.PositionalArguments.Add(CreateSimpleConstantValue(KnownTypeReference.Int32, offset));
				targetEntity.Attributes.Add(interningProvider.Intern(fieldOffset));
			}
			
			// NonSerializedAttribute
			if ((fieldDefinition.Attributes & FieldAttributes.NotSerialized) != 0) {
				targetEntity.Attributes.Add(nonSerializedAttribute);
			}
			AddMarshalInfo(fieldDefinition.GetMarshallingDescriptor(), targetEntity.Attributes);			
			AddCustomAttributes(fieldDefinition.GetCustomAttributes(), targetEntity.Attributes);
		}
		#endregion
		
		#region Event Attributes
		void AddAttributes(EventDefinition eventDefinition, IUnresolvedEntity targetEntity)
		{
			AddCustomAttributes(eventDefinition.GetCustomAttributes(), targetEntity.Attributes);
		}
		#endregion
		
		#region Property Attributes
		void AddAttributes(PropertyDefinition propertyDefinition, IUnresolvedEntity targetEntity)
		{
			AddCustomAttributes(propertyDefinition.GetCustomAttributes(), targetEntity.Attributes);
		}
		#endregion
		
		#region Type Parameter Attributes
		void AddAttributes(GenericParameter genericParameter, IUnresolvedTypeParameter targetTP)
		{
			AddCustomAttributes(genericParameter.GetCustomAttributes(), targetTP.Attributes);
		}
		#endregion
		
		#region MarshalAsAttribute (ConvertMarshalInfo)
		static readonly ITypeReference marshalAsAttributeTypeRef = typeof(MarshalAsAttribute).ToTypeReference();
		static readonly ITypeReference unmanagedTypeTypeRef = typeof(UnmanagedType).ToTypeReference();

		void AddMarshalInfo(BlobHandle marshalInfo, IList<IUnresolvedAttribute> target)
		{
			if (marshalInfo.IsNil) return;

		}
		
		IUnresolvedAttribute ConvertMarshalInfo(System.Reflection.Metadata.BlobReader marshalInfo)
		{
			int type = marshalInfo.ReadByte();
			DefaultUnresolvedAttribute attr = new DefaultUnresolvedAttribute(marshalAsAttributeTypeRef, new[] { unmanagedTypeTypeRef });
			attr.PositionalArguments.Add(CreateSimpleConstantValue(unmanagedTypeTypeRef, type));

			int size;
			switch (type) {
				case 0x1e: // FixedArray
					if (!marshalInfo.TryReadCompressedInteger(out size))
						size = 0;
					type = marshalInfo.ReadByte();
					if (type != 0x66) // None
						attr.AddNamedFieldArgument("ArraySubType", CreateSimpleConstantValue(unmanagedTypeTypeRef, type));
					break;
				case 0x1d: // SafeArray
					VarEnum varType = (VarEnum)marshalInfo.ReadByte();
					if (varType != VarEnum.VT_EMPTY)
						attr.AddNamedFieldArgument("SafeArraySubType", CreateSimpleConstantValue(typeof(VarEnum).ToTypeReference(), (int)varType));
					break;
				case 0x2a: // NATIVE_TYPE_ARRAY
					if (marshalInfo.RemainingBytes > 0) {
						type = marshalInfo.ReadByte();
					} else {
						type = 0x66; // Cecil uses NativeType.None as default.
					}
					if (type != 0x50) // Max
						attr.AddNamedFieldArgument("ArraySubType", CreateSimpleConstantValue(unmanagedTypeTypeRef, type));
					int sizeParameterIndex = marshalInfo.TryReadCompressedInteger(out int value) ? value : -1;
					size = marshalInfo.TryReadCompressedInteger(out value) ? value : -1;
					int sizeParameterMultiplier = marshalInfo.TryReadCompressedInteger(out value) ? value : -1;
					if (size >= 0)
						attr.AddNamedFieldArgument("SizeConst", CreateSimpleConstantValue(KnownTypeReference.Int32, size));
					if (sizeParameterMultiplier != 0 && sizeParameterIndex >= 0)
						attr.AddNamedFieldArgument("SizeParamIndex", CreateSimpleConstantValue(KnownTypeReference.Int16, (short)sizeParameterIndex));
					break;
				case 0x2c: // CustomMarshaler
					string guidValue = marshalInfo.ReadSerializedString();
					string unmanagedType = marshalInfo.ReadSerializedString();
					string managedType = marshalInfo.ReadSerializedString();
					string cookie = marshalInfo.ReadSerializedString();
					if (managedType != null)
						attr.AddNamedFieldArgument("MarshalType", CreateSimpleConstantValue(KnownTypeReference.String, managedType));
					if (!string.IsNullOrEmpty(cookie))
						attr.AddNamedFieldArgument("MarshalCookie", CreateSimpleConstantValue(KnownTypeReference.String, cookie));
					break;
				case 0x17: // FixedSysString
					attr.AddNamedFieldArgument("SizeConst", CreateSimpleConstantValue(KnownTypeReference.Int32, marshalInfo.ReadCompressedInteger()));
					break;
			}
			
			return InterningProvider.Intern(attr);
		}
		#endregion
		
		#region Custom Attributes (ReadAttribute)
		void AddCustomAttributes(CustomAttributeHandleCollection attributes, IList<IUnresolvedAttribute> targetCollection)
		{
			foreach (var handle in attributes) {
				var attribute = currentModuleMetadata.GetCustomAttribute(handle);
				var typeHandle = attribute.GetAttributeType(currentModuleMetadata);
				switch (typeHandle.GetFullTypeName(currentModuleMetadata).ReflectionName) {
					case "System.Runtime.CompilerServices.DynamicAttribute":
					case "System.Runtime.CompilerServices.ExtensionAttribute":
					case "System.Runtime.CompilerServices.DecimalConstantAttribute":
					case "System.ParamArrayAttribute":
						continue;
				}
				targetCollection.Add(ReadAttribute(attribute));
			}
		}
		
		public IUnresolvedAttribute ReadAttribute(CustomAttribute attribute)
		{
			ITypeReference attributeType = ReadTypeReference(attribute.GetAttributeType(currentModuleMetadata));
			return interningProvider.Intern(new MetadataUnresolvedAttributeBlob(currentModuleMetadata, attributeType, attribute));
		}
		#endregion
		
		#region Security Attributes
		/// <summary>
		/// Reads a security declaration.
		/// </summary>
		public IList<IUnresolvedAttribute> ReadSecurityDeclaration(DeclarativeSecurityAttributeHandleCollection secDecl)
		{
			var result = new List<IUnresolvedAttribute>();
			AddSecurityAttributes(secDecl, result);
			return result;
		}
		
		void AddSecurityAttributes(DeclarativeSecurityAttributeHandleCollection securityDeclarations, IList<IUnresolvedAttribute> targetCollection)
		{
			foreach (var secDecl in securityDeclarations) {
				if (secDecl.IsNil) continue;
				AddSecurityAttributes(currentModuleMetadata.GetDeclarativeSecurityAttribute(secDecl), targetCollection);
			}
		}
		
		void AddSecurityAttributes(DeclarativeSecurityAttribute secDecl, IList<IUnresolvedAttribute> targetCollection)
		{
			var blob = currentModuleMetadata.GetBlobBytes(secDecl.PermissionSet);
			var blobSecDecl = new UnresolvedSecurityDeclarationBlob((int)secDecl.Action, blob);
			targetCollection.AddRange(blobSecDecl.UnresolvedAttributes);
		}
		#endregion
		#endregion
		
		#region Read Type Definition
		DefaultUnresolvedTypeDefinition CreateTopLevelTypeDefinition(TypeDefinition typeDefinition)
		{
			string name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(currentModuleMetadata.GetString(typeDefinition.Name));
			var td = new DefaultUnresolvedTypeDefinition(currentModuleMetadata.GetString(typeDefinition.Namespace), name);
			InitTypeParameters(currentModuleMetadata, typeDefinition, td.TypeParameters);
			return td;
		}
		
		static void InitTypeParameters(MetadataReader currentModuleMetadata, TypeDefinition typeDefinition, IList<IUnresolvedTypeParameter> typeParameters)
		{
			// Type parameters are initialized within the constructor so that the class can be put into the type storage
			// before the rest of the initialization runs - this allows it to be available for early binding as soon as possible.
			var genericParams = typeDefinition.GetGenericParameters();
			for (int i = 0; i < genericParams.Count; i++) {
				var gp = currentModuleMetadata.GetGenericParameter(genericParams[i]);
				if (gp.Index != i)
					throw new InvalidOperationException("g.Position != i");
				typeParameters.Add(new DefaultUnresolvedTypeParameter(
					SymbolKind.TypeDefinition, i, currentModuleMetadata.GetString(gp.Name)));
			}
		}
		
		void InitTypeParameterConstraints(TypeDefinition typeDefinition, IList<IUnresolvedTypeParameter> typeParameters)
		{
			var genericParams = typeDefinition.GetGenericParameters().ToArray();
			for (int i = 0; i < typeParameters.Count; i++) {
				var tp = (DefaultUnresolvedTypeParameter)typeParameters[i];
				var gp = currentModuleMetadata.GetGenericParameter(genericParams[i]);
				AddConstraints(tp, gp);
				AddAttributes(gp, tp);
				tp.ApplyInterningProvider(interningProvider);
			}
		}
		
		void InitTypeDefinition(TypeDefinitionHandle handle, DefaultUnresolvedTypeDefinition td)
		{
			var typeDefinition = currentModuleMetadata.GetTypeDefinition(handle);
			td.Kind = GetTypeKind(currentModuleMetadata, typeDefinition);
			InitTypeModifiers(typeDefinition, td);
			InitTypeParameterConstraints(typeDefinition, td.TypeParameters);
			
			// nested types can be initialized only after generic parameters were created
			InitNestedTypes(handle, td, td.NestedTypes);
			AddAttributes(typeDefinition, td);
			td.HasExtensionMethods = HasExtensionAttribute(currentModuleMetadata, typeDefinition.GetCustomAttributes());
			
			InitBaseTypes(handle, td.BaseTypes);
			
			td.AddDefaultConstructorIfRequired = (td.Kind == TypeKind.Struct || td.Kind == TypeKind.Enum);
			InitMembers(typeDefinition, td, td.Members);
			td.ApplyInterningProvider(interningProvider);
			td.Freeze();
			RegisterCecilObject(td, new Metadata.TypeDefinition(currentModule, handle));
		}
		
		void InitBaseTypes(TypeDefinitionHandle handle, IList<ITypeReference> baseTypes)
		{
			var typeDefinition = currentModuleMetadata.GetTypeDefinition(handle);
			// set base classes
			if (typeDefinition.IsEnum(currentModuleMetadata)) {
				foreach (FieldDefinitionHandle h in typeDefinition.GetFields()) {
					var enumField = currentModuleMetadata.GetFieldDefinition(h);
					if ((enumField.Attributes & FieldAttributes.Static) == 0) {
						baseTypes.Add(enumField.DecodeSignature(new TypeReferenceSignatureDecoder(), default(Unit)));
						break;
					}
				}
			} else {
				if (typeDefinition.BaseType != null) {
					baseTypes.Add(ReadTypeReference(typeDefinition.BaseType));
				}
				foreach (var h in typeDefinition.GetInterfaceImplementations()) {
					var iface = currentModuleMetadata.GetInterfaceImplementation(h);
					baseTypes.Add(ReadTypeReference(iface.Interface, iface.GetCustomAttributes()));
				}
			}
		}
		
		void InitNestedTypes(TypeDefinitionHandle typeDefinitionHandle, IUnresolvedTypeDefinition declaringTypeDefinition, IList<IUnresolvedTypeDefinition> nestedTypes)
		{
			var typeDefinition = currentModuleMetadata.GetTypeDefinition(typeDefinitionHandle);
			foreach (TypeDefinitionHandle h in typeDefinition.GetNestedTypes()) {
				var nestedTypeDef = currentModuleMetadata.GetTypeDefinition(h);
				TypeAttributes visibility = nestedTypeDef.Attributes & TypeAttributes.VisibilityMask;
				if (this.IncludeInternalMembers
				    || visibility == TypeAttributes.NestedPublic
				    || visibility == TypeAttributes.NestedFamily
				    || visibility == TypeAttributes.NestedFamORAssem)
				{
					string name = currentModuleMetadata.GetString(nestedTypeDef.Name);
					int pos = name.LastIndexOf('/');
					if (pos > 0)
						name = name.Substring(pos + 1);
					name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(name);
					var nestedType = new DefaultUnresolvedTypeDefinition(declaringTypeDefinition, name);
					InitTypeParameters(currentModuleMetadata, nestedTypeDef, nestedType.TypeParameters);
					nestedTypes.Add(nestedType);
					InitTypeDefinition(h, nestedType);
				}
			}
		}
		
		static TypeKind GetTypeKind(MetadataReader module, TypeDefinition typeDefinition)
		{
			// set classtype
			if ((typeDefinition.Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface) {
				return TypeKind.Interface;
			} else if (typeDefinition.IsEnum(module)) {
				return TypeKind.Enum;
			} else if (typeDefinition.IsValueType(module)) {
				return TypeKind.Struct;
			} else if (IsDelegate(module, typeDefinition)) {
				return TypeKind.Delegate;
			} else if (IsModule(module, typeDefinition)) {
				return TypeKind.Module;
			} else {
				return TypeKind.Class;
			}
		}
		
		static void InitTypeModifiers(TypeDefinition typeDefinition, AbstractUnresolvedEntity td)
		{
			td.IsSealed = (typeDefinition.Attributes & TypeAttributes.Sealed) == TypeAttributes.Sealed;
			td.IsAbstract = (typeDefinition.Attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract;
			switch (typeDefinition.Attributes & TypeAttributes.VisibilityMask) {
				case TypeAttributes.NotPublic:
				case TypeAttributes.NestedAssembly:
					td.Accessibility = Accessibility.Internal;
					break;
				case TypeAttributes.Public:
				case TypeAttributes.NestedPublic:
					td.Accessibility = Accessibility.Public;
					break;
				case TypeAttributes.NestedPrivate:
					td.Accessibility = Accessibility.Private;
					break;
				case TypeAttributes.NestedFamily:
					td.Accessibility = Accessibility.Protected;
					break;
				case TypeAttributes.NestedFamANDAssem:
					td.Accessibility = Accessibility.ProtectedAndInternal;
					break;
				case TypeAttributes.NestedFamORAssem:
					td.Accessibility = Accessibility.ProtectedOrInternal;
					break;
			}
		}
		
		static bool IsDelegate(MetadataReader currentModuleMetadata, TypeDefinition type)
		{
			if (type.BaseType != null) {
				var baseTypeName = type.BaseType.GetFullTypeName(currentModuleMetadata).ToString();
				if (baseTypeName == "System.MulticastDelegate")
					return true;
				var thisTypeName = type.GetFullTypeName(currentModuleMetadata).ToString();
				if (baseTypeName == "Delegate" && thisTypeName != "System.MulticastDelegate")
					return true;
			}
			return false;
		}
		
		static bool IsModule(MetadataReader reader, TypeDefinition type)
		{
			foreach (var h in type.GetCustomAttributes()) {
				var attType = reader.GetCustomAttribute(h).GetAttributeType(reader).GetFullTypeName(reader);
				if (attType.ToString() == "Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute"
				    || attType.ToString() == "System.Runtime.CompilerServices.CompilerGlobalScopeAttribute")
				{
					return true;
				}
			}
			return false;
		}
		
		void InitMembers(TypeDefinition typeDefinition, IUnresolvedTypeDefinition td, IList<IUnresolvedMember> members)
		{
			foreach (MethodDefinitionHandle h in typeDefinition.GetMethods()) {
				var method = currentModuleMetadata.GetMethodDefinition(h);
				/*if (IsVisible(method.Attributes) && !IsAccessor(h.GetMethodSemanticsAttributes(currentModuleMetadata))) {
					SymbolKind type = SymbolKind.Method;
					if ((method.Attributes & MethodAttributes.SpecialName) != 0) {
						if (method.IsConstructor(currentModuleMetadata))
							type = SymbolKind.Constructor;
						else if (currentModuleMetadata.GetString(method.Name).StartsWith("op_", StringComparison.Ordinal))
							type = SymbolKind.Operator;
					}
					members.Add(ReadMethod(h, td, type));
				}*/
			}
			foreach (FieldDefinitionHandle h in typeDefinition.GetFields()) {
				var field = currentModuleMetadata.GetFieldDefinition(h);
				if (IsVisible(field.Attributes) && (field.Attributes & FieldAttributes.SpecialName) != 0) {
					members.Add(ReadField(h, td));
				}
			}
			string defaultMemberName = GetDefaultMemberName(currentModuleMetadata, typeDefinition);
			foreach (PropertyDefinitionHandle handle in typeDefinition.GetProperties()) {
				var property = currentModuleMetadata.GetPropertyDefinition(handle);
				bool getterVisible = !property.GetAccessors().Getter.IsNil && IsVisible(currentModuleMetadata.GetMethodDefinition(property.GetAccessors().Getter).Attributes);
				bool setterVisible = !property.GetAccessors().Setter.IsNil && IsVisible(currentModuleMetadata.GetMethodDefinition(property.GetAccessors().Setter).Attributes);
				if (getterVisible || setterVisible) {
					var accessor = property.GetAccessors().Getter.IsNil ? property.GetAccessors().Setter : property.GetAccessors().Getter;
					SymbolKind type = SymbolKind.Property;
					if (handle.HasParameters(currentModuleMetadata)) {
						// Try to detect indexer:
						if (currentModuleMetadata.GetString(property.Name) == defaultMemberName) {
							type = SymbolKind.Indexer; // normal indexer
						} else if (currentModuleMetadata.GetString(property.Name).EndsWith(".Item", StringComparison.Ordinal) && accessor.HasOverrides(currentModuleMetadata)) {
							// explicit interface implementation of indexer
							type = SymbolKind.Indexer;
							// We can't really tell parameterized properties and indexers apart in this case without
							// resolving the interface, so we rely on the "Item" naming convention instead.
						}
					}
					members.Add(ReadProperty(handle, td, type));
				}
			}
			foreach (EventDefinitionHandle h in typeDefinition.GetEvents()) {
				var ev = currentModuleMetadata.GetEventDefinition(h);
				if (ev.GetAccessors().Adder.IsNil) continue;
				var addMethod = currentModuleMetadata.GetMethodDefinition(ev.GetAccessors().Adder);
				if (IsVisible(addMethod.Attributes)) {
					members.Add(ReadEvent(h, td));
				}
			}
		}

		static string GetDefaultMemberName(MetadataReader reader, TypeDefinition typeDefinition)
		{
			foreach (var h in typeDefinition.GetCustomAttributes()) {
				var a = reader.GetCustomAttribute(h);
				var type = a.GetAttributeType(reader).GetFullTypeName(reader);
				if (type.ToString() != typeof(System.Reflection.DefaultMemberAttribute).FullName)
					continue;
				var value = a.DecodeValue(new TypeSystemAttributeTypeProvider(minimalCorlibContext));
				if (value.FixedArguments.Length == 1 && value.FixedArguments[0].Value is string name)
					return name;
			}
			return null;
		}

		static bool IsAccessor(MethodSemanticsAttributes semantics)
		{
			return semantics != MethodSemanticsAttributes.Other;
		}
		#endregion
		
		#region Lazy-Loaded Type Definition
		sealed class LazySRMTypeDefinition : AbstractUnresolvedEntity, IUnresolvedTypeDefinition
		{
			// loader + cecilTypeDef, used for lazy-loading; and set to null after lazy loading is complete
			readonly MetadataLoader loader;
			readonly Metadata.TypeDefinition typeDefinition;
			
			readonly string namespaceName;
			readonly TypeKind kind;
			readonly IList<IUnresolvedTypeParameter> typeParameters;
			
			// lazy-loaded fields
			IList<ITypeReference> baseTypes;
			IList<IUnresolvedTypeDefinition> nestedTypes;
			IList<IUnresolvedMember> members;
			
			public LazySRMTypeDefinition(MetadataLoader loader, Metadata.TypeDefinition typeDefinition)
			{
				this.loader = loader;
				this.typeDefinition = typeDefinition;
				this.SymbolKind = SymbolKind.TypeDefinition;
				var metadata = typeDefinition.Module.GetMetadataReader();
				var td = typeDefinition.This();
				this.namespaceName = metadata.GetString(td.Namespace);
				this.Name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(metadata.GetString(td.Name));
				var tps = new List<IUnresolvedTypeParameter>();
				InitTypeParameters(metadata, td, tps);
				this.typeParameters = FreezableHelper.FreezeList(tps);
				
				this.kind = GetTypeKind(metadata, td);
				InitTypeModifiers(td, this);
				loader.InitTypeParameterConstraints(td, typeParameters);
				
				loader.AddAttributes(td, this);
				flags[FlagHasExtensionMethods] = HasExtensionAttribute(metadata, td.GetCustomAttributes());
				
				this.ApplyInterningProvider(loader.interningProvider);
				this.Freeze();
			}
			
			public override string Namespace {
				get { return namespaceName; }
				set { throw new NotSupportedException(); }
			}
			
			public override string ReflectionName {
				get { return this.FullTypeName.ReflectionName; }
			}
			
			public FullTypeName FullTypeName {
				get {
					return new TopLevelTypeName(namespaceName, this.Name, typeParameters.Count);
				}
			}
			
			public TypeKind Kind {
				get { return kind; }
			}
			
			public IList<IUnresolvedTypeParameter> TypeParameters {
				get { return typeParameters; }
			}

			public Metadata.IMetadataEntity MetadataToken => typeDefinition;
			
			public IList<ITypeReference> BaseTypes {
				get {
					var result = LazyInit.VolatileRead(ref this.baseTypes);
					if (result != null) {
						return result;
					} else {
						return LazyInit.GetOrSet(ref this.baseTypes, TryInitBaseTypes());
					}
				}
			}
			
			IList<ITypeReference> TryInitBaseTypes()
			{
				lock (loader.currentModuleMetadata) {
					var result = new List<ITypeReference>();
					loader.InitBaseTypes(typeDefinition.Handle, result);
					return FreezableHelper.FreezeList(result);
				}
			}
			
			public IList<IUnresolvedTypeDefinition> NestedTypes {
				get {
					var result = LazyInit.VolatileRead(ref this.nestedTypes);
					if (result != null) {
						return result;
					} else {
						return LazyInit.GetOrSet(ref this.nestedTypes, TryInitNestedTypes());
					}
				}
			}
			
			IList<IUnresolvedTypeDefinition> TryInitNestedTypes()
			{
				lock (loader.currentModuleMetadata) {
					var result = new List<IUnresolvedTypeDefinition>();
					loader.InitNestedTypes(typeDefinition.Handle, this, result);
					return FreezableHelper.FreezeList(result);
				}
			}
			
			public IList<IUnresolvedMember> Members {
				get {
					var result = LazyInit.VolatileRead(ref this.members);
					if (result != null) {
						return result;
					} else {
						return LazyInit.GetOrSet(ref this.members, TryInitMembers());
					}
				}
			}
			
			IList<IUnresolvedMember> TryInitMembers()
			{
				lock (loader.currentModuleMetadata) {
					if (this.members != null)
						return this.members;
					var result = new List<IUnresolvedMember>();
					var td = typeDefinition.This();
					loader.InitMembers(td, this, result);
					return FreezableHelper.FreezeList(result);
				}
			}

			public IEnumerable<IUnresolvedMethod> Methods {
				get { return Members.OfType<IUnresolvedMethod>(); }
			}
			
			public IEnumerable<IUnresolvedProperty> Properties {
				get { return Members.OfType<IUnresolvedProperty>(); }
			}
			
			public IEnumerable<IUnresolvedField> Fields {
				get { return Members.OfType<IUnresolvedField>(); }
			}
			
			public IEnumerable<IUnresolvedEvent> Events {
				get { return Members.OfType<IUnresolvedEvent>(); }
			}
			
			public bool AddDefaultConstructorIfRequired {
				get { return kind == TypeKind.Struct || kind == TypeKind.Enum; }
			}
			
			public bool? HasExtensionMethods {
				get { return flags[FlagHasExtensionMethods]; }
				// we always return true or false, never null.
				// FlagHasNoExtensionMethods is unused in LazyCecilTypeDefinition
			}
			
			public bool IsPartial {
				get { return false; }
			}
			
			public override object Clone()
			{
				throw new NotSupportedException();
			}
			
			public IType Resolve(ITypeResolveContext context)
			{
				if (context == null)
					throw new ArgumentNullException("context");
				if (context.CurrentAssembly == null)
					throw new ArgumentException("An ITypeDefinition cannot be resolved in a context without a current assembly.");
				return context.CurrentAssembly.GetTypeDefinition(this.FullTypeName)
					?? (IType)new UnknownType(this.Namespace, this.Name, this.TypeParameters.Count);
			}
			
			public ITypeResolveContext CreateResolveContext(ITypeResolveContext parentContext)
			{
				return parentContext;
			}
		}
		#endregion
		
		#region Read Method
		public IUnresolvedMethod ReadMethod(MethodDefinitionHandle method, IUnresolvedTypeDefinition parentType, SymbolKind methodType = SymbolKind.Method)
		{
			return ReadMethod(method, parentType, methodType, null);
		}
		
		IUnresolvedMethod ReadMethod(MethodDefinitionHandle handle, IUnresolvedTypeDefinition parentType, SymbolKind methodType, IUnresolvedMember accessorOwner)
		{
			if (handle.IsNil)
				return null;
			var method = currentModuleMetadata.GetMethodDefinition(handle);
			DefaultUnresolvedMethod m = new DefaultUnresolvedMethod(parentType, currentModuleMetadata.GetString(method.Name));
			m.SymbolKind = methodType;
			m.AccessorOwner = accessorOwner;
			m.HasBody = (method.Attributes & MethodAttributes.Abstract) == 0 &&
				(method.Attributes & MethodAttributes.PinvokeImpl) == 0 &&
				(method.ImplAttributes & MethodImplAttributes.InternalCall) == 0 &&
				(method.ImplAttributes & MethodImplAttributes.Native) == 0 &&
				(method.ImplAttributes & MethodImplAttributes.Unmanaged) == 0 &&
				(method.ImplAttributes & MethodImplAttributes.Runtime) == 0;
			var genParams = method.GetGenericParameters();
			if (genParams.Count > 0) {
				for (int i = 0; i < genParams.Count; i++) {
					var gp = currentModuleMetadata.GetGenericParameter(genParams[i]);
					if (gp.Index != i)
						throw new InvalidOperationException("gp.Index != i");
					m.TypeParameters.Add(new DefaultUnresolvedTypeParameter(
						SymbolKind.Method, i, currentModuleMetadata.GetString(gp.Name)));
				}
				for (int i = 0; i < genParams.Count; i++) {
					var gp = currentModuleMetadata.GetGenericParameter(genParams[i]);
					var tp = (DefaultUnresolvedTypeParameter)m.TypeParameters[i];
					AddConstraints(tp, gp);
					AddAttributes(gp, tp);
					tp.ApplyInterningProvider(interningProvider);
				}
			}

			var declaringType = currentModuleMetadata.GetTypeDefinition(method.GetDeclaringType());
			var reader = currentModuleMetadata.GetBlobReader(method.Signature);
			var signature = method.DecodeSignature(new TypeReferenceSignatureDecoder(), default(Unit));

			m.ReturnType = signature.ReturnType;
			
			if (HasAnyAttributes(method))
				AddAttributes(method, m.Attributes, m.ReturnTypeAttributes);
			TranslateModifiers(handle, m);

			int j = 0;
			foreach (var par in method.GetParameters()) {
				m.Parameters.Add(ReadParameter(par, signature.ParameterTypes[j]));
				j++;
			}
			
			if (signature.Header.CallingConvention == SignatureCallingConvention.VarArgs) {
				m.Parameters.Add(new DefaultUnresolvedParameter(SpecialType.ArgList, string.Empty));
			}
			
			// mark as extension method if the attribute is set
			if ((method.Attributes & MethodAttributes.Static) == MethodAttributes.Static && HasExtensionAttribute(currentModuleMetadata, method.GetCustomAttributes())) {
				m.IsExtensionMethod = true;
			}
			/* TODO overrides
			int lastDot = m.Name.LastIndexOf('.');
			if (lastDot >= 0 && method.HasOverrides) {
				// To be consistent with the parser-initialized type system, shorten the method name:
				if (ShortenInterfaceImplNames)
					m.Name = method.Name.Substring(lastDot + 1);
				m.IsExplicitInterfaceImplementation = true;
				foreach (var or in method.Overrides) {
					m.ExplicitInterfaceImplementations.Add(new DefaultMemberReference(
						accessorOwner != null ? SymbolKind.Accessor : SymbolKind.Method,
						ReadTypeReference(or.DeclaringType),
						or.Name, or.GenericParameters.Count, m.Parameters.Select(p => p.Type).ToList()));
				}
			}*/

			FinishReadMember(m, new Metadata.MethodDefinition(currentModule, handle));
			return m;
		}
		
		static bool HasExtensionAttribute(MetadataReader currentModuleMetadata, CustomAttributeHandleCollection attributes)
		{
			foreach (var h in attributes) {
				var attr = currentModuleMetadata.GetCustomAttribute(h);
				var type = attr.GetAttributeType(currentModuleMetadata);
				if (type.GetFullTypeName(currentModuleMetadata).ToString() == "System.Runtime.CompilerServices.ExtensionAttribute")
					return true;
			}
			return false;
		}
		
		bool IsVisible(MethodAttributes att)
		{
			att &= MethodAttributes.MemberAccessMask;
			return IncludeInternalMembers
				|| att == MethodAttributes.Public
				|| att == MethodAttributes.Family
				|| att == MethodAttributes.FamORAssem;
		}
		
		static Accessibility GetAccessibility(MethodAttributes attr)
		{
			switch (attr & MethodAttributes.MemberAccessMask) {
				case MethodAttributes.Public:
					return Accessibility.Public;
				case MethodAttributes.FamANDAssem:
					return Accessibility.ProtectedAndInternal;
				case MethodAttributes.Assembly:
					return Accessibility.Internal;
				case MethodAttributes.Family:
					return Accessibility.Protected;
				case MethodAttributes.FamORAssem:
					return Accessibility.ProtectedOrInternal;
				default:
					return Accessibility.Private;
			}
		}
		
		void TranslateModifiers(MethodDefinitionHandle handle, AbstractUnresolvedMember m)
		{
			var method = currentModuleMetadata.GetMethodDefinition(handle);
			if (m.DeclaringTypeDefinition.Kind == TypeKind.Interface) {
				// interface members don't have modifiers, but we want to handle them as "public abstract"
				m.Accessibility = Accessibility.Public;
				m.IsAbstract = true;
			} else {
				m.Accessibility = GetAccessibility(method.Attributes);
				if ((method.Attributes & MethodAttributes.Abstract) == MethodAttributes.Abstract) {
					m.IsAbstract = true;
					m.IsOverride = (method.Attributes & MethodAttributes.NewSlot) != MethodAttributes.NewSlot;
				} else if ((method.Attributes & MethodAttributes.Final) == MethodAttributes.Final) {
					if ((method.Attributes & MethodAttributes.NewSlot) != MethodAttributes.NewSlot) {
						m.IsSealed = true;
						m.IsOverride = true;
					}
				} else if ((method.Attributes & MethodAttributes.Virtual) == MethodAttributes.Virtual) {
					if ((method.Attributes & MethodAttributes.NewSlot) == MethodAttributes.NewSlot)
						m.IsVirtual = true;
					else
						m.IsOverride = true;
				}
				m.IsStatic = (method.Attributes & MethodAttributes.Static) == MethodAttributes.Static;
			}
		}
		#endregion
		
		#region Read Parameter
		public IUnresolvedParameter ReadParameter(ParameterHandle handle, ITypeReference type)
		{
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			var parameter = currentModuleMetadata.GetParameter(handle);
			var p = new DefaultUnresolvedParameter(type, interningProvider.Intern(currentModuleMetadata.GetString(parameter.Name)));
			
			if (type is ByReferenceTypeReference) {
				if ((parameter.Attributes & ParameterAttributes.In) == 0 && (parameter.Attributes & ParameterAttributes.Out) != 0)
					p.IsOut = true;
				else
					p.IsRef = true;
			}
			AddAttributes(parameter, p);
			
			if ((parameter.Attributes & ParameterAttributes.Optional) != 0) {
				var constantHandle = parameter.GetDefaultValue();
				if (!constantHandle.IsNil) {
					var constant = currentModuleMetadata.GetConstant(constantHandle);
					var blobReader = currentModuleMetadata.GetBlobReader(constant.Value);
					p.DefaultValue = CreateSimpleConstantValue(type, blobReader.ReadConstant(constant.TypeCode));
				}
			}
			
			if (type is ArrayTypeReference) {
				foreach (CustomAttributeHandle h in parameter.GetCustomAttributes()) {
					var att = currentModuleMetadata.GetCustomAttribute(h);
					if (att.GetAttributeType(currentModuleMetadata).GetFullTypeName(currentModuleMetadata).ToString() == typeof(ParamArrayAttribute).FullName) {
						p.IsParams = true;
						break;
					}
				}
			}
			
			return interningProvider.Intern(p);
		}
		#endregion
		
		#region Read Field
		bool IsVisible(FieldAttributes att)
		{
			att &= FieldAttributes.FieldAccessMask;
			return IncludeInternalMembers
				|| att == FieldAttributes.Public
				|| att == FieldAttributes.Family
				|| att == FieldAttributes.FamORAssem;
		}
		/*
		decimal? TryDecodeDecimalConstantAttribute(CustomAttribute attribute)
		{
			if (attribute.ConstructorArguments.Count != 5)
				return null;

			BlobReader reader = new BlobReader(attribute.GetBlob(), null);
			if (reader.ReadUInt16() != 0x0001) {
				Debug.WriteLine("Unknown blob prolog");
				return null;
			}

			// DecimalConstantAttribute has the arguments (byte scale, byte sign, uint hi, uint mid, uint low) or (byte scale, byte sign, int hi, int mid, int low)
			// Both of these invoke the Decimal constructor (int lo, int mid, int hi, bool isNegative, byte scale) with explicit argument conversions if required.
			var ctorArgs = new object[attribute.ConstructorArguments.Count];
			for (int i = 0; i < ctorArgs.Length; i++) {
				switch (attribute.ConstructorArguments[i].Type.FullName) {
					case "System.Byte":
						ctorArgs[i] = reader.ReadByte();
						break;
					case "System.Int32":
						ctorArgs[i] = reader.ReadInt32();
						break;
					case "System.UInt32":
						ctorArgs[i] = unchecked((int)reader.ReadUInt32());
						break;
					default:
						return null;
				}
			}

			if (!ctorArgs.Select(a => a.GetType()).SequenceEqual(new[] { typeof(byte), typeof(byte), typeof(int), typeof(int), typeof(int) }))
				return null;

			return new decimal((int)ctorArgs[4], (int)ctorArgs[3], (int)ctorArgs[2], (byte)ctorArgs[1] != 0, (byte)ctorArgs[0]);
		}
		*/
		
		public IUnresolvedField ReadField(FieldDefinitionHandle handle, IUnresolvedTypeDefinition parentType)
		{
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			if (parentType == null)
				throw new ArgumentNullException(nameof(parentType));

			var field = currentModuleMetadata.GetFieldDefinition(handle);
			DefaultUnresolvedField f = new DefaultUnresolvedField(parentType, currentModuleMetadata.GetString(field.Name));
			f.Accessibility = GetAccessibility(field.Attributes);
			f.IsReadOnly = (field.Attributes & FieldAttributes.InitOnly) == FieldAttributes.InitOnly;
			f.IsStatic = (field.Attributes & FieldAttributes.Static) == FieldAttributes.Static;
			f.ReturnType = ResolveDynamicTypes(field.DecodeSignature(new TypeReferenceSignatureDecoder(), default(Unit)), field.GetCustomAttributes());
			var constantHandle = field.GetDefaultValue();
			if (!constantHandle.IsNil) {
				var constant = currentModuleMetadata.GetConstant(constantHandle);
				var blobReader = currentModuleMetadata.GetBlobReader(constant.Value);
				f.ConstantValue = CreateSimpleConstantValue(f.ReturnType, blobReader.ReadConstant(constant.TypeCode));
			} else {
				// TODO decimal constants
				/*var decConstant = field.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.DecimalConstantAttribute");
				if (decConstant != null) {
					var constValue = TryDecodeDecimalConstantAttribute(decConstant);
					if (constValue != null)
						f.ConstantValue = CreateSimpleConstantValue(f.ReturnType, constValue);
				}*/
			}
			AddAttributes(field, f);

			if (f.ReturnType is ModifiedTypeReference mod
				&& mod.IsRequired
				&& mod.ModifierType is GetClassTypeReference modifier
				&& modifier.FullTypeName.ToString() == typeof(IsVolatile).FullName) {
				f.IsVolatile = true;
			}

			FinishReadMember(f, new Metadata.FieldDefinition(currentModule, handle));
			return f;
		}
		
		static Accessibility GetAccessibility(FieldAttributes attr)
		{
			switch (attr & FieldAttributes.FieldAccessMask) {
				case FieldAttributes.Public:
					return Accessibility.Public;
				case FieldAttributes.FamANDAssem:
					return Accessibility.ProtectedAndInternal;
				case FieldAttributes.Assembly:
					return Accessibility.Internal;
				case FieldAttributes.Family:
					return Accessibility.Protected;
				case FieldAttributes.FamORAssem:
					return Accessibility.ProtectedOrInternal;
				default:
					return Accessibility.Private;
			}
		}
		#endregion

		#region Resolve dynamic type
		ITypeReference ResolveDynamicTypes(ITypeReference type, CustomAttributeHandleCollection attributes)
		{
			int typeIndex = 0;
			return ResolveDynamicTypes(type, attributes, ref typeIndex);
		}

		ITypeReference ResolveDynamicTypes(ITypeReference type, CustomAttributeHandleCollection attributes, ref int typeIndex)
		{
			// TODO : interning??
			ITypeReference replacement;
			switch (type) {
				case ByReferenceTypeReference brtr:
					typeIndex++;
					replacement = ResolveDynamicTypes(brtr.ElementType, attributes, ref typeIndex);
					if (replacement == brtr.ElementType)
						return brtr;
					else
						return new ByReferenceTypeReference(replacement);
				case PointerTypeReference ptr:
					typeIndex++;
					replacement = ResolveDynamicTypes(ptr.ElementType, attributes, ref typeIndex);
					if (replacement == ptr.ElementType)
						return ptr;
					else
						return new PointerTypeReference(replacement);
				case TypeParameterReference tpr:
					return tpr;
				case ParameterizedTypeReference genericType:
					ITypeReference baseType = ResolveDynamicTypes(genericType.GenericType, attributes, ref typeIndex);
					ITypeReference[] para = new ITypeReference[genericType.TypeArguments.Count];
					for (int i = 0; i < para.Length; ++i) {
						typeIndex++;
						para[i] = ResolveDynamicTypes(genericType.TypeArguments[i], attributes, ref typeIndex);
					}
					return new ParameterizedTypeReference(baseType, para);
				case NestedTypeReference ntr:
					replacement = ResolveDynamicTypes(ntr.DeclaringTypeReference, attributes, ref typeIndex);
					if (replacement == ntr.DeclaringTypeReference)
						return ntr;
					else
						return new NestedTypeReference(replacement, ntr.Name, ntr.AdditionalTypeParameterCount);
				case GetClassTypeReference gctr:
					return gctr;
				case KnownTypeReference ktr:
					if (ktr.KnownTypeCode == KnownTypeCode.Object && HasDynamicAttribute(attributes, typeIndex))
						return SpecialType.Dynamic.ToTypeReference();
					else
						return ktr;
				default:
					return type;
			}
		}

		static readonly ITypeResolveContext minimalCorlibContext = new SimpleTypeResolveContext(MinimalCorlib.Instance.CreateCompilation());

		bool HasDynamicAttribute(CustomAttributeHandleCollection attributes, int typeIndex)
		{
			foreach (CustomAttributeHandle handle in attributes) {
				var a = currentModuleMetadata.GetCustomAttribute(handle);
				var type = a.GetAttributeType(currentModuleMetadata);
				if (type.GetFullTypeName(currentModuleMetadata).ToString() == "System.Runtime.CompilerServices.DynamicAttribute") {
					var ctor = a.DecodeValue(new TypeSystemAttributeTypeProvider(minimalCorlibContext));
					if (ctor.FixedArguments.Length == 1) {
						if (ctor.FixedArguments[0].Value is bool[] values && typeIndex < values.Length)
							return values[typeIndex];
					}
					return true;
				}
			}
			return false;
		}
		#endregion

		#region Type Parameter Constraints
		void AddConstraints(DefaultUnresolvedTypeParameter tp, GenericParameter g)
		{
			switch (g.Attributes & GenericParameterAttributes.VarianceMask) {
				case GenericParameterAttributes.Contravariant:
					tp.Variance = VarianceModifier.Contravariant;
					break;
				case GenericParameterAttributes.Covariant:
					tp.Variance = VarianceModifier.Covariant;
					break;
			}
			
			tp.HasReferenceTypeConstraint = (g.Attributes & GenericParameterAttributes.ReferenceTypeConstraint) != 0;
			tp.HasValueTypeConstraint = (g.Attributes & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0;
			tp.HasDefaultConstructorConstraint = (g.Attributes & GenericParameterAttributes.DefaultConstructorConstraint) != 0;
			
			foreach (GenericParameterConstraintHandle h in g.GetConstraints()) {
				var constraint = currentModuleMetadata.GetGenericParameterConstraint(h);
				tp.Constraints.Add(ReadTypeReference(constraint.Type));
			}
		}
		#endregion
		
		#region Read Property

		Accessibility MergePropertyAccessibility (Accessibility left, Accessibility right)
		{
			if (left == Accessibility.Public || right == Accessibility.Public)
				return Accessibility.Public;

			if (left == Accessibility.ProtectedOrInternal || right == Accessibility.ProtectedOrInternal)
				return Accessibility.ProtectedOrInternal;

			if (left == Accessibility.Protected && right == Accessibility.Internal ||
			    left == Accessibility.Internal && right == Accessibility.Protected)
				return Accessibility.ProtectedOrInternal;

			if (left == Accessibility.Protected || right == Accessibility.Protected)
				return Accessibility.Protected;

			if (left == Accessibility.Internal || right == Accessibility.Internal)
				return Accessibility.Internal;

			if (left == Accessibility.ProtectedAndInternal || right == Accessibility.ProtectedAndInternal)
				return Accessibility.ProtectedAndInternal;

			return left;
		}

		public IUnresolvedProperty ReadProperty(PropertyDefinitionHandle handle, IUnresolvedTypeDefinition parentType, SymbolKind propertyType = SymbolKind.Property)
		{
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			if (parentType == null)
				throw new ArgumentNullException(nameof(parentType));
			var property = currentModuleMetadata.GetPropertyDefinition(handle);
			var propertyName = currentModuleMetadata.GetString(property.Name);
			DefaultUnresolvedProperty p = new DefaultUnresolvedProperty(parentType, propertyName);
			p.SymbolKind = propertyType;
			var accessors = property.GetAccessors();
			TranslateModifiers(accessors.Getter.IsNil ? accessors.Setter : accessors.Getter, p);
			if (!accessors.Getter.IsNil && !accessors.Setter.IsNil)
				p.Accessibility = MergePropertyAccessibility(GetAccessibility(currentModuleMetadata.GetMethodDefinition(accessors.Getter).Attributes), GetAccessibility (currentModuleMetadata.GetMethodDefinition(accessors.Setter).Attributes));

			var signature = property.DecodeSignature(new TypeReferenceSignatureDecoder(), default(Unit));
			p.ReturnType = signature.ReturnType;
			
			p.Getter = ReadMethod(accessors.Getter, parentType, SymbolKind.Accessor, p);
			p.Setter = ReadMethod(accessors.Setter, parentType, SymbolKind.Accessor, p);

			ParameterHandleCollection parameterHandles = default(ParameterHandleCollection);
			int parameterCount = 0;
			if (!accessors.Getter.IsNil) {
				var getter = currentModuleMetadata.GetMethodDefinition(accessors.Getter);
				parameterHandles = getter.GetParameters();
				parameterCount = parameterHandles.Count;
			} else {
				if (!accessors.Setter.IsNil) {
					var setter = currentModuleMetadata.GetMethodDefinition(accessors.Setter);
					parameterHandles = setter.GetParameters();
					parameterCount = parameterHandles.Count - 1;
				}
			}

			int i = 0;
			foreach (var par in parameterHandles) {
				if (i >= parameterCount) break;
				p.Parameters.Add(ReadParameter(par, signature.ParameterTypes[i]));
				i++;
			}
			AddAttributes(property, p);

			var accessor = p.Getter ?? p.Setter;
			if (accessor != null && accessor.IsExplicitInterfaceImplementation) {
				if (ShortenInterfaceImplNames)
					p.Name = propertyName.Substring(propertyName.LastIndexOf('.') + 1);
				p.IsExplicitInterfaceImplementation = true;
				foreach (var mr in accessor.ExplicitInterfaceImplementations) {
					p.ExplicitInterfaceImplementations.Add(new AccessorOwnerMemberReference(mr));
				}
			}

			FinishReadMember(p, new Metadata.PropertyDefinition(currentModule, handle));
			return p;
		}
		#endregion
		
		#region Read Event
		public IUnresolvedEvent ReadEvent(EventDefinitionHandle ev, IUnresolvedTypeDefinition parentType)
		{
			if (ev.IsNil)
				throw new ArgumentNullException(nameof(ev));
			if (parentType == null)
				throw new ArgumentNullException(nameof(parentType));

			var ed = currentModuleMetadata.GetEventDefinition(ev);
			var eventName = currentModuleMetadata.GetString(ed.Name);

			DefaultUnresolvedEvent e = new DefaultUnresolvedEvent(parentType, eventName);
			var accessors = ed.GetAccessors();
			TranslateModifiers(accessors.Adder, e);
			e.ReturnType = ReadTypeReference(ed.Type, typeAttributes: ed.GetCustomAttributes());
			
			e.AddAccessor    = ReadMethod(accessors.Adder,   parentType, SymbolKind.Accessor, e);
			e.RemoveAccessor = ReadMethod(accessors.Remover, parentType, SymbolKind.Accessor, e);
			e.InvokeAccessor = ReadMethod(accessors.Raiser,  parentType, SymbolKind.Accessor, e);
			
			AddAttributes(ed, e);
			
			var accessor = e.AddAccessor ?? e.RemoveAccessor ?? e.InvokeAccessor;
			if (accessor != null && accessor.IsExplicitInterfaceImplementation) {
				if (ShortenInterfaceImplNames)
					e.Name = eventName.Substring(eventName.LastIndexOf('.') + 1);
				e.IsExplicitInterfaceImplementation = true;
				foreach (var mr in accessor.ExplicitInterfaceImplementations) {
					e.ExplicitInterfaceImplementations.Add(new AccessorOwnerMemberReference(mr));
				}
			}

			FinishReadMember(e, new Metadata.EventDefinition(currentModule, ev));
			
			return e;
		}
		#endregion
		
		#region FinishReadMember / Interning
		void FinishReadMember(AbstractUnresolvedMember member, Metadata.IMetadataEntity cecilDefinition)
		{
			member.ApplyInterningProvider(interningProvider);
			member.Freeze();
			RegisterCecilObject(member, cecilDefinition);
		}
		#endregion
		
		#region Type system translation table
		readonly Dictionary<object, Metadata.IMetadataEntity> typeSystemTranslationTable;
		
		void RegisterCecilObject(IUnresolvedEntity typeSystemObject, Metadata.IMetadataEntity cecilObject)
		{
			OnEntityLoaded?.Invoke(typeSystemObject, cecilObject);
			
			AddToTypeSystemTranslationTable(typeSystemObject, cecilObject);
		}
		
		void AddToTypeSystemTranslationTable(object typeSystemObject, Metadata.IMetadataEntity cecilObject)
		{
			if (typeSystemTranslationTable != null) {
				// When lazy-loading, the dictionary might be shared between multiple cecil-loaders that are used concurrently
				lock (typeSystemTranslationTable) {
					typeSystemTranslationTable[typeSystemObject] = cecilObject;
				}
			}
		}
		
		T InternalGetCecilObject<T> (object typeSystemObject) where T : Metadata.IMetadataEntity
		{
			if (typeSystemObject == null)
				throw new ArgumentNullException ("typeSystemObject");
			if (!HasCecilReferences)
				throw new NotSupportedException ("This instance contains no cecil references.");
			Metadata.IMetadataEntity result;
			lock (typeSystemTranslationTable) {
				if (!typeSystemTranslationTable.TryGetValue (typeSystemObject, out result))
					return default;
			}
			return (T)result;
		}
		
		public AssemblyDefinition GetCecilObject (IUnresolvedAssembly content)
		{
			throw new NotImplementedException();
			//return InternalGetCecilObject<AssemblyDefinition> (content);
		}


		public TypeDefinition GetCecilObject (IUnresolvedTypeDefinition type)
		{
			if (type == null)
				throw new ArgumentNullException ("type");
			throw new NotImplementedException();
			//return InternalGetCecilObject<TypeDefinition> (type);
		}


		public MethodDefinition GetCecilObject (IUnresolvedMethod method)
		{
			throw new NotImplementedException();
			//return InternalGetCecilObject<MethodDefinition> (method);
		}


		public FieldDefinition GetCecilObject (IUnresolvedField field)
		{
			throw new NotImplementedException();
			//return InternalGetCecilObject<FieldDefinition> (field);
		}


		public EventDefinition GetCecilObject (IUnresolvedEvent evt)
		{
			throw new NotImplementedException();
			//return InternalGetCecilObject<EventDefinition> (evt);
		}


		public PropertyDefinition GetCecilObject (IUnresolvedProperty property)
		{
			throw new NotImplementedException();
			//return InternalGetCecilObject<PropertyDefinition> (property);
		}
		#endregion
	}
}
