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

	static class KnownAttributes
	{
		internal const int Count = (int)KnownAttribute.PreserveBaseOverrides + 1;

		static readonly TopLevelTypeName[] typeNames = new TopLevelTypeName[Count]{
			default,
			new("System.Runtime.CompilerServices", nameof(CompilerGeneratedAttribute)),
			new("System.Runtime.CompilerServices", nameof(ExtensionAttribute)),
			new("System.Runtime.CompilerServices", nameof(DynamicAttribute)),
			new("System.Runtime.CompilerServices", nameof(TupleElementNamesAttribute)),
			new("System.Runtime.CompilerServices", "NullableAttribute"),
			new("System.Runtime.CompilerServices", "NullableContextAttribute"),
			new("System.Runtime.CompilerServices", "NullablePublicOnlyAttribute"),
			new("System.Diagnostics", nameof(ConditionalAttribute)),
			new("System", nameof(ObsoleteAttribute)),
			new("System.Runtime.CompilerServices", "IsReadOnlyAttribute"),
			new("System.Runtime.CompilerServices", nameof(SpecialNameAttribute)),
			new("System.Diagnostics", nameof(DebuggerHiddenAttribute)),
			new("System.Diagnostics", nameof(DebuggerStepThroughAttribute)),
			new("System.Diagnostics", nameof(DebuggerBrowsableAttribute)),
			// Assembly attributes:
			new("System.Reflection", nameof(AssemblyVersionAttribute)),
			new("System.Runtime.CompilerServices", nameof(InternalsVisibleToAttribute)),
			new("System.Runtime.CompilerServices", nameof(TypeForwardedToAttribute)),
			new("System.Runtime.CompilerServices", nameof(ReferenceAssemblyAttribute)),
			// Type attributes:
			new("System", nameof(SerializableAttribute)),
			new("System", nameof(FlagsAttribute)),
			new("System.Runtime.InteropServices", nameof(ComImportAttribute)),
			new("System.Runtime.InteropServices", nameof(CoClassAttribute)),
			new("System.Runtime.InteropServices", nameof(StructLayoutAttribute)),
			new("System.Reflection", nameof(DefaultMemberAttribute)),
			new("System.Runtime.CompilerServices", "IsByRefLikeAttribute"),
			new("System.Runtime.CompilerServices", nameof(IteratorStateMachineAttribute)),
			new("System.Runtime.CompilerServices", nameof(AsyncStateMachineAttribute)),
			new("System.Runtime.CompilerServices", "AsyncMethodBuilderAttribute"),
			new("System.Runtime.CompilerServices", "AsyncIteratorStateMachineAttribute"),
			// Field attributes:
			new("System.Runtime.InteropServices", nameof(FieldOffsetAttribute)),
			new("System", nameof(NonSerializedAttribute)),
			new("System.Runtime.CompilerServices", nameof(DecimalConstantAttribute)),
			new("System.Runtime.CompilerServices", nameof(FixedBufferAttribute)),
			// Method attributes:
			new("System.Runtime.InteropServices", nameof(DllImportAttribute)),
			new("System.Runtime.InteropServices", nameof(PreserveSigAttribute)),
			new("System.Runtime.CompilerServices", nameof(MethodImplAttribute)),
			// Property attributes:
			new("System.Runtime.CompilerServices", nameof(IndexerNameAttribute)),
			// Parameter attributes:
			new("System", nameof(ParamArrayAttribute)),
			new("System.Runtime.InteropServices", nameof(InAttribute)),
			new("System.Runtime.InteropServices", nameof(OutAttribute)),
			new("System.Runtime.InteropServices", nameof(OptionalAttribute)),
			new("System.Runtime.CompilerServices", nameof(CallerMemberNameAttribute)),
			new("System.Runtime.CompilerServices", nameof(CallerFilePathAttribute)),
			new("System.Runtime.CompilerServices", nameof(CallerLineNumberAttribute)),
			// Type parameter attributes:
			new("System.Runtime.CompilerServices", "IsUnmanagedAttribute"),
			// Marshalling attributes:
			new("System.Runtime.InteropServices", nameof(MarshalAsAttribute)),
			// Security attributes:
			new("System.Security.Permissions", "PermissionSetAttribute"),
			// C# 9 attributes:
			new("System.Runtime.CompilerServices", "NativeIntegerAttribute"),
			new("System.Runtime.CompilerServices", "PreserveBaseOverridesAttribute"),
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
	}
}
