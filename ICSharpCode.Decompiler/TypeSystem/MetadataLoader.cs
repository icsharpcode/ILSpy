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
		public Action<IUnresolvedEntity, EntityHandle> OnEntityLoaded { get; set; }

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
		MetadataReader currentMetadata;
		DefaultUnresolvedAssembly currentAssembly;
		static readonly ITypeResolveContext minimalCorlibContext = new SimpleTypeResolveContext(MinimalCorlib.Instance.CreateCompilation());

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
			this.IncludeInternalMembers = loader.IncludeInternalMembers;
			this.LazyLoad = loader.LazyLoad;
			this.OnEntityLoaded = loader.OnEntityLoaded;
			this.ShortenInterfaceImplNames = loader.ShortenInterfaceImplNames;
			this.currentModule = loader.currentModule;
			this.currentMetadata = loader.currentMetadata;
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
			this.currentMetadata = module.GetMetadataReader();

			// Read assembly and module attributes
			IList<IUnresolvedAttribute> assemblyAttributes = new List<IUnresolvedAttribute>();
			IList<IUnresolvedAttribute> moduleAttributes = new List<IUnresolvedAttribute>();
			if (currentMetadata.IsAssembly) {
				AddAttributes(currentMetadata.GetAssemblyDefinition(), assemblyAttributes);
			}
			AddAttributes(Handle.ModuleDefinition, moduleAttributes);

			assemblyAttributes = interningProvider.InternList(assemblyAttributes);
			moduleAttributes = interningProvider.InternList(moduleAttributes);

			this.currentAssembly = new DefaultUnresolvedAssembly(currentMetadata.IsAssembly ? currentMetadata.GetFullAssemblyName() : currentMetadata.GetString(currentMetadata.GetModuleDefinition().Name));
			currentAssembly.AssemblyAttributes.AddRange(assemblyAttributes);
			currentAssembly.ModuleAttributes.AddRange(assemblyAttributes);

			// Register type forwarders:
			foreach (ExportedTypeHandle t in currentMetadata.ExportedTypes) {
				var type = currentMetadata.GetExportedType(t);
				if (type.IsForwarder) {
					IAssemblyReference assemblyRef;
					switch (type.Implementation.Kind) {
						case HandleKind.AssemblyFile:
							assemblyRef = DefaultAssemblyReference.CurrentAssembly;
							break;
						case HandleKind.ExportedType:
							throw new NotImplementedException();
						case HandleKind.AssemblyReference:
							var asmRef = currentMetadata.GetAssemblyReference((AssemblyReferenceHandle)type.Implementation);
							assemblyRef = new DefaultAssemblyReference(asmRef.GetFullAssemblyName(currentMetadata));
							break;
						default:
							throw new NotSupportedException();
					}
					int typeParameterCount;
					string ns = currentMetadata.GetString(type.Namespace);
					string name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(currentMetadata.GetString(type.Name), out typeParameterCount);
					ns = interningProvider.Intern(ns);
					name = interningProvider.Intern(name);

					var typeRef = new GetClassTypeReference(assemblyRef, ns, name, typeParameterCount);
					typeRef = interningProvider.Intern(typeRef);
					var key = new TopLevelTypeName(ns, name, typeParameterCount);
					currentAssembly.AddTypeForwarder(key, typeRef);
				}
			}

			// Create and register all types:
			MetadataLoader cecilLoaderCloneForLazyLoading = LazyLoad ? new MetadataLoader(this) : null;
			List<TypeDefinitionHandle> cecilTypeDefs = new List<TypeDefinitionHandle>();
			List<DefaultUnresolvedTypeDefinition> typeDefs = new List<DefaultUnresolvedTypeDefinition>();
			foreach (TypeDefinitionHandle h in currentMetadata.TypeDefinitions) {
				this.CancellationToken.ThrowIfCancellationRequested();
				var td = currentMetadata.GetTypeDefinition(h);
				if (!td.GetDeclaringType().IsNil) continue;
				if (this.IncludeInternalMembers || (td.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.Public) {
					string name = currentMetadata.GetString(td.Name);
					if (name.Length == 0)
						continue;

					if (this.LazyLoad) {
						var t = new LazySRMTypeDefinition(cecilLoaderCloneForLazyLoading, module, h);
						currentAssembly.AddTypeDefinition(t);
						RegisterCecilObject(t, h);
					} else {
						var t = CreateTopLevelTypeDefinition(h, td);
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
			this.currentMetadata = null;
			this.currentModule = null;
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
			this.currentMetadata = module.GetMetadataReader();
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
			var td = CreateTopLevelTypeDefinition(typeDefinition, currentMetadata.GetTypeDefinition(typeDefinition));
			InitTypeDefinition(typeDefinition, td);
			return td;
		}
		#endregion

		#region Load Assembly From Disk
		public IUnresolvedAssembly LoadAssemblyFile(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException(nameof(fileName));
			var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			return LoadModule(new Metadata.PEFile(fileName, fileStream, PEStreamOptions.Default));
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
		public ITypeReference ReadTypeReference(EntityHandle type, CustomAttributeHandleCollection? typeAttributes = null)
		{
			ITypeReference CreateTypeReference(TypeReferenceHandle handle)
			{
				var t = currentMetadata.GetTypeReference(handle);
				var asmref = handle.GetDeclaringAssembly(currentMetadata);
				if (asmref.IsNil)
					return new GetClassTypeReference(handle.GetFullTypeName(currentMetadata), DefaultAssemblyReference.CurrentAssembly);
				var asm = currentMetadata.GetAssemblyReference(asmref);
				return new GetClassTypeReference(handle.GetFullTypeName(currentMetadata), new DefaultAssemblyReference(currentMetadata.GetString(asm.Name)));
			}

			switch (type.Kind) {
				case HandleKind.TypeSpecification:
					return DynamicAwareTypeReference.Create(currentMetadata.GetTypeSpecification((TypeSpecificationHandle)type)
						.DecodeSignature(TypeReferenceSignatureDecoder.Instance, default), typeAttributes, currentMetadata);
				case HandleKind.TypeReference:
					return CreateTypeReference((TypeReferenceHandle)type);
				case HandleKind.TypeDefinition:
					return new TypeDefTokenTypeReference(type);
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
			AddCustomAttributes(currentMetadata.GetCustomAttributes((EntityHandle)Handle.AssemblyDefinition), outputList);
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
			AddCustomAttributes(currentMetadata.GetCustomAttributes(module), outputList);
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
				var retParam = currentMetadata.GetParameter(methodDefinition.GetParameters().First());

				if (retParam.GetCustomAttributes().Count > 0)
					return true;
				if ((retParam.Attributes & ParameterAttributes.HasFieldMarshal) == ParameterAttributes.HasFieldMarshal)
					return true;
			}
			return methodDefinition.GetCustomAttributes().Count > 0 || methodDefinition.GetParameters().Any(p => currentMetadata.GetParameter(p).GetCustomAttributes().Any());
		}

		void AddAttributes(MethodDefinition methodDefinition, IList<IUnresolvedAttribute> attributes, IList<IUnresolvedAttribute> returnTypeAttributes)
		{
			MethodImplAttributes implAttributes = methodDefinition.ImplAttributes & ~MethodImplAttributes.CodeTypeMask;

			#region DllImportAttribute
			var info = methodDefinition.GetImport();
			if ((methodDefinition.Attributes & MethodAttributes.PinvokeImpl) == MethodAttributes.PinvokeImpl && !info.Module.IsNil) {
				var dllImport = new DefaultUnresolvedAttribute(dllImportAttributeTypeRef, new[] { KnownTypeReference.String });
				dllImport.PositionalArguments.Add(CreateSimpleConstantValue(KnownTypeReference.String, currentMetadata.GetString(currentMetadata.GetModuleReference(info.Module).Name)));

				if ((info.Attributes & MethodImportAttributes.BestFitMappingDisable) == MethodImportAttributes.BestFitMappingDisable)
					dllImport.AddNamedFieldArgument("BestFitMapping", falseValue);
				if ((info.Attributes & MethodImportAttributes.BestFitMappingEnable) == MethodImportAttributes.BestFitMappingEnable)
					dllImport.AddNamedFieldArgument("BestFitMapping", trueValue);

				CallingConvention callingConvention;
				switch (info.Attributes & MethodImportAttributes.CallingConventionMask) {
					case 0:
						Debug.WriteLine($"P/Invoke calling convention not set on: {methodDefinition.GetDeclaringType().GetFullTypeName(currentMetadata).ToString()}.{currentMetadata.GetString(methodDefinition.Name)}");
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
					dllImport.AddNamedFieldArgument("EntryPoint", CreateSimpleConstantValue(KnownTypeReference.String, currentMetadata.GetString(info.Name)));

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
				var retParam = currentMetadata.GetParameter(methodDefinition.GetParameters().First());
				if (retParam.SequenceNumber == 0) {
					var marshallingDesc = retParam.GetMarshallingDescriptor();

					if (!marshallingDesc.IsNil) {
						returnTypeAttributes.Add(ConvertMarshalInfo(currentMetadata.GetBlobReader(marshallingDesc)));
					}

					AddCustomAttributes(retParam.GetCustomAttributes(), returnTypeAttributes);
				}
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
			LayoutKind defaultLayoutKind = (typeDefinition.IsValueType(currentMetadata) && !typeDefinition.IsEnum(currentMetadata)) ? LayoutKind.Sequential : LayoutKind.Auto;
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
			target.Add(ConvertMarshalInfo(currentMetadata.GetBlobReader(marshalInfo)));
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
					attr.AddNamedFieldArgument("SizeConst", CreateSimpleConstantValue(KnownTypeReference.Int32, size));
					if (marshalInfo.RemainingBytes > 0) {
						type = marshalInfo.ReadByte();
						if (type != 0x66) // None
							attr.AddNamedFieldArgument("ArraySubType", CreateSimpleConstantValue(unmanagedTypeTypeRef, type));
					}
					break;
				case 0x1d: // SafeArray
					if (marshalInfo.RemainingBytes > 0) {
						VarEnum varType = (VarEnum)marshalInfo.ReadByte();
						if (varType != VarEnum.VT_EMPTY)
							attr.AddNamedFieldArgument("SafeArraySubType", CreateSimpleConstantValue(typeof(VarEnum).ToTypeReference(), (int)varType));
					}
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
				var attribute = currentMetadata.GetCustomAttribute(handle);
				var typeHandle = attribute.GetAttributeType(currentMetadata);
				switch (typeHandle.GetFullTypeName(currentMetadata).ReflectionName) {
					case "System.Runtime.CompilerServices.DynamicAttribute":
					case "System.Runtime.CompilerServices.ExtensionAttribute":
					case "System.Runtime.CompilerServices.DecimalConstantAttribute":
					case "System.ParamArrayAttribute":
						continue;
				}
				targetCollection.Add(ReadAttribute(handle));
			}
		}

		public IUnresolvedAttribute ReadAttribute(CustomAttributeHandle handle)
		{
			var attribute = currentMetadata.GetCustomAttribute(handle);

			ITypeReference attributeType = ReadTypeReference(attribute.GetAttributeType(currentMetadata));
			MethodSignature<ITypeReference> signature;
			switch (attribute.Constructor.Kind) {
				case HandleKind.MethodDefinition:
					var md = currentMetadata.GetMethodDefinition((MethodDefinitionHandle)attribute.Constructor);
					signature = md.DecodeSignature(TypeReferenceSignatureDecoder.Instance, default);
					break;
				case HandleKind.MemberReference:
					var mr = currentMetadata.GetMemberReference((MemberReferenceHandle)attribute.Constructor);
					Debug.Assert(mr.GetKind() == MemberReferenceKind.Method);
					signature = mr.DecodeMethodSignature(TypeReferenceSignatureDecoder.Instance, default);
					break;
				default:
					throw new NotSupportedException();
			}
			return interningProvider.Intern(new UnresolvedAttributeBlob(attributeType, signature.ParameterTypes, currentMetadata.GetBlobBytes(attribute.Value)));
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
				AddSecurityAttributes(currentMetadata.GetDeclarativeSecurityAttribute(secDecl), targetCollection);
			}
		}

		void AddSecurityAttributes(DeclarativeSecurityAttribute secDecl, IList<IUnresolvedAttribute> targetCollection)
		{
			var blob = currentMetadata.GetBlobBytes(secDecl.PermissionSet);
			var blobSecDecl = new UnresolvedSecurityDeclarationBlob((int)secDecl.Action, blob);
			targetCollection.AddRange(blobSecDecl.UnresolvedAttributes);
		}
		#endregion
		#endregion

		#region Read Type Definition
		DefaultUnresolvedTypeDefinition CreateTopLevelTypeDefinition(TypeDefinitionHandle handle, TypeDefinition typeDefinition)
		{
			string name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(currentMetadata.GetString(typeDefinition.Name));
			var td = new DefaultUnresolvedTypeDefinition(currentMetadata.GetString(typeDefinition.Namespace), name);
			td.MetadataToken = handle;
			InitTypeParameters(currentMetadata, typeDefinition, td.TypeParameters);
			return td;
		}

		static void InitTypeParameters(MetadataReader currentModule, TypeDefinition typeDefinition, IList<IUnresolvedTypeParameter> typeParameters)
		{
			// Type parameters are initialized within the constructor so that the class can be put into the type storage
			// before the rest of the initialization runs - this allows it to be available for early binding as soon as possible.
			var genericParams = typeDefinition.GetGenericParameters();
			for (int i = 0; i < genericParams.Count; i++) {
				var gp = currentModule.GetGenericParameter(genericParams[i]);
				if (gp.Index != i)
					throw new InvalidOperationException("g.Position != i");
				typeParameters.Add(new DefaultUnresolvedTypeParameter(
					SymbolKind.TypeDefinition, i, currentModule.GetString(gp.Name)));
			}
		}

		void InitTypeParameterConstraints(TypeDefinition typeDefinition, IList<IUnresolvedTypeParameter> typeParameters)
		{
			var genericParams = typeDefinition.GetGenericParameters().ToArray();
			for (int i = 0; i < typeParameters.Count; i++) {
				var tp = (DefaultUnresolvedTypeParameter)typeParameters[i];
				var gp = currentMetadata.GetGenericParameter(genericParams[i]);
				AddConstraints(tp, gp);
				AddAttributes(gp, tp);
				tp.ApplyInterningProvider(interningProvider);
			}
		}

		void InitTypeDefinition(TypeDefinitionHandle handle, DefaultUnresolvedTypeDefinition td)
		{
			var typeDefinition = currentMetadata.GetTypeDefinition(handle);
			td.Kind = GetTypeKind(currentMetadata, typeDefinition);
			InitTypeModifiers(typeDefinition, td);
			InitTypeParameterConstraints(typeDefinition, td.TypeParameters);

			// nested types can be initialized only after generic parameters were created
			InitNestedTypes(handle, td, td.NestedTypes);
			AddAttributes(typeDefinition, td);
			td.HasExtensionMethods = HasExtensionAttribute(currentMetadata, typeDefinition.GetCustomAttributes());

			InitBaseTypes(handle, td.BaseTypes);

			td.AddDefaultConstructorIfRequired = (td.Kind == TypeKind.Struct || td.Kind == TypeKind.Enum);
			InitMembers(typeDefinition, td, td.Members);
			td.ApplyInterningProvider(interningProvider);
			td.Freeze();
			RegisterCecilObject(td, handle);
		}

		void InitBaseTypes(TypeDefinitionHandle handle, IList<ITypeReference> baseTypes)
		{
			var typeDefinition = currentMetadata.GetTypeDefinition(handle);
			// set base classes
			if (typeDefinition.IsEnum(currentMetadata)) {
				foreach (FieldDefinitionHandle h in typeDefinition.GetFields()) {
					var enumField = currentMetadata.GetFieldDefinition(h);
					if (!enumField.HasFlag(FieldAttributes.Static)) {
						baseTypes.Add(enumField.DecodeSignature(TypeReferenceSignatureDecoder.Instance, default));
						break;
					}
				}
			} else {
				if (typeDefinition.BaseType != null) {
					baseTypes.Add(ReadTypeReference(typeDefinition.BaseType));
				}
				foreach (var h in typeDefinition.GetInterfaceImplementations()) {
					var iface = currentMetadata.GetInterfaceImplementation(h);
					baseTypes.Add(ReadTypeReference(iface.Interface, iface.GetCustomAttributes()));
				}
			}
		}

		void InitNestedTypes(TypeDefinitionHandle typeDefinitionHandle, IUnresolvedTypeDefinition declaringTypeDefinition, IList<IUnresolvedTypeDefinition> nestedTypes)
		{
			var typeDefinition = currentMetadata.GetTypeDefinition(typeDefinitionHandle);
			foreach (TypeDefinitionHandle h in typeDefinition.GetNestedTypes()) {
				var nestedTypeDef = currentMetadata.GetTypeDefinition(h);
				TypeAttributes visibility = nestedTypeDef.Attributes & TypeAttributes.VisibilityMask;
				if (this.IncludeInternalMembers
				    || visibility == TypeAttributes.NestedPublic
				    || visibility == TypeAttributes.NestedFamily
					|| visibility == TypeAttributes.NestedFamORAssem) {
					string name = currentMetadata.GetString(nestedTypeDef.Name);
					int pos = name.LastIndexOf('/');
					if (pos > 0)
						name = name.Substring(pos + 1);
					name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(name);
					var nestedType = new DefaultUnresolvedTypeDefinition(declaringTypeDefinition, name);
					nestedType.MetadataToken = h;
					InitTypeParameters(currentMetadata, nestedTypeDef, nestedType.TypeParameters);
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

		static bool IsDelegate(MetadataReader currentModule, TypeDefinition type)
		{
			if (!type.BaseType.IsNil) {
				var baseTypeName = type.BaseType.GetFullTypeName(currentModule).ToString();
				if (baseTypeName == "System.MulticastDelegate")
					return true;
				var thisTypeName = type.GetFullTypeName(currentModule).ToString();
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
					|| attType.ToString() == "System.Runtime.CompilerServices.CompilerGlobalScopeAttribute") {
					return true;
				}
			}
			return false;
		}

		void InitMembers(TypeDefinition typeDefinition, IUnresolvedTypeDefinition td, IList<IUnresolvedMember> members)
		{
			foreach (MethodDefinitionHandle h in typeDefinition.GetMethods()) {
				var method = currentMetadata.GetMethodDefinition(h);
				if (IsVisible(method.Attributes) && !IsAccessor(h.GetMethodSemanticsAttributes(currentMetadata))) {
					SymbolKind type = SymbolKind.Method;
					if ((method.Attributes & MethodAttributes.SpecialName) != 0) {
						if (method.IsConstructor(currentMetadata))
							type = SymbolKind.Constructor;
						else if (currentMetadata.GetString(method.Name).StartsWith("op_", StringComparison.Ordinal))
							type = SymbolKind.Operator;
					}
					members.Add(ReadMethod(h, td, type));
				}
			}
			foreach (FieldDefinitionHandle h in typeDefinition.GetFields()) {
				var field = currentMetadata.GetFieldDefinition(h);
				if (IsVisible(field.Attributes) && (field.Attributes & FieldAttributes.SpecialName) == 0) {
					members.Add(ReadField(h, td));
				}
			}
			string defaultMemberName = GetDefaultMemberName(currentMetadata, typeDefinition);
			foreach (PropertyDefinitionHandle handle in typeDefinition.GetProperties()) {
				var property = currentMetadata.GetPropertyDefinition(handle);
				bool getterVisible = !property.GetAccessors().Getter.IsNil && IsVisible(currentMetadata.GetMethodDefinition(property.GetAccessors().Getter).Attributes);
				bool setterVisible = !property.GetAccessors().Setter.IsNil && IsVisible(currentMetadata.GetMethodDefinition(property.GetAccessors().Setter).Attributes);
				if (getterVisible || setterVisible) {
					var accessor = property.GetAccessors().Getter.IsNil ? property.GetAccessors().Setter : property.GetAccessors().Getter;
					SymbolKind type = SymbolKind.Property;
					if (handle.HasParameters(currentMetadata)) {
						// Try to detect indexer:
						if (currentMetadata.GetString(property.Name) == defaultMemberName) {
							type = SymbolKind.Indexer; // normal indexer
						} else if (currentMetadata.GetString(property.Name).EndsWith(".Item", StringComparison.Ordinal) && accessor.HasOverrides(currentMetadata)) {
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
				var ev = currentMetadata.GetEventDefinition(h);
				if (ev.GetAccessors().Adder.IsNil) continue;
				var addMethod = currentMetadata.GetMethodDefinition(ev.GetAccessors().Adder);
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
			return semantics > (MethodSemanticsAttributes)0 && semantics != MethodSemanticsAttributes.Other;
		}
		#endregion

		#region Lazy-Loaded Type Definition
		sealed class LazySRMTypeDefinition : AbstractUnresolvedEntity, IUnresolvedTypeDefinition
		{
			// loader + cecilTypeDef, used for lazy-loading; and set to null after lazy loading is complete
			readonly MetadataLoader loader;
			readonly Metadata.PEFile module;
			readonly MetadataReader metadata;

			readonly string namespaceName;
			readonly TypeKind kind;
			readonly IList<IUnresolvedTypeParameter> typeParameters;

			// lazy-loaded fields
			IList<ITypeReference> baseTypes;
			IList<IUnresolvedTypeDefinition> nestedTypes;
			IList<IUnresolvedMember> members;

			public LazySRMTypeDefinition(MetadataLoader loader, Metadata.PEFile module, TypeDefinitionHandle typeDefinition)
			{
				this.loader = loader;
				this.module = module;
				this.metadata = module.GetMetadataReader();
				this.MetadataToken = typeDefinition;
				this.SymbolKind = SymbolKind.TypeDefinition;
				var td = metadata.GetTypeDefinition(typeDefinition);
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
				lock (loader.currentMetadata) {
					var result = new List<ITypeReference>();
					loader.InitBaseTypes((TypeDefinitionHandle)MetadataToken, result);
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
				lock (loader.currentMetadata) {
					var result = new List<IUnresolvedTypeDefinition>();
					loader.InitNestedTypes((TypeDefinitionHandle)MetadataToken, this, result);
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
				lock (loader.currentMetadata) {
					if (this.members != null)
						return this.members;
					var result = new List<IUnresolvedMember>();
					var td = metadata.GetTypeDefinition((TypeDefinitionHandle)MetadataToken);
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
			var method = currentMetadata.GetMethodDefinition(handle);
			DefaultUnresolvedMethod m = new DefaultUnresolvedMethod(parentType, currentMetadata.GetString(method.Name));
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
					var gp = currentMetadata.GetGenericParameter(genParams[i]);
					if (gp.Index != i)
						throw new InvalidOperationException("gp.Index != i");
					m.TypeParameters.Add(new DefaultUnresolvedTypeParameter(
						SymbolKind.Method, i, currentMetadata.GetString(gp.Name)));
				}
				for (int i = 0; i < genParams.Count; i++) {
					var gp = currentMetadata.GetGenericParameter(genParams[i]);
					var tp = (DefaultUnresolvedTypeParameter)m.TypeParameters[i];
					AddConstraints(tp, gp);
					AddAttributes(gp, tp);
					tp.ApplyInterningProvider(interningProvider);
				}
			}

			if (HasAnyAttributes(method))
				AddAttributes(method, m.Attributes, m.ReturnTypeAttributes);
			TranslateModifiers(handle, m);

			var declaringType = currentMetadata.GetTypeDefinition(method.GetDeclaringType());
			var reader = currentMetadata.GetBlobReader(method.Signature);
			var signature = method.DecodeSignature(TypeReferenceSignatureDecoder.Instance, default);

			var parameters = method.GetParameters();
			m.ReturnType = HandleReturnType(parameters.FirstOrDefault(), signature.ReturnType);

			int j = 0;
			foreach (var p in parameters) {
				var par = currentMetadata.GetParameter(p);
				if (par.SequenceNumber > 0) {
					m.Parameters.Add(ReadParameter(par, signature.ParameterTypes[j]));
					j++;
				}
			}

			if (signature.Header.CallingConvention == SignatureCallingConvention.VarArgs) {
				m.Parameters.Add(new DefaultUnresolvedParameter(SpecialType.ArgList, string.Empty));
			}

			// mark as extension method if the attribute is set
			if ((method.Attributes & MethodAttributes.Static) == MethodAttributes.Static && HasExtensionAttribute(currentMetadata, method.GetCustomAttributes())) {
				m.IsExtensionMethod = true;
			}
			int lastDot = m.Name.LastIndexOf('.');
			var overrides = handle.GetMethodImplementations(currentMetadata);
			if (lastDot >= 0 && overrides.Any()) {
				// To be consistent with the parser-initialized type system, shorten the method name:
				if (ShortenInterfaceImplNames)
					m.Name = m.Name.Substring(lastDot + 1);
				m.IsExplicitInterfaceImplementation = true;
				foreach (var h in overrides) {
					var or = currentMetadata.GetMethodImplementation(h);
					EntityHandle orDeclaringType;
					string orName;
					int genericParameterCount;
					MethodSignature<ITypeReference> orSignature;
					switch (or.MethodDeclaration.Kind) {
						case HandleKind.MethodDefinition:
							var md = currentMetadata.GetMethodDefinition((MethodDefinitionHandle)or.MethodDeclaration);
							orDeclaringType = md.GetDeclaringType();
							orName = currentMetadata.GetString(md.Name);
							orSignature = md.DecodeSignature(TypeReferenceSignatureDecoder.Instance, default);
							genericParameterCount = orSignature.GenericParameterCount;
							break;
						case HandleKind.MemberReference:
							var mr = currentMetadata.GetMemberReference((MemberReferenceHandle)or.MethodDeclaration);
							Debug.Assert(mr.GetKind() == MemberReferenceKind.Method);
							orDeclaringType = mr.Parent;
							orName = currentMetadata.GetString(mr.Name);
							orSignature = mr.DecodeMethodSignature(TypeReferenceSignatureDecoder.Instance, default);
							genericParameterCount = orSignature.GenericParameterCount;
							break;
						default:
							throw new NotSupportedException();
					}
					m.ExplicitInterfaceImplementations.Add(new DefaultMemberReference(
						accessorOwner != null ? SymbolKind.Accessor : SymbolKind.Method,
						ReadTypeReference(orDeclaringType),
						orName, genericParameterCount, m.Parameters.Select(p => p.Type).ToList()));
				}
			}

			FinishReadMember(m, handle);
			return m;
		}

		ITypeReference HandleReturnType(ParameterHandle parameterHandle, ITypeReference returnType)
		{
			CustomAttributeHandleCollection? attributes = null;
			if (!parameterHandle.IsNil) {
				var par = currentMetadata.GetParameter(parameterHandle);
				if (par.SequenceNumber == 0) {
					attributes = par.GetCustomAttributes();
				}
			}
			return DynamicAwareTypeReference.Create(returnType, attributes, currentMetadata);
		}

		static bool HasExtensionAttribute(MetadataReader currentModule, CustomAttributeHandleCollection attributes)
		{
			foreach (var h in attributes) {
				var attr = currentModule.GetCustomAttribute(h);
				var type = attr.GetAttributeType(currentModule);
				if (type.GetFullTypeName(currentModule).ToString() == "System.Runtime.CompilerServices.ExtensionAttribute")
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
			var method = currentMetadata.GetMethodDefinition(handle);
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
		public IUnresolvedParameter ReadParameter(Parameter parameter, ITypeReference type)
		{
			var p = new DefaultUnresolvedParameter(DynamicAwareTypeReference.Create(type, parameter.GetCustomAttributes(), currentMetadata), interningProvider.Intern(currentMetadata.GetString(parameter.Name)));

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
					var constant = currentMetadata.GetConstant(constantHandle);
					var blobReader = currentMetadata.GetBlobReader(constant.Value);
					p.DefaultValue = CreateSimpleConstantValue(type, blobReader.ReadConstant(constant.TypeCode));
				} else {
					p.DefaultValue = CreateSimpleConstantValue(type, null);
				}
			}

			if (type is ArrayTypeReference) {
				foreach (CustomAttributeHandle h in parameter.GetCustomAttributes()) {
					var att = currentMetadata.GetCustomAttribute(h);
					if (att.GetAttributeType(currentMetadata).GetFullTypeName(currentMetadata).ToString() == typeof(ParamArrayAttribute).FullName) {
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
		
		decimal? TryDecodeDecimalConstantAttribute(CustomAttributeHandle handle)
		{
			var attribute = currentMetadata.GetCustomAttribute(handle);

			ITypeReference attributeType = ReadTypeReference(attribute.GetAttributeType(currentMetadata));
			MethodSignature<ITypeReference> signature;
			switch (attribute.Constructor.Kind) {
				case HandleKind.MethodDefinition:
					var md = currentMetadata.GetMethodDefinition((MethodDefinitionHandle)attribute.Constructor);
					signature = md.DecodeSignature(TypeReferenceSignatureDecoder.Instance, default);
					break;
				case HandleKind.MemberReference:
					var mr = currentMetadata.GetMemberReference((MemberReferenceHandle)attribute.Constructor);
					Debug.Assert(mr.GetKind() == MemberReferenceKind.Method);
					signature = mr.DecodeMethodSignature(TypeReferenceSignatureDecoder.Instance, default);
					break;
				default:
					throw new NotSupportedException();
			}

			if (signature.RequiredParameterCount != 5)
				return null;

			var reader = new Implementation.BlobReader(currentMetadata.GetBlobBytes(attribute.Value), null);
			if (reader.ReadUInt16() != 0x0001) {
				Debug.WriteLine("Unknown blob prolog");
				return null;
			}

			// DecimalConstantAttribute has the arguments (byte scale, byte sign, uint hi, uint mid, uint low) or (byte scale, byte sign, int hi, int mid, int low)
			// Both of these invoke the Decimal constructor (int lo, int mid, int hi, bool isNegative, byte scale) with explicit argument conversions if required.
			var ctorArgs = new object[signature.RequiredParameterCount];
			for (int i = 0; i < ctorArgs.Length; i++) {
				switch (signature.ParameterTypes[i].Resolve(minimalCorlibContext).FullName) {
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

		public IUnresolvedField ReadField(FieldDefinitionHandle handle, IUnresolvedTypeDefinition parentType)
		{
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			if (parentType == null)
				throw new ArgumentNullException(nameof(parentType));

			var field = currentMetadata.GetFieldDefinition(handle);
			DefaultUnresolvedField f = new DefaultUnresolvedField(parentType, currentMetadata.GetString(field.Name));
			f.Accessibility = GetAccessibility(field.Attributes);
			f.IsReadOnly = (field.Attributes & FieldAttributes.InitOnly) == FieldAttributes.InitOnly;
			f.IsStatic = (field.Attributes & FieldAttributes.Static) == FieldAttributes.Static;
			f.ReturnType = DynamicAwareTypeReference.Create(field.DecodeSignature(TypeReferenceSignatureDecoder.Instance, default), field.GetCustomAttributes(), currentMetadata);
			var constantHandle = field.GetDefaultValue();
			if (!constantHandle.IsNil) {
				var constant = currentMetadata.GetConstant(constantHandle);
				var blobReader = currentMetadata.GetBlobReader(constant.Value);
				f.ConstantValue = CreateSimpleConstantValue(f.ReturnType, blobReader.ReadConstant(constant.TypeCode));
			} else {
				var decConstant = field.GetCustomAttributes().FirstOrDefault(a => currentMetadata.GetCustomAttribute(a).GetAttributeType(currentMetadata).GetFullTypeName(currentMetadata).ReflectionName == "System.Runtime.CompilerServices.DecimalConstantAttribute");
				if (!decConstant.IsNil) {
					var constValue = TryDecodeDecimalConstantAttribute(decConstant);
					if (constValue != null)
						f.ConstantValue = CreateSimpleConstantValue(f.ReturnType, constValue);
				}
			}
			AddAttributes(field, f);

			if (f.ReturnType is ModifiedTypeReference mod
				&& mod.IsRequired
				&& mod.ModifierType is GetClassTypeReference modifier
				&& modifier.FullTypeName.ToString() == typeof(IsVolatile).FullName) {
				f.IsVolatile = true;
			}

			FinishReadMember(f, handle);
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
				var constraint = currentMetadata.GetGenericParameterConstraint(h);
				tp.Constraints.Add(ReadTypeReference(constraint.Type));
			}
		}
		#endregion

		#region Read Property

		Accessibility MergePropertyAccessibility(Accessibility left, Accessibility right)
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
			var property = currentMetadata.GetPropertyDefinition(handle);
			var propertyName = currentMetadata.GetString(property.Name);
			DefaultUnresolvedProperty p = new DefaultUnresolvedProperty(parentType, propertyName);
			p.SymbolKind = propertyType;
			var accessors = property.GetAccessors();
			TranslateModifiers(accessors.Getter.IsNil ? accessors.Setter : accessors.Getter, p);
			if (!accessors.Getter.IsNil && !accessors.Setter.IsNil)
				p.Accessibility = MergePropertyAccessibility(GetAccessibility(currentMetadata.GetMethodDefinition(accessors.Getter).Attributes), GetAccessibility(currentMetadata.GetMethodDefinition(accessors.Setter).Attributes));

			var signature = property.DecodeSignature(TypeReferenceSignatureDecoder.Instance, default);
			p.Getter = ReadMethod(accessors.Getter, parentType, SymbolKind.Accessor, p);
			p.Setter = ReadMethod(accessors.Setter, parentType, SymbolKind.Accessor, p);

			ParameterHandleCollection parameterHandles = default(ParameterHandleCollection);
			if (!accessors.Getter.IsNil) {
				var getter = currentMetadata.GetMethodDefinition(accessors.Getter);
				parameterHandles = getter.GetParameters();
			} else {
				if (!accessors.Setter.IsNil) {
					var setter = currentMetadata.GetMethodDefinition(accessors.Setter);
					parameterHandles = setter.GetParameters();
				}
			}

			p.ReturnType = HandleReturnType(parameterHandles.FirstOrDefault(), signature.ReturnType);

			int i = 0;
			foreach (var h in parameterHandles) {
				var par = currentMetadata.GetParameter(h);
				if (par.SequenceNumber > 0 && i < signature.ParameterTypes.Length) {
					p.Parameters.Add(ReadParameter(par, signature.ParameterTypes[i]));
					i++;
				}
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

			FinishReadMember(p, handle);
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

			var ed = currentMetadata.GetEventDefinition(ev);
			var eventName = currentMetadata.GetString(ed.Name);

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

			FinishReadMember(e, ev);

			return e;
		}
		#endregion

		#region FinishReadMember / Interning
		void FinishReadMember(AbstractUnresolvedMember member, EntityHandle cecilDefinition)
		{
			member.MetadataToken = cecilDefinition;
			member.ApplyInterningProvider(interningProvider);
			member.Freeze();
			RegisterCecilObject(member, cecilDefinition);
		}
		#endregion

		#region Type system translation table
		void RegisterCecilObject(IUnresolvedEntity typeSystemObject, EntityHandle cecilObject)
		{
			OnEntityLoaded?.Invoke(typeSystemObject, cecilObject);
		}
		#endregion
	}
}
