// Reproducer for issue #4295:
// A positional record whose base record lives in an assembly (Library1) that
// cannot be resolved at decompile time. Its constructor chains to the base
// record's constructor, whose declaring type definition is then unknown.
// Disassembled from a csc build of:
//   public record BaseRecord(int A);                          // in Library1
//   public record Issue4295(int A, int B) : BaseRecord(A);

.assembly extern Library1
{
}
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 4:0:0:0
}
.assembly Issue4295
{
  .ver 1:0:0:0
}
.module Issue4295.dll

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295
	extends [Library1]Library1.BaseRecord
	implements class [System.Runtime]System.IEquatable`1<class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295>
{
	.custom instance void [System.Runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
		01 00 01 00 00
	)
	.custom instance void [System.Runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = (
		01 00 00 00 00
	)
	.interfaceimpl type class [System.Runtime]System.IEquatable`1<class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295>
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = (
			01 00 00 00 00
		)

	// Fields
	.field private initonly int32 '<B>k__BackingField'
	.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
		01 00 00 00
	)

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			int32 A,
			int32 B
		) cil managed 
	{
		// Method begins at RVA 0x2050
		// Header size: 1
		// Code size: 15 (0xf)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.2
		IL_0002: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::'<B>k__BackingField'
		IL_0007: ldarg.0
		IL_0008: ldarg.1
		IL_0009: call instance void [Library1]Library1.BaseRecord::.ctor(int32)
		IL_000e: ret
	} // end of method Issue4295::.ctor

	.method family hidebysig specialname virtual 
		instance class [System.Runtime]System.Type get_EqualityContract () cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2060
		// Header size: 1
		// Code size: 11 (0xb)
		.maxstack 8

		IL_0000: ldtoken ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295
		IL_0005: call class [System.Runtime]System.Type [System.Runtime]System.Type::GetTypeFromHandle(valuetype [System.Runtime]System.RuntimeTypeHandle)
		IL_000a: ret
	} // end of method Issue4295::get_EqualityContract

	.method public hidebysig specialname 
		instance int32 get_B () cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x206c
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::'<B>k__BackingField'
		IL_0006: ret
	} // end of method Issue4295::get_B

	.method public hidebysig specialname 
		instance void modreq([System.Runtime]System.Runtime.CompilerServices.IsExternalInit) set_B (
			int32 'value'
		) cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2074
		// Header size: 1
		// Code size: 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::'<B>k__BackingField'
		IL_0007: ret
	} // end of method Issue4295::set_B

	.method public hidebysig virtual 
		instance string ToString () cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2080
		// Header size: 12
		// Code size: 64 (0x40)
		.maxstack 2
		.locals init (
			[0] class [System.Runtime]System.Text.StringBuilder
		)

		IL_0000: newobj instance void [System.Runtime]System.Text.StringBuilder::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldstr "Issue4295"
		IL_000c: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
		IL_0011: pop
		IL_0012: ldloc.0
		IL_0013: ldstr " { "
		IL_0018: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
		IL_001d: pop
		IL_001e: ldarg.0
		IL_001f: ldloc.0
		IL_0020: callvirt instance bool [Library1]Library1.BaseRecord::PrintMembers(class [System.Runtime]System.Text.StringBuilder)
		IL_0025: brfalse.s IL_0030

		IL_0027: ldloc.0
		IL_0028: ldc.i4.s 32
		IL_002a: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(char)
		IL_002f: pop

		IL_0030: ldloc.0
		IL_0031: ldc.i4.s 125
		IL_0033: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(char)
		IL_0038: pop
		IL_0039: ldloc.0
		IL_003a: callvirt instance string [System.Runtime]System.Object::ToString()
		IL_003f: ret
	} // end of method Issue4295::ToString

	.method family hidebysig virtual 
		instance bool PrintMembers (
			class [System.Runtime]System.Text.StringBuilder builder
		) cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x20cc
		// Header size: 12
		// Code size: 62 (0x3e)
		.maxstack 2
		.locals init (
			[0] int32
		)

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance bool [Library1]Library1.BaseRecord::PrintMembers(class [System.Runtime]System.Text.StringBuilder)
		IL_0007: brfalse.s IL_0015

		IL_0009: ldarg.1
		IL_000a: ldstr ", "
		IL_000f: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
		IL_0014: pop

		IL_0015: ldarg.1
		IL_0016: ldstr "B = "
		IL_001b: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
		IL_0020: pop
		IL_0021: ldarg.1
		IL_0022: ldarg.0
		IL_0023: call instance int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::get_B()
		IL_0028: stloc.0
		IL_0029: ldloca.s 0
		IL_002b: constrained. [System.Runtime]System.Int32
		IL_0031: callvirt instance string [System.Runtime]System.Object::ToString()
		IL_0036: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
		IL_003b: pop
		IL_003c: ldc.i4.1
		IL_003d: ret
	} // end of method Issue4295::PrintMembers

	.method public hidebysig specialname static 
		bool op_Inequality (
			class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295 left,
			class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295 right
		) cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
			01 00 02 00 00
		)
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2116
		// Header size: 1
		// Code size: 11 (0xb)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call bool ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::op_Equality(class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295, class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295)
		IL_0007: ldc.i4.0
		IL_0008: ceq
		IL_000a: ret
	} // end of method Issue4295::op_Inequality

	.method public hidebysig specialname static 
		bool op_Equality (
			class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295 left,
			class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295 right
		) cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
			01 00 02 00 00
		)
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2122
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
		IL_0009: callvirt instance bool ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::Equals(class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295)
		IL_000e: ret

		IL_000f: ldc.i4.0
		IL_0010: ret

		IL_0011: ldc.i4.1
		IL_0012: ret
	} // end of method Issue4295::op_Equality

	.method public hidebysig virtual 
		instance int32 GetHashCode () cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2136
		// Header size: 1
		// Code size: 30 (0x1e)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance int32 [Library1]Library1.BaseRecord::GetHashCode()
		IL_0006: ldc.i4 -1521134295 // 0xa5555529
		IL_000b: mul
		IL_000c: call class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<int32>::get_Default()
		IL_0011: ldarg.0
		IL_0012: ldfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::'<B>k__BackingField'
		IL_0017: callvirt instance int32 class [System.Collections]System.Collections.Generic.EqualityComparer`1<int32>::GetHashCode(!0)
		IL_001c: add
		IL_001d: ret
	} // end of method Issue4295::GetHashCode

	.method public hidebysig virtual 
		instance bool Equals (
			object obj
		) cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
			01 00 02 00 00
		)
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2155
		// Header size: 1
		// Code size: 13 (0xd)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: isinst ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295
		IL_0007: callvirt instance bool ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::Equals(class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295)
		IL_000c: ret
	} // end of method Issue4295::Equals

	.method public final hidebysig virtual 
		instance bool Equals (
			class [Library1]Library1.BaseRecord other
		) cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
			01 00 02 00 00
		)
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2163
		// Header size: 1
		// Code size: 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: callvirt instance bool [System.Runtime]System.Object::Equals(object)
		IL_0007: ret
	} // end of method Issue4295::Equals

	.method public hidebysig newslot virtual 
		instance bool Equals (
			class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295 other
		) cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
			01 00 02 00 00
		)
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x216c
		// Header size: 1
		// Code size: 40 (0x28)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: beq.s IL_0026

		IL_0004: ldarg.0
		IL_0005: ldarg.1
		IL_0006: call instance bool [Library1]Library1.BaseRecord::Equals(class [Library1]Library1.BaseRecord)
		IL_000b: brfalse.s IL_0024

		IL_000d: call class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0> class [System.Collections]System.Collections.Generic.EqualityComparer`1<int32>::get_Default()
		IL_0012: ldarg.0
		IL_0013: ldfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::'<B>k__BackingField'
		IL_0018: ldarg.1
		IL_0019: ldfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::'<B>k__BackingField'
		IL_001e: callvirt instance bool class [System.Collections]System.Collections.Generic.EqualityComparer`1<int32>::Equals(!0, !0)
		IL_0023: ret

		IL_0024: ldc.i4.0
		IL_0025: ret

		IL_0026: ldc.i4.1
		IL_0027: ret
	} // end of method Issue4295::Equals

	.method public hidebysig newslot virtual 
		instance class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295 '<Clone>$' () cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.PreserveBaseOverridesAttribute::.ctor() = (
			01 00 00 00
		)
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		.override method instance class [Library1]Library1.BaseRecord [Library1]Library1.BaseRecord::'<Clone>$'()
		// Method begins at RVA 0x2195
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: newobj instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::.ctor(class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295)
		IL_0006: ret
	} // end of method Issue4295::'<Clone>$'

	.method family hidebysig specialname rtspecialname 
		instance void .ctor (
			class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295 original
		) cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x219d
		// Header size: 1
		// Code size: 20 (0x14)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance void [Library1]Library1.BaseRecord::.ctor(class [Library1]Library1.BaseRecord)
		IL_0007: ldarg.0
		IL_0008: ldarg.1
		IL_0009: ldfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::'<B>k__BackingField'
		IL_000e: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::'<B>k__BackingField'
		IL_0013: ret
	} // end of method Issue4295::.ctor

	.method public hidebysig 
		instance void Deconstruct (
			[out] int32& A,
			[out] int32& B
		) cil managed 
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x21b2
		// Header size: 1
		// Code size: 17 (0x11)
		.maxstack 8

		IL_0000: ldarg.1
		IL_0001: ldarg.0
		IL_0002: call instance int32 [Library1]Library1.BaseRecord::get_A()
		IL_0007: stind.i4
		IL_0008: ldarg.2
		IL_0009: ldarg.0
		IL_000a: call instance int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::get_B()
		IL_000f: stind.i4
		IL_0010: ret
	} // end of method Issue4295::Deconstruct

	// Properties
	.property instance class [System.Runtime]System.Type EqualityContract()
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		.get instance class [System.Runtime]System.Type ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::get_EqualityContract()
	}
	.property instance int32 B()
	{
		.get instance int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::get_B()
		.set instance void modreq([System.Runtime]System.Runtime.CompilerServices.IsExternalInit) ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295::set_B(int32)
	}

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue4295

