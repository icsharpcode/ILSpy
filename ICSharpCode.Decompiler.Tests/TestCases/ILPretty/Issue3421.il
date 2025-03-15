#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}

.class private auto ansi beforefieldinit Issue3421
    extends [CORE_ASSEMBLY]System.Object
{
    // Fields
    .field private string name
    .field private object 'value'
    .field private static class [CORE_ASSEMBLY]System.Collections.Generic.Dictionary`2<string, int32> '<>f__switch$map1D'
    .custom instance void [CORE_ASSEMBLY]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )

    // Methods
    .method public hidebysig virtual 
        instance void SetValue (
            object 'value'
        ) cil managed 
    {
        // Method begins at RVA 0x2050
        // Header size: 12
        // Code size: 180 (0xb4)
        .maxstack 27
        .locals init (
            [0] string,
            [1] class [CORE_ASSEMBLY]System.Collections.Generic.Dictionary`2<string, int32>,
            [2] int32
        )

        IL_0000: ldarg.0
        IL_0001: ldfld string Issue3421::name
        IL_0006: stloc.0
        IL_0007: ldloc.0
        IL_0008: brfalse IL_0093

        IL_000d: ldsfld class [CORE_ASSEMBLY]System.Collections.Generic.Dictionary`2<string, int32> Issue3421::'<>f__switch$map1D'
        IL_0012: brtrue IL_0048

        IL_0017: ldc.i4.3
        IL_0018: newobj instance void class [CORE_ASSEMBLY]System.Collections.Generic.Dictionary`2<string, int32>::.ctor(int32)
        IL_001d: stloc.1
        IL_001e: ldloc.1
        IL_001f: ldstr "##Name##"
        IL_0024: ldc.i4.0
        IL_0025: callvirt instance void class [CORE_ASSEMBLY]System.Collections.Generic.Dictionary`2<string, int32>::Add(!0, !1)
        IL_002a: ldloc.1
        IL_002b: ldstr "##Value##"
        IL_0030: ldc.i4.1
        IL_0031: callvirt instance void class [CORE_ASSEMBLY]System.Collections.Generic.Dictionary`2<string, int32>::Add(!0, !1)
        IL_0036: ldloc.1
        IL_0037: ldstr "##InnerText##"
        IL_003c: ldc.i4.2
        IL_003d: callvirt instance void class [CORE_ASSEMBLY]System.Collections.Generic.Dictionary`2<string, int32>::Add(!0, !1)
        IL_0042: ldloc.1
        IL_0043: stsfld class [CORE_ASSEMBLY]System.Collections.Generic.Dictionary`2<string, int32> Issue3421::'<>f__switch$map1D'

        IL_0048: ldsfld class [CORE_ASSEMBLY]System.Collections.Generic.Dictionary`2<string, int32> Issue3421::'<>f__switch$map1D'
        IL_004d: ldloc.0
        IL_004e: ldloca.s 2
        IL_0050: callvirt instance bool class [CORE_ASSEMBLY]System.Collections.Generic.Dictionary`2<string, int32>::TryGetValue(!0, !1&)
        IL_0055: brfalse IL_0098

        IL_005a: ldloc.2
        IL_005b: switch (IL_0071, IL_0076, IL_0082)

        IL_006c: br IL_0098

        IL_0071: br IL_00b3

        IL_0076: ldarg.0
        IL_0077: ldarg.1
        IL_0078: stfld object Issue3421::'value'
        IL_007d: br IL_00b3

        IL_0082: ldarg.0
        IL_0083: ldarg.1
        IL_0084: callvirt instance string [CORE_ASSEMBLY]System.Object::ToString()
        IL_0089: stfld object Issue3421::'value'
        IL_008e: br IL_00b3

        IL_0093: br IL_00b3

        IL_0098: ldarg.0
        IL_0099: ldfld object Issue3421::'value'
        IL_009e: brtrue IL_00ae

        IL_00a3: ldarg.0
        IL_00a4: ldstr ""
        IL_00a9: stfld object Issue3421::'value'

        IL_00ae: br IL_00b3

        IL_00b3: ret
    } // end of method Issue3421::SetValue

    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        // Method begins at RVA 0x2110
        // Header size: 1
        // Code size: 8 (0x8)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [CORE_ASSEMBLY]System.Object::.ctor()
        IL_0006: nop
        IL_0007: ret
    } // end of method Issue3421::.ctor

} // end of class Issue3421
