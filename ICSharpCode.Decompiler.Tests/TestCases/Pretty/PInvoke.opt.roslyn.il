.module extern xyz.dll
.module extern ws2_32.dll
.assembly extern mscorlib
{
	.publickeytoken = (
		b7 7a 5c 56 19 34 e0 89
	)
	.ver 4:0:0:0
}
.assembly PInvoke
{
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = (
		01 00 08 00 00 00 00 00
	)
	.custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = (
		01 00 01 00 54 02 16 57 72 61 70 4e 6f 6e 45 78
		63 65 70 74 69 6f 6e 54 68 72 6f 77 73 01
	)
	.custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = (
		01 00 02 00 00 00 00 00
	)
	.permissionset reqmin = {
		[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {
			property bool SkipVerification = bool(true)
		}
	}
	.hash algorithm 0x00008004 // SHA1
	.ver 0:0:0:0
}

.module PInvoke.opt.roslyn
// MVID: {63CD2985-0EDA-4C6F-91E0-BFDEBA4905D5}
.corflags 0x00000001 // ILOnly
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = (
	01 00 00 00
)

.class private auto ansi '<Module>'
{
} // end of class <Module>

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.PInvoke
	extends [mscorlib]System.Object
{
	// Nested Types
	.class nested public sequential unicode sealed beforefieldinit MarshalAsTest
		extends [mscorlib]System.ValueType
	{
		.pack 2
		.size 0

		// Fields
		.field public marshal(fixed array[3]) uint32[] FixedArray
		.field public marshal(fixed array[4] bool) int32[] FixedBoolArray
		.field public marshal(safearray bstr) string[] SafeBStrArray
		.field public marshal(fixed sysstring[8]) string FixedString

	} // end of class MarshalAsTest

	.class nested public explicit ansi sealed beforefieldinit Rect
		extends [mscorlib]System.ValueType
	{
		// Fields
		.field [0] public int32 left
		.field [4] public int32 top
		.field [8] public int32 right
		.field [12] public int32 bottom

	} // end of class Rect


	// Methods
	.method public hidebysig specialname static 
		valuetype [mscorlib]System.Decimal marshal(currency) get_MarshalAttributesOnPropertyAccessors () cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 6 (0x6)
		.maxstack 8

		IL_0000: newobj instance void [mscorlib]System.NotImplementedException::.ctor()
		IL_0005: throw
	} // end of method PInvoke::get_MarshalAttributesOnPropertyAccessors

	.method public hidebysig specialname static 
		void set_MarshalAttributesOnPropertyAccessors (
			valuetype [mscorlib]System.Decimal marshal(currency) 'value'
		) cil managed 
	{
		// Method begins at RVA 0x2057
		// Code size 1 (0x1)
		.maxstack 8

		IL_0000: ret
	} // end of method PInvoke::set_MarshalAttributesOnPropertyAccessors

	.method public hidebysig static pinvokeimpl("xyz.dll" autochar winapi) 
		bool marshal(bool) Method (
			string marshal(lpstr) input
		) cil managed preservesig 
	{
	} // end of method PInvoke::Method

	.method private hidebysig static pinvokeimpl("xyz.dll" winapi) 
		void New1 (
			int32 ElemCnt,
			int32[] marshal([ + 0]) ar
		) cil managed preservesig 
	{
	} // end of method PInvoke::New1

	.method private hidebysig static pinvokeimpl("xyz.dll" winapi) 
		void New2 (
			int32[] marshal([128]) ar
		) cil managed preservesig 
	{
	} // end of method PInvoke::New2

	.method private hidebysig static pinvokeimpl("xyz.dll" winapi) 
		void New3 (
			int32[] marshal(bool[64 + 1]) ar
		) cil managed preservesig 
	{
	} // end of method PInvoke::New3

	.method private hidebysig static pinvokeimpl("xyz.dll" winapi) 
		void New4 (
			int32[] marshal([]) ar
		) cil managed preservesig 
	{
	} // end of method PInvoke::New4

	.method public hidebysig 
		instance void CustomMarshal1 (
			object marshal(custom("MyCompany.MyMarshaler", "")) o
		) cil managed 
	{
		// Method begins at RVA 0x2059
		// Code size 1 (0x1)
		.maxstack 8

		IL_0000: ret
	} // end of method PInvoke::CustomMarshal1

	.method public hidebysig 
		instance void CustomMarshal2 (
			object marshal(custom("MyCompany.MyMarshaler", "Cookie")) o
		) cil managed 
	{
		// Method begins at RVA 0x205b
		// Code size 1 (0x1)
		.maxstack 8

		IL_0000: ret
	} // end of method PInvoke::CustomMarshal2

	.method assembly hidebysig static pinvokeimpl("ws2_32.dll" lasterr winapi) 
		native int ioctlsocket (
			[in] native int socketHandle,
			[in] int32 cmd,
			[in] [out] int32& argp
		) cil managed preservesig 
	{
	} // end of method PInvoke::ioctlsocket

	.method public hidebysig 
		instance void CallMethodWithInOutParameter () cil managed 
	{
		// Method begins at RVA 0x2060
		// Code size 17 (0x11)
		.maxstack 3
		.locals init (
			[0] int32
		)

		IL_0000: ldc.i4.0
		IL_0001: stloc.0
		IL_0002: ldsfld native int [mscorlib]System.IntPtr::Zero
		IL_0007: ldc.i4.0
		IL_0008: ldloca.s 0
		IL_000a: call native int ICSharpCode.Decompiler.Tests.TestCases.Pretty.PInvoke::ioctlsocket(native int, int32, int32&)
		IL_000f: pop
		IL_0010: ret
	} // end of method PInvoke::CallMethodWithInOutParameter

	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x207d
		// Code size 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ret
	} // end of method PInvoke::.ctor

	// Properties
	.property valuetype [mscorlib]System.Decimal MarshalAttributesOnPropertyAccessors()
	{
		.get valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.PInvoke::get_MarshalAttributesOnPropertyAccessors()
		.set void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PInvoke::set_MarshalAttributesOnPropertyAccessors(valuetype [mscorlib]System.Decimal)
	}

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.PInvoke

