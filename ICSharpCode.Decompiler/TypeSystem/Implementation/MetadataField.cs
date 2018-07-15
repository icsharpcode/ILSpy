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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Field definition backed by System.Reflection.Metadata
	/// </summary>
	sealed class MetadataField : IField
	{
		readonly MetadataModule module;
		readonly FieldDefinitionHandle handle;
		readonly FieldAttributes attributes;

		// lazy-loaded fields:
		ITypeDefinition declaringType;
		string name;
		object constantValue;
		IType type;
		bool isVolatile; // initialized together with this.type
		byte decimalConstant; // 0=no, 1=yes, 2=unknown

		internal MetadataField(MetadataModule module, FieldDefinitionHandle handle)
		{
			Debug.Assert(module != null);
			Debug.Assert(!handle.IsNil);
			this.module = module;
			this.handle = handle;
			var def = module.metadata.GetFieldDefinition(handle);
			this.attributes = def.Attributes;
			if ((attributes & (FieldAttributes.Static | FieldAttributes.InitOnly)) == (FieldAttributes.Static | FieldAttributes.InitOnly)) {
				decimalConstant = 2; // may be decimal constant
			}
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
				var fieldDef = metadata.GetFieldDefinition(handle);
				return LazyInit.GetOrSet(ref this.name, metadata.GetString(fieldDef.Name));
			}
		}

		public Accessibility Accessibility {
			get {
				switch (attributes & FieldAttributes.FieldAccessMask) {
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
		}

		public bool IsReadOnly => (attributes & FieldAttributes.InitOnly) != 0;
		public bool IsStatic => (attributes & FieldAttributes.Static) != 0;
		
		SymbolKind ISymbol.SymbolKind => SymbolKind.Field;
		IMember IMember.MemberDefinition => this;
		TypeParameterSubstitution IMember.Substitution => TypeParameterSubstitution.Identity;

		// Fields can't implement interfaces:
		IEnumerable<IMember> IMember.ExplicitlyImplementedInterfaceMembers => EmptyList<IMember>.Instance;
		bool IMember.IsExplicitInterfaceImplementation => false;
		bool IMember.IsVirtual => false;
		bool IMember.IsOverride => false;
		bool IMember.IsOverridable => false;
		bool IEntity.IsAbstract => false;
		bool IEntity.IsSealed => false;

		public ITypeDefinition DeclaringTypeDefinition {
			get {
				var declType = LazyInit.VolatileRead(ref this.declaringType);
				if (declType != null) {
					return declType;
				} else {
					var def = module.metadata.GetFieldDefinition(handle);
					return LazyInit.GetOrSet(ref this.declaringType,
						module.GetDefinition(def.GetDeclaringType()));
				}
			}
		}

		public IType DeclaringType => DeclaringTypeDefinition;
		public IModule ParentModule => module;
		public ICompilation Compilation => module.Compilation;
		
		public IEnumerable<IAttribute> GetAttributes()
		{
			var b = new AttributeListBuilder(module);
			var metadata = module.metadata;
			var fieldDef = metadata.GetFieldDefinition(handle);

			// FieldOffsetAttribute
			int offset = fieldDef.GetOffset();
			if (offset != -1) {
				b.Add(KnownAttribute.FieldOffset, KnownTypeCode.Int32, offset);
			}

			// NonSerializedAttribute
			if ((fieldDef.Attributes & FieldAttributes.NotSerialized) != 0) {
				b.Add(KnownAttribute.NonSerialized);
			}

			b.AddMarshalInfo(fieldDef.GetMarshallingDescriptor());
			b.Add(fieldDef.GetCustomAttributes());

			return b.Build();
		}

		public string FullName => $"{DeclaringType?.FullName}.{Name}";
		public string ReflectionName => $"{DeclaringType?.ReflectionName}.{Name}";
		public string Namespace => DeclaringType?.Namespace ?? string.Empty;

		public bool IsVolatile {
			get {
				if (LazyInit.VolatileRead(ref this.type) == null) {
					DecodeTypeAndVolatileFlag();
				}
				return this.isVolatile;
			}
		}
		IType IMember.ReturnType => Type;
		public IType Type {
			get {
				var ty = LazyInit.VolatileRead(ref this.type);
				if (ty != null) {
					return ty;
				}
				return DecodeTypeAndVolatileFlag();
			}
		}

		private IType DecodeTypeAndVolatileFlag()
		{
			var metadata = module.metadata;
			var fieldDef = metadata.GetFieldDefinition(handle);
			var ty = fieldDef.DecodeSignature(module.TypeProvider, new GenericContext(DeclaringType?.TypeParameters));
			if (ty is ModifiedType mod && mod.Modifier.Name == "IsVolatile" && mod.Modifier.Namespace == "System.Runtime.CompilerServices") {
				Volatile.Write(ref this.isVolatile, true);
				ty = mod.ElementType;
			}
			ty = ApplyAttributeTypeVisitor.ApplyAttributesToType(ty, Compilation,
				fieldDef.GetCustomAttributes(), metadata, module.TypeSystemOptions);
			return LazyInit.GetOrSet(ref this.type, ty);
		}

		public bool IsConst => (attributes & FieldAttributes.Literal) != 0 || IsDecimalConstant;

		bool IsDecimalConstant {
			get {
				if (decimalConstant == 2) {
					var metadata = module.metadata;
					var fieldDef = metadata.GetFieldDefinition(handle);
					if (fieldDef.GetCustomAttributes().HasKnownAttribute(metadata, KnownAttribute.DecimalConstant))
						decimalConstant = 1;
					else
						decimalConstant = 0;
				}
				return decimalConstant == 1;
			}
		}

		public object ConstantValue {
			get {
				object val = LazyInit.VolatileRead(ref this.constantValue);
				if (val != null)
					return val;
				var metadata = module.metadata;
				var fieldDef = metadata.GetFieldDefinition(handle);
				if (IsDecimalConstant) {
					foreach (var attrHandle in fieldDef.GetCustomAttributes()) {
						var attribute = metadata.GetCustomAttribute(attrHandle);
						if (attribute.IsKnownAttribute(metadata, KnownAttribute.DecimalConstant)) {
							val = TryDecodeDecimalConstantAttribute(attribute);
						}
					}
				} else {
					var constantHandle = fieldDef.GetDefaultValue();
					if (constantHandle.IsNil)
						return null;
					var constant = metadata.GetConstant(constantHandle);
					var blobReader = metadata.GetBlobReader(constant.Value);
					val = blobReader.ReadConstant(constant.TypeCode);
				}
				return LazyInit.GetOrSet(ref this.constantValue, val);
			}
		}

		decimal? TryDecodeDecimalConstantAttribute(System.Reflection.Metadata.CustomAttribute attribute)
		{
			var attrValue = attribute.DecodeValue(module.TypeProvider);
			if (attrValue.FixedArguments.Length != 5)
				return null;
			// DecimalConstantAttribute has the arguments (byte scale, byte sign, uint hi, uint mid, uint low) or (byte scale, byte sign, int hi, int mid, int low)
			// Both of these invoke the Decimal constructor (int lo, int mid, int hi, bool isNegative, byte scale) with explicit argument conversions if required.
			if (!(attrValue.FixedArguments[0].Value is byte scale && attrValue.FixedArguments[1].Value is byte sign))
				return null;
			unchecked {
				if (attrValue.FixedArguments[2].Value is uint hi
					&& attrValue.FixedArguments[3].Value is uint mid
					&& attrValue.FixedArguments[4].Value is uint lo) {
					return new decimal((int)lo, (int)mid, (int)hi, sign != 0, scale);
				}
			}
			{
				if (attrValue.FixedArguments[2].Value is int hi
					&& attrValue.FixedArguments[3].Value is int mid
					&& attrValue.FixedArguments[4].Value is int lo) {
					return new decimal(lo, mid, hi, sign != 0, scale);
				}
			}
			return null;
		}

		public override bool Equals(object obj)
		{
			if (obj is MetadataField f) {
				return handle == f.handle && module.PEFile == f.module.PEFile;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 0x11dda32b ^ module.PEFile.GetHashCode() ^ handle.GetHashCode();
		}

		bool IMember.Equals(IMember obj, TypeVisitor typeNormalization)
		{
			return Equals(obj);
		}

		public IMember Specialize(TypeParameterSubstitution substitution)
		{
			return SpecializedField.Create(this, substitution);
		}
	}
}
