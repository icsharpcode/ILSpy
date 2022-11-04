// Copyright (c) 2010-2018 Daniel Grunwald
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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ICSharpCode.Decompiler.TypeSystem
{
	public enum KnownAttribute
	{
		/// <summary>
		/// Not a known attribute
		/// </summary>
		None,

		CompilerGenerated,
		/// <summary>
		/// Marks a method as extension method; or a class as containing extension methods.
		/// </summary>
		Extension,
		Dynamic,
		TupleElementNames,
		Nullable,
		NullableContext,
		NullablePublicOnly,
		Conditional,
		Obsolete,
		Embedded,
		IsReadOnly,
		SpecialName,
		DebuggerHidden,
		DebuggerStepThrough,
		DebuggerBrowsable,

		// Assembly attributes:
		AssemblyVersion,
		InternalsVisibleTo,
		TypeForwardedTo,
		ReferenceAssembly,

		// Type attributes:
		Serializable,
		Flags,
		ComImport,
		CoClass,
		StructLayout,
		DefaultMember,
		IsByRefLike,
		IteratorStateMachine,
		AsyncStateMachine,
		AsyncMethodBuilder,
		AsyncIteratorStateMachine,

		// Field attributes:
		FieldOffset,
		NonSerialized,
		DecimalConstant,
		FixedBuffer,

		// Method attributes:
		DllImport,
		PreserveSig,
		MethodImpl,

		// Property attributes:
		IndexerName,

		// Parameter attributes:
		ParamArray,
		In,
		Out,
		Optional,
		CallerMemberName,
		CallerFilePath,
		CallerLineNumber,
		LifetimeAnnotation,

		// Type parameter attributes:
		IsUnmanaged,

		// Marshalling attributes:
		MarshalAs,

		// Security attributes:
		PermissionSet,

		// C# 9 attributes:
		NativeInteger,
		PreserveBaseOverrides,
	}

	public static class KnownAttributes
	{
		internal const int Count = (int)KnownAttribute.PreserveBaseOverrides + 1;

		static readonly TopLevelTypeName[] typeNames = new TopLevelTypeName[Count]{
			default,
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(CompilerGeneratedAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(ExtensionAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(DynamicAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(TupleElementNamesAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", "NullableAttribute"),
			new TopLevelTypeName("System.Runtime.CompilerServices", "NullableContextAttribute"),
			new TopLevelTypeName("System.Runtime.CompilerServices", "NullablePublicOnlyAttribute"),
			new TopLevelTypeName("System.Diagnostics", nameof(ConditionalAttribute)),
			new TopLevelTypeName("System", nameof(ObsoleteAttribute)),
			new TopLevelTypeName("Microsoft.CodeAnalysis", "EmbeddedAttribute"),
			new TopLevelTypeName("System.Runtime.CompilerServices", "IsReadOnlyAttribute"),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(SpecialNameAttribute)),
			new TopLevelTypeName("System.Diagnostics", nameof(DebuggerHiddenAttribute)),
			new TopLevelTypeName("System.Diagnostics", nameof(DebuggerStepThroughAttribute)),
			new TopLevelTypeName("System.Diagnostics", nameof(DebuggerBrowsableAttribute)),
			// Assembly attributes:
			new TopLevelTypeName("System.Reflection", nameof(AssemblyVersionAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(InternalsVisibleToAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(TypeForwardedToAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(ReferenceAssemblyAttribute)),
			// Type attributes:
			new TopLevelTypeName("System", nameof(SerializableAttribute)),
			new TopLevelTypeName("System", nameof(FlagsAttribute)),
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(ComImportAttribute)),
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(CoClassAttribute)),
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(StructLayoutAttribute)),
			new TopLevelTypeName("System.Reflection", nameof(DefaultMemberAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", "IsByRefLikeAttribute"),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(IteratorStateMachineAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(AsyncStateMachineAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", "AsyncMethodBuilderAttribute"),
			new TopLevelTypeName("System.Runtime.CompilerServices", "AsyncIteratorStateMachineAttribute"),
			// Field attributes:
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(FieldOffsetAttribute)),
			new TopLevelTypeName("System", nameof(NonSerializedAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(DecimalConstantAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(FixedBufferAttribute)),
			// Method attributes:
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(DllImportAttribute)),
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(PreserveSigAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(MethodImplAttribute)),
			// Property attributes:
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(IndexerNameAttribute)),
			// Parameter attributes:
			new TopLevelTypeName("System", nameof(ParamArrayAttribute)),
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(InAttribute)),
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(OutAttribute)),
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(OptionalAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(CallerMemberNameAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(CallerFilePathAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(CallerLineNumberAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", "LifetimeAnnotationAttribute"),
			// Type parameter attributes:
			new TopLevelTypeName("System.Runtime.CompilerServices", "IsUnmanagedAttribute"),
			// Marshalling attributes:
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(MarshalAsAttribute)),
			// Security attributes:
			new TopLevelTypeName("System.Security.Permissions", "PermissionSetAttribute"),
			// C# 9 attributes:
			new TopLevelTypeName("System.Runtime.CompilerServices", "NativeIntegerAttribute"),
			new TopLevelTypeName("System.Runtime.CompilerServices", "PreserveBaseOverridesAttribute"),
		};

		public static ref readonly TopLevelTypeName GetTypeName(this KnownAttribute attr)
		{
			Debug.Assert(attr != KnownAttribute.None);
			return ref typeNames[(int)attr];
		}

		public static IType FindType(this ICompilation compilation, KnownAttribute attrType)
		{
			return compilation.FindType(attrType.GetTypeName());
		}

		public static KnownAttribute IsKnownAttributeType(this ITypeDefinition attributeType)
		{
			if (!attributeType.GetNonInterfaceBaseTypes().Any(t => t.IsKnownType(KnownTypeCode.Attribute)))
				return KnownAttribute.None;
			for (int i = 1; i < typeNames.Length; i++)
			{
				if (typeNames[i] == attributeType.FullTypeName)
					return (KnownAttribute)i;
			}
			return KnownAttribute.None;
		}

		public static bool IsCustomAttribute(this KnownAttribute knownAttribute)
		{
			switch (knownAttribute)
			{
				case KnownAttribute.Serializable:
				case KnownAttribute.ComImport:
				case KnownAttribute.StructLayout:
				case KnownAttribute.DllImport:
				case KnownAttribute.PreserveSig:
				case KnownAttribute.MethodImpl:
				case KnownAttribute.FieldOffset:
				case KnownAttribute.NonSerialized:
				case KnownAttribute.MarshalAs:
				case KnownAttribute.PermissionSet:
				case KnownAttribute.Optional:
				case KnownAttribute.In:
				case KnownAttribute.Out:
				case KnownAttribute.IndexerName:
				case KnownAttribute.SpecialName:
					return false;
				default:
					return true;
			}
		}
	}
}
