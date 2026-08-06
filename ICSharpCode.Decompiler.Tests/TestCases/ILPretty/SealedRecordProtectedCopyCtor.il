#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}

.assembly extern System.Collections
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}

// A record compiled as non-sealed, then sealed afterwards, which leaves the
// compiler-generated copy constructor 'protected' where Roslyn would emit
// 'private'. Obfuscators produce this combination.

.class public auto ansi sealed beforefieldinit TestRecord
	extends [CORE_ASSEMBLY]System.Object
	implements class [CORE_ASSEMBLY]System.IEquatable`1<class TestRecord>
{
	// Fields
	.field private initonly int32 '<A>k__BackingField'
	.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
		01 00 00 00
	)
	.field private initonly string '<B>k__BackingField'
	.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
		01 00 00 00
	)

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			int32 A,
			string B
		) cil managed 
	{
		// Method begins at RVA 0x2050
		// Header size: 1
		// Code size: 21 (0x15)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: stfld int32 TestRecord::'<A>k__BackingField'
		IL_0007: ldarg.0
		IL_0008: ldarg.2
		IL_0009: stfld string TestRecord::'<B>k__BackingField'
		IL_000e: ldarg.0
		IL_000f: call instance void [CORE_ASSEMBLY]System.Object::.ctor()
		IL_0014: ret
	} // end of method TestRecord::.ctor

	.method family hidebysig specialname newslot virtual 
		instance class [CORE_ASSEMBLY]System.Type get_EqualityContract () cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
			01 00 01 00 00
		)
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2066
		// Header size: 1
		// Code size: 11 (0xb)
		.maxstack 8

		IL_0000: ldtoken TestRecord
		IL_0005: call class [CORE_ASSEMBLY]System.Type [CORE_ASSEMBLY]System.Type::GetTypeFromHandle(valuetype [CORE_ASSEMBLY]System.RuntimeTypeHandle)
		IL_000a: ret
	} // end of method TestRecord::get_EqualityContract

	.method public hidebysig specialname 
		instance int32 get_A () cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2072
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld int32 TestRecord::'<A>k__BackingField'
		IL_0006: ret
	} // end of method TestRecord::get_A

	.method public hidebysig specialname 
		instance void modreq([CORE_ASSEMBLY]System.Runtime.CompilerServices.IsExternalInit) set_A (
			int32 'value'
		) cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x207a
		// Header size: 1
		// Code size: 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: stfld int32 TestRecord::'<A>k__BackingField'
		IL_0007: ret
	} // end of method TestRecord::set_A

	.method public hidebysig specialname 
		instance string get_B () cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2083
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld string TestRecord::'<B>k__BackingField'
		IL_0006: ret
	} // end of method TestRecord::get_B

	.method public hidebysig specialname 
		instance void modreq([CORE_ASSEMBLY]System.Runtime.CompilerServices.IsExternalInit) set_B (
			string 'value'
		) cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x208b
		// Header size: 1
		// Code size: 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: stfld string TestRecord::'<B>k__BackingField'
		IL_0007: ret
	} // end of method TestRecord::set_B

	.method public hidebysig virtual 
		instance string ToString () cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
			01 00 01 00 00
		)
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2094
		// Header size: 12
		// Code size: 64 (0x40)
		.maxstack 2
		.locals init (
			[0] class [CORE_ASSEMBLY]System.Text.StringBuilder
		)

		IL_0000: newobj instance void [CORE_ASSEMBLY]System.Text.StringBuilder::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldstr "TestRecord"
		IL_000c: callvirt instance class [CORE_ASSEMBLY]System.Text.StringBuilder [CORE_ASSEMBLY]System.Text.StringBuilder::Append(string)
		IL_0011: pop
		IL_0012: ldloc.0
		IL_0013: ldstr " { "
		IL_0018: callvirt instance class [CORE_ASSEMBLY]System.Text.StringBuilder [CORE_ASSEMBLY]System.Text.StringBuilder::Append(string)
		IL_001d: pop
		IL_001e: ldarg.0
		IL_001f: ldloc.0
		IL_0020: callvirt instance bool TestRecord::PrintMembers(class [CORE_ASSEMBLY]System.Text.StringBuilder)
		IL_0025: brfalse.s IL_0030

		IL_0027: ldloc.0
		IL_0028: ldc.i4.s 32
		IL_002a: callvirt instance class [CORE_ASSEMBLY]System.Text.StringBuilder [CORE_ASSEMBLY]System.Text.StringBuilder::Append(char)
		IL_002f: pop

		IL_0030: ldloc.0
		IL_0031: ldc.i4.s 125
		IL_0033: callvirt instance class [CORE_ASSEMBLY]System.Text.StringBuilder [CORE_ASSEMBLY]System.Text.StringBuilder::Append(char)
		IL_0038: pop
		IL_0039: ldloc.0
		IL_003a: callvirt instance string [CORE_ASSEMBLY]System.Object::ToString()
		IL_003f: ret
	} // end of method TestRecord::ToString

	.method family hidebysig newslot virtual 
		instance bool PrintMembers (
			class [CORE_ASSEMBLY]System.Text.StringBuilder builder
		) cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
			01 00 01 00 00
		)
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x20e0
		// Header size: 12
		// Code size: 71 (0x47)
		.maxstack 2
		.locals init (
			[0] int32
		)

		IL_0000: call void [CORE_ASSEMBLY]System.Runtime.CompilerServices.RuntimeHelpers::EnsureSufficientExecutionStack()
		IL_0005: ldarg.1
		IL_0006: ldstr "A = "
		IL_000b: callvirt instance class [CORE_ASSEMBLY]System.Text.StringBuilder [CORE_ASSEMBLY]System.Text.StringBuilder::Append(string)
		IL_0010: pop
		IL_0011: ldarg.1
		IL_0012: ldarg.0
		IL_0013: call instance int32 TestRecord::get_A()
		IL_0018: stloc.0
		IL_0019: ldloca.s 0
		IL_001b: constrained. [CORE_ASSEMBLY]System.Int32
		IL_0021: callvirt instance string [CORE_ASSEMBLY]System.Object::ToString()
		IL_0026: callvirt instance class [CORE_ASSEMBLY]System.Text.StringBuilder [CORE_ASSEMBLY]System.Text.StringBuilder::Append(string)
		IL_002b: pop
		IL_002c: ldarg.1
		IL_002d: ldstr ", B = "
		IL_0032: callvirt instance class [CORE_ASSEMBLY]System.Text.StringBuilder [CORE_ASSEMBLY]System.Text.StringBuilder::Append(string)
		IL_0037: pop
		IL_0038: ldarg.1
		IL_0039: ldarg.0
		IL_003a: call instance string TestRecord::get_B()
		IL_003f: callvirt instance class [CORE_ASSEMBLY]System.Text.StringBuilder [CORE_ASSEMBLY]System.Text.StringBuilder::Append(object)
		IL_0044: pop
		IL_0045: ldc.i4.1
		IL_0046: ret
	} // end of method TestRecord::PrintMembers

	.method public hidebysig specialname static 
		bool op_Inequality (
			class TestRecord left,
			class TestRecord right
		) cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
			01 00 02 00 00
		)
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2133
		// Header size: 1
		// Code size: 11 (0xb)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call bool TestRecord::op_Equality(class TestRecord, class TestRecord)
		IL_0007: ldc.i4.0
		IL_0008: ceq
		IL_000a: ret
	} // end of method TestRecord::op_Inequality

	.method public hidebysig specialname static 
		bool op_Equality (
			class TestRecord left,
			class TestRecord right
		) cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
			01 00 02 00 00
		)
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x213f
		// Header size: 1
		// Code size: 19 (0x13)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: beq.s IL_0011

		IL_0004: ldarg.0
		IL_0005: brfalse.s IL_000f

		IL_0007: ldarg.0
		IL_0008: ldarg.1
		IL_0009: callvirt instance bool TestRecord::Equals(class TestRecord)
		IL_000e: ret

		IL_000f: ldc.i4.0
		IL_0010: ret

		IL_0011: ldc.i4.1
		IL_0012: ret
	} // end of method TestRecord::op_Equality

	.method public hidebysig virtual 
		instance int32 GetHashCode () cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2153
		// Header size: 1
		// Code size: 63 (0x3f)
		.maxstack 8

		IL_0000: call class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<class [CORE_ASSEMBLY]System.Type>::get_Default()
		IL_0005: ldarg.0
		IL_0006: callvirt instance class [CORE_ASSEMBLY]System.Type TestRecord::get_EqualityContract()
		IL_000b: callvirt instance int32 class [System.Collections]System.Collections.Generic.EqualityComparer`1<class [CORE_ASSEMBLY]System.Type>::GetHashCode(!0)
		IL_0010: ldc.i4 -1521134295
		IL_0015: mul
		IL_0016: call class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<int32>::get_Default()
		IL_001b: ldarg.0
		IL_001c: ldfld int32 TestRecord::'<A>k__BackingField'
		IL_0021: callvirt instance int32 class [System.Collections]System.Collections.Generic.EqualityComparer`1<int32>::GetHashCode(!0)
		IL_0026: add
		IL_0027: ldc.i4 -1521134295
		IL_002c: mul
		IL_002d: call class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<string>::get_Default()
		IL_0032: ldarg.0
		IL_0033: ldfld string TestRecord::'<B>k__BackingField'
		IL_0038: callvirt instance int32 class [System.Collections]System.Collections.Generic.EqualityComparer`1<string>::GetHashCode(!0)
		IL_003d: add
		IL_003e: ret
	} // end of method TestRecord::GetHashCode

	.method public hidebysig virtual 
		instance bool Equals (
			object obj
		) cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
			01 00 02 00 00
		)
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2193
		// Header size: 1
		// Code size: 13 (0xd)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: isinst TestRecord
		IL_0007: callvirt instance bool TestRecord::Equals(class TestRecord)
		IL_000c: ret
	} // end of method TestRecord::Equals

	.method public hidebysig newslot virtual 
		instance bool Equals (
			class TestRecord other
		) cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
			01 00 02 00 00
		)
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x21a4
		// Header size: 12
		// Code size: 77 (0x4d)
		.maxstack 3

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: beq.s IL_004b

		IL_0004: ldarg.1
		IL_0005: brfalse.s IL_0049

		IL_0007: ldarg.0
		IL_0008: callvirt instance class [CORE_ASSEMBLY]System.Type TestRecord::get_EqualityContract()
		IL_000d: ldarg.1
		IL_000e: callvirt instance class [CORE_ASSEMBLY]System.Type TestRecord::get_EqualityContract()
		IL_0013: call bool [CORE_ASSEMBLY]System.Type::op_Equality(class [CORE_ASSEMBLY]System.Type, class [CORE_ASSEMBLY]System.Type)
		IL_0018: brfalse.s IL_0049

		IL_001a: call class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<int32>::get_Default()
		IL_001f: ldarg.0
		IL_0020: ldfld int32 TestRecord::'<A>k__BackingField'
		IL_0025: ldarg.1
		IL_0026: ldfld int32 TestRecord::'<A>k__BackingField'
		IL_002b: callvirt instance bool class [System.Collections]System.Collections.Generic.EqualityComparer`1<int32>::Equals(!0, !0)
		IL_0030: brfalse.s IL_0049

		IL_0032: call class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<string>::get_Default()
		IL_0037: ldarg.0
		IL_0038: ldfld string TestRecord::'<B>k__BackingField'
		IL_003d: ldarg.1
		IL_003e: ldfld string TestRecord::'<B>k__BackingField'
		IL_0043: callvirt instance bool class [System.Collections]System.Collections.Generic.EqualityComparer`1<string>::Equals(!0, !0)
		IL_0048: ret

		IL_0049: ldc.i4.0
		IL_004a: ret

		IL_004b: ldc.i4.1
		IL_004c: ret
	} // end of method TestRecord::Equals

	.method public hidebysig newslot virtual 
		instance class TestRecord '<Clone>$' () cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
			01 00 01 00 00
		)
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x21fd
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: newobj instance void TestRecord::.ctor(class TestRecord)
		IL_0006: ret
	} // end of method TestRecord::'<Clone>$'

	.method family hidebysig specialname rtspecialname 
		instance void .ctor (
			class TestRecord original
		) cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		.param [1]
			.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = (
				01 00 01 00 00
			)
		// Method begins at RVA 0x2205
		// Header size: 1
		// Code size: 31 (0x1f)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [CORE_ASSEMBLY]System.Object::.ctor()
		IL_0006: ldarg.0
		IL_0007: ldarg.1
		IL_0008: ldfld int32 TestRecord::'<A>k__BackingField'
		IL_000d: stfld int32 TestRecord::'<A>k__BackingField'
		IL_0012: ldarg.0
		IL_0013: ldarg.1
		IL_0014: ldfld string TestRecord::'<B>k__BackingField'
		IL_0019: stfld string TestRecord::'<B>k__BackingField'
		IL_001e: ret
	} // end of method TestRecord::.ctor

	.method public hidebysig 
		instance void Deconstruct (
			[out] int32& A,
			[out] string& B
		) cil managed 
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2225
		// Header size: 1
		// Code size: 17 (0x11)
		.maxstack 8

		IL_0000: ldarg.1
		IL_0001: ldarg.0
		IL_0002: call instance int32 TestRecord::get_A()
		IL_0007: stind.i4
		IL_0008: ldarg.2
		IL_0009: ldarg.0
		IL_000a: call instance string TestRecord::get_B()
		IL_000f: stind.ref
		IL_0010: ret
	} // end of method TestRecord::Deconstruct

	// Properties
	.property instance class [CORE_ASSEMBLY]System.Type EqualityContract()
	{
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = (
			01 00 01 00 00
		)
		.custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		.get instance class [CORE_ASSEMBLY]System.Type TestRecord::get_EqualityContract()
	}
	.property instance int32 A()
	{
		.get instance int32 TestRecord::get_A()
		.set instance void modreq([CORE_ASSEMBLY]System.Runtime.CompilerServices.IsExternalInit) TestRecord::set_A(int32)
	}
	.property instance string B()
	{
		.get instance string TestRecord::get_B()
		.set instance void modreq([CORE_ASSEMBLY]System.Runtime.CompilerServices.IsExternalInit) TestRecord::set_B(string)
	}

} // end of class TestRecord

