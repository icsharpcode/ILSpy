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
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Util;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	readonly struct AttributeListBuilder
	{
		readonly MetadataModule module;
		readonly List<IAttribute> attributes;

		public AttributeListBuilder(MetadataModule module)
		{
			Debug.Assert(module != null);
			this.module = module;
			this.attributes = new List<IAttribute>();
		}

		public AttributeListBuilder(MetadataModule module, int capacity)
		{
			Debug.Assert(module != null);
			this.module = module;
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
			Add(module.MakeAttribute(type));
		}

		/// <summary>
		/// Construct a builtin attribute with a single positional argument of known type.
		/// </summary>
		public void Add(KnownAttribute type, KnownTypeCode argType, object argValue)
		{
			Add(type, ImmutableArray.Create(new CustomAttributeTypedArgument<IType>(module.Compilation.FindType(argType), argValue)));
		}

		/// <summary>
		/// Construct a builtin attribute with a single positional argument of known type.
		/// </summary>
		public void Add(KnownAttribute type, TopLevelTypeName argType, object argValue)
		{
			Add(type, ImmutableArray.Create(new CustomAttributeTypedArgument<IType>(module.Compilation.FindType(argType), argValue)));
		}

		/// <summary>
		/// Construct a builtin attribute.
		/// </summary>
		public void Add(KnownAttribute type, ImmutableArray<CustomAttributeTypedArgument<IType>> fixedArguments)
		{
			Add(new DefaultAttribute(module.GetAttributeType(type), fixedArguments,
				ImmutableArray.Create<CustomAttributeNamedArgument<IType>>()));
		}

		#region MarshalAsAttribute (ConvertMarshalInfo)
		internal void AddMarshalInfo(BlobHandle marshalInfo)
		{
			if (marshalInfo.IsNil)
				return;
			var metadata = module.metadata;
			Add(ConvertMarshalInfo(metadata.GetBlobReader(marshalInfo)));
		}

		const string InteropServices = "System.Runtime.InteropServices";

		IAttribute ConvertMarshalInfo(SRM.BlobReader marshalInfo)
		{
			var b = new AttributeBuilder(module, KnownAttribute.MarshalAs);
			IType unmanagedTypeType = module.Compilation.FindType(new TopLevelTypeName(InteropServices, nameof(UnmanagedType)));

			int type = marshalInfo.ReadByte();
			b.AddFixedArg(unmanagedTypeType, type);

			int size;
			switch (type)
			{
				case 0x1e: // FixedArray
					if (!marshalInfo.TryReadCompressedInteger(out size))
						size = 0;
					b.AddNamedArg("SizeConst", KnownTypeCode.Int32, size);
					if (marshalInfo.RemainingBytes > 0)
					{
						type = marshalInfo.ReadByte();
						if (type != 0x66) // None
							b.AddNamedArg("ArraySubType", unmanagedTypeType, type);
					}
					break;
				case 0x1d: // SafeArray
					if (marshalInfo.RemainingBytes > 0)
					{
						VarEnum varType = (VarEnum)marshalInfo.ReadByte();
						if (varType != VarEnum.VT_EMPTY)
						{
							var varEnumType = new TopLevelTypeName(InteropServices, nameof(VarEnum));
							b.AddNamedArg("SafeArraySubType", varEnumType, (int)varType);
						}
					}
					break;
				case 0x2a: // NATIVE_TYPE_ARRAY
					if (marshalInfo.RemainingBytes > 0)
					{
						type = marshalInfo.ReadByte();
					}
					else
					{
						type = 0x66; // Cecil uses NativeType.None as default.
					}
					if (type != 0x50)
					{ // Max
						b.AddNamedArg("ArraySubType", unmanagedTypeType, type);
					}
					int sizeParameterIndex = marshalInfo.TryReadCompressedInteger(out int value) ? value : -1;
					size = marshalInfo.TryReadCompressedInteger(out value) ? value : -1;
					int sizeParameterMultiplier = marshalInfo.TryReadCompressedInteger(out value) ? value : -1;
					if (size >= 0)
					{
						b.AddNamedArg("SizeConst", KnownTypeCode.Int32, size);
					}
					if (sizeParameterMultiplier != 0 && sizeParameterIndex >= 0)
					{
						b.AddNamedArg("SizeParamIndex", KnownTypeCode.Int16, (short)sizeParameterIndex);
					}
					break;
				case 0x2c: // CustomMarshaler
					string guidValue = marshalInfo.ReadSerializedString();
					string unmanagedType = marshalInfo.ReadSerializedString();
					string managedType = marshalInfo.ReadSerializedString();
					string cookie = marshalInfo.ReadSerializedString();
					if (managedType != null)
					{
						b.AddNamedArg("MarshalType", KnownTypeCode.String, managedType);
					}
					if (!string.IsNullOrEmpty(cookie))
					{
						b.AddNamedArg("MarshalCookie", KnownTypeCode.String, cookie);
					}
					break;
				case 0x17: // FixedSysString
					b.AddNamedArg("SizeConst", KnownTypeCode.Int32, marshalInfo.ReadCompressedInteger());
					break;
			}

			return b.Build();
		}
		#endregion

		#region Custom Attributes (ReadAttribute)
		public void Add(CustomAttributeHandleCollection attributes, SymbolKind target)
		{
			var metadata = module.metadata;
			foreach (var handle in attributes)
			{
				var attribute = metadata.GetCustomAttribute(handle);
				// Attribute types shouldn't be open generic, so we don't need a generic context.
				var ctor = module.ResolveMethod(attribute.Constructor, new GenericContext());
				var type = ctor.DeclaringType;
				if (IgnoreAttribute(type, target))
				{
					continue;
				}
				Add(new CustomAttribute(module, ctor, handle));
			}
		}

		bool IgnoreAttribute(IType attributeType, SymbolKind target)
		{
			if (attributeType.DeclaringType != null || attributeType.TypeParameterCount != 0)
				return false;
			return IgnoreAttribute(new TopLevelTypeName(attributeType.Namespace, attributeType.Name), target);
		}

		internal bool IgnoreAttribute(TopLevelTypeName attributeType, SymbolKind target)
		{
			switch (attributeType.Namespace)
			{
				case "System.Runtime.CompilerServices":
					var options = module.TypeSystemOptions;
					switch (attributeType.Name)
					{
						case "DynamicAttribute":
							return (options & TypeSystemOptions.Dynamic) != 0;
						case "NativeIntegerAttribute":
							return (options & TypeSystemOptions.NativeIntegers) != 0;
						case "TupleElementNamesAttribute":
							return (options & TypeSystemOptions.Tuple) != 0;
						case "ExtensionAttribute":
							return (options & TypeSystemOptions.ExtensionMethods) != 0;
						case "DecimalConstantAttribute":
							return (options & TypeSystemOptions.DecimalConstants) != 0 && (target == SymbolKind.Field || target == SymbolKind.Parameter);
						case "IsReadOnlyAttribute":
							switch (target)
							{
								case SymbolKind.TypeDefinition:
								case SymbolKind.Parameter:
									return (options & TypeSystemOptions.ReadOnlyStructsAndParameters) != 0;
								case SymbolKind.Method:
								case SymbolKind.Accessor:
									return (options & TypeSystemOptions.ReadOnlyMethods) != 0;
								case SymbolKind.ReturnType:
								case SymbolKind.Property:
								case SymbolKind.Indexer:
									return true;  // "ref readonly" is currently always active
								default:
									return false;
							}
						case "IsByRefLikeAttribute":
							return (options & TypeSystemOptions.RefStructs) != 0 && target == SymbolKind.TypeDefinition;
						case "IsUnmanagedAttribute":
							return (options & TypeSystemOptions.UnmanagedConstraints) != 0 && target == SymbolKind.TypeParameter;
						case "NullableAttribute":
							return (options & TypeSystemOptions.NullabilityAnnotations) != 0;
						case "NullableContextAttribute":
							return (options & TypeSystemOptions.NullabilityAnnotations) != 0
								&& (target == SymbolKind.TypeDefinition || IsMethodLike(target));
						case "LifetimeAnnotationAttribute":
							return (options & TypeSystemOptions.LifetimeAnnotations) != 0
								&& (target == SymbolKind.Parameter);
						default:
							return false;
					}
				case "System":
					return attributeType.Name == "ParamArrayAttribute" && target == SymbolKind.Parameter;
				default:
					return false;
			}
		}

		internal bool HasAttribute(MetadataReader metadata, CustomAttributeHandleCollection customAttributes, KnownAttribute attribute, SymbolKind symbolKind)
		{
			Debug.Assert(attribute.IsCustomAttribute());
			foreach (var h in customAttributes)
			{
				var attr = metadata.GetCustomAttribute(h);
				if (attr.IsKnownAttribute(metadata, attribute))
				{
					return !IgnoreAttribute(attribute.GetTypeName(), symbolKind);
				}
			}

			return false;
		}

		internal IAttribute GetAttribute(MetadataReader metadata, CustomAttributeHandleCollection customAttributes, KnownAttribute attribute, SymbolKind symbolKind)
		{
			Debug.Assert(attribute.IsCustomAttribute());
			foreach (var h in customAttributes)
			{
				var attr = metadata.GetCustomAttribute(h);
				if (attr.IsKnownAttribute(metadata, attribute)
					&& !IgnoreAttribute(attribute.GetTypeName(), symbolKind))
				{
					// Attribute types shouldn't be open generic, so we don't need a generic context.
					var ctor = module.ResolveMethod(attr.Constructor, new GenericContext());
					return new CustomAttribute(module, ctor, h);
				}
			}

			return null;
		}

		static bool IsMethodLike(SymbolKind kind)
		{
			return kind switch {
				SymbolKind.Method => true,
				SymbolKind.Operator => true,
				SymbolKind.Constructor => true,
				SymbolKind.Destructor => true,
				SymbolKind.Accessor => true,
				_ => false
			};
		}
		#endregion

		#region Security Attributes
		public void AddSecurityAttributes(DeclarativeSecurityAttributeHandleCollection securityDeclarations)
		{
			var metadata = module.metadata;
			foreach (var secDecl in securityDeclarations)
			{
				if (secDecl.IsNil)
					continue;
				try
				{
					AddSecurityAttributes(metadata.GetDeclarativeSecurityAttribute(secDecl));
				}
				catch (EnumUnderlyingTypeResolveException)
				{
					// ignore resolve errors
				}
				catch (BadImageFormatException)
				{
					// ignore invalid security declarations
				}
			}
		}

		public void AddSecurityAttributes(DeclarativeSecurityAttribute secDecl)
		{
			var securityActionType = module.Compilation.FindType(new TopLevelTypeName("System.Security.Permissions", "SecurityAction"));
			var securityAction = new CustomAttributeTypedArgument<IType>(securityActionType, (int)secDecl.Action);
			var metadata = module.metadata;
			var reader = metadata.GetBlobReader(secDecl.PermissionSet);
			if (reader.ReadByte() == '.')
			{
				// binary attribute
				int attributeCount = reader.ReadCompressedInteger();
				for (int i = 0; i < attributeCount; i++)
				{
					Add(ReadBinarySecurityAttribute(ref reader, securityAction));
				}
			}
			else
			{
				// for backward compatibility with .NET 1.0: XML-encoded attribute
				reader.Reset();
				Add(ReadXmlSecurityAttribute(ref reader, securityAction));
			}
		}

		private IAttribute ReadXmlSecurityAttribute(ref SRM.BlobReader reader, CustomAttributeTypedArgument<IType> securityAction)
		{
			string xml = reader.ReadUTF16(reader.RemainingBytes);
			var b = new AttributeBuilder(module, KnownAttribute.PermissionSet);
			b.AddFixedArg(securityAction);
			b.AddNamedArg("XML", KnownTypeCode.String, xml);
			return b.Build();
		}

		private IAttribute ReadBinarySecurityAttribute(ref SRM.BlobReader reader, CustomAttributeTypedArgument<IType> securityAction)
		{
			string attributeTypeName = reader.ReadSerializedString();
			IType attributeType = module.TypeProvider.GetTypeFromSerializedName(attributeTypeName);

			reader.ReadCompressedInteger(); // ??
											// The specification seems to be incorrect here, so I'm using the logic from Cecil instead.
			int numNamed = reader.ReadCompressedInteger();

			var decoder = new Metadata.CustomAttributeDecoder<IType>(module.TypeProvider, module.metadata);
			var namedArgs = decoder.DecodeNamedArguments(ref reader, numNamed);

			return new DefaultAttribute(
				attributeType,
				fixedArguments: ImmutableArray.Create(securityAction),
				namedArguments: namedArgs);
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

	struct AttributeBuilder
	{
		readonly ICompilation compilation;
		readonly IType attributeType;
		ImmutableArray<CustomAttributeTypedArgument<IType>>.Builder fixedArgs;
		ImmutableArray<CustomAttributeNamedArgument<IType>>.Builder namedArgs;

		public AttributeBuilder(MetadataModule module, KnownAttribute attributeType)
			: this(module, module.GetAttributeType(attributeType))
		{
		}

		public AttributeBuilder(MetadataModule module, IType attributeType)
		{
			this.compilation = module.Compilation;
			this.attributeType = attributeType;
			this.fixedArgs = ImmutableArray.CreateBuilder<CustomAttributeTypedArgument<IType>>();
			this.namedArgs = ImmutableArray.CreateBuilder<CustomAttributeNamedArgument<IType>>();
		}

		public void AddFixedArg(CustomAttributeTypedArgument<IType> arg)
		{
			fixedArgs.Add(arg);
		}

		public void AddFixedArg(KnownTypeCode type, object value)
		{
			AddFixedArg(compilation.FindType(type), value);
		}

		public void AddFixedArg(TopLevelTypeName type, object value)
		{
			AddFixedArg(compilation.FindType(type), value);
		}

		public void AddFixedArg(IType type, object value)
		{
			fixedArgs.Add(new CustomAttributeTypedArgument<IType>(type, value));
		}

		public void AddNamedArg(string name, KnownTypeCode type, object value)
		{
			AddNamedArg(name, compilation.FindType(type), value);
		}

		public void AddNamedArg(string name, TopLevelTypeName type, object value)
		{
			AddNamedArg(name, compilation.FindType(type), value);
		}

		public void AddNamedArg(string name, IType type, object value)
		{
			CustomAttributeNamedArgumentKind kind;
			if (attributeType.GetFields(f => f.Name == name, GetMemberOptions.ReturnMemberDefinitions).Any())
				kind = CustomAttributeNamedArgumentKind.Field;
			else
				kind = CustomAttributeNamedArgumentKind.Property;
			namedArgs.Add(new CustomAttributeNamedArgument<IType>(name, kind, type, value));
		}

		public IAttribute Build()
		{
			return new DefaultAttribute(attributeType, fixedArgs.ToImmutable(), namedArgs.ToImmutable());
		}
	}
}
