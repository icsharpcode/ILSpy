using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	enum KnownAttribute
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

		// Assembly attributes:
		AssemblyVersion,
		InternalsVisibleTo,
		
		// Type attributes:
		Serializable,
		ComImport,
		StructLayout,
		DefaultMember,

		// Field attributes:
		FieldOffset,
		NonSerialized,
		DecimalConstant,

		// Method attributes:
		DllImport,
		PreserveSig,
		MethodImpl,

		// Parameter attributes:
		ParamArray,
		In,
		Out,

		// Marshalling attributes:
		MarshalAs,

		// Security attributes:
		PermissionSet,
	}

	static class KnownAttributes
	{
		internal const int Count = (int)KnownAttribute.PermissionSet + 1;

		static readonly TopLevelTypeName[] typeNames = new TopLevelTypeName[Count]{
			default,
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(CompilerGeneratedAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(ExtensionAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(DynamicAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(TupleElementNamesAttribute)),
			// Assembly attributes:
			new TopLevelTypeName("System.Reflection", nameof(AssemblyVersionAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(InternalsVisibleToAttribute)),
			// Type attributes:
			new TopLevelTypeName("System", nameof(SerializableAttribute)),
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(ComImportAttribute)),
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(StructLayoutAttribute)),
			new TopLevelTypeName("System.Reflection", nameof(DefaultMemberAttribute)),
			// Field attributes:
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(FieldOffsetAttribute)),
			new TopLevelTypeName("System", nameof(NonSerializedAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(DecimalConstantAttribute)),
			// Method attributes:
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(DllImportAttribute)),
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(PreserveSigAttribute)),
			new TopLevelTypeName("System.Runtime.CompilerServices", nameof(MethodImplAttribute)),
			// Parameter attributes:
			new TopLevelTypeName("System", nameof(ParamArrayAttribute)),
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(InAttribute)),
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(OutAttribute)),
			// Marshalling attributes:
			new TopLevelTypeName("System.Runtime.InteropServices", nameof(MarshalAsAttribute)),
			// Security attributes:
			new TopLevelTypeName("System.Security", "PermissionSetAttribute"),
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
	}
}
