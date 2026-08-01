// Trimmed disassembly of an assembly compiled with MSVC from IdStringProbe.cpp
// (next to this file) with /clr /doc. The signatures are verbatim from the
// MSVC-produced metadata; the accompanying IdStringProbe.xml is the unmodified
// output of MSVC's xml doc generator for the same compilation and serves as the
// reference for the C++/CLI (ECMA-372-style) ID string dialect.

.assembly extern mscorlib
{
	.ver 4:0:0:0
}
.assembly IdStringProbe
{
	.ver 0:0:0:0
}
.module IdStringProbe.dll

.class public auto ansi beforefieldinit IdProbe
	extends [mscorlib]System.Object
{
	.custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = (
		01 00 04 49 74 65 6d 00 00
	)
	// Methods
	.method public hidebysig static 
		int32 ConstValue (
			int32 modopt([mscorlib]System.Runtime.CompilerServices.IsConst) x
		) cil managed 
	{
		// Method begins at RVA 0x10e0
		// Header size: 12
		// Code size: 4 (0x4)
		.maxstack 1
		.locals (
			[0] int32
		)

		IL_0000: ldarg.0
		IL_0001: stloc.0
		IL_0002: ldloc.0
		IL_0003: ret
	} // end of method IdProbe::ConstValue

	.method public hidebysig static 
		void TakesLong (
			int32 modopt([mscorlib]System.Runtime.CompilerServices.IsLong) x
		) cil managed 
	{
		// Method begins at RVA 0x10f0
		// Header size: 12
		// Code size: 1 (0x1)
		.maxstack 0

		IL_0000: ret
	} // end of method IdProbe::TakesLong

	.method public hidebysig static 
		void TakesULong (
			uint32 modopt([mscorlib]System.Runtime.CompilerServices.IsLong) x
		) cil managed 
	{
		// Method begins at RVA 0x1100
		// Header size: 12
		// Code size: 1 (0x1)
		.maxstack 0

		IL_0000: ret
	} // end of method IdProbe::TakesULong

	.method public hidebysig static 
		void TakesCharPtr (
			int8 modopt([mscorlib]System.Runtime.CompilerServices.IsSignUnspecifiedByte)* p
		) cil managed 
	{
		// Method begins at RVA 0x1110
		// Header size: 12
		// Code size: 1 (0x1)
		.maxstack 0

		IL_0000: ret
	} // end of method IdProbe::TakesCharPtr

	.method public hidebysig static 
		void TakesConstCharPtr (
			int8 modopt([mscorlib]System.Runtime.CompilerServices.IsSignUnspecifiedByte) modopt([mscorlib]System.Runtime.CompilerServices.IsConst)* p
		) cil managed 
	{
		// Method begins at RVA 0x1120
		// Header size: 12
		// Code size: 1 (0x1)
		.maxstack 0

		IL_0000: ret
	} // end of method IdProbe::TakesConstCharPtr

	.method public hidebysig static 
		void TakesVolatilePtr (
			int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile)* p
		) cil managed 
	{
		// Method begins at RVA 0x1130
		// Header size: 12
		// Code size: 1 (0x1)
		.maxstack 0

		IL_0000: ret
	} // end of method IdProbe::TakesVolatilePtr

	.method public hidebysig static 
		void TakesConstVolatilePtr (
			int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) modopt([mscorlib]System.Runtime.CompilerServices.IsConst)* p
		) cil managed 
	{
		// Method begins at RVA 0x1140
		// Header size: 12
		// Code size: 1 (0x1)
		.maxstack 0

		IL_0000: ret
	} // end of method IdProbe::TakesConstVolatilePtr

	.method public hidebysig static 
		void TakesTrackingRef (
			int32& r
		) cil managed 
	{
		// Method begins at RVA 0x1150
		// Header size: 12
		// Code size: 4 (0x4)
		.maxstack 2

		IL_0000: ldarg.0
		IL_0001: ldc.i4.0
		IL_0002: stind.i4
		IL_0003: ret
	} // end of method IdProbe::TakesTrackingRef

	.method public hidebysig static 
		void TakesConstTrackingRef (
			int32 modopt([mscorlib]System.Runtime.CompilerServices.IsConst)& r
		) cil managed 
	{
		// Method begins at RVA 0x1160
		// Header size: 12
		// Code size: 1 (0x1)
		.maxstack 0

		IL_0000: ret
	} // end of method IdProbe::TakesConstTrackingRef

	.method public hidebysig static 
		void TakesNativeRef (
			int32* modopt([mscorlib]System.Runtime.CompilerServices.IsImplicitlyDereferenced) r
		) cil managed 
	{
		// Method begins at RVA 0x1170
		// Header size: 12
		// Code size: 4 (0x4)
		.maxstack 2

		IL_0000: ldarg.0
		IL_0001: ldc.i4.0
		IL_0002: stind.i4
		IL_0003: ret
	} // end of method IdProbe::TakesNativeRef

	.method public hidebysig static 
		void TakesArray2 (
			int32[0..., 0...] a
		) cil managed 
	{
		// Method begins at RVA 0x1180
		// Header size: 12
		// Code size: 1 (0x1)
		.maxstack 0

		IL_0000: ret
	} // end of method IdProbe::TakesArray2

	.method public hidebysig static 
		void TakesEnumerator (
			valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32> e
		) cil managed 
	{
		// Method begins at RVA 0x1190
		// Header size: 12
		// Code size: 1 (0x1)
		.maxstack 0

		IL_0000: ret
	} // end of method IdProbe::TakesEnumerator

	.method public hidebysig static 
		void Gen<T> (
			!!T t
		) cil managed 
	{
		// Method begins at RVA 0x11a0
		// Header size: 12
		// Code size: 3 (0x3)
		.maxstack 1
		.locals (
			[0] !!T
		)

		IL_0000: ldarg.0
		IL_0001: stloc.0
		IL_0002: ret
	} // end of method IdProbe::Gen

	.method public hidebysig specialname static 
		int32 op_Explicit (
			class IdProbe modreq([mscorlib]System.Runtime.CompilerServices.IsByValue) x
		) cil managed 
	{
		// Method begins at RVA 0x11b0
		// Header size: 12
		// Code size: 4 (0x4)
		.maxstack 1
		.locals (
			[0] int32
		)

		IL_0000: ldc.i4.0
		IL_0001: stloc.0
		IL_0002: ldloc.0
		IL_0003: ret
	} // end of method IdProbe::op_Explicit

	.method public hidebysig specialname 
		instance int32 get_Item (
			int32 modopt([mscorlib]System.Runtime.CompilerServices.IsLong) i
		) cil managed 
	{
		// Method begins at RVA 0x11c0
		// Header size: 12
		// Code size: 4 (0x4)
		.maxstack 1
		.locals (
			[0] int32
		)

		IL_0000: ldarg.1
		IL_0001: stloc.0
		IL_0002: ldloc.0
		IL_0003: ret
	} // end of method IdProbe::get_Item

	.method public hidebysig specialname 
		instance void set_Item (
			int32 modopt([mscorlib]System.Runtime.CompilerServices.IsLong) i,
			int32 'value'
		) cil managed 
	{
		// Method begins at RVA 0x11d0
		// Header size: 12
		// Code size: 1 (0x1)
		.maxstack 0

		IL_0000: ret
	} // end of method IdProbe::set_Item

	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x11e0
		// Header size: 12
		// Code size: 7 (0x7)
		.maxstack 1

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ret
	} // end of method IdProbe::.ctor

	// Properties
	.property instance int32 Item(
		int32 modopt([mscorlib]System.Runtime.CompilerServices.IsLong) i
	)
	{
		.get instance int32 IdProbe::get_Item(int32 modopt([mscorlib]System.Runtime.CompilerServices.IsLong))
		.set instance void IdProbe::set_Item(int32 modopt([mscorlib]System.Runtime.CompilerServices.IsLong), int32)
	}

} // end of class IdProbe
.class public auto ansi beforefieldinit GBox`1<T>
	extends [mscorlib]System.Object
{
	// Methods
	.method public hidebysig 
		instance void Hold (
			!T item
		) cil managed 
	{
		// Method begins at RVA 0x11f4
		// Header size: 12
		// Code size: 3 (0x3)
		.maxstack 1
		.locals (
			[0] !T
		)

		IL_0000: ldarg.1
		IL_0001: stloc.0
		IL_0002: ret
	} // end of method GBox`1::Hold

	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x1204
		// Header size: 12
		// Code size: 7 (0x7)
		.maxstack 1

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ret
	} // end of method GBox`1::.ctor

} // end of class GBox`1
.class public auto ansi beforefieldinit GBoxUser
	extends [mscorlib]System.Object
{
	// Methods
	.method public hidebysig static 
		void Use (
			class GBox`1<int32> b
		) cil managed 
	{
		// Method begins at RVA 0x1218
		// Header size: 12
		// Code size: 1 (0x1)
		.maxstack 0

		IL_0000: ret
	} // end of method GBoxUser::Use

	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x1228
		// Header size: 12
		// Code size: 7 (0x7)
		.maxstack 1

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ret
	} // end of method GBoxUser::.ctor

} // end of class GBoxUser
