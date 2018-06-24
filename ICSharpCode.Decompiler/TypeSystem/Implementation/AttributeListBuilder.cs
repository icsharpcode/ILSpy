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
using System.Reflection.Metadata;
using SRM = System.Reflection.Metadata;
using System.Text;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.Decompiler.Semantics;
using System.Runtime.InteropServices;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	readonly struct AttributeListBuilder
	{
		readonly MetadataAssembly assembly;
		readonly List<IAttribute> attributes;

		public AttributeListBuilder(MetadataAssembly assembly)
		{
			Debug.Assert(assembly != null);
			this.assembly = assembly;
			this.attributes = new List<IAttribute>();
		}

		public AttributeListBuilder(MetadataAssembly assembly, int capacity)
		{
			Debug.Assert(assembly != null);
			this.assembly = assembly;
			this.attributes = new List<IAttribute>(capacity);
		}

		public void Add(IAttribute attr)
		{
			attributes.Add(attr);
		}

		/// <summary>
		/// Add a builtin attribute without any arguments.
		/// </summary>
		public void Add(KnownAttribute type)
		{
			// use the assemblies' cache for simple attributes
			Add(assembly.MakeAttribute(type));
		}

		/// <summary>
		/// Construct a builtin attribute with a single positional argument of known type.
		/// </summary>
		public void Add(KnownAttribute type, KnownTypeCode argType, object argValue)
		{
			Add(type, new ConstantResolveResult(assembly.Compilation.FindType(argType), argValue));
		}

		/// <summary>
		/// Construct a builtin attribute.
		/// </summary>
		public void Add(KnownAttribute type, params ResolveResult[] positionalArguments)
		{
			Add(new DefaultAttribute(assembly.GetAttributeType(type), positionalArguments));
		}

		internal KeyValuePair<IMember, ResolveResult> MakeNamedArg(IType attrType, string name, KnownTypeCode valueType, object value)
		{
			return MakeNamedArg(attrType, name, assembly.Compilation.FindType(valueType), value);
		}

		internal KeyValuePair<IMember, ResolveResult> MakeNamedArg(IType attrType, string name, IType valueType, object value)
		{
			var rr = new ConstantResolveResult(valueType, value);
			return Implementation.CustomAttribute.MakeNamedArg(assembly.Compilation, attrType, name, rr);
		}

		#region MarshalAsAttribute (ConvertMarshalInfo)
		internal void AddMarshalInfo(BlobHandle marshalInfo)
		{
			if (marshalInfo.IsNil) return;
			var metadata = assembly.metadata;
			Add(ConvertMarshalInfo(metadata.GetBlobReader(marshalInfo)));
		}

		const string InteropServices = "System.Runtime.InteropServices";

		IAttribute ConvertMarshalInfo(SRM.BlobReader marshalInfo)
		{
			IType marshalAsAttributeType = assembly.GetAttributeType(KnownAttribute.MarshalAs);
			IType unmanagedTypeType = assembly.Compilation.FindType(new TopLevelTypeName(InteropServices, nameof(UnmanagedType)));

			int type = marshalInfo.ReadByte();
			var positionalArguments = new ResolveResult[] {
				new ConstantResolveResult(unmanagedTypeType, type)
			};
			var namedArguments = new List<KeyValuePair<IMember, ResolveResult>>();

			int size;
			switch (type) {
				case 0x1e: // FixedArray
					if (!marshalInfo.TryReadCompressedInteger(out size))
						size = 0;
					namedArguments.Add(MakeNamedArg(marshalAsAttributeType, "SizeConst", KnownTypeCode.Int32, size));
					if (marshalInfo.RemainingBytes > 0) {
						type = marshalInfo.ReadByte();
						if (type != 0x66) // None
							namedArguments.Add(MakeNamedArg(marshalAsAttributeType, "ArraySubType", unmanagedTypeType, type));
					}
					break;
				case 0x1d: // SafeArray
					if (marshalInfo.RemainingBytes > 0) {
						VarEnum varType = (VarEnum)marshalInfo.ReadByte();
						if (varType != VarEnum.VT_EMPTY) {
							var varEnumType = assembly.Compilation.FindType(new TopLevelTypeName(InteropServices, nameof(VarEnum)));
							namedArguments.Add(MakeNamedArg(marshalAsAttributeType,
								"SafeArraySubType", varEnumType, (int)varType));
						}
					}
					break;
				case 0x2a: // NATIVE_TYPE_ARRAY
					if (marshalInfo.RemainingBytes > 0) {
						type = marshalInfo.ReadByte();
					} else {
						type = 0x66; // Cecil uses NativeType.None as default.
					}
					if (type != 0x50) { // Max
						namedArguments.Add(MakeNamedArg(marshalAsAttributeType,
							"ArraySubType", unmanagedTypeType, type));
					}
					int sizeParameterIndex = marshalInfo.TryReadCompressedInteger(out int value) ? value : -1;
					size = marshalInfo.TryReadCompressedInteger(out value) ? value : -1;
					int sizeParameterMultiplier = marshalInfo.TryReadCompressedInteger(out value) ? value : -1;
					if (size >= 0) {
						namedArguments.Add(MakeNamedArg(marshalAsAttributeType,
							"SizeConst", KnownTypeCode.Int32, size));
					}
					if (sizeParameterMultiplier != 0 && sizeParameterIndex >= 0) {
						namedArguments.Add(MakeNamedArg(marshalAsAttributeType,
							"SizeParamIndex", KnownTypeCode.Int16, (short)sizeParameterIndex));
					}
					break;
				case 0x2c: // CustomMarshaler
					string guidValue = marshalInfo.ReadSerializedString();
					string unmanagedType = marshalInfo.ReadSerializedString();
					string managedType = marshalInfo.ReadSerializedString();
					string cookie = marshalInfo.ReadSerializedString();
					if (managedType != null) {
						namedArguments.Add(MakeNamedArg(marshalAsAttributeType,
							"MarshalType", KnownTypeCode.String, managedType));
					}
					if (!string.IsNullOrEmpty(cookie)) {
						namedArguments.Add(MakeNamedArg(marshalAsAttributeType,
							"MarshalCookie", KnownTypeCode.String, cookie));
					}
					break;
				case 0x17: // FixedSysString
					namedArguments.Add(MakeNamedArg(marshalAsAttributeType,
						"SizeConst", KnownTypeCode.Int32, marshalInfo.ReadCompressedInteger()));
					break;
			}

			return new DefaultAttribute(marshalAsAttributeType, positionalArguments, namedArguments);
		}
		#endregion

		#region Custom Attributes (ReadAttribute)
		public void Add(CustomAttributeHandleCollection attributes)
		{
			var metadata = assembly.metadata;
			foreach (var handle in attributes) {
				var attribute = metadata.GetCustomAttribute(handle);
				var ctor = assembly.ResolveMethod(attribute.Constructor);
				var type = ctor.DeclaringType;
				if (IgnoreAttribute(type)) {
					continue;
				}
				Add(new CustomAttribute(assembly, ctor, handle));
			}
		}

		bool IgnoreAttribute(IType attributeType)
		{
			if (attributeType.DeclaringType != null || attributeType.TypeParameterCount != 0)
				return false;
			switch (attributeType.Namespace) {
				case "System.Runtime.CompilerServices":
					var options = assembly.TypeSystemOptions;
					switch (attributeType.Name) {
						case "DynamicAttribute":
							return (options & TypeSystemOptions.Dynamic) != 0;
						case "TupleElementNamesAttribute":
							return (options & TypeSystemOptions.Tuple) != 0;
						case "ExtensionAttribute":
							return (options & TypeSystemOptions.ExtensionMethods) != 0;
						case "DecimalConstantAttribute":
							return true;
						default:
							return false;
					}
				case "System":
					return attributeType.Name == "ParamArrayAttribute";
				default:
					return false;
			}
		}
		#endregion

		#region Security Attributes
		public void AddSecurityAttributes(DeclarativeSecurityAttributeHandleCollection securityDeclarations)
		{
			var metadata = assembly.metadata;
			foreach (var secDecl in securityDeclarations) {
				if (secDecl.IsNil)
					continue;
				AddSecurityAttributes(metadata.GetDeclarativeSecurityAttribute(secDecl));
			}
		}

		public void AddSecurityAttributes(DeclarativeSecurityAttribute secDecl)
		{
			var securityActionType = assembly.Compilation.FindType(new TopLevelTypeName("System.Security.Permissions", "SecurityAction"));
			var securityAction = new ConstantResolveResult(securityActionType, (int)secDecl.Action);
			var metadata = assembly.metadata;
			var reader = metadata.GetBlobReader(secDecl.PermissionSet);
			if (reader.ReadByte() == '.') {
				// binary attribute
				int attributeCount = reader.ReadCompressedInteger();
				for (int i = 0; i < attributeCount; i++) {
					Add(ReadBinarySecurityAttribute(ref reader, securityAction));
				}
			} else {
				// for backward compatibility with .NET 1.0: XML-encoded attribute
				reader.Reset();
				ReadXmlSecurityAttribute(ref reader, securityAction);
			}
		}

		private void ReadXmlSecurityAttribute(ref SRM.BlobReader reader, ConstantResolveResult securityAction)
		{
			string xml = reader.ReadUTF16(reader.RemainingBytes);
			var permissionSetAttributeType = assembly.GetAttributeType(KnownAttribute.PermissionSet);
			Add(new DefaultAttribute(
				permissionSetAttributeType,
				positionalArguments: new ResolveResult[] { securityAction },
				namedArguments: new[] {
						MakeNamedArg(permissionSetAttributeType, "XML", assembly.Compilation.FindType(KnownTypeCode.String), xml)
				}
			));
		}

		private IAttribute ReadBinarySecurityAttribute(ref SRM.BlobReader reader, ResolveResult securityActionRR)
		{
			string attributeTypeName = reader.ReadSerializedString();
			IType attributeType = assembly.Compilation.FindType(new FullTypeName(attributeTypeName));

			reader.ReadCompressedInteger(); // ??
											// The specification seems to be incorrect here, so I'm using the logic from Cecil instead.
			int numNamed = reader.ReadCompressedInteger();

			var decoder = new Metadata.CustomAttributeDecoder<IType>(assembly.TypeProvider, assembly.metadata);
			var namedArgs = decoder.DecodeNamedArguments(ref reader, numNamed);

			return new DefaultAttribute(
				attributeType,
				positionalArguments: new ResolveResult[] { securityActionRR },
				namedArguments: CustomAttribute.ConvertNamedArguments(assembly.Compilation, attributeType, namedArgs));
		}
		#endregion

		public IAttribute[] Build()
		{
			if (attributes.Count == 0)
				return Empty<IAttribute>.Array;
			else
				return attributes.ToArray();
		}
	}
}
