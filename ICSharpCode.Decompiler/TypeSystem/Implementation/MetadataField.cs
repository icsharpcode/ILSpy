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
using System.Runtime.InteropServices;
using System.Threading;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Field definition backed by System.Reflection.Metadata
	/// </summary>
	class MetadataField : IField
	{
		readonly MetadataAssembly assembly;
		readonly FieldDefinitionHandle handle;
		readonly FieldAttributes attributes;

		// lazy-loaded fields:
		ITypeDefinition declaringType;
		string name;
		object constantValue;
		IType type;
		bool isVolatile; // initialized together with this.type
		IAttribute[] customAttributes;

		internal MetadataField(MetadataAssembly assembly, FieldDefinitionHandle handle)
		{
			Debug.Assert(assembly != null);
			Debug.Assert(!handle.IsNil);
			this.assembly = assembly;
			this.handle = handle;
			var def = assembly.metadata.GetFieldDefinition(handle);
			this.attributes = def.Attributes;
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
				var metadata = assembly.metadata;
				var fieldDef = metadata.GetFieldDefinition(handle);
				return LazyInit.GetOrSet(ref this.name, metadata.GetString(fieldDef.Name));
			}
		}

		public Accessibility Accessibility => MetadataLoader.GetAccessibility(attributes);
		public bool IsReadOnly => (attributes & FieldAttributes.InitOnly) != 0;
		public bool IsStatic => (attributes & FieldAttributes.Static) != 0;

		// not sure if we need IsFixed anywhere...
		public bool IsFixed => throw new NotImplementedException();

		// Do we still want IsShadowing in the TS?
		// We never set it for assemblies loaded from disk; only for those parsed from C#...
		bool IEntity.IsShadowing => throw new NotImplementedException();

		SymbolKind ISymbol.SymbolKind => SymbolKind.Field;
		IMember IMember.MemberDefinition => this;
		TypeParameterSubstitution IMember.Substitution => TypeParameterSubstitution.Identity;

		// Fields can't implement interfaces:
		IReadOnlyList<IMember> IMember.ImplementedInterfaceMembers => EmptyList<IMember>.Instance;
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
					var def = assembly.metadata.GetFieldDefinition(handle);
					return LazyInit.GetOrSet(ref this.declaringType,
						assembly.GetDefinition(def.GetDeclaringType()));
				}
			}
		}

		public IType DeclaringType => DeclaringTypeDefinition;
		public IAssembly ParentAssembly => assembly;
		public ICompilation Compilation => assembly.Compilation;

		public IReadOnlyList<IAttribute> Attributes {
			get {
				var attr = LazyInit.VolatileRead(ref this.customAttributes);
				if (attr != null)
					return attr;
				return LazyInit.GetOrSet(ref this.customAttributes, DecodeAttributes());
			}
		}

		IAttribute[] DecodeAttributes()
		{
			var b = new AttributeListBuilder(assembly);
			var metadata = assembly.metadata;
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
		public string Namespace => DeclaringType?.Namespace;

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
			var metadata = assembly.metadata;
			var fieldDef = metadata.GetFieldDefinition(handle);
			var ty = fieldDef.DecodeSignature(assembly.TypeProvider, new GenericContext(DeclaringType?.TypeParameters));
			if (ty is ModifiedType mod && mod.Modifier.Name == "IsVolatile" && mod.Modifier.Namespace == "System.Runtime.CompilerServices") {
				Volatile.Write(ref this.isVolatile, true);
				ty = mod.ElementType;
			}
			return LazyInit.GetOrSet(ref this.type, ty);
		}

		// TODO: decimal constants
		public bool IsConst => (attributes & FieldAttributes.Literal) != 0;

		public object ConstantValue {
			get {
				object val = LazyInit.VolatileRead(ref this.constantValue);
				if (val != null)
					return val;
				var metadata = assembly.metadata;
				var fieldDef = metadata.GetFieldDefinition(handle);
				var constantHandle = fieldDef.GetDefaultValue();
				if (constantHandle.IsNil)
					return null;
				var constant = metadata.GetConstant(constantHandle);
				var blobReader = metadata.GetBlobReader(constant.Value);
				val = blobReader.ReadConstant(constant.TypeCode);
				return LazyInit.GetOrSet(ref this.constantValue, val);
			}
		}
		
		public bool Equals(IMember obj, TypeVisitor typeNormalization)
		{
			return this == obj;
		}

		public IMember Specialize(TypeParameterSubstitution substitution)
		{
			return SpecializedField.Create(this, substitution);
		}
	}
}
